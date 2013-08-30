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
using System.IO;

namespace RepetierHost.model
{
    public class SkeinConfig
    {
        string[] lines;
        string[] orig;
        string path;
        bool exists;
        public SkeinConfig(string _path)
        {
            path = _path;
            exists = File.Exists(path);
            if (!exists) return;
            lines = File.ReadAllLines(path);
            orig = (string[])lines.Clone();
        }
        public void writeModified()
        {
            if (!exists) return;
            File.WriteAllLines(path, lines);
        }
        public void writeOriginal()
        {
            if (!exists) return;
            File.WriteAllLines(path, orig);
        }
        private int lineForKey(string key)
        {
            key += "\t";
            for (int i = 0; i < lines.Count(); i++)
            {
                if(lines[i].StartsWith(key)) return i;
            }
            return -1;
        }
        public string getValue(string key)
        {
            if (!exists) return null;
            int idx = lineForKey(key);
            if (idx < 0) return null;
            return lines[idx].Substring(key.Length + 1);
        }
        public void setValue(string key, string val)
        {
            if (!exists) return;
            int idx = lineForKey(key);
            if (idx < 0) return;
            lines[idx] = key + "\t" + val;
        }
    }
}
