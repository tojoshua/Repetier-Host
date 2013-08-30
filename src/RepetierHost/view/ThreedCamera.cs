using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;
using OpenTK;
using RepetierHost.model;
using RepetierHost.model.geom;

namespace RepetierHost.view
{
    public class ThreeDCamera
    {
        public Vector3 viewCenter = new Vector3(0, 0, 0);
        public Vector3 defaultCenter = new Vector3(0, 0, 0);
        public double distance, minDistance = 10, defaultDistance = 200;
        public double theta = 0;
        public double phi = 0;
        public double angle = 15.0 * Math.PI / 180;
        public Vector3 viewCenterStart = new Vector3();
        public double startTheta, startPhi,startDistance;
        FormPrinterSettings ps = Main.printerSettings;
        ThreeDControl control;
        public ThreeDCamera(ThreeDControl ctl)
        {
            control = ctl;
        }
        public Vector3 CameraPosition
        {
            get
            {
                Vector3 cam = new Vector3();
                cam.X = viewCenter.X + (float)(distance * Math.Cos(theta) * Math.Sin(phi));
                cam.Y = viewCenter.Y + (float)(distance * Math.Sin(theta) * Math.Sin(phi));
                cam.Z = viewCenter.Z + (float)(distance * Math.Cos(phi));
                return cam;
            }
        }
        public Vector3 EdgeTranslation()
        {
            double dist = 0.06;
            Vector3 trans = new Vector3();
            trans.X = (float)(dist * Math.Cos(theta) * Math.Sin(phi));
            trans.Y = (float)(dist * Math.Sin(theta) * Math.Sin(phi));
            trans.Z = (float)(dist * Math.Cos(phi));
            return trans;
        }
        public Vector3 ViewDirection()
        {
            Vector3 direction = new Vector3();
            direction.X = (float)(-Math.Cos(theta) * Math.Sin(phi));
            direction.Y = (float)(-Math.Sin(theta) * Math.Sin(phi));
            direction.Z = (float)(-Math.Cos(phi));
            return direction;
        }
        public void OrientFront()
        {
            viewCenter.X = defaultCenter.X;
            viewCenter.Y = defaultCenter.Y;
            viewCenter.Z = defaultCenter.Z;
            theta = -Math.PI/2;
            phi = Math.PI / 2;
            distance = defaultDistance;
            FitPrinter();
        }
        public void OrientBack()
        {
            viewCenter.X = defaultCenter.X;
            viewCenter.Y = defaultCenter.Y;
            viewCenter.Z = defaultCenter.Z;
            theta = Math.PI / 2;
            phi = Math.PI / 2;
            distance = defaultDistance;
            FitPrinter();
        }
        public void OrientLeft()
        {
            viewCenter.X = defaultCenter.X;
            viewCenter.Y = defaultCenter.Y;
            viewCenter.Z = defaultCenter.Z;
            theta = Math.PI;
            phi = Math.PI / 2;
            distance = defaultDistance;
            FitPrinter();
        }
        public void OrientRight()
        {
            viewCenter.X = defaultCenter.X;
            viewCenter.Y = defaultCenter.Y;
            viewCenter.Z = defaultCenter.Z;
            theta = 0;
            phi = Math.PI / 2;
            distance = defaultDistance;
            FitPrinter();
        }
        public void OrientTop()
        {
            viewCenter.X = defaultCenter.X;
            viewCenter.Y = defaultCenter.Y;
            viewCenter.Z = defaultCenter.Z;
            theta = -Math.PI / 2;
            phi = 1e-5;
            distance = defaultDistance;
            FitPrinter();
        }
        public void OrientBottom()
        {
            viewCenter.X = defaultCenter.X;
            viewCenter.Y = defaultCenter.Y;
            viewCenter.Z = defaultCenter.Z;
            theta = -Math.PI / 2;
            phi = Math.PI -1e-5;
            distance = defaultDistance;
            FitPrinter();
        }
        public void OrientIsometric()
        {
            viewCenter.X = defaultCenter.X;
            viewCenter.Y = defaultCenter.Y;
            viewCenter.Z = defaultCenter.Z;
            theta = Math.PI * 1.25;
            phi = Math.PI / 4;
            distance = defaultDistance;
            FitPrinter();
        }
        public void PreparePanZoomRot()
        {
            viewCenterStart.X = viewCenter.X;
            viewCenterStart.Y = viewCenter.Y;
            viewCenterStart.Z = viewCenter.Z;
            startDistance = distance;
            startPhi = phi;
            startTheta = theta;
        }
        public void Zoom(double factor)
        {
            distance = startDistance * factor;
            if (distance < minDistance)
                distance = minDistance;
            if (distance > 6 * defaultDistance)
                distance = 6 * defaultDistance;
            if (distance > 1)
            {
                angle = 15.0 * Math.PI / 180;
            }
            else
            {
                angle = Math.Atan(distance * Math.Tan(15.0 * Math.PI / 180.0));
            }
        }
        public void Rotate(double side, double updown)
        {
            theta = startTheta + side;
            phi = startPhi - updown;
            while (theta > Math.PI)
                theta -= 2 * Math.PI;
            while (theta < -Math.PI)
                theta += 2 * Math.PI;
            while (phi > Math.PI)
                phi = Math.PI-1e-5;
            while (phi < 0)
                phi = 1e-5;
            //Console.WriteLine("Phi:" + phi);
            //if (Math.Abs(phi) < 1e-5) phi = 1e-5;

        }
        public void RotateDegrees(double rotX, double rotZ)
        {
            theta += rotX*Math.PI/180.0;
            phi += rotZ * Math.PI / 180.0;
            while (theta > Math.PI)
                theta -= 2 * Math.PI;
            while (theta < -Math.PI)
                theta += 2 * Math.PI;
            while (phi > Math.PI)
                phi = Math.PI - 1e-5;
            while (phi < 0)
                phi = 1e-5;
            //Console.WriteLine("Phi:" + phi);
            //if (Math.Abs(phi) < 1e-5) phi = 1e-5;

        }
        public void Pan(double leftRight, double upDown,double dist)
        {
            if (dist < 0) dist = distance;
            leftRight *= Math.Max(1,dist) * Math.Tan(angle)*2.0;
            upDown *= -Math.Max(1, dist) * Math.Tan(angle) * 2.0;
            Vector3 ud = new Vector3(0, 0, 1);
            Vector3 camCenter = new Vector3();
            Vector3 cp = CameraPosition;
            Vector3.Subtract(ref viewCenter, ref cp, out camCenter);
            Vector3 lr = new Vector3();
            Vector3.Cross(ref camCenter,ref ud, out lr);
            Vector3.Cross(ref lr, ref camCenter, out ud);
            lr.Normalize();
            ud.Normalize();
            viewCenter.X = (float)(viewCenterStart.X + leftRight * lr.X + upDown * ud.X);
            viewCenter.Y = (float)(viewCenterStart.Y + leftRight * lr.Y + upDown * ud.Y);
            viewCenter.Z = (float)(viewCenterStart.Z + leftRight * lr.Z + upDown * ud.Z);
        }
        public RHBoundingBox PrinterBoundingBox()
        {
            RHBoundingBox b = new RHBoundingBox();
            b.Add(ps.BedLeft, ps.BedFront, -0.0 * ps.PrintAreaHeight);
            b.Add(ps.BedLeft + ps.PrintAreaWidth, ps.BedFront + ps.PrintAreaDepth, 1.0 * ps.PrintAreaHeight);
            return b;
        }
        public RHBoundingBox ObjectsBoundingBox()
        {
            RHBoundingBox b = new RHBoundingBox();
            foreach (PrintModel model in Main.main.objectPlacement.ListObjects(false))
            {
                b.Add(model.bbox.minPoint);
                b.Add(model.bbox.maxPoint);
            }
            if (b.minPoint == null) return PrinterBoundingBox();
            return b;
        }
        public void FitPrinter()
        {
            FitBoundingBox(PrinterBoundingBox());
        }
        public void FitObjects()
        {
            FitBoundingBox(ObjectsBoundingBox());
        }

        public void FitBoundingBox(RHBoundingBox box)
        {
            float bedRadius = (float)(1.5 * Math.Sqrt((ps.PrintAreaDepth * ps.PrintAreaDepth + ps.PrintAreaHeight * ps.PrintAreaHeight + ps.PrintAreaWidth * ps.PrintAreaWidth) * 0.25));
            RHVector3 shift = new RHVector3(-ps.BedLeft - 0.5 * ps.PrintAreaWidth, -ps.BedFront - 0.5 * ps.PrintAreaDepth, -0.5 * ps.PrintAreaHeight);
            viewCenter = box.Center.asVector3();
            distance = defaultDistance;
            int loops = 5;
            while (loops > 0)
            {
                loops--;
                angle = 15.0 * Math.PI / 180;
                double ratio = (double)control.gl.Width / (double)control.gl.Height;
                Vector3 camPos = CameraPosition;
                Matrix4 lookAt = Matrix4.LookAt(camPos.X, camPos.Y, camPos.Z, viewCenter.X, viewCenter.Y, viewCenter.Z, 0, 0, 1.0f);
                Matrix4 persp;
                Vector3 dir = new Vector3();
                Vector3.Subtract(ref viewCenter, ref camPos, out dir);
                dir.Normalize();
                float dist;
                Vector3.Dot(ref dir, ref camPos, out dist);
                dist = -dist;

                float nearDist = Math.Max(1, dist - bedRadius);
                float farDist = Math.Max(bedRadius * 2, dist + bedRadius);
                float nearHeight = 2.0f * (float)Math.Tan(angle) * dist;

                if (control.toolParallelProjection.Checked)
                {
                    persp = Matrix4.CreateOrthographic(nearHeight * (float)ratio, nearHeight, nearDist, farDist);
                    loops = 0;
                }
                else
                {
                    persp = Matrix4.CreatePerspectiveFieldOfView((float)(angle * 2.0), (float)ratio, nearDist, farDist);
                }
                Matrix4 trans = Matrix4.Mult(lookAt, persp);
                RHVector3 min = new RHVector3(0, 0, 0);
                RHVector3 max = new RHVector3(0, 0, 0);
                Vector4 pos;
                RHBoundingBox bb = new RHBoundingBox();
                pos = Vector4.Transform(box.minPoint.asVector4(), trans);
                bb.Add(new RHVector3(pos));
                pos = Vector4.Transform(box.maxPoint.asVector4(), trans);
                bb.Add(new RHVector3(pos));
                Vector4 pnt = new Vector4((float)box.xMin, (float)box.yMax, (float)box.zMin, 1);
                pos = Vector4.Transform(pnt, trans);
                bb.Add(new RHVector3(pos));
                pnt = new Vector4((float)box.xMin, (float)box.yMax, (float)box.zMin, 1);
                pos = Vector4.Transform(pnt, trans);
                bb.Add(new RHVector3(pos));
                pnt = new Vector4((float)box.xMax, (float)box.yMax, (float)box.zMin, 1);
                pos = Vector4.Transform(pnt, trans);
                bb.Add(new RHVector3(pos));
                pnt = new Vector4((float)box.xMin, (float)box.yMax, (float)box.zMax, 1);
                pos = Vector4.Transform(pnt, trans);
                bb.Add(new RHVector3(pos));
                pnt = new Vector4((float)box.xMin, (float)box.yMin, (float)box.zMax, 1);
                pos = Vector4.Transform(pnt, trans);
                bb.Add(new RHVector3(pos));
                pnt = new Vector4((float)box.xMax, (float)box.yMin, (float)box.zMax, 1);
                pos = Vector4.Transform(pnt, trans);
                bb.Add(new RHVector3(pos));
                double fac = Math.Max(Math.Abs(bb.xMin), Math.Abs(bb.xMax));
                fac = Math.Max(fac, Math.Abs(bb.yMin));
                fac = Math.Max(fac, Math.Abs(bb.yMax));
                distance *= fac * 1.03;
                if (distance < 1) angle = Math.Atan(distance * Math.Tan(15.0 * Math.PI / 180.0));
            }
        }
    }
}
