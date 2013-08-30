namespace RepetierHost.view
{
    partial class GlobalSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GlobalSettings));
            this.groupFilesAndDirectories = new System.Windows.Forms.GroupBox();
            this.labelOKMasg = new System.Windows.Forms.Label();
            this.labelInfoWorkdir = new System.Windows.Forms.Label();
            this.checkLogfile = new System.Windows.Forms.CheckBox();
            this.buttonSearchWorkdir = new System.Windows.Forms.Button();
            this.textWorkdir = new System.Windows.Forms.TextBox();
            this.labelWorkdir = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonAbort = new System.Windows.Forms.Button();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBehaviour = new System.Windows.Forms.GroupBox();
            this.checkDisableQualityReduction = new System.Windows.Forms.CheckBox();
            this.groupGUI = new System.Windows.Forms.GroupBox();
            this.checkRedGreenSwitch = new System.Windows.Forms.CheckBox();
            this.checkReduceToolbarSize = new System.Windows.Forms.CheckBox();
            this.groupFileAssociations = new System.Windows.Forms.GroupBox();
            this.checkSTL = new System.Windows.Forms.CheckBox();
            this.checkOBJ = new System.Windows.Forms.CheckBox();
            this.checkGCode = new System.Windows.Forms.CheckBox();
            this.checkG = new System.Windows.Forms.CheckBox();
            this.checkGCO = new System.Windows.Forms.CheckBox();
            this.checkNC = new System.Windows.Forms.CheckBox();
            this.buttonAssociate = new System.Windows.Forms.Button();
            this.groupFilesAndDirectories.SuspendLayout();
            this.groupBehaviour.SuspendLayout();
            this.groupGUI.SuspendLayout();
            this.groupFileAssociations.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupFilesAndDirectories
            // 
            this.groupFilesAndDirectories.Controls.Add(this.labelOKMasg);
            this.groupFilesAndDirectories.Controls.Add(this.labelInfoWorkdir);
            this.groupFilesAndDirectories.Controls.Add(this.checkLogfile);
            this.groupFilesAndDirectories.Controls.Add(this.buttonSearchWorkdir);
            this.groupFilesAndDirectories.Controls.Add(this.textWorkdir);
            this.groupFilesAndDirectories.Controls.Add(this.labelWorkdir);
            this.groupFilesAndDirectories.Location = new System.Drawing.Point(13, 12);
            this.groupFilesAndDirectories.Name = "groupFilesAndDirectories";
            this.groupFilesAndDirectories.Size = new System.Drawing.Size(518, 158);
            this.groupFilesAndDirectories.TabIndex = 0;
            this.groupFilesAndDirectories.TabStop = false;
            this.groupFilesAndDirectories.Text = "Files and directories";
            // 
            // labelOKMasg
            // 
            this.labelOKMasg.AutoSize = true;
            this.labelOKMasg.ForeColor = System.Drawing.Color.Red;
            this.labelOKMasg.Location = new System.Drawing.Point(125, 42);
            this.labelOKMasg.Name = "labelOKMasg";
            this.labelOKMasg.Size = new System.Drawing.Size(69, 13);
            this.labelOKMasg.TabIndex = 5;
            this.labelOKMasg.Text = "workdirstatus";
            // 
            // labelInfoWorkdir
            // 
            this.labelInfoWorkdir.Location = new System.Drawing.Point(10, 75);
            this.labelInfoWorkdir.Name = "labelInfoWorkdir";
            this.labelInfoWorkdir.Size = new System.Drawing.Size(491, 52);
            this.labelInfoWorkdir.TabIndex = 4;
            this.labelInfoWorkdir.Text = resources.GetString("labelInfoWorkdir.Text");
            // 
            // checkLogfile
            // 
            this.checkLogfile.AutoSize = true;
            this.checkLogfile.Location = new System.Drawing.Point(10, 51);
            this.checkLogfile.Name = "checkLogfile";
            this.checkLogfile.Size = new System.Drawing.Size(82, 17);
            this.checkLogfile.TabIndex = 2;
            this.checkLogfile.Text = "Log session";
            this.checkLogfile.UseVisualStyleBackColor = true;
            // 
            // buttonSearchWorkdir
            // 
            this.buttonSearchWorkdir.Location = new System.Drawing.Point(400, 20);
            this.buttonSearchWorkdir.Name = "buttonSearchWorkdir";
            this.buttonSearchWorkdir.Size = new System.Drawing.Size(101, 22);
            this.buttonSearchWorkdir.TabIndex = 1;
            this.buttonSearchWorkdir.Text = "Browse ...";
            this.buttonSearchWorkdir.UseVisualStyleBackColor = true;
            this.buttonSearchWorkdir.Click += new System.EventHandler(this.buttonSearchWorkdir_Click);
            // 
            // textWorkdir
            // 
            this.textWorkdir.Location = new System.Drawing.Point(119, 20);
            this.textWorkdir.Name = "textWorkdir";
            this.textWorkdir.Size = new System.Drawing.Size(275, 20);
            this.textWorkdir.TabIndex = 0;
            this.textWorkdir.TextChanged += new System.EventHandler(this.textWorkdir_TextChanged);
            // 
            // labelWorkdir
            // 
            this.labelWorkdir.AutoSize = true;
            this.labelWorkdir.Location = new System.Drawing.Point(7, 23);
            this.labelWorkdir.Name = "labelWorkdir";
            this.labelWorkdir.Size = new System.Drawing.Size(79, 13);
            this.labelWorkdir.TabIndex = 0;
            this.labelWorkdir.Text = "Work directory:";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(365, 418);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(80, 22);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonAbort
            // 
            this.buttonAbort.Location = new System.Drawing.Point(451, 418);
            this.buttonAbort.Name = "buttonAbort";
            this.buttonAbort.Size = new System.Drawing.Size(80, 22);
            this.buttonAbort.TabIndex = 1;
            this.buttonAbort.Text = "Cancel";
            this.buttonAbort.UseVisualStyleBackColor = true;
            this.buttonAbort.Click += new System.EventHandler(this.buttonAbort_Click);
            // 
            // folderBrowser
            // 
            this.folderBrowser.Description = "Select working directory";
            this.folderBrowser.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // groupBehaviour
            // 
            this.groupBehaviour.Controls.Add(this.checkDisableQualityReduction);
            this.groupBehaviour.Location = new System.Drawing.Point(13, 176);
            this.groupBehaviour.Name = "groupBehaviour";
            this.groupBehaviour.Size = new System.Drawing.Size(518, 51);
            this.groupBehaviour.TabIndex = 2;
            this.groupBehaviour.TabStop = false;
            this.groupBehaviour.Text = "Behaviour";
            // 
            // checkDisableQualityReduction
            // 
            this.checkDisableQualityReduction.AutoSize = true;
            this.checkDisableQualityReduction.Location = new System.Drawing.Point(13, 20);
            this.checkDisableQualityReduction.Name = "checkDisableQualityReduction";
            this.checkDisableQualityReduction.Size = new System.Drawing.Size(196, 17);
            this.checkDisableQualityReduction.TabIndex = 0;
            this.checkDisableQualityReduction.Text = "Disable quality reduction during print";
            this.checkDisableQualityReduction.UseVisualStyleBackColor = true;
            // 
            // groupGUI
            // 
            this.groupGUI.Controls.Add(this.checkRedGreenSwitch);
            this.groupGUI.Controls.Add(this.checkReduceToolbarSize);
            this.groupGUI.Location = new System.Drawing.Point(13, 233);
            this.groupGUI.Name = "groupGUI";
            this.groupGUI.Size = new System.Drawing.Size(519, 77);
            this.groupGUI.TabIndex = 3;
            this.groupGUI.TabStop = false;
            this.groupGUI.Text = "GUI";
            // 
            // checkRedGreenSwitch
            // 
            this.checkRedGreenSwitch.AutoSize = true;
            this.checkRedGreenSwitch.Location = new System.Drawing.Point(13, 44);
            this.checkRedGreenSwitch.Name = "checkRedGreenSwitch";
            this.checkRedGreenSwitch.Size = new System.Drawing.Size(244, 17);
            this.checkRedGreenSwitch.TabIndex = 1;
            this.checkRedGreenSwitch.Text = "Use red/green switch buttons (requires restart)";
            this.checkRedGreenSwitch.UseVisualStyleBackColor = true;
            // 
            // checkReduceToolbarSize
            // 
            this.checkReduceToolbarSize.AutoSize = true;
            this.checkReduceToolbarSize.Location = new System.Drawing.Point(13, 20);
            this.checkReduceToolbarSize.Name = "checkReduceToolbarSize";
            this.checkReduceToolbarSize.Size = new System.Drawing.Size(126, 17);
            this.checkReduceToolbarSize.TabIndex = 0;
            this.checkReduceToolbarSize.Text = "Reduce Toolbar Size";
            this.checkReduceToolbarSize.UseVisualStyleBackColor = true;
            this.checkReduceToolbarSize.CheckedChanged += new System.EventHandler(this.checkReduceToolbarSize_CheckedChanged);
            // 
            // groupFileAssociations
            // 
            this.groupFileAssociations.Controls.Add(this.buttonAssociate);
            this.groupFileAssociations.Controls.Add(this.checkNC);
            this.groupFileAssociations.Controls.Add(this.checkGCO);
            this.groupFileAssociations.Controls.Add(this.checkG);
            this.groupFileAssociations.Controls.Add(this.checkGCode);
            this.groupFileAssociations.Controls.Add(this.checkOBJ);
            this.groupFileAssociations.Controls.Add(this.checkSTL);
            this.groupFileAssociations.Location = new System.Drawing.Point(13, 316);
            this.groupFileAssociations.Name = "groupFileAssociations";
            this.groupFileAssociations.Size = new System.Drawing.Size(518, 69);
            this.groupFileAssociations.TabIndex = 4;
            this.groupFileAssociations.TabStop = false;
            this.groupFileAssociations.Text = "File Associations";
            // 
            // checkSTL
            // 
            this.checkSTL.AutoSize = true;
            this.checkSTL.Checked = true;
            this.checkSTL.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkSTL.Location = new System.Drawing.Point(13, 20);
            this.checkSTL.Name = "checkSTL";
            this.checkSTL.Size = new System.Drawing.Size(39, 17);
            this.checkSTL.TabIndex = 0;
            this.checkSTL.Text = ".stl";
            this.checkSTL.UseVisualStyleBackColor = true;
            // 
            // checkOBJ
            // 
            this.checkOBJ.AutoSize = true;
            this.checkOBJ.Checked = true;
            this.checkOBJ.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkOBJ.Location = new System.Drawing.Point(13, 43);
            this.checkOBJ.Name = "checkOBJ";
            this.checkOBJ.Size = new System.Drawing.Size(43, 17);
            this.checkOBJ.TabIndex = 0;
            this.checkOBJ.Text = ".obj";
            this.checkOBJ.UseVisualStyleBackColor = true;
            // 
            // checkGCode
            // 
            this.checkGCode.AutoSize = true;
            this.checkGCode.Checked = true;
            this.checkGCode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkGCode.Location = new System.Drawing.Point(80, 20);
            this.checkGCode.Name = "checkGCode";
            this.checkGCode.Size = new System.Drawing.Size(59, 17);
            this.checkGCode.TabIndex = 0;
            this.checkGCode.Text = ".gcode";
            this.checkGCode.UseVisualStyleBackColor = true;
            // 
            // checkG
            // 
            this.checkG.AutoSize = true;
            this.checkG.Checked = true;
            this.checkG.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkG.Location = new System.Drawing.Point(159, 20);
            this.checkG.Name = "checkG";
            this.checkG.Size = new System.Drawing.Size(35, 17);
            this.checkG.TabIndex = 0;
            this.checkG.Text = ".g";
            this.checkG.UseVisualStyleBackColor = true;
            // 
            // checkGCO
            // 
            this.checkGCO.AutoSize = true;
            this.checkGCO.Checked = true;
            this.checkGCO.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkGCO.Location = new System.Drawing.Point(80, 43);
            this.checkGCO.Name = "checkGCO";
            this.checkGCO.Size = new System.Drawing.Size(47, 17);
            this.checkGCO.TabIndex = 0;
            this.checkGCO.Text = ".gco";
            this.checkGCO.UseVisualStyleBackColor = true;
            // 
            // checkNC
            // 
            this.checkNC.AutoSize = true;
            this.checkNC.Location = new System.Drawing.Point(159, 43);
            this.checkNC.Name = "checkNC";
            this.checkNC.Size = new System.Drawing.Size(41, 17);
            this.checkNC.TabIndex = 0;
            this.checkNC.Text = ".nc";
            this.checkNC.UseVisualStyleBackColor = true;
            // 
            // buttonAssociate
            // 
            this.buttonAssociate.Location = new System.Drawing.Point(372, 20);
            this.buttonAssociate.Name = "buttonAssociate";
            this.buttonAssociate.Size = new System.Drawing.Size(140, 23);
            this.buttonAssociate.TabIndex = 1;
            this.buttonAssociate.Text = "Associate Extensions";
            this.buttonAssociate.UseVisualStyleBackColor = true;
            this.buttonAssociate.Click += new System.EventHandler(this.buttonAssociate_Click);
            // 
            // GlobalSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(544, 494);
            this.ControlBox = false;
            this.Controls.Add(this.groupFileAssociations);
            this.Controls.Add(this.groupGUI);
            this.Controls.Add(this.groupBehaviour);
            this.Controls.Add(this.buttonAbort);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupFilesAndDirectories);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GlobalSettings";
            this.ShowInTaskbar = false;
            this.Text = "Repetier settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GlobalSettings_FormClosing);
            this.groupFilesAndDirectories.ResumeLayout(false);
            this.groupFilesAndDirectories.PerformLayout();
            this.groupBehaviour.ResumeLayout(false);
            this.groupBehaviour.PerformLayout();
            this.groupGUI.ResumeLayout(false);
            this.groupGUI.PerformLayout();
            this.groupFileAssociations.ResumeLayout(false);
            this.groupFileAssociations.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupFilesAndDirectories;
        private System.Windows.Forms.Label labelInfoWorkdir;
        private System.Windows.Forms.CheckBox checkLogfile;
        private System.Windows.Forms.Button buttonSearchWorkdir;
        private System.Windows.Forms.TextBox textWorkdir;
        private System.Windows.Forms.Label labelWorkdir;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonAbort;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
        private System.Windows.Forms.Label labelOKMasg;
        private System.Windows.Forms.GroupBox groupBehaviour;
        private System.Windows.Forms.CheckBox checkDisableQualityReduction;
        private System.Windows.Forms.GroupBox groupGUI;
        private System.Windows.Forms.CheckBox checkReduceToolbarSize;
        private System.Windows.Forms.CheckBox checkRedGreenSwitch;
        private System.Windows.Forms.GroupBox groupFileAssociations;
        private System.Windows.Forms.Button buttonAssociate;
        private System.Windows.Forms.CheckBox checkNC;
        private System.Windows.Forms.CheckBox checkGCO;
        private System.Windows.Forms.CheckBox checkG;
        private System.Windows.Forms.CheckBox checkGCode;
        private System.Windows.Forms.CheckBox checkOBJ;
        private System.Windows.Forms.CheckBox checkSTL;

    }
}