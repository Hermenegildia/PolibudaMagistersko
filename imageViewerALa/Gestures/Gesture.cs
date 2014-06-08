using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestures
{
    public abstract class Gesture
    {
        abstract public void Update(Skeleton[] skeletons, long frameTimeStamp);
    }
}
