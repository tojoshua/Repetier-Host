namespace RepetierHost.view.utils
{
    partial class SlicingInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SlicingInfo));
            this.labelSlicer_ = new System.Windows.Forms.Label();
            this.labelAction_ = new System.Windows.Forms.Label();
            this.labelDuration_ = new System.Windows.Forms.Label();
            this.labelSlicer = new System.Windows.Forms.Label();
            this.labelAction = new System.Windows.Forms.Label();
            this.labelDuration = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.checkStartBoxAfterSlicing = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // labelSlicer_
            // 
            this.labelSlicer_.AutoSize = true;
            this.labelSlicer_.Location = new System.Drawing.Point(13, 13);
            this.labelSlicer_.Name = "labelSlicer_";
            this.labelSlicer_.Size = new System.Drawing.Size(36, 13);
            this.labelSlicer_.TabIndex = 0;
            this.labelSlicer_.Text = "Slicer:";
            // 
            // labelAction_
            // 
            this.labelAction_.AutoSize = true;
            this.labelAction_.Location = new System.Drawing.Point(13, 32);
            this.labelAction_.Name = "labelAction_";
            this.labelAction_.Size = new System.Drawing.Size(40, 13);
            this.labelAction_.TabIndex = 1;
            this.labelAction_.Text = "Action:";
            // 
            // labelDuration_
            // 
            this.labelDuration_.AutoSize = true;
            this.labelDuration_.Location = new System.Drawing.Point(13, 52);
            this.labelDuration_.Name = "labelDuration_";
            this.labelDuration_.Size = new System.Drawing.Size(50, 13);
            this.labelDuration_.TabIndex = 2;
            this.labelDuration_.Text = "Duration:";
            // 
            // labelSlicer
            // 
            this.labelSlicer.AutoSize = true;
            this.labelSlicer.Location = new System.Drawing.Point(84, 13);
            this.labelSlicer.Name = "labelSlicer";
            this.labelSlicer.Size = new System.Drawing.Size(33, 13);
            this.labelSlicer.TabIndex = 3;
            this.labelSlicer.Text = "Slic3r";
            // 
            // labelAction
            // 
            this.labelAction.AutoSize = true;
            this.labelAction.Location = new System.Drawing.Point(84, 32);
            this.labelAction.Name = "labelAction";
            this.labelAction.Size = new System.Drawing.Size(50, 13);
            this.labelAction.TabIndex = 4;
            this.labelAction.Text = "Slicing ...";
            // 
            // labelDuration
            // 
            this.labelDuration.AutoSize = true;
            this.labelDuration.Location = new System.Drawing.Point(87, 51);
            this.labelDuration.Name = "labelDuration";
            this.labelDuration.Size = new System.Drawing.Size(34, 13);
            this.labelDuration.TabIndex = 5;
            this.labelDuration.Text = "00:00";
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // checkStartBoxAfterSlicing
            // 
            this.checkStartBoxAfterSlicing.AutoSize = true;
            this.checkStartBoxAfterSlicing.Location = new System.Drawing.Point(16, 73);
            this.checkStartBoxAfterSlicing.Name = "checkStartBoxAfterSlicing";
            this.checkStartBoxAfterSlicing.Size = new System.Drawing.Size(121, 17);
            this.checkStartBoxAfterSlicing.TabIndex = 6;
            this.checkStartBoxAfterSlicing.Text = "Start job after slicing";
            this.checkStartBoxAfterSlicing.UseVisualStyleBackColor = true;
            // 
            // SlicingInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(184, 102);
            this.ControlBox = false;
            this.Controls.Add(this.checkStartBoxAfterSlicing);
            this.Controls.Add(this.labelDuration);
            this.Controls.Add(this.labelAction);
            this.Controls.Add(this.labelSlicer);
            this.Controls.Add(this.labelDuration_);
            this.Controls.Add(this.labelAction_);
            this.Controls.Add(this.labelSlicer_);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SlicingInfo";
            this.ShowInTaskbar = false;
            this.Text = "Slicing info";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelSlicer_;
        private System.Windows.Forms.Label labelAction_;
        private System.Windows.Forms.Label labelDuration_;
        private System.Windows.Forms.Label labelSlicer;
        private System.Windows.Forms.Label labelAction;
        private System.Windows.Forms.Label labelDuration;
        private System.Windows.Forms.Timer timer;
        public System.Windows.Forms.CheckBox checkStartBoxAfterSlicing;
    }
}