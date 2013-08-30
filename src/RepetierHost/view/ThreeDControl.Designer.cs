namespace RepetierHost.view
{
    partial class ThreeDControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThreeDControl));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolRotate = new System.Windows.Forms.ToolStripButton();
            this.toolMoveViewpoint = new System.Windows.Forms.ToolStripButton();
            this.toolMoveObject = new System.Windows.Forms.ToolStripButton();
            this.toolZoom = new System.Windows.Forms.ToolStripButton();
            this.toolIsometric = new System.Windows.Forms.ToolStripButton();
            this.toolResetView = new System.Windows.Forms.ToolStripButton();
            this.toolTopView = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolParallelProjection = new System.Windows.Forms.ToolStripButton();
            this.toolStripClear = new System.Windows.Forms.ToolStripButton();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.gl = new RepetierHost.view.utils.RHOpenGL();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Left;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolRotate,
            this.toolMoveViewpoint,
            this.toolMoveObject,
            this.toolZoom,
            this.toolIsometric,
            this.toolResetView,
            this.toolTopView,
            this.toolStripSeparator1,
            this.toolParallelProjection,
            this.toolStripClear});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(37, 504);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolRotate
            // 
            this.toolRotate.Checked = true;
            this.toolRotate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolRotate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolRotate.Image = ((System.Drawing.Image)(resources.GetObject("toolRotate.Image")));
            this.toolRotate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolRotate.Name = "toolRotate";
            this.toolRotate.Size = new System.Drawing.Size(34, 36);
            this.toolRotate.Text = "Rotate";
            this.toolRotate.Click += new System.EventHandler(this.toolRotate_Click);
            // 
            // toolMoveViewpoint
            // 
            this.toolMoveViewpoint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolMoveViewpoint.Image = ((System.Drawing.Image)(resources.GetObject("toolMoveViewpoint.Image")));
            this.toolMoveViewpoint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolMoveViewpoint.Name = "toolMoveViewpoint";
            this.toolMoveViewpoint.Size = new System.Drawing.Size(34, 36);
            this.toolMoveViewpoint.Text = "Move viewpoint";
            this.toolMoveViewpoint.Click += new System.EventHandler(this.toolMoveViewpoint_Click);
            // 
            // toolMoveObject
            // 
            this.toolMoveObject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolMoveObject.Image = ((System.Drawing.Image)(resources.GetObject("toolMoveObject.Image")));
            this.toolMoveObject.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolMoveObject.Name = "toolMoveObject";
            this.toolMoveObject.Size = new System.Drawing.Size(34, 36);
            this.toolMoveObject.Text = "Move object";
            this.toolMoveObject.Click += new System.EventHandler(this.toolMoveObject_Click);
            // 
            // toolZoom
            // 
            this.toolZoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolZoom.Image = ((System.Drawing.Image)(resources.GetObject("toolZoom.Image")));
            this.toolZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolZoom.Name = "toolZoom";
            this.toolZoom.Size = new System.Drawing.Size(34, 36);
            this.toolZoom.Text = "Zoom";
            this.toolZoom.Click += new System.EventHandler(this.toolZoom_Click);
            // 
            // toolIsometric
            // 
            this.toolIsometric.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolIsometric.Image = ((System.Drawing.Image)(resources.GetObject("toolIsometric.Image")));
            this.toolIsometric.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolIsometric.Name = "toolIsometric";
            this.toolIsometric.Size = new System.Drawing.Size(34, 36);
            this.toolIsometric.Text = "toolStripButton1";
            this.toolIsometric.Click += new System.EventHandler(this.toolIsometric_Click);
            // 
            // toolResetView
            // 
            this.toolResetView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolResetView.Image = ((System.Drawing.Image)(resources.GetObject("toolResetView.Image")));
            this.toolResetView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolResetView.Name = "toolResetView";
            this.toolResetView.Size = new System.Drawing.Size(34, 36);
            this.toolResetView.Text = "Reset view";
            this.toolResetView.Click += new System.EventHandler(this.toolResetView_Click);
            // 
            // toolTopView
            // 
            this.toolTopView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolTopView.Image = ((System.Drawing.Image)(resources.GetObject("toolTopView.Image")));
            this.toolTopView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolTopView.Name = "toolTopView";
            this.toolTopView.Size = new System.Drawing.Size(34, 36);
            this.toolTopView.Text = "Top view";
            this.toolTopView.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(34, 6);
            // 
            // toolParallelProjection
            // 
            this.toolParallelProjection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolParallelProjection.Image = ((System.Drawing.Image)(resources.GetObject("toolParallelProjection.Image")));
            this.toolParallelProjection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolParallelProjection.Name = "toolParallelProjection";
            this.toolParallelProjection.Size = new System.Drawing.Size(34, 36);
            this.toolParallelProjection.Text = "toolStripButton1";
            this.toolParallelProjection.ToolTipText = "Use parallel projection";
            this.toolParallelProjection.Click += new System.EventHandler(this.toolParallelProjection_Click);
            // 
            // toolStripClear
            // 
            this.toolStripClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripClear.Image = ((System.Drawing.Image)(resources.GetObject("toolStripClear.Image")));
            this.toolStripClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripClear.Name = "toolStripClear";
            this.toolStripClear.Size = new System.Drawing.Size(34, 36);
            this.toolStripClear.Text = "Clear";
            this.toolStripClear.Click += new System.EventHandler(this.toolStripClear_Click);
            // 
            // timer
            // 
            this.timer.Interval = 33;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // gl
            // 
            this.gl.BackColor = System.Drawing.Color.Black;
            this.gl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gl.Location = new System.Drawing.Point(37, 0);
            this.gl.Name = "gl";
            this.gl.Size = new System.Drawing.Size(793, 504);
            this.gl.TabIndex = 2;
            this.gl.VSync = false;
            this.gl.Paint += new System.Windows.Forms.PaintEventHandler(this.gl_Paint);
            this.gl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ThreeDControl_KeyDown);
            this.gl.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ThreeDControl_KeyPress);
            this.gl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gl_MouseDown);
            this.gl.MouseEnter += new System.EventHandler(this.ThreeDControl_MouseEnter);
            this.gl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gl_MouseMove);
            this.gl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gl_MouseUp);
            this.gl.Resize += new System.EventHandler(this.gl_Resize);
            // 
            // ThreeDControl
            // 
            this.Controls.Add(this.gl);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ThreeDControl";
            this.Size = new System.Drawing.Size(830, 504);
            this.Load += new System.EventHandler(this.ThreeDControl_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ThreeDControl_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ThreeDControl_KeyPress);
            this.MouseEnter += new System.EventHandler(this.ThreeDControl_MouseEnter);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolRotate;
        private System.Windows.Forms.ToolStripButton toolResetView;
        private System.Windows.Forms.ToolStripButton toolMoveViewpoint;
        private System.Windows.Forms.ToolStripButton toolZoom;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolMoveObject;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStripButton toolStripClear;
        public RepetierHost.view.utils.RHOpenGL gl;
        private System.Windows.Forms.ToolStripButton toolTopView;
        public System.Windows.Forms.ToolStripButton toolParallelProjection;
        private System.Windows.Forms.ToolStripButton toolIsometric;
    }
}
