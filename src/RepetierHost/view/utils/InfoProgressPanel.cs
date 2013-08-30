using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RepetierHost.view.utils
{
    public partial class InfoProgressPanel : Panel
    {
        private bool killed = false;
        private string lastAction = "dddf";
        private string action = "";
        private int progress = 0, lastProgress = 0;
        private Object mutex = new Object();

        public static InfoProgressPanel Create(string title, bool killable)
        {
            InfoProgressPanel p = new InfoProgressPanel();
            if (!killable)
                p.Visible = false;
            p.labelTitle.Text = title;
            p.timer.Start();
            return p;
        }
        public void Finished()
        {
            if (Parent != null)
                Parent.Controls.Remove(this);
            timer.Stop();
        }
        public InfoProgressPanel()
        {
            InitializeComponent();
        }
        public string Action
        {
            get { return action; }
            set
            {
                lock (mutex)
                {
                    action = value;
                }
            }
        }
        public int Progress
        {
            get { lock(mutex) return progress; }
            set { lock(mutex) progress = value;}
        }
        public bool IsKilled {get {return killed;}}
        private void timer_Tick(object sender, EventArgs e)
        {
            lock (mutex)
            {
                if (action != lastAction)
                {
                    labelAction.Text = lastAction = action;
                }
                if (lastProgress != progress)
                {
                    progressBar.Value = lastProgress = progress;
                }
            }
        }

        private void buttonKill_Click(object sender, EventArgs e)
        {
            killed = true;
        }
    }
}
