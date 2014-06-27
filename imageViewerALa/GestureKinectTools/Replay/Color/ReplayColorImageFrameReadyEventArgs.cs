using System;

namespace GestureKinectTools.Replay.Color
{
    public class ReplayColorImageFrameReadyEventArgs : EventArgs
    {
        public ReplayColorImageFrame ColorImageFrame { get; set; }
    }
}
