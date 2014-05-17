using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectSimpleGesture
{
    class Program
    {
        static WaveGesture _gesture = new WaveGesture();

        static void Main(string[] args)
        {
            var sensor = KinectSensor.KinectSensors.Where(s => s.Status == KinectStatus.Connected).FirstOrDefault();

            try
            {
                Console.WriteLine("Status: " + sensor.Status);
            }
            catch { }
            if (sensor != null)
            {
            
                sensor.SkeletonStream.Enable();
                sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                sensor.SkeletonFrameReady += Sensor_SkeletonFrameReady;
                //sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                //var colorPixels = new byte[sensor.DepthStream.FramePixelDataLength * sizeof(int)];
                //sensor.ColorFrameReady += ColorImageReady;
                _gesture.GestureRecognized += Gesture_GestureRecognized;

                sensor.Start();
            }

            Console.ReadKey();
        }

        static void Sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (var frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    Skeleton[] skeletons = new Skeleton[frame.SkeletonArrayLength];
                    
                    frame.CopySkeletonDataTo(skeletons);

                    if (skeletons.Length > 0)
                    {
                        var user = skeletons.Where(u => u.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();

                        if (user != null)
                        {
                            //Console.WriteLine("jakiś szkielet");
                            _gesture.Update(user);
                        }
                    }
                }
            }
        }

        static void Gesture_GestureRecognized(object sender, EventArgs e)
        {
            Console.WriteLine("You just waved!");
        }

        //static void ColorImageReady(object sender, ColorImageFrameReadyEventArgs e)
        //{
        //    using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
        //    //using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
        //    {
        //        if (colorFrame != null)
        //        //if (depthFrame != null)
        //        {
        //            // Copy the pixel data from the image to a temporary array
        //            //depthFrame.CopyDepthImagePixelDataTo(this.depthPixels);
        //            colorFrame.CopyPixelDataTo(this.colorPixels);
        //            // Get the min and max reliable depth for the current frame
        //            //int minDepth = depthFrame.MinDepth;
        //            //int maxDepth = depthFrame.MaxDepth;

        //            //// Convert the depth to RGB
        //            //int colorPixelIndex = 0;
        //            //for (int i = 0; i < this.depthPixels.Length; ++i)
        //            //{
        //            //    // Get the depth for this pixel
        //            //    short depth = depthPixels[i].Depth;

        //            //    // To convert to a byte, we're discarding the most-significant
        //            //    // rather than least-significant bits.
        //            //    // We're preserving detail, although the intensity will "wrap."
        //            //    // Values outside the reliable depth range are mapped to 0 (black).

        //            //    // Note: Using conditionals in this loop could degrade performance.
        //            //    // Consider using a lookup table instead when writing production code.
        //            //    // See the KinectDepthViewer class used by the KinectExplorer sample
        //            //    // for a lookup table example.
        //            //    byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);

        //            //    // Write out blue byte
        //            //    this.colorPixels[colorPixelIndex++] = intensity;

        //            //    // Write out green byte
        //            //    this.colorPixels[colorPixelIndex++] = intensity;

        //            //    // Write out red byte                        
        //            //    this.colorPixels[colorPixelIndex++] = intensity;

        //            //    // We're outputting BGR, the last byte in the 32 bits is unused so skip it
        //            //    // If we were outputting BGRA, we would write alpha here.
        //            //    ++colorPixelIndex;
        //            //}

        //            // Write the pixel data into our bitmap
        //            this.colorBitmap.WritePixels(
        //                new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
        //                this.colorPixels,
        //                this.colorBitmap.PixelWidth * sizeof(int),
        //                0);
        //        }
        //    }
        //}
    }
}
