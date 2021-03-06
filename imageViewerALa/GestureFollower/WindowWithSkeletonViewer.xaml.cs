﻿using Gestures;
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
        int skeletonId;
        int depthId;

        public WindowWithSkeletonViewer()
        {
            InitializeComponent();
            waveGesture = new WaveGesture();
            waveGesture.GestureDetected += new EventHandler(waveGesture_gestureDetected);
            stretchGesture = new StretchGesture();
            stretchGesture.GestureDetected += new EventHandler(stretchGesture_gestureDetected);
          

            parameters = new TransformSmoothParameters
            {
                Smoothing = 0.75f,
                Correction = 0.07f,
                Prediction = 0.08f,
                JitterRadius = 0.08f,
                MaxDeviationRadius = 0.07f
            };
           
        }

        private void stretchGesture_gestureDetected(object sender, EventArgs e)
        {
    
        }

        private void waveGesture_gestureDetected(object sender, EventArgs e)
        {
            gestureStateTB.Text = "waveeeeee...!";
            var clearingthread = new Thread(() => ClearGestureStatusTextBlock(skeletonId, depthId));
                     clearingthread.Start(); //skeletonId
        }

        void ClearGestureStatusTextBlock(int playerIndex, int playerIndexDepth)
        {
            Thread.Sleep(TimeSpan.FromSeconds(5));
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
            {
                gestureStateTB.Text = "waiting for a gesture recognition.... player index: " + playerIndex + " / " + playerIndexDepth;
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
            //kinectRegion.add
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
                    //e.OldSensor.SkeletonFrameReady -= SensorSkeletonFrameReady;
                    //e.OldSensor.ColorFrameReady -= sensor_ColorFrameReady;
                    //e.OldSensor.DepthFrameReady -= sensor_DepthFrameReady;
                    e.OldSensor.AllFramesReady -= sensor_AllFramesReady;
                    this.SkeletonViewerControl.KinectDevice = null;
                    this.kinectRegion.KinectSensor = null;
                        
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
                   this.sensor.SkeletonStream.Enable(parameters); //
                   
                   //this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;
                   //this.sensor.ColorFrameReady += sensor_ColorFrameReady;
                   //this.sensor.DepthFrameReady += sensor_DepthFrameReady;
                   this.sensor.AllFramesReady += sensor_AllFramesReady;
                   this.kinectRegion.KinectSensor = this.sensor;
               
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

        private void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            DepthImageFrame depthFrame = e.OpenDepthImageFrame();
            SkeletonFrame skeletonFrame = e.OpenSkeletonFrame();

            if (skeletonFrame != null) //skeleton processing
            {
                if (depthFrame != null)
                {
                    Skeleton[] skeletons = new Skeleton[sensor.SkeletonStream.FrameSkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                    skeletonId = TrackClosestSkeleton(skeletons);
                    short[] depthArray = new short[this.sensor.DepthStream.FramePixelDataLength];
                    depthFrame.CopyPixelDataTo(depthArray);
                    depthId = ScanDepthForPlayer(depthArray);
                    waveGesture.Update(skeletons, skeletonFrame.Timestamp);
                }
            }
        }

        private int ScanDepthForPlayer(short[] depthArray) //todo: Ala sprawdzić na kilku osobach jak działa playerindexing!
        {
            int id = 0;
            for (int i = 0; i < depthArray.Length; i++)
            {
                int buf = (depthArray[i] & DepthImageFrame.PlayerIndexBitmask);
                if (buf != 0)
                    id = buf;

            }
            return id;
        }

        private int TrackClosestSkeleton(Skeleton[] skeletons)
        {

            if (!this.sensor.SkeletonStream.AppChoosesSkeletons)
            {
                this.sensor.SkeletonStream.AppChoosesSkeletons = true; // Ensure AppChoosesSkeletons is set
            }

            float closestDistance = 10000f; // Start with a far enough distance
            int closestID = 0;

            foreach (Skeleton skeleton in skeletons.Where(s => s.TrackingState != SkeletonTrackingState.NotTracked))
            {
                if (skeleton.Position.Z < closestDistance)
                {
                    closestID = skeleton.TrackingId;
                    closestDistance = skeleton.Position.Z;
                }
            }

            if (closestID > 0)
            {
                this.sensor.SkeletonStream.ChooseSkeletons(closestID); // Track this skeleton
            }

            return closestID;
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
            //using (SkeletonFrame frame = e.OpenSkeletonFrame())
            //{
            //    if (frame!= null)
            //    {
            //        Skeleton[] skeletons = new Skeleton[sensor.SkeletonStream.FrameSkeletonArrayLength];
            //        frame.CopySkeletonDataTo(skeletons);
            //        waveGesture.Update(skeletons, frame.Timestamp);
            //    }
            //}
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

