using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepetierHost.model.geom
{
    public class TopoTriangle
    {
        public static bool debugIntersections = false;
        public static double epsilonZero = 1e-7;
        public static double epsilonZeroMinus = -1e-7;
        public static double epsilonOneMinus = 1 - 1e-7;
        public static double epsilonOnePlus = 1 + 1e-7;
        public TopoVertex[] vertices = new TopoVertex[3];
        public TopoEdge[] edges = new TopoEdge[3];
        public RHBoundingBox boundingBox = new RHBoundingBox();
        public RHVector3 normal;
        public bool bad = false;
        public bool hasIntersections = false;
        public int algHelper;
        public int shell = -1;
        public TopoTriangle(TopoModel model,TopoVertex v1, TopoVertex v2, TopoVertex v3, double nx, double ny, double nz)
        {
            vertices[0] = v1;
            vertices[1] = v2;
            vertices[2] = v3;
            RHVector3 normalTest = new RHVector3(nx, ny, nz);
            //normal.NormalizeSafe();
            edges[0] = model.getOrCreateEdgeBetween(v1, v2);
            edges[1] = model.getOrCreateEdgeBetween(v2, v3);
            edges[2] = model.getOrCreateEdgeBetween(v3, v1);
            edges[0].connectFace(this);
            edges[1].connectFace(this);
            edges[2].connectFace(this);
            v1.connectFace(this);
            v2.connectFace(this);
            v3.connectFace(this);
            boundingBox.Add(v1.pos);
            boundingBox.Add(v2.pos);
            boundingBox.Add(v3.pos);
            RecomputeNormal();
            if (normalTest.ScalarProduct(normal) < 0)
                FlipDirection();
           /* double d1 = edges[0].EdgeLength;
            double d2 = edges[1].EdgeLength;
            double d3 = edges[2].EdgeLength;
            if (d1 < epsilonZero || d2 < epsilonZero || d3 < epsilonZero)
                Console.WriteLine("Df:" + this);*/
        }
        public TopoTriangle(TopoModel model, TopoVertex v1, TopoVertex v2, TopoVertex v3)
        {
            vertices[0] = v1;
            vertices[1] = v2;
            vertices[2] = v3;
            RecomputeNormal();
            edges[0] = model.getOrCreateEdgeBetween(v1, v2);
            edges[1] = model.getOrCreateEdgeBetween(v2, v3);
            edges[2] = model.getOrCreateEdgeBetween(v3, v1);
            edges[0].connectFace(this);
            edges[1].connectFace(this);
            edges[2].connectFace(this);
            v1.connectFace(this);
            v2.connectFace(this);
            v3.connectFace(this);
            boundingBox.Add(v1.pos);
            boundingBox.Add(v2.pos);
            boundingBox.Add(v3.pos);
            /*double d1 = edges[0].EdgeLength;
            double d2 = edges[1].EdgeLength;
            double d3 = edges[2].EdgeLength;
            if (d1 < epsilonZero || d2 < epsilonZero || d3 < epsilonZero)
                Console.WriteLine("Df:" + this);*/
        }
        public void Unlink(TopoModel model)
        {
            edges[0].disconnectFace(this,model);
            edges[1].disconnectFace(this,model);
            edges[2].disconnectFace(this,model);
            vertices[0].disconnectFace(this);
            vertices[1].disconnectFace(this);
            vertices[2].disconnectFace(this);
        }
        public void FlipDirection()
        {
            normal.Scale(-1);
            TopoVertex v = vertices[0];
            vertices[0] = vertices[1];
            vertices[1] = v;
            TopoEdge e = edges[1];
            edges[1] = edges[2];
            edges[2] = e;
        }

        public void RecomputeNormal()
        {
            RHVector3 d1 = vertices[1].pos.Subtract(vertices[0].pos);
            RHVector3 d2 = vertices[2].pos.Subtract(vertices[1].pos);
            normal = d1.CrossProduct(d2);
            normal.NormalizeSafe();
        }
        public int VertexIndexFor(TopoVertex test)
        {
            if (test == vertices[0]) return 0;
            if (test == vertices[1]) return 1;
            if (test == vertices[2]) return 2;
            return -1;
        }
        public double SignedVolume()
        {
            return vertices[0].pos.ScalarProduct(vertices[1].pos.CrossProduct(vertices[2].pos)) / 6.0;
        }

        public double Area()
        {
            RHVector3 d1 = vertices[1].pos.Subtract(vertices[0].pos);
            RHVector3 d2 = vertices[2].pos.Subtract(vertices[1].pos);
            return 0.5* d1.CrossProduct(d2).Length;
        }

        public TopoEdge EdgeWithVertices(TopoVertex v1, TopoVertex v2)
        {
            foreach (TopoEdge e in edges)
            {
                if ((e.v1 == v1 && e.v2 == v2) || (e.v2 == v1 && e.v1 == v2))
                    return e;
            }
            return null;
        }
        public bool IsDegenerated()
        {
            if (vertices[0] == vertices[1] || vertices[1] == vertices[2] || vertices[2] == vertices[0]) 
                return true;
            return false;
        }
        public bool IsIsolated()
        {
            return vertices[0].connectedFaces == 1 && vertices[1].connectedFaces == 1 && vertices[2].connectedFaces == 1;
        }
        /// <summary>
        /// Checks if all vertices are colinear preventing a normal computation. If point are coliniear the center vertex is
        /// moved in the direction of the edge to allow normal computations.
        /// </summary>
        /// <returns></returns>
        public bool CheckIfColinear()
        {
            RHVector3 d1 = vertices[1].pos.Subtract(vertices[0].pos);
            RHVector3 d2 = vertices[2].pos.Subtract(vertices[1].pos);
            double angle = d1.Angle(d2);
            if (angle > 0.001 && angle<Math.PI-0.001) return false;
            return true;
        }
        public void FixColinear(TopoModel model) {
            RHVector3 center = vertices[0].pos.Add(vertices[1].pos).Add(vertices[2].pos);
            center.Scale(1 / 3.0);
            int best = -1;
            double bestdist = 1e30;
            for (int i = 0; i < 3; i++)
            {
                if (vertices[i].connectedFaces == 1) continue;
                double dist = center.Subtract(vertices[i].pos).Length;
                if (dist < bestdist)
                {
                    bestdist = dist;
                    best = i;
                }
            }
            if(best==-1) throw new Exception("CheckIfColinearAndFix called on isolated triangle");
            edges[(best + 1) % 3].InsertVertex(model,vertices[best]);
            /*
            // Find an other face sharing vertex
            TopoTriangle otherFace = null;
            TopoVertex moveVertex = vertices[best];
            foreach (TopoTriangle triangle in moveVertex.connectedFacesList)
            {
                if (triangle != this)
                {
                    otherFace = triangle;
                    break;
                }
            }
            // Now find the not shared vertex
            TopoVertex oppositeVertex = null;
            for (int i = 0; i < 3; i++)
            {
                bool notSame = true;
                for (int j = 0; j < 3; j++)
                {
                    if (otherFace.vertices[i] == vertices[j])
                    {
                        notSame = false;
                        break;
                    }
                }
                if (notSame)
                {
                    oppositeVertex = otherFace.vertices[i];
                }
            }
            RHVector3 line = moveVertex.pos.Subtract(oppositeVertex.pos);
            double lineLength = line.Length;
            double moveFactor = 0.01;
            if (0.99 * lineLength > 0.01) moveFactor = 0.01 / lineLength;
            line.Scale(moveFactor);
            moveVertex.pos = moveVertex.pos.Add(line);
            RecomputeNormal();*/
        }
        public int NumberOfSharedVertices(TopoTriangle tri)
        {
            int sameVertices = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (vertices[i] == tri.vertices[j])
                    {
                        sameVertices++;
                        break;
                    }
                }
            }
            return sameVertices;
        }
        public bool SameNormalOrientation(TopoTriangle test)
        {
            for (int i = 0; i < 3; i++)
            {
                for(int j =0;j<3;j++) {
                    if (vertices[i] == test.vertices[j] && vertices[(i + 1) % 3] == test.vertices[(j + 2) % 3])
                        return true;
                }
            }
            return false;
        }
        public RHVector3 Center
        {
            get
            {
                RHVector3 c = vertices[0].pos.Add(vertices[1].pos).Add(vertices[2].pos);
                c.Scale(1.0 / 3.0);
                return c;
            }
        }
        public bool IntersectsLine(RHVector3 lineStart, RHVector3 lineDirection,out double delta)
        {
            RHVector3 A = vertices[0].pos;
            RHVector3 a = vertices[1].pos.Subtract(A);
            RHVector3 b = vertices[2].pos.Subtract(A);
            double det = ((a.x*b.y-a.y*b.x)*lineDirection.z+(a.z*b.x-a.x*b.z)*lineDirection.y+(a.y*b.z-a.z*b.y)*lineDirection.x);
            delta = 0;
            if (det == 0) return false; // Coplanar doe snot count as intersection here.
            double alpha = ((b.x * lineDirection.y - b.y * lineDirection.x) * lineStart.z + 
                (b.z * lineDirection.x - b.x * lineDirection.z) * lineStart.y + 
                (b.y * lineDirection.z - b.z * lineDirection.y) * lineStart.x + 
                (A.y * b.x - A.x * b.y) * lineDirection.z + (A.x * b.z - A.z * b.x) * lineDirection.y + 
                (A.z * b.y - A.y * b.z) * lineDirection.x) / det;
            double beta = -((a.x * lineDirection.y - a.y * lineDirection.x) * lineStart.z + 
                (a.z * lineDirection.x - a.x * lineDirection.z) * lineStart.y + 
                (a.y * lineDirection.z - a.z * lineDirection.y) * lineStart.x + 
                (a.x * A.y - a.y * A.x) * lineDirection.z + (a.z * A.x - a.x * A.z) * lineDirection.y + 
                (a.y * A.z - a.z * A.y) * lineDirection.x) / det;
            if( alpha >= 0 && beta >= 0 && alpha + beta < 1) {
                delta = -((a.x * b.y - a.y * b.x) * lineStart.z + (a.z * b.x - a.x * b.z) * lineStart.y + 
                    (a.y * b.z - a.z * b.y) * lineStart.x + (a.x * A.y - a.y * A.x) * b.z + 
                    (a.z * A.x - a.x * A.z) * b.y + (a.y * A.z - a.z * A.y) * b.x) / det;
                return true;
            }
            return false;
        }
        public double DistanceToPlane(RHVector3 pos)
        {
            double d = vertices[0].pos.ScalarProduct(normal);
            return pos.ScalarProduct(normal)-d;
        }
        private bool InPlaneIntersectLine(int idx1,int idx2, RHVector3 a1, RHVector3 a2, RHVector3 b1, RHVector3 b2)
        {
            double ax = a2[idx1] - a1[idx1];
            double ay = a2[idx2]-a1[idx2];
            double bx = b2[idx1] - b1[idx1];
            double by = b2[idx2]-b1[idx2];
            double det = ax*by-ay*bx;
            if(det==0) return false; // Parallel is not intersect
            double alpha = -(bx * b1[idx2] - by * b1[idx1] + a1[idx1] * by - a1[idx2] * bx) / det;
            if (alpha < epsilonZeroMinus || alpha > epsilonOnePlus) return false;
            double beta = -(ax * b1[idx2] - ay * b1[idx1] - ax * a1[idx2] + ay * a1[idx1]) / det;
            bool intersect = beta >= epsilonZeroMinus && beta <= epsilonOnePlus;
            if (intersect && debugIntersections)
                Console.WriteLine("InPlane beta=" + beta);
            return intersect;
        }
        private bool InPlaneIntersectLineSmall(int idx1,int idx2, RHVector3 a1, RHVector3 a2, RHVector3 b1, RHVector3 b2)
        {
            double ax = a2[idx1] - a1[idx1];
            double ay = a2[idx2] - a1[idx2];
            double bx = b2[idx1] - b1[idx1];
            double by = b2[idx2] - b1[idx2];
            double det = ax * by - ay * bx;
            if (det == 0) return false; // Parallel is not intersect
            double alpha = -(bx * b1[idx2] - by * b1[idx1] + a1[idx1] * by - a1[idx2] * bx) / det;
            if (alpha < epsilonZero || alpha > epsilonOneMinus) return false;
            double beta = -(ax * b1[idx2] - ay * b1[idx1] - ax * a1[idx2] + ay * a1[idx1]) / det;
            bool intersect = beta >= epsilonZero && beta <= epsilonOneMinus;
            if (intersect && debugIntersections)
                Console.WriteLine("InPlane beta=" + beta);
            return intersect;
        }
        private bool InPlanePointInside(int d1,int d2, RHVector3 p)
        {
            RHVector3 A = vertices[0].pos;
            double ax = vertices[1].pos[d1]-vertices[0].pos[d1];
            double ay = vertices[1].pos[d2]-vertices[0].pos[d2];
            double bx = vertices[2].pos[d1]-vertices[0].pos[d1];
            double by = vertices[2].pos[d2]-vertices[0].pos[d2];
            double det = ax*by-ay*bx;
            if(det==0) return false;
            double alpha = -(bx * p[d2] - by * p[d1] + A[d1] * by - A[d2] * bx) / det;
            double beta = (ax * p[d2] - ay * p[d1] - ax * A[d2] + ay * A[d1]) / det;
            bool intersect = alpha >= epsilonZeroMinus && beta >= epsilonZeroMinus && alpha + beta <= epsilonOnePlus;
            if (intersect && debugIntersections)
                Console.WriteLine("InPlanePointInside alpha="+alpha+", beta = "+beta);
            return intersect;
        }
        private void DominantAxis(out int d1, out int d2)
        {
            double n1 = Math.Abs(normal.x);
            double n2 = Math.Abs(normal.y);
            double n3 = Math.Abs(normal.z);
            if (n1 > n2 && n1 > n3)
            {
                d1 = 1;
                d2 = 2;
            }
            else if (n2 > n3)
            {
                d1 = 0;
                d2 = 2;
            }
            else
            {
                d1 = 0;
                d2 = 1;
            }
        }
        private bool IntersectsSharedVertex(TopoVertex shared, TopoVertex[] a, TopoVertex[] b,TopoTriangle tri)
        {
            double d1 = DistanceToPlane(b[0].pos);
            double d2 = DistanceToPlane(b[1].pos);            
            // Compute intersection point with plane
            int idx1,idx2;
            DominantAxis(out idx1, out idx2);
            if (Math.Abs(d1) < 1e-8 && Math.Abs(d2)<1e-8) // In plane
            {
                if (InPlanePointInside(idx1,idx2, b[0].pos))
                    return true;
                if (InPlanePointInside(idx1,idx2, b[1].pos))
                    return true;
                if (InPlaneIntersectLineSmall(idx1,idx2, a[0].pos, a[1].pos, shared.pos, b[0].pos))
                    return true;
                if (InPlaneIntersectLineSmall(idx1,idx2, a[0].pos, a[1].pos, shared.pos, b[1].pos))
                    return true;
                return false;
            }
            if (d1 * d2 > 0) return false; // Both points on same side, no intersection possible
            double factor = Math.Abs(d1 / (d1 - d2));
            RHVector3 p1 = b[0].pos; // new RHVector3(normal);
            RHVector3 p2 = b[1].pos; // new RHVector3(normal);
            /*p1.Scale(-d1);
            p2.Scale(-d2);
            p1.AddInternal(b[0].pos);
            p2.AddInternal(b[1].pos);*/
            RHVector3 inter = new RHVector3((1 - factor) * p1.x + factor * p2.x, (1 - factor) * p1.y + factor * p2.y, (1 - factor) * p1.z + factor * p2.z);
            if(inter.Subtract(shared.pos).Length < epsilonZero) {
                if (Math.Abs(d1) < epsilonZero || Math.Abs(d2) < epsilonZero)
                    return false; // Connection ends at shared vertex - does not count as intersection
            }
            if (InPlanePointInside(idx1,idx2, inter))
                return true;
            if(InPlaneIntersectLineSmall(idx1,idx2,a[0].pos,a[1].pos,shared.pos,inter)) 
                return true;
            return false;
        }
        private bool IntersectsSharedEdge(TopoVertex[] shared, TopoVertex a, TopoVertex b, TopoTriangle tri)
        {
            // Test if coplanar. If not no intersection is possible
            if (Math.Abs(DistanceToPlane(b.pos)) > epsilonZero) return false;
            int idx2,idx1;
            DominantAxis(out idx1, out idx2);
            if (InPlanePointInside(idx1,idx2, b.pos)) 
                return true;
            if(tri.InPlanePointInside(idx1,idx2,a.pos)) 
                return true;
            return false;
        }
        public bool Intersects(TopoTriangle tri)
        {
            // First detect shared edges for more reliable and faster tests
            TopoVertex[] shared = new TopoVertex[3];
            TopoVertex[] uniqueA = new TopoVertex[3];
            TopoVertex[] uniqueB = new TopoVertex[3];
            int nShared = 0, nUniqueA = 0, nUniqueB = 0;
            for (int i = 0; i < 3; i++)
            {
                bool isDouble = false;
                for (int j = 0; j < 3; j++)
                {
                    if (vertices[i] == tri.vertices[j])
                    {
                        shared[nShared++] = vertices[i];
                        isDouble = true;
                        break;
                    }
                }
                if (!isDouble)
                    uniqueA[nUniqueA++] = vertices[i];
            }
            if (nShared > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    bool isDouble = false;
                    for (int j = 0; j < nShared; j++)
                    {
                        if (tri.vertices[i] == shared[j])
                        {
                            isDouble = true;
                            break;
                        }
                    }
                    if (!isDouble)
                        uniqueB[nUniqueB++] = tri.vertices[i];
                }
                if (nShared == 1) return IntersectsSharedVertex(shared[0], uniqueA, uniqueB,tri);
                if (nShared == 2) return IntersectsSharedEdge(shared, uniqueA[0], uniqueB[0],tri);
                return true;
            }
            // Nice to read but unoptimized intersection computation
            RHMatrix3 A = new RHMatrix3();
            RHVector3 p1 = vertices[1].pos.Subtract(vertices[0].pos);
            RHVector3 p2 = vertices[2].pos.Subtract(vertices[0].pos);
            A.SetXColumn(p1);
            A.SetYColumn(p2);
            RHVector3 P = new RHVector3(vertices[0].pos);
            RHVector3 q1 = tri.vertices[1].pos.Subtract(tri.vertices[0].pos);
            RHVector3 q2 = tri.vertices[2].pos.Subtract(tri.vertices[0].pos);
            RHVector3 r1 = tri.vertices[0].pos.Subtract(P); // r2 == r1!
            RHVector3 r3 = tri.vertices[2].pos.Subtract(P);
            A.SetZColumn(q1);
            double detAq1 = A.Determinant;
            A.SetZColumn(q2);
            double detAq2 = A.Determinant;
            //A.SetZColumn(q3);
            double detAq3 = detAq1 - detAq2; // A.Determinant;
            A.SetZColumn(r1);
            double detAr1 = A.Determinant;
            //A.SetZColumn(r3);
            double detAr3 = detAr1 + detAq2; // A.Determinant;
            int intersect = 0;
            if (detAq1 == 0 && detAq2 == 0 && detAq3 == 0) // same plane case
            {
                if (detAr1 != 0) return false; // other parallel plance
                // Select plane for computation x-y or x-z based on normal
                int idx1,idx2;
                DominantAxis(out idx1,out idx2);
                if (InPlaneIntersectLine(idx1,idx2, vertices[0].pos, vertices[1].pos, tri.vertices[0].pos, tri.vertices[1].pos)) 
                    return true;
                if (InPlaneIntersectLine(idx1,idx2, vertices[0].pos, vertices[1].pos, tri.vertices[1].pos, tri.vertices[2].pos)) 
                    return true;
                if (InPlaneIntersectLine(idx1,idx2, vertices[0].pos, vertices[1].pos, tri.vertices[2].pos, tri.vertices[0].pos)) 
                    return true;
                if (InPlaneIntersectLine(idx1,idx2, vertices[1].pos, vertices[2].pos, tri.vertices[0].pos, tri.vertices[1].pos)) 
                    return true;
                if (InPlaneIntersectLine(idx1,idx2, vertices[1].pos, vertices[2].pos, tri.vertices[1].pos, tri.vertices[2].pos)) 
                    return true;
                if (InPlaneIntersectLine(idx1,idx2, vertices[1].pos, vertices[2].pos, tri.vertices[2].pos, tri.vertices[0].pos)) 
                    return true;
                if (InPlaneIntersectLine(idx1,idx2, vertices[2].pos, vertices[0].pos, tri.vertices[0].pos, tri.vertices[1].pos)) 
                    return true;
                if (InPlaneIntersectLine(idx1,idx2, vertices[2].pos, vertices[0].pos, tri.vertices[1].pos, tri.vertices[2].pos)) 
                    return true;
                if (InPlaneIntersectLine(idx1,idx2, vertices[2].pos, vertices[0].pos, tri.vertices[2].pos, tri.vertices[0].pos)) 
                    return true;
                // Test if point inside. 1 test per triangle is enough
                if (InPlanePointInside(idx1,idx2, tri.vertices[0].pos)) 
                    return true;
                if (InPlanePointInside(idx1,idx2, tri.vertices[1].pos)) 
                    return true;
                if (InPlanePointInside(idx1,idx2, tri.vertices[2].pos)) 
                    return true;
                if (tri.InPlanePointInside(idx1,idx2, vertices[0].pos)) 
                    return true;
                if (tri.InPlanePointInside(idx1,idx2, vertices[1].pos)) 
                    return true;
                if (tri.InPlanePointInside(idx1,idx2, vertices[2].pos)) 
                    return true;
                return false;
            }
            double beta1=-1, beta2=-1, beta3=-1;
            if (detAq1 != 0) {
                beta1 = -detAr1 / detAq1;
                if (beta1 >= epsilonZeroMinus && beta1 <= epsilonOnePlus) intersect = 1;
            }
            if(detAq2!=0) {
                beta2 = -detAr1 / detAq2;
                if (beta2 >= epsilonZeroMinus && beta2 <= epsilonOnePlus) intersect |= 2;
            }
            if (detAq3 != 0)
            {
                beta3 = -detAr3 / detAq3;
                if (beta3 >= epsilonZeroMinus && beta3 <= epsilonOnePlus) intersect |= 4;
            }
            if (intersect == 7)
            { // Special case intersection in one point caused 3 valid betas
                if (Math.Abs(beta1) < epsilonZero)
                    intersect = 6;
                else if (Math.Abs(beta3) < epsilonZero)
                    intersect = 3;
                else
                    intersect = 5;
            }
            //if (intersect == 0) return false; // Lies on wrong side
            RHVector3 T = null, t = null;
            if ((intersect & 1) == 1)
            {
                T = new RHVector3(q1);
                T.Scale(beta1);
                T.AddInternal(tri.vertices[0].pos);
            }
            if ((intersect & 2) == 2)
            {
                if (T == null)
                {
                    T = new RHVector3(q2);
                    T.Scale(beta2);
                    T.AddInternal(tri.vertices[0].pos);
                }
                else
                {
                    q2.Scale(beta2);
                    q2.AddInternal(tri.vertices[0].pos);
                    t = q2.Subtract(T);
                }
            }
            if ((intersect & 4) == 4 && T!=null && (t == null || t.Length<epsilonZero))
            {
                RHVector3 q3 = tri.vertices[1].pos.Subtract(tri.vertices[2].pos);
                q3.Scale(beta3);
                q3.AddInternal(tri.vertices[2].pos);
                t = q3.Subtract(T);
            }
            if (t == null) 
                return false;
            if (t.Length < epsilonZero)
            { // Only one point touches the plane
                int idx1,idx2;
                DominantAxis(out idx1, out idx2);
                return InPlanePointInside(idx1,idx2, T);
            }
            // Compute intersection points with this triangle
            double d1 = p1.x*t.y-p1.y*t.x;
            double d2 = p1.x*t.z-p1.z*t.x;
            double delta1=-1,delta2=-1,delta3=-1,gamma1=-1,gamma2=-1,gamma3=-1;
            if (Math.Abs(d1) > epsilonZero || Math.Abs(d2) > epsilonZero)
            {
                if (Math.Abs(d1) > Math.Abs(d2))
                {
                    delta1 = -(t.x * T.y - t.y * T.x + P.x * t.y - P.y * t.x) / d1;
                    gamma1 = -(p1.x * T.y - p1.y * T.x - p1.x * P.y + p1.y * P.x) / d1;
                }
                else
                {
                    delta1 = -(t.x * T.z - t.z * T.x + P.x * t.z - P.z * t.x) / d2;
                    gamma1 = -(p1.x * T.z - p1.z * T.x - p1.x * P.z + p1.z * P.x) / d2;
                }
            }
            d1 = p2.x * t.y - p2.y * t.x;
            d2 = p2.x * t.z - p2.z * t.x;
            if (Math.Abs(d1) > epsilonZero || Math.Abs(d2) > epsilonZero)
            {
                if (Math.Abs(d1) > Math.Abs(d2))
                {
                    delta2 = -(t.x * T.y - t.y * T.x + P.x * t.y - P.y * t.x) / d1;
                    gamma2 = -(p2.x * T.y - p2.y * T.x - p2.x * P.y + p2.y * P.x) / d1;
                }
                else
                {
                    delta2 = -(t.x * T.z - t.z * T.x + P.x * t.z - P.z * t.x) / d2;
                    gamma2 = -(p2.x * T.z - p2.z * T.x - p2.x * P.z + p2.z * P.x) / d2;
                }
            }
            P.AddInternal(p1);
            p2.SubtractInternal(p1); // p2 is now p3!
            d1 = p2.x * t.y - p2.y * t.x;
            d2 = p2.x * t.z - p2.z * t.x;
            if (Math.Abs(d1)>epsilonZero || Math.Abs(d2)>epsilonZero)
            {
                if (Math.Abs(d1) > Math.Abs(d2))
                {
                    delta3 = -(t.x * T.y - t.y * T.x + P.x * t.y - P.y * t.x) / d1;
                    gamma3 = -(p2.x * T.y - p2.y * T.x - p2.x * P.y + p2.y * P.x) / d1;
                }
                else
                {
                    delta3 = -(t.x * T.z - t.z * T.x + P.x * t.z - P.z * t.x) / d2;
                    gamma3 = -(p2.x * T.z - p2.z * T.x - p2.x * P.z + p2.z * P.x) / d2;
                }
            }
            // Check for line intersection inside the line. Hits at the vertices to not count!
            if (delta1 >= epsilonZero && delta1 <= epsilonOneMinus && gamma1 >= epsilonZero && gamma1 <= epsilonOneMinus)
                return true;
            if (delta2 >= epsilonZero && delta2 <= epsilonOneMinus && gamma2 >= epsilonZero && gamma2 <= epsilonOneMinus)
                return true;
            if (delta3 >= epsilonZero && delta3 <= epsilonOneMinus && gamma3 >= epsilonZero && gamma3 <= epsilonOneMinus)
                return true;
            // Test if intersection is inside triangle
            intersect = 0;
            if (delta1 >= epsilonZeroMinus && delta1 <= epsilonOnePlus) intersect |= 1;
            if (delta2 >= epsilonZeroMinus && delta2 <= epsilonOnePlus) intersect |= 2;
            if (delta3 >= epsilonZeroMinus && delta3 <= epsilonOnePlus) intersect |= 4;
         /*   if (gamma1 == 0) gamma1 = -1;
            if (gamma2 == 0) gamma2 = -1;
            if (gamma3 == 0) gamma3 = -1;*/
            if (gamma1 == 0) intersect &= ~1;
            if (gamma2 == 0) intersect &= ~2;
            if (gamma3 == 0) intersect &= ~4;
            /*       if ((intersect & 3) == 3) return gamma1 * gamma2 < 0;
                   if ((intersect & 5) == 5) return gamma1 * gamma3 < 0;
                   if ((intersect & 6) == 6) return gamma3 * gamma2 < 0;*/
            if ((intersect & 3) == 3) 
                if (gamma1 * gamma2 < 0)
                    return true;
                else
                    return false;
            if ((intersect & 5) == 5)
                if( gamma1 * gamma3 < 0)
                    return true;
                else
                    return false;
            if ((intersect & 6) == 6) 
                if (gamma3 * gamma2 < 0)
                    return true;
                else
                    return false;
           // if (intersect!=0) happens only with numeric problems
            //    return true;
            return false; // No intersection found :-)
        }
        public int LongestEdgeIndex()
        {
            int best = 0;
            double bestLength = edges[0].EdgeLength;
            for (int i = 1; i < 3; i++)
            {
                double t = edges[i].EdgeLength;
                if (t > bestLength)
                {
                    best = i;
                    bestLength = t;
                }
            }
            return best;
        }
        public void LongestShortestEdgeLength(out int longestEdge, out double longestEdgeLength, out int shortestEdge, out double shortestEdgeLength)
        {
            longestEdge = shortestEdge = 0;
            longestEdgeLength = shortestEdgeLength = edges[0].EdgeLength;
            for (int i = 1; i < 3; i++)
            {
                double t = edges[i].EdgeLength;
                if (t > longestEdgeLength)
                {
                    longestEdge = i;
                    longestEdgeLength = t;
                }
                if (t < shortestEdgeLength)
                {
                    shortestEdge = i;
                    shortestEdgeLength = t;
                }
            }
        }
        public double AngleEdgePoint(int edge, RHVector3 point)
        {
            double a = edges[edge].EdgeLength;
            double b = edges[edge].v1.pos.Subtract(point).Length;
            double c = edges[edge].v2.pos.Subtract(point).Length;
            return Math.Acos((b * b + c * c - a * a) / (2 * b * c));
        }
        /// <summary>
        /// Returns triangle quality in terms of outer circle diameter/inner circle diameter > 2
        /// </summary>
        public double TriangleQuality
        {
            get
            {
                double a = edges[0].EdgeLength;
                double b = edges[1].EdgeLength;
                double c = edges[2].EdgeLength;
                double bc2 = 2 * b * c;
                double sinalpha = Math.Sin(Math.Acos((b * b + c * c - a * a) / (bc2)));
                return a * (a + b + c) / (bc2 * sinalpha * sinalpha);
            }
        }
        static public double TriangleQualityFromPositions(RHVector3 p1, RHVector3 p2, RHVector3 p3)
        {
            double a = p1.Distance(p2);
            double b = p1.Distance(p3);
            double c = p2.Distance(p3);
            double bc2 = 2 * b * c;
            double sinalpha = Math.Sin(Math.Acos((b * b + c * c - a * a) / (bc2)));
            return a * (a + b + c) / (bc2 * sinalpha * sinalpha);
        }
        public bool SmoothAspectRatio(TopoModel model,double maxRatio)
        {
            double maxLen,minLen;
            int maxIdx,minIdx;
            LongestShortestEdgeLength(out maxIdx,out maxLen,out minIdx,out minLen);
            if (minLen == 0) 
                return false;
            if (maxLen>1 && maxLen / minLen > maxRatio)
            {
                RHVector3 center = edges[maxIdx].v1.pos.Add(edges[maxIdx].v2.pos);
                center.Scale(0.5);
                TopoVertex newVertex = new TopoVertex(0, center);
                model.addVertex(newVertex);
                edges[maxIdx].InsertVertex(model,newVertex);
                return true;
            }
            return false;
        }
        public override string ToString()
        {
            return "Tri: " + vertices[0] + " | " + vertices[1] + " | " + vertices[2]+
                " Edges:"+edges[0].faces.Count+"/"+edges[1].faces.Count+"/"+edges[2].faces.Count;
        }
    }
    public class TopoTriangleDistance : IComparer<TopoTriangleDistance>, IComparable<TopoTriangleDistance>
    {
        public double distance;
        public TopoTriangle triangle;
        public TopoTriangleDistance(double dist, TopoTriangle tri)
        {
            triangle = tri;
            distance = dist;
        }
        public int Compare(TopoTriangleDistance td1, TopoTriangleDistance td2)
        {
            return -td1.distance.CompareTo(td2.distance);
        }
        public int CompareTo(TopoTriangleDistance td)
        {
            return -distance.CompareTo(td.distance);
        }
    }
}
