using GestureKinectTools.MathTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace GestureKinectTools.Gestures
{
    public class Entry
    {
        public DateTime Time { get; set; }
        public Vector3 Position { get; set; }
        public Ellipse DisplayEllipse { get; set; }
    }
}
