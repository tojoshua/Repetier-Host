namespace RepetierHost.view.utils
{
    partial class EditInstanceName
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditInstanceName));
            this.labelInstanceName = new System.Windows.Forms.Label();
            this.labelColor = new System.Windows.Forms.Label();
            this.textName = new System.Windows.Forms.TextBox();
            this.panelColor = new System.Windows.Forms.Panel();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.SuspendLayout();
            // 
            // labelInstanceName
            // 
            this.labelInstanceName.AutoSize = true;
            this.labelInstanceName.Location = new System.Drawing.Point(13, 13);
            this.labelInstanceName.Name = "labelInstanceName";
            this.labelInstanceName.Size = new System.Drawing.Size(82, 13);
            this.labelInstanceName.TabIndex = 0;
            this.labelInstanceName.Text = "Instance Name:";
            // 
            // labelColor
            // 
            this.labelColor.AutoSize = true;
            this.labelColor.Location = new System.Drawing.Point(13, 48);
            this.labelColor.Name = "labelColor";
            this.labelColor.Size = new System.Drawing.Size(95, 13);
            this.labelColor.TabIndex = 1;
            this.labelColor.Text = "Background Color:";
            // 
            // textName
            // 
            this.textName.Location = new System.Drawing.Point(143, 10);
            this.textName.Name = "textName";
            this.textName.Size = new System.Drawing.Size(220, 20);
            this.textName.TabIndex = 2;
            // 
            // panelColor
            // 
            this.panelColor.Location = new System.Drawing.Point(143, 41);
            this.panelColor.Name = "panelColor";
            this.panelColor.Size = new System.Drawing.Size(220, 25);
            this.panelColor.TabIndex = 3;
            this.panelColor.Click += new System.EventHandler(this.panelColor_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(156, 85);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(288, 85);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // EditInstanceName
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 139);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.panelColor);
            this.Controls.Add(this.textName);
            this.Controls.Add(this.labelColor);
            this.Controls.Add(this.labelInstanceName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EditInstanceName";
            this.Text = "Edit Instance Name";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelInstanceName;
        private System.Windows.Forms.Label labelColor;
        private System.Windows.Forms.TextBox textName;
        private System.Windows.Forms.Panel panelColor;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ColorDialog colorDialog;
    }
}