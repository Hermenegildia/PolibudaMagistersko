using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureKinectTools.Postures
{
    public abstract class PostureDetector
    {
        public event Action<string> PostureDetected;

        readonly int accumulatorTarget;
        string previousPosture;
        int accumulator;
        string accumulatedPosture;

        public string CurrentPosture
        {
            get { return previousPosture; }
            protected set { previousPosture = value; }
        }

        protected PostureDetector(int accumulators)
        {
            accumulatorTarget = accumulators;
            this.previousPosture = string.Empty;
            this.accumulatedPosture = string.Empty;
        }

        public abstract void TrackPostures(Skeleton skeleton);

        protected void RaisePostureDetected(string posture)
        {
            if (accumulator < accumulatorTarget)
            {
                if (accumulatedPosture != posture)
                {
                    accumulator = 0;
                    accumulatedPosture = posture;
                }

                accumulator++;
                return;
            }

            if (previousPosture == posture)
                return;

            previousPosture = posture;
            if (PostureDetected != null)
            {
                PostureDetected(posture);
            }

            accumulator = 0;
        }

        protected void Reset()
        {
            previousPosture = string.Empty;
            accumulator = 0;
        }
    }
}
