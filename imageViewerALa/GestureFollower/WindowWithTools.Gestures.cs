using GestureKinectTools.Replay.Skeletons;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestureKinectTools;

namespace GestureFollower
{
    partial class WindowWithTools
    {
        void ProcessFrame(ReplaySkeletonFrame frame)
        {
            Dictionary<int, string> stabilities = new Dictionary<int, string>();
            foreach (var skeleton in frame.Skeletons)
            {
                if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
                    continue;

                contextTracker.Add(skeleton.Position.ToVector3(), skeleton.TrackingId);
                stabilities.Add(skeleton.TrackingId, contextTracker.IsStableRelativeToAverageSpeed(skeleton.TrackingId) ? "Stable" : "Non stable");
                if (!contextTracker.IsStableRelativeToCurrentSpeed(skeleton.TrackingId))
                    continue;

                //foreach (Joint joint in skeleton.Joints)
                //{
                //    if (joint.TrackingState != JointTrackingState.Tracked)
                //        continue;

                //    if (joint.JointType == JointType.HandRight)
                //    {
                //        swipeGestureRecognizer.Add(joint.Position, kinectSensor);
                //        circleGestureRecognizer.Add(joint.Position, kinectSensor);
                //    }
                //    //else if (joint.JointType == JointType.HandLeft && controlMouse.IsChecked == true)
                //    //{
                //    //    MouseController.Current.SetHandPosition(kinectSensor, joint, skeleton);
                //    //}
                //}

                //algorithmicPostureRecognizer.TrackPostures(skeleton);
                //templatePostureDetector.TrackPostures(skeleton);

                //if (recordNextFrameForPosture)
                //{
                //    templatePostureDetector.AddTemplate(skeleton);
                //    recordNextFrameForPosture = false;
                //}
            }

            skeletonManager.Draw(frame.Skeletons);

            stabilitiesList.ItemsSource = stabilities;
        }
    }
}
