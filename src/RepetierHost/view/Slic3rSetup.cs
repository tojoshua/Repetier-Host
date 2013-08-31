using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RepetierHost.model;

namespace RepetierHost.view
{
    public partial class Slic3rSetup : Form
    {
        static Slic3rSetup form = null;
        public static void Execute()
        {
            if (form == null)
            {
                form = new Slic3rSetup();
            }
            if (form.Visible)
                form.BringToFront();
            else
                form.Show();
        }
        public Slic3rSetup()
        {
            InitializeComponent();
            LoadData();
            translate();
            Main.main.languageChanged += translate;
        }
        private void translate()
        {
            Text = Trans.T("W_SLIC3R_SETUP");
            groupSlic3rSetup.Text = Trans.T("L_SLIC3R_SETUP");
            labelLeaveBlankBundle.Text = Trans.T("L_SLIC3R_BLANK_BUNDLE");
            labelSlic3rConfigDir.Text = Trans.T("L_SLIC3R_CONFIG_DIR");
            labelSlic3rExecutable.Text = Trans.T("L_SLIC3R_EXECUTABLE");
            labelSlic3rInfo.Text = Trans.T("L_SLIC3R_INFO");
            labelSlic3rVersion.Text = Trans.T("L_SLIC3R_VERSION");
            labelSlic3rLeaveBlankConfig.Text = Trans.T("L_SLIC3R_BLANK_CONFIG");            
            buttonBrowseConfigDir.Text = Trans.T("B_BROWSE");
            buttonBrowseExecutable.Text = Trans.T("B_BROWSE");
            buttonCancel.Text = Trans.T("B_CANCEL");
            buttonOK.Text = Trans.T("B_OK");
        }
        private void LoadData()
        {
            BasicConfiguration b = BasicConfiguration.basicConf;
            //textIni.Text = b.ExternalSlic3rIniFile;
            //textPath.Text = b.ExternalSlic3rPath;
            textSlic3rConfigDir.Text = b.Slic3rConfigDir;
            textExecutable.Text = b.Slic3rExecutable;
            comboVersion.SelectedIndex = comboVersion.Items.Count - b.Slic3rVersionGroup - 1;
            //checkBoxUseBundledVersion.Checked = b.InternalSlic3rUseBundledVersion;
        }
       /* private void buttonBrowseSlic3r_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                textPath.Text = openFileDialog.FileName;
        }

        private void buttonBrowseIni_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                textIni.Text = openFileDialog.FileName;

        }
        */
        private void buttonOK_Click(object sender, EventArgs e)
        {
            BasicConfiguration b = BasicConfiguration.basicConf;
            //b.InternalSlic3rUseBundledVersion = checkBoxUseBundledVersion.Checked;
            //b.ExternalSlic3rPath = textPath.Text;
            //b.ExternalSlic3rIniFile = textIni.Text;
            b.Slic3rConfigDir = textSlic3rConfigDir.Text;
            b.Slic3rExecutable = textExecutable.Text;
            b.Slic3rVersionGroup = comboVersion.Items.Count - 1 - comboVersion.SelectedIndex;
            Hide();
            Main.slicer.Update();
            Main.main.slicerPanel.UpdateSelection();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            LoadData();
            Hide();
        }

        private void buttonBrowseConfigDir_Click(object sender, EventArgs e)
        {
            if (folderBrowser.ShowDialog() == DialogResult.OK)
                textSlic3rConfigDir.Text = folderBrowser.SelectedPath;
        }

        private void buttonBrowseExecutable_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                textExecutable.Text = openFileDialog.FileName;
        }
    }
}
