/*
   Copyright 2011 repetier repetierdev@gmail.com

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RepetierHost.model;

namespace RepetierHost.view
{
    public partial class LogBox : UserControl
    {
        int _row = 0;
        int selRow = 0;
        bool hasSel = false, forceSel = false;
        int _topRow = 0; // First visible row
        int rowsVisible = 10,colsVisible = 7;
        Font drawFont = new Font(FontFamily.GenericMonospace/*"Courier New"*/, 12, GraphicsUnit.Pixel);
        SolidBrush blackBrush = new SolidBrush(Color.Black);
        Brush normalBrush = Brushes.DarkBlue;
        Brush infoBrush = Brushes.DarkBlue;
        Brush warningBrush = Brushes.OrangeRed;
        Brush errorBrush = Brushes.Maroon;
        Brush linesBgColor = Brushes.CadetBlue;
        Brush linesTextColor = Brushes.White;
        Brush backBrush = Brushes.White;
        Brush evenBackBrush = Brushes.Linen;
        Brush selectionBrush = Brushes.LightSeaGreen;
        Brush selectionTextBrush = Brushes.White;
        float fontHeight;
        float fontWidth;
        bool hasFocus = false;
        int linesWidth = 100;
        bool ignoreMouseDown = false;
        bool ignoreScrollChange = false;
        int maxLines = 2000;
        public bool Autoscroll = true;
        List<LogLine> lines = new List<LogLine>();

        public LogBox()
        {
            InitializeComponent();
            fontHeight = drawFont.Height;
            log.MouseWheel += MouseWheelHandler;
        }
        public void ScrollBottom()
        {
            if (lines.Count >= rowsVisible - 1)
            {
                topRow = lines.Count - rowsVisible+1;
            }
        }
        public void UpdateBox()
        {
            ignoreScrollChange = true;
            topRow = Math.Min(topRow, lines.Count - rowsVisible - 1);
            scroll.Maximum = Math.Max(0,lines.Count-rowsVisible-1);
            scroll.Value = topRow;
            scroll.LargeChange = Math.Max(1, rowsVisible - 1);
            if (Autoscroll && !hasFocus)
                ScrollBottom();
            ignoreScrollChange = false;
            log.Invalidate();
        }
        public void Clear()
        {
            topRow = _row = selRow = 0;
            lines.Clear();
        }
        public void Add(LogLine l)
        {
            if(!hasFocus)
                while (lines.Count >= maxLines)
                {
                    _row--;
                    selRow--;
                    lines.RemoveAt(0);
                }
            lines.Add(l);
        }
        private int row
        {
            get { return _row; }
            set { _row = value; }
        }
        private int topRow
        {
            get { return _topRow; }
            set
            {
                _topRow = value; if (_topRow >= lines.Count) _topRow = lines.Count - 1;
                if (_topRow < 0) _topRow = 0;
                ignoreScrollChange = true;
                scroll.Maximum = Math.Max(_topRow, scroll.Maximum);
                scroll.Value = _topRow;
                ignoreScrollChange = false;
            }
        }
        private void DrawRow(Graphics g, int line, LogLine logline, float y)
        {
            int p = logline.text.IndexOf(" : ");
            string time = "", text = "";
            if (p < 0)
                text = logline.text;
            else
            {
                time = logline.text.Substring(0, p);
                text = logline.text.Substring(p + 3);
            }
            Brush fontBrush = normalBrush;
            Brush bgBrush = ((line & 1)!=0 ? evenBackBrush : backBrush);
            if (hasFocus && line >= Math.Min(row, selRow) && line <= Math.Max(row, selRow))
            { // mark selection
                bgBrush = selectionBrush;
                fontBrush = selectionTextBrush;
            }
            else
            {
                switch (logline.level)
                {
                    case 0:
                        fontBrush = normalBrush;
                        break;
                    case 1:
                        fontBrush = warningBrush;
                        break;
                    case 2:
                        fontBrush = errorBrush;
                        break;
                    case 3:
                        fontBrush = infoBrush;
                        break;
                    case 4:
                        fontBrush = normalBrush;
                        break;
                }
            }
            g.FillRectangle(bgBrush, linesWidth, y, log.Width-linesWidth, fontHeight);
            g.DrawString(text, drawFont, fontBrush, linesWidth+4, y);
            g.DrawString(time, drawFont, linesTextColor, linesWidth - 3 - fontWidth * time.Length, y);
        }
        private void log_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            SizeF sz = g.MeasureString("MMMMMMMMMM", drawFont);
            fontWidth = (int)(sz.Width / 10) + 1;
            fontHeight = (int)Math.Ceiling(sz.Height);
            string maxl = lines.Count.ToString();
            linesWidth = (int)(fontWidth * 12 + 6);
            rowsVisible = (int)Math.Ceiling((double)log.Height / fontHeight);
            colsVisible = (int)Math.Ceiling((double)(log.Width - linesWidth) / fontWidth);

            int r;
            int rmax = rowsVisible;
            if (topRow + rmax >= lines.Count)
                rmax = lines.Count - topRow;
            g.FillRectangle(linesBgColor, 0, 0, linesWidth, log.Height);
            for (r = 0; r < rmax; r++)
            {
                DrawRow(g, topRow + r, lines.ElementAt(topRow + r), r * fontHeight);
            }
        }
        private void CursorDown()
        {
            if (row < lines.Count - 1)
            {
                row++;
                PositionShowCursor();
            }
        }
        private void CursorHome()
        {
            _row = topRow = 0;
            PositionShowCursor();
        }
        private void CursorEnd()
        {
            _row = lines.Count - 1;
            topRow = Math.Max(0, lines.Count - rowsVisible - 1);
            PositionShowCursor();
        }
        private void CursorPageDown()
        {
            if (row + rowsVisible < lines.Count)
            {
                topRow += rowsVisible - 1;
                row += rowsVisible - 1;
                PositionShowCursor();
            }
            else
            {
                row = lines.Count - 1;
                PositionShowCursor();
            }
        }
        private void CursorPageUp()
        {
            if (topRow > 0)
            {
                topRow -= rowsVisible - 1;
                row -= rowsVisible - 1;
                if (topRow < 0) topRow = 0;
                if (row < 0) { row = 0; }
                PositionShowCursor();
            }
            else
            {
                row = 0;
                PositionShowCursor();
            }
        }
        private void PositionShowCursor()
        {
            PositionShowCursor(false, true);
        }
        private void PositionShowCursor(bool repaint, bool moved)
        {
            scroll.Maximum = lines.Count();
            repaint |= hasSel;
            if (row < topRow)
            {
                topRow = row;
                repaint = true;
            }
            else if (row > topRow + rowsVisible - 2)
            {
                topRow = row - rowsVisible + 2;
                repaint = true;
            }
            if (moved)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    repaint = true;
                    hasSel = true;
                }
                else
                {
                    if (!forceSel)
                    {
                        selRow = row;
                        hasSel = false;
                    }
                }
            }
            else
            {
                selRow = row;
                hasSel = false;
            }
            log.Invalidate();
            
        }
        private void CursorUp()
        {
            if (row > 0)
            {
                row--;
                PositionShowCursor();
            }
        }

        public string getSelection()
        {
            if (lines.Count == 0) return ""; // Nothing to copy
            int rstart = row;
            int rend = selRow;
            if (row > selRow)
            {
                rstart = selRow;
                rend = row;
            }
            int i;
            rend = Math.Min(rend, lines.Count - 1);
            rstart = Math.Min(rstart, lines.Count - 1);
            StringBuilder sb = new StringBuilder();
            for (i = rstart; i <= rend; i++)
            {
                sb.AppendLine(lines[i].text);                
            }
            return sb.ToString();
        }
        public bool hasSelection
        {
            get { return row != selRow; }
        }

        private void log_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    CursorDown();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Up:
                    CursorUp();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.End:
                    CursorEnd();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Home:
                    CursorHome();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.PageDown:
                    CursorPageDown();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.PageUp:
                    CursorPageUp();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.A:
                    if (e.Control)
                    {
                        selRow = 0;
                        row = Math.Max(0, lines.Count - 1);
                        forceSel = true;
                        PositionShowCursor(true, true);
                        forceSel = false;
                        hasSel = true;
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;
                case Keys.C:
                case Keys.X:
                    if (e.Control)
                    {
                        if (hasSelection)
                            Clipboard.SetText(getSelection());
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;
            }
        }

        private void scrollRows_ValueChanged(object sender, EventArgs e)
        {
            topRow = scroll.Value;
            log.Invalidate();
        }
        private void MouseWheelHandler(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                if (e.Delta > 0)
                    topRow -= rowsVisible/2;
                else
                    topRow += rowsVisible / 2;
                log.Invalidate();
            }
        }

        private void log_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= 32)
            {
               // InsertChar(e.KeyChar);
            }
            e.Handled = true;

        }

        private void log_MouseDown(object sender, MouseEventArgs e)
        {
            if (ignoreMouseDown) return;
            log.Focus();
            hasFocus = true;
            if (Control.ModifierKeys == Keys.Shift)
            {
                row = Math.Max(0, Math.Min(lines.Count - 1, topRow + (int)(e.Y / fontHeight)));
            }
            else
            {
                row = selRow = Math.Max(0, Math.Min(lines.Count - 1, topRow + (int)(e.Y / fontHeight)));
            }
            PositionShowCursor();

        }

        private void log_MouseMove(object sender, MouseEventArgs e)
        {
            if (ignoreMouseDown) return;
            if (e.Button == MouseButtons.Left)
            {
                row = Math.Max(0, Math.Min(lines.Count - 1, topRow + (int)(e.Y / fontHeight)));
                if (row < topRow + 3 && topRow > 0) topRow--;
                if (row > topRow - 4 + rowsVisible && topRow + rowsVisible - 3 < lines.Count) topRow++;
                hasSel = true;
                forceSel = true;
                PositionShowCursor(true,true);
                forceSel = false;
            }
        }

        private void log_MouseUp(object sender, MouseEventArgs e)
        {
            ignoreMouseDown = false;
        }

        private void log_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void log_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }

        private void log_MouseEnter(object sender, EventArgs e)
        {
            if (Control.MouseButtons == MouseButtons.None)
                ignoreMouseDown = false;
        }

        private void log_MouseLeave(object sender, EventArgs e)
        {
            ignoreMouseDown = true;
        }

        private void log_Enter(object sender, EventArgs e)
        {
            hasFocus = true;
            log.Invalidate();

        }

        private void log_Leave(object sender, EventArgs e)
        {
            hasFocus = false;
            UpdateBox();
            log.Invalidate();
        }

        private void scroll_ValueChanged(object sender, EventArgs e)
        {
            if (!ignoreScrollChange)
            {
                topRow = scroll.Value;
                log.Invalidate();
            }
        }
    }
}
