using System;

namespace GestureKinectTools.Replay.Depth
{
    public class ReplayDepthImageFrameReadyEventArgs : EventArgs
    {
        public ReplayDepthImageFrame DepthImageFrame { get; set; }
    }
}
