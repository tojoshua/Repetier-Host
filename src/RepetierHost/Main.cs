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
using RepetierHost.model;
using RepetierHost.view;
using RepetierHost.view.utils;
using RepetierHost.model.geom;
using Microsoft.Win32;
using System.Threading;
using System.Diagnostics;
using RepetierHost.model;
using RepetierHost.connector; 

namespace RepetierHost
{
    public delegate void executeHostCommandDelegate(GCode code);
    public delegate void languageChangedEvent();

    public partial class Main : Form
    {
        public event languageChangedEvent languageChanged;
        private const int InfoPanel2MinSize = 440;
        public static PrinterConnection conn;
        public static Main main;
        public static FormPrinterSettings printerSettings;
        public static PrinterModel printerModel;
        public static ThreeDSettings threeDSettings;
        public static GlobalSettings globalSettings = null;
        public static GCodeGenerator generator = null;
        public string basicTitle = "";
        public static bool IsMono = Type.GetType("Mono.Runtime") != null;
        public static Slicer slicer = null;
        public static Slic3r slic3r = null;
        public static bool IsMac = false;

        public Skeinforge skeinforge = null;
        public EEPROMRepetier eepromSettings = null;
        public EEPROMMarlin eepromSettingsm = null;
        public LogView logView = null;
        public PrintPanel printPanel = null;
        public RegistryKey repetierKey;
        public ThreeDControl threedview = null;
        public ThreeDView jobPreview = null;
        public ThreeDView printPreview = null;
        public GCodeVisual jobVisual = new GCodeVisual();
        public GCodeVisual printVisual = null;
        public STLComposer objectPlacement = null;
        public volatile GCodeVisual newVisual = null;
        public volatile bool jobPreviewThreadFinished = true;
        public volatile Thread previewThread = null;
        public RegMemory.FilesHistory fileHistory = new RegMemory.FilesHistory("fileHistory", 8);
        public int refreshCounter = 0;
        public executeHostCommandDelegate executeHostCall;
        bool recalcJobPreview = false;
        List<GCodeShort> previewArray0, previewArray1, previewArray2;
        public TemperatureHistory history = null;
        public TemperatureView tempView = null;
        public Trans trans = null;
        public RepetierHost.view.RepetierEditor editor;
        public double gcodePrintingTime = 0;
        public string lastFileLoadedName = null;

        public class JobUpdater
        {
            GCodeVisual visual = null;
            // This method will be called when the thread is started.
            public void DoWork()
            {
                RepetierEditor ed = Main.main.editor;
                
                Stopwatch sw = new Stopwatch();
                sw.Start();
                visual = new GCodeVisual();
                visual.showSelection = true;
                switch (ed.ShowMode)
                {
                    case 0:
                        visual.minLayer = 0;
                        visual.maxLayer = 999999;
                        break;
                    case 1:
                        visual.minLayer = visual.maxLayer = ed.ShowMinLayer;
                        break;
                    case 2:
                        visual.minLayer = ed.ShowMinLayer;
                        visual.maxLayer = ed.ShowMaxLayer;
                        break;
                }
                visual.parseGCodeShortArray(Main.main.previewArray0, true,0);
                visual.parseGCodeShortArray(Main.main.previewArray1, false,1);
                visual.parseGCodeShortArray(Main.main.previewArray2, false,2);
                Main.main.previewArray0 = Main.main.previewArray1 = Main.main.previewArray2 = null;
                visual.Reduce();
                Main.main.gcodePrintingTime = visual.ana.printingTime;
                //visual.stats();
                Main.main.newVisual = visual;
                Main.main.jobPreviewThreadFinished = true;
                Main.main.previewThread = null;
                sw.Stop();
                //Main.conn.log("Update time:" + sw.ElapsedMilliseconds, false, 3);
            }
        }
        //From Managed.Windows.Forms/XplatUI
        static bool IsRunningOnMac()
        {
            IntPtr buf = IntPtr.Zero;
            try
            {
                buf = System.Runtime.InteropServices.Marshal.AllocHGlobal(8192);
                // This is a hacktastic way of getting sysname from uname ()
                if (uname(buf) == 0)
                {
                    string os = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(buf);
                    if (os == "Darwin")
                        return true;
                }
            }
            catch
            {
            }
            finally
            {
                if (buf != IntPtr.Zero)
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(buf);
            }
            return false;
        }
        private void Main_Load(object sender, EventArgs e)
        {
        /*    RegMemory.RestoreWindowPos("mainWindow", this);
           // if (WindowState == FormWindowState.Maximized)
           //     Application.DoEvents(); // This crashes mono if run here
            splitLog.SplitterDistance = RegMemory.GetInt("logSplitterDistance", splitLog.SplitterDistance);
            splitInfoEdit.SplitterDistance = RegMemory.GetInt("infoEditSplitterDistance", Width - 470);
            //A bug causes the splitter to throw an exception if the PanelMinSize is set too soon.
            splitInfoEdit.Panel2MinSize = Main.InfoPanel2MinSize;
            //splitInfoEdit.SplitterDistance = (splitInfoEdit.Width - splitInfoEdit.Panel2MinSize);
         * */
        }
        [System.Runtime.InteropServices.DllImport("libc")]
        static extern int uname(IntPtr buf);
        public Main()
        {
            executeHostCall = new executeHostCommandDelegate(this.executeHostCommand);
            repetierKey = Custom.BaseKey; // Registry.CurrentUser.CreateSubKey("SOFTWARE\\Repetier");
            repetierKey.SetValue("installPath", Application.StartupPath);
            if (Path.DirectorySeparatorChar != '\\' && IsRunningOnMac())
                IsMac = true;
            /*String[] parms = Environment.GetCommandLineArgs();
            string lastcom = "";
            foreach (string s in parms)
            {
                if (lastcom == "-home")
                {
                    repetierKey.SetValue("installPath",s);
                    lastcom = "";
                    continue;
                }
                if (s == "-macosx") IsMac = true;
                lastcom = s;
            }*/
            main = this;
            SplashScreen.run();
            trans = new Trans(Application.StartupPath + Path.DirectorySeparatorChar + "data" + Path.DirectorySeparatorChar + "translations");
            SwitchButton.imageOffset = RegMemory.GetInt("onOffImageOffset", 0);
            generator = new GCodeGenerator();
            globalSettings = new GlobalSettings();
            conn = new PrinterConnection();
            printerSettings = new FormPrinterSettings();
            printerModel = new PrinterModel();
            conn.analyzer.start(true);
            threeDSettings = new ThreeDSettings();
            InitializeComponent();
            tdSettings.DataSource = threeDSettings;
            tdSettings_DataMemberChanged(null, null);
            editor = new RepetierEditor();
            editor.Dock = DockStyle.Fill;
            tabGCode.Controls.Add(editor);
            updateShowFilament();
            RegMemory.RestoreWindowPos("mainWindow", this);
            if (WindowState == FormWindowState.Maximized)
                Application.DoEvents();
            splitLog.SplitterDistance = RegMemory.GetInt("logSplitterDistance", splitLog.SplitterDistance);
            splitInfoEdit.SplitterDistance = RegMemory.GetInt("infoEditSplitterDistance", Width-470);
            if (IsMono)
            {
                if (!IsMac)
                {
                    foreach (ToolStripItem m in menu.Items)
                    {
                        m.Text = m.Text.Replace("&", null);
                    }
                }
                if (IsMac)
                {
                    /*Application.Events.Quit += delegate (object sender, ApplicationEventArgs e) {
                        Application.Quit ();
                        e.Handled = true;
                    };
 
                    ApplicationEvents.Reopen += delegate (object sender, ApplicationEventArgs e) {
                        WindowState = FormWindowState.Normal;
                        e.Handled = true;
                    };*/

                    MinimumSize = new Size(500, 640);
                    tab.MinimumSize = new Size(500, 500);
                    splitLog.Panel1MinSize = 520;
                    splitLog.Panel2MinSize = 100;
                    splitLog.IsSplitterFixed = true;
                    //splitContainerPrinterGraphic.SplitterDistance -= 52;
                    splitLog.SplitterDistance = splitLog.Height - 100;
                }
            }
            slicerToolStripMenuItem.Visible = false;
            splitLog.Panel2Collapsed = !RegMemory.GetBool("logShow", true);
            splitPrinterId.Panel1Collapsed = !RegMemory.GetBool("printerIdShow", false);
            conn.eventConnectionChange += OnPrinterConnectionChange;
            conn.eventPrinterAction += OnPrinterAction;
            conn.eventJobProgress += OnJobProgress;
            objectPlacement = new STLComposer();
            objectPlacement.Dock = DockStyle.Fill;
            tabModel.Controls.Add(objectPlacement);
            printPanel = new PrintPanel();
            printPanel.Dock = DockStyle.Fill;
            tabPrint.Controls.Add(printPanel);
            printerSettings.formToCon();
            logView = new LogView();
            logView.Dock = DockStyle.Fill;
            splitLog.Panel2.Controls.Add(logView);
            skeinforge = new Skeinforge();
            PrinterChanged(printerSettings.currentPrinterKey, true);
            printerSettings.eventPrinterChanged += PrinterChanged;
            // GCode print preview
            threedview = new ThreeDControl();
            threedview.Dock = DockStyle.Fill;
            tabPage3DView.Controls.Add(threedview);

            printPreview = new ThreeDView();
           // printPreview.Dock = DockStyle.Fill;
          //  splitContainerPrinterGraphic.Panel2.Controls.Add(printPreview);
            printPreview.SetEditor(false);
            printPreview.autoupdateable = true;
            printVisual = new GCodeVisual(conn.analyzer);
            printVisual.liveView = true;
            printPreview.models.AddLast(printVisual);
            basicTitle = Text;
            jobPreview = new ThreeDView();
         //   jobPreview.Dock = DockStyle.Fill;
         //   splitJob.Panel2.Controls.Add(jobPreview);
            jobPreview.SetEditor(false);
            jobPreview.models.AddLast(jobVisual);
            editor.contentChangedEvent += JobPreview;
            editor.commands = new Commands();
            editor.commands.Read("default", "en");
            UpdateHistory();
            UpdateConnections();
            Main.slic3r = new Slic3r();
            slicer = new Slicer();
            //toolShowLog_CheckedChanged(null, null);
            updateShowFilament();
            assign3DView();
            history = new TemperatureHistory();
            tempView = new TemperatureView();
            tempView.Dock = DockStyle.Fill;
            tabPageTemp.Controls.Add(tempView);
            if (IsMono)
            {
                showWorkdirectoryToolStripMenuItem.Visible = false;
                toolStrip.Height = 56;
            }
            new SoundConfig();
            //stlComposer1.buttonSlice.Text = Trans.T1("L_SLICE_WITH", slicer.SlicerName);

            // Customizations

            if(Custom.GetBool("removeTestgenerator",false)) {
                internalSlicingParameterToolStripMenuItem.Visible = false;
                testCaseGeneratorToolStripMenuItem.Visible = false;
            }
            string titleAdd = Custom.GetString("titleAddition", "");
            string titlePrefix = Custom.GetString("titlePrefix", "");
            if (titleAdd.Length > 0 || titlePrefix.Length>0)
            {
                int p = basicTitle.IndexOf(' ');
                basicTitle = titlePrefix+basicTitle.Substring(0, p) + titleAdd + basicTitle.Substring(p);
                Text = basicTitle;
            }
            slicerPanel.UpdateSelection();
            if (Custom.GetBool("removeUpdates", false))
                checkForUpdatesToolStripMenuItem.Visible = false;
            else
                RHUpdater.checkForUpdates(true);
            UpdateToolbarSize();
            // Add languages
            foreach (Translation t in trans.translations.Values)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(t.language,null, languageSelected);
                item.Tag = t;
                languageToolStripMenuItem.DropDownItems.Add(item);
            }
            languageChanged += translate;
            translate();
            if (Custom.GetBool("removeSkeinforge", false))
            {
                Main.slicer.ActiveSlicer = Slicer.SlicerID.Slic3r;
            }
            if (Custom.GetBool("extraSupportButton", false))
            {
                supportToolStripMenuItem.Text = Custom.GetString("extraSupportText", "Support");
            }
            else
            {
                toolStripAskSeperator.Visible = false;
                supportToolStripMenuItem.Visible = false;
            }
            if (Custom.GetString("extraLink1Title", "").Length>0)
            {
                extraUrl1ToolStripMenuItem.Text = Custom.GetString("extraLink1Title", "");
                toolStripAskSeperator.Visible = true;
            }
            else extraUrl1ToolStripMenuItem.Visible = false;
            if (Custom.GetString("extraLink2Title", "").Length > 0)
            {
                extraUrl2ToolStripMenuItem.Text = Custom.GetString("extraLink2Title", "");
                toolStripAskSeperator.Visible = true;
            }
            else extraUrl2ToolStripMenuItem.Visible = false;
            if (Custom.GetString("extraLink3Title", "").Length > 0)
            {
                extraUrl3ToolStripMenuItem.Text = Custom.GetString("extraLink3Title", "");
                toolStripAskSeperator.Visible = true;
            }
            else extraUrl3ToolStripMenuItem.Visible = false;
            if (Custom.GetString("extraLink4Title", "").Length > 0)
            {
                extraUrl4ToolStripMenuItem.Text = Custom.GetString("extraLink4Title", "");
                toolStripAskSeperator.Visible = true;
            }
            else extraUrl4ToolStripMenuItem.Visible = false;
            if (Custom.GetString("extraLink5Title", "").Length > 0)
            {
                extraUrl5ToolStripMenuItem.Text = Custom.GetString("extraLink5Title", "");
                toolStripAskSeperator.Visible = true;
            }
            else extraUrl5ToolStripMenuItem.Visible = false;
            string supportImage = Custom.GetString("extraSupportToolbarImage", "");
            if (supportImage.Length > 0 && File.Exists(Application.StartupPath + Path.DirectorySeparatorChar + supportImage))
            {
                toolStripButtonSupport.Image = Image.FromFile(Application.StartupPath + Path.DirectorySeparatorChar + Custom.GetString("extraSupportToolbarImage", ""));
                toolStripButtonSupport.Text = Custom.GetString("extraSupportText", "Support");
            }
            else
            {
                toolStripButtonSupport.Visible = false;
            }
            toolAction.Text = Trans.T("L_IDLE");
            toolConnection.Text = Trans.T("L_DISCONNECTED");
            updateTravelMoves();
            printerIdLabel.Text = printerSettings.comboPrinter.Text;
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
            extensions.ExtensionManager.Initalize();
            if (conn.connector != null)
                conn.connector.Activate();
            //TestTopoTriangle triTests = new TestTopoTriangle();
            //triTests.RunTests();

            //everything done.  Now look at command line
            ProcessCommandLine();


        }

        void ProcessCommandLine()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length < 1) return;
             
            //for now, just check the last arg and load it. Could add other inputs/commands later.
            for (int i = 1; i < args.Length; i++)
            {
                string file = args[i];
                if (File.Exists(file))
                {
                    LoadGCodeOrSTL(file);
                }
            }
         }
        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files) LoadGCodeOrSTL(file);
        }
        public void translate()
        {
            fileToolStripMenuItem.Text = Trans.T("M_FILE");
            settingsToolStripMenuItem.Text = Trans.T("M_CONFIG");
            slicerToolStripMenuItem.Text = Trans.T("M_SLICER");
            printerToolStripMenuItem.Text = Trans.T("M_PRINTER");
            temperatureToolStripMenuItem.Text = Trans.T("M_TEMPERATURE");
            helpToolStripMenuItem.Text = Trans.T("M_HELP");
            loadGCodeToolStripMenuItem.Text = Trans.T("M_LOAD_GCODE");
            showWorkdirectoryToolStripMenuItem.Text = Trans.T("M_SHOW_WORKDIRECTORY");
            languageToolStripMenuItem.Text = Trans.T("M_LANGUAGE");
            printerSettingsToolStripMenuItem.Text = Trans.T("M_PRINTER_SETTINGS");
            eeprom.Text = Trans.T("M_EEPROM_SETTINGS");
            threeDSettingsMenu.Text = Trans.T("M_3D_VIEWER_CONFIGURATION");
            repetierSettingsToolStripMenuItem.Text = Trans.T("M_REPETIER_SETTINGS");
            internalSlicingParameterToolStripMenuItem.Text = Trans.T("M_TESTCASE_SETTINGS");
            soundConfigurationToolStripMenuItem.Text = Trans.T("M_SOUND_CONFIGURATION");
            showExtruderTemperaturesMenuItem.Text = Trans.T("M_SHOW_EXTRUDER_TEMPERATURES");
            showHeatedBedTemperaturesMenuItem.Text = Trans.T("M_SHOW_HEATED_BED_TEMPERATURES");
            showTargetTemperaturesMenuItem.Text = Trans.T("M_SHOW_TARGET_TEMPERATURES");
            showAverageTemperaturesMenuItem.Text = Trans.T("M_SHOW_AVERAGE_TEMPERATURES");
            showHeaterPowerMenuItem.Text = Trans.T("M_SHOW_HEATER_POWER");
            autoscrollTemperatureViewMenuItem.Text = Trans.T("M_AUTOSCROLL_TEMPERATURE_VIEW");
            timeperiodMenuItem.Text = Trans.T("M_TIMEPERIOD");
            temperatureZoomMenuItem.Text = Trans.T("M_TEMPERATURE_ZOOM");
            buildAverageOverMenuItem.Text = Trans.T("M_BUILD_AVERAGE_OVER");
            secondsToolStripMenuItem.Text = Trans.T("M_30_SECONDS");
            minuteToolStripMenuItem.Text = Trans.T("M_1_MINUTE");
            minuteToolStripMenuItem1.Text = Trans.T("M_1_MINUTE");
            minutesToolStripMenuItem.Text = Trans.T("M_2_MINUTES");
            minutesToolStripMenuItem1.Text = Trans.T("M_5_MINUTES");
            minutes5ToolStripMenuItem.Text = Trans.T("M_5_MINUTES");
            minutes10ToolStripMenuItem.Text = Trans.T("M_10_MINUTES");
            minutes15ToolStripMenuItem.Text = Trans.T("M_15_MINUTES");
            minutes30ToolStripMenuItem.Text = Trans.T("M_30_MINUTES");
            minutes60ToolStripMenuItem.Text = Trans.T("M_60_MINUTES");
            continuousMonitoringMenuItem.Text = Trans.T("M_CONTINUOUS_MONITORING");
            disableToolStripMenuItem.Text = Trans.T("M_DISABLE");
            extruder1ToolStripMenuItem.Text = Trans.T("M_EXTRUDER_1");
            extruder2ToolStripMenuItem.Text = Trans.T("M_EXTRUDER_2");
            heatedBedToolStripMenuItem.Text = Trans.T("M_HEATED_BED");
            printerInformationsToolStripMenuItem.Text = Trans.T("M_PRINTER_INFORMATION");
            jobStatusToolStripMenuItem.Text = Trans.T("M_JOB_STATUS");
            menuSDCardManager.Text = Trans.T("M_SD_CARD_MANAGER");
            testCaseGeneratorToolStripMenuItem.Text = Trans.T("M_TEST_CASE_GENERATOR");
            sendScript1ToolStripMenuItem.Text = Trans.T("M_SEND_SCRIPT_1");
            sendScript2ToolStripMenuItem.Text = Trans.T("M_SEND_SCRIPT_2");
            sendScript3ToolStripMenuItem.Text = Trans.T("M_SEND_SCRIPT_3");
            sendScript4ToolStripMenuItem.Text = Trans.T("M_SEND_SCRIPT_4");
            sendScript5ToolStripMenuItem.Text = Trans.T("M_SEND_SCRIPT_5");
            repetierHostHomepageToolStripMenuItem.Text = Trans.T("M_REPETIER_HOST_HOMEPAGE");
            repetierHostDownloadPageToolStripMenuItem.Text = Trans.T("M_REPETIER_HOST_DOWNLOAD_PAGE");
            manualToolStripMenuItem.Text = Trans.T("M_MANUAL");
            slic3rHomepageToolStripMenuItem.Text = Trans.T("M_SLIC3R_HOMEPAGE");
            skeinforgeHomepageToolStripMenuItem.Text = Trans.T("M_SKEINFORGE_HOMEPAGE");
            repRapWebsiteToolStripMenuItem.Text = Trans.T("M_REPRAP_WEBSITE");
            repRapForumToolStripMenuItem.Text = Trans.T("M_REPRAP_FORUM");
            thingiverseNewestToolStripMenuItem.Text = Trans.T("M_THINGIVERSE_NEWEST");
            thingiversePopularToolStripMenuItem.Text = Trans.T("M_THINGIVERSE_POPULAR");
            aboutRepetierHostToolStripMenuItem.Text = Trans.T("M_ABOUT_REPETIER_HOST");
            checkForUpdatesToolStripMenuItem.Text = Trans.T("M_CHECK_FOR_UPDATES");
            quitToolStripMenuItem.Text = Trans.T("M_QUIT");
            donateToolStripMenuItem.Text = Trans.T("M_DONATE");
            tabPage3DView.Text = Trans.T("TAB_3D_VIEW");
            tabPageTemp.Text = Trans.T("TAB_TEMPERATURE_CURVE");
            tabModel.Text = Trans.T("TAB_OBJECT_PLACEMENT");
            tabSlicer.Text = Trans.T("TAB_SLICER");
            tabGCode.Text = Trans.T("TAB_GCODE_EDITOR");
            tabPrint.Text = Trans.T("TAB_MANUAL_CONTROL");
            toolPrinterSettings.Text = Trans.T("M_PRINTER_SETTINGS");
            toolPrinterSettings.ToolTipText = Trans.T("M_PRINTER_SETTINGS");
            toolStripEmergencyButton.Text = Trans.T("M_EMERGENCY_STOP");
            toolStripSDCard.Text = Trans.T("M_SD_CARD");
            toolStripSDCard.ToolTipText = Trans.T("L_SD_CARD_MANAGEMENT");
            toolStripEmergencyButton.ToolTipText = Trans.T("M_EMERGENCY_STOP");
            toolLoad.Text = Trans.T("M_LOAD");
            toolStripSaveJob.Text = Trans.T("M_SAVE_JOB");
            toolKillJob.Text = Trans.T("M_KILL_JOB");
            toolStripSDCard.Text = Trans.T("M_SD_CARD");
            toolShowLog.Text = toolShowLog.ToolTipText = Trans.T("M_TOGGLE_LOG");
            toolShowFilament.Text = Trans.T("M_SHOW_FILAMENT");
            if (conn.connector.IsConnected())
            {
                toolConnect.ToolTipText = Trans.T("L_DISCONNECT_PRINTER"); // "Disconnect printer";
                toolConnect.Text = Trans.T("M_DISCONNECT"); // "Disconnect";
            }
            else
            {
                toolConnect.ToolTipText = Trans.T("L_CONNECT_PRINTER"); // "Connect printer";
                toolConnect.Text = Trans.T("M_CONNECT"); // "Connect";
            }
            if (threeDSettings.checkDisableFilamentVisualization.Checked)
            {
                toolShowFilament.ToolTipText = Trans.T("L_FILAMENT_VISUALIZATION_DISABLED"); // "Filament visualization disabled";
                toolShowFilament.Text = Trans.T("M_SHOW_FILAMENT"); // "Show filament";
            }
            else
            {
                toolShowFilament.ToolTipText = Trans.T("L_FILAMENT_VISUALIZATION_ENABLED"); // "Filament visualization enabled";
                toolShowFilament.Text = Trans.T("M_HIDE_FILAMENT"); // "Hide filament";
            }
            if (!conn.connector.IsJobRunning())
            {
                Main.main.toolRunJob.ToolTipText = Trans.T("M_RUN_JOB"); // "Run job";
                Main.main.toolRunJob.Text = Trans.T("M_RUN_JOB"); //"Run job";
            }
            else
            {
                Main.main.toolRunJob.ToolTipText = Trans.T("M_PAUSE_JOB"); //"Pause job";
                Main.main.toolRunJob.Text = Trans.T("M_PAUSE_JOB"); //"Pause job";
            }
            toolLoad.ToolTipText = Trans.T("L_LOAD_FILE"); // Load file
            toolStripSaveJob.ToolTipText = Trans.T("M_SAVE_JOB");
            openGCode.Title = Trans.T("L_IMPORT_G_CODE"); // Import G-Code
            saveJobDialog.Title = Trans.T("L_SAVE_G_CODE"); //Save G-Code
            isometricToolStripMenuItem.Text = Trans.T("L_ISOMETRIC_VIEW");
            topViewToolStripMenuItem.Text = Trans.T("L_TOP_VIEW");
            bottomViewToolStripMenuItem.Text = Trans.T("L_BOTTOM_VIEW");
            leftViewToolStripMenuItem.Text = Trans.T("L_LEFT_VIEW");
            rightViewToolStripMenuItem.Text = Trans.T("L_RIGHT_VIEW");
            frontViewToolStripMenuItem.Text = Trans.T("L_FRONT_VIEW");
            backViewToolStripMenuItem.Text = Trans.T("L_BACK_VIEW");
            viewToolStripMenuItem.Text = Trans.T("M_VIEW");
            showEdgesToolStripMenuItem.Text = Trans.T("M_SHOW_EDGES");
            showFacesToolStripMenuItem.Text = Trans.T("M_SHOW_FACES");
            showCompassToolStripMenuItem.Text = Trans.T("L_SHOW_COMPASS");
            toolsToolStripMenuItem.Text = Trans.T("M_TOOLS");
            beltCalculatorToolStripMenuItem.Text = Trans.T("M_BELT_CALCULATOR");
            leadscrewCalculatorToolStripMenuItem.Text = Trans.T("M_LEADSCREW_CALCULATOR");
            fitPrinterToolStripMenuItem.Text = Trans.T("M_FIT_PRINTER");
            fitObjectsToolStripMenuItem.Text = Trans.T("M_FIT_OBJECTS");
            snapshotToolStripMenuItem.Text = Trans.T("M_POSPONED_JOBS");
            loadStateToolStripMenuItem.Text = Trans.T("M_RESUME_JOB");
            saveStateToolStripMenuItem.Text = Trans.T("M_POSTPONE_JOB");
            togglePrinterIdToolStripMenuItem.Text = Trans.T("M_TOGGLE_PRINTER_ID");
            updateTravelMoves();
            updateShowFilament();
            foreach (ToolStripMenuItem item in languageToolStripMenuItem.DropDownItems)
            {
                item.Checked = item.Tag == trans.active;
            }
        }
        public void UpdateToolbarSize()
        {
            if (globalSettings == null) return;
            bool mini = globalSettings.ReduceToolbarSize;
            foreach (ToolStripItem it in toolStrip.Items)
            {
                if (mini)
                    it.DisplayStyle = ToolStripItemDisplayStyle.Image;
                else
                    it.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            }
        }
        private void languageSelected(object sender, EventArgs e)
        {
            ToolStripItem it = (ToolStripItem)sender;
            trans.selectLanguage((Translation)it.Tag);
            if (languageChanged != null)
                languageChanged();
        }
        public void UpdateConnections()
        {
            toolConnect.DropDownItems.Clear();
            foreach (string s in printerSettings.printerKey.GetSubKeyNames())
            {
                toolConnect.DropDownItems.Add(s, null, ConnectHandler);
            }
            foreach (ToolStripItem it in toolConnect.DropDownItems)
                it.Enabled = !conn.connector.IsConnected();
        }
        public void UpdateHistory()
        {
            bool delFlag = false;
            LinkedList<ToolStripItem> delArray = new LinkedList<ToolStripItem>();
            int pos = 0;
            foreach (ToolStripItem c in fileToolStripMenuItem.DropDownItems)
            {
                if (c == toolStripEndHistory) break;
                if (!delFlag) pos++;
                if (c == toolStripStartHistory)
                {
                    delFlag = true;
                    continue;
                }
                if (delFlag)
                    delArray.AddLast(c);
            }
            foreach (ToolStripItem i in delArray)
                fileToolStripMenuItem.DropDownItems.Remove(i);
            toolLoad.DropDownItems.Clear();
            foreach (RegMemory.HistoryFile f in fileHistory.list)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(); // You would obviously calculate this value at runtime
                item = new ToolStripMenuItem();
                item.Name = "file" + pos;
                item.Tag = f;
                item.Text = f.ToString();
                item.Click += new EventHandler(HistoryHandler);
                fileToolStripMenuItem.DropDownItems.Insert(pos++, item);
                item = new ToolStripMenuItem();
                item.Name = "filet" + pos;
                item.Tag = f;
                item.Text = f.ToString();
                item.Click += new EventHandler(HistoryHandler);
                toolLoad.DropDownItems.Add(item);
            }
        }

        private void HistoryHandler(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            RegMemory.HistoryFile f = (RegMemory.HistoryFile)clickedItem.Tag;
            LoadGCodeOrSTL(f.file);
            // Take some action based on the data in clickedItem
        }
        private void ConnectHandler(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            printerSettings.load(clickedItem.Text);
            printerSettings.formToCon();
            slicerPanel.UpdateSelection();
            printerSettings.UpdateDimensions();
            Update3D();
            conn.open();
        }
        public void PrinterChanged(RegistryKey pkey, bool printerChanged)
        {
            if (printerChanged && editor!=null)
            {
                editor.UpdatePrependAppend();
            }
        }
        public string Title
        {
            set { Text = basicTitle + " - " + value; }
            get { return Text; }
        }
        public void FormToFront(Form f)
        {
            // Make this form the active form and make it TopMost
            //f.ShowInTaskbar = false;
            /*f.TopMost = true;
            f.Focus();*/
            f.BringToFront();
            // f.TopMost = false;
        }
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void OnPrinterConnectionChange(string msg)
        {
            toolConnection.Text = msg;
            bool connected = conn.connector.IsConnected();
            sendScript1ToolStripMenuItem.Enabled = connected;
            sendScript2ToolStripMenuItem.Enabled = connected;
            sendScript3ToolStripMenuItem.Enabled = connected;
            sendScript4ToolStripMenuItem.Enabled = connected;
            sendScript5ToolStripMenuItem.Enabled = connected;
            saveStateToolStripMenuItem.Enabled = connected;
            if (connected)
            {
                toolConnect.Image = imageList.Images[0];
                toolConnect.ToolTipText = Trans.T("L_DISCONNECT_PRINTER"); // "Disconnect printer";
                toolConnect.Text = Trans.T("M_DISCONNECT"); // "Disconnect";
                foreach (ToolStripItem it in toolConnect.DropDownItems)
                    it.Enabled = false;
                //eeprom.Enabled = true;
                toolStripEmergencyButton.Enabled = true;
                //toolPrinterSettings.Enabled = false;
                printerSettingsToolStripMenuItem.Enabled = false;
            }
            else
            {
                toolConnect.Image = imageList.Images[1];
                toolConnect.ToolTipText = Trans.T("L_CONNECT_PRINTER"); // "Connect printer";
                toolConnect.Text = Trans.T("M_CONNECT"); // "Connect";
                eeprom.Enabled = false;
                continuousMonitoringMenuItem.Enabled = false;
                if (eepromSettings != null && eepromSettings.Visible)
                {
                    eepromSettings.Close();
                    eepromSettings.Dispose();
                    eepromSettings = null;
                }
                if (eepromSettingsm != null && eepromSettingsm.Visible)
                {
                    eepromSettingsm.Close();
                    eepromSettingsm.Dispose();
                    eepromSettingsm = null;
                }
                foreach (ToolStripItem it in toolConnect.DropDownItems)
                    it.Enabled = true;
                toolStripEmergencyButton.Enabled = false;
                //toolPrinterSettings.Enabled = true;
                printerSettingsToolStripMenuItem.Enabled = true;
                SDCard.Disconnected();
            }
        }
        private void OnPrinterAction(string msg)
        {
            toolAction.Text = msg;
        }
        private void OnJobProgress(float per)
        {
            toolProgress.Value = (int)per;
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (conn.isRepetier)
            {
                if (eepromSettings != null)
                {
                    if (eepromSettings.Visible)
                    {
                        eepromSettings.BringToFront();
                        return;
                    }
                    else
                    {
                        eepromSettings.Dispose();
                        eepromSettings = null;
                    }
                }
                if (eepromSettings == null)
                    eepromSettings = new EEPROMRepetier();
                eepromSettings.Show2();
            }
            if (conn.isMarlin)
            {
                if (eepromSettingsm != null)
                {
                    if (eepromSettingsm.Visible)
                    {
                        eepromSettingsm.BringToFront();
                        return;
                    }
                    else
                    {
                        eepromSettingsm.Dispose();
                        eepromSettingsm = null;
                    }
                }
                if (eepromSettingsm == null)
                    eepromSettingsm = new EEPROMMarlin();
                eepromSettingsm.Show2();
            }
        }



        private void toolGCodeLoad_Click(object sender, EventArgs e)
        {
            if (openGCode.ShowDialog() == DialogResult.OK)
            {
                LoadGCodeOrSTL(openGCode.FileName);
            }
        }
        public void LoadGCodeOrSTL(string file)
        {
            if (!File.Exists(file)) return;
            FileInfo f = new FileInfo(file);
            Title = f.Name;
            lastFileLoadedName = Path.GetFileNameWithoutExtension(f.Name);
            fileHistory.Save(file);
            UpdateHistory();
            string fileLow = file.ToLower();
            if (fileLow.EndsWith(".stl") || fileLow.EndsWith(".obj"))
            {
              /*  if (MessageBox.Show("Do you want to slice the STL-File? No adds it to the object grid.", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    slicer.RunSlice(file); // Slice it and load
                }
                else
                {*/
                    tab.SelectTab(tabModel);
                    objectPlacement.openAndAddObject(file);
                //}
            }
            else
            {
                try {
                    tab.SelectTab(tabGCode);
                    editor.selectContent(0);
                    editor.setContent(0, System.IO.File.ReadAllText(file));
                }
                catch (System.IO.IOException ex)
                {
                    MessageBox.Show("Error encountered while processing file. " + ex.Message);
                }
            }
        }
        public void LoadGCode(string file)
        {
            try
            {
                editor.setContent(0, System.IO.File.ReadAllText(file));
                tab.SelectTab(tabGCode);
                editor.selectContent(0);
                fileHistory.Save(file);
                UpdateHistory();
            }
            catch (System.IO.FileNotFoundException)
            {
                GCodeNotFound.execute(file);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void LoadGCodeText(string text)
        {
            try
            {
                editor.setContent(0, text);
                tab.SelectTab(tabGCode);
                editor.selectContent(0);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public MethodInvoker StartJob = delegate
        {
            Main.main.toolPrintJob_Click(null, null);
        };
        private void toolPrintJob_Click(object sender, EventArgs e)
        {
            if (conn.connector.IsJobRunning())
                conn.connector.PauseJob(Trans.T("L_PAUSE_MSG"));
            else
            {
                Main.main.printVisual.Clear();
                conn.connector.RunJob();
            }
            /*
            Printjob job = conn.job;
            if (job.dataComplete)
            {
                conn.pause(Trans.T("L_PAUSE_MSG")); //"Press OK to continue.\n\nYou can add pauses in your code with\n@pause Some text like this");
            }
            else
            {
                tab.SelectedTab = tabPrint;
                //conn.analyzer.StartJob();
                toolRunJob.Image = imageList.Images[3];
                job.BeginJob();
                job.PushGCodeShortArray(editor.getContentArray(1));
                job.PushGCodeShortArray(editor.getContentArray(0));
                job.PushGCodeShortArray(editor.getContentArray(2));
                job.EndJob();
            }*/
        }



        private void toolKillJob_Click(object sender, EventArgs e)
        {
            conn.connector.KillJob();
            //conn.job.KillJob();
        }

        private void printerSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printerSettings.Show(this);
            FormToFront(printerSettings);
        }

        private void skeinforgeSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            skeinforge.Show();
            skeinforge.BringToFront();
        }

        private void skeinforgeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            skeinforge.RunSkeinforge();
        }

        private void threeDSettingsMenu_Click(object sender, EventArgs e)
        {
            threeDSettings.Show();
            threeDSettings.BringToFront();
        }
        public PrinterInfo printerInfo = null;
        private void printerInformationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (printerInfo == null)
                printerInfo = new PrinterInfo();
            printerInfo.Show();
            printerInfo.BringToFront();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (conn.connector.IsJobRunning())
            {
                if (MessageBox.Show(Trans.T("L_REALLY_QUIT"), Trans.T("L_SECURITY_QUESTION"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
            if (!conn.close())
            {
                e.Cancel = true;
                return;
            }
            RegMemory.StoreWindowPos("mainWindow", this, true, true);
            RegMemory.SetInt("logSplitterDistance", splitLog.SplitterDistance);
            RegMemory.SetInt("infoEditSplitterDistance", splitInfoEdit.SplitterDistance);

            RegMemory.SetBool("logShow", !splitLog.Panel2Collapsed);
            RegMemory.SetBool("printerIdShow", !splitPrinterId.Panel1Collapsed);

            if (previewThread != null)
                previewThread.Join();
            conn.Destroy();
        }

        private void repetierSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            globalSettings.Show(this);
            globalSettings.BringToFront();
        }
        public About about = null;
        private void aboutRepetierHostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (about == null) about = new About();
            about.Show(this);
            about.BringToFront();
        }

        private void jobStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JobStatus.ShowStatus();
        }
        public void openLink(string link)
        {
            try
            {
                System.Diagnostics.Process.Start(link);
            }
            catch
            (
            System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }
        private void repetierHostHomepageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLink("http://www.repetier.com");
        }

        private void manualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLink("http://www.repetier.com/documentation/repetier-host/");
        }
        public MethodInvoker FirmwareDetected = delegate
        {
            Main.main.printPanel.UpdateConStatus(true);
            if (conn.isRepetier)
            {
                Main.main.continuousMonitoringMenuItem.Enabled = true;
            }
        };
        public MethodInvoker UpdateJobButtons = delegate
        {
            if (!conn.connector.IsJobRunning())
            {
                Main.main.toolKillJob.Enabled = false;
                Main.main.toolRunJob.Enabled = conn.connector.IsConnected();
                Main.main.toolRunJob.ToolTipText = Trans.T("M_RUN_JOB"); // "Run job";
                Main.main.toolRunJob.Text = Trans.T("M_RUN_JOB"); //"Run job";
                Main.main.toolRunJob.Image = Main.main.imageList.Images[2];
            }
            else
            {                
                Main.main.toolRunJob.Enabled = true;
                Main.main.toolKillJob.Enabled = true;
                Main.main.toolRunJob.Image = Main.main.imageList.Images[3];
                Main.main.toolRunJob.ToolTipText = Trans.T("M_PAUSE_JOB"); //"Pause job";
                Main.main.toolRunJob.Text = Trans.T("M_PAUSE_JOB"); //"Pause job";
            }
        };
        public MethodInvoker UpdateEEPROM = delegate
        {
            if (conn.isMarlin || conn.isRepetier || conn.isSprinter) // Activate special menus and function
            {
                main.eeprom.Enabled = true;
            }
            else main.eeprom.Enabled = false;

        };
        /*  private void toolStripSaveGCode_Click(object sender, EventArgs e)
          {
              if (saveJobDialog.ShowDialog() == DialogResult.OK)
              {
                  System.IO.File.WriteAllText(saveJobDialog.FileName, textGCode.Text, Encoding.Default);
              }
          }

          private void toolStripSavePrepend_Click(object sender, EventArgs e)
          {
              printerSettings.currentPrinterKey.SetValue("gcodePrepend", textGCodePrepend.Text);
          }

          private void toolStripSaveAppend_Click(object sender, EventArgs e)
          {
              printerSettings.currentPrinterKey.SetValue("gcodeAppend", textGCodeAppend.Text);
          }*/
        private void JobPreview()
        {
            if (editor.autopreview == false) return;
            /*       if (splitJob.Panel2Collapsed)
                   {
                       splitJob.Panel2Collapsed = false;
                       splitJob.SplitterDistance = 300;
                       jobPreview = new ThreeDControl();
                       jobPreview.Dock = DockStyle.Fill;
                       splitJob.Panel2.Controls.Add(jobPreview);
                       jobPreview.SetEditor(false);
                       jobPreview.models.AddLast(jobVisual);
                       //jobPreview.SetObjectSelected(false);
                   }*/
            /* Read the initial time. */
            recalcJobPreview = true;
            /*DateTime startTime = DateTime.Now;
            jobVisual.ParseText(editor.getContent(1), true);
            jobVisual.ParseText(editor.getContent(0), false);
            jobVisual.ParseText(editor.getContent(2), false);
            DateTime stopTime = DateTime.Now;
            TimeSpan duration = stopTime - startTime;
            Main.conn.log(duration.ToString(), false, 3);
            jobPreview.UpdateChanges();*/
        }
        public void Update3D()
        {
            if(threedview!=null)
                threedview.UpdateChanges();
        }

        private void testCaseGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TestGenerator.Execute();
        }

        private void internalSlicingParameterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SlicingParameter.Execute();
        }

        private void toolStripSDCard_Click(object sender, EventArgs e)
        {
            SDCard.Execute();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (newVisual != null)
            {
                jobPreview.models.RemoveLast();
                jobVisual.Clear();
                jobVisual = newVisual;
                jobPreview.models.AddLast(jobVisual);
                threedview.UpdateChanges();
                newVisual = null;
                editor.toolUpdating.Text = "";
                if (Main.main.gcodePrintingTime > 0)
                {
                    Main.main.editor.printingTime = Main.main.gcodePrintingTime;
                    int sec = (int)(Main.main.editor.printingTime * (1 + 0.01 * Main.conn.addPrintingTime));
                    int hours = sec / 3600;
                    sec -= 3600 * hours;
                    int min = sec / 60;
                    sec -= min * 60;
                    StringBuilder s = new StringBuilder();
                    if (hours > 0)
                        s.Append(Trans.T1("L_TIME_H:", hours.ToString())); //"h:");
                    if (min > 0)
                        s.Append(Trans.T1("L_TIME_M:", min.ToString()));
                    s.Append(Trans.T1("L_TIME_S", sec.ToString()));
                    Main.main.editor.toolPrintingTime.Text = Trans.T1("L_PRINTING_TIME:", s.ToString());
                }

                editor.UpdateLayerInfo();
                editor.MaxLayer = editor.getContentArray(0).Last<GCodeShort>().layer;
            }
            if (recalcJobPreview && jobPreviewThreadFinished)
            {
                previewArray0 = new List<GCodeShort>();
                previewArray1 = new List<GCodeShort>();
                previewArray2 = new List<GCodeShort>();
                previewArray0.AddRange(((RepetierEditor.Content)editor.toolFile.Items[1]).textArray);
                previewArray1.AddRange(((RepetierEditor.Content)editor.toolFile.Items[0]).textArray);
                previewArray2.AddRange(((RepetierEditor.Content)editor.toolFile.Items[2]).textArray);
                recalcJobPreview = false;
                jobPreviewThreadFinished = false;
                JobUpdater workerObject = new JobUpdater();
                editor.toolUpdating.Text = Trans.T("L_UPDATING..."); // "Updating ...";
                previewThread = new Thread(workerObject.DoWork);
                previewThread.Start();

            }
            if (refreshCounter > 0)
            {
                if (--refreshCounter == 0)
                {
                    Invalidate();
                }
            }
        }

        private void toolConnect_Click(object sender, EventArgs e)
        {
            if (conn.connector.IsConnected())
            {
                conn.close();
            }
            else
            {
                conn.open();
            }
        }

        private void toolShowLog_Click(object sender, EventArgs e)
        {
            if (splitLog.Panel2Collapsed == true)
            {
                splitLog.Panel2Collapsed = false;
            }
            else
            {
                splitLog.Panel2Collapsed = true;
            }            
            //toolShowLog.Checked = !toolShowLog.Checked;
        }

        private void toolShowLog_CheckedChanged(object sender, EventArgs e)
        {
            if (splitLog.Panel2Collapsed == true)
            {
                splitLog.Panel2Collapsed = false;
            }
            else
            {
                splitLog.Panel2Collapsed = true;
            }
        }

        private void repRapWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLink("http://www.reprap.org");
        }

        private void repRapForumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLink("http://forum.reprap.org");
        }

        private void slic3rHomepageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLink("http://www.slic3r.org");

        }

        private void skeinforgeHomepageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLink("http://fabmetheus.crsndoo.com/");

        }

        private void thingiverseNewestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLink("http://www.thingiverse.com/newest");

        }

        private void thingiversePopularToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLink("http://www.thingiverse.com/popular");

        }

        private void slic3rToolStripMenuItem_Click(object sender, EventArgs e)
        {
            slicer.ActiveSlicer = Slicer.SlicerID.Slic3r;
            //stlComposer1.buttonSlice.Text = Trans.T1("L_SLICE_WITH", slicer.SlicerName);
        }

        private void skeinforgeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            slicer.ActiveSlicer = Slicer.SlicerID.Skeinforge;
            //stlComposer1.buttonSlice.Text = Trans.T1("L_SLICE_WITH", slicer.SlicerName);
        }

        private void slic3rConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            slic3r.Show();
            slic3r.BringToFront();
        }
        public void assign3DView()
        {
            if (tab == null) return;
            switch (tab.SelectedIndex)
            {
                case 0:
                case 1:
                    threedview.SetView(objectPlacement.cont);
                    break;
                case 2:
                    threedview.SetView(jobPreview);
                    break;
                case 3:
                    threedview.SetView(printPreview);
                    break;
            }
        }
        private void tab_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Console.WriteLine("index changed " + Environment.OSVersion.Platform + " Mac=" + PlatformID.MacOSX);
            //if (Environment.OSVersion.Platform == PlatformID.MacOSX )
            if (IsMac)
            {
                // In MacOSX the OpenGL windows shine through the
                // tabs, so we need to disable all GL windows except the active.
                if (tab.SelectedTab != tabModel)
                {
                    if (tabModel.Controls.Contains(objectPlacement))
                    {
                        tabModel.Controls.Remove(objectPlacement);
                    }
                }
                if (tab.SelectedTab == tabModel)
                {
                    if (!tabModel.Controls.Contains(objectPlacement))
                        tabModel.Controls.Add(objectPlacement);
                }
                refreshCounter = 6;
            }
            if (tab.SelectedTab == tabModel || tab.SelectedTab == tabSlicer)
            {
                tabControlView.SelectedIndex = 0;
            }
            assign3DView();
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            if (IsMac)
            {
                if (Height < 740) Height = 740;
                refreshCounter = 8;
                Application.DoEvents();
                /*             Invalidate();
                             Application.DoEvents();
                             tab.SelectedTab.Invalidate();*/
            }
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            if (!globalSettings.WorkdirOK())
                globalSettings.Show();
            if (Custom.GetBool("showGCodeExample", false) && RegMemory.GetBool("gcodeExampleShown", false) == false)
            {
                string file = Application.StartupPath + Path.DirectorySeparatorChar + Custom.GetString("GCodeExample", "");
                if (File.Exists(file))
                    LoadGCodeOrSTL(file);
                RegMemory.SetBool("gcodeExampleShown", true);
            }
        }
        public void executeHostCommand(GCode code)
        {
            string com = code.getHostCommand();
            string param = code.getHostParameter();
            if (com.Equals("@info"))
            {
                conn.log(param, false, 3);
            }
            else if (com.Equals("@pause"))
            {
                SoundConfig.PlayPrintPaused(false);
                conn.pause(param);
            }
            else if (com.Equals("@sound"))
            {
                SoundConfig.PlaySoundCommand(false);
            }
            else if (com.Equals("@execute"))
            {
                CommandExecutioner ce = new CommandExecutioner();
                ce.setExeArgs(code.getHostParameter());
                ce.run();
            }
        }
        public void updateShowFilament()
        {
            if (threeDSettings.checkDisableFilamentVisualization.Checked)
            {
                toolShowFilament.Image = imageList.Images[5];
                toolShowFilament.ToolTipText = Trans.T("L_FILAMENT_VISUALIZATION_DISABLED"); // "Filament visualization disabled";
                toolShowFilament.Text = Trans.T("M_HIDE_FILAMENT"); // "Show filament";
            }
            else
            {
                toolShowFilament.Image = imageList.Images[4];
                toolShowFilament.ToolTipText = Trans.T("L_FILAMENT_VISUALIZATION_ENABLED"); // "Filament visualization enabled";
                toolShowFilament.Text = Trans.T("M_SHOW_FILAMENT"); // "Hide filament";
            }
        }
        public void updateTravelMoves()
        {
            if (threeDSettings == null) return;
            if (threeDSettings.checkDisableTravelMoves.Checked)
            {
                toolShowTravel.Image = imageList.Images[5];
                toolShowTravel.ToolTipText = Trans.T("L_TRAVEL_VISUALIZATION_DISABLED"); // "Travel visualization disabled";
                toolShowTravel.Text = Trans.T("M_HIDE_TRAVEL"); // "Hide Travel";
            }
            else
            {
                toolShowTravel.Image = imageList.Images[4];
                toolShowTravel.ToolTipText = Trans.T("L_TRAVEL_VISUALIZATION_ENABLED"); // "Travel visualization enabled";
                toolShowTravel.Text = Trans.T("M_SHOW_TRAVEL"); // "Show Travel";
            }
            toolShowTravel.Visible = threeDSettings.drawMethod == 2;
        }
        private void toolShowFilament_Click(object sender, EventArgs e)
        {
            threeDSettings.checkDisableFilamentVisualization.Checked = !threeDSettings.checkDisableFilamentVisualization.Checked;
            // updateShowFilament();
        }

        private void toolStripEmergencyButton_Click(object sender, EventArgs e)
        {
            if (!conn.connector.IsConnected()) return;
            conn.connector.Emergency();
            //conn.injectManualCommandFirst("M112");
            /*conn.job.KillJob();
            conn.close(true);
            conn.open();
            return;*/
            /*RLog.info("Old dtr:" + conn.serial.DtrEnable);
            conn.serial.DtrEnable = true;
            RLog.info("Old dtr:" + conn.serial.DtrEnable);
            //conn.serial.RtsEnable = true;
            Thread.Sleep(400);
            RLog.info("Old dtr:" + conn.serial.DtrEnable);
            //conn.serial.RtsEnable = false;
            conn.serial.DtrEnable = false;
            RLog.info("Old dtr:" + conn.serial.DtrEnable);
            //Thread.Sleep(200);
            //conn.serial.DtrEnable = false;
            conn.log(Trans.T("L_EMERGENCY_STOP_MSG"), false, 3);
            while (conn.hasInjectedMCommand(112))
            {
                Application.DoEvents();
            }*/
            //conn.close();
        }

        private void killSlicingProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            skeinforge.KillSlice();
            slic3r.KillSlice();
            SlicingInfo.Stop();
        }

        private void externalSlic3rSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Slic3rSetup.Execute();
        }

        private void externalSlic3rToolStripMenuItem_Click(object sender, EventArgs e)
        {
            slicer.ActiveSlicer = Slicer.SlicerID.Slic3rExternal;
            //stlComposer1.buttonSlice.Text = Trans.T1("L_SLICE_WITH", slicer.SlicerName);
        }

        private void externalSlic3rConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            slic3r.RunExternalConfig();
        }

        private void Main_Activated(object sender, EventArgs e)
        {
            objectPlacement.recheckChangedFiles();
            slicerPanel.UpdateSelection();
        }
        public void selectTimePeriod(object sender, EventArgs e)
        {
            history.CurrentPos = (int)((ToolStripMenuItem)sender).Tag;
        }
        public void selectAverage(object sender, EventArgs e)
        {
            history.AvgPeriod = int.Parse(((ToolStripMenuItem)sender).Tag.ToString());
        }
        public void selectZoom(object sender, EventArgs e)
        {
            history.CurrentZoomLevel = int.Parse(((ToolStripMenuItem)sender).Tag.ToString());
        }

        private void showExtruderTemperaturesMenuItem_Click(object sender, EventArgs e)
        {
            history.ShowExtruder = !history.ShowExtruder;
        }

        private void showHeatedBedTemperaturesMenuItem_Click(object sender, EventArgs e)
        {
            history.ShowBed = !history.ShowBed;
        }

        private void showTargetTemperaturesMenuItem_Click(object sender, EventArgs e)
        {
            history.ShowTarget = !history.ShowTarget;
        }

        private void showAverageTemperaturesMenuItem_Click(object sender, EventArgs e)
        {
            history.ShowAverage = !history.ShowAverage;
        }

        private void showHeaterPowerMenuItem_Click(object sender, EventArgs e)
        {
            history.ShowOutput = !history.ShowOutput;
        }

        private void autoscrollTemperatureViewMenuItem_Click(object sender, EventArgs e)
        {
            history.Autoscroll = !history.Autoscroll;
        }

        private void disableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            conn.injectManualCommand("M203 S255");
        }

        private void extruder1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            conn.injectManualCommand("M203 S0");
        }

        private void extruder2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            conn.injectManualCommand("M203 S1");
        }

        private void heatedBedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            conn.injectManualCommand("M203 S100");
        }

        private void showWorkdirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Main.globalSettings.Workdir))
                Process.Start("explorer.exe", Main.globalSettings.Workdir);
        }

        private void soundConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SoundConfig.config.ShowDialog();
        }

        private void toolStripSaveJob_Click(object sender, EventArgs e)
        {
            StoreCode.Execute();
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (tabControlView.SelectedIndex == 0)
            {
                threedview.ThreeDControl_KeyDown(sender, e);
            }
        }

        public void repetierHostDownloadPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLink(Custom.GetString("downloadUrl","http://www.repetier.com/download/"));
        }

        private void sendScript1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (GCodeShort code in editor.getContentArray(5))
            {
                conn.injectManualCommand(code.text);
            }
        }

        private void sendScript2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (GCodeShort code in editor.getContentArray(6))
            {
                conn.injectManualCommand(code.text);
            }
        }

        private void sendScript3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (GCodeShort code in editor.getContentArray(7))
            {
                conn.injectManualCommand(code.text);
            }
        }

        private void sendScript4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (GCodeShort code in editor.getContentArray(8))
            {
                conn.injectManualCommand(code.text);
            }
        }

        private void sendScript5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (GCodeShort code in editor.getContentArray(9))
            {
                conn.injectManualCommand(code.text);
            }
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RHUpdater.checkForUpdates(false);
        }


        static bool firstSizeCall = true;
        private void Main_SizeChanged(object sender, EventArgs e)
        {
            if(firstSizeCall) {
                firstSizeCall = false;
                splitLog.SplitterDistance = RegMemory.GetInt("logSplitterDistance", splitLog.SplitterDistance);
                splitInfoEdit.SplitterDistance = RegMemory.GetInt("infoEditSplitterDistance", Width - 470);
                //A bug causes the splitter to throw an exception if the PanelMinSize is set too soon.
                splitInfoEdit.Panel2MinSize = Main.InfoPanel2MinSize;
            }
        }

        private void donateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLink("http://www.repetier.com/donate-or-support/");
        }

        private void toolShowTravel_Click(object sender, EventArgs e)
        {
            threeDSettings.checkDisableTravelMoves.Checked = !threeDSettings.checkDisableTravelMoves.Checked;
            threeDSettings.FormToRegistry();
        }

        private void slicerPanel_Load(object sender, EventArgs e)
        {

        }

        private void toolAction_Click(object sender, EventArgs e)
        {
            conn.connector.ToggleETAMode();
        }

        private void supportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLink(Custom.GetString("extraSupportURL", "http://www.repetier.com"));
        }

        private void isometricToolStripMenuItem_Click(object sender, EventArgs e)
        {
            threedview.isometricView();
        }

        private void frontViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            threedview.frontView();
        }

        private void leftViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            threedview.leftView();
        }

        private void rightViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            threedview.rightView();
        }

        private void backViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            threedview.backView();
        }

        private void topViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            threedview.topView();
        }

        private void bottomViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            threedview.bottomView();
        }

        private void tdSettings_DataMemberChanged(object sender, EventArgs e)
        {
            showEdgesToolStripMenuItem.Checked = threeDSettings.ShowEdges;
            showFacesToolStripMenuItem.Checked = threeDSettings.ShowFaces;
            showCompassToolStripMenuItem.Checked = threeDSettings.ShowCompass;
        }

        private void showEdgesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            threeDSettings.ShowEdges = !threeDSettings.ShowEdges;
        }

        private void showFacesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            threeDSettings.ShowFaces = !threeDSettings.ShowFaces;
        }

        public void tdSettings_CurrentChanged(object sender, EventArgs e)
        {
            showEdgesToolStripMenuItem.Checked = threeDSettings.ShowEdges;
            showFacesToolStripMenuItem.Checked = threeDSettings.ShowFaces;
            showCompassToolStripMenuItem.Checked = threeDSettings.ShowCompass;
        }

        private void beltCalculatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BeltCalculatorDialog d = new BeltCalculatorDialog();
            d.Show(this);
        }

        private void leadscrewCalculatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LeadScrewCalculatorDialog d = new LeadScrewCalculatorDialog();
            d.Show(this);
        }

        private void fitPrinterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            threedview.FitPrinter();
        }

        private void fitObjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            threedview.FitObjects();
        }

        private void loadStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //ValidatePreconditionsToLoadStateSnapshot(Main.conn);

                PendingPrintJobsDialog dialog = new PendingPrintJobsDialog();
                dialog.ShowDialog();

                PendingPrintJob job = dialog.GetSelectedJob();
                if (job == null)
                {
                    // User cancelled
                    return;
                }
                PrintingStateSnapshot state = job.GetSnapshot(); //LoadStateFile();
                lastFileLoadedName = job.Name;
                this.Title = job.Name;

                GCodeExecutor executor = new PrinterConnectionGCodeExecutor(conn, false);
                state.RestoreState(executor);
                Main.main.Invoke(Main.main.UpdateJobButtons);
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.ToString(), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.ToString(), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private OnPosChange SaveStateOnNewLayerDelegate;
        private PrinterConnectorBase.OnPauseChanged SaveStateOnPauseDelegate;
        private SnapshotDialog snapshotDialog;
        // NOTE: Used an array for the lock object because strings could be
        // immutable.
        private string[] lockObject = new string[0];
        private string snapshotNameOnNextSaveState;
        private void saveStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ValidatePreconditionsToSaveStateSnapshot(Main.conn))
            {
                // conditions not met
                return;
            }

            string snapshotName = ReadSnapshotName();
            if (snapshotName == null)
            {
                // User cancelled
                return;
            }

            double lastZ = Main.conn.analyzer.z;
            lock (lockObject)
            {
                // We save the state when the first of these three events happen:
                // - Print job is paused (if it's paused at the moment the user
                // tries to take the snapshot, then it's taken at that moment)
                // - A new layer is reached.
                // - The user forces the snapshot from the snapshot dialog.

                SaveStateOnNewLayerDelegate = new OnPosChange(delegate(GCode gc, float x, float y, float z)
                {
                    if (z != lastZ)
                    {
                        // New Layer, ready to save state.
                        OnReadyToSaveStateCallback();
                    }
                });

                SaveStateOnPauseDelegate = new PrinterConnectorBase.OnPauseChanged(delegate(bool paused)
                {
                    if (paused)
                    {
                        OnReadyToSaveStateCallback();
                    }
                });

                snapshotNameOnNextSaveState = snapshotName;
                Main.conn.analyzer.eventPosChanged += SaveStateOnNewLayerDelegate;
                Main.conn.connector.eventPauseChanged += SaveStateOnPauseDelegate;
                if (snapshotDialog == null)
                {
                    // FIXME this can lead to a race condition
                    snapshotDialog = new SnapshotDialog();
                }

                if (Main.conn.connector.IsPaused)
                {
                    // In this case we mustn't wait until the new layer is
                    // reached, but instead we must take the snapshot now.
                    OnReadyToSaveStateCallback();
                }
            }
            snapshotDialog.Show();
        }



        private bool ValidatePreconditionsToSaveStateSnapshot(PrinterConnection conn)
        {
            return conn.connector.IsJobRunning();
        }

        internal void CancelSaveState()
        {
            lock (lockObject)
            {
                if (SaveStateOnNewLayerDelegate != null)
                {
                    Main.conn.analyzer.eventPosChanged -= SaveStateOnNewLayerDelegate;
                    Main.conn.connector.eventPauseChanged -= SaveStateOnPauseDelegate;
                    SaveStateOnNewLayerDelegate = null;
                    SaveStateOnPauseDelegate = null;
                    if (snapshotDialog != null)
                    {
                        // FIXME this can lead to a race condition
                        snapshotDialog.Close();
                        snapshotDialog = null;
                    }
                }
            }
        }

        public void OnReadyToSaveStateCallback()
        {
            string snapshotName;
            lock (lockObject)
            {
                if (SaveStateOnNewLayerDelegate == null)
                {
                    // Already cancelled.
                    return;
                }

                // keep snapshot name
                snapshotName = snapshotNameOnNextSaveState;

                // We want the event to execute only once, so, we must remove it
                // after the condition was met.
                Main.conn.analyzer.eventPosChanged -= SaveStateOnNewLayerDelegate;
                Main.conn.connector.eventPauseChanged -= SaveStateOnPauseDelegate;
                SaveStateOnNewLayerDelegate = null;
                SaveStateOnPauseDelegate = null;
                snapshotNameOnNextSaveState = null;
                if (snapshotDialog != null)
                {
                    // FIXME this can lead to a race condition
                    snapshotDialog.Close();
                    snapshotDialog = null;
                }
            }
            // Capture state, so that any movement we do later is not affected.
            PrintingStateSnapshot state = SnapshotFactory.TakeSnapshot(Main.conn);

            // Kill job. This will move the extruder to prevent melting the
            // object.

            conn.connector.KillJob();
            try
            {
                SaveStateFile(state, snapshotName);
                MessageBox.Show(Trans.T("L_PRINT_STATE_SAVED_SUCCESSFULLY"));
            }

            catch (IOException ex)
            {
                MessageBox.Show(ex.ToString(), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.ToString(), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void SaveStateFile(PrintingStateSnapshot state, string snapshotName)
        {
            // Check if there's already a file with that name. The file would be overwritten.
            //bool fileExists = (PendingPrintJobs.GetPendingJobWithName(snapshotName) != null);
            PendingPrintJobs.Add(state, snapshotName);
        }

        /// <summary>
        /// Show the user a dialog requesting a snapshot name.
        /// If the name is iinvalid, ask him again until he sets a right name.
        /// If the user cancels, returns null.
        /// </summary>
        /// <returns></returns>
        private static string ReadSnapshotName()
        {
            // Propose as snapshot name, one that contains a timestamp.
            string snapshotName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            do
            {
                snapshotName = StringInput.GetString(Trans.T("L_POSTPONED_JOB_NAME"), Trans.T("L_NAME_POSTPONED_JOB"), snapshotName, true);
                if (snapshotName == null)
                {
                    // User cancelled.
                    return null;
                }
            } while (PendingPrintJob.IsInvalidSnapshotName(snapshotName));
            return snapshotName;
        }

        private void printerIdLabel_DoubleClick(object sender, EventArgs e)
        {
            EditInstanceName.Execute();
            /*
            string printerId = StringInput.GetString(Trans.T("L_PRINTER_ID"), Trans.T("L_SET_PRINTER_ID"), printerIdLabel.Text, true);
            if (printerId != null)
            {
                printerIdLabel.Text = printerId;
                ColorDialog picker = new ColorDialog();
                picker.Color = printerIdLabel.BackColor;
                if (picker.ShowDialog() == DialogResult.OK)
                {
                    printerIdLabel.BackColor = picker.Color;
                }
            }*/
        }
        private void togglePrinterIdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitPrinterId.Panel1Collapsed = !splitPrinterId.Panel1Collapsed;
        }
        private void extraUrl1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLink(Custom.GetString("extraLink1URL", ""));
        }

        private void extraUrl2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLink(Custom.GetString("extraLink2URL", ""));
        }

        private void extraUrl3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLink(Custom.GetString("extraLink3URL", ""));
        }

        private void extraUrl4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLink(Custom.GetString("extraLink4URL", ""));
        }

        private void extraUrl5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLink(Custom.GetString("extraLink5URL", ""));
        }

        private void showCompassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            threeDSettings.ShowCompass = !threeDSettings.ShowCompass;
        }
    }
}
