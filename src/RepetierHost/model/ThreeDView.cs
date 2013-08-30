using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;
using OpenTK;
using RepetierHost.model;

namespace RepetierHost.view
{
    public delegate void onObjectMoved(float dx, float dy);
    public delegate void onObjectSelected(ThreeDModel selModel);

    public class ThreeDView
    {
        FormPrinterSettings ps = Main.printerSettings;
        public onObjectMoved eventObjectMoved;
        public onObjectSelected eventObjectSelected;
        //public float zoom = 1.0f;
        //public Vector3 viewCenter;
        //public Vector3 userPosition;
        //public Matrix4 lookAt, persp, modelView;
        //public double normX = 0, normY = 0;
        //public float nearDist, farDist, aspectRatio, nearHeight;
        //public float rotZ = 0, rotX = 0;
        //public int mode = 0;
        public bool editor = false;
        public bool autoupdateable = false;
        public int slowCounter = 0; // Indicates slow framerates
        public uint timeCall = 0;
        public bool objectsSelected = false;
        public LinkedList<ThreeDModel> models;

        public ThreeDView()
        {
            //viewCenter = new Vector3(0, 0, 0);
            //rotX = 20;
            //userPosition = new Vector3(0f * ps.PrintAreaWidth, -1.6f * (float)Math.Sqrt(ps.PrintAreaDepth * ps.PrintAreaDepth + ps.PrintAreaWidth * ps.PrintAreaWidth + ps.PrintAreaHeight * ps.PrintAreaHeight), 0.0f * ps.PrintAreaHeight);
            //userPosition = new Vector3(0, -1.7f * (float)Math.Sqrt(ps.PrintAreaDepth * ps.PrintAreaDepth + ps.PrintAreaWidth * ps.PrintAreaWidth), 0.0f * ps.PrintAreaHeight);
            models = new LinkedList<ThreeDModel>();
        }
        public void SetEditor(bool ed)
        {
            editor = ed;
        }

    }
}
