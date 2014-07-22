using Kinect.Toolbox;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
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

namespace NewRegion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor sensor;
        private KinectSensorChooser kinectSensorChooser;
        StretchGestureDetector stretchGestureRecognizer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSensorChooser();
            this.DataContext = kinectSensorChooser;
         
        }

        private void InitializeGestureDetectors()
        {
            stretchGestureRecognizer = new StretchGestureDetector(sensor);
            stretchGestureRecognizer.OnGestureDetected += OnGestureDetected;
        }

        private void OnGestureDetected(string gesture)
        {
            gestureStateTB.Text = (string.Format("{0} : {1}", gesture, DateTime.Now.TimeOfDay));
            
        }

        private void InitializeSensorChooser()
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
                    this.sensor.SkeletonStream.Enable(new TransformSmoothParameters
                    {
                        Smoothing = 0.5f,
                        Correction = 0.5f,
                        Prediction = 0.5f,
                        JitterRadius = 0.05f,
                        MaxDeviationRadius = 0.04f
                    });//(parameters); 

                    this.sensor.SkeletonFrameReady += sensor_SkeletonFrameReady;
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
                kinectRegion.KinectSensor = sensor;
              
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
            }
        }

        private void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (var frame = e.OpenSkeletonFrame())
            {
                if (frame == null)
                    return;

                Skeleton[] skeletons = new Skeleton[6];
                frame.CopySkeletonDataTo(skeletons);

                if (skeletons.All(s => s.TrackingState == SkeletonTrackingState.NotTracked))
                    return;

                int skeletonId = TrackClosestSkeleton(skeletons);
                   Skeleton closestSkeleton = skeletons.Where(skel => skel.TrackingId == skeletonId).FirstOrDefault();
                   if (closestSkeleton != null && sensor != null)
                   {
                       if (closestSkeleton.TrackingState != SkeletonTrackingState.Tracked)
                           return;
                       if (closestSkeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked && closestSkeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked)
                       {
                           stretchGestureRecognizer.Add(closestSkeleton);
                       }
                   }
            }
        }

        private int TrackClosestSkeleton(Skeleton[] skeletons)
        {
            if (this.sensor != null)
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
            else return -1;

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                sensor.DepthFrameReady -= sensor_DepthFrameReady;
                sensor.SkeletonFrameReady -= sensor_SkeletonFrameReady;
                sensor.ColorFrameReady -= sensor_ColorFrameReady;
                this.sensor.Stop();
                sensor = null;
            }

            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeGestureDetectors();
        }

      
    }
}
