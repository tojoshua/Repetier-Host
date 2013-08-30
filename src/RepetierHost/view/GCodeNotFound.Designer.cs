namespace RepetierHost.view
{
    partial class GCodeNotFound
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GCodeNotFound));
            this.labelGCodeNotFoundInfo = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.labelExpectedFilenameLocation = new System.Windows.Forms.Label();
            this.labelFile = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelGCodeNotFoundInfo
            // 
            this.labelGCodeNotFoundInfo.Location = new System.Drawing.Point(13, 13);
            this.labelGCodeNotFoundInfo.Name = "labelGCodeNotFoundInfo";
            this.labelGCodeNotFoundInfo.Size = new System.Drawing.Size(390, 128);
            this.labelGCodeNotFoundInfo.TabIndex = 0;
            this.labelGCodeNotFoundInfo.Text = resources.GetString("labelGCodeNotFoundInfo.Text");
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(28, 200);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(353, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // labelExpectedFilenameLocation
            // 
            this.labelExpectedFilenameLocation.AutoSize = true;
            this.labelExpectedFilenameLocation.Location = new System.Drawing.Point(12, 155);
            this.labelExpectedFilenameLocation.Name = "labelExpectedFilenameLocation";
            this.labelExpectedFilenameLocation.Size = new System.Drawing.Size(158, 13);
            this.labelExpectedFilenameLocation.TabIndex = 2;
            this.labelExpectedFilenameLocation.Text = "Expected filename and location:";
            // 
            // labelFile
            // 
            this.labelFile.AutoSize = true;
            this.labelFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFile.Location = new System.Drawing.Point(15, 172);
            this.labelFile.Name = "labelFile";
            this.labelFile.Size = new System.Drawing.Size(132, 13);
            this.labelFile.TabIndex = 3;
            this.labelFile.Text = "filename and directory";
            // 
            // GCodeNotFound
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 251);
            this.ControlBox = false;
            this.Controls.Add(this.labelFile);
            this.Controls.Add(this.labelExpectedFilenameLocation);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.labelGCodeNotFoundInfo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GCodeNotFound";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Generated G-code not found";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelGCodeNotFoundInfo;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label labelExpectedFilenameLocation;
        private System.Windows.Forms.Label labelFile;
    }
}