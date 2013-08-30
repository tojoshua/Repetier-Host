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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.IO;
using OpenTK;
using RepetierHost.model;
using RepetierHost.model.geom;
using RepetierHost.view.utils;
using System.Diagnostics;

namespace RepetierHost.view
{
    public delegate void RepairToolDelegate(PrintModel model);
    public delegate void ObjectModelRemovedEvent(PrintModel model);

    public partial class STLComposer : UserControl
    {
        private bool writeSTLBinary = true;
        public ThreeDView cont;
        private bool autosizeFailed = false;
        private CopyObjectsDialog copyDialog = new CopyObjectsDialog();
        private Dictionary<ListViewItem, Button> delButtonList = new Dictionary<ListViewItem, Button>();
        private RepairToolDelegate repairToolDeleagte = null;
        public event ObjectModelRemovedEvent objectModelRemovedEvent = null;
        public STLComposer()
        {
            InitializeComponent();
            toolRepair.Visible = false;
            try
            {
                cont = new ThreeDView();
                //  cont.Dock = DockStyle.None;
                //  cont.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
                //  cont.Width = Width - panelControls.Width;
                //  cont.Height = Height;
                //  Controls.Add(cont);
                cont.SetEditor(true);
                cont.objectsSelected = false;
                cont.eventObjectMoved += objectMoved;
                cont.eventObjectSelected += objectSelected;
                cont.autoupdateable = true;
                updateEnabled();
                if (Main.main != null)
                {
                    Main.main.languageChanged += translate;
                    translate();
                }
            }
            catch { }
        }
        public void ConnectRepairTool(RepairToolDelegate tool)
        {
            repairToolDeleagte = tool;
            toolRepair.Visible = true;
        }
        public void translate()
        {
            labelTranslation.Text = Trans.T("L_TRANSLATION:");
            labelScale.Text = Trans.T("L_SCALE:");
            labelRotate.Text = Trans.T("L_ROTATE:");
            toolSavePlate.ToolTipText = Trans.T("B_SAVE_AS_STL");
            toolRemoveObjects.ToolTipText = Trans.T("B_REMOVE_STL_OBJECT");
            toolAddObjects.ToolTipText = Trans.T("B_ADD_STL_OBJECT");
            toolAutoposition.ToolTipText = Trans.T("B_AUTOPOSITION");
            toolLandObject.ToolTipText = Trans.T("B_DROP_OBJECT");
            toolCopyObjects.ToolTipText = Trans.T("B_COPY_OBJECTS");
            toolCenterObject.ToolTipText = Trans.T("B_CENTER_OBJECT");
            toolStripInfo.ToolTipText = Trans.T("L_OBJECT_INFORMATIONS");
            toolSplitObject.ToolTipText = Trans.T("L_SPLIT_OBJECT");
            toolFixNormals.ToolTipText = Trans.T("L_FIX_NORMALS");
            toolRepair.ToolTipText = Trans.T("L_REPAIR");
            textModied.Text = Trans.T("L_ANA_MODIFIED");
            textManifold.Text = Trans.T("L_ANA_MANIFOLD");
            textIntersectingTriangles.Text = Trans.T("L_ANA_INTERSECTING_TRIANGLES");
            textNormals.Text = Trans.T("L_ANA_NORMALS");
            textLoopEdges.Text = Trans.T("L_ANA_LOOP_EDGES");
            textHighlyConnected.Text = Trans.T("L_ANA_HIGHLY_CONNECTED");
            textVertices.Text = Trans.T("L_ANA_VERTICES");
            textEdges.Text = Trans.T("L_ANA_EDGES");
            textFaces.Text = Trans.T("L_ANA_FACES");
            textShells.Text = Trans.T("L_ANA_SHELLS");
            //buttonLockAspect.Text = Trans.T("L_LOCK_ASPECT_RATIO");
            //if (Main.slicer != null)
            //    buttonSlice.Text = Trans.T1("L_SLICE_WITH", Main.slicer.SlicerName);
        }
        public void AddObject(PrintModel model)
        {
            ListViewItem item = new ListViewItem(model.name);
            item.Tag = model;
            Button button = new Button();
            button.ImageList = imageList16;
            button.ImageIndex = 4;
            button.ImageAlign = ContentAlignment.MiddleCenter;
            button.Width = 16;
            button.Height = 16;
            button.TextImageRelation = TextImageRelation.Overlay;
            button.Text = "";
            button.Tag = model;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Click += buttonRemoveObject_Click;
            button.Visible = false;
            delButtonList.Add(item, button);
            item.SubItems.Add("");
            item.SubItems.Add("");
            item.SubItems.Add("");            
            listObjects.Controls.Add(button);
            listObjects.Items.Add(item);
            SetObjectSelected(model, true);
        }
        public void RemoveObject(PrintModel model)
        {
            ListViewItem item = null;
            foreach (ListViewItem test in listObjects.Items)
            {
                if (test.Tag == model)
                {
                    item = test;
                    break;
                }
            }
            if (item == null) return;
            Button trash = delButtonList[item];
            if (trash != null)
            {
                listObjects.Controls.Remove(trash);
                delButtonList.Remove(item);
            }
            foreach (Button b in delButtonList.Values)
                b.Visible = false;
            listObjects.Items.Remove(item);
            if (objectModelRemovedEvent != null)
                objectModelRemovedEvent(model);
        }
        public LinkedList<PrintModel> ListObjects(bool selected) {
            LinkedList<PrintModel> list = new LinkedList<PrintModel>();
            if (selected)
            {
                foreach (ListViewItem item in listObjects.SelectedItems)
                    list.AddLast((PrintModel)item.Tag);
            }
            else
            {
                foreach (ListViewItem item in listObjects.Items)
                    list.AddLast((PrintModel)item.Tag);
            }
            return list;
        }
        public PrintModel SingleSelectedModel
        {
            get
            {
                if (listObjects.SelectedItems.Count != 1) return null;
                return (PrintModel)listObjects.SelectedItems[0].Tag;
            }
        }
        public void Update3D()
        {
            Main.main.threedview.UpdateChanges();
        }
        private void float_Validating(object sender, CancelEventArgs e)
        {
            TextBox box = (TextBox)sender;
            try
            {
                float.Parse(box.Text, NumberStyles.Float, GCode.format);
                errorProvider.SetError(box, "");
            }
            catch
            {
                errorProvider.SetError(box, "Not a number.");
            }
        }
        public void UpdateAnalyserData()
        {
            PrintModel model = SingleSelectedModel;
            if (model == null) return;
            labelVertices.Text = model.ActiveModel.vertices.Count.ToString();
            labelEdges.Text = model.ActiveModel.edges.Count.ToString();
            labelFaces.Text = model.ActiveModel.triangles.Count.ToString();
            labelShells.Text = model.ActiveModel.shells.ToString();
            if (model.ActiveModel.intersectionsUpToDate)
                labelIntersectingTriangles.Text = model.ActiveModel.intersectingTriangles.Count.ToString();
            else
                labelIntersectingTriangles.Text = Trans.T("L_NOT_TESTED");
            labelIntersectingTriangles.ForeColor = (model.ActiveModel.intersectingTriangles.Count == 0 ? Color.Black : Color.Red);
            labelLoopEdges.Text = model.ActiveModel.loopEdges.ToString();
            labelLoopEdges.ForeColor = (model.ActiveModel.loopEdges == 0 ? Color.Black : Color.Red);
            labelHighConnected.Text = model.ActiveModel.manyShardEdges.ToString();
            labelHighConnected.ForeColor = (model.ActiveModel.manyShardEdges == 0 ? Color.Black : Color.Red);
            if (model.ActiveModel.manifold)
            {
                labelManifold.Text = Trans.T("L_YES");
                labelManifold.ForeColor = Color.Green;
            }
            else
            {
                labelManifold.Text = Trans.T("L_NO");
                labelManifold.ForeColor = Color.Red;
            }
            if (model.ActiveModel.normalsOriented)
            {
                labelNormals.Text = Trans.T("L_ANA_ORIENTED");
                labelNormals.ForeColor = Color.Green;
            }
            else
            {
                labelNormals.Text = Trans.T("L_ANA_NOT_ORIENTED");
                labelNormals.ForeColor = Color.Red;
            }
            if (model.activeModel == 1)
            {
                labelModified.Text = Trans.T("L_YES");
            }
            else
            {
                labelModified.Text = Trans.T("L_NO");
            }
        }

        private void updateEnabled()
        {
            int n = listObjects.SelectedItems.Count;
            Debug.WriteLine("updateENabled "+n);
            if (n != 1)
            {
                textRotX.Enabled = false;
                textRotY.Enabled = false;
                textRotZ.Enabled = false;
                textScaleX.Enabled = false;
                textScaleY.Enabled = false;
                textScaleZ.Enabled = false;
                buttonLockAspect.Enabled = false;
                textTransX.Enabled = false;
                textTransY.Enabled = false;
                textTransZ.Enabled = false;
                toolCenterObject.Enabled = false;
                toolAutoposition.Enabled = listObjects.Items.Count > 1;
                toolLandObject.Enabled = n > 0;
                if (Main.main.threedview != null)
                    Main.main.threedview.SetObjectSelected(n > 0);
                toolCopyObjects.Enabled = n > 0;
                toolRepair.Enabled = false;
                toolSplitObject.Enabled = false;
                panelAnalysis.Visible = false;
                toolStripInfo.Enabled = false;
            }
            else
            {
                toolAutoposition.Enabled = listObjects.Items.Count > 1;
                toolCopyObjects.Enabled = true;
                textRotX.Enabled = true;
                textRotY.Enabled = true;
                textRotZ.Enabled = true;
                textScaleX.Enabled = true;
                textScaleY.Enabled = !LockAspectRatio;
                textScaleZ.Enabled = !LockAspectRatio;
                buttonLockAspect.Enabled = true;
                textTransX.Enabled = true;
                textTransY.Enabled = true;
                textTransZ.Enabled = true;
                toolCenterObject.Enabled = true;
                toolLandObject.Enabled = true;
                if (Main.main.threedview != null)
                    Main.main.threedview.SetObjectSelected(true);
                toolRepair.Enabled = n == 1;
                toolSplitObject.Enabled = SingleSelectedModel.ActiveModel.shells > 1;
                panelAnalysis.Visible = true;
                toolStripInfo.Enabled = true;
                UpdateAnalyserData();
            }
            toolFixNormals.Enabled = n != 0;
            toolRemoveObjects.Enabled = n != 0;
            //buttonSlice.Enabled = listObjects.Items.Count > 0;
            toolSavePlate.Enabled = listObjects.Items.Count > 0;
        }
        public void openAndAddObject(string file)
        {
            PrintModel model = new PrintModel();
            FileInfo f = new FileInfo(file);

            InfoProgressPanel ipp = InfoProgressPanel.Create(Trans.T1("IMPORTING_1", f.Name), true);
            ipp.Action = Trans.T("L_LOADING...");
            ipp.Dock = DockStyle.Top;
            panelControls.Controls.Add(ipp);
            panelControls.Update();
            model.Load(file,ipp);
            model.Center(Main.printerSettings.PrintAreaWidth / 2, Main.printerSettings.PrintAreaDepth / 2);
            model.Land();
            if (model.ActiveModel.triangles.Count > 0)
            {
                AddObject(model);
                cont.models.AddLast(model);

                Autoposition();
                model.addAnimation(new DropAnimation("drop"));
                updateSTLState(model);
            }
            else
            {
                if(!ipp.IsKilled)
                    MessageBox.Show(Trans.T1("L_LOADING_3D_FAILED", file), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ipp.Finished();
        }
        private void buttonAddSTL_Click(object sender, EventArgs e)
        {
            if (openFileSTL.ShowDialog() == DialogResult.OK)
            {
                foreach (string fname in openFileSTL.FileNames)
                    openAndAddObject(fname);
            }
        }
        /// <summary>
        /// Checks the state of the object.
        /// If it is outside print are it starts pulsing
        /// </summary>
        public void updateSTLState(PrintModel stl2)
        {
            bool dataChanged = false;
            FormPrinterSettings ps = Main.printerSettings;
            stl2.UpdateBoundingBox();
            LinkedList<PrintModel> testList = ListObjects(false);
            foreach (PrintModel pm in testList)
            {
                pm.oldOutside = pm.outside;
                pm.outside = false;
            }
            foreach (PrintModel pm in testList)
            {
                foreach (PrintModel pm2 in testList)
                {
                    if (pm == pm2) continue;
                    if (pm2.bbox.IntersectsBox(pm.bbox))
                    {
                        pm.outside = true;
                        pm2.outside = true;
                    }
                }
            }
            foreach (PrintModel stl in testList)
            {
                if (!ps.PointInside(stl.xMin, stl.yMin, stl.zMin) ||
                    !ps.PointInside(stl.xMax, stl.yMin, stl.zMin) ||
                    !ps.PointInside(stl.xMin, stl.yMax, stl.zMin) ||
                    !ps.PointInside(stl.xMax, stl.yMax, stl.zMin) ||
                    !ps.PointInside(stl.xMin, stl.yMin, stl.zMax) ||
                    !ps.PointInside(stl.xMax, stl.yMin, stl.zMax) ||
                    !ps.PointInside(stl.xMin, stl.yMax, stl.zMax) ||
                    !ps.PointInside(stl.xMax, stl.yMax, stl.zMax))
                {
                    stl.outside = true;
                }
            }
            foreach (PrintModel pm in testList)
            {
                if (pm.oldOutside != pm.outside)
                {
                    dataChanged = true;
                    pm.ForceViewRegeneration();
                    if (Main.threeDSettings.pulseOutside.Checked)
                    {
                        if (!pm.hasAnimationWithName("pulse") && pm.outside)
                            pm.addAnimation(new PulseAnimation("pulse", 0.03, 0.03, 0.03, 0.3));
                        if (pm.hasAnimationWithName("pulse") && !pm.outside)
                            pm.removeAnimationWithName("pulse");
                    }
                }
            }

            if (dataChanged)
            {
                listObjects.Refresh();
            }
        }
        private void listSTLObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            Main.main.threedview.updateCuts = true;
            updateEnabled();
            LinkedList<PrintModel> list = ListObjects(false);
            LinkedList<PrintModel> sellist = ListObjects(true);
            PrintModel stl = (sellist.Count == 1 ? sellist.First.Value : null);
            foreach (PrintModel s in list)
            {
                s.Selected = sellist.Contains(s);
            }
            if (stl != null)
            {
                textRotX.Text = stl.Rotation.x.ToString(GCode.format);
                textRotY.Text = stl.Rotation.y.ToString(GCode.format);
                textRotZ.Text = stl.Rotation.z.ToString(GCode.format);
                LockAspectRatio = (stl.Scale.x == stl.Scale.y && stl.Scale.x == stl.Scale.z);
                textScaleX.Text = stl.Scale.x.ToString(GCode.format);
                textScaleY.Text = stl.Scale.y.ToString(GCode.format);
                textScaleZ.Text = stl.Scale.z.ToString(GCode.format);
                textTransX.Text = stl.Position.x.ToString(GCode.format);
                textTransY.Text = stl.Position.y.ToString(GCode.format);
                textTransZ.Text = stl.Position.z.ToString(GCode.format);
            }
            Main.main.threedview.UpdateChanges();
        }
        public bool LockAspectRatio
        {
            get
            {
                return buttonLockAspect.ImageIndex == 1;
            }
            set
            {
                buttonLockAspect.ImageIndex = value ? 1 : 0;
                textScaleY.Enabled = !value;
                textScaleZ.Enabled = !value;
            }
        }
        private void textTransX_TextChanged(object sender, EventArgs e)
        {
            PrintModel stl = SingleSelectedModel;
            if (stl == null) return;
            float.TryParse(textTransX.Text, NumberStyles.Float, GCode.format, out stl.Position.x);
            updateSTLState(stl);
            Main.main.threedview.UpdateChanges();
        }

        private void textTransY_TextChanged(object sender, EventArgs e)
        {
            PrintModel stl = SingleSelectedModel;
            if (stl == null) return;
            float.TryParse(textTransY.Text, NumberStyles.Float, GCode.format, out stl.Position.y);
            updateSTLState(stl);
            Main.main.threedview.UpdateChanges();
        }

        private void textTransZ_TextChanged(object sender, EventArgs e)
        {
            PrintModel stl = SingleSelectedModel;
            if (stl == null) return;
            float.TryParse(textTransZ.Text, NumberStyles.Float, GCode.format, out stl.Position.z);
            updateSTLState(stl);
            Main.main.threedview.UpdateChanges();
        }
        private void objectMoved(float dx, float dy)
        {
            //STL stl = (STL)listSTLObjects.SelectedItem;
            //if (stl == null) return;
            foreach (PrintModel stl in ListObjects(true))
            {
                stl.Position.x += dx;
                stl.Position.y += dy;
                if (listObjects.SelectedItems.Count == 1)
                {
                    textTransX.Text = stl.Position.x.ToString(GCode.format);
                    textTransY.Text = stl.Position.y.ToString(GCode.format);
                }
                updateSTLState(stl);
            }
            Main.main.threedview.UpdateChanges();
        }
        public void SetObjectSelected(PrintModel model, bool select)
        {
            foreach (ListViewItem item in listObjects.Items)
            {
                if (item.Tag == model)
                    item.Selected = select;
            }
        }
        private void objectSelected(ThreeDModel sel)
        {
            if (Control.ModifierKeys == Keys.Shift)
            {
                if (!sel.Selected)
                    SetObjectSelected((PrintModel)sel,true);
            }
            else
                if (Control.ModifierKeys == Keys.Control)
                {
                    if (sel.Selected)
                        SetObjectSelected((PrintModel)sel,false);
                    else
                        SetObjectSelected((PrintModel)sel, true);
                }
                else
                {
                    foreach (ListViewItem test in listObjects.Items) {
                        if (test.Selected && (PrintModel)test.Tag != (PrintModel)sel)
                            test.Selected = false;
                    }
                    //listObjects.SelectedItems.Clear();
                    SetObjectSelected((PrintModel)sel,true);
                }
        }
        private void textScaleX_TextChanged(object sender, EventArgs e)
        {
            PrintModel model = SingleSelectedModel;
            if (model == null) return;
            float.TryParse(textScaleX.Text, NumberStyles.Float, GCode.format, out model.Scale.x);
            if (LockAspectRatio)
            {
                model.Scale.y = model.Scale.z = model.Scale.x;
                textScaleY.Text = model.Scale.y.ToString(GCode.format);
                textScaleZ.Text = model.Scale.z.ToString(GCode.format);
            }
            updateSTLState(model);
            Main.main.threedview.UpdateChanges();
        }

        private void textScaleY_TextChanged(object sender, EventArgs e)
        {
            PrintModel stl = SingleSelectedModel;
            if (stl == null) return;
            float.TryParse(textScaleY.Text, NumberStyles.Float, GCode.format, out stl.Scale.y);
            updateSTLState(stl);
            Main.main.threedview.UpdateChanges();
        }

        private void textScaleZ_TextChanged(object sender, EventArgs e)
        {
            PrintModel stl = SingleSelectedModel;
            if (stl == null) return;
            float.TryParse(textScaleZ.Text, NumberStyles.Float, GCode.format, out stl.Scale.z);
            updateSTLState(stl);
            Main.main.threedview.UpdateChanges();
        }

        private void textRotX_TextChanged(object sender, EventArgs e)
        {
            PrintModel stl = SingleSelectedModel;
            if (stl == null) return;
            float.TryParse(textRotX.Text, NumberStyles.Float, GCode.format, out stl.Rotation.x);
            updateSTLState(stl);
            Main.main.threedview.UpdateChanges();
        }

        private void textRotY_TextChanged(object sender, EventArgs e)
        {
            PrintModel stl = SingleSelectedModel;
            if (stl == null) return;
            float.TryParse(textRotY.Text, NumberStyles.Float, GCode.format, out stl.Rotation.y);
            updateSTLState(stl);
            Main.main.threedview.UpdateChanges();
        }

        private void textRotZ_TextChanged(object sender, EventArgs e)
        {
            PrintModel stl = SingleSelectedModel;
            if (stl == null) return;
            float.TryParse(textRotZ.Text, NumberStyles.Float, GCode.format, out stl.Rotation.z);
            updateSTLState(stl);
            Main.main.threedview.UpdateChanges();
        }
        private void buttonRemoveObject_Click(object sender, EventArgs e)
        {
            PrintModel model = (PrintModel)((Button)sender).Tag;
            cont.models.Remove(model);
            RemoveObject(model);
            autosizeFailed = false;
            Main.main.threedview.UpdateChanges();
        }
        public void buttonRemoveSTL_Click(object sender, EventArgs e)
        {
            //STL stl = (STL)listSTLObjects.SelectedItem;
            //if (stl == null) return;
            LinkedList<PrintModel> list = new LinkedList<PrintModel>();
            foreach (PrintModel stl in ListObjects(true))
            {
                cont.models.Remove(stl);
                RemoveObject(stl);
                autosizeFailed = false; // Reset autoposition
            }
            Main.main.threedview.UpdateChanges();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (saveSTL.ShowDialog() == DialogResult.OK)
            {
                saveComposition(saveSTL.FileName);
            }
        }
        private bool AssertVector3NotNaN(Vector3 v)
        {
            //return true;
            if (float.IsNaN(v.X) || float.IsNaN(v.Y) || float.IsNaN(v.Z))
            {
            RLog.info("NaN value in STL file export");
                return false;
            }
            if (float.IsInfinity(v.X) || float.IsInfinity(v.Y) || float.IsInfinity(v.Z))
            {
               RLog.info("Infinity value in STL file export");
                return false;
            }
            return true;
        }
        private void saveComposition(string filename)
        {
            TopoModel model = new TopoModel();
            foreach (PrintModel stl in ListObjects(false))
            {
                stl.UpdateMatrix();
                model.Merge(stl.ActiveModel, stl.trans);
            }
            if (filename.EndsWith(".obj") || filename.EndsWith(".OBJ"))
                model.exportObj(filename,true);
            else
                model.exportSTL(filename, writeSTLBinary);
            Slicer.lastBox.Clear();
            Slicer.lastBox.Add(model.boundingBox);
        }

        private void buttonLand_Click(object sender, EventArgs e)
        {
            //STL stl = (STL)listSTLObjects.SelectedItem;
            //if (stl == null) return;
            foreach (PrintModel stl in ListObjects(true))
            {
                stl.Land();
                listSTLObjects_SelectedIndexChanged(null, null);
            }
            Main.main.threedview.UpdateChanges();
        }

        private void buttonCenter_Click(object sender, EventArgs e)
        {
            //STL stl = (STL)listSTLObjects.SelectedItem;
            //if (stl == null) return;
            foreach (PrintModel stl in ListObjects(true))
            {
                stl.Center(Main.printerSettings.BedLeft + Main.printerSettings.PrintAreaWidth / 2, Main.printerSettings.BedFront + Main.printerSettings.PrintAreaDepth / 2);
                listSTLObjects_SelectedIndexChanged(null, null);

            }
            Main.main.threedview.UpdateChanges();
        }

        public void buttonSlice_Click(object sender, EventArgs e)
        {
            string dir = Main.globalSettings.Workdir;
            if (!Directory.Exists(dir))
            {
                MessageBox.Show(Trans.T("L_EXISTING_WORKDIR_REQUIRED"), Trans.T("L_ERROR"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                Main.globalSettings.Show();
                return;
            }
            if (listObjects.Items.Count == 0) return;
            bool itemsOutide = false;
            foreach (PrintModel stl in ListObjects(false))
            {
                if (stl.outside) itemsOutide = true;
            }
            if (itemsOutide)
            {
                if (MessageBox.Show(Trans.T("L_OBJECTS_OUTSIDE_SLICE_QUEST"), Trans.T("L_WARNING"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    return;
            }
            string t = listObjects.Items[0].ToString();
            if (listObjects.Items.Count > 1)
                t += " + " + (listObjects.Items.Count - 1).ToString();
            Main.main.Title = t;
            if(Main.slicer.ActiveSlicer == RepetierHost.view.utils.Slicer.SlicerID.Slic3r)
                dir += Path.DirectorySeparatorChar + "composition.obj";
            else
                dir += Path.DirectorySeparatorChar + "composition.stl";
            saveComposition(dir);
            Main.slicer.RunSlice(dir); // Slice it and load
        }

        public void Autoposition()
        {
            if (listObjects.Items.Count == 1)
            {
                PrintModel stl = (PrintModel)listObjects.Items[0].Tag;
                stl.Center(Main.printerSettings.BedLeft + Main.printerSettings.PrintAreaWidth / 2, Main.printerSettings.BedFront + Main.printerSettings.PrintAreaDepth / 2);
                Main.main.threedview.UpdateChanges();
                return;
            }
            if (autosizeFailed) return;
            RectPacker packer = new RectPacker(1, 1);
            int border = 3;
            FormPrinterSettings ps = Main.printerSettings;
            float maxW = ps.PrintAreaWidth;
            float maxH = ps.PrintAreaDepth;
            float xOff = ps.BedLeft, yOff = ps.BedFront;
            if (ps.printerType == 1)
            {
                if (ps.DumpAreaFront <= 0)
                {
                    yOff = ps.BedFront + ps.DumpAreaDepth - ps.DumpAreaFront;
                    maxH -= yOff;
                }
                else if (ps.DumpAreaDepth + ps.DumpAreaFront >= maxH)
                {
                    yOff = ps.BedFront + -(maxH - ps.DumpAreaFront);
                    maxH += yOff;
                }
                else if (ps.DumpAreaLeft <= 0)
                {
                    xOff = ps.BedLeft + ps.DumpAreaWidth - ps.DumpAreaLeft;
                    maxW -= xOff;
                }
                else if (ps.DumpAreaWidth + ps.DumpAreaLeft >= maxW)
                {
                    xOff = ps.BedLeft + maxW - ps.DumpAreaLeft;
                    maxW += xOff;
                }
            }
            foreach (PrintModel stl in ListObjects(false))
            {
                int w = 2 * border + (int)Math.Ceiling(stl.xMax - stl.xMin);
                int h = 2 * border + (int)Math.Ceiling(stl.yMax - stl.yMin);
                if (!packer.addAtEmptySpotAutoGrow(new PackerRect(0, 0, w, h, stl), (int)maxW, (int)maxH))
                {
                    autosizeFailed = true;
                }
            }
            if (autosizeFailed)
            {
                MessageBox.Show("Too many objects on printer bed for automatic packing.\r\nPacking disabled until elements are removed.",
                "Printer bed full", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            float xAdd = (maxW - packer.w) / 2.0f;
            float yAdd = (maxH - packer.h) / 2.0f;
            foreach (PackerRect rect in packer.vRects)
            {
                PrintModel s = (PrintModel)rect.obj;
                float xPos = xOff + xAdd + rect.x + border;
                float yPos = yOff + yAdd + rect.y + border;
                s.Position.x += xPos - s.xMin;
                s.Position.y += yPos - s.yMin;
                s.UpdateBoundingBox();
            }
            Main.main.threedview.UpdateChanges();
        }
        private void buttonAutoplace_Click(object sender, EventArgs e)
        {
            Autoposition();
        }

        private void buttonCopyObjects_Click(object sender, EventArgs e)
        {
            if (copyDialog.ShowDialog(Main.main) == DialogResult.Cancel) return;
            int numberOfCopies = (int)copyDialog.numericCopies.Value;

            List<PrintModel> newSTL = new List<PrintModel>();
            foreach (PrintModel act in ListObjects(true))
            {
                PrintModel last = act;
                for (int i = 0; i < numberOfCopies; i++)
                {
                    PrintModel stl = last.copyPrintModel();
                    last = stl;
                    newSTL.Add(stl);
                }
            }
            foreach (PrintModel stl in newSTL)
            {
                AddObject(stl);
                cont.models.AddLast(stl);
            }
            if (copyDialog.checkAutoposition.Checked)
            {
                Autoposition();
            }
            Main.main.threedview.UpdateChanges();
        }
        static bool inRecheckFiles = false;
        public void recheckChangedFiles()
        {
            if (inRecheckFiles) return;
            inRecheckFiles = true;
            bool changed = false;
            foreach (PrintModel model in ListObjects(false))
            {
                if (model.changedOnDisk())
                {
                    changed = true;
                    break;
                }
            }
            if (changed)
            {
                if (MessageBox.Show(Trans.T("L_MODEL_CHANGED_RELOAD"), Trans.T("L_FILES_CHANGED"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    foreach (PrintModel stl in ListObjects(false))
                    {
                        if (stl.changedOnDisk())
                            stl.reload();
                    }
                    Main.main.threedview.UpdateChanges();
                }
                else
                {
                    foreach (PrintModel stl in ListObjects(false))
                    {
                        if (stl.changedOnDisk())
                            stl.resetModifiedDate();
                    }
                }
            }
            inRecheckFiles = false;
        }

        public void listSTLObjects_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A)
            {
                foreach (ListViewItem item in listObjects.Items)
                    item.Selected = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                buttonRemoveSTL_Click(sender, null);
                e.Handled = true;
            }
        }

        private void buttonRepair_Click(object sender, EventArgs e)
        {
            if (listObjects.SelectedItems.Count != 1) return;
            PrintModel model = (PrintModel)ListObjects(true).First.Value;
            if (model != null && repairToolDeleagte!=null)
                repairToolDeleagte(model);
        }

        private void listObjects_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                e.DrawDefault = false;
              //  if ((e.ItemState & ListViewItemStates.Selected) == 0)
              //      e.DrawBackground();
    
                Graphics g = e.Graphics;
                PrintModel model = (PrintModel)e.Item.Tag;
                g.DrawImage(imageList16.Images[model.Model.manifold ? 2 : 3], e.Bounds.Left + e.Bounds.Width / 2 - 8, e.Bounds.Top + e.Bounds.Height / 2 - 8);
            } else if (e.ColumnIndex == 2)
            {
                PrintModel model = (PrintModel)e.Item.Tag;
                e.DrawDefault = false;
              //  if ((e.ItemState & ListViewItemStates.Selected) == 0)
              //      e.DrawBackground();
                Graphics g = e.Graphics;
                int idx = model.outside ? 3 : 2;
                g.DrawImage(imageList16.Images[idx], e.Bounds.Left + e.Bounds.Width / 2 - 8, e.Bounds.Top + e.Bounds.Height / 2 - 8);
            }
            else if (e.ColumnIndex == 3) // Trash
            {
                e.DrawDefault = false;
                //if ((e.ItemState & ListViewItemStates.Selected) == 0)
                //    e.DrawBackground();
                Button b = delButtonList.ContainsKey(e.Item) ? delButtonList[e.Item] : null;
                if (b!=null)
                {
                    b.Bounds = new Rectangle(e.Bounds.Left+1,e.Bounds.Top+1,e.Bounds.Width-2,e.Bounds.Height-2);
                    b.Visible = true;
                }
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private void listObjects_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void buttonLockAspect_Click(object sender, EventArgs e)
        {
            LockAspectRatio = !LockAspectRatio;
            if(LockAspectRatio)
                textScaleX_TextChanged(null,null);
        }

        private void listObjects_ClientSizeChanged(object sender, EventArgs e)
        {
            int nameWith = listObjects.Width - columnCollision.Width - columnMesh.Width - columnDelete.Width-5;
            if (nameWith > 0)
                columnName.Width = nameWith;
        }

        private void toolFixNormals_Click(object sender, EventArgs e)
        {
            foreach(PrintModel model in ListObjects(true))
                model.FixNormals();
            updateEnabled();
            Main.main.threedview.UpdateChanges();
        }

        private void listObjects_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = false;
            e.DrawBackground();
            e.DrawText();
        }

        private void checkCutFaces_CheckedChanged(object sender, EventArgs e)
        {
            Main.main.threedview.UpdateChanges();
        }

        private void cutPositionSlider_ValueChanged(object sender, EventArgs e)
        {
            if(checkCutFaces.Checked)
                Main.main.threedview.UpdateChanges();
        }

        private void toolSplitObject_Click(object sender, EventArgs e)
        {
            PrintModel act = SingleSelectedModel;
            if(act==null) return;
            List<TopoModel> models = act.ActiveModel.SplitIntoSurfaces();
            if (models.Count == 1) return;
            int idx = 1;
            foreach (TopoModel m in models)
            {
                PrintModel pm = act.cloneWithModel(m, idx++);
                cont.models.AddLast(pm);
                AddObject(pm);
            }
            cont.models.Remove(act);
            RemoveObject(act);
        }

        private void labelModified_Click(object sender, EventArgs e)
        {
            PrintModel act = SingleSelectedModel;
            if (act == null) return;
            act.ShowRepaired(act.activeModel == 0 && act.repairedModel!=null);
            UpdateAnalyserData();
        }

        private void toolStripInfo_Click(object sender, EventArgs e)
        {
            PrintModel act = SingleSelectedModel;
            if (act == null) return;
            ObjectInformation.Execute(act);
        }

        private void buttonAnalyse_Click(object sender, EventArgs e)
        {
            if (listObjects.SelectedItems.Count != 1) return;
            PrintModel model = (PrintModel)ListObjects(true).First.Value;
            if (model != null)
            {
                InfoProgressPanel ipp = InfoProgressPanel.Create(Trans.T("L_ANAYLSING"), true);
                ipp.Action = ""; // Trans.T("L_LOADING...");
                ipp.Dock = DockStyle.Top;
                panelControls.Controls.Add(ipp);
                panelControls.Update();
                model.DeepAnalysis(ipp);
                ipp.Finished();
                UpdateAnalyserData();
            }
        }
    }
    public class EnglishStreamWriter : StreamWriter
    {
        public EnglishStreamWriter(Stream path)
            : base(path, Encoding.ASCII)
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        }
        public override IFormatProvider FormatProvider
        {
            get
            {
                return System.Globalization.CultureInfo.InvariantCulture;
            }
        }
    }
}
