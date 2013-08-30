/*
   Copyright 2011-2013 repetier repetierdev@gmail.com

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
using System.ComponentModel;
using System.IO;
using System.Threading;
using Microsoft.Win32;
using System.Diagnostics;
using RepetierHost;
using RepetierHost.view.utils;

namespace RepetierHost.model
{
    public class PrinterModel : INotifyPropertyChanged
    {
        private RegistryKey key;
        private bool updating = false;
        private string slic3rPrint;
        private string slic3rPrinter;
        private string slic3rFilament1;
        private string slic3rFilament2;
        private string slic3rFilament3;
        private string skeinforgeProfile;
        private Slicer.SlicerID activeSlicer = Slicer.SlicerID.Slic3r;
        public PrinterModel()
        {
            key = Main.printerSettings.currentPrinterKey;
            Main.printerSettings.eventPrinterChanged += PrinterChanged;
            // Default settinsg from previous installations without separate storage
            BasicConfiguration b = BasicConfiguration.basicConf;
            slic3rPrint = b.Slic3rPrintSettings;
            slic3rPrinter = b.Slic3rPrinterSettings;
            slic3rFilament1 = b.Slic3rFilamentSettings;
            slic3rFilament2 = b.Slic3rFilament2Settings;
            slic3rFilament3 = b.Slic3rFilament3Settings;
            skeinforgeProfile = b.SkeinforgeProfile;
            activeSlicer = (Slicer.SlicerID)(int)Main.main.repetierKey.GetValue("ActiveSlicer", (int)activeSlicer);
            readPrinterSettings();
        }
        public Slicer.SlicerID ActiveSlicer
        {
            get { return activeSlicer; }
            set
            {
                if (!updating && activeSlicer == value) return;
                activeSlicer = value;
                key.SetValue("activeSlicer", (int)activeSlicer);
                OnPropertyChanged(new PropertyChangedEventArgs("ActiveSlicer"));
            }
        }
        public string Slic3rPrint
        {
            get { return slic3rPrint; }
            set
            {
                if (!updating && slic3rPrint == value) return;
                slic3rPrint = value;
                key.SetValue("slic3rPrint", slic3rPrint);
                OnPropertyChanged(new PropertyChangedEventArgs("Slic3rPrint"));
            }
        }
        public string Slic3rPrinter
        {
            get { return slic3rPrinter; }
            set
            {
                if (!updating && slic3rPrinter == value) return;
                slic3rPrinter = value;
                key.SetValue("slic3rPrinter", slic3rPrinter);
                OnPropertyChanged(new PropertyChangedEventArgs("Slic3rPrinter"));
            }
        }
        public string Slic3rFilament1
        {
            get { return slic3rFilament1; }
            set
            {
                if (!updating && slic3rFilament1 == value) return;
                slic3rFilament1 = value;
                key.SetValue("slic3rFilament1", slic3rFilament1);
                OnPropertyChanged(new PropertyChangedEventArgs("Slic3rFilament1"));
            }
        }
        public string Slic3rFilament2
        {
            get { return slic3rFilament2; }
            set
            {
                if (!updating && slic3rFilament2 == value) return;
                slic3rFilament2 = value;
                key.SetValue("slic3rFilament2", slic3rFilament2);
                OnPropertyChanged(new PropertyChangedEventArgs("Slic3rFilament2"));
            }
        }
        public string Slic3rFilament3
        {
            get { return slic3rFilament3; }
            set
            {
                if (!updating && slic3rFilament3 == value) return;
                slic3rFilament3 = value;
                key.SetValue("slic3rFilament3", slic3rFilament3);
                OnPropertyChanged(new PropertyChangedEventArgs("Slic3rFilament3"));
            }
        }
        public string SkeinforgeProfile
        {
            get { return skeinforgeProfile; }
            set
            {
                if (!updating && skeinforgeProfile == value) return;
                skeinforgeProfile = value;
                key.SetValue("skeinforgeProfile", skeinforgeProfile);
                OnPropertyChanged(new PropertyChangedEventArgs("SkeinforgeProfile"));
            }
        }
        private void readPrinterSettings() {
            updating = true;
            Slic3rPrint = (string)key.GetValue("slic3rPrint", slic3rPrint);
            Slic3rPrinter = (string)key.GetValue("slic3rPrinter", slic3rPrinter);
            Slic3rFilament1 = (string)key.GetValue("slic3rFilament1", slic3rFilament1);
            Slic3rFilament2 = (string)key.GetValue("slic3rFilament2", slic3rFilament2);
            Slic3rFilament3 = (string)key.GetValue("slic3rFilament3", slic3rFilament3);
            SkeinforgeProfile = (string)key.GetValue("skeinforgeProfile", skeinforgeProfile);
            ActiveSlicer = (Slicer.SlicerID)key.GetValue("activeSlicer", activeSlicer);
            updating = false;
        }
        public void PrinterChanged(RegistryKey printerKey,bool printerChanged) {
            key = printerKey;
            readPrinterSettings();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
    }
}
