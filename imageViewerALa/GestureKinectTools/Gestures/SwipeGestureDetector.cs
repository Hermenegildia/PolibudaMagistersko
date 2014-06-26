using GestureKinectTools.MathTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureKinectTools.Gestures
{
    public class SwipeGestureDetector: GestureDetector
    {
        public float SwipeMinLength { get; set; }
        public float SwipeMaxHeight { get; set; }
        public int SwipeMinDuration { get; set; }
        public int SwipeMaxDuration { get; set; }

        public SwipeGestureDetector(int iterationCount = 20) :
            base(iterationCount)
        {
            SwipeMinLength = 0.4f;
            SwipeMaxHeight = 0.2f;
            SwipeMinDuration = 250;
            SwipeMaxDuration = 1500;
        }

        protected bool ScanPositions(Func<Vector3, Vector3, bool> heightFunction, Func<Vector3, Vector3, bool> directionFunction,
            Func<Vector3, Vector3, bool> lengthFunction, int minTime, int maxTime)
        {
            int start = 0;
            
            for (int i = 0; i < Entries.Count - 1; i++)
            {
                if (!heightFunction(Entries[0].Position, Entries[i].Position) || !directionFunction(Entries[i].Position, Entries[i + 1].Position))
                    start = i;

                if (lengthFunction(Entries[i].Position, Entries[start].Position))
                {
                    double totalMiliseconds = (Entries[i].Time - Entries[start].Time).TotalMilliseconds;

                    if (totalMiliseconds >= minTime && totalMiliseconds <= maxTime)
                        return true;
                }

            }
            
            return false;
        }

        protected override void LookForGesture()
        {
            //swipe to right
          
            Func<Vector3, Vector3, bool> func1 = (p1, p2) => {return Math.Abs(p2.Y-p1.Y) < SwipeMaxHeight;};
            Func<Vector3, Vector3, bool> func2 = (p1, p2) => {return p2.X-p1.X > -0.1f;}; //przesuniecie w prawo
            Func<Vector3, Vector3, bool> func3 = (p1, p2) => {return Math.Abs(p2.X-p1.X) < SwipeMinLength;};

            bool condition = ScanPositions(func1, func2, func3, SwipeMinDuration, SwipeMaxDuration);

            if (condition)
            {
                RaiseGestureDetected("SwipeToRight");
                return;
            }

            //swipe to left
            func2 = (p1, p2) => { return p2.X - p1.X < 0.1f; }; //przesuniecie w lewo

            condition = ScanPositions(func1, func2, func3, SwipeMinDuration, SwipeMaxDuration);
            if (condition)
            {
                RaiseGestureDetected("SwipeToLeft");
                return;
            }

        }
    }
}
