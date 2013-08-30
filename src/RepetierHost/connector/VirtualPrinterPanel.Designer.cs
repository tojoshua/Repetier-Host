namespace RepetierHost.connector
{
    partial class VirtualPrinterPanel
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
            this.comboBaud = new System.Windows.Forms.ComboBox();
            this.labelBaudRate = new System.Windows.Forms.Label();
            this.bindingConnection = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.bindingConnection)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBaud
            // 
            this.comboBaud.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBaud.FormattingEnabled = true;
            this.comboBaud.Items.AddRange(new object[] {
            "9600",
            "14400",
            "19200",
            "28800",
            "38400",
            "56000",
            "57600",
            "76800",
            "111112",
            "115200",
            "128000",
            "230400",
            "250000",
            "256000",
            "460800",
            "500000",
            "921600",
            "1000000",
            "1500000"});
            this.comboBaud.Location = new System.Drawing.Point(140, 16);
            this.comboBaud.Name = "comboBaud";
            this.comboBaud.Size = new System.Drawing.Size(129, 21);
            this.comboBaud.TabIndex = 21;
            this.comboBaud.SelectedIndexChanged += new System.EventHandler(this.comboBaud_SelectedIndexChanged);
            // 
            // labelBaudRate
            // 
            this.labelBaudRate.AutoSize = true;
            this.labelBaudRate.Location = new System.Drawing.Point(15, 16);
            this.labelBaudRate.Name = "labelBaudRate";
            this.labelBaudRate.Size = new System.Drawing.Size(56, 13);
            this.labelBaudRate.TabIndex = 22;
            this.labelBaudRate.Text = "Baud rate:";
            // 
            // bindingConnection
            // 
            this.bindingConnection.DataSource = typeof(RepetierHost.connector.VirtualPrinterConnector);
            this.bindingConnection.CurrentItemChanged += new System.EventHandler(this.bindingConnection_CurrentItemChanged);
            // 
            // VirtualPrinterPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comboBaud);
            this.Controls.Add(this.labelBaudRate);
            this.Name = "VirtualPrinterPanel";
            this.Size = new System.Drawing.Size(297, 60);
            ((System.ComponentModel.ISupportInitialize)(this.bindingConnection)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBaud;
        private System.Windows.Forms.Label labelBaudRate;
        private System.Windows.Forms.BindingSource bindingConnection;
    }
}
