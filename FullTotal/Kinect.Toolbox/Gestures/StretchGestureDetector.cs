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
        double distanceTotal = 0;
        double ratioX = 0;
        double ratioY = 0;
        FrameworkElement control;


        //public delegate void GestureDetection(string gestureName, double distance);
        public delegate void GestureDetection(string gestureName, double ratioX, double ratioY);
        public event GestureDetection OnGestureWithDistanceDetected;


        public StretchGestureDetector(KinectSensor sensor, FrameworkElement control, string gestureName = "stretch", int windowSize = 5) :
            base(sensor, gestureName, windowSize)
        {
            this.control = control;
        }

        public override void Add(SkeletonPoint position, KinectSensor sensor) //tego nie uzywamy
        {
            base.Add(position, sensor);
        }


        protected bool ScanPositions()
        {
            if (Entries.Count ==WindowSize && LeftEntries.Count == WindowSize)
            {
                //Vector vec1 = new Vector(((EntryKinect)Entries[0]).SkeletonPosition.X, ((EntryKinect)Entries[0]).SkeletonPosition.Y); //takie same wyniki jak dla Vector2
                //Vector vec2 = new Vector(((EntryKinect)LeftEntries[0]).SkeletonPosition.X, ((EntryKinect)LeftEntries[0]).SkeletonPosition.Y);
                //var vec1 = Tools.Convert(Sensor, ((EntryKinect)Entries[0]).SkeletonPosition);
                //var vec2 = Tools.Convert(Sensor, ((EntryKinect)LeftEntries[0]).SkeletonPosition);
                //var pointRightEnd = Tools.GetJointPoint(Sensor, kinectRegion, ((EntryKinect)Entries[0]).SkeletonPosition); //we wspolrzednych wyswietlania
                //var pointLeftEnd = Tools.GetJointPoint(Sensor, kinectRegion, ((EntryKinect)LeftEntries[0]).SkeletonPosition);
                var pointRightCurrent = Tools.GetJointPoint(Sensor, control, ((EntryKinect)Entries[WindowSize-1]).SkeletonPosition);
                var pointLeftCurrent = Tools.GetJointPoint(Sensor, control, ((EntryKinect)LeftEntries[WindowSize-1]).SkeletonPosition);
                //Debug.WriteLine("dłonie: Right = " + pointRightCurrent.X + " rightY = " + pointRightCurrent.Y);
                //if (pointRightCurrent.X > pointLeftCurrent.Y) //tylko gdy prawa po prawej - coś dziwnego z pointami o.O,
                    //todo: Ala ten warunek nie działa :/
                //{
                    //double currentRightDistance = (pointRightCurrent - pointRightEnd).Length;
                    //double currentLeftDistance = (pointLeftCurrent - pointLeftEnd).Length;
                    double currentTotalDistance = (pointRightCurrent - pointLeftCurrent).Length;
                    //przeskalowanie
                    //var buf = Math.Abs(currentTotalDistance - distanceTotal);
                    
                    if(Math.Abs(currentTotalDistance-distanceTotal) > 8) //jesli zmiana dystansu miedzy dlonmi, a nie tylko przesuniecie!
                    {
                        double deltaX = pointRightCurrent.X - pointLeftCurrent.X;
                        double deltaY = pointRightCurrent.Y - pointLeftCurrent.Y;

                        double currentRatioX = deltaX / control.ActualWidth;
                        double currentRatioY = deltaY / control.ActualHeight;

                        this.distanceTotal = currentTotalDistance;
                        this.ratioX = currentRatioX;
                        this.ratioY = currentRatioY;
                        
                        return true;
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
                RaiseGestureDetected(gestureName, this.ratioX, this.ratioY);
                //Debug.WriteLine("stretch: " + distance.ToString());
            }
        }
    }
}
