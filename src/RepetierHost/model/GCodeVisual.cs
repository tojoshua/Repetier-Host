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
using OpenTK;
//using OpenTK.Graphics;
using OpenTK.Platform;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using RepetierHost.view;

namespace RepetierHost.model
{
    public class GCodePoint
    {
        public float e;
        public float dist;
        public Vector3 p;
        public int fline; // fileid+4*line
        public int element; // posistion of the opengl element where visualization starts
        public static int toFileLine(int file, int line) { if (file < 0) return 0; return (file << 29) + line; }
    }
    public class GCodeTravel
    {
        public Vector3 p1;
        public Vector3 p2;
        public int fline;
    }
    public class GCodePath
    {
        public static bool correctNorms = true; // Draw correct normals
        public int pointsCount = 0;
        public int drawMethod = -1;
        public float[] positions = null;
        public float[] normals = null;
        public int[] elements = null;
        public int[] buf = new int[3];
        public bool hasBuf = false;
        public int elementsLength;
        public LinkedList<LinkedList<GCodePoint>> pointsLists = new LinkedList<LinkedList<GCodePoint>>();
        public void Add(Vector3 v, float e, float d, int fline)
        {
            if (pointsLists.Count == 0)
                pointsLists.AddLast(new LinkedList<GCodePoint>());
            GCodePoint pt = new GCodePoint();
            pt.p = v;
            pt.e = e;
            pt.dist = d;
            pt.fline = fline;
            pointsCount++;
            pointsLists.Last.Value.AddLast(pt);
            drawMethod = -1; // invalidate old 
        }
        public float lastDist
        {
            get { return pointsLists.Last.Value.Last.Value.dist; }
        }
        public void Join(GCodePath path)
        {
            foreach (LinkedList<GCodePoint> frag in path.pointsLists)
            {
                pointsLists.AddLast(frag);
            }
            pointsCount += path.pointsCount;
            if (elements != null && path.elements != null)
            {
                if (/*normals!=null && */path.elements != null && drawMethod == path.drawMethod) // both parts are already up to date, so just join them
                {
                    int[] newelements = new int[elementsLength + path.elementsLength];
                    int p, l = elementsLength, i;
                    for (p = 0; p < l; p++) newelements[p] = elements[p];
                    int[] pe = path.elements;
                    l = path.elementsLength;
                    int pointsold = positions.Length / 3;
                    for (i = 0; i < l; i++) newelements[p++] = pe[i] + pointsold;
                    elements = newelements;
                    elementsLength = elements.Length;
                    float[] newnormals = null;
                    if (normals != null) newnormals = new float[normals.Length + path.normals.Length];
                    float[] newpoints = new float[positions.Length + path.positions.Length];
                    if (normals != null)
                    {
                        l = normals.Length;
                        for (p = 0; p < l; p++)
                        {
                            newnormals[p] = normals[p];
                            newpoints[p] = positions[p];
                        }
                        float[] pn = path.normals;
                        float[] pp = path.positions;
                        l = pp.Length;
                        for (i = 0; i < l; i++)
                        {
                            newnormals[p] = pn[i];
                            newpoints[p++] = pp[i];
                        }
                        normals = newnormals;
                        positions = newpoints;
                    }
                    else
                    {
                        l = positions.Length;
                        for (p = 0; p < l; p++)
                        {
                            newpoints[p] = positions[p];
                        }
                        float[] pp = path.positions;
                        l = pp.Length;
                        for (i = 0; i < l; i++)
                        {
                            newpoints[p++] = pp[i];
                        }
                        positions = newpoints;
                    }
                }
                else
                {
                    elements = null;
                    normals = null;
                    positions = null;
                    drawMethod = -1;
                }
                if (hasBuf)
                {
                    GL.DeleteBuffers(3, buf);
                    hasBuf = false;
                }
            }
            else
            {
                drawMethod = -1;
            }
        }
        public void Dispose(bool disposing)
        {
            Free();
        }
        public void Free()
        {
            if (elements != null)
            {
                elements = null;
                normals = null;
                positions = null;
                pointsLists.Clear();
                if (hasBuf)
                    GL.DeleteBuffers(3, buf);
                hasBuf = false;
            }
        }
        /// <summary>
        /// Refill VBOs with current values of elements etc.
        /// </summary>
        public void RefillVBO()
        {
            if (positions == null) return;
            if (hasBuf)
                GL.DeleteBuffers(3, buf);
            GL.GenBuffers(3, buf);
            GL.BindBuffer(BufferTarget.ArrayBuffer, buf[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(positions.Length * sizeof(float)), positions, BufferUsageHint.StaticDraw);
            if (normals != null)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, buf[1]);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(normals.Length * sizeof(float)), normals, BufferUsageHint.StaticDraw);
            }
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buf[2]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(elementsLength * sizeof(int)), elements, BufferUsageHint.StaticDraw);
            hasBuf = true;
        }
        public void UpdateVBO(bool buffer)
        {
            if (pointsCount < 2) return;
            if (hasBuf)
                GL.DeleteBuffers(3, buf);
            hasBuf = false;
            int method = Main.threeDSettings.filamentVisualization;
            float h = Main.threeDSettings.layerHeight;
            float wfac = Main.threeDSettings.widthOverHeight;
            float w = h * wfac;
            bool fixedH = Main.threeDSettings.useLayerHeight;
            float dfac = (float)(Math.PI * Main.threeDSettings.filamentDiameter * Main.threeDSettings.filamentDiameter * 0.25 / wfac);
            int nv = 8 * (method - 1), i;
            if (method == 1) nv = 4;
            if (method == 0) nv = 1;
            int n = nv * (method == 0 ? 1 : 2) * (pointsCount - pointsLists.Count);
            //if (method != 0) positions = new float[n * 3]; else positions = new float[3 * pointsCount];
            //if (method != 0) normals = new float[n * 3]; else normals = null;
            if (method != 0) positions = new float[pointsCount * nv * 3 * (correctNorms ? 2 : 1)]; else positions = new float[3 * pointsCount];
            if (method != 0) normals = new float[pointsCount * nv * 3 * (correctNorms ? 2 : 1)]; else normals = null;
            if (method != 0) elements = new int[(pointsCount - pointsLists.Count) * nv * 4 + pointsLists.Count * (nv - 2) * 4]; else elements = new int[n * 2];
            int pos = 0;
            int npos = 0;
            int vpos = 0;
            if (method > 0)
            {
                float alpha, dalpha = (float)Math.PI * 2f / nv;
                float[] dir = new float[3];
                float[] dirs = new float[3];
                float[] diru = new float[3];
                float[] actdirs = new float[3];
                float[] actdiru = new float[3];
                float[] norm = new float[3];
                float[] lastdir = new float[3];
                float[] actdir = new float[3];
                diru[0] = diru[1] = 0;
                diru[2] = 1;
                actdiru[0] = actdiru[1] = 0;
                actdiru[2] = 1;
                float laste = 0;
                float dh = 0.5f * h;
                float dw = 0.5f * w;
                bool first = true;
                float deltae;
                Vector3 last = new Vector3();
                w *= 0.5f;
                int nv2 = 2 * nv;
                foreach (LinkedList<GCodePoint> points in pointsLists)
                {
                    if (points.Count < 2)
                        continue;
                    first = true;
                    LinkedListNode<GCodePoint> ptNode = points.First;
                    while (ptNode != null)
                    {
                        GCodePoint pt = ptNode.Value;
                        pt.element = pos;
                        GCodePoint ptn = null;
                        if (ptNode.Next != null)
                            ptn = ptNode.Next.Value;
                        ptNode = ptNode.Next;
                        Vector3 v = pt.p;
                        if (first)
                        {
                            last = v;
                            laste = pt.e;
                            lastdir[0] = actdir[0] = ptn.p.X - v.X;
                            lastdir[1] = actdir[1] = ptn.p.Y - v.Y;
                            lastdir[2] = actdir[2] = ptn.p.Z - v.Z;
                            deltae = ptn.e - pt.e;
                            GCodeVisual.normalize(ref lastdir);
                            // first = false;
                            // continue;
                        }
                        else
                        {
                            bool isLast = pt == points.Last.Value;
                            if (isLast)
                            {
                                actdir[0] = v.X - last.X;
                                actdir[1] = v.Y - last.Y;
                                actdir[2] = v.Z - last.Z;
                                deltae = pt.e - laste;
                            }
                            else
                            {
                                actdir[0] = ptn.p.X - v.X;
                                actdir[1] = ptn.p.Y - v.Y;
                                actdir[2] = ptn.p.Z - v.Z;
                                deltae = ptn.e - pt.e;
                            }
                        }
                        if (!fixedH)
                        {
                            float dist = (float)Math.Sqrt(actdir[0] * actdir[0] + actdir[1] * actdir[1] + actdir[2] * actdir[2]);
                            if (dist > 0)
                            {
                                h = (float)Math.Sqrt(deltae * dfac / dist);
                                w = h * wfac;
                                dh = 0.5f * h;
                                dw = 0.5f * w;
                            }
                        }
                        GCodeVisual.normalize(ref actdir);
                        dir[0] = actdir[0] + lastdir[0];
                        dir[1] = actdir[1] + lastdir[1];
                        dir[2] = actdir[2] + lastdir[2];
                        GCodeVisual.normalize(ref dir);
                        double vacos = dir[0] * lastdir[0] + dir[1] * lastdir[1] + dir[2] * lastdir[2];
                        if (vacos > 1) vacos = 1;
                        if (vacos < 0.3)
                            vacos = 0.3;
                        float zoomw = (float)vacos; // Math.Cos(Math.Acos(vacos));
                        lastdir[0] = actdir[0];
                        lastdir[1] = actdir[1];
                        lastdir[2] = actdir[2];
                        dirs[0] = -dir[1];
                        dirs[1] = dir[0];
                        dirs[2] = dir[2];
                        actdirs[0] = -actdir[1];
                        actdirs[1] = actdir[0];
                        actdirs[2] = actdir[2];
                        alpha = 0;
                        float c, s;
                        int b = vpos / 3 - nv*(correctNorms ? 2 : 1);
                        for (i = 0; i < nv; i++)
                        {
                            c = (float)Math.Cos(alpha) * dh;
                            s = (float)Math.Sin(alpha) * dw / zoomw;
                            if (correctNorms)
                            {
                                float s2 = (float)Math.Sin(alpha) * dw;
                                norm[0] = (float)(s2 * actdirs[0] + c * actdiru[0]);
                                norm[1] = (float)(s2 * actdirs[1] + c * actdiru[1]);
                                norm[2] = (float)(s2 * actdirs[2] + c * actdiru[2]);
                            }
                            else
                            {
                                norm[0] = (float)(s * dirs[0] + c * diru[0]);
                                norm[1] = (float)(s * dirs[1] + c * diru[1]);
                                norm[2] = (float)(s * dirs[2] + c * diru[2]);
                            }
                            GCodeVisual.normalize(ref norm);
                            if (!first)
                            {
                                if (correctNorms)
                                {
                                    elements[pos++] = b + 2*((i + 1) % nv)+1;
                                    elements[pos++] = b + 2*i+1;
                                    elements[pos++] = b + 2*(i + nv);
                                    elements[pos++] = b + 2*((i + 1) % nv + nv);
                                }
                                else
                                {
                                    elements[pos++] = b + (i + 1) % nv;
                                    elements[pos++] = b + i;
                                    elements[pos++] = b + i + nv;
                                    elements[pos++] = b + (i + 1) % nv + nv;
                                }
                            }
                            if (correctNorms)
                            {
                                if (first || ptNode == null)
                                {
                                    if (first)
                                    {
                                        normals[npos++] = -actdir[0];
                                        normals[npos++] = -actdir[1];
                                        normals[npos++] = -actdir[2];
                                    }
                                    else
                                    {
                                        normals[npos++] = norm[0];
                                        normals[npos++] = norm[1];
                                        normals[npos++] = norm[2];
                                    }
                                    positions[vpos++] = v.X + s * dirs[0] + c * diru[0];
                                    positions[vpos++] = v.Y + s * dirs[1] + c * diru[1];
                                    positions[vpos++] = v.Z - dh + s * dirs[2] + c * diru[2];
                                }
                                else
                                {
                                    normals[npos] = normals[npos - 6 * nv+3];
                                    normals[npos + 1] = normals[npos - 6 * nv +4];
                                    normals[npos + 2] = normals[npos - 6 * nv +5];
                                    npos += 3;
                                    positions[vpos++] = v.X + s * dirs[0] + c * diru[0];
                                    positions[vpos++] = v.Y + s * dirs[1] + c * diru[1];
                                    positions[vpos++] = v.Z - dh + s * dirs[2] + c * diru[2];
                                }
                            }
                            if (correctNorms && ptNode == null)
                            {
                                normals[npos++] = actdir[0];
                                normals[npos++] = actdir[1];
                                normals[npos++] = actdir[2];
                            }
                            else
                            {
                                normals[npos++] = norm[0];
                                normals[npos++] = norm[1];
                                normals[npos++] = norm[2];
                            }
                            positions[vpos++] = v.X + s * dirs[0] + c * diru[0];
                            positions[vpos++] = v.Y + s * dirs[1] + c * diru[1];
                            positions[vpos++] = v.Z - dh + s * dirs[2] + c * diru[2];
                            alpha += dalpha;
                        }
                        if (first || ptNode == null) // Draw cap
                        {
                            b = vpos / 3 - nv * (correctNorms ? 2 : 1);
                            int nn = (nv - 2) / 2;
                            for (i = 0; i < nn; i++)
                            {
                                if (correctNorms)
                                {
                                    if (first)
                                    {
                                        elements[pos++] = b + 2*i;
                                        elements[pos++] = b + 2*i + 2;
                                        elements[pos++] = b + 2*nv - 2*i - 4;
                                        elements[pos++] = b + 2*nv - 2*i - 2;
                                    }
                                    else
                                    {
                                        elements[pos++] = b + 2*(nv -i -1)+1;
                                        elements[pos++] = b + 2*(nv - i -2)+1;
                                        elements[pos++] = b + 2*i + 3;
                                        elements[pos++] = b + 2*i+1;
                                    }
                                }
                                else
                                {
                                    if (first)
                                    {
                                        elements[pos++] = b + i;
                                        elements[pos++] = b + i + 1;
                                        elements[pos++] = b + nv - i - 2;
                                        elements[pos++] = b + nv - i - 1;
                                    }
                                    else
                                    {
                                        elements[pos++] = b + nv - i - 1;
                                        elements[pos++] = b + nv - i - 2;
                                        elements[pos++] = b + i + 1;
                                        elements[pos++] = b + i;
                                    }
                                }
                            }
                        }
                        last = v;
                        laste = pt.e;
                        first = false;
                    }
                }
                elementsLength = pos;
                if (buffer)
                {
                    GL.GenBuffers(3, buf);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, buf[0]);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(positions.Length * sizeof(float)), positions, BufferUsageHint.StaticDraw);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, buf[1]);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(normals.Length * sizeof(float)), normals, BufferUsageHint.StaticDraw);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, buf[2]);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(elementsLength * sizeof(int)), elements, BufferUsageHint.StaticDraw);
                    // GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                    hasBuf = true;
                }
            }
            else
            {
                // Draw edges
                bool first = true;
                foreach (LinkedList<GCodePoint> points in pointsLists)
                {
                    if (points.Count < 2)
                        continue;
                    first = true;
                    foreach (GCodePoint pt in points)
                    {
                        pt.element = pos;
                        Vector3 v = pt.p;
                        positions[vpos++] = v.X;
                        positions[vpos++] = v.Y;
                        positions[vpos++] = v.Z;

                        if (!first)
                        {
                            // elements[pos] = pos / 2;
                            // elements[pos + 1] = pos / 2 + 1;
                            elements[pos] = vpos / 3 - 1;
                            elements[pos + 1] = vpos / 3 - 2;
                            pos += 2;
                        }
                        first = false;
                    }
                }
                elementsLength = pos;
                if (buffer)
                {
                    GL.GenBuffers(3, buf);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, buf[0]);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(positions.Length * sizeof(float)), positions, BufferUsageHint.StaticDraw);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, buf[2]);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(elementsLength * sizeof(int)), elements, BufferUsageHint.StaticDraw);
                    hasBuf = true;
                }
            }
            drawMethod = method;
        }
    }
    public class GCodeVisual : ThreeDModel
    {
        static int MaxExtruder = 3;
        LinkedList<GCodePath>[] segments;
        List<GCodeTravel> travelMoves = new List<GCodeTravel>();
        int[] travelBuf = new int[2];
        int travelMovesBuffered = 0;
        bool hasTravelBuf = false;
        public GCodeAnalyzer ana = new GCodeAnalyzer(true);
        public GCode act = null;
        public float lastFilHeight = 999;
        public float lastFilWidth = 999;
        public float lastFilDiameter = 999;
        public bool lastFilUseHeight = true;
        public bool lastCorrectNormals = true;
        public float laste = -999;
        public float hotFilamentLength = 1000;
        public float minHotDist = 0;
        public float totalDist = 0;
        public float[] defaultColor = new float[4];
        public float[] hotColor = new float[4];
        public float[] curColor = new float[4];
        public bool liveView = false; // Is live view of print. If true, color transion for end is shown
        private int method = 0;
        private int[] colbuf = new int[1];
        private int colbufSize = 0;
        bool recompute;
        float wfac;
        float h, w;
        bool fixedH;
        float dfac;
        float lastx = 1e20f, lasty = 0, lastz = 0;
        int lastLayer = 0;
        bool changed = false;
        public bool startOnClear = false;
        public int minLayer, maxLayer;
        int fileid = 0;
        int actLine = 0;
        public bool showSelection = false;
        public int selectionStart = 0;
        public int selectionEnd = 0;
        int lastMachineType = -1; // Type id for last draw. cnc has different visualization style!

        public GCodeVisual()
        {
            segments = new LinkedList<GCodePath>[3];
            for (int i = 0; i < MaxExtruder; i++)
                segments[i] = new LinkedList<GCodePath>();
            ana = new GCodeAnalyzer(true);
            startOnClear = true;
            ana.eventPosChanged += OnPosChange;
            ana.eventPosChangedFast += OnPosChangeFast;
        }
        public GCodeVisual(GCodeAnalyzer a)
        {
            segments = new LinkedList<GCodePath>[3];
            for (int i = 0; i < MaxExtruder; i++)
                segments[i] = new LinkedList<GCodePath>();
            ana = a;
            startOnClear = false;
            ana.eventPosChanged += OnPosChange;
            ana.eventPosChangedFast += OnPosChangeFast;
        }
        public void Dispose(bool disposing)
        {
            for (int i = 0; i < MaxExtruder; i++)
            {
                foreach (GCodePath p in segments[i])
                    p.Free();
                segments[i].Clear();
            }
            if (colbufSize > 0)
                GL.DeleteBuffers(1, colbuf);
            if (hasTravelBuf)
                GL.DeleteBuffers(2, travelBuf);
            travelMoves.Clear();
            hasTravelBuf = false;
            travelMovesBuffered = 0;
            colbufSize = 0;
        }
        public override void ReduceQuality()
        {
            if (!liveView) return;
            if (Main.threeDSettings.filamentVisualization < ana.maxDrawMethod)
                ana.maxDrawMethod = Main.threeDSettings.filamentVisualization;
            if (ana.maxDrawMethod > 0)
            {
                ana.maxDrawMethod--;
                Main.conn.log("Reduced visual quality for better framerates and to protect print quality.", false, 1);
                Main.conn.log("You can disable this in Config->Repetier settings->Behaviour.", false, 1);
            }
            else
            {
                if (ana.drawing)
                {
                    ana.drawing = false;
                    Main.conn.log("Disabled additional filament drawing for better framerates and to protect print quality.", false, 1);
                    Main.conn.log("You can disable this in Config->Repetier settings->Behaviour.", false, 1);
                }
            }
        }
        public override void ResetQuality()
        {
            ana.drawing = true;
            ana.maxDrawMethod = 10;
        }
        public void Reduce()
        {
            for (int i = 0; i < MaxExtruder; i++)
            {
                LinkedList<GCodePath> seg = segments[i];
                if (seg.Count < 2) continue;
                if (!liveView)
                {
                    GCodePath first = seg.First.Value;
                    while (seg.Count > 1)
                    {
                        first.Join(seg.First.Next.Value);
                        seg.First.Next.Value.Free();
                        seg.Remove(seg.First.Next.Value);
                    }
                }
                else
                {
                    LinkedListNode<GCodePath> act = seg.First, next;
                    while (act.Next != null)
                    {
                        next = act.Next;
                        if (next.Next == null)
                        {
                            break; // Don't touch last segment we are writing to
                        }
                        GCodePath nextval = next.Value;
                        if (nextval.pointsCount < 2)
                        {
                            act = next;
                            if (act.Next != null)
                                act = act.Next;
                        }
                        else if (nextval.lastDist > minHotDist)
                        {
                            if (act.Value.pointsCount < 500)
                            {
                                act.Value.Join(nextval);
                                seg.Remove(nextval);
                                nextval.Free();
                            }
                            else
                            {
                                act = next;
                            }
                        }
                        else
                            if (act.Value.pointsCount < 5000 || (nextval.pointsCount >= 5000 && act.Value.pointsCount < 27000))
                            {
                                act.Value.Join(nextval);
                                seg.Remove(nextval);
                                nextval.Free();
                            }
                            else
                            {
                                act = next;
                            }
                    }
                }
            }
        }
        public void stats()
        {
            int cnt = 0;
            for (int i = 0; i < MaxExtruder; i++)
                cnt += segments[i].Count;
            PrinterConnection.logInfo("Path segments:" + cnt.ToString());
            int pts = 0;
            for (int i = 0; i < MaxExtruder; i++)
                foreach (GCodePath p in segments[i])
                {
                    pts += p.pointsCount;

                }
            PrinterConnection.logInfo("Points total:" + pts.ToString());
        }
        /// <summary>
        /// Add a GCode line to be visualized.
        /// </summary>
        /// <param name="g"></param>
        public void AddGCode(GCode g)
        {
            act = g;
            ana.Analyze(g);
            laste = ana.activeExtruder.emax;
        }
        /// <summary>
        /// Remove all drawn lines.
        /// </summary>
        public override void Clear()
        {
            for (int i = 0; i < MaxExtruder; i++)
            {
                foreach (GCodePath p in segments[i])
                    p.Free();
                segments[i].Clear();
            }
            lastx = 1e20f; // Don't ignore first point if it was the last! 
            totalDist = 0;
            if (colbufSize > 0)
                GL.DeleteBuffers(1, colbuf);
            if (hasTravelBuf)
                GL.DeleteBuffers(2, travelBuf);
            hasTravelBuf = false;
            travelMoves.Clear();
            travelMovesBuffered = 0;
            colbufSize = 0;
            ResetQuality();

            if (startOnClear)
                ana.start(true);
            else
                ana.layer = 0;
        }
        void OnPosChange(GCode act, float x, float y, float z)
        {
            if (!ana.drawing)
            {
                lastx = x;
                lasty = y;
                lastz = z;
                laste = ana.activeExtruder.emax;
                return;
            }
            float locDist = (float)Math.Sqrt((x - lastx) * (x - lastx) + (y - lasty) * (y - lasty) + (z - lastz) * (z - lastz));
            bool isLastPos = locDist < 0.00001;
            if (!act.hasG || (act.G > 3 && act.G != 28)) return;
            bool isTravel = (FormPrinterSettings.ps.printerType == 3 ? Math.Max(z, lastz) - ana.zOffset >= FormPrinterSettings.ps.cncZTop : !ana.eChanged);
            int segpos = ana.activeExtruderId;
            if (segpos < 0 || segpos >= MaxExtruder) segpos = 0;
            LinkedList<GCodePath> seg = segments[segpos];
            if (isTravel)
            {
                GCodeTravel travel = new GCodeTravel();
                travel.fline = GCodePoint.toFileLine(fileid, actLine);
                travel.p1.X = lastx;
                travel.p1.Y = lasty;
                travel.p1.Z = lastz;
                travel.p2.X = x;
                travel.p2.Y = y;
                travel.p2.Z = z;
                travelMoves.Add(travel);
                if (FormPrinterSettings.ps.printerType == 3 && z - ana.zOffset < FormPrinterSettings.ps.cncZTop) // End od travel moves
                { // Start new milling move sequence
                    GCodePath p = new GCodePath();
                    p.Add(new Vector3(x, y, z), ana.activeExtruder.emax, totalDist, GCodePoint.toFileLine(fileid, actLine));
                    if (seg.Count > 0 && seg.Last.Value.pointsLists.Last.Value.Count == 1)
                    {
                        seg.RemoveLast();
                    }
                    seg.AddLast(p);
                }
            }
            if (seg.Count == 0 || (laste >= ana.activeExtruder.e && FormPrinterSettings.ps.printerType != 3)) // start new segment
            {
                if (!isLastPos) // no move, no action
                {
                    GCodePath p = new GCodePath();
                    p.Add(new Vector3(x, y, z), ana.activeExtruder.emax, totalDist, GCodePoint.toFileLine(fileid, actLine));
                    if (seg.Count > 0 && seg.Last.Value.pointsLists.Last.Value.Count == 1)
                    {
                        seg.RemoveLast();
                    }
                    seg.AddLast(p);
                    changed = true;
                }
            }
            else
            {
                if (!isLastPos && (FormPrinterSettings.ps.printerType != 3 || !isTravel))
                {
                    totalDist += locDist;
                    seg.Last.Value.Add(new Vector3(x, y, z), ana.activeExtruder.emax, totalDist, GCodePoint.toFileLine(fileid, actLine));
                    changed = true;
                }
            }
            lastx = x;
            lasty = y;
            lastz = z;
            laste = ana.activeExtruder.emax;
        }
        void OnPosChangeFast(float x, float y, float z, float e)
        {
            if (!ana.drawing || ana.layer < minLayer || ana.layer > maxLayer)
            {
                lastx = x;
                lasty = y;
                lastz = z;
                laste = ana.activeExtruder.emax;
                lastLayer = ana.layer;
                return;
            }
            float locDist = (float)Math.Sqrt((x - lastx) * (x - lastx) + (y - lasty) * (y - lasty) + (z - lastz) * (z - lastz));
            bool isLastPos = locDist < 0.00001;
            int segpos = ana.activeExtruderId;
            bool isTravel = (FormPrinterSettings.ps.printerType == 3 ? Math.Max(z,lastz) - ana.zOffset >= FormPrinterSettings.ps.cncZTop : !ana.eChanged);
            if (segpos < 0 || segpos >= MaxExtruder) segpos = 0;
            LinkedList<GCodePath> seg = segments[segpos];
            if (isTravel)
            {
                GCodeTravel travel = new GCodeTravel();
                travel.fline = GCodePoint.toFileLine(fileid, actLine);
                travel.p1.X = lastx;
                travel.p1.Y = lasty;
                travel.p1.Z = lastz;
                travel.p2.X = x;
                travel.p2.Y = y;
                travel.p2.Z = z;
                travelMoves.Add(travel);
                if (FormPrinterSettings.ps.printerType == 3 && z - ana.zOffset < FormPrinterSettings.ps.cncZTop) // End od travel moves
                { // Start new milling move sequence
                    GCodePath p = new GCodePath();
                    p.Add(new Vector3(x, y, z), ana.activeExtruder.emax, totalDist, GCodePoint.toFileLine(fileid, actLine));
                    if (seg.Count > 0 && seg.Last.Value.pointsLists.Last.Value.Count == 1)
                    {
                        seg.RemoveLast();
                    }
                    seg.AddLast(p);
                }
            }
            
            //if (!act.hasG || (act.G > 1 && act.G != 28)) return;
            if (lastLayer == minLayer - 1 && (laste < e || FormPrinterSettings.ps.printerType == 3))
            {
                GCodePath p = new GCodePath();
                p.Add(new Vector3(lastx, lasty, lastz), laste, totalDist, GCodePoint.toFileLine(fileid, actLine));

                if (seg.Count > 0 && seg.Last.Value.pointsLists.Last.Value.Count == 1)
                {
                    seg.RemoveLast();
                }
                seg.AddLast(p);
            }

            if (seg.Count == 0 || (laste >= ana.activeExtruder.e && FormPrinterSettings.ps.printerType != 3)) // start new segment
            {
                if (!isLastPos) // no move, no action
                {
                    GCodePath p = new GCodePath();
                    p.Add(new Vector3(x, y, z), ana.activeExtruder.emax, totalDist, GCodePoint.toFileLine(fileid, actLine));
                    if (seg.Count > 0 && seg.Last.Value.pointsLists.Count>0 && seg.Last.Value.pointsLists.Last.Value.Count == 1)
                    {
                        seg.RemoveLast();
                    }
                    seg.AddLast(p);
                    //changed = true;
                }
            }
            else
            {
                if (!isLastPos && (FormPrinterSettings.ps.printerType != 3 || !isTravel))
                {
                    totalDist += locDist;
                    seg.Last.Value.Add(new Vector3(x, y, z), ana.activeExtruder.emax, totalDist, GCodePoint.toFileLine(fileid, actLine));
                    //changed = true;
                }
            }
            lastx = x;
            lasty = y;
            lastz = z;
            laste = ana.activeExtruder.emax;
            lastLayer = ana.layer;
        }
        public void ParseText(string text, bool clear)
        {
            GCode gc = new GCode();
            if (clear)
                Clear();
            foreach (string s in text.Split('\n'))
            {
                gc.Parse(s);
                AddGCode(gc);
            }
        }
        public void parseGCodeShortArray(List<GCodeShort> codes, bool clear, int fileid)
        {
            if (clear)
                Clear();
            this.fileid = fileid;
            actLine = 0;
            foreach (GCodeShort code in codes)
            {
                ana.analyzeShort(code);
                laste = ana.activeExtruder.emax;
                actLine++;
            }
        }
        public static void normalize(ref float[] n)
        {
            float d = (float)Math.Sqrt(n[0] * n[0] + n[1] * n[1] + n[2] * n[2]);
            n[0] /= d;
            n[1] /= d;
            n[2] /= d;
        }
        public void setColor(float dist)
        {
            if (!liveView || dist < minHotDist)
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.AmbientAndDiffuse, defaultColor);
            else
            {
                float fak = (totalDist - dist) / hotFilamentLength; // 1 = default 0 = hot
                float fak2 = 1 - fak;
                curColor[0] = defaultColor[0] * fak + hotColor[0] * fak2;
                curColor[1] = defaultColor[1] * fak + hotColor[1] * fak2;
                curColor[2] = defaultColor[2] * fak + hotColor[2] * fak2;
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.AmbientAndDiffuse, curColor);
            }
        }
        public void computeColor(float dist)
        {
            if (!liveView || dist < minHotDist)
            {
                curColor[0] = defaultColor[0];
                curColor[1] = defaultColor[1];
                curColor[2] = defaultColor[2];
            }
            else
            {
                float fak = (totalDist - dist) / hotFilamentLength; // 1 = default 0 = hot
                float fak2 = 1 - fak;
                curColor[0] = defaultColor[0] * fak + hotColor[0] * fak2;
                curColor[1] = defaultColor[1] * fak + hotColor[1] * fak2;
                curColor[2] = defaultColor[2] * fak + hotColor[2] * fak2;
            }
        }
        public void drawSegment(GCodePath path)
        {
            if (Main.threeDSettings.drawMethod == 2)
            {
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.AmbientAndDiffuse, defaultColor);
                GL.EnableClientState(ArrayCap.VertexArray);
                if (path.drawMethod != method || recompute)
                {
                    path.UpdateVBO(true);
                }
                else if (path.hasBuf == false && path.elements != null)
                    path.RefillVBO();
                if (path.elements == null) return;
                GL.BindBuffer(BufferTarget.ArrayBuffer, path.buf[0]);
                GL.VertexPointer(3, VertexPointerType.Float, 0, 0);
                float[] cp;
                if (liveView && path.lastDist > minHotDist)
                {
                    GL.EnableClientState(ArrayCap.ColorArray);
                    cp = new float[path.positions.Length];
                    int nv = 8 * (method - 1);
                    if (method == 1) nv = 4;
                    if (GCodePath.correctNorms) nv *= 2;
                    if (method == 0) nv = 1;
                    int p = 0;
                    foreach (LinkedList<GCodePoint> points in path.pointsLists)
                    {
                        if (points.Count < 2) continue;
                        foreach (GCodePoint pt in points)
                        {
                            computeColor(pt.dist);
                            for (int j = 0; j < nv; j++)
                            {
                                cp[p++] = curColor[0];
                                cp[p++] = curColor[1];
                                cp[p++] = curColor[2];
                            }
                        }
                    }
                    GL.Enable(EnableCap.ColorMaterial);
                    GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);
                    if (colbufSize < cp.Length)
                    {
                        if (colbufSize != 0)
                            GL.DeleteBuffers(1, colbuf);
                        GL.GenBuffers(1, colbuf);
                        colbufSize = cp.Length;
                        GL.BindBuffer(BufferTarget.ArrayBuffer, colbuf[0]);
                        GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(cp.Length * sizeof(float) * 2), (IntPtr)0, BufferUsageHint.StaticDraw);
                    }
                    GL.BindBuffer(BufferTarget.ArrayBuffer, colbuf[0]);
                    GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, (IntPtr)(cp.Length * sizeof(float)), cp);
                    GL.ColorPointer(3, ColorPointerType.Float, 0, 0);
                }
                if (method == 0)
                {
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, path.buf[2]);
                    GL.DrawElements(BeginMode.Lines, path.elementsLength, DrawElementsType.UnsignedInt, 0);
                }
                else
                {
                    GL.EnableClientState(ArrayCap.NormalArray);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, path.buf[1]);
                    GL.NormalPointer(NormalPointerType.Float, 0, 0);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, path.buf[2]);
                    GL.DrawElements(BeginMode.Quads, path.elementsLength, DrawElementsType.UnsignedInt, 0);
                    GL.DisableClientState(ArrayCap.NormalArray);
                }
                if (liveView && path.lastDist > minHotDist)
                {
                    GL.Disable(EnableCap.ColorMaterial);
                    GL.DisableClientState(ArrayCap.ColorArray);
                }
                GL.DisableClientState(ArrayCap.VertexArray);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }
            else
            {
                if (path.drawMethod != method || recompute || path.hasBuf)
                    path.UpdateVBO(false);
                if (Main.threeDSettings.drawMethod > 0) // Is also fallback for vbos with dynamic colors
                {
                    GL.EnableClientState(ArrayCap.VertexArray);
                    if (path.elements == null) return;
                    GL.VertexPointer(3, VertexPointerType.Float, 0, path.positions);
                    float[] cp;
                    if (liveView && path.lastDist > minHotDist)
                    {
                        GL.EnableClientState(ArrayCap.ColorArray);
                        cp = new float[path.positions.Length];
                        int nv = 8 * (method - 1);
                        if (method == 1) nv = 4;
                        if (GCodePath.correctNorms) nv *= 2;
                        if (method == 0) nv = 1;
                        int p = 0;
                        foreach (LinkedList<GCodePoint> points in path.pointsLists)
                        {
                            foreach (GCodePoint pt in points)
                            {
                                computeColor(pt.dist);
                                for (int j = 0; j < nv; j++)
                                {
                                    cp[p++] = curColor[0];
                                    cp[p++] = curColor[1];
                                    cp[p++] = curColor[2];
                                }
                            }
                        }
                        GL.Enable(EnableCap.ColorMaterial);
                        GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);
                        GL.ColorPointer(3, ColorPointerType.Float, 0, cp);
                    }
                    if (method == 0)
                        GL.DrawElements(BeginMode.Lines, path.elementsLength, DrawElementsType.UnsignedInt, path.elements);
                    else
                    {
                        GL.EnableClientState(ArrayCap.NormalArray);
                        GL.NormalPointer(NormalPointerType.Float, 0, path.normals);
                        GL.DrawElements(BeginMode.Quads, path.elementsLength, DrawElementsType.UnsignedInt, path.elements);
                        GL.DisableClientState(ArrayCap.NormalArray);
                    }
                    /*ErrorCode err = GL.GetError();
                    if (err != ErrorCode.NoError)
                    {
                        PrinterConnection.logInfo("D1 error" + err);
                    }*/
                    if (liveView && path.lastDist > minHotDist)
                    {
                        GL.Disable(EnableCap.ColorMaterial);
                        GL.DisableClientState(ArrayCap.ColorArray);
                    }
                    GL.DisableClientState(ArrayCap.VertexArray);
                }
                else
                {
                    if (!liveView || path.lastDist < minHotDist)
                    {
                        int i, l = path.elementsLength;
                        if (method == 0)
                        {
                            GL.Begin(BeginMode.Lines);
                            for (i = 0; i < l; i++)
                            {
                                int p = path.elements[i] * 3;
                                GL.Vertex3(ref path.positions[p]);
                            }
                            GL.End();
                        }
                        else
                        {
                            GL.Begin(BeginMode.Quads);
                            for (i = 0; i < l; i++)
                            {
                                int p = path.elements[i] * 3;
                                GL.Normal3(ref path.normals[p]);
                                GL.Vertex3(ref path.positions[p]);
                            }
                            GL.End();
                        }
                    }
                    else
                    {
                        if (method > 0)
                        {
                            int nv = 8 * (method - 1), i;
                            if (method == 1) nv = 4;
                            float alpha, dalpha = (float)Math.PI * 2f / nv;
                            float[] dir = new float[3];
                            float[] dirs = new float[3];
                            float[] diru = new float[3];
                            float[] n = new float[3];
                            float dh = 0.5f * h;
                            float dw = 0.5f * w;
                            if (path.pointsCount < 2) return;
                            diru[0] = diru[1] = 0;
                            diru[2] = 1;
                            GL.Begin(BeginMode.Quads);
                            bool first = true;
                            Vector3 last = new Vector3();
                            foreach (LinkedList<GCodePoint> points in path.pointsLists)
                            {
                                first = true;
                                foreach (GCodePoint pt in points)
                                {
                                    Vector3 v = pt.p;
                                    setColor(pt.dist);
                                    if (first)
                                    {
                                        last = v;
                                        first = false;
                                        continue;
                                    }
                                    bool isLast = pt == points.Last.Value;
                                    dir[0] = v.X - last.X;
                                    dir[1] = v.Y - last.Y;
                                    dir[2] = v.Z - last.Z;
                                    if (!fixedH)
                                    {
                                        float dist = (float)Math.Sqrt(dir[0] * dir[0] + dir[1] * dir[1] + dir[2] * dir[2]);
                                        if (dist > 0)
                                        {
                                            h = (float)Math.Sqrt((pt.e - laste) * dfac / dist);
                                            w = h * wfac;
                                            dh = 0.5f * h;
                                            dw = 0.5f * w;
                                        }
                                    }
                                    normalize(ref dir);
                                    dirs[0] = -dir[1];
                                    dirs[1] = dir[0];
                                    dirs[2] = dir[2];
                                    alpha = 0;
                                    float c = (float)Math.Cos(alpha) * dh;
                                    float s = (float)Math.Sin(alpha) * dw;
                                    n[0] = (float)(s * dirs[0] + c * diru[0]);
                                    n[1] = (float)(s * dirs[1] + c * diru[1]);
                                    n[2] = (float)(s * dirs[2] + c * diru[2]);
                                    normalize(ref n);
                                    GL.Normal3(n[0], n[1], n[2]);
                                    for (i = 0; i < nv; i++)
                                    {
                                        GL.Vertex3(last.X + s * dirs[0] + c * diru[0], last.Y + s * dirs[1] + c * diru[1], last.Z - dh + s * dirs[2] + c * diru[2]);
                                        GL.Vertex3(v.X + s * dirs[0] + c * diru[0], v.Y + s * dirs[1] + c * diru[1], v.Z - dh + s * dirs[2] + c * diru[2]);
                                        alpha += dalpha;
                                        c = (float)Math.Cos(alpha) * dh;
                                        s = (float)Math.Sin(alpha) * dw;
                                        n[0] = (float)(s * dirs[0] + c * diru[0]);
                                        n[1] = (float)(s * dirs[1] + c * diru[1]);
                                        n[2] = (float)(s * dirs[2] + c * diru[2]);
                                        normalize(ref n);
                                        GL.Normal3(n[0], n[1], n[2]);
                                        GL.Vertex3(v.X + s * dirs[0] + c * diru[0], v.Y + s * dirs[1] + c * diru[1], v.Z - dh + s * dirs[2] + c * diru[2]);
                                        GL.Vertex3(last.X + s * dirs[0] + c * diru[0], last.Y + s * dirs[1] + c * diru[1], last.Z - dh + s * dirs[2] + c * diru[2]);
                                    }
                                    last = v;
                                }
                            }
                            GL.End();
                        }
                        else if (method == 0)
                        {
                            // Draw edges
                            if (path.pointsCount < 2) return;
                            GL.Material(MaterialFace.Front, MaterialParameter.Emission, defaultColor);
                            GL.Begin(BeginMode.Lines);
                            bool first = true;
                            foreach (LinkedList<GCodePoint> points in path.pointsLists)
                            {
                                first = true;
                                foreach (GCodePoint pt in points)
                                {
                                    Vector3 v = pt.p;
                                    if (liveView && pt.dist >= minHotDist)
                                    {
                                        float fak = (totalDist - pt.dist) / hotFilamentLength; // 1 = default 0 = hot
                                        float fak2 = 1 - fak;
                                        curColor[0] = defaultColor[0] * fak + hotColor[0] * fak2;
                                        curColor[1] = defaultColor[1] * fak + hotColor[1] * fak2;
                                        curColor[2] = defaultColor[2] * fak + hotColor[2] * fak2;
                                        GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, curColor);
                                    }

                                    GL.Vertex3(v);
                                    if (!first && pt != points.Last.Value)
                                        GL.Vertex3(v);
                                    first = false;
                                }
                            }
                            GL.End();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Used to mark a section of the path. Is called after drawSegment so VBOs are already
        /// computed. Not used inside live preview.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mstart"></param>
        /// <param name="mend"></param>
        public void drawSegmentMarked(GCodePath path, int mstart, int mend)
        {
            // Check if inside mark area
            int estart = 0;
            int eend = path.elementsLength;
            GCodePoint lastP = null, startP = null, endP = null;
            foreach (LinkedList<GCodePoint> plist in path.pointsLists)
            {
                if (plist.Count > 1)
                    foreach (GCodePoint point in plist)
                    {
                        if (startP == null)
                        {
                            if (point.fline >= mstart && point.fline <= mend)
                                startP = point;
                        }
                        else
                        {
                            if (point.fline > mend)
                            {
                                endP = point;
                                break;
                            }
                        }
                        lastP = point;
                    }
                if (endP != null) break;
            }
            if (startP == null) return;
            estart = startP.element;
            if (endP != null) eend = endP.element;
            if (estart == eend) return;
            if (Main.threeDSettings.drawMethod == 2)
            {
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.AmbientAndDiffuse, defaultColor);
                GL.EnableClientState(ArrayCap.VertexArray);
                if (path.elements == null) return;
                GL.BindBuffer(BufferTarget.ArrayBuffer, path.buf[0]);
                GL.VertexPointer(3, VertexPointerType.Float, 0, 0);
                if (method == 0)
                {
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, path.buf[2]);
                    GL.DrawElements(BeginMode.Lines, eend - estart, DrawElementsType.UnsignedInt, sizeof(int) * estart);
                }
                else
                {
                    GL.EnableClientState(ArrayCap.NormalArray);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, path.buf[1]);
                    GL.NormalPointer(NormalPointerType.Float, 0, 0);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, path.buf[2]);
                    //GL.DrawElements(BeginMode.Quads, path.elementsLength, DrawElementsType.UnsignedInt, 0);
                    //GL.DrawRangeElements(BeginMode.Quads, estart, eend, path.elementsLength, DrawElementsType.UnsignedInt, 0);
                    GL.DrawElements(BeginMode.Quads, eend - estart, DrawElementsType.UnsignedInt, sizeof(int) * estart);
                    GL.DisableClientState(ArrayCap.NormalArray);
                }

                GL.DisableClientState(ArrayCap.VertexArray);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }
            else
            {
                if (path.drawMethod != method || recompute || path.hasBuf)
                    path.UpdateVBO(false);
                if (Main.threeDSettings.drawMethod > 0) // Is also fallback for vbos with dynamic colors
                {
                    GL.EnableClientState(ArrayCap.VertexArray);
                    if (path.elements == null) return;
                    GL.VertexPointer(3, VertexPointerType.Float, 0, path.positions);
                    GCHandle handle = GCHandle.Alloc(path.elements, GCHandleType.Pinned);
                    try
                    {
                        IntPtr pointer = new IntPtr(handle.AddrOfPinnedObject().ToInt32() + sizeof(int) * estart);
                        if (method == 0)

                            GL.DrawElements(BeginMode.Lines, eend - estart, DrawElementsType.UnsignedInt, pointer);
                        else
                        {
                            GL.EnableClientState(ArrayCap.NormalArray);
                            GL.NormalPointer(NormalPointerType.Float, 0, path.normals);
                            GL.DrawElements(BeginMode.Quads, eend - estart, DrawElementsType.UnsignedInt, pointer);
                            GL.DisableClientState(ArrayCap.NormalArray);
                        }
                    }
                    finally
                    {
                        if (handle.IsAllocated)
                        {
                            handle.Free();
                        }
                    }
                    GL.DisableClientState(ArrayCap.VertexArray);
                }
                else
                {
                    int i, l = path.elementsLength;
                    if (method == 0)
                    {
                        GL.Begin(BeginMode.Lines);
                        for (i = estart; i < eend; i++)
                        {
                            int p = path.elements[i] * 3;
                            GL.Vertex3(ref path.positions[p]);
                        }
                        GL.End();
                    }
                    else
                    {
                        GL.Begin(BeginMode.Quads);
                        for (i = estart; i < eend; i++)
                        {
                            int p = path.elements[i] * 3;
                            GL.Normal3(ref path.normals[p]);
                            GL.Vertex3(ref path.positions[p]);
                        }
                        GL.End();
                    }
                }
            }
        }
        /** Draw stored travel moves */
        public void drawMoves()
        {
            if (Main.threeDSettings.drawMethod != 2) return;
            int l = travelMoves.Count;
            if (!hasTravelBuf || travelMovesBuffered + 100 < l)
            {
                // Revill vbo
                if (hasTravelBuf)
                    GL.DeleteBuffers(2, travelBuf);
                int len = 6 * l;
                float[] pts = new float[len];
                int[] idx = new int[2 * l];
                int idxp = 0;
                int p = 0;
                int n = 0, ic = 0;
                foreach (GCodeTravel t in travelMoves)
                {
                    idx[idxp++] = ic++;
                    idx[idxp++] = ic++;
                    pts[p++] = t.p1.X;
                    pts[p++] = t.p1.Y;
                    pts[p++] = t.p1.Z;
                    pts[p++] = t.p2.X;
                    pts[p++] = t.p2.Y;
                    pts[p++] = t.p2.Z;
                    n++;
                }
                // NSLog(@"Count %d n %d",l,n);
                GL.GenBuffers(2, travelBuf);
                GL.BindBuffer(BufferTarget.ArrayBuffer, travelBuf[0]);
                GL.BufferData(BufferTarget.ArrayBuffer,(IntPtr)(sizeof(float)* len), pts,BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, travelBuf[1]);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(l * 2 * sizeof(int)), idx, BufferUsageHint.StaticDraw);
                hasTravelBuf = true;
                travelMovesBuffered = l;
            }
            float[] black = new float[4]{0,0,0,1};
            float[] travel = new float[4];
            Color col = Main.threeDSettings.travelMoves.BackColor;
            travel[0] = (float)col.R / 255.0f;
            travel[1] = (float)col.G / 255.0f;
            travel[2] = (float)col.B / 255.0f;
            travel[3] = 1;
            GL.LineWidth(1f);
            GL.Disable(EnableCap.LineSmooth);
            // Set move color
            GL.Material(MaterialFace.FrontAndBack,MaterialParameter.AmbientAndDiffuse, black);
            GL.Material(MaterialFace.FrontAndBack,MaterialParameter.Specular,black);
            GL.Material(MaterialFace.FrontAndBack,MaterialParameter.Emission,travel);
            // Draw buffer
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, travelBuf[0]);
            GL.VertexPointer(3, VertexPointerType.Float, 0, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, travelBuf[1]);
            GL.DrawElements(BeginMode.Lines, travelMovesBuffered * 2, DrawElementsType.UnsignedInt, 0);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            // Draw new lines one by one
            GL.Begin(BeginMode.Lines);
            for (int i = travelMovesBuffered; i < l; i++)
            {
                GCodeTravel t = travelMoves[i];
                GL.Vertex3(t.p1);
                GL.Vertex3(t.p2);
            }
            GL.End();
            GL.Material(MaterialFace.FrontAndBack,MaterialParameter.Emission,black);
        }
        public void drawMovesFromTo(int mstart, int mend)
        {
            if (Main.threeDSettings.drawMethod != 2) return;
            float[] black = new float[4] { 0, 0, 0, 1 };
            int l = travelMoves.Count;
            // Check if inside mark area
            int estart = 0;
            int eend = l;
            //GCodePoint *lastP = nil;
            int startP = -1, endP = -1, p = 0;
            foreach (GCodeTravel t in travelMoves)
            {
                if (startP < 0)
                {
                    if (t.fline >= mstart && t.fline <= mend)
                        startP = p;
                }
                else
                {
                    if (t.fline > mend)
                    {
                        endP = p;
                        break;
                    }
                }
                //lastP = point;
                if (endP >= 0) break;
                p++;
            }
            if (startP == -1)
            {
                return;
            }
            estart = startP;
            if (endP >= 0) eend = endP;
            if (estart == eend)
            {
                return;
            }

            // Set move color
            GL.Material(MaterialFace.FrontAndBack,MaterialParameter.AmbientAndDiffuse, black);
            GL.Material(MaterialFace.FrontAndBack,MaterialParameter.Specular, black);
            GL.Material(MaterialFace.FrontAndBack,MaterialParameter.Emission,defaultColor);
            // Draw buffer
            GL.Color4(defaultColor);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, travelBuf[0]);
            GL.VertexPointer(3, VertexPointerType.Float, 0, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, travelBuf[1]);
            GL.DrawElements(BeginMode.Lines, 2 * (eend - estart), DrawElementsType.UnsignedInt,(sizeof(int) * estart * 2));
            //glDrawElements(GL_LINES, travelMovesBuffered*2, GL_UNSIGNED_INT, 0);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.Material(MaterialFace.FrontAndBack,MaterialParameter.Emission,black);
        }
        public override void Paint()
        {
            changed = false;
            if (Main.threeDSettings.drawMethod != 2 && colbufSize > 0)
            {
                GL.DeleteBuffers(1, colbuf);
                colbufSize = 0;
            }
            if (Main.threeDSettings.checkDisableFilamentVisualization.Checked) return; // Disabled too much for card
            hotFilamentLength = Main.threeDSettings.hotFilamentLength;
            minHotDist = totalDist - hotFilamentLength;
            Reduce(); // Minimize number of VBO
            //long timeStart = DateTime.Now.Ticks;
            Color col;
            col = Main.threeDSettings.hotFilament.BackColor;
            hotColor[0] = (float)col.R / 255.0f;
            hotColor[1] = (float)col.G / 255.0f;
            hotColor[2] = (float)col.B / 255.0f;
            hotColor[3] = curColor[3] = 1;
            //           GL.Material(MaterialFace.FrontAndBack, MaterialParameter.AmbientAndDiffuse, new OpenTK.Graphics.Color4(col.R, col.G, col.B, 255));
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, new OpenTK.Graphics.Color4(0, 0, 0, 0));
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, 50f);
            method = Main.threeDSettings.filamentVisualization;
            if (method > ana.maxDrawMethod) method = ana.maxDrawMethod;
            wfac = Main.threeDSettings.widthOverHeight;
            h = Main.threeDSettings.layerHeight;
            w = h * wfac;
            fixedH = Main.threeDSettings.useLayerHeight;
            dfac = (float)(Math.PI * Main.threeDSettings.filamentDiameter * Main.threeDSettings.filamentDiameter * 0.25 / wfac);
            recompute = lastFilHeight != h || lastFilWidth != w || fixedH != lastFilUseHeight || dfac != lastFilDiameter || lastCorrectNormals != GCodePath.correctNorms || lastMachineType != FormPrinterSettings.ps.printerType;
            lastFilHeight = h;
            lastFilWidth = w;
            lastFilDiameter = dfac;
            lastFilUseHeight = fixedH;
            lastMachineType = FormPrinterSettings.ps.printerType;
            lastCorrectNormals = GCodePath.correctNorms;
            for (int i = 0; i < MaxExtruder; i++)
            {
                if (i == 1) col = Main.threeDSettings.filament2.BackColor;
                else if (i == 2) col = Main.threeDSettings.filament3.BackColor;
                else col = Main.threeDSettings.filament.BackColor;
                defaultColor[0] = (float)col.R / 255.0f;
                defaultColor[1] = (float)col.G / 255.0f;
                defaultColor[2] = (float)col.B / 255.0f;
                defaultColor[3] = 1;
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.AmbientAndDiffuse, defaultColor);
                foreach (GCodePath path in segments[i])
                {
                    drawSegment(path);
                }
            }
            if (!Main.threeDSettings.checkDisableTravelMoves.Checked)
                drawMoves();

            if (showSelection)
            {
                RepetierEditor ed = Main.main.editor;
                selectionStart = selectionEnd = 0;
                if (!ed.hasSelection)
                {
                    selectionStart = selectionEnd = GCodePoint.toFileLine(ed.FileIndex, ed._row);
                }
                else
                {
                    if (ed._row < ed.selRow)
                    {
                        selectionStart = GCodePoint.toFileLine(ed.FileIndex, ed._row);
                        selectionEnd = GCodePoint.toFileLine(ed.FileIndex, ed.selRow);
                    }
                    else
                    {
                        selectionEnd = GCodePoint.toFileLine(ed.FileIndex, ed._row);
                        selectionStart = GCodePoint.toFileLine(ed.FileIndex, ed.selRow);
                    }
                }
                col = Main.threeDSettings.selectedFilament.BackColor;
                defaultColor[0] = (float)col.R / 255.0f;
                defaultColor[1] = (float)col.G / 255.0f;
                defaultColor[2] = (float)col.B / 255.0f;
                GL.DepthFunc(DepthFunction.Lequal);
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.AmbientAndDiffuse, defaultColor);
                for (int i = 0; i < MaxExtruder; i++)
                    foreach (GCodePath path in segments[i])
                    {
                        drawSegmentMarked(path, selectionStart, selectionEnd);
                    }
                if (!Main.threeDSettings.checkDisableTravelMoves.Checked)
                {
                    drawMovesFromTo(selectionStart, selectionEnd);
                }
            }
            // timeStart = DateTime.Now.Ticks - timeStart;
            //  double time = (double)timeStart * 0.1;
            // Main.conn.log("OpenGL paint time " + time.ToString("0.0", GCode.format) + " microseconds",false,4);
        }
        public override bool Changed
        {
            get
            {
                return changed;
            }
        }
    }
}
