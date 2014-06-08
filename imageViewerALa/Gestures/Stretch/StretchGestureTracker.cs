using Gestures.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestures.Stretch
{
    public class StretchGestureTracker
    {
            public int Iterator;
            public GestureState State;
            public long Timestamp;
            public double HandsDistance;

            const int FRAMES_IN_ROW = 5;


            public StretchGestureTracker()
            {
                Reset();
            }

            public void Reset()
            {
                Iterator = 0;
                State = GestureState.None;
                Timestamp = 0;
                HandsDistance = 0.0;
            }

            public void UpdateState(GestureState state, long timestamp)
            {
                this.State = state;
                this.Timestamp = timestamp;
            }

            public void UpdateLength(double handsDistance, long timestamp)
            {
                if (this.State == GestureState.InProgress)
                {
                    if (handsDistance > this.HandsDistance)
                    {
                        if (this.Iterator == FRAMES_IN_ROW)
                        {
                            this.State = GestureState.Success;
                            this.Iterator = 0;  
                        }
                    }
                }
                this.Timestamp = timestamp;
                this.HandsDistance = handsDistance;
                this.Iterator++;
            }
    }
}
