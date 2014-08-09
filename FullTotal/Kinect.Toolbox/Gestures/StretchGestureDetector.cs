using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace Kinect.Toolbox
{
    public class StretchGestureDetector : TwoHandsAlgorithmicGessureDetector
    {
        const double startThresholdLevel = 5;
        double distanceTotal = 0;
        double ratioX = 0;
        double ratioY = 0;
        double ratioTotal = 0;
        FrameworkElement control;

        double threshold;
        EntryKinect leftStartPosition;
        EntryKinect rightStartPosition;
        Vector originalControlSize;


        //public delegate void GestureDetection(string gestureName, double distance);
        public delegate void GestureDetection(string gestureName, double ratioX, double ratioY);
        public event GestureDetection OnGestureWithDistanceDetected;


        public StretchGestureDetector(KinectSensor sensor, FrameworkElement control, Vector originalControlSize, string gestureName = "stretch", int windowSize = 5) :
            base(sensor, gestureName, windowSize)
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

            //przeskalowanie domyślnej dyszki dla rozdzielczości 1600x900 (kinectRegion ma rozmiary 1600x717) na aktualne rozmiary kontrolki
            //zoomBorder ma 1024x693
            var actualSize = control.PointToScreen(new Point(control.ActualWidth, control.ActualHeight)) - control.PointToScreen(new Point(0, 0));
            threshold = (startThresholdLevel * actualSize.Y * actualSize.X) / (originalControlSize.X * originalControlSize.Y); 
        }

        protected bool ScanPositions()
        {
            if (Entries.Count ==WindowSize && LeftEntries.Count == WindowSize)
            {
                var pointRightCurrent = Tools.GetJointPoint(Sensor, control, ((EntryKinect)Entries[WindowSize-1]).SkeletonPosition);
                var pointLeftCurrent = Tools.GetJointPoint(Sensor, control, ((EntryKinect)LeftEntries[WindowSize-1]).SkeletonPosition);
                //Debug.WriteLine("dłonie: Right = " + pointRightCurrent.X + " rightY = " + pointRightCurrent.Y);
                if (pointRightCurrent.X > pointLeftCurrent.X) //tylko gdy prawa po prawej - coś dziwnego z pointami o.O,
                {  //todo: Ala ten warunek nie działa :/
                    //{
                    //double currentRightDistance = (pointRightCurrent - pointRightEnd).Length;
                    //double currentLeftDistance = (pointLeftCurrent - pointLeftEnd).Length;
                    double currentTotalDistance = (pointRightCurrent - pointLeftCurrent).Length;
                    //przeskalowanie
                    //var buf = Math.Abs(currentTotalDistance - distanceTotal);
                    distanceTotal = (Tools.GetJointPoint(Sensor, control, rightStartPosition.SkeletonPosition) - Tools.GetJointPoint(Sensor, control, leftStartPosition.SkeletonPosition)).Length;
                    //Debug.WriteLine("distanceTotal: " + distanceTotal);

                    var actualSize = control.PointToScreen(new Point(control.ActualWidth, control.ActualHeight)) - control.PointToScreen(new Point(0, 0));
                    threshold = (startThresholdLevel * actualSize.Y * actualSize.X) / (originalControlSize.X * originalControlSize.Y); 
                    if (Math.Abs(currentTotalDistance - distanceTotal) > threshold) //jesli zmiana dystansu miedzy dlonmi, a nie tylko przesuniecie!
                    {
                        //double deltaX = pointRightCurrent.X - pointLeftCurrent.X;
                        //double deltaY = pointRightCurrent.Y - pointLeftCurrent.Y;

                        //double currentRatioX = deltaX / actualSize.X; //wzgledne przesuniecie wzgledem rozmiaru okna
                        //double currentRatioY = deltaY / actualSize.Y;

                        double totalRatio = currentTotalDistance / distanceTotal;
                        
                        //this.ratioX = currentRatioX;
                        //this.ratioY = currentRatioY;
                        this.ratioTotal = totalRatio;

                        return true;
                    }
                }
                    
                    return false;
                //}
                //else
                //    return false;
            }
            else
                return false;

        }

        protected void RaiseGestureDetected(string gesture, double xRatio, double yRatio)
        {
            // Too close?
            if (DateTime.Now.Subtract(lastGestureDate).TotalMilliseconds > MinimalPeriodBetweenGestures)
            {
                //if (OnGestureDetected != null)
                //    OnGestureDetected(gesture);

                if (OnGestureWithDistanceDetected != null)
                    OnGestureWithDistanceDetected(gesture, xRatio, yRatio);

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
                RaiseGestureDetected(gestureName, this.ratioTotal, this.ratioY);
                //Debug.WriteLine("stretch: " + distance.ToString());
            }
        }
    }
}
