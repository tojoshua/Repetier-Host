namespace RepetierHost.view
{
    partial class CopyObjectsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CopyObjectsDialog));
            this.labelNumberOfCopies = new System.Windows.Forms.Label();
            this.numericCopies = new System.Windows.Forms.NumericUpDown();
            this.checkAutoposition = new System.Windows.Forms.CheckBox();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericCopies)).BeginInit();
            this.SuspendLayout();
            // 
            // labelNumberOfCopies
            // 
            this.labelNumberOfCopies.AutoSize = true;
            this.labelNumberOfCopies.Location = new System.Drawing.Point(13, 13);
            this.labelNumberOfCopies.Name = "labelNumberOfCopies";
            this.labelNumberOfCopies.Size = new System.Drawing.Size(93, 13);
            this.labelNumberOfCopies.TabIndex = 0;
            this.labelNumberOfCopies.Text = "Number of copies:";
            // 
            // numericCopies
            // 
            this.numericCopies.Location = new System.Drawing.Point(113, 13);
            this.numericCopies.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numericCopies.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericCopies.Name = "numericCopies";
            this.numericCopies.Size = new System.Drawing.Size(51, 20);
            this.numericCopies.TabIndex = 1;
            this.numericCopies.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // checkAutoposition
            // 
            this.checkAutoposition.AutoSize = true;
            this.checkAutoposition.Checked = true;
            this.checkAutoposition.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkAutoposition.Location = new System.Drawing.Point(16, 47);
            this.checkAutoposition.Name = "checkAutoposition";
            this.checkAutoposition.Size = new System.Drawing.Size(183, 17);
            this.checkAutoposition.TabIndex = 2;
            this.checkAutoposition.Text = "Auto position after adding objects";
            this.checkAutoposition.UseVisualStyleBackColor = true;
            // 
            // buttonCopy
            // 
            this.buttonCopy.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonCopy.Location = new System.Drawing.Point(16, 70);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(75, 23);
            this.buttonCopy.TabIndex = 3;
            this.buttonCopy.Text = "Copy";
            this.buttonCopy.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(140, 70);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // CopyObjectsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(237, 113);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonCopy);
            this.Controls.Add(this.checkAutoposition);
            this.Controls.Add(this.numericCopies);
            this.Controls.Add(this.labelNumberOfCopies);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CopyObjectsDialog";
            this.ShowInTaskbar = false;
            this.Text = "Copy marked objects";
            ((System.ComponentModel.ISupportInitialize)(this.numericCopies)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelNumberOfCopies;
        public System.Windows.Forms.NumericUpDown numericCopies;
        public System.Windows.Forms.CheckBox checkAutoposition;
        private System.Windows.Forms.Button buttonCopy;
        private System.Windows.Forms.Button buttonCancel;
    }
}