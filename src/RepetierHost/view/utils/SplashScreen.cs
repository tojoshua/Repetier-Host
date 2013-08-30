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

namespace RepetierHost.view.utils
{
    public partial class SplashScreen : Form
    {
        static string file;
        static SplashScreen splash = null;
        public static void run()
        {
            int dur = Custom.GetInteger("splashscreenDelay",0);
            if (dur <= 0) return;
            file = Application.StartupPath + Path.DirectorySeparatorChar + Custom.GetString("splashscreenImage", "doesnotexist");
            if (!File.Exists(file))
                return;
            splash = new SplashScreen();
            splash.Show();
            splash.timer.Interval = 1000 * dur;
            splash.timer.Start();
        }
        public SplashScreen()
        {
            InitializeComponent();
            Image img = Image.FromFile(file);
            BackgroundImage = img;
            Width = img.Width;
            Height = img.Height;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            Hide();
        }
    }
}
