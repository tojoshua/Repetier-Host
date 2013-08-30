namespace RepetierHost.view.utils
{
    partial class RHUpdater
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RHUpdater));
            this.labelInstalled = new System.Windows.Forms.Label();
            this.labelAvailable = new System.Windows.Forms.Label();
            this.labelInformationOnUpdate = new System.Windows.Forms.Label();
            this.textUpdate = new System.Windows.Forms.TextBox();
            this.buttonSkipVersion = new System.Windows.Forms.Button();
            this.buttonRemindMe = new System.Windows.Forms.Button();
            this.buttonDownload = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelInstalledVersion = new System.Windows.Forms.Label();
            this.labelAvailableVersion = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelInstalled
            // 
            this.labelInstalled.AutoSize = true;
            this.labelInstalled.Location = new System.Drawing.Point(12, 9);
            this.labelInstalled.Name = "labelInstalled";
            this.labelInstalled.Size = new System.Drawing.Size(86, 13);
            this.labelInstalled.TabIndex = 0;
            this.labelInstalled.Text = "Installed version:";
            // 
            // labelAvailable
            // 
            this.labelAvailable.AutoSize = true;
            this.labelAvailable.Location = new System.Drawing.Point(12, 32);
            this.labelAvailable.Name = "labelAvailable";
            this.labelAvailable.Size = new System.Drawing.Size(90, 13);
            this.labelAvailable.TabIndex = 0;
            this.labelAvailable.Text = "Available version:";
            // 
            // labelInformationOnUpdate
            // 
            this.labelInformationOnUpdate.AutoSize = true;
            this.labelInformationOnUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInformationOnUpdate.Location = new System.Drawing.Point(15, 62);
            this.labelInformationOnUpdate.Name = "labelInformationOnUpdate";
            this.labelInformationOnUpdate.Size = new System.Drawing.Size(135, 13);
            this.labelInformationOnUpdate.TabIndex = 1;
            this.labelInformationOnUpdate.Text = "Information on update:";
            // 
            // textUpdate
            // 
            this.textUpdate.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textUpdate.Location = new System.Drawing.Point(15, 88);
            this.textUpdate.Multiline = true;
            this.textUpdate.Name = "textUpdate";
            this.textUpdate.ReadOnly = true;
            this.textUpdate.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textUpdate.Size = new System.Drawing.Size(366, 189);
            this.textUpdate.TabIndex = 2;
            // 
            // buttonSkipVersion
            // 
            this.buttonSkipVersion.Location = new System.Drawing.Point(18, 283);
            this.buttonSkipVersion.Name = "buttonSkipVersion";
            this.buttonSkipVersion.Size = new System.Drawing.Size(117, 23);
            this.buttonSkipVersion.TabIndex = 3;
            this.buttonSkipVersion.Text = "Skip this version";
            this.buttonSkipVersion.UseVisualStyleBackColor = true;
            this.buttonSkipVersion.Click += new System.EventHandler(this.buttonSkipVersion_Click);
            // 
            // buttonRemindMe
            // 
            this.buttonRemindMe.Location = new System.Drawing.Point(141, 283);
            this.buttonRemindMe.Name = "buttonRemindMe";
            this.buttonRemindMe.Size = new System.Drawing.Size(117, 23);
            this.buttonRemindMe.TabIndex = 3;
            this.buttonRemindMe.Text = "Remind me later";
            this.buttonRemindMe.UseVisualStyleBackColor = true;
            this.buttonRemindMe.Click += new System.EventHandler(this.buttonRemindMe_Click);
            // 
            // buttonDownload
            // 
            this.buttonDownload.Location = new System.Drawing.Point(264, 283);
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.Size = new System.Drawing.Size(117, 23);
            this.buttonDownload.TabIndex = 3;
            this.buttonDownload.Text = "Download";
            this.buttonDownload.UseVisualStyleBackColor = true;
            this.buttonDownload.Click += new System.EventHandler(this.buttonDownload_Click);
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Location = new System.Drawing.Point(317, 11);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(64, 64);
            this.panel1.TabIndex = 4;
            // 
            // labelInstalledVersion
            // 
            this.labelInstalledVersion.AutoSize = true;
            this.labelInstalledVersion.Location = new System.Drawing.Point(130, 11);
            this.labelInstalledVersion.Name = "labelInstalledVersion";
            this.labelInstalledVersion.Size = new System.Drawing.Size(35, 13);
            this.labelInstalledVersion.TabIndex = 5;
            this.labelInstalledVersion.Text = "label4";
            // 
            // labelAvailableVersion
            // 
            this.labelAvailableVersion.AutoSize = true;
            this.labelAvailableVersion.Location = new System.Drawing.Point(130, 32);
            this.labelAvailableVersion.Name = "labelAvailableVersion";
            this.labelAvailableVersion.Size = new System.Drawing.Size(35, 13);
            this.labelAvailableVersion.TabIndex = 6;
            this.labelAvailableVersion.Text = "label5";
            // 
            // RHUpdater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 340);
            this.Controls.Add(this.labelAvailableVersion);
            this.Controls.Add(this.labelInstalledVersion);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonDownload);
            this.Controls.Add(this.buttonRemindMe);
            this.Controls.Add(this.buttonSkipVersion);
            this.Controls.Add(this.textUpdate);
            this.Controls.Add(this.labelInformationOnUpdate);
            this.Controls.Add(this.labelAvailable);
            this.Controls.Add(this.labelInstalled);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RHUpdater";
            this.Text = "Repetier-Host Update check";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelInstalled;
        private System.Windows.Forms.Label labelAvailable;
        private System.Windows.Forms.Label labelInformationOnUpdate;
        private System.Windows.Forms.TextBox textUpdate;
        private System.Windows.Forms.Button buttonSkipVersion;
        private System.Windows.Forms.Button buttonRemindMe;
        private System.Windows.Forms.Button buttonDownload;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelInstalledVersion;
        private System.Windows.Forms.Label labelAvailableVersion;
    }
}