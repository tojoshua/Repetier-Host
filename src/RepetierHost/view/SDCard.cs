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
    public delegate void OnResponse(string response);
    public partial class SDCard : Form
    {
        public class FileEntry
        {
            public bool isDirectory;
            public string name;
            public string folder;
            public string size="";
            public FileEntry(string[] fparts)
            {
                if (fparts[0].StartsWith("/")) fparts[0] = fparts[0].Substring(1);
                string fullname = fparts[0];
                if (fparts.Length > 1)
                    size = fparts[1];
                string[] parts = fullname.Split('/');
                isDirectory = fullname.EndsWith("/");
                if (isDirectory)
                {
                    name = parts[parts.Length - 2];
                    folder = string.Join("/", parts, 0, parts.Length - 2);
                    if(parts.Length>2) folder+="/";
                    if (name != "..")
                    {
                        SDCard.f.allDirs.Add(fullname, this);
                        SDCard.f.allFiles.AddLast(new FileEntry(new string[] { fullname.ToLower() + "../", "" }));
                    }
                }
                else
                {
                    name = parts[parts.Length - 1];
                    if (parts.Length == 1)
                        folder = "";
                    else
                    {
                        folder = string.Join("/", parts, 0, parts.Length - 1) + "/";
                        if (name!=".." && !SDCard.f.allDirs.Keys.Contains(folder))
                        {
                            FileEntry ent = new FileEntry(folder);
                            SDCard.f.allDirs.Add(folder,ent );
                            SDCard.f.allFiles.AddLast(ent);
                            SDCard.f.allFiles.AddLast(new FileEntry(new string[] { folder.ToLower() + "../", "" }));
                        }
                    }
                }
            }
            public FileEntry(string directory)
            {
                isDirectory = true;
                string[] parts = directory.Split('/');
                name = parts[parts.Length - 2];
                if (parts.Length == 2)
                    folder = "";
                else
                {
                    folder = string.Join("/", parts, 0, parts.Length - 2) + "/";
                    if (!SDCard.f.allDirs.Keys.Contains(folder))
                    {
                        FileEntry ent = new FileEntry(folder);
                        SDCard.f.allDirs.Add(folder, ent);
                        SDCard.f.allFiles.AddLast(ent);
                        SDCard.f.allFiles.AddLast(new FileEntry(new string[] { folder.ToLower() + "../", "" }));
                    }
                }
            }
        }
        public Dictionary<string, FileEntry> allDirs = new Dictionary<string, FileEntry>();
        public LinkedList<FileEntry> allFiles = new LinkedList<FileEntry>();
        static SDCard f = null;
        bool mounted = true;
        bool printing = false;
        bool printPaused = false;
        bool uploading = false;
        bool readFilenames = false;
        bool updateFilenames = false;
        bool startPrint = false;
        long startTime = 0;
        int printWait = 0;
        int waitDelete = 0;
        string currentDirectory = "";
        private event OnResponse ana;
        public static void Execute() {
            if (f == null)
            {
                f = new SDCard();
                Main.conn.eventResponse += f.analyzeEvent;
            }
            f.RefreshFilenames();
            f.Show(Main.main);
            f.BringToFront();
            f.updateButtons();
        }
        public static void Disconnected()
        {
            if (f == null) return;
            if (f.Visible)
                f.Hide();
            f.mounted = true;
            f.printing = false;
            f.uploading = false;
            Main.conn.analyzer.uploading = false;
            f.readFilenames = false;
            f.startPrint = false;
            f.currentDirectory = "";
        }
        public SDCard()
        {
            ana += analyzer;
            InitializeComponent();
            RegMemory.RestoreWindowPos("sdcardWindow",this);
            translate();
            Main.main.languageChanged += translate;
        }
        private void translate()
        {
            Text = Trans.T("W_SD_CARD_MANAGER");
            buttonClose.Text = Trans.T("B_CLOSE");
            columnHeaderName.Text = Trans.T("L_FILENAME");
            columnHeaderSize.Text = Trans.T("L_SIZE");
            toolAddFile.ToolTipText = Trans.T("L_UPLOAD_FILE");
            toolDelFile.ToolTipText = Trans.T("L_DELETE_FILE");
            toolStartPrint.ToolTipText = Trans.T("L_RUN_CONTINUE_UPLOAD"); // Run selected file / Continue paused print
            toolStopPrint.ToolTipText = Trans.T("L_STOP_UPLOAD");
            toolMount.ToolTipText = Trans.T("L_MOUNT_SD_CARD");
            toolUnmount.ToolTipText = Trans.T("L_UNMOUNT_SD_CARD");
            toolNewFolder.ToolTipText = Trans.T("L_NEW_FOLDER");
        }
        public void RefreshFilenames()
        {
            updateFilenames = false;
            allFiles.Clear();
            Main.conn.injectManualCommand("M20");
        }
        private void analyzeEvent(string res)
        {
            try
            {
                Main.main.Invoke(ana, res); //new object[] {res});
            }
            catch { }
        }
        private string reduceSpace(string a)
        {
            StringBuilder b = new StringBuilder();
            char lastc = 'X';
            foreach (char c in a)
            {
                if (c != lastc || c != ' ')
                    b.Append(c);
                lastc = c;
            }
            return b.ToString();
        }
        void fillFiles(string folder)
        {
            Text = Trans.T("W_SD_CARD_MANAGER") + (currentDirectory.Length > 0 ? " - " + currentDirectory : "");
            files.Items.Clear();
            foreach(FileEntry f in allFiles) {
                if (f.folder == folder)
                {
                    ListViewItem item = new ListViewItem(new string[] { f.name, f.size },(f.isDirectory?1:0));
                    item.Tag = f;
                    files.Items.Add(item);
                }
            }
        }
        private void analyzer(string res)
        {
            if (readFilenames)
            {
                if(res.StartsWith("End file list")) {
                    readFilenames = false;
                    fillFiles(currentDirectory);
                    return;
                }
                string[] parts = reduceSpace(res.ToLower()).Trim().Split(' ');
                allFiles.AddLast(new FileEntry(parts));
                //files.Items.Add(new ListViewItem(parts));
                return;
            }
            if(res.StartsWith("Begin file list")) {
                readFilenames = true;
                allFiles.Clear();
                allDirs.Clear();
                return;
            }
            // Printing done?
            if (res.IndexOf("Not SD printing") != -1 || res.IndexOf("Done printing file")!=-1)
            {
                printing = false;
                toolStatus.Text = Trans.T("L_PRINT_FINISHED"); // "Print finished";
                progress.Value = 200;
            }
            else if (res.IndexOf("SD printing byte ") != -1) // Print status update
            {
                string s = res.Substring(res.IndexOf("SD printing byte ") + 17);
                string[] s2 = s.Split('/');
                if (s2.Length == 2)
                {
                    long a, b;
                    long.TryParse(s2[0], out a);
                    long.TryParse(s2[1], out b);
                    progress.Value = (int)(200 * a / b);
                }
            }
            else if (res.IndexOf("SD init fail") != -1 || res.IndexOf("volume.init failed") != -1 ||
                res.IndexOf("openRoot failed")!=-1) // mount failed
            {
                mounted = false;
            }
            else if (uploading && (res.Contains("M999") || res.IndexOf("error writing to file") != -1)) // write error
            {
                Main.conn.connector.KillJob();
                Main.conn.analyzer.uploading = false;
                uploading = false;
                toolStatus.Text = Trans.T("L_UPLOAD_FAILED"); // "Upload failed.";
            }
            else if (res.IndexOf("Done saving file") != -1) // save finished
            {
                double time = (double)(DateTime.Now.Ticks-startTime)*0.0000001;
                uploading = false;
                progress.Value = 200;
                toolStatus.Text = Trans.T("L_UPLOAD_FINISHED"); // "Upload finished.";
                updateFilenames = true;
                Main.conn.log(Trans.T1("L_UPLOADING_TIME", Printjob.DoubleToTime(time)), false, 3);
            }
            else if(res.Contains("Invalid directory")) {
                if(uploading) {
                    Main.conn.connector.KillJob();
                    Main.conn.analyzer.uploading = false;
                    uploading = false;
                    toolStatus.Text = Trans.T("L_UPLOAD_FAILED"); // "Upload failed.";
                }
            } else if (res.IndexOf("File selected") != -1)
            {
                toolStatus.Text = Trans.T("L_SD_PRINTING..."); // "SD printing ...";
                progress.Value = 0;
                printing = true;
                printPaused = false;
                startPrint = true;
            }
            else if (uploading && res.StartsWith("open failed, File"))
            {
                Main.conn.connector.KillJob();
                Main.conn.analyzer.uploading = false;
                toolStatus.Text = Trans.T("L_UPLOAD_FAILED"); // "Upload failed.";
            }
            else if (res.StartsWith("File deleted"))
            {
                waitDelete = 0;
                toolStatus.Text = Trans.T("L_FILE_DELETED"); // "File deleted";
                updateFilenames = true;
            }
            else if (res.StartsWith("Deletion failed"))
            {
                waitDelete = 0;
                toolStatus.Text = Trans.T("L_DELETE_FAILED"); // "Delete failed";
            }
        }
        private void updateButtons()
        {
            if (!Main.conn.connector.IsConnected())
            {
                toolAddFile.Enabled = false;
                toolDelFile.Enabled = false;
                toolUnmount.Enabled = false;
                toolMount.Enabled = false;
                toolStartPrint.Enabled = false;
                toolStopPrint.Enabled = false;
                toolNewFolder.Enabled = false;
                return;
            }
            if (uploading || printing || Main.conn.connector.IsJobRunning())
            {
                toolAddFile.Enabled = false;
                toolDelFile.Enabled = false;
                toolUnmount.Enabled = false;
                toolMount.Enabled = false;
                toolStartPrint.Enabled = false;
                toolNewFolder.Enabled = false;
                toolStopPrint.Enabled = mounted;
            }
            else
            {
                toolStopPrint.Enabled = printPaused && mounted;
                toolAddFile.Enabled = mounted;
                bool fc = files.SelectedIndices.Count > 0;
                toolStartPrint.Enabled = (fc || printPaused) && mounted;
                toolDelFile.Enabled = fc && mounted;
                toolMount.Enabled = true;
                toolUnmount.Enabled = true;
                toolNewFolder.Enabled = true;
            }

        }
        private void buttonClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void toolAddFile_Click(object sender, EventArgs e)
        {
            Printjob job = Main.conn.connector.Job;
            if (Main.conn.connector.IsJobRunning())
            {
                updateButtons();
                return;
            }
            SDCardUpload f = SDCardUpload.Execute();
            if (f.exit)
            {
                toolStatus.Text = Trans.T("L_UPLOADING_FILE..."); // "Uploading file ...";
                progress.Value = 0;
                job.BeginJob();
                job.exclusive = true;
                job.PushData("M28 " +(currentDirectory.Length>0?"/":"")+currentDirectory+f.textFilename.Text);
                if(f.checkAppendPrepend.Checked)
                    job.PushGCodeShortArray(Main.main.editor.getContentArray(1));
                if (f.radioCurrent.Checked)
                {
                    job.PushGCodeShortArray(Main.main.editor.getContentArray(0));
                }
                else
                {
                    try
                    {
                        job.PushData(System.IO.File.ReadAllText(f.extFilename.Text));
                    }
                    catch (Exception ex)
                    {
                        job.exclusive = false;
                        job.BeginJob();
                        job.EndJob();
                        MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                if (f.checkAppendPrepend.Checked)
                    job.PushGCodeShortArray(Main.main.editor.getContentArray(2));
                if (f.checkJobFinished.Checked)
                {
                    PrinterConnection con = Main.conn;
                    if (con.afterJobDisableExtruder)
                    {
                        for (int i = 0; i < Main.conn.numberExtruder; i++)
                            job.PushData("M104 S0 T"+i.ToString());
                    }
                    if (con.afterJobDisablePrintbed)
                        job.PushData("M140 S0");
                    if (con.afterJobGoDispose)
                    {
                        job.PushData("G90");
                        job.PushData("G1 X" + con.disposeX.ToString(GCode.format) + " Y" + con.disposeY.ToString(GCode.format) + " F" + con.travelFeedRate.ToString(GCode.format));
                    }
                }
                job.PushData("M29");
                job.EndJob();
                uploading = true;
                startTime = DateTime.Now.Ticks;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (printing && printWait == 0)
            {
                printWait = 2;
                if(!Main.conn.connector.HasInjectedMCommand(27))
                    Main.conn.injectManualCommand("M27");
            }
            if (printWait <= 0) printWait = 2;
            if (uploading)
            {
                progress.Value = (int)(Main.conn.connector.Job.PercentDone * 2);
            }
            printWait--;
            if (updateFilenames) RefreshFilenames();
            if (startPrint)
            {
                startPrint = false;
                Main.conn.injectManualCommand("M24");
            }
            if (waitDelete > 0)
            {
                if (--waitDelete == 0)
                {
                    //MessageBox.Show("Your firmware doesn't implement file delete or has an unknown implementation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MessageBox.Show(Trans.T("L_FIRMWARE_NO_DELETE"), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            updateButtons();
        }

        private void toolStopPrint_Click(object sender, EventArgs e)
        {
            if(uploading) {
                Main.conn.connector.KillJob();
                Main.conn.analyzer.uploading = false;
                uploading = false;
                toolStatus.Text = Trans.T("L_UPLOAD_FAILED"); // "Upload failed.";
                Main.conn.injectManualCommand("M29");
            }
            if (printPaused)
            {
                printPaused = false;
                toolStatus.Text = Trans.T("L_PRINT_ABORTED"); // "Print aborted";
                return;
            }
            Main.conn.injectManualCommand("M25");
            printPaused = true;
            printing = false;
            toolStatus.Text = Trans.T("L_PRINT_PAUSED"); // "Print paused";
        }

        private void toolStartPrint_Click(object sender, EventArgs e)
        {
            if (printPaused)
            {
                toolStatus.Text = Trans.T("L_SD_PRINTING...");
                printing = true;
                printPaused = false;
                Main.conn.injectManualCommand("M24");
                return;
            }
            foreach(ListViewItem v in files.SelectedItems) {
                string name = v.Text;
                Main.conn.injectManualCommand("M23 " + (currentDirectory.Length > 0 ? "/" : "") + currentDirectory + name);
                break;
            }
        }

        private void files_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateButtons();
        }

        private void toolMount_Click(object sender, EventArgs e)
        {
            Main.conn.injectManualCommand("M21");
            mounted = true;
            currentDirectory = "";
            RefreshFilenames();
        }

        private void toolUnmount_Click(object sender, EventArgs e)
        {
            Main.conn.injectManualCommand("M22");
            mounted = false;
            currentDirectory = "";
            MessageBox.Show(Trans.T("L_REMOVE_SD_CARD")/*"You can remove the sd card."*/,Trans.T("L_INFORMATION"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (files.SelectedItems.Count == 0) return;
            string fname = (currentDirectory.Length>0?"/":"")+currentDirectory+files.SelectedItems[0].Text;
            //if (MessageBox.Show("Really delete "+fname,"Security question",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
            if (MessageBox.Show(Trans.T1("L_REALLY_DELETE_X",fname), Trans.T("L_SECURITY_QUESTION"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                waitDelete = 6;
                Main.conn.injectManualCommand("M30 " + fname);
            }
        }

        private void SDCard_FormClosing(object sender, FormClosingEventArgs e)
        {
            RegMemory.StoreWindowPos("sdcardWindow", this, false, false);
        }
        public static bool validate83Filename(string t)
        {
            bool ok = true;
            try
            {
                if (t.Length > 12 || t.Length == 0) ok = false;
                int p = t.IndexOf('.');
                if (p > 8) ok = false;
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
            return ok;
        }
        private void toolNewFolder_Click(object sender, EventArgs e)
        {
            string foldername = StringInput.GetString(Trans.T("L_CREATE_FOLDER"), Trans.T("L_CREATE_FOLDER_INFO")).ToLower();
            if (foldername.Length == 0) return;
            int p = foldername.IndexOf('.');
            if (!SDCard.validate83Filename(foldername))
            {
                MessageBox.Show(Trans.T("L_NOT_VALID_83_FILENAME"), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Main.conn.injectManualCommand("M32 " +(currentDirectory.Length>0?"/":"")+currentDirectory + foldername);
            RefreshFilenames();
        }

        private void files_DoubleClick(object sender, EventArgs e)
        {
            if(files.SelectedItems.Count<1) return;
            FileEntry sel = (FileEntry)files.SelectedItems[0].Tag;
            if (sel.isDirectory)
            {
                if (sel.name == "..")
                    currentDirectory = allDirs[sel.folder].folder;
                else 
                    currentDirectory = sel.folder + sel.name + "/";
                fillFiles(currentDirectory);
            }
        }
    }
}
