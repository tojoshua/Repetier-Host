using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace RepetierHost.model.geom
{
    public class SubmeshEdge
    {
        public int vertex1, vertex2;
        public int color;
        public SubmeshEdge(int v1, int v2, int col)
        {
            vertex1 = v1;
            vertex2 = v2;
            color = col;
        }
    }
    public class SubmeshTriangle
    {
        public int vertex1, vertex2, vertex3;
        public int color;
        public SubmeshTriangle(int v1, int v2, int v3, int col)
        {
            vertex1 = v1;
            vertex2 = v2;
            vertex3 = v3;
            color = col;
        }
        public void Normal(Submesh mesh, out float nx, out float ny, out float nz)
        {
            Vector3 v0 = mesh.vertices[vertex1];
            Vector3 v1 = mesh.vertices[vertex2];
            Vector3 v2 = mesh.vertices[vertex3];
            float a1 = v1.X - v0.X;
            float a2 = v1.Y - v0.Y;
            float a3 = v1.Z - v0.Z;
            float b1 = v2.X - v1.X;
            float b2 = v2.Y - v1.Y;
            float b3 = v2.Z - v1.Z;
            nx = a2 * b3 - a3 * b2;
            ny = a3 * b1 - a1 * b3;
            nz = a1 * b2 - a2 * b1;
            float length = (float)Math.Sqrt(nx * nx + ny * ny + nz * nz);
            if (length == 0)
            {
                nx = ny = 0;
                nz = 1;
            }
            else
            {
                nx /= length;
                ny /= length;
                nz /= length;
            }
        }
    }
    public class Submesh
    {
        public const int MESHCOLOR_FRONTBACK = -1;
        public const int MESHCOLOR_ERRORFACE = -2;
        public const int MESHCOLOR_ERROREDGE = -3;
        public const int MESHCOLOR_OUTSIDE = -4;
        public const int MESHCOLOR_EDGE_LOOP = -5;
        public const int MESHCOLOR_CUT_EDGE = -6;
        public const int MESHCOLOR_NORMAL = -7;
        public const int MESHCOLOR_EDGE = -8;
        public const int MESHCOLOR_BACK = -9;

        public List<Vector3> vertices = new List<Vector3>();
        //public Dictionary<RHVector3, int> rhvertextMap = new Dictionary<RHVector3, int>();
        public List<SubmeshEdge> edges = new List<SubmeshEdge>();
        public List<SubmeshTriangle> triangles = new List<SubmeshTriangle>();
        public List<SubmeshTriangle> trianglesError = new List<SubmeshTriangle>();
        public bool selected = false;
        public float[] glVertices = null;
        public int[] glColors = null;
        public int[] glEdges = null;
        public int[] glTriangles = null;
        public int[] glTrianglesError = null;
        public int[] glBuffer = null;
        public float[] glNormals = null;

        public void Clear()
        {
            vertices.Clear();
            //rhvertextMap.Clear();
            edges.Clear();
            triangles.Clear();
            trianglesError.Clear();
            ClearGL();
        }

        public static int ColorToRgba32(Color c)
        {
            return (int)((c.A << 24) | (c.B << 16) | (c.G << 8) | c.R);
        }

        int ConvertColorIndex(int idx)
        {
            if (idx >= 0)
                return 255 << 24 | idx;
            switch (idx)
            {
                case MESHCOLOR_FRONTBACK:
                    if (selected)
                        return ColorToRgba32(Main.threeDSettings.selectedFaces.BackColor);
                    return ColorToRgba32(Main.threeDSettings.faces.BackColor);
                case MESHCOLOR_BACK:
                    return ColorToRgba32(Main.threeDSettings.insideFaces.BackColor);
                case MESHCOLOR_ERRORFACE:
                    return ColorToRgba32(Main.threeDSettings.errorModel.BackColor);
                case MESHCOLOR_ERROREDGE:
                    return ColorToRgba32(Main.threeDSettings.errorModelEdge.BackColor);
                case MESHCOLOR_OUTSIDE:
                    return ColorToRgba32(Main.threeDSettings.outsidePrintbed.BackColor);
                case MESHCOLOR_EDGE_LOOP:
                    return ColorToRgba32(Main.threeDSettings.edges.BackColor);
                case MESHCOLOR_CUT_EDGE:
                    return ColorToRgba32(Main.threeDSettings.cutFaces.BackColor);
                case MESHCOLOR_NORMAL:
                    return ColorToRgba32(Color.Blue);
                case MESHCOLOR_EDGE:
                    return ColorToRgba32(Main.threeDSettings.edges.BackColor);
            }
            return ColorToRgba32(Color.Wheat);
        }
        /// <summary>
        /// Remove unneded temporary data
        /// </summary>
        public void Compress()
         {
            Compress(false, 0);
        }
        public void Compress(bool override_color, int color)
        {
            glVertices = new float[3 * vertices.Count];
            glNormals = new float[3 * vertices.Count];
            glColors = new int[vertices.Count];
            glEdges = new int[edges.Count * 2];
            glTriangles = new int[triangles.Count * 3];
            glTrianglesError = new int[trianglesError.Count * 3];
            UpdateDrawLists();
            UpdateColors(override_color, color);
            vertices.Clear();
        }
        public int VertexId(RHVector3 v)
        {
            //if (rhvertextMap.ContainsKey(v))
            //    return rhvertextMap[v];
            int pos = vertices.Count;
            vertices.Add(new Vector3((float)v.x, (float)v.y, (float)v.z));
            //rhvertextMap.Add(v, pos);
            return pos;
        }

        public int VertexId(double x, double y, double z)
        {
            int pos = vertices.Count;
            vertices.Add(new Vector3((float)x, (float)y, (float)z));
            return pos;
        }

        public void AddEdge(RHVector3 v1, RHVector3 v2, int color)
        {
            edges.Add(new SubmeshEdge(VertexId(v1), VertexId(v2), color));
        }

        public void AddTriangle(RHVector3 v1, RHVector3 v2, RHVector3 v3, int color)
        {
            if(color == MESHCOLOR_ERRORFACE)
                trianglesError.Add(new SubmeshTriangle(VertexId(v1), VertexId(v2), VertexId(v3), color));
            else
                triangles.Add(new SubmeshTriangle(VertexId(v1), VertexId(v2), VertexId(v3), color));
        }

        private void ClearGL()
        {
            if (glBuffer != null)
            {
                GL.DeleteBuffers(glBuffer.Length, glBuffer);
                glBuffer = null;
            }
        }
        public void UpdateColors()
        {
            UpdateColors(false, 0);
        }

        public void UpdateColors(bool override_color, int color)
        {
            foreach (SubmeshTriangle t in triangles)
            {
                if(!override_color)
                    glColors[t.vertex1] = glColors[t.vertex2] = glColors[t.vertex3] = ConvertColorIndex(t.color);
                else
                    glColors[t.vertex1] = glColors[t.vertex2] = glColors[t.vertex3] = color;
                //glColors2[t.vertex1] = glColors2[t.vertex2] = glColors2[t.vertex3] = ConvertColorIndex((t.color == Submesh.MESHCOLOR_FRONTBACK ? Submesh.MESHCOLOR_BACK : t.color));
            }
            foreach (SubmeshTriangle t in trianglesError)
            {
                if (!override_color)
                    glColors[t.vertex1] = glColors[t.vertex2] = glColors[t.vertex3] = ConvertColorIndex(t.color);
                else
                    glColors[t.vertex1] = glColors[t.vertex2] = glColors[t.vertex3] = color;
                //glColors2[t.vertex1] = glColors2[t.vertex2] = glColors2[t.vertex3] = ConvertColorIndex((t.color == Submesh.MESHCOLOR_FRONTBACK ? Submesh.MESHCOLOR_BACK : t.color));
            }
            foreach (SubmeshEdge e in edges)
            {
                if (!override_color)
                    glColors[e.vertex1] = glColors[e.vertex2] = ConvertColorIndex(e.color);
                else
                    glColors[e.vertex1] = glColors[e.vertex2] = color;
            }
            if (glBuffer != null)
            {
                // Bind current context to Array Buffer ID
                GL.BindBuffer(BufferTarget.ArrayBuffer, glBuffer[2]);
                // Send data to buffer
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(glColors.Length * sizeof(int)), glColors, BufferUsageHint.StaticDraw);
                // Validate that the buffer is the correct size
                int bufferSize;
                GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
                if (glColors.Length * sizeof(int) != bufferSize)
                    throw new ApplicationException("Vertex array not uploaded correctly");
                // Clear the buffer Binding
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }
        }
        public void UpdateDrawLists()
        {
            int idx = 0;
            foreach (SubmeshTriangle t in triangles)
            {
                int n1 = 3 * t.vertex1;
                int n2 = 3 * t.vertex2;
                int n3 = 3 * t.vertex3;
                Vector3 v1 = vertices[t.vertex1];
                Vector3 v2 = vertices[t.vertex2];
                Vector3 v3 = vertices[t.vertex3];
                t.Normal(this, out glNormals[n1], out glNormals[n1 + 1], out glNormals[n1 + 2]);
                glNormals[n2] = glNormals[n3] = glNormals[n1];
                glNormals[n2 + 1] = glNormals[n3 + 1] = glNormals[n1 + 1];
                glNormals[n2 + 2] = glNormals[n3 + 2] = glNormals[n1 + 2];
                glVertices[n1++] = v1.X;
                glVertices[n1++] = v1.Y;
                glVertices[n1] = v1.Z;
                glVertices[n2++] = v2.X;
                glVertices[n2++] = v2.Y;
                glVertices[n2] = v2.Z;
                glVertices[n3++] = v3.X;
                glVertices[n3++] = v3.Y;
                glVertices[n3] = v3.Z;
                glTriangles[idx++] = t.vertex1;
                glTriangles[idx++] = t.vertex2;
                glTriangles[idx++] = t.vertex3;
            }
            idx = 0;
            foreach (SubmeshTriangle t in trianglesError)
            {
                int n1 = 3 * t.vertex1;
                int n2 = 3 * t.vertex2;
                int n3 = 3 * t.vertex3;
                Vector3 v1 = vertices[t.vertex1];
                Vector3 v2 = vertices[t.vertex2];
                Vector3 v3 = vertices[t.vertex3];
                t.Normal(this, out glNormals[n1], out glNormals[n1 + 1], out glNormals[n1 + 2]);
                glNormals[n2] = glNormals[n3] = glNormals[n1];
                glNormals[n2 + 1] = glNormals[n3 + 1] = glNormals[n1 + 1];
                glNormals[n2 + 2] = glNormals[n3 + 2] = glNormals[n1 + 2];
                glVertices[n1++] = v1.X;
                glVertices[n1++] = v1.Y;
                glVertices[n1] = v1.Z;
                glVertices[n2++] = v2.X;
                glVertices[n2++] = v2.Y;
                glVertices[n2] = v2.Z;
                glVertices[n3++] = v3.X;
                glVertices[n3++] = v3.Y;
                glVertices[n3] = v3.Z;
                glTrianglesError[idx++] = t.vertex1;
                glTrianglesError[idx++] = t.vertex2;
                glTrianglesError[idx++] = t.vertex3;
            }
            idx = 0;
            foreach (SubmeshEdge e in edges)
            {
                int n1 = 3 * e.vertex1;
                int n2 = 3 * e.vertex2;
                Vector3 v1 = vertices[e.vertex1];
                Vector3 v2 = vertices[e.vertex2];
                glNormals[n1] = glNormals[n2] = 0;
                glNormals[n1 + 1] = glNormals[n2 + 1] = 0;
                glNormals[n1 + 2] = glNormals[n2 + 2] = 1;
                glVertices[n1++] = v1.X;
                glVertices[n1++] = v1.Y;
                glVertices[n1] = v1.Z;
                glVertices[n2++] = v2.X;
                glVertices[n2++] = v2.Y;
                glVertices[n2] = v2.Z;
                glEdges[idx++] = e.vertex1;
                glEdges[idx++] = e.vertex2;
            }
        }
        public OpenTK.Graphics.Color4 convertColor(Color col)
        {
            return new OpenTK.Graphics.Color4(col.R, col.G, col.B, col.A);
        }
        public void Draw(int method,Vector3 edgetrans,bool forceFaces=false)
        {
            GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
            GL.Material(MaterialFace.Back, MaterialParameter.AmbientAndDiffuse, convertColor(Main.threeDSettings.insideFaces.BackColor));
            //GL.Material(MaterialFace.Back, MaterialParameter.AmbientAndDiffuse, new float[] { 0.0f, 0.0f, 1.0f, 1.0f });
            //    GL.Material(MaterialFace.Front, MaterialParameter.AmbientAndDiffuse, convertColor(col));
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, new OpenTK.Graphics.Color4(0, 0, 0, 0));
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, 50f);
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.Normalize);
            GL.LineWidth(1f);
            GL.DepthFunc(DepthFunction.Less);
            if (method == 2)
            {
                if (glBuffer == null)
                {
                    glBuffer = new int[6];
                    GL.GenBuffers(6, glBuffer);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, glBuffer[0]);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(glVertices.Length * sizeof(float)), glVertices, BufferUsageHint.StaticDraw);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, glBuffer[1]);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(glNormals.Length * sizeof(float)), glNormals, BufferUsageHint.StaticDraw);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, glBuffer[2]);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(glColors.Length * sizeof(int)), glColors, BufferUsageHint.StaticDraw);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, glBuffer[3]);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(glTriangles.Length * sizeof(int)), glTriangles, BufferUsageHint.StaticDraw);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, glBuffer[4]);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(glEdges.Length * sizeof(int)), glEdges, BufferUsageHint.StaticDraw);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, glBuffer[5]);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(glTrianglesError.Length * sizeof(int)), glTrianglesError, BufferUsageHint.StaticDraw);
                }
                GL.BindBuffer(BufferTarget.ArrayBuffer, glBuffer[0]);
                GL.VertexPointer(3, VertexPointerType.Float, 0, 0);
                GL.EnableClientState(ArrayCap.VertexArray);
                GL.BindBuffer(BufferTarget.ArrayBuffer, glBuffer[1]);
                GL.NormalPointer(NormalPointerType.Float, 0, 0);
                GL.EnableClientState(ArrayCap.NormalArray);
                GL.BindBuffer(BufferTarget.ArrayBuffer, glBuffer[2]);
                GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.AmbientAndDiffuse);
                GL.ColorPointer(4, ColorPointerType.UnsignedByte, sizeof(int), IntPtr.Zero);
                GL.EnableClientState(ArrayCap.ColorArray);
                GL.Enable(EnableCap.ColorMaterial);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, glBuffer[3]);
                if (Main.threeDSettings.ShowFaces || forceFaces)
                    GL.DrawElements(BeginMode.Triangles, glTriangles.Length, DrawElementsType.UnsignedInt, 0);
                GL.LightModel(LightModelParameter.LightModelTwoSide, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, glBuffer[5]);
                GL.DrawElements(BeginMode.Triangles, glTrianglesError.Length, DrawElementsType.UnsignedInt, 0);
                GL.Disable(EnableCap.ColorMaterial);
                GL.DisableClientState(ArrayCap.NormalArray);
                GL.Disable(EnableCap.Lighting);
                GL.DepthFunc(DepthFunction.Lequal);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, glBuffer[4]);
                GL.PushMatrix();
                GL.Translate(edgetrans);
                GL.DrawElements(BeginMode.Lines, glEdges.Length, DrawElementsType.UnsignedInt, 0);
                GL.PopMatrix();
                GL.Enable(EnableCap.Lighting);
                GL.DisableClientState(ArrayCap.ColorArray);
                GL.DisableClientState(ArrayCap.VertexArray);
                GL.DisableClientState(ArrayCap.ColorArray);
                GL.Disable(EnableCap.ColorMaterial);
            }
            else if (method == 1)
            {
                GL.EnableClientState(ArrayCap.VertexArray);
                GL.VertexPointer(3, VertexPointerType.Float, 0, glVertices);
                GL.EnableClientState(ArrayCap.NormalArray);
                GL.NormalPointer(NormalPointerType.Float, 0, glNormals);
                GL.EnableClientState(ArrayCap.ColorArray);
                GL.Enable(EnableCap.ColorMaterial);
                GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.AmbientAndDiffuse);
                GL.ColorPointer(4, ColorPointerType.UnsignedByte, 0, glColors);
                GL.EnableClientState(ArrayCap.ColorArray);
                if (Main.threeDSettings.ShowFaces || forceFaces)
                    GL.DrawElements(BeginMode.Triangles, glTriangles.Length, DrawElementsType.UnsignedInt, glTriangles);
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.AmbientAndDiffuse, convertColor(Main.threeDSettings.errorModel.BackColor));
                GL.LightModel(LightModelParameter.LightModelTwoSide, 0);
                GL.DrawElements(BeginMode.Triangles, glTrianglesError.Length, DrawElementsType.UnsignedInt, glTrianglesError);
                GL.DepthFunc(DepthFunction.Lequal);
                GL.Disable(EnableCap.Lighting);
                GL.PushMatrix();
                GL.Translate(edgetrans);
                GL.DrawElements(BeginMode.Lines, glEdges.Length, DrawElementsType.UnsignedInt, glEdges);
                GL.PopMatrix();
                GL.Enable(EnableCap.Lighting);
                GL.Disable(EnableCap.ColorMaterial);
                GL.DisableClientState(ArrayCap.ColorArray);
                GL.DisableClientState(ArrayCap.VertexArray);
                GL.DisableClientState(ArrayCap.NormalArray);

            }
            else if (method == 0)
            {
                int n;
                GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.AmbientAndDiffuse);
                GL.Enable(EnableCap.ColorMaterial);
                if (Main.threeDSettings.ShowFaces || forceFaces)
                {
                    GL.Begin(BeginMode.Triangles);
                    n = glTriangles.Length;
                    for (int i = 0; i < n; i++)
                    {
                        int p = glTriangles[i] * 3;
                        int col = glColors[glTriangles[i]];
                        GL.Color4(new OpenTK.Graphics.Color4((byte)(col & 255), (byte)((col >> 8) & 255), (byte)((col >> 16) & 255), (byte)(col >> 24)));
                        GL.Normal3(glNormals[p], glNormals[p + 1], glNormals[p + 2]);
                        GL.Vertex3(glVertices[p], glVertices[p + 1], glVertices[p + 2]);
                    }
                    GL.End();
                }
                GL.LightModel(LightModelParameter.LightModelTwoSide, 0);
                GL.Begin(BeginMode.Triangles);
                n = glTrianglesError.Length;
                for (int i = 0; i < n; i++)
                {
                    int p = glTrianglesError[i] * 3;
                    int col = glColors[glTrianglesError[i]];
                    GL.Color4(new OpenTK.Graphics.Color4((byte)(col & 255), (byte)((col >> 8) & 255), (byte)((col >> 16) & 255), (byte)(col >> 24)));
                    GL.Normal3(glNormals[p], glNormals[p + 1], glNormals[p + 2]);
                    GL.Vertex3(glVertices[p], glVertices[p + 1], glVertices[p + 2]);
                }
                GL.End();

                GL.Disable(EnableCap.Lighting);
                n = glEdges.Length;
                GL.PushMatrix();
                GL.Translate(edgetrans);
                GL.Begin(BeginMode.Lines);
                for (int i = 0; i < n; i++)
                {
                    int p = glEdges[i] * 3;
                    int col = glColors[glEdges[i]];
                    GL.Color4(new OpenTK.Graphics.Color4((byte)(col & 255), (byte)((col >> 8) & 255), (byte)((col >> 16) & 255), (byte)(col >> 24)));
                    GL.Normal3(glNormals[p], glNormals[p + 1], glNormals[p + 2]);
                    GL.Vertex3(glVertices[p], glVertices[p + 1], glVertices[p + 2]);
                }
                GL.End();
                GL.PopMatrix();
                GL.Enable(EnableCap.Lighting);
                GL.Disable(EnableCap.ColorMaterial);
            }
            GL.LightModel(LightModelParameter.LightModelTwoSide, 0);
            GL.Disable(EnableCap.Normalize);
        }
    }
}
