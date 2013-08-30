namespace RepetierHost.view
{
    partial class StoreCode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StoreCode));
            this.checkIncludeStartEnd = new System.Windows.Forms.CheckBox();
            this.checkBinary = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelSaveDirectInfo = new System.Windows.Forms.Label();
            this.checkIncludeJobFinished = new System.Windows.Forms.CheckBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // checkIncludeStartEnd
            // 
            this.checkIncludeStartEnd.AutoSize = true;
            this.checkIncludeStartEnd.Checked = true;
            this.checkIncludeStartEnd.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkIncludeStartEnd.Location = new System.Drawing.Point(13, 13);
            this.checkIncludeStartEnd.Name = "checkIncludeStartEnd";
            this.checkIncludeStartEnd.Size = new System.Drawing.Size(153, 17);
            this.checkIncludeStartEnd.TabIndex = 0;
            this.checkIncludeStartEnd.Text = "Include start and end code";
            this.checkIncludeStartEnd.UseVisualStyleBackColor = true;
            // 
            // checkBinary
            // 
            this.checkBinary.AutoSize = true;
            this.checkBinary.Location = new System.Drawing.Point(13, 60);
            this.checkBinary.Name = "checkBinary";
            this.checkBinary.Size = new System.Drawing.Size(222, 17);
            this.checkBinary.TabIndex = 1;
            this.checkBinary.Text = "Save in binary Format (Repetier-Firmware)";
            this.checkBinary.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Location = new System.Drawing.Point(274, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(64, 64);
            this.panel1.TabIndex = 10;
            // 
            // labelSaveDirectInfo
            // 
            this.labelSaveDirectInfo.Location = new System.Drawing.Point(13, 93);
            this.labelSaveDirectInfo.Name = "labelSaveDirectInfo";
            this.labelSaveDirectInfo.Size = new System.Drawing.Size(325, 115);
            this.labelSaveDirectInfo.TabIndex = 11;
            this.labelSaveDirectInfo.Text = resources.GetString("labelSaveDirectInfo.Text");
            // 
            // checkIncludeJobFinished
            // 
            this.checkIncludeJobFinished.AutoSize = true;
            this.checkIncludeJobFinished.Checked = true;
            this.checkIncludeJobFinished.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkIncludeJobFinished.Location = new System.Drawing.Point(13, 37);
            this.checkIncludeJobFinished.Name = "checkIncludeJobFinished";
            this.checkIncludeJobFinished.Size = new System.Drawing.Size(171, 17);
            this.checkIncludeJobFinished.TabIndex = 12;
            this.checkIncludeJobFinished.Text = "Include job finished commands";
            this.checkIncludeJobFinished.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(127, 226);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 13;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(228, 226);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 14;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "gco";
            this.saveFileDialog.Filter = "G-Code|*.gco|All files|*.*";
            this.saveFileDialog.Title = "Save g-code for direct print";
            // 
            // StoreCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 273);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.checkIncludeJobFinished);
            this.Controls.Add(this.labelSaveDirectInfo);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.checkBinary);
            this.Controls.Add(this.checkIncludeStartEnd);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StoreCode";
            this.Text = "Save G-code for direct print";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkIncludeStartEnd;
        private System.Windows.Forms.CheckBox checkBinary;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelSaveDirectInfo;
        private System.Windows.Forms.CheckBox checkIncludeJobFinished;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}