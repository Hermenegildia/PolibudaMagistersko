using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace Gestures
{
    public class BroadenGesture: Gesture
    {
        double[,] vector_length;
        int iterator;

        public BroadenGesture()
        {
            InitializeVectorLengthArray();
            iterator = 0;
        }

        private void InitializeVectorLengthArray()
        {
            vector_length = new double[6,5];
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j< 5; j++)
                    vector_length[i,j] = 0.0;
            }
        }

        public override void Update(Skeleton[] skeletons, long frameTimeStamp)
        {
            if (skeletons != null)
            {
                for (int i = 0; i < skeletons.Length; i++)
                {
                    if (CheckHandElbowPosition(skeletons[i]))
                    {
                        double pythagoras1 = Math.Pow(skeletons[i].Joints[JointType.HandRight].Position.X - skeletons[i].Joints[JointType.HandLeft].Position.X, 2);
                        double pythagoras2 = Math.Pow(skeletons[i].Joints[JointType.HandLeft].Position.Y - skeletons[i].Joints[JointType.HandRight].Position.Y,2);
                        vector_length[i,iterator] = Math.Sqrt(pythagoras1 + pythagoras2);
                        
                        if (iterator != 4)
                            iterator++;
                        else
                            iterator = 0;
                        
                    }

                }
            }
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

          private void TrackGesture()
          {
          }

    }
}
