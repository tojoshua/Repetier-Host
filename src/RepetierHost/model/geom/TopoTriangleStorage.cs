using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepetierHost.model.geom
{
    public class TopoTriangleStorage
    {
        public HashSet<TopoTriangle> triangles = new HashSet<TopoTriangle>();
        public TopoTriangleNode tree = null;
        public void Add(TopoTriangle triangle) {
            triangles.Add(triangle);
            if (tree != null)
                tree.AddTriangle(triangle);
        }
        public bool Remove(TopoTriangle triangle)
        {
            if (tree != null)
                tree.RemoveTriangle(triangle);
            return triangles.Remove(triangle);
        }
        public System.Collections.IEnumerator GetEnumerator()
        {
            foreach (TopoTriangle t in triangles)
               yield return t;
        }
        public void Clear()
        {
            tree = null;
            triangles.Clear();
        }
        public bool Contains(TopoTriangle test)
        {
            return triangles.Contains(test);
        }
        public int Count
        {
            get { return triangles.Count; }
        }
        public void PrepareFastSearch()
        {
            tree = new TopoTriangleNode(null);
            foreach (TopoTriangle triangle in triangles)
            {
                tree.AddTriangle(triangle);
            }
        }
        public HashSet<TopoTriangle> FindIntersectionCandidates(TopoTriangle triangle)
        {
            if (tree == null) PrepareFastSearch();
            HashSet<TopoTriangle> result = new HashSet<TopoTriangle>();
            tree.FindIntersectionCandidates(triangle, result);
            return result;
        }
    }
    public class TopoTriangleNode
    {
        int dimension = -1;
        double middlePosition;
        int nextTrySplit = 50;
        TopoTriangleNode parent = null;
        TopoTriangleNode left = null;
        TopoTriangleNode middle = null;
        TopoTriangleNode right = null;
        RHBoundingBox box = new RHBoundingBox();
        private HashSet<TopoTriangle> triangles = null;

        public TopoTriangleNode(TopoTriangleNode _parent)
        {
            parent = _parent;
        }
        public void AddTriangle(TopoTriangle triangle) {
            if(left == null && triangles == null) {
                triangles = new HashSet<TopoTriangle>();
            }
            if (triangles != null)
            {
                triangles.Add(triangle);
                box.Add(triangle.boundingBox);
                if (triangles.Count > nextTrySplit) TrySplit();
            }
            else
            {
                if (triangle.boundingBox.maxPoint[dimension] < middlePosition)
                    left.AddTriangle(triangle);
                else if (triangle.boundingBox.minPoint[dimension] > middlePosition)
                    right.AddTriangle(triangle);
                else
                    middle.AddTriangle(triangle);
            }
        }
        public bool RemoveTriangle(TopoTriangle triangle)
        {
            TopoTriangleNode node = FindNodeForTriangle(triangle);
            if (node != null)
                return node.triangles.Remove(triangle);
            return false;
        }
        public TopoTriangleNode FindNodeForTriangle(TopoTriangle triangle)
        {
            if (triangles != null) return this;
            if (triangle.boundingBox.maxPoint[dimension] < middlePosition)
                return left.FindNodeForTriangle(triangle);
            else if (triangle.boundingBox.minPoint[dimension] > middlePosition)
                return right.FindNodeForTriangle(triangle);
            else
                return middle.FindNodeForTriangle(triangle);
        }
        public void FindIntersectionCandidates(TopoTriangle triangle,HashSet<TopoTriangle> resultList)
        {
            if (triangles != null) // end leaf, test all boxes for intersection
            {
                foreach (TopoTriangle test in triangles)
                {
                    if(test!=triangle && test.boundingBox.IntersectsBox(triangle.boundingBox))
                        resultList.Add(test);
                }
                return;
            }
            if (left == null) return;
            if(triangle.boundingBox.minPoint[dimension]<middlePosition)
                left.FindIntersectionCandidates(triangle,resultList);
            if(triangle.boundingBox.maxPoint[dimension]>middlePosition)
                right.FindIntersectionCandidates(triangle,resultList);
            if(triangle.boundingBox.minPoint[dimension]<middlePosition && triangle.boundingBox.maxPoint[dimension]>middlePosition)
                middle.FindIntersectionCandidates(triangle,resultList);
        }
        private void TrySplit()
        {
            int newDim = 0;
            int parentDimension = -1;
            if (parent != null)
                parentDimension = parent.dimension;
            RHVector3 size = box.Size;
            if (parentDimension != 0) newDim = 0;
            if (parentDimension != 1 && size.x < size.y) newDim = 1;
            if (parentDimension != 2 && size.z > size[newDim]) newDim = 2;
            int loop = 0;
            int maxLoop = 4;
            double bestCenter = 0;
            double bestPercentage = 3000;
            int bestDim = 0;
            for (int dim = 0; dim < 3; dim++)
            {
                if (dim == parentDimension) continue;
                for (loop = 0; loop < maxLoop; loop++)
                {
                    int count = 0;
                    double testDist = box.minPoint[newDim] + size[newDim] * (1 + loop) / (maxLoop + 1);
                    foreach (TopoTriangle tri in triangles)
                    {
                        if (tri.boundingBox.maxPoint[newDim] < testDist)
                            count++;
                    }
                    double percent = 100.0 * (double)count / triangles.Count;
                    if (Math.Abs(50 - percent) < bestPercentage)
                    {
                        bestPercentage = percent;
                        bestCenter = testDist;
                        bestDim = dim;
                    }
                }
            }
            if (bestPercentage < 5)
            {
                nextTrySplit = (nextTrySplit * 3) / 2;
                return; // not effective enough
            }
            left = new TopoTriangleNode(this);
            right = new TopoTriangleNode(this);
            middle = new TopoTriangleNode(this);
            dimension = newDim;
            middlePosition = bestCenter;
            foreach (TopoTriangle tri in triangles)
            {
                if (tri.boundingBox.maxPoint[dimension] < middlePosition)
                    left.AddTriangle(tri);
                else if (tri.boundingBox.minPoint[dimension] > middlePosition)
                    right.AddTriangle(tri);
                else
                    middle.AddTriangle(tri);
            }
            triangles = null;
        }
    }
}
