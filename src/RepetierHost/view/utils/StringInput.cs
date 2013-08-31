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
    public partial class StringInput : Form
    {
        public static string GetString(string head, string info)
        {
            StringInput ip = new StringInput();
            ip.Text = head;
            ip.labelInfo.Text = info;
            ip.ShowDialog();
            string r = ip.textBox1.Text;
            ip.Dispose(true);
            return r;
        }
        public StringInput()
        {
            InitializeComponent();
            buttonOK.Text = Trans.T("B_OK");
        }
    }
}
