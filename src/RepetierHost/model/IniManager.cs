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
    public class IniSection
    {
        public string name;
        public Dictionary<string, string> entries;
        public IniSection(string _name)
        {
            entries = new Dictionary<string, string>();
            name = _name;
        }
        public void addLine(string l)
        {
            int p = l.IndexOf('=');
            string name = l.Substring(0, p).Trim();
            if (!entries.ContainsKey(name))
                entries.Add(name, l);
        }
        public void merge(IniSection s)
        {
            foreach (String name in s.entries.Keys)
            {
                if (name == "extrusion_multiplier" || name == "filament_diameter" || name=="first_layer_temperature"
                    || name =="temperature")
                {
                    if (entries.ContainsKey(name))
                    {
                        string full = s.entries[name];
                        int p = full.IndexOf('=');
                        if (p >= 0)
                            full = full.Substring(p + 1).Trim();
                        entries[name] += "," + full;
                    }
                    else
                    {
                        entries.Add(name, s.entries[name]);
                    }
                }
            }
        }
    }
    public class IniFile
    {
        public string path = "";
        public Dictionary<string, IniSection> sections = new Dictionary<string, IniSection>();
        public void read(string _path)
        {
            if (_path != null)
                path = _path;

            IniSection actSect = null;
            actSect = new IniSection("");
            sections.Add("", actSect);
            if (!File.Exists(path)) return;
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            foreach (string line in lines)
            {
                string tl = line.Trim();
                if (tl.StartsWith("#")) continue; // comment
                if (tl.StartsWith("[") && tl.EndsWith("]"))
                {
                    string secname = tl.Substring(1, tl.Length - 2);
                    actSect = sections[secname];
                    if (actSect == null)
                    {
                        actSect = new IniSection(secname);
                        sections.Add(secname, actSect);
                    }
                    continue;
                }
                int p = tl.IndexOf('=');
                if (p < 0) continue;
                actSect.addLine(line);
            }
        }
        public void add(IniFile f)
        {
            foreach (IniSection s in f.sections.Values)
            {
                if (!sections.ContainsKey(s.name))
                {
                    sections.Add(s.name, new IniSection(s.name));
                }
                IniSection ms = sections[s.name];
                foreach (string ent in s.entries.Values)
                    ms.addLine(ent);
            }
        }
        /// <summary>
        /// Merges the values of both ini files by seperating values by a ,
        /// </summary>
        /// <param name="f"></param>
        public void merge(IniFile f)
        {
            foreach (IniSection s in f.sections.Values)
            {
                if (!sections.ContainsKey(s.name))
                {
                    sections.Add(s.name, new IniSection(s.name));
                }
                else
                {
                    sections[s.name].merge(s);
                }
                /*IniSection ms = sections[s.name];
                foreach (string ent in s.entries.Values)
                    ms.addLine(ent);*/
            }
        }
        public void flatten()
        {
            IniSection flat = sections[""];
            LinkedList<IniSection> dellist = new LinkedList<IniSection>();
            foreach (IniSection s in sections.Values)
            {
                if (s.name == "") continue;
                foreach (string line in s.entries.Values)
                    flat.addLine(line);
                dellist.AddLast(s);
            }
            foreach (IniSection s in dellist)
                sections.Remove(s.name);
        }
        public void write(string path)
        {
            LinkedList<string> lines = new LinkedList<string>();
            foreach (IniSection s in sections.Values)
            {
                if (s.name != "")
                    lines.AddLast("[" + s.name + "]");
                foreach (string line in s.entries.Values)
                    lines.AddLast(line);
            }
            File.WriteAllLines(path, lines.ToArray());
        }
    }
}
