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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RepetierHost.model;
using RepetierHost.view.utils;

namespace RepetierHost.view
{
    public partial class EEPROMRepetier : Form
    {
        EEPROMStorage storage;
        BindingList<EEPROMParameter> data = new BindingList<EEPROMParameter>();
        bool reinit = true;
        public EEPROMRepetier()
        {
            InitializeComponent();
            RegMemory.RestoreWindowPos("eepromWindow", this);
            storage = Main.conn.eeprom;
            grid.Columns.Add("Description", "Description");
            grid.Columns[0].DataPropertyName = "Description";
            grid.Columns[0].ReadOnly = true;
            grid.Columns.Add("Value", "Value");
            grid.Columns[1].DataPropertyName = "Value";
            grid.DataSource = data;
            translate();
            Main.main.languageChanged += translate;
        }
        public void translate()
        {
            Text = Trans.T("W_FIRMWARE_EEPROM_SETTINGS");
            buttonAbort.Text = Trans.T("B_CANCEL");
            buttonOK.Text = Trans.T("B_OK");
        }
        public void Show2()
        {
            reinit = true;
            Show();
            BringToFront();
        }
        private void newline(EEPROMParameter p)
        {
            data.Add(p);
        }
        private void buttonOK_Click(object sender, EventArgs e)
        {
            storage.Save();
            storage.Clear();
            storage.eventAdded -= newline;
            grid.DataSource = null;
            data.Clear();
            Hide();
        }

        private void buttonAbort_Click(object sender, EventArgs e)
        {
            storage.Clear();
            data.Clear();
            storage.eventAdded -= newline;
            grid.DataSource = null;
            Hide();
        }

        private void EEPROMRepetier_FormClosing(object sender, FormClosingEventArgs e)
        {
            RegMemory.StoreWindowPos("eepromWindow", this, false, false);
        }

        private void EEPROMRepetier_Activated(object sender, EventArgs e)
        {
            if (reinit)
            {
                reinit = false;
                storage.Clear();
                data.Clear();
                grid.DataSource = data;
                storage.eventAdded += newline;
                storage.Update();
            }
        }
    }
}
