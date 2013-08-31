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
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using RepetierHost.view.utils;
using RepetierHost.model;

namespace RepetierHost.view
{
    public partial class Skeinforge : Form
    {
        public RegistryKey repetierKey;
        Process procSkein = null;
        Process procConvert = null;
        string slicefile = null;
        SkeinConfig profileConfig = null;
        SkeinConfig exportConfig = null;
        SkeinConfig extrusionConfig = null;
        SkeinConfig multiplyConfig = null;
        string name = "Skeinforge";

        public Skeinforge()
        {
            InitializeComponent();
            RegMemory.RestoreWindowPos("skeinforgeWindow", this);
            repetierKey = Custom.BaseKey; // Registry.CurrentUser.CreateSubKey("SOFTWARE\\Repetier");
            regToForm();
            translate();
            if (BasicConfiguration.basicConf.SkeinforgeProfileDir.IndexOf("sfact") >= 0)
                name = "SFACT";
            else
                name = "Skeinforge";
            Main.main.languageChanged += translate;
        }
        private void translate()
        {
            Text = Trans.T("W_SKEIN_SETTINGS");
            labelApplication.Text = Trans.T("L_SKEIN_APPLICARTION");
            labelCraft.Text = Trans.T("L_SKEIN_CRAFT");
            labelProfdirInfo.Text = Trans.T("L_SKEIN_PROFDIR_INFO");
            labelProfilesDirectory.Text = Trans.T("L_SKEIN_PROFILES_DIRECTORY");
            labelPypy.Text = Trans.T("L_SKEIN_PYPY");
            labelPypyInfo.Text = Trans.T("L_SKEIN_PYPY_INFO");
            labelPython.Text = Trans.T("L_SKEIN_PYTHON");
            labelWorkdirInfo.Text = Trans.T("L_SKEIN_WORKDIR_INFO");
            labelWorkingDirectory.Text = Trans.T("L_SKEIN_WORKING_DIRECTORY");
            openFile.Title = Trans.T("L_SKEIN_OPEN_FILE");
            openPython.Title = Trans.T("L_SKEIN_OPEN_PYTHON");
            buttonAbort.Text = Trans.T("B_CANCEL");
            buttonOK.Text = Trans.T("B_OK");
            buttonSearchCraft.Text = Trans.T("B_BROWSE");
            buttonSerach.Text = Trans.T("B_BROWSE");
            buttonSerachPy.Text = Trans.T("B_BROWSE");
            buttonBrosePyPy.Text = Trans.T("B_BROWSE");
            buttonBrowseProfilesDir.Text = Trans.T("B_BROWSE");
            buttonBrowseWorkingDirectory.Text = Trans.T("B_BROWSE");
            
        }
        public string wrapQuotes(string text)
        {
            if (text.StartsWith("\"") && text.EndsWith("\"")) return text;
            return "\"" + text.Replace("\"", "\\\"") + "\"";
        }
        public void RestoreConfigs()
        {
            if (profileConfig != null)
                profileConfig.writeOriginal();
            if (exportConfig != null)
                exportConfig.writeOriginal();
            if (extrusionConfig != null)
                extrusionConfig.writeOriginal();
            if (multiplyConfig != null)
                multiplyConfig.writeOriginal();
            profileConfig = null;
            exportConfig = null;
            extrusionConfig = null;
            multiplyConfig = null;
        }
        public void RunSkeinforge()
        {
            if (procSkein != null)
            {
                return;
            }
            string python = findPythonw();
            if (python == null)
            {
                MessageBox.Show(Trans.T("L_PYTHON_NOT_FOUND"), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string sk = findSkeinforge();
            if (sk == null)
            {
                MessageBox.Show(Trans.T("L_SKEINFORGE_NOT_FOUND"), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            procSkein = new Process();
            try
            {
                procSkein.EnableRaisingEvents = true;
                procSkein.Exited += new EventHandler(SkeinExited);
                procSkein.StartInfo.FileName = Main.IsMono ? python : wrapQuotes(python);
                procSkein.StartInfo.Arguments = wrapQuotes(sk);
                procSkein.StartInfo.WorkingDirectory = textWorkingDirectory.Text;
                procSkein.StartInfo.UseShellExecute = false;
                procSkein.StartInfo.RedirectStandardOutput = true;
                procSkein.OutputDataReceived += new DataReceivedEventHandler(OutputDataHandler);
                procSkein.StartInfo.RedirectStandardError = true;
                procSkein.ErrorDataReceived += new DataReceivedEventHandler(OutputDataHandler);
                procSkein.Start();
                // Start the asynchronous read of the standard output stream.
                procSkein.BeginOutputReadLine();
                procSkein.BeginErrorReadLine();
            }
            catch (Exception e)
            {
                Main.conn.log(e.ToString(), false, 2);
            }
        }
        public void KillSlice()
        {
            if (procConvert != null)
            {
                procConvert.Kill();
                procConvert = null;
                Main.conn.log(Trans.T1("L_SKEIN_KILLED",name),false,2); //"Skeinforge slicing process killed on user request.", false, 2);
                RestoreConfigs();
            }
        }
        public string findSkeinforgeProfiles()
        {
            if (Directory.Exists(textProfilesDir.Text))
                return textProfilesDir.Text;
            string test = ((Environment.OSVersion.Platform == PlatformID.Unix ||
                   Environment.OSVersion.Platform == PlatformID.MacOSX)
    ? Environment.GetEnvironmentVariable("HOME")
    : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%")) + Path.DirectorySeparatorChar + ".skeinforge"+Path.DirectorySeparatorChar+"profiles";
            if (Directory.Exists(test)) return test;
            return null;
        }
        public string findPypy()
        {
            if (File.Exists(textPypy.Text)) // use preconfigured
                return textPypy.Text;
            string[] possibleNames = { "pypy.exe", "pypy"};
            if (textPypy.Text.Length > 1)
            {
                if(File.Exists(textPypy.Text))
                    return textPypy.Text;
            }
            // Search in PATH environment var
            foreach (string test in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator))
            {
                string path = test.Trim();
                foreach (string exname in possibleNames) // Search bundled version
                {
                    if (!String.IsNullOrEmpty(path) && File.Exists(Path.Combine(path, exname)))
                        return Path.GetFullPath(Path.Combine(path, exname));
                }
            }
            string[] possibleNames2 = { "python.exe", "python2","python"};
            foreach (string test in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator))
            {
                string path = test.Trim();
                foreach (string exname in possibleNames2) // Search bundled version
                {
                    if (!String.IsNullOrEmpty(path) && File.Exists(Path.Combine(path, exname)))
                        return Path.GetFullPath(Path.Combine(path, exname));
                }
            }

            return findPythonw();
        }
        public string findPythonw()
        {
            if (File.Exists(textPython.Text)) // use preconfigured
                return textPython.Text;
            string[] possibleNames = { "pythonw.exe", "python2","python" };
            if (textPypy.Text.Length > 1)
            {
                if (File.Exists(textPypy.Text))
                    return textPypy.Text;
            }
            // Search in PATH environment var
            foreach (string test in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator))
            {
                string path = test.Trim();
                foreach (string exname in possibleNames) // Search bundled version
                {
                    if (!String.IsNullOrEmpty(path) && File.Exists(Path.Combine(path, exname)))
                        return Path.GetFullPath(Path.Combine(path, exname));
                }
            }
            return null;
        }
        public string findCraft()
        {
            if(textSkeinforgeCraft.Text.Length>1 && File.Exists(textSkeinforgeCraft.Text)) return textSkeinforgeCraft.Text;
            if (File.Exists("/usr/lib/python2.7/site-packages/skeinforge/skeinforge_application/skeinforge_utilities/skeinforge_craft.py"))
                return "/usr/lib/python2.7/site-packages/skeinforge/skeinforge_application/skeinforge_utilities/skeinforge_craft.py";
            return null;
        }
        public string findSkeinforge()
        {
            if (textSkeinforge.Text.Length > 1 && File.Exists(textSkeinforge.Text)) return textSkeinforge.Text;
            if (File.Exists("/usr/lib/python2.7/site-packages/skeinforge/skeinforge_application/skeinforge.py"))
                return "/usr/lib/python2.7/site-packages/skeinforge/skeinforge_application/skeinforge.py";
            return null;
        }
        public string PyPy
        {
            get
            {
                return findPypy();
            }
        }
        public void RunSlice(string file, string profile)
        {
            if (procConvert != null)
            {
                MessageBox.Show(Trans.T("L_SKEIN_STILL_RUNNING") /*"Last slice job still running. Slicing of new job is canceled."*/,Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string py = PyPy;
            if (py == null)
            {
                MessageBox.Show(Trans.T("L_PYPY_NOT_FOUND"), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string craft = findCraft();
            if (craft == null)
            {
                MessageBox.Show(Trans.T("L_SKEINCRAFT_NOT_FOUND"), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string profdir = findSkeinforgeProfiles();
            if (profdir == null)
            {
                MessageBox.Show(Trans.T("L_SKEINCRAFT_PROFILES_NOT_FOUND"), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            profileConfig = new SkeinConfig(Path.Combine(profdir,"skeinforge_profile.csv"));
            extrusionConfig = new SkeinConfig(Path.Combine(profdir,"extrusion.csv"));
            exportConfig = new SkeinConfig(Path.Combine(profdir,"extrusion" +
                Path.DirectorySeparatorChar + profile + Path.DirectorySeparatorChar + "export.csv"));
            multiplyConfig = new SkeinConfig(Path.Combine(profdir,"extrusion" +
                Path.DirectorySeparatorChar + profile + Path.DirectorySeparatorChar + "multiply.csv"));
            // Set profile to extrusion
            /* cutting	False
extrusion	True
milling	False
winding	False
*/
            profileConfig.setValue("cutting", "False");
            profileConfig.setValue("milling", "False");
            profileConfig.setValue("extrusion", "True");
            profileConfig.setValue("winding", "False");
            profileConfig.writeModified();
            // Set used profile
            extrusionConfig.setValue("Profile Selection:", profile);
            extrusionConfig.writeModified();
            // Set export to correct values
            exportConfig.setValue("Activate Export", "True");
            exportConfig.setValue("Add Profile Extension", "False");
            exportConfig.setValue("Add Profile Name to Filename", "False");
            exportConfig.setValue("Add Timestamp Extension", "False");
            exportConfig.setValue("Add Timestamp to Filename", "False");
            exportConfig.setValue("Add Description to Filename", "False");
            exportConfig.setValue("Add Descriptive Extension", "False");
            exportConfig.writeModified();

            multiplyConfig.setValue("Activate Multiply:", "False");
            multiplyConfig.setValue("Activate Multiply: ", "False");
            multiplyConfig.setValue("Activate Multiply", "False");
            multiplyConfig.writeModified();

            string target = StlToGCode(file);
            if (File.Exists(target))
                File.Delete(target);
            procConvert = new Process();
            try
            {
                SlicingInfo.Start(name);
                SlicingInfo.SetAction(Trans.T("L_SLICING_STL_FILE...")); //"Slicing STL file ...");
                slicefile = file;
                procConvert.EnableRaisingEvents = true;
                procConvert.Exited += new EventHandler(ConversionExited);

                procConvert.StartInfo.FileName = Main.IsMono ? py : wrapQuotes(py);
                procConvert.StartInfo.Arguments = wrapQuotes(craft) + " " + wrapQuotes(file);
                procConvert.StartInfo.UseShellExecute = false;
                procConvert.StartInfo.WorkingDirectory = textWorkingDirectory.Text;
                procConvert.StartInfo.RedirectStandardOutput = true;
                procConvert.OutputDataReceived += new DataReceivedEventHandler(OutputDataHandler);
                procConvert.StartInfo.RedirectStandardError = true;
                procConvert.ErrorDataReceived += new DataReceivedEventHandler(OutputDataHandler);
                procConvert.Start();
                // Start the asynchronous read of the standard output stream.
                procConvert.BeginOutputReadLine();
                procConvert.BeginErrorReadLine();
                //Main.main.tab.SelectedTab = Main.main.tabPrint;
            }
            catch (Exception e)
            {
                Main.conn.log(e.ToString(), false, 2);
                RestoreConfigs();
            }
        }
        public delegate void LoadGCode(String myString);
        private void ConversionExited(object sender, System.EventArgs e)
        {
            if (procConvert == null) return;
            try
            {
                procConvert.Close();
                procConvert = null;
                string gcodefile = StlToGCode(slicefile);
                Main.slicer.Postprocess(gcodefile);
                RestoreConfigs();
            }
            catch { }
        }
        private void SkeinExited(object sender, System.EventArgs e)
        {
            procSkein.Close();
            procSkein = null;
            Main.main.Invoke(Main.main.slicerPanel.UpdateSelectionInvoker);
        }
        private static void OutputDataHandler(object sendingProcess,
            DataReceivedEventArgs outLine)
        {
            // Collect the net view command output.
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                string[] lines = outLine.Data.Split((char)0x0d);
                foreach (string l in lines)
                    Main.conn.log("<"+Main.main.skeinforge.name+"> " + l, false, 4);
            }
        }

        public string StlToGCode(string stl)
        {
            int p = stl.LastIndexOf('.');
            if (p > 0) stl = stl.Substring(0, p);
            string extension = exportConfig.getValue("File Extension:");
            if (extension == null)
                extension = exportConfig.getValue("File Extension (gcode):");
            string export = exportConfig.getValue("Add Export Suffix");
            if (export == null)
                export = exportConfig.getValue("Add _export to filename (filename_export)");
            if (export == null || export != "True") export = ""; else export = "_export";
            return stl + export + "." + extension;
        }
        private void regToForm()
        {

            textSkeinforge.Text = (string)repetierKey.GetValue("SkeinforgePath", textSkeinforge.Text);
            textSkeinforgeCraft.Text = (string)repetierKey.GetValue("SkeinforgeCraftPath", textSkeinforgeCraft.Text);
            textPython.Text = (string)repetierKey.GetValue("SkeinforgePython", textPython.Text);
            textPypy.Text = (string)repetierKey.GetValue("SkeinforgePypy", textPypy.Text);
            //textExtension.Text = (string)repetierKey.GetValue("SkeinforgeExtension", textExtension.Text);
            //textPostfix.Text = (string)repetierKey.GetValue("SkeinforgePostfix", textPostfix.Text);
            textWorkingDirectory.Text = (string)repetierKey.GetValue("SkeinforgeWorkdir", textWorkingDirectory.Text);
            textProfilesDir.Text = BasicConfiguration.basicConf.SkeinforgeProfileDir;
        }
        private void FormToReg()
        {
            BasicConfiguration.basicConf.SkeinforgeProfileDir = textProfilesDir.Text;
            repetierKey.SetValue("SkeinforgePath", textSkeinforge.Text);
            repetierKey.SetValue("SkeinforgeCraftPath", textSkeinforgeCraft.Text);
            repetierKey.SetValue("SkeinforgePython", textPython.Text);
            repetierKey.SetValue("SkeinforgePypy", textPypy.Text);
            //repetierKey.SetValue("SkeinforgeExtension", textExtension.Text);
            //repetierKey.SetValue("SkeinforgePostfix", textPostfix.Text);
            repetierKey.SetValue("SkeinforgeWorkdir", textWorkingDirectory.Text);
        }
        private void buttonAbort_Click(object sender, EventArgs e)
        {
            regToForm();
            Hide();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            FormToReg();
            Hide();
            if (BasicConfiguration.basicConf.SkeinforgeProfileDir.IndexOf("sfact") >= 0)
                name = "SFACT";
            else
                name = "Skeinforge";
            Main.main.languageChanged += translate;
            Main.slicer.Update();
            Main.main.slicerPanel.UpdateSelection();
        }

        private void buttonSerach_Click(object sender, EventArgs e)
        {
            openFile.Title = Trans.T("L_SKEIN_OPEN_FILE");
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                textSkeinforge.Text = openFile.FileName;
            }
        }

        private void buttonSearchCraft_Click(object sender, EventArgs e)
        {
            openFile.Title = Trans.T("L_SKEIN_OPEN_CRAFT");
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                textSkeinforgeCraft.Text = openFile.FileName;
            }
        }

        private void buttonSerachPy_Click(object sender, EventArgs e)
        {
            openPython.Title = Trans.T("L_SKEIN_OPEN_PYTHON");
            if (openPython.ShowDialog() == DialogResult.OK)
                textPython.Text = openPython.FileName;
        }

        private void Skeinforge_FormClosing(object sender, FormClosingEventArgs e)
        {
            RegMemory.StoreWindowPos("skeinforgeWindow", this, false, false);
        }

        private void buttonBrowseWorkingDirectory_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.Description = Trans.T("L_SKEIN_SELECT_WORKING_FOLDER");
            folderBrowserDialog.SelectedPath = textWorkingDirectory.Text;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textWorkingDirectory.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void buttonBrowseProfilesDir_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.Description = Trans.T("L_SKEIN_SELECT_PROFILE_FOLDER");
            folderBrowserDialog.SelectedPath = textProfilesDir.Text;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textProfilesDir.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void buttonBrosePyPy_Click(object sender, EventArgs e)
        {
            openPython.Title = Trans.T("L_SKEIN_OPEN_PYPY");
            if (openPython.ShowDialog() == DialogResult.OK)
                textPypy.Text = openPython.FileName;
        }
    }
}
