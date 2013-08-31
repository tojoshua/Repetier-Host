namespace RepetierHost.connector
{
    partial class SerialConnectionPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SerialConnectionPanel));
            this.buttonRefreshPorts = new System.Windows.Forms.Button();
            this.comboBaud = new System.Windows.Forms.ComboBox();
            this.labelBaudRate = new System.Windows.Forms.Label();
            this.comboPort = new System.Windows.Forms.ComboBox();
            this.labelPort = new System.Windows.Forms.Label();
            this.labelCacheSizeHint = new System.Windows.Forms.Label();
            this.checkPingPong = new System.Windows.Forms.CheckBox();
            this.labelReceiveCacheSize = new System.Windows.Forms.Label();
            this.textReceiveCacheSize = new System.Windows.Forms.TextBox();
            this.labelConnectionInfo = new System.Windows.Forms.Label();
            this.labelTransferProtocol = new System.Windows.Forms.Label();
            this.comboTransferProtocol = new System.Windows.Forms.ComboBox();
            this.comboResetOnConnect = new System.Windows.Forms.ComboBox();
            this.labelResetOnConnect = new System.Windows.Forms.Label();
            this.comboResetOnEmergency = new System.Windows.Forms.ComboBox();
            this.labelResetOnEmergency = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.bindingConnection = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingConnection)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonRefreshPorts
            // 
            this.buttonRefreshPorts.Location = new System.Drawing.Point(294, 8);
            this.buttonRefreshPorts.Name = "buttonRefreshPorts";
            this.buttonRefreshPorts.Size = new System.Drawing.Size(149, 23);
            this.buttonRefreshPorts.TabIndex = 21;
            this.buttonRefreshPorts.Text = "Refresh Ports";
            this.buttonRefreshPorts.UseVisualStyleBackColor = true;
            this.buttonRefreshPorts.Click += new System.EventHandler(this.buttonRefreshPorts_Click);
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
            this.comboBaud.Location = new System.Drawing.Point(140, 35);
            this.comboBaud.Name = "comboBaud";
            this.comboBaud.Size = new System.Drawing.Size(129, 21);
            this.comboBaud.TabIndex = 19;
            this.comboBaud.SelectedIndexChanged += new System.EventHandler(this.comboBaud_SelectedIndexChanged);
            // 
            // labelBaudRate
            // 
            this.labelBaudRate.AutoSize = true;
            this.labelBaudRate.Location = new System.Drawing.Point(15, 35);
            this.labelBaudRate.Name = "labelBaudRate";
            this.labelBaudRate.Size = new System.Drawing.Size(56, 13);
            this.labelBaudRate.TabIndex = 20;
            this.labelBaudRate.Text = "Baud rate:";
            // 
            // comboPort
            // 
            this.comboPort.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingConnection, "Port", true));
            this.comboPort.FormattingEnabled = true;
            this.comboPort.Location = new System.Drawing.Point(140, 8);
            this.comboPort.Name = "comboPort";
            this.comboPort.Size = new System.Drawing.Size(129, 21);
            this.comboPort.TabIndex = 17;
            // 
            // labelPort
            // 
            this.labelPort.AutoSize = true;
            this.labelPort.Location = new System.Drawing.Point(15, 11);
            this.labelPort.Name = "labelPort";
            this.labelPort.Size = new System.Drawing.Size(29, 13);
            this.labelPort.TabIndex = 18;
            this.labelPort.Text = "Port:";
            // 
            // labelCacheSizeHint
            // 
            this.labelCacheSizeHint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCacheSizeHint.AutoSize = true;
            this.labelCacheSizeHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCacheSizeHint.Location = new System.Drawing.Point(138, 170);
            this.labelCacheSizeHint.Name = "labelCacheSizeHint";
            this.labelCacheSizeHint.Size = new System.Drawing.Size(305, 12);
            this.labelCacheSizeHint.TabIndex = 28;
            this.labelCacheSizeHint.Text = "From Arduino 1 on the receiving cache was reduced from 127 to 63 bytes!";
            // 
            // checkPingPong
            // 
            this.checkPingPong.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkPingPong.AutoSize = true;
            this.checkPingPong.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.bindingConnection, "PingPong", true));
            this.checkPingPong.Location = new System.Drawing.Point(18, 190);
            this.checkPingPong.Name = "checkPingPong";
            this.checkPingPong.Size = new System.Drawing.Size(264, 17);
            this.checkPingPong.TabIndex = 27;
            this.checkPingPong.Text = "Use Ping-Pong communication (send only after ok)";
            this.checkPingPong.UseVisualStyleBackColor = true;
            // 
            // labelReceiveCacheSize
            // 
            this.labelReceiveCacheSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelReceiveCacheSize.AutoSize = true;
            this.labelReceiveCacheSize.Location = new System.Drawing.Point(15, 143);
            this.labelReceiveCacheSize.Name = "labelReceiveCacheSize";
            this.labelReceiveCacheSize.Size = new System.Drawing.Size(104, 13);
            this.labelReceiveCacheSize.TabIndex = 26;
            this.labelReceiveCacheSize.Text = "Receive cache size:";
            // 
            // textReceiveCacheSize
            // 
            this.textReceiveCacheSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textReceiveCacheSize.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingConnection, "ReceiveCacheSizeString", true));
            this.textReceiveCacheSize.Location = new System.Drawing.Point(140, 140);
            this.textReceiveCacheSize.Name = "textReceiveCacheSize";
            this.textReceiveCacheSize.Size = new System.Drawing.Size(75, 20);
            this.textReceiveCacheSize.TabIndex = 23;
            this.textReceiveCacheSize.Text = "63";
            this.textReceiveCacheSize.Validating += new System.ComponentModel.CancelEventHandler(this.int_Validating);
            // 
            // labelConnectionInfo
            // 
            this.labelConnectionInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelConnectionInfo.Location = new System.Drawing.Point(18, 220);
            this.labelConnectionInfo.Name = "labelConnectionInfo";
            this.labelConnectionInfo.Size = new System.Drawing.Size(425, 85);
            this.labelConnectionInfo.TabIndex = 25;
            this.labelConnectionInfo.Text = resources.GetString("labelConnectionInfo.Text");
            // 
            // labelTransferProtocol
            // 
            this.labelTransferProtocol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTransferProtocol.AutoSize = true;
            this.labelTransferProtocol.Location = new System.Drawing.Point(15, 62);
            this.labelTransferProtocol.Name = "labelTransferProtocol";
            this.labelTransferProtocol.Size = new System.Drawing.Size(87, 13);
            this.labelTransferProtocol.TabIndex = 24;
            this.labelTransferProtocol.Text = "Transfer protocol";
            // 
            // comboTransferProtocol
            // 
            this.comboTransferProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTransferProtocol.FormattingEnabled = true;
            this.comboTransferProtocol.Items.AddRange(new object[] {
            "Autodetect",
            "ASCII",
            "Repetier Protocol"});
            this.comboTransferProtocol.Location = new System.Drawing.Point(140, 62);
            this.comboTransferProtocol.Name = "comboTransferProtocol";
            this.comboTransferProtocol.Size = new System.Drawing.Size(129, 21);
            this.comboTransferProtocol.TabIndex = 22;
            this.comboTransferProtocol.SelectedIndexChanged += new System.EventHandler(this.comboTransferProtocol_SelectedIndexChanged);
            // 
            // comboResetOnConnect
            // 
            this.comboResetOnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboResetOnConnect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboResetOnConnect.FormattingEnabled = true;
            this.comboResetOnConnect.Items.AddRange(new object[] {
            "Disabled",
            "DTR low->high",
            "DTR low->high->low",
            "DTR toggle"});
            this.comboResetOnConnect.Location = new System.Drawing.Point(140, 89);
            this.comboResetOnConnect.Name = "comboResetOnConnect";
            this.comboResetOnConnect.Size = new System.Drawing.Size(303, 21);
            this.comboResetOnConnect.TabIndex = 22;
            this.comboResetOnConnect.SelectedIndexChanged += new System.EventHandler(this.comboResetOnConnect_SelectedIndexChanged);
            // 
            // labelResetOnConnect
            // 
            this.labelResetOnConnect.AutoSize = true;
            this.labelResetOnConnect.Location = new System.Drawing.Point(15, 89);
            this.labelResetOnConnect.Name = "labelResetOnConnect";
            this.labelResetOnConnect.Size = new System.Drawing.Size(93, 13);
            this.labelResetOnConnect.TabIndex = 24;
            this.labelResetOnConnect.Text = "Reset on Connect";
            // 
            // comboResetOnEmergency
            // 
            this.comboResetOnEmergency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboResetOnEmergency.FormattingEnabled = true;
            this.comboResetOnEmergency.Items.AddRange(new object[] {
            "Send emergency command",
            "Send emergency command + Toggle DTR",
            "Send emergency command and reconnect"});
            this.comboResetOnEmergency.Location = new System.Drawing.Point(140, 116);
            this.comboResetOnEmergency.Name = "comboResetOnEmergency";
            this.comboResetOnEmergency.Size = new System.Drawing.Size(303, 21);
            this.comboResetOnEmergency.TabIndex = 22;
            this.comboResetOnEmergency.SelectedIndexChanged += new System.EventHandler(this.comboResetOnEmergency_SelectedIndexChanged);
            // 
            // labelResetOnEmergency
            // 
            this.labelResetOnEmergency.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelResetOnEmergency.AutoSize = true;
            this.labelResetOnEmergency.Location = new System.Drawing.Point(15, 116);
            this.labelResetOnEmergency.Name = "labelResetOnEmergency";
            this.labelResetOnEmergency.Size = new System.Drawing.Size(106, 13);
            this.labelResetOnEmergency.TabIndex = 24;
            this.labelResetOnEmergency.Text = "Reset on Emergency";
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // bindingConnection
            // 
            this.bindingConnection.DataSource = typeof(RepetierHost.connector.SerialConnector);
            this.bindingConnection.CurrentItemChanged += new System.EventHandler(this.bindingConnection_CurrentItemChanged);
            // 
            // SerialConnectionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelCacheSizeHint);
            this.Controls.Add(this.checkPingPong);
            this.Controls.Add(this.labelReceiveCacheSize);
            this.Controls.Add(this.textReceiveCacheSize);
            this.Controls.Add(this.labelConnectionInfo);
            this.Controls.Add(this.labelResetOnEmergency);
            this.Controls.Add(this.labelResetOnConnect);
            this.Controls.Add(this.labelTransferProtocol);
            this.Controls.Add(this.comboResetOnEmergency);
            this.Controls.Add(this.comboResetOnConnect);
            this.Controls.Add(this.comboTransferProtocol);
            this.Controls.Add(this.buttonRefreshPorts);
            this.Controls.Add(this.comboBaud);
            this.Controls.Add(this.labelBaudRate);
            this.Controls.Add(this.comboPort);
            this.Controls.Add(this.labelPort);
            this.Name = "SerialConnectionPanel";
            this.Size = new System.Drawing.Size(460, 365);
            this.Load += new System.EventHandler(this.SerialConnectionPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingConnection)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource bindingConnection;
        private System.Windows.Forms.Button buttonRefreshPorts;
        private System.Windows.Forms.ComboBox comboBaud;
        private System.Windows.Forms.Label labelBaudRate;
        private System.Windows.Forms.ComboBox comboPort;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.Label labelCacheSizeHint;
        private System.Windows.Forms.CheckBox checkPingPong;
        private System.Windows.Forms.Label labelReceiveCacheSize;
        private System.Windows.Forms.TextBox textReceiveCacheSize;
        private System.Windows.Forms.Label labelConnectionInfo;
        private System.Windows.Forms.Label labelTransferProtocol;
        private System.Windows.Forms.ComboBox comboTransferProtocol;
        private System.Windows.Forms.ComboBox comboResetOnConnect;
        private System.Windows.Forms.Label labelResetOnConnect;
        private System.Windows.Forms.ComboBox comboResetOnEmergency;
        private System.Windows.Forms.Label labelResetOnEmergency;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
