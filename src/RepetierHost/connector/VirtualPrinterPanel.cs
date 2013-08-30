using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RepetierHost.model;

namespace RepetierHost.connector
{
    public partial class VirtualPrinterPanel : UserControl
    {
        VirtualPrinterConnector con = null;
        public VirtualPrinterPanel()
        {
            InitializeComponent();
            Main.main.languageChanged += Translate;
            Translate();
        }
        public void Connect(VirtualPrinterConnector con)
        {
            this.con = con;
            bindingConnection.DataSource = con;
            bindingConnection_CurrentItemChanged(null, null);
        }
        public void Translate()
        {
            labelBaudRate.Text = Trans.T("L_BAUD_RATE");
        }
        bool updating = false;
        private void bindingConnection_CurrentItemChanged(object sender, EventArgs e)
        {
            updating = true;
            comboBaud.Text = con.BaudRate;
            updating = false;
        }

        private void comboBaud_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updating || con==null) return;
            con.BaudRate = comboBaud.Text;
        }
    }
}
