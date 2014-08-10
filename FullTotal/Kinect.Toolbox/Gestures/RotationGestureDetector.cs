using Kinect.Toolbox.Gestures.Learning_Machine;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace Kinect.Toolbox
{
    public class RotationGestureDetector: TwoHandsAlgorithmicGessureDetector
    {
        float angle = 0.0f;
        EntryKinect leftStartPosition;
        EntryKinect rightStartPosition;
        FrameworkElement control;
        Vector originalControlSize;
        Vector initialVector;

        public delegate void GestureDetection(string gestureName, float angle);
        public event GestureDetection OnGestureWithAngleDetected;
    

        public RotationGestureDetector(KinectSensor sensor, FrameworkElement control, Vector originalControlSize, string gestureName = "rotation", int windowSize = 1)
            : base(sensor, gestureName, windowSize)
        {
            this.control = control;
            this.originalControlSize = originalControlSize;
        }

        public override void Add(SkeletonPoint position, KinectSensor sensor) //tego nie uzywamy
        {
            base.Add(position, sensor);
        }

        public void SetStartPosition(Skeleton skeleton)
        {
            SkeletonPoint position;

            if (skeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
            {
                position = skeleton.Joints[JointType.HandRight].Position;
                rightStartPosition = new EntryKinect { Position = position.ToVector3(), SkeletonPosition = position, Time = DateTime.Now };
            }

            if (skeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked)
            {
                position = skeleton.Joints[JointType.HandLeft].Position;
                leftStartPosition = new EntryKinect { Position = position.ToVector3(), SkeletonPosition = position, Time = DateTime.Now };

            }
            initialVector = Tools.GetJointPoint(Sensor, control, rightStartPosition.SkeletonPosition) - Tools.GetJointPoint(Sensor, control, leftStartPosition.SkeletonPosition));
            
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

        protected void RaiseGestureDetected(string gesture, float angle)
        {
            // Too close?
            if (DateTime.Now.Subtract(lastGestureDate).TotalMilliseconds > MinimalPeriodBetweenGestures)
            {
                //if (OnGestureDetected != null)
                //    OnGestureDetected(gesture);

                if (OnGestureWithAngleDetected != null)
                    OnGestureWithAngleDetected(gesture, angle);

                lastGestureDate = DateTime.Now;
            }

            Entries.ForEach(e =>
            {
                if (DisplayCanvas != null)
                    DisplayCanvas.Children.Remove(e.DisplayEllipse);
            });
            Entries.Clear();
        }

        protected override void LookForGesture()
        {
            if (ScanPositions())
            {
                RaiseGestureDetected(gestureName, this.angle);
                //Debug.WriteLine("stretch: " + distance.ToString());
            }
        }
    }
}
