/*
   Copyright 2011 repetier repetierdev@googlemail.com

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;
using System.Timers;
using System.Threading;
using System.Windows.Forms;
using RepetierHost.view.utils;
using System.Globalization;
using RepetierHost.connector;

namespace RepetierHost.model
{
    /// <summary>
    /// Connection has changed.
    /// </summary>
    /// <param name="msg"></param>
    public delegate void OnPrinterConnectionChange(string msg);
    public delegate void OnPrinterAction(string action);
    public delegate void OnLogCleared();
    public delegate void OnLogAppend(LogLine line);
    public delegate void OnLogRemoveLast(LogLine line);
    public delegate void OnTempUpdate(float extruder, float printbed);
    public delegate void OnJobProgress(float percent);
    public delegate void OnTempMonitor(UInt32 time, float temp, float target, int output);
    public delegate void OnTempHistory(TemperatureEntry ent);
    public delegate void OnResponse(string response);
    public class PrinterConnection : IDisposable
    {
        public event OnPrinterConnectionChange eventConnectionChange;
        public event OnPrinterAction eventPrinterAction;
        public event OnLogCleared eventLogCleared;
        //public event OnLogAppend eventLogAppend;
        //public event OnLogRemoveLast eventLogRemoveLast;
        public event OnLogAppend eventLogUpdate;
        public event OnTempUpdate eventTempChange;
        public event OnJobProgress eventJobProgress;
        public event OnTempMonitor eventTempMonitor;
        public event OnResponse eventResponse;
        public event OnTempHistory eventTempHistory;
        TextWriter logWriter = null;
        public GCodeAnalyzer analyzer = new GCodeAnalyzer(false);
        //public bool connected = false;
        // ======== Printer data =============
        public string printerName = "default";
     //   public int transferProtocol = 0; // 0 = auto, 1 = force ascii, 2 = force binary
        public int binaryVersion = 0; // needed for gcode compression
     //   public int baud = 57600;
        public float addPrintingTime = 8;
      //  public bool garbageCleared = false; // Skip old output
      //  public Parity parity = Parity.None;
      //  public StopBits stopbits = StopBits.One;
      //  public int databits = 8;
      //  public SerialPort serial = null;
      //  public string port = "COM10";
        public float travelFeedRate = 4800;
        public float printFeedRate = 2400;
        public float maxZFeedRate = 100;
        public float disposeX = 130;
        public float disposeY = 0;
        public float disposeZ = 0;
        public bool afterJobGoDispose = true;
        public bool afterJobDisableExtruder = true;
        public bool afterJobDisablePrintbed = true;
        public bool afterJobDisableMotors = false;
        public bool sdcardMounted = true;
       // string read = "";
        public LinkedList<LogLine> logList = new LinkedList<LogLine>();
        public LinkedList<LogLine> newLogs = new LinkedList<LogLine>();
        public int maxLogLines = 1000;
       // bool readyForNextSend = true;
      //  public bool pingpong = false;
        //public LinkedList<GCode> injectCommands = new LinkedList<GCode>();
        //public LinkedList<GCode> history = new LinkedList<GCode>();
        //LinkedListNode<GCode> resendNode = null;
        public EEPROMStorage eeprom = new EEPROMStorage();
        public EEPROMMarlinStorage eepromm = new EEPROMMarlinStorage();
        //public Printjob job;
       // private Object nextlineLock = new Object();
        // Printer data
        public string machine = "unknown";
        public string firmware = "";
        public string firmware_url = "";
        public string protocol = "";
        public int numberExtruder = 1;
        public Dictionary<int, float> extruderTemp = new Dictionary<int, float>();
        public bool multiTempRead = false; // true if M105 sends temperatures for all extruder
        public float bedTemp;
        public Dictionary<int, int> extruderOutput = new Dictionary<int, int>();
        public float x, y, z, e;
        //public bool paused = false;
        public bool logM105 = false;
      //  public int lastline = 0;
      //  long lastReceived = 0;
        public bool autocheckTemp = true;
        public long autocheckInterval = 3000;
        public long lastAutocheck = 0;
        System.Timers.Timer timer = null;
        public string nextPrinterAction = null;
        public string lastPrinterAction = "";
        public LinkedList<int> nackLines = new LinkedList<int>(); // Lines, whoses receivement were not acknowledged
        public float lastlogprogress = -1000;
        public string filterCommand = "yourFilter #in #out";
        public bool runFilterEverySlice = false;
        public bool isRepetier = false;
        public bool isMarlin = false;
        public bool isSprinter = false;
        public int speedMultiply = 100;
        public int flowMultiply = 100;
        public bool boostUpload = false;
        public int numExtruder = 1;
        public double ignoreFeedbackUntil = 0;
        public PrinterConnectorBase connector = null;

        public PrinterConnection()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 100;
            timer.AutoReset = true;
            timer.Elapsed += handleTimer;
            timer.Start();

            try
            {
                string dir = Main.globalSettings.Workdir;
                if (dir.Length > 0 && Main.globalSettings.LogEnabled)
                    logWriter = new StreamWriter(dir + Path.DirectorySeparatorChar + "repetier.log");
            }
            catch
            {
                logWriter = null;
            }
            //writeEvent = new AutoResetEvent(false);
        }
                protected virtual void Dispose(bool disposing) 
        {
            if (disposing) 
            {
                if(timer !=null)
                    timer.Dispose();
                if (logWriter != null)
                    logWriter.Dispose();
                logWriter = null;
                timer = null;
            }
        }

        public void Dispose()
        {
                Dispose(true);
                GC.SuppressFinalize(this);
        }

        // Disposable types implement a finalizer.
        ~PrinterConnection()
        {
            Dispose(false);
        }
        public void Destroy()
        {
            if (logWriter != null)
            {
                logWriter.Close();
            }
        }
        public void FireConnectionChange(string text)
        {
            if (eventConnectionChange != null)
                Main.main.Invoke(eventConnectionChange, text);
        }
        public void FireJobProgressAsync(float prg)
        {
            Main.main.Invoke(eventJobProgress, prg);
        }
        public void ignoreFeedback()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            ignoreFeedbackUntil = ts.TotalSeconds + 0.5;
        }
        public bool shouldIgnoreFeedback()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return ignoreFeedbackUntil > ts.TotalSeconds;
        }
        void handleTimer(object sender, EventArgs e)
        {
            if (nextPrinterAction != null && !nextPrinterAction.Equals(lastPrinterAction))
            {
                lastPrinterAction = nextPrinterAction;
                try
                {
                    if (eventPrinterAction != null)
                        Main.main.Invoke(eventPrinterAction, nextPrinterAction);
                }
                catch { }
                nextPrinterAction = null;
            }
            if(connector!=null)
                connector.RunPeriodicalTasks();
        }
        LogLine useNextLog = null;

        public static void logInfo(string text)
        {
            Main.conn.log(text, false, 3);
        }
        public void log(string t, bool response, int level)
        {
            LogLine l;
            bool update = false;
            if (logM105 && t.IndexOf("M105") >= 0) return;
            if (useNextLog != null)
            {
                l = useNextLog;
                l.text = t;
                l.response = response;
                l.level = level;
                update = true;
                useNextLog = null;
            }
            else
                l = new LogLine(t, response, level);
            if (logWriter != null)
            {
                if (!response)
                {
                    lock (logWriter)
                    {
                        logWriter.WriteLine("< " + l.text);
                    }
                }
            }
            lock (logList)
            {
                if (l.text.Length > 0 && l.text[l.text.Length - 1] == 27)
                {
                    //useNextLog = l;
                    l.text = l.text.Substring(0, l.text.Length - 1);
                }
                if (!update) logList.AddLast(l);
            }
            if (logList.Count > maxLogLines)
            {
                LogLine removed = null;
                lock (logList)
                {
                    removed = logList.First.Value;
                    logList.RemoveFirst();
                }
                /* if (eventLogRemoveLast!=null)
                 {
                     try
                     {
                         Main.main.Invoke(eventLogRemoveLast, removed);
                     }
                     catch { } // Closing the app can cause an exception, if event comes after Main handle is destroyed
                 }*/
            }
            if (!update /*&& eventLogAppend!=null*/)
                try
                {
                    lock (newLogs)
                    {
                        newLogs.AddLast(l);
                    }
                    //Main.main.Invoke(eventLogAppend, l);
                }
                catch { } // Closing the app can cause an exception, if event comes after Main handle is destroyed

            if (update && eventLogUpdate != null)
                try
                {
                    Main.main.Invoke(eventLogUpdate, l);
                }
                catch { } // Closing the app can cause an exception, if event comes after Main handle is destroyed
        }
       /* private void StoreHistory(GCode gcode)
        {
            history.AddLast(gcode);
            log(gcode.getAscii(true, true), false, 0);
            if (history.Count > 40)
                history.RemoveFirst();
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
        public void ResendLine(int line)
        {
            resendError++;
            errorsReceived++;
            if (pingpong)
                readyForNextSend = true;
            else
                nackLines.Clear(); // printer flushed all coming commands

            LinkedListNode<GCode> node = history.Last;
            if (resendError > 5 || node == null)
            {
                log(Trans.T("L_RECEIVING_ONLY_ERRORS"), false, 2); // Receiving only error messages. Stopped communication.
                close();
                RepetierHost.view.SoundConfig.PlayError(false);
                return; // give up, something is terribly wrong
            }
            line &= 65535;
            do
            {
                GCode gc = node.Value;
                if (gc.hasN && (gc.N & 65535) == line)
                {
                    resendNode = node;
                    if (binaryVersion != 0)
                    {
                        int send = receivedCount();
                        serial.DiscardOutBuffer();
                        System.Threading.Thread.Sleep(send * 10000 / baud); // Wait for buffer to empty
                        byte[] buf = new byte[32];
                        for (int i = 0; i < 32; i++) buf[i] = 0;
                        serial.Write(buf, 0, 32);
                        System.Threading.Thread.Sleep(320000 / baud); // Wait for buffer to empty
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(receiveCacheSize * 10000 / baud); // Wait for buffer to empty
                    }
                    TrySendNextLine();
                    return;
                }
                if (node.Previous == null) return;
                node = node.Previous;
            } while (true);
        }*/
        public void pause(string text)
        {
            connector.PauseJob(text);
            /*if (paused) return;
            paused = true;
            PauseInfo.ShowPause(text);
            foreach (GCodeShort code in Main.main.editor.getContentArray(4))
            {
                injectManualCommand(code.text);
            }*/
            //MessageBox.Show(Main.main, text, "Printer paused", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //paused = false;
        }
        /*public void TrySendNextLine()
        {
            writeEvent.Set(); // Reactivate write look
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
                        if (pingpong && !readyForNextSend)
                        {
                            writeEvent.WaitOne(1);
                        }
                        else if (serial == null && !isVirtualActive)
                        {
                            // Not ready yet
                            writeEvent.WaitOne(1);
                        }
                        else
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
            try
            {
                lock (nextlineLock)
                {
                    if (pingpong && !readyForNextSend) return false;
                    if (serial == null && !isVirtualActive) return false; // Not ready yet
                    if (!isVirtualActive && !serial.IsOpen) // someone unplugged the cord?
                    {
                        close();
                        return false;
                    }
                    GCode gc = null;
                    try
                    {
                        // first resolve old communication problems
                        if (resendNode != null)
                        {
                            gc = resendNode.Value;
                            if (binaryVersion == 0 || gc.forceAscii)
                            {
                                string cmd = gc.getAscii(true, true);
                                if (!pingpong && receivedCount() + cmd.Length + 2 > receiveCacheSize) return false; // printer cache full
                                if (pingpong) readyForNextSend = false;
                                else { lock (nackLines) { nackLines.AddLast(cmd.Length + 2); } }
                                serial.WriteLine(cmd);
                                bytesSend += cmd.Length + 2;
                            }
                            else
                            {
                                byte[] cmd = gc.getBinary(binaryVersion);
                                if (!pingpong && receivedCount() + cmd.Length > receiveCacheSize) return false; // printer cache full
                                if (pingpong) readyForNextSend = false;
                                else { lock (nackLines) { nackLines.AddLast(cmd.Length); } }
                                serial.Write(cmd, 0, cmd.Length);
                                bytesSend += cmd.Length;
                            }
                            linesSend++;
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
                                    if (isVirtualActive)
                                    {
                                        if (!pingpong && receivedCount() + gc.orig.Length > receiveCacheSize) { if (lineInc) --lastline; return false; } // printer cache full
                                        if (pingpong) readyForNextSend = false;
                                        else { lock (nackLines) { nackLines.AddLast(gc.orig.Length); } }
                                        virtualPrinter.receiveLine(gc);
                                        bytesSend += gc.orig.Length;
                                    }
                                    else
                                        if (binaryVersion == 0 || gc.forceAscii)
                                        {
                                            string cmd = gc.getAscii(true, true);
                                            if (!pingpong && receivedCount() + cmd.Length + 2 > receiveCacheSize) { if (lineInc) --lastline; return false; } // printer cache full
                                            if (pingpong) readyForNextSend = false;
                                            else { lock (nackLines) { nackLines.AddLast(cmd.Length); } }
                                            serial.WriteLine(cmd);
                                            bytesSend += cmd.Length + 2;
                                        }
                                        else
                                        {
                                            byte[] cmd = gc.getBinary(binaryVersion);
                                            if (!pingpong && receivedCount() + cmd.Length > receiveCacheSize) { if (lineInc) --lastline; return false; } // printer cache full
                                            if (pingpong) readyForNextSend = false;
                                            else { lock (nackLines) { nackLines.AddLast(cmd.Length); } }
                                            serial.Write(cmd, 0, cmd.Length);
                                            bytesSend += cmd.Length;
                                        }
                                }
                                injectCommands.RemoveFirst();
                            }
                            if (!gc.hostCommand)
                            {
                                linesSend++;
                                lastCommandSend = DateTime.Now.Ticks;
                                historygc = gc;
                            }
                            analyzer.Analyze(gc);
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
                                    if (isVirtualActive)
                                    {
                                        string cmd = gc.getAscii(true, true);
                                        if (!pingpong && receivedCount() + cmd.Length > receiveCacheSize) { if (lineInc) --lastline; return false; } // printer cache full
                                        if (pingpong) readyForNextSend = false;
                                        else { lock (nackLines) { nackLines.AddLast(cmd.Length ); } }
                                        virtualPrinter.receiveLine(gc);
                                        bytesSend += cmd.Length; // gc.orig.Length;
                                    }
                                    else
                                    {
                                        // bool forceReady = job.exclusive && boostUpload;
                                        if (binaryVersion == 0 || gc.forceAscii)
                                        {
                                            string cmd = gc.getAscii(true, true);
                                            if (!pingpong && receivedCount() + cmd.Length + 2 > receiveCacheSize) { if (lineInc) --lastline; return false; } // printer cache full
                                            if (pingpong) readyForNextSend = false;
                                            else { lock (nackLines) { nackLines.AddLast(cmd.Length + 2); } }
                                            serial.WriteLine(cmd);
                                            bytesSend += cmd.Length + 2;
                                        }
                                        else
                                        {
                                            byte[] cmd = gc.getBinary(binaryVersion);
                                            if (!pingpong && receivedCount() + cmd.Length > receiveCacheSize) { if (lineInc) --lastline; return false; } // printer cache full
                                            if (pingpong) readyForNextSend = false;
                                            else { lock (nackLines) { nackLines.AddLast(cmd.Length); } }
                                            serial.Write(cmd, 0, cmd.Length);
                                            bytesSend += cmd.Length;
                                        }
                                    }
                                    historygc = gc;
                                }
                                job.PopData();
                            }
                            if (!gc.hostCommand)
                            {
                                linesSend++;
                                lastCommandSend = DateTime.Now.Ticks;
                                printeraction = Trans.T1("L_PRINTING..ETA", job.ETA); //"Printing...ETA " + job.ETA;
                                if (job.maxLayer > 0)
                                    printeraction += " " + Trans.T2("L_LAYER_X/Y", analyzer.layer.ToString(), job.maxLayer.ToString()); // Layer " + analyzer.layer + "/" + job.maxLayer;
                                logprogress = job.PercentDone;
                            }
                            analyzer.Analyze(gc);
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
                    log(logtext, false, loglevel);
                if (printeraction != null)
                    firePrinterAction(printeraction);
                if (logprogress >= 0 && Math.Abs(lastlogprogress - logprogress) > 0.3 && eventJobProgress != null)
                {
                    lastlogprogress = logprogress;
                    Main.main.Invoke(eventJobProgress, job.PercentDone);
                }
            }
            return false;
        }
         * */
        /// <summary>
        /// Clean log.
        /// </summary>
        public void clearLog()
        {
            logList.Clear();
            if (eventLogCleared != null)
                eventLogCleared();
        }
        public void open()
        {
            if (connector.IsConnected()) return;
            isMarlin = isRepetier = isSprinter = false;
            Main.printerSettings.Hide();
            connector.Connect();
        }

        public bool close()
        {
            return close(false);
        }
        public bool close(bool force)
        {
            bool ret = connector.Disconnect(force);
            if (ret)
            {
                try
                {
                    FireConnectionChange(Trans.T("L_DISCONNECTED"));
                }
                catch { } // Closing the app can cause an exception, if event comes after Main handle is destroyed
                firePrinterAction(Trans.T("L_IDLE"));
                Main.main.Invoke(Main.main.UpdateJobButtons);
            }
            return ret;
        }

        public void firePrinterAction(string s)
        {
            nextPrinterAction = s;
        }
        /// <summary>
        /// Send a print command, that does not belong to a print job.
        /// </summary>
        /// <param name="command">GCode command</param>
        public void injectManualCommand(string command)
        {
            connector.InjectManualCommand(command);
        }
        public void injectManualCommandFirst(string command)
        {
            connector.InjectManualCommandFirst(command);
        }
        public MethodInvoker firmwareRequestedPause = delegate
        {
            Main.conn.pause(Trans.T("L_PAUSED_FIRMWARE"));
        };
        public float getTemperature(int extr)
        {
            if (extr < 0) extr = analyzer.activeExtruderId;
            if (!extruderTemp.ContainsKey(extr))
                extruderTemp.Add(extr, 0.0f);
            return extruderTemp[extr];
        }
        public void setTemperature(int extr, float t)
        {
            if (extr < 0) extr = analyzer.activeExtruderId;
            if (!extruderTemp.ContainsKey(extr))
                extruderTemp.Add(extr, t);
            else extruderTemp[extr] = t;
        }
        public float getOutput(int extr)
        {
            if (extr < 0) extr = analyzer.activeExtruderId;
            if (!extruderOutput.ContainsKey(extr))
                extruderOutput.Add(extr, -1);
            return extruderOutput[extr];
        }
        public void setOutput(int extr, int o)
        {
            if (extr < 0) extr = analyzer.activeExtruderId;
            if (!extruderOutput.ContainsKey(extr))
                extruderOutput.Add(extr, o);
            else extruderOutput[extr] = o;
        }
        /// <summary>
        /// Analyzes a response from the printer.
        /// Updates data and sends events according to the data.
        /// </summary>
        /// <param name="res"></param>
        public void analyzeResponse(string res,ref int level)
        {
            while (res.Length > 0 && res[0] < 32) res = res.Substring(1);
            res = res.Trim();
            bool ignoreFB = shouldIgnoreFeedback();
            if (logWriter != null)
            {
                DateTime time = DateTime.Now;
                lock (logWriter)
                {
                    logWriter.WriteLine("> " + time.Hour.ToString("00") + ":" + time.Minute.ToString("00") + ":" +
                    time.Second.ToString("00") + "." + time.Millisecond.ToString("000") + " : " + res);
                }
            }
            if (eventResponse != null)
            {
                eventResponse(res);
            }
            string h = extract(res, "FIRMWARE_NAME:");
            if (h != null)
            {
                level = 3;
                firmware = h;
                h = h.ToLower();
                if (h.IndexOf("repetier") >= 0)
                {
                    isRepetier = true;
                }
                if (h.IndexOf("marlin") >= 0) isMarlin = true;
                if (h.IndexOf("sprinter") >= 0) isSprinter = true;
                if (isMarlin || isRepetier || isSprinter) // Activate special menus and function
                {
                    Main.main.Invoke(Main.main.UpdateEEPROM);
                    injectManualCommand("M220 S" + speedMultiply.ToString());
                    injectManualCommand("M221 S" + flowMultiply.ToString());
                }
                Main.main.Invoke(Main.main.FirmwareDetected);
            }
            h = extract(res, "FIRMWARE_URL:");
            if (h != null)
            {
                level = 3;
                firmware_url = h;
            }
            h = extract(res, "PROTOCOL_VERSION:");
            if (h != null)
            {
                level = 3;
                protocol = h;
            }
            h = extract(res, "MACHINE_TYPE:");
            if (h != null)
            {
                level = 3;
                machine = h;
            }
            h = extract(res, "EXTRUDER_COUNT:");
            if (h != null)
            {
                level = 3;
                if (!int.TryParse(h, out numberExtruder)) numberExtruder = 1;
            }
            h = extract(res, "X:");
            if (h != null)
            {
                level = 3;
                float.TryParse(h, NumberStyles.Float, GCode.format, out x);
                analyzer.x = x;
                analyzer.hasXHome = true;
            }
            h = extract(res, "Y:");
            if (h != null)
            {
                level = 3;
                float.TryParse(h, NumberStyles.Float, GCode.format, out y);
                analyzer.y = y;
                analyzer.hasYHome = true;
            }
            h = extract(res, "Z:");
            if (h != null)
            {
                level = 3;
                float.TryParse(h, NumberStyles.Float, GCode.format, out z);
                analyzer.z = z;
                analyzer.hasZHome = true;
            }
            h = extract(res, "E:");
            if (h != null)
            {
                level = 3;
                float.TryParse(h, NumberStyles.Float, GCode.format, out e);
                analyzer.activeExtruder.e = e;
            }
            if (!ignoreFB)
            {
                if ((h = extract(res, "SpeedMultiply:")) != null)
                {
                    int.TryParse(h, out speedMultiply);
                    if (speedMultiply < 25) speedMultiply = 25;
                    if (speedMultiply > 300) speedMultiply = 300;
                    analyzer.fireChanged();
                }
                if ((h = extract(res, "FlowMultiply:")) != null)
                {
                    int.TryParse(h, out flowMultiply);
                    if (flowMultiply < 50) flowMultiply = 50;
                    if (flowMultiply > 150) flowMultiply = 150;
                    analyzer.fireChanged();
                }
            }
            if ((h = extract(res, "TargetExtr0:")) != null)
            {
                float et;
                float.TryParse(h, NumberStyles.Float, GCode.format, out et);
                analyzer.setTemperature(0, et);
                analyzer.fireChanged();
            }
            if ((h = extract(res, "TargetExtr1:")) != null)
            {
                float et;
                float.TryParse(h, NumberStyles.Float, GCode.format, out et);
                analyzer.setTemperature(1, et);
                analyzer.fireChanged();
            }
            if ((h = extract(res, "TargetBed:")) != null)
            {
                float.TryParse(h, NumberStyles.Float, GCode.format, out analyzer.bedTemp);
                analyzer.fireChanged();
            }
            if ((h = extract(res, "Fanspeed:")) != null)
            {
                int.TryParse(h, out analyzer.fanVoltage);
                analyzer.fireChanged();
            }
            if (res.Contains("RequestPause:"))
            {
                Main.main.Invoke(firmwareRequestedPause);
            }
            bool tempChange = false;
            h = extract(res, "T0:");
            if (h != null)
            {
                multiTempRead = true;
                int n = 0;
                level = -1; // dont log, we see result in status
                //if (h.IndexOf('.') > 0) h = h.Substring(0, h.IndexOf('.'));
                do
                {
                    float et;
                    float.TryParse(h, NumberStyles.Float, GCode.format, out et);
                    tempChange = true;
                    setTemperature(n, et);
                    h = extract(res, "@" + n + ":");
                    int eo = -1;
                    int.TryParse(h, out eo);
                    if (isMarlin) eo *= 2;
                    setOutput(n, eo);
                    n++;
                    h = extract(res, "T" + n + ":");
                } while (h != null);
            }
            else
            {
                h = extract(res, "T:");
                if (h != null)
                {
                    level = -1; // dont log, we see result in status
                    //if (h.IndexOf('.') > 0) h = h.Substring(0, h.IndexOf('.'));
                    float et;
                    float.TryParse(h, NumberStyles.Float, GCode.format, out et);
                    setTemperature(-1, et);
                    tempChange = true;
                    int eo = -1;
                    h = extract(res, "@:");
                    int.TryParse(h, out eo);
                    if (isMarlin) eo *= 2;
                    setOutput(-1, eo);
                }
            }
            h = extract(res, "B:");
            if (h != null)
            {
                level = -1; // don't log, we see result in status
                //if (h.IndexOf('.') > 0) h = h.Substring(0, h.IndexOf('.'));
                float.TryParse(h, NumberStyles.Float, GCode.format, out bedTemp);
                tempChange = true;
            }
            if (isRepetier)
            { // Repetier specific answers
                if (res.StartsWith("EPR:"))
                {
                    eeprom.Add(res);
                }
            }
            if (isMarlin)
            { // Marlin specifix answers
                if (res.StartsWith("echo:") && (res.IndexOf("M92") > 0) || (res.IndexOf("M203") > 0) || (res.IndexOf("M201") > 0) ||
                    (res.IndexOf("M204") > 0) || (res.IndexOf("M205") > 0) || (res.IndexOf("M206") > 0) || (res.IndexOf("M301") > 0))
                {
                    eepromm.Add(res);
                }
            }
            if (res.StartsWith("MTEMP:")) // Temperature monitor 
            {
                level = -1; // this happens to often to log. Temperture monitor i sthe log
                string[] sl = res.Substring(6).Split(' ');
                if (sl.Length == 4)
                {
                    UInt32 time;
                    int temp, target, output;
                    UInt32.TryParse(sl[0], out time);
                    int.TryParse(sl[1], out temp);
                    int.TryParse(sl[2], out target);
                    int.TryParse(sl[3], out output);
                    if (time > 0 && eventTempMonitor != null)
                    {
                        try
                        {
                            Main.main.Invoke(eventTempMonitor, time, temp, target, output);
                        }
                        catch { }
                    }
                    if (eventTempHistory != null)
                    {
                        TemperatureEntry te = new TemperatureEntry(analyzer.tempMonitor, temp, output, analyzer.bedTemp, analyzer.getTemperature(-1));
                        Main.main.Invoke(eventTempHistory, te);
                    }
                }
            }

            if (extract(res, "Error:") != null)
            {
                level = 2;
                RepetierHost.view.SoundConfig.PlayError(false);
            }
            if (tempChange && eventTempChange != null)
            {
                Main.main.Invoke(eventTempChange, getTemperature(-1), bedTemp);
            }
            if (tempChange && eventTempHistory != null)
            {
                TemperatureEntry te = new TemperatureEntry(getTemperature(-1), bedTemp, analyzer.bedTemp, analyzer.getTemperature(-1));
                if (getOutput(-1) >= 0)
                    te.output = getOutput(-1);
                Main.main.Invoke(eventTempHistory, te);
            }
            if (res.StartsWith(" ")) level = 3;
        }
        public string extract(string source, string ident)
        {
            int pos = 0;
            do
            {
                pos = source.IndexOf(ident, pos == 0 ? pos : pos + 1);
                if (pos < 0) return null;
            } while (pos > 0 && source[pos - 1] != ' ');
            int start = pos + ident.Length;
            while (start < source.Length && source[start] == ' ') start++;
            int end = start;
            while (end < source.Length && source[end] != ' ') end++;
            return source.Substring(start, end - start);
        }
        public void doDispose()
        {
            if (analyzer.hasXHome == false || analyzer.hasYHome == false) return; // don't know where we are
            float dx = disposeX - analyzer.xOffset - (analyzer.relative ? analyzer.x : 0);
            float dy = disposeY - analyzer.yOffset - (analyzer.relative ? analyzer.y : 0);
            string zextra = "";
            connector.GetInjectLock();
            injectManualCommand("G1 X" + dx.ToString(GCode.format) + " Y" + dy.ToString(GCode.format) + " F" + travelFeedRate.ToString(GCode.format));
            if (analyzer.hasZHome && analyzer.z + analyzer.zOffset < disposeZ && disposeZ > 0 && disposeZ <= Main.printerSettings.PrintAreaHeight)
            {
                float dz = disposeZ - analyzer.zOffset - (analyzer.relative ? analyzer.z : 0);
                zextra = "G1 Z" + dz.ToString(GCode.format) + " F" + maxZFeedRate.ToString(GCode.format);
                injectManualCommand(zextra);
            }
            connector.ReturnInjectLock();
        }
    }
}
