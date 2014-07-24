using Kinect.Toolbox.Gestures.Learning_Machine;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Kinect.Toolbox
{
    public class RotationGestureDetector: TwoHandsAlgorithmicGessureDetector
    {
        float angle = 0.0f;

        public RotationGestureDetector(KinectSensor sensor, string gestureName = "rotation", int windowSize = 1)
            : base(sensor, gestureName, windowSize)
        {
        }

        public override void Add(Microsoft.Kinect.SkeletonPoint position, Microsoft.Kinect.KinectSensor sensor)
        {
            base.Add(position, sensor);
        }


        protected bool ScanPositions()
        {
            if (Entries.Count > 0 && LeftEntries.Count > 0)
            {
                var vec1 = Tools.Convert(Sensor, ((EntryKinect)Entries[0]).SkeletonPosition);
                var vec2 = Tools.Convert(Sensor, ((EntryKinect)LeftEntries[0]).SkeletonPosition);
                angle = GoldenSection.GetAngleBetween(Vector2.Zero, vec2 - vec1);
                return true;
            }
            else
                return false;

        }

        protected override void LookForGesture()
        {
            if (ScanPositions())
            {
                //RaiseGestureDetected(gestureName + " " + distance.ToString());
                Debug.WriteLine("angle: " + angle.ToString());
            }
        }
    }
}
