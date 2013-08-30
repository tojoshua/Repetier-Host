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
    public partial class EEPROMMarlin : Form
    {
        EEPROMMarlinStorage storage;
        bool reinit = true;       
        
        public EEPROMMarlin()
        {
            InitializeComponent();
            RegMemory.RestoreWindowPos("eepromMarlinWindow", this);
            storage = Main.conn.eepromm;
            storage.eventAdded += newline;
            translate();
            Main.main.languageChanged += translate;
            newline(Main.conn.eepromm);
        }
        private void translate()
        {
            Text = Trans.T("W_EEPROM_MARLIN");
            labelAcceleration.Text = Trans.T("L_M_ACCELERATION");
            labelAdvancedVariables.Text = Trans.T("L_M_ADVANCED_VARIABLES");
            labelHomingOffset.Text = Trans.T("L_M_HOMING_OFFSET");
            labelMaxAcceleration.Text = Trans.T("L_M_MAX_ACCELERATION");
            labelMaxFeedrate.Text = Trans.T("L_M_MAX_FEEDRATE");
            labelMaxXYJerk.Text = Trans.T("L_M_MAX_XY_JERK");
            labelMaxZJerk.Text = Trans.T("L_M_MAX_Z_JERK");
            labelMinFeedrate.Text = Trans.T("L_M_MIN_FEEDRATE");
            labelMinSegmentTime.Text = Trans.T("L_M_MIN_SEGMENT_TIME");
            labelMinTravelFeedrate.Text = Trans.T("L_M_MIN_TRAVEL_FEEDRATE");
            labelPIDSettings.Text = Trans.T("L_M_PID_SETTINGS");
            labelStepsPerMM.Text = Trans.T("L_M_STEPS_PER_MM");
            lableRetractAcceleration.Text = Trans.T("L_M_RETRACT_ACCELERATION");
            buttonAbort.Text = Trans.T("B_CANCEL");
            buttonLoad.Text = Trans.T("B_M_LOAD");
            buttonRestore.Text = Trans.T("B_M_RESTORE");
            buttonSave.Text = Trans.T("B_M_SAVE");            
        }
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            reinit = false;
            storage.Update();
        }
        
        private void buttonRestore_Click(object sender, EventArgs e)
        {
            reinit = true;
            storage.retriev_factory_settings();
            string message = Trans.T("L_M_SAVE_RETIEVED"); // "Save retrieved Changes to EEPROM?";
            string caption = Trans.T("L_M_FACTORY_RETRIEVED"); // "Factory Settings retrieved";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;
            result = MessageBox.Show(message, caption, buttons);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                storage.SaveToEEPROM();
            }
            storage.Update();
        }

        private void buttonAbort_Click_1(object sender, EventArgs e)
        {
            storage.eventAdded -= newline;
            Hide();
        }

        public void Show2()
        {
            reinit = true;
            Show();
            BringToFront();
        }

        private void newline(EEPROMMarlinStorage p)
        {
            xstepsbox.Text = p.SX;
            ystepsbox.Text = p.SY;
            zstepsbox.Text = p.SZ;
            estepsbox.Text = p.SE;
            xfeedbox.Text = p.FX;
            yfeedbox.Text = p.FY;
            zfeedbox.Text = p.FZ;
            efeedbox.Text = p.FE;
            maccxbox.Text = p.AX;
            maccybox.Text = p.AY;
            macczbox.Text = p.AZ;
            maccebox.Text = p.AE;
            accbox.Text = p.ACC;
            raccbox.Text = p.RACC;
            minfeedbox.Text = p.AVS;
            mintfeedbox.Text = p.AVT;
            minsegtbox.Text = p.AVB;
            maxxyjerkbox.Text = p.AVX;
            mzjerkbox.Text = p.AVZ;
            ppidbox.Enabled = ipidbox.Enabled = dpidbox.Enabled = p.hasPID;
            ppidbox.Text = p.PPID;
            ipidbox.Text = p.IPID;
            dpidbox.Text = p.DPID;
            hoxbox.Text = p.hox;
            hoybox.Text = p.hoy;
            hozbox.Text = p.hoz;
        }

        private void EEPROMMarlin_Activated(object sender, EventArgs e)
        {
            if (reinit)
            {
                reinit = false;
                storage.Update();
            }
        }
        private void EEPROMMarlin_FormClosing(object sender, FormClosingEventArgs e)
        {
            RegMemory.StoreWindowPos("eepromMarlinWindow", this, false, false);
        }
        private void buttonSave_Click(object sender, EventArgs e)
        {
            storage.SX = xstepsbox.Text;
            storage.SY = ystepsbox.Text;
            storage.SZ = zstepsbox.Text;
            storage.SE = estepsbox.Text;
            storage.FX = xfeedbox.Text;
            storage.FY = yfeedbox.Text;
            storage.FZ = zfeedbox.Text;
            storage.FE = efeedbox.Text;
            storage.AX = maccxbox.Text;
            storage.AY = maccybox.Text;
            storage.AZ = macczbox.Text;
            storage.AE = maccebox.Text;
            storage.ACC = accbox.Text;
            storage.RACC = raccbox.Text;
            storage.AVS = minfeedbox.Text;
            storage.AVT = mintfeedbox.Text;
            storage.AVB = minsegtbox.Text;
            storage.AVX = maxxyjerkbox.Text;
            storage.AVZ = mzjerkbox.Text;
            storage.PPID = ppidbox.Text;
            storage.IPID = ipidbox.Text;
            storage.DPID = dpidbox.Text;
            storage.HOX = hoxbox.Text;
            storage.HOY = hoybox.Text;
            storage.HOZ = hozbox.Text;
            
            storage.Save();

            string message = Trans.T("L_M_CONFIRM_WRITE"); // "Settings stored to running config.\n Write Changes to EEPROM?";
            string caption = Trans.T("L_M_SETTINGS_STORED"); // "Settings stored";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;
            result = MessageBox.Show(message, caption, buttons);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                storage.SaveToEEPROM();
                buttonAbort_Click_1(null, null);
            }
        }
        private void floatPos_Validating(object sender, CancelEventArgs e)
        {
            TextBox box = (TextBox)sender;
            try
            {
                float x = float.Parse(box.Text,GCode.format);
                if (x >= 0)
                    errorProvider.SetError(box, "");
                else
                    errorProvider.SetError(box, Trans.T("L_POSITIVE_NUMBER_REQUIRED"));
            }
            catch
            {
                errorProvider.SetError(box, Trans.T("L_NOT_A_NUMBER"));
            }
        }
        private void int_Validating(object sender, CancelEventArgs e)
        {
            TextBox box = (TextBox)sender;
            try
            {
                int.Parse(box.Text);
                errorProvider.SetError(box, "");
            }
            catch
            {
                errorProvider.SetError(box, Trans.T("L_NOT_AN_INTEGER"));
            }
        }

    }
}
