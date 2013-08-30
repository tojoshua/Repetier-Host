using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using RepetierHost.view.utils;

namespace RepetierHost.model
{
    public class Translation
    {
        public string language;
        public string file;
        public string fileshort;
        public Dictionary<string, string> trans;

        public Translation(string _file,string _fileShort)
        {
            file = _file;
            fileshort = _fileShort;
            language = "unknown";
            trans = new Dictionary<string, string>();
            if (!File.Exists(file)) return;

            try
            {
                //Load the the document with the last book node.
                XmlTextReader reader = new XmlTextReader(file);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                XmlNodeList rootlist = doc.GetElementsByTagName("translation");
                foreach (XmlNode n in rootlist)
                {
                    XmlAttribute att = n.Attributes["language"];
                    if (att != null) language = att.InnerText;
                }
                foreach (XmlNode n in doc.GetElementsByTagName("t"))
                {
                    XmlAttribute att = n.Attributes["id"];
                    if (att == null) continue; // missing id!
                    string id = att.InnerText;
                    string value = n.InnerText.Trim();
                    if (trans.ContainsKey(id))
                        Console.WriteLine("Double id:" + id);
                    else
                        trans.Add(id, value);
                }
            }
            catch(Exception e) { 
                MessageBox.Show("Error reading translation "+_file+":\n"+e.ToString(),"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        public override string ToString()
        {
            return language;
        }
    }

    public class Trans
    {
        public Translation english = null;
        public Translation active = null;
        public SortedList<string,Translation> translations;
        static Trans trans = null;
        public Trans(string folder)
        {
            string[] lfiles = Directory.GetFiles(folder, "*.xml");
            CultureInfo info = CultureInfo.CurrentUICulture;
            string deflang = info.TwoLetterISOLanguageName;
            //Console.WriteLine("Default language:" + deflang);
            string lastactive = RegMemory.GetString("lastLanguage", deflang+".xml");
            translations = new SortedList<string, Translation>();
            foreach (string l in lfiles)
            {
                try
                {
                    //Console.WriteLine("Adding language " + l);
                    FileInfo f = new FileInfo(l);
                    string shortname = f.Name;
                    Translation t = new Translation(l, shortname);
                    if (shortname == "en.xml")
                        english = t;
                    if (shortname == lastactive)
                        active = t;
                    translations.Add(t.language, t);
                } catch {}
            }
            Trans.trans = this;
        }
        public void selectLanguage(Translation t)
        {
            active = t;
            RegMemory.SetString("lastLanguage", t.fileshort);
        }
        public static string T(string id)
        {
            string res = null;
            if (trans.active != null && trans.active.trans.ContainsKey(id))
                res = trans.active.trans[id];
            if (res != null) return res;
            if (trans.english != null && trans.english.trans.ContainsKey(id))
                res = trans.english.trans[id];
            if (res != null) return res;
            return id;
        }
        public static string T1(string id, string v1)
        {
            string res = T(id);
            res = res.Replace("$1", v1);
            return res;
        }
        public static string T2(string id, string v1,string v2)
        {
            string res = T(id);
            res = res.Replace("$1", v1);
            res = res.Replace("$2", v2);
            return res;
        }
    }
}
