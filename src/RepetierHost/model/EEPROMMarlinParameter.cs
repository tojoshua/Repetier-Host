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
  
   written by scuba 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace RepetierHost.model
{
    public delegate void OnEEPROMMarlinAdded(EEPROMMarlinStorage param);

    public class EEPROMMarlinStorage
    {
        public event OnEEPROMMarlinAdded eventAdded = null;
        public string sx="0";
        public string sy="0";
        public string sz="0";
        public string se="0";
        public string fx="0";
        public string fy="0";
        public string fz="0";
        public string fe="0";
        public string ax="0";
        public string ay="0";
        public string az="0";
        public string ae="0";
        public string acc="0";
        public string racc="0";
        public string avs="0";
        public string avt="0";
        public string avb="0";
        public string avx="0";
        public string avz="0";
        public string ppid="0";
        public string ipid="0";
        public string dpid="0";
        public string hox = "0";
        public string hoy = "0";
        public string hoz = "0";
        public bool hasPID = false;

        bool changed = false;

        public void update(string line)
        {
            // string[] lines = line.Substring(5).Split(' ');
            string[] test = line.Split(' ');
            string mode = "";
            foreach (string token in test)
            {
                if ((token != " ") && ((token == "M92") || (mode == "M92")))
                {
                    mode = "M92";
                    if (token[0] == 'X')
                    {
                        sx = token.Substring(1);
                    }
                    if (token[0] == 'Y')
                    {
                        sy = token.Substring(1);
                    }
                    if (token[0] == 'Z')
                    {
                        sz = token.Substring(1);
                    }
                    if (token[0] == 'E')
                    {
                        se = token.Substring(1);
                    }
                }
                if ((token != " ") && ((token == "M203") || (mode == "M203")))
                {
                    mode = "M203";
                    if (token[0] == 'X')
                    {
                        fx = token.Substring(1);
                    }
                    if (token[0] == 'Y')
                    {
                        fy = token.Substring(1);
                    }
                    if (token[0] == 'Z')
                    {
                        fz = token.Substring(1);
                    }
                    if (token[0] == 'E')
                    {
                        fe = token.Substring(1);
                    }
                }
                if ((token != " ") && ((token == "M201") || (mode == "M201")))
                {
                    mode = "M201";
                    if (token[0] == 'X')
                    {
                        ax = token.Substring(1);
                    }
                    if (token[0] == 'Y')
                    {
                        ay = token.Substring(1);
                    }
                    if (token[0] == 'Z')
                    {
                        az = token.Substring(1);
                    }
                    if (token[0] == 'E')
                    {
                        ae = token.Substring(1);
                    }
                }
                if ((token != " ") && ((token == "M204") || (mode == "M204")))
                {
                    mode = "M204";
                    if (token[0] == 'S')
                    {
                        acc = token.Substring(1);
                    }
                    if (token[0] == 'T')
                    {
                        racc = token.Substring(1);
                    }
                }
                if ((token != " ") && ((token == "M205") || (mode == "M205")))
                {
                    mode = "M205";
                    if (token[0] == 'S')
                    {
                        avs = token.Substring(1);
                    }
                    if (token[0] == 'T')
                    {
                        avt = token.Substring(1);

                    }
                    if (token[0] == 'B')
                    {
                        avb = token.Substring(1);

                    }
                    if (token[0] == 'X')
                    {
                        avx = token.Substring(1);

                    }
                    if (token[0] == 'Z')
                    {
                        avz = token.Substring(1);
                    }
                }
                if ((token != " ") && ((token == "M301") || (mode == "M301")))
                {
                    mode = "M301";
                    hasPID = true;
                    if (token[0] == 'P')
                    {
                        ppid = token.Substring(1);
                    }
                    if (token[0] == 'I')
                    {
                        ipid = token.Substring(1);

                    }
                    if (token[0] == 'D')
                    {
                        dpid = token.Substring(1);
                    }
                }
                if ((token != " ") && ((token == "M206") || (mode == "M206")))
                {
                    mode = "M206";
                    hasPID = true;
                    if (token[0] == 'X')
                    {
                        hox = token.Substring(1);
                    }
                    if (token[0] == 'Y')
                    {
                        hoy = token.Substring(1);

                    }
                    if (token[0] == 'Z')
                    {
                        hoz = token.Substring(1);
                    }
                }
            }
            changed = false;
        }
        public void Save()
        {
            if (!changed) return; // nothing changed
            string cmdsteps = "M92 X" + sx + " Y" + sy + " Z" + sz + " E" + se;
            string cmdfeed = "M203 X" + fx + " Y" + fy + " Z" + fz + " E" + fe;
            string cmdmacc = "M201 X" + ax + " Y" + ay + " Z" + az + " E" + ae;
            string cmdacc = "M204 S" + acc + " T" + racc;
            string cmdav = "M205 S" + avs + " T" + avt + " B" + avb + " X" + avx + " Z" + avz;
            string cmdho = "M206 X" + hox + " Y" + hoy + " Z" + hoz;
            string cmdpid = "M301 P" + ppid + " I" + ipid + " D" + dpid;
            Main.conn.injectManualCommand(cmdsteps);
            Main.conn.injectManualCommand(cmdfeed);
            Main.conn.injectManualCommand(cmdmacc);
            Main.conn.injectManualCommand(cmdacc);
            Main.conn.injectManualCommand(cmdav);
            Main.conn.injectManualCommand(cmdho);
            if(hasPID)
                Main.conn.injectManualCommand(cmdpid);
            changed = false;
        }
        public string SX
        {
            get { return sx; }
            set { if (sx.Equals(value)) return; sx = value; changed = true; }
        }
        public string SY
        {
            get { return sy; }
            set { if (sy.Equals(value)) return; sy = value; changed = true; }
        }
        public string SZ
        {
            get { return sz; }
            set { if (sz.Equals(value)) return; sz = value; changed = true; }
        }
        public string SE
        {
            get { return se; }
            set { if (se.Equals(value)) return; se = value; changed = true; }
        }
        public string FX
        {
            get { return fx; }
            set { if (fx.Equals(value)) return; fx = value; changed = true; }
        }
        public string FY
        {
            get { return fy; }
            set { if (fy.Equals(value)) return; fy = value; changed = true; }
        }
        public string FZ
        {
            get { return fz; }
            set { if (fz.Equals(value)) return; fz = value; changed = true; }
        }
        public string FE
        {
            get { return fe; }
            set { if (fe.Equals(value)) return; fe = value; changed = true; }
        }
        public string AX
        {
            get { return ax; }
            set { if (ax.Equals(value)) return; ax = value; changed = true; }
        }
        public string AY
        {
            get { return ay; }
            set { if (ay.Equals(value)) return; ay = value; changed = true; }
        }
        public string AZ
        {
            get { return az; }
            set { if (az.Equals(value)) return; az = value; changed = true; }
        }
        public string AE
        {
            get { return ae; }
            set { if (ae.Equals(value)) return; ae = value; changed = true; }
        }
        public string ACC
        {
            get { return acc; }
            set { if (acc.Equals(value)) return; acc = value; changed = true; }
        }
        public string RACC
        {
            get { return racc; }
            set { if (racc.Equals(value)) return; racc = value; changed = true; }
        }
        public string AVS
        {
            get { return avs; }
            set { if (avs.Equals(value)) return; avs = value; changed = true; }
        }
        public string AVT
        {
            get { return avt; }
            set { if (avt.Equals(value)) return; avt = value; changed = true; }
        }
        public string AVB
        {
            get { return avb; }
            set { if (avb.Equals(value)) return; avb = value; changed = true; }
        }
        public string AVX
        {
            get { return avx; }
            set { if (avx.Equals(value)) return; avx = value; changed = true; }
        }
        public string AVZ
        {
            get { return avz; }
            set { if (avz.Equals(value)) return; avz = value; changed = true; }
        }
        public string PPID
        {
            get { return ppid; }
            set { if (ppid.Equals(value)) return; ppid = value; changed = true; }
        }
        public string IPID
        {
            get { return ipid; }
            set { if (ipid.Equals(value)) return; ipid = value; changed = true; }
        }
        public string DPID
        {
            get { return dpid; }
            set { if (dpid.Equals(value)) return; dpid = value; changed = true; }
        }
        public string HOX
        {
            get { return hox; }
            set { if (hox.Equals(value)) return; hox = value; changed = true; }
        }
        public string HOY
        {
            get { return hoy; }
            set { if (hoy.Equals(value)) return; hoy = value; changed = true; }
        }
        public string HOZ
        {
            get { return hoz; }
            set { if (hoz.Equals(value)) return; hoz = value; changed = true; }
        }
        public void SaveToEEPROM()
        {
            Main.conn.injectManualCommand("M500");
        }
        public void retriev_factory_settings()
        {
            hasPID = false;
            Main.conn.injectManualCommand("M502");
        }
        public void Add(string line)
        {
            update(line);
            if (eventAdded != null)
                Main.main.Invoke(eventAdded, this);
        }
        public void Update()
        {
            hasPID = false;
            Main.conn.injectManualCommand("M503");
        }
    }
}
