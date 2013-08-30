namespace RepetierHost.view
{
    partial class Slic3rSetup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Slic3rSetup));
            this.groupSlic3rSetup = new System.Windows.Forms.GroupBox();
            this.labelSlic3rInfo = new System.Windows.Forms.Label();
            this.labelLeaveBlankBundle = new System.Windows.Forms.Label();
            this.labelSlic3rLeaveBlankConfig = new System.Windows.Forms.Label();
            this.labelSlic3rExecutable = new System.Windows.Forms.Label();
            this.labelSlic3rConfigDir = new System.Windows.Forms.Label();
            this.buttonBrowseExecutable = new System.Windows.Forms.Button();
            this.buttonBrowseConfigDir = new System.Windows.Forms.Button();
            this.textExecutable = new System.Windows.Forms.TextBox();
            this.textSlic3rConfigDir = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.labelSlic3rVersion = new System.Windows.Forms.Label();
            this.comboVersion = new System.Windows.Forms.ComboBox();
            this.groupSlic3rSetup.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupSlic3rSetup
            // 
            this.groupSlic3rSetup.Controls.Add(this.comboVersion);
            this.groupSlic3rSetup.Controls.Add(this.labelSlic3rInfo);
            this.groupSlic3rSetup.Controls.Add(this.labelLeaveBlankBundle);
            this.groupSlic3rSetup.Controls.Add(this.labelSlic3rLeaveBlankConfig);
            this.groupSlic3rSetup.Controls.Add(this.labelSlic3rVersion);
            this.groupSlic3rSetup.Controls.Add(this.labelSlic3rExecutable);
            this.groupSlic3rSetup.Controls.Add(this.labelSlic3rConfigDir);
            this.groupSlic3rSetup.Controls.Add(this.buttonBrowseExecutable);
            this.groupSlic3rSetup.Controls.Add(this.buttonBrowseConfigDir);
            this.groupSlic3rSetup.Controls.Add(this.textExecutable);
            this.groupSlic3rSetup.Controls.Add(this.textSlic3rConfigDir);
            this.groupSlic3rSetup.Location = new System.Drawing.Point(13, 13);
            this.groupSlic3rSetup.Name = "groupSlic3rSetup";
            this.groupSlic3rSetup.Size = new System.Drawing.Size(433, 298);
            this.groupSlic3rSetup.TabIndex = 0;
            this.groupSlic3rSetup.TabStop = false;
            this.groupSlic3rSetup.Text = "Slic3r setup";
            // 
            // labelSlic3rInfo
            // 
            this.labelSlic3rInfo.Location = new System.Drawing.Point(3, 226);
            this.labelSlic3rInfo.Name = "labelSlic3rInfo";
            this.labelSlic3rInfo.Size = new System.Drawing.Size(402, 66);
            this.labelSlic3rInfo.TabIndex = 8;
            this.labelSlic3rInfo.Text = resources.GetString("labelSlic3rInfo.Text");
            // 
            // labelLeaveBlankBundle
            // 
            this.labelLeaveBlankBundle.AutoSize = true;
            this.labelLeaveBlankBundle.Location = new System.Drawing.Point(3, 127);
            this.labelLeaveBlankBundle.Name = "labelLeaveBlankBundle";
            this.labelLeaveBlankBundle.Size = new System.Drawing.Size(197, 13);
            this.labelLeaveBlankBundle.TabIndex = 7;
            this.labelLeaveBlankBundle.Text = "Leave blank to use the bundled version.";
            // 
            // labelSlic3rLeaveBlankConfig
            // 
            this.labelSlic3rLeaveBlankConfig.AutoSize = true;
            this.labelSlic3rLeaveBlankConfig.Location = new System.Drawing.Point(6, 65);
            this.labelSlic3rLeaveBlankConfig.Name = "labelSlic3rLeaveBlankConfig";
            this.labelSlic3rLeaveBlankConfig.Size = new System.Drawing.Size(184, 13);
            this.labelSlic3rLeaveBlankConfig.TabIndex = 7;
            this.labelSlic3rLeaveBlankConfig.Text = "Leave blank to use guessed location.";
            // 
            // labelSlic3rExecutable
            // 
            this.labelSlic3rExecutable.AutoSize = true;
            this.labelSlic3rExecutable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSlic3rExecutable.Location = new System.Drawing.Point(3, 87);
            this.labelSlic3rExecutable.Name = "labelSlic3rExecutable";
            this.labelSlic3rExecutable.Size = new System.Drawing.Size(105, 13);
            this.labelSlic3rExecutable.TabIndex = 0;
            this.labelSlic3rExecutable.Text = "Slic3r executable";
            // 
            // labelSlic3rConfigDir
            // 
            this.labelSlic3rConfigDir.AutoSize = true;
            this.labelSlic3rConfigDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSlic3rConfigDir.Location = new System.Drawing.Point(6, 25);
            this.labelSlic3rConfigDir.Name = "labelSlic3rConfigDir";
            this.labelSlic3rConfigDir.Size = new System.Drawing.Size(170, 13);
            this.labelSlic3rConfigDir.TabIndex = 0;
            this.labelSlic3rConfigDir.Text = "Slic3r configuration directory";
            // 
            // buttonBrowseExecutable
            // 
            this.buttonBrowseExecutable.Location = new System.Drawing.Point(327, 101);
            this.buttonBrowseExecutable.Name = "buttonBrowseExecutable";
            this.buttonBrowseExecutable.Size = new System.Drawing.Size(97, 23);
            this.buttonBrowseExecutable.TabIndex = 3;
            this.buttonBrowseExecutable.Text = "Browse";
            this.buttonBrowseExecutable.UseVisualStyleBackColor = true;
            this.buttonBrowseExecutable.Click += new System.EventHandler(this.buttonBrowseExecutable_Click);
            // 
            // buttonBrowseConfigDir
            // 
            this.buttonBrowseConfigDir.Location = new System.Drawing.Point(330, 39);
            this.buttonBrowseConfigDir.Name = "buttonBrowseConfigDir";
            this.buttonBrowseConfigDir.Size = new System.Drawing.Size(97, 23);
            this.buttonBrowseConfigDir.TabIndex = 1;
            this.buttonBrowseConfigDir.Text = "Browse";
            this.buttonBrowseConfigDir.UseVisualStyleBackColor = true;
            this.buttonBrowseConfigDir.Click += new System.EventHandler(this.buttonBrowseConfigDir_Click);
            // 
            // textExecutable
            // 
            this.textExecutable.Location = new System.Drawing.Point(6, 104);
            this.textExecutable.Name = "textExecutable";
            this.textExecutable.Size = new System.Drawing.Size(315, 20);
            this.textExecutable.TabIndex = 2;
            // 
            // textSlic3rConfigDir
            // 
            this.textSlic3rConfigDir.Location = new System.Drawing.Point(9, 42);
            this.textSlic3rConfigDir.Name = "textSlic3rConfigDir";
            this.textSlic3rConfigDir.Size = new System.Drawing.Size(315, 20);
            this.textSlic3rConfigDir.TabIndex = 0;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(226, 338);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(343, 338);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Title = "Slic3r setup";
            // 
            // folderBrowser
            // 
            this.folderBrowser.RootFolder = System.Environment.SpecialFolder.ApplicationData;
            // 
            // labelSlic3rVersion
            // 
            this.labelSlic3rVersion.AutoSize = true;
            this.labelSlic3rVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSlic3rVersion.Location = new System.Drawing.Point(3, 149);
            this.labelSlic3rVersion.Name = "labelSlic3rVersion";
            this.labelSlic3rVersion.Size = new System.Drawing.Size(85, 13);
            this.labelSlic3rVersion.TabIndex = 0;
            this.labelSlic3rVersion.Text = "Slic3r Version";
            // 
            // comboVersion
            // 
            this.comboVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboVersion.FormattingEnabled = true;
            this.comboVersion.Items.AddRange(new object[] {
            "0.9.9 or higher",
            "0.9.0 - 0.9.8"});
            this.comboVersion.Location = new System.Drawing.Point(7, 166);
            this.comboVersion.Name = "comboVersion";
            this.comboVersion.Size = new System.Drawing.Size(314, 21);
            this.comboVersion.TabIndex = 9;
            // 
            // Slic3rSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(461, 390);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupSlic3rSetup);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Slic3rSetup";
            this.Text = "Slic3r Setup";
            this.groupSlic3rSetup.ResumeLayout(false);
            this.groupSlic3rSetup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupSlic3rSetup;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label labelSlic3rLeaveBlankConfig;
        private System.Windows.Forms.Label labelSlic3rConfigDir;
        private System.Windows.Forms.Button buttonBrowseConfigDir;
        private System.Windows.Forms.TextBox textSlic3rConfigDir;
        private System.Windows.Forms.Label labelSlic3rInfo;
        private System.Windows.Forms.Label labelLeaveBlankBundle;
        private System.Windows.Forms.Label labelSlic3rExecutable;
        private System.Windows.Forms.Button buttonBrowseExecutable;
        private System.Windows.Forms.TextBox textExecutable;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
        private System.Windows.Forms.ComboBox comboVersion;
        private System.Windows.Forms.Label labelSlic3rVersion;
    }
}