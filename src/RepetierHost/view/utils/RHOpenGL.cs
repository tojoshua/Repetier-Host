using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace RepetierHost.view.utils
{
    public class RHOpenGL : OpenTK.GLControl
    {
        public RHOpenGL()
        //    : base(new GraphicsMode(32, 24, 8, RHOpenGL.MaxAntiAlias()))
//            : base(new GraphicsMode(32, 24, 8, 4), 2, 0, GraphicsContextFlags.ForwardCompatible)
        {
        }
        public static int MaxAntiAlias()
        {
            var aa_modes = new List<int>();
            int aa = 0;
            do
            {
                GraphicsMode mode = new GraphicsMode(32, 24, 0, aa);
                Console.WriteLine("Samples:"+mode.Samples);
                if (!aa_modes.Contains(mode.Samples))
                    aa_modes.Add(aa);
                aa += 2;
            } while (aa <= 32);
            int best = aa_modes.Last();
            Console.WriteLine("Best AntiAlias:" + best);
            return best;
        }
        // Returns a System.Drawing.Bitmap with the contents of the current framebuffer
        // Call Dispose on bitmap when done
        public void SaveScreenshot(string file)
        {
            if (GraphicsContext.CurrentContext == null)
                throw new GraphicsContextMissingException();

            Bitmap bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(this.ClientRectangle, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            OpenTK.Graphics.OpenGL.GL.ReadPixels(0, 0, this.ClientSize.Width, this.ClientSize.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, OpenTK.Graphics.OpenGL.PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            bmp.Save(file, ImageFormat.Png);
            bmp.Dispose();
        }
        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    return true;
                case Keys.Shift | Keys.Right:
                case Keys.Shift | Keys.Left:
                case Keys.Shift | Keys.Up:
                case Keys.Shift | Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }
    }
}
