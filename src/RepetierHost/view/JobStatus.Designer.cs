namespace RepetierHost.view
{
    partial class JobStatus
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JobStatus));
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.lStatus = new System.Windows.Forms.Label();
            this.lStartTime = new System.Windows.Forms.Label();
            this.lFinishTime = new System.Windows.Forms.Label();
            this.lETA = new System.Windows.Forms.Label();
            this.lTotalLines = new System.Windows.Forms.Label();
            this.lLinesSend = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelStartTime = new System.Windows.Forms.Label();
            this.labelFinishTime = new System.Windows.Forms.Label();
            this.labelETA = new System.Windows.Forms.Label();
            this.labelTotalLines = new System.Windows.Forms.Label();
            this.labelLinesSend = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Interval = 250;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // lStatus
            // 
            this.lStatus.AutoSize = true;
            this.lStatus.Location = new System.Drawing.Point(14, 13);
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(40, 13);
            this.lStatus.TabIndex = 0;
            this.lStatus.Text = "Status:";
            // 
            // lStartTime
            // 
            this.lStartTime.AutoSize = true;
            this.lStartTime.Location = new System.Drawing.Point(14, 33);
            this.lStartTime.Name = "lStartTime";
            this.lStartTime.Size = new System.Drawing.Size(54, 13);
            this.lStartTime.TabIndex = 1;
            this.lStartTime.Text = "Start time:";
            // 
            // lFinishTime
            // 
            this.lFinishTime.AutoSize = true;
            this.lFinishTime.Location = new System.Drawing.Point(14, 52);
            this.lFinishTime.Name = "lFinishTime";
            this.lFinishTime.Size = new System.Drawing.Size(59, 13);
            this.lFinishTime.TabIndex = 2;
            this.lFinishTime.Text = "Finish time:";
            // 
            // lETA
            // 
            this.lETA.AutoSize = true;
            this.lETA.Location = new System.Drawing.Point(14, 72);
            this.lETA.Name = "lETA";
            this.lETA.Size = new System.Drawing.Size(31, 13);
            this.lETA.TabIndex = 3;
            this.lETA.Text = "ETA:";
            // 
            // lTotalLines
            // 
            this.lTotalLines.AutoSize = true;
            this.lTotalLines.Location = new System.Drawing.Point(14, 91);
            this.lTotalLines.Name = "lTotalLines";
            this.lTotalLines.Size = new System.Drawing.Size(58, 13);
            this.lTotalLines.TabIndex = 4;
            this.lTotalLines.Text = "Total lines:";
            // 
            // lLinesSend
            // 
            this.lLinesSend.AutoSize = true;
            this.lLinesSend.Location = new System.Drawing.Point(14, 111);
            this.lLinesSend.Name = "lLinesSend";
            this.lLinesSend.Size = new System.Drawing.Size(61, 13);
            this.lLinesSend.TabIndex = 5;
            this.lLinesSend.Text = "Lines send:";
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(89, 140);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(80, 22);
            this.buttonClose.TabIndex = 6;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(121, 13);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(10, 13);
            this.labelStatus.TabIndex = 7;
            this.labelStatus.Text = "-";
            // 
            // labelStartTime
            // 
            this.labelStartTime.AutoSize = true;
            this.labelStartTime.Location = new System.Drawing.Point(121, 33);
            this.labelStartTime.Name = "labelStartTime";
            this.labelStartTime.Size = new System.Drawing.Size(10, 13);
            this.labelStartTime.TabIndex = 8;
            this.labelStartTime.Text = "-";
            // 
            // labelFinishTime
            // 
            this.labelFinishTime.AutoSize = true;
            this.labelFinishTime.Location = new System.Drawing.Point(121, 52);
            this.labelFinishTime.Name = "labelFinishTime";
            this.labelFinishTime.Size = new System.Drawing.Size(10, 13);
            this.labelFinishTime.TabIndex = 8;
            this.labelFinishTime.Text = "-";
            // 
            // labelETA
            // 
            this.labelETA.AutoSize = true;
            this.labelETA.Location = new System.Drawing.Point(121, 72);
            this.labelETA.Name = "labelETA";
            this.labelETA.Size = new System.Drawing.Size(10, 13);
            this.labelETA.TabIndex = 8;
            this.labelETA.Text = "-";
            // 
            // labelTotalLines
            // 
            this.labelTotalLines.AutoSize = true;
            this.labelTotalLines.Location = new System.Drawing.Point(121, 91);
            this.labelTotalLines.Name = "labelTotalLines";
            this.labelTotalLines.Size = new System.Drawing.Size(10, 13);
            this.labelTotalLines.TabIndex = 8;
            this.labelTotalLines.Text = "-";
            // 
            // labelLinesSend
            // 
            this.labelLinesSend.AutoSize = true;
            this.labelLinesSend.Location = new System.Drawing.Point(121, 111);
            this.labelLinesSend.Name = "labelLinesSend";
            this.labelLinesSend.Size = new System.Drawing.Size(10, 13);
            this.labelLinesSend.TabIndex = 8;
            this.labelLinesSend.Text = "-";
            // 
            // JobStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(251, 174);
            this.ControlBox = false;
            this.Controls.Add(this.labelLinesSend);
            this.Controls.Add(this.labelTotalLines);
            this.Controls.Add(this.labelETA);
            this.Controls.Add(this.labelFinishTime);
            this.Controls.Add(this.labelStartTime);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.lLinesSend);
            this.Controls.Add(this.lTotalLines);
            this.Controls.Add(this.lETA);
            this.Controls.Add(this.lFinishTime);
            this.Controls.Add(this.lStartTime);
            this.Controls.Add(this.lStatus);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "JobStatus";
            this.Text = "Job status";
            this.Shown += new System.EventHandler(this.JobStatus_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JobStatus_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label lStatus;
        private System.Windows.Forms.Label lStartTime;
        private System.Windows.Forms.Label lFinishTime;
        private System.Windows.Forms.Label lETA;
        private System.Windows.Forms.Label lTotalLines;
        private System.Windows.Forms.Label lLinesSend;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label labelStartTime;
        private System.Windows.Forms.Label labelFinishTime;
        private System.Windows.Forms.Label labelETA;
        private System.Windows.Forms.Label labelTotalLines;
        private System.Windows.Forms.Label labelLinesSend;
    }
}