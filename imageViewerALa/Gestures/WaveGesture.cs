//************************************************************************************************************************************************
//Copyright (c) 2012 Jarrett Webb & James Ashley                                                                                                *
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files              *
//(the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge,           *
//publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,        *
//subject to the following conditions:                                                                                                          *
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.                *
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF            *
//MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE           *
//FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION         *
//WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                                                                               *
//***********************************************************************************************************************************************


using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Gestures
{
    public class WaveGesture: Gesture
    {
        const float WAVE_TRESHOLD = 0.1f;
        const int WAVE_MOMENT_TIMEOUT = 5000;
        const int REQUIRED_ITERATIONS = 4;
        const int LEFT_HAND = 0;
        const int RIGHT_HAND = 1;

        WaveGestureTracker[,] playerWaveTracker = new WaveGestureTracker[6, 2];
        public event EventHandler GestureDetected;

        public void Update(Skeleton[] skeletons, long frameTimeStamp)
        {
            if (skeletons != null)
            {

                for (int i = 0; i < skeletons.Length; i++)
                {
                    if (skeletons[i].TrackingState != SkeletonTrackingState.NotTracked)
                    {
                        TrackWave(skeletons[i], true, ref this.playerWaveTracker[i, LEFT_HAND], frameTimeStamp);
                        TrackWave(skeletons[i], false, ref this.playerWaveTracker[i, RIGHT_HAND], frameTimeStamp);
                    }
                    else
                    {
                        this.playerWaveTracker[i, LEFT_HAND].Reset();
                        this.playerWaveTracker[i, RIGHT_HAND].Reset();
                    }
                }
            }
        }

        private void TrackWave(Skeleton skeleton, bool isLeft, ref WaveGestureTracker tracker, long timeStamp)
        {
            JointType handJointId = (isLeft) ? JointType.HandLeft : JointType.HandRight;
            JointType elbowJointId = (isLeft) ? JointType.ElbowLeft : JointType.ElbowRight;
            
            Joint hand = skeleton.Joints[handJointId];
            Joint elbow = skeleton.Joints[elbowJointId];

            if (hand.TrackingState != JointTrackingState.NotTracked && elbow.TrackingState != JointTrackingState.NotTracked)
            {
                if (tracker.State == GestureState.InProgress && tracker.Timestamp + WAVE_MOMENT_TIMEOUT < timeStamp)
                {
                    tracker.UpdateState(GestureState.Failure, timeStamp);
                }
                else if (hand.Position.Y > elbow.Position.Y)
                {
                    if (hand.Position.X <= elbow.Position.X - WAVE_TRESHOLD)
                        tracker.UpdatePosition(WavePosition.Left, timeStamp);
                    else if (hand.Position.X >= elbow.Position.X + WAVE_TRESHOLD)
                        tracker.UpdatePosition(WavePosition.Right, timeStamp);
                    else
                        tracker.UpdatePosition(WavePosition.Neutral, timeStamp);

                    if (tracker.State != GestureState.Success && tracker.IterationCount == REQUIRED_ITERATIONS)
                    {
                        tracker.UpdateState(GestureState.Success, timeStamp);
                        System.Diagnostics.Debug.WriteLine("Success!");

                        if (GestureDetected != null)
                        {
                            GestureDetected(this, new EventArgs());
                        }
                    }
                }
                else
                {
                    if (tracker.State == GestureState.InProgress)
                    {
                        tracker.UpdateState(GestureState.Failure, timeStamp);
                        //System.Diagnostics.Debug.WriteLine("Fail!");
                    }
                    else
                        tracker.Reset();
                }
            }
            else
                tracker.Reset();
        }


        #region Helpers
        enum WavePosition
        {
            None = 0,
            Left = 1,
            Right = 2,
            Neutral = 3
        }

        //enum WaveGestureState
        //{
        //    None = 0,
        //    Success = 1,
        //    Failure = 2,
        //    InProgress = 3
        //}

        struct WaveGestureTracker
        {
            public int IterationCount;
            public GestureState State;
            public WavePosition StartPosition;
            public WavePosition CurrentPosition;
            public long Timestamp;

            public void Reset()
            {
                IterationCount = 0;
                State = GestureState.None;
                Timestamp = 0;
                StartPosition = WavePosition.None;
                CurrentPosition = WavePosition.None;
            }

            public void UpdateState(GestureState state, long timestamp)
            {
                State = state;
                Timestamp = timestamp;
            }

            public void UpdatePosition(WavePosition position, long timestamp)
            {
                if (CurrentPosition != position)
                {
                    if (position == WavePosition.Left || position == WavePosition.Right)
                    {
                        if (State != GestureState.InProgress)
                        {
                            State = GestureState.InProgress;
                            IterationCount = 0;
                            StartPosition = position;
                        }
                        IterationCount++;
                    }
                    CurrentPosition = position;
                    Timestamp = timestamp;
                }

            }
        }
        #endregion Helpers
    }


}

