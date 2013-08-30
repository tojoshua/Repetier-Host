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
using System.IO;
using Microsoft.Win32;
using RepetierHost.view.utils;
using RepetierHost.model;
using System.Runtime.InteropServices;

namespace RepetierHost.view
{
    public partial class GlobalSettings : Form
    {
        RegistryKey repetierKey;

        public GlobalSettings()
        {
            InitializeComponent();
            if (Main.IsMono)
            {
                buttonAssociate.Enabled = false;
                checkG.Enabled = false;
                checkGCO.Enabled = false;
                checkGCode.Enabled = false;
                checkSTL.Enabled = false;
                checkOBJ.Enabled = false;
                checkNC.Enabled = false;
            }
            RegMemory.RestoreWindowPos("globalSettingsWindow", this);
            repetierKey = Custom.BaseKey; // Registry.CurrentUser.CreateSubKey("SOFTWARE\\Repetier");
            RegToForm();
            translate();
            Main.main.languageChanged += translate;
        }
        public void translate()
        {
            Text = Trans.T("W_REPETIER_SETTINGS");
            groupBehaviour.Text = Trans.T("L_BEHAVIOUR");
            groupFilesAndDirectories.Text = Trans.T("L_FILES_AND_DIRECTORIES");
            groupGUI.Text = Trans.T("L_GUI");
            labelInfoWorkdir.Text = Trans.T("L_INFO_WORKDIR");
            checkLogfile.Text = Trans.T("L_LOG_SESSION");
            checkReduceToolbarSize.Text = Trans.T("REDUCE_TOOLBAR_SIZE");
            checkDisableQualityReduction.Text = Trans.T("L_DISABLE_QUALITY_REDUCTION");
            labelWorkdir.Text = Trans.T("L_WORKDIR:");
            buttonSearchWorkdir.Text = Trans.T("B_BROWSE");
            folderBrowser.Description = Trans.T("L_SELECT_WORKING_DIRECTORY"); // Select working directory
            checkRedGreenSwitch.Text = Trans.T("L_USE_RED_GREEN_SWITCH");
            buttonAbort.Text = Trans.T("B_CANCEL");
            buttonOK.Text = Trans.T("B_OK");
            groupFileAssociations.Text = Trans.T("L_FILE_ASSOCIATIONS");
            buttonAssociate.Text = Trans.T("L_ASSOCIATE_EXTENSIONS");
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        } 
        public bool WorkdirOK()
        {
            string wd = Workdir;
            if (wd.Length == 0 || !Directory.Exists(wd))
            {
                labelOKMasg.Text = Trans.T("L_EXISTING_WORKDIR_REQUIRED"); // "Existing work directory required!";
                return false;
            }
            labelOKMasg.Text = "";
            return true;
        }
        public void FormToReg()
        {
            repetierKey.SetValue("workdir", Workdir);
            repetierKey.SetValue("logEnabled", LogEnabled ? 1 : 0);
            repetierKey.SetValue("disableQualityReduction", DisableQualityReduction ? 1 : 0);
            repetierKey.SetValue("reduceToolbarSize", ReduceToolbarSize ? 1 : 0);
            RegMemory.SetInt("onOffImageOffset", checkRedGreenSwitch.Checked ? 2 : 0);
        }
        public void RegToForm()
        {
            Workdir = (string)repetierKey.GetValue("workdir", Workdir);
            checkLogfile.Checked = 1== (int) repetierKey.GetValue("logEnabled", LogEnabled ? 1 : 0);
            checkDisableQualityReduction.Checked = 1 == (int)repetierKey.GetValue("disableQualityReduction", DisableQualityReduction ? 1 : 0);
            checkReduceToolbarSize.Checked = 1 == (int)repetierKey.GetValue("reduceToolbarSize", ReduceToolbarSize ? 1 : 0);
            checkRedGreenSwitch.Checked = 2 == RegMemory.GetInt("onOffImageOffset", 0);
        }
        public static void Associate(string extension,
           string progID, string description)
        {
            string icon = Application.StartupPath + Path.DirectorySeparatorChar + "repetier-logo-trans32.ico";
            string application = Application.ExecutablePath;
            RegistryKey classes = Registry.CurrentUser.OpenSubKey("Software\\Classes",true);
            classes.CreateSubKey(extension).SetValue("", progID);
            if (progID != null && progID.Length > 0)
                using (RegistryKey key = classes.CreateSubKey(progID))
                {
                    if (description != null)
                        key.SetValue("", description);
                    if (icon != null)
                        key.CreateSubKey("DefaultIcon").SetValue("", ToShortPathName(icon));
                    if (application != null)
                        key.CreateSubKey(@"Shell\Open\Command").SetValue("",
                                    ToShortPathName(application) + " \"%1\"");
                }
        }
        [DllImport("Kernel32.dll")]
        private static extern uint GetShortPathName(string lpszLongPath,
            [Out] StringBuilder lpszShortPath, uint cchBuffer);
        [DllImport("Shell32.dll")]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        // Return short path format of a file name
        private static string ToShortPathName(string longName)
        {
            StringBuilder s = new StringBuilder(1000);
            uint iSize = (uint)s.Capacity;
            uint iRet = GetShortPathName(longName, s, iSize);
            return s.ToString();
        }
        public string Workdir
        {
            get { return textWorkdir.Text; }
            set { textWorkdir.Text = value; }
        }
        public Boolean LogEnabled
        {
            get { return checkLogfile.Checked; }
        }
        public Boolean DisableQualityReduction
        {
            get { return checkDisableQualityReduction.Checked; }
        }
        public Boolean ReduceToolbarSize
        {
            get { return checkReduceToolbarSize.Checked; }
        }
        private void buttonAbort_Click(object sender, EventArgs e)
        {
            RegToForm();
            if(WorkdirOK())
                Hide();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            FormToReg();
            if(WorkdirOK())
                Hide();
        }

        private void buttonSearchWorkdir_Click(object sender, EventArgs e)
        {
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                textWorkdir.Text = folderBrowser.SelectedPath;
            }
        }

        private void GlobalSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            RegMemory.StoreWindowPos("globalSettingsWindow", this, false, false);
        }

        private void textWorkdir_TextChanged(object sender, EventArgs e)
        {
            WorkdirOK();
        }

        private void checkReduceToolbarSize_CheckedChanged(object sender, EventArgs e)
        {
            Main.main.UpdateToolbarSize();
        }
        private void buttonAssociate_Click(object sender, EventArgs e)
        {
            string progid = Main.main.basicTitle;
            int p = -1,p2 = -1;
            for (int i = 0; i < progid.Length; i++)
            {
                char c = progid[i];
                if (c == ' ') p2 = i;
                if (c >= '0' && c <= '9')
                {
                    p = i;
                    break;
                }
            }
            if (p > 0)
                progid = progid.Substring(0, p2>0 ? p2 : p).Trim();
            progid = progid.Replace(" ", "-");
            if (checkSTL.Checked)
                Associate(".stl", progid, "STL file");
            if (checkOBJ.Checked)
                Associate(".obj", progid, "OBJ file");
            if (checkG.Checked)
                Associate(".g", progid, "G-Code");
            if (checkGCO.Checked)
                Associate(".gco", progid, "G-Code");
            if (checkGCode.Checked)
                Associate(".gcode", progid, "G-Code");
            if (checkNC.Checked)
                Associate(".nc", progid, "G-Code");

            SHChangeNotify(0x8000000, 0x1000, IntPtr.Zero, IntPtr.Zero); 
        }
    }
}
