using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using RepetierHost.model;

namespace RepetierHost.connector
{
    public partial class SerialConnectionPanel : UserControl
    {
        SerialConnector con;
        public SerialConnectionPanel()
        {
            InitializeComponent();
            Main.main.languageChanged += Translate;
            Translate();
        }
        public void Connect(SerialConnector con)
        {
            this.con = con;
            bindingConnection.DataSource = con;
            bindingConnection_CurrentItemChanged(null, null);
        }
        public void Translate()
        {
            labelBaudRate.Text = Trans.T("L_BAUD_RATE");
            labelCacheSizeHint.Text = Trans.T("L_CACHE_SIZE_HINT");
            labelConnectionInfo.Text = Trans.T("L_CONNECTION_INFO");
            labelPort.Text = Trans.T("L_PORT");
            labelReceiveCacheSize.Text = Trans.T("L_RECEIVE_CACHE_SIZE");
            labelTransferProtocol.Text = Trans.T("L_TRANSFER_PROTOCOL");
            checkPingPong.Text = Trans.T("L_PING_PONG_MODE");
            buttonRefreshPorts.Text = Trans.T("B_REFRESH_PORTS");
            labelResetOnConnect.Text = Trans.T("L_RESET_ON_CONNECT");
            labelResetOnEmergency.Text = Trans.T("L_RESET_ON_EMERGENCY");
            comboResetOnConnect.Items[0] = Trans.T("L_DISABLED");
            comboResetOnConnect.Items[1] = Trans.T("L_DTR_LOW_HIGH");
            comboResetOnConnect.Items[2] = Trans.T("L_DTR_LOW_HIGH_LOW");
            comboResetOnConnect.Items[3] = Trans.T("L_DTR_TOGGLE");
            comboResetOnEmergency.Items[0] = Trans.T("L_SEND_EMERGENCY_CMD");
            comboResetOnEmergency.Items[1] = Trans.T("L_SEND_EMERGENCY_CMD_DTR_TOGGLE");
            comboResetOnEmergency.Items[2] = Trans.T("L_SEND_EMERGENCY_CMD_RECONNECT");
        }
        public void UpdatePorts()
        {
            comboPort.Items.Clear();
            //comboPort.Items.Add("Virtual Printer");
            if (Main.IsMono && Environment.OSVersion.Platform == PlatformID.Unix)
            {
                DirectoryInfo di = new DirectoryInfo("/dev");
                FileInfo[] list = di.GetFiles("tty*");
                foreach (FileInfo info in list)
                    comboPort.Items.Add(info.FullName);
            }
            else
            {
                foreach (string p in SerialPort.GetPortNames())
                {
                    comboPort.Items.Add(p);
                }
            }
        }

        private void buttonRefreshPorts_Click(object sender, EventArgs e)
        {
            UpdatePorts();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {

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
        bool updating = false;
        private void bindingConnection_CurrentItemChanged(object sender, EventArgs e)
        {
            updating = true;
            comboBaud.Text = con.BaudRate;
            comboTransferProtocol.SelectedIndex = con.Protocol;
            comboResetOnConnect.SelectedIndex = con.ResetOnConnect;
            comboResetOnEmergency.SelectedIndex = con.ResetOnEmergency;
            updating = false;
        }

        private void comboBaud_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updating) return;
            con.BaudRate = comboBaud.Text;
        }

        private void comboTransferProtocol_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updating) return;
            con.Protocol = comboTransferProtocol.SelectedIndex;
        }

        private void comboResetOnConnect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updating) return;
            con.ResetOnConnect = comboResetOnConnect.SelectedIndex;
        }

        private void comboResetOnEmergency_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updating) return;
            con.ResetOnEmergency = comboResetOnEmergency.SelectedIndex;
        }

        private void SerialConnectionPanel_Load(object sender, EventArgs e)
        {
            UpdatePorts();
        }

    }
}
