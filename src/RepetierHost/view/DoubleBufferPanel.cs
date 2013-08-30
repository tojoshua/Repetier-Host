using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RepetierHost.view
{
        public class DoubleBufferPanel :Control
        {
            public DoubleBufferPanel()
            {
                // Set the value of the double-buffering style bits to true.
                this.SetStyle(ControlStyles.DoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint | ControlStyles.AllPaintingInWmPaint
                |ControlStyles.Selectable,
                true);

                this.UpdateStyles();
            }
            protected override bool ProcessKeyPreview(ref Message m)
            {
                return true;
                //return base.ProcessKeyPreview(ref m);
            }
            protected override bool IsInputKey(System.Windows.Forms.Keys
keyData)
            {
                return true;
            }
        }
}
