using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepetierHost.model.geom
{
    public class TopoEdge
    {
        public TopoVertex v1;
        public TopoVertex v2;
        public LinkedList<TopoTriangle> faces = new LinkedList<TopoTriangle>();
        public int algHelper;

        public TopoEdge(TopoVertex _v1, TopoVertex _v2)
        {
            v1 = _v1;
            v2 = _v2;
        }
        public bool isBuildOf(TopoVertex _v1, TopoVertex _v2)
        {
            return (v1 == _v1 && v2 == _v2) || (v1 == _v2 && v2 == _v1);
        }
        public int connectedFaces
        {
            get { return faces.Count; }
        }
        public void connectFace(TopoTriangle face)
        {
            faces.AddLast(face);
        }
        public void disconnectFace(TopoTriangle face,TopoModel model)
        {
            faces.Remove(face);
            if (faces.Count == 0)
                model.edges.Remove(this);
        }
        public void MarkConnectedFacesBad()
        {
            foreach (TopoTriangle triangle in faces)
                triangle.bad = true;
        }
        public double DihedralAngle()
        {
            if (faces.Count != 2) throw new Exception("DihedralAngle requires edge with 2 faces");
            return faces.First.Value.normal.AngleForNormalizedVectors(faces.Last.Value.normal);
        }
        public bool ContainsVertex(TopoVertex v)
        {
            return v1==v || v2==v;
        }
        public List<TopoEdge> FindNeighbourEdgesWithOneFace()
        {
            List<TopoEdge> list = null;
            TopoTriangle thisFace = faces.First.Value;
            foreach (TopoTriangle face in v1.connectedFacesList)
            {
                if (face == thisFace) continue;
                for (int e = 0; e < 3; e++)
                {
                    TopoEdge testEdge = face.edges[e];
                    if(testEdge.connectedFaces!=1) continue;
                    if (testEdge.ContainsVertex(v1))
                    {
                        if (list == null) list = new List<TopoEdge>();
                        list.Add(testEdge);
                    }
                }
            }
            foreach (TopoTriangle face in v2.connectedFacesList)
            {
                if (face == thisFace) continue;
                for (int e = 0; e < 3; e++)
                {
                    TopoEdge testEdge = face.edges[e];
                    if (testEdge.connectedFaces != 1) continue;
                    if (testEdge.ContainsVertex(v2))
                    {
                        if (list == null) list = new List<TopoEdge>();
                        list.Add(testEdge);
                    }
                }
            }
            return list;
        }
        public double EdgeLength
        {
            get { return v1.pos.Subtract(v2.pos).Length; }
        }
        public TopoTriangle GetFaceExcept(TopoTriangle notThis)
        {
            foreach (TopoTriangle test in faces)
            {
                if (test != notThis)
                    return test;
            }
            return null;
        }
        /// <summary>
        /// Splits an edge and changes the connected triangles to maintain
        /// a topological correct system.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="vertex"></param>
        public void InsertVertex(TopoModel model,TopoVertex vertex) {
            LinkedList<TopoTriangle> delList = new LinkedList<TopoTriangle>();
            LinkedList<TopoTriangle> testFaces = new LinkedList<TopoTriangle>();
            foreach (TopoTriangle oldTriangle in faces)
                testFaces.AddLast(oldTriangle);
            foreach (TopoTriangle oldTriangle in testFaces)
            {
                delList.AddLast(oldTriangle);
                for (int i = 0; i < 3; i++)
                {
                    if (oldTriangle.vertices[i] != v1 && oldTriangle.vertices[i] != v2)
                    {
                        TopoTriangle newTriangle = new TopoTriangle(model, v1, vertex, oldTriangle.vertices[i]);
                        if (newTriangle.IsDegenerated())
                            newTriangle.Unlink(model);
                        else
                        {
                            // Test orientation
                            for (int e = 0; e < 3; e++)
                            {
                                TopoTriangle neigbour = newTriangle.edges[i].GetFaceExcept(newTriangle);
                                if (neigbour != null)
                                {
                                    if(!newTriangle.SameNormalOrientation(neigbour))
                                        newTriangle.FlipDirection();
                                    break;
                                }
                            }
                            model.AddTriangle(newTriangle);
                        }
                        newTriangle = new TopoTriangle(model, vertex, v2, oldTriangle.vertices[i]);
                        if (newTriangle.IsDegenerated())
                            newTriangle.Unlink(model);
                        else
                        {
                            // Test orientation
                            for (int e = 0; e < 3; e++)
                            {
                                TopoTriangle neigbour = newTriangle.edges[i].GetFaceExcept(newTriangle);
                                if (neigbour != null)
                                {
                                    if (!newTriangle.SameNormalOrientation(neigbour))
                                        newTriangle.FlipDirection();
                                    break;
                                }
                            }
                            model.AddTriangle(newTriangle);
                        }
                    }
                }
            }
            foreach (TopoTriangle tri in delList)
            {
                tri.Unlink(model);
                model.removeTriangle(tri);
            }
        }
        public TopoTriangle FirstTriangle
        {
            get
            {
                if (faces.Count == 0) return null;
                return faces.First.Value;
            }
        }
        public bool ProjectPoint(RHVector3 p, out double lambda, RHVector3 pProjected)
        {
            RHVector3 u = v2.pos.Subtract(v1.pos);
            lambda = p.Subtract(v1.pos).ScalarProduct(u)/u.ScalarProduct(u);
            pProjected.x = v1.pos.x + lambda * u.x;
            pProjected.y = v1.pos.y + lambda * u.y;
            pProjected.z = v1.pos.z + lambda * u.z;
            return lambda >= 0 && lambda <= 1;
        }
        public TopoVertex this[int pos]
        {
            get
            {
                if (pos == 0) return v1;
                else return v2;
            }
        }
        public override string ToString()
        {
            return "Edge(" + connectedFaces + ") = [" + v1.pos + "," + v2.pos + "]";
        }
    }

    public class TopoEdgePair: IComparer<TopoEdgePair>,IComparable<TopoEdgePair>
    {
        public TopoEdge edgeA, edgeB;
        public double alphaBeta; // Sum of dihedral angles to a virtual shared triangle

        public TopoEdgePair(TopoEdge _edgeA, TopoEdge _edgeB)
        {
            edgeA = _edgeA;
            edgeB = _edgeB;
            RHVector3 sharedPoint = null;
            RHVector3 p1 = null, p2 = null;
            if (edgeA.v1 == edgeB.v1)
            {
                sharedPoint = edgeA.v1.pos;
                p1 = edgeA.v2.pos;
                p2 = edgeB.v2.pos;
            }
            else if (edgeA.v1 == edgeB.v2)
            {
                sharedPoint = edgeA.v1.pos;
                p1 = edgeA.v2.pos;
                p2 = edgeB.v1.pos;
            }
            else if (edgeA.v2 == edgeB.v1)
            {
                sharedPoint = edgeA.v1.pos;
                p1 = edgeA.v1.pos;
                p2 = edgeB.v2.pos;
            }
            else if (edgeA.v2 == edgeB.v2)
            {
                sharedPoint = edgeA.v2.pos;
                p1 = edgeA.v1.pos;
                p2 = edgeB.v1.pos;
            }
            RHVector3 d1 = p1.Subtract(sharedPoint);
            RHVector3 d2 = p2.Subtract(sharedPoint);
            RHVector3 normal = d1.CrossProduct(d2);
            normal.NormalizeSafe();
            alphaBeta = normal.AngleForNormalizedVectors(edgeA.faces.First.Value.normal) + normal.AngleForNormalizedVectors(edgeB.faces.First.Value.normal);
            if (alphaBeta > Math.PI) // normal was wrong direction
                alphaBeta = 2 * Math.PI - alphaBeta;
        }
        public bool ContainsEdge(TopoEdge edge)
        {
            return edgeA == edge || edgeB == edge;
        }
        public bool ContainsEdgePair(TopoEdge a, TopoEdge b)
        {
            return ContainsEdge(a) && ContainsEdge(b);
        }
        public static bool ContainsListPair(List<TopoEdgePair> list, TopoEdge a, TopoEdge b)
        {
            foreach (TopoEdgePair pair in list)
            {
                if (pair.ContainsEdgePair(a, b)) return true;
            }
            return false;
        }
        public TopoEdge CommonThirdEdge()
        {
            TopoVertex v1 = edgeA.v1;
            if (edgeB.v1 == v1 || edgeB.v2 == v1)
                v1 = edgeA.v2;
            TopoVertex v2 = edgeB.v1;
            if (edgeA.v1 == v2 || edgeA.v2 == v2)
                v2 = edgeB.v2;
            foreach (TopoTriangle t in v1.connectedFacesList)
            {
                TopoEdge e = t.EdgeWithVertices(v1, v2);
                if (e != null)
                    return e;
            }
            return null;
        }
        public TopoTriangle BuildTriangle(TopoModel model)
        {
            TopoVertex sharedPoint = null;
            TopoVertex p1 = null, p2 = null;
            if (edgeA.v1 == edgeB.v1)
            {
                sharedPoint = edgeA.v1;
                p1 = edgeA.v2;
                p2 = edgeB.v2;
            }
            else if (edgeA.v1 == edgeB.v2)
            {
                sharedPoint = edgeA.v1;
                p1 = edgeA.v2;
                p2 = edgeB.v1;
            }
            else if (edgeA.v2 == edgeB.v1)
            {
                sharedPoint = edgeA.v1;
                p1 = edgeA.v1;
                p2 = edgeB.v2;
            }
            else if (edgeA.v2 == edgeB.v2)
            {
                sharedPoint = edgeA.v2;
                p1 = edgeA.v1;
                p2 = edgeB.v1;
            }
            TopoTriangle faceA = edgeA.faces.First.Value;
            TopoTriangle newTriangle = new TopoTriangle(model, sharedPoint, p1, p2, 0, 0, 1);
            if (newTriangle.SameNormalOrientation(faceA) == false)
                newTriangle.FlipDirection();
            newTriangle.RecomputeNormal();
            model.AddTriangle(newTriangle);
            return newTriangle;
        }
        public int Compare(TopoEdgePair pair1, TopoEdgePair pair2)
        {
            int returnValue = 1;
            if (pair1 != null && pair2 == null)
            {
                returnValue = 0;
            }
            else if (pair1 == null && pair2 != null)
            {
                returnValue = 0;
            }
            else if (pair1 != null && pair2 != null)
            {
                returnValue = pair1.alphaBeta.CompareTo(pair2.alphaBeta);
            }
            return returnValue;
        }
        public bool Valid()
        {
            return edgeA.connectedFaces == 1 && edgeB.connectedFaces == 1;
        }
        public int CompareTo(TopoEdgePair pair)
        {
            return Compare(this, pair);
        }
        public override string ToString()
        {
            return "EdgePair:" + edgeA + " - " + edgeB+" ab = "+alphaBeta;
        }
    }
}
