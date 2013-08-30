using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepetierHost.model.geom
{
    public class RHMatrix3
    {
        public double xx, xy, xz, yx, yy, yz, zx, zy, zz;
        public void SetXColumn(double x, double y, double z)
        {
            xx = x;
            yx = y;
            zx = z;
        }
        public void SetYColumn(double x, double y, double z)
        {
            xy = x;
            yy = y;
            zy = z;
        }
        public void SetZColumn(double x, double y, double z)
        {
            xz = x;
            yz = y;
            zz = z;
        }
        public void SetXRow(double x, double y, double z)
        {
            xx = x;
            xy = y;
            xz = z;
        }
        public void SetYRow(double x, double y, double z)
        {
            yx = x;
            yy = y;
            yz = z;
        }
        public void SetZRow(double x, double y, double z)
        {
            zx = x;
            zy = y;
            zz = z;
        }
        public void SetXColumn(RHVector3 v)
        {
            xx = v.x;
            yx = v.y;
            zx = v.z;
        }
        public void SetYColumn(RHVector3 v)
        {
            xy = v.x;
            yy = v.y;
            zy = v.z;
        }
        public void SetZColumn(RHVector3 v)
        {
            xz = v.x;
            yz = v.y;
            zz = v.z;
        }
        public double Determinant
        {
            get { return xx * yy * zz + xy * yz * zx + xz * yx * zy - xx * yz * zy - xy * yx * zz - xz * yy * zx; }
        }
    }
}
