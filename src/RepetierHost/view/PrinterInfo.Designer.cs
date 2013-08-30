namespace RepetierHost.view
{
    partial class PrinterInfo
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrinterInfo));
            this.groupPrinterData = new System.Windows.Forms.GroupBox();
            this.labelFirmwareURL = new System.Windows.Forms.LinkLabel();
            this.labelNumExtruder = new System.Windows.Forms.Label();
            this.labelProtocol = new System.Windows.Forms.Label();
            this.labelFirmware = new System.Windows.Forms.Label();
            this.labelMachine = new System.Windows.Forms.Label();
            this.labNumberExtruder = new System.Windows.Forms.Label();
            this.labProtocol = new System.Windows.Forms.Label();
            this.labFirmwareURL = new System.Windows.Forms.Label();
            this.labFirmware = new System.Windows.Forms.Label();
            this.labMachineType = new System.Windows.Forms.Label();
            this.groupConnectionInformation = new System.Windows.Forms.GroupBox();
            this.labelErrorsReceived = new System.Windows.Forms.Label();
            this.labelBytesSend = new System.Windows.Forms.Label();
            this.labelLinesSend = new System.Windows.Forms.Label();
            this.labBytesSend = new System.Windows.Forms.Label();
            this.labErrorsReceived = new System.Windows.Forms.Label();
            this.labLinesSend = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.groupPrinterData.SuspendLayout();
            this.groupConnectionInformation.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupPrinterData
            // 
            this.groupPrinterData.Controls.Add(this.labelFirmwareURL);
            this.groupPrinterData.Controls.Add(this.labelNumExtruder);
            this.groupPrinterData.Controls.Add(this.labelProtocol);
            this.groupPrinterData.Controls.Add(this.labelFirmware);
            this.groupPrinterData.Controls.Add(this.labelMachine);
            this.groupPrinterData.Controls.Add(this.labNumberExtruder);
            this.groupPrinterData.Controls.Add(this.labProtocol);
            this.groupPrinterData.Controls.Add(this.labFirmwareURL);
            this.groupPrinterData.Controls.Add(this.labFirmware);
            this.groupPrinterData.Controls.Add(this.labMachineType);
            this.groupPrinterData.Location = new System.Drawing.Point(13, 13);
            this.groupPrinterData.Name = "groupPrinterData";
            this.groupPrinterData.Size = new System.Drawing.Size(445, 140);
            this.groupPrinterData.TabIndex = 0;
            this.groupPrinterData.TabStop = false;
            this.groupPrinterData.Text = "Printer data";
            // 
            // labelFirmwareURL
            // 
            this.labelFirmwareURL.AutoSize = true;
            this.labelFirmwareURL.Location = new System.Drawing.Point(135, 64);
            this.labelFirmwareURL.Name = "labelFirmwareURL";
            this.labelFirmwareURL.Size = new System.Drawing.Size(10, 13);
            this.labelFirmwareURL.TabIndex = 6;
            this.labelFirmwareURL.TabStop = true;
            this.labelFirmwareURL.Text = "-";
            this.labelFirmwareURL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.labelFirmwareURL_LinkClicked);
            // 
            // labelNumExtruder
            // 
            this.labelNumExtruder.AutoSize = true;
            this.labelNumExtruder.Location = new System.Drawing.Point(132, 111);
            this.labelNumExtruder.Name = "labelNumExtruder";
            this.labelNumExtruder.Size = new System.Drawing.Size(10, 13);
            this.labelNumExtruder.TabIndex = 5;
            this.labelNumExtruder.Text = "-";
            // 
            // labelProtocol
            // 
            this.labelProtocol.AutoSize = true;
            this.labelProtocol.Location = new System.Drawing.Point(132, 87);
            this.labelProtocol.Name = "labelProtocol";
            this.labelProtocol.Size = new System.Drawing.Size(10, 13);
            this.labelProtocol.TabIndex = 5;
            this.labelProtocol.Text = "-";
            // 
            // labelFirmware
            // 
            this.labelFirmware.AutoSize = true;
            this.labelFirmware.Location = new System.Drawing.Point(132, 41);
            this.labelFirmware.Name = "labelFirmware";
            this.labelFirmware.Size = new System.Drawing.Size(10, 13);
            this.labelFirmware.TabIndex = 5;
            this.labelFirmware.Text = "-";
            // 
            // labelMachine
            // 
            this.labelMachine.AutoSize = true;
            this.labelMachine.Location = new System.Drawing.Point(132, 20);
            this.labelMachine.Name = "labelMachine";
            this.labelMachine.Size = new System.Drawing.Size(10, 13);
            this.labelMachine.TabIndex = 5;
            this.labelMachine.Text = "-";
            // 
            // labNumberExtruder
            // 
            this.labNumberExtruder.AutoSize = true;
            this.labNumberExtruder.Location = new System.Drawing.Point(7, 111);
            this.labNumberExtruder.Name = "labNumberExtruder";
            this.labNumberExtruder.Size = new System.Drawing.Size(88, 13);
            this.labNumberExtruder.TabIndex = 4;
            this.labNumberExtruder.Text = "Number extruder:";
            // 
            // labProtocol
            // 
            this.labProtocol.AutoSize = true;
            this.labProtocol.Location = new System.Drawing.Point(7, 87);
            this.labProtocol.Name = "labProtocol";
            this.labProtocol.Size = new System.Drawing.Size(49, 13);
            this.labProtocol.TabIndex = 3;
            this.labProtocol.Text = "Protocol:";
            // 
            // labFirmwareURL
            // 
            this.labFirmwareURL.AutoSize = true;
            this.labFirmwareURL.Location = new System.Drawing.Point(7, 64);
            this.labFirmwareURL.Name = "labFirmwareURL";
            this.labFirmwareURL.Size = new System.Drawing.Size(77, 13);
            this.labFirmwareURL.TabIndex = 2;
            this.labFirmwareURL.Text = "Firmware URL:";
            // 
            // labFirmware
            // 
            this.labFirmware.AutoSize = true;
            this.labFirmware.Location = new System.Drawing.Point(7, 41);
            this.labFirmware.Name = "labFirmware";
            this.labFirmware.Size = new System.Drawing.Size(52, 13);
            this.labFirmware.TabIndex = 1;
            this.labFirmware.Text = "Firmware:";
            // 
            // labMachineType
            // 
            this.labMachineType.AutoSize = true;
            this.labMachineType.Location = new System.Drawing.Point(7, 20);
            this.labMachineType.Name = "labMachineType";
            this.labMachineType.Size = new System.Drawing.Size(74, 13);
            this.labMachineType.TabIndex = 0;
            this.labMachineType.Text = "Machine type:";
            // 
            // groupConnectionInformation
            // 
            this.groupConnectionInformation.Controls.Add(this.labelErrorsReceived);
            this.groupConnectionInformation.Controls.Add(this.labelBytesSend);
            this.groupConnectionInformation.Controls.Add(this.labelLinesSend);
            this.groupConnectionInformation.Controls.Add(this.labBytesSend);
            this.groupConnectionInformation.Controls.Add(this.labErrorsReceived);
            this.groupConnectionInformation.Controls.Add(this.labLinesSend);
            this.groupConnectionInformation.Location = new System.Drawing.Point(13, 160);
            this.groupConnectionInformation.Name = "groupConnectionInformation";
            this.groupConnectionInformation.Size = new System.Drawing.Size(445, 100);
            this.groupConnectionInformation.TabIndex = 1;
            this.groupConnectionInformation.TabStop = false;
            this.groupConnectionInformation.Text = "Connection information";
            // 
            // labelErrorsReceived
            // 
            this.labelErrorsReceived.AutoSize = true;
            this.labelErrorsReceived.Location = new System.Drawing.Point(132, 65);
            this.labelErrorsReceived.Name = "labelErrorsReceived";
            this.labelErrorsReceived.Size = new System.Drawing.Size(10, 13);
            this.labelErrorsReceived.TabIndex = 5;
            this.labelErrorsReceived.Text = "-";
            // 
            // labelBytesSend
            // 
            this.labelBytesSend.AutoSize = true;
            this.labelBytesSend.Location = new System.Drawing.Point(132, 43);
            this.labelBytesSend.Name = "labelBytesSend";
            this.labelBytesSend.Size = new System.Drawing.Size(10, 13);
            this.labelBytesSend.TabIndex = 5;
            this.labelBytesSend.Text = "-";
            // 
            // labelLinesSend
            // 
            this.labelLinesSend.AutoSize = true;
            this.labelLinesSend.Location = new System.Drawing.Point(132, 20);
            this.labelLinesSend.Name = "labelLinesSend";
            this.labelLinesSend.Size = new System.Drawing.Size(10, 13);
            this.labelLinesSend.TabIndex = 5;
            this.labelLinesSend.Text = "-";
            // 
            // labBytesSend
            // 
            this.labBytesSend.AutoSize = true;
            this.labBytesSend.Location = new System.Drawing.Point(7, 43);
            this.labBytesSend.Name = "labBytesSend";
            this.labBytesSend.Size = new System.Drawing.Size(62, 13);
            this.labBytesSend.TabIndex = 2;
            this.labBytesSend.Text = "Bytes send:";
            // 
            // labErrorsReceived
            // 
            this.labErrorsReceived.AutoSize = true;
            this.labErrorsReceived.Location = new System.Drawing.Point(7, 65);
            this.labErrorsReceived.Name = "labErrorsReceived";
            this.labErrorsReceived.Size = new System.Drawing.Size(81, 13);
            this.labErrorsReceived.TabIndex = 1;
            this.labErrorsReceived.Text = "Errors received:";
            // 
            // labLinesSend
            // 
            this.labLinesSend.AutoSize = true;
            this.labLinesSend.Location = new System.Drawing.Point(7, 20);
            this.labLinesSend.Name = "labLinesSend";
            this.labLinesSend.Size = new System.Drawing.Size(61, 13);
            this.labLinesSend.TabIndex = 0;
            this.labLinesSend.Text = "Lines send:";
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(374, 275);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // PrinterInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 317);
            this.ControlBox = false;
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.groupConnectionInformation);
            this.Controls.Add(this.groupPrinterData);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "PrinterInfo";
            this.Text = "Printer information";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PrinterInfo_FormClosing);
            this.groupPrinterData.ResumeLayout(false);
            this.groupPrinterData.PerformLayout();
            this.groupConnectionInformation.ResumeLayout(false);
            this.groupConnectionInformation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupPrinterData;
        private System.Windows.Forms.Label labMachineType;
        private System.Windows.Forms.Label labProtocol;
        private System.Windows.Forms.Label labFirmwareURL;
        private System.Windows.Forms.Label labFirmware;
        private System.Windows.Forms.Label labNumberExtruder;
        private System.Windows.Forms.GroupBox groupConnectionInformation;
        private System.Windows.Forms.Label labErrorsReceived;
        private System.Windows.Forms.Label labLinesSend;
        private System.Windows.Forms.Label labBytesSend;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label labelNumExtruder;
        private System.Windows.Forms.Label labelProtocol;
        private System.Windows.Forms.Label labelFirmware;
        private System.Windows.Forms.Label labelMachine;
        private System.Windows.Forms.Label labelLinesSend;
        private System.Windows.Forms.Label labelBytesSend;
        private System.Windows.Forms.Label labelErrorsReceived;
        private System.Windows.Forms.LinkLabel labelFirmwareURL;
    }
}