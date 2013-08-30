namespace RepetierHost.view
{
    partial class RepetierEditor
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
                blackBrush.Dispose();
                drawFont.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepetierEditor));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolNew = new System.Windows.Forms.ToolStripButton();
            this.toolSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolCut = new System.Windows.Forms.ToolStripButton();
            this.toolCopy = new System.Windows.Forms.ToolStripButton();
            this.toolPaste = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolUndo = new System.Windows.Forms.ToolStripButton();
            this.toolRedo = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolPreview = new System.Windows.Forms.ToolStripButton();
            this.toolFile = new System.Windows.Forms.ToolStripComboBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolRow = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolColumn = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolMode = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolLayer = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolExtruder = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolPrintingTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolUpdating = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.scrollRows = new System.Windows.Forms.VScrollBar();
            this.scrollColumns = new System.Windows.Forms.HScrollBar();
            this.tabHelpview = new System.Windows.Forms.TabControl();
            this.tabPageVisualization = new System.Windows.Forms.TabPage();
            this.buttonGoLastLayer = new System.Windows.Forms.Button();
            this.buttonGoFirstLayer = new System.Windows.Forms.Button();
            this.labelMaxLayer = new System.Windows.Forms.Label();
            this.numericShowMaxLayer = new System.Windows.Forms.NumericUpDown();
            this.numericShowMinLayer = new System.Windows.Forms.NumericUpDown();
            this.radioShowLayerRange = new System.Windows.Forms.RadioButton();
            this.radioShowSingleLayer = new System.Windows.Forms.RadioButton();
            this.radioShowAll = new System.Windows.Forms.RadioButton();
            this.tabPageHelp = new System.Windows.Forms.TabPage();
            this.help = new System.Windows.Forms.RichTextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.editor = new RepetierHost.view.DoubleBufferPanel();
            this.sliderShowMaxLayer = new MB.Controls.ColorSlider();
            this.sliderShowFirstLayer = new MB.Controls.ColorSlider();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.tabHelpview.SuspendLayout();
            this.tabPageVisualization.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericShowMaxLayer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericShowMinLayer)).BeginInit();
            this.tabPageHelp.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolNew,
            this.toolSave,
            this.toolStripSeparator1,
            this.toolCut,
            this.toolCopy,
            this.toolPaste,
            this.toolStripSeparator2,
            this.toolUndo,
            this.toolRedo,
            this.toolStripSeparator3,
            this.toolPreview,
            this.toolFile});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(615, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip";
            // 
            // toolNew
            // 
            this.toolNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolNew.Image = ((System.Drawing.Image)(resources.GetObject("toolNew.Image")));
            this.toolNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolNew.Name = "toolNew";
            this.toolNew.Size = new System.Drawing.Size(23, 22);
            this.toolNew.Text = "toolStripButton1";
            this.toolNew.ToolTipText = "New text";
            this.toolNew.Click += new System.EventHandler(this.toolNew_Click);
            // 
            // toolSave
            // 
            this.toolSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolSave.Image = ((System.Drawing.Image)(resources.GetObject("toolSave.Image")));
            this.toolSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolSave.Name = "toolSave";
            this.toolSave.Size = new System.Drawing.Size(23, 22);
            this.toolSave.Text = "toolStripButton2";
            this.toolSave.ToolTipText = "Save";
            this.toolSave.Click += new System.EventHandler(this.toolSave_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolCut
            // 
            this.toolCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolCut.Image = ((System.Drawing.Image)(resources.GetObject("toolCut.Image")));
            this.toolCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolCut.Name = "toolCut";
            this.toolCut.Size = new System.Drawing.Size(23, 22);
            this.toolCut.Text = "toolStripButton1";
            this.toolCut.ToolTipText = "Cut";
            this.toolCut.Click += new System.EventHandler(this.toolCut_Click);
            // 
            // toolCopy
            // 
            this.toolCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolCopy.Image = ((System.Drawing.Image)(resources.GetObject("toolCopy.Image")));
            this.toolCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolCopy.Name = "toolCopy";
            this.toolCopy.Size = new System.Drawing.Size(23, 22);
            this.toolCopy.Text = "toolStripButton1";
            this.toolCopy.ToolTipText = "Copy";
            this.toolCopy.Click += new System.EventHandler(this.toolCopy_Click);
            // 
            // toolPaste
            // 
            this.toolPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolPaste.Image = ((System.Drawing.Image)(resources.GetObject("toolPaste.Image")));
            this.toolPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolPaste.Name = "toolPaste";
            this.toolPaste.Size = new System.Drawing.Size(23, 22);
            this.toolPaste.Text = "toolStripButton1";
            this.toolPaste.ToolTipText = "Paste";
            this.toolPaste.Click += new System.EventHandler(this.toolPaste_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolUndo
            // 
            this.toolUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolUndo.Image = ((System.Drawing.Image)(resources.GetObject("toolUndo.Image")));
            this.toolUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolUndo.Name = "toolUndo";
            this.toolUndo.Size = new System.Drawing.Size(23, 22);
            this.toolUndo.Text = "toolStripButton1";
            this.toolUndo.ToolTipText = "Undo";
            this.toolUndo.Click += new System.EventHandler(this.toolUndo_Click);
            // 
            // toolRedo
            // 
            this.toolRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolRedo.Image = ((System.Drawing.Image)(resources.GetObject("toolRedo.Image")));
            this.toolRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolRedo.Name = "toolRedo";
            this.toolRedo.Size = new System.Drawing.Size(23, 22);
            this.toolRedo.Text = "toolStripButton1";
            this.toolRedo.ToolTipText = "Redo";
            this.toolRedo.Click += new System.EventHandler(this.toolRedo_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolPreview
            // 
            this.toolPreview.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolPreview.Checked = true;
            this.toolPreview.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolPreview.Image = ((System.Drawing.Image)(resources.GetObject("toolPreview.Image")));
            this.toolPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolPreview.Name = "toolPreview";
            this.toolPreview.Size = new System.Drawing.Size(23, 22);
            this.toolPreview.Text = "toolStripButton3";
            this.toolPreview.ToolTipText = "Autoupdate preview code";
            this.toolPreview.CheckedChanged += new System.EventHandler(this.toolPreview_CheckedChanged);
            this.toolPreview.Click += new System.EventHandler(this.toolPreview_Click);
            // 
            // toolFile
            // 
            this.toolFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolFile.Items.AddRange(new object[] {
            "G-Code",
            "Start code",
            "End code",
            "Run on kill",
            "Run on pause"});
            this.toolFile.Name = "toolFile";
            this.toolFile.Size = new System.Drawing.Size(110, 25);
            this.toolFile.SelectedIndexChanged += new System.EventHandler(this.toolFile_SelectedIndexChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolRow,
            this.toolColumn,
            this.toolMode,
            this.toolLayer,
            this.toolExtruder,
            this.toolPrintingTime,
            this.toolUpdating});
            this.statusStrip1.Location = new System.Drawing.Point(0, 468);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(615, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolRow
            // 
            this.toolRow.Name = "toolRow";
            this.toolRow.Size = new System.Drawing.Size(20, 17);
            this.toolRow.Text = "R1";
            // 
            // toolColumn
            // 
            this.toolColumn.Name = "toolColumn";
            this.toolColumn.Size = new System.Drawing.Size(21, 17);
            this.toolColumn.Text = "C1";
            // 
            // toolMode
            // 
            this.toolMode.Name = "toolMode";
            this.toolMode.Size = new System.Drawing.Size(36, 17);
            this.toolMode.Text = "Insert";
            // 
            // toolLayer
            // 
            this.toolLayer.Name = "toolLayer";
            this.toolLayer.Size = new System.Drawing.Size(43, 17);
            this.toolLayer.Text = "Layer -";
            // 
            // toolExtruder
            // 
            this.toolExtruder.Name = "toolExtruder";
            this.toolExtruder.Size = new System.Drawing.Size(59, 17);
            this.toolExtruder.Text = "Extruder 0";
            // 
            // toolPrintingTime
            // 
            this.toolPrintingTime.Name = "toolPrintingTime";
            this.toolPrintingTime.Size = new System.Drawing.Size(0, 17);
            // 
            // toolUpdating
            // 
            this.toolUpdating.Name = "toolUpdating";
            this.toolUpdating.Size = new System.Drawing.Size(421, 17);
            this.toolUpdating.Spring = true;
            this.toolUpdating.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 500;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(0, 25);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.editor);
            this.splitContainer.Panel1.Controls.Add(this.scrollRows);
            this.splitContainer.Panel1.Controls.Add(this.scrollColumns);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.AutoScroll = true;
            this.splitContainer.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer.Panel2.Controls.Add(this.tabHelpview);
            this.splitContainer.Panel2.Margin = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.splitContainer.Size = new System.Drawing.Size(615, 443);
            this.splitContainer.SplitterDistance = 318;
            this.splitContainer.TabIndex = 2;
            this.splitContainer.KeyUp += new System.Windows.Forms.KeyEventHandler(this.splitContainer_KeyUp);
            this.splitContainer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RepetierEditor_KeyPress);
            this.splitContainer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RepetierEditor_KeyDown);
            // 
            // scrollRows
            // 
            this.scrollRows.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.scrollRows.Location = new System.Drawing.Point(598, 0);
            this.scrollRows.Name = "scrollRows";
            this.scrollRows.Size = new System.Drawing.Size(17, 301);
            this.scrollRows.TabIndex = 1;
            this.scrollRows.ValueChanged += new System.EventHandler(this.scrollRows_ValueChanged);
            // 
            // scrollColumns
            // 
            this.scrollColumns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.scrollColumns.Location = new System.Drawing.Point(3, 301);
            this.scrollColumns.Name = "scrollColumns";
            this.scrollColumns.Size = new System.Drawing.Size(595, 18);
            this.scrollColumns.TabIndex = 0;
            this.scrollColumns.ValueChanged += new System.EventHandler(this.scrollColumns_ValueChanged);
            // 
            // tabHelpview
            // 
            this.tabHelpview.Controls.Add(this.tabPageVisualization);
            this.tabHelpview.Controls.Add(this.tabPageHelp);
            this.tabHelpview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabHelpview.Location = new System.Drawing.Point(0, 0);
            this.tabHelpview.Name = "tabHelpview";
            this.tabHelpview.SelectedIndex = 0;
            this.tabHelpview.Size = new System.Drawing.Size(615, 121);
            this.tabHelpview.TabIndex = 1;
            // 
            // tabPageVisualization
            // 
            this.tabPageVisualization.Controls.Add(this.flowLayoutPanel1);
            this.tabPageVisualization.Controls.Add(this.buttonGoLastLayer);
            this.tabPageVisualization.Controls.Add(this.buttonGoFirstLayer);
            this.tabPageVisualization.Controls.Add(this.labelMaxLayer);
            this.tabPageVisualization.Controls.Add(this.numericShowMaxLayer);
            this.tabPageVisualization.Controls.Add(this.numericShowMinLayer);
            this.tabPageVisualization.Controls.Add(this.sliderShowMaxLayer);
            this.tabPageVisualization.Controls.Add(this.sliderShowFirstLayer);
            this.tabPageVisualization.Location = new System.Drawing.Point(4, 22);
            this.tabPageVisualization.Name = "tabPageVisualization";
            this.tabPageVisualization.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageVisualization.Size = new System.Drawing.Size(607, 95);
            this.tabPageVisualization.TabIndex = 1;
            this.tabPageVisualization.Text = "Visualization";
            this.tabPageVisualization.UseVisualStyleBackColor = true;
            // 
            // buttonGoLastLayer
            // 
            this.buttonGoLastLayer.Location = new System.Drawing.Point(10, 65);
            this.buttonGoLastLayer.Name = "buttonGoLastLayer";
            this.buttonGoLastLayer.Size = new System.Drawing.Size(132, 23);
            this.buttonGoLastLayer.TabIndex = 7;
            this.buttonGoLastLayer.Text = "Last Layer";
            this.buttonGoLastLayer.UseVisualStyleBackColor = true;
            this.buttonGoLastLayer.Click += new System.EventHandler(this.buttonGoLastLayer_Click);
            // 
            // buttonGoFirstLayer
            // 
            this.buttonGoFirstLayer.Location = new System.Drawing.Point(10, 37);
            this.buttonGoFirstLayer.Name = "buttonGoFirstLayer";
            this.buttonGoFirstLayer.Size = new System.Drawing.Size(132, 23);
            this.buttonGoFirstLayer.TabIndex = 7;
            this.buttonGoFirstLayer.Text = "First Layer";
            this.buttonGoFirstLayer.UseVisualStyleBackColor = true;
            this.buttonGoFirstLayer.Click += new System.EventHandler(this.buttonGoFirstLayer_Click);
            // 
            // labelMaxLayer
            // 
            this.labelMaxLayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMaxLayer.AutoSize = true;
            this.labelMaxLayer.Location = new System.Drawing.Point(572, 71);
            this.labelMaxLayer.Name = "labelMaxLayer";
            this.labelMaxLayer.Size = new System.Drawing.Size(10, 13);
            this.labelMaxLayer.TabIndex = 6;
            this.labelMaxLayer.Text = "-";
            this.labelMaxLayer.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // numericShowMaxLayer
            // 
            this.numericShowMaxLayer.Location = new System.Drawing.Point(144, 66);
            this.numericShowMaxLayer.Name = "numericShowMaxLayer";
            this.numericShowMaxLayer.Size = new System.Drawing.Size(59, 20);
            this.numericShowMaxLayer.TabIndex = 4;
            this.numericShowMaxLayer.ValueChanged += new System.EventHandler(this.numericShowMaxLayer_ValueChanged);
            // 
            // numericShowMinLayer
            // 
            this.numericShowMinLayer.Location = new System.Drawing.Point(144, 40);
            this.numericShowMinLayer.Name = "numericShowMinLayer";
            this.numericShowMinLayer.Size = new System.Drawing.Size(59, 20);
            this.numericShowMinLayer.TabIndex = 3;
            this.numericShowMinLayer.ValueChanged += new System.EventHandler(this.numericShowMinLayer_ValueChanged);
            // 
            // radioShowLayerRange
            // 
            this.radioShowLayerRange.AutoSize = true;
            this.radioShowLayerRange.Location = new System.Drawing.Point(247, 3);
            this.radioShowLayerRange.Name = "radioShowLayerRange";
            this.radioShowLayerRange.Size = new System.Drawing.Size(107, 17);
            this.radioShowLayerRange.TabIndex = 2;
            this.radioShowLayerRange.Tag = "2";
            this.radioShowLayerRange.Text = "Show layer range";
            this.radioShowLayerRange.UseVisualStyleBackColor = true;
            this.radioShowLayerRange.Click += new System.EventHandler(this.radioShowMode_Click);
            // 
            // radioShowSingleLayer
            // 
            this.radioShowSingleLayer.AutoSize = true;
            this.radioShowSingleLayer.Location = new System.Drawing.Point(134, 3);
            this.radioShowSingleLayer.Name = "radioShowSingleLayer";
            this.radioShowSingleLayer.Size = new System.Drawing.Size(107, 17);
            this.radioShowSingleLayer.TabIndex = 1;
            this.radioShowSingleLayer.Tag = "1";
            this.radioShowSingleLayer.Text = "Show single layer";
            this.radioShowSingleLayer.UseVisualStyleBackColor = true;
            this.radioShowSingleLayer.Click += new System.EventHandler(this.radioShowMode_Click);
            // 
            // radioShowAll
            // 
            this.radioShowAll.AutoSize = true;
            this.radioShowAll.Checked = true;
            this.radioShowAll.Location = new System.Drawing.Point(3, 3);
            this.radioShowAll.Name = "radioShowAll";
            this.radioShowAll.Size = new System.Drawing.Size(125, 17);
            this.radioShowAll.TabIndex = 0;
            this.radioShowAll.TabStop = true;
            this.radioShowAll.Tag = "0";
            this.radioShowAll.Text = "Show complete code";
            this.radioShowAll.UseVisualStyleBackColor = true;
            this.radioShowAll.Click += new System.EventHandler(this.radioShowMode_Click);
            // 
            // tabPageHelp
            // 
            this.tabPageHelp.Controls.Add(this.help);
            this.tabPageHelp.Location = new System.Drawing.Point(4, 22);
            this.tabPageHelp.Name = "tabPageHelp";
            this.tabPageHelp.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageHelp.Size = new System.Drawing.Size(607, 95);
            this.tabPageHelp.TabIndex = 0;
            this.tabPageHelp.Text = "Help";
            this.tabPageHelp.UseVisualStyleBackColor = true;
            // 
            // help
            // 
            this.help.Dock = System.Windows.Forms.DockStyle.Fill;
            this.help.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.help.Location = new System.Drawing.Point(3, 3);
            this.help.Name = "help";
            this.help.ReadOnly = true;
            this.help.Size = new System.Drawing.Size(601, 89);
            this.help.TabIndex = 0;
            this.help.TabStop = false;
            this.help.Text = "";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.radioShowAll);
            this.flowLayoutPanel1.Controls.Add(this.radioShowSingleLayer);
            this.flowLayoutPanel1.Controls.Add(this.radioShowLayerRange);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(607, 31);
            this.flowLayoutPanel1.TabIndex = 8;
            // 
            // editor
            // 
            this.editor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.editor.Location = new System.Drawing.Point(3, 3);
            this.editor.Name = "editor";
            this.editor.Size = new System.Drawing.Size(595, 298);
            this.editor.TabIndex = 2;
            this.editor.MouseLeave += new System.EventHandler(this.editor_MouseLeave);
            this.editor.Paint += new System.Windows.Forms.PaintEventHandler(this.editor_Paint);
            this.editor.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.editor_PreviewKeyDown);
            this.editor.MouseMove += new System.Windows.Forms.MouseEventHandler(this.editor_MouseMove);
            this.editor.Click += new System.EventHandler(this.editor_Click);
            this.editor.Leave += new System.EventHandler(this.RepetierEditor_Leave);
            this.editor.KeyUp += new System.Windows.Forms.KeyEventHandler(this.editor_KeyUp);
            this.editor.MouseClick += new System.Windows.Forms.MouseEventHandler(this.editor_MouseClick);
            this.editor.MouseDown += new System.Windows.Forms.MouseEventHandler(this.editor_MouseDown);
            this.editor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RepetierEditor_KeyPress);
            this.editor.Enter += new System.EventHandler(this.RepetierEditor_Enter);
            this.editor.MouseUp += new System.Windows.Forms.MouseEventHandler(this.editor_MouseUp);
            this.editor.SizeChanged += new System.EventHandler(this.editor_SizeChanged);
            this.editor.MouseEnter += new System.EventHandler(this.editor_MouseEnter);
            this.editor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RepetierEditor_KeyDown);
            // 
            // sliderShowMaxLayer
            // 
            this.sliderShowMaxLayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderShowMaxLayer.BackColor = System.Drawing.Color.Transparent;
            this.sliderShowMaxLayer.BarInnerColor = System.Drawing.Color.DimGray;
            this.sliderShowMaxLayer.BarOuterColor = System.Drawing.Color.LightGray;
            this.sliderShowMaxLayer.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.sliderShowMaxLayer.DrawFocusRectangle = false;
            this.sliderShowMaxLayer.ElapsedInnerColor = System.Drawing.Color.DarkGray;
            this.sliderShowMaxLayer.ElapsedOuterColor = System.Drawing.Color.LightGray;
            this.sliderShowMaxLayer.LargeChange = ((uint)(5u));
            this.sliderShowMaxLayer.Location = new System.Drawing.Point(209, 64);
            this.sliderShowMaxLayer.Name = "sliderShowMaxLayer";
            this.sliderShowMaxLayer.Size = new System.Drawing.Size(357, 22);
            this.sliderShowMaxLayer.SmallChange = ((uint)(1u));
            this.sliderShowMaxLayer.TabIndex = 6;
            this.sliderShowMaxLayer.Text = "colorSlider1";
            this.sliderShowMaxLayer.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.sliderShowMaxLayer.ValueChanged += new System.EventHandler(this.sliderShowMaxLayer_ValueChanged);
            // 
            // sliderShowFirstLayer
            // 
            this.sliderShowFirstLayer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderShowFirstLayer.BackColor = System.Drawing.Color.Transparent;
            this.sliderShowFirstLayer.BarInnerColor = System.Drawing.Color.DimGray;
            this.sliderShowFirstLayer.BarOuterColor = System.Drawing.Color.LightGray;
            this.sliderShowFirstLayer.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.sliderShowFirstLayer.DrawFocusRectangle = false;
            this.sliderShowFirstLayer.ElapsedInnerColor = System.Drawing.Color.DarkGray;
            this.sliderShowFirstLayer.ElapsedOuterColor = System.Drawing.Color.LightGray;
            this.sliderShowFirstLayer.LargeChange = ((uint)(5u));
            this.sliderShowFirstLayer.Location = new System.Drawing.Point(209, 40);
            this.sliderShowFirstLayer.Name = "sliderShowFirstLayer";
            this.sliderShowFirstLayer.Size = new System.Drawing.Size(357, 22);
            this.sliderShowFirstLayer.SmallChange = ((uint)(1u));
            this.sliderShowFirstLayer.TabIndex = 5;
            this.sliderShowFirstLayer.Text = "colorSlider1";
            this.sliderShowFirstLayer.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.sliderShowFirstLayer.ValueChanged += new System.EventHandler(this.sliderShowFirstLayer_ValueChanged);
            // 
            // RepetierEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.MinimumSize = new System.Drawing.Size(150, 140);
            this.Name = "RepetierEditor";
            this.Size = new System.Drawing.Size(615, 490);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.tabHelpview.ResumeLayout(false);
            this.tabPageVisualization.ResumeLayout(false);
            this.tabPageVisualization.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericShowMaxLayer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericShowMinLayer)).EndInit();
            this.tabPageHelp.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolNew;
        private System.Windows.Forms.ToolStripButton toolSave;
        private System.Windows.Forms.ToolStripButton toolPreview;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolRow;
        private System.Windows.Forms.ToolStripStatusLabel toolColumn;
        private System.Windows.Forms.ToolStripStatusLabel toolMode;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.HScrollBar scrollColumns;
        private DoubleBufferPanel editor;
        private System.Windows.Forms.VScrollBar scrollRows;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolCut;
        private System.Windows.Forms.ToolStripButton toolCopy;
        private System.Windows.Forms.ToolStripButton toolPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolUndo;
        private System.Windows.Forms.ToolStripButton toolRedo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.RichTextBox help;
        public System.Windows.Forms.ToolStripStatusLabel toolUpdating;
        private System.Windows.Forms.ToolStripStatusLabel toolLayer;
        private System.Windows.Forms.ToolStripStatusLabel toolExtruder;
        public System.Windows.Forms.ToolStripComboBox toolFile;
        private System.Windows.Forms.TabControl tabHelpview;
        private System.Windows.Forms.TabPage tabPageHelp;
        private System.Windows.Forms.TabPage tabPageVisualization;
        private System.Windows.Forms.NumericUpDown numericShowMaxLayer;
        private System.Windows.Forms.NumericUpDown numericShowMinLayer;
        private System.Windows.Forms.RadioButton radioShowLayerRange;
        private System.Windows.Forms.RadioButton radioShowSingleLayer;
        private System.Windows.Forms.RadioButton radioShowAll;
        private MB.Controls.ColorSlider sliderShowFirstLayer;
        private System.Windows.Forms.Label labelMaxLayer;
        private MB.Controls.ColorSlider sliderShowMaxLayer;
        public System.Windows.Forms.ToolStripStatusLabel toolPrintingTime;
        private System.Windows.Forms.Button buttonGoLastLayer;
        private System.Windows.Forms.Button buttonGoFirstLayer;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}
