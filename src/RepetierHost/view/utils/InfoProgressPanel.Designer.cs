namespace RepetierHost.view.utils
{
    partial class InfoProgressPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InfoProgressPanel));
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelAction = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.buttonKill = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.Location = new System.Drawing.Point(4, 4);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(52, 17);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "label1";
            // 
            // labelAction
            // 
            this.labelAction.AutoSize = true;
            this.labelAction.Location = new System.Drawing.Point(4, 26);
            this.labelAction.Name = "labelAction";
            this.labelAction.Size = new System.Drawing.Size(35, 13);
            this.labelAction.TabIndex = 0;
            this.labelAction.Text = "label1";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(7, 42);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(350, 16);
            this.progressBar.TabIndex = 2;
            this.progressBar.Minimum = 0;
            this.progressBar.Maximum = 100;
            // 
            // timer
            // 
            this.timer.Interval = 150;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // buttonKill
            // 
            this.buttonKill.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonKill.FlatAppearance.BorderSize = 0;
            this.buttonKill.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonKill.Image = ((System.Drawing.Image)(resources.GetObject("buttonKill.Image")));
            this.buttonKill.Location = new System.Drawing.Point(318, 4);
            this.buttonKill.Name = "buttonKill";
            this.buttonKill.Size = new System.Drawing.Size(39, 35);
            this.buttonKill.TabIndex = 1;
            this.buttonKill.UseVisualStyleBackColor = true;
            this.buttonKill.Click += new System.EventHandler(this.buttonKill_Click);
            // 
            // InfoProgressPanel
            // 
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonKill);
            this.Controls.Add(this.labelAction);
            this.Controls.Add(this.labelTitle);
            this.Name = "InfoProgressPanel";
            this.Size = new System.Drawing.Size(369, 69);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelAction;
        private System.Windows.Forms.Button buttonKill;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Timer timer;
    }
}
