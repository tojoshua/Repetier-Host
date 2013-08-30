using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepetierHost.model
{
    public class Geom3DVector
    {
        public float x, y, z;
        public Geom3DVector(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }
        public Geom3DVector(Geom3DVector v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }
        public Geom3DVector scale(float fac)
        {
            return new Geom3DVector(x * fac, y * fac, z * fac);
        }
        public Geom3DVector add(Geom3DVector v)
        {
            return new Geom3DVector(x + v.x, y + v.y, z + v.z);
        }
        public Geom3DVector sub(Geom3DVector v)
        {
            return new Geom3DVector(x - v.x, y - v.y, z - v.z);
        }
        public float Length
        {
            get { return (float)Math.Sqrt(x * x + y * y + z * z); }
        }
        public void normalize()
        {
            float f = 1.0f / Length;
            x *= f;
            y *= f;
            z *= f;
        }
        public override string ToString()
        {
            return "("+x+"; "+y+"; "+z+")";
        }
    }
    public class Geom3DLine
    {
        public Geom3DVector point;
        public Geom3DVector dir;
        public Geom3DLine(Geom3DVector pt, Geom3DVector v, bool isDir)
        {
            point = new Geom3DVector(pt);
            if (isDir)
                dir = new Geom3DVector(v);
            else
                dir = v.sub(pt);
        }
        public override string ToString()
        {
            return "Line "+point+"->"+dir;
        }
    }
    public class Geom3DPlane
    {
        public Geom3DVector origin;
        public Geom3DVector normal;
        public Geom3DPlane(Geom3DVector o, Geom3DVector norm)
        {
            origin = new Geom3DVector(o);
            normal = new Geom3DVector(norm);
        }
        /// <summary>
        /// Inersection of plane with line
        /// </summary>
        /// <param name="line"></param>
        /// <param name="inter"></param>
        /// <returns>true if intersection exists</returns>
        public bool intersectLine(Geom3DLine line, Geom3DVector inter)
        {
            float q = normal.x * (origin.x - line.point.x) + normal.y * (origin.y - line.point.y) + normal.z * (origin.z - line.point.z);
            float d = normal.x * line.dir.x + normal.y * line.dir.y + normal.z * line.dir.z;
            if (d == 0)
            {
                inter.x = inter.y = inter.z = 0;
                return false;
            }
            float r = q / d;
            inter.x = line.point.x + r * line.dir.x;
            inter.y = line.point.y + r * line.dir.y;
            inter.z = line.point.z + r * line.dir.z;
            return true;
        }
        public override string ToString()
        {
            return "Plane "+origin+"*"+normal;
        }
    }
}
