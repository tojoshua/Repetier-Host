namespace RepetierHost.view.utils
{
    partial class PauseInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PauseInfo));
            this.labelInfo = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonContinuePrinting = new System.Windows.Forms.Button();
            this.labelPauseHint = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelInfo
            // 
            this.labelInfo.AutoEllipsis = true;
            this.labelInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInfo.Location = new System.Drawing.Point(83, 13);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(313, 64);
            this.labelInfo.TabIndex = 0;
            this.labelInfo.Text = "label1";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(13, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // buttonContinuePrinting
            // 
            this.buttonContinuePrinting.Location = new System.Drawing.Point(13, 83);
            this.buttonContinuePrinting.Name = "buttonContinuePrinting";
            this.buttonContinuePrinting.Size = new System.Drawing.Size(383, 23);
            this.buttonContinuePrinting.TabIndex = 2;
            this.buttonContinuePrinting.Text = "Continue printing";
            this.buttonContinuePrinting.UseVisualStyleBackColor = true;
            this.buttonContinuePrinting.Click += new System.EventHandler(this.buttonContinuePrinting_Click);
            // 
            // labelPauseHint
            // 
            this.labelPauseHint.Location = new System.Drawing.Point(13, 113);
            this.labelPauseHint.Name = "labelPauseHint";
            this.labelPauseHint.Size = new System.Drawing.Size(383, 56);
            this.labelPauseHint.TabIndex = 3;
            this.labelPauseHint.Text = resources.GetString("labelPauseHint.Text");
            // 
            // PauseInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 178);
            this.ControlBox = false;
            this.Controls.Add(this.labelPauseHint);
            this.Controls.Add(this.buttonContinuePrinting);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.labelInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PauseInfo";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Print paused";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonContinuePrinting;
        private System.Windows.Forms.Label labelPauseHint;
    }
}