using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Kinect.Toolbox.Learning_Machine
{
    [Serializable]
    public class RecordedPathDouble: RecordedPath
    {
         List<Vector2> leftHandPoints;
         List<Vector2> rightHandPoints;

        public List<Vector2> LeftHandPoints
        {
            get { return leftHandPoints; }
            set { leftHandPoints = value; }
        }

        public List<Vector2> RightHandPoints
        {
            get { return rightHandPoints; }
            set { rightHandPoints = value; }
        }

        public RecordedPathDouble(int samplesCount) :
            base(samplesCount)
        {
            
        }

        new protected  WriteableBitmap GetDisplayBitmap()
        {
            points = JoinPoints();
            base.GetDisplayBitmap();
        }

        private List<Vector2> JoinPoints()
        {
           
        }
    }
}
