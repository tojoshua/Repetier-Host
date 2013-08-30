using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RepetierHost.model;

namespace RepetierHost.view
{
    public partial class TemperatureView : UserControl
    {
        double[] tempTickSizes = { 100, 50, 25, 20, 10, 5, 1 };
        double[] timeTickSizes = { 1800000, 900000, 600000, 300000, 60000, 30000, 15000, 5000, 1000 };
        Font font = null;
        TemperatureHistory hist;
        double righttime;
        double lefttime;
        double timeScale;
        double tempScale;
        double outScale;
        double minTemp, maxTemp;
        double timeTick;
        double tempTick;
        float axisWidth;
        float tickExtra = 3, spaceExtra = 3;
        float timeWidth, timeHeight, tempWidth;

        public TemperatureView()
        {
            InitializeComponent();
        }

        private void TemperatureView_Paint(object sender, PaintEventArgs ev)
        {
            hist = Main.main.history;
            if (hist == null) return;
            Graphics g = ev.Graphics;
            SolidBrush backBrush = new SolidBrush(hist.backgroundColor);
            if (font == null)
            {
                font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);

                SizeF sz = g.MeasureString("00:00", font);
                timeWidth = sz.Width;
                timeHeight = sz.Height;
                sz = g.MeasureString("000", font);
                tempWidth = sz.Width;
            }
            g.FillRectangle(backBrush, 0, 0, Width, Height);

            float height = Height;
            float width = Width;
            float fontLeft = tempWidth + tickExtra + spaceExtra + 5;
            float fontBottom = (timeHeight + tickExtra + 3);
            float marginTop = 5;
            double timespan = hist.CurrentZoomLevel * 60000.0;
            if (hist.Autoscroll)
                hist.xpos = 100.0;
            righttime = (hist.currentHistory.maxTime / 10000) - (3600000 - timespan) * 0.01 * (100.0 - hist.xpos);
            lefttime = righttime - timespan;
            Rectangle outputRect, tempRect;
            minTemp = 0; maxTemp = 300;
            bool hasTemp = false;
            foreach (TemperatureEntry e in hist.currentHistory.entries)
            {
                if (e.time/10000 < lefttime || e.time/10000 > righttime) continue;
                if (hist.ShowExtruder)
                {
                    { if (e.extruder >= 0) { if (!hasTemp) { minTemp = maxTemp = e.extruder; hasTemp = true; } else { minTemp = Math.Min(e.extruder, minTemp); maxTemp = Math.Max(e.extruder, maxTemp); } } }
                    if (hist.ShowAverage)
                    { if (e.avgExtruder >= 0) { if (!hasTemp) { minTemp = maxTemp = e.avgExtruder; hasTemp = true; } else { minTemp = Math.Min(e.avgExtruder, minTemp); maxTemp = Math.Max(e.avgExtruder, maxTemp); } } }
                    if (hist.ShowTarget)
                    { if (e.targetExtruder >= 0) { if (!hasTemp) { minTemp = maxTemp = e.targetExtruder; hasTemp = true; } else { minTemp = Math.Min(e.targetExtruder, minTemp); maxTemp = Math.Max(e.targetExtruder, maxTemp); } } }
                }
                if (hist.ShowBed)
                {
                    { if (e.bed >= 0) { if (!hasTemp) { minTemp = maxTemp = e.bed; hasTemp = true; } else { minTemp = Math.Min(e.bed, minTemp); maxTemp = Math.Max(e.bed, maxTemp); } } }
                    if (hist.ShowAverage)
                    { if (e.avgBed >= 0) { if (!hasTemp) { minTemp = maxTemp = e.avgBed; hasTemp = true; } else { minTemp = Math.Min(e.avgBed, minTemp); maxTemp = Math.Max(e.avgBed, maxTemp); } } }
                    if (hist.ShowTarget)
                    { if (e.targetBed >= 0) { if (!hasTemp) { minTemp = maxTemp = e.targetBed; hasTemp = true; } else { minTemp = Math.Min(e.targetBed, minTemp); maxTemp = Math.Max(e.targetBed, maxTemp); } } }
                }
            }
            maxTemp += 4;
            minTemp -= 4;
            maxTemp = Math.Ceiling(maxTemp / 10.0) * 10.0;
            minTemp = Math.Floor(minTemp / 10.0) * 10.0;
            if (minTemp < 0) minTemp = 0;
            int i;
            if (hist.ShowOutput && height > 4 * (fontBottom + marginTop))
            {
                double h1 = 0.75 * height;
                double h2 = 0.25 * height;
                tempRect = new Rectangle((int)fontLeft, (int)0, (int)(width - 2 * fontLeft), (int)(h1 - fontBottom - marginTop));
                outputRect = new Rectangle((int)fontLeft, (int)h1, (int)(width - 2 * fontLeft), (int)(h2 - fontBottom - marginTop));
                outScale = 255.0 / (h2 - fontBottom - marginTop);
            }
            else
            {
                tempRect = new Rectangle((int)fontLeft, (int)0, (int)(width - 2 * fontLeft), (int)(height - fontBottom - marginTop));
            }
            double theight = tempRect.Height;
            tempScale = theight / (maxTemp - minTemp);
            int best = 0;
            for (i = 0; i < 7; i++)
            {
                double dist = tempScale * tempTickSizes[i];
                if (dist > 20) best = i;
            }
            tempTick = tempTickSizes[best];
            best = 0;
            double twidth = tempRect.Width;
            timeScale = twidth / timespan;
            for (i = 0; i < 9; i++)
            {
                double dist = timeScale * timeTickSizes[i];
                if (dist > 40) best = i;
            }
            timeTick = timeTickSizes[best];
            drawGrid(tempRect, true, g);
            if (hist.ShowOutput && height > 4 * (fontBottom + marginTop))
            {
                double h1 = 0.75 * height;
                double h2 = 0.25 * height;
                outputRect = new Rectangle((int)fontLeft, (int)h1, (int)(width - 2 * fontLeft), (int)(h2 - fontBottom - marginTop));
                drawGrid(outputRect, false, g);
            }

        }

        private void drawGrid(Rectangle rect, bool isTemp, Graphics g)
        {
            // Draw grid lines
            Pen pen = new Pen(hist.gridColor, 1);
            double x = timeTick * (Math.Floor(lefttime / timeTick));
            while (x < lefttime) x += timeTick;
            Pen axis = new Pen(hist.axisColor, 2);
            float ybot = rect.Bottom;
            float ytop = rect.Top;
            float xleft = rect.Left;
            axisWidth = rect.Width;
            float xright = xleft + axisWidth;
            SolidBrush fontBrush = new SolidBrush(hist.fontColor);
            for (; x < righttime; x += timeTick)
            {
                float xp = (float)(xleft + (x - lefttime) * timeScale);
                g.DrawLine(pen, xp, ybot, xp, ytop);
                g.DrawLine(axis, xp, ybot, xp, ybot + tickExtra);
                DateTime d = new DateTime((long)(x) * 10000);
                string time = d.ToString("mm:ss");
                g.DrawString(time, font, fontBrush, (float)(xp - 0.5 * timeWidth), (float)(ybot + tickExtra /*- timeHeight*/));
            }
            if (isTemp)
            {
                double y = minTemp;
                for (; y <= maxTemp; y += tempTick)
                {
                    float yp = (float)(ybot - (y - minTemp) * tempScale);
                    g.DrawLine(pen, xleft, yp, xright, yp);
                    g.DrawLine(axis, xleft, yp, xleft - tickExtra, yp);
                    g.DrawLine(axis, xright, yp, xright + tickExtra, yp);
                    string tempText = y.ToString();
                    g.DrawString(tempText, font, fontBrush, (float)(xleft - tickExtra - tempWidth - spaceExtra), (float)(yp - 0.5 * timeHeight));
                    g.DrawString(tempText, font, fontBrush, (float)(xright + tickExtra + spaceExtra), (float)(yp - 0.5 * timeHeight));
                }
            }
            else
            {
                for (int i = 0; i < 110; i += 50)
                {
                    float yp = (float)i * .01f * (ytop - ybot) + ybot;
                    g.DrawLine(pen, xleft, yp, xright, yp);
                    string tempText = i.ToString();
                    g.DrawString(tempText, font, fontBrush, (float)(xleft - tickExtra - tempWidth - spaceExtra), (float)(yp - 0.5 * timeHeight));
                    g.DrawString(tempText, font, fontBrush, (float)(xright + tickExtra + spaceExtra), (float)(yp - 0.5 * timeHeight));
                }
            }
            g.DrawRectangle(axis, xleft, ytop, xright - xleft, ybot - ytop);
            g.SetClip(rect);
            if (isTemp)
            {
                LinkedList<PointF> pExt = null;
                LinkedList<PointF> pAvgExt = null;
                LinkedList<PointF> pTarExt = null;
                LinkedList<PointF> pBed = null;
                LinkedList<PointF> pTarBed = null;
                LinkedList<PointF> pAvgBed = null;
                if (hist.ShowExtruder)
                {
                    pExt = new LinkedList<PointF>();
                    if (hist.ShowAverage)
                        pAvgExt = new LinkedList<PointF>();
                    if (hist.ShowTarget)
                        pTarExt = new LinkedList<PointF>();
                }
                if (hist.ShowBed)
                {
                    pBed = new LinkedList<PointF>();
                    if (hist.ShowAverage)
                        pAvgBed = new LinkedList<PointF>();
                    if (hist.ShowTarget)
                        pTarBed = new LinkedList<PointF>();
                }
                foreach (TemperatureEntry e in hist.currentHistory.entries)
                {
                    float xp = (float)(xleft + (e.time / 10000 - lefttime) * timeScale);
                    if (pExt != null && e.extruder >= 0)
                    {
                        pExt.AddLast(new PointF(xp, (float)(ybot - (e.extruder - minTemp) * tempScale)));
                    }
                    if (pAvgExt != null && e.avgExtruder >= 0)
                    {
                        pAvgExt.AddLast(new PointF(xp, (float)(ybot - (e.avgExtruder - minTemp) * tempScale)));
                    }
                    if (pTarExt != null && e.targetExtruder >= 0)
                    {
                        pTarExt.AddLast(new PointF(xp, (float)(ybot - (e.targetExtruder - minTemp) * tempScale)));
                    }
                    if (pBed != null && e.bed >= 0)
                    {
                        pBed.AddLast(new PointF(xp, (float)(ybot - (e.bed - minTemp) * tempScale)));
                    }
                    if (pAvgBed != null && e.avgExtruder >= 0)
                    {
                        pAvgBed.AddLast(new PointF(xp, (float)(ybot - (e.avgBed - minTemp) * tempScale)));
                    }
                    if (pTarBed != null && e.targetBed >= 0)
                    {
                        pTarBed.AddLast(new PointF(xp, (float)(ybot - (e.targetBed - minTemp) * tempScale)));
                    }
                    if (e.time / 10000 > righttime) break;
                }
                // Draw temperatures
                if (pTarExt != null && pTarExt.Count > 1)
                {
                    g.DrawLines(new Pen(hist.targetExtruderColor, (float)hist.targetExtruderWidth), pTarExt.ToArray());
                }
                if (pAvgExt != null && pAvgExt.Count > 1)
                {
                    g.DrawLines(new Pen(hist.avgExtruderColor, (float)hist.avgExtruderWidth), pAvgExt.ToArray());
                }
                if (pExt != null && pExt.Count > 1)
                {
                    g.DrawLines(new Pen(hist.extruderColor, (float)hist.extruderWidth), pExt.ToArray());
                }
                if (pTarBed != null && pTarBed.Count > 1)
                {
                    g.DrawLines(new Pen(hist.targetBedColor, (float)hist.targetBedWidth), pTarBed.ToArray());
                }
                if (pAvgBed != null && pAvgBed.Count > 1)
                {
                    g.DrawLines(new Pen(hist.avgBedColor, (float)hist.avgBedWidth), pAvgBed.ToArray());
                }
                if (pBed != null && pBed.Count > 1)
                {
                    g.DrawLines(new Pen(hist.bedColor, (float)hist.bedWidth), pBed.ToArray());
                }
            }
            else
            {
                LinkedList<PointF> pOut = null;
                LinkedList<PointF> pAvgOut = null;
                pOut = new LinkedList<PointF>();
                if (hist.ShowAverage)
                    pAvgOut = new LinkedList<PointF>();

                float xp = 0;
                foreach (TemperatureEntry e in hist.currentHistory.entries)
                {
                    if (e.time / 10000 < lefttime - 1) continue;
                    xp = (float)(xleft + (e.time / 10000 - lefttime) * timeScale);
                    if (pOut != null && e.output >= 0)
                    {
                        float yp = ybot + (float)e.output / 255 * (ytop - ybot);
                        if (pOut.Count == 0)
                            pOut.AddLast(new PointF(xp, ybot));
                        pOut.AddLast(new PointF(xp, yp));
                    }
                    if (pAvgOut != null && e.avgOutput >= 0)
                    {
                        float yp = ybot + (float)e.avgOutput / 255 * (ytop - ybot);
                        pAvgOut.AddLast(new PointF(xp, yp));
                    }
                    if (e.time / 10000 > righttime) break;
                }
                if (pOut.Count != 0)
                {
                    pOut.AddLast(new PointF(xp, ybot));
                    //[pOut closePath];
                }
                if (pOut != null && pOut.Count > 2)
                {
                    g.FillPolygon(new SolidBrush(hist.outputColor), pOut.ToArray());
                }
                if (pAvgOut != null && pAvgOut.Count > 1)
                {
                    g.DrawLines(new Pen(hist.avgOutputColor, (float)hist.avgOutputWidth), pAvgOut.ToArray());
                }
            }
            g.ResetClip();
        }

        int mdx;
        private void TemperatureView_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int dx = e.X - mdx;
                double delta = 100 * dx * (righttime - lefttime) / (axisWidth * 3600000);
                hist.xpos -= delta;
                if (hist.xpos < 0) hist.xpos = 0;
                if (hist.xpos > 100) hist.xpos = 100;
                hist.Autoscroll = hist.xpos == 100;
                mdx = e.X;
                this.Refresh();
            }
        }

        private void TemperatureView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mdx = e.X;
            }
        }
    }
}
