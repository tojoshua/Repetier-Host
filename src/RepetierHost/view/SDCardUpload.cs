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
using RepetierHost.view.utils;
using RepetierHost.model;

namespace RepetierHost.view
{
    public partial class SDCardUpload : Form
    {
        public static SDCardUpload f = null;
        public bool exit = false;
        public static SDCardUpload Execute()
        {
            if (f == null)
            {
                f = new SDCardUpload();
            }
            f.ShowDialog();
            return f;
        }
        public SDCardUpload()
        {
            InitializeComponent();
            RegMemory.RestoreWindowPos("sdcardUploadWindow",this);
            translate();
            Main.main.languageChanged += translate;
        }
        private void translate()
        {
            Text = Trans.T("W_SD_CARD_UPLOAD");
            labelFile.Text = Trans.T("L_FILE:");
            labelUploadedFilename.Text = Trans.T("L_UPLOADED_FILENAME:");
            checkJobFinished.Text = Trans.T("L_INCLUDE_JOB_FINISHED_COMMANDS");
            checkAppendPrepend.Text = Trans.T("L_INCLUDE_START_END_CODE");
            radioCurrent.Text = Trans.T("L_UPLOAD_CURRENG_GCODE");
            radioExternal.Text = Trans.T("L_UPLOAD_EXTERNAL_FILE");
            buttonBrowse.Text = Trans.T("B_BROWSE");
            buttonCancel.Text = Trans.T("B_CANCEL");
            buttonOK.Text = Trans.T("B_OK");
        }
        private void buttonOK_Click(object sender, EventArgs e)
        {
            exit = true;
            Hide();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            exit = false;
            Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                extFilename.Text = openFileDialog.FileName;
            }
        }

        private void textFilename_Validating(object sender, CancelEventArgs e)
        {
            TextBox box = (TextBox)sender;
            bool ok = true;
            try
            {
                box.Text = box.Text.ToLower();
                string t = box.Text;
                if (t.Length > 12 || t.Length == 0) ok = false;
                int p = t.IndexOf('.');
                if (p>8) ok = false;
                if (p < 0 && t.Length > 8) ok = false;
                int i;
                for (i = 0; i < t.Length; i++)
                {
                    if (i == p) continue;
                    char c = t[i];
                    bool cok = false;
                    if (c >= '0' && c <= '9') cok = true;
                    else if (c >= 'a' && c <= 'z') cok = true;
                    else if (c == '_') cok = true;
                    if (!cok)
                    {
                        ok = false;
                        break;
                    }
                }
            }
            catch
            {
                ok = false;
            }
            buttonOK.Enabled = ok;
            if(ok)
                errorProvider.SetError(box, "");
            else
                errorProvider.SetError(box, Trans.T("L_NOT_VALID_83_FILENAME")); //"Not a valid 8.3 filename.");
        }

        private void SDCardUpload_FormClosing(object sender, FormClosingEventArgs e)
        {
            RegMemory.StoreWindowPos("sdcardUploadWindow", this, false, false);
        }
    }
}
