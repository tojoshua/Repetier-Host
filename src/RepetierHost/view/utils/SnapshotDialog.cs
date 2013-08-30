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

namespace RepetierHost.view.utils
{
    public partial class SnapshotDialog : Form
    {
        public SnapshotDialog()
        {
            InitializeComponent();

            Main.main.languageChanged += translate;
            translate();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void translate()
        {
            this.Text = Trans.T("W_POSTPONED_JOB_DIALOG");
            descriptionLabel.Text = Trans.T("L_POSTPONED_JOB_DIALOG_DESCRIPTION");
            buttonForceSnapshot.Text = Trans.T("B_FORCE_SAVE_POSTPONED_JOB");
            buttonResumeJob.Text = Trans.T("B_RESUME_PRINTING_JOB");
        }

        private void buttonForceSnapshot_Click(object sender, EventArgs e)
        {
            Main.main.OnReadyToSaveStateCallback();
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Main.main.CancelSaveState();
            this.Close();
        }
    }
}
