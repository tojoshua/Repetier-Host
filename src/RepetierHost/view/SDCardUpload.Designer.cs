namespace RepetierHost.view
{
    partial class SDCardUpload
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SDCardUpload));
            this.labelUploadedFilename = new System.Windows.Forms.Label();
            this.textFilename = new System.Windows.Forms.TextBox();
            this.radioCurrent = new System.Windows.Forms.RadioButton();
            this.radioExternal = new System.Windows.Forms.RadioButton();
            this.labelFile = new System.Windows.Forms.Label();
            this.extFilename = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkAppendPrepend = new System.Windows.Forms.CheckBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.checkJobFinished = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // labelUploadedFilename
            // 
            this.labelUploadedFilename.AutoSize = true;
            this.labelUploadedFilename.Location = new System.Drawing.Point(14, 13);
            this.labelUploadedFilename.Name = "labelUploadedFilename";
            this.labelUploadedFilename.Size = new System.Drawing.Size(98, 13);
            this.labelUploadedFilename.TabIndex = 0;
            this.labelUploadedFilename.Text = "Uploaded filename:";
            // 
            // textFilename
            // 
            this.textFilename.Location = new System.Drawing.Point(153, 10);
            this.textFilename.Name = "textFilename";
            this.textFilename.Size = new System.Drawing.Size(106, 20);
            this.textFilename.TabIndex = 1;
            this.textFilename.Validating += new System.ComponentModel.CancelEventHandler(this.textFilename_Validating);
            // 
            // radioCurrent
            // 
            this.radioCurrent.AutoSize = true;
            this.radioCurrent.Checked = true;
            this.radioCurrent.Location = new System.Drawing.Point(17, 81);
            this.radioCurrent.Name = "radioCurrent";
            this.radioCurrent.Size = new System.Drawing.Size(134, 17);
            this.radioCurrent.TabIndex = 2;
            this.radioCurrent.TabStop = true;
            this.radioCurrent.Text = "Upload current G-Code";
            this.radioCurrent.UseVisualStyleBackColor = true;
            // 
            // radioExternal
            // 
            this.radioExternal.AutoSize = true;
            this.radioExternal.Location = new System.Drawing.Point(17, 104);
            this.radioExternal.Name = "radioExternal";
            this.radioExternal.Size = new System.Drawing.Size(115, 17);
            this.radioExternal.TabIndex = 3;
            this.radioExternal.Text = "Upload external file";
            this.radioExternal.UseVisualStyleBackColor = true;
            // 
            // labelFile
            // 
            this.labelFile.AutoSize = true;
            this.labelFile.Location = new System.Drawing.Point(14, 131);
            this.labelFile.Name = "labelFile";
            this.labelFile.Size = new System.Drawing.Size(26, 13);
            this.labelFile.TabIndex = 4;
            this.labelFile.Text = "File:";
            // 
            // extFilename
            // 
            this.extFilename.Location = new System.Drawing.Point(79, 128);
            this.extFilename.Name = "extFilename";
            this.extFilename.Size = new System.Drawing.Size(301, 20);
            this.extFilename.TabIndex = 5;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(386, 123);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(80, 22);
            this.buttonBrowse.TabIndex = 6;
            this.buttonBrowse.Text = "Browse ...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(279, 170);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(80, 22);
            this.buttonOK.TabIndex = 7;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(386, 170);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(80, 22);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "G_Code|*.gcode|All files|*.*";
            this.openFileDialog.SupportMultiDottedExtensions = true;
            this.openFileDialog.Title = "Select G-Code file for upload";
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Location = new System.Drawing.Point(403, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(64, 64);
            this.panel1.TabIndex = 9;
            // 
            // checkAppendPrepend
            // 
            this.checkAppendPrepend.AutoSize = true;
            this.checkAppendPrepend.Checked = true;
            this.checkAppendPrepend.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkAppendPrepend.Location = new System.Drawing.Point(17, 37);
            this.checkAppendPrepend.Name = "checkAppendPrepend";
            this.checkAppendPrepend.Size = new System.Drawing.Size(176, 17);
            this.checkAppendPrepend.TabIndex = 10;
            this.checkAppendPrepend.Text = "Include \"Start\" and \"End\" code";
            this.checkAppendPrepend.UseVisualStyleBackColor = true;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // checkJobFinished
            // 
            this.checkJobFinished.AutoSize = true;
            this.checkJobFinished.Checked = true;
            this.checkJobFinished.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkJobFinished.Location = new System.Drawing.Point(17, 59);
            this.checkJobFinished.Name = "checkJobFinished";
            this.checkJobFinished.Size = new System.Drawing.Size(171, 17);
            this.checkJobFinished.TabIndex = 11;
            this.checkJobFinished.Text = "Include job finished commands";
            this.checkJobFinished.UseVisualStyleBackColor = true;
            // 
            // SDCardUpload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(479, 236);
            this.ControlBox = false;
            this.Controls.Add(this.checkJobFinished);
            this.Controls.Add(this.checkAppendPrepend);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.extFilename);
            this.Controls.Add(this.labelFile);
            this.Controls.Add(this.radioExternal);
            this.Controls.Add(this.radioCurrent);
            this.Controls.Add(this.textFilename);
            this.Controls.Add(this.labelUploadedFilename);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SDCardUpload";
            this.Text = "SD-Card upload";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SDCardUpload_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelUploadedFilename;
        private System.Windows.Forms.Label labelFile;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.CheckBox checkAppendPrepend;
        private System.Windows.Forms.ErrorProvider errorProvider;
        public System.Windows.Forms.TextBox textFilename;
        public System.Windows.Forms.RadioButton radioCurrent;
        public System.Windows.Forms.RadioButton radioExternal;
        public System.Windows.Forms.TextBox extFilename;
        public System.Windows.Forms.CheckBox checkJobFinished;
    }
}