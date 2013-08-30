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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace RepetierHost.model
{
    /// <summary>
    /// This is a compressed version of GCode to reduce the space needed.
    /// It is only used to store a compact version for the printjob in ram.
    /// </summary>
    public class GCodeCompressed
    {
        static public System.Text.UTF8Encoding enc = new UTF8Encoding();
        byte[] data;
        public GCodeCompressed(GCode c)
        {
            int p = c.orig.IndexOf(';');
            string tmp = (p >= 0 ? c.orig.Substring(0, p) : c.orig).Trim();
            data = enc.GetBytes(tmp);
        }
        public GCodeCompressed(string c)
        {
            int p = c.IndexOf(';');
            string tmp = (p >= 0 ? c.Substring(0, p - 1) : c);
            data = enc.GetBytes(tmp);
        }
        public GCode getCode()
        {
            return new GCode(enc.GetString(data));
        }
        public string getCommand()
        {
            return enc.GetString(data);
        }
    }
    /// <summary>
    /// Stores the complete data of a gcode command in an easy 
    /// accessible data structure. This structure can be converted
    /// into a binary or ascii representation to be send to a
    /// reprap printer.
    /// </summary>
    public class GCode
    {
        public static NumberFormatInfo format = CultureInfo.InvariantCulture.NumberFormat;
        public static string floatNoExp = "0.#####";
        public bool forceAscii = false; // true if unpaseable content is found
        public bool hostCommand = false; // True if it contains a host command to be executed
        private ushort fields = 128;
        private ushort fields2 = 0;
        int n;
        public bool comment = false;
        private ushort g = 0, m = 0;
        private byte t = 0;
        private float x, y, z, e, f,ii,j,r;
        private int s;
        private int p;
        private String text = null;
        public String orig;

        public GCode(GCodeCompressed gc)
        {
            Parse(gc.getCommand());
        }
        public GCode(string s)
        {
            Parse(s);
        }
        public GCode()
        {
        }
        public bool hasCode { get { return fields != 128; } }
        public bool hasText { get { return (fields & 32768) != 0; } }
        public String Text
        {
            get { return text; }
            set { text = value; if (text.Length > 16) ActivateV2OrForceAscii(); fields |= 32768; }
        }
        public bool hasN { get { return (fields & 1) != 0; } }
        public int N
        {
            get { return n; }
            set { n = value; fields |= 1; }
        }
        public bool hasM {get { return (fields & 2) != 0; }}
        public ushort M
        {
            get { return m; }
            set { m = value; fields |= 2; }
        }
        public bool hasG { get { return (fields & 4) != 0; } }
        public ushort G
        {
            get { return g; }
            set { g = value; fields |= 4; }
        }
        public bool hasT { get { return (fields & 512) != 0; } }
        public byte T
        {
            get { return t; }
            set { t = value; fields |= 512; }
        }
        public bool hasS { get { return (fields & 1024) != 0; } }
        public int S
        {
            get { return s; }
            set { s = value; fields |= 1024; }
        }
        public bool hasP { get { return (fields & 2048) != 0; } }
        public int P
        {
            get { return p; }
            set { p = value; fields |= 2048; }
        }

        public bool hasX { get { return (fields & 8) != 0; } }
        public float X
        {
            get { return x; }
            set { x=value;fields|=8;}
        }
        public bool hasY { get { return (fields & 16) != 0; } }
        public float Y
        {
            get { return y; }
            set { y = value; fields |= 16; }
        }
        public bool hasZ { get { return (fields & 32) != 0; } }
        public float Z
        {
            get { return z; }
            set { z = value; fields |= 32; }
        }
        public bool hasE { get { return (fields & 64) != 0; } }
        public float E
        {
            get { return e; }
            set { e = value; fields |= 64; }
        }
        public bool hasF { get { return (fields & 256) != 0; } }
        public float F
        {
            get { return f; }
            set { f = value; fields |= 256; }
        }
        public bool hasI { get { return (fields2 & 1) != 0; } }
        public float I
        {
            get { return ii; }
            set { ii = value; fields2 |= 1; ActivateV2OrForceAscii(); }
        }
        public bool hasJ { get { return (fields2 & 2) != 0; } }
        public float J
        {
            get { return j; }
            set { j = value; fields2 |= 2; ActivateV2OrForceAscii(); }
        }
        public bool hasR { get { return (fields2 & 4) != 0; } }
        public float R
        {
            get { return r; }
            set { r = value; fields2 |= 4; ActivateV2OrForceAscii(); }
        }
        public bool isV2 { get { return (fields & 4096) != 0; } }
        /// <summary>
        /// Converts a gcode line into a binary representation.
        /// </summary>
        /// <param name="version">Binary protocol version</param>
        /// <returns></returns>
        public byte[] getBinary(int version)
        {
            bool v2 = isV2;
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms,Encoding.ASCII);
            bw.Write(fields);
            if (v2)
            {
                bw.Write(fields2);
                if(hasText)
                    bw.Write((byte)text.Length);
            }
            if (hasN) bw.Write((ushort)(n & 65535));
            if (v2)
            {
                if (hasM) bw.Write(m);
                if (hasG) bw.Write(g);
            }
            else
            {
                if (hasM) bw.Write((byte)m);
                if (hasG) bw.Write((byte)g);
            }
            if (hasX) bw.Write(x);
            if (hasY) bw.Write(y);
            if (hasZ) bw.Write(z);
            if (hasE) bw.Write(e);
            if (hasF) bw.Write(f);
            if (hasT) bw.Write(t);
            if (hasS) bw.Write(s);
            if (hasP) bw.Write(p);
            if (hasI) bw.Write(ii);
            if (hasJ) bw.Write(j);
            if (hasR) bw.Write(r);
            if (hasText)
            {
                int i, len = text.Length;
                if (v2)
                {
                    for (i = 0; i < len; i++)
                    {
                        bw.Write((byte)text[i]);
                    }
                }
                else
                {
                    if (len > 16) len = 16;
                    for (i = 0; i < len; i++)
                    {
                        bw.Write((byte)text[i]);
                    }
                    for (; i < 16; i++) bw.Write((byte)0);
                }
            }
            // compute fletcher-16 checksum
            int sum1 = 0, sum2 = 0;
            bw.Flush();
            ms.Flush();
            byte[] buf = ms.ToArray();
            foreach (byte c in buf)
            {
                sum1 = (sum1 + c) % 255;
                sum2 = (sum2 + sum1) % 255;
            }
            bw.Write((byte)sum1);
            bw.Write((byte)sum2);
            bw.Close();
            ms.Flush();
            return ms.ToArray();
        }
        public String getAscii(bool inclLine,bool inclChecksum)
        {
            if (hostCommand) return orig;
            StringBuilder s = new StringBuilder();
            /*
            if (hasM)
            {
                if (m == 117) inclChecksum = false; // For marlin
            }*/
            if (inclLine && hasN)
            {
                s.Append("N");
                s.Append(n);
                s.Append(" ");
            }
            if (forceAscii)
            {
                int pc = orig.IndexOf(';');
                if (pc < 0)
                    s.Append(orig);
                else
                    s.Append(orig.Substring(0, pc).Trim());
            }
            else
            {
                if (hasM)
                {
                    s.Append("M");
                    s.Append(m);
                }
                if (hasG)
                {
                    s.Append("G");
                    s.Append(g);
                }
                if (hasT)
                {
                    if (hasM) s.Append(" ");
                    s.Append("T");
                    s.Append(t);
                }
                if (hasX)
                {
                    s.Append(" X");
                    s.Append(x.ToString(floatNoExp,format));
                }
                if (hasY)
                {
                    s.Append(" Y");
                    s.Append(y.ToString(floatNoExp,format));
                }
                if (hasZ)
                {
                    s.Append(" Z");
                    s.Append(z.ToString(floatNoExp,format));
                }
                if (hasE)
                {
                    s.Append(" E");
                    s.Append(e.ToString(floatNoExp,format));
                }
                if (hasF)
                {
                    s.Append(" F");
                    s.Append(f.ToString(floatNoExp,format));
                }
                if (hasI)
                {
                    s.Append(" I");
                    s.Append(ii.ToString(floatNoExp,format));
                }
                if (hasJ)
                {
                    s.Append(" J");
                    s.Append(j.ToString(floatNoExp,format));
                }
                if (hasR)
                {
                    s.Append(" R");
                    s.Append(r.ToString(floatNoExp,format));
                }
                if (hasS)
                {
                    s.Append(" S");
                    s.Append(this.s);
                }
                if (hasP)
                {
                    s.Append(" P");
                    s.Append(p);
                }
                if (hasText)
                {
                    s.Append(" ");
                    s.Append(text);
                }
            }
            if (inclChecksum)
            {
                int check = 0;
                foreach (char ch in s.ToString()) check ^= (ch & 0xff);
                check ^= 32;
                s.Append(" *");
                s.Append(check);
            }
            return s.ToString();
        }
        private void ActivateV2OrForceAscii()
        {
            if (Main.conn.binaryVersion < 2)
            {
                forceAscii = true;
                return;
            }
            fields |= 4096;
        }
        private void AddCode(char c,string val) {
            double d;
            double.TryParse(val, NumberStyles.Float, format, out d);
            switch (c)
            {
                case 'G':
                    if (d > 255) ActivateV2OrForceAscii();
                    G = (ushort)d;
                    break;
                case 'M':
                    if (d > 255) ActivateV2OrForceAscii();
                    M = (ushort)d;
                    break;
                case 'N':
                    N = (int)d;
                    break;
                case 'T':
                    if (d > 255) forceAscii = true;
                    T = (byte)d;
                    break;
                case 'S':
                    S = (int)d;
                    break;
                case 'P':
                    P = (int)d;
                    break;
                case 'X':
                    X = (float)d;
                    break;
                case 'Y':
                    Y = (float)d;
                    break;
                case 'Z':
                    Z = (float)d;
                    break;
                case 'E':
                    E = (float)d;
                    break;
                case 'A':
                    E = (float)d;
                    forceAscii = true;
                    break;
                case 'F':
                    F = (float)d;
                    break;
                case 'I':
                    I = (float)d;
                    break;
                case 'J':
                    J = (float)d;
                    break;
                case 'R':
                    R = (float)d;
                    break;
                default:
                    forceAscii = true;
                    break;
            }
        }
        public string getHostCommand()
        {
            int p = orig.IndexOf(' ');
            if (p < 0) return orig;
            int off = (orig.StartsWith(";")?1:0);
            return orig.Substring(off, p-off);
        }
        public string getHostParameter()
        {
            int p = orig.IndexOf(' ');
            if (p < 0) return "";
            return orig.Substring(p+1);
        }
        public string Respace(string line)
        {
            char last = ' ';
            StringBuilder b = new StringBuilder();
            int i, l = line.Length;
            bool comment = false;
            for (i = 0; i < l; i++)
            {
                char c = line[i];
                if (c == ';') comment = true;
                if (char.IsLetter(c) && !comment)
                {
                    if (char.IsLower(c))
                        c = char.ToUpper(c);
                    //if (last != ' ' ) b.Append(' ');
                }
                b.Append(c);
                last = c;
            }
            return b.ToString();
        }
        public void Parse(String line)
        {
            hostCommand = false;
            orig = line.Trim();
            if (orig.StartsWith("@") || orig.StartsWith(";@"))
            {
                hostCommand = true;
                return;
            }            
            //orig = Respace(orig); // destroys M117 and sd card commands
            fields = 128;
            fields2 = 0;
            int l = orig.Length,i;
            int mode = 0; // 0 = search code, 1 = search value
            char code = ';';
            int p1=0;
            for (i = 0; i < l; i++)
            {
                char c = orig[i];
                if (mode == 0 && c >= 'a' && c <= 'z')
                {
                    c -= (char)32;
                    orig = orig.Substring(0,i)+c+orig.Substring(i+1);
                }
                if (mode == 0 && c >= 'A' && c <= 'Z')
                {
                    code = c;
                    mode = 1;
                    p1 = i + 1;
                    continue;
                }
                else if (mode == 1)
                {
                    if (c == ' ' || c=='\t' || c==';')
                    {
                        AddCode(code,orig.Substring(p1, i - p1));
                        mode = 0;
                        if (hasM && (m == 23 || m == 28 || m == 29 || m == 30 || m == 32|| m == 117))
                        {
                            int pos = i;
                            while (pos < orig.Length && char.IsWhiteSpace(orig[pos])) pos++;
                            int end = pos;
                            while (end < orig.Length && (m==117 || !char.IsWhiteSpace(orig[end]))) end++;
                            Text = orig.Substring(pos, end - pos);
                            if (Text.Length > 16) ActivateV2OrForceAscii();
                            break;
                        }
                    }
                }
                if (c == ';') break;
            }
            if (mode == 1)
                AddCode(code, orig.Substring(p1, orig.Length - p1));
            /* Slow version
            int iv;
            float fv;
            if (line.IndexOf(';') >= 0) line = line.Substring(0, line.IndexOf(';')); // Strip comments
            if (ExtractInt(line, "G", out iv)) G = (byte)iv;
            if (ExtractInt(line, "M", out iv)) M = (byte)iv;
            if (ExtractInt(line, "T", out iv)) T = (byte)iv;
            if (ExtractInt(line, "S", out iv)) S = iv;
            if (ExtractInt(line, "P", out iv)) P = iv;
            if (ExtractFloat(line, "X", out fv)) X = fv;
            if (ExtractFloat(line, "Y", out fv)) Y = fv;
            if (ExtractFloat(line, "Z", out fv)) Z = fv;
            if (ExtractFloat(line, "E", out fv)) E = fv;
            if (ExtractFloat(line, "F", out fv)) F = fv;
            if (hasM && (m == 23 || m == 28 || m == 29 || m == 30))
            {
                int pos = line.IndexOf('M') + 3;
                while (pos < line.Length && char.IsWhiteSpace(line[pos])) pos++;
                int end = pos;
                while (end < line.Length && !char.IsWhiteSpace(line[end])) end++;
                Text = line.Substring(pos, end - pos);
            }
              */
            comment = fields == 128;
        }
        private bool ExtractInt(string s,string code,out int value) {
            value = 0;
            int p = s.IndexOf(code);
            if (p < 0) return false;
            p++;
            int end = p;
            while(end<s.Length && ((end==p && (s[end]=='-' || s[end]=='+')) || char.IsDigit(s[end]))) end++;
            int.TryParse(s.Substring(p,end-p),out value);
            return true;
        }
        private bool ExtractFloat(string s, string code, out float value)
        {
            value = 0;
            int p = s.IndexOf(code);
            if (p < 0) return false;
            p++;
            int end = p;
            while (end < s.Length && !char.IsWhiteSpace(s[end])) end++;

            float.TryParse(s.Substring(p, end - p), NumberStyles.Float, format, out value);
            return true;
        }
        public override string ToString()
        {
            return getAscii(true,true);
        }
    }
}
