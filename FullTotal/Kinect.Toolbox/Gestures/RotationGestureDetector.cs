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
        double angle = 0;
        EntryKinect leftStartPosition;
        EntryKinect rightStartPosition;
        FrameworkElement control;
        Vector originalControlSize;
        Vector initialVector;
        Point initialRightPoint;
        Point initialLeftPoint;

        public delegate void GestureDetection(string gestureName, double angle);
        public event GestureDetection OnGestureWithAngleDetected;
    

        public RotationGestureDetector(KinectSensor sensor, FrameworkElement control, Vector originalControlSize, string gestureName = "rotation", int windowSize = 10)
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
            //initialVector = Tools.GetJointPoint(Sensor, control, rightStartPosition.SkeletonPosition) - Tools.GetJointPoint(Sensor, control, leftStartPosition.SkeletonPosition);
            initialLeftPoint = Tools.GetJointPoint(Sensor, control, leftStartPosition.SkeletonPosition);
            initialRightPoint = Tools.GetJointPoint(Sensor, control, rightStartPosition.SkeletonPosition);
        }


        protected bool ScanPositions()
        {
            if (Entries.Count == WindowSize && LeftEntries.Count == WindowSize)
            {
                var pointRightCurrent = Tools.GetJointPoint(Sensor, control, ((EntryKinect)Entries[WindowSize - 1]).SkeletonPosition);
                var pointLeftCurrent = Tools.GetJointPoint(Sensor, control, ((EntryKinect)LeftEntries[WindowSize - 1]).SkeletonPosition);

                //var vecR = new Vector2((float)pointRightCurrent.X, (float)pointRightCurrent.Y);
                //var vecL = new Vector2((float)pointLeftCurrent.X, (float)pointLeftCurrent.Y);
                ////angle = GoldenSection.GetAngleBetween(Vector2.Zero, vec2 - vec1);
                ////wartosc kata wyrazona w radianach
                //var vecDif = vecR - vecL;
                //Vector2 handsVec = new Vector2(Math.Abs( vecDif.X), Math.Abs(vecDif.Y));
                //var bufAngle = GoldenSection.GetAngleBetween(new Vector2((float)initialVector.X, (float)initialVector.Y), handsVec);//new Vector2(Math.Abs(handsVec.X), Math.Abs(handsVec.Y)));//vecR - vecL);
                //if (Math.Abs(bufAngle) == MathHelper.PiOver2)
                //    bufAngle = 0;
                if (pointRightCurrent.X == pointLeftCurrent.X)
                    return false;
                else
                {
                    var m1 = (pointRightCurrent.Y - pointLeftCurrent.Y) / (pointRightCurrent.X - pointLeftCurrent.X);
                    var m2 = (initialRightPoint.Y - initialLeftPoint.Y) / (initialRightPoint.X - initialLeftPoint.X);
                    //tg(alfa) =  (m2-m1)/(1+m1*m2)
                    if (m1 * m2 != -1)
                        angle = Math.Atan((m2 - m1) / (1 + m1 * m2));
                    else
                        return false;
                }
                angle = 180 * angle / Math.PI;
                return true;

            }

            return false;
        }

        protected void RaiseGestureDetected(string gesture, double angle)
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
