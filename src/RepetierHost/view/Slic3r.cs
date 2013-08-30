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
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using RepetierHost.view.utils;
using RepetierHost.model;
using RepetierHost.model.geom;

namespace RepetierHost.view
{
    public partial class Slic3r : Form
    {
        RegistryKey rconfigs;
        RegistryKey rcon;
        string config="";
        Process procConvert = null;
        Process procSlic3r = null;

        string slicefile = null;

        public Slic3r()
        {
            InitializeComponent();
            if (Main.IsMono)
                panelCloseButtons.Location = new Point(panelCloseButtons.Location.X, panelCloseButtons.Location.Y - 14);
            comboFillPattern.SelectedIndex = 0;
            comboSolidFillPattern.SelectedIndex = 0;
            comboGCodeFlavor.SelectedIndex = 0;
            comboSupportMaterialTool.SelectedIndex = 0;
            comboSupportPattern.SelectedIndex = 0;
            comboSupportPattern.Text = "rectilinear";

            RegMemory.RestoreWindowPos("slic3rWindow", this);
            rconfigs = Main.main.repetierKey.CreateSubKey("slic3r");
            foreach (string s in rconfigs.GetSubKeyNames())
                comboConfig.Items.Add(s);
            config = (string)rconfigs.GetValue("currentConfig", "default");
            if (comboConfig.Items.Count == 0)
            {
                comboConfig.Items.Add("default");
                comboConfig.SelectedIndex = 0;
            }
            else
            {
                for (int i = 0; i < comboConfig.Items.Count; i++)
                {
                    if (comboConfig.Items[i].ToString() == config)
                    {
                        comboConfig.SelectedIndex = i;
                        break;
                    }
                }
            }
        }
        private void loadConfig(string name)
        {
            if (name != config)
            {
                rconfigs.SetValue("currentConfig", name);
                config = name;
            }
            comboConfig.Text = config;
            rcon = rconfigs.CreateSubKey(config);
            RegistryKey c = rcon;
            textNozzleDiameter.Text = (string)c.GetValue("NozzleDiameter", textNozzleDiameter.Text);
            textZOffset.Text = (string)c.GetValue("ZOffset", textZOffset.Text);
            checkRelativeE.Checked  = ((int)c.GetValue("UseRelativeE", checkRelativeE.Checked ? 1 : 0))==1;
            checkComments.Checked = ((int)c.GetValue("Comments", checkComments.Checked ? 1 : 0)) == 1;
            textDiameter.Text = (string)c.GetValue("FilamentDiameter", textDiameter.Text);
            textPackingDensity.Text = (string)c.GetValue("PackingDensity", textPackingDensity.Text);
            textTemperature.Text = (string)c.GetValue("Temperature", textTemperature.Text);
            textPrintFeedrate.Text = (string)c.GetValue("PrintFeedrate", textPrintFeedrate.Text);
            textTravelFeedrate.Text = (string)c.GetValue("TravelFeedrate", textTravelFeedrate.Text);
            textPerimeterFeedrate.Text = (string)c.GetValue("PerimeterFeedrate", textPerimeterFeedrate.Text);
            textLayerHeight.Text = (string)c.GetValue("LayerHeight", textLayerHeight.Text);
            textFirstLayerHeight.Text = (string)c.GetValue("FirstLayerHeight", textFirstLayerHeight.Text);
            textInfillEvery.Text = (string)c.GetValue("InfillEvery", textInfillEvery.Text);
            textPerimeters.Text = (string)c.GetValue("Perimeters", textPerimeters.Text);
            textSolidLayers.Text = (string)c.GetValue("SolidLayers", textSolidLayers.Text);
            textFillDensity.Text = (string)c.GetValue("FillDensity", textFillDensity.Text);
            textFillAngle.Text = (string)c.GetValue("FillAngle", textFillAngle.Text);
            comboFillPattern.Text = (string)c.GetValue("FillPattern", comboFillPattern.Text);
            comboSolidFillPattern.Text = (string)c.GetValue("SolidFillPattern", comboSolidFillPattern.Text);
            textRetLength.Text = (string)c.GetValue("RetLength", textRetLength.Text);
            textRetExtraDistance.Text = (string)c.GetValue("RetExtraDistance", textRetExtraDistance.Text);
            textRetLift.Text = (string)c.GetValue("RetLift", textRetLift.Text);
            textRetMinTravel.Text = (string)c.GetValue("RetMinTravel", textRetMinTravel.Text);
            textRetSpeed.Text = (string)c.GetValue("RetSpeed", textRetSpeed.Text);
            textSkirtLoops.Text = (string)c.GetValue("SkirtLoops", textSkirtLoops.Text);
            textSkirtHeight.Text = (string)c.GetValue("SkirtHeight", textSkirtHeight.Text);
            textSkirtDistance.Text = (string)c.GetValue("SkirtDistance", textSkirtDistance.Text);
            textExtrusionWidth.Text = (string)c.GetValue("ExtrusionWidth", textExtrusionWidth.Text);
            textBridgeFlowRatio.Text = (string) c.GetValue("BridgeFlowRatio", textBridgeFlowRatio.Text);
            textBridgeSpeed.Text = (string) c.GetValue("BridgeSpeed", textBridgeSpeed.Text);
            textSolidInfillSpeed.Text = (string) c.GetValue("SolidInfillSpeed", textSolidInfillSpeed.Text);
            textSmallPerimeterSpeed.Text = (string) c.GetValue("SmallPerimeterSpeed", textSmallPerimeterSpeed.Text);
            textCoolBridgeFanSpeed.Text = (string)c.GetValue("CoolBridgeFanSpeed", textCoolBridgeFanSpeed.Text);
            textCoolDisableLayer.Text = (string)c.GetValue("CoolDisplayLayer", textCoolDisableLayer.Text);
            textCoolEnableBelow.Text = (string)c.GetValue("CoolEnableBelow", textCoolEnableBelow.Text);
            textCoolMaxFanSpeed.Text = (string)c.GetValue("CoolMaxFanSpeed", textCoolMaxFanSpeed.Text);
            textCoolMinFanSpeed.Text = (string)c.GetValue("CoolMinFanSpeed", textCoolMinFanSpeed.Text);
            textCoolMinPrintSpeed.Text = (string)c.GetValue("CoolMinPrintSpeed", textCoolMinPrintSpeed.Text);
            textCoolSlowDownBelow.Text = (string)c.GetValue("CoolSlowDownBelow", textCoolSlowDownBelow.Text);
            checkEnableCooling.Checked = ((int)c.GetValue("CoolEnable", checkEnableCooling.Checked ? 1 : 0)) == 1;
            checkGenerateSupportMaterial.Checked = ((int)c.GetValue("GenerateSupportMaterial", checkGenerateSupportMaterial.Checked ? 1 : 0)) == 1;
            comboGCodeFlavor.SelectedIndex = (int)c.GetValue("GCodeFlavor", comboGCodeFlavor.SelectedIndex);
            comboSupportMaterialTool.SelectedIndex = (int)c.GetValue("SupportMaterialTool", comboSupportMaterialTool.SelectedIndex);
            textFirstLayerTemperature.Text = (string)c.GetValue("FirstLayerTemperature", textFirstLayerTemperature.Text);
            checkFanAlwaysEnabled.Checked = ((int)c.GetValue("FanAlwaysEnabled", checkFanAlwaysEnabled.Checked ? 1 : 0)) == 1;
            textFirstLayerBedTemperature.Text = (string)c.GetValue("FirstLayerBedTemperature", textFirstLayerBedTemperature.Text);
            textBedTemperature.Text = (string)c.GetValue("BedTemperature", textBedTemperature.Text);
            checkRandomizeStartingPoints.Checked = ((int)c.GetValue("RandomizeStartingPoints", checkRandomizeStartingPoints.Checked ? 1 : 0)) == 1;
            textNumberOfThreads.Text = (string)c.GetValue("NumberOfThreads", textNumberOfThreads.Text);
            textFirstLayerSpeed.Text = (string)c.GetValue("FirstLayerSpeed", textFirstLayerSpeed.Text);
            textOverhangTreshold.Text = (string)c.GetValue("OverhangTreshold", textOverhangTreshold.Text);
            textPatternSpacing.Text = (string)c.GetValue("PatternSpacing", textPatternSpacing.Text);
            textPatternAngle.Text = (string)c.GetValue("PatternAngle", textPatternAngle.Text);
            comboSupportPattern.Text = (string)c.GetValue("SupportPattern", comboSupportPattern.Text);
            textBrim.Text = (string)c.GetValue("BrimWidth",textBrim.Text);
        }
        private void saveConfig(string name)
        {
            RegistryKey c = rconfigs.CreateSubKey(name);
            c.SetValue("NozzleDiameter", textNozzleDiameter.Text);
            c.SetValue("ZOffset", textZOffset.Text);
            c.SetValue("UseRelativeE", checkRelativeE.Checked ? 1 : 0);
            c.SetValue("Comments", checkComments.Checked ? 1 : 0);
            c.SetValue("FilamentDiameter", textDiameter.Text);
            c.SetValue("PackingDensity", textPackingDensity.Text);
            c.SetValue("Temperature", textTemperature.Text);
            c.SetValue("PrintFeedrate", textPrintFeedrate.Text);
            c.SetValue("TravelFeedrate", textTravelFeedrate.Text);
            c.SetValue("PerimeterFeedrate", textPerimeterFeedrate.Text);
            c.SetValue("LayerHeight", textLayerHeight.Text);
            c.SetValue("FirstLayerHeight", textFirstLayerHeight.Text);
            c.SetValue("InfillEvery", textInfillEvery.Text);
            c.SetValue("Perimeters", textPerimeters.Text);
            c.SetValue("SolidLayers", textSolidLayers.Text);
            c.SetValue("FillDensity", textFillDensity.Text);
            c.SetValue("FillAngle", textFillAngle.Text);
            c.SetValue("FillPattern", comboFillPattern.Text);
            c.SetValue("SolidFillPattern", comboSolidFillPattern.Text);
            c.SetValue("RetLength", textRetLength.Text);
            c.SetValue("RetExtraDistance", textRetExtraDistance.Text);
            c.SetValue("RetLift", textRetLift.Text);
            c.SetValue("RetMinTravel", textRetMinTravel.Text);
            c.SetValue("RetSpeed", textRetSpeed.Text);
            c.SetValue("SkirtLoops", textSkirtLoops.Text);
            c.SetValue("SkirtHeight", textSkirtHeight.Text);
            c.SetValue("SkirtDistance", textSkirtDistance.Text);
            c.SetValue("ExtrusionWidth", textExtrusionWidth.Text);
            c.SetValue("BridgeFlowRatio", textBridgeFlowRatio.Text);
            c.SetValue("BridgeSpeed", textBridgeSpeed.Text);
            c.SetValue("SolidInfillSpeed", textSolidInfillSpeed.Text);
            c.SetValue("SmallPerimeterSpeed", textSmallPerimeterSpeed.Text);
            c.SetValue("CoolBridgeFanSpeed", textCoolBridgeFanSpeed.Text);
            c.SetValue("CoolDisplayLayer", textCoolDisableLayer.Text);
            c.SetValue("CoolEnableBelow", textCoolEnableBelow.Text);
            c.SetValue("CoolMaxFanSpeed", textCoolMaxFanSpeed.Text);
            c.SetValue("CoolMinFanSpeed", textCoolMinFanSpeed.Text);
            c.SetValue("CoolMinPrintSpeed", textCoolMinPrintSpeed.Text);
            c.SetValue("CoolSlowDownBelow", textCoolSlowDownBelow.Text);
            c.SetValue("CoolEnable", checkEnableCooling.Checked ? 1 : 0);
            c.SetValue("GenerateSupportMaterial", checkGenerateSupportMaterial.Checked ? 1 : 0);
            c.SetValue("GCodeFlavor", comboGCodeFlavor.SelectedIndex);
            c.SetValue("SupportMaterialTool", comboSupportMaterialTool.SelectedIndex);
            c.SetValue("FirstLayerTemperature", textFirstLayerTemperature.Text);
            c.SetValue("FanAlwaysEnabled", checkFanAlwaysEnabled.Checked ? 1 : 0);
            c.SetValue("FirstLayerBedTemperature", textFirstLayerBedTemperature.Text);
            c.SetValue("BedTemperature", textBedTemperature.Text);
            c.SetValue("RandomizeStartingPoints", checkRandomizeStartingPoints.Checked ? 1 : 0);
            c.SetValue("NumberOfThreads", textNumberOfThreads.Text);
            c.SetValue("FirstLayerSpeed", textFirstLayerSpeed.Text);
            c.SetValue("OverhangTreshold", textOverhangTreshold.Text);
            c.SetValue("PatternSpacing", textPatternSpacing.Text);
            c.SetValue("PatternAngle", textPatternAngle.Text);
            c.SetValue("SupportPattern", comboSupportPattern.Text);
            c.SetValue("BrimWidth",textBrim.Text);
        }
        private void buttonOK_Click(object sender, EventArgs e)
        {
            saveConfig(config);
            Hide();
        }

        private void Slic3r_FormClosing(object sender, FormClosingEventArgs e)
        {
            RegMemory.StoreWindowPos("slic3rWindow", this, false, false);
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            saveConfig(config);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            loadConfig(config);
            Hide();
        }

        private void buttonNewConfig_Click(object sender, EventArgs e)
        {
            string name = StringInput.GetString("Create new config", "Name of the new configuration");
            comboConfig.Items.Add(name);
            comboConfig.SelectedIndex = comboConfig.Items.Count - 1;
        }
        private void float_Validating(object sender, CancelEventArgs e)
        {
            TextBox box = (TextBox)sender;
            try
            {
                float f = float.Parse(box.Text, NumberStyles.AllowDecimalPoint, GCode.format);
                if(f>=0) 
                    errorProvider.SetError(box, "");
                else
                    errorProvider.SetError(box, "No negative values allowed.");
            }
            catch
            {
                errorProvider.SetError(box, "Not a number.");
            }
        }
        private void floatOrPercent_Validating(object sender, CancelEventArgs e)
        {
            TextBox box = (TextBox)sender;
            try
            {
                String t = box.Text;
                if (t.EndsWith("%")) t = t.Substring(0, t.Length - 1);
                float f = float.Parse(t, NumberStyles.AllowDecimalPoint, GCode.format);
                if (f >= 0)
                    errorProvider.SetError(box, "");
                else
                    errorProvider.SetError(box, "No negative values allowed.");
            }
            catch
            {
                errorProvider.SetError(box, "Not a number.");
            }
        }
        private void int_Validating(object sender, CancelEventArgs e)
        {
            TextBox box = (TextBox)sender;
            try
            {
                int i = int.Parse(box.Text);
                if (i >= 0)
                    errorProvider.SetError(box, "");
                else
                    errorProvider.SetError(box, "No negative values allowed.");
            }
            catch
            {
                errorProvider.SetError(box, "Not an integer.");
            }
        }
        public string wrapQuotes(string text)
        {
            if (text.StartsWith("\"") && text.EndsWith("\"")) return text;
            return "\"" + text.Replace("\"", "\\\"") + "\"";
        }
        public void KillSlice()
        {
            if (procConvert != null)
            {
                procConvert.Kill();
                procConvert = null;
                Main.conn.log("Slic3r slicing process killed on user request.", false, 2);
            }
        }
        public void RunExternalConfig()
        {
            if (procSlic3r != null)
            {
                return;
            }
            procSlic3r = new Process();
            try
            {
                string exe = findSlic3rExecutable();
                if (exe == null)
                {
                    MessageBox.Show(Trans.T("L_SLIC3R_NOT_FOUND"), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                /*string basedir = (string)Main.main.repetierKey.GetValue("installPath", "");
                string exname = "slic3r.exe";
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                    exname = "bin" + Path.DirectorySeparatorChar + "slic3r";
                if (Main.IsMac)
                    exname = "MacOS" + Path.DirectorySeparatorChar + "slic3r";
                string exe = basedir + Path.DirectorySeparatorChar + "Slic3r" + Path.DirectorySeparatorChar + exname;
                if (File.Exists(BasicConfiguration.basicConf.ExternalSlic3rPath))
                    exe = BasicConfiguration.basicConf.ExternalSlic3rPath;
                */
                StringBuilder sb = new StringBuilder();
                if (File.Exists(BasicConfiguration.basicConf.ExternalSlic3rIniFile))
                {
                    sb.Append("--load ");
                    sb.Append(wrapQuotes(BasicConfiguration.basicConf.ExternalSlic3rIniFile));
                }
                procSlic3r.EnableRaisingEvents = true;
                procSlic3r.Exited += new EventHandler(Slic3rExited);
                procSlic3r.StartInfo.FileName = Main.IsMono ? exe : wrapQuotes(exe);
                procSlic3r.StartInfo.UseShellExecute = false;
                procSlic3r.StartInfo.RedirectStandardOutput = true;
                procSlic3r.OutputDataReceived += new DataReceivedEventHandler(OutputDataHandler);
                procSlic3r.StartInfo.RedirectStandardError = true;
                procSlic3r.ErrorDataReceived += new DataReceivedEventHandler(OutputDataHandler);
                procSlic3r.StartInfo.Arguments = sb.ToString();
                procSlic3r.Start();
                // Start the asynchronous read of the standard output stream.
                procSlic3r.BeginOutputReadLine();
                procSlic3r.BeginErrorReadLine();
            }
            catch (Exception e)
            {
                Main.conn.log(e.ToString(), false, 2);
            }
        }
        public void RunConfig()
        {
            if (procSlic3r != null)
            {
                return;
            }
            procSlic3r = new Process();
            try
            {
                string exe = findSlic3rExecutable();
                if (exe == null)
                {
                    MessageBox.Show(Trans.T("L_SLIC3R_NOT_FOUND"), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
              /*  string basedir = (string)Main.main.repetierKey.GetValue("installPath", "");
                string exname = "slic3r.exe";
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                    exname = "slic3r.pl";
                if (Main.IsMac)
                    exname = "MacOS" + Path.DirectorySeparatorChar + "slic3r";
                string exe = basedir + Path.DirectorySeparatorChar + "Slic3r" + Path.DirectorySeparatorChar + exname;
                if (File.Exists(BasicConfiguration.basicConf.Slic3rExecutable))
                    exe = BasicConfiguration.basicConf.Slic3rExecutable;
                */
                StringBuilder sb = new StringBuilder();
                if(BasicConfiguration.basicConf.Slic3rVersionGroup>=1 && Main.conn.numberExtruder<=1)
                    sb.Append("--no-plater --gui-mode expert");
                procSlic3r.EnableRaisingEvents = true;
                procSlic3r.Exited += new EventHandler(Slic3rExited);
                procSlic3r.StartInfo.FileName = Main.IsMono ? exe : wrapQuotes(exe);
                procSlic3r.StartInfo.UseShellExecute = false;
                procSlic3r.StartInfo.RedirectStandardOutput = true;
                procSlic3r.OutputDataReceived += new DataReceivedEventHandler(OutputDataHandler);
                procSlic3r.StartInfo.RedirectStandardError = true;
                procSlic3r.ErrorDataReceived += new DataReceivedEventHandler(OutputDataHandler);
                procSlic3r.StartInfo.Arguments = sb.ToString();
                procSlic3r.Start();
                // Start the asynchronous read of the standard output stream.
                procSlic3r.BeginOutputReadLine();
                procSlic3r.BeginErrorReadLine();
            }
            catch (Exception e)
            {
                Main.conn.log(e.ToString(), false, 2);
            }
        }
        private void Slic3rExited(object sender, System.EventArgs e)
        {
            procSlic3r.Close();
            procSlic3r = null;
            Main.main.Invoke(Main.main.slicerPanel.UpdateSelectionInvoker);
        }
      /*  public void RunSlice(string file,float centerx,float centery)
        {
            if (procConvert != null)
            {
                MessageBox.Show("Last slice job still running. Slicing of new job is canceled.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SlicingInfo.Start("Slic3r");
            SlicingInfo.SetAction("Analyzing STL file ...");
            try
            {
                STL stl = new STL();
                stl.Load(file);
                stl.UpdateBoundingBox();
                if (stl.xMin > 0 && stl.yMin > 0 && stl.xMax < Main.printerSettings.PrintAreaWidth && stl.yMax < Main.printerSettings.PrintAreaDepth)
                {
                    // User assigned valid position, so we use this
                    centerx = stl.xMin + (stl.xMax - stl.xMin) / 2;
                    centery = stl.yMin + (stl.yMax - stl.yMin) / 2;
                }
                stl.Clear();
            }
            catch (Exception e)
            {
                Main.conn.log(e.ToString(), false, 2);
                SlicingInfo.Stop();
                return;
            }
            SlicingInfo.SetAction("Slicing STL file ...");
            procConvert = new Process();
            try
            {
            string basedir = (string)Main.main.repetierKey.GetValue("installPath","");
            string exname = "slic3r.exe";
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                    exname = "bin"+Path.DirectorySeparatorChar+"slic3r";
            if (Main.IsMac)
                exname = "MacOS" + Path.DirectorySeparatorChar + "slic3r";
            string exe = basedir + Path.DirectorySeparatorChar + "Slic3r" + Path.DirectorySeparatorChar + exname;
                slicefile = file;
                string target = StlToGCode(file);
                if (File.Exists(target))
                    File.Delete(target);
                procConvert.EnableRaisingEvents = true;
                procConvert.Exited += new EventHandler(ConversionExited);
                procConvert.StartInfo.FileName = Main.IsMono ? exe : wrapQuotes(exe);
                StringBuilder sb = new StringBuilder();
                sb.Append("--nozzle-diameter ");
                sb.Append(textNozzleDiameter.Text);
                sb.Append(" ");
                sb.Append(" -o ");
                sb.Append(wrapQuotes(StlToGCode(file)));
                sb.Append(" ");
                if (checkRelativeE.Checked)
                    sb.Append("--use-relative-e-distances ");
                if (checkComments.Checked)
                    sb.Append("--gcode-comments ");
               // else
               //     sb.Append("--gcode-comments 0 ");
                sb.Append("-j ");
                sb.Append(textNumberOfThreads.Text);
                if (checkRandomizeStartingPoints.Checked)
                    sb.Append(" --randomize-start");
                sb.Append(" --z-offset ");
                sb.Append(textZOffset.Text);
                sb.Append(" --filament-diameter ");
                sb.Append(textDiameter.Text);
                sb.Append(" --extrusion-multiplier ");
                sb.Append(textPackingDensity.Text);
                sb.Append(" --temperature ");
                sb.Append(textTemperature.Text);
                sb.Append(" --infill-speed ");
                sb.Append(textPrintFeedrate.Text);
                sb.Append(" --solid-infill-speed ");
                sb.Append(textSolidInfillSpeed.Text);
                sb.Append(" --travel-speed ");
                sb.Append(textTravelFeedrate.Text);
                sb.Append(" --bridge-speed ");
                sb.Append(textBridgeSpeed.Text);
                sb.Append(" --perimeter-speed ");
                sb.Append(textPerimeterFeedrate.Text);
                sb.Append(" --small-perimeter-speed ");
                sb.Append(textSmallPerimeterSpeed.Text);
                sb.Append(" --bridge-flow-ratio ");
                sb.Append(textBridgeFlowRatio.Text);
                sb.Append(" --layer-height ");
                sb.Append(textLayerHeight.Text);
                sb.Append(" --first-layer-speed ");
                sb.Append(textFirstLayerSpeed.Text);
                sb.Append(" --first-layer-height ");
                sb.Append(textFirstLayerHeight.Text);
                sb.Append(" --infill-every-layers ");
                sb.Append(textInfillEvery.Text);
                sb.Append(" --perimeters ");
                sb.Append(textPerimeters.Text);
                sb.Append(" --solid-layers ");
                sb.Append(textSolidLayers.Text);
                sb.Append(" --fill-density ");
                sb.Append(textFillDensity.Text);
                sb.Append(" --fill-angle ");
                sb.Append(textFillAngle.Text);
                sb.Append(" --fill-pattern ");
                sb.Append(comboFillPattern.SelectedItem);
                sb.Append(" --solid-fill-pattern ");
                sb.Append(comboSolidFillPattern.SelectedItem);
                sb.Append(" --retract-length ");
                sb.Append(textRetLength.Text);
                sb.Append(" --retract-speed ");
                sb.Append(textRetSpeed.Text);
                sb.Append(" --retract-restart-extra ");
                sb.Append(textRetExtraDistance.Text);
                sb.Append(" --retract-before-travel ");
                sb.Append(textRetMinTravel.Text);
                sb.Append(" --retract-lift ");
                sb.Append(textRetLift.Text);
                sb.Append(" --skirts ");
                sb.Append(textSkirtLoops.Text);
                sb.Append(" --skirt-distance ");
                sb.Append(textSkirtDistance.Text);
                sb.Append(" --skirt-height ");
                sb.Append(textSkirtHeight.Text);
                sb.Append(" --extrusion-width ");
                sb.Append(textExtrusionWidth.Text);
                sb.Append(" --brim-width ");
                sb.Append(textBrim.Text);
                sb.Append(" --support-material-threshold ");
                sb.Append(textOverhangTreshold.Text);
                sb.Append(" --support-material-pattern ");
                sb.Append(comboSupportPattern.SelectedItem);
                sb.Append(" --support-material-spacing ");
                sb.Append(textPatternSpacing.Text);
                sb.Append(" --support-material-angle ");
                sb.Append(textPatternAngle.Text);
                sb.Append(" --print-center ");
                sb.Append(centerx.ToString("0",GCode.format));
                sb.Append(",");
                sb.Append(centery.ToString("0", GCode.format));
                if (checkEnableCooling.Checked)
                {
                    sb.Append(" --cooling --bridge-fan-speed ");
                    sb.Append(textCoolBridgeFanSpeed.Text);
                    sb.Append(" --disable-fan-first-layers ");
                    sb.Append(textCoolDisableLayer.Text);
                    sb.Append(" --fan-below-layer-time ");
                    sb.Append(textCoolEnableBelow.Text);
                    sb.Append(" --max-fan-speed ");
                    sb.Append(textCoolMaxFanSpeed.Text);
                    sb.Append(" --min-fan-speed ");
                    sb.Append(textCoolMinFanSpeed.Text);
                    sb.Append(" --min-print-speed ");
                    sb.Append(textCoolMinPrintSpeed.Text);
                    sb.Append(" --slowdown-below-layer-time ");
                    sb.Append(textCoolSlowDownBelow.Text);
                }
                if (checkGenerateSupportMaterial.Checked)
                {
                    sb.Append(" --support-material --support-material-tool " + comboSupportMaterialTool.SelectedIndex);
                }
                sb.Append(" --gcode-flavor ");
                switch (comboGCodeFlavor.SelectedIndex)
                {
                    case 0:
                    default:
                        sb.Append("reprap");
                        break;
                    case 1:
                        sb.Append("teacup");
                        break;
                    case 2:
                        sb.Append("makerbot");
                        break;
                    case 3:
                        sb.Append("mach3");
                        break;
                    case 4:
                        sb.Append("no-extrusion");
                        break;
                }
                sb.Append(" --first-layer-temperature ");
                sb.Append(textFirstLayerTemperature.Text);
                sb.Append(" --bed-temperature ");
                sb.Append(textBedTemperature.Text);
                sb.Append(" --first-layer-bed-temperature ");
                sb.Append(textFirstLayerBedTemperature.Text);
                if (checkFanAlwaysEnabled.Checked)
                {
                    sb.Append(" --fan-always-on");
                }
                sb.Append(" --start-gcode ");
                sb.Append(wrapQuotes(basedir+Path.DirectorySeparatorChar+"empty.txt"));
                sb.Append(" --end-gcode ");
                sb.Append(wrapQuotes(basedir+Path.DirectorySeparatorChar+"empty.txt"));
                sb.Append(" ");
                sb.Append(wrapQuotes(file));
                Main.conn.log(sb.ToString(), false, 3);
                procConvert.StartInfo.Arguments = sb.ToString();
                procConvert.StartInfo.UseShellExecute = false;
                procConvert.StartInfo.RedirectStandardOutput = true;
                procConvert.OutputDataReceived += new DataReceivedEventHandler(OutputDataHandler);
                procConvert.StartInfo.RedirectStandardError = true;
                procConvert.ErrorDataReceived += new DataReceivedEventHandler(OutputDataHandler);
                procConvert.Start();
                // Start the asynchronous read of the standard output stream.
                procConvert.BeginOutputReadLine();
                procConvert.BeginErrorReadLine();
                //Main.main.tab.SelectedTab = Main.main.tabPrint;
            }
            catch (Exception e)
            {
                Main.conn.log(e.ToString(), false, 2);
            }
        }*/
      /*  public void RunSliceExternal(string file, float centerx, float centery)
        {
            if (procConvert != null)
            {
                MessageBox.Show("Last slice job still running. Slicing of new job is canceled.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SlicingInfo.Start("External Slic3r");
            SlicingInfo.SetAction("Analyzing STL file ...");
            try
            {
                STL stl = new STL();
                stl.Load(file);
                stl.UpdateBoundingBox();
                if (stl.xMin > 0 && stl.yMin > 0 && stl.xMax < Main.printerSettings.PrintAreaWidth && stl.yMax < Main.printerSettings.PrintAreaDepth)
                {
                    // User assigned valid position, so we use this
                    centerx = stl.xMin + (stl.xMax - stl.xMin) / 2;
                    centery = stl.yMin + (stl.yMax - stl.yMin) / 2;
                }
                stl.Clear();
            }
            catch (Exception e)
            {
                Main.conn.log(e.ToString(), false, 2);
                SlicingInfo.Stop();
                return;
            }
            SlicingInfo.SetAction("Slicing STL file ...");
            procConvert = new Process();
            try
            {
                string basedir = (string)Main.main.repetierKey.GetValue("installPath", "");
                string exname = "slic3r.exe";
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                    exname = "bin" + Path.DirectorySeparatorChar + "slic3r";
                if (Main.IsMac)
                    exname = "MacOS" + Path.DirectorySeparatorChar + "slic3r";
                string exe = basedir + Path.DirectorySeparatorChar + "Slic3r" + Path.DirectorySeparatorChar + exname;
                if (File.Exists(BasicConfiguration.basicConf.ExternalSlic3rPath))
                    exe = BasicConfiguration.basicConf.ExternalSlic3rPath;

                slicefile = file;
                string target = StlToGCode(file);
                if (File.Exists(target))
                    File.Delete(target);
                procConvert.EnableRaisingEvents = true;
                procConvert.Exited += new EventHandler(ConversionExited);
                procConvert.StartInfo.FileName = Main.IsMono ? exe : wrapQuotes(exe);
                StringBuilder sb = new StringBuilder();
                sb.Append("--load ");
                sb.Append(wrapQuotes(BasicConfiguration.basicConf.ExternalSlic3rIniFile));
                sb.Append(" --print-center ");
                sb.Append(centerx.ToString("0", GCode.format));
                sb.Append(",");
                sb.Append(centery.ToString("0", GCode.format));
                sb.Append(" -o ");
                sb.Append(wrapQuotes(StlToGCode(file)));
                sb.Append(" ");
                sb.Append(wrapQuotes(file));
                procConvert.StartInfo.Arguments = sb.ToString();
                procConvert.StartInfo.UseShellExecute = false;
                procConvert.StartInfo.RedirectStandardOutput = true;
                procConvert.OutputDataReceived += new DataReceivedEventHandler(OutputDataHandler);
                procConvert.StartInfo.RedirectStandardError = true;
                procConvert.ErrorDataReceived += new DataReceivedEventHandler(OutputDataHandler);
                procConvert.Start();
                // Start the asynchronous read of the standard output stream.
                procConvert.BeginOutputReadLine();
                procConvert.BeginErrorReadLine();
                //Main.main.tab.SelectedTab = Main.main.tabPrint;
            }
            catch (Exception e)
            {
                Main.conn.log(e.ToString(), false, 2);
            }
        }*/
        public string findSlic3rExecutable()
        {
            if (File.Exists(BasicConfiguration.basicConf.Slic3rExecutable)) // use preconfigured
                return BasicConfiguration.basicConf.Slic3rExecutable;
            string[] possibleNames = { "slic3r.exe", "slic3r", "slic3r.pl" };
            string basedir = (string)Main.main.repetierKey.GetValue("installPath", "");
            string exe;
            foreach (string exname in possibleNames) // Search bundled version
            {
                exe = basedir + Path.DirectorySeparatorChar + "Slic3r" + Path.DirectorySeparatorChar + exname;
                if (File.Exists(exe)) return exe;
            }
            // Search in PATH environment var
            foreach (string test in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator)) {
                string path = test.Trim();
                foreach (string exname in possibleNames) // Search bundled version
                {
                    if (!String.IsNullOrEmpty(path) && File.Exists(Path.Combine(path, exname)))
                        return Path.GetFullPath(Path.Combine(path, exname));
                }
            }
            return null;
        }
        public bool RunSliceNew(string file, float centerx, float centery)
        {
            if (procConvert != null)
            {
                MessageBox.Show(Trans.T("L_LAST_SLICE_RUNNING"), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            string exe = findSlic3rExecutable();
            if (exe == null)
            {
                MessageBox.Show(Trans.T("L_SLIC3R_NOT_FOUND"), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            FormPrinterSettings ps = Main.printerSettings;
            SlicingInfo.Start("Slic3r");
            SlicingInfo.SetAction(Trans.T("L_ANALYSING_STL"));
            try
            {
                RHBoundingBox stl = Slicer.lastBox;
                if (stl.xMin > ps.BedLeft && stl.yMin > ps.BedFront && stl.xMax < ps.BedLeft + ps.PrintAreaWidth && stl.yMax < ps.BedFront+ps.PrintAreaDepth)
                {
                    // User assigned valid position, so we use this
                    centerx = (float)(stl.xMin + (stl.xMax - stl.xMin) / 2);
                    centery = (float)(stl.yMin + (stl.yMax - stl.yMin) / 2);
                }
            }
            catch (Exception e)
            {
                Main.conn.log(e.ToString(), false, 2);
                SlicingInfo.Stop();
                return false;
            }
            SlicingInfo.SetAction(Trans.T("L_SLICING_STL"));
            string dir = Main.globalSettings.Workdir;
            string config = dir + Path.DirectorySeparatorChar + "slic3r.ini";
            string cdir = Main.main.slicerPanel.slic3rDirectory;
            IniFile ini = new IniFile();
            //BasicConfiguration b = BasicConfiguration.basicConf;
            string fPrinter = cdir + Path.DirectorySeparatorChar + "print"+Path.DirectorySeparatorChar + Main.printerModel.Slic3rPrint + ".ini";
            ini.read(fPrinter);
            IniFile ini2 = new IniFile();
            ini2.read(cdir + Path.DirectorySeparatorChar + "printer" + Path.DirectorySeparatorChar + Main.printerModel.Slic3rPrinter + ".ini");
            IniFile ini3 = new IniFile();
            ini3.read(cdir + Path.DirectorySeparatorChar + "filament" + Path.DirectorySeparatorChar + Main.printerModel.Slic3rFilament1 + ".ini");
            IniFile ini3_2 = new IniFile();
            if(Main.conn.numberExtruder>1)
                ini3_2.read(cdir + Path.DirectorySeparatorChar + "filament" + Path.DirectorySeparatorChar + Main.printerModel.Slic3rFilament2 + ".ini");
            IniFile ini3_3 = new IniFile();
            if (Main.conn.numberExtruder > 2)
                ini3_3.read(cdir + Path.DirectorySeparatorChar + "filament" + Path.DirectorySeparatorChar + Main.printerModel.Slic3rFilament3 + ".ini");
            if (Main.conn.numberExtruder > 1)
                ini3.merge(ini3_2);
            if (Main.conn.numberExtruder > 2)
                ini3.merge(ini3_3);
            ini.add(ini2);
            ini.add(ini3);
            ini.flatten();
            ini.write(config);
            procConvert = new Process();
            try
            {
                string basedir = (string)Main.main.repetierKey.GetValue("installPath", "");
                /*string exname = "slic3r.exe";
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                    exname = "slic3r.pl";
                if (Main.IsMac)
                    exname = "MacOS" + Path.DirectorySeparatorChar + "slic3r";
                string exe = basedir + Path.DirectorySeparatorChar + "Slic3r" + Path.DirectorySeparatorChar + exname;
                if (File.Exists(BasicConfiguration.basicConf.Slic3rExecutable))
                    exe = BasicConfiguration.basicConf.Slic3rExecutable;*/
                slicefile = file;
                string target = StlToGCode(file);
                if (File.Exists(target))
                    File.Delete(target);
                procConvert.EnableRaisingEvents = true;
                procConvert.Exited += new EventHandler(ConversionExited);
                procConvert.StartInfo.FileName = Main.IsMono ? exe : wrapQuotes(exe);
                StringBuilder sb = new StringBuilder();
                sb.Append("--load ");
                sb.Append(wrapQuotes(config));
                sb.Append(" --print-center ");
                sb.Append(centerx.ToString("0", GCode.format));
                sb.Append(",");
                sb.Append(centery.ToString("0", GCode.format));
                sb.Append(" -o ");
                sb.Append(wrapQuotes(StlToGCode(file)));
                sb.Append(" ");
                sb.Append(wrapQuotes(file));
                RLog.info("Slic3r command:"+exe+" "+sb.ToString());
                procConvert.StartInfo.Arguments = sb.ToString();
                procConvert.StartInfo.UseShellExecute = false;
                procConvert.StartInfo.RedirectStandardOutput = true;
                procConvert.OutputDataReceived += new DataReceivedEventHandler(OutputDataHandler);
                procConvert.StartInfo.RedirectStandardError = true;
                procConvert.ErrorDataReceived += new DataReceivedEventHandler(OutputDataHandler);
                procConvert.Start();
                // Start the asynchronous read of the standard output stream.
                procConvert.BeginOutputReadLine();
                procConvert.BeginErrorReadLine();
                //Main.main.tab.SelectedTab = Main.main.tabPrint;
            }
            catch (Exception e)
            {
                Main.conn.log(e.ToString(), false, 2);
            }
            return true;
        }
        
        public string StlToGCode(string stl)
        {
            int p = stl.LastIndexOf('.');
            if (p > 0) stl = stl.Substring(0, p);
            return stl + ".gcode";
        }
        public delegate void LoadGCode(String myString);
        private void ConversionExited(object sender, System.EventArgs e)
        {
            if (procConvert == null) return;
            try
            {
                procConvert.Close();
                procConvert = null;
                string gcodefile = StlToGCode(slicefile);
                Main.slicer.Postprocess(gcodefile);
            }
            catch { }
        }
        private static void OutputDataHandler(object sendingProcess,
            DataReceivedEventArgs outLine)
        {
            // Collect the net view command output.
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                Main.conn.log("<Slic3r> "+outLine.Data, false, 4);
            }
        }

        private void buttonDeleteConfig_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete config " + config, "Security question", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string oc = config;
                comboConfig.Items.Remove(comboConfig.SelectedItem);
                rconfigs.DeleteSubKeyTree(config);
                if (comboConfig.Items.Count == 0)
                {
                    comboConfig.Items.Add("default");
                    comboConfig.SelectedIndex = 0;
                }
                else comboConfig.SelectedIndex = 0;
            }
        }

        private void comboConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadConfig(comboConfig.Text);
        }
    }
}
