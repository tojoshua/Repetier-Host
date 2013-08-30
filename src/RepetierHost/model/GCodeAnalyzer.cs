/*
   Copyright 2011 repetier repetierdev@gmail.com

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
using RepetierHost.view;
using RepetierHost.model;

namespace RepetierHost.model
{
    public delegate void OnPosChange(GCode code, float x, float y, float z);
    public delegate void OnPosChangeFast(float x, float y, float z, float e);
    public delegate void OnAnalyzerChange();
    public class ExtruderData
    {
        public ExtruderData(int _id) { id = _id; }
        public int id;
        public float temperature=0;
        public float e = 0;
        public float emax = 0;
        public float lastE = 0;
        public float eOffset = 0;
        public bool retracted = false;
    }
    public class GCodeAnalyzer
    {
        public event OnPosChange eventPosChanged;
        public event OnPosChangeFast eventPosChangedFast;
        public event OnAnalyzerChange eventChange;
        public int activeExtruderId = 0;
        public ExtruderData activeExtruder = null;
        //public float extruderTemp = 0;
        public Dictionary<int, ExtruderData> extruder = new Dictionary<int, ExtruderData>();
        public LinkedList<GCodeShort> unchangedLayer = new LinkedList<GCodeShort>();
        public bool uploading = false;
        public float bedTemp = 0;
        public float x = 0, y = 0, z = 0, f = 1000;
        public float lastX = 0, lastY = 0, lastZ = 0;
        public float xOffset = 0, yOffset = 0, zOffset = 0, lastZPrint = 0, layerZ = 0;
        public bool fanOn = false;
        public int fanVoltage = 0;
        public bool powerOn = true;
        public bool relative = false;
        public bool eRelative = false;
        public int debugLevel = 6;
        public int lastline = 0;
        public bool hasXHome = false, hasYHome = false, hasZHome = false;
        public bool privateAnalyzer = false;
        public int maxDrawMethod = 2;
        public bool drawing = true;
        public int layer = 0, lastlayer = 0;
        public bool isG1Move = false;
        public int speedMultiply = 100;
        public float printerWidth, printerHeight, printerDepth;
        public int tempMonitor = 0;
        public double printingTime = 0;
        public bool eChanged;
        public long estimatedCommandTime; // safe estimate of execution time in milliseconds

        public GCodeAnalyzer(bool privAnal)
        {
            privateAnalyzer = privAnal;
            foreach (int k in extruder.Keys)
                extruder[k] = new ExtruderData(k);
            if (!extruder.ContainsKey(activeExtruderId))
                extruder.Add(activeExtruderId, new ExtruderData(activeExtruderId));
            activeExtruder = extruder[activeExtruderId];
            bedTemp = 0;
        }
        public float RealX
        {
            get { return x + xOffset; }
        }
        public float RealY
        {
            get { return y + yOffset; }
        }
        public float RealZ
        {
            get { return z + zOffset; }
        }
        public float getTemperature(int extr)
        {
            if (extr < 0) extr = activeExtruderId;
            if (!extruder.ContainsKey(extr))
                extruder.Add(extr, new ExtruderData(extr));
            return extruder[extr].temperature;
        }
        public void setTemperature(int extr, float t)
        {
            if (extr < 0) extr = activeExtruderId;
            if (!extruder.ContainsKey(extr))
            {
                ExtruderData ed = new ExtruderData(extr);
                ed.temperature = t;
                extruder.Add(extr, ed);
            }
            else extruder[extr].temperature = t;
        }
        public void fireChanged()
        {
            if (eventChange != null)
            {
                try
                {
                    Main.main.Invoke(eventChange);
                }
                catch { }
            }
        }
        // set to start condition
        public void start(bool fire)
        {
            relative = false;
            eRelative = false;
            activeExtruderId = 0;
            List<int> keys = new List<int>();
            foreach (int k in extruder.Keys)
                keys.Add(k);
            if(!keys.Contains(activeExtruderId))
                keys.Add(activeExtruderId);
            foreach (int k in keys)
                extruder[k] = new ExtruderData(k);
            activeExtruder = extruder[activeExtruderId];
            bedTemp = 0;
            fanOn = false;
            powerOn = true;
            fanVoltage = 0;
            maxDrawMethod = 2;
            drawing = true;
            lastline = 0;
            layer = 0;
            lastlayer = 0;
            layerZ = 0;
            x = y = z = lastZPrint = 0;
            xOffset = yOffset = zOffset = 0;
            lastX = 0; lastY = 0; lastZ = 0;
            hasXHome = hasYHome = hasZHome = false;
            printerWidth = Main.printerSettings.PrintAreaWidth;
            printerDepth = Main.printerSettings.PrintAreaDepth;
            printerHeight = Main.printerSettings.PrintAreaHeight;
            if (!privateAnalyzer)
                Main.main.jobVisual.ResetQuality();
            if(fire)
                fireChanged();
        }
        public void StartJob()
        {
            layer = 0;
            lastZPrint = 0;
            printingTime = 0;
            lastX = 0; lastY = 0; lastZ = 0;
            lastlayer = 0;
            layerZ = 0;
            drawing = true;
            uploading = false;
            foreach (ExtruderData ed in extruder.Values)
            {
                ed.lastE = 0;
                ed.emax = 0;
                ed.e = 0;
                ed.eOffset = 0;
                ed.retracted = false;
            }
            if (!privateAnalyzer)
                Main.main.jobVisual.ResetQuality();
        }
        public void Analyze(GCode code)
        {
            estimatedCommandTime = 1000;
            if (code.hostCommand)
            {
                string cmd = code.getHostCommand();
                if (cmd.Equals("@hide"))
                    drawing = false;
                else if (cmd.Equals("@show"))
                    drawing = true;
                else if (cmd.Equals("@isathome"))
                {
                    hasXHome = hasYHome = hasZHome = true;
                    x = Main.printerSettings.XHomePos;
                    y = Main.printerSettings.YHomePos;
                    z = Main.printerSettings.ZHomePos;
                    if (FormPrinterSettings.ps.printerType != 3)
                        xOffset = yOffset = zOffset = 0;
                }
                return;
            }
            //if (code.forceAscii) return; // Don't analyse host commands and unknown commands
            if (code.hasN)
                lastline = code.N;
            if (uploading && !code.hasM && code.M != 29) return; // ignore upload commands
            if (code.hasG)
            {
                switch (code.G)
                {
                    case 0:
                    case 1:
                        eChanged = false;
                        if (code.hasF) f = code.F;
                        if (relative)
                        {
                            if (code.hasX) x += code.X;
                            if (code.hasY) y += code.Y;
                            if (code.hasZ) z += code.Z;
                            if (code.hasE)
                            {
                                if (eChanged = code.E != 0)
                                {
                                    if (code.E < 0) activeExtruder.retracted = true;
                                    else if (activeExtruder.retracted)
                                    {
                                        activeExtruder.retracted = false;
                                        activeExtruder.e = activeExtruder.emax;
                                    }
                                    else
                                        activeExtruder.e += code.E;
                                }
                            }
                        }
                        else
                        {
                            if (code.hasX) x = xOffset + code.X;
                            if (code.hasY) y = yOffset + code.Y;
                            if (code.hasZ)
                            {
                                z = zOffset + code.Z;
                            }
                            if (code.hasE)
                            {
                                if (eRelative)
                                {
                                    if (eChanged = code.E != 0)
                                    {
                                        if (code.E < 0) activeExtruder.retracted = true;
                                        else if (activeExtruder.retracted)
                                        {
                                            activeExtruder.retracted = false;
                                            activeExtruder.e = activeExtruder.emax;
                                        }
                                        else
                                            activeExtruder.e += code.E;
                                    }
                                }
                                else
                                {
                                    if (eChanged = activeExtruder.e != (activeExtruder.eOffset + code.E))
                                    {
                                        activeExtruder.e = activeExtruder.eOffset + code.E;
                                        if (activeExtruder.e < activeExtruder.lastE)
                                            activeExtruder.retracted = true;
                                        else if (activeExtruder.retracted)
                                        {
                                            activeExtruder.retracted = false;
                                            activeExtruder.e = activeExtruder.emax;
                                            activeExtruder.eOffset = activeExtruder.e - code.E;
                                        }
                                    }
                                }
                            }
                        }
                        if (x < Main.printerSettings.XMin) { x = Main.printerSettings.XMin; hasXHome = false; }
                        if (y < Main.printerSettings.YMin) { y = Main.printerSettings.YMin; hasYHome = false; }
                        if (z < 0 && FormPrinterSettings.ps.printerType!=3) { z = 0; hasZHome = false; }
                        if (x > Main.printerSettings.XMax) { hasXHome = false; }
                        if (y > Main.printerSettings.YMax) { hasYHome = false; }
                        if (z > printerHeight) { hasZHome = false; }
                        if (activeExtruder.e > activeExtruder.emax)
                        {
                            activeExtruder.emax = activeExtruder.e;
                            if (z != lastZPrint)
                            {
                                layer++;
                                lastZPrint = z;
                                if (!privateAnalyzer && Main.conn.connector.IsJobRunning() && Main.conn.connector.MaxLayer >= 0)
                                {
                                    //PrinterConnection.logInfo("Printing layer " + layer.ToString() + " of " + Main.conn.job.maxLayer.ToString());
                                    PrinterConnection.logInfo(Trans.T2("L_PRINTING_LAYER_X_OF_Y", layer.ToString(), Main.conn.connector.MaxLayer.ToString()));
                                }
                            }
                        }
                        if (eventPosChanged != null)
                            if (privateAnalyzer)
                                eventPosChanged(code, x, y, z);
                            else
                                Main.main.Invoke(eventPosChanged, code, x, y, z);
                        float dx = Math.Abs(x - lastX);
                        float dy = Math.Abs(y - lastY);
                        float dz = Math.Abs(z - lastZ);
                        float de = Math.Abs(activeExtruder.e - activeExtruder.lastE);
                        double time;
                        if (dx + dy + dz > 0.001)
                            time = Math.Sqrt(dx * dx + dy * dy + dz * dz) * 60.0f / f;
                        else 
                            time = de * 60.0f / f;
                        printingTime += time;
                        estimatedCommandTime = (long)(1100 * time);
                        if (z != lastZ) unchangedLayer.Clear();
                        lastX = x;
                        lastY = y;
                        lastZ = z;
                        activeExtruder.lastE = activeExtruder.e;
                        break;
                    case 2:
                    case 3:
                        {
                            isG1Move = true;
                            eChanged = false;
                            if (code.hasF) f = code.F;
                            if (relative)
                            {
                                if (code.hasX)
                                {
                                    x += code.X;
                                }
                                if (code.hasY)
                                {
                                    y += code.Y;
                                }
                                if (code.hasZ)
                                {
                                    z += code.Z;
                                }
                                if (code.hasE)
                                {
                                    if (eChanged = code.E != 0)
                                    {
                                        if (code.E < 0) activeExtruder.retracted = true;
                                        else if (activeExtruder.retracted)
                                        {
                                            activeExtruder.retracted = false;
                                            activeExtruder.e = activeExtruder.emax;
                                        }
                                        else
                                            activeExtruder.e += code.E;
                                    }
                                }
                            }
                            else
                            {
                                if (code.hasX)
                                {
                                    x = xOffset + code.X;
                                }
                                if (code.hasY)
                                {
                                    y = yOffset + code.Y;
                                }
                                if (code.hasZ)
                                {
                                    z = zOffset + code.Z;
                                    //if (z < 0) { z = 0; hasZHome = NO; }
                                    //if (z > printerHeight) { hasZHome = NO; }
                                }
                                if (code.hasE )
                                {
                                    if (eRelative)
                                    {
                                        if (eChanged = code.E != 0)
                                        {
                                            if (code.E < 0) activeExtruder.retracted = true;
                                            else if (activeExtruder.retracted)
                                            {
                                                activeExtruder.retracted = false;
                                                activeExtruder.e = activeExtruder.emax;
                                            }
                                            else
                                                activeExtruder.e += code.E;
                                        }
                                    }
                                    else
                                    {
                                        if (eChanged = activeExtruder.e != (activeExtruder.eOffset + code.E))
                                        {
                                            activeExtruder.e = activeExtruder.eOffset + code.E;
                                            if (activeExtruder.e < activeExtruder.lastE)
                                                activeExtruder.retracted = true;
                                            else if (activeExtruder.retracted)
                                            {
                                                activeExtruder.retracted = false;
                                                activeExtruder.e = activeExtruder.emax;
                                                activeExtruder.eOffset = activeExtruder.e - code.E;
                                            }
                                        }
                                    }
                                }
                            }

                            float[] offset = new float[] { code.I, code.J};
                            /* if(unit_inches) {
                               offset[0]*=25.4;
                               offset[1]*=25.4;
                             }*/
                            float[] position = new float[] { lastX, lastY };
                            float[] target = new float[] { x, y };
                            float r = code.R;
                            if (r > 0)
                            {
                                /* 
                                  We need to calculate the center of the circle that has the designated radius and passes
                                  through both the current position and the target position. This method calculates the following
                                  set of equations where [x,y] is the vector from current to target position, d == magnitude of 
                                  that vector, h == hypotenuse of the triangle formed by the radius of the circle, the distance to
                                  the center of the travel vector. A vector perpendicular to the travel vector [-y,x] is scaled to the 
                                  length of h [-y/d*h, x/d*h] and added to the center of the travel vector [x/2,y/2] to form the new point 
                                  [i,j] at [x/2-y/d*h, y/2+x/d*h] which will be the center of our arc.
          
                                  d^2 == x^2 + y^2
                                  h^2 == r^2 - (d/2)^2
                                  i == x/2 - y/d*h
                                  j == y/2 + x/d*h
          
                                                                                       O <- [i,j]
                                                                                    -  |
                                                                          r      -     |
                                                                              -        |
                                                                           -           | h
                                                                        -              |
                                                          [0,0] ->  C -----------------+--------------- T  <- [x,y]
                                                                    | <------ d/2 ---->|
                    
                                  C - Current position
                                  T - Target position
                                  O - center of circle that pass through both C and T
                                  d - distance from C to T
                                  r - designated radius
                                  h - distance from center of CT to O
          
                                  Expanding the equations:

                                  d -> sqrt(x^2 + y^2)
                                  h -> sqrt(4 * r^2 - x^2 - y^2)/2
                                  i -> (x - (y * sqrt(4 * r^2 - x^2 - y^2)) / sqrt(x^2 + y^2)) / 2 
                                  j -> (y + (x * sqrt(4 * r^2 - x^2 - y^2)) / sqrt(x^2 + y^2)) / 2
         
                                  Which can be written:
          
                                  i -> (x - (y * sqrt(4 * r^2 - x^2 - y^2))/sqrt(x^2 + y^2))/2
                                  j -> (y + (x * sqrt(4 * r^2 - x^2 - y^2))/sqrt(x^2 + y^2))/2
          
                                  Which we for size and speed reasons optimize to:

                                  h_x2_div_d = sqrt(4 * r^2 - x^2 - y^2)/sqrt(x^2 + y^2)
                                  i = (x - (y * h_x2_div_d))/2
                                  j = (y + (x * h_x2_div_d))/2
          
                                */
                                //if(unit_inches) r*=25.4;
                                // Calculate the change in position along each selected axis
                                float cx = target[0] - position[0];
                                float cy = target[1] - position[1];

                                float h_x2_div_d = -(float)Math.Sqrt(4 * r * r - cx * cx - cy * cy) / (float)Math.Sqrt(cx * cx + cy * cy); // == -(h * 2 / d)
                                // If r is smaller than d, the arc is now traversing the complex plane beyond the reach of any
                                // real CNC, and thus - for practical reasons - we will terminate promptly:
                                // if(isnan(h_x2_div_d)) { OUT_P_LN("error: Invalid arc"); break; }
                                // Invert the sign of h_x2_div_d if the circle is counter clockwise (see sketch below)
                                if (code.G == 3) { h_x2_div_d = -h_x2_div_d; }

                                /* The counter clockwise circle lies to the left of the target direction. When offset is positive,
                                   the left hand circle will be generated - when it is negative the right hand circle is generated.
           
           
                                                                                 T  <-- Target position
                                                         
                                                                                 ^ 
                                      Clockwise circles with this center         |          Clockwise circles with this center will have
                                      will have > 180 deg of angular travel      |          < 180 deg of angular travel, which is a good thing!
                                                                       \         |          /   
                          center of arc when h_x2_div_d is positive ->  x <----- | -----> x <- center of arc when h_x2_div_d is negative
                                                                                 |
                                                                                 |
                                                         
                                                                                 C  <-- Current position                                 */


                                // Negative R is g-code-alese for "I want a circle with more than 180 degrees of travel" (go figure!), 
                                // even though it is advised against ever generating such circles in a single line of g-code. By 
                                // inverting the sign of h_x2_div_d the center of the circles is placed on the opposite side of the line of
                                // travel and thus we get the unadvisably long arcs as prescribed.
                                if (r < 0)
                                {
                                    h_x2_div_d = -h_x2_div_d;
                                    r = -r; // Finished with r. Set to positive for mc_arc
                                }
                                // Complete the operation by calculating the actual center of the arc
                                offset[0] = 0.5f * (cx - (cy * h_x2_div_d));
                                offset[1] = 0.5f * (cy + (cx * h_x2_div_d));

                            }
                            else
                            { // Offset mode specific computations
                                r = (float)Math.Sqrt(offset[0] * offset[0] + offset[1] * offset[1]); // Compute arc radius for mc_arc
                            }

                            // Set clockwise/counter-clockwise sign for mc_arc computations
                            bool isclockwise = code.G == 2;

                            // Trace the arc
                            arc(position, target, offset, r, isclockwise,code);
                            lastX = x;
                            lastY = y;
                            lastZ = z;
                            activeExtruder.lastE = activeExtruder.e;
                            if (x < Main.printerSettings.XMin) { x = Main.printerSettings.XMin; hasXHome = false; }
                            if (y < Main.printerSettings.YMin) { y = Main.printerSettings.YMin; hasYHome = false; }
                            if (z < 0) { z = 0; hasZHome = false; }
                            if (x > Main.printerSettings.XMax) { hasXHome = false; }
                            if (y > Main.printerSettings.YMax) { hasYHome = false; }
                            if (z > printerHeight) { hasZHome = false; }
                            if (activeExtruder.e > activeExtruder.emax)
                            {
                                activeExtruder.emax = activeExtruder.e;
                                if (z > lastZPrint)
                                {
                                    lastZPrint = z;
                                    layer++;
                                }
                            }

                        }
                        break;
                    case 28:
                    case 161:
                        {
                            bool homeAll = !(code.hasX || code.hasY || code.hasZ);
                            if (code.hasX || homeAll) { xOffset = 0; x = Main.printerSettings.XHomePos; hasXHome = true; }
                            if (code.hasY || homeAll) { yOffset = 0; y = Main.printerSettings.YHomePos; hasYHome = true; }
                            if (code.hasZ || homeAll) { zOffset = 0; z = Main.printerSettings.ZHomePos; hasZHome = true; }
                            if (code.hasE) { activeExtruder.eOffset = 0; activeExtruder.e = 0; activeExtruder.emax = 0; }
                            if (eventPosChanged != null)
                                if (privateAnalyzer)
                                    eventPosChanged(code, x, y, z);
                                else
                                    Main.main.Invoke(eventPosChanged, code, x, y, z);
                            estimatedCommandTime = 60000;
                        }
                        break;
                    case 162:
                        {
                            bool homeAll = !(code.hasX || code.hasY || code.hasZ);
                            if (code.hasX || homeAll) { xOffset = 0; x = Main.printerSettings.XMax; hasXHome = true; }
                            if (code.hasY || homeAll) { yOffset = 0; y = Main.printerSettings.YMax; hasYHome = true; }
                            if (code.hasZ || homeAll) { zOffset = 0; z = Main.printerSettings.PrintAreaHeight; hasZHome = true; }
                            if (eventPosChanged != null)
                                if (privateAnalyzer)
                                    eventPosChanged(code, x, y, z);
                                else
                                    Main.main.Invoke(eventPosChanged, code, x, y, z);
                            estimatedCommandTime = 60000;
                        }
                        break;
                    case 90:
                        relative = false;
                        break;
                    case 91:
                        relative = true;
                        break;
                    case 92:
                        if (FormPrinterSettings.ps.printerType != 3)
                        {
                            if (code.hasX) { float old = xOffset + x; xOffset += code.X; x = old - xOffset; }
                            if (code.hasY) { float old = zOffset + y; yOffset += code.Y; y = old - yOffset; }
                            if (code.hasZ) { float old = zOffset + z; zOffset += code.Z; z = old - zOffset; }
                        }
                        else
                        {
                            if (code.hasX) { xOffset = code.X; x = 0; }
                            if (code.hasY) { yOffset = code.Y; y = 0; }
                            if (code.hasZ) { zOffset = code.Z; z = 0; }
                        }
                        if (code.hasE) { activeExtruder.eOffset = activeExtruder.e - code.E; activeExtruder.lastE = activeExtruder.e = activeExtruder.eOffset; }
                        if (eventPosChanged != null)
                            if (privateAnalyzer)
                                eventPosChanged(code, x, y, z);
                            else
                                Main.main.Invoke(eventPosChanged, code, x, y, z);
                        break;
                    default:
                        estimatedCommandTime = 5 * 60 * 1000;
                        break;
                }
            }
            else if (code.hasM)
            {
                switch (code.M)
                {
                    case 28:
                        uploading = true;
                        break;
                    case 29:
                        uploading = false;
                        break;
                    case 80:
                        powerOn = true;
                        fireChanged();
                        break;
                    case 81:
                        powerOn = false;
                        fireChanged();
                        break;
                    case 82:
                        eRelative = false;
                        break;
                    case 83:
                        eRelative = true;
                        break;
                    case 104:
                    case 109:
                        {
                            int idx = activeExtruderId;
                            if (code.hasT) idx = code.T;
                            if (code.hasS) setTemperature(idx, code.S);
                            if (code.M == 109)
                                estimatedCommandTime = 6*60*1000;
                        }
                        fireChanged();
                        break;
                    case 106:
                        fanOn = true;
                        if (code.hasS) fanVoltage = code.S;
                        fireChanged();
                        break;
                    case 107:
                        fanOn = false;
                        fireChanged();
                        break;
                    case 110:
                        lastline = code.N;
                        break;
                    case 111:
                        if (code.hasS)
                        {
                            debugLevel = code.S;
                        }
                        break;
                    case 140:
                    case 190:
                        if (code.hasS) bedTemp = code.S;
                        if (code.M == 190)
                            estimatedCommandTime = 20*60*1000;
                        fireChanged();
                        break;
                    case 203: // Temp monitor
                        if (code.hasS)
                            tempMonitor = code.S;
                        break;
                    case 220:
                        if (code.hasS)
                            speedMultiply = code.S;
                        break;
                    case 108: // Catch fast commands to not get slowed down for beeing unknown
                    case 3:
                    case 4:
                    case 5:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 30:
                    case 42:
                    case 92:
                    case 101:
                    case 102:
                    case 103:
                    case 105:
                    case 201:
                    case 202:
                    case 204:
                    case 205:
                    case 206:
                    case 221:
                    case 300:
                    case 340:
                    case 350:
                    case 400:
                    case 401:
                    case 402:
                    case 500:
                    case 501:
                    case 502:
                    case 666:
                    case 908:
                        break;
                    case 116:
                        estimatedCommandTime = 20 * 60 * 1000;
                        break;
                    case 303:
                        estimatedCommandTime = 30 * 60 * 1000;
                        break;
                    default:
                        estimatedCommandTime = 5 * 60 * 1000;
                        break;
                }
            }
            else if (code.hasT)
            {
                activeExtruderId = code.T;
                if (!extruder.ContainsKey(activeExtruderId))
                    extruder.Add(activeExtruderId, new ExtruderData(activeExtruderId));
                activeExtruder = extruder[activeExtruderId];
                fireChanged();
            }
        }
        private void arc(float[] position, float[] target, float[] offset, float radius, bool isclockwise,GCode code)
        {
            //   int acceleration_manager_was_enabled = plan_is_acceleration_manager_enabled();
            //   plan_set_acceleration_manager_enabled(false); // disable acceleration management for the duration of the arc
            float center_axis0 = position[0] + offset[0];
            float center_axis1 = position[1] + offset[1];
            //float linear_travel = 0; //target[axis_linear] - position[axis_linear];
            float r_axis0 = -offset[0];  // Radius vector from center to current location
            float r_axis1 = -offset[1];
            float rt_axis0 = target[0] - center_axis0;
            float rt_axis1 = target[1] - center_axis1;

            // CCW angle between position and target from circle center. Only one atan2() trig computation required.
            float angular_travel = (float)Math.Atan2(r_axis0 * rt_axis1 - r_axis1 * rt_axis0, r_axis0 * rt_axis0 + r_axis1 * rt_axis1);
            if (angular_travel < 0) { angular_travel += 2 * (float)Math.PI; }
            if (isclockwise) { angular_travel -= 2 * (float)Math.PI; }

            float millimeters_of_travel = Math.Abs(angular_travel) * radius; //hypot(angular_travel*radius, fabs(linear_travel));
            if (millimeters_of_travel < 0.001) { return; }
            double time = millimeters_of_travel * 60.0f / f;
            printingTime += time;
            estimatedCommandTime = (long)(1000 * time);
            if (eventPosChangedFast == null) return;
            //uint16_t segments = (radius>=BIG_ARC_RADIUS ? floor(millimeters_of_travel/MM_PER_ARC_SEGMENT_BIG) : floor(millimeters_of_travel/MM_PER_ARC_SEGMENT));
            // Increase segment size if printing faster then computation speed allows
            int segments = (int)Math.Min(millimeters_of_travel,millimeters_of_travel*10/radius);
            if (segments > 32) segments = 32;
            if (segments == 0) segments = 1;
            /*  
              // Multiply inverse feed_rate to compensate for the fact that this movement is approximated
              // by a number of discrete segments. The inverse feed_rate should be correct for the sum of 
              // all segments.
              if (invert_feed_rate) { feed_rate *= segments; }
            */
            float theta_per_segment = angular_travel / segments;
            //float linear_per_segment = linear_travel / segments;
            float extruder_per_segment = (activeExtruder.e - activeExtruder.lastE) / segments;
            float arc_target_e = activeExtruder.lastE;
            float sin_Ti;
            float cos_Ti;
            int i;
            
            for (i = 1; i < segments; i++)
            { // Increment (segments-1)
                // Arc correction to radius vector. Computed only every N_ARC_CORRECTION increments.
                // Compute exact location by applying transformation matrix from initial radius vector(=-offset).
                cos_Ti = (float)Math.Cos(i * theta_per_segment);
                sin_Ti = (float)Math.Sin(i * theta_per_segment);
                r_axis0 = -offset[0] * cos_Ti + offset[1] * sin_Ti;
                r_axis1 = -offset[0] * sin_Ti - offset[1] * cos_Ti;

                // Update arc_target location
                //arc_target[axis_linear] += linear_per_segment;
                arc_target_e += extruder_per_segment;
                if (arc_target_e > activeExtruder.emax)
                {
                    activeExtruder.emax = arc_target_e;
                    if (z > lastZPrint)
                    {
                        lastZPrint = z;
                        layer++;
                        if (code!=null)
                        {
                            if (!privateAnalyzer && Main.conn.connector.IsJobRunning() && Main.conn.connector.MaxLayer >= 0)
                            {
                                //PrinterConnection.logInfo("Printing layer " + layer.ToString() + " of " + Main.conn.job.maxLayer.ToString());
                                PrinterConnection.logInfo(Trans.T2("L_PRINTING_LAYER_X_OF_Y", layer.ToString(), Main.conn.connector.MaxLayer.ToString()));
                            }
                        }
                    }
                }
                if (code!=null)
                {
                    if (privateAnalyzer)
                        eventPosChanged(code, center_axis0 + r_axis0, center_axis1 + r_axis1, z);
                    else
                        Main.main.Invoke(eventPosChanged, code, center_axis0 + r_axis0, center_axis1 + r_axis1, z);
                } else
                eventPosChangedFast(center_axis0 + r_axis0, center_axis1 + r_axis1, z, arc_target_e);
            }
            // Ensure last segment arrives at target location.
            if (activeExtruder.e > activeExtruder.emax)
            {
                activeExtruder.emax = activeExtruder.e;
                if (z > lastZPrint)
                {
                    lastZPrint = z;
                    layer++;
                    if (code!=null)
                    {
                        if (!privateAnalyzer && Main.conn.connector.IsJobRunning() && Main.conn.connector.MaxLayer >= 0)
                        {
                            //PrinterConnection.logInfo("Printing layer " + layer.ToString() + " of " + Main.conn.job.maxLayer.ToString());
                            PrinterConnection.logInfo(Trans.T2("L_PRINTING_LAYER_X_OF_Y", layer.ToString(), Main.conn.connector.MaxLayer.ToString()));
                        }
                    }
                }
            }
            if (code!=null)
            {
                if (privateAnalyzer)
                    eventPosChanged(code, x, y, z);
                else
                    Main.main.Invoke(eventPosChanged, code, x, y, z);
            }
            else
                eventPosChangedFast(x, y, z, activeExtruder.e);

        }
        public void analyzeShort(GCodeShort code)
        {
            isG1Move = false;
            switch (code.compressedCommand)
            {
                case 1:
                    isG1Move = true;
                    eChanged = false;
                    if (code.hasF) f = code.f;
                    if (relative)
                    {
                        if (code.hasX)
                        {
                            x += code.x;
                            //if (x < 0) { x = 0; hasXHome = NO; }
                            //if (x > printerWidth) { hasXHome = NO; }
                        }
                        if (code.hasY)
                        {
                            y += code.y;
                            //if (y < 0) { y = 0; hasYHome = NO; }
                            //if (y > printerDepth) { hasYHome = NO; }
                        }
                        if (code.hasZ)
                        {
                            z += code.z;
                            //if (z < 0) { z = 0; hasZHome = NO; }
                            //if (z > printerHeight) { hasZHome = NO; }
                        }
                        if (code.hasE)
                        {
                            if (eChanged = code.e != 0)
                            {
                                if (code.e < 0) activeExtruder.retracted = true;
                                else if (activeExtruder.retracted)
                                {
                                    activeExtruder.retracted = false;
                                    activeExtruder.e = activeExtruder.emax;
                                } else
                                activeExtruder.e += code.e;
                                if (activeExtruder.e > activeExtruder.emax)
                                {
                                    activeExtruder.emax = activeExtruder.e;
                                    if (z > lastZPrint)
                                    {
                                        lastZPrint = z;
                                        layer++;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (code.x != -99999)
                        {
                            x = xOffset + code.x;
                            //if (x < 0) { x = 0; hasXHome = NO; }
                            //if (x > printerWidth) { hasXHome = NO; }
                        }
                        if (code.y != -99999)
                        {
                            y = yOffset + code.y;
                            //if (y < 0) { y = 0; hasYHome = NO; }
                            //if (y > printerDepth) { hasYHome = NO; }
                        }
                        if (code.z != -99999)
                        {
                            z = zOffset + code.z;
                            //if (z < 0) { z = 0; hasZHome = NO; }
                            //if (z > printerHeight) { hasZHome = NO; }
                        }
                        if (code.e != -99999)
                        {
                            if (eRelative)
                            {
                                if (eChanged = code.e != 0)
                                {
                                    if (code.e < 0) activeExtruder.retracted = true;
                                    else if (activeExtruder.retracted)
                                    {
                                        activeExtruder.retracted = false;
                                        activeExtruder.e = activeExtruder.emax;
                                    }
                                    else
                                        activeExtruder.e += code.e;
                                }
                            }
                            else
                            {
                                if (eChanged = activeExtruder.e != (activeExtruder.eOffset + code.e))
                                {
                                    activeExtruder.e = activeExtruder.eOffset + code.e;
                                    if (activeExtruder.e < activeExtruder.lastE)
                                        activeExtruder.retracted = true;
                                    else if (activeExtruder.retracted)
                                    {
                                        activeExtruder.retracted = false;
                                        activeExtruder.e = activeExtruder.emax;
                                        activeExtruder.eOffset = activeExtruder.e - code.e;
                                    }
                                }
                            }
                            if (activeExtruder.e > activeExtruder.emax)
                            {
                                activeExtruder.emax = activeExtruder.e;
                                if (z != lastZPrint)
                                {
                                    lastZPrint = z;
                                    layer++;
                                }
                            }
                        }
                    }
                    if (eventPosChangedFast != null)
                        eventPosChangedFast(x, y, z, activeExtruder.e);
                    float dx = Math.Abs(x - lastX);
                    float dy = Math.Abs(y - lastY);
                    float dz = Math.Abs(z - lastZ);
                    float de = Math.Abs(activeExtruder.e - activeExtruder.lastE);
                    if (dx + dy + dz > 0.001)
                    {
                        printingTime += Math.Sqrt(dx * dx + dy * dy + dz * dz) * 60.0f / f;
                    }
                    else printingTime += de * 60.0f / f;
                    lastX = x;
                    lastY = y;
                    lastZ = z;
                    activeExtruder.lastE = activeExtruder.e;
                    break;
                case 2:
                case 3:
                    {
                        isG1Move = true;
                        eChanged = false;
                        if (code.hasF) f = code.f;
                        if (relative)
                        {
                            if (code.hasX)
                            {
                                x += code.x;
                                //if (x < 0) { x = 0; hasXHome = NO; }
                                //if (x > printerWidth) { hasXHome = NO; }
                            }
                            if (code.hasY)
                            {
                                y += code.y;
                                //if (y < 0) { y = 0; hasYHome = NO; }
                                //if (y > printerDepth) { hasYHome = NO; }
                            }
                            if (code.hasZ)
                            {
                                z += code.z;
                                //if (z < 0) { z = 0; hasZHome = NO; }
                                //if (z > printerHeight) { hasZHome = NO; }
                            }
                            if (code.hasE)
                            {
                                if (eChanged = code.e != 0)
                                {
                                    if (code.e < 0) activeExtruder.retracted = true;
                                    else if (activeExtruder.retracted)
                                    {
                                        activeExtruder.retracted = false;
                                        activeExtruder.e = activeExtruder.emax;
                                    }
                                    else
                                        activeExtruder.e += code.e;
                                    if (activeExtruder.e > activeExtruder.emax)
                                    {
                                        activeExtruder.emax = activeExtruder.e;
                                        if (z > lastZPrint)
                                        {
                                            lastZPrint = z;
                                            layer++;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (code.x != -99999)
                            {
                                x = xOffset + code.x;
                                //if (x < 0) { x = 0; hasXHome = NO; }
                                //if (x > printerWidth) { hasXHome = NO; }
                            }
                            if (code.y != -99999)
                            {
                                y = yOffset + code.y;
                                //if (y < 0) { y = 0; hasYHome = NO; }
                                //if (y > printerDepth) { hasYHome = NO; }
                            }
                            if (code.z != -99999)
                            {
                                z = zOffset + code.z;
                                //if (z < 0) { z = 0; hasZHome = NO; }
                                //if (z > printerHeight) { hasZHome = NO; }
                            }
                            if (code.e != -99999)
                            {
                                if (eRelative)
                                {
                                    if (eChanged = code.e != 0)
                                    {
                                        if (code.e < 0) activeExtruder.retracted = true;
                                        else if (activeExtruder.retracted)
                                        {
                                            activeExtruder.retracted = false;
                                            activeExtruder.e = activeExtruder.emax;
                                        }
                                        else
                                            activeExtruder.e += code.e;
                                    }
                                }
                                else
                                {
                                    if (eChanged = activeExtruder.e != (activeExtruder.eOffset + code.e))
                                    {
                                        activeExtruder.e = activeExtruder.eOffset + code.e;
                                        if (activeExtruder.e < activeExtruder.lastE)
                                            activeExtruder.retracted = true;
                                        else if (activeExtruder.retracted)
                                        {
                                            activeExtruder.retracted = false;
                                            activeExtruder.e = activeExtruder.emax;
                                            activeExtruder.eOffset = activeExtruder.e - code.e;
                                        }
                                    }
                                }
                            }
                        }
                        float[] offset = new float[] { code.getValueFor("I", 0), code.getValueFor("J", 0) };
                        /* if(unit_inches) {
                           offset[0]*=25.4;
                           offset[1]*=25.4;
                         }*/
                        float[] position = new float[] { lastX, lastY };
                        float[] target = new float[] { x, y };
                        float r = code.getValueFor("R", -1000000);
                        if (r > 0)
                        {
                            /* 
                              We need to calculate the center of the circle that has the designated radius and passes
                              through both the current position and the target position. This method calculates the following
                              set of equations where [x,y] is the vector from current to target position, d == magnitude of 
                              that vector, h == hypotenuse of the triangle formed by the radius of the circle, the distance to
                              the center of the travel vector. A vector perpendicular to the travel vector [-y,x] is scaled to the 
                              length of h [-y/d*h, x/d*h] and added to the center of the travel vector [x/2,y/2] to form the new point 
                              [i,j] at [x/2-y/d*h, y/2+x/d*h] which will be the center of our arc.
          
                              d^2 == x^2 + y^2
                              h^2 == r^2 - (d/2)^2
                              i == x/2 - y/d*h
                              j == y/2 + x/d*h
          
                                                                                   O <- [i,j]
                                                                                -  |
                                                                      r      -     |
                                                                          -        |
                                                                       -           | h
                                                                    -              |
                                                      [0,0] ->  C -----------------+--------------- T  <- [x,y]
                                                                | <------ d/2 ---->|
                    
                              C - Current position
                              T - Target position
                              O - center of circle that pass through both C and T
                              d - distance from C to T
                              r - designated radius
                              h - distance from center of CT to O
          
                              Expanding the equations:

                              d -> sqrt(x^2 + y^2)
                              h -> sqrt(4 * r^2 - x^2 - y^2)/2
                              i -> (x - (y * sqrt(4 * r^2 - x^2 - y^2)) / sqrt(x^2 + y^2)) / 2 
                              j -> (y + (x * sqrt(4 * r^2 - x^2 - y^2)) / sqrt(x^2 + y^2)) / 2
         
                              Which can be written:
          
                              i -> (x - (y * sqrt(4 * r^2 - x^2 - y^2))/sqrt(x^2 + y^2))/2
                              j -> (y + (x * sqrt(4 * r^2 - x^2 - y^2))/sqrt(x^2 + y^2))/2
          
                              Which we for size and speed reasons optimize to:

                              h_x2_div_d = sqrt(4 * r^2 - x^2 - y^2)/sqrt(x^2 + y^2)
                              i = (x - (y * h_x2_div_d))/2
                              j = (y + (x * h_x2_div_d))/2
          
                            */
                            //if(unit_inches) r*=25.4;
                            // Calculate the change in position along each selected axis
                            float cx = target[0] - position[0];
                            float cy = target[1] - position[1];

                            float h_x2_div_d = -(float)Math.Sqrt(4 * r * r - cx * cx - cy * cy) / (float)Math.Sqrt(cx * cx + cy * cy); // == -(h * 2 / d)
                            // If r is smaller than d, the arc is now traversing the complex plane beyond the reach of any
                            // real CNC, and thus - for practical reasons - we will terminate promptly:
                            // if(isnan(h_x2_div_d)) { OUT_P_LN("error: Invalid arc"); break; }
                            // Invert the sign of h_x2_div_d if the circle is counter clockwise (see sketch below)
                            if (code.compressedCommand == 3) { h_x2_div_d = -h_x2_div_d; }

                            /* The counter clockwise circle lies to the left of the target direction. When offset is positive,
                               the left hand circle will be generated - when it is negative the right hand circle is generated.
           
           
                                                                             T  <-- Target position
                                                         
                                                                             ^ 
                                  Clockwise circles with this center         |          Clockwise circles with this center will have
                                  will have > 180 deg of angular travel      |          < 180 deg of angular travel, which is a good thing!
                                                                   \         |          /   
                      center of arc when h_x2_div_d is positive ->  x <----- | -----> x <- center of arc when h_x2_div_d is negative
                                                                             |
                                                                             |
                                                         
                                                                             C  <-- Current position                                 */


                            // Negative R is g-code-alese for "I want a circle with more than 180 degrees of travel" (go figure!), 
                            // even though it is advised against ever generating such circles in a single line of g-code. By 
                            // inverting the sign of h_x2_div_d the center of the circles is placed on the opposite side of the line of
                            // travel and thus we get the unadvisably long arcs as prescribed.
                            if (r < 0)
                            {
                                h_x2_div_d = -h_x2_div_d;
                                r = -r; // Finished with r. Set to positive for mc_arc
                            }
                            // Complete the operation by calculating the actual center of the arc
                            offset[0] = 0.5f * (cx - (cy * h_x2_div_d));
                            offset[1] = 0.5f * (cy + (cx * h_x2_div_d));

                        }
                        else
                        { // Offset mode specific computations
                            r = (float)Math.Sqrt(offset[0] * offset[0] + offset[1] * offset[1]); // Compute arc radius for mc_arc
                        }

                        // Set clockwise/counter-clockwise sign for mc_arc computations
                        bool isclockwise = code.compressedCommand == 2;

                        // Trace the arc
                        arc(position, target, offset, r, isclockwise,null);
                        lastX = x;
                        lastY = y;
                        lastZ = z;
                        activeExtruder.lastE = activeExtruder.e;
                        if (activeExtruder.e > activeExtruder.emax)
                        {
                            activeExtruder.emax = activeExtruder.e;
                            if (z > lastZPrint)
                            {
                                lastZPrint = z;
                                layer++;
                            }
                        }

                    }
                    break;
                case 4:
                    {
                        bool homeAll = !(code.hasX || code.hasY || code.hasZ);
                        if (code.hasX || homeAll) { xOffset = 0; x = Main.printerSettings.XHomePos; hasXHome = true; }
                        if (code.hasY || homeAll) { yOffset = 0; y = Main.printerSettings.YHomePos; hasYHome = true; }
                        if (code.hasZ || homeAll) { zOffset = 0; z = Main.printerSettings.ZHomePos; hasZHome = true; }
                        if (code.hasE) { activeExtruder.eOffset = 0; activeExtruder.e = 0; activeExtruder.emax = 0; }
                        // [delegate positionChangedFastX:x y:y z:z e:e];
                    }
                    break;
                case 5:
                    {
                        bool homeAll = !(code.hasX || code.hasY || code.hasZ);
                        if (code.hasX || homeAll) { xOffset = 0; x = Main.printerSettings.XMax; hasXHome = true; }
                        if (code.hasY || homeAll) { yOffset = 0; y = Main.printerSettings.YMax; hasYHome = true; }
                        if (code.hasZ || homeAll) { zOffset = 0; z = Main.printerSettings.PrintAreaHeight; hasZHome = true; }
                        //[delegate positionChangedFastX:x y:y z:z e:e];
                    }
                    break;
                case 6:
                    relative = false;
                    break;
                case 7:
                    relative = true;
                    break;
                case 8:
                    if (FormPrinterSettings.ps.printerType != 3)
                    {
                        if (code.hasX) { float old = xOffset + x; xOffset += code.x; x = old - xOffset; }
                        if (code.hasY) { float old = zOffset + y; yOffset += code.y; y = old - yOffset; }
                        if (code.hasZ) { float old = zOffset + z; zOffset += code.z; z = old - zOffset; }
                    }
                    else
                    {
                        if (code.hasX) { xOffset = code.x; x = 0; }
                        if (code.hasY) { yOffset = code.y; y = 0; }
                        if (code.hasZ) { zOffset = code.z; z = 0; }
                    }
                    if (code.hasE) { activeExtruder.eOffset = activeExtruder.e - code.e; activeExtruder.lastE = activeExtruder.e = activeExtruder.eOffset; }
                    break;
                case 12: // Host command
                    {
                        string hc = code.text.Trim();
                        if (hc == "@hide")
                            drawing = false;
                        else if (hc == "@show")
                            drawing = true;
                        else if (hc == "@isathome")
                        {
                            hasXHome = hasYHome = hasZHome = true;
                            x = xOffset = Main.printerSettings.XHomePos;
                            y = yOffset = Main.printerSettings.YHomePos;
                            z = zOffset = Main.printerSettings.ZHomePos;
                        }
                    }
                    break;
                case 9:
                    eRelative = false;
                    break;
                case 10:
                    eRelative = true;
                    break;
                case 11:
                    activeExtruderId = code.tool;
                    if (!extruder.ContainsKey(activeExtruderId))
                        extruder.Add(activeExtruderId, new ExtruderData(activeExtruderId));
                    activeExtruder = extruder[activeExtruderId];
                    break;
            }
            if (layer != lastlayer)
            {
                foreach (GCodeShort c in unchangedLayer)
                {
                    c.layer = layer;
                }
                unchangedLayer.Clear();
                layerZ = z;
                lastlayer = layer;
            }
            else if (z != layerZ)
                unchangedLayer.AddLast(code);
            code.layer = layer;
            code.tool = activeExtruderId;
            code.emax = totalFilamentUsed();
        }
        public float totalFilamentUsed()
        {
            float sum = 0;
            foreach (ExtruderData ed in extruder.Values)
                sum += ed.emax;
            return sum;
        }
    }
}
