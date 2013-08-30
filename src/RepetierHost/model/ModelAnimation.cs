using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepetierHost.model
{
    /// <summary>
    /// Base class for animations. Animations are used to modify the appeareance
    /// of the underlying content. E.g. it could pulse an object.
    /// </summary>
    public class ModelAnimation
    {
        double startTime;
        public string name;
        public ModelAnimation(string _name)
        {
            name = _name;
            startTime = (double)DateTime.Now.Ticks / 10000000.0;
        }
        public double Time
        {
            get { return (double)DateTime.Now.Ticks / 10000000.0 - startTime; }
        }
        /// <summary>
        /// Return true, if the animation is finished
        /// </summary>
        /// <returns></returns>
        public virtual bool AnimationFinished()
        {
            return false;
        }
        public virtual void BeforeAction(ThreeDModel model) { }
        public virtual void AfterAction(ThreeDModel model) { }
    }
}
