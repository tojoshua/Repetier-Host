using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace RepetierHost.model.geom
{
    public class TopoVertex
    {
        public RHVector3 pos;
        public int id;
        public LinkedList<TopoTriangle> connectedFacesList = new LinkedList<TopoTriangle>();
        public TopoVertex(int _id, RHVector3 _pos)
        {
            id = _id;
            pos = new RHVector3(_pos);
        }
        public TopoVertex(int _id,RHVector3 _pos,Matrix4 trans)
        {
            id = _id;
            pos = new RHVector3(
                _pos.x*trans.Column0.X+_pos.y*trans.Column0.Y+_pos.z*trans.Column0.Z+trans.Column0.W,
                _pos.x*trans.Column1.X+_pos.y*trans.Column1.Y+_pos.z*trans.Column1.Z+trans.Column1.W,
                _pos.x*trans.Column2.X+_pos.y*trans.Column2.Y+_pos.z*trans.Column2.Z+trans.Column2.W
            );
        } 
        public int connectedFaces
        {
            get { return connectedFacesList.Count; }
        }

        public void connectFace(TopoTriangle triangle)
        {
            connectedFacesList.AddLast(triangle);
        }
        public void disconnectFace(TopoTriangle triangle)
        {
            connectedFacesList.Remove(triangle);
        }
        public double distance(TopoVertex vertex)
        {
            return pos.Distance(vertex.pos);
        }
        public double distance(RHVector3 vertex)
        {
            return pos.Distance(vertex);
        }
        public void LaplaceRelaxation(TopoModel model)
        {
            double n = 2;
            RHVector3 newPos = new RHVector3(pos);
            newPos.AddInternal(pos);
            foreach (TopoTriangle t in connectedFacesList)
            {
                int idx = t.VertexIndexFor(this);
                n += 2;
                newPos.AddInternal(t.vertices[(idx + 1) % 3].pos);
                newPos.AddInternal(t.vertices[(idx + 2) % 3].pos);
            }
            newPos.Scale(1.0 / n);
            // validate newPos does not create intersecting triangles or bad shapes
            foreach (TopoTriangle t in connectedFacesList)
            {
                int idx = t.VertexIndexFor(this);
                RHVector3 d1 = t.vertices[(idx+1)%3].pos.Subtract(newPos);
                RHVector3 d2 = t.vertices[(idx+2)%3].pos.Subtract(t.vertices[(idx+1)%3].pos);
                RHVector3 normal = d1.CrossProduct(d2);
                if (normal.ScalarProduct(t.normal) < 0)
                    return;
                double angle = t.AngleEdgePoint((idx + 1) % 3, newPos);
                if(angle < 0.088 || angle > 2.96)
                    return; // Angle gets to small
            }
            model.vertices.ChangeCoordinates(this, newPos);
        }
        public override string ToString()
        {
            return "("+connectedFacesList.Count+")"+pos.ToString();
        }
    }
}
