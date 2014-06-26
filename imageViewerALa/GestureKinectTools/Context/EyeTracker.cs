using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.FaceTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureKinectTools.Context
{
    public class EyeTracker: IDisposable
    {
        FaceTracker faceTracker;
        KinectSensor sensor;
        byte[] colors;
        short[] depths;
        float epsilon;

        public bool? IsLookingTowardsSensor
        {
            get;
            private set;
        }

        public EyeTracker(KinectSensor sensor, float epsilon = 0.02f)
        {
            this.sensor = sensor;
            faceTracker = new FaceTracker(this.sensor);
            this.epsilon = epsilon;
        }

        public void Track(Skeleton skeleton, ColorImageFrame colorFrame, DepthImageFrame depthFrame)
        {
            if (colorFrame != null && depthFrame != null)
            {
                colors = new byte[colorFrame.PixelDataLength];
                depths = new short[depthFrame.PixelDataLength];
               
                var faceFrame = faceTracker.Track(sensor.ColorStream.Format, colors, sensor.DepthStream.Format, depths, skeleton);

                if (faceFrame == null)
                {
                    IsLookingTowardsSensor = null;
                    return;
                }

                var shape = faceFrame.Get3DShape();
                var leftEyeZ = shape[FeaturePoint.AboveMidUpperLeftEyelid].Z;
                var rightEyeZ = shape[FeaturePoint.AboveMidUpperRightEyelid].Z;

                IsLookingTowardsSensor = Math.Abs(leftEyeZ - rightEyeZ) <= epsilon;
           }
        }
        public void Dispose()
        {
            faceTracker.Dispose();
        }
     
    }
}
