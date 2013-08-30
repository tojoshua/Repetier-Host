using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepetierHost.model.geom
{
    public class RHBoundingBox
    {
        static double epsilon = 1e-7;
        public RHVector3 minPoint = null;
        public RHVector3 maxPoint = null;
        public void Add(RHVector3 point)
        {
            if (minPoint == null)
            {
                minPoint = new RHVector3(point);
                maxPoint = new RHVector3(point);
            }
            else
            {
                minPoint.StoreMinimum(point);
                maxPoint.StoreMaximum(point);
            }
        }
        public void Add(double x, double y, double z)
        {
            Add(new RHVector3(x, y, z));
        }
        public void Add(RHBoundingBox box)
        {
            if (box.minPoint == null) return;
            Add(box.minPoint);
            Add(box.maxPoint);
        }
        public void Clear()
        {
            minPoint = maxPoint = null;
        }
        public bool ContainsPoint(RHVector3 point)
        {
            if (minPoint == null) return false;
            return point.x >= minPoint.x && point.x <= maxPoint.x &&
                point.y >= minPoint.y && point.y <= maxPoint.y &&
                point.z >= minPoint.z && point.z <= maxPoint.z;
        }
        public bool IntersectsBox(RHBoundingBox box)
        {
            if (minPoint == null || box.minPoint == null) return false;
            bool xOverlap = Overlap(minPoint.x, maxPoint.x, box.minPoint.x, box.maxPoint.x);
            bool yOverlap = Overlap(minPoint.y, maxPoint.y, box.minPoint.y, box.maxPoint.y);
            bool zOverlap = Overlap(minPoint.z, maxPoint.z, box.minPoint.z, box.maxPoint.z);
            return xOverlap && yOverlap && zOverlap;
        }
        private bool Overlap(double p1min, double p1max, double p2min, double p2max)
        {
            if (p2min > p1max+epsilon) return false;
            if (p2max+epsilon < p1min) return false;
            return true;
        }
        public double xMin
        {
            get { return MinPoint.x; }
        }
        public double yMin
        {
            get { return MinPoint.y; }
        }
        public double zMin
        {
            get { return MinPoint.z; }
        }
        public double xMax
        {
            get { return MaxPoint.x; }
        }
        public double yMax
        {
            get { return MaxPoint.y; }
        }
        public double zMax
        {
            get { return MaxPoint.z; }
        }
        public RHVector3 MaxPoint 
        {
            get { return (maxPoint == null ? new RHVector3(0,0,0) : maxPoint); }
        }
        public RHVector3 MinPoint
        {
            get { return (minPoint == null ? new RHVector3(0, 0, 0) : minPoint); }
        }
        public RHVector3 Size
        {
            get { return MaxPoint.Subtract(MinPoint); }
        }
        public RHVector3 Center
        {
            get { 
                RHVector3 center = MaxPoint.Add(MinPoint);
                center.Scale(0.5);
                return center;
            }
        }
        /// <summary>
        /// Convert the box range into bitpattern for a fast intersection test.
        /// 
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public int RangeToBits(RHBoundingBox box)
        {
            double dx = (maxPoint.x - minPoint.x) / 10;
            double dy = (maxPoint.y - minPoint.y) / 10;
            double dz = (maxPoint.z - minPoint.z) / 10;
            int p = 0;
            int i;
            double px = minPoint.x;
            double px2 = px+dx;
            double vx = box.minPoint.x;
            double vx2 = box.maxPoint.x;
            double py = minPoint.y;
            double py2 = py + dy;
            double vy = box.minPoint.y;
            double vy2 = box.maxPoint.y;
            double pz = minPoint.z;
            double pz2 = pz + dz;
            double vz = box.minPoint.z;
            double vz2 = box.maxPoint.z;
            for (i = 0; i < 10; i++)
            {
                if (Overlap(px, px2, vx, vx2)) p |= 1 << i;
                if (Overlap(py, py2, vy, vy2)) p |= 1 << (10+i);
                if (Overlap(pz, pz2, vz, vz2)) p |= 1 << (20+i);
                px = px2;
                px2 += dx;
                py = py2;
                py2 += dy;
                pz = pz2;
                pz2 += dz;
            }
            return p;
        }
        static public bool IntersectBits(int a, int b)
        {
            int r = a & b;
            if (r == 0) return false;
            if ((r & 1023) == 0) return false;
            r >>= 10;
            if ((r & 1023) == 0) return false;
            r >>= 10;
            if ((r & 1023) == 0) return false;
            return true; ;
        }
    }
}
