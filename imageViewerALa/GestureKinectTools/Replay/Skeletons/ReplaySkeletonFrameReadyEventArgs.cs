using System;

namespace GestureKinectTools.Replay.Skeletons
{
    public class ReplaySkeletonFrameReadyEventArgs : EventArgs
    {
        public ReplaySkeletonFrame SkeletonFrame { get; set; }
    }
}
