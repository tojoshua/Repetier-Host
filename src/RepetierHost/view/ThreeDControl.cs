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
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;
using OpenTK;
using System.Diagnostics;
using System.Globalization;
using RepetierHost.model;
using RepetierHost.model.geom;

namespace RepetierHost.view
{
    //public delegate void onObjectMoved(float dx, float dy);
    //public delegate void onObjectSelected(ThreeDModel selModel);
    public partial class ThreeDControl : UserControl
    {
        public ThreeDCamera cam;
        float bedRadius;
        FormPrinterSettings ps = Main.printerSettings;
        bool loaded = false;
        float xDown, yDown;
        float xPos, yPos;
        float speedX, speedY;
        double normX = 0, normY = 0;
        float lastX, lastY;
        Stopwatch sw = new Stopwatch();
        Stopwatch fpsTimer = new Stopwatch();
        int mode = 0;
        int slowCounter = 0; // Indicates slow framerates
        uint timeCall = 0;
        public float zoom = 1.0f;
        public Matrix4 lookAt, persp, modelView;
        public float nearDist, farDist, aspectRatio, nearHeight, midHeight;
        Coordinate coord;

        public ThreeDView view = null;

        public ThreeDControl()
        {
            InitializeComponent();
            cam = new ThreeDCamera(this);
            SetCameraDefaults();
            cam.OrientIsometric();
            gl.MouseWheel += gl_MouseWheel;
            timer.Start();
            translate();
            Main.main.languageChanged += translate;
        }
        private void translate()
        {
            //toolMove.ToolTipText = Trans.T("L_MOVE_CAMERA");
            toolMoveObject.ToolTipText = Trans.T("L_MOVE_OBJECT");
            toolMoveViewpoint.ToolTipText = Trans.T("L_MOVE_VIEWPOINT");
            toolResetView.ToolTipText = Trans.T("L_FRONT_VIEW");
            toolIsometric.ToolTipText = Trans.T("L_ISOMETRIC_VIEW");
            toolRotate.ToolTipText = Trans.T("L_ROTATE");
            toolTopView.ToolTipText = Trans.T("L_TOP_VIEW");
            toolZoom.ToolTipText = Trans.T("T_ZOOM_VIEW");
            toolStripClear.ToolTipText = Trans.T("T_CLEAR_OBJECTS");
            toolParallelProjection.ToolTipText = Trans.T("L_USE_PARALLEL_PROJECTION");
        }
        public void SetView(ThreeDView view)
        {
            this.view = view;
            toolStripClear.Visible = view.autoupdateable;
            if (view.editor)
            {
                toolMoveObject.Visible = true;
                toolStripClear.Enabled = false;
            }
            toolMoveObject.Enabled = view.objectsSelected;
            toolStripClear.Enabled = view.objectsSelected;
            UpdateChanges();
        }
        public void MakeVisible(bool vis)
        {
            //return;
            //Console.WriteLine("Vis = " + vis);
            if (vis)
            {
                if (!Controls.Contains(gl))
                    Controls.Add(gl);
                //gl.Dock = DockStyle.None;
                //gl.Width = 0;
                //gl.Height = 0;
            }
            else
            {
                if (Controls.Contains(gl))
                    Controls.Remove(gl);
                //gl.Dock = DockStyle.Fill;
            }
            gl.Visible = vis;
        }
        public void SetObjectSelected(bool sel)
        {
            toolMoveObject.Enabled = sel;
            toolStripClear.Enabled = sel;
            view.objectsSelected = sel;
        }
        public bool AutoUpdateable
        {
            get { return view.autoupdateable; }
            set
            {
                view.autoupdateable = value;
                if (view.autoupdateable)
                {
                    timer.Start();
                }
                else
                {
                    timer.Stop();
                }
                toolStripClear.Visible = value;
            }
        }
        public void UpdateChanges()
        {
            gl.Invalidate();
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
        private void SetupViewport()
        {
            try
            {
                int w = gl.Width;
                int h = gl.Height;
                bedRadius = (float)(1.5 * Math.Sqrt((ps.PrintAreaDepth * ps.PrintAreaDepth + ps.PrintAreaHeight * ps.PrintAreaHeight + ps.PrintAreaWidth * ps.PrintAreaWidth) * 0.25));
                GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
                GL.MatrixMode(MatrixMode.Projection);
                //GL.LoadIdentity();
                float angle;
                Vector3 camPos = cam.CameraPosition;
                float dist = (float)cam.distance;
                nearDist = Math.Max(1, dist - bedRadius);
                farDist = Math.Max(bedRadius * 2, dist + bedRadius);
                midHeight = 2.0f * (float)Math.Tan(cam.angle) * dist;
                nearHeight = 2.0f * (float)Math.Tan(cam.angle) * nearDist;
                aspectRatio = (float)w / (float)h;
                angle = (float)(cam.angle) * 2.0f;
                if (toolParallelProjection.Checked)
                    persp = Matrix4.CreateOrthographic(midHeight * aspectRatio, midHeight, nearDist, farDist);
                else
                    persp = Matrix4.CreatePerspectiveFieldOfView((float)(angle), aspectRatio, nearDist, farDist);
                GL.LoadMatrix(ref persp);
                // GL.Ortho(0, w, 0, h, -1, 1); // Bottom-left corner pixel has coordinate (0, 0)

            }
            catch { }
        }
        public OpenTK.Graphics.Color4 convertColor(Color col)
        {
            return new OpenTK.Graphics.Color4(col.R, col.G, col.B, col.A);
        }
        private void AddLights()
        {
            //Enable lighting
            GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 0.2f, 0.2f, 0.2f, 1f });
            GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 0, 0, 0, 0 });
            GL.Light(LightName.Light0, LightParameter.Specular, new float[] { 0, 0, 0, 0 });
            GL.Enable(EnableCap.Light0);
            if (Main.threeDSettings.enableLight1.Checked)
            {
                GL.Light(LightName.Light1, LightParameter.Ambient, Main.threeDSettings.Ambient1());
                GL.Light(LightName.Light1, LightParameter.Diffuse, Main.threeDSettings.Diffuse1());
                GL.Light(LightName.Light1, LightParameter.Specular, Main.threeDSettings.Specular1());
                GL.Light(LightName.Light1, LightParameter.Position, Main.threeDSettings.Dir1());
                //  GL.Light(LightName.Light1, LightParameter.SpotExponent, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.Enable(EnableCap.Light1);
            }
            else GL.Disable(EnableCap.Light1);
            if (Main.threeDSettings.enableLight2.Checked)
            {
                GL.Light(LightName.Light2, LightParameter.Ambient, Main.threeDSettings.Ambient2());
                GL.Light(LightName.Light2, LightParameter.Diffuse, Main.threeDSettings.Diffuse2());
                GL.Light(LightName.Light2, LightParameter.Specular, Main.threeDSettings.Specular2());
                GL.Light(LightName.Light2, LightParameter.Position, Main.threeDSettings.Dir2());
                /*  GL.Light(LightName.Light2, LightParameter.Diffuse, new float[] { 0.7f, 0.7f, 0.7f, 1f });
                  GL.Light(LightName.Light2, LightParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                  GL.Light(LightName.Light2, LightParameter.Position, (new Vector4(100f, 200f, 300f, 0)));*/
                GL.Light(LightName.Light2, LightParameter.SpotExponent, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.Enable(EnableCap.Light2);
            }
            else GL.Disable(EnableCap.Light2);
            if (Main.threeDSettings.enableLight3.Checked)
            {
                GL.Light(LightName.Light3, LightParameter.Ambient, Main.threeDSettings.Ambient3());
                GL.Light(LightName.Light3, LightParameter.Diffuse, Main.threeDSettings.Diffuse3());
                GL.Light(LightName.Light3, LightParameter.Specular, Main.threeDSettings.Specular3());
                GL.Light(LightName.Light3, LightParameter.Position, Main.threeDSettings.Dir3());
                /*  GL.Light(LightName.Light3, LightParameter.Diffuse, new float[] { 0.8f, 0.8f, 0.8f, 1f });
                  GL.Light(LightName.Light3, LightParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                  GL.Light(LightName.Light3, LightParameter.Position, (new Vector4(100f, -200f, 200f, 0)));*/
                GL.Light(LightName.Light3, LightParameter.SpotExponent, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.Enable(EnableCap.Light3);
            }
            else GL.Disable(EnableCap.Light3);
            if (Main.threeDSettings.enableLight4.Checked)
            {
                GL.Light(LightName.Light4, LightParameter.Ambient, Main.threeDSettings.Ambient4());
                GL.Light(LightName.Light4, LightParameter.Diffuse, Main.threeDSettings.Diffuse4());
                GL.Light(LightName.Light4, LightParameter.Specular, Main.threeDSettings.Specular4());
                GL.Light(LightName.Light4, LightParameter.Position, Main.threeDSettings.Dir4());
                /* GL.Light(LightName.Light4, LightParameter.Diffuse, new float[] { 0.7f, 0.7f, 0.7f, 1f });
                 GL.Light(LightName.Light4, LightParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                 GL.Light(LightName.Light4, LightParameter.Position, (new Vector4(170f, -100f, -250f, 0)));*/
                GL.Light(LightName.Light4, LightParameter.SpotExponent, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
                GL.Enable(EnableCap.Light4);
            }
            else GL.Disable(EnableCap.Light4);

            GL.Enable(EnableCap.Lighting);
        }
        private void DetectDrawingMethod()
        {
            // Check drawing method
            int om = Main.threeDSettings.drawMethod;
            switch (Main.threeDSettings.comboDrawMethod.SelectedIndex)
            {
                case 0: // Autodetect;
                    if (Main.threeDSettings.useVBOs && Main.threeDSettings.openGLVersion >= 1.499)
                        Main.threeDSettings.drawMethod = 2;
                    else if (Main.threeDSettings.openGLVersion >= 1.099)
                        Main.threeDSettings.drawMethod = 1;
                    else
                        Main.threeDSettings.drawMethod = 0;
                    break;
                case 1: // VBOs
                    Main.threeDSettings.drawMethod = 2;
                    break;
                case 2: // drawElements
                    Main.threeDSettings.drawMethod = 1;
                    break;
                case 3: // elements
                    Main.threeDSettings.drawMethod = 0;
                    break;
            }
            if (om != Main.threeDSettings.drawMethod)
                Main.main.updateTravelMoves();
        }
        private void DrawViewpoint()
        {
            GL.Color4(convertColor(Main.threeDSettings.printerFrame.BackColor));
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.DepthTest);
            GL.Begin(BeginMode.Lines);
            GL.Vertex3(cam.viewCenter.X - 2, cam.viewCenter.Y, cam.viewCenter.Z);
            GL.Vertex3(cam.viewCenter.X + 2, cam.viewCenter.Y, cam.viewCenter.Z);
            GL.Vertex3(cam.viewCenter.X, cam.viewCenter.Y - 2, cam.viewCenter.Z);
            GL.Vertex3(cam.viewCenter.X, cam.viewCenter.Y + 2, cam.viewCenter.Z);
            GL.Vertex3(cam.viewCenter.X, cam.viewCenter.Y, cam.viewCenter.Z - 2);
            GL.Vertex3(cam.viewCenter.X, cam.viewCenter.Y, cam.viewCenter.Z + 2);
            GL.End();
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.DepthTest);

        }
        private void DrawModels()
        {
            GL.Disable(EnableCap.LineSmooth);
            if (Main.main.tab.SelectedIndex > 1)
                GL.Enable(EnableCap.CullFace);
            else
                GL.Disable(EnableCap.CullFace);
            foreach (ThreeDModel model in view.models)
            {
                GL.PushMatrix();
                model.AnimationBefore();
                GL.Translate(model.Position.x, model.Position.y, model.Position.z);
                GL.Rotate(model.Rotation.z, Vector3.UnitZ);
                GL.Rotate(model.Rotation.y, Vector3.UnitY);
                GL.Rotate(model.Rotation.x, Vector3.UnitX);
                GL.Scale(model.Scale.x, model.Scale.y, model.Scale.z);
                model.Paint();
                model.AnimationAfter();
                GL.PopMatrix();
                if (model.Selected)
                {
                    GL.PushMatrix();
                    model.AnimationBefore();
                    Color col = Main.threeDSettings.selectionBox.BackColor;
                    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.AmbientAndDiffuse, new OpenTK.Graphics.Color4(0, 0, 0, 255));
                    GL.Material(MaterialFace.Front, MaterialParameter.Emission, new OpenTK.Graphics.Color4(0, 0, 0, 0));
                    GL.Material(MaterialFace.Front, MaterialParameter.Specular, new float[] { 0.0f, 0.0f, 0.0f, 1.0f });
                    GL.Material(
                        MaterialFace.Front,
                        MaterialParameter.Emission,
                        new OpenTK.Graphics.Color4(col.R, col.G, col.B, col.A));
                    GL.Begin(BeginMode.Lines);
                    GL.Vertex3(model.xMin, model.yMin, model.zMin);
                    GL.Vertex3(model.xMax, model.yMin, model.zMin);

                    GL.Vertex3(model.xMin, model.yMin, model.zMin);
                    GL.Vertex3(model.xMin, model.yMax, model.zMin);

                    GL.Vertex3(model.xMin, model.yMin, model.zMin);
                    GL.Vertex3(model.xMin, model.yMin, model.zMax);

                    GL.Vertex3(model.xMax, model.yMax, model.zMax);
                    GL.Vertex3(model.xMin, model.yMax, model.zMax);

                    GL.Vertex3(model.xMax, model.yMax, model.zMax);
                    GL.Vertex3(model.xMax, model.yMin, model.zMax);

                    GL.Vertex3(model.xMax, model.yMax, model.zMax);
                    GL.Vertex3(model.xMax, model.yMax, model.zMin);

                    GL.Vertex3(model.xMin, model.yMax, model.zMax);
                    GL.Vertex3(model.xMin, model.yMax, model.zMin);

                    GL.Vertex3(model.xMin, model.yMax, model.zMax);
                    GL.Vertex3(model.xMin, model.yMin, model.zMax);

                    GL.Vertex3(model.xMax, model.yMax, model.zMin);
                    GL.Vertex3(model.xMax, model.yMin, model.zMin);

                    GL.Vertex3(model.xMax, model.yMax, model.zMin);
                    GL.Vertex3(model.xMin, model.yMax, model.zMin);

                    GL.Vertex3(model.xMax, model.yMin, model.zMax);
                    GL.Vertex3(model.xMin, model.yMin, model.zMax);

                    GL.Vertex3(model.xMax, model.yMin, model.zMax);
                    GL.Vertex3(model.xMax, model.yMin, model.zMin);

                    GL.End();
                    model.AnimationAfter();
                    GL.PopMatrix();
                }
            }
        }
        private void DrawPrintbedFrame()
        {
            float dx1 = ps.DumpAreaLeft;
            float dx2 = dx1 + ps.DumpAreaWidth;
            float dy1 = ps.DumpAreaFront;
            float dy2 = dy1 + ps.DumpAreaDepth;
            GL.LineWidth(1f);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.LineSmooth);
            GL.DepthFunc(DepthFunction.Lequal);
            Color col = Main.threeDSettings.printerFrame.BackColor;
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.AmbientAndDiffuse, new OpenTK.Graphics.Color4(0, 0, 0, 255));
            GL.Material(MaterialFace.Front, MaterialParameter.Emission, new OpenTK.Graphics.Color4(0, 0, 0, 0));
            GL.Material(MaterialFace.Front, MaterialParameter.Specular, new float[] { 0.0f, 0.0f, 0.0f, 1.0f });
            GL.Material(
                MaterialFace.Front,
                MaterialParameter.Emission,
                new OpenTK.Graphics.Color4(col.R, col.G, col.B, col.A));
            if (Main.threeDSettings.showPrintbed.Checked)
            {
                int i;
                // Draw origin
                GL.Disable(EnableCap.CullFace);
                GL.Begin(BeginMode.Triangles);
                GL.Normal3(0, 0, 1);
                double delta = Math.PI / 8;
                double rad = 2.5;
                for (i = 0; i < 16; i++)
                {
                    GL.Vertex3(0, 0, 0);
                    GL.Vertex3(rad * Math.Sin(i * delta), rad * Math.Cos(i * delta), 0);
                    GL.Vertex3(rad * Math.Sin((i + 1) * delta), rad * Math.Cos((i + 1) * delta), 0);
                }
                GL.End();
                GL.Begin(BeginMode.Lines);
                if (ps.printerType < 2)
                {
                    // Print cube
                    GL.Vertex3(ps.BedLeft, ps.BedFront, 0);
                    GL.Vertex3(ps.BedLeft, ps.BedFront, ps.PrintAreaHeight);
                    GL.Vertex3(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront, 0);
                    GL.Vertex3(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront, ps.PrintAreaHeight);
                    GL.Vertex3(ps.BedLeft, ps.BedFront + ps.PrintAreaDepth, 0);
                    GL.Vertex3(ps.BedLeft, ps.BedFront + ps.PrintAreaDepth, ps.PrintAreaHeight);
                    GL.Vertex3(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront + ps.PrintAreaDepth, 0);
                    GL.Vertex3(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront + ps.PrintAreaDepth, ps.PrintAreaHeight);
                    GL.Vertex3(ps.BedLeft, ps.BedFront, ps.PrintAreaHeight);
                    GL.Vertex3(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront, ps.PrintAreaHeight);
                    GL.Vertex3(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront, ps.PrintAreaHeight);
                    GL.Vertex3(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront + ps.PrintAreaDepth, ps.PrintAreaHeight);
                    GL.Vertex3(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront + ps.PrintAreaDepth, ps.PrintAreaHeight);
                    GL.Vertex3(ps.BedLeft, ps.BedFront + ps.PrintAreaDepth, ps.PrintAreaHeight);
                    GL.Vertex3(ps.BedLeft, ps.BedFront + ps.PrintAreaDepth, ps.PrintAreaHeight);
                    GL.Vertex3(ps.BedLeft, ps.BedFront, ps.PrintAreaHeight);
                    if (ps.printerType == 1)
                    {
                        if (dy1 != 0)
                        {
                            GL.Vertex3(ps.BedLeft + dx1, ps.BedFront + dy1, 0);
                            GL.Vertex3(ps.BedLeft + dx2, ps.BedFront + dy1, 0);
                        }
                        GL.Vertex3(ps.BedLeft + dx2, ps.BedFront + dy1, 0);
                        GL.Vertex3(ps.BedLeft + dx2, ps.BedFront + dy2, 0);
                        GL.Vertex3(ps.BedLeft + dx2, ps.BedFront + dy2, 0);
                        GL.Vertex3(ps.BedLeft + dx1, ps.BedFront + dy2, 0);
                        GL.Vertex3(ps.BedLeft + dx1, ps.BedFront + dy2, 0);
                        GL.Vertex3(ps.BedLeft + dx1, ps.BedFront + dy1, 0);
                    }
                    float dx = 10; // ps.PrintAreaWidth / 20f;
                    float dy = 10; // ps.PrintAreaDepth / 20f;
                    float x, y;
                    for (i = 0; i < 200; i++)
                    {
                        x = (float)i * dx;
                        if (x >= ps.PrintAreaWidth)
                            x = ps.PrintAreaWidth;
                        if (ps.printerType == 1 && x >= dx1 && x <= dx2)
                        {
                            GL.Vertex3(ps.BedLeft + x, ps.BedFront, 0);
                            GL.Vertex3(ps.BedLeft + x, ps.BedFront + dy1, 0);
                            GL.Vertex3(ps.BedLeft + x, ps.BedFront + dy2, 0);
                            GL.Vertex3(ps.BedLeft + x, ps.BedFront + ps.PrintAreaDepth, 0);
                        }
                        else
                        {
                            GL.Vertex3(ps.BedLeft + x, ps.BedFront, 0);
                            GL.Vertex3(ps.BedLeft + x, ps.BedFront + ps.PrintAreaDepth, 0);
                        }
                        if (x >= ps.PrintAreaWidth) break;
                    }
                    for (i = 0; i < 200; i++)
                    {
                        y = (float)i * dy;
                        if (y > ps.PrintAreaDepth)
                            y = ps.PrintAreaDepth;
                        if (ps.printerType == 1 && y >= dy1 && y <= dy2)
                        {
                            GL.Vertex3(ps.BedLeft, ps.BedFront + y, 0);
                            GL.Vertex3(ps.BedLeft + dx1, ps.BedFront + y, 0);
                            GL.Vertex3(ps.BedLeft + dx2, ps.BedFront + y, 0);
                            GL.Vertex3(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront + y, 0);
                        }
                        else
                        {
                            GL.Vertex3(ps.BedLeft, ps.BedFront + y, 0);
                            GL.Vertex3(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront + y, 0);
                        }
                        if (y >= ps.PrintAreaDepth)
                            break;
                    }
                }
                else if (ps.printerType == 2) // Cylinder shape
                {
                    int ncirc = 32;
                    int vertexevery = 4;
                    delta = (float)(Math.PI * 2 / ncirc);
                    float alpha = 0;
                    for (i = 0; i < ncirc; i++)
                    {
                        float alpha2 = (float)(alpha + delta);
                        float x1 = (float)(ps.rostockRadius * Math.Sin(alpha));
                        float y1 = (float)(ps.rostockRadius * Math.Cos(alpha));
                        float x2 = (float)(ps.rostockRadius * Math.Sin(alpha2));
                        float y2 = (float)(ps.rostockRadius * Math.Cos(alpha2));
                        GL.Vertex3(x1, y1, 0);
                        GL.Vertex3(x2, y2, 0);
                        GL.Vertex3(x1, y1, ps.rostockHeight);
                        GL.Vertex3(x2, y2, ps.rostockHeight);
                        if ((i % vertexevery) == 0)
                        {
                            GL.Vertex3(x1, y1, 0);
                            GL.Vertex3(x1, y1, ps.rostockHeight);
                        }
                        alpha = alpha2;
                    }
                    delta = 10;
                    float x = (float)(Math.Floor(ps.rostockRadius / delta) * delta);
                    while (x > -ps.rostockRadius)
                    {
                        alpha = (float)Math.Acos(x / ps.rostockRadius);
                        float y = (float)(ps.rostockRadius * Math.Sin(alpha));
                        GL.Vertex3(x, -y, 0);
                        GL.Vertex3(x, y, 0);
                        GL.Vertex3(y, x, 0);
                        GL.Vertex3(-y, x, 0);
                        x -= (float)delta;
                    }
                }
                else if (ps.printerType == 3)
                {
                    float dx = 10; // ps.PrintAreaWidth / 20f;
                    float dy = 10; // ps.PrintAreaDepth / 20f;
                    float x, y;
                    for (i = 0; i < 200; i++)
                    {
                        x = (float)i * dx;
                        if (x >= ps.PrintAreaWidth)
                            x = ps.PrintAreaWidth;
                        if (ps.printerType == 1 && x >= dx1 && x <= dx2)
                        {
                            GL.Vertex3(ps.BedLeft + x, ps.BedFront, 0);
                            GL.Vertex3(ps.BedLeft + x, ps.BedFront + dy1, 0);
                            GL.Vertex3(ps.BedLeft + x, ps.BedFront + dy2, 0);
                            GL.Vertex3(ps.BedLeft + x, ps.BedFront + ps.PrintAreaDepth, 0);
                        }
                        else
                        {
                            GL.Vertex3(ps.BedLeft + x, ps.BedFront, 0);
                            GL.Vertex3(ps.BedLeft + x, ps.BedFront + ps.PrintAreaDepth, 0);
                        }
                        if (x >= ps.PrintAreaWidth) break;
                    }
                    for (i = 0; i < 200; i++)
                    {
                        y = (float)i * dy;
                        if (y > ps.PrintAreaDepth)
                            y = ps.PrintAreaDepth;
                        if (ps.printerType == 1 && y >= dy1 && y <= dy2)
                        {
                            GL.Vertex3(ps.BedLeft, ps.BedFront + y, 0);
                            GL.Vertex3(ps.BedLeft + dx1, ps.BedFront + y, 0);
                            GL.Vertex3(ps.BedLeft + dx2, ps.BedFront + y, 0);
                            GL.Vertex3(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront + y, 0);
                        }
                        else
                        {
                            GL.Vertex3(ps.BedLeft, ps.BedFront + y, 0);
                            GL.Vertex3(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront + y, 0);
                        }
                        if (y >= ps.PrintAreaDepth)
                            break;
                    }
                }
                GL.End();
                GL.DepthFunc(DepthFunction.Less);
            }
        }
        private void DrawPrintbedBase()
        {
            float dx1 = ps.DumpAreaLeft;
            float dx2 = dx1 + ps.DumpAreaWidth;
            float dy1 = ps.DumpAreaFront;
            float dy2 = dy1 + ps.DumpAreaDepth;

            if (Main.threeDSettings.showPrintbed.Checked)
            {
                GL.Disable(EnableCap.CullFace);
                GL.Enable(EnableCap.Blend);	// Turn Blending On
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                GL.DepthFunc(DepthFunction.Less);
                //GL.Disable(EnableCap.Lighting);
                // Draw bottom
                Color col = Main.threeDSettings.printerBase.BackColor;
                float[] transblack = new float[] { 0, 0, 0, 0 };
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.AmbientAndDiffuse, new OpenTK.Graphics.Color4(col.R, col.G, col.B, 130));
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, transblack);
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, transblack);
                GL.PushMatrix();
                if (cam.phi < Math.PI / 2)
                    GL.Translate(0, 0, -0.04);
                else
                    GL.Translate(0, 0, +0.04);
                if (ps.printerType < 2 || ps.printerType == 3)
                {
                    GL.Begin(BeginMode.Quads);
                    GL.Normal3(0, 0, 1);

                    if (ps.printerType == 1)
                    {
                        if (dy1 > 0)
                        {
                            GL.Vertex3(ps.BedLeft, ps.BedFront, 0);
                            GL.Vertex3(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront, 0);
                            GL.Vertex3(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront + dy1, 0);
                            GL.Vertex3(ps.BedLeft, ps.BedFront + dy1, 0);
                        }
                        if (dy2 < ps.PrintAreaDepth)
                        {
                            GL.Vertex3(ps.BedLeft, ps.BedFront + dy2, 0);
                            GL.Vertex3(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront + dy2, 0);
                            GL.Vertex3(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront + ps.PrintAreaDepth, 0);
                            GL.Vertex3(ps.BedLeft, ps.BedFront + ps.PrintAreaDepth, 0);
                        }
                        if (dx1 > 0)
                        {
                            GL.Vertex3(ps.BedLeft, ps.BedFront + dy1, 0);
                            GL.Vertex3(ps.BedLeft + dx1, ps.BedFront + dy1, 0);
                            GL.Vertex3(ps.BedLeft + dx1, ps.BedFront + dy2, 0);
                            GL.Vertex3(ps.BedLeft, ps.BedFront + dy2, 0);
                        }
                        if (dx2 < ps.PrintAreaWidth)
                        {
                            GL.Vertex3(ps.BedLeft + dx2, ps.BedFront + dy1, 0);
                            GL.Vertex3(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront + dy1, 0);
                            GL.Vertex3(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront + dy2, 0);
                            GL.Vertex3(ps.BedLeft + dx2, ps.BedFront + dy2, 0);
                        }
                    }
                    else
                    {
                        GL.Vertex3(ps.BedLeft, ps.BedFront, 0);
                        GL.Vertex3(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront, 0);
                        GL.Vertex3(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront + ps.PrintAreaDepth, 0);
                        GL.Vertex3(ps.BedLeft + 0, ps.BedFront + ps.PrintAreaDepth, 0);
                    }

                    GL.End();
                }
                else if (ps.printerType == 2)
                {
                    int ncirc = 32;
                    float delta = (float)(Math.PI * 2 / ncirc);
                    float alpha = 0;
                    GL.Begin(BeginMode.Quads);
                    GL.Normal3(0, 0, 1);
                    for (int i = 0; i < ncirc / 4; i++)
                    {
                        float alpha2 = (float)(alpha + delta);
                        float x1 = (float)(ps.rostockRadius * Math.Sin(alpha));
                        float y1 = (float)(ps.rostockRadius * Math.Cos(alpha));
                        float x2 = (float)(ps.rostockRadius * Math.Sin(alpha2));
                        float y2 = (float)(ps.rostockRadius * Math.Cos(alpha2));
                        GL.Vertex3(x1, y1, 0);
                        GL.Vertex3(x2, y2, 0);
                        GL.Vertex3(-x2, y2, 0);
                        GL.Vertex3(-x1, y1, 0);
                        GL.Vertex3(x1, -y1, 0);
                        GL.Vertex3(x2, -y2, 0);
                        GL.Vertex3(-x2, -y2, 0);
                        GL.Vertex3(-x1, -y1, 0);
                        alpha = alpha2;
                    }
                    GL.End();
                }
                GL.PopMatrix();
                GL.Disable(EnableCap.Blend);

            }
        }
        bool oldCut = false;
        int oldPosition = -1;
        int oldInclination = -1;
        int oldAzimuth = -1;
        public bool updateCuts = false;
        public RHVector3 cutPos = new RHVector3(0, 0, 0);
        public RHVector3 cutDirection = new RHVector3(0, 0, 1);
        RHBoundingBox cutBBox = new RHBoundingBox();
        void UpdateCutData()
        {
            updateCuts = true;
            cutBBox.Clear();
            foreach (PrintModel model in Main.main.objectPlacement.ListObjects(false))
            {
                cutBBox.Add(model.bbox);
            }
            oldPosition = Main.main.objectPlacement.cutPositionSlider.Value;
            oldInclination = Main.main.objectPlacement.cutInclinationSlider.Value;
            oldAzimuth = Main.main.objectPlacement.cutAzimuthSlider.Value;
            double inclination = (double)oldInclination * Math.PI / 1800.0;
            double azimuth = (double)oldAzimuth * Math.PI / 1800.0;
            cutDirection.x = Math.Sin(inclination) * Math.Cos(azimuth);
            cutDirection.y = Math.Sin(inclination) * Math.Sin(azimuth);
            cutDirection.z = Math.Cos(inclination);
            RHVector3 center = cutBBox.Center;
            TopoPlane plane = new TopoPlane(cutDirection, center);
            double min = 0, max = 0, dist;
            RHVector3 p = new RHVector3(cutBBox.xMin, cutBBox.yMin, cutBBox.zMin);
            dist = plane.VertexDistance(p);
            max = Math.Max(dist, max);
            min = Math.Min(dist, min);
            p = new RHVector3(cutBBox.xMax, cutBBox.yMin, cutBBox.zMin);
            dist = plane.VertexDistance(p);
            max = Math.Max(dist, max);
            min = Math.Min(dist, min);
            p = new RHVector3(cutBBox.xMin, cutBBox.yMax, cutBBox.zMin);
            dist = plane.VertexDistance(p);
            max = Math.Max(dist, max);
            min = Math.Min(dist, min);
            p = new RHVector3(cutBBox.xMax, cutBBox.yMax, cutBBox.zMin);
            dist = plane.VertexDistance(p);
            max = Math.Max(dist, max);
            min = Math.Min(dist, min);
            p = new RHVector3(cutBBox.xMin, cutBBox.yMin, cutBBox.zMax);
            dist = plane.VertexDistance(p);
            max = Math.Max(dist, max);
            min = Math.Min(dist, min);
            p = new RHVector3(cutBBox.xMax, cutBBox.yMin, cutBBox.zMax);
            dist = plane.VertexDistance(p);
            max = Math.Max(dist, max);
            min = Math.Min(dist, min);
            p = new RHVector3(cutBBox.xMin, cutBBox.yMax, cutBBox.zMax);
            dist = plane.VertexDistance(p);
            max = Math.Max(dist, max);
            min = Math.Min(dist, min);
            p = new RHVector3(cutBBox.xMax, cutBBox.yMax, cutBBox.zMax);
            dist = plane.VertexDistance(p);
            max = Math.Max(dist, max);
            min = Math.Min(dist, min);
            double spos = min + 1.001 * (max - min) * (double)oldPosition / 1000.0;
            cutPos.x = center.x + spos * cutDirection.x;
            cutPos.y = center.y + spos * cutDirection.y;
            cutPos.z = center.z + spos * cutDirection.z;
        }
        private void gl_Paint(object sender, PaintEventArgs e)
        {
            if (view == null) return;
            try
            {
                if (!loaded) return;
                DetectDrawingMethod();
                if (oldCut != Main.main.objectPlacement.checkCutFaces.Checked || oldPosition != Main.main.objectPlacement.cutPositionSlider.Value
                    || oldInclination != Main.main.objectPlacement.cutInclinationSlider.Value || oldAzimuth != Main.main.objectPlacement.cutAzimuthSlider.Value || updateCuts)
                {
                    UpdateCutData();
                }
                fpsTimer.Reset();
                fpsTimer.Start();
                gl.MakeCurrent();
                //GL.Enable(EnableCap.Multisample);
                GL.ClearColor(Main.threeDSettings.backgroundTop.BackColor);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                // Draw gradient background
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();
                GL.Disable(EnableCap.DepthTest);
                GL.Disable(EnableCap.Lighting);
                GL.Begin(BeginMode.Quads);
                GL.Color4(convertColor(Main.threeDSettings.backgroundBottom.BackColor));
                GL.Vertex2(-1.0, -1.0);
                GL.Vertex2(1.0, -1.0);
                GL.Color4(convertColor(Main.threeDSettings.backgroundTop.BackColor));
                GL.Vertex2(1.0, 1.0);
                GL.Vertex2(-1.0, 1.0);
                GL.End();

                GL.Enable(EnableCap.DepthTest);
                SetupViewport();
                Vector3 camPos = cam.CameraPosition;
                lookAt = Matrix4.LookAt(camPos.X, camPos.Y, camPos.Z, cam.viewCenter.X, cam.viewCenter.Y, cam.viewCenter.Z, 0, 0, 1.0f);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref lookAt);
                GL.ShadeModel(ShadingModel.Smooth);
                // GL.Enable(EnableCap.LineSmooth);
                AddLights();
                //Enable Backfaceculling
                GL.Enable(EnableCap.CullFace);
                //GL.Enable(EnableCap.PolygonSmooth);
                GL.Enable(EnableCap.Blend);
                GL.LineWidth(2f);
                GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                Color col = Main.threeDSettings.printerBase.BackColor;
                //  GL.Translate(-ps.BedLeft-ps.PrintAreaWidth * 0.5f,-ps.BedFront -ps.PrintAreaDepth * 0.5f, -0.5f * ps.PrintAreaHeight);
                GL.GetFloat(GetPName.ModelviewMatrix, out modelView);
                GL.Material(
                    MaterialFace.Front,
                    MaterialParameter.Specular,
                    new OpenTK.Graphics.Color4(255, 255, 255, 255));

                DrawModels();
                DrawPrintbedFrame();
                DrawPrintbedBase();
                if (Control.MouseButtons != MouseButtons.None)
                    DrawViewpoint();
                DrawCoordinate();

                gl.SwapBuffers();
                fpsTimer.Stop();
                // double time = fpsTimer.Elapsed.Milliseconds / 1000.0;
                // PrinterConnection.logInfo("OpenGL update time:" + time.ToString());
                double framerate = 1.0 / fpsTimer.Elapsed.TotalSeconds;
                Main.main.fpsLabel.Text = framerate.ToString("0") + " FPS";
                if (framerate < 30 && Main.globalSettings.DisableQualityReduction == false)
                {
                    slowCounter++;
                    if (slowCounter >= 10)
                    {
                        slowCounter = 0;
                        foreach (ThreeDModel model in view.models)
                        {
                            model.ReduceQuality();
                        }
                    }
                }
                else if (slowCounter > 0)
                    slowCounter--;

            }
            catch { }
            updateCuts = false;
            oldCut = Main.main.objectPlacement.checkCutFaces.Checked;
        }

        static bool configureSettings = true;
        private void ThreeDControl_Load(object sender, EventArgs e)
        {
            if (configureSettings)
            {
                try
                {
                    Main.conn.log("OpenGL version:" + GL.GetString(StringName.Version), false, 3);
                    Main.conn.log("OpenGL extensions:" + GL.GetString(StringName.Extensions), false, 3);
                    Main.conn.log("OpenGL renderer:" + GL.GetString(StringName.Renderer), false, 3);
                    string sv = GL.GetString(StringName.Version).Trim();
                    int p = sv.IndexOf(" ");
                    if (p > 0) sv = sv.Substring(0, p);
                    p = sv.IndexOf('.');
                    if (p > 0)
                    {
                        p = sv.IndexOf('.', p + 1);
                        if (p > 0)
                            sv = sv.Substring(0, p);
                    }
                    try
                    {
                        float val = 0;
                        float.TryParse(sv, NumberStyles.Float, GCode.format, out val);
                        Main.threeDSettings.openGLVersion = val;
                    }
                    catch
                    {
                        Main.threeDSettings.openGLVersion = 1.1f;
                    }
                    string extensions = GL.GetString(StringName.Extensions);
                    Main.threeDSettings.useVBOs = false;
                    foreach (string s in extensions.Split(' '))
                    {
                        if (s.Equals("GL_ARB_vertex_buffer_object")/* && Main.threeDSettings.openGLVersion>1.49*/)
                        {
                            Main.threeDSettings.useVBOs = true;
                        }
                    }
                    if (Main.threeDSettings.useVBOs)
                        Main.conn.log("Using fast VBOs for rendering is possible", false, 3);
                    else
                        Main.conn.log("Fast VBOs for rendering not supported. Using slower default method.", false, 3);
                    //  Main.threeDSettings.useVBOs = false;
                }
                catch { }
                configureSettings = false;
            }
            loaded = true;
            SetupViewport();
        }
        private Matrix4 GluPickMatrix(float x, float y, float width, float height, int[] viewport)
        {
            Matrix4 result = Matrix4.Identity;
            if ((width <= 0.0f) || (height <= 0.0f))
            {
                return result;
            }

            float translateX = (viewport[2] - (2.0f * (x - viewport[0]))) / width;
            float translateY = (viewport[3] - (2.0f * (y - viewport[1]))) / height;
            result = Matrix4.Mult(Matrix4.CreateTranslation(translateX, translateY, 0.0f), result);
            float scaleX = viewport[2] / width;
            float scaleY = viewport[3] / height;
            result = Matrix4.Mult(Matrix4.Scale(scaleX, scaleY, 1.0f), result);
            return result;
        }
        public uint lastDepth = 0;
        public Geom3DLine pickLine = null; // Last pick up line ray
        public Geom3DLine viewLine = null; // Direction of view
        public Geom3DVector pickPoint = new Geom3DVector(0, 0, 0); // Koordinates of last pick

        public void UpdatePickLine(int x, int y)
        {
            if (view == null) return;
            // Intersection on bottom plane

            int window_y = (Height - y) - Height / 2;
            double norm_y = (double)window_y / (double)(Height / 2);
            int window_x = x - Width / 2;
            double norm_x = (double)window_x / (double)(Width / 2);
            float fpy = (float)(nearHeight * 0.5 * norm_y) * (toolParallelProjection.Checked ? 1f : 1f);
            float fpx = (float)(nearHeight * 0.5 * aspectRatio * norm_x) * (toolParallelProjection.Checked ? 1f : 1f);


            Vector4 frontPointN = (toolParallelProjection.Checked ? new Vector4(fpx, fpy, 0, 1) : new Vector4(0, 0, 0, 1));
            Vector4 dirN = (toolParallelProjection.Checked ? new Vector4(0, 0, -nearDist, 0) : new Vector4(fpx, fpy, -nearDist, 0));
            Matrix4 ntrans;
            Vector3 camPos = cam.CameraPosition;
            ntrans = Matrix4.LookAt(camPos.X, camPos.Y, camPos.Z, cam.viewCenter.X, cam.viewCenter.Y, cam.viewCenter.Z, 0, 0, 1.0f);
            ntrans = Matrix4.Invert(ntrans);
            Vector4 frontPoint = (toolParallelProjection.Checked ? Vector4.Transform(frontPointN, ntrans) : ntrans.Row3);
            //Vector4 frontPoint = (toolParallelProjection.Checked ? frontPointN : ntrans.Row3);
            Vector4 dirVec = Vector4.Transform(dirN, ntrans);
            pickLine = new Geom3DLine(new Geom3DVector(frontPoint.X / frontPoint.W, frontPoint.Y / frontPoint.W, frontPoint.Z / frontPoint.W),
                new Geom3DVector(dirVec.X, dirVec.Y, dirVec.Z), true);
            pickLine.dir.normalize();
            /*Geom3DPlane plane = new Geom3DPlane(new Geom3DVector(0, 0, 0), new Geom3DVector(0, 0, 1));
            Geom3DVector cross = new Geom3DVector(0, 0, 0);
            plane.intersectLine(pickLine, cross);
            */
        }
        private ThreeDModel Picktest(int x, int y)
        {
            if (view == null) return null;
            // int x = Mouse.X;
            // int y = Mouse.Y;
            // Console.WriteLine("X:" + x + " Y:" + y);
            gl.MakeCurrent();
            uint[] selectBuffer = new uint[128];
            GL.SelectBuffer(128, selectBuffer);
            GL.RenderMode(RenderingMode.Select);
            SetupViewport();

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();

            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);

            Matrix4 m = GluPickMatrix(x, viewport[3] - y, 1, 1, viewport);
            GL.MultMatrix(ref m);


            //GluPerspective(45, 32 / 24, 0.1f, 100.0f);
            //Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, 1, 0.1f, 100.0f);
            GL.MultMatrix(ref persp);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.ClearColor(Main.threeDSettings.backgroundTop.BackColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);
            Vector3 camPos = cam.CameraPosition;
            lookAt = Matrix4.LookAt(camPos.X, camPos.Y, camPos.Z, cam.viewCenter.X, cam.viewCenter.Y, cam.viewCenter.Z, 0, 0, 1.0f);

            // Intersection on bottom plane

            int window_y = (viewport[3] - y) - viewport[3] / 2;
            double norm_y = (double)window_y / (double)(viewport[3] / 2);
            int window_x = x - viewport[2] / 2;
            double norm_x = (double)window_x / (double)(viewport[2] / 2);
            float fpy = (float)(nearHeight * 0.5 * norm_y) * (toolParallelProjection.Checked ? 1f : 1f);
            float fpx = (float)(nearHeight * 0.5 * aspectRatio * norm_x) * (toolParallelProjection.Checked ? 1f : 1f);


            Vector4 frontPointN = (toolParallelProjection.Checked ? new Vector4(fpx, fpy, 0, 1) : new Vector4(0, 0, 0, 1));
            Vector4 dirN = (toolParallelProjection.Checked ? new Vector4(0, 0, -nearDist, 0) : new Vector4(fpx, fpy, -nearDist, 0));
            //Matrix4 trans = Matrix4.CreateTranslation(-ps.BedLeft - ps.PrintAreaWidth * 0.5f, -ps.BedFront - ps.PrintAreaDepth * 0.5f, -0.5f * ps.PrintAreaHeight);
            Matrix4 ntrans = lookAt;
            // ntrans = Matrix4.Mult(trans, ntrans);
            ntrans = Matrix4.Invert(ntrans);
            Vector4 frontPoint = (toolParallelProjection.Checked ? Vector4.Transform(frontPointN, ntrans) : ntrans.Row3);
            Vector4 dirVec = Vector4.Transform(dirN, ntrans);
            pickLine = new Geom3DLine(new Geom3DVector(frontPoint.X / frontPoint.W, frontPoint.Y / frontPoint.W, frontPoint.Z / frontPoint.W),
                new Geom3DVector(dirVec.X, dirVec.Y, dirVec.Z), true);
            dirN = new Vector4(0, 0, -nearDist, 0);
            dirVec = Vector4.Transform(dirN, ntrans);
            viewLine = new Geom3DLine(new Geom3DVector(frontPoint.X / frontPoint.W, frontPoint.Y / frontPoint.W, frontPoint.Z / frontPoint.W),
                new Geom3DVector(dirVec.X, dirVec.Y, dirVec.Z), true);
            viewLine.dir.normalize();
            pickLine.dir.normalize();
            /* Geom3DPlane plane = new Geom3DPlane(new Geom3DVector(0, 0, 0), new Geom3DVector(0, 0, 1));
             Geom3DVector cross = new Geom3DVector(0, 0, 0);
             plane.intersectLine(pickLine, cross);
             Main.conn.log("Linie: " + pickLine, false, 3);
             Main.conn.log("Schnittpunkt: " + cross, false, 3);
             */
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookAt);
            //GL.Translate(-ps.BedLeft - ps.PrintAreaWidth * 0.5f, -ps.BedFront - ps.PrintAreaDepth * 0.5f, -0.5f * ps.PrintAreaHeight);



            GL.InitNames();
            int pos = 0;
            foreach (ThreeDModel model in view.models)
            {
                GL.PushName(pos++);
                GL.PushMatrix();
                model.AnimationBefore();
                GL.Translate(model.Position.x, model.Position.y, model.Position.z);
                GL.Rotate(model.Rotation.z, Vector3.UnitZ);
                GL.Rotate(model.Rotation.y, Vector3.UnitY);
                GL.Rotate(model.Rotation.x, Vector3.UnitX);
                GL.Scale(model.Scale.x, model.Scale.y, model.Scale.z);
                model.Paint();
                model.AnimationAfter();
                GL.PopMatrix();
                GL.PopName();
            }
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);

            int hits = GL.RenderMode(RenderingMode.Render);
            ThreeDModel selected = null;
            if (hits > 0)
            {
                selected = view.models.ElementAt((int)selectBuffer[3]);
                lastDepth = selectBuffer[1];
                for (int i = 1; i < hits; i++)
                {
                    if (selectBuffer[4 * i + 1] < lastDepth)
                    {
                        lastDepth = selectBuffer[i * 4 + 1];
                        selected = view.models.ElementAt((int)selectBuffer[i * 4 + 3]);
                    }
                }
                double dfac = (double)lastDepth / uint.MaxValue;
                dfac = -(farDist * nearDist) / (dfac * (farDist - nearDist) - farDist);
                Geom3DVector crossPlanePoint = new Geom3DVector(viewLine.dir).scale((float)dfac).add(viewLine.point);
                Geom3DPlane objplane = new Geom3DPlane(crossPlanePoint, viewLine.dir);
                objplane.intersectLine(pickLine, pickPoint);
                //Main.conn.log("Objekttreffer: " + pickPoint, false, 3);

            }
            //PrinterConnection.logInfo("Hits: " + hits);
            return selected;
        }
        private void gl_Resize(object sender, EventArgs e)
        {
            SetupViewport();
            gl.Invalidate();
        }
        Geom3DPlane movePlane = new Geom3DPlane(new Geom3DVector(0, 0, 0), new Geom3DVector(0, 0, 1)); // Plane where object movement occurs
        Geom3DVector moveStart = new Geom3DVector(0, 0, 0), moveLast = new Geom3DVector(0, 0, 0), movePos = new Geom3DVector(0, 0, 0);
        private void gl_MouseDown(object sender, MouseEventArgs e)
        {
            cam.PreparePanZoomRot();
            lastX = xDown = e.X;
            lastY = yDown = e.Y;
            movePlane = new Geom3DPlane(new Geom3DVector(0, 0, 0), new Geom3DVector(0, 0, 1));
            moveStart = moveLast = new Geom3DVector(0, 0, 0);
            UpdatePickLine(e.X, e.Y);
            movePlane.intersectLine(pickLine, moveStart);
            if (e.Button == MouseButtons.Right)
            {
                ThreeDModel sel = Picktest(e.X, e.Y);
                if (sel != null)
                {
                    movePlane = new Geom3DPlane(pickPoint, new Geom3DVector(0, 0, 1));
                    moveStart = moveLast = new Geom3DVector(pickPoint);
                }
                if (sel != null && view.eventObjectMoved != null)
                    view.eventObjectSelected(sel);
                //computeRay();
            }
            gl.Invalidate();
        }

        private void gl_MouseMove(object sender, MouseEventArgs e)
        {
            double window_y = (gl.Height - e.Y) - gl.Height / 2;
            normY = window_y * 2.0 / (double)(gl.Height);
            double window_x = e.X - gl.Width / 2;
            normX = window_x * 2.0 / (double)(gl.Width);
            if (e.Button == MouseButtons.None)
            {
                speedX = speedY = 0;
                return;
            }
            xPos = e.X;
            yPos = e.Y;
            UpdatePickLine(e.X, e.Y);
            movePos = new Geom3DVector(0, 0, 0);
            movePlane.intersectLine(pickLine, movePos);
            float d = Math.Min(gl.Width, gl.Height) / 3;
            speedX = Math.Max(-1, Math.Min(1, (xPos - xDown) / d));
            speedY = Math.Max(-1, Math.Min(1, (yPos - yDown) / d));
        }

        private void gl_MouseUp(object sender, MouseEventArgs e)
        {
            speedX = speedY = 0;
            gl.Invalidate();
        }
        private void gl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                cam.PreparePanZoomRot();
                cam.Zoom(1f - e.Delta / 6000f);
                if (toolParallelProjection.Checked)
                    zoom *= 1f - e.Delta / 4000f;
                else
                    zoom *= 1f - e.Delta / 2000f;
                if (zoom < 0.002) zoom = 0.002f;
                if (zoom > 5.9) zoom = 5.9f;
                //userPosition.Y += e.Delta;
                gl.Invalidate();
            }
        }
        void Application_Idle(object sender, EventArgs e)
        {
            if (!loaded || (speedX == 0 && speedY == 0)) return;
            // no guard needed -- we hooked into the event in Load handler

            sw.Stop(); // we've measured everything since last Idle run
            double milliseconds = sw.Elapsed.TotalMilliseconds;
            sw.Reset(); // reset stopwatch
            sw.Start(); // restart stopwatch
            Keys k = Control.ModifierKeys;
            int emode = mode;
            if (k == Keys.Shift || Control.MouseButtons == MouseButtons.Middle) emode = 2;
            if (k == Keys.Control) emode = 0;
            if (k == Keys.Alt || Control.MouseButtons == MouseButtons.Right) emode = 4;
            if (emode == 0) // Rotate
            {
                float d = Math.Min(gl.Width, gl.Height) / 3;
                speedX = (xPos - xDown) / d;
                speedY = (yPos - yDown) / d;
                cam.Rotate(-speedX * 0.9, speedY * 0.9);
                gl.Invalidate();
            }
            else if (emode == 1) // Pan
            {
                speedX = (xPos - xDown) / gl.Width;
                speedY = (yPos - yDown) / gl.Height;
                Vector3 planeVec = Vector3.Subtract(new Vector3(moveStart.x,moveStart.y,moveStart.z), cam.CameraPosition);
                float dot = Vector3.Dot(planeVec, cam.ViewDirection());
                double len = (dot>0 ? planeVec.Length : -1);                
                cam.Pan(speedX * 200, speedY * 200,len);
                gl.Invalidate();
            }
            else if (emode == 2)
            {
                speedX = (xPos - xDown) / gl.Width;
                speedY = (yPos - yDown) / gl.Height;
                Vector3 planeVec = Vector3.Subtract(new Vector3(moveStart.x, moveStart.y, moveStart.z), cam.CameraPosition);
                float dot = Vector3.Dot(planeVec, cam.ViewDirection());
                double len = (dot > 0 ? planeVec.Length : -1);
                cam.Pan(-speedX, -speedY,len);
                gl.Invalidate();
            }
            else if (emode == 3)
            {
                cam.Zoom(1 - speedY / 3f);
                gl.Invalidate();
            }
            else if (emode == 4)
            {
                Geom3DVector diff = movePos.sub(moveLast);
                moveLast = movePos;
                speedX = (xPos - lastX) * 200 * zoom / gl.Width;
                speedY = (yPos - lastY) * 200 * zoom / gl.Height;
                if (view.eventObjectMoved != null)
                    view.eventObjectMoved(diff.x, diff.y);
                //               view.eventObjectMoved(speedX, -speedY);
                //eventObjectMoved((float)milliseconds * speedX * Math.Abs(speedX) / 10.0f,
                //   -(float)milliseconds * speedY * Math.Abs(speedY) / 10.0f);
                lastX = xPos;
                lastY = yPos;
                gl.Invalidate();
            }
        }

        public void SetMode(int _mode)
        {
            mode = _mode;
            toolRotate.Checked = mode == 0;
            // toolMove.Checked = mode == 1;
            toolMoveViewpoint.Checked = mode == 2;
            toolZoom.Checked = mode == 3;
            toolMoveObject.Checked = mode == 4;
        }
        private void toolRotate_Click(object sender, EventArgs e)
        {
            SetMode(0);
        }

        private void toolMove_Click(object sender, EventArgs e)
        {
            SetMode(1);
        }
        private void SetCameraDefaults()
        {
            cam.viewCenter = new Vector3(0, 0, 0);
            cam.defaultDistance = 1.6f * (float)Math.Sqrt(ps.PrintAreaDepth * ps.PrintAreaDepth + ps.PrintAreaWidth * ps.PrintAreaWidth + ps.PrintAreaHeight * ps.PrintAreaHeight);
            cam.minDistance = 0.001 * cam.defaultDistance;
        }
        public void frontView()
        {
            SetCameraDefaults();
            cam.OrientFront();
            gl.Invalidate();
        }
        public void backView()
        {
            SetCameraDefaults();
            cam.OrientBack();
            gl.Invalidate();
        }
        public void leftView()
        {
            SetCameraDefaults();
            cam.OrientLeft();
            gl.Invalidate();
        }
        public void rightView()
        {
            SetCameraDefaults();
            cam.OrientRight();
            gl.Invalidate();
        }
        public void topView()
        {
            SetCameraDefaults();
            cam.OrientTop();
            if (Main.threeDSettings.checkAutoenableParallelInTopView.Checked)
                toolParallelProjection.Checked = true;
            gl.Invalidate();
        }
        public void bottomView()
        {
            SetCameraDefaults();
            cam.OrientBottom();
            gl.Invalidate();
        }
        public void isometricView()
        {
            SetCameraDefaults();
            cam.OrientIsometric();
            gl.Invalidate();
        }
        public void FitPrinter()
        {
            cam.FitPrinter();
            gl.Invalidate();
        }
        public void FitObjects()
        {
            cam.FitObjects();
            gl.Invalidate();
        }
        private void toolResetView_Click(object sender, EventArgs e)
        {
            SetCameraDefaults();
            cam.OrientFront();
            gl.Invalidate();
        }

        private void toolMoveViewpoint_Click(object sender, EventArgs e)
        {
            SetMode(2);
        }

        private void toolZoom_Click(object sender, EventArgs e)
        {
            SetMode(3);
        }

        private void toolMoveObject_Click(object sender, EventArgs e)
        {
            SetMode(4);
        }

        private void ThreeDControl_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            /*if (e.KeyChar == Keys.Left)
            {
                view.rotZ -= 5;
                gl.Invalidate();
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                view.rotZ += 5;
                gl.Invalidate();
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Up)
            {
                view.rotX -= 5;
                gl.Invalidate();
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Down)
            {
                view.rotX += 5;
                gl.Invalidate();
                e.Handled = true;
            }*/
            if (e.KeyChar == '-')
            {
                zoom *= 1.05f;
                if (zoom > 10) zoom = 10;
                gl.Invalidate();
                e.Handled = true;
            }
            if (e.KeyChar == '+')
            {
                zoom *= 0.95f;
                if (zoom < 0.01) zoom = 0.01f;
                gl.Invalidate();
                e.Handled = true;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Application_Idle(sender, e);
            timeCall++;
            foreach (ThreeDModel m in view.models)
            {
                if (m.Changed || m.hasAnimations)
                {
                    if ((Main.threeDSettings.drawMethod == 0 && (timeCall % 9) != 0))
                        return;
                    if (m.hasAnimations && Main.threeDSettings.drawMethod != 0)
                        gl.Invalidate();
                    else if ((timeCall % 3) == 0)
                        gl.Invalidate();
                    return;
                }
            }
        }

        private void toolStripClear_Click(object sender, EventArgs e)
        {
            if (view.editor)
            {
                Main.main.objectPlacement.buttonRemoveSTL_Click(null, null);
            }
            foreach (ThreeDModel m in view.models)
            {
                m.Clear();
            }
            gl.Invalidate();
        }

        private void ThreeDControl_MouseEnter(object sender, EventArgs e)
        {
            // Focus();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            topView();
        }
        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    return true;
                case Keys.Shift | Keys.Right:
                case Keys.Shift | Keys.Left:
                case Keys.Shift | Keys.Up:
                case Keys.Shift | Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }
        public void ThreeDControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                cam.RotateDegrees( 5,0);
                gl.Invalidate();
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                cam.RotateDegrees(-5,0);
                gl.Invalidate();
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Up)
            {
                cam.RotateDegrees(0,5);
                gl.Invalidate();
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Down)
            {
                cam.RotateDegrees(0,-5);
                gl.Invalidate();
                e.Handled = true;
            }
            if (e.KeyValue == '-')
            {
                cam.PreparePanZoomRot();
                cam.Zoom(1.05);
                gl.Invalidate();
                e.Handled = true;
            }
            if (e.KeyValue == '+')
            {
                cam.PreparePanZoomRot();
                cam.Zoom(0.95);
                gl.Invalidate();
                e.Handled = true;
            }
            if (Main.main.tab.SelectedTab == Main.main.tabModel && e.Handled == false)
            {
                Main.main.objectPlacement.listSTLObjects_KeyDown(sender, e);
            }
        }

        private void toolParallelProjection_Click(object sender, EventArgs e)
        {
            toolParallelProjection.Checked = !toolParallelProjection.Checked;
            gl.Invalidate();
        }

        private void toolIsometric_Click(object sender, EventArgs e)
        {
            isometricView();
        }

        private void DrawCoordinate()
        {
            if (Main.threeDSettings.ShowCompass)
            {
                bool firstchance = false;

                if (coord == null)
                {
                    coord = new Coordinate();
                    firstchance = true;
                }

                coord.Draw(gl.Width, gl.Height, cam.phi, cam.theta);

                if (firstchance) gl.Invalidate();
            }
            else
            {
                coord = null;
            }
        }
    }
}
