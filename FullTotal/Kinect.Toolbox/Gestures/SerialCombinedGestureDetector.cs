using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect.Toolbox
{
    public class SerialCombinedGestureDetector : CombinedGestureDetector
    {
        DateTime? previousGestureTime;
        List<string> detectedGesturesNames = new List<string>();

        public SerialCombinedGestureDetector(double epsilon = 1000)
            :base(epsilon)
        {
        }

        protected override void CheckGestures(string gesture)
        {
            var currentTime = DateTime.Now;

            bool condition = (!previousGestureTime.HasValue || detectedGesturesNames.Contains(gesture) || currentTime.Subtract(previousGestureTime.Value).TotalMilliseconds > Epsilon);

            if (condition)
                detectedGesturesNames.Clear();

            previousGestureTime = currentTime;

            detectedGesturesNames.Add(gesture);

            if (detectedGesturesNames.Count == GestureDetectorsCount)
            {
                RaiseGestureDetected(string.Join(" >", detectedGesturesNames));
                previousGestureTime = null;
            }
        }
    }
}
