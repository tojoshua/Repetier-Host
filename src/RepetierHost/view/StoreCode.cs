/*
   Copyright 2012 repetier repetierdev@gmail.com

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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using RepetierHost.model;

namespace RepetierHost.view
{
    public partial class StoreCode : Form
    {
        private static StoreCode form = null;
        public static void Execute()
        {
            if (form == null)
            {
                form = new StoreCode();
            }
            form.ShowDialog();
        }
        public StoreCode()
        {
            InitializeComponent();
            translate();
            Main.main.languageChanged += translate;
        }
        private void translate()
        {
            Text = Trans.T("W_SAVE_GCODE_DIRECT_PRINT");
            labelSaveDirectInfo.Text = Trans.T("L_SAVE_DIRECT_INFO");
            checkBinary.Text = Trans.T("L_SAVE_AS_BINARY");
            checkIncludeJobFinished.Text = Trans.T("L_INCLUDE_JOB_FINISHED_COMMANDS");
            checkIncludeStartEnd.Text = Trans.T("L_INCLUDE_START_END_CODE");
            buttonCancel.Text = Trans.T("B_CANCEL");
            buttonSave.Text = Trans.T("B_SAVE");
        }
        private void buttonSave_Click(object sender, EventArgs e)
        {
            Close();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                saveJob(saveFileDialog.FileName);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
            //Hide();
        }
        private void writeArray(BinaryWriter file, List<GCodeShort> list, bool binary)
        {
            foreach (GCodeShort code in list)
            {
                GCode gc = new GCode();
                gc.Parse(code.text);
                if (gc.hostCommand) continue;
                if (binary)
                {
                    if (gc.hasCode)
                    {
                        byte[] data = gc.getBinary(1);
                        file.Write(data);
                    }
                }
                else
                {
                    System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                    string cmd = gc.getAscii(false, false);
                    if (cmd.Length > 0)
                        file.Write(enc.GetBytes(cmd + "\n"));
                }
            }
        }
        private void writeString(BinaryWriter file, string code, bool binary)
        {
            GCode gc = new GCode();
            gc.Parse(code);
            if (gc.hostCommand) return;
            if (binary)
            {
                if (gc.hasCode)
                {
                    byte[] data = gc.getBinary(1);
                    file.Write(data);
                }
            }
            else
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                string cmd = gc.getAscii(false, false);
                if (cmd.Length > 0)
                    file.Write(enc.GetBytes(cmd + "\n"));
            }
        }
        public void saveJob(string name)
        {
            bool binary = checkBinary.Checked;
            bool startend = checkIncludeStartEnd.Checked;
            System.IO.BinaryWriter file = new System.IO.BinaryWriter(File.Open(name, FileMode.Create));
            if (startend)
                writeArray(file, Main.main.editor.getContentArray(1), binary);
            writeArray(file, Main.main.editor.getContentArray(0), binary);
            if (startend)
                writeArray(file, Main.main.editor.getContentArray(2), binary);
            if (checkIncludeJobFinished.Checked)
            {
                PrinterConnection con = Main.conn;
                if (con.afterJobDisableExtruder)
                {
                    for (int i = 0; i < Main.conn.numberExtruder; i++)
                        writeString(file, "M104 S0 T"+i.ToString(), binary);
                }
                if (con.afterJobDisablePrintbed)
                    writeString(file, "M140 S0", binary);
                if (con.afterJobGoDispose)
                {
                    writeString(file, "G90", binary);
                    writeString(file, "G1 X" + con.disposeX.ToString(GCode.format) + " Y" + con.disposeY.ToString(GCode.format) + " F" + con.travelFeedRate.ToString(GCode.format), binary);
                }
                if (con.afterJobDisableMotors)
                    writeString(file,"M84",binary);

            }
            file.Close();
        }
    }
}
