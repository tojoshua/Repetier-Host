using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RepetierHost.model;
using RepetierHost.model.geom;

namespace RepetierHost.view
{
    public partial class ObjectInformation : Form
    {
        static public void Execute(PrintModel pm)
        {
            ObjectInformation info = new ObjectInformation();
            info.Analyse(pm);
            info.Show(Main.main);
        }
        public void Analyse(PrintModel pm) {
            TopoModel m = new TopoModel();
            m.Merge(pm.Model, pm.trans);
            infoVolume.Text = (0.001*m.Volume()).ToString("0.0000") + " cm³";
            infoSurface.Text = (0.01 * m.Surface()).ToString("0.0000") + " cm²";
            infoShells.Text = pm.Model.shells.ToString();
            infoPoints.Text = pm.Model.vertices.Count.ToString();
            infoEdges.Text = pm.Model.edges.Count.ToString();
            infoFaces.Text = pm.Model.triangles.Count.ToString();
            infoMinX.Text = m.boundingBox.minPoint.x.ToString("0.00")+" mm";
            infoMaxX.Text = m.boundingBox.maxPoint.x.ToString("0.00") + " mm";
            infoSizeX.Text = m.boundingBox.Size.x.ToString("0.00") + " mm";
            infoMinY.Text = m.boundingBox.minPoint.y.ToString("0.00") + " mm";
            infoMaxY.Text = m.boundingBox.maxPoint.y.ToString("0.00") + " mm";
            infoSizeY.Text = m.boundingBox.Size.y.ToString("0.00") + " mm";
            infoMinZ.Text = m.boundingBox.minPoint.z.ToString("0.00") + " mm";
            infoMaxZ.Text = m.boundingBox.maxPoint.z.ToString("0.00") + " mm";
            infoSizeZ.Text = m.boundingBox.Size.z.ToString("0.00") + " mm";
            groupBox1.Text = pm.name;
        }
        public ObjectInformation()
        {
            InitializeComponent();
            Text = Trans.T("L_OBJECT_INFORMATIONS");
            labelPoints.Text = Trans.T("L_ANA_VERTICES");
            labelEdges.Text = Trans.T("L_ANA_EDGES");
            labelFaces.Text = Trans.T("L_ANA_FACES");
            labelShells.Text = Trans.T("L_ANA_SHELLS");
            labelVolume.Text = Trans.T("L_VOLUME:");
            labelSurface.Text = Trans.T("L_SURFACE:");
            labelMinimum.Text = Trans.T("L_MINIMUM:");
            labelMaximum.Text = Trans.T("L_MAXIMUM:");
            labelSize.Text = Trans.T("L_SIZE:");
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
