namespace RepetierHost.view
{
    partial class LogBox
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
                drawFont.Dispose();
                blackBrush.Dispose();

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
            this.scroll = new System.Windows.Forms.VScrollBar();
            this.log = new RepetierHost.view.DoubleBufferPanel();
            this.SuspendLayout();
            // 
            // scroll
            // 
            this.scroll.Dock = System.Windows.Forms.DockStyle.Right;
            this.scroll.Location = new System.Drawing.Point(843, 0);
            this.scroll.Name = "scroll";
            this.scroll.Size = new System.Drawing.Size(17, 329);
            this.scroll.TabIndex = 0;
            this.scroll.ValueChanged += new System.EventHandler(this.scroll_ValueChanged);
            // 
            // log
            // 
            this.log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.log.Location = new System.Drawing.Point(0, 0);
            this.log.Name = "log";
            this.log.Size = new System.Drawing.Size(843, 329);
            this.log.TabIndex = 1;
            this.log.Text = "doubleBufferPanel1";
            this.log.Paint += new System.Windows.Forms.PaintEventHandler(this.log_Paint);
            this.log.MouseMove += new System.Windows.Forms.MouseEventHandler(this.log_MouseMove);
            this.log.Leave += new System.EventHandler(this.log_Leave);
            this.log.KeyUp += new System.Windows.Forms.KeyEventHandler(this.log_KeyUp);
            this.log.MouseDown += new System.Windows.Forms.MouseEventHandler(this.log_MouseDown);
            this.log.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.log_KeyPress);
            this.log.Enter += new System.EventHandler(this.log_Enter);
            this.log.MouseUp += new System.Windows.Forms.MouseEventHandler(this.log_MouseUp);
            this.log.KeyDown += new System.Windows.Forms.KeyEventHandler(this.log_KeyDown);
            // 
            // LogBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.log);
            this.Controls.Add(this.scroll);
            this.Name = "LogBox";
            this.Size = new System.Drawing.Size(860, 329);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.VScrollBar scroll;
        private DoubleBufferPanel log;
    }
}
