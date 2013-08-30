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
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;
using OpenTK;

namespace RepetierHost.model
{
    public class Coord3D
    {
        public float x = 0, y = 0, z = 0;
        public Coord3D() { }
        public Coord3D(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }
    }
    public abstract class ThreeDModel
    {
        private bool selected = false;
        private Coord3D position = new Coord3D();
        private Coord3D rotation = new Coord3D();
        private Coord3D scale = new Coord3D(1, 1, 1);
        public LinkedList<ModelAnimation> animations = new LinkedList<ModelAnimation>();
        public float xMin = 0, yMin = 0, zMin = 0, xMax = 0, yMax = 0, zMax = 0;

        public void addAnimation(ModelAnimation anim)
        {
            animations.AddLast(anim);
        }
        public void removeAnimationWithName(string aname)
        {
            bool found = true;
            while (found)
            {
                found = false;
                foreach (ModelAnimation a in animations)
                {
                    if (a.name.Equals(aname))
                    {
                        found = true;
                        animations.Remove(a);
                        break;
                    }
                }
            }
        }
        public bool hasAnimationWithName(string aname)
        {
            foreach (ModelAnimation a in animations)
            {
                if (a.name.Equals(aname))
                {
                    return true;
                }
            }
            return false;
        }
        public void clearAnimations()
        {
            animations.Clear();
        }
        public bool hasAnimations
        {
            get { return animations.Count > 0; }
        }
        public void AnimationBefore()
        {
            foreach (ModelAnimation a in animations)
                a.BeforeAction(this);
        }
        /// <summary>
        /// Plays the after action and removes finished animations.
        /// </summary>
        public void AnimationAfter()
        {
            bool remove = false;
            foreach (ModelAnimation a in animations)
            {
                a.AfterAction(this);
                remove |= a.AnimationFinished();
            }
            if (remove)
            {
                bool found = true;
                while (found)
                {
                    found = false;
                    foreach (ModelAnimation a in animations)
                    {
                        if (a.AnimationFinished())
                        {
                            found = true;
                            animations.Remove(a);
                            break;
                        }
                    }
                }
            }
        }
        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }
        public Coord3D Position
        {
            get { return position; }
            set { position = value; }
        }
        public Coord3D Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public Coord3D Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        public virtual void ReduceQuality() {
        }
        public virtual void ResetQuality() { }
        /// <summary>
        /// Has the model changed since last paint?
        /// </summary>
        public virtual bool Changed
        {
            get { return false; }
        }
        public virtual void Clear() { }
        abstract public void Paint();
        public virtual Vector3 getCenter()
        {
            return new Vector3(0, 0, 0);
        }
    }
}
