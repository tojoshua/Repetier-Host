namespace RepetierHost.view
{
    partial class LeadScrewCalculatorDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LeadScrewCalculatorDialog));
            this.LSMotorAngle = new System.Windows.Forms.ComboBox();
            this.LSMicroSteps = new System.Windows.Forms.ComboBox();
            this.LSScrewPitch = new System.Windows.Forms.ComboBox();
            this.buttonCalculate = new System.Windows.Forms.Button();
            this.GearRatio0 = new System.Windows.Forms.TextBox();
            this.GearRatio1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelMotorStepAngle = new System.Windows.Forms.Label();
            this.labelMicrostepping = new System.Windows.Forms.Label();
            this.labelLeadscrewPitch = new System.Windows.Forms.Label();
            this.labelGearRatio = new System.Windows.Forms.Label();
            this.LSOutput = new System.Windows.Forms.TextBox();
            this.labelStepsPerMM = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LSMotorAngle
            // 
            this.LSMotorAngle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LSMotorAngle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LSMotorAngle.FormattingEnabled = true;
            this.LSMotorAngle.Items.AddRange(new object[] {
            "1.8",
            "0.9",
            "7.5"});
            this.LSMotorAngle.Location = new System.Drawing.Point(186, 12);
            this.LSMotorAngle.Name = "LSMotorAngle";
            this.LSMotorAngle.Size = new System.Drawing.Size(161, 21);
            this.LSMotorAngle.TabIndex = 0;
            // 
            // LSMicroSteps
            // 
            this.LSMicroSteps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LSMicroSteps.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LSMicroSteps.FormattingEnabled = true;
            this.LSMicroSteps.Items.AddRange(new object[] {
            "1",
            "1/2",
            "1/4",
            "1/8",
            "1/16",
            "1/32"});
            this.LSMicroSteps.Location = new System.Drawing.Point(186, 40);
            this.LSMicroSteps.Name = "LSMicroSteps";
            this.LSMicroSteps.Size = new System.Drawing.Size(161, 21);
            this.LSMicroSteps.TabIndex = 1;
            // 
            // LSScrewPitch
            // 
            this.LSScrewPitch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LSScrewPitch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LSScrewPitch.FormattingEnabled = true;
            this.LSScrewPitch.Items.AddRange(new object[] {
            "M8 ( 1.25 mm per rotation )",
            "M6 ( 1 mm per rotation )",
            "M10/M11 (1.5 mm per rotation)",
            "M12 (1.75 mm per rotation)",
            "5/16 ( 1.41111 mm per rotation )",
            "1/4\" - 16 ( 1.5875 mm per rotation )",
            "Tr 8 x 1.5",
            "Tr 8 x 2.0",
            "Tr 11 x 3.0",
            "Tr 16 x 4.0"});
            this.LSScrewPitch.Location = new System.Drawing.Point(186, 68);
            this.LSScrewPitch.Name = "LSScrewPitch";
            this.LSScrewPitch.Size = new System.Drawing.Size(161, 21);
            this.LSScrewPitch.TabIndex = 2;
            // 
            // buttonCalculate
            // 
            this.buttonCalculate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCalculate.Location = new System.Drawing.Point(15, 134);
            this.buttonCalculate.Name = "buttonCalculate";
            this.buttonCalculate.Size = new System.Drawing.Size(332, 23);
            this.buttonCalculate.TabIndex = 4;
            this.buttonCalculate.Text = "Calculate";
            this.buttonCalculate.UseVisualStyleBackColor = true;
            this.buttonCalculate.Click += new System.EventHandler(this.button1_Click);
            // 
            // GearRatio0
            // 
            this.GearRatio0.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GearRatio0.Location = new System.Drawing.Point(226, 96);
            this.GearRatio0.Name = "GearRatio0";
            this.GearRatio0.Size = new System.Drawing.Size(48, 20);
            this.GearRatio0.TabIndex = 5;
            this.GearRatio0.Text = "1";
            // 
            // GearRatio1
            // 
            this.GearRatio1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GearRatio1.Location = new System.Drawing.Point(299, 95);
            this.GearRatio1.Name = "GearRatio1";
            this.GearRatio1.Size = new System.Drawing.Size(48, 20);
            this.GearRatio1.TabIndex = 6;
            this.GearRatio1.Text = "1";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(282, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(10, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = ":";
            // 
            // labelMotorStepAngle
            // 
            this.labelMotorStepAngle.AutoSize = true;
            this.labelMotorStepAngle.Location = new System.Drawing.Point(12, 15);
            this.labelMotorStepAngle.Name = "labelMotorStepAngle";
            this.labelMotorStepAngle.Size = new System.Drawing.Size(86, 13);
            this.labelMotorStepAngle.TabIndex = 8;
            this.labelMotorStepAngle.Text = "Motor step angle";
            // 
            // labelMicrostepping
            // 
            this.labelMicrostepping.AutoSize = true;
            this.labelMicrostepping.Location = new System.Drawing.Point(12, 43);
            this.labelMicrostepping.Name = "labelMicrostepping";
            this.labelMicrostepping.Size = new System.Drawing.Size(103, 13);
            this.labelMicrostepping.TabIndex = 9;
            this.labelMicrostepping.Text = "Driver microstepping";
            // 
            // labelLeadscrewPitch
            // 
            this.labelLeadscrewPitch.AutoSize = true;
            this.labelLeadscrewPitch.Location = new System.Drawing.Point(12, 71);
            this.labelLeadscrewPitch.Name = "labelLeadscrewPitch";
            this.labelLeadscrewPitch.Size = new System.Drawing.Size(85, 13);
            this.labelLeadscrewPitch.TabIndex = 10;
            this.labelLeadscrewPitch.Text = "Leadscrew pitch";
            // 
            // labelGearRatio
            // 
            this.labelGearRatio.AutoSize = true;
            this.labelGearRatio.Location = new System.Drawing.Point(12, 98);
            this.labelGearRatio.Name = "labelGearRatio";
            this.labelGearRatio.Size = new System.Drawing.Size(53, 13);
            this.labelGearRatio.TabIndex = 11;
            this.labelGearRatio.Text = "Gear ratio";
            // 
            // LSOutput
            // 
            this.LSOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LSOutput.Location = new System.Drawing.Point(186, 163);
            this.LSOutput.Name = "LSOutput";
            this.LSOutput.ReadOnly = true;
            this.LSOutput.Size = new System.Drawing.Size(161, 20);
            this.LSOutput.TabIndex = 12;
            // 
            // labelStepsPerMM
            // 
            this.labelStepsPerMM.AutoSize = true;
            this.labelStepsPerMM.Location = new System.Drawing.Point(12, 166);
            this.labelStepsPerMM.Name = "labelStepsPerMM";
            this.labelStepsPerMM.Size = new System.Drawing.Size(71, 13);
            this.labelStepsPerMM.TabIndex = 11;
            this.labelStepsPerMM.Text = "Steps per mm";
            // 
            // LeadScrewCalculatorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 198);
            this.Controls.Add(this.LSOutput);
            this.Controls.Add(this.labelStepsPerMM);
            this.Controls.Add(this.labelGearRatio);
            this.Controls.Add(this.labelLeadscrewPitch);
            this.Controls.Add(this.labelMicrostepping);
            this.Controls.Add(this.labelMotorStepAngle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.GearRatio1);
            this.Controls.Add(this.GearRatio0);
            this.Controls.Add(this.buttonCalculate);
            this.Controls.Add(this.LSScrewPitch);
            this.Controls.Add(this.LSMicroSteps);
            this.Controls.Add(this.LSMotorAngle);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LeadScrewCalculatorDialog";
            this.Text = "Lead Screw Calculator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox LSMotorAngle;
        private System.Windows.Forms.ComboBox LSMicroSteps;
        private System.Windows.Forms.ComboBox LSScrewPitch;
        private System.Windows.Forms.Button buttonCalculate;
        private System.Windows.Forms.TextBox GearRatio0;
        private System.Windows.Forms.TextBox GearRatio1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelMotorStepAngle;
        private System.Windows.Forms.Label labelMicrostepping;
        private System.Windows.Forms.Label labelLeadscrewPitch;
        private System.Windows.Forms.Label labelGearRatio;
        private System.Windows.Forms.TextBox LSOutput;
        private System.Windows.Forms.Label labelStepsPerMM;
    }
}