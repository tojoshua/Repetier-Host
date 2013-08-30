namespace RepetierHost.view
{
    partial class SwitchButton
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SwitchButton));
            this.button = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // button
            // 
            this.button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button.ImageIndex = 0;
            this.button.ImageList = this.imageList1;
            this.button.Location = new System.Drawing.Point(0, 0);
            this.button.Name = "button";
            this.button.Size = new System.Drawing.Size(100, 23);
            this.button.TabIndex = 0;
            this.button.Text = "button1";
            this.button.UseVisualStyleBackColor = true;
            this.button.Click += new System.EventHandler(this.button_Click);
            this.button.SizeChanged += new System.EventHandler(this.button_SizeChanged);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "switch_off2.png");
            this.imageList1.Images.SetKeyName(1, "switch_on2.png");
            this.imageList1.Images.SetKeyName(2, "switch_off.png");
            this.imageList1.Images.SetKeyName(3, "switch_on.png");
            // 
            // SwitchButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.button);
            this.MinimumSize = new System.Drawing.Size(90, 23);
            this.Name = "SwitchButton";
            this.Size = new System.Drawing.Size(100, 23);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button;
        private System.Windows.Forms.ImageList imageList1;
    }
}
