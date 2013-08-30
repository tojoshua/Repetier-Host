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
    public partial class PendingPrintJobsDialog : Form
    {
        public PendingPrintJob selectedJob;

        public PendingPrintJobsDialog()
        {
            InitializeComponent();

            LoadPendingJobs();

            translate();
            Main.main.languageChanged += translate;
        }

        private void translate()
        {
            Text = Trans.T("W_POSTPONED_PRINT_JOBS");
            toolStripButtonSelectJob.ToolTipText = Trans.T("M_POSTPONED_JOB_SELECT_JOB");
            toolStripButtonKillJob.ToolTipText = Trans.T("M_POSTPONED_JOB_KILL_JOB");
            toolStripButtonRename.ToolTipText = Trans.T("M_POSTPONED_JOB_RENAME_JOB");
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Main.main.languageChanged -= translate;
            base.OnClosing(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case (Keys.Escape):
                    Cancel();
                break;
                case (Keys.Delete):
                    KillSelectedJob();
                break;
                case (Keys.Return):
                    RestoreSelectedJob();
                break;
                case (Keys.F2):
                    RenameSelectedJob();
                break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void LoadPendingJobs()
        {
            List<PendingPrintJob> list = PendingPrintJobs.GetPendingJobs();
            foreach (PendingPrintJob job in list)
            {
                pendingJobsListbox.Items.Add(job);
            }
        }

        public PendingPrintJob GetSelectedJob()
        {
            return selectedJob;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void toolStripButtonSelectJob_Click(object sender, EventArgs e)
        {
            RestoreSelectedJob();
        }

        private void toolStripButtonKillJob_Click(object sender, EventArgs e)
        {
            KillSelectedJob();
        }

        private void toolStripButtonRename_Click(object sender, EventArgs e)
        {
            RenameSelectedJob();
        }

        private void pendingJobsListbox_DoubleClick(object sender, EventArgs e)
        {
            if (pendingJobsListbox.SelectedIndex > -1)
            {
                RestoreSelectedJob();
            }
        }

        
        private void RestoreSelectedJob()
        {
            this.selectedJob = (PendingPrintJob)pendingJobsListbox.SelectedItem;
            this.Close();
        }

        private void KillSelectedJob()
        {
            PendingPrintJob job = (PendingPrintJob)pendingJobsListbox.SelectedItem;
            if (job != null)
            {
                if (MessageBox.Show(Trans.T1("L_CONFIRM_DELETE_JOB", job.Name), Trans.T("L_SECURITY_QUESTION"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        job.Delete();
                        pendingJobsListbox.Items.Remove(job);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void RenameSelectedJob()
        {
            PendingPrintJob job = (PendingPrintJob)pendingJobsListbox.SelectedItem;
            if (job != null)
            {
                string currentSnapshotName = job.Name;
                string newSnapshotName = ReadSnapshotName(currentSnapshotName);
                if (newSnapshotName == null)
                {
                    // User cancelled
                    return;
                }
                if (!newSnapshotName.Equals(currentSnapshotName))
                {
                    try
                    {
                        job.Rename(newSnapshotName);
                        pendingJobsListbox.Items[pendingJobsListbox.SelectedIndex] = job;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Cancel()
        {
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Show the user a dialog requesting a snapshot name.
        /// If the name is iinvalid, ask him again until he sets a right name.
        /// If the user cancels, returns null.
        /// </summary>
        /// <returns></returns>
        private static string ReadSnapshotName(string currentSnapshotName)
        {
            string snapshotName = currentSnapshotName;
            do
            {
                snapshotName = StringInput.GetString(Trans.T("L_POSTPONED_JOB_NAME"), Trans.T("L_NAME_POSTPONED_JOB"), snapshotName, true);
                if (snapshotName == null)
                {
                    // User cancelled.
                    return null;
                }
            } while (PendingPrintJob.IsInvalidSnapshotName(snapshotName));

            return snapshotName;
        }
    
    }

}
