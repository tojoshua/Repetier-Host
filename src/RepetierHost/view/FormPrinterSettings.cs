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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using Microsoft.Win32;
using RepetierHost.model;
using RepetierHost.view.utils;
using System.Globalization;
using System.IO;
using RepetierHost.connector;

namespace RepetierHost.view
{
    public delegate void PrinterChanged(RegistryKey printerKey,bool printerChanged);
    public partial class FormPrinterSettings : Form
    {
        public static FormPrinterSettings ps = null;
        public event PrinterChanged eventPrinterChanged;
        public RegistryKey repetierKey;
        public RegistryKey printerKey;
        public RegistryKey currentPrinterKey;
        public PrinterConnection con;
        public float PrintAreaWidth;
        public float PrintAreaDepth;
        public float PrintAreaHeight;
        public float XMin, XMax, YMin, YMax, BedLeft, BedFront;
        //public bool HasDumpArea;
        public float DumpAreaLeft;
        public float DumpAreaFront;
        public float DumpAreaWidth;
        public float DumpAreaDepth;
        public int printerType;
        public float rostockHeight;
        public float rostockRadius;
        public float cncZTop;
        public List<PrinterConnectorBase> connectors = new List<PrinterConnectorBase>();
        int xhomeMode = 0, yhomeMode = 0, zhomemode = 0;
        UserControl connectorPanel = null;

        public FormPrinterSettings()
        {
            ps = this;
            connectors.Add(new SerialConnector());
            connectors.Add(new VirtualPrinterConnector());
            InitializeComponent();
            RegMemory.RestoreWindowPos("printerSettingsWindow", this);
            repetierKey = Custom.BaseKey; // Registry.CurrentUser.CreateSubKey("SOFTWARE\\Repetier");
            printerKey = repetierKey.CreateSubKey("printer");
            con = Main.conn;
            conToForm();
            comboPrinter.Items.Clear();
            bindingConnectors.DataSource = connectors;
            comboConnector.DataSource = bindingConnectors.DataSource;
            comboConnector.DisplayMember = "Name";
            comboConnector.ValueMember = "Id";
            foreach (string s in printerKey.GetSubKeyNames())
                comboPrinter.Items.Add(s);
            con.printerName = (string)repetierKey.GetValue("currentPrinter", "default");
            load(con.printerName);
            formToCon();
            UpdateDimensions();
           /* if (Custom.GetBool("noDisposeArea", false))
            {
                labelDumpAreaDepth.Visible = false;
                labelDumpAreaFront.Visible = false;
                labelDumpAreaLeft.Visible = false;
                labelDumpAreaWidth.Visible = false;
                labelDumpUnit1.Visible = false;
                labelDumpUnit2.Visible = false;
                labelDumpUnit3.Visible = false;
                labelDumpUnit4.Visible = false;
                checkHasDumpArea.Visible = false;
                textDumpAreaDepth.Visible = false;
                textDumpAreaFront.Visible = false;
                textDumpAreaLeft.Visible = false;
                textDumpAreaWidth.Visible = false;
            }*/
            Main.main.languageChanged += translate;
            translate();
        }
        public void translate()
        {
            labelAddPrintingTime.Text = Trans.T("L_ADD_PRINTING_TIME");
            labelCheckEveryX.Text = Trans.T("L_CHECK_EVERY_X");
            labelDefExtruderTemp.Text = Trans.T("L_DEFAULT_EXTRUDER_TEMPERATURE");
            labelDefHeatedBedTemp.Text = Trans.T("L_DEFAULT_HEATED_BED_TEMPERATURE");
            labelDumpAreaDepth.Text = Trans.T("L_DUMP_AREA_DEPTH");
            labelDumpAreaFront.Text = Trans.T("L_DUMP_AREA_FRONT");
            labelDumpAreaLeft.Text = Trans.T("L_DUMP_AREA_LEFT");
            labelDumpAreaWidth.Text = Trans.T("L_DUMP_AREA_WIDTH");
            labelFilterInfo.Text = Trans.T("L_FILTER_INFO");
            labelFilterPathParam.Text = Trans.T("L_FILTER_PATH_PARAM");
            labelParkPosition.Text = Trans.T("L_PARK_POSITION");
            labelPrintAreaDepth.Text = Trans.T("L_PRINT_AREA_DEPTH");
            labelPrintAreaHeight.Text = Trans.T("L_PRINT_AREA_HEIGHT");
            labelPrintAreaWidth.Text = Trans.T("L_PRINT_AREA_WIDTH");
            labelPrinter.Text = Trans.T("L_PRINTER");
            labelTravelFeedRate.Text = Trans.T("L_TRAVEL_FEED_RATE");
            labelZFeedRate.Text = Trans.T("L_ZFEED_RATE");
            checkDisableExtruderAfterJob.Text = Trans.T("L_DISABLE_EXTRUDER_AFTER_JOB");
            checkDisableMotors.Text = Trans.T("L_DISABLE_MOTORS");
            checkDisbaleHeatedBedAfterJob.Text = Trans.T("L_DISABLE_HEATED_BED_AFTER_JOB");
            checkGoDisposeAfterJob.Text = Trans.T("L_GO_PARK_POSITION");
            //checkHasDumpArea.Text = Trans.T("L_HAS_DUMP_AREA");
            checkRunFilterEverySlice.Text = Trans.T("L_RUN_FILTER_EVERY_SLICE");
            labelCheckEveryX.Text = Trans.T1("L_CHECK_EVERY_X", trackTempPeriod.Value.ToString());
            checkTemp.Text = Trans.T("L_CHECK_EXTRUDER_BED_TEMPERATURE");
            logM105Checkbox.Text = Trans.T("L_REMOVE_M105_REQUEST_LOG");
            buttonAbort.Text = Trans.T("B_CANCEL");
            buttonApply.Text = Trans.T("B_APPLY");
            buttonOK.Text = Trans.T("B_OK");
            //buttonDelete.Text = Trans.T("B_DELETE_PRINTER_SETTINGS"); // is now an icon!
            tabPageConnection.Text = Trans.T("TAB_CONNECTION");
            tabPagePrinter.Text = Trans.T("TAB_PRINTER");
            tabPageShape.Text = Trans.T("TAB_PRINTER_SHAPE");
            tabAdvanced.Text = Trans.T("TAB_ADVANCED");
            this.Text = Trans.T("W_PRINTER_SETTINGS");
            groupBoxPostSliceFilter.Text = Trans.T("L_POST_SLICE_FILTER");
            labelZMin.Text = Trans.T("L_Z_MIN");
            labelXMax.Text = Trans.T("L_X_MAX:");
            labelYMin.Text = Trans.T("L_Y_MIN:");
            labelYMax.Text = Trans.T("L_Y_MAX:");
            labelBedLeft.Text = Trans.T("L_BED_LEFT:");
            labelBedFront.Text = Trans.T("L_BED_FRONT:");
            labelXMin.Text = Trans.T("L_X_MIN:");
            labelShapeInfo.Text = Trans.T("L_SHAPE_INFO");
            labelHomeX.Text = Trans.T("L_HOME_X:");
            labelHomeY.Text = Trans.T("L_HOME_Y:");
            labelHomeZ.Text = Trans.T("L_HOME_Z:");
            comboHomeX.Items[0] = Trans.T("L_MIN");
            comboHomeX.Items[1] = Trans.T("L_MAX");
            comboHomeY.Items[0] = Trans.T("L_MIN");
            comboHomeY.Items[1] = Trans.T("L_MAX");
            comboHomeZ.Items[0] = Trans.T("L_MIN");
            comboHomeZ.Items[1] = Trans.T("L_MAX");
            labelNumberOfExtruder.Text = Trans.T("L_NUMBER_OF_EXTRUDER:");
            labelRosPrintableHeight.Text = Trans.T("L_ROS_PRINTABLE_HEIGHT:");
            labelRosPrintableRadius.Text = Trans.T("L_ROS_PRINTABLE_RADIUS:");
            comboBoxPrinterType.Items[0] = Trans.T("L_CARTESIAN_PRINTER");
            comboBoxPrinterType.Items[1] = Trans.T("L_CARTESIAN_PRINTER_DUMP");
            comboBoxPrinterType.Items[2] = Trans.T("L_ROSTOCK_CIRCLE");
            comboBoxPrinterType.Items[3] = Trans.T("L_CNC_ROUTER");

        }
        public void save(string printername)
        {
            if (printername.Length == 0) return;
            RegistryKey p = printerKey.CreateSubKey(printername);
            currentPrinterKey = p;
            p.SetValue("travelFeedrate", textTravelFeedrate.Text);
            p.SetValue("zAxisFeedrate", textZFeedrate.Text);
            p.SetValue("checkTemp", checkTemp.Checked ? 1 : 0);
            p.SetValue("checkTempInterval", trackTempPeriod.Value);
            p.SetValue("disposeX", textDisposeX.Text);
            p.SetValue("disposeY", textDisposeY.Text);
            p.SetValue("disposeZ", textDisposeZ.Text);
            p.SetValue("goDisposeAfterJob", checkGoDisposeAfterJob.Checked ? 1 : 0);
            p.SetValue("disableHeatedBetAfterJob", checkDisbaleHeatedBedAfterJob.Checked ? 1 : 0);
            p.SetValue("disableExtruderAfterJob", checkDisableExtruderAfterJob.Checked ? 1 : 0);
            p.SetValue("disableMotorsAfterJob", checkDisableMotors.Checked ? 1 : 0);
            p.SetValue("printAreaWidth", textPrintAreaWidth.Text);
            p.SetValue("printAreaDepth", textPrintAreaDepth.Text);
            p.SetValue("printAreaHeight", textPrintAreaHeight.Text);
            //p.SetValue("hasDumpArea", checkHasDumpArea.Checked ? 1 : 0);
            p.SetValue("dumpAreaLeft", textDumpAreaLeft.Text);
            p.SetValue("dumpAreaFront", textDumpAreaFront.Text);
            p.SetValue("dumpAreaWidth", textDumpAreaWidth.Text);
            p.SetValue("dumpAreaDepth", textDumpAreaDepth.Text);
            p.SetValue("defaultExtruderTemp", textDefaultExtruderTemp.Text);
            p.SetValue("defaultHeatedBedTemp", textDefaultHeatedBedTemp.Text);
            p.SetValue("filterPath", textFilterPath.Text);
            p.SetValue("runFilterEverySlice", checkRunFilterEverySlice.Checked ? 1 : 0);
            p.SetValue("logM105", logM105Checkbox.Checked ? 1 : 0);
            p.SetValue("addPrintingTime", textAddPrintingTime.Text);
            p.SetValue("xhomeMax", comboHomeX.SelectedIndex); // checkHomeXMax.Checked ? 1 : 0);
            p.SetValue("yhomeMax", comboHomeY.SelectedIndex); // checkHomeYMax.Checked ? 1 : 0);
            p.SetValue("zhomeMax", comboHomeZ.SelectedIndex); // checkHomeZMax.Checked ? 1 : 0);
            p.SetValue("printerXMax", textPrinterXMax.Text);
            p.SetValue("printerXMin", textPrinterXMin.Text);
            p.SetValue("printerYMax", textPrinterYMax.Text);
            p.SetValue("printerYMin", textPrinterYMin.Text);
            p.SetValue("printerBedLeft", textBedLeft.Text);
            p.SetValue("printerBedFront", textBedFront.Text);
            p.SetValue("numExtruder", (int)numericNumExtruder.Value);
            p.SetValue("printerType", comboBoxPrinterType.SelectedIndex);
            p.SetValue("rostockHeight", textBoxRostockHeight.Text);
            p.SetValue("rostockRadius", textBoxRostockRadius.Text);
            p.SetValue("cncZTop", textCNCZTop.Text);
            p.SetValue("connector",Main.conn.connector.Id);
            Main.conn.connector.SaveToRegistry();
        }
        public void load(string printername)
        {
            if (printername.Length == 0) return;
            comboPrinter.Text = printername;
            RegistryKey p = printerKey.CreateSubKey(printername);
            currentPrinterKey = p;
            string id = (string)p.GetValue("connector","SerialConnector");
            int idx = 0;
            foreach (PrinterConnectorBase b in connectors)
            {
                if (b.Id == id) break;
                idx++;
            }
            comboConnector.SelectedIndex = idx;
            comboConnector_SelectedIndexChanged(null, null);
            textTravelFeedrate.Text = (string)p.GetValue("travelFeedrate",textTravelFeedrate.Text);
            textZFeedrate.Text = (string)p.GetValue("zAxisFeedrate",textZFeedrate.Text);
            checkTemp.Checked = ((int)p.GetValue("checkTemp", checkTemp.Checked ? 1 : 0))==1?true:false;
            trackTempPeriod.Value = (int)p.GetValue("checkTempInterval", trackTempPeriod.Value);
            textDisposeX.Text = (string)p.GetValue("disposeX", textDisposeX.Text);
            textDisposeY.Text = (string)p.GetValue("disposeY", textDisposeY.Text);
            textDisposeZ.Text = (string)p.GetValue("disposeZ", textDisposeZ.Text);
            checkGoDisposeAfterJob.Checked = 1 == (int)p.GetValue("goDisposeAfterJob", checkGoDisposeAfterJob.Checked ? 1 : 0);
            checkDisbaleHeatedBedAfterJob.Checked = 1 == (int)p.GetValue("disableHeatedBetAfterJob", checkDisbaleHeatedBedAfterJob.Checked ? 1 : 0);
            checkDisableExtruderAfterJob.Checked = 1 == (int)p.GetValue("disableExtruderAfterJob", checkDisableExtruderAfterJob.Checked ? 1 : 0);
            checkDisableMotors.Checked = 1 == (int) p.GetValue("disableMotorsAfterJob", checkDisableMotors.Checked ? 1 : 0);
            labelCheckEveryX.Text = Trans.T1("L_CHECK_EVERY_X",trackTempPeriod.Value.ToString());
            textPrintAreaWidth.Text = (string)p.GetValue("printAreaWidth", textPrintAreaWidth.Text);
            textPrintAreaDepth.Text = (string)p.GetValue("printAreaDepth", textPrintAreaDepth.Text);
            textPrintAreaHeight.Text = (string)p.GetValue("printAreaHeight", textPrintAreaHeight.Text);
            bool hasDump = 1==(int)p.GetValue("hasDumpArea", 0);
            textDumpAreaLeft.Text = (string)p.GetValue("dumpAreaLeft", textDumpAreaLeft.Text);
            textDumpAreaFront.Text = (string)p.GetValue("dumpAreaFront", textDumpAreaFront.Text);
            textDumpAreaWidth.Text = (string)p.GetValue("dumpAreaWidth", textDumpAreaWidth.Text);
            textDumpAreaDepth.Text = (string)p.GetValue("dumpAreaDepth", textDumpAreaDepth.Text);
            textDefaultExtruderTemp.Text = (string)p.GetValue("defaultExtruderTemp", textDefaultExtruderTemp.Text);
            textDefaultHeatedBedTemp.Text = (string)p.GetValue("defaultHeatedBedTemp", textDefaultHeatedBedTemp.Text);
            textFilterPath.Text = (string)p.GetValue("filterPath", textFilterPath.Text);
            checkRunFilterEverySlice.Checked = 1 == (int)p.GetValue("runFilterEverySlice", checkRunFilterEverySlice.Checked ? 1 : 0);
            logM105Checkbox.Checked = 1 == (int)p.GetValue("logM105", logM105Checkbox.Checked ? 1 : 0);
            textAddPrintingTime.Text = (string)p.GetValue("addPrintingTime", textAddPrintingTime.Text);
            //checkHomeXMax.Checked = 1 == (int)p.GetValue("xhomeMax", checkHomeXMax.Checked ? 1 : 0);
            //checkHomeYMax.Checked = 1 == (int)p.GetValue("yhomeMax", checkHomeYMax.Checked ? 1 : 0);
            //checkHomeZMax.Checked = 1 == (int)p.GetValue("zhomeMax", checkHomeZMax.Checked ? 1 : 0);
            comboHomeX.SelectedIndex = (int)p.GetValue("xhomeMax", 0);
            comboHomeY.SelectedIndex = (int)p.GetValue("yhomeMax", 0);
            comboHomeZ.SelectedIndex = (int)p.GetValue("zhomeMax", 0);
            textPrinterXMax.Text = (string)p.GetValue("printerXMax", textPrintAreaWidth.Text);
            textPrinterXMin.Text = (string)p.GetValue("printerXMin", "0");
            textPrinterYMax.Text = (string)p.GetValue("printerYMax", textPrintAreaDepth.Text);
            textPrinterYMin.Text = (string)p.GetValue("printerYMin", "0");
            textBedLeft.Text = (string)p.GetValue("printerBedLeft", "0");
            textBedFront.Text = (string)p.GetValue("printerBedFront", "0");
            numericNumExtruder.Value = (int)p.GetValue("numExtruder", 1);
            comboBoxPrinterType.SelectedIndex = (int)p.GetValue("printerType", hasDump ? 1 : 0);
            textBoxRostockHeight.Text = (string)p.GetValue("rostockHeight", textBoxRostockHeight.Text);
            textBoxRostockRadius.Text = (string)p.GetValue("rostockRadius", textBoxRostockRadius.Text);
            textCNCZTop.Text = (string)p.GetValue("cncZTop", textCNCZTop.Text);
        }
        public void UpdateDimensions()
        {
            printerType = comboBoxPrinterType.SelectedIndex;
            float.TryParse(textPrintAreaWidth.Text, NumberStyles.Float, GCode.format, out PrintAreaWidth);
            float.TryParse(textPrintAreaHeight.Text, NumberStyles.Float, GCode.format, out PrintAreaHeight);
            float.TryParse(textPrintAreaDepth.Text, NumberStyles.Float, GCode.format, out PrintAreaDepth);
            float.TryParse(textDumpAreaDepth.Text, NumberStyles.Float, GCode.format, out DumpAreaDepth);
            float.TryParse(textDumpAreaWidth.Text, NumberStyles.Float, GCode.format, out DumpAreaWidth);
            float.TryParse(textDumpAreaLeft.Text, NumberStyles.Float, GCode.format, out DumpAreaLeft);
            float.TryParse(textDumpAreaFront.Text, NumberStyles.Float, GCode.format, out DumpAreaFront);
            float.TryParse(textPrinterXMin.Text, NumberStyles.Float, GCode.format, out XMin);
            float.TryParse(textPrinterXMax.Text, NumberStyles.Float, GCode.format, out XMax);
            float.TryParse(textPrinterYMin.Text, NumberStyles.Float, GCode.format, out YMin);
            float.TryParse(textPrinterYMax.Text, NumberStyles.Float, GCode.format, out YMax);
            float.TryParse(textBedLeft.Text, NumberStyles.Float, GCode.format, out BedLeft);
            float.TryParse(textBedFront.Text, NumberStyles.Float, GCode.format, out BedFront);
            float.TryParse(textBoxRostockHeight.Text, NumberStyles.Float, GCode.format, out rostockHeight);
            float.TryParse(textBoxRostockRadius.Text, NumberStyles.Float, GCode.format, out rostockRadius);
            float.TryParse(textCNCZTop.Text, NumberStyles.Float, GCode.format, out cncZTop);
            //HasDumpArea = printerType == 1;
            if (printerType == 2)
            {
                PrintAreaHeight = rostockHeight;
                PrintAreaWidth = PrintAreaDepth = 2 * rostockRadius;
                BedFront = BedLeft = -rostockRadius;
                XMin = YMin = -rostockRadius;
                XMax = YMax = rostockRadius;

            }
        }
        public bool PointInside(float x, float y, float z)
        {
            if (z < -0.001 || z > PrintAreaHeight) return false;
            if (printerType < 2)
            {
                if (x < BedLeft || x > BedLeft + PrintAreaWidth) return false;
                if (y < BedFront || y > BedFront + PrintAreaDepth) return false;
            }
            else
            {
                float d = (float)Math.Sqrt(x * x + y * y);
                return d <= rostockRadius;
            }
            return true;
        }
        public void formToCon()
        {
            bool pnchanged = !con.printerName.Equals(comboPrinter.Text);
            con.printerName = comboPrinter.Text;
            /*  con.port = comboPort.Text;
              con.baud = int.Parse(comboBaud.Text);
              con.transferProtocol = comboTransferProtocol.SelectedIndex;
              switch (comboStopbits.SelectedIndex)
              {
                  case 0: con.stopbits = StopBits.None; break;
                  case 1: con.stopbits = StopBits.One; break;
                  case 2: con.stopbits = StopBits.Two; break;
              }
              switch (comboParity.SelectedIndex)
              {
                  case 0: con.parity = Parity.None; break;
                  case 1: con.parity = Parity.Even; break;
                  case 2: con.parity = Parity.Odd; break;
                  case 3: con.parity = Parity.Mark; break;
                  case 4: con.parity = Parity.Space; break;
              }
              con.pingpong = checkPingPong.Checked;
            int.TryParse(textReceiveCacheSize.Text, out con.receiveCacheSize);
            
             * */
            float.TryParse(textTravelFeedrate.Text, out con.travelFeedRate);
            float.TryParse(textZFeedrate.Text, out con.maxZFeedRate);
            con.autocheckTemp = checkTemp.Checked;
            con.autocheckInterval = trackTempPeriod.Value*1000;
            float.TryParse(textDisposeX.Text, NumberStyles.Float, GCode.format, out con.disposeX);
            float.TryParse(textDisposeY.Text, NumberStyles.Float, GCode.format, out con.disposeY);
            float.TryParse(textDisposeZ.Text, NumberStyles.Float, GCode.format, out con.disposeZ);
            con.afterJobGoDispose = checkGoDisposeAfterJob.Checked;
            con.afterJobDisableExtruder = checkDisableExtruderAfterJob.Checked;
            con.afterJobDisablePrintbed = checkDisbaleHeatedBedAfterJob.Checked;
            con.afterJobDisableMotors = checkDisableMotors.Checked;
            con.logM105 = logM105Checkbox.Checked;
            con.runFilterEverySlice = checkRunFilterEverySlice.Checked;
            con.filterCommand = textFilterPath.Text;
            con.numberExtruder = con.numExtruder = (int)numericNumExtruder.Value;
            float.TryParse(textAddPrintingTime.Text, out con.addPrintingTime);
            if (Main.main.printPanel != null)
            {
                try
                {
                    Main.main.printPanel.numericUpDownExtruder.Value = int.Parse(textDefaultExtruderTemp.Text);
                }
                catch (FormatException)
                {
                    Main.main.printPanel.numericUpDownExtruder.Value = 0;
                    textDefaultExtruderTemp.Text = "0";
                }
                try
                {
                    Main.main.printPanel.numericPrintBed.Value = int.Parse(textDefaultHeatedBedTemp.Text);
                }
                catch (FormatException)
                {
                    Main.main.printPanel.numericPrintBed.Value = 0;
                    textDefaultHeatedBedTemp.Text = "0";
                }
                Main.main.printPanel.refillExtruder();
            }
            if (eventPrinterChanged != null)
                eventPrinterChanged(currentPrinterKey,pnchanged);
        }
        public void conToForm()
        {
            comboPrinter.Text = con.printerName;
            /* comboBaud.Text = con.baud.ToString();
             comboPort.Text = con.port;
             comboTransferProtocol.SelectedIndex = con.transferProtocol;
             switch (con.stopbits)
             {
                 case StopBits.None: comboStopbits.SelectedIndex = 0; break;
                 case StopBits.One: comboStopbits.SelectedIndex = 1; break;
                 case StopBits.Two: comboStopbits.SelectedIndex = 2; break;
             }
             switch (con.parity)
             {
                 case Parity.None: comboParity.SelectedIndex = 0; break;
                 case Parity.Even: comboParity.SelectedIndex = 1; break;
                 case Parity.Odd: comboParity.SelectedIndex = 2; break;
                 case Parity.Mark: comboParity.SelectedIndex = 3; break;
                 case Parity.Space: comboParity.SelectedIndex = 4; break;
             }
             checkPingPong.Checked = con.pingpong;
            textReceiveCacheSize.Text = con.receiveCacheSize.ToString();
             */
            textTravelFeedrate.Text = con.travelFeedRate.ToString(GCode.format);
            textZFeedrate.Text = con.maxZFeedRate.ToString(GCode.format);
            checkTemp.Checked = con.autocheckTemp;
            trackTempPeriod.Value = (int)(con.autocheckInterval/1000);
            textDisposeX.Text = con.disposeX.ToString(GCode.format);
            textDisposeY.Text = con.disposeY.ToString(GCode.format);
            textDisposeZ.Text = con.disposeZ.ToString(GCode.format);
            checkGoDisposeAfterJob.Checked = con.afterJobGoDispose;
            checkDisableExtruderAfterJob.Checked = con.afterJobDisableExtruder;
            checkDisbaleHeatedBedAfterJob.Checked = con.afterJobDisablePrintbed;
            checkDisableMotors.Checked = con.afterJobDisableMotors;
            labelCheckEveryX.Text = Trans.T1("L_CHECK_EVERY_X", trackTempPeriod.Value.ToString());
            textFilterPath.Text = con.filterCommand;
            checkRunFilterEverySlice.Checked = con.runFilterEverySlice;
            logM105Checkbox.Checked = con.logM105;
            textAddPrintingTime.Text = con.addPrintingTime.ToString(GCode.format);
            numericNumExtruder.Value = con.numExtruder;
            if (Main.main.printPanel != null)
            {
                textDefaultExtruderTemp.Text = Main.main.printPanel.numericUpDownExtruder.Value.ToString("0");
                textDefaultHeatedBedTemp.Text = Main.main.printPanel.numericPrintBed.Value.ToString("0");
            }
        }
        private void buttonOK_Click(object sender, EventArgs e)
        {
            buttonApply_Click(null, null);
            formToCon();
            UpdateDimensions();
            Hide();
            Main.main.slicerPanel.UpdateSelection();
            Main.main.Update3D();
            Main.main.UpdateConnections();
            if (Main.main != null && Main.main.editor != null)
                Main.main.editor.Changed();
        }

        private void buttonAbort_Click(object sender, EventArgs e)
        {
            load(con.printerName);
            UpdateDimensions();
            Hide();
            Main.main.Update3D();
            Main.main.UpdateConnections();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            string name = comboPrinter.Text;
            if(name.Length==0) return;
            save(comboPrinter.Text);
            if (comboPrinter.Items.IndexOf(name) < 0)
            {
                comboPrinter.Items.Add(name);
                comboPrinter.SelectedIndex = comboPrinter.Items.IndexOf(name);
            }
            Main.main.Update3D();
            Main.main.UpdateConnections();
        }

        private void comboPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboPrinter.SelectedIndex < 0) return;
            load(comboPrinter.Text);
            formToCon();
            UpdateDimensions(); 
            repetierKey.SetValue("currentPrinter", comboPrinter.Text);
            if (Main.main != null && Main.main.editor != null)
                Main.main.editor.Changed();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Trans.T1("L_REALLY_DELETE_PRINTER",/*Do you realy want to delete all settings for*/ comboPrinter.Text),Trans.T("L_SECURITY_QUESTION"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string name = comboPrinter.Text;
                printerKey.DeleteSubKeyTree(comboPrinter.Text);
                comboPrinter.Items.Remove(name);
                comboPrinter.Text = "";
                if(comboPrinter.Items.Count>0)
                    comboPrinter.SelectedIndex = 0;
            }
        }

        private void trackTempPeriod_ValueChanged(object sender, EventArgs e)
        {
            labelCheckEveryX.Text = Trans.T1("L_CHECK_EVERY_X", trackTempPeriod.Value.ToString());
        }

        private void float_Validating(object sender, CancelEventArgs e)
        {
            TextBox box = (TextBox)sender;
            try
            {
                float.Parse(box.Text, NumberStyles.Float, GCode.format);
                errorProvider.SetError(box, "");
            }
            catch
            {
                errorProvider.SetError(box, Trans.T("L_NOT_A_NUMBER"));
            }
        }
        private void int_Validating(object sender, CancelEventArgs e)
        {
            TextBox box = (TextBox)sender;
            try
            {
                int.Parse(box.Text);
                errorProvider.SetError(box, "");
            }
            catch
            {
                errorProvider.SetError(box, Trans.T("L_NOT_AN_INTEGER"));
            }
        }

        private void FormPrinterSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            RegMemory.StoreWindowPos("printerSettingsWindow", this, false, false);
        }
        public float XHomePos
        {
            get
            {
                switch (xhomeMode)
                {
                    case 0: return XMin;
                    case 1: return XMax;
                    case 2: return 0;
                }
                return 0;
            }
        }
        public float YHomePos
        {
            get
            {
                switch (yhomeMode)
                {
                    case 0: return YMin;
                    case 1: return YMax;
                    case 2: return 0;
                }
                return 0;
            }
        }
        public float ZHomePos
        {
            get
            {
                switch (zhomemode)
                {
                    case 0: return 0;
                    case 1: return PrintAreaHeight;
                    case 2: return 0;
                }
                return 0;
            }
        }

        private void comboHomeX_SelectedIndexChanged(object sender, EventArgs e)
        {
            xhomeMode = comboHomeX.SelectedIndex;
        }

        private void comboHomeY_SelectedIndexChanged(object sender, EventArgs e)
        {
            yhomeMode = comboHomeY.SelectedIndex;
        }

        private void comboHomeZ_SelectedIndexChanged(object sender, EventArgs e)
        {
            zhomemode = comboHomeZ.SelectedIndex;
        }

        private void comboBoxPrinterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = comboBoxPrinterType.SelectedIndex;
            panelRostock.Visible = idx == 2;
            panelDumpArea.Visible = idx == 1;
            panelTotalArea.Visible = idx != 2;
            panelCNC.Visible = idx == 3;
        }

        private void comboConnector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Main.conn.connector != null)
                Main.conn.connector.Deactivate();
            if (connectorPanel != null)
            {
               tabPageConnection.Controls.Remove(connectorPanel);
            }
            Main.conn.connector = (PrinterConnectorBase)comboConnector.SelectedItem;
            if (currentPrinterKey != null)
            {
                Main.conn.connector.SetConfiguration(currentPrinterKey);
                Main.conn.connector.LoadFromRegistry();
            }
            connectorPanel = Main.conn.connector.ConnectionDialog();
            connectorPanel.Dock = DockStyle.Top;
            tabPageConnection.Controls.Add(connectorPanel);
            tabPageConnection.Controls.SetChildIndex(connectorPanel,0);
            Main.conn.connector.Activate();
        }
    }
}
