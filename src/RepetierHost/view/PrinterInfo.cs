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
using System.Windows.Forms;
using RepetierHost.model;
using RepetierHost.view.utils;
using RepetierHost.connector;

namespace RepetierHost.view
{
    public partial class PrinterInfo : Form
    {
        PrinterConnection con;
        SerialConnector connector = null;
        public PrinterInfo()
        {
            con = Main.conn;
            InitializeComponent();
            RegMemory.RestoreWindowPos("printerInfoWindow", this);
            timer_Tick(null, null);
            translate();
            Main.main.languageChanged += translate;
        }
        public void ConnectWith(SerialConnector c) {
            connector = c;
        }
        private void translate()
        {
            Text = Trans.T("W_PRINTER_INFO");
            groupConnectionInformation.Text = Trans.T("L_CONNECTION_INFORMATION");
            groupPrinterData.Text = Trans.T("L_PRINTER_DATA");
            labBytesSend.Text = Trans.T("L_BYTES_SEND:");
            labErrorsReceived.Text = Trans.T("L_ERRORS_RECEIVED:");
            labFirmware.Text = Trans.T("L_FIRMWARE:");
            labFirmwareURL.Text = Trans.T("L_FIRMWARE_URL:");
            labLinesSend.Text = Trans.T("L_LINES_SEND:");
            labMachineType.Text = Trans.T("L_MACHINE_TYPE:");
            labNumberExtruder.Text = Trans.T("L_NUMBER_EXTRUDER:");
            labProtocol.Text = Trans.T("L_PROTOCOL:");
            buttonClose.Text = Trans.T("B_CLOSE");
        }
        /// <summary>
        /// Update the informations every second.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            if (con.connector.IsConnected() == false) return;
            labelMachine.Text = con.machine;
            labelFirmware.Text = con.firmware;
            labelFirmwareURL.Text = con.firmware_url;
            labelNumExtruder.Text = con.numberExtruder.ToString();
            labelProtocol.Text = con.protocol;
            if (connector != null)
            {
                labelLinesSend.Text = connector.linesSend.ToString();
                labelBytesSend.Text = connector.bytesSend.ToString();
                labelErrorsReceived.Text = connector.errorsReceived.ToString();
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void labelFirmwareURL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Main.main.openLink(labelFirmwareURL.Text);
        }

        private void PrinterInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            RegMemory.StoreWindowPos("printerInfoWindow", this, false, false);
        }

    }
}
