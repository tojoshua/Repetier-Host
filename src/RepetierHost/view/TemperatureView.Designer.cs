namespace RepetierHost.view
{
    partial class TemperatureView
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
                font.Dispose();
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
            this.SuspendLayout();
            // 
            // TemperatureView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "TemperatureView";
            this.Size = new System.Drawing.Size(976, 587);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.TemperatureView_Paint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TemperatureView_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TemperatureView_MouseDown);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
