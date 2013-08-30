namespace RepetierHost.view
{
    partial class About
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.textLicence = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelLicenceAndLibraries = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.labelRepetierInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(13, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(137, 126);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // textLicence
            // 
            this.textLicence.Location = new System.Drawing.Point(13, 165);
            this.textLicence.Multiline = true;
            this.textLicence.Name = "textLicence";
            this.textLicence.ReadOnly = true;
            this.textLicence.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textLicence.Size = new System.Drawing.Size(592, 235);
            this.textLicence.TabIndex = 1;
            this.textLicence.Text = resources.GetString("textLicence.Text");
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(269, 414);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(80, 22);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // labelLicenceAndLibraries
            // 
            this.labelLicenceAndLibraries.AutoSize = true;
            this.labelLicenceAndLibraries.Location = new System.Drawing.Point(14, 145);
            this.labelLicenceAndLibraries.Name = "labelLicenceAndLibraries";
            this.labelLicenceAndLibraries.Size = new System.Drawing.Size(108, 13);
            this.labelLicenceAndLibraries.TabIndex = 3;
            this.labelLicenceAndLibraries.Text = "Licence and Libraries";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(157, 25);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(128, 13);
            this.linkLabel1.TabIndex = 4;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "https://www.repetier.com";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(157, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(272, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Repetier-Host (C) 2011-2013 Hot-World GmbH && Co. KG";
            // 
            // labelRepetierInfo
            // 
            this.labelRepetierInfo.Location = new System.Drawing.Point(160, 52);
            this.labelRepetierInfo.Name = "labelRepetierInfo";
            this.labelRepetierInfo.Size = new System.Drawing.Size(445, 86);
            this.labelRepetierInfo.TabIndex = 6;
            this.labelRepetierInfo.Text = resources.GetString("labelRepetierInfo.Text");
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(618, 448);
            this.ControlBox = false;
            this.Controls.Add(this.labelRepetierInfo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.labelLicenceAndLibraries);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textLicence);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "About";
            this.ShowInTaskbar = false;
            this.Text = "About Repetier-Host";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.About_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox textLicence;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label labelLicenceAndLibraries;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelRepetierInfo;
    }
}