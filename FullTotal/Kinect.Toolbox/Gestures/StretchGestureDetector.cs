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
        //double distanceLeft = 0;
        //double distanceRight = 0;
        double distanceTotal = 0;
        //double distance = 0;
        KinectRegion kinectRegion;


        public delegate void GestureDetection(string gestureName, double distance);
        public event GestureDetection OnGestureWithDistanceDetected;


        public StretchGestureDetector(KinectSensor sensor,KinectRegion kinectRegion,  string gestureName = "stretch", int windowSize = 5) :
            base(sensor, gestureName, windowSize)
        {
            this.kinectRegion = kinectRegion;
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
                var pointRightCurrent = Tools.GetJointPoint(Sensor, kinectRegion, ((EntryKinect)Entries[WindowSize-1]).SkeletonPosition);
                var pointLeftCurrent = Tools.GetJointPoint(Sensor, kinectRegion, ((EntryKinect)LeftEntries[WindowSize-1]).SkeletonPosition);
                //Debug.WriteLine("dłonie: Right = " + pointRight.X + " rightY = " + pointRight.Y);
                if (pointRightCurrent.X > pointLeftCurrent.Y) //tylko gdy prawa po prawej - coś dziwnego z pointami o.O,
                    //todo: Ala ten warunek nie działa :/
                {
                    //double currentRightDistance = (pointRightCurrent - pointRightEnd).Length;
                    //double currentLeftDistance = (pointLeftCurrent - pointLeftEnd).Length;
                    double currentTotalDistance = (pointRightCurrent - pointLeftCurrent).Length;
                    //przeskalowanie
                   

                    if(Math.Abs( currentTotalDistance-distanceTotal) > 50) //jesli zmiana dystansu miedzy dlonmi, a nie tylko przesuniecie!
                    {
                        distanceTotal = currentTotalDistance;
                        return true;
                    }

                    //distanceLeft = currentLeftDistance;
                    //distanceRight = currentRightDistance;
                    
                    return false;
                }
                else
                    return false;
            }
            else
                return false;

        }

        protected void RaiseGestureDetected(string gesture, double distance)
        {
            // Too close?
            if (DateTime.Now.Subtract(lastGestureDate).TotalMilliseconds > MinimalPeriodBetweenGestures)
            {
                //if (OnGestureDetected != null)
                //    OnGestureDetected(gesture);

                if (OnGestureWithDistanceDetected != null)
                    OnGestureWithDistanceDetected(gesture, distance);

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
                RaiseGestureDetected(gestureName, distanceTotal);
                //Debug.WriteLine("stretch: " + distance.ToString());
            }
        }
    }
}
