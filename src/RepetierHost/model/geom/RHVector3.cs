using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace RepetierHost.model.geom
{
    public class RHVector3
    {
        public double x = 0, y = 0, z = 0;
        public RHVector3(double _x, double _y, double _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }
        public RHVector3(RHVector3 orig)
        {
            x = orig.x;
            y = orig.y;
            z = orig.z;
        }
        public RHVector3(Vector3 orig)
        {
            x = orig.X;
            y = orig.Y;
            z = orig.Z;
        }
        public RHVector3(Vector4 orig)
        {
            x = orig.X/orig.W;
            y = orig.Y / orig.W;
            z = orig.Z / orig.W;
        }
        public Vector4 asVector4()
        {
            return new Vector4((float)x, (float)y, (float)z, 1);
        }
        public Vector3 asVector3()
        {
            return new Vector3((float)x, (float)y, (float)z);
        }
        public double Length
        {
            get
            {
                return Math.Sqrt(x * x + y * y + z * z);
            }
        }
        public void NormalizeSafe()
        {
            double l = Length;
            if (l == 0)
            {
                x = y = 0;
                z = 0;
            }
            else
            {
                x /= l;
                y /= l;
                z /= l;
            }
        }
        public void StoreMinimum(RHVector3 vec)
        {
            x = Math.Min(x, vec.x);
            y = Math.Min(y, vec.y);
            z = Math.Min(z, vec.z);
        }
        public void StoreMaximum(RHVector3 vec)
        {
            x = Math.Max(x, vec.x);
            y = Math.Max(y, vec.y);
            z = Math.Max(z, vec.z);
        }
        public double Distance(RHVector3 point)
        {
            double dx = point.x - x;
            double dy = point.y - y;
            double dz = point.z - z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
        public void Scale(double factor)
        {
            x *= factor;
            y *= factor;
            z *= factor;
        }
        public double ScalarProduct(RHVector3 vector)
        {
            return x * vector.x + y * vector.y + z * vector.z;
        }
        public double AngleForNormalizedVectors(RHVector3 direction)
        {
            return Math.Acos(ScalarProduct(direction));
        }
        public double Angle(RHVector3 direction)
        {
            return Math.Acos(ScalarProduct(direction)/(Length*direction.Length));
        }
        public RHVector3 Subtract(RHVector3 vector)
        {
            return new RHVector3(x - vector.x, y - vector.y, z - vector.z);
        }
        public RHVector3 Add(RHVector3 vector)
        {
            return new RHVector3(x + vector.x, y + vector.y, z + vector.z);
        }
        public void SubtractInternal(RHVector3 vector)
        {
            x -= vector.x;
            y -= vector.y;
            z -= vector.z;
        }
        public void AddInternal(RHVector3 vector)
        {
            x += vector.x;
            y += vector.y;
            z += vector.z;
        }
        public RHVector3 CrossProduct(RHVector3 vector)
        {
            return new RHVector3(
                y*vector.z-z*vector.y,
                z*vector.x-x*vector.z,
                x*vector.y-y*vector.x);
        }
        public double this[int dimension]
        {
            get
            {
                if (dimension == 0) return x;
                else if (dimension == 1) return y;
                else return z;
            }
            set
            {
                if (dimension == 0) x = value;
                else if (dimension == 1) y = value;
                else z = value;
            }
        }
        public override string ToString()
        {
            return "(" + x.ToString() + ";" + y.ToString() + ";" + z.ToString() + ")";
        }

    }
}
