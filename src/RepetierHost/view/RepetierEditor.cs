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
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.Win32;
using RepetierHost.model;

namespace RepetierHost.view
{
    public delegate void ContentChangedEvent();
    public partial class RepetierEditor : UserControl
    {
        public enum UndoAction
        {
            ReplaceSelection = 1,

        }
        public class Undo
        {
            int col, row, selCol, selRow;
            string text,oldtext;
            UndoAction action;
            public Undo(UndoAction act, string t,string ot, int c, int r, int sc, int sr)
            {
                action = act;
                text = t;
                oldtext = ot;
                col = c;
                row = r;
                selCol = sc;
                selRow = sr;
            }
            public void DeleteSelection(RepetierEditor e,int cstart,int rstart,int cend,int rend)
            {
                // start row = begin first + end last row
                e.lines[rstart] = new GCodeShort(e.lines[rstart].text.Substring(0, cstart) + e.lines[rend].text.Substring(cend));
                if (rend > rstart)
                    e.lines.RemoveRange(rstart + 1, rend - rstart);
                e.row = e.selRow = rstart;
                e.col = e.selCol = cstart;
                if (e.lines.Count == 0) e.Clear();
            }
            private void InsertString(RepetierEditor e,string s)
            {
                int rstart = row;
                int cstart = col;
                int rend = selRow;
                int cend = selCol;
                if (row > selRow || (row == selRow && col > selCol))
                {
                    rstart = selRow;
                    cstart = selCol;
                    rend = row;
                    cend = col;
                }
                e._row = rstart;
                e._col = cstart;
                s = s.Replace("\r\n", "\n");
                s = s.Replace('\r', '\n');
                string[] la = s.Split('\n');
                string l = e.lines[e.row].text;
                if (e.col > l.Length) e.col = l.Length;
                la[0] = l.Substring(0, e.col) + la[0];
                int nc = la[la.Length - 1].Length;
                la[la.Length - 1] = la[la.Length - 1] + l.Substring(e.col);
                e.col = nc;
                e.lines[rstart] = new GCodeShort(la[0]);
                string[] la2 = new string[la.Length - 1];
                for (int i = 1; i < la.Length; i++)
                {
                    la2[i - 1] = la[i];
                }
                if (la.Length > 1)
                {
                    for (int j = 0; j < la2.Length; j++)
                        e.lines.Insert(e.row + 1 + j, new GCodeShort(la2[j]));
                }
                e.row += la.Length - 1;
            }
            private void EndPos(RepetierEditor e, string s,out int cpos,out int rpos)
            {
                int rstart = row;
                int cstart = col;
                int rend = selRow;
                int cend = selCol;
                if (row > selRow || (row == selRow && col > selCol))
                {
                    rstart = selRow;
                    cstart = selCol;
                    rend = row;
                    cend = col;
                }
                s = s.Replace("\r\n", "\n");
                s = s.Replace('\r', '\n');
                string[] la = s.Split('\n');
                string l = e.lines[rstart].text;
                if (cstart > l.Length) cstart = l.Length;
                la[0] = l.Substring(0, cstart) + la[0];
                int nc = la[la.Length - 1].Length;
                if (la.Length == 1) cpos = la[0].Length;
                else cpos = la[la.Length - 1].Length;
                rpos = rstart + la.Length - 1;
            }

            public void UndoAction(RepetierEditor e)
            {
                int rstart = row;
                int cstart = col;
                int rend = selRow;
                int cend = selCol;
                int ce, re;
                if (row > selRow || (row == selRow && col > selCol))
                {
                    rstart = selRow;
                    cstart = selCol;
                    rend = row;
                    cend = col;
                }
                EndPos(e, text, out ce, out re);
                DeleteSelection(e, cstart, rstart, ce, re);
                InsertString(e, oldtext);
                e.row = row;
                e.col = col;
                e.PositionShowCursor(true,false);
                e.Changed();
            }
            public void RedoAction(RepetierEditor e)
            {
                int rstart = row;
                int cstart = col;
                int rend = selRow;
                int cend = selCol;
                if (row > selRow || (row == selRow && col > selCol))
                {
                    rstart = selRow;
                    cstart = selCol;
                    rend = row;
                    cend = col;
                }
                e._row = row;
                e._col = col;
                e.selCol = selCol;
                e.selRow = selRow;
                DeleteSelection(e,cstart,rstart,cend,rend);
                InsertString(e, text);
                if (text.Length == 0)
                {
                    e.row = rstart;
                    e.col = cstart;
                }
                else
                {
                    int ce, re;
                    EndPos(e, text, out ce, out re);
                    e.row = re;
                    e.col = ce;
                }
                e.PositionShowCursor(true, false);
                e.Changed();
            }
        }
        public class Content
        {
            //public string Text;
            public List<GCodeShort> textArray;
            int col=0, row=0, selCol=0, selRow=0;
            int topRow=0, topCol=0;
            bool hasSel;
            LinkedList<Undo> undo = new LinkedList<Undo>();
            LinkedList<Undo> redo = new LinkedList<Undo>();
            RepetierEditor editor = null;
            public string name;
            public int etype; // 0 = G-Code, 1 = prepend, 2 = append
            public Content(RepetierEditor e, int tp, string _name)
            {
                name = _name;
                //Text = "";
                textArray = new List<GCodeShort>();
                textArray.Add(new GCodeShort(""));
                editor = e;
                etype = tp;
            }
            public string Text
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (GCodeShort code in textArray)
                        sb.AppendLine(code.text);
                    return sb.ToString();
                }
                set
                {
                    string[] la = value.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
                    textArray.Clear();
                    if (la.Length == 0) la = new string[] { "" };
                    foreach (string s in la)
                    {
                        textArray.Add(new GCodeShort(s));
                    }
                    row = col = topRow = topCol = selRow = selCol = 0;
                }
            }

            public void ResetPos()
            {
                col = row = selCol = selRow = topCol = topRow = topCol = 0;
                hasSel = false;
            }
            public void FromActive()
            {
                col = editor._col;
                row = editor._row;
                selCol = editor.selCol;
                selRow = editor.selRow;
                topCol = editor.topCol;
                topRow = editor.topRow;
                hasSel = editor.hasSel;
                //Text = editor.Text;
            }
            public void ToActive()
            {
                editor.lines = textArray;
                editor._col = col;
                editor._row = row;
                editor._topRow = topRow;
                editor._topCol = topCol;
                editor.selCol = selCol;
                editor.selRow = selRow;
                editor.hasSel = hasSel;
                editor.cur = this;
                editor.PositionShowCursor(true, false);
                editor.toolRow.Text = "R" + (row + 1).ToString();
                editor.toolColumn.Text = "C" + (col + 1).ToString();
                UpdateUndoButtons();
            }
            public void UpdateUndoButtons()
            {
                editor.toolUndo.Enabled = undo.Count > 0;
                editor.toolRedo.Enabled = redo.Count > 0;
            }
            public void ClearUndo() {
                undo.Clear();
                redo.Clear();
                UpdateUndoButtons();
            }
            public override string ToString()
            {
                return name;
            }
            public void Undo()
            {
                if (undo.Count > 0)
                {
                    Undo u = undo.First.Value;
                    undo.RemoveFirst();
                    redo.AddFirst(u);
                    u.UndoAction(editor);
                }
                UpdateUndoButtons();
            }
            public void Redo()
            {
                if (redo.Count > 0)
                {
                    Undo u = redo.First.Value;
                    redo.RemoveFirst();
                    undo.AddFirst(u);
                    u.RedoAction(editor);
                }
                UpdateUndoButtons();
            }
            public void AddUndo(Undo u)
            {
                undo.AddFirst(u);
                if (undo.Count > 100) undo.RemoveLast();
                redo.Clear();
                UpdateUndoButtons();
            }
        }
        Content cur = null;
        public Commands commands = null;
        public event ContentChangedEvent contentChangedEvent=null;
        int changedCounter = 0;
        public int _row = 0, _col = 0;
        public int selCol = 0, selRow = 0;
        bool hasSel = false,forceSel=false;
        int _topRow = 0; // First visible row
        int _topCol = 0;
        bool _overwrite = false;
        int rowsVisible = 10;
        int colsVisible = 10;
        int maxCol = 10;
        Font drawFont = new Font(FontFamily.GenericMonospace/*"Courier New"*/, 12,GraphicsUnit.Pixel);
        SolidBrush blackBrush = new SolidBrush(Color.Black);
        Brush commandBrush = Brushes.DarkBlue;
        Brush commentBrush = Brushes.OliveDrab;
        Brush paramTypeBrush = Brushes.Maroon;
        Brush linesBgColor = Brushes.CadetBlue;
        Brush linesBgAltColor = Brushes.DarkCyan;
        Brush linesTextColor = Brushes.White;
        Brush backBrush = Brushes.White;
        Brush evenBackBrush = Brushes.Linen;
        Brush hostBrush = Brushes.SeaGreen;
        Brush selectionBrush = Brushes.DarkTurquoise;
        Brush selectionTextColor = Brushes.White;
        Pen cursorBrush = Pens.Black;
        float fontHeight;
        float fontWidth;
        bool hasFocus = false;
        int linesWidth = 100;
        bool ignoreMouseDown = true;
        bool blink = true;
        int _maxLayer = 0;
        int _showMode = 0;
        int _showMinLayer = 0;
        int _showMaxLayer = 1;
        public double printingTime = 0;
        public event EventHandler ShowModeChanged;
        public event EventHandler ShowMinLayerChanged;
        public event EventHandler ShowMaxLayerChanged;
        public event EventHandler MaxLayerChanged;

        List<GCodeShort> lines=null;
        [DllImport("User32.dll")]
        static extern bool CreateCaret(IntPtr hWnd, int hBitmap, int nWidth, int nHeight);

        [DllImport("User32.dll")]
        static extern bool SetCaretPos(int x, int y);

        [DllImport("User32.dll")]
        static extern bool DestroyCaret();

        [DllImport("User32.dll")]
        static extern bool ShowCaret(IntPtr hWnd);

        [DllImport("User32.dll")]
        static extern bool HideCaret(IntPtr hWnd); 
        public RepetierEditor()
        {
            InitializeComponent();
            
            fontHeight = drawFont.Height;
            editor.MouseWheel += MouseWheelHandler;
            
    //        Text = System.IO.File.ReadAllText("d:\\arduino\\Mendel\\models\\work\\oozetest.gcode");
            Content c = new Content(this, 0, "G-Code");
            //c.Text = ""; // System.IO.File.ReadAllText("d:\\arduino\\Mendel\\models\\foambowl_export.gcode");
            toolFile.Items.Clear();
            toolFile.Items.Add(c);
            toolFile.Items.Add(new Content(this, 1, "Start code"));
            toolFile.Items.Add(new Content(this, 2, "End code"));
            toolFile.Items.Add(new Content(this, 3, "Run on kill"));
            toolFile.Items.Add(new Content(this, 4, "Run on pause"));
            toolFile.Items.Add(new Content(this, 5, "Script 1"));
            toolFile.Items.Add(new Content(this, 6, "Script 2"));
            toolFile.Items.Add(new Content(this, 7, "Script 3"));
            toolFile.Items.Add(new Content(this, 8, "Script 4"));
            toolFile.Items.Add(new Content(this, 9, "Script 5"));
            toolFile.SelectedIndex = 0;
            labelMaxLayer.DataBindings.Add("Text",
                           this, "MaxLayer");
            numericShowMinLayer.DataBindings.Add("Value",
                           this, "ShowMinLayer");
            numericShowMaxLayer.DataBindings.Add("Value",
                           this, "ShowMaxLayer");
            sliderShowFirstLayer.DataBindings.Add("value", this, "ShowMinLayer");
            sliderShowMaxLayer.DataBindings.Add("value", this, "ShowMaxLayer");
            Main.main.languageChanged += translate;
            translate();
        }
        void translate()
        {
            toolCopy.ToolTipText = Trans.T("L_COPY");
            toolCut.ToolTipText = Trans.T("L_CUT");
            toolNew.ToolTipText = Trans.T("L_NEW_TEXT");
            toolSave.ToolTipText = Trans.T("L_SAVE");
            toolPaste.ToolTipText = Trans.T("L_PASTE");
            toolUndo.ToolTipText = Trans.T("L_UNDO");
            toolRedo.ToolTipText = Trans.T("L_REDO");
            toolPreview.ToolTipText = Trans.T("L_AUTO_UPDATE_PREVIEW"); // Autoupdate preview
            overwrite = _overwrite;
            row = _row;
            col = _col;
            UpdateLayerInfo();
            ((Content)toolFile.Items[1]).name = Trans.T("L_START_CODE");
            ((Content)toolFile.Items[2]).name = Trans.T("L_END_CODE");
            ((Content)toolFile.Items[3]).name = Trans.T("L_RUN_ON_KILL");
            ((Content)toolFile.Items[4]).name = Trans.T("L_RUN_ON_PAUSE");
            ((Content)toolFile.Items[5]).name = Trans.T1("L_SCRIPT_X","1");
            ((Content)toolFile.Items[6]).name = Trans.T1("L_SCRIPT_X", "2");
            ((Content)toolFile.Items[7]).name = Trans.T1("L_SCRIPT_X", "3");
            ((Content)toolFile.Items[8]).name = Trans.T1("L_SCRIPT_X", "4");
            ((Content)toolFile.Items[9]).name = Trans.T1("L_SCRIPT_X", "5");
            tabPageHelp.Text = Trans.T("TAB_HELP");
            tabPageVisualization.Text = Trans.T("TAB_VISUALIZATION");
            radioShowAll.Text = Trans.T("L_SHOW_COMPLETE_CODE");
            radioShowLayerRange.Text = Trans.T("L_SHOW_LAYER_RANGE");
            radioShowSingleLayer.Text = Trans.T("L_SHOW_SINGLE_LAYER");
            buttonGoFirstLayer.Text = Trans.T("L_FIRST_LAYER");
            buttonGoLastLayer.Text = Trans.T("L_LAST_LAYER");
        }
        public int FileIndex
        {
            get
            {
                int i = toolFile.SelectedIndex;
                if (i == 0) return 1;
                if (i == 1) return 0;
                return i;
            }
        }
        protected void OnShowModeChanged(EventArgs e)
        {
            if (ShowModeChanged != null)
            {
                ShowModeChanged(this, e);
            }
        }
        protected void OnShowMinLayerChanged(EventArgs e)
        {
            if (ShowMinLayerChanged != null)
            {
                ShowMinLayerChanged(this, e);
            }
        }
        protected void OnShowMaxLayerChanged(EventArgs e)
        {
            if (ShowMaxLayerChanged != null)
            {
                ShowMaxLayerChanged(this, e);
            }
        }
        protected void OnMaxLayerChanged(EventArgs e)
        {
            if (MaxLayerChanged != null)
            {
                MaxLayerChanged(this, e);
            }
        }
        public int ShowMode
        {
            get { return _showMode; }
            set
            {
                if (value != _showMode)
                {
                    _showMode = value;
                    OnShowModeChanged(EventArgs.Empty);
                    if (contentChangedEvent != null)
                        contentChangedEvent();
                }
            }
        }
        public int ShowMinLayer
        {
            get { return _showMinLayer; }
            set
            {
                if (value < 0) value = 0;
                if (value > _maxLayer) value = MaxLayer;
                if (value != _showMinLayer)
                {
                    _showMinLayer = value;
                    sliderShowFirstLayer.Value = value;
                    OnShowMinLayerChanged(EventArgs.Empty);
                    if (_showMode == 1 || _showMinLayer > _showMaxLayer)
                    {
                        _showMaxLayer = value;
                        sliderShowMaxLayer.Value = value;
                        OnShowMaxLayerChanged(EventArgs.Empty);
                    }
                    if (_showMode != 0)
                        if (contentChangedEvent != null)
                            contentChangedEvent();
                }
            }
        }
        public int ShowMaxLayer
        {
            get { return _showMaxLayer; }
            set
            {
                if (value < 0) value = 0;
                if (value > _maxLayer) value = MaxLayer;
                if (value != _showMaxLayer)
                {
                    _showMaxLayer = value;
                    sliderShowMaxLayer.Value = value;
                    OnShowMaxLayerChanged(EventArgs.Empty);
                    if (_showMode == 1 || _showMaxLayer < _showMinLayer)
                    {
                        _showMinLayer = value;
                        sliderShowFirstLayer.Value = value;
                        OnShowMinLayerChanged(EventArgs.Empty);
                    }
                    if(_showMode!=0)
                        if(contentChangedEvent != null)
                            contentChangedEvent();
                }
            }
        }
        public int MaxLayer
        {
            get { return _maxLayer; }
            set
            {
                if (value != _maxLayer)
                {
                    _maxLayer = value;
                    OnMaxLayerChanged(EventArgs.Empty);
                    sliderShowFirstLayer.Maximum = _maxLayer;
                    sliderShowMaxLayer.Maximum = _maxLayer;
                    numericShowMinLayer.Maximum = _maxLayer;
                    numericShowMaxLayer.Maximum = _maxLayer;
                }
            }
        }
        private int MaxCol
        {
            get { return maxCol; }
            set {
                if (value == maxCol) return;
                maxCol = value;
                scrollColumns.Maximum = Math.Max(0, maxCol - colsVisible + 2);
            }
        }
        private int topCol
        {
            get { return _topCol; }
            set { _topCol = value; if (_topCol > scrollColumns.Maximum) scrollColumns.Maximum = _topCol;
                scrollColumns.Value = _topCol; 
            }
        }
        public void UpdateLayerInfo()
        {
            if (_row < 0 || _row >= lines.Count) return;
            GCodeShort s = lines[_row];
            toolLayer.Text = Trans.T1("L_LAYER_X",(!s.hasLayer ? "-" : s.layer.ToString()))+(s.emax>0?" "+Trans.T1("L_FILAMENT_POS",s.emax.ToString("0.0")):"");
            toolExtruder.Text = Trans.T1("L_EXTRUDER_X",(!s.hasLayer ? "-" : s.tool.ToString()));
        }
        private int col
        {
            get { return _col; }
            set
            {
                _col = value; 
                toolColumn.Text = Trans.T1("L_EDITOR_C",(col + 1).ToString());
            }
        }
        private int row
        {
            get { return _row; }
            set { _row = value; toolRow.Text = Trans.T1("L_EDITOR_R",(row + 1).ToString());
                UpdateLayerInfo(); if(Main.main.threedview!=null) Main.main.threedview.UpdateChanges(); }
        }
        private int topRow
        {
            get { return _topRow; }
            set
            {
                _topRow = value; if (_topRow >= lines.Count) _topRow = lines.Count - 1;
                if (_topRow < 0) _topRow = 0;
                scrollRows.Value = _topRow;
            }
        }
        private bool overwrite
        {
            get { return _overwrite; }
            set { _overwrite = value; toolMode.Text = value ? Trans.T("L_OVERWRITE") : Trans.T("L_INSERT"); CreateCursor(); }
        }
        public void AppendLine(string l)
        {
            lines.Add(new GCodeShort(l));
            MaxCol = Math.Max(maxCol, l.Length);
            scrollRows.Maximum = lines.Count;
        }
        /// <summary>
        /// Replaces current content with text. Resets undo cache.
        /// </summary>
        public override string Text
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (GCodeShort code in lines)
                    sb.AppendLine(code.text);
                return sb.ToString();
            }
            set
            {
                ignoreMouseDown = Control.MouseButtons != MouseButtons.None;
                cur.ClearUndo();
                string[] la = value.Replace("\r\n","\n").Replace('\r','\n').Split('\n');
                Clear();
                lines.Clear();
                
                maxCol = 1;
                if (la.Length == 0) la = new string[] { "" };
                foreach (string s in la)
                {
                    lines.Add(new GCodeShort(s));
                    MaxCol = Math.Max(maxCol, s.Length);
                }
                scrollRows.Maximum = lines.Count();
                row = col = topRow = topCol = selRow = selCol = 0;
                hasSel = true;
                editor.Focus();
                CreateCursor();
                PositionShowCursor(true, false);
                Changed();
            }
        }
        /// <summary>
        /// Sets focus on editor
        /// </summary>
        public void setFocus()
        {
            editor.Focus();
        }
        /// <summary>
        /// Clears active editor
        /// </summary>
        public void Clear()
        {
            lines.Clear();
            row = col = topRow = topCol = 0;
            hasSel = true;
            AppendLine("");
            PositionShowCursor(true, false);
            Changed();
        }
        private void DrawRow(Graphics g, int line, GCodeShort code, float x, float y)
        {
            string text = code.text;
            float s1 = 0, s2 = 0;
            g.FillRectangle(((line & 1)==0?backBrush:evenBackBrush), linesWidth, y, editor.Width - linesWidth, fontHeight);
            string ln = line.ToString();
            line--;
            int minc=-1, maxc=-1;
            if (line >= Math.Min(row, selRow) && line <= Math.Max(row, selRow))
            { // mark selection
                if(row<selRow || (row==selRow && col<selCol)) {
                    minc = col;maxc=selCol;
                } else {
                    minc = selCol;maxc=col;
                }
                if (line > Math.Min(row, selRow)) { minc = 0; s1 = linesWidth; }
                else s1 = linesWidth + minc * fontWidth;
                if (line < Math.Max(row, selRow)) { s2 = editor.Width; maxc = 10000; }
                else s2 = linesWidth + maxc * fontWidth;
                g.FillRectangle(selectionBrush, s1, y, s2 - s1, fontHeight);
            }
            string comment = "";
            string command = "";
            string parameter = "";
            float ps = linesWidth + x;
            int i,ac=0;
            if (text.StartsWith("@"))
            {
                for (i = 0; i < text.Length; i++)
                {
                    g.DrawString(text[i].ToString(), drawFont,(ac >=minc && ac<maxc ? selectionTextColor : hostBrush), ps, y);
                    ac++;
                    ps += fontWidth;
                }
                text = "";
            }
            int p = text.IndexOf(';');
            if (p != -1)
            {
                comment = text.Substring(p);
                text = text.Substring(0, p);
            }
            p = text.IndexOf(' ');
            if (p == -1)
            {
                command = text;
            }
            else
            {
                parameter = text.Substring(p);
                command = text.Substring(0, p);
            }
            if (command.Length > 0)
            {
                for (i = 0; i < command.Length; i++)
                {
                    g.DrawString(command[i].ToString(), drawFont, (ac >=minc && ac<maxc ? selectionTextColor :commandBrush), ps, y);
                    ps += fontWidth;
                    ac++;
                }
            }
            if (parameter.Length > 0)
            {
                for (i = 0; i < parameter.Length; i++)
                {
                    char c = parameter[i];
                    if((c>='A' && c<='Z') || (c>='a' && c<='z')) 
                        g.DrawString(c.ToString(), drawFont,(ac >=minc && ac<maxc ? selectionTextColor : paramTypeBrush), ps, y);
                    else
                        g.DrawString(c.ToString(), drawFont, (ac >=minc && ac<maxc ? selectionTextColor :blackBrush), ps, y);
                    ps += fontWidth;
                    ac++;
                }
            }
            if (comment.Length > 0)
            {
                for (i = 0; i < comment.Length; i++)
                {
                    g.DrawString(comment[i].ToString(), drawFont,(ac >=minc && ac<maxc ? selectionTextColor : commentBrush), ps, y);
                    ps += fontWidth;
                    ac++;
                }
            }
            if((code.layer & 1)==0)
                g.FillRectangle(linesBgColor, 0, y, linesWidth, fontHeight);
            else
                g.FillRectangle(linesBgAltColor, 0, y, linesWidth, fontHeight);
            g.DrawString(ln, drawFont, linesTextColor, linesWidth - 3 - fontWidth * ln.Length, y);
            PositionCursor();
        }
        private void CreateCursor()
        {
            if (!Main.IsMono)
            {
                CreateCaret(editor.Handle, 0, _overwrite ? (int)fontWidth : 1, (int)fontHeight);
                ShowCaret(editor.Handle);
            }
            PositionCursor();
        }
        private void HideCursor()
        {
            if (!Main.IsMono)
            {
                HideCaret(editor.Handle);
                DestroyCaret();
            }
            else editor.Invalidate();
        }
        private void PositionCursor()
        {
            UpdateHelp();
            if (!hasFocus) return;
            int x,y;
            x = (int)(linesWidth + (col - topCol) * fontWidth+1);
            y = (int)((row-topRow)*fontHeight);
            if (!Main.IsMono)
            {
                SetCaretPos(x, y);
            }
        }
        private void editor_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            SizeF sz = g.MeasureString("MMMMMMMMMM", drawFont);
            fontWidth = (int)(sz.Width/10)+1;
            fontHeight = (int)Math.Ceiling(sz.Height);
            string maxl = lines.Count.ToString();
            linesWidth = (int)(fontWidth * maxl.Length+6);
            rowsVisible = (int)Math.Ceiling((double)editor.Height / fontHeight);
            colsVisible = (int)Math.Ceiling((double)(editor.Width-linesWidth) / fontWidth);

            int r;
            int rmax = rowsVisible;
            if (topRow + rmax >= lines.Count)
                rmax = lines.Count - topRow;
            for (r = 0; r < rmax; r++)
            {
                DrawRow(g, topRow + r + 1, lines[topRow + r], -fontWidth*topCol, r * fontHeight);
            }
            if (Main.IsMono && blink && editor.Focused && _col>=topCol && _row>=topRow && _row<=topRow+rowsVisible)
            {
                int x, y;
                x = (int)(linesWidth + (col - topCol) * fontWidth + 1);
                y = (int)((row - topRow) * fontHeight);
                g.DrawLine(cursorBrush, x, y, x, y + fontHeight);
                if (_overwrite)
                {
                    g.DrawLine(cursorBrush,x, y + fontHeight,x+fontWidth,y+fontHeight);
                    g.DrawLine(cursorBrush, x + fontWidth, y + fontHeight,x+fontWidth, y);
                    g.DrawLine(cursorBrush, x + fontWidth, y,x, y);
                }
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
                col = lines[row].Length;
                PositionShowCursor();
            }
        }
        private void goLayer(int lay)
        {
            int line = 0;
            foreach(GCodeShort gc in cur.textArray) {
                if (gc.layer == lay) break;
                line++;
            }
            if (line < cur.textArray.Count)
            {
                row = line;
                col = 0;
            }
            else
            {
                row = cur.textArray.Count - 1;
                col = cur.textArray[row].text.Length;
            }
            PositionShowCursor();
            editor.Focus();
        }
        private void CursorPageUp()
        {
            if (topRow > 0)
            {
                topRow -= rowsVisible - 1;
                row -= rowsVisible - 1;
                if (topRow < 0) topRow = 0;
                if (row < 0) { row = 0; col = 0; }
                PositionShowCursor();
            }
            else
            {
                row = col = 0;
                PositionShowCursor();
            }
        }
        private void PositionShowCursor()
        {
            PositionShowCursor(false,true);
        }
        private void PositionShowCursor(bool repaint,bool moved)
        {
            scrollRows.Maximum = lines.Count();
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
            if (col < topCol)
            {
                topCol = col;
                repaint = true;
            }
            else if (col > topCol + colsVisible - 2)
            {
                topCol = col - colsVisible + 2;
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
                        selCol = col;
                        selRow = row;
                        hasSel = false;
                    }
                }
            }
            else
            {
                selCol = col;
                selRow = row;
                hasSel = false;
            }
            if (Main.IsMono)
            {
                blink = true;
                repaint = true;
            }
            if (repaint) { editor.Invalidate(); UpdateHelp(); }
            else PositionCursor();
        }
        private void CursorUp()
        {
            if (row >0)
            {
                row--;
                PositionShowCursor();
            }
        }
        private void CursorEnd()
        {
            col = lines[row].Length;
            PositionShowCursor();
        }
        private void CursorStart()
        {
            col = 0;
            PositionShowCursor();
        }
        private bool CursorLeft()
        {
            if (col > 0)
            {
                col--;
                PositionShowCursor();
            }
            else
            {
                if (row == 0) return false;
                CursorUp();
                CursorEnd();
            }
            return true;
        }
        private void CursorRight()
        {
            if (col <lines[row].Length)
            {
                col++;
                PositionShowCursor();
            }
            else
            {
                CursorDown();
                CursorStart();
            }
        }

        private void InsertChar(char c) {
            cur.AddUndo(new Undo(UndoAction.ReplaceSelection, c.ToString(), getSelection(), col, row, selCol, selRow));
            if (hasSelection)
                DeleteSelection(false);
            string l = lines[row].text;
            if(col>l.Length) col = l.Length;
            if(_overwrite && col < l.Length)
                lines[row] = new GCodeShort(l.Substring(0, col) + c.ToString() + l.Substring(col+1));
            else
                lines[row] = new GCodeShort(l.Substring(0,col)+c.ToString()+l.Substring(col));
            col++;
            PositionShowCursor(true, false);
            Changed();
        }
        private void InsertString(string s)
        {
            cur.AddUndo(new Undo(UndoAction.ReplaceSelection,s,getSelection(),col,row,selCol,selRow));
            if (hasSelection)
                DeleteSelection(false);
            s = s.Replace("\r\n", "\n");
            s = s.Replace('\r', '\n');
            string[] la = s.Split('\n');
            string l = lines[row].text;
            if (col > l.Length) col = l.Length;
            la[0] = l.Substring(0, col) + la[0];
            int nc = la[la.Length - 1].Length;
            la[la.Length - 1] = la[la.Length - 1] + l.Substring(col);
            col = nc;
            lines[row] = new GCodeShort(la[0]);
            string[] la2 = new string[la.Length - 1];
            for (int i = 1; i < la.Length; i++)
            {
                la2[i - 1] = la[i];
            }
            for (int i = 0; i < la2.Length; i++)
                lines.Insert(row + 1 + i, new GCodeShort(la2[i]));
            row += la.Length - 1;
            PositionShowCursor(true,false);
            Changed();
        }
        public string getSelection()
        {
            int rstart = row;
            int cstart = col;
            int rend = selRow;
            int cend = selCol;
            if (row > selRow || (row == selRow && col > selCol))
            {
                rstart = selRow;
                cstart = selCol;
                rend = row;
                cend = col;
            }
            cstart = Math.Min(cstart, lines[rstart].Length);
            cend = Math.Min(cend, lines[rend].Length);
            int i;
            StringBuilder sb = new StringBuilder();
            for (i = rstart; i <= rend; i++)
            {
                string l = lines[i].text;
                if (i == rend)
                {
                    cend = Math.Min(cend, l.Length);
                    l = l.Substring(0, cend);
                }
                else l += "\r\n";
                if (i == rstart)
                {
                    cstart = Math.Min(l.Length, cstart);
                    l = l.Substring(cstart);
                }
                sb.Append(l);
            }
            return sb.ToString();
        }
        public void DeleteSelection(bool redraw)
        {
            int rstart = row;
            int cstart = col;
            int rend = selRow;
            int cend = selCol;
            if (row > selRow || (row == selRow && col > selCol))
            {
                rstart = selRow;
                cstart = selCol;
                rend = row;
                cend = col;
            }
            cstart = Math.Min(cstart, lines[rstart].Length);
            cend = Math.Min(cend, lines[rend].Length);
            cur.AddUndo(new Undo(UndoAction.ReplaceSelection, "", getSelection(), col, row, selCol, selRow));
            // start row = begin first + end last row
            lines[rstart] = new GCodeShort(lines[rstart].text.Substring(0, cstart) + lines[rend].text.Substring(cend));
            if(rend>rstart)
                lines.RemoveRange(rstart + 1, rend - rstart);
            row = selRow = rstart;
            col = selCol = cstart;
            if (lines.Count == 0) Clear();
            if (redraw)
                PositionShowCursor(true, false);
            Changed();
        }
        public void DeleteChar()
        {
            string t = lines[row].text;
            if (t.Length == col)
            { // Join with next line
                if (row == lines.Count - 1) return;
                lines[row] = new GCodeShort(lines[row].text+lines[row + 1].text);
                lines.RemoveAt(row + 1);
                cur.AddUndo(new Undo(UndoAction.ReplaceSelection,"","\n",col,row,0,row+1));
            }
            else
            {
                cur.AddUndo(new Undo(UndoAction.ReplaceSelection,"",t.Substring(col,1),col,row,col+1,row));
                lines[row] = new GCodeShort(t.Substring(0, col) + t.Substring(col + 1));
            }
            editor.Invalidate();
            Changed();
        }
        public void Backspace()
        {
            string t = lines[row].text;
            if (col > t.Length)
            {
                col = t.Length;
            } else
            if (col==0)
            { // Join with next line
                if (row == 0) return;
                cur.AddUndo(new Undo(UndoAction.ReplaceSelection, "", "\n", col, row, lines[row-1].Length, row -1));
                col = lines[row-1].Length;
                lines[row - 1] = new GCodeShort(lines[row - 1].text+lines[row].text);
                lines.RemoveAt(row);
                row--;
            }
            else
            {
                cur.AddUndo(new Undo(UndoAction.ReplaceSelection, "", t.Substring(col-1, 1), col, row, col -1, row));
                lines[row] = new GCodeShort(t.Substring(0, col-1) + t.Substring(col ));
                CursorLeft();
            }
            PositionShowCursor(true, false);
            Changed();
        }
        public bool hasSelection
        {
            get {return row!=selRow || col!=selCol;}
        }
        private void editor_Click(object sender, EventArgs e)
        {
            if(!editor.Focused)
                editor.Focus();
        }

        private void RepetierEditor_KeyDown(object sender, KeyEventArgs e)
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
                case Keys.Left:
                    CursorLeft();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Right:
                    CursorRight();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.End:
                    CursorEnd();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Home:
                    CursorStart();
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
                case Keys.Return:
                    InsertString("\n");
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Delete:
                    if (hasSelection)
                    {
                        DeleteSelection(true);
                    }
                    else
                    {
                        DeleteChar();
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Back:
                    if (hasSelection)
                    {
                        DeleteSelection(true);
                    }
                    else
                    {
                        Backspace();
                    }
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Insert:
                    overwrite = !overwrite;
                    break;
                case Keys.A:
                    if (e.Control)
                    {
                        selCol = 0;
                        selRow = 0;
                        row = Math.Max(0, lines.Count - 1);
                        col = lines[row].Length;
                        forceSel = true;
                        PositionShowCursor(true,true);
                        forceSel = false;
                        hasSel = true;
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;
                case Keys.C:
                    if (e.Control)
                    {
                        if(hasSelection)
                            Clipboard.SetText(getSelection());
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;
                case Keys.V:
                    if(e.Control) {
                        InsertString(Clipboard.GetText());
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;
                case Keys.X:
                    if (e.Control)
                    {
                        if (hasSelection)
                        {
                            Clipboard.SetText(getSelection());
                            DeleteSelection(true);
                        }
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;
                case Keys.Z:
                    if (e.Control)
                    {
                        cur.Undo();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;
                case Keys.Y:
                    if (e.Control)
                    {
                        cur.Redo();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;
            }
            //e.Handled = true;
            //e.SuppressKeyPress = true;
        }

        private void scrollRows_ValueChanged(object sender, EventArgs e)
        {
            topRow = scrollRows.Value;
            editor.Invalidate();
        }
        private void MouseWheelHandler(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                if (e.Delta > 0)
                    topRow -= 5;
                else
                    topRow += 5;
                editor.Invalidate();
            }
        }
        private void scrollColumns_ValueChanged(object sender, EventArgs e)
        {
            topCol = scrollColumns.Value;
            editor.Invalidate();
        }

        private void toolNew_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void RepetierEditor_Enter(object sender, EventArgs e)
        {
            hasFocus = true;
            CreateCursor();
        }

        private void RepetierEditor_Leave(object sender, EventArgs e)
        {
            hasFocus = false;
            HideCursor();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            blink = !blink;
            if(changedCounter>0) {
                changedCounter--;
                if(changedCounter==0 && contentChangedEvent!=null)
                    contentChangedEvent();
            }
            if(Main.IsMono && editor.Focused && _col>=topCol && _row>=topRow && _row<=topRow+rowsVisible)
                editor.Invalidate();
        }

        private void RepetierEditor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= 32)
            {
                InsertChar(e.KeyChar);
            }
            e.Handled = true;
           
        }

        private void splitContainer_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void editor_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void editor_MouseDown(object sender, MouseEventArgs e)
        {
            if (ignoreMouseDown) return;
            Focus();
            //CreateCursor();
            if (Control.ModifierKeys == Keys.Shift)
            {
                row = Math.Max(0,Math.Min(lines.Count-1,topRow + (int)(e.Y / fontHeight)));
                col = Math.Max(topCol,Math.Min(lines[row].Length,topCol + (int)(Math.Round((e.X - linesWidth) / fontWidth))));
            }
            else
            {
                row = selRow = Math.Max(0, Math.Min(lines.Count - 1, topRow + (int)(e.Y / fontHeight)));
                col = selCol = Math.Max(topCol, Math.Min(lines[row].Length, topCol + (int)(Math.Round((e.X - linesWidth) / fontWidth))));
            }
            PositionCursor();    
           
        }

        private void editor_MouseMove(object sender, MouseEventArgs e)
        {
            if (ignoreMouseDown) return;
            if (e.Button == MouseButtons.Left)
            {
                row = Math.Max(0, Math.Min(lines.Count - 1, topRow + (int)(e.Y / fontHeight)));
                col = Math.Max(topCol, Math.Min(lines[row].Length, topCol + (int)(Math.Round((e.X - linesWidth) / fontWidth))));
                if (row < topRow + 3 && topRow > 0) topRow--;
                if (row > topRow - 4 + rowsVisible && topRow + rowsVisible - 3 < lines.Count) topRow++;
                hasSel = true;
                editor.Invalidate();
                PositionCursor();
            }
        }
        public void Changed() {
            changedCounter = 4;
        }
        private void editor_SizeChanged(object sender, EventArgs e)
        {
            scrollColumns.Maximum = Math.Max(0, maxCol - colsVisible + 2);
        }

        private void toolPaste_Click(object sender, EventArgs e)
        {
            InsertString(Clipboard.GetText());
        }

        private void toolCut_Click(object sender, EventArgs e)
        {
            if (hasSelection)
            {
                Clipboard.SetText(getSelection());
                DeleteSelection(true);
            }
        }

        private void toolCopy_Click(object sender, EventArgs e)
        {
            if(hasSelection)
                Clipboard.SetText(getSelection());
        }

        private void toolFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cur != null)
                cur.FromActive();
            cur = (Content)toolFile.SelectedItem;
            cur.ToActive();
            editor.Focus();
        }

        private void toolUndo_Click(object sender, EventArgs e)
        {
            cur.Undo();
        }

        private void toolRedo_Click(object sender, EventArgs e)
        {
            cur.Redo();
        }

        private void toolSave_Click(object sender, EventArgs e)
        {
            if (cur.etype == 1)
            {
                Main.printerSettings.currentPrinterKey.SetValue("gcodePrepend", Text);
            }
            else if (cur.etype == 2)
            {
                Main.printerSettings.currentPrinterKey.SetValue("gcodeAppend", Text);
            }
            else if (cur.etype == 3)
            {
                Main.printerSettings.currentPrinterKey.SetValue("gcodeKill", Text);
            }
            else if (cur.etype == 4)
            {
                Main.printerSettings.currentPrinterKey.SetValue("gcodePause", Text);
            }
            else if (cur.etype == 5)
            {
                Main.printerSettings.currentPrinterKey.SetValue("gcodeScript1", Text);
            }
            else if (cur.etype == 6)
            {
                Main.printerSettings.currentPrinterKey.SetValue("gcodeScript2", Text);
            }
            else if (cur.etype == 7)
            {
                Main.printerSettings.currentPrinterKey.SetValue("gcodeScript3", Text);
            }
            else if (cur.etype == 8)
            {
                Main.printerSettings.currentPrinterKey.SetValue("gcodeScript4", Text);
            }
            else if (cur.etype == 9)
            {
                Main.printerSettings.currentPrinterKey.SetValue("gcodeScript5", Text);
            }
            else
            {
                if (Main.main.lastFileLoadedName != null && Main.main.lastFileLoadedName.Length > 0)
                {
                    // Propose default name
                    Main.main.saveJobDialog.FileName = Main.main.lastFileLoadedName + ".gcode";
                }
                if (Main.main.saveJobDialog.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(Main.main.saveJobDialog.FileName, Text, Encoding.Default);
                }
            }
        }
        public void UpdatePrependAppend()
        {
            Content c = (Content)toolFile.Items[1];
            c.Text = (string)Main.printerSettings.currentPrinterKey.GetValue("gcodePrepend", getContent(1));
            c.ResetPos();
            if (c == cur) c.ToActive();
            c = (Content)toolFile.Items[2];
            c.Text = (string)Main.printerSettings.currentPrinterKey.GetValue("gcodeAppend", getContent(2));
            c.ResetPos();
            c = (Content)toolFile.Items[3];
            c.Text = (string)Main.printerSettings.currentPrinterKey.GetValue("gcodeKill", getContent(3));
            c.ResetPos();
            c = (Content)toolFile.Items[4];
            c.Text = (string)Main.printerSettings.currentPrinterKey.GetValue("gcodePause", getContent(4));
            c.ResetPos();
            c = (Content)toolFile.Items[5];
            c.Text = (string)Main.printerSettings.currentPrinterKey.GetValue("gcodeScript1", getContent(5));
            c.ResetPos();
            c = (Content)toolFile.Items[6];
            c.Text = (string)Main.printerSettings.currentPrinterKey.GetValue("gcodeScript2", getContent(6));
            c.ResetPos();
            c = (Content)toolFile.Items[7];
            c.Text = (string)Main.printerSettings.currentPrinterKey.GetValue("gcodeScript3", getContent(7));
            c.ResetPos();
            c = (Content)toolFile.Items[8];
            c.Text = (string)Main.printerSettings.currentPrinterKey.GetValue("gcodeScript4", getContent(8));
            c.ResetPos();
            c = (Content)toolFile.Items[9];
            c.Text = (string)Main.printerSettings.currentPrinterKey.GetValue("gcodeScript5", getContent(9));
            c.ResetPos();
            if (c == cur) c.ToActive();
        }
        public List<GCodeShort> getContentArray(int idx)
        {
            Content c = (Content)toolFile.Items[idx];
            return c.textArray;
        }
        public string getContent(int idx)
        {
            Content c = (Content)toolFile.Items[idx];
            StringBuilder sb = new StringBuilder();
            foreach (GCodeShort code in c.textArray)
                sb.AppendLine(code.text);
            return sb.ToString();
        }
        public void fastLayerUpdate()
        {
            GCodeAnalyzer a = new GCodeAnalyzer(true);
            foreach (GCodeShort code in getContentArray(1))
                a.analyzeShort(code);
            foreach (GCodeShort code in getContentArray(0))
                a.analyzeShort(code);
            foreach (GCodeShort code in getContentArray(2))
                a.analyzeShort(code);
            MaxLayer = a.layer;
            if (a.printingTime > 0)
            {
                printingTime = a.printingTime;
                int sec = (int)(printingTime*(1+0.01*Main.conn.addPrintingTime));
                int hours = sec / 3600;
                sec -= 3600 * hours;
                int min = sec/60;
                sec-=min*60;
                StringBuilder s = new StringBuilder();
                if (hours > 0)
                    s.Append(Trans.T1("L_TIME_H:", hours.ToString())); //"h:");
                if (min > 0)
                    s.Append(Trans.T1("L_TIME_M:", min.ToString()));
                s.Append(Trans.T1("L_TIME_S", sec.ToString()));
                toolPrintingTime.Text = Trans.T1("L_PRINTING_TIME:",s.ToString());
            }
        }
        public void setContent(int idx,string text)
        {
            Content c = (Content)toolFile.Items[idx];
            if (c == cur)
            {
                Text = text;
            }
            else
            {
                c.Text = text;
            }
            fastLayerUpdate();
        }
        public void selectContent(int idx)
        {
            toolFile.SelectedIndex = idx;
        }
        public bool autopreview
        {
            get { return toolPreview.Checked; }
            set { toolPreview.Checked = value; }
        }

        private void editor_MouseUp(object sender, MouseEventArgs e)
        {
            ignoreMouseDown = false;
        }

        private void toolPreview_CheckedChanged(object sender, EventArgs e)
        {
            if (toolPreview.Checked && contentChangedEvent != null) // Update view
                contentChangedEvent();
        }

        private void editor_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void editor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }
        string lastHelpCommand = "";
        public void UpdateHelp()
        {
            if (commands == null) return;
            string l = lines[row].text.Trim();
            int p = l.IndexOf(' ');
            if (p == -1)
            {
                p = l.Length;
            }
            string com = l.Substring(0, p);
            if (com.Equals(lastHelpCommand)) return;
            lastHelpCommand = com;
            CommandDescription desc = null;
            if (commands.commands.ContainsKey(com))
                desc = commands.commands[com];
            if (desc == null)
            {
                help.Rtf = "";
                return;
            }
            StringBuilder s = new StringBuilder();
            s.Append("{\\rtf1\\ansi \\b ");
            s.Append(desc.command);
            s.Append(":");
            s.Append(desc.title);
            s.Append("\\b0 \\line "+Trans.T("L_SYNTAX")+":");
            s.Append(desc.command);
            s.Append(" ");
            foreach (CommandParameter pa in desc.parameter)
                s.Append(pa);
            s.Append("\\line ");
            s.Append(desc.description);
            s.Append("}");
            help.Rtf = s.ToString();
            //help.Rtf = @"{\rtf1\ansi This text is in \b bold\b0, " +
//@"this is in \i italics\i0, " +
//@"and this is \ul underlined\ul0.}";

            return;

        }

        private void toolPreview_Click(object sender, EventArgs e)
        {
                toolPreview.Checked = !toolPreview.Checked;
        }

        private void editor_MouseEnter(object sender, EventArgs e)
        {
            if (Control.MouseButtons == MouseButtons.None)
                ignoreMouseDown = false;
        }

        private void editor_MouseLeave(object sender, EventArgs e)
        {
            ignoreMouseDown = true;
        }

        private void radioShowMode_Click(object sender, EventArgs e)
        {
            ShowMode = int.Parse((string)((RadioButton)sender).Tag);
        }

        private void sliderShowFirstLayer_ValueChanged(object sender, EventArgs e)
        {
            ShowMinLayer = sliderShowFirstLayer.Value;
        }

        private void sliderShowMaxLayer_ValueChanged(object sender, EventArgs e)
        {
            ShowMaxLayer = sliderShowMaxLayer.Value;
        }

        private void numericShowMinLayer_KeyPress(object sender, KeyPressEventArgs e)
        {
            ShowMinLayer = (int)numericShowMinLayer.Value;
        }

        private void numericShowMinLayer_ValueChanged(object sender, EventArgs e)
        {
            ShowMinLayer = (int)numericShowMinLayer.Value;
        }

        private void numericShowMaxLayer_ValueChanged(object sender, EventArgs e)
        {
            ShowMaxLayer = (int)numericShowMaxLayer.Value;
        }

        private void buttonGoFirstLayer_Click(object sender, EventArgs e)
        {
            goLayer(ShowMinLayer);
        }

        private void buttonGoLastLayer_Click(object sender, EventArgs e)
        {
            goLayer(ShowMaxLayer + 1);
        }
    }
}
