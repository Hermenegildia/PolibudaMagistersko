﻿using GestureKinectTools;
using GestureKinectTools.Context;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace GestureFollower
{
    /// <summary>
    /// Interaction logic for WindowWithTools.xaml
    /// </summary>
    public partial class WindowWithTools : Window
    {
        private KinectSensor sensor;
        private KinectSensorChooser kinectSensorChooser;
        readonly ColorStreamManager colorManager = new ColorStreamManager();
         SkeletonDisplayManager skeletonManager;
        Skeleton[] skeletons;

        string circleKBPath;
        string letterT_KBPath;

        readonly ContextTracker contextTracker = new ContextTracker();

        //public KinectSensor Sensor
        //{
        //    get { return kinectSensorChooser.Kinect; }
        //}

        public WindowWithTools()
        {
            InitializeComponent();
         
            InitializeKinect();
            this.DataContext = kinectSensorChooser;
        }

        private void InitializeKinect()
        {
            kinectSensorChooser = new KinectSensorChooser();
            kinectSensorChooser.KinectChanged += kinectSencorChooser_KinectChanged;
            //kinectSensorChooserUI.KinectSensorChooser = this.kinectSensorChooser;
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
                    e.OldSensor.DepthFrameReady -= sensor_DepthFrameReady;
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
                    this.sensor.SkeletonStream.Enable(new TransformSmoothParameters {
                                                 Smoothing = 0.5f,
                                                 Correction = 0.5f,
                                                 Prediction = 0.5f,
                                                 JitterRadius = 0.05f,
                                                 MaxDeviationRadius = 0.04f
                                             });//(parameters); 

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
                    // E.g.: sensor1 might be abruptly unplugged.
                }
                this.sensor.Start();

                kinectDisplay.DataContext = colorManager;
                //kinectRegion.KinectSensor = sensor;
                
                skeletonManager = new SkeletonDisplayManager(this.sensor, kinectCanvas);
              
            }
        }

        private void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (var frame = e.OpenDepthImageFrame())
            {
                if (frame == null)
                    return;
            }
        }

        private void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {

            using (var frame = e.OpenColorImageFrame())
            {
                if (frame == null)
                    return;

                colorManager.Update(frame);
            }
        }

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame == null)
                    return;
                frame.GetSkeletons(ref skeletons);
                if (skeletons.All(s => s.TrackingState == SkeletonTrackingState.NotTracked))
                    return;
                skeletonManager.Draw(skeletons);
                //skeletonDisplayManager.Draw(frame.Skeletons, seatedMode.IsChecked == true);
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            circleKBPath = Path.Combine(Environment.CurrentDirectory, @"circleKB.save");
            letterT_KBPath = Path.Combine(Environment.CurrentDirectory, @"t_KB.save");

        }
    }
}
