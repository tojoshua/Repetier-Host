using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RepetierHost.model;

namespace RepetierHost.view.utils
{
    public partial class SlicingInfo : Form
    {
        public static SlicingInfo f = null;
        public static void Start(string slicerName) {
            if (f == null)
                f = new SlicingInfo();
            f.labelSlicer.Text = slicerName;
            f.ResetTimer();
            f.checkStartBoxAfterSlicing.Checked = false;
            if(f.Visible==false)
                f.Show(Main.main);
            f.timer.Start();
        }
        public static void Stop()
        {
            if (f != null)
            {
                f.timer.Stop();
                f.Hide();
            }
        }
        public MethodInvoker StopInfo = delegate
        {
            SlicingInfo.Stop();
        };
        public MethodInvoker PostprocInfo = delegate
        {
            SetAction("Postprocessing...");
        };
        public static void SetAction(string action)
        {
            f.labelAction.Text = action;
        }
        int min = 0;
        int sec = 0;
        public SlicingInfo()
        {
            InitializeComponent();
            translate();
            Main.main.languageChanged += translate;
        }
        void translate()
        {
            Text = Trans.T("W_SLICING_INFO");
            labelAction_.Text = Trans.T("L_ACTION:");
            labelDuration_.Text = Trans.T("L_DURATION:");
            labelSlicer_.Text = Trans.T("L_SLICER:");
            checkStartBoxAfterSlicing.Text = Trans.T("L_START_JOB_AFTER_SLICING");
        }
        private void ResetTimer() {
            min = sec = 0;
            labelDuration.Text = "0:00";
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            sec++;
            if(sec>=60) {
                sec = 0;
                min++;
            }
            labelDuration.Text = min.ToString() + ":" + sec.ToString("00");
        }
    }
}
