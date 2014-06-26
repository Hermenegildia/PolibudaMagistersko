using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureKinectTools.Gestures
{
    public abstract class CombinedGestureDetector : GestureDetector
    {
        List<GestureDetector> gestureDetectors;

        public double Epsilon
        {
            get;
            private set;
        }

        public int GestureDetectorsCount
        {
            get { return gestureDetectors.Count; }
        }

        public CombinedGestureDetector(double epsilon = 1000)
        {
            Epsilon = epsilon;
            gestureDetectors = new List<GestureDetector>();
        }

        public void Add(GestureDetector gestureDetector)
        {
            gestureDetector.OnGestureDetected += gestureDetector_OnGestureDetected;
            gestureDetectors.Add(gestureDetector);
        }

        public void Remove(GestureDetector gestureDetector)
        {
            gestureDetector.OnGestureDetected -= gestureDetector_OnGestureDetected;
            gestureDetectors.Remove(gestureDetector);
        }

        private void gestureDetector_OnGestureDetected(string gesture)
        {
            CheckGestures(gesture);
        }

        protected abstract void CheckGestures(string gesture);

        protected override void LookForGesture()
        {
            
        }

    }
}
