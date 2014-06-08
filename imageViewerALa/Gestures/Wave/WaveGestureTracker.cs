using Gestures.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestures.Wave
{
    
        public struct WaveGestureTracker
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
    }
