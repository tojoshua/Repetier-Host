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
using Microsoft.Win32;

namespace RepetierHost.view
{
    public partial class LogView : UserControl
    {
        public Color errorColor = Color.FromArgb(238, 198, 198);
        public Color warningColor = Color.FromArgb(223,175,83);
        public Color infoColor = Color.FromArgb(140, 179, 251);
        RegistryKey key;
        public LogView()
        {
            InitializeComponent();
            key = Custom.BaseKey; // Registry.CurrentUser.CreateSubKey("SOFTWARE\\Repetier");
            RegToForm();
            if (Main.main != null)
            {
                Main.main.languageChanged += translate;
                translate();
            }
        }
        static bool readingReg = false;
        public void translate()
        {
            labelShowInLog.Text = Trans.T("L_SHOW_IN_LOG");
            switchACK.TextOff = switchACK.TextOn = Trans.T("B_ACK");
            switchAutoscroll.TextOff = switchAutoscroll.TextOn = Trans.T("B_AUTO_SCROLL");
            switchCommandsSend.TextOff = switchCommandsSend.TextOn = Trans.T("B_COMMANDS");
            switchErrors.TextOff = switchErrors.TextOn = Trans.T("B_LOG_ERRORS");
            switchInfo.TextOff = switchInfo.TextOn = Trans.T("B_LOG_INFO");
            switchWarnings.TextOff = switchWarnings.TextOn = Trans.T("B_LOG_WARNINGS");
            buttonCopy.Text = Trans.T("B_COPY");
            buttonClearLog.Text = Trans.T("B_CLEAR_LOG");
        }
        public void FormToReg()
        {
            if (readingReg) return;
            key.SetValue("logSend", switchCommandsSend.On ? 1 : 0);
            key.SetValue("logErrors", switchErrors.On ? 1 : 0);
            key.SetValue("logWarning", switchWarnings.On ? 1 : 0);
            key.SetValue("logACK", switchACK.On ? 1 : 0);
            key.SetValue("logInfo", switchInfo.On ? 1 : 0);
            key.SetValue("logAutoscroll", switchAutoscroll.On ? 1 : 0);

        }
        public void RegToForm()
        {
            readingReg = true;
            switchCommandsSend.On = 1 == (int)key.GetValue("logSend", switchCommandsSend.On ? 1 : 0);
            switchErrors.On = 1 == (int)key.GetValue("logErrors", switchErrors.On ? 1 : 0);
            switchWarnings.On = 1 == (int)key.GetValue("logWarning", switchWarnings.On ? 1 : 0);
            switchACK.On = 1 == (int)key.GetValue("logACK", switchACK.On ? 1 : 0);
            switchInfo.On = 1 == (int)key.GetValue("logInfo", switchInfo.On ? 1 : 0);
            switchAutoscroll.On = 1 == (int)key.GetValue("logAutoscroll", switchAutoscroll.On ? 1 : 0);
            readingReg = false;
        }
        private void filter()
        {
            listLog.Clear();
            lock (Main.conn.logList)
            {
                foreach (LogLine l in Main.conn.logList)
                {
                    logAppend(l);
                }
            }
            if (switchAutoscroll.On)
            {
                listLog.ScrollBottom();
            }
            listLog.UpdateBox();
        }
        private void logUpdate(LogLine line)
        {
            logAppend(line);
            listLog.UpdateBox();
        }
        private bool isAck(string t)
        {
            if (t.StartsWith("ok") || t.StartsWith("wait")) return true;
            if (t.IndexOf("SD printing byte")!=-1) return true;
            if (t.IndexOf("Not SD printing")!=-1) return true;
            if (t.IndexOf("SpeedMultiply:")!=-1) return true;
            if (t.IndexOf("FlowMultiply:") != -1) return true;
            if (t.IndexOf("TargetExtr0:") != -1) return true;
            if (t.IndexOf("TargetExtr1:")!=-1) return true;
            if (t.IndexOf("TargetBed:")!=-1) return true;
            if (t.IndexOf("Fanspeed:")!=-1) return true;
            return false;
        }
        private void UpdateNewEntries(object sender, EventArgs e)
        {
            LinkedList<LogLine> nl = null;
            lock (Main.conn.newLogs)
            {
                LinkedList<LogLine> l = Main.conn.newLogs;
                if (l.Count == 0) return;
                nl = new LinkedList<LogLine>(l);
                l.Clear();
            }
            while (nl.Count > 0)
            {
                LogLine line = nl.First.Value;
                if (switchACK.On == false && isAck(line.text)) nl.RemoveFirst();
                else if (line.level == 0 && line.response==false && switchCommandsSend.On == false) nl.RemoveFirst();
                else if (line.level == 1 && switchWarnings.On == false) nl.RemoveFirst();
                else if (line.level == 2 && switchErrors.On == false) nl.RemoveFirst();
                else if (line.level == 3 && switchInfo.On == false) nl.RemoveFirst();
                else break;
            }
            if (nl.Count == 0) return;
            foreach (LogLine line in nl)
                logAppend(line);
            listLog.UpdateBox();
        }
        private void logAppend(LogLine line)
        {
            if (switchACK.On == false && isAck(line.text)) return;
            if (line.level == 0 && line.response==false && switchCommandsSend.On == false) return;
            if (line.level == 1 && switchWarnings.On == false) return;
            if (line.level == 2 && switchErrors.On == false) return;
            if (line.level == 3 && switchInfo.On == false) return;
            listLog.Add(line);
        }

        private void listLog_Resize(object sender, EventArgs e)
        {
        }

        private void toolClear_Click(object sender, EventArgs e)
        {
            Main.conn.clearLog();
        }


        private void toolCopy_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            string sel = listLog.getSelection();
            if(sel.Length>0)
                Clipboard.SetText(sel);
        }

        private void LogView_Load(object sender, EventArgs e)
        {
            Main.conn.eventLogCleared += filter;
            Main.conn.eventLogUpdate += logUpdate;
            Application.Idle += new EventHandler(UpdateNewEntries);
        }


        private void switchCommandsSend_OnChange(SwitchButton button)
        {
            filter();
            FormToReg();
        }

        private void switchInfo_OnChange(SwitchButton button)
        {
            filter();
            FormToReg();
        }

        private void switchWarnings_OnChange(SwitchButton button)
        {
            filter();
            FormToReg();
        }

        private void switchErrors_OnChange(SwitchButton button)
        {
            filter();
            FormToReg();
        }

        private void switchACK_OnChange(SwitchButton button)
        {
            filter();
            FormToReg();
        }

        private void switchAutoscroll_OnChange(SwitchButton button)
        {
            listLog.Autoscroll = switchAutoscroll.On;
            FormToReg();
        }
    }
}
