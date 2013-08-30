namespace RepetierHost.view
{
    partial class BeltCalculatorDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BeltCalculatorDialog));
            this.MotorStepAngleInput = new System.Windows.Forms.ComboBox();
            this.MotorMicrosteppingInput = new System.Windows.Forms.ComboBox();
            this.BeltPitchInput = new System.Windows.Forms.ComboBox();
            this.ToothCountInput = new System.Windows.Forms.TextBox();
            this.labelMotorStepAngle = new System.Windows.Forms.Label();
            this.labelDriverMicrostepping = new System.Windows.Forms.Label();
            this.labelBeltPitch = new System.Windows.Forms.Label();
            this.labelToothCount = new System.Windows.Forms.Label();
            this.labelStepsPerMM = new System.Windows.Forms.Label();
            this.StepsPerMMOutput = new System.Windows.Forms.TextBox();
            this.BeltCalculatorCalculateButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // MotorStepAngleInput
            // 
            this.MotorStepAngleInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MotorStepAngleInput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MotorStepAngleInput.FormattingEnabled = true;
            this.MotorStepAngleInput.Items.AddRange(new object[] {
            "1.8",
            "0.9",
            "7.5"});
            this.MotorStepAngleInput.Location = new System.Drawing.Point(231, 12);
            this.MotorStepAngleInput.Name = "MotorStepAngleInput";
            this.MotorStepAngleInput.Size = new System.Drawing.Size(121, 21);
            this.MotorStepAngleInput.TabIndex = 0;
            // 
            // MotorMicrosteppingInput
            // 
            this.MotorMicrosteppingInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MotorMicrosteppingInput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MotorMicrosteppingInput.FormattingEnabled = true;
            this.MotorMicrosteppingInput.Items.AddRange(new object[] {
            "1",
            "1/2",
            "1/4",
            "1/8",
            "1/16",
            "1/32"});
            this.MotorMicrosteppingInput.Location = new System.Drawing.Point(231, 40);
            this.MotorMicrosteppingInput.Name = "MotorMicrosteppingInput";
            this.MotorMicrosteppingInput.Size = new System.Drawing.Size(121, 21);
            this.MotorMicrosteppingInput.TabIndex = 1;
            // 
            // BeltPitchInput
            // 
            this.BeltPitchInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BeltPitchInput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BeltPitchInput.FormattingEnabled = true;
            this.BeltPitchInput.Items.AddRange(new object[] {
            "2mm ( GT2 Belt )",
            "2.03mm ( MXL Belt )",
            "2.5mm ( T2.5 )",
            "3mm ( GT2, HTD )",
            "5mm ( T5, GT2, HTD )",
            "5.08mm ( 0.2\" XL Belt )"});
            this.BeltPitchInput.Location = new System.Drawing.Point(231, 68);
            this.BeltPitchInput.Name = "BeltPitchInput";
            this.BeltPitchInput.Size = new System.Drawing.Size(121, 21);
            this.BeltPitchInput.TabIndex = 2;
            // 
            // ToothCountInput
            // 
            this.ToothCountInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ToothCountInput.Location = new System.Drawing.Point(231, 96);
            this.ToothCountInput.Name = "ToothCountInput";
            this.ToothCountInput.Size = new System.Drawing.Size(121, 20);
            this.ToothCountInput.TabIndex = 3;
            this.ToothCountInput.Text = "18";
            // 
            // labelMotorStepAngle
            // 
            this.labelMotorStepAngle.AutoSize = true;
            this.labelMotorStepAngle.Location = new System.Drawing.Point(12, 15);
            this.labelMotorStepAngle.Name = "labelMotorStepAngle";
            this.labelMotorStepAngle.Size = new System.Drawing.Size(86, 13);
            this.labelMotorStepAngle.TabIndex = 4;
            this.labelMotorStepAngle.Text = "Motor step angle";
            // 
            // labelDriverMicrostepping
            // 
            this.labelDriverMicrostepping.AutoSize = true;
            this.labelDriverMicrostepping.Location = new System.Drawing.Point(12, 43);
            this.labelDriverMicrostepping.Name = "labelDriverMicrostepping";
            this.labelDriverMicrostepping.Size = new System.Drawing.Size(104, 13);
            this.labelDriverMicrostepping.TabIndex = 5;
            this.labelDriverMicrostepping.Text = "Driver Microstepping";
            // 
            // labelBeltPitch
            // 
            this.labelBeltPitch.AutoSize = true;
            this.labelBeltPitch.Location = new System.Drawing.Point(12, 71);
            this.labelBeltPitch.Name = "labelBeltPitch";
            this.labelBeltPitch.Size = new System.Drawing.Size(52, 13);
            this.labelBeltPitch.TabIndex = 6;
            this.labelBeltPitch.Text = "Belt Pitch";
            // 
            // labelToothCount
            // 
            this.labelToothCount.AutoSize = true;
            this.labelToothCount.Location = new System.Drawing.Point(12, 99);
            this.labelToothCount.Name = "labelToothCount";
            this.labelToothCount.Size = new System.Drawing.Size(66, 13);
            this.labelToothCount.TabIndex = 7;
            this.labelToothCount.Text = "Tooth Count";
            // 
            // labelStepsPerMM
            // 
            this.labelStepsPerMM.AutoSize = true;
            this.labelStepsPerMM.Location = new System.Drawing.Point(12, 154);
            this.labelStepsPerMM.Name = "labelStepsPerMM";
            this.labelStepsPerMM.Size = new System.Drawing.Size(103, 13);
            this.labelStepsPerMM.TabIndex = 8;
            this.labelStepsPerMM.Text = "Steps per mm value ";
            // 
            // StepsPerMMOutput
            // 
            this.StepsPerMMOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StepsPerMMOutput.Location = new System.Drawing.Point(231, 151);
            this.StepsPerMMOutput.Name = "StepsPerMMOutput";
            this.StepsPerMMOutput.ReadOnly = true;
            this.StepsPerMMOutput.Size = new System.Drawing.Size(121, 20);
            this.StepsPerMMOutput.TabIndex = 9;
            // 
            // BeltCalculatorCalculateButton
            // 
            this.BeltCalculatorCalculateButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BeltCalculatorCalculateButton.Location = new System.Drawing.Point(15, 122);
            this.BeltCalculatorCalculateButton.Name = "BeltCalculatorCalculateButton";
            this.BeltCalculatorCalculateButton.Size = new System.Drawing.Size(337, 23);
            this.BeltCalculatorCalculateButton.TabIndex = 10;
            this.BeltCalculatorCalculateButton.Text = "Calculate";
            this.BeltCalculatorCalculateButton.UseVisualStyleBackColor = true;
            this.BeltCalculatorCalculateButton.Click += new System.EventHandler(this.BeltCalculatorCalculateButton_Click);
            // 
            // BeltCalculatorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 177);
            this.Controls.Add(this.BeltCalculatorCalculateButton);
            this.Controls.Add(this.StepsPerMMOutput);
            this.Controls.Add(this.labelStepsPerMM);
            this.Controls.Add(this.labelToothCount);
            this.Controls.Add(this.labelBeltPitch);
            this.Controls.Add(this.labelDriverMicrostepping);
            this.Controls.Add(this.labelMotorStepAngle);
            this.Controls.Add(this.ToothCountInput);
            this.Controls.Add(this.BeltPitchInput);
            this.Controls.Add(this.MotorMicrosteppingInput);
            this.Controls.Add(this.MotorStepAngleInput);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BeltCalculatorDialog";
            this.Text = "Belt Calculator";
            this.Load += new System.EventHandler(this.BeltCalculator_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox MotorStepAngleInput;
        private System.Windows.Forms.ComboBox MotorMicrosteppingInput;
        private System.Windows.Forms.ComboBox BeltPitchInput;
        private System.Windows.Forms.TextBox ToothCountInput;
        private System.Windows.Forms.Label labelMotorStepAngle;
        private System.Windows.Forms.Label labelDriverMicrostepping;
        private System.Windows.Forms.Label labelBeltPitch;
        private System.Windows.Forms.Label labelToothCount;
        private System.Windows.Forms.Label labelStepsPerMM;
        private System.Windows.Forms.TextBox StepsPerMMOutput;
        private System.Windows.Forms.Button BeltCalculatorCalculateButton;
    }
}