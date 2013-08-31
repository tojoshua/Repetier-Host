/*
   Copyright 2011 repetier repetierdev@gmail.com

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RepetierHost.model;
using RepetierHost.view;
using System.IO;
using OpenTK;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics;
using RepetierHost.view.utils;

namespace RepetierHost.model.geom
{
    public class TopoModel
    {
        public const bool debugRepair = false;
        public const float epsilon = 0.001f;
        public TopoVertexStorage vertices = new TopoVertexStorage();
        public TopoTriangleStorage triangles = new TopoTriangleStorage();
        public LinkedList<TopoEdge> edges = new LinkedList<TopoEdge>();
        public RHBoundingBox boundingBox = new RHBoundingBox();
        public HashSet<TopoTriangle> intersectingTriangles = new HashSet<TopoTriangle>();
        public int badEdges = 0;
        public int badTriangles = 0;
        public int shells = 0;
        public int updatedNormals = 0;
        public int loopEdges = 0;
        public int manyShardEdges = 0;
        public bool manifold = false;
        public bool normalsOriented = false;
        public bool intersectionsUpToDate = false;
        public InfoProgressPanel ipp = null;

        public void StartAction(string name)
        {
            if (ipp == null) return;
            ipp.Action = Trans.T(name);
            ipp.Progress = 0;
        }
        public void Progress(double prg)
        {
            prg *= 100.0;
            if (ipp == null) return;
            if (prg < 0) prg = 0;
            if (prg > 100) prg = 100;
            ipp.Progress = (int)prg;
        }
        public bool IsActionStopped()
        {
            if (ipp == null) return false;
            return ipp.IsKilled;
        }
        public void clear()
        {
            vertices.Clear();
            triangles.Clear();
            edges.Clear();
            boundingBox.Clear();
            intersectionsUpToDate = false;
        }
        public TopoModel Copy()
        {
            TopoModel newModel = new TopoModel();
            int nOld = vertices.Count;
            int i = 0;
            List<TopoVertex> vcopy = new List<TopoVertex>(vertices.Count);
            foreach (TopoVertex v in vertices)
            {
                v.id = i++;
                TopoVertex newVert = new TopoVertex(v.id, v.pos);
                newModel.addVertex(newVert);
                vcopy.Add(newVert);
            }
            foreach (TopoTriangle t in triangles)
            {
                TopoTriangle triangle = new TopoTriangle(newModel, vcopy[t.vertices[0].id], vcopy[t.vertices[1].id], vcopy[t.vertices[2].id], t.normal.x, t.normal.y, t.normal.z);
                newModel.triangles.Add(triangle);
            }
            UpdateVertexNumbers();
            newModel.UpdateVertexNumbers();
            newModel.badEdges = 0;
            newModel.badTriangles = badTriangles;
            newModel.shells = shells;
            newModel.updatedNormals = updatedNormals;
            newModel.loopEdges = loopEdges;
            newModel.manyShardEdges = 0;
            newModel.manifold = manifold;
            newModel.normalsOriented = normalsOriented;
            return newModel;
        }
        public void Merge(TopoModel model, Matrix4 trans)
        {
            int nOld = vertices.Count;
            int i = 0;
            List<TopoVertex> vcopy = new List<TopoVertex>(model.vertices.Count);
            foreach (TopoVertex v in model.vertices)
            {
                v.id = i++;
                TopoVertex newVert = new TopoVertex(v.id, v.pos, trans);
                addVertex(newVert);
                vcopy.Add(newVert);
            }
            foreach (TopoTriangle t in model.triangles)
            {
                TopoTriangle triangle = new TopoTriangle(this, vcopy[t.vertices[0].id], vcopy[t.vertices[1].id], vcopy[t.vertices[2].id]);
                triangle.RecomputeNormal();
                triangles.Add(triangle);
            }
            RemoveUnusedDatastructures();
            intersectionsUpToDate = false;
        }
        public void addVertex(TopoVertex v)
        {
            vertices.Add(v);
            boundingBox.Add(v.pos);
        }
        public TopoVertex findVertexOrNull(RHVector3 pos)
        {
            return vertices.SearchPoint(pos);
            /*  foreach (TopoVertex v in vertices)
              {
                  if (v.distance(pos) < epsilon)
                      return v;
              }
              return null;*/
        }
        public TopoVertex addVertex(RHVector3 pos)
        {
            /*if (Math.Abs(pos.x + 3.94600009918213) < 0.001 && Math.Abs(pos.y + 2.16400003433228) < 0.001 && Math.Abs(pos.z - 7.9980001449585) < 0.001)
            {
                Console.WriteLine(pos);
            }*/
            TopoVertex newVertex = findVertexOrNull(pos);
            if (newVertex == null)
            {
                newVertex = new TopoVertex(vertices.Count + 1, pos);
                addVertex(newVertex);
            }
            return newVertex;
        }
        public void UpdateVertexNumbers()
        {
            int i = 1;
            foreach (TopoVertex v in vertices)
            {
                v.id = i++;
            }
        }
        public TopoEdge getOrCreateEdgeBetween(TopoVertex v1, TopoVertex v2)
        {
            foreach (TopoTriangle t in v1.connectedFacesList)
            {
                for (int i = 0; i < 3; i++)
                {
                    if ((v1 == t.vertices[i] && v2 == t.vertices[(i + 1) % 3]) || (v2 == t.vertices[i] && v1 == t.vertices[(i + 1) % 3]))
                        return t.edges[i];
                }
            }
            /*foreach (TopoEdge edge in edges)
            {
                if (edge.isBuildOf(v1, v2))
                    return edge;
            }*/
            TopoEdge newEdge = new TopoEdge(v1, v2);
            edges.AddLast(newEdge);
            return newEdge;
        }
        public void UpdateIntersectingTriangles()
        {
            if (intersectionsUpToDate) return;
            intersectingTriangles.Clear();
            HashSet<TopoTriangle> candidates;
            int counter = 0,n = triangles.Count;
            StartAction("L_INTERSECTION_TESTS");
            foreach (TopoTriangle test in triangles)
            {
                test.hasIntersections = false;
            }
            foreach (TopoTriangle test in triangles)
            {
                counter++;
                Progress((double)counter / n);
                if (counter % 600 == 0)
                {
                    Application.DoEvents();
                    if (IsActionStopped()) return;
                }
                if (test.hasIntersections) continue;
                candidates = triangles.FindIntersectionCandidates(test);
                foreach (TopoTriangle candidate in candidates)
                {
                    if (test.Intersects(candidate))
                    {
                        candidate.CheckIfColinear();
                        test.Intersects(candidate);
                        if (test.hasIntersections == false)
                        {
                            test.hasIntersections = true;
                            test.bad = true;
                            intersectingTriangles.Add(test);
                           // Debug.WriteLine(test);
                        }
                        if (candidate.hasIntersections == false)
                        {
                            candidate.hasIntersections = true;
                            candidate.bad = true;
                            intersectingTriangles.Add(candidate);
                            //Debug.WriteLine(candidate);
                        }
                    }
                }
            }
            intersectionsUpToDate = true;
        }
        public TopoTriangle addTriangle(double p1x, double p1y, double p1z, double p2x, double p2y, double p2z,
            double p3x, double p3y, double p3z, double nx, double ny, double nz)
        {
            TopoVertex v1 = addVertex(new RHVector3(p1x, p1y, p1z));
            TopoVertex v2 = addVertex(new RHVector3(p2x, p2y, p2z));
            TopoVertex v3 = addVertex(new RHVector3(p3x, p3y, p3z));
            TopoTriangle triangle = new TopoTriangle(this, v1, v2, v3, nx, ny, nz);
            return AddTriangle(triangle);
        }
        public TopoTriangle addTriangle(RHVector3 p1,RHVector3 p2,RHVector3 p3,RHVector3 normal)
        {
            TopoVertex v1 = addVertex(p1);
            TopoVertex v2 = addVertex(p2);
            TopoVertex v3 = addVertex(p3);
            TopoTriangle triangle = new TopoTriangle(this, v1, v2, v3, normal.x, normal.y, normal.z);
            return AddTriangle(triangle);
        }
        public TopoTriangle AddTriangle(TopoTriangle triangle)
        {
            if (triangle.IsDegenerated())
                triangle.Unlink(this);
            else
                triangles.Add(triangle);
            return triangle;
        }
        public void removeTriangle(TopoTriangle triangle)
        {
            triangle.Unlink(this);
            triangles.Remove(triangle);
        }
        public void UpdateNormals()
        {
            CountShells();
            StartAction("L_FIXING_NORMALS");
            ResetTriangleMarker();
            updatedNormals = 0;
            int count = 0;
            foreach (TopoTriangle triangle in triangles)
            {
                count++;
                Progress((double)count / triangles.Count);
                if ((count % 2500) == 0)
                    Application.DoEvents();
                if (triangle.algHelper == 0)
                {
                    int testShell = triangle.shell;
                    /*triangle.RecomputeNormal();
                    RHVector3 lineStart = triangle.Center;
                    RHVector3 lineDirection = triangle.normal;
                    int hits = 0;
                    double delta;
                    foreach (TopoTriangle test in triangles)
                    {
                        if (test != triangle && test.IntersectsLine(lineStart, lineDirection, out delta))
                        {
                           // Debug.WriteLine(test);
                           // Debug.WriteLine(triangle);
                            if (delta > 0) hits++;
                        }
                    }
                    if ((hits & 1) == 1)
                    {
                        triangle.FlipDirection();
                        updatedNormals++;
                    }*/
                    FloodFillNormals(triangle);
                    if (SignedShellVolume(testShell) < 0)
                    {
                        foreach (TopoTriangle flip in triangles)
                        {
                            triangle.FlipDirection();
                        }
                    }
                }
                else
                    triangle.RecomputeNormal();
            }
            RLog.info(Trans.T("L_ANA_CORRECTED_NORMAL_ORIENTATIONS") + updatedNormals);
        }

        private void FloodFillNormals(TopoTriangle good)
        {
            good.algHelper = 1;
            HashSet<TopoTriangle> front = new HashSet<TopoTriangle>();
            front.Add(good);
            HashSet<TopoTriangle> newFront = new HashSet<TopoTriangle>();
            int i;
            int cnt = 0;
            while (front.Count > 0)
            {
                foreach (TopoTriangle t in front)
                {
                    cnt++;
                    if((cnt % 2000) == 0)
                        Application.DoEvents();
                    for (i = 0; i < 3; i++)
                    {
                        foreach (TopoTriangle test in t.edges[i].faces)
                        {
                            if (t != test && test.algHelper == 0)
                            {
                                test.algHelper = 1;
                                newFront.Add(test);
                                if (!t.SameNormalOrientation(test))
                                {
                                    test.FlipDirection();
                                    updatedNormals++;
                                }
                            }
                        }
                    }
                }
                front = newFront;
                newFront = new HashSet<TopoTriangle>();
            }
        }
        public bool CheckNormals()
        {
            CountShells();
            ResetTriangleMarker();
            normalsOriented = true;
            foreach (TopoTriangle triangle in triangles)
            {
                if (triangle.algHelper == 0)
                {
                    int testShell = triangle.shell;
                    /*triangle.RecomputeNormal();
                    RHVector3 lineStart = triangle.Center;
                    RHVector3 lineDirection = triangle.normal;
                    int hits = 0;
                    double delta;
                    foreach (TopoTriangle test in triangles)
                    {
                        if (test != triangle && test.IntersectsLine(lineStart, lineDirection, out delta))
                        {
                            if (delta > 0) hits++;
                        }
                    }
                    if ((hits & 1) == 1)
                    {
                        normalsOriented = false;
                        return false;
                    }*/
                    if (SignedShellVolume(testShell) < 0)
                    {
                        normalsOriented = false;
                        return false;
                    }
                    if (!FloodFillCheckNormals(triangle))
                    {
                        normalsOriented = false;
                        return false;
                    }
                }
            }
            return true;
        }
        
        public double Surface()
        {
            double surface = 0;
            foreach (TopoTriangle t in triangles)
            {
                surface += t.Area();
            }
            return surface;
        }
        
        public double Volume()
        {
            double volume = 0;
            foreach (TopoTriangle t in triangles)
                volume += t.SignedVolume();
            return Math.Abs(volume);
        }
        
        public double SignedShellVolume(int shell)
        {
            double volume = 0;
            foreach (TopoTriangle t in triangles)
            {
                if(t.shell == shell)
                    volume += t.SignedVolume();
            }
            return volume;
        }

        private bool FloodFillCheckNormals(TopoTriangle good)
        {
            good.algHelper = 1;
            HashSet<TopoTriangle> front = new HashSet<TopoTriangle>();
            front.Add(good);
            HashSet<TopoTriangle> newFront = new HashSet<TopoTriangle>();
            int i;
            int cnt = 0;
            while (front.Count > 0)
            {
                foreach (TopoTriangle t in front)
                {
                    cnt++;
                    if((cnt % 2000) == 0)
                        Application.DoEvents();
                    for (i = 0; i < 3; i++)
                    {
                        foreach (TopoTriangle test in t.edges[i].faces)
                        {
                            if (t != test && test.algHelper == 0)
                            {
                                test.algHelper = 1;
                                newFront.Add(test);
                                test.RecomputeNormal();
                                if (!t.SameNormalOrientation(test))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                front = newFront;
                newFront = new HashSet<TopoTriangle>();
            }
            return true;
        }
        public int MarkPlanarRegions()
        {
            ResetTriangleMarker();
            int regionCounter = 0;
            foreach (TopoTriangle triangle in triangles)
            {
                if (triangle.algHelper == 0)
                {
                    FloodFillPlanarRegions(triangle, ++regionCounter);
                }
            }
            return regionCounter;
        }
        private void FloodFillPlanarRegions(TopoTriangle good, int marker)
        {
            good.algHelper = marker;
            HashSet<TopoTriangle> front = new HashSet<TopoTriangle>();
            front.Add(good);
            HashSet<TopoTriangle> newFront = new HashSet<TopoTriangle>();
            int i;
            while (front.Count > 0)
            {
                Application.DoEvents();
                foreach (TopoTriangle t in front)
                {
                    for (i = 0; i < 3; i++)
                    {
                        foreach (TopoTriangle test in t.edges[i].faces)
                        {
                            if (test.algHelper == 0)
                            {
                                if (t.normal.Angle(test.normal) < 1e-6)
                                {
                                    test.algHelper = 1;
                                    newFront.Add(test);
                                }
                            }
                        }
                    }
                }
                front = newFront;
                newFront = new HashSet<TopoTriangle>();
            }
        }
        public HashSet<TopoEdge> OpenLoopEdges()
        {
            HashSet<TopoEdge> list = new HashSet<TopoEdge>();
            foreach (TopoEdge edge in edges)
            {
                if (edge.connectedFaces == 1)
                    list.Add(edge);
            }
            return list;
        }
        public bool JoinTouchedOpenEdges(double limit)
        {
            /*Console.WriteLine("Open Edges:");
            foreach (TopoEdge e in OpenLoopEdges())
            {
                Console.WriteLine(e);
            }
            Console.WriteLine("=========");*/
            bool changed = false;
            bool somethingChanged = false;
            RHVector3 projectedPoint = new RHVector3(0, 0, 0);
            double lambda;
            int cnt = 0;
            do
            {
                changed = false;
                HashSet<TopoEdge> list = OpenLoopEdges();
                foreach (TopoEdge edge in list)
                {
                    cnt++;
                    if ((cnt % 10) == 0)
                    {
                        Application.DoEvents();
                        if (IsActionStopped()) return true;
                    }
                    foreach (TopoEdge test in list)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            if (edge.ProjectPoint(test[i].pos, out lambda, projectedPoint))
                            {
                                double dist = test[i].pos.Distance(projectedPoint);
                                //Console.WriteLine("Distance:" + dist + " lambda " + lambda);
                                if (dist < limit && edge.FirstTriangle.VertexIndexFor(test[i]) == -1)
                                {
                                    //Console.WriteLine("RedLine:" + edge);
                                    edge.InsertVertex(this, test[i]);
                                    changed = true;
                                    somethingChanged = true;
                                    break;
                                }
                            }
                        }
                        if (changed) break;
                    }
                    if (changed) break;
                }
            } while (changed == true);
            if (somethingChanged) intersectionsUpToDate = false;
            return somethingChanged;
        }
        public bool RemoveUnusedDatastructures()
        {
            LinkedList<TopoEdge> removeEdges = new LinkedList<TopoEdge>();
            LinkedList<TopoVertex> removeVertices = new LinkedList<TopoVertex>();
            foreach (TopoEdge edge in edges)
            {
                if (edge.connectedFaces == 0)
                    removeEdges.AddLast(edge);
            }
            foreach (TopoVertex vertex in vertices)
            {
                if (vertex.connectedFaces == 0)
                    removeVertices.AddLast(vertex);
            }
            bool changed = removeEdges.Count > 0 || removeVertices.Count > 0;
            foreach (TopoEdge edge in removeEdges)
                edges.Remove(edge);
            foreach (TopoVertex vertex in removeVertices)
                vertices.Remove(vertex);
            int vertexNumber = 1;
            foreach (TopoVertex vertex in vertices)
                vertex.id = vertexNumber++;
            return changed;
        }
        public void ResetTriangleMarker()
        {
            foreach (TopoTriangle triangle in triangles)
            {
                triangle.algHelper = 0;
            }
        }
        public bool RemoveDegeneratedFaces()
        {
            bool changed = false;
            HashSet<TopoTriangle> deleteFaces = new HashSet<TopoTriangle>();
            int defectTriangles = 0;
            int isolatedTriangles = 0;
            int doubleTriangles = 0;
            ResetTriangleMarker();
            foreach (TopoTriangle triangle in triangles)
            {
                if (triangle.IsIsolated())
                {
                    isolatedTriangles++;
                    deleteFaces.Add(triangle);
                }
                else if (triangle.IsDegenerated())
                {
                    defectTriangles++;
                    deleteFaces.Add(triangle);
                }

                //HashSet<TopoTriangle> candidates = triangles.FindIntersectionCandidates(triangle);
                if (triangle.algHelper == 0)
                    foreach (TopoTriangle candidate in triangle.vertices[0].connectedFacesList)
                    {
                        if (triangle.NumberOfSharedVertices(candidate) == 3)
                        {
                            triangle.algHelper = 1;
                            if (candidate.algHelper == 0)
                            {
                                deleteFaces.Add(candidate);
                                doubleTriangles++;
                            }
                        }
                    }
            }
            changed = deleteFaces.Count > 0;
            foreach (TopoTriangle face in deleteFaces)
            {
                face.Unlink(this);
                triangles.Remove(face);
            }
            if (defectTriangles > 0)
                RLog.info(Trans.T("L_REMOVED_DEGENERATED") + defectTriangles);
            if (isolatedTriangles > 0)
                RLog.info(Trans.T("L_REMOVED_ISOLATED") + isolatedTriangles);
            if (doubleTriangles > 0)
                RLog.info(Trans.T("L_REMOVED_DOUBLE") + doubleTriangles);
            if (changed) intersectionsUpToDate = false;
            return changed;
        }
        public void statistics()
        {
            shells = CountShells();
            RLog.info(Trans.T("L_ANA_VERTICES") + vertices.Count);
            RLog.info(Trans.T("L_ANA_EDGES") + edges.Count);
            RLog.info(Trans.T("L_ANA_FACES") + triangles.Count);
            RLog.info(Trans.T("L_ANA_SHELLS") + shells);
        }
        public int RemoveColinearFaces()
        {
            LinkedList<TopoTriangle> todo = new LinkedList<TopoTriangle>();
            foreach (TopoTriangle triangle in triangles)
            {
                if (triangle.CheckIfColinear())
                    todo.AddLast(triangle);
            }
            foreach (TopoTriangle triangle in todo)
            {
                if (triangles.Contains(triangle))
                    triangle.FixColinear(this);
            }
            if (todo.Count > 0) intersectionsUpToDate = false;
            return todo.Count;
        }
        public void RepairUnobtrusive()
        {
            RemoveDegeneratedFaces();
            int colinearCount = RemoveColinearFaces();
            if (colinearCount > 0)
                RLog.info(Trans.T("L_ANA_FIXED_COLLINEAR_TRIANGLES") + colinearCount);
            JoinTouchedOpenEdges(0.1);
            RemoveUnusedDatastructures();
            UpdateNormals();
        }
        public void Analyse()
        {
            RLog.info(Trans.T("L_STARTING_ANALYSER"));
            //RepairUnobtrusive();
            UpdateIntersectingTriangles();
            CheckNormals();
            manyShardEdges = 0;
            loopEdges = 0;
            foreach (TopoEdge edge in edges)
            {
                if (edge.connectedFaces < 2) 
                    loopEdges++;
                else if (edge.connectedFaces > 2) 
                    manyShardEdges++;
            }
            if (loopEdges > 0)
                RLog.info(Trans.T("L_ANA_LOOP_EDGES") + loopEdges);
            if (manyShardEdges > 0)
                RLog.info(Trans.T("L_ANA_HIGHLY_CONNECTED") + manyShardEdges);
            if (intersectingTriangles.Count > 0)
                RLog.info(Trans.T("L_ANA_INTERSECTING_TRIANGLES") + intersectingTriangles.Count);
            if (updatedNormals > 0)
                RLog.info(Trans.T("L_ANA_CORRECTED_NORMAL_ORIENTATIONS") + updatedNormals);
            statistics();
            if (loopEdges + manyShardEdges == 0)
            {
                manifold = true;
                RLog.info(Trans.T("L_OBJECT_IS_MANIFOLD"));
            }
            else
            {
                manifold = false;
                RLog.info(Trans.T("L_OBJECT_IS_NON_MANIFOLD"));
            }
            UpdateVertexNumbers();
            RLog.info(Trans.T("L_ANALYSER_FINISHED"));
            if (false)
            {
                Debug.WriteLine("Intersecting triangles:");
                foreach (TopoTriangle t in intersectingTriangles)
                    Debug.WriteLine(t);
                Debug.WriteLine("========");
            }
        }
        public void RetestIntersectingTriangles()
        {
            foreach (TopoTriangle t in intersectingTriangles)
            {
                TopoTriangle hit = IntersectsTriangleAnyTriangle(t);
                if (hit != null)
                {
                    Debug.WriteLine(t);
                    Debug.WriteLine("Hits:" + hit);
                }
            }
        }
        public TopoTriangle IntersectsTriangleAnyTriangle(TopoTriangle test)
        {
            HashSet<TopoTriangle> candidates = triangles.FindIntersectionCandidates(test);
            foreach (TopoTriangle candidate in candidates)
            {
                if (test.Intersects(candidate)) return candidate;
            }
            return null;
        }
        public void checkEdgesOver2()
        {
            foreach (TopoEdge e in edges)
            {
                if (e.connectedFaces > 2)
                {
                    Console.WriteLine("Too many connected faces");
                    return;
                }
            }
        }
        public void updateBad()
        {
            badTriangles = badEdges = 0;
            foreach (TopoTriangle triangle in triangles)
            {
                triangle.bad = triangle.hasIntersections; // intersectingTriangles.Contains(triangle);
            }
            foreach (TopoEdge edge in edges)
            {
                if (edge.connectedFaces != 2)
                {
                    badEdges++;
                }
            }
            foreach (TopoTriangle triangle in triangles)
            {
                if (triangle.bad)
                    badTriangles++;
            }
        }
        private void FloodFillTriangles(TopoTriangle tri, int value)
        {
            tri.algHelper = value;
            HashSet<TopoTriangle> front = new HashSet<TopoTriangle>();
            front.Add(tri);
            HashSet<TopoTriangle> newFront = new HashSet<TopoTriangle>();
            int i;
            while (front.Count > 0)
            {
                foreach (TopoTriangle t in front)
                {
                    for (i = 0; i < 3; i++)
                    {
                        foreach (TopoTriangle test in t.edges[i].faces)
                        {
                            if (test.algHelper == 0)
                            {
                                test.algHelper = value;
                                newFront.Add(test);
                            }
                        }
                    }
                }
                front = newFront;
                newFront = new HashSet<TopoTriangle>();
            }
        }
        private void FloodFillShells(TopoTriangle tri, int value)
        {
            tri.shell = value;
            HashSet<TopoTriangle> front = new HashSet<TopoTriangle>();
            front.Add(tri);
            HashSet<TopoTriangle> newFront = new HashSet<TopoTriangle>();
            int i;
            while (front.Count > 0)
            {
                foreach (TopoTriangle t in front)
                {
                    for (i = 0; i < 3; i++)
                    {
                        foreach (TopoTriangle test in t.edges[i].faces)
                        {
                            if (test.shell == 0)
                            {
                                test.shell = value;
                                newFront.Add(test);
                            }
                        }
                    }
                }
                front = newFront;
                newFront = new HashSet<TopoTriangle>();
            }
        }
        public int CountShells()
        {
            foreach (TopoTriangle t in triangles)
                t.shell = 0;
            int nShells = 0;
            foreach (TopoTriangle t in triangles)
            {
                if (t.shell == 0)
                {
                    nShells++;
                    FloodFillShells(t, nShells);
                }
            }
            return nShells;
        }
        public List<TopoModel> SplitIntoSurfaces()
        {
            CountShells();
            foreach (TopoTriangle tri in triangles)
                tri.algHelper = tri.shell;

            List<TopoModel> models = new List<TopoModel>();
            Dictionary<int, TopoModel> modelMap = new Dictionary<int, TopoModel>();
            foreach (TopoTriangle tri in triangles)
            {
                int shell = tri.algHelper;
                if (modelMap.ContainsKey(shell))
                {
                    RHVector3 v1 = tri.vertices[0].pos;
                    RHVector3 v2 = tri.vertices[1].pos;
                    RHVector3 v3 = tri.vertices[2].pos;
                    RHVector3 n = tri.normal;
                    modelMap[shell].addTriangle(v1.x, v1.y, v1.z, v2.x, v2.y, v2.z, v3.x, v3.y, v3.z, n.x, n.y, n.z);
                }
                else
                {
                    List<TopoTriangleDistance> intersections = new List<TopoTriangleDistance>();
                    RHVector3 lineStart = tri.Center;
                    RHVector3 lineDirection = tri.normal;
                    double delta;
                    foreach (TopoTriangle test in triangles)
                    {
                        if(test.IntersectsLine(lineStart, lineDirection, out delta))
                        {
                            intersections.Add(new TopoTriangleDistance(delta,test));
                        }
                    }
                    intersections.Sort();
                    Stack<TopoTriangleDistance> tdStack = new Stack<TopoTriangleDistance>();
                    foreach (TopoTriangleDistance td in intersections)
                    {
                        if (td.triangle == tri)
                        {
                            TopoModel m = null;
                            if ((tdStack.Count & 2) == 0)
                            {
                                m = new TopoModel();
                                models.Add(m);
                                modelMap.Add(shell, m);
                            }
                            else
                            {
                                int trueShell = tdStack.ElementAt(tdStack.Count-1).triangle.algHelper;
                                foreach (TopoTriangle t in triangles)
                                {
                                    if (t.algHelper == shell)
                                        t.algHelper = trueShell;
                                }
                                if (modelMap.ContainsKey(trueShell))
                                    m = modelMap[trueShell];
                                else
                                {
                                    m = new TopoModel();
                                    models.Add(m);
                                    modelMap.Add(shell, m);
                                }
                            }
                            RHVector3 v1 = tri.vertices[0].pos;
                            RHVector3 v2 = tri.vertices[1].pos;
                            RHVector3 v3 = tri.vertices[2].pos;
                            RHVector3 n = tri.normal;
                            m.addTriangle(v1.x, v1.y, v1.z, v2.x, v2.y, v2.z, v3.x, v3.y, v3.z, n.x, n.y, n.z);
                            break;
                        }
                        else if (tdStack.Count > 0 && tdStack.Peek().triangle.algHelper == td.triangle.algHelper)
                        {
                            tdStack.Pop();
                        }
                        else
                        {
                            tdStack.Push(td);
                        }
                    }
                }
            }
            foreach (TopoModel m in models)
            {
                m.Analyse();
            }
            return models;
        }


        public void CutMesh(Submesh mesh, RHVector3 normal, RHVector3 point,int defaultFaceColor)
        {
            TopoPlane plane = new TopoPlane(normal, point);
            bool drawEdges = Main.threeDSettings.ShowEdges;
            foreach (TopoEdge e in edges)
                e.algHelper = 0; // Mark drawn edges, so we insert them only once
            foreach (TopoTriangle t in triangles)
            {
                int side = plane.testTriangleSideFast(t);
                if (side == -1)
                {
                    mesh.AddTriangle(t.vertices[0].pos, t.vertices[1].pos, t.vertices[2].pos, (t.bad ? Submesh.MESHCOLOR_ERRORFACE : defaultFaceColor));
                    if (drawEdges)
                    {
                        if (t.edges[0].algHelper == 0)
                        {
                            mesh.AddEdge(t.vertices[0].pos, t.vertices[1].pos, t.edges[0].connectedFaces == 2 ? Submesh.MESHCOLOR_EDGE : Submesh.MESHCOLOR_ERROREDGE);
                            t.edges[0].algHelper = 1;
                        }
                        if (t.edges[1].algHelper == 0)
                        {
                            mesh.AddEdge(t.vertices[1].pos, t.vertices[2].pos, t.edges[1].connectedFaces == 2 ? Submesh.MESHCOLOR_EDGE : Submesh.MESHCOLOR_ERROREDGE);
                            t.edges[1].algHelper = 1;
                        }
                        if (t.edges[2].algHelper == 0)
                        {
                            mesh.AddEdge(t.vertices[2].pos, t.vertices[0].pos, t.edges[2].connectedFaces == 2 ? Submesh.MESHCOLOR_EDGE : Submesh.MESHCOLOR_ERROREDGE);
                            t.edges[2].algHelper = 1;
                        }
                    }
                    else
                    {
                        if (t.edges[0].algHelper == 0 && t.edges[0].connectedFaces != 2)
                        {
                            mesh.AddEdge(t.vertices[0].pos, t.vertices[1].pos, Submesh.MESHCOLOR_ERROREDGE);
                            t.edges[0].algHelper = 1;
                        }
                        if (t.edges[1].algHelper == 0 && t.edges[1].connectedFaces != 2)
                        {
                            mesh.AddEdge(t.vertices[1].pos, t.vertices[2].pos, Submesh.MESHCOLOR_ERROREDGE);
                            t.edges[1].algHelper = 1;
                        }
                        if (t.edges[2].algHelper == 0 && t.edges[2].connectedFaces != 2)
                        {
                            mesh.AddEdge(t.vertices[2].pos, t.vertices[0].pos, Submesh.MESHCOLOR_ERROREDGE);
                            t.edges[2].algHelper = 1;
                        }
                    }
                }
                else if (side == 0)
                {
                    plane.addIntersectionToSubmesh(mesh, t, drawEdges, (t.bad ? Submesh.MESHCOLOR_ERRORFACE : defaultFaceColor));
                }
            }
        }
        public void FillMesh(Submesh mesh,int defaultColor)
        {
            bool drawEdges = Main.threeDSettings.ShowEdges;
            foreach (TopoTriangle t in triangles)
            {
                mesh.AddTriangle(t.vertices[0].pos, t.vertices[1].pos, t.vertices[2].pos, (t.bad ? Submesh.MESHCOLOR_ERRORFACE : defaultColor));
                if (drawEdges)
                {
                    mesh.AddEdge(t.vertices[0].pos, t.vertices[1].pos, t.edges[0].connectedFaces == 2 ? Submesh.MESHCOLOR_EDGE : Submesh.MESHCOLOR_ERROREDGE);
                    mesh.AddEdge(t.vertices[1].pos, t.vertices[2].pos, t.edges[1].connectedFaces == 2 ? Submesh.MESHCOLOR_EDGE : Submesh.MESHCOLOR_ERROREDGE);
                    mesh.AddEdge(t.vertices[2].pos, t.vertices[0].pos, t.edges[2].connectedFaces == 2 ? Submesh.MESHCOLOR_EDGE : Submesh.MESHCOLOR_ERROREDGE);
                }
                else
                {
                    if(t.edges[0].connectedFaces != 2)
                        mesh.AddEdge(t.vertices[0].pos, t.vertices[1].pos, Submesh.MESHCOLOR_ERROREDGE);
                    if (t.edges[1].connectedFaces != 2)
                        mesh.AddEdge(t.vertices[1].pos, t.vertices[2].pos, Submesh.MESHCOLOR_ERROREDGE);
                    if (t.edges[2].connectedFaces != 2)
                        mesh.AddEdge(t.vertices[2].pos, t.vertices[0].pos, Submesh.MESHCOLOR_ERROREDGE);
                }
            }
        }
        public void FillMeshTrianglesOnly(Submesh mesh, int defaultColor)
        {
            bool drawEdges = Main.threeDSettings.ShowEdges;
            foreach (TopoTriangle t in triangles)
            {
                mesh.AddTriangle(t.vertices[0].pos, t.vertices[1].pos, t.vertices[2].pos, (t.bad ? Submesh.MESHCOLOR_ERRORFACE : defaultColor));
            }
        }

        public void exportObj(string filename, bool withNormals)
        {
            FileStream fs = File.Open(filename, FileMode.Create);
            TextWriter w = new EnglishStreamWriter(fs);
            w.WriteLine("# exported by Repetier-Host");
            foreach (TopoVertex v in vertices)
            {
                w.Write("v ");
                w.Write(v.pos.x);
                w.Write(" ");
                w.Write(v.pos.y);
                w.Write(" ");
                w.WriteLine(v.pos.z);
            }
            int idx = 1;
            if (withNormals)
            {
                foreach (TopoTriangle t in triangles)
                {
                    w.Write("vn ");
                    w.Write(t.normal.x);
                    w.Write(" ");
                    w.Write(t.normal.y);
                    w.Write(" ");
                    w.WriteLine(t.normal.z);
                }
            }
            foreach (TopoTriangle t in triangles)
            {
                w.Write("f ");
                w.Write(t.vertices[0].id);
                if (withNormals)
                {
                    w.Write("//");
                    w.Write(idx);
                }
                w.Write(" ");
                w.Write(t.vertices[1].id);
                if (withNormals)
                {
                    w.Write("//");
                    w.Write(idx);
                }
                w.Write(" ");
                w.Write(t.vertices[2].id);
                if (withNormals)
                {
                    w.Write("//");
                    w.Write(idx);
                }
                w.WriteLine();
            }
            w.Close();
            fs.Close();
        }
        public void exportSTL(string filename, bool binary)
        {
            FileStream fs = File.Open(filename, FileMode.Create);
            if (binary)
            {
                BinaryWriter w = new BinaryWriter(fs);
                int i;
                for (i = 0; i < 20; i++) w.Write((int)0);
                w.Write(triangles.Count);
                foreach (TopoTriangle t in triangles)
                {
                    w.Write((float)t.normal.x);
                    w.Write((float)t.normal.y);
                    w.Write((float)t.normal.z);
                    for (i = 0; i < 3; i++)
                    {
                        w.Write((float)t.vertices[i].pos.x);
                        w.Write((float)t.vertices[i].pos.y);
                        w.Write((float)t.vertices[i].pos.z);
                    }
                    w.Write((short)0);
                }
                w.Close();
            }
            else
            {
                TextWriter w = new EnglishStreamWriter(fs);
                w.WriteLine("solid RepetierHost");
                foreach (TopoTriangle t in triangles)
                {
                    w.Write("  facet normal ");
                    w.Write(t.normal.x);
                    w.Write(" ");
                    w.Write(t.normal.y);
                    w.Write(" ");
                    w.WriteLine(t.normal.z);
                    w.WriteLine("    outer loop");
                    w.Write("      vertex ");
                    w.Write(t.vertices[0].pos.x);
                    w.Write(" ");
                    w.Write(t.vertices[0].pos.y);
                    w.Write(" ");
                    w.WriteLine(t.vertices[0].pos.z);
                    w.Write("      vertex ");
                    w.Write(t.vertices[1].pos.x);
                    w.Write(" ");
                    w.Write(t.vertices[1].pos.y);
                    w.Write(" ");
                    w.WriteLine(t.vertices[1].pos.z);
                    w.Write("      vertex ");
                    w.Write(t.vertices[2].pos.x);
                    w.Write(" ");
                    w.Write(t.vertices[2].pos.y);
                    w.Write(" ");
                    w.WriteLine(t.vertices[2].pos.z);
                    w.WriteLine("    endloop");
                    w.WriteLine("  endfacet");
                }
                w.WriteLine("endsolid RepetierHost");
                w.Close();
            }
            fs.Close();
        }
        private RHVector3 extractVector(string s)
        {
            RHVector3 v = new RHVector3(0,0,0);
            s = s.Trim().Replace("  ", " ");
            int p = s.IndexOf(' ');
            if (p < 0) throw new Exception("Format error");
            double.TryParse(s.Substring(0, p), NumberStyles.Float, GCode.format, out v.x);
            s = s.Substring(p).Trim();
            p = s.IndexOf(' ');
            if (p < 0) throw new Exception("Format error");
            double.TryParse(s.Substring(0, p), NumberStyles.Float, GCode.format, out v.y);
            s = s.Substring(p).Trim();
            double.TryParse(s, NumberStyles.Float, GCode.format, out v.z);
            return v;
        }
        private void ReadArray(Stream stream, byte[] data)
        {
            int offset = 0;
            int remaining = data.Length;
            while (remaining > 0)
            {
                int read = stream.Read(data, offset, remaining);
                if (read <= 0)
                    throw new EndOfStreamException
                        (String.Format("End of stream reached with {0} bytes left to read", remaining));
                remaining -= read;
                offset += read;
            }
        }
        public bool importObj(string filename)
        {
            clear();
            bool error = false;
            try
            {
                string[] text = System.IO.File.ReadAllText(filename).Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                int vPos = 0;
                int vnPos = 0;
                int count = 0,countMax = text.Length;
                List<TopoVertex> vList = new List<TopoVertex>();
                List<RHVector3> vnList = new List<RHVector3>();
                foreach (string currentLine in text)
                {
                    count++;
                    Progress((double)count / countMax);
                    if (count % 2000 == 0)
                    {
                        Application.DoEvents();
                        if (IsActionStopped()) return true;
                    }
                    string line = currentLine.Trim().ToLower();
                    if (line.Length == 0 || line.StartsWith("#")) continue;
                    int p = line.IndexOf(" ");
                    if (p < 0) continue;
                    string cmd = line.Substring(0, p);
                    if (cmd == "v")
                    {
                        RHVector3 vert = extractVector(line.Substring(p + 1));
                        vList.Add(addVertex(vert));
                        vPos++;
                    }
                    else if (cmd == "vn")
                    {
                        RHVector3 vert = extractVector(line.Substring(p + 1));
                        vnList.Add(vert);
                        vnPos++;
                    }
                    else if (cmd == "f")
                    {
                        line = line.Substring(p + 1).Replace("  ", " ");
                        string[] parts = line.Split(new char[] { ' ' });
                        int[] vidx = new int[parts.Length];
                        int[] nidx = new int[parts.Length];
                        RHVector3 norm = null;
                        p = 0;
                        foreach (string part in parts)
                        {
                            string[] sp = part.Split('/');
                            if (sp.Length > 0)
                            {
                                int.TryParse(sp[0], out vidx[p]);
                                if (vidx[p] < 0) vidx[p] += vPos;
                                else vidx[p]--;
                                if (vidx[p] < 0 || vidx[p] >= vPos)
                                {
                                    error = true;
                                    break;
                                }
                            }
                            if (sp.Length > 2 && norm == null)
                            {
                                int.TryParse(sp[0], out nidx[p]);
                                if (nidx[p] < 0) nidx[p] += vnPos;
                                else nidx[p]--;
                                if (nidx[p] >= 0 && nidx[p] < vnPos)
                                    norm = vnList[nidx[p]];
                            }
                            p++;
                        }
                        for (int i = 2; i < parts.Length; i++)
                        {
                            TopoTriangle triangle = new TopoTriangle(this, vList[vidx[0]], vList[vidx[1]], vList[vidx[2]]);
                            if (norm != null && norm.ScalarProduct(triangle.normal) < 0)
                                triangle.FlipDirection();
                            AddTriangle(triangle);
                        }
                    }
                    if (error) break;
                }
            }
            catch
            {
                error = true;
            }
            if (error)
            {
                clear();
            }
            return error;
        }
        private void importSTLAscii(string filename)
        {
            string text = System.IO.File.ReadAllText(filename);
            int lastP = 0, p, pend, normal, outer, vertex, vertex2;
            int count = 0,max = text.Length;
            while ((p = text.IndexOf("facet", lastP)) > 0)
            {
                count++;
                Progress((double)lastP / max);
                if (count % 2000 == 0)
                {
                    Application.DoEvents();
                    if (IsActionStopped()) return;
                }

                pend = text.IndexOf("endfacet", p + 5);
                normal = text.IndexOf("normal", p) + 6;
                outer = text.IndexOf("outer loop", normal);
                RHVector3 normalVect = extractVector(text.Substring(normal, outer - normal));
                normalVect.NormalizeSafe();
                outer += 10;
                vertex = text.IndexOf("vertex", outer) + 6;
                vertex2 = text.IndexOf("vertex", vertex);
                RHVector3 p1 = extractVector(text.Substring(vertex, vertex2 - vertex));
                vertex2 += 7;
                vertex = text.IndexOf("vertex", vertex2);
                RHVector3 p2 = extractVector(text.Substring(vertex2, vertex - vertex2));
                vertex += 7;
                vertex2 = text.IndexOf("endloop", vertex);
                RHVector3 p3 = extractVector(text.Substring(vertex, vertex2 - vertex));
                lastP = pend + 8;
                addTriangle(p1, p2, p3, normalVect);
            }
        }
        public void importSTL(string filename)
        {
            StartAction("L_LOADING...");
            clear();
            try
            {
                FileStream f = File.OpenRead(filename);
                byte[] header = new byte[80];
                ReadArray(f, header);
                /*   if (header[0] == 's' && header[1] == 'o' && header[2] == 'l' && header[3] == 'i' && header[4] == 'd')
                   {
                       f.Close();
                       LoadText(file);
                   }
                   else
                   {*/
                BinaryReader r = new BinaryReader(f);
                int nTri = r.ReadInt32();
                if (f.Length != 84 + nTri * 50)
                {
                    f.Close();
                    importSTLAscii(filename);
                }
                else
                {
                    for (int i = 0; i < nTri; i++)
                    {
                        Progress((double)i / nTri);
                        if (i % 2000 == 0)
                        {
                            Application.DoEvents();
                            if(IsActionStopped()) return;
                        }
                        RHVector3 normal = new RHVector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
                        RHVector3 p1 = new RHVector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
                        RHVector3 p2 = new RHVector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
                        RHVector3 p3 = new RHVector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
                        normal.NormalizeSafe();
                        addTriangle(p1, p2, p3, normal);
                        r.ReadUInt16();
                    }
                    r.Close();
                    f.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error reading STL file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
