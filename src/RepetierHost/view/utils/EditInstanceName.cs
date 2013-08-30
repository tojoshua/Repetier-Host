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
    public partial class EditInstanceName : Form
    {
        static EditInstanceName form = null;

        public static void Execute()
        {
            if (form == null)
            {
                form = new EditInstanceName();
            }
            form.Open();
        }
        public EditInstanceName()
        {
            InitializeComponent();
        }
        public void Open()
        {
            labelInstanceName.Text = Trans.T("L_PRINTER_ID");
            labelColor.Text = Trans.T("L_BGCOLOR:");
            buttonOK.Text = Trans.T("B_OK");
            buttonCancel.Text = Trans.T("B_CANCEL");
            Text = Trans.T("L_EDIT_INSTANCE_NAME");
            textName.Text = Main.main.printerIdLabel.Text;
            panelColor.BackColor = Main.main.printerIdLabel.BackColor;
            Show(Main.main);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        } 

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Main.main.printerIdLabel.BackColor = panelColor.BackColor;
            Main.main.printerIdLabel.Text = textName.Text;
            Hide();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void panelColor_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                panelColor.BackColor = colorDialog.Color;
            }
        }
    }
}
