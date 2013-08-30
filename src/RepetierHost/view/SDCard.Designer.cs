namespace RepetierHost.view
{
    partial class SDCard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SDCard));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolAddFile = new System.Windows.Forms.ToolStripButton();
            this.toolDelFile = new System.Windows.Forms.ToolStripButton();
            this.toolStartPrint = new System.Windows.Forms.ToolStripButton();
            this.toolStopPrint = new System.Windows.Forms.ToolStripButton();
            this.toolMount = new System.Windows.Forms.ToolStripButton();
            this.toolUnmount = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.progress = new System.Windows.Forms.ToolStripProgressBar();
            this.files = new System.Windows.Forms.ListView();
            this.columnHeaderName = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderSize = new System.Windows.Forms.ColumnHeader();
            this.buttonClose = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.toolNewFolder = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolAddFile,
            this.toolDelFile,
            this.toolNewFolder,
            this.toolStartPrint,
            this.toolStopPrint,
            this.toolMount,
            this.toolUnmount});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(383, 39);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolAddFile
            // 
            this.toolAddFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolAddFile.Image = ((System.Drawing.Image)(resources.GetObject("toolAddFile.Image")));
            this.toolAddFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolAddFile.Name = "toolAddFile";
            this.toolAddFile.Size = new System.Drawing.Size(36, 36);
            this.toolAddFile.Text = "toolStripButton1";
            this.toolAddFile.ToolTipText = "Upload current file";
            this.toolAddFile.Click += new System.EventHandler(this.toolAddFile_Click);
            // 
            // toolDelFile
            // 
            this.toolDelFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolDelFile.Image = ((System.Drawing.Image)(resources.GetObject("toolDelFile.Image")));
            this.toolDelFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolDelFile.Name = "toolDelFile";
            this.toolDelFile.Size = new System.Drawing.Size(36, 36);
            this.toolDelFile.Text = "toolDelFile";
            this.toolDelFile.ToolTipText = "Delete file";
            this.toolDelFile.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStartPrint
            // 
            this.toolStartPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStartPrint.Enabled = false;
            this.toolStartPrint.Image = ((System.Drawing.Image)(resources.GetObject("toolStartPrint.Image")));
            this.toolStartPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStartPrint.Name = "toolStartPrint";
            this.toolStartPrint.Size = new System.Drawing.Size(36, 36);
            this.toolStartPrint.Text = "toolStripButton2";
            this.toolStartPrint.ToolTipText = "Run selected file / Continue paused print";
            this.toolStartPrint.Click += new System.EventHandler(this.toolStartPrint_Click);
            // 
            // toolStopPrint
            // 
            this.toolStopPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStopPrint.Image = ((System.Drawing.Image)(resources.GetObject("toolStopPrint.Image")));
            this.toolStopPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStopPrint.Name = "toolStopPrint";
            this.toolStopPrint.Size = new System.Drawing.Size(36, 36);
            this.toolStopPrint.Text = "toolStripButton3";
            this.toolStopPrint.ToolTipText = "Pause/Stop running sd print";
            this.toolStopPrint.Click += new System.EventHandler(this.toolStopPrint_Click);
            // 
            // toolMount
            // 
            this.toolMount.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolMount.Image = ((System.Drawing.Image)(resources.GetObject("toolMount.Image")));
            this.toolMount.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolMount.Name = "toolMount";
            this.toolMount.Size = new System.Drawing.Size(36, 36);
            this.toolMount.Text = "toolStripButton2";
            this.toolMount.ToolTipText = "Mount sd card";
            this.toolMount.Click += new System.EventHandler(this.toolMount_Click);
            // 
            // toolUnmount
            // 
            this.toolUnmount.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolUnmount.Image = ((System.Drawing.Image)(resources.GetObject("toolUnmount.Image")));
            this.toolUnmount.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolUnmount.Name = "toolUnmount";
            this.toolUnmount.Size = new System.Drawing.Size(36, 36);
            this.toolUnmount.Text = "toolStripButton3";
            this.toolUnmount.ToolTipText = "Unmount sd card";
            this.toolUnmount.Click += new System.EventHandler(this.toolUnmount_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStatus,
            this.progress});
            this.statusStrip1.Location = new System.Drawing.Point(0, 386);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 15, 0);
            this.statusStrip1.Size = new System.Drawing.Size(383, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStatus
            // 
            this.toolStatus.Name = "toolStatus";
            this.toolStatus.Size = new System.Drawing.Size(151, 17);
            this.toolStatus.Spring = true;
            this.toolStatus.Text = "-";
            // 
            // progress
            // 
            this.progress.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.progress.Maximum = 200;
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(214, 16);
            // 
            // files
            // 
            this.files.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.files.AllowColumnReorder = true;
            this.files.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.files.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderSize});
            this.files.FullRowSelect = true;
            this.files.Location = new System.Drawing.Point(13, 42);
            this.files.MultiSelect = false;
            this.files.Name = "files";
            this.files.Size = new System.Drawing.Size(360, 294);
            this.files.SmallImageList = this.imageList;
            this.files.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.files.TabIndex = 2;
            this.files.UseCompatibleStateImageBehavior = false;
            this.files.View = System.Windows.Forms.View.Details;
            this.files.SelectedIndexChanged += new System.EventHandler(this.files_SelectedIndexChanged);
            this.files.DoubleClick += new System.EventHandler(this.files_DoubleClick);
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 250;
            // 
            // columnHeaderSize
            // 
            this.columnHeaderSize.Text = "Size";
            this.columnHeaderSize.Width = 83;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(291, 342);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(80, 22);
            this.buttonClose.TabIndex = 3;
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
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "load16.png");
            this.imageList.Images.SetKeyName(1, "folder.png");
            // 
            // toolNewFolder
            // 
            this.toolNewFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolNewFolder.Image = ((System.Drawing.Image)(resources.GetObject("toolNewFolder.Image")));
            this.toolNewFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolNewFolder.Name = "toolNewFolder";
            this.toolNewFolder.Size = new System.Drawing.Size(36, 36);
            this.toolNewFolder.Text = "New folder";
            this.toolNewFolder.ToolTipText = "Create new folder";
            this.toolNewFolder.Click += new System.EventHandler(this.toolNewFolder_Click);
            // 
            // SDCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(383, 408);
            this.ControlBox = false;
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.files);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SDCard";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "SD card manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SDCard_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolAddFile;
        private System.Windows.Forms.ToolStripButton toolStartPrint;
        private System.Windows.Forms.ToolStripButton toolStopPrint;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStatus;
        private System.Windows.Forms.ToolStripProgressBar progress;
        private System.Windows.Forms.ListView files;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderSize;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStripButton toolDelFile;
        private System.Windows.Forms.ToolStripButton toolMount;
        private System.Windows.Forms.ToolStripButton toolUnmount;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStripButton toolNewFolder;
    }
}