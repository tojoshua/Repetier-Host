namespace RepetierHost.view
{
    partial class EEPROMMarlin
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

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EEPROMMarlin));
            this.labelStepsPerMM = new System.Windows.Forms.Label();
            this.xstepsbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ystepsbox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.zstepsbox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.estepsbox = new System.Windows.Forms.TextBox();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonRestore = new System.Windows.Forms.Button();
            this.buttonAbort = new System.Windows.Forms.Button();
            this.labelMaxFeedrate = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.xfeedbox = new System.Windows.Forms.TextBox();
            this.yfeedbox = new System.Windows.Forms.TextBox();
            this.zfeedbox = new System.Windows.Forms.TextBox();
            this.efeedbox = new System.Windows.Forms.TextBox();
            this.labelMaxAcceleration = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.maccxbox = new System.Windows.Forms.TextBox();
            this.maccybox = new System.Windows.Forms.TextBox();
            this.macczbox = new System.Windows.Forms.TextBox();
            this.maccebox = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.mzjerkbox = new System.Windows.Forms.TextBox();
            this.labelMaxZJerk = new System.Windows.Forms.Label();
            this.maxxyjerkbox = new System.Windows.Forms.TextBox();
            this.labelMaxXYJerk = new System.Windows.Forms.Label();
            this.minsegtbox = new System.Windows.Forms.TextBox();
            this.labelMinSegmentTime = new System.Windows.Forms.Label();
            this.mintfeedbox = new System.Windows.Forms.TextBox();
            this.labelMinTravelFeedrate = new System.Windows.Forms.Label();
            this.minfeedbox = new System.Windows.Forms.TextBox();
            this.labelMinFeedrate = new System.Windows.Forms.Label();
            this.labelAdvancedVariables = new System.Windows.Forms.Label();
            this.labelPIDSettings = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.ppidbox = new System.Windows.Forms.TextBox();
            this.ipidbox = new System.Windows.Forms.TextBox();
            this.dpidbox = new System.Windows.Forms.TextBox();
            this.accbox = new System.Windows.Forms.TextBox();
            this.lableRetractAcceleration = new System.Windows.Forms.Label();
            this.labelAcceleration = new System.Windows.Forms.Label();
            this.raccbox = new System.Windows.Forms.TextBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.hozbox = new System.Windows.Forms.TextBox();
            this.hoybox = new System.Windows.Forms.TextBox();
            this.hoxbox = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.labelHomingOffset = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // labelStepsPerMM
            // 
            this.labelStepsPerMM.AutoSize = true;
            this.labelStepsPerMM.Location = new System.Drawing.Point(12, 9);
            this.labelStepsPerMM.Name = "labelStepsPerMM";
            this.labelStepsPerMM.Size = new System.Drawing.Size(74, 13);
            this.labelStepsPerMM.TabIndex = 0;
            this.labelStepsPerMM.Text = "Steps per mm:";
            // 
            // xstepsbox
            // 
            this.xstepsbox.Location = new System.Drawing.Point(207, 6);
            this.xstepsbox.Name = "xstepsbox";
            this.xstepsbox.Size = new System.Drawing.Size(63, 20);
            this.xstepsbox.TabIndex = 0;
            this.xstepsbox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(184, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "X:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(276, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Y:";
            // 
            // ystepsbox
            // 
            this.ystepsbox.Location = new System.Drawing.Point(299, 6);
            this.ystepsbox.Name = "ystepsbox";
            this.ystepsbox.Size = new System.Drawing.Size(63, 20);
            this.ystepsbox.TabIndex = 1;
            this.ystepsbox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(368, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Z:";
            // 
            // zstepsbox
            // 
            this.zstepsbox.Location = new System.Drawing.Point(391, 6);
            this.zstepsbox.Name = "zstepsbox";
            this.zstepsbox.Size = new System.Drawing.Size(63, 20);
            this.zstepsbox.TabIndex = 2;
            this.zstepsbox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(460, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "E:";
            // 
            // estepsbox
            // 
            this.estepsbox.Location = new System.Drawing.Point(483, 6);
            this.estepsbox.Name = "estepsbox";
            this.estepsbox.Size = new System.Drawing.Size(63, 20);
            this.estepsbox.TabIndex = 3;
            this.estepsbox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(15, 356);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(149, 22);
            this.buttonLoad.TabIndex = 20;
            this.buttonLoad.Text = "Reload Config";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(349, 357);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(128, 22);
            this.buttonSave.TabIndex = 22;
            this.buttonSave.Text = "Save to EEPROM";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonRestore
            // 
            this.buttonRestore.Location = new System.Drawing.Point(170, 357);
            this.buttonRestore.Name = "buttonRestore";
            this.buttonRestore.Size = new System.Drawing.Size(173, 23);
            this.buttonRestore.TabIndex = 21;
            this.buttonRestore.Text = "Restore factory settings";
            this.buttonRestore.UseVisualStyleBackColor = true;
            this.buttonRestore.Click += new System.EventHandler(this.buttonRestore_Click);
            // 
            // buttonAbort
            // 
            this.buttonAbort.Location = new System.Drawing.Point(496, 356);
            this.buttonAbort.Name = "buttonAbort";
            this.buttonAbort.Size = new System.Drawing.Size(95, 23);
            this.buttonAbort.TabIndex = 23;
            this.buttonAbort.Text = "Cancel";
            this.buttonAbort.UseVisualStyleBackColor = true;
            this.buttonAbort.Click += new System.EventHandler(this.buttonAbort_Click_1);
            // 
            // labelMaxFeedrate
            // 
            this.labelMaxFeedrate.AutoSize = true;
            this.labelMaxFeedrate.Location = new System.Drawing.Point(12, 35);
            this.labelMaxFeedrate.Name = "labelMaxFeedrate";
            this.labelMaxFeedrate.Size = new System.Drawing.Size(136, 13);
            this.labelMaxFeedrate.TabIndex = 13;
            this.labelMaxFeedrate.Text = "Maximum feedrates [mm/s]:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(184, 35);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "X:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(276, 35);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Y:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(368, 35);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "Z:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(460, 35);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 13);
            this.label10.TabIndex = 17;
            this.label10.Text = "E:";
            // 
            // xfeedbox
            // 
            this.xfeedbox.Location = new System.Drawing.Point(207, 32);
            this.xfeedbox.Name = "xfeedbox";
            this.xfeedbox.Size = new System.Drawing.Size(63, 20);
            this.xfeedbox.TabIndex = 4;
            this.xfeedbox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // yfeedbox
            // 
            this.yfeedbox.Location = new System.Drawing.Point(299, 32);
            this.yfeedbox.Name = "yfeedbox";
            this.yfeedbox.Size = new System.Drawing.Size(63, 20);
            this.yfeedbox.TabIndex = 5;
            this.yfeedbox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // zfeedbox
            // 
            this.zfeedbox.Location = new System.Drawing.Point(391, 32);
            this.zfeedbox.Name = "zfeedbox";
            this.zfeedbox.Size = new System.Drawing.Size(63, 20);
            this.zfeedbox.TabIndex = 6;
            this.zfeedbox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // efeedbox
            // 
            this.efeedbox.Location = new System.Drawing.Point(483, 32);
            this.efeedbox.Name = "efeedbox";
            this.efeedbox.Size = new System.Drawing.Size(63, 20);
            this.efeedbox.TabIndex = 7;
            this.efeedbox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // labelMaxAcceleration
            // 
            this.labelMaxAcceleration.AutoSize = true;
            this.labelMaxAcceleration.Location = new System.Drawing.Point(12, 61);
            this.labelMaxAcceleration.Name = "labelMaxAcceleration";
            this.labelMaxAcceleration.Size = new System.Drawing.Size(154, 13);
            this.labelMaxAcceleration.TabIndex = 22;
            this.labelMaxAcceleration.Text = "Maximum Acceleration [mm/s²]:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(184, 61);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(17, 13);
            this.label12.TabIndex = 23;
            this.label12.Text = "X:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(276, 61);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(17, 13);
            this.label13.TabIndex = 24;
            this.label13.Text = "Y:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(368, 61);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(17, 13);
            this.label14.TabIndex = 25;
            this.label14.Text = "Z:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(460, 61);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(17, 13);
            this.label15.TabIndex = 26;
            this.label15.Text = "E:";
            // 
            // maccxbox
            // 
            this.maccxbox.Location = new System.Drawing.Point(207, 58);
            this.maccxbox.Name = "maccxbox";
            this.maccxbox.Size = new System.Drawing.Size(63, 20);
            this.maccxbox.TabIndex = 8;
            this.maccxbox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // maccybox
            // 
            this.maccybox.Location = new System.Drawing.Point(299, 58);
            this.maccybox.Name = "maccybox";
            this.maccybox.Size = new System.Drawing.Size(63, 20);
            this.maccybox.TabIndex = 9;
            this.maccybox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // macczbox
            // 
            this.macczbox.Location = new System.Drawing.Point(391, 58);
            this.macczbox.Name = "macczbox";
            this.macczbox.Size = new System.Drawing.Size(63, 20);
            this.macczbox.TabIndex = 10;
            this.macczbox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // maccebox
            // 
            this.maccebox.Location = new System.Drawing.Point(483, 58);
            this.maccebox.Name = "maccebox";
            this.maccebox.Size = new System.Drawing.Size(63, 20);
            this.maccebox.TabIndex = 11;
            this.maccebox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.mzjerkbox);
            this.panel2.Controls.Add(this.labelMaxZJerk);
            this.panel2.Controls.Add(this.maxxyjerkbox);
            this.panel2.Controls.Add(this.labelMaxXYJerk);
            this.panel2.Controls.Add(this.minsegtbox);
            this.panel2.Controls.Add(this.labelMinSegmentTime);
            this.panel2.Controls.Add(this.mintfeedbox);
            this.panel2.Controls.Add(this.labelMinTravelFeedrate);
            this.panel2.Controls.Add(this.minfeedbox);
            this.panel2.Controls.Add(this.labelMinFeedrate);
            this.panel2.Controls.Add(this.labelAdvancedVariables);
            this.panel2.Location = new System.Drawing.Point(15, 188);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(576, 134);
            this.panel2.TabIndex = 36;
            // 
            // mzjerkbox
            // 
            this.mzjerkbox.Location = new System.Drawing.Point(467, 60);
            this.mzjerkbox.Name = "mzjerkbox";
            this.mzjerkbox.Size = new System.Drawing.Size(63, 20);
            this.mzjerkbox.TabIndex = 4;
            this.mzjerkbox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // labelMaxZJerk
            // 
            this.labelMaxZJerk.AutoSize = true;
            this.labelMaxZJerk.Location = new System.Drawing.Point(312, 63);
            this.labelMaxZJerk.Name = "labelMaxZJerk";
            this.labelMaxZJerk.Size = new System.Drawing.Size(116, 13);
            this.labelMaxZJerk.TabIndex = 43;
            this.labelMaxZJerk.Text = "Maximum Z jerk [mm/s]";
            // 
            // maxxyjerkbox
            // 
            this.maxxyjerkbox.Location = new System.Drawing.Point(467, 34);
            this.maxxyjerkbox.Name = "maxxyjerkbox";
            this.maxxyjerkbox.Size = new System.Drawing.Size(63, 20);
            this.maxxyjerkbox.TabIndex = 3;
            this.maxxyjerkbox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // labelMaxXYJerk
            // 
            this.labelMaxXYJerk.AutoSize = true;
            this.labelMaxXYJerk.Location = new System.Drawing.Point(312, 37);
            this.labelMaxXYJerk.Name = "labelMaxXYJerk";
            this.labelMaxXYJerk.Size = new System.Drawing.Size(126, 13);
            this.labelMaxXYJerk.TabIndex = 41;
            this.labelMaxXYJerk.Text = "Maximum X-Y jerk [mm/s]";
            // 
            // minsegtbox
            // 
            this.minsegtbox.Location = new System.Drawing.Point(204, 86);
            this.minsegtbox.Name = "minsegtbox";
            this.minsegtbox.Size = new System.Drawing.Size(63, 20);
            this.minsegtbox.TabIndex = 2;
            this.minsegtbox.Validating += new System.ComponentModel.CancelEventHandler(this.int_Validating);
            // 
            // labelMinSegmentTime
            // 
            this.labelMinSegmentTime.AutoSize = true;
            this.labelMinSegmentTime.Location = new System.Drawing.Point(13, 89);
            this.labelMinSegmentTime.Name = "labelMinSegmentTime";
            this.labelMinSegmentTime.Size = new System.Drawing.Size(135, 13);
            this.labelMinSegmentTime.TabIndex = 39;
            this.labelMinSegmentTime.Text = "Minimum segment time [ms]";
            // 
            // mintfeedbox
            // 
            this.mintfeedbox.Location = new System.Drawing.Point(204, 60);
            this.mintfeedbox.Name = "mintfeedbox";
            this.mintfeedbox.Size = new System.Drawing.Size(63, 20);
            this.mintfeedbox.TabIndex = 1;
            this.mintfeedbox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // labelMinTravelFeedrate
            // 
            this.labelMinTravelFeedrate.AutoSize = true;
            this.labelMinTravelFeedrate.Location = new System.Drawing.Point(13, 63);
            this.labelMinTravelFeedrate.Name = "labelMinTravelFeedrate";
            this.labelMinTravelFeedrate.Size = new System.Drawing.Size(130, 13);
            this.labelMinTravelFeedrate.TabIndex = 37;
            this.labelMinTravelFeedrate.Text = "Min travel feedrate [mm/s]";
            // 
            // minfeedbox
            // 
            this.minfeedbox.Location = new System.Drawing.Point(204, 34);
            this.minfeedbox.Name = "minfeedbox";
            this.minfeedbox.Size = new System.Drawing.Size(63, 20);
            this.minfeedbox.TabIndex = 0;
            this.minfeedbox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // labelMinFeedrate
            // 
            this.labelMinFeedrate.AutoSize = true;
            this.labelMinFeedrate.Location = new System.Drawing.Point(13, 37);
            this.labelMinFeedrate.Name = "labelMinFeedrate";
            this.labelMinFeedrate.Size = new System.Drawing.Size(101, 13);
            this.labelMinFeedrate.TabIndex = 1;
            this.labelMinFeedrate.Text = "Min feedrate [mm/s]";
            // 
            // labelAdvancedVariables
            // 
            this.labelAdvancedVariables.AutoSize = true;
            this.labelAdvancedVariables.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAdvancedVariables.Location = new System.Drawing.Point(13, 4);
            this.labelAdvancedVariables.Name = "labelAdvancedVariables";
            this.labelAdvancedVariables.Size = new System.Drawing.Size(123, 13);
            this.labelAdvancedVariables.TabIndex = 0;
            this.labelAdvancedVariables.Text = "Advanced variables:";
            // 
            // labelPIDSettings
            // 
            this.labelPIDSettings.AutoSize = true;
            this.labelPIDSettings.Location = new System.Drawing.Point(12, 137);
            this.labelPIDSettings.Name = "labelPIDSettings";
            this.labelPIDSettings.Size = new System.Drawing.Size(67, 13);
            this.labelPIDSettings.TabIndex = 37;
            this.labelPIDSettings.Text = "PID settings:";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(184, 137);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(17, 13);
            this.label25.TabIndex = 46;
            this.label25.Text = "P:";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(276, 137);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(13, 13);
            this.label26.TabIndex = 47;
            this.label26.Text = "I:";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(364, 137);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(18, 13);
            this.label27.TabIndex = 49;
            this.label27.Text = "D:";
            // 
            // ppidbox
            // 
            this.ppidbox.Location = new System.Drawing.Point(207, 136);
            this.ppidbox.Name = "ppidbox";
            this.ppidbox.Size = new System.Drawing.Size(63, 20);
            this.ppidbox.TabIndex = 14;
            this.ppidbox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // ipidbox
            // 
            this.ipidbox.Location = new System.Drawing.Point(295, 134);
            this.ipidbox.Name = "ipidbox";
            this.ipidbox.Size = new System.Drawing.Size(63, 20);
            this.ipidbox.TabIndex = 15;
            this.ipidbox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // dpidbox
            // 
            this.dpidbox.Location = new System.Drawing.Point(388, 134);
            this.dpidbox.Name = "dpidbox";
            this.dpidbox.Size = new System.Drawing.Size(63, 20);
            this.dpidbox.TabIndex = 16;
            this.dpidbox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // accbox
            // 
            this.accbox.Location = new System.Drawing.Point(207, 84);
            this.accbox.Name = "accbox";
            this.accbox.Size = new System.Drawing.Size(63, 20);
            this.accbox.TabIndex = 12;
            this.accbox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // lableRetractAcceleration
            // 
            this.lableRetractAcceleration.AutoSize = true;
            this.lableRetractAcceleration.Location = new System.Drawing.Point(12, 113);
            this.lableRetractAcceleration.Name = "lableRetractAcceleration";
            this.lableRetractAcceleration.Size = new System.Drawing.Size(107, 13);
            this.lableRetractAcceleration.TabIndex = 33;
            this.lableRetractAcceleration.Text = "Retract Acceleration:";
            // 
            // labelAcceleration
            // 
            this.labelAcceleration.AutoSize = true;
            this.labelAcceleration.Location = new System.Drawing.Point(12, 87);
            this.labelAcceleration.Name = "labelAcceleration";
            this.labelAcceleration.Size = new System.Drawing.Size(69, 13);
            this.labelAcceleration.TabIndex = 31;
            this.labelAcceleration.Text = "Acceleration:";
            // 
            // raccbox
            // 
            this.raccbox.Location = new System.Drawing.Point(207, 110);
            this.raccbox.Name = "raccbox";
            this.raccbox.Size = new System.Drawing.Size(63, 20);
            this.raccbox.TabIndex = 13;
            this.raccbox.Validating += new System.ComponentModel.CancelEventHandler(this.floatPos_Validating);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // hozbox
            // 
            this.hozbox.Location = new System.Drawing.Point(388, 159);
            this.hozbox.Name = "hozbox";
            this.hozbox.Size = new System.Drawing.Size(63, 20);
            this.hozbox.TabIndex = 19;
            // 
            // hoybox
            // 
            this.hoybox.Location = new System.Drawing.Point(295, 159);
            this.hoybox.Name = "hoybox";
            this.hoybox.Size = new System.Drawing.Size(63, 20);
            this.hoybox.TabIndex = 18;
            // 
            // hoxbox
            // 
            this.hoxbox.Location = new System.Drawing.Point(207, 161);
            this.hoxbox.Name = "hoxbox";
            this.hoxbox.Size = new System.Drawing.Size(63, 20);
            this.hoxbox.TabIndex = 17;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(364, 162);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(17, 13);
            this.label17.TabIndex = 58;
            this.label17.Text = "Z:";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(276, 162);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(17, 13);
            this.label28.TabIndex = 57;
            this.label28.Text = "Y:";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(184, 162);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(17, 13);
            this.label29.TabIndex = 56;
            this.label29.Text = "X:";
            // 
            // labelHomingOffset
            // 
            this.labelHomingOffset.AutoSize = true;
            this.labelHomingOffset.Location = new System.Drawing.Point(12, 162);
            this.labelHomingOffset.Name = "labelHomingOffset";
            this.labelHomingOffset.Size = new System.Drawing.Size(81, 13);
            this.labelHomingOffset.TabIndex = 55;
            this.labelHomingOffset.Text = "Homeing offset:";
            // 
            // EEPROMMarlin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(615, 404);
            this.ControlBox = false;
            this.Controls.Add(this.hozbox);
            this.Controls.Add(this.hoybox);
            this.Controls.Add(this.hoxbox);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label28);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.labelHomingOffset);
            this.Controls.Add(this.raccbox);
            this.Controls.Add(this.dpidbox);
            this.Controls.Add(this.labelAcceleration);
            this.Controls.Add(this.ipidbox);
            this.Controls.Add(this.lableRetractAcceleration);
            this.Controls.Add(this.ppidbox);
            this.Controls.Add(this.accbox);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.labelPIDSettings);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.maccebox);
            this.Controls.Add(this.macczbox);
            this.Controls.Add(this.maccybox);
            this.Controls.Add(this.maccxbox);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.labelMaxAcceleration);
            this.Controls.Add(this.efeedbox);
            this.Controls.Add(this.zfeedbox);
            this.Controls.Add(this.yfeedbox);
            this.Controls.Add(this.xfeedbox);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.labelMaxFeedrate);
            this.Controls.Add(this.buttonAbort);
            this.Controls.Add(this.buttonRestore);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.estepsbox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.zstepsbox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ystepsbox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.xstepsbox);
            this.Controls.Add(this.labelStepsPerMM);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EEPROMMarlin";
            this.Text = "Marlin Firmware EEPROM settings";
            this.Activated += new System.EventHandler(this.EEPROMMarlin_Activated);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelStepsPerMM;
        private System.Windows.Forms.TextBox xstepsbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ystepsbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox zstepsbox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox estepsbox;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonRestore;
        private System.Windows.Forms.Button buttonAbort;
        private System.Windows.Forms.Label labelMaxFeedrate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox xfeedbox;
        private System.Windows.Forms.TextBox yfeedbox;
        private System.Windows.Forms.TextBox zfeedbox;
        private System.Windows.Forms.TextBox efeedbox;
        private System.Windows.Forms.Label labelMaxAcceleration;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox maccxbox;
        private System.Windows.Forms.TextBox maccybox;
        private System.Windows.Forms.TextBox macczbox;
        private System.Windows.Forms.TextBox maccebox;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label labelAdvancedVariables;
        private System.Windows.Forms.Label labelMinFeedrate;
        private System.Windows.Forms.Label labelMinTravelFeedrate;
        private System.Windows.Forms.TextBox minfeedbox;
        private System.Windows.Forms.TextBox mintfeedbox;
        private System.Windows.Forms.Label labelMinSegmentTime;
        private System.Windows.Forms.TextBox minsegtbox;
        private System.Windows.Forms.TextBox maxxyjerkbox;
        private System.Windows.Forms.Label labelMaxXYJerk;
        private System.Windows.Forms.TextBox mzjerkbox;
        private System.Windows.Forms.Label labelMaxZJerk;
        private System.Windows.Forms.Label labelPIDSettings;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox ppidbox;
        private System.Windows.Forms.TextBox ipidbox;
        private System.Windows.Forms.TextBox dpidbox;
        private System.Windows.Forms.TextBox accbox;
        private System.Windows.Forms.Label lableRetractAcceleration;
        private System.Windows.Forms.Label labelAcceleration;
        private System.Windows.Forms.TextBox raccbox;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.TextBox hozbox;
        private System.Windows.Forms.TextBox hoybox;
        private System.Windows.Forms.TextBox hoxbox;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label labelHomingOffset;
       
    }
}