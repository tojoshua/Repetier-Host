using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;

namespace RepetierHost.model
{
    public class CommandParameter
    {
        public bool optional = false;
        public string parameter = "";
        public string description = "";

        public CommandParameter(XmlNode n)
        {
            description = n.InnerText;
            parameter = n.Attributes["name"].InnerText;
            string o = n.Attributes["optional"].InnerText;
            if (o.Equals("1")) optional = true;
        }
        public override string ToString()
        {
            if (optional)
                return "[" + parameter + "\\{{\\i " + description + "}\\}] ";
            return parameter + "\\{{\\i " + description + "}\\} ";
        }
    }
    public class CommandDescription
    {
        public string command;
        public string title;
        public LinkedList<CommandParameter> parameter = new LinkedList<CommandParameter>();
        public string description;
        public CommandDescription(XmlNode n)
        {
            try
            {
                command = n.Attributes["name"].InnerText;
                title = n.Attributes["title"].InnerText;
                foreach (XmlNode pn in n.ChildNodes)
                {
                    if (pn.NodeType != XmlNodeType.Element) continue;
                    if (pn.Name.Equals("Param"))
                        parameter.AddLast(new CommandParameter(pn));
                    if (pn.Name.Equals("Description"))
                        description = pn.InnerText;
                }
            }
            catch { }
        }
    }
    public class Commands
    {
        public Dictionary<string, CommandDescription> commands;
        public Commands()
        {
            commands = new Dictionary<string, CommandDescription>();
        }
        public void Read(string firmware, string lang)
        {
            RegistryKey repetierKey = Custom.BaseKey; // Registry.CurrentUser.CreateSubKey("SOFTWARE\\Repetier");

            string basedir = (string)repetierKey.GetValue("installPath");

            ReadFile(basedir + Path.DirectorySeparatorChar+"data"+Path.DirectorySeparatorChar+"default"+
                Path.DirectorySeparatorChar+"syntax_en.xml");
            if(lang.Equals("en")==false)
                ReadFile(basedir + Path.DirectorySeparatorChar+"data"+Path.DirectorySeparatorChar+"default"+
                    Path.DirectorySeparatorChar+"syntax_"+lang+".xml");
            if (firmware.Equals("default") == false)
            {
                ReadFile(basedir + Path.DirectorySeparatorChar+"data"+Path.DirectorySeparatorChar+firmware+
                    Path.DirectorySeparatorChar+"syntax_en.xml");
                if (lang.Equals("en") == false)
                    ReadFile(basedir + Path.DirectorySeparatorChar+"data"+Path.DirectorySeparatorChar+firmware+
                        Path.DirectorySeparatorChar+"syntax_" + lang + ".xml");
            }
        }
        private void ReadFile(string file)
        {
            if (!File.Exists(file)) return;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);
                foreach (XmlNode n in doc.GetElementsByTagName("Command"))
                {
                    CommandDescription cd = new CommandDescription(n);
                    commands[cd.command] = cd;
                }
            }
            catch { }
        }
    }
}
