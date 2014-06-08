using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Gestures.Common;

namespace Gestures.Stretch
{
    public class StretchGesture: Gesture
    {
        const int MOMENT_TIMEOUT = 5000;
     
        double handsDistance;
        StretchGestureTracker tracker;
     
        public StretchGesture()
        {
            handsDistance = 0.0;
            tracker = new StretchGestureTracker();
        }


        public override void Update(Skeleton[] skeletons, long frameTimeStamp)
        {
            if (skeletons != null)
            {
                for (int i = 0; i < skeletons.Length; i++)
                {
                    if (skeletons[i].TrackingState != SkeletonTrackingState.NotTracked)
                    {
                        Joint leftHand = skeletons[i].Joints[JointType.HandLeft];
                        Joint rightHand = skeletons[i].Joints[JointType.HandRight];
                        if (leftHand.TrackingState != JointTrackingState.NotTracked && rightHand.TrackingState != JointTrackingState.NotTracked)
                        {
                            double currentHandsDistance = CountHandsDistance(leftHand, rightHand);
                            if (tracker.State == GestureState.InProgress && tracker.Timestamp - frameTimeStamp < MOMENT_TIMEOUT)
                            {
                                tracker.UpdateState(GestureState.Failure, frameTimeStamp);
                            }
                            else
                            {
                                {
                                    double handsDistance = CountHandsDistance(leftHand, rightHand);
                                    if (handsDistance > tracker.HandsDistance + 0.05)
                                    {
                                        tracker.UpdateLength(handsDistance, frameTimeStamp);

                                    }
                                    
                                   
                                }

                            }
                        }
                    }
                }
            }
        }

        private static double CountHandsDistance(Joint leftHand, Joint rightHand)
        {
            double pythagoras1 = Math.Pow(rightHand.Position.X - leftHand.Position.X, 2);
            double pythagoras2 = Math.Pow(leftHand.Position.Y - rightHand.Position.Y, 2);
            return Math.Sqrt(pythagoras1 + pythagoras2);

        }

        private bool CheckHandElbowPosition(Skeleton skeleton)
        {
            if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ElbowLeft].Position.Y)
            {
                if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ElbowRight].Position.Y)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

    }
}
