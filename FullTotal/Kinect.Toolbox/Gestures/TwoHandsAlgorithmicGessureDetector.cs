using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect.Toolbox
{
   public class TwoHandsAlgorithmicGessureDetector: GestureDetector
    {
       readonly List<EntryKinect> leftEntries = new List<EntryKinect>();
        protected readonly string gestureName;
        KinectSensor sensor;

        public List<EntryKinect> LeftEntries
       {
           get { return leftEntries; }
       }

        public KinectSensor Sensor
        {
            get { return sensor; }
            set { sensor = value; }
        }

        public TwoHandsAlgorithmicGessureDetector(KinectSensor sensor, string gestureName, int windowSize = 1)
            : base(windowSize)
        {
            this.gestureName = gestureName;
            this.sensor = sensor;
        }

       public override void Add(SkeletonPoint position, KinectSensor sensor)
       {
           base.Add(position, sensor);
       }

       public void Add(Skeleton skeleton)
       {
           SkeletonPoint position ;

           if (skeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
           {
               position = skeleton.Joints[JointType.HandRight].Position;
               EntryKinect rightEntry = new EntryKinect { Position = position.ToVector3(), SkeletonPosition=position, Time = DateTime.Now };
               Entries.Add(rightEntry);

               if (Entries.Count > WindowSize)
               {
                   EntryKinect entryToRemove = (EntryKinect)Entries[0];
                   Entries.Remove(entryToRemove);
               }
           }

           if (skeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked)
           {
               position = skeleton.Joints[JointType.HandLeft].Position;
               EntryKinect leftEntry = new EntryKinect { Position = position.ToVector3(), SkeletonPosition = position, Time = DateTime.Now };
               LeftEntries.Add(leftEntry);

               if (LeftEntries.Count > WindowSize)
               {
                   EntryKinect entryToRemove = LeftEntries[0];
                   LeftEntries.Remove(entryToRemove);
               }
           }

           LookForGesture();
       }

       protected override void LookForGesture()
       {
           
       }
    }
}
