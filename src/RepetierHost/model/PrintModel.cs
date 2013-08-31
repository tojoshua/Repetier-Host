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
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Globalization;
using System.Drawing;
using System.Windows.Forms;
using RepetierHost.model.geom;
using RepetierHost.view;
using RepetierHost.view.utils;

namespace RepetierHost.model
{
    public delegate void PrintModelChangedEvent(PrintModel model);
    public class PrintModel : ThreeDModel
    {
        public TopoModel originalModel = new TopoModel();
        public TopoModel repairedModel = null;
        public int activeModel = 0;
        public string name = "Unknown";
        public string filename = "";
        long lastModified = 0;
        public bool outside = false,oldOutside = false;
        public Matrix4 trans,invTrans;
        public Submesh submesh = new Submesh();
        public int activeSubmesh = -1;
        public RHBoundingBox bbox = new RHBoundingBox();
        public event PrintModelChangedEvent printModelChangedEvent = null;
        public TopoModel Model
        {
            get
            {
                return (activeModel == 0 ? originalModel : repairedModel);
            }
        }
        public TopoModel ActiveModel
        {
            get
            {
                if (activeModel == 0 || repairedModel == null) return originalModel;
                return repairedModel;
            }
        }
        public void Reset()
        {
            repairedModel = originalModel.Copy();
            repairedModel.Analyse();
            ShowRepaired(true);
        }
        public void FixNormals()
        {
            if (repairedModel == null)
                repairedModel = originalModel.Copy();
            repairedModel.UpdateNormals();
            repairedModel.Analyse();
            repairedModel.updateBad();
            ShowRepaired(true);
        }
        public void ShowRepaired(bool showRepaired)
        {
            if (showRepaired)
            {
                activeModel = 1;
            }
            else
            {
                activeModel = 0;
            }
            ForceViewRegeneration();
            Main.main.threedview.Refresh();
            if (printModelChangedEvent != null)
                printModelChangedEvent(this);
        }
        public void RunTest()
        {
            if (repairedModel == null)
            {
                repairedModel = originalModel.Copy();
                repairedModel.RepairUnobtrusive();
            }
            repairedModel.RetestIntersectingTriangles();
            //repairedModel.JoinTouchedOpenEdges(0.1);
            //repairedModel.UpdateNormals();
            repairedModel.Analyse();
            repairedModel.updateBad();
            ShowRepaired(true);
        }
        public PrintModel copyPrintModel()
        {
            PrintModel stl = new PrintModel();
            stl.filename = filename;
            stl.name = name;
            stl.lastModified = lastModified;
            stl.Position.x = Position.x;
            stl.Position.y = Position.y + 5 + yMax - yMin;
            stl.Position.z = Position.z;
            stl.Scale.x = Scale.x;
            stl.Scale.y = Scale.y;
            stl.Scale.z = Scale.z;
            stl.Rotation.x = Rotation.x;
            stl.Rotation.y = Rotation.y;
            stl.Rotation.z = Rotation.z;
            stl.Selected = false;
            stl.activeModel = activeModel;
            stl.originalModel = originalModel.Copy();
            if (repairedModel != null)
                stl.repairedModel = repairedModel.Copy();
            else
                stl.repairedModel = null;
            stl.UpdateBoundingBox();
            return stl;
        }
        public PrintModel cloneWithModel(TopoModel m,int idx)
        {
            PrintModel stl = new PrintModel();
            stl.filename = "";
            stl.name = name+" ("+idx+")";
            stl.lastModified = lastModified;
            stl.Position.x = Position.x;
            stl.Position.y = Position.y;
            stl.Position.z = Position.z;
            stl.Scale.x = Scale.x;
            stl.Scale.y = Scale.y;
            stl.Scale.z = Scale.z;
            stl.Rotation.x = Rotation.x;
            stl.Rotation.y = Rotation.y;
            stl.Rotation.z = Rotation.z;
            stl.Selected = false;
            stl.activeModel = 0;
            stl.originalModel = m;
            stl.repairedModel = null;
            stl.UpdateBoundingBox();
            return stl;
        }

        public bool changedOnDisk()
        {
            if (filename == null || filename.Length == 0) return false;
            DateTime lastModiefied2 = File.GetLastWriteTime(filename);
            return lastModified != lastModiefied2.Ticks;
        }
        public void resetModifiedDate()
        {
            if (filename == null || filename.Length == 0) return;
            DateTime lastModified2 = File.GetLastWriteTime(filename);
            lastModified = lastModified2.Ticks;
        }
        public void reload()
        {
            if (File.Exists(filename))
            {
                InfoProgressPanel ipp = InfoProgressPanel.Create(Trans.T1("IMPORTING_1", name), true);
                ipp.Action = Trans.T("L_LOADING...");
                ipp.Dock = DockStyle.Top;
                Main.main.objectPlacement.panelControls.Controls.Add(ipp);
                Main.main.objectPlacement.panelControls.Update();
                Load(filename, ipp);
                Land();
                ForceViewRegeneration();
                ipp.Finished();
            }
        }

        public void Load(string file,InfoProgressPanel ipp)
        {
            filename = file;
            DateTime lastModified2 = File.GetLastWriteTime(filename);
            lastModified = lastModified2.Ticks;
            originalModel.ipp = ipp;
            string lname = filename.ToLower();
            if (lname.EndsWith(".stl"))
                originalModel.importSTL(filename);
            else if (lname.EndsWith(".obj"))
                originalModel.importObj(filename);
            FileInfo info = new FileInfo(file);
            name = info.Name;
            originalModel.Analyse();
            if (ipp.IsKilled)
            {
                originalModel.clear();
                return;
            }
            originalModel.Analyse();
            if (ipp.IsKilled)
            {
                originalModel.clear();
                return;
            }
            originalModel.updateBad();
            if (originalModel.intersectingTriangles.Count>0 || originalModel.badTriangles>0 || originalModel.manifold == false || originalModel.manyShardEdges != 0 || originalModel.loopEdges != 0 || originalModel.normalsOriented==false)
            {
                if (repairedModel == null)
                    repairedModel = originalModel.Copy();
                repairedModel.ipp = ipp;
                repairedModel.RepairUnobtrusive();
                repairedModel.Analyse();
                originalModel.Analyse();
                if (ipp.IsKilled)
                {
                    originalModel.clear();
                    repairedModel.clear();
                    return;
                }
                repairedModel.updateBad();
                ShowRepaired(true);
                repairedModel.ipp = null;
            }
            originalModel.ipp = null;
        }
        public override string ToString()
        {
            return name;
        }
        /// <summary>
        /// Translate Object, so that the lowest point is 0.
        /// </summary>
        public void Land()
        {
            UpdateBoundingBox();
            Position.z -= zMin;
        }
        public void Center(float x, float y)
        {
            Land();
            RHVector3 center = bbox.Center;
            Position.x += x - (float)center.x;
            Position.y += y - (float)center.y;
        }
        public override Vector3 getCenter()
        {
            return bbox.Center.asVector3();
        }
        public void UpdateMatrix()
        {
            Matrix4 transl = Matrix4.CreateTranslation(Position.x, Position.y, Position.z);
            Matrix4 scale = Matrix4.Scale(Scale.x!=0 ? Scale.x : 1, Scale.y !=0 ? Scale.y : 1, Scale.z !=0 ? Scale.z : 1);
            Matrix4 rotx = Matrix4.CreateRotationX(Rotation.x * (float)Math.PI / 180.0f);
            Matrix4 roty = Matrix4.CreateRotationY(Rotation.y * (float)Math.PI / 180.0f);
            Matrix4 rotz = Matrix4.CreateRotationZ(Rotation.z * (float)Math.PI / 180.0f);
            trans = Matrix4.Mult(scale, rotx);
            trans = Matrix4.Mult(trans, roty);
            trans = Matrix4.Mult(trans, rotz);
            trans = Matrix4.Mult(trans, transl);
            invTrans = Matrix4.Invert(trans);
        }
        public void UpdateBoundingBox()
        {
            UpdateMatrix();
            bbox.Clear();

            foreach (TopoVertex v in ActiveModel.vertices)
            {
                includePoint(v.pos);
            }
            xMin = (float)bbox.xMin;
            xMax = (float)bbox.xMax;
            yMin = (float)bbox.yMin;
            yMax = (float)bbox.yMax;
            zMin = (float)bbox.zMin;
            zMax = (float)bbox.zMax;
            if (Main.main.objectPlacement.checkCutFaces.Checked)
                Main.main.threedview.updateCuts = true;
        }
        private void includePoint(RHVector3 v)
        {
            float x, y, z;
            Vector4 v4 = v.asVector4();
            x = Vector4.Dot(trans.Column0, v4);
            y = Vector4.Dot(trans.Column1, v4);
            z = Vector4.Dot(trans.Column2, v4);
            bbox.Add(new RHVector3(x, y, z));
        }
        public void TransformPoint(ref Vector3 v, out float x, out float y, out float z)
        {
            Vector4 v4 = new Vector4(v, 1);
            x = Vector4.Dot(trans.Column0, v4);
            y = Vector4.Dot(trans.Column1, v4);
            z = Vector4.Dot(trans.Column2, v4);
        }
        public void TransformPoint(RHVector3 v, out float x, out float y, out float z)
        {
            Vector4 v4 = v.asVector4();
            x = Vector4.Dot(trans.Column0, v4);
            y = Vector4.Dot(trans.Column1, v4);
            z = Vector4.Dot(trans.Column2, v4);
        }
        public void TransformPoint(RHVector3 v,RHVector3 outv)
        {
            Vector4 v4 = v.asVector4();
            outv.x = Vector4.Dot(trans.Column0, v4);
            outv.y = Vector4.Dot(trans.Column1, v4);
            outv.z = Vector4.Dot(trans.Column2, v4);
        }
        public void ReverseTransformPoint(RHVector3 v, RHVector3 outv)
        {
            Vector4 v4 = v.asVector4();
            outv.x = Vector4.Dot(invTrans.Column0, v4);
            outv.y = Vector4.Dot(invTrans.Column1, v4);
            outv.z = Vector4.Dot(invTrans.Column2, v4);
        }
        public void ForceViewRegeneration()
        {
            ForceRefresh = true;
        }
        bool lastShowEdges = false;
        int lastRendered = -1; // 0 = all , 1 = mesh
        bool lastSelected = false;
        public bool ForceRefresh = false;
        public override void Paint()
        {
            TopoModel model = ActiveModel;

            if (Main.main.objectPlacement.checkCutFaces.Checked)
            {
                int cutpos = Main.main.objectPlacement.cutPositionSlider.Value;
                if (ForceRefresh || Main.main.threedview.updateCuts || lastRendered!=1 || activeModel != activeSubmesh || lastShowEdges != Main.threeDSettings.ShowEdges || lastSelected != Selected)
                {
                    RHVector3 normpoint = Main.main.threedview.cutPos.Add(Main.main.threedview.cutDirection);
                    RHVector3 point = new RHVector3(0, 0, 0);
                    ReverseTransformPoint(Main.main.threedview.cutPos, point);
                    ReverseTransformPoint(normpoint, normpoint);
                    RHVector3 normal = normpoint.Subtract(point);
                    
                    submesh.Clear();
                    model.CutMesh(submesh, normal, point,outside ? Submesh.MESHCOLOR_OUTSIDE : Submesh.MESHCOLOR_FRONTBACK);
                    submesh.selected = Selected;
                    submesh.Compress();
                    lastShowEdges = Main.threeDSettings.ShowEdges;
                    lastSelected = Selected;
                    activeSubmesh = activeModel;
                    lastRendered = 1;
                }
            }
            else
            {
                if (ForceRefresh || Main.main.threedview.updateCuts || lastRendered != 0 || activeModel != activeSubmesh || lastShowEdges != Main.threeDSettings.ShowEdges)
                {
                    submesh.Clear();
                    model.FillMesh(submesh, outside ? Submesh.MESHCOLOR_OUTSIDE : Submesh.MESHCOLOR_FRONTBACK);
                    submesh.selected = Selected;
                    submesh.Compress();
                    lastShowEdges = Main.threeDSettings.ShowEdges;
                    lastSelected = Selected;
                    activeSubmesh = activeModel;
                    lastRendered = 0;
                }
            }
            submesh.Draw(Main.threeDSettings.drawMethod,Main.main.threedview.cam.EdgeTranslation());
            ForceRefresh = false;
        }
        public OpenTK.Graphics.Color4 convertColor(Color col)
        {
            return new OpenTK.Graphics.Color4(col.R, col.G, col.B, col.A);
        }

    }
}
