using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureKinectTools.Record
{
    [FlagsAttribute]
    public enum KinectRecordOptions
    {
        Color = 1,
        Depth = 2,
        Skeletons = 4
    }
}
