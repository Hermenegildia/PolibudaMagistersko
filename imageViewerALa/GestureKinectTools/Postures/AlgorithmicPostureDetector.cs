using GestureKinectTools.MathTools;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureKinectTools.Postures
{
    public class AlgorithmicPostureDetector: PostureDetector
    {
        public float Epsilon { get; set; }
        public float MaxRange { get; set; }

        public AlgorithmicPostureDetector() :
            base(10)
        {
            Epsilon = 0.1f;
            MaxRange = 0.25f;
        }

        public override void TrackPosture(Skeleton skeleton)
        {
            if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
                return;

            Vector3? headPosition = null;
            Vector3? leftHandPosition = null;
            Vector3? rightHandPosition = null;

            foreach (Joint joint in skeleton.Joints)
            {
                if (joint.TrackingState != JointTrackingState.Tracked)
                    continue;

                switch (joint.JointType)
                {
                    case JointType.Head:
                        headPosition = joint.Position.ToVector3();
                        break;
                    case JointType.HandLeft:
                        leftHandPosition = joint.Position.ToVector3();
                        break;
                    case JointType.HandRight:
                        rightHandPosition = joint.Position.ToVector3();
                        break;
                }

                if (CheckHandsTogether(rightHandPosition, leftHandPosition))
                {
                    RaisePostureDetected("HandsJoined");
                    return;
                }

                if (CheckHandOverHead(leftHandPosition, headPosition))
                {
                    RaisePostureDetected("LeftHandOverHead");
                    return;
                }

                if (CheckHandOverHead(rightHandPosition, headPosition))
                {
                    RaisePostureDetected("RightHandOverHead");
                    return;
                }

                if (CheckHello(headPosition, leftHandPosition))
                {
                    RaisePostureDetected("LeftHello");
                    return;
                }

                if (CheckHello(headPosition, rightHandPosition))
                {
                    RaisePostureDetected("RightHello");
                    return;
                }

                Reset();
            }

        }

        private bool CheckHandsTogether(Vector3? rightHandPosition, Vector3? leftHandPosition)
        {
            if (!leftHandPosition.HasValue || !rightHandPosition.HasValue)
                return false;

            float distance = (leftHandPosition.Value - rightHandPosition.Value).Length;

            if (distance > Epsilon)
                return false;

            return true;
        }

        private bool CheckHandOverHead(Vector3? handPosition, Vector3? headPosition)
        {
            if (!headPosition.HasValue || !handPosition.HasValue)
                return false;

            if (headPosition.Value.Y > handPosition.Value.Y)
                return false;

            if (Math.Abs(handPosition.Value.X - headPosition.Value.X) < MaxRange)
                return false;

            if (Math.Abs(handPosition.Value.Z - headPosition.Value.Z) < MaxRange)
                return false;

            return true;
        }

        private bool CheckHello(Vector3? headPosition, Vector3? handPosition)
        {
            if (!headPosition.HasValue || !handPosition.HasValue)
                return false;

            if (Math.Abs(handPosition.Value.X - headPosition.Value.X) < MaxRange)
                return false;

            if (Math.Abs(handPosition.Value.Y - headPosition.Value.Y) > MaxRange)
                return false;

            if (Math.Abs(handPosition.Value.Z - headPosition.Value.Z) > MaxRange)
                return false;

            return true;
        }

     
    }
}
