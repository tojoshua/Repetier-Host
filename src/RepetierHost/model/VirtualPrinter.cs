using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using RepetierHost.connector;

namespace RepetierHost.model
{
    class VirtualPrinter
    {
        float bedTemp = 20;
        float[] extruderTemp = new float[3];
        float[] extruderOut= new float[3];
        GCodeAnalyzer ana;
        LinkedList<string> output;
        Thread writeThread = null;
        int cnt = 0;
        int baudrate = 250000;
        int numExtruder = 3;
        int activeExtruder = 0;
        volatile int bytesin = 0;
        VirtualPrinterConnector vcon;

        private void WriteThread()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(2);
                    timer_Tick(null, null);
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

        void timer_Tick(object sender, EventArgs e2)
        {
            string res = null;
            if (baudrate >= 250000) bytesin = 0;
            if (bytesin > 0)
            {
                bytesin -= baudrate / 4000;
            } else
            do
            {
                res = null;
                lock (output)
                {
                    if (output.Count > 0)
                    {
                        res = output.First.Value;
                        output.RemoveFirst();
                    }
                }
                if (res != null)
                {
                    if (res.Length > 0)
                    {
                        vcon.AnalyzeResponse(res);
                    }
                    Main.conn.connector.TrySendNextLine();
                    //lastReceived = DateTime.Now.Ticks / 10000;
                    //Main.conn.VirtualResponse(res);
                }
            } while (res != null);
            cnt++;
            if (cnt > 500)
            {
                cnt = 0;
                bedTemp = bedTemp + Math.Sign(ana.bedTemp - bedTemp) * 2;
                if (ana.bedTemp > 20 && bedTemp > ana.bedTemp) bedTemp = ana.bedTemp;
                if (bedTemp < 20) bedTemp = 20;
                for (int e = 0; e < numExtruder; e++)
                {
                    extruderTemp[e] = extruderTemp[e] + Math.Sign(ana.getTemperature(e) - extruderTemp[e]) * 4;
                    if (ana.getTemperature(e) > 20 && extruderTemp[e] > ana.getTemperature(e)) extruderTemp[e] = ana.getTemperature(e);
                    if (extruderTemp[e] < 20) extruderTemp[e] = 20;
                    extruderOut[e] = (float)((ana.getTemperature(e) - 20.0) * 255.0 / 350 * (1.0 + 0.05 * Math.Sin((DateTime.Now.Ticks / 10000 % 9000) * 0.000897)));
                    if (extruderOut[e] < 0) extruderOut[e] = 0;
                    if (extruderOut[e] > 255) extruderOut[e] = 255;
                }
            }
        }
        public VirtualPrinter(VirtualPrinterConnector vc)
        {
            vcon = vc;
            ana = new GCodeAnalyzer(true);
            output = new LinkedList<string>();
            extruderTemp[0] = extruderTemp[1] = extruderTemp[2] = 0;
            extruderOut[0] = extruderOut[1] = extruderOut[2] = 0;
        }

        public void open(int baudrate)
        {
            this.baudrate = baudrate;
            ana.start(true);
            output.AddLast("start");
            writeThread = new Thread(new ThreadStart(this.WriteThread));
            writeThread.Start();
        }

        public void close()
        {
            writeThread.Abort();
        }
        public void receiveLine(GCode code)
        {
            bytesin += code.orig.Length;
            ana.Analyze(code);
            lock (output)
            {
                if (code.hasM) switch (code.M)
                    {
                        case 115: // Firmware
                            output.AddLast("FIRMWARE_NAME:RepetierVirtualPrinter FIRMWARE_URL:https://github.com/repetier/Repetier-Firmware/ PROTOCOL_VERSION:1.0 MACHINE_TYPE:Mendel EXTRUDER_COUNT:1 REPETIER_PROTOCOL:1");
                            //output.AddLast("FIRMWARE_NAME:Marlin FIRMWARE_URL:https://github.com/repetier/Repetier-Firmware/ PROTOCOL_VERSION:1.0 MACHINE_TYPE:Mendel EXTRUDER_COUNT:1 REPETIER_PROTOCOL:1");
                            break;
                        case 105: // Print Temperatures
                            output.AddLast("T:" + extruderTemp[activeExtruder].ToString("0") + " B:" + bedTemp.ToString("0.00")+" @:"+extruderOut[activeExtruder].ToString("0")+
                                " T0:"+extruderTemp[0].ToString("0.00")+" @0:"+extruderOut[0].ToString("0")+
                                " T1:"+extruderTemp[1].ToString("0.00")+" @1:"+extruderOut[1].ToString("0")+
                                " T2:"+extruderTemp[2].ToString("0.00")+" @2:"+extruderOut[2].ToString("0"));
                            break;
                        case 205: // EEPROM Settings
                            output.AddLast("EPR:2 75 76800 Baudrate");
                            output.AddLast("EPR:2 79 0 Max. inactive time [ms,0=off]");
                            output.AddLast("EPR:2 83 60000 Stop stepper afer inactivity [ms,0=off]");
                            output.AddLast("EPR:3 3 40.00 X-axis steps per mm");
                            output.AddLast("EPR:3 7 40.00 Y-axis steps per mm");
                            output.AddLast("EPR:3 11 3333.59 Z-axis steps per mm");
                            output.AddLast("EPR:3 15 20000.00 X-axis max. feedrate [mm/min]");
                            output.AddLast("EPR:3 19 20000.00 Y-axis max. feedrate [mm/min]");
                            output.AddLast("EPR:3 23 2.00 Z-axis max. feedrate [mm/min]");
                            output.AddLast("EPR:3 27 1500.00 X-axis homing feedrate [mm/min]");
                            output.AddLast("EPR:3 31 1500.00 Y-axis homing feedrate [mm/min]");
                            output.AddLast("EPR:3 35 100.00 Z-axis homing feedrate [mm/min]");
                            output.AddLast("EPR:3 39 20.00 X-axis start speed [mm/s]");
                            output.AddLast("EPR:3 43 20.00 Y-axis start speed [mm/s]");
                            output.AddLast("EPR:3 47 1.00 Z-axis start speed [mm/s]");
                            output.AddLast("EPR:3 51 750.00 X-axis acceleration [mm/s^2]");
                            output.AddLast("EPR:3 55 750.00 Y-axis acceleration [mm/s^2]");
                            output.AddLast("EPR:3 59 50.00 Z-axis acceleration [mm/s^2]");
                            output.AddLast("EPR:3 63 750.00 X-axis travel acceleration [mm/s^2]");
                            output.AddLast("EPR:3 67 750.00 Y-axis travel acceleration [mm/s^2]");
                            output.AddLast("EPR:3 71 50.00 Z-axis travel acceleration [mm/s^2]");
                            output.AddLast("EPR:3 150 373.00 Extr. steps per mm");
                            output.AddLast("EPR:3 154 1200.00 Extr. max. feedrate [mm/min]");
                            output.AddLast("EPR:3 158 10.00 Extr. start feedrate [mm/s]");
                            output.AddLast("EPR:3 162 10000.00 Extr. acceleration [mm/s^2]");
                            output.AddLast("EPR:0 166 1 Heat manager [0-1]");
                            output.AddLast("EPR:0 167 130 PID drive max");
                            output.AddLast("EPR:2 168 300 PID P-gain [*0.01]");
                            output.AddLast("EPR:2 172 2 PID I-gain [*0.01]");
                            output.AddLast("EPR:2 176 2000 PID D-gain [*0.01]");
                            output.AddLast("EPR:0 180 200 PID max value [0-255]");
                            output.AddLast("EPR:2 181 0 X-offset [steps]");
                            output.AddLast("EPR:2 185 0 Y-offset [steps]");
                            output.AddLast("EPR:2 189 40 Temp. stabilize time [s]");
                            break;
                    }
                else if (code.hasT)
                {
                    if (code.T >= 0 && code.T < numExtruder)
                        activeExtruder = code.T;
                }
                output.AddLast("ok");
            }
        }
    }
}
