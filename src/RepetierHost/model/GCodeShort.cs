using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace RepetierHost.model
{
    public class GCodeShort
    {
        public float x, y, z, e,f,emax;
        // Bit 0-19 : Layer 
        // Bit 20-23 : Tool
        // Bit 24-29 : Compressed command
        int flags;
        public string text;
        public GCodeShort(string cmd)
        {
            text = cmd;
            flags = 1048575 + (0 << 24);
            x = y = z = e = f = -99999;
            emax = -1;
            parse();
        }
        public int layer
        {
            get
            {
                return flags & 1048575;
            }
            set
            {
                flags = (flags & ~1048575) | value;
            }
        }
        public bool hasLayer
        {
            get
            {
                return (flags & 1048575) != 1048575;
            }
         }
        public int tool
        {
            get
            {
                return (flags >> 20) & 15;
            }
            set
            {
                value = value & 15;
                flags = (flags & ~(15 << 20)) | (value << 20);
            }
        }
        public int compressedCommand
        {
            set
            {
                flags = (flags & ~(63 << 24)) | (value << 24);
            }
            get
            {
                return (flags >> 24) & 63;
            }
        }
        public int Length
        {
            get { return text.Length; }
        }
        public bool hasX { get { return x != -99999; } }
        public bool hasY { get { return y != -99999; } }
        public bool hasZ { get { return z != -99999; } }
        public bool hasE { get { return e != -99999; } }
        public bool hasF { get { return f != -99999; } }
        public float getValueFor(string key, float def)
        {
            int p = text.IndexOf(key);
            if (p < 0) return def;
            p++;
            int e = text.IndexOf(' ', p);
            if(e<0) e = text.Length;
            float d = def;
            float.TryParse(text.Substring(p,e-p), NumberStyles.Float, GCode.format, out d);
            return d;
        }
        /**
        Command values:
         0 = unimportant command
         1 = G0/G1
         2 = G2
         3 = G3
         4 = G28 xzy = 1 => Set this
         5 = G162
         6 = G90 relative
         7 = G91 absolute
         8 = G92 x/y/z/e != -99999 if set
         9 = M82 eRelative
         10 = M83 eAbsolute
         11 = Txx Set Tool
         12 = Host command
         63 = unparsed
        */

        public bool addCode(char c, string val)
        {
            double d;
            double.TryParse(val, NumberStyles.Float, GCode.format, out d);
            switch (c)
            {
                case 'G':
                    {
                        int g = (int)d;
                        if (g > 0 && g < 4) compressedCommand = g;
                        else if (g >= 90 && g <= 92) compressedCommand = g - 84;
                        else if (g == 0) compressedCommand = 1;
                        else if (g == 28 || g == 161) compressedCommand = 4;
                        else if (g == 162) compressedCommand = 5;
                        return true;
                    }
                case 'M':
                    {
                        int m = (int)d;
                        if (m == 82) compressedCommand = 9;
                        if (m == 83) compressedCommand = 10;
                        return true;
                    }
                case 'T':
                    tool = (int)d;
                    compressedCommand = 11;
                    break;
                case 'X':
                    x = (float)d;
                    break;
                case 'Y':
                    y = (float)d;
                    break;
                case 'Z':
                    z = (float)d;
                    break;
                case 'E':
                case 'A':
                    e = (float)d;
                    break;
                case 'F':
                    f = (float)d;
                    break;
            }
            return false;
        }

        private void parse()
        {
            int l = text.Length, i;
            if (text.StartsWith(";@"))
            {
                compressedCommand = 12; // Host command
                return;
            }
            int mode = 0; // 0 = search code, 1 = search value
            char code = ';';
            int p1 = 0;
            for (i = 0; i < l; i++)
            {
                char c = text[i];
                if (i ==0 && c == '@')
                {
                    compressedCommand = 12; // Host command
                    return;
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
                    if (c == ' ' || c == '\t' || c == ';')
                    {
                        if (addCode(code, text.Substring(p1, i - p1)))
                        {
                            if (compressedCommand == 0) return; // Not interresting
                        }
                        mode = 0;
                    }
                }
                if (c == ';') break;
            }
            if (mode == 1)
            {
                addCode(code, text.Substring(p1, l - p1));
            }
        }
    }
}
