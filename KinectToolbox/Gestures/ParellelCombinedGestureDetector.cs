using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect.Toolbox
{
    public class ParellelCombinedGestureDetector: CombinedGestureDetector
    {
        DateTime? firstDetectedGestureTime;
        List<string> detectedGesturesNames = new List<string>();

        public ParellelCombinedGestureDetector(double epsilon = 1000)
            : base(epsilon)
        {
        }

        protected override void CheckGestures(string gesture)
        {
            bool condition = (!firstDetectedGestureTime.HasValue || DateTime.Now.Subtract(firstDetectedGestureTime.Value).TotalMilliseconds >Epsilon ||detectedGesturesNames.Contains(gesture));
            if (condition)
            {
                firstDetectedGestureTime = DateTime.Now;
                detectedGesturesNames.Clear();
            }

            detectedGesturesNames.Add(gesture);

            if (detectedGesturesNames.Count == GestureDetectorsCount)
            {
                RaiseGestureDetected(string.Join(" &", detectedGesturesNames));
                firstDetectedGestureTime = null;
            }
        }
    }
}
