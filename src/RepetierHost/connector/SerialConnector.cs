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
using RepetierHost.view;

namespace RepetierHost.connector
{
    
    public class SerialConnector : PrinterConnectorBase, INotifyPropertyChanged, IDisposable
    {
        private class NackData
        {
            public int length;
            public long expire;
            public NackData(int l)
            {
                length = l;
                expire = 0;
            }
            public NackData(int l,long milli)
            {
                length = l;
                expire = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + milli+1000;
            }
            public void SetExpire(long milli)
            {
                expire = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond+milli+1000;
            }
        }
        private SerialConnectionPanel panel = null;
        private bool connected = false;
        private RegistryKey key = null;
        public event PropertyChangedEventHandler PropertyChanged;

        private string baudRate = "250000";
        private string port = "COM1";
        private bool pingPong = false;
        private int receiveCacheSize = 127;
        private int transferProtocol = 0;
        private int resetOnConnect = 2;
        private int resetOnEmergency = 2;
        private int doRunStartCommands = -1;
        PrinterConnection con;
        //System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
        Encoding enc = System.Text.Encoding.GetEncoding(1252); 

        // ----------- Connection handline variables -------------


        public bool garbageCleared = false; // Skip old output
        bool readyForNextSend = true;
        public LinkedList<GCode> injectCommands = new LinkedList<GCode>();
        public LinkedList<GCode> history = new LinkedList<GCode>();
        LinkedListNode<GCode> resendNode = null;
        public bool paused = false;
        public int lastline = 0;
        private int resendError = 0;
        public int linesSend = 0, errorsReceived = 0;
        public int bytesSend = 0;
        public int openResend = -1;
        private LinkedList<NackData> nackLinesBuffered = new LinkedList<NackData>();
        private LinkedList<NackData> nackLines = new LinkedList<NackData>(); // Lines, whoses receivement were not acknowledged
        Thread readThread = null;
        public Parity parity = Parity.None;
        public StopBits stopbits = StopBits.One;
        public int databits = 8;
        public SerialPort serial = null;
        public Printjob job;
        Thread writeThread = null;
        int binaryVersion = 0;
        private long lastCommandSend = DateTime.Now.Ticks;
        static AutoResetEvent writeEvent;
        private Object nextlineLock = new Object();
        public float lastlogprogress = -1000;
        string read = "";
        long lastReceived = 0;
        bool ignoreNextOk = false;
        bool initalizationFinished = false;
        private ManualResetEvent injectLock = new ManualResetEvent(true);
       // int lastResendLine = -1;
       // int ignoreXEqualResendsResend = 0;
        bool prequelFinished = false;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        public SerialConnector()
        {
            con = Main.conn;
            job = new Printjob(Main.conn);
            writeEvent = new AutoResetEvent(false);
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                port = "/dev/ttyUSB0";

        }
        protected virtual void Dispose(bool disposing) 
        {
            if (disposing) 
            {
                if(panel!=null)
                    panel.Dispose();
                if (serial != null)
                    serial.Dispose();
                if(injectLock!=null)
                   ((IDisposable)injectLock).Dispose();
            }
        }

        public void Dispose()
        {
                Dispose(true);
                GC.SuppressFinalize(this);
        }

        // Disposable types implement a finalizer.
        ~SerialConnector()
        {
            Dispose(false);
        }
        public override void Activate()
        {
            if (Main.main.printerInformationsToolStripMenuItem == null) return;
            if (Main.main.printerInfo == null)
                Main.main.printerInfo = new PrinterInfo();
            Main.main.printerInfo.ConnectWith(this);
            Main.main.printerInformationsToolStripMenuItem.Enabled = true;
        }
        public override void Deactivate()
        {
            if (Main.main.printerInformationsToolStripMenuItem != null)
                Main.main.printerInformationsToolStripMenuItem.Enabled = false;
        }

        public override void RunPeriodicalTasks()
        {
            if (doRunStartCommands > 0)
            {
                if (--doRunStartCommands == 0)
                    RunStartCommands();
            }
            if (((serial == null || connected == false)) || garbageCleared == false || !initalizationFinished) return;
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
            if ((!pingPong && nackLines.Count == 0) || (pingPong && readyForNextSend)) TrySendNextLine();

            // If the reprap starts sending response it should finish soon
            else if (resendError < 4 && read.Length > 0 && lastReceived - actTime > 400)
            {
                // force response, even if we
                // get a resend request
                con.log(Trans.T1("L_RESET_OUTPUT", read), false, 2); // "Reset output. After some wait, I got only " + read
                read = "";
                if (pingPong)
                    readyForNextSend = true;
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

        }

        public override bool Connect()
        {
            resendError = 0;
            try
            {
                if (Main.IsMono)
                    serial = new SerialPort();
                else
                    serial = new ProtectedSerialPort();
                garbageCleared = false;
                serial.PortName = port;
                serial.BaudRate = int.Parse(baudRate);
                serial.Parity = parity;
                serial.DataBits = databits;
                serial.StopBits = stopbits;
                ignoreResendLine = -1;
                doRunStartCommands = -1;
                lastline = 0;
                openResend = -1;
                initalizationFinished = false;
                if (!Main.IsMono)
                    serial.DataReceived += received;
                serial.ErrorReceived += error;
                //serial.RtsEnable = false;
                if (resetOnConnect == 1)
                {
                    serial.DtrEnable = true;
                    serial.RtsEnable = true;
                }
                else if (resetOnConnect == 2)
                {
                    serial.DtrEnable = false;
                    serial.RtsEnable = false;
                }
                serial.Open();
                if (writeThread == null)
                {
                    writeThread = new Thread(new ThreadStart(this.WriteLoop));
                    writeThread.Start();
                }
                connected = true;
                if (resetOnConnect == 2)
                {
                    //Thread.Sleep(200);
                    serial.DtrEnable = true;
                    serial.RtsEnable = true;
                }
                if (resetOnConnect == 1)
                {
                    Thread.Sleep(1000);
                    serial.DtrEnable = false;
                    serial.RtsEnable = false;
                }
                if (resetOnConnect == 2)
                {
                    Thread.Sleep(1000);
                    serial.DtrEnable = false;
                    serial.RtsEnable = false;
                }
                if (resetOnConnect == 3)
                {
                    serial.DtrEnable = !serial.DtrEnable;
                    serial.RtsEnable = !serial.RtsEnable;
                    Thread.Sleep(1000);
                }
                // If we didn't restart the connection we need to eat
                // all unread data on this port.
                //serial.DiscardInBuffer();
                /*while(serial.BytesToRead > 0)
                {
                    string indata = serial.ReadExisting();
                }*/
                Application.DoEvents();
                Thread.Sleep(1000);
                prequelFinished = false;
                //serial.WriteLine("");
                //serial.WriteLine("M105 *89");
                if (transferProtocol < 2)
                    binaryVersion = 0;
                else binaryVersion = transferProtocol - 1;
                con.binaryVersion = binaryVersion;
                readyForNextSend = true;
                nackLines.Clear();
                ignoreNextOk = false;
                linesSend = errorsReceived = bytesSend = 0;
                if (readThread == null && Main.IsMono)
                {
                    readThread = new Thread(new ThreadStart(this.ReadThread));
                    readThread.Start();
                }
                if (resetOnConnect == 0)
                {
                    // Create safe start if we connect without reset. If we reset anything is lost anyway.
                    if (transferProtocol == 2 || transferProtocol == 0)
                    {
                        // System.Threading.Thread.Sleep(500); // Wait for buffer to empty
                        byte[] buf = new byte[100];
                        for (int i = 0; i < 100; i++) buf[i] = 0;
                        serial.Write(buf, 0, 100);
                        serial.WriteLine("");
                        System.Threading.Thread.Sleep(10 + 1000000 / int.Parse(baudRate)); // Wait for buffer to empty
                    }
                    GetInjectLock();
                    InjectManualCommand("N1 M110"); // Make sure we tal about the same linenumbers
                    InjectManualCommand("N1 M110"); // Make sure we tal about the same linenumbers
                    InjectManualCommand("M115"); // Check firmware
                    InjectManualCommand("T" + Main.main.printPanel.comboExtruder.SelectedIndex);
                    InjectManualCommand("M105"); // Read temperature
                    ReturnInjectLock();
                }
                if (resetOnConnect == 0)
                {
                    garbageCleared = true;
                    con.FireConnectionChange(Trans.T("L_CONNECTED") + ":" + con.printerName);
                    Main.main.Invoke(Main.main.UpdateJobButtons);
                    initalizationFinished = true;
                    con.analyzer.fireChanged();
                    if (con.analyzer.powerOn)
                        InjectManualCommand("M80");
                }
                else doRunStartCommands = 25; // in case reset did not work, do the same as without reset
            }
            catch (IOException ex)
            {
                if (writeThread != null)
                {
                    writeThread.Abort();
                    writeThread = null;
                }

                if (serial != null)
                {
                    if (!Main.IsMono)
                        serial.DataReceived -= received;
                    serial.ErrorReceived -= error;
                }
                serial = null;
                con.log(ex.Message, true, 2);
                con.FireConnectionChange(Trans.T("L_CONNECTION_ERROR") + ":" + con.printerName);
                RepetierHost.view.SoundConfig.PlayError(false);
                if (MessageBox.Show(Trans.T1("L_CONNECTION_FAILED", ex.Message), Trans.T("L_CONNECTION_ERROR"), MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    Main.printerSettings.Show(Main.main);
                    Main.main.FormToFront(Main.printerSettings);

                }
            }
            catch (System.UnauthorizedAccessException ex)
            {
                if (writeThread != null)
                {
                    writeThread.Abort();
                    writeThread = null;
                }

                if (serial != null)
                {
                    if (!Main.IsMono)
                        serial.DataReceived -= received;
                    serial.ErrorReceived -= error;
                }
                serial = null;
                con.log(ex.Message, true, 2);
                con.FireConnectionChange(Trans.T("L_CONNECTION_ERROR") + ":" + con.printerName);
                RepetierHost.view.SoundConfig.PlayError(false);
                if (MessageBox.Show(Trans.T1("L_CONNECTION_FAILED", ex.Message), Trans.T("L_CONNECTION_ERROR"), MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    Main.printerSettings.Show(Main.main);
                    Main.main.FormToFront(Main.printerSettings);

                }
            }

            return true;
        }

        public override bool Disconnect(bool force)
        {
            if (serial == null) return true;
            // Test if we should warn about heaters still on.
            bool heateron = false;
            if (con.analyzer.bedTemp > 0 && con.bedTemp > 0) heateron = true;
            foreach (int extr in con.extruderTemp.Keys)
            {
                if (con.analyzer.getTemperature(extr) >= 20) heateron = true;
            }
            if (heateron && !force)
            {
                DialogResult heatres = MessageBox.Show(Trans.T("L_HEATERS_ON_QUEST"), Trans.T("L_WARNING"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (heatres == DialogResult.Cancel) return false;
                if (heatres == DialogResult.Yes)
                {
                    for (int i = 0; i < Main.conn.numberExtruder; i++)
                        InjectManualCommand("M104 S0 T" + i.ToString());
                    if (con.bedTemp > 0)
                        InjectManualCommand("M140 S0");
                    return false;
                }
            }
            connected = false;
            if (writeThread != null)
            {
                writeThread.Abort();
                writeThread.Join();
                writeThread = null;
            }
            if (readThread != null)
            {
                readThread.Abort();
                readThread.Join();
                readThread = null;
            }
            if (!Main.IsMono)
                serial.DataReceived -= received;
            serial.ErrorReceived -= error;


            if (job.mode == 1)
                job.KillJob();
            Application.DoEvents();
            Thread.Sleep(100);
            Application.DoEvents();
            //  lock (nextlineLock)
            // {
            try
            {
                if (serial != null)
                {
                    serial.Close();
                    serial.Dispose();
                }
            }
            catch (Exception) { }
            serial = null;
            // }
            job.KillJob();
            history.Clear();
            injectCommands.Clear();
            resendNode = null;
            comErrorsReceived = 0;
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
                panel = new SerialConnectionPanel();
                panel.Connect(this);
            }
            panel.UpdatePorts();
            return panel;
        }
        public override string Name
        {
            get
            {
                return Trans.T("L_SERIAL_CONNECTION");
            }
        }
        public override string Id
        {
            get
            {
                return "SerialConnector";
            }
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
        public override void SetConfiguration(RegistryKey key)
        {
            this.key = key;
        }
        public override void SaveToRegistry()
        {
            key.SetValue("port", port);
            key.SetValue("baud", baudRate);
            key.SetValue("transferProtocol", transferProtocol);
            key.SetValue("resetOnConnect", resetOnConnect);
            key.SetValue("resetOnEmergency", resetOnEmergency);
            key.SetValue("receiveCacheSize", receiveCacheSize.ToString());
            key.SetValue("pingPong", pingPong ? 1 : 0);
        }
        public override void LoadFromRegistry()
        {
            Port = (string)key.GetValue("port", port);
            BaudRate = (string)key.GetValue("baud", baudRate);
            Protocol = (int)key.GetValue("transferProtocol", transferProtocol);
            ResetOnConnect = (int)key.GetValue("resetOnConnect", resetOnConnect);
            resetOnEmergency = (int)key.GetValue("resetOnEmergency", resetOnEmergency);
            ReceiveCacheSizeString = (string)key.GetValue("receiveCacheSize", receiveCacheSize.ToString());
            PingPong = ((int)key.GetValue("pingPong", pingPong ? 1 : 0) > 0 ? true : false);
        }

        public override void Emergency()
        {
            InjectManualCommandFirst("M112");
            job.KillJob();
            if (resetOnEmergency == 2)
            {
                con.close(true);
                con.open();
            }
            else if (resetOnEmergency == 1)
            {                
                //serial.DtrEnable = !serial.DtrEnable;
                serial.DtrEnable = true;
                Thread.Sleep(200);
                serial.DtrEnable = false;
            }
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
        public string Port
        {
            get { return port; }
            set
            {
                if (port == value) return;
                port = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Port"));
            }
        }
        public string BaudRate
        {
            get { return baudRate; }
            set
            {
                if (baudRate == value) return;
                baudRate = value;
                OnPropertyChanged(new PropertyChangedEventArgs("BaudRate"));
            }
        }
        public int Protocol
        {
            get { return transferProtocol; }
            set
            {
                if (transferProtocol == value) return;
                transferProtocol = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Protocol"));
            }
        }
        public string ReceiveCacheSizeString
        {
            get { return receiveCacheSize.ToString(); }
            set
            {
                int size;
                int.TryParse(value, out size);
                if (size == receiveCacheSize) return;
                receiveCacheSize = size;
                OnPropertyChanged(new PropertyChangedEventArgs("ReceivingCacheSizeString"));
            }
        }
        public int ResetOnConnect
        {
            get { return resetOnConnect; }
            set
            {
                if (resetOnConnect == value) return;
                resetOnConnect = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ResetOnConnect"));
            }
        }
        public int ResetOnEmergency
        {
            get { return resetOnEmergency; }
            set
            {
                if (resetOnEmergency == value) return;
                resetOnEmergency = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ResetOnEmergency"));
            }
        }
        public bool PingPong
        {
            get { return pingPong; }
            set
            {
                if (pingPong == value) return;
                pingPong = value;
                OnPropertyChanged(new PropertyChangedEventArgs("PingPong"));
            }
        }

        // ============== Connection handling code =====================

        private void StoreHistory(GCode gcode)
        {
            lock (history)
            {
                history.AddLast(gcode);
                con.log(gcode.getAscii(true, true), false, 0);
                if (history.Count > 40)
                    history.RemoveFirst();
            }
        }
        private int receivedCount()
        {
            int n = 0;
            lock (nackLines)
            {
                foreach (NackData i in nackLines)
                    n += i.length;
            }
            return n;
        }
        private void TestFakeOk()
        {
            return;
            long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            lock (nackLines)
            {
                if(nackLines.Count==0) return; // empty, nothing to do anyway
                foreach (NackData nd in nackLinesBuffered)
                {
                    if (nd.expire != 0 && nd.expire > now)
                    {
                        return; // might really be busy, do nothing
                    }
                }
                long ex = nackLines.First.Value.expire;
                if (ex != 0 && ex < now)
                {
                    nackLines.RemoveFirst();
                    if (pingPong)
                        readyForNextSend = true;
                }
            }
        }
        private int ignoreResendLine = -1;
        private int ignoreResendLineForXCalls = 0;
        public override void ResendLine(int line)
        {
            if (line == openResend) return; // line was not send yet, do not do it twice
            if (binaryVersion != 0)
            {
                int send = receivedCount();
                //serial.DiscardOutBuffer();
                System.Threading.Thread.Sleep(send * 10000 / int.Parse(baudRate)); // Wait for buffer to empty
                byte[] buf = new byte[32];
                for (int i = 0; i < 32; i++) buf[i] = 0;
                serial.Write(buf, 0, 32);
                System.Threading.Thread.Sleep(320000 / int.Parse(baudRate)); // Wait for buffer to empty
            }
            else
            {
                //serial.DiscardOutBuffer();
                serial.WriteLine("");
                System.Threading.Thread.Sleep(receiveCacheSize * 10000 / int.Parse(baudRate)); // Wait for buffer to empty
            }
            if (line > lastline) return; // resend request during connect can request unpossible resends
            if (line == ignoreResendLine && con.isRepetier == true) // Ignore repeated resend requests
            {
                if (ignoreResendLineForXCalls > 0) ignoreResendLineForXCalls--;
                if (ignoreResendLineForXCalls > 0) return;
            }
            ignoreResendLine = line;
            ignoreResendLineForXCalls = 7;
          /*  if (!prequelFinished && line > lastline)
            {
                RLog.message("ignoring resend in prequel");
                if (pingPong)
                    readyForNextSend = true;
                return;
            }*/
            if (!connected) return;
            errorsReceived++;
            resendError++;
            lock (nextlineLock)
            {
                if (serial == null) return;
                /*if (ignoreXEqualResendsResend>0 && (line & 65535) == (lastResendLine & 65535))
                {
                    ignoreXEqualResendsResend--;
                    return;
                }
                lastResendLine = line;
                ignoreXEqualResendsResend = 15;*/
                if (pingPong)
                    readyForNextSend = true;
                lock (nackLines)
                {
                    nackLinesBuffered.Clear();
                    nackLines.Clear(); // printer flushed all coming commands
                }
                lock (history)
                {
                    LinkedListNode<GCode> node = history.Last;
                    GCode gc = null;
                    if (node == null)
                    {
                        gc = new GCode("N" + line + " M105");
                        openResend = line;
                        history.AddLast(gc);
                        resendNode = history.Last;
                        history.AddFirst(new GCode("N1 M110"));
                        return;
                    }
                    if (resendError > 5 || node == null)
                    {
                        con.log(Trans.T("L_RECEIVING_ONLY_ERRORS"), false, 2); // Receiving only error messages. Stopped communication.
                        con.close(true);
                        RepetierHost.view.SoundConfig.PlayError(false);
                        return; // give up, something is terribly wrong
                    }
                    line &= 65535;
                    do
                    {
                        gc = node.Value;
                        if (gc.hasN && (gc.N & 65535) == line)
                        {
                            openResend = line;
                            resendNode = node;
                            TrySendNextLine();
                            return;
                        }
                        if (node.Previous == null)
                        {
                            history.Clear();
                            openResend = line;
                            history.AddFirst(new GCode("N" + line + " M105"));
                            resendNode = history.First;
                            history.AddFirst(new GCode("N1 M110"));
                            return;
                        }
                        node = node.Previous;
                        //history.RemoveLast();
                        //node = history.Last;
                    } while (true);
                }
            }
        }
        public override void TrySendNextLine()
        {
            writeEvent.Set(); // Reactivate write loop
        }
        public void WriteLoop()
        {
            bool abort = false;
            do
            {
                try
                {
                    while (true)
                    {
                        writeEvent.WaitOne(1);
                        while (TrySendNextLine2()) { }
                    }
                }
                catch (ThreadAbortException)
                {
                    abort = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            } while (abort == false);
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
            byte[] cmd = null;
            TestFakeOk();
            try
            {
                if (lastline > 30)
                    prequelFinished = true;
                lock (nextlineLock)
                {
                    if (pingPong && !readyForNextSend) return false;
                    if (serial == null) return false; // Not ready yet
                    if (!serial.IsOpen) // someone unplugged the cord?
                    {
                        con.close(true);
                        return false;
                    }
                    GCode gc = null;
                    try
                    {
                        // first resolve old communication problems
                        if (resendNode != null)
                        {
                            gc = resendNode.Value;
                            lastline = gc.N;
                            cmd = (binaryVersion == 0 || gc.forceAscii ? enc.GetBytes(gc.getAscii(true, true) + "\r\n") : gc.getBinary(binaryVersion));
                            if (!pingPong && receivedCount() + cmd.Length > receiveCacheSize) return false; // printer cache full
                            if (pingPong) readyForNextSend = false;
                            lock (nackLines) { nackLines.AddLast(new NackData(cmd.Length)); }
                            serial.Write(cmd, 0, cmd.Length);
                            bytesSend += cmd.Length;
                            linesSend++;
                            openResend = -1;
                            lastCommandSend = DateTime.Now.Ticks;
                            resendNode = resendNode.Next;
                            logtext = "Resend: " + gc.getAscii(true, true);
                            //  if (resendNode == null) readyForNextSend = true;
                            //readyForNextSend = false;
                            return true;
                        }
                        if (resendError > 0) resendError--; // Drop error counter
                        // then check for manual commands
                        if (injectCommands.Count > 0 && !job.exclusive)
                        {
                            lock (history)
                            {
                                gc = injectCommands.First.Value;
                                if (gc.hostCommand)
                                    hostCommand = gc;
                                else
                                {
                                    if (gc.M == 110)
                                        lastline = gc.N;
                                    else //if (gc.M != 117)
                                    {
                                        gc.N = ++lastline;
                                        lineInc = true;
                                    }
                                    cmd = (binaryVersion == 0 || gc.forceAscii ? enc.GetBytes(gc.getAscii(true, true) + "\r\n") : gc.getBinary(binaryVersion));
                                    if (!pingPong && receivedCount() + cmd.Length > receiveCacheSize) { if (lineInc) --lastline; return false; } // printer cache full
                                    if (pingPong) readyForNextSend = false;
                                }
                                injectCommands.RemoveFirst();
                            } // History
                            con.analyzer.Analyze(gc);
                            if (!gc.hostCommand)
                            {
                                lock (nackLines) { nackLines.AddLast(new NackData(cmd.Length, con.analyzer.estimatedCommandTime)); }
                                serial.Write(cmd, 0, cmd.Length);
                                // RLog.info("Send:" + gc.getAscii(true, true));
                                bytesSend += cmd.Length;
                                linesSend++;
                                lastCommandSend = DateTime.Now.Ticks;
                                historygc = gc;
                            }
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
                                    // bool forceReady = job.exclusive && boostUpload;
                                    cmd = (binaryVersion == 0 || gc.forceAscii ? enc.GetBytes(gc.getAscii(true, true) + "\r\n") : gc.getBinary(binaryVersion));
                                    if (!pingPong && receivedCount() + cmd.Length > receiveCacheSize) { if (lineInc) --lastline; return false; } // printer cache full
                                    if (pingPong) readyForNextSend = false;
                                    historygc = gc;
                                }
                                job.PopData();
                            }
                            con.analyzer.Analyze(gc);
                            if (!gc.hostCommand)
                            {
                                lock (nackLines) { nackLines.AddLast(new NackData(cmd.Length, con.analyzer.estimatedCommandTime)); }
                                RLog.info("string command to send: " + gc.Text);

                                serial.Write(cmd, 0, cmd.Length);
                                bytesSend += cmd.Length;
                                linesSend++;
                                lastCommandSend = DateTime.Now.Ticks;
                                printeraction = Trans.T1("L_PRINTING..ETA", job.ETA); //"Printing...ETA " + job.ETA;
                                if (job.maxLayer > 0)
                                    printeraction += " " + Trans.T2("L_LAYER_X/Y", con.analyzer.layer.ToString(), job.maxLayer.ToString()); // Layer " + analyzer.layer + "/" + job.maxLayer;
                                logprogress = job.PercentDone;
                            }
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
                    StoreHistory(historygc);
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
        int comErrorsReceived = 0;
        private void error(Object sender, SerialErrorReceivedEventArgs e)
        {
            comErrorsReceived++;
            con.log(Trans.T("L_SERIAL_COM_ERROR") + e.ToString(), false, 2); // "Serial com error:"
            if (comErrorsReceived == 10)
                con.close(true);
        }
        /// <summary>
        /// Mono version as mono does not execute received event.
        /// </summary>
        private void ReadThread()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(2);
                    if (!connected || serial == null || !serial.IsOpen) continue; // Not connected
                    if (serial.BytesToRead > 0)
                    {
                        try
                        {
                            string indata = serial.ReadExisting();
                            //Console.Write(indata);
                            read += indata.Replace('\r', '\n');
                            do
                            {
                                int pos = read.IndexOf('\n');
                                if (pos < 0) break;
                                string response = read.Substring(0, pos);
                                read = read.Substring(pos + 1);
                                if (response.Length > 0)
                                {

                                    RLog.info("the printer answered: " + response);
                                    AnalyzeResponse(response);
                                }
                                TrySendNextLine();
                            } while (true);
                        }
                        catch (ThreadAbortException)
                        {
                            return;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        lastReceived = DateTime.Now.Ticks / 10000;
                    }
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private void received(object sender,
                        SerialDataReceivedEventArgs e)
        {
            if (serial == null) return;
            string indata = serial.ReadExisting();
            read += indata.Replace('\r', '\n');
            do
            {
                int pos = read.IndexOf('\n');
                if (pos < 0) break;
                string response = read.Substring(0, pos);
                read = read.Substring(pos + 1);
                if (response.Length > 0)
                {
                    RLog.info("the printer answered: " + response);
                    AnalyzeResponse(response);
                }
                TrySendNextLine();
            } while (true);
            lastReceived = DateTime.Now.Ticks / 10000;
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
        private void RunStartCommands()
        {
            doRunStartCommands = -1;
            InjectManualCommand("N1 M110"); // Make sure we tal about the same linenumbers
            InjectManualCommand("N1 M110"); // Make sure we tal about the same linenumbers
            InjectManualCommand("M115"); // Check firmware
            Main.main.printPanel.sendDebug();
            initalizationFinished = true;
            con.FireConnectionChange(Trans.T("L_CONNECTED") + ":" + con.printerName);
            Main.main.Invoke(Main.main.UpdateJobButtons);
            con.analyzer.fireChanged();
            if (con.analyzer.powerOn)
                InjectManualCommand("M80");
        }
        public override void AnalyzeResponse(string res)
        {
            //RLog.info("Recv:" + res);
            string h;
            int level = 0;
            while (res.Length > 0 && res[0] < 32) res = res.Substring(1);
            res = res.Trim();
            if (res.Equals("start") || (garbageCleared == false && res.IndexOf("start") != -1))
            {
                prequelFinished = true;
                lastline = 0;
                job.KillJob(); // continuing the old job makes no sense, better save the plastic
                lock (history)
                {
                    resendNode = null;
                    history.Clear();
                }
                con.sdcardMounted = true;
                con.analyzer.start(false);
                readyForNextSend = true;
                lock (nackLines)
                {
                    nackLinesBuffered.Clear();
                    nackLines.Clear();
                }
                garbageCleared = true;
                doRunStartCommands = 5;
            }
            h = con.extract(res, "REPETIER_PROTOCOL:");
            if (h != null)
            {
                level = 3;
                lock (nextlineLock)
                {
                    int.TryParse(h, out binaryVersion);
                    if (transferProtocol == 1) binaryVersion = 0; // force ascii transfer
                    con.binaryVersion = binaryVersion;
                }
            }
            con.analyzeResponse(res, ref level);
            h = con.extract(res, "Resend:");
            if (h != null)
            {
                level = 1;
                con.log(res, true, level);
                int line;
                int.TryParse(h, out line);
                ignoreNextOk = true;
                ResendLine(line);
            }
            else if (res.StartsWith("ok"))
            {
                garbageCleared = true;
                if (Main.main.logView.switchACK.On)
                    con.log(res, true, level);
                if (!ignoreNextOk)  // ok in response of resend?
                {
                    if (pingPong) readyForNextSend = true;
                    lock (nackLines)
                    {
                        if (nackLines.Count > 0)
                        {
                            nackLinesBuffered.AddLast(nackLines.First.Value);
                            nackLines.RemoveFirst();
                            while (nackLinesBuffered.Count > 10)
                                nackLinesBuffered.RemoveFirst();
                        }
                    }
                    resendError = 0;
                    TrySendNextLine();
                }
                else
                    ignoreNextOk = false;
            }
            else if (res.Equals("wait")) //  && DateTime.Now.Ticks - lastCommandSend > 5000)
            {
                ignoreResendLine = -1;
                if (Main.main.logView.switchACK.On)
                    con.log(res, true, level);
                if (pingPong) readyForNextSend = true;
                lock (nackLines)
                {
                    nackLinesBuffered.Clear();
                    if (nackLines.Count > 0)
                        nackLines.Clear();
                }
                resendError = 0;
                TrySendNextLine();
            }
            else if (level >= 0 && garbageCleared) con.log(res, true, level);


        }
    }
}
