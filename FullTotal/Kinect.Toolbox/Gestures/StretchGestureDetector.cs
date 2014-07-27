using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Kinect.Toolbox
{
   public class StretchGestureDetector: TwoHandsAlgorithmicGessureDetector
    {
       float distance = 0.0f;


       public StretchGestureDetector(KinectSensor sensor, string gestureName = "stretch", int windowSize = 1) :
           base(sensor, gestureName, windowSize)
       {
       }

       public override void Add(Microsoft.Kinect.SkeletonPoint position, Microsoft.Kinect.KinectSensor sensor) //tego nie uzywamy
       {
           base.Add(position, sensor);
       }


       protected bool ScanPositions()
       {
           if (Entries.Count > 0 && LeftEntries.Count > 0)
           {
               var vec1 = Tools.Convert(Sensor, ((EntryKinect)Entries[0]).SkeletonPosition);
               var vec2 = Tools.Convert(Sensor, ((EntryKinect)LeftEntries[0]).SkeletonPosition);
               distance = (vec1 - vec2).Length;

               return true;
           }
           else
               return false;

       }

       protected void RaiseGestureDetected(string gesture, float distance)
       {
           // Too close?
           if (DateTime.Now.Subtract(lastGestureDate).TotalMilliseconds > MinimalPeriodBetweenGestures)
           {
               if (OnGestureDetected != null)
                   OnGestureDetected(gesture);

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
               RaiseGestureDetected(gestureName + " " + distance.ToString());
               //Debug.WriteLine("stretch: " + distance.ToString());
           }
       }
    }
}
