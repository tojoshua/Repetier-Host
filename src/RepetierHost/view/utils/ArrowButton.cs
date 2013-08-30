using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using RepetierHost.model;

namespace RepetierHost.view.utils
{
    public delegate void ArrowValueChanged(ArrowButton sender,string value);
    public partial class ArrowButton : Button
    {
        private GraphicsPath path;
        private GraphicsPath innerPath;
        private Color borderColor = Color.Black;
        private Color gradientStart = Color.WhiteSmoke;
        private Color gradientEnd = Color.Violet;
        private Color highGradientStart = Color.White;
        private Color highGradientEnd = Color.Red;
        public event ArrowValueChanged arrowValueChanged = null;
        private bool _clicked = false;
        private int hoverWidth;
        private int _rotation = 0;
        private float arrowBaseHeight = 0.5f;
        private int arrowHeadWidth = 30;
        private string possibleValues = "0.1;1;10;100";
        private string currentValue = "";
        private System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 14, FontStyle.Bold);
        private string title = "+X";

        public ArrowButton()
        {
            InitializeComponent();
        }
        public string CurrentValue
        {
            get { return currentValue; }
        }
        public float CurrentValueF
        {
            get { if (currentValue.Length == 0) return 0; return float.Parse(currentValue, GCode.format); }
        }
        public string Title
        {
            get { return title; }
            set { title = value; Invalidate(); }
        }
        public string PossibleValues
        {
            get { return possibleValues; }
            set { possibleValues = value; Invalidate(); }
        }
        public Color GradientStartColor
        {
            get { return gradientStart; }
            set { gradientStart = value; Invalidate(); }
        }
        public Color GradientEndColor
        {
            get { return gradientEnd; }
            set { gradientEnd = value; Invalidate(); }
        }
        public Color HighGradientStartColor
        {
            get { return highGradientStart; }
            set { highGradientStart = value; Invalidate(); }
        }
        public Color HighGradientEndColor
        {
            get { return highGradientEnd; }
            set { highGradientEnd = value; Invalidate(); }
        }

        public Font TitleFont
        {
            get { return drawFont; }
            set { drawFont = value; Invalidate(); }
        }
        public float ArrowBaseHeight
        {
            get { return arrowBaseHeight; }
            set { arrowBaseHeight = value; Invalidate(); }
        }
        public int ArrowHeadWidth
        {
            get { return arrowHeadWidth; }
            set { arrowHeadWidth = value; Invalidate(); }
        }

        public bool Clicked
        {
            get { return _clicked; }
            set
            {
                _clicked = value;
                Invalidate();
            }
        }

        public int Rotation
        {
            get { return _rotation; }
            set { _rotation = value; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.FillRectangle(SystemBrushes.Control, 0, 0, Width, Height);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Create painting objects
            Brush b = new SolidBrush(this.ForeColor);

            // Create Rectangle To Limit brush area.
            Rectangle rect = new Rectangle(0, 0, Width, Height);

            LinearGradientBrush linearBrush = null;
            if(Enabled)
              linearBrush = new LinearGradientBrush(rect,
              gradientStart,
              gradientEnd,
              _rotation);
            else
                linearBrush = new LinearGradientBrush(rect,
                Color.SlateGray,
                Color.DarkGray,
                _rotation);
            LinearGradientBrush highlightBrush =
              new LinearGradientBrush(rect,
              highGradientStart,
              highGradientEnd,
              _rotation);

            path = new GraphicsPath();
            innerPath = new GraphicsPath();
            float textX = Width / 2;
            float textY = Height / 2;
            Region cliphover = Region;
            switch (Rotation)
            {
                case 0:
                    {
                        int h = Height-1;
                        int w = Width-1;
                        int wa = w - arrowHeadWidth;
                        int h1 = (int)(h * (1 - arrowBaseHeight) / 2.0f);
                        PointF[] la =
             {
                 new PointF(1,h1),
                 new PointF(wa,h1),
                 new PointF(wa,1),
                 new PointF(w,h/2),
                 new PointF(wa,h),
                 new PointF(wa,h-h1),
                 new PointF(1,h-h1),
                 new PointF(1,h1)
             };
                        path.AddLines(la);
                        PointF[] la2 =
             {
                 new PointF(0,h1-1),
                 new PointF(wa-1,h1-1),
                 new PointF(wa-1,0),
                 new PointF(w+1,h/2),
                 new PointF(wa-1,h+1),
                 new PointF(wa-1,h-h1+1),
                 new PointF(0,h-h1+1),
                 new PointF(0,h1-1)
             };
                        innerPath.AddLines(la2);
                        cliphover = new Region(new Rectangle(0, 0, hoverWidth, Height));
                    }
                    break;
                case 90:
                    {
                        int h = Height - 1;
                        int w = Width - 1;
                        int ha = h-arrowHeadWidth+1;
                        int w1 = (int)(w * (1 - arrowBaseHeight) / 2.0f);
                        PointF[] la =
             {
                 new PointF(w1,1),
                 new PointF(w1,ha),
                 new PointF(1,ha),
                 new PointF(w/2,h),
                 new PointF(w,ha),
                 new PointF(w-w1,ha),
                 new PointF(w-w1,1),
                 new PointF(w1,1)
             };
                        path.AddLines(la);
                        PointF[] la2 =
             {
                 new PointF(w1-1,0),
                 new PointF(w1-1,ha-1),
                 new PointF(0,ha-1),
                 new PointF(w/2,h+1),
                 new PointF(w+1,ha-1),
                 new PointF(w-w1+1,ha-1),
                 new PointF(w-w1+1,0),
                 new PointF(w1-1,0)
             };
                        innerPath.AddLines(la2);
                        cliphover = new Region(new Rectangle(0, 0, Width, hoverWidth));

                    }
                    break;
                case 180:
                    {
                        int h = Height - 1;
                        int w = Width - 1;
                        int wa = arrowHeadWidth+1;
                        int h1 = (int)(h * (1 - arrowBaseHeight) / 2.0f);
                        PointF[] la =
             {
                 new PointF(w,h1),
                 new PointF(wa,h1),
                 new PointF(wa,1),
                 new PointF(1,h/2),
                 new PointF(wa,h),
                 new PointF(wa,h-h1),
                 new PointF(w,h-h1),
                 new PointF(w,h1)
             };
                        path.AddLines(la);
                        PointF[] la2 =
             {
                 new PointF(w+1,h1-1),
                 new PointF(wa-1,h1-1),
                 new PointF(wa-1,0),
                 new PointF(0,h/2),
                 new PointF(wa-1,h+1),
                 new PointF(wa-1,h-h1+1),
                 new PointF(w+1,h-h1+1),
                 new PointF(w+1,h1-1)
             };
                        innerPath.AddLines(la2);

                    }
                    cliphover = new Region(new Rectangle(Width-hoverWidth, 0, hoverWidth, Height));
                    break;
                case 270:
                    {
                        int h = Height - 1;
                        int w = Width - 1;
                        int ha = arrowHeadWidth + 1;
                        int w1 = (int)(w * (1 - arrowBaseHeight) / 2.0f);
                        PointF[] la =
             {
                 new PointF(w1,h),
                 new PointF(w1,ha),
                 new PointF(1,ha),
                 new PointF(w/2,1),
                 new PointF(w,ha),
                 new PointF(w-w1,ha),
                 new PointF(w-w1,h),
                 new PointF(w1,h)
             };
                        path.AddLines(la);
                        PointF[] la2 =
             {
                 new PointF(w1-1,h+1),
                 new PointF(w1-1,ha+1),
                 new PointF(0,ha-1),
                 new PointF(w/2,0),
                 new PointF(w+1,ha+1),
                 new PointF(w-w1+1,ha+1),
                 new PointF(w-w1+1,h+1),
                 new PointF(w1-1,h+1)
             };
                        innerPath.AddLines(la2);

                        cliphover = new Region(new Rectangle(0, Height-hoverWidth, Width, hoverWidth));
                    }
                    break;
            }
            //this.Region = new Region(innerPath); // Set shape for hit detection
            Brush fontBrush = Brushes.Black;
            PathGradientBrush pgbrush = new PathGradientBrush(innerPath);
            pgbrush.CenterPoint = new Point(75, 75);
            pgbrush.CenterColor = Color.White;
            pgbrush.SurroundColors = new Color[] { this.ForeColor };
            Pen p = new Pen(borderColor, 1);
            g.FillPath(linearBrush, path);
            Region clip = g.Clip;
            g.Clip = cliphover;
            g.FillPath(highlightBrush, path);
            // Draw seperations
            string[] va = possibleValues.Split(';');
            g.Clip = new Region(path);
            switch (_rotation)
            {
                case 0: {
                    for (int i = 0; i < va.Length-1; i++)
                    {
                        int pos = (i + 1) * Width / va.Length;
                        g.DrawLine(Pens.DarkGray,pos,0,pos,Height);
                    }
                }
                    break;
                case 180:
                    {
                        for (int i = 0; i < va.Length - 1; i++)
                        {
                            int pos = Width-(i + 1) * Width / va.Length;
                            g.DrawLine(Pens.DarkGray, pos, 0, pos, Height);
                        }
                    }
                    break;
                case 90:
                    {
                        for (int i = 0; i < va.Length - 1; i++)
                        {
                            int pos = (i + 1) * Height / va.Length;
                            g.DrawLine(Pens.DarkGray,0, pos, Width, pos);
                        }
                    }
                    break;
                case 270:
                    {
                        for (int i = 0; i < va.Length - 1; i++)
                        {
                            int pos = Height - (i + 1) * Height / va.Length;
                            g.DrawLine(Pens.DarkGray,0, pos, Width, pos);
                        }
                    }
                    break;
            }
            g.Clip = clip;
            g.DrawPath(p, path);
            SizeF fsize = g.MeasureString(title, drawFont);
            textX -= fsize.Width / 2;
            textY -= fsize.Height / 2;
            g.DrawString(title, drawFont, fontBrush, textX, textY);
            // Dispose of painting objects
            b.Dispose();
            p.Dispose();
            pgbrush.Dispose();
            linearBrush.Dispose();
            highlightBrush.Dispose();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.Cursor = Cursors.Hand;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            hoverWidth = 0;
            currentValue = "";
            base.OnMouseLeave(e);
            if (arrowValueChanged != null)
                arrowValueChanged(this, currentValue);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            _clicked = true;
            base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            _clicked = false;
            base.OnMouseUp(mevent);
        }
        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            if (!Enabled) return;
            string oldcur = currentValue;
            base.OnMouseMove(mevent);
            float percent = 0;
            int full = 0;
            if (_rotation == 0 || _rotation == 180)
            {
                full = Width;
                percent = (_rotation==0? mevent.X :Width-mevent.X) / (float)Width;
            }
            else
            {
                full = Height;
                percent = (_rotation==90 ? mevent.Y : Height-mevent.Y)/ (float)Height;
            }
            string[] vl = possibleValues.Split(';');
            int p = (int)(percent * vl.Length);
            if (p < 0) p = 0;
            if (p >= vl.Length) p = vl.Length - 1;
            currentValue = vl[p];
            hoverWidth = (p + 1) * full / vl.Length;
            Invalidate();
            if (currentValue != oldcur && arrowValueChanged != null)
                arrowValueChanged(this, currentValue);
        }
    }
}
