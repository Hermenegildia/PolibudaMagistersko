using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using FullTotal.ViewModels;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;

namespace FullTotal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectController controller;
        TransformSmoothParameters parameters;
        KinectSensorChooser kinectSensorChooser;
        KinectSensor sensor;

        public MainWindow(KinectController controller)
        {
            InitializeComponent();

            //if (controller == null)
            //{
            //    throw new ArgumentNullException("controller", Properties.Resources.KinectControllerInvalid);
            //}

            //this.controller = controller;

            //controller.EngagedUserColor = (Color)this.Resources["EngagedUserColor"];
            //controller.TrackedUserColor = (Color)this.Resources["TrackedUserColor"];
            //controller.EngagedUserMessageBrush = (Brush)this.Resources["EngagedUserMessageBrush"];
            //controller.TrackedUserMessageBrush = (Brush)this.Resources["TrackedUserMessageBrush"];

            //this.kinectRegion.HandPointersUpdated += (sender, args) => controller.OnHandPointersUpdated(this.kinectRegion.HandPointers);

            //this.DataContext = controller;

            parameters = new TransformSmoothParameters
            {
                Smoothing = 0.75f,
                Correction = 0.07f,
                Prediction = 0.08f,
                JitterRadius = 0.08f,
                MaxDeviationRadius = 0.07f
            };
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
                    e.OldSensor.SkeletonFrameReady -= sensor_SkeletonFrameReady;
                    e.OldSensor.ColorFrameReady -= sensor_ColorFrameReady;
                    e.OldSensor.DepthFrameReady -= sensor_DepthFrameReady;
                    //e.OldSensor.AllFramesReady -= sensor_AllFramesReady;
                    //this.SkeletonViewerControl.KinectDevice = null;
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
                    //this.SkeletonViewerControl.KinectDevice = this.sensor;
                    this.sensor.SkeletonStream.Enable(parameters); //

                    this.sensor.SkeletonFrameReady += sensor_SkeletonFrameReady;
                    this.sensor.ColorFrameReady += sensor_ColorFrameReady;
                    this.sensor.DepthFrameReady += sensor_DepthFrameReady;
                    //this.sensor.AllFramesReady += sensor_AllFramesReady;
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
                if (this.sensor != null) //w miedzyczasie ktos mogl odlaczyc kinecta
                {
                    this.sensor.Start();
                    this.medicalImage.Visibility = Visibility.Visible;
                    this.statusBarText.Text = Properties.Resources.KinectReady;
                }

            }
            else
                this.statusBarText.Text = Properties.Resources.NoKinectReady;
        }

        private void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            
        }

        private void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
           
        }

        private void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }
    }
}
