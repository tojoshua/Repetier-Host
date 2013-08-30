namespace RepetierHost.view
{
    partial class Skeinforge
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Skeinforge));
            this.openFile = new System.Windows.Forms.OpenFileDialog();
            this.labelApplication = new System.Windows.Forms.Label();
            this.textSkeinforge = new System.Windows.Forms.TextBox();
            this.buttonSerach = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonAbort = new System.Windows.Forms.Button();
            this.textPython = new System.Windows.Forms.TextBox();
            this.labelPython = new System.Windows.Forms.Label();
            this.buttonSerachPy = new System.Windows.Forms.Button();
            this.openPython = new System.Windows.Forms.OpenFileDialog();
            this.labelCraft = new System.Windows.Forms.Label();
            this.textSkeinforgeCraft = new System.Windows.Forms.TextBox();
            this.buttonSearchCraft = new System.Windows.Forms.Button();
            this.textWorkingDirectory = new System.Windows.Forms.TextBox();
            this.buttonBrowseWorkingDirectory = new System.Windows.Forms.Button();
            this.labelWorkingDirectory = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.textProfilesDir = new System.Windows.Forms.TextBox();
            this.buttonBrowseProfilesDir = new System.Windows.Forms.Button();
            this.labelProfilesDirectory = new System.Windows.Forms.Label();
            this.labelProfdirInfo = new System.Windows.Forms.Label();
            this.textPypy = new System.Windows.Forms.TextBox();
            this.labelPypy = new System.Windows.Forms.Label();
            this.buttonBrosePyPy = new System.Windows.Forms.Button();
            this.labelPypyInfo = new System.Windows.Forms.Label();
            this.labelWorkdirInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // openFile
            // 
            this.openFile.Filter = "Python|*.py|All files|*.*";
            this.openFile.Title = "Skeinforge application";
            // 
            // labelApplication
            // 
            this.labelApplication.AutoSize = true;
            this.labelApplication.Location = new System.Drawing.Point(14, 13);
            this.labelApplication.Name = "labelApplication";
            this.labelApplication.Size = new System.Drawing.Size(115, 13);
            this.labelApplication.TabIndex = 0;
            this.labelApplication.Text = "Skeinforge application:";
            // 
            // textSkeinforge
            // 
            this.textSkeinforge.Location = new System.Drawing.Point(144, 13);
            this.textSkeinforge.Name = "textSkeinforge";
            this.textSkeinforge.Size = new System.Drawing.Size(401, 20);
            this.textSkeinforge.TabIndex = 0;
            // 
            // buttonSerach
            // 
            this.buttonSerach.Location = new System.Drawing.Point(553, 13);
            this.buttonSerach.Name = "buttonSerach";
            this.buttonSerach.Size = new System.Drawing.Size(80, 22);
            this.buttonSerach.TabIndex = 1;
            this.buttonSerach.Text = "Browse ...";
            this.buttonSerach.UseVisualStyleBackColor = true;
            this.buttonSerach.Click += new System.EventHandler(this.buttonSerach_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(74, 308);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(80, 22);
            this.buttonOK.TabIndex = 12;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonAbort
            // 
            this.buttonAbort.Location = new System.Drawing.Point(503, 306);
            this.buttonAbort.Name = "buttonAbort";
            this.buttonAbort.Size = new System.Drawing.Size(80, 22);
            this.buttonAbort.TabIndex = 13;
            this.buttonAbort.Text = "Cancel";
            this.buttonAbort.UseVisualStyleBackColor = true;
            this.buttonAbort.Click += new System.EventHandler(this.buttonAbort_Click);
            // 
            // textPython
            // 
            this.textPython.Location = new System.Drawing.Point(144, 188);
            this.textPython.Name = "textPython";
            this.textPython.Size = new System.Drawing.Size(401, 20);
            this.textPython.TabIndex = 8;
            // 
            // labelPython
            // 
            this.labelPython.AutoSize = true;
            this.labelPython.Location = new System.Drawing.Point(17, 188);
            this.labelPython.Name = "labelPython";
            this.labelPython.Size = new System.Drawing.Size(93, 13);
            this.labelPython.TabIndex = 10;
            this.labelPython.Text = "Python interpreter:";
            // 
            // buttonSerachPy
            // 
            this.buttonSerachPy.Location = new System.Drawing.Point(553, 188);
            this.buttonSerachPy.Name = "buttonSerachPy";
            this.buttonSerachPy.Size = new System.Drawing.Size(80, 22);
            this.buttonSerachPy.TabIndex = 9;
            this.buttonSerachPy.Text = "Browse ...";
            this.buttonSerachPy.UseVisualStyleBackColor = true;
            this.buttonSerachPy.Click += new System.EventHandler(this.buttonSerachPy_Click);
            // 
            // openPython
            // 
            this.openPython.Filter = "All files|*.*";
            this.openPython.Title = "Python interpreter";
            // 
            // labelCraft
            // 
            this.labelCraft.AutoSize = true;
            this.labelCraft.Location = new System.Drawing.Point(14, 38);
            this.labelCraft.Name = "labelCraft";
            this.labelCraft.Size = new System.Drawing.Size(85, 13);
            this.labelCraft.TabIndex = 12;
            this.labelCraft.Text = "Skeinforge craft:";
            // 
            // textSkeinforgeCraft
            // 
            this.textSkeinforgeCraft.Location = new System.Drawing.Point(144, 38);
            this.textSkeinforgeCraft.Name = "textSkeinforgeCraft";
            this.textSkeinforgeCraft.Size = new System.Drawing.Size(401, 20);
            this.textSkeinforgeCraft.TabIndex = 2;
            // 
            // buttonSearchCraft
            // 
            this.buttonSearchCraft.Location = new System.Drawing.Point(553, 38);
            this.buttonSearchCraft.Name = "buttonSearchCraft";
            this.buttonSearchCraft.Size = new System.Drawing.Size(80, 22);
            this.buttonSearchCraft.TabIndex = 3;
            this.buttonSearchCraft.Text = "Browse ...";
            this.buttonSearchCraft.UseVisualStyleBackColor = true;
            this.buttonSearchCraft.Click += new System.EventHandler(this.buttonSearchCraft_Click);
            // 
            // textWorkingDirectory
            // 
            this.textWorkingDirectory.Location = new System.Drawing.Point(144, 64);
            this.textWorkingDirectory.Name = "textWorkingDirectory";
            this.textWorkingDirectory.Size = new System.Drawing.Size(401, 20);
            this.textWorkingDirectory.TabIndex = 4;
            // 
            // buttonBrowseWorkingDirectory
            // 
            this.buttonBrowseWorkingDirectory.Location = new System.Drawing.Point(553, 64);
            this.buttonBrowseWorkingDirectory.Name = "buttonBrowseWorkingDirectory";
            this.buttonBrowseWorkingDirectory.Size = new System.Drawing.Size(80, 22);
            this.buttonBrowseWorkingDirectory.TabIndex = 5;
            this.buttonBrowseWorkingDirectory.Text = "Browse ...";
            this.buttonBrowseWorkingDirectory.UseVisualStyleBackColor = true;
            this.buttonBrowseWorkingDirectory.Click += new System.EventHandler(this.buttonBrowseWorkingDirectory_Click);
            // 
            // labelWorkingDirectory
            // 
            this.labelWorkingDirectory.AutoSize = true;
            this.labelWorkingDirectory.Location = new System.Drawing.Point(14, 64);
            this.labelWorkingDirectory.Name = "labelWorkingDirectory";
            this.labelWorkingDirectory.Size = new System.Drawing.Size(93, 13);
            this.labelWorkingDirectory.TabIndex = 12;
            this.labelWorkingDirectory.Text = "Working directory:";
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Select working directory";
            // 
            // textProfilesDir
            // 
            this.textProfilesDir.Location = new System.Drawing.Point(144, 112);
            this.textProfilesDir.Name = "textProfilesDir";
            this.textProfilesDir.Size = new System.Drawing.Size(401, 20);
            this.textProfilesDir.TabIndex = 6;
            // 
            // buttonBrowseProfilesDir
            // 
            this.buttonBrowseProfilesDir.Location = new System.Drawing.Point(553, 112);
            this.buttonBrowseProfilesDir.Name = "buttonBrowseProfilesDir";
            this.buttonBrowseProfilesDir.Size = new System.Drawing.Size(80, 22);
            this.buttonBrowseProfilesDir.TabIndex = 7;
            this.buttonBrowseProfilesDir.Text = "Browse ...";
            this.buttonBrowseProfilesDir.UseVisualStyleBackColor = true;
            this.buttonBrowseProfilesDir.Click += new System.EventHandler(this.buttonBrowseProfilesDir_Click);
            // 
            // labelProfilesDirectory
            // 
            this.labelProfilesDirectory.AutoSize = true;
            this.labelProfilesDirectory.Location = new System.Drawing.Point(14, 112);
            this.labelProfilesDirectory.Name = "labelProfilesDirectory";
            this.labelProfilesDirectory.Size = new System.Drawing.Size(87, 13);
            this.labelProfilesDirectory.TabIndex = 12;
            this.labelProfilesDirectory.Text = "Profiles directory:";
            // 
            // labelProfdirInfo
            // 
            this.labelProfdirInfo.Location = new System.Drawing.Point(144, 139);
            this.labelProfdirInfo.Name = "labelProfdirInfo";
            this.labelProfdirInfo.Size = new System.Drawing.Size(429, 46);
            this.labelProfdirInfo.TabIndex = 13;
            this.labelProfdirInfo.Text = "Select the profiles subdirectory in the skeinforge configuration directory. This " +
                "is normally\r\n HOME/.skeinforge/profiles. For some custom versions like SFACT the" +
                " path may vary.";
            // 
            // textPypy
            // 
            this.textPypy.Location = new System.Drawing.Point(144, 214);
            this.textPypy.Name = "textPypy";
            this.textPypy.Size = new System.Drawing.Size(401, 20);
            this.textPypy.TabIndex = 10;
            // 
            // labelPypy
            // 
            this.labelPypy.AutoSize = true;
            this.labelPypy.Location = new System.Drawing.Point(17, 214);
            this.labelPypy.Name = "labelPypy";
            this.labelPypy.Size = new System.Drawing.Size(34, 13);
            this.labelPypy.TabIndex = 10;
            this.labelPypy.Text = "PyPy:";
            // 
            // buttonBrosePyPy
            // 
            this.buttonBrosePyPy.Location = new System.Drawing.Point(553, 214);
            this.buttonBrosePyPy.Name = "buttonBrosePyPy";
            this.buttonBrosePyPy.Size = new System.Drawing.Size(80, 22);
            this.buttonBrosePyPy.TabIndex = 11;
            this.buttonBrosePyPy.Text = "Browse ...";
            this.buttonBrosePyPy.UseVisualStyleBackColor = true;
            this.buttonBrosePyPy.Click += new System.EventHandler(this.buttonBrosePyPy_Click);
            // 
            // labelPypyInfo
            // 
            this.labelPypyInfo.Location = new System.Drawing.Point(144, 239);
            this.labelPypyInfo.Name = "labelPypyInfo";
            this.labelPypyInfo.Size = new System.Drawing.Size(401, 58);
            this.labelPypyInfo.TabIndex = 13;
            this.labelPypyInfo.Text = resources.GetString("labelPypyInfo.Text");
            // 
            // labelWorkdirInfo
            // 
            this.labelWorkdirInfo.AutoSize = true;
            this.labelWorkdirInfo.Location = new System.Drawing.Point(144, 90);
            this.labelWorkdirInfo.Name = "labelWorkdirInfo";
            this.labelWorkdirInfo.Size = new System.Drawing.Size(317, 13);
            this.labelWorkdirInfo.TabIndex = 14;
            this.labelWorkdirInfo.Text = "The working directory determines, where SFACT will store profiles!";
            // 
            // Skeinforge
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(643, 369);
            this.ControlBox = false;
            this.Controls.Add(this.labelWorkdirInfo);
            this.Controls.Add(this.labelPypyInfo);
            this.Controls.Add(this.labelProfdirInfo);
            this.Controls.Add(this.labelProfilesDirectory);
            this.Controls.Add(this.labelWorkingDirectory);
            this.Controls.Add(this.labelCraft);
            this.Controls.Add(this.buttonBrosePyPy);
            this.Controls.Add(this.buttonSerachPy);
            this.Controls.Add(this.labelPypy);
            this.Controls.Add(this.labelPython);
            this.Controls.Add(this.textPypy);
            this.Controls.Add(this.textPython);
            this.Controls.Add(this.buttonAbort);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonBrowseProfilesDir);
            this.Controls.Add(this.buttonBrowseWorkingDirectory);
            this.Controls.Add(this.buttonSearchCraft);
            this.Controls.Add(this.buttonSerach);
            this.Controls.Add(this.textProfilesDir);
            this.Controls.Add(this.textWorkingDirectory);
            this.Controls.Add(this.textSkeinforgeCraft);
            this.Controls.Add(this.textSkeinforge);
            this.Controls.Add(this.labelApplication);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Skeinforge";
            this.RightToLeftLayout = true;
            this.ShowInTaskbar = false;
            this.Text = "Skeinforge/SFACT settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Skeinforge_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFile;
        private System.Windows.Forms.Label labelApplication;
        private System.Windows.Forms.Button buttonSerach;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonAbort;
        private System.Windows.Forms.Label labelPython;
        private System.Windows.Forms.Button buttonSerachPy;
        private System.Windows.Forms.OpenFileDialog openPython;
        private System.Windows.Forms.Label labelCraft;
        private System.Windows.Forms.Button buttonSearchCraft;
        public System.Windows.Forms.TextBox textSkeinforge;
        public System.Windows.Forms.TextBox textPython;
        public System.Windows.Forms.TextBox textSkeinforgeCraft;
        public System.Windows.Forms.TextBox textWorkingDirectory;
        private System.Windows.Forms.Button buttonBrowseWorkingDirectory;
        private System.Windows.Forms.Label labelWorkingDirectory;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        public System.Windows.Forms.TextBox textProfilesDir;
        private System.Windows.Forms.Button buttonBrowseProfilesDir;
        private System.Windows.Forms.Label labelProfilesDirectory;
        private System.Windows.Forms.Label labelProfdirInfo;
        public System.Windows.Forms.TextBox textPypy;
        private System.Windows.Forms.Label labelPypy;
        private System.Windows.Forms.Button buttonBrosePyPy;
        private System.Windows.Forms.Label labelPypyInfo;
        private System.Windows.Forms.Label labelWorkdirInfo;
    }
}