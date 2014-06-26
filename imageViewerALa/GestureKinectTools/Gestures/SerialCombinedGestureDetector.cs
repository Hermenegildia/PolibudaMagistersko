using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureKinectTools.Gestures
{
    public class SerialCombinedGestureDetector: CombinedGestureDetector
    {
        DateTime? previousGestureTime;
        List<string> detectedGesturesName;

        public SerialCombinedGestureDetector(double epsilon = 1000)
            : base(epsilon)
        {
            detectedGesturesName = new List<string>();
        }

        protected override void CheckGestures(string gesture)
        {
            var currentTime = DateTime.Now;

            if (!previousGestureTime.HasValue || detectedGesturesName.Contains(gesture) || currentTime.Subtract(previousGestureTime.Value).TotalMilliseconds > Epsilon)
            {
                detectedGesturesName.Clear();
            }

            previousGestureTime = currentTime;

            detectedGesturesName.Add(gesture);

            if (detectedGesturesName.Count == GestureDetectorsCount)
            {
                RaiseGestureDetected(string.Join(">", detectedGesturesName));
                previousGestureTime = null;
            }
        }
    }
}
