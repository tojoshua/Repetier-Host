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
using RepetierHost.view.utils;

namespace RepetierHost.model
{
    class BasicConfiguration
    {
        private string externalSlic3rPath="";
        private string externalSlic3rIniFile = "";
        private string slic3rConfigDir = "";
        private string slic3rPrintSettings = "";
        private string slic3rPrinterSettings = "";
        private string slic3rFilamentSettings = "";
        private string slic3rFilament2Settings = "";
        private string slic3rFilament3Settings = "";
        private string slic3rExecutable = "";
        private int slic3rVersionGroup = 1;
        private string skeinforgeProfile = "";
        private string skeinforgeProfileDir = "";
        private bool internalSlic3rUseBundledVersion = true;
        public static BasicConfiguration basicConf = new BasicConfiguration(); 
        public BasicConfiguration()
        {
            slic3rConfigDir = RegMemory.GetString("slic3rConfigDir", slic3rConfigDir);
            slic3rExecutable = RegMemory.GetString("slic3rExecutable", slic3rExecutable);
            slic3rPrintSettings = RegMemory.GetString("slic3rPrintSettings", slic3rPrintSettings);
            slic3rPrinterSettings = RegMemory.GetString("slic3rPrinterSettings", slic3rPrinterSettings);
            slic3rFilamentSettings = RegMemory.GetString("slic3rFilamentSettings", slic3rFilamentSettings);
            slic3rFilament2Settings = RegMemory.GetString("slic3rFilament2Settings", slic3rFilament2Settings);
            slic3rFilament3Settings = RegMemory.GetString("slic3rFilament3Settings", slic3rFilament3Settings);
            slic3rVersionGroup = RegMemory.GetInt("slic3rVersionGroup", slic3rVersionGroup);
            skeinforgeProfile = RegMemory.GetString("skeinforgeProfile", skeinforgeProfile);
            skeinforgeProfileDir = RegMemory.GetString("skeinforgeProfileDir", skeinforgeProfileDir);
            externalSlic3rPath = RegMemory.GetString("externalSlic3rPath", externalSlic3rPath);
            externalSlic3rIniFile = RegMemory.GetString("externalSlic3rIniFile", externalSlic3rIniFile);
            internalSlic3rUseBundledVersion = RegMemory.GetBool("internalSlic3rUseBundledVersion", internalSlic3rUseBundledVersion);
        }

        public string Slic3rExecutable
        {
            get { return slic3rExecutable; }
            set { slic3rExecutable = value; RegMemory.SetString("slic3rExecutable", slic3rExecutable); }
        }
        public string Slic3rConfigDir
        {
            get { return slic3rConfigDir; }
            set { slic3rConfigDir = value; RegMemory.SetString("slic3rConfigDir", slic3rConfigDir); }
        }
        public string Slic3rPrintSettings
        {
            get { return slic3rPrintSettings; }
            set { slic3rPrintSettings = value; RegMemory.SetString("slic3rPrintSettings", slic3rPrintSettings); }
        }
        public string Slic3rPrinterSettings
        {
            get { return slic3rPrinterSettings; }
            set { slic3rPrinterSettings = value; RegMemory.SetString("slic3rPrinterSettings", slic3rPrinterSettings); }
        }
        public string Slic3rFilamentSettings
        {
            get { return slic3rFilamentSettings; }
            set { slic3rFilamentSettings = value; RegMemory.SetString("slic3rFilamentSettings", slic3rFilamentSettings); }
        }
        public string Slic3rFilament2Settings
        {
            get { return slic3rFilament2Settings; }
            set { slic3rFilament2Settings = value; RegMemory.SetString("slic3rFilament2Settings", slic3rFilament2Settings); }
        }
        public string Slic3rFilament3Settings
        {
            get { return slic3rFilament3Settings; }
            set { slic3rFilament3Settings = value; RegMemory.SetString("slic3rFilament3Settings", slic3rFilament3Settings); }
        }
        public int Slic3rVersionGroup
        {
            get { return slic3rVersionGroup; }
            set { slic3rVersionGroup = value; RegMemory.SetInt("slic3rVersionGroup", slic3rVersionGroup); }
        }
        public string SkeinforgeProfile
        {
            get { return skeinforgeProfile; }
            set { skeinforgeProfile = value; RegMemory.SetString("skeinforgeProfile", skeinforgeProfile); }
        }
        public string SkeinforgeProfileDir
        {
            get { return skeinforgeProfileDir; }
            set { skeinforgeProfileDir = value; RegMemory.SetString("skeinforgeProfileDir", skeinforgeProfileDir); }
        }
        public string ExternalSlic3rPath
        {
            get { return externalSlic3rPath; }
            set { externalSlic3rPath = value; RegMemory.SetString("externalSlic3rPath", externalSlic3rPath); }
        }
        public string ExternalSlic3rIniFile
        {
            get { return externalSlic3rIniFile; }
            set { externalSlic3rIniFile = value; RegMemory.SetString("externalSlic3rIniFile", externalSlic3rIniFile); }
        }
        public bool InternalSlic3rUseBundledVersion
        {
            get { return internalSlic3rUseBundledVersion; }
            set { internalSlic3rUseBundledVersion = value; RegMemory.SetBool("internalSlic3rUseBundledVersion",internalSlic3rUseBundledVersion); }
        }
    }
}
