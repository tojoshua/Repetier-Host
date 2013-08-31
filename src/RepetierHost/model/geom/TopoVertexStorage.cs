using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepetierHost.model.geom
{
    public class TopoVertexStorage
    {
        public const int maxVerticesPerNode = 50;
        TopoVertexStorage left = null, right = null;
        TopoVertexStorageLeaf leaf = null;
        int splitDimension = -1;
        private int count=0;
        double splitPosition;

        public bool IsLeaf
        {
            get { return leaf != null; }
        }
        public int Count
        {
            get { return count; }
        }
        public void Clear()
        {
            left = right = null;
            leaf = null;
            count = 0;
        }
        public void ChangeCoordinates(TopoVertex vertex, RHVector3 newPos)
        {
            Remove(vertex);
            vertex.pos = new RHVector3(newPos);
            Add(vertex);
        }
        public void Add(TopoVertex vertex)
        {
            Add(vertex, 0);
        }
        public void Add(TopoVertex vertex,int level)
        {
          //  if (level == 20)
            //    Console.WriteLine("Level 20");
            count++;
            if (left == null && leaf == null)
            {
                leaf = new TopoVertexStorageLeaf();
            }
            if (IsLeaf)
            {
                if (leaf.vertices.Count < maxVerticesPerNode || level == 20)
                {
                    leaf.Add(vertex);
                    return;
                }
                // Split into 2 container
                left = new TopoVertexStorage();
                right = new TopoVertexStorage();
                splitDimension = leaf.LargestDimension();
                splitPosition = 0.5 * (leaf.box.minPoint[splitDimension] + leaf.box.maxPoint[splitDimension]);
                foreach (TopoVertex moveVertex in leaf.vertices)
                {
                    if (moveVertex.pos[splitDimension] < splitPosition)
                        left.Add(moveVertex,level+1);
                    else
                        right.Add(moveVertex,level+1);
                }
                leaf = null;
            }
            if (vertex.pos[splitDimension] < splitPosition)
                left.Add(vertex,level+1);
            else
                right.Add(vertex,level+1);
        }
        public TopoVertex SearchPoint(RHVector3 vertex)
        {
            if (IsLeaf) return leaf.SearchPoint(vertex);
            if (left == null) return null;
            if (vertex[splitDimension] < splitPosition)
                return left.SearchPoint(vertex);
            else
                return right.SearchPoint(vertex);
        }
        public void Remove(TopoVertex vertex)
        {
            if (leaf == null && left == null) return;
            if (RemoveTraverse(vertex)) count--;
        }
        private bool RemoveTraverse(TopoVertex vertex)
        {
            if (IsLeaf)
            {
                if (leaf.vertices.Remove(vertex))
                    return true;
                else
                    return false; // should not happen
            }
            if (vertex.pos[splitDimension] < splitPosition)
                return left.RemoveTraverse(vertex);
            else
                return right.RemoveTraverse(vertex);
        }
        public System.Collections.IEnumerator GetEnumerator()
        {
            if (left != null)
            {
                foreach (TopoVertex v in left)
                    yield return v;
                foreach (TopoVertex v in right)
                    yield return v;
            }
            if (leaf!=null)
            {
                foreach (TopoVertex v in leaf.vertices)
                    yield return v;
            }
        }
        public HashSet<TopoVertex> SearchBox(RHBoundingBox box)
        {
            HashSet<TopoVertex> set = new HashSet<TopoVertex>();
            if(leaf!=null || left!=null)
                SearchBoxTraverse(box,set);
            return set;
        }
        private void SearchBoxTraverse(RHBoundingBox box,HashSet<TopoVertex> set) {
            if (IsLeaf)
            {
                foreach (TopoVertex v in leaf.vertices)
                {
                    if (box.ContainsPoint(v.pos))
                        set.Add(v);
                }
                return;
            }

        }
    }

    public class TopoVertexStorageLeaf
    {
        public RHBoundingBox box = new RHBoundingBox();
        public List<TopoVertex> vertices = new List<TopoVertex>();

        public void Add(TopoVertex vertex)
        {
            vertices.Add(vertex);
            box.Add(vertex.pos);
        }
        public int LargestDimension()
        {
            RHVector3 size = box.Size;
            if (size.x > size.y && size.x > size.z) return 0;
            if (size.y > size.z) return 1;
            return 2;
        }
        public TopoVertex SearchPoint(RHVector3 vertex)
        {
            foreach (TopoVertex v in vertices)
            {
                if (vertex.Distance(v.pos) < TopoModel.epsilon)
                    return v;
            }
            return null;
        }
    }
}
