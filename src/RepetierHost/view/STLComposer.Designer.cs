namespace RepetierHost.view
{
    partial class STLComposer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(STLComposer));
            this.panelControls = new System.Windows.Forms.Panel();
            this.panelCut = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelCutPosition = new System.Windows.Forms.Label();
            this.cutAzimuthSlider = new MB.Controls.ColorSlider();
            this.cutInclinationSlider = new MB.Controls.ColorSlider();
            this.cutPositionSlider = new MB.Controls.ColorSlider();
            this.checkCutFaces = new System.Windows.Forms.CheckBox();
            this.panelAnalysis = new System.Windows.Forms.Panel();
            this.groupBoxObjectAnalysis = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.textModied = new System.Windows.Forms.Label();
            this.labelModified = new System.Windows.Forms.Label();
            this.textVertices = new System.Windows.Forms.Label();
            this.labelVertices = new System.Windows.Forms.Label();
            this.textEdges = new System.Windows.Forms.Label();
            this.labelEdges = new System.Windows.Forms.Label();
            this.textFaces = new System.Windows.Forms.Label();
            this.labelFaces = new System.Windows.Forms.Label();
            this.textLoopEdges = new System.Windows.Forms.Label();
            this.labelLoopEdges = new System.Windows.Forms.Label();
            this.textShells = new System.Windows.Forms.Label();
            this.labelShells = new System.Windows.Forms.Label();
            this.textIntersectingTriangles = new System.Windows.Forms.Label();
            this.labelIntersectingTriangles = new System.Windows.Forms.Label();
            this.textManifold = new System.Windows.Forms.Label();
            this.labelManifold = new System.Windows.Forms.Label();
            this.textHighlyConnected = new System.Windows.Forms.Label();
            this.labelHighConnected = new System.Windows.Forms.Label();
            this.textNormals = new System.Windows.Forms.Label();
            this.labelNormals = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelTranslation = new System.Windows.Forms.Label();
            this.textTransX = new System.Windows.Forms.TextBox();
            this.buttonLockAspect = new System.Windows.Forms.Button();
            this.imageList16 = new System.Windows.Forms.ImageList(this.components);
            this.labelScale = new System.Windows.Forms.Label();
            this.textTransY = new System.Windows.Forms.TextBox();
            this.labelRotate = new System.Windows.Forms.Label();
            this.textScaleX = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textScaleY = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textRotX = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textRotY = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textTransZ = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textScaleZ = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textRotZ = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.listObjects = new System.Windows.Forms.ListView();
            this.columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnMesh = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnCollision = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnDelete = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolSavePlate = new System.Windows.Forms.ToolStripButton();
            this.toolAddObjects = new System.Windows.Forms.ToolStripButton();
            this.toolRemoveObjects = new System.Windows.Forms.ToolStripButton();
            this.toolCopyObjects = new System.Windows.Forms.ToolStripButton();
            this.toolAutoposition = new System.Windows.Forms.ToolStripButton();
            this.toolCenterObject = new System.Windows.Forms.ToolStripButton();
            this.toolLandObject = new System.Windows.Forms.ToolStripButton();
            this.toolSplitObject = new System.Windows.Forms.ToolStripButton();
            this.toolFixNormals = new System.Windows.Forms.ToolStripButton();
            this.toolRepair = new System.Windows.Forms.ToolStripButton();
            this.toolStripInfo = new System.Windows.Forms.ToolStripButton();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.openFileSTL = new System.Windows.Forms.OpenFileDialog();
            this.saveSTL = new System.Windows.Forms.SaveFileDialog();
            this.panelControls.SuspendLayout();
            this.panelCut.SuspendLayout();
            this.panelAnalysis.SuspendLayout();
            this.groupBoxObjectAnalysis.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControls
            // 
            this.panelControls.AutoScroll = true;
            this.panelControls.Controls.Add(this.panelCut);
            this.panelControls.Controls.Add(this.panelAnalysis);
            this.panelControls.Controls.Add(this.panel1);
            this.panelControls.Controls.Add(this.listObjects);
            this.panelControls.Controls.Add(this.toolStrip1);
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControls.Location = new System.Drawing.Point(0, 0);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(360, 624);
            this.panelControls.TabIndex = 0;
            // 
            // panelCut
            // 
            this.panelCut.Controls.Add(this.label5);
            this.panelCut.Controls.Add(this.label1);
            this.panelCut.Controls.Add(this.labelCutPosition);
            this.panelCut.Controls.Add(this.cutAzimuthSlider);
            this.panelCut.Controls.Add(this.cutInclinationSlider);
            this.panelCut.Controls.Add(this.cutPositionSlider);
            this.panelCut.Controls.Add(this.checkCutFaces);
            this.panelCut.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelCut.Location = new System.Drawing.Point(0, 481);
            this.panelCut.Name = "panelCut";
            this.panelCut.Size = new System.Drawing.Size(360, 115);
            this.panelCut.TabIndex = 24;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 87);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Azimuth";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Inclination";
            // 
            // labelCutPosition
            // 
            this.labelCutPosition.AutoSize = true;
            this.labelCutPosition.Location = new System.Drawing.Point(12, 31);
            this.labelCutPosition.Name = "labelCutPosition";
            this.labelCutPosition.Size = new System.Drawing.Size(44, 13);
            this.labelCutPosition.TabIndex = 2;
            this.labelCutPosition.Text = "Position";
            // 
            // cutAzimuthSlider
            // 
            this.cutAzimuthSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cutAzimuthSlider.BackColor = System.Drawing.Color.Transparent;
            this.cutAzimuthSlider.BarInnerColor = System.Drawing.Color.LightGray;
            this.cutAzimuthSlider.BarOuterColor = System.Drawing.Color.Gray;
            this.cutAzimuthSlider.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.cutAzimuthSlider.ElapsedInnerColor = System.Drawing.Color.LightGray;
            this.cutAzimuthSlider.ElapsedOuterColor = System.Drawing.Color.Gray;
            this.cutAzimuthSlider.LargeChange = ((uint)(5u));
            this.cutAzimuthSlider.Location = new System.Drawing.Point(83, 85);
            this.cutAzimuthSlider.Maximum = 3600;
            this.cutAzimuthSlider.Name = "cutAzimuthSlider";
            this.cutAzimuthSlider.Size = new System.Drawing.Size(271, 20);
            this.cutAzimuthSlider.SmallChange = ((uint)(1u));
            this.cutAzimuthSlider.TabIndex = 1;
            this.cutAzimuthSlider.Text = "colorSlider1";
            this.cutAzimuthSlider.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.cutAzimuthSlider.ThumbSize = 10;
            this.cutAzimuthSlider.Value = 0;
            this.cutAzimuthSlider.ValueChanged += new System.EventHandler(this.cutPositionSlider_ValueChanged);
            // 
            // cutInclinationSlider
            // 
            this.cutInclinationSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cutInclinationSlider.BackColor = System.Drawing.Color.Transparent;
            this.cutInclinationSlider.BarInnerColor = System.Drawing.Color.LightGray;
            this.cutInclinationSlider.BarOuterColor = System.Drawing.Color.Gray;
            this.cutInclinationSlider.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.cutInclinationSlider.ElapsedInnerColor = System.Drawing.Color.LightGray;
            this.cutInclinationSlider.ElapsedOuterColor = System.Drawing.Color.Gray;
            this.cutInclinationSlider.LargeChange = ((uint)(5u));
            this.cutInclinationSlider.Location = new System.Drawing.Point(83, 57);
            this.cutInclinationSlider.Maximum = 1800;
            this.cutInclinationSlider.Name = "cutInclinationSlider";
            this.cutInclinationSlider.Size = new System.Drawing.Size(271, 20);
            this.cutInclinationSlider.SmallChange = ((uint)(1u));
            this.cutInclinationSlider.TabIndex = 1;
            this.cutInclinationSlider.Text = "colorSlider1";
            this.cutInclinationSlider.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.cutInclinationSlider.ThumbSize = 10;
            this.cutInclinationSlider.Value = 0;
            this.cutInclinationSlider.ValueChanged += new System.EventHandler(this.cutPositionSlider_ValueChanged);
            // 
            // cutPositionSlider
            // 
            this.cutPositionSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cutPositionSlider.BackColor = System.Drawing.Color.Transparent;
            this.cutPositionSlider.BarInnerColor = System.Drawing.Color.LightGray;
            this.cutPositionSlider.BarOuterColor = System.Drawing.Color.Gray;
            this.cutPositionSlider.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.cutPositionSlider.ElapsedInnerColor = System.Drawing.Color.LightGray;
            this.cutPositionSlider.ElapsedOuterColor = System.Drawing.Color.Gray;
            this.cutPositionSlider.LargeChange = ((uint)(5u));
            this.cutPositionSlider.Location = new System.Drawing.Point(83, 29);
            this.cutPositionSlider.Maximum = 1000;
            this.cutPositionSlider.Name = "cutPositionSlider";
            this.cutPositionSlider.Size = new System.Drawing.Size(271, 20);
            this.cutPositionSlider.SmallChange = ((uint)(1u));
            this.cutPositionSlider.TabIndex = 1;
            this.cutPositionSlider.Text = "colorSlider1";
            this.cutPositionSlider.ThumbRoundRectSize = new System.Drawing.Size(8, 8);
            this.cutPositionSlider.ThumbSize = 10;
            this.cutPositionSlider.Value = 500;
            this.cutPositionSlider.ValueChanged += new System.EventHandler(this.cutPositionSlider_ValueChanged);
            // 
            // checkCutFaces
            // 
            this.checkCutFaces.AutoSize = true;
            this.checkCutFaces.Location = new System.Drawing.Point(12, 6);
            this.checkCutFaces.Name = "checkCutFaces";
            this.checkCutFaces.Size = new System.Drawing.Size(81, 17);
            this.checkCutFaces.TabIndex = 0;
            this.checkCutFaces.Text = "Cut Objects";
            this.checkCutFaces.UseVisualStyleBackColor = true;
            this.checkCutFaces.CheckedChanged += new System.EventHandler(this.checkCutFaces_CheckedChanged);
            // 
            // panelAnalysis
            // 
            this.panelAnalysis.Controls.Add(this.groupBoxObjectAnalysis);
            this.panelAnalysis.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelAnalysis.Location = new System.Drawing.Point(0, 260);
            this.panelAnalysis.Name = "panelAnalysis";
            this.panelAnalysis.Size = new System.Drawing.Size(360, 221);
            this.panelAnalysis.TabIndex = 23;
            // 
            // groupBoxObjectAnalysis
            // 
            this.groupBoxObjectAnalysis.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxObjectAnalysis.BackColor = System.Drawing.Color.White;
            this.groupBoxObjectAnalysis.Controls.Add(this.tableLayoutPanel1);
            this.groupBoxObjectAnalysis.Location = new System.Drawing.Point(3, 6);
            this.groupBoxObjectAnalysis.Name = "groupBoxObjectAnalysis";
            this.groupBoxObjectAnalysis.Size = new System.Drawing.Size(354, 212);
            this.groupBoxObjectAnalysis.TabIndex = 0;
            this.groupBoxObjectAnalysis.TabStop = false;
            this.groupBoxObjectAnalysis.Text = "Object Analysis";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.textModied, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelModified, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.textVertices, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.labelVertices, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.textEdges, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.labelEdges, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.textFaces, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.labelFaces, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.textLoopEdges, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelLoopEdges, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.textShells, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.labelShells, 1, 9);
            this.tableLayoutPanel1.Controls.Add(this.textIntersectingTriangles, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelIntersectingTriangles, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.textManifold, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelManifold, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.textHighlyConnected, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.labelHighConnected, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.textNormals, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelNormals, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 10;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(348, 193);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // textModied
            // 
            this.textModied.AutoSize = true;
            this.textModied.Location = new System.Drawing.Point(3, 0);
            this.textModied.Name = "textModied";
            this.textModied.Size = new System.Drawing.Size(50, 13);
            this.textModied.TabIndex = 18;
            this.textModied.Text = "Modified:";
            this.textModied.Click += new System.EventHandler(this.labelModified_Click);
            // 
            // labelModified
            // 
            this.labelModified.AutoSize = true;
            this.labelModified.Location = new System.Drawing.Point(264, 0);
            this.labelModified.Name = "labelModified";
            this.labelModified.Size = new System.Drawing.Size(35, 13);
            this.labelModified.TabIndex = 19;
            this.labelModified.Text = "label5";
            this.labelModified.Click += new System.EventHandler(this.labelModified_Click);
            // 
            // textVertices
            // 
            this.textVertices.AutoSize = true;
            this.textVertices.Location = new System.Drawing.Point(3, 108);
            this.textVertices.Name = "textVertices";
            this.textVertices.Size = new System.Drawing.Size(48, 13);
            this.textVertices.TabIndex = 0;
            this.textVertices.Text = "Vertices:";
            // 
            // labelVertices
            // 
            this.labelVertices.AutoSize = true;
            this.labelVertices.Location = new System.Drawing.Point(264, 108);
            this.labelVertices.Name = "labelVertices";
            this.labelVertices.Size = new System.Drawing.Size(35, 13);
            this.labelVertices.TabIndex = 1;
            this.labelVertices.Text = "label2";
            // 
            // textEdges
            // 
            this.textEdges.AutoSize = true;
            this.textEdges.Location = new System.Drawing.Point(3, 126);
            this.textEdges.Name = "textEdges";
            this.textEdges.Size = new System.Drawing.Size(40, 13);
            this.textEdges.TabIndex = 2;
            this.textEdges.Text = "Edges:";
            // 
            // labelEdges
            // 
            this.labelEdges.AutoSize = true;
            this.labelEdges.Location = new System.Drawing.Point(264, 126);
            this.labelEdges.Name = "labelEdges";
            this.labelEdges.Size = new System.Drawing.Size(35, 13);
            this.labelEdges.TabIndex = 3;
            this.labelEdges.Text = "label4";
            // 
            // textFaces
            // 
            this.textFaces.AutoSize = true;
            this.textFaces.Location = new System.Drawing.Point(3, 144);
            this.textFaces.Name = "textFaces";
            this.textFaces.Size = new System.Drawing.Size(39, 13);
            this.textFaces.TabIndex = 4;
            this.textFaces.Text = "Faces:";
            // 
            // labelFaces
            // 
            this.labelFaces.AutoSize = true;
            this.labelFaces.Location = new System.Drawing.Point(264, 144);
            this.labelFaces.Name = "labelFaces";
            this.labelFaces.Size = new System.Drawing.Size(35, 13);
            this.labelFaces.TabIndex = 5;
            this.labelFaces.Text = "label6";
            // 
            // textLoopEdges
            // 
            this.textLoopEdges.AutoSize = true;
            this.textLoopEdges.Location = new System.Drawing.Point(3, 72);
            this.textLoopEdges.Name = "textLoopEdges";
            this.textLoopEdges.Size = new System.Drawing.Size(66, 13);
            this.textLoopEdges.TabIndex = 6;
            this.textLoopEdges.Text = "Loop edges:";
            // 
            // labelLoopEdges
            // 
            this.labelLoopEdges.AutoSize = true;
            this.labelLoopEdges.Location = new System.Drawing.Point(264, 72);
            this.labelLoopEdges.Name = "labelLoopEdges";
            this.labelLoopEdges.Size = new System.Drawing.Size(35, 13);
            this.labelLoopEdges.TabIndex = 7;
            this.labelLoopEdges.Text = "label8";
            // 
            // textShells
            // 
            this.textShells.AutoSize = true;
            this.textShells.Location = new System.Drawing.Point(3, 162);
            this.textShells.Name = "textShells";
            this.textShells.Size = new System.Drawing.Size(38, 13);
            this.textShells.TabIndex = 8;
            this.textShells.Text = "Shells:";
            // 
            // labelShells
            // 
            this.labelShells.AutoSize = true;
            this.labelShells.Location = new System.Drawing.Point(264, 162);
            this.labelShells.Name = "labelShells";
            this.labelShells.Size = new System.Drawing.Size(41, 13);
            this.labelShells.TabIndex = 9;
            this.labelShells.Text = "label10";
            // 
            // textIntersectingTriangles
            // 
            this.textIntersectingTriangles.AutoSize = true;
            this.textIntersectingTriangles.Location = new System.Drawing.Point(3, 36);
            this.textIntersectingTriangles.Name = "textIntersectingTriangles";
            this.textIntersectingTriangles.Size = new System.Drawing.Size(107, 13);
            this.textIntersectingTriangles.TabIndex = 10;
            this.textIntersectingTriangles.Text = "Intersecting triangles:";
            // 
            // labelIntersectingTriangles
            // 
            this.labelIntersectingTriangles.AutoSize = true;
            this.labelIntersectingTriangles.Location = new System.Drawing.Point(264, 36);
            this.labelIntersectingTriangles.Name = "labelIntersectingTriangles";
            this.labelIntersectingTriangles.Size = new System.Drawing.Size(13, 13);
            this.labelIntersectingTriangles.TabIndex = 11;
            this.labelIntersectingTriangles.Text = "0";
            // 
            // textManifold
            // 
            this.textManifold.AutoSize = true;
            this.textManifold.Location = new System.Drawing.Point(3, 18);
            this.textManifold.Name = "textManifold";
            this.textManifold.Size = new System.Drawing.Size(47, 13);
            this.textManifold.TabIndex = 12;
            this.textManifold.Text = "Manifold";
            // 
            // labelManifold
            // 
            this.labelManifold.AutoSize = true;
            this.labelManifold.Location = new System.Drawing.Point(264, 18);
            this.labelManifold.Name = "labelManifold";
            this.labelManifold.Size = new System.Drawing.Size(35, 13);
            this.labelManifold.TabIndex = 13;
            this.labelManifold.Text = "label6";
            // 
            // textHighlyConnected
            // 
            this.textHighlyConnected.AutoSize = true;
            this.textHighlyConnected.Location = new System.Drawing.Point(3, 90);
            this.textHighlyConnected.Name = "textHighlyConnected";
            this.textHighlyConnected.Size = new System.Drawing.Size(125, 13);
            this.textHighlyConnected.TabIndex = 14;
            this.textHighlyConnected.Text = "Highly connected edges:";
            // 
            // labelHighConnected
            // 
            this.labelHighConnected.AutoSize = true;
            this.labelHighConnected.Location = new System.Drawing.Point(264, 90);
            this.labelHighConnected.Name = "labelHighConnected";
            this.labelHighConnected.Size = new System.Drawing.Size(13, 13);
            this.labelHighConnected.TabIndex = 15;
            this.labelHighConnected.Text = "0";
            // 
            // textNormals
            // 
            this.textNormals.AutoSize = true;
            this.textNormals.Location = new System.Drawing.Point(3, 54);
            this.textNormals.Name = "textNormals";
            this.textNormals.Size = new System.Drawing.Size(45, 13);
            this.textNormals.TabIndex = 16;
            this.textNormals.Text = "Normals";
            // 
            // labelNormals
            // 
            this.labelNormals.AutoSize = true;
            this.labelNormals.Location = new System.Drawing.Point(264, 54);
            this.labelNormals.Name = "labelNormals";
            this.labelNormals.Size = new System.Drawing.Size(45, 13);
            this.labelNormals.TabIndex = 17;
            this.labelNormals.Text = "oriented";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelTranslation);
            this.panel1.Controls.Add(this.textTransX);
            this.panel1.Controls.Add(this.buttonLockAspect);
            this.panel1.Controls.Add(this.labelScale);
            this.panel1.Controls.Add(this.textTransY);
            this.panel1.Controls.Add(this.labelRotate);
            this.panel1.Controls.Add(this.textScaleX);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.textScaleY);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.textRotX);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.textRotY);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.textTransZ);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.textScaleZ);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.textRotZ);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 173);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(360, 87);
            this.panel1.TabIndex = 22;
            // 
            // labelTranslation
            // 
            this.labelTranslation.AutoSize = true;
            this.labelTranslation.Location = new System.Drawing.Point(12, 8);
            this.labelTranslation.Name = "labelTranslation";
            this.labelTranslation.Size = new System.Drawing.Size(62, 13);
            this.labelTranslation.TabIndex = 2;
            this.labelTranslation.Text = "Translation:";
            // 
            // textTransX
            // 
            this.textTransX.Location = new System.Drawing.Point(102, 5);
            this.textTransX.Name = "textTransX";
            this.textTransX.Size = new System.Drawing.Size(49, 20);
            this.textTransX.TabIndex = 2;
            this.textTransX.TextChanged += new System.EventHandler(this.textTransX_TextChanged);
            this.textTransX.Validating += new System.ComponentModel.CancelEventHandler(this.float_Validating);
            // 
            // buttonLockAspect
            // 
            this.buttonLockAspect.FlatAppearance.BorderSize = 0;
            this.buttonLockAspect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLockAspect.ImageIndex = 0;
            this.buttonLockAspect.ImageList = this.imageList16;
            this.buttonLockAspect.Location = new System.Drawing.Point(295, 29);
            this.buttonLockAspect.Name = "buttonLockAspect";
            this.buttonLockAspect.Size = new System.Drawing.Size(43, 23);
            this.buttonLockAspect.TabIndex = 20;
            this.buttonLockAspect.UseVisualStyleBackColor = true;
            this.buttonLockAspect.Click += new System.EventHandler(this.buttonLockAspect_Click);
            // 
            // imageList16
            // 
            this.imageList16.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList16.ImageStream")));
            this.imageList16.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList16.Images.SetKeyName(0, "unlock16.png");
            this.imageList16.Images.SetKeyName(1, "lock16.png");
            this.imageList16.Images.SetKeyName(2, "ok16.png");
            this.imageList16.Images.SetKeyName(3, "bad16.png");
            this.imageList16.Images.SetKeyName(4, "trash16.png");
            // 
            // labelScale
            // 
            this.labelScale.AutoSize = true;
            this.labelScale.Location = new System.Drawing.Point(12, 34);
            this.labelScale.Name = "labelScale";
            this.labelScale.Size = new System.Drawing.Size(37, 13);
            this.labelScale.TabIndex = 2;
            this.labelScale.Text = "Scale:";
            // 
            // textTransY
            // 
            this.textTransY.Location = new System.Drawing.Point(171, 5);
            this.textTransY.Name = "textTransY";
            this.textTransY.Size = new System.Drawing.Size(49, 20);
            this.textTransY.TabIndex = 3;
            this.textTransY.TextChanged += new System.EventHandler(this.textTransY_TextChanged);
            this.textTransY.Validating += new System.ComponentModel.CancelEventHandler(this.float_Validating);
            // 
            // labelRotate
            // 
            this.labelRotate.AutoSize = true;
            this.labelRotate.Location = new System.Drawing.Point(13, 60);
            this.labelRotate.Name = "labelRotate";
            this.labelRotate.Size = new System.Drawing.Size(42, 13);
            this.labelRotate.TabIndex = 2;
            this.labelRotate.Text = "Rotate:";
            // 
            // textScaleX
            // 
            this.textScaleX.Location = new System.Drawing.Point(102, 31);
            this.textScaleX.Name = "textScaleX";
            this.textScaleX.Size = new System.Drawing.Size(49, 20);
            this.textScaleX.TabIndex = 5;
            this.textScaleX.TextChanged += new System.EventHandler(this.textScaleX_TextChanged);
            this.textScaleX.Validating += new System.ComponentModel.CancelEventHandler(this.float_Validating);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(222, 59);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(14, 13);
            this.label12.TabIndex = 4;
            this.label12.Text = "Z";
            // 
            // textScaleY
            // 
            this.textScaleY.Location = new System.Drawing.Point(171, 31);
            this.textScaleY.Name = "textScaleY";
            this.textScaleY.Size = new System.Drawing.Size(49, 20);
            this.textScaleY.TabIndex = 6;
            this.textScaleY.TextChanged += new System.EventHandler(this.textScaleY_TextChanged);
            this.textScaleY.Validating += new System.ComponentModel.CancelEventHandler(this.float_Validating);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(222, 33);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(14, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Z";
            // 
            // textRotX
            // 
            this.textRotX.Location = new System.Drawing.Point(102, 57);
            this.textRotX.Name = "textRotX";
            this.textRotX.Size = new System.Drawing.Size(49, 20);
            this.textRotX.TabIndex = 9;
            this.textRotX.TextChanged += new System.EventHandler(this.textRotX_TextChanged);
            this.textRotX.Validating += new System.ComponentModel.CancelEventHandler(this.float_Validating);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(222, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Z";
            // 
            // textRotY
            // 
            this.textRotY.Location = new System.Drawing.Point(171, 57);
            this.textRotY.Name = "textRotY";
            this.textRotY.Size = new System.Drawing.Size(49, 20);
            this.textRotY.TabIndex = 10;
            this.textRotY.TextChanged += new System.EventHandler(this.textRotY_TextChanged);
            this.textRotY.Validating += new System.ComponentModel.CancelEventHandler(this.float_Validating);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(154, 59);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(14, 13);
            this.label11.TabIndex = 4;
            this.label11.Text = "Y";
            // 
            // textTransZ
            // 
            this.textTransZ.Location = new System.Drawing.Point(240, 5);
            this.textTransZ.Name = "textTransZ";
            this.textTransZ.Size = new System.Drawing.Size(49, 20);
            this.textTransZ.TabIndex = 4;
            this.textTransZ.TextChanged += new System.EventHandler(this.textTransZ_TextChanged);
            this.textTransZ.Validating += new System.ComponentModel.CancelEventHandler(this.float_Validating);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(154, 33);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(14, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Y";
            // 
            // textScaleZ
            // 
            this.textScaleZ.Location = new System.Drawing.Point(240, 31);
            this.textScaleZ.Name = "textScaleZ";
            this.textScaleZ.Size = new System.Drawing.Size(49, 20);
            this.textScaleZ.TabIndex = 7;
            this.textScaleZ.TextChanged += new System.EventHandler(this.textScaleZ_TextChanged);
            this.textScaleZ.Validating += new System.ComponentModel.CancelEventHandler(this.float_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(154, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Y";
            // 
            // textRotZ
            // 
            this.textRotZ.Location = new System.Drawing.Point(240, 57);
            this.textRotZ.Name = "textRotZ";
            this.textRotZ.Size = new System.Drawing.Size(49, 20);
            this.textRotZ.TabIndex = 11;
            this.textRotZ.TextChanged += new System.EventHandler(this.textRotZ_TextChanged);
            this.textRotZ.Validating += new System.ComponentModel.CancelEventHandler(this.float_Validating);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(84, 59);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(14, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "X";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(84, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "X";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(84, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(14, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "X";
            // 
            // listObjects
            // 
            this.listObjects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnMesh,
            this.columnCollision,
            this.columnDelete});
            this.listObjects.Dock = System.Windows.Forms.DockStyle.Top;
            this.listObjects.FullRowSelect = true;
            this.listObjects.GridLines = true;
            this.listObjects.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listObjects.HideSelection = false;
            this.listObjects.Location = new System.Drawing.Point(0, 25);
            this.listObjects.Name = "listObjects";
            this.listObjects.OwnerDraw = true;
            this.listObjects.ShowGroups = false;
            this.listObjects.Size = new System.Drawing.Size(360, 148);
            this.listObjects.SmallImageList = this.imageList16;
            this.listObjects.TabIndex = 21;
            this.listObjects.UseCompatibleStateImageBehavior = false;
            this.listObjects.View = System.Windows.Forms.View.Details;
            this.listObjects.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listObjects_DrawColumnHeader);
            this.listObjects.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listObjects_DrawSubItem);
            this.listObjects.SelectedIndexChanged += new System.EventHandler(this.listSTLObjects_SelectedIndexChanged);
            this.listObjects.ClientSizeChanged += new System.EventHandler(this.listObjects_ClientSizeChanged);
            this.listObjects.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listSTLObjects_KeyDown);
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            // 
            // columnMesh
            // 
            this.columnMesh.Text = "Mesh";
            this.columnMesh.Width = 50;
            // 
            // columnCollision
            // 
            this.columnCollision.Text = "Collision";
            this.columnCollision.Width = 50;
            // 
            // columnDelete
            // 
            this.columnDelete.Text = "";
            this.columnDelete.Width = 26;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolSavePlate,
            this.toolAddObjects,
            this.toolRemoveObjects,
            this.toolCopyObjects,
            this.toolAutoposition,
            this.toolCenterObject,
            this.toolLandObject,
            this.toolSplitObject,
            this.toolFixNormals,
            this.toolRepair,
            this.toolStripInfo});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(360, 25);
            this.toolStrip1.TabIndex = 19;
            this.toolStrip1.Text = "toolStripObjectEditor";
            // 
            // toolSavePlate
            // 
            this.toolSavePlate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolSavePlate.Image = ((System.Drawing.Image)(resources.GetObject("toolSavePlate.Image")));
            this.toolSavePlate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolSavePlate.Name = "toolSavePlate";
            this.toolSavePlate.Size = new System.Drawing.Size(23, 22);
            this.toolSavePlate.Text = "toolStripSavePlate";
            this.toolSavePlate.ToolTipText = "Save objects";
            this.toolSavePlate.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // toolAddObjects
            // 
            this.toolAddObjects.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolAddObjects.Image = ((System.Drawing.Image)(resources.GetObject("toolAddObjects.Image")));
            this.toolAddObjects.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolAddObjects.Name = "toolAddObjects";
            this.toolAddObjects.Size = new System.Drawing.Size(23, 22);
            this.toolAddObjects.Text = "toolStripButton2";
            this.toolAddObjects.ToolTipText = "Add objects";
            this.toolAddObjects.Click += new System.EventHandler(this.buttonAddSTL_Click);
            // 
            // toolRemoveObjects
            // 
            this.toolRemoveObjects.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolRemoveObjects.Image = ((System.Drawing.Image)(resources.GetObject("toolRemoveObjects.Image")));
            this.toolRemoveObjects.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolRemoveObjects.Name = "toolRemoveObjects";
            this.toolRemoveObjects.Size = new System.Drawing.Size(23, 22);
            this.toolRemoveObjects.ToolTipText = "Remove objects";
            this.toolRemoveObjects.Click += new System.EventHandler(this.buttonRemoveSTL_Click);
            // 
            // toolCopyObjects
            // 
            this.toolCopyObjects.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolCopyObjects.Image = ((System.Drawing.Image)(resources.GetObject("toolCopyObjects.Image")));
            this.toolCopyObjects.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolCopyObjects.Name = "toolCopyObjects";
            this.toolCopyObjects.Size = new System.Drawing.Size(23, 22);
            this.toolCopyObjects.Text = "toolStripButton4";
            this.toolCopyObjects.ToolTipText = "Copy objects";
            this.toolCopyObjects.Click += new System.EventHandler(this.buttonCopyObjects_Click);
            // 
            // toolAutoposition
            // 
            this.toolAutoposition.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolAutoposition.Image = ((System.Drawing.Image)(resources.GetObject("toolAutoposition.Image")));
            this.toolAutoposition.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolAutoposition.Name = "toolAutoposition";
            this.toolAutoposition.Size = new System.Drawing.Size(23, 22);
            this.toolAutoposition.Text = "toolStripButton5";
            this.toolAutoposition.ToolTipText = "Autoposition";
            this.toolAutoposition.Click += new System.EventHandler(this.buttonAutoplace_Click);
            // 
            // toolCenterObject
            // 
            this.toolCenterObject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolCenterObject.Image = ((System.Drawing.Image)(resources.GetObject("toolCenterObject.Image")));
            this.toolCenterObject.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolCenterObject.Name = "toolCenterObject";
            this.toolCenterObject.Size = new System.Drawing.Size(23, 22);
            this.toolCenterObject.Text = "toolStripButton6";
            this.toolCenterObject.ToolTipText = "Center object";
            this.toolCenterObject.Click += new System.EventHandler(this.buttonCenter_Click);
            // 
            // toolLandObject
            // 
            this.toolLandObject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolLandObject.Image = ((System.Drawing.Image)(resources.GetObject("toolLandObject.Image")));
            this.toolLandObject.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolLandObject.Name = "toolLandObject";
            this.toolLandObject.Size = new System.Drawing.Size(23, 22);
            this.toolLandObject.Text = "toolStripButton7";
            this.toolLandObject.ToolTipText = "Land object";
            this.toolLandObject.Click += new System.EventHandler(this.buttonLand_Click);
            // 
            // toolSplitObject
            // 
            this.toolSplitObject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolSplitObject.Image = ((System.Drawing.Image)(resources.GetObject("toolSplitObject.Image")));
            this.toolSplitObject.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolSplitObject.Name = "toolSplitObject";
            this.toolSplitObject.Size = new System.Drawing.Size(23, 22);
            this.toolSplitObject.Text = "toolStripButton8";
            this.toolSplitObject.ToolTipText = "Split object";
            this.toolSplitObject.Click += new System.EventHandler(this.toolSplitObject_Click);
            // 
            // toolFixNormals
            // 
            this.toolFixNormals.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolFixNormals.Image = ((System.Drawing.Image)(resources.GetObject("toolFixNormals.Image")));
            this.toolFixNormals.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolFixNormals.Name = "toolFixNormals";
            this.toolFixNormals.Size = new System.Drawing.Size(23, 22);
            this.toolFixNormals.Text = "toolFixNormals";
            this.toolFixNormals.ToolTipText = "Fix normals";
            this.toolFixNormals.Click += new System.EventHandler(this.toolFixNormals_Click);
            // 
            // toolRepair
            // 
            this.toolRepair.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolRepair.Image = ((System.Drawing.Image)(resources.GetObject("toolRepair.Image")));
            this.toolRepair.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolRepair.Name = "toolRepair";
            this.toolRepair.Size = new System.Drawing.Size(23, 22);
            this.toolRepair.Text = "toolRepair";
            this.toolRepair.ToolTipText = "Repair object";
            this.toolRepair.Click += new System.EventHandler(this.buttonRepair_Click);
            // 
            // toolStripInfo
            // 
            this.toolStripInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripInfo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripInfo.Image")));
            this.toolStripInfo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripInfo.Name = "toolStripInfo";
            this.toolStripInfo.Size = new System.Drawing.Size(23, 22);
            this.toolStripInfo.Text = "Info";
            this.toolStripInfo.Click += new System.EventHandler(this.toolStripInfo_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // openFileSTL
            // 
            this.openFileSTL.DefaultExt = "stl";
            this.openFileSTL.Filter = "3D-Files|*.stl;*.STL;*.obj;*.OBJ|STL-Files|*.stl;*.STL|OBJ-Files|*.obj;*.OBJ|All " +
    "files|*.*";
            this.openFileSTL.Multiselect = true;
            this.openFileSTL.Title = "Add STL file";
            // 
            // saveSTL
            // 
            this.saveSTL.DefaultExt = "stl";
            this.saveSTL.Filter = "STL-Files|*.stl;*.STL|OBJ-Files|*.obj;*.OBJ";
            this.saveSTL.Title = "Save composition";
            // 
            // STLComposer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelControls);
            this.Name = "STLComposer";
            this.Size = new System.Drawing.Size(360, 624);
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            this.panelCut.ResumeLayout(false);
            this.panelCut.PerformLayout();
            this.panelAnalysis.ResumeLayout(false);
            this.groupBoxObjectAnalysis.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textTransZ;
        private System.Windows.Forms.TextBox textTransY;
        private System.Windows.Forms.TextBox textTransX;
        private System.Windows.Forms.Label labelTranslation;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textRotZ;
        private System.Windows.Forms.TextBox textScaleZ;
        private System.Windows.Forms.TextBox textRotY;
        private System.Windows.Forms.TextBox textRotX;
        private System.Windows.Forms.TextBox textScaleY;
        private System.Windows.Forms.TextBox textScaleX;
        private System.Windows.Forms.Label labelRotate;
        private System.Windows.Forms.Label labelScale;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.OpenFileDialog openFileSTL;
        private System.Windows.Forms.SaveFileDialog saveSTL;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolSavePlate;
        private System.Windows.Forms.ToolStripButton toolAddObjects;
        private System.Windows.Forms.ToolStripButton toolRemoveObjects;
        private System.Windows.Forms.ToolStripButton toolCopyObjects;
        private System.Windows.Forms.ToolStripButton toolAutoposition;
        private System.Windows.Forms.ToolStripButton toolCenterObject;
        private System.Windows.Forms.ToolStripButton toolLandObject;
        private System.Windows.Forms.ToolStripButton toolSplitObject;
        private System.Windows.Forms.ImageList imageList16;
        private System.Windows.Forms.Button buttonLockAspect;
        private System.Windows.Forms.ListView listObjects;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnMesh;
        private System.Windows.Forms.ColumnHeader columnCollision;
        private System.Windows.Forms.ColumnHeader columnDelete;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripButton toolRepair;
        private System.Windows.Forms.Panel panelAnalysis;
        private System.Windows.Forms.GroupBox groupBoxObjectAnalysis;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label textVertices;
        private System.Windows.Forms.Label labelVertices;
        private System.Windows.Forms.Label textEdges;
        private System.Windows.Forms.Label labelEdges;
        private System.Windows.Forms.Label textFaces;
        private System.Windows.Forms.Label labelFaces;
        private System.Windows.Forms.Label textLoopEdges;
        private System.Windows.Forms.Label labelLoopEdges;
        private System.Windows.Forms.Label textShells;
        private System.Windows.Forms.Label labelShells;
        private System.Windows.Forms.Label textIntersectingTriangles;
        private System.Windows.Forms.Label labelIntersectingTriangles;
        private System.Windows.Forms.Label textManifold;
        private System.Windows.Forms.Label labelManifold;
        private System.Windows.Forms.Label textHighlyConnected;
        private System.Windows.Forms.Label labelHighConnected;
        private System.Windows.Forms.Label textNormals;
        private System.Windows.Forms.Label labelNormals;
        private System.Windows.Forms.Label textModied;
        private System.Windows.Forms.Label labelModified;
        private System.Windows.Forms.ToolStripButton toolFixNormals;
        private System.Windows.Forms.Panel panelCut;
        private System.Windows.Forms.Label labelCutPosition;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        public MB.Controls.ColorSlider cutPositionSlider;
        public System.Windows.Forms.CheckBox checkCutFaces;
        public MB.Controls.ColorSlider cutAzimuthSlider;
        public MB.Controls.ColorSlider cutInclinationSlider;
        private System.Windows.Forms.ToolStripButton toolStripInfo;
        public System.Windows.Forms.Panel panelControls;
    }
}
