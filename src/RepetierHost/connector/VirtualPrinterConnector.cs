using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RepetierHost.model;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Data;
using System.ComponentModel;
using System.Threading;
using System.IO.Ports;
using System.IO;
using RepetierHost.view.utils;
using System.Timers;

namespace RepetierHost.connector
{
    public class VirtualPrinterConnector : PrinterConnectorBase, INotifyPropertyChanged,IDisposable
    {
        VirtualPrinterPanel panel = null;
        private RegistryKey key = null;
        public event PropertyChangedEventHandler PropertyChanged;
        private string baudRate = "250000";
        VirtualPrinter virtualPrinter;
        PrinterConnection con;
        int receiveCacheSize = 127;
        bool pingpong = false;
        int transferProtocol = 0;

        bool connected = false;
        public bool garbageCleared = false; // Skip old output
        public LinkedList<GCode> injectCommands = new LinkedList<GCode>();
        public LinkedList<GCode> history = new LinkedList<GCode>();
        public bool paused = false;
        public int lastline = 0;
        public int linesSend = 0, errorsReceived = 0;
        public int bytesSend = 0;
        public LinkedList<int> nackLines = new LinkedList<int>(); // Lines, whoses receivement were not acknowledged
        public SerialPort serial = null;
        public Printjob job;
        Thread writeThread = null;
        int binaryVersion = 0;
        private long lastCommandSend = DateTime.Now.Ticks;
        private Object nextlineLock = new Object();
        public float lastlogprogress = -1000;
        protected ManualResetEvent injectLock = new ManualResetEvent(true);
        bool readyForNextSend = true;

        public VirtualPrinterConnector()
        {
            virtualPrinter = new VirtualPrinter(this);
            con = Main.conn;
            job = new Printjob(Main.conn);
        }
        protected virtual void Dispose(bool disposing) 
        {
            if (disposing) 
            {
                if (panel != null)
                    panel.Dispose();
                if(injectLock !=null)
                    ((IDisposable)injectLock).Dispose();
                panel = null;
                injectLock = null;
            }
        }

        public void Dispose()
        {
                Dispose(true);
                GC.SuppressFinalize(this);
        }

        // Disposable types implement a finalizer.
        ~VirtualPrinterConnector()
        {
            Dispose(false);
        }
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        public override void Activate()
        {
        }

        public override void Deactivate()
        {
        }


        public override void RunPeriodicalTasks()
        {
            if (connected == false) return;
            long actTime = DateTime.Now.Ticks / 10000;
            if (con.autocheckTemp && actTime - con.lastAutocheck > con.autocheckInterval && job.exclusive == false)
            {
                con.lastAutocheck = actTime;
                // only inject temp check, if not present. Some commands
                // take a long time and it makes no sense, to push 30 M105
                // commands as soon as it's ready.
                bool found = HasInjectedMCommand(105);
                if (!found)
                {
                    GetInjectLock();
                    InjectManualCommand("M105");
                    ReturnInjectLock();
                }
            }
            if ((!pingpong && nackLines.Count == 0) || (pingpong && readyForNextSend)) TrySendNextLine();
        }

        public override bool Connect()
        {
            con.isMarlin = con.isRepetier = con.isSprinter = false;
            connected = true;
            virtualPrinter.open(int.Parse(baudRate));
            GCode gc = new GCode();
            gc.Parse("M105");
            virtualPrinter.receiveLine(gc);
            connected = true;
            if (transferProtocol < 2)
                binaryVersion = 0;
            else binaryVersion = transferProtocol - 1;
            con.binaryVersion = binaryVersion;
            readyForNextSend = true;
            lock (nackLines)
            {
                nackLines.Clear();
            }
            linesSend = errorsReceived = bytesSend = 0;
            gc.Parse("N0 M110");
            virtualPrinter.receiveLine(gc);
            gc.Parse("M115");
            virtualPrinter.receiveLine(gc);
            gc.Parse("M105");
            virtualPrinter.receiveLine(gc);
            con.FireConnectionChange(Trans.T("L_CONNECTED") + ":" + con.printerName);
            Main.main.Invoke(Main.main.UpdateJobButtons);
            return true;
        }
        public override bool Disconnect(bool force)
        {
            if (!connected) return true;
            // Test if we should warn about heaters still on.
            if (writeThread != null)
            {
                writeThread.Abort();
                writeThread = null;
            }
            connected = false;
            virtualPrinter.close();
            job.KillJob();
            history.Clear();
            injectCommands.Clear();
            try
            {
                con.FireConnectionChange(Trans.T("L_DISCONNECTED"));
            }
            catch { } // Closing the app can cause an exception, if event comes after Main handle is destroyed
            con.firePrinterAction(Trans.T("L_IDLE"));
            Main.main.Invoke(Main.main.UpdateJobButtons);
            return true;

        }
        public override bool IsConnected()
        {
            return connected;
        }
        public override void InjectManualCommand(string command)
        {
            if (!connected) return;
            GCode gc = new GCode();
            gc.Parse(command);
            if (gc.comment) return;
            lock (history)
                injectCommands.AddLast(gc);
            if (job.dataComplete == false)
            {
                if (injectCommands.Count == 0)
                {
                    con.firePrinterAction(Trans.T("L_IDLE"));
                }
                else
                {
                    con.firePrinterAction(Trans.T1("L_X_COMMANDS_WAITING", injectCommands.Count.ToString()));
                }
            }

        }
        public override void InjectManualCommandFirst(string command)
        {
            GCode gc = new GCode();
            gc.Parse(command);
            if (gc.comment) return;
            lock (history)
                injectCommands.AddFirst(gc);
            if (job.dataComplete == false)
            {
                if (injectCommands.Count == 0)
                {
                    con.firePrinterAction(Trans.T("L_IDLE"));
                }
                else
                {
                    con.firePrinterAction(Trans.T1("L_X_COMMANDS_WAITING", injectCommands.Count.ToString()));
                }
            }
        }
        public override bool HasInjectedMCommand(int code)
        {
            bool has = false;
            lock (history)
            {
                foreach (GCode co in injectCommands)
                {
                    if (co.hasM && co.M == code)
                    {
                        has = true;
                        break;
                    }
                }
            }
            return has;
        }

        public override UserControl ConnectionDialog()
        {
            if (panel == null)
            {
                panel = new VirtualPrinterPanel();
                panel.Connect(this);
            }
            return panel;

        }
        public override string Name
        {
            get
            {
                return Trans.T("L_VIRTUAL_CONNECTION");
            }
        }
        public override string Id
        {
            get
            {
                return "VirtualConnector";
            }
        }

        public override void SetConfiguration(RegistryKey key)
        {
            this.key = key;
        }
        public override void SaveToRegistry()
        {
            key.SetValue("baud", baudRate);
        }
        public override void LoadFromRegistry()
        {
            BaudRate = (string)key.GetValue("baud", baudRate);
        }
        public override void Emergency()
        {
        }
        public override void ToggleETAMode()
        {
            job.etaModeNormal = !job.etaModeNormal;
        }
        public override string ETA { get { return job.ETA; } }

        public override void RunJob()
        {
            job.BeginJob();
            job.PushGCodeShortArray(Main.main.editor.getContentArray(1));
            job.PushGCodeShortArray(Main.main.editor.getContentArray(0));
            job.PushGCodeShortArray(Main.main.editor.getContentArray(2));
            job.EndJob();
            Main.main.Invoke(Main.main.UpdateJobButtons);
        }
        float pauseX, pauseY, pauseZ, pauseE, pauseF;
        bool pauseRelative;
        public override void PauseJob(string text)
        {
            if (paused) return;
            paused = true;
            GCodeAnalyzer a = con.analyzer;
            pauseX = a.x - a.xOffset;
            pauseY = a.y - a.yOffset;
            pauseZ = a.z - a.zOffset;
            pauseE = a.activeExtruder.e - a.activeExtruder.eOffset;
            pauseF = a.f;
            pauseRelative = a.relative;

            PauseInfo.ShowPause(text);
            foreach (GCodeShort code in Main.main.editor.getContentArray(4))
            {
                InjectManualCommand(code.text);
            }
            Main.main.Invoke(Main.main.UpdateJobButtons);
            if (eventPauseChanged != null)
            {
                eventPauseChanged(true);
            }
        }
        public override void ContinueJob()
        {
            GCodeAnalyzer a = con.analyzer;
            InjectManualCommand("G90");
            InjectManualCommand("G1 X" + pauseX.ToString(GCode.format) + " Y" + pauseY.ToString(GCode.format) + " F" + con.travelFeedRate.ToString(GCode.format));
            InjectManualCommand("G1 Z" + pauseZ.ToString(GCode.format) + " F" + con.maxZFeedRate.ToString(GCode.format));
            InjectManualCommand("G92 E" + pauseE.ToString(GCode.format));
            if (a.relative != pauseRelative)
            {
                InjectManualCommand(pauseRelative ? "G91" : "G90");
            }
            InjectManualCommand("G1 F" + pauseF.ToString(GCode.format)); // Reset old speed
            paused = false;
            Main.main.Invoke(Main.main.UpdateJobButtons);
            if (eventPauseChanged != null)
            {
                eventPauseChanged(false);
            }
        }
        public override Printjob Job { get { return job; } }

        public override void KillJob()
        {
            job.KillJob();
            Main.main.Invoke(Main.main.UpdateJobButtons);
        }
        public override bool IsJobRunning()
        {
            return job.dataComplete;
        }

        public override string ToString()
        {
            return Name;
        }
        public override bool IsPaused
        {
            get
            {
                return paused;
            }
        }
        public override int MaxLayer
        {
            get
            {
                return job.maxLayer;
            }
        }
        public override int InjectedCommands { get { return injectCommands.Count; } }

        public string BaudRate
        {
            get { return baudRate; }
            set
            {
                baudRate = value;
                OnPropertyChanged(new PropertyChangedEventArgs("BaudRate"));
            }
        }
        public override void ResendLine(int line)
        {
        }
        public override void TrySendNextLine()
        {
            TrySendNextLine2();
        }
        public bool TrySendNextLine2()
        {
            string logtext = null;
            int loglevel = 0;
            float logprogress = -1;
            string printeraction = null;
            GCode historygc = null;
            GCode hostCommand = null;
            bool lineInc = false;
            if (!garbageCleared) return false;
            try
            {
                lock (nextlineLock)
                {
                    if (pingpong && !readyForNextSend) return false;
                    if (!connected) return false; // Not ready yet
                    GCode gc = null;
                    try
                    {
                        // then check for manual commands
                        if (injectCommands.Count > 0 && !job.exclusive)
                        {
                            lock (history)
                            {
                                gc = injectCommands.First.Value;
                                if (gc.hostCommand)
                                {
                                    hostCommand = gc;
                                }
                                else
                                {
                                    if (gc.M == 110)
                                        lastline = gc.N;
                                    else //if (gc.M != 117)
                                    {
                                        gc.N = ++lastline;
                                        lineInc = true;
                                    }
                                    if (!pingpong && receivedCount() + gc.orig.Length > receiveCacheSize) { if (lineInc) --lastline; return false; } // printer cache full
                                    if (pingpong) readyForNextSend = false;
                                    else { lock (nackLines) { nackLines.AddLast(gc.orig.Length); } }
                                    virtualPrinter.receiveLine(gc);
                                    bytesSend += gc.orig.Length;
                                }
                                injectCommands.RemoveFirst();
                            }
                            if (!gc.hostCommand)
                            {
                                linesSend++;
                                lastCommandSend = DateTime.Now.Ticks;
                                historygc = gc;
                            }
                            con.analyzer.Analyze(gc);
                            if (job.dataComplete == false)
                            {
                                if (injectCommands.Count == 0)
                                {
                                    printeraction = Trans.T("L_IDLE");
                                }
                                else
                                {
                                    printeraction = Trans.T1("L_X_COMMANDS_WAITING", injectCommands.Count.ToString());
                                }
                            }
                            return true;
                        }
                        // do we have a printing job?
                        if (job.dataComplete && !paused)
                        {
                            lock (history)
                            {
                                gc = job.PeekData();
                                if (gc.hostCommand)
                                {
                                    hostCommand = gc;
                                }
                                else
                                {
                                    if (gc.M == 110)
                                        lastline = gc.N;
                                    else //if (gc.M != 117)
                                    {
                                        gc.N = ++lastline;
                                        lineInc = true;
                                    }
                                    string cmd = gc.getAscii(true, true);
                                    if (!pingpong && receivedCount() + cmd.Length /*gc.orig.Length*/ > receiveCacheSize) { if (lineInc) --lastline; return false; } // printer cache full
                                    if (pingpong) readyForNextSend = false;
                                    else { lock (nackLines) { nackLines.AddLast(cmd.Length /*gc.orig.Length*/); } }
                                    virtualPrinter.receiveLine(gc);
                                    bytesSend += cmd.Length; // gc.orig.Length;
                                }
                                job.PopData();
                            }
                            if (!gc.hostCommand)
                            {
                                linesSend++;
                                lastCommandSend = DateTime.Now.Ticks;
                                printeraction = Trans.T1("L_PRINTING..ETA", job.ETA); //"Printing...ETA " + job.ETA;
                                if (job.maxLayer > 0)
                                    printeraction += " " + Trans.T2("L_LAYER_X/Y", con.analyzer.layer.ToString(), job.maxLayer.ToString()); // Layer " + analyzer.layer + "/" + job.maxLayer;
                                logprogress = job.PercentDone;
                            }
                            con.analyzer.Analyze(gc);
                            return true;
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        logtext = Trans.T("L_ERROR_SENDING_DATA:") + ex; // "Error sending data:" + ex;
                        loglevel = 2;
                    }
                }
            }
            finally
            {
                // need to extract log/event calls because they cause deadlocks inside
                // the lock statement.
                if (hostCommand != null)
                    Main.main.Invoke(Main.main.executeHostCall, hostCommand);
                if (historygc != null)
                {
                    con.log(historygc.getAscii(true, true), false, 0);
                }
                if (logtext != null)
                    con.log(logtext, false, loglevel);
                if (printeraction != null)
                    con.firePrinterAction(printeraction);
                if (logprogress >= 0 && Math.Abs(lastlogprogress - logprogress) > 0.3)
                {
                    lastlogprogress = logprogress;
                    con.FireJobProgressAsync(job.PercentDone);
                }
            }
            return false;
        }
        private int receivedCount()
        {
            int n = 0;
            lock (nackLines)
            {
                foreach (int i in nackLines)
                    n += i;
            }
            return n;
        }
        private Object injectLockLock = new Object();
        public override void GetInjectLock()
        {
            try
            {
                injectLock.WaitOne();
                injectLock.Reset();
            }
            catch (Exception e)
            {
                con.firePrinterAction(e.ToString());
            }
            /*lock(injectLockLock) {
                while(!injectLock) {}
                injectLock = false;
            }*/
        }
        public override void ReturnInjectLock()
        {
            injectLock.Set();
        }
        public override void AnalyzeResponse(string res)
        {
            string h;
            int level = 0;
            while (res.Length > 0 && res[0] < 32) res = res.Substring(1);
            res = res.Trim();
            if (res.Equals("start") || (garbageCleared == false && res.IndexOf("start") != -1))
            {
                lastline = 0;
                job.KillJob(); // continuing the old job makes no sense, better save the plastic
                con.sdcardMounted = true;
                history.Clear();
                con.analyzer.start(true);
                readyForNextSend = true;
                lock (nackLines)
                {
                    nackLines.Clear();
                }
                garbageCleared = true;
            }
            con.analyzeResponse(res, ref level);
            h = con.extract(res, "REPETIER_PROTOCOL:");
            if (h != null)
            {
                level = 3;
                int.TryParse(h, out binaryVersion);
                if (transferProtocol == 1) binaryVersion = 0; // force ascii transfer
                con.binaryVersion = binaryVersion;
            }
            h = con.extract(res, "Resend:");
            if (h != null)
            {
                level = 1;
                con.log(res, true, level);
                int line;
                int.TryParse(h, out line);
                ResendLine(line);
            }
            else if (res.StartsWith("ok"))
            {
                garbageCleared = true;
                if (Main.main.logView.switchACK.On)
                    con.log(res, true, level);
                if (pingpong) readyForNextSend = true;
                else
                {
                    lock (nackLines)
                    {
                        if (nackLines.Count > 0)
                            nackLines.RemoveFirst();
                    }
                }
                TrySendNextLine();
            }
            else if (res.Equals("wait")) //  && DateTime.Now.Ticks - lastCommandSend > 5000)
            {
                if (Main.main.logView.switchACK.On)
                    con.log(res, true, level);
                if (pingpong) readyForNextSend = true;
                else
                {
                    lock (nackLines)
                    {
                        if (nackLines.Count > 0)
                            nackLines.Clear();
                    }
                }
                TrySendNextLine();
            }
            else if (level >= 0 && garbageCleared) con.log(res, true, level);
        }
    }
}
