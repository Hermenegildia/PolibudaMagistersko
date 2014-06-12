using Gestures;
using Gestures.Wave;
using Gestures.Stretch;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;



namespace GestureFollower
{
    /// <summary>
    /// Interaction logic for WindowWithSkeletonViewer.xaml
    /// </summary>
    public partial class WindowWithSkeletonViewer : Window
    {
        private KinectSensor sensor;
        private KinectSensorChooser kinectSensorChooser;
        WaveGesture waveGesture;
        StretchGesture stretchGesture;
        TransformSmoothParameters parameters;

        public WindowWithSkeletonViewer()
        {
            InitializeComponent();
            waveGesture = new WaveGesture();
            waveGesture.GestureDetected += new EventHandler(waveGesture_gestureDetected);
            stretchGesture = new StretchGesture();
            stretchGesture.GestureDetected += new EventHandler(stretchGesture_gestureDetected);

            parameters = new TransformSmoothParameters
            {
                Smoothing = 0.0f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 1.0f,
                MaxDeviationRadius = 0.05f
            };
           
        }

        private void stretchGesture_gestureDetected(object sender, EventArgs e)
        {
    
        }

        private void waveGesture_gestureDetected(object sender, EventArgs e)
        {
            gestureStateTB.Text = "waveeeeee...!";
                     var clearingthread = new Thread(ClearGestureStatusTextBlock);
            clearingthread.Start();
        }

        void ClearGestureStatusTextBlock(object playerIndex)
        {
            Thread.Sleep(TimeSpan.FromSeconds(5));
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
            {
                gestureStateTB.Text = "waiting for a gesture recognition.... player index: ";//+ (Int32)playerIndex;
            });
            
        }

        private void CheckBoxSeatedModeChanged(object sender, RoutedEventArgs e)
        {
            if (null != this.sensor)
            {
                if (this.checkBoxSeatedMode.IsChecked.GetValueOrDefault())
                {
                    this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                }
                else
                {
                    this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser = new KinectSensorChooser();
            kinectSensorChooser.KinectChanged += kinectSencorChooser_KinectChanged;
            kinectSensorChooserUI.KinectSensorChooser = this.kinectSensorChooser;
            kinectSensorChooser.Start();
        }

        private void kinectSencorChooser_KinectChanged(object sender, KinectChangedEventArgs e)
        {
             if (e.OldSensor != null)
            {
                try
                {
                    e.OldSensor.DepthStream.Range = DepthRange.Default;
                    e.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    e.OldSensor.DepthStream.Disable();
                    e.OldSensor.SkeletonStream.Disable();
                    e.OldSensor.SkeletonFrameReady -= SensorSkeletonFrameReady;
                    e.OldSensor.ColorFrameReady -= sensor_ColorFrameReady;
                    this.SkeletonViewerControl.KinectDevice = null;
                        
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }

           if (e.NewSensor != null)
           {
               try
               {
                   this.sensor = e.NewSensor;
                   this.sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                   this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                   this.SkeletonViewerControl.KinectDevice = this.sensor;
                   this.sensor.SkeletonStream.Enable(parameters);
                   this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;
                   this.sensor.ColorFrameReady += sensor_ColorFrameReady;
                   this.sensor.DepthFrameReady += sensor_DepthFrameReady;

                   try
                   {
                       this.sensor.DepthStream.Range = DepthRange.Near;
                       this.sensor.SkeletonStream.EnableTrackingInNearRange = true;
                   }
                   catch (InvalidOperationException)
                   {
                       // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                       this.sensor.DepthStream.Range = DepthRange.Default;
                       this.sensor.SkeletonStream.EnableTrackingInNearRange = false;
                   }
               }
               catch (InvalidOperationException)
               {
                   // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                   // E.g.: sensor might be abruptly unplugged.
               }
               this.sensor.Start();
           }
           else
               this.statusBarText.Text = Properties.Resources.NoKinectReady;
        }

        private void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {

            //using (DepthImageFrame frame = e.OpenDepthImageFrame())
            //{
            //    if (frame != null)
            //    { 
            //        CreatePlayerDepthImage(frame, this._RawDepthPixelData);
            //    }
            //}

        }

        //private void CreatePlayerDepthImage(DepthImageFrame depthFrame, short[] pixelData)
        //{
        //    int playerIndex;
        //    int depthBytePerPixel = 4;
        //    byte[] enhPixelData = new byte[depthFrame.Width * depthFrame.Height * depthBytePerPixel];


        //    for (int i = 0, j = 0; i < pixelData.Length; i++, j += depthBytePerPixel)
        //    {
        //        playerIndex = pixelData[i] & DepthImageFrame.PlayerIndexBitmask;

        //        if (playerIndex == 0)
        //        {
        //            enhPixelData[j] = 0xFF;
        //            enhPixelData[j + 1] = 0xFF;
        //            enhPixelData[j + 2] = 0xFF;
        //        }
        //        else
        //        {
        //            enhPixelData[j] = 0x00;
        //            enhPixelData[j + 1] = 0x00;
        //            enhPixelData[j + 2] = 0x00;
        //        }
        //    }


        //    this._EnhDepthImage.WritePixels(this._EnhDepthImageRect, enhPixelData, this._EnhDepthImageStride, 0);
        //}

        private void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
 	
        }

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame!= null)
                {
                    Skeleton[] skeletons = new Skeleton[sensor.SkeletonStream.FrameSkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                    waveGesture.Update(skeletons, frame.Timestamp);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        }
    }

