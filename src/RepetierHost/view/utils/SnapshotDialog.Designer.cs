namespace RepetierHost.view.utils
{
    partial class SnapshotDialog
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
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.buttonForceSnapshot = new System.Windows.Forms.Button();
            this.buttonResumeJob = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Location = new System.Drawing.Point(13, 9);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(383, 48);
            this.descriptionLabel.TabIndex = 0;
            this.descriptionLabel.Text = "The Job state will be saved once the current layer is completed. You can also for" +
                "ce saving the job state now, or resume printing.";
            // 
            // buttonForceSnapshot
            // 
            this.buttonForceSnapshot.Location = new System.Drawing.Point(243, 69);
            this.buttonForceSnapshot.Name = "buttonForceSnapshot";
            this.buttonForceSnapshot.Size = new System.Drawing.Size(75, 23);
            this.buttonForceSnapshot.TabIndex = 1;
            this.buttonForceSnapshot.Text = "&Force";
            this.buttonForceSnapshot.UseVisualStyleBackColor = true;
            this.buttonForceSnapshot.Click += new System.EventHandler(this.buttonForceSnapshot_Click);
            // 
            // buttonResumeJob
            // 
            this.buttonResumeJob.Location = new System.Drawing.Point(321, 69);
            this.buttonResumeJob.Name = "buttonResumeJob";
            this.buttonResumeJob.Size = new System.Drawing.Size(75, 23);
            this.buttonResumeJob.TabIndex = 2;
            this.buttonResumeJob.Text = "&Resume";
            this.buttonResumeJob.UseVisualStyleBackColor = true;
            this.buttonResumeJob.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // SnapshotDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 100);
            this.Controls.Add(this.buttonResumeJob);
            this.Controls.Add(this.buttonForceSnapshot);
            this.Controls.Add(this.descriptionLabel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SnapshotDialog";
            this.Text = "SnapshotDialog";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Button buttonForceSnapshot;
        private System.Windows.Forms.Button buttonResumeJob;
    }
}