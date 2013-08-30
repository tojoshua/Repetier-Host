using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace RepetierHost.model
{
    public class PulseAnimation : ModelAnimation
    {
        double frequency;
        double scalex, scaley, scalez;
        public PulseAnimation(string name, double sx, double sy, double sz, double fq)
            : base(name)
        {
            frequency = fq;
            scalex = sx;
            scaley = sy;
            scalez = sz;
        }
        public override void BeforeAction(ThreeDModel model) {

            double baseamp = Math.Sin(Time * 2.0 * Math.PI * frequency);
            Vector3 center = model.getCenter();
            //center = Vector3.Add(center,new Vector3(model.Position.x,model.Position.y,model.Position.z));
            GL.Translate(center);
            GL.Scale(1.0 + scalex * baseamp, 1.0 + scaley * baseamp, 1.0 + scalez * baseamp);
            GL.Translate(-center.X, -center.Y, -center.Z);
        }
    }
    public class DropAnimation : ModelAnimation
    {
        int mode;
        double height;
        public DropAnimation(string name)
            : base(name)
        {
            mode = 0;
        }
        public override bool AnimationFinished()
        {
            return mode==2;
        }
        public override void BeforeAction(ThreeDModel model)
        {
            double t = Time;
            Vector3 c = model.getCenter();
            //c = Vector3.Add(c,new Vector3(model.Position.x, model.Position.y, model.Position.z));
            if (mode == 0)
            {
                height = Main.printerSettings.PrintAreaHeight * 1.2;
                mode = 1;
            }
            if (t < 1)
            {
                // s = 0,5*a*t^2
                // land after 1.5 sec =>a = 2*s/2.25

                GL.Translate(0, 0, height - 1.0 / 1.0 * height * t * t);
            }
            else if (t < 1.25)
            {
                double zamp = 0.3 * c.Z * (t - 1.25) / 0.25;
                GL.Translate(c.X,c.Y,c.Z-zamp);
                GL.Scale(1.0 + zamp/c.Z, 1.0 + zamp/c.Z, 1.0 - zamp/c.Z);
                GL.Translate(-c.X, -c.Y, -c.Z);
            }
            else if (t < 1.5)
            {
                double zamp = 0.3 * c.Z * (1.5-t) / 0.25;
                GL.Translate(c.X, c.Y, c.Z - zamp);
                GL.Scale(1.0 + zamp / c.Z, 1.0 + zamp / c.Z, 1.0 - zamp / c.Z);
                GL.Translate(-c.X, -c.Y, -c.Z);
            }
            else mode = 2;
        }
    }

}
