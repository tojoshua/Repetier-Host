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
        private bool cancelled = false;
        public static string GetString(string head, string info)
        {
            return GetString(head, info, "", false);
        }
        public static string GetString(string head, string info, string defaultValue, bool allowCancel)
        {
            StringInput ip = new StringInput();
            ip.buttonCancel.Visible = allowCancel;
            ip.buttonOK.Left = ip.buttonCancel.Left - (allowCancel ? ip.buttonOK.Width : 0);
            ip.Text = head;
            ip.textBox1.Text = defaultValue;
            ip.labelInfo.Text = info;
            ip.ShowDialog();
            string r = ip.cancelled?null:ip.textBox1.Text;
            ip.Dispose(true);
            return r;
        }
        public StringInput()
        {
            InitializeComponent();
            buttonOK.Text = Trans.T("B_OK");
            buttonCancel.Text = Trans.T("B_CANCEL");
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case (Keys.Escape):
                    Cancel();
                    break;
                case (Keys.Return):
                    this.DialogResult = DialogResult.OK;
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void Cancel()
        {
            cancelled = true;
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
