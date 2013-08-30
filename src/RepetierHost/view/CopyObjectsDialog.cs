using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RepetierHost.model;

namespace RepetierHost.view
{
    public partial class CopyObjectsDialog : Form
    {
        public CopyObjectsDialog()
        {
            InitializeComponent();
            Text = Trans.T("W_COPY_MARKED_OBJECTS");
            labelNumberOfCopies.Text = Trans.T("L_NUMBER_OF_COPIES:");
            checkAutoposition.Text = Trans.T("L_AUTO_POSITION_AFTER_COPY");
            buttonCancel.Text = Trans.T("B_CANCEL");
            buttonCopy.Text = Trans.T("B_COPY");
        }
    }
}
