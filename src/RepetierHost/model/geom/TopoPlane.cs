using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepetierHost.model.geom
{
    public class TopoPlane
    {
        double d;
        RHVector3 normal;
        public TopoPlane(RHVector3 _normal, RHVector3 pointOnPlane)
        {
            normal = new RHVector3(_normal);
            normal.NormalizeSafe();
            d = pointOnPlane.ScalarProduct(normal);
        }
        public double VertexDistance(RHVector3 v)
        {
            return v.x * normal.x + v.y * normal.y + v.z * normal.z - d;
        }
        public int testTriangleSide(TopoTriangle triangle)
        {
            double d1 = VertexDistance(triangle.vertices[0].pos);
            double d2 = VertexDistance(triangle.vertices[1].pos);
            double d3 = VertexDistance(triangle.vertices[2].pos);
            if (d1 >= 0 && d2 >= 0 && d3 >= 0) return 1;
            if (d1 <= 0 && d2 <= 0 && d3 <= 0) return -1;
            return 0;
        }
        public int testTriangleSideFast(TopoTriangle triangle)
        {
             
            bool d1 = VertexDistance(triangle.vertices[0].pos)>0;
            bool d2 = VertexDistance(triangle.vertices[1].pos)>0;
            bool d3 = VertexDistance(triangle.vertices[2].pos)>0;
            if (d1 && d2 && d3) return 1;
            if (d1 == d2 && d2 == d3) return -1;
            return 0;
        }
        public void addIntersectionToSubmesh(Submesh mesh, TopoTriangle triangle, bool addEdges,int color)
        {
            int[] outside = new int[3];
            int[] inside = new int[3];
            int nOutside = 0, nInside = 0;
            int i;
            for (i = 0; i < 3; i++)
            {
                if (VertexDistance(triangle.vertices[i].pos) > 0)
                    outside[nOutside++] = i;
                else
                    inside[nInside++] = i;
            }
            if (nOutside != 1 && nOutside != 2) return;
            RHVector3[] intersections = new RHVector3[3];
            int nIntersections = 0;
            for (int iInside = 0; iInside < nInside; iInside++)
            {
                for (int iOutside = 0; iOutside < nOutside; iOutside++)
                {
                    RHVector3 v1 = triangle.vertices[inside[iInside]].pos;
                    RHVector3 v2 = triangle.vertices[outside[iOutside]].pos;
                    double dist1 = VertexDistance(v1);
                    double dist2 = VertexDistance(v2);
                    double pos = Math.Abs(dist1) / Math.Abs(dist2 - dist1);
                    intersections[nIntersections++] = new RHVector3(
                        v1.x+pos*(v2.x-v1.x),
                        v1.y+pos*(v2.y-v1.y),
                        v1.z+pos*(v2.z-v1.z)
                        );
                }
            }
            if (nInside == 2)
            {
                if (outside[0] == 1)
                {
                    mesh.AddTriangle(triangle.vertices[inside[1]].pos, triangle.vertices[inside[0]].pos, intersections[1], color);
                    mesh.AddTriangle(triangle.vertices[inside[0]].pos, intersections[0], intersections[1], color);
                }
                else
                {
                    mesh.AddTriangle(triangle.vertices[inside[0]].pos, triangle.vertices[inside[1]].pos, intersections[0], color);
                    mesh.AddTriangle(triangle.vertices[inside[1]].pos, intersections[1], intersections[0], color);
                }
            }
            else
            {
                if(inside[0] == 1)
                    mesh.AddTriangle(triangle.vertices[inside[0]].pos, intersections[1], intersections[0], color);
                else
                    mesh.AddTriangle(triangle.vertices[inside[0]].pos, intersections[0], intersections[1], color);
            }
            if (addEdges)
            {
                if (nInside == 2)
                {
                    mesh.AddEdge(triangle.vertices[inside[0]].pos, triangle.vertices[inside[1]].pos, triangle.edges[0].connectedFaces == 2 ? Submesh.MESHCOLOR_EDGE : Submesh.MESHCOLOR_ERROREDGE);
                    mesh.AddEdge(triangle.vertices[inside[0]].pos, intersections[0], triangle.edges[1].connectedFaces == 2 ? Submesh.MESHCOLOR_EDGE : Submesh.MESHCOLOR_ERROREDGE);
                    mesh.AddEdge(triangle.vertices[inside[1]].pos, intersections[1], triangle.edges[2].connectedFaces == 2 ? Submesh.MESHCOLOR_EDGE : Submesh.MESHCOLOR_ERROREDGE);
                }
                else
                {
                    for (int iInter = 0; iInter < nIntersections; iInter++)
                    {
                        mesh.AddEdge(triangle.vertices[inside[0]].pos, intersections[iInter], triangle.edges[(inside[0]+2*iInter) % 3].connectedFaces == 2 ? Submesh.MESHCOLOR_EDGE : Submesh.MESHCOLOR_ERROREDGE);
                    }
                }
            }
            if (nIntersections == 2)
                mesh.AddEdge(intersections[0], intersections[1], Submesh.MESHCOLOR_CUT_EDGE);
        }
    }
}
