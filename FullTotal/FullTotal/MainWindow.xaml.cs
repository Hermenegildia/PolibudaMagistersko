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
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System.Diagnostics;
using FullTotal.ImageTransformations;
using Kinect.Toolbox;

namespace FullTotal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   
        TransformSmoothParameters parameters;
        KinectSensorChooser kinectSensorChooser;
        KinectSensor sensor;

        //gestures and postures
        readonly ContextTracker contextTracker = new ContextTracker();
        StretchGestureDetector stretchGestureDetector;
        RotationGestureDetector rotationGestureDetector;
        bool isStretchGestureActive;
        bool isRotateGestureActive;
        AlgorithmicPostureDetector algorithmicPostureDetector = new AlgorithmicPostureDetector();
        int counterStretch = 0;
        int counterRotate = 0;
        


        public MainWindow()
        {
            InitializeComponent();
            isStretchGestureActive = false;
            isRotateGestureActive = false;
            
            parameters = new TransformSmoothParameters
            {
                //Smoothing = 0.5f,
                //Correction = 0.5f,
                //Prediction = 0.5f,
                //JitterRadius = 0.05f,
                //MaxDeviationRadius = 0.04f
                Smoothing = 0.75f,
                Correction = 0.07f,
                Prediction = 0.08f,
                JitterRadius = 0.08f,
                MaxDeviationRadius = 0.07f
            };
            
        }

        private void InitializeGestures()
        {
            var originalSize = this.zoomBorder.PointToScreen(new Point(this.zoomBorder.ActualWidth, this.zoomBorder.ActualHeight)) - this.zoomBorder.PointToScreen(new Point(0, 0));
            stretchGestureDetector = new StretchGestureDetector(this.sensor, this.zoomBorder, originalSize);
            stretchGestureDetector.MinimalPeriodBetweenGestures = 50;
            stretchGestureDetector.OnGestureWithDistanceDetected += stretchGestureDetector_OnGestureWithDistanceDetected;

            rotationGestureDetector = new RotationGestureDetector(this.sensor, this.zoomBorder, originalSize);
            //rotationGestureDetector.MinimalPeriodBetweenGestures = 50;
            rotationGestureDetector.OnGestureWithAngleDetected +=rotationGestureDetector_OnGestureWithAngleDetected;
        }

        private void rotationGestureDetector_OnGestureWithAngleDetected(string gestureName, double angle)
        {
            statusBarText.Text = gestureName + " " + angle.ToString();
            zoomBorder.SetRotationAngle(angle);
        }

        private void InitializePostures()
        {
            algorithmicPostureDetector.PostureDetected +=algorithmicPostureDetector_PostureDetected;
        }

        private void algorithmicPostureDetector_PostureDetected(string posture)
        {
            this.NameTextBlock.Text = posture;
        }

        private void stretchGestureDetector_OnGestureWithDistanceDetected(string gestureName, double totalRatio)
        {
            //statusBarText.Text = gestureName + " " + totalRatio.ToString();
            this.zoomBorder.SetZoomFactor(totalRatio);
            
        }

      
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser = new KinectSensorChooser();
            kinectSensorChooser.KinectChanged += kinectSencorChooser_KinectChanged;
            kinectSensorChooserUI.KinectSensorChooser = this.kinectSensorChooser;
            kinectSensorChooser.Start();

            //przypisz wlasciwosc "Kinect" sensorChooser'a do wlasciwosci "KinectSensorProperty" w this.kinectRegion
            var regionSensorBinding = new Binding("Kinect") { Source = this.kinectSensorChooser };
            BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);

            this.zoomBorder.AssignKinectRegion(this.kinectRegion);
            this.zoomBorder.OnStartStretchGestureFollowing += border_StartStretchGestureFollowing;
            this.zoomBorder.OnEndStretchGestureFollowing +=zoomBorder_EndStretchGestureFollowing;
            this.zoomBorder.OnStartRotateFestureFollowing +=zoomBorder_StartRotateFestureFollowing;
            this.zoomBorder.OnEndRotateFestureFollowing += zoomBorder_EndRotateFestureFollowing;
            this.zoomBorder.OnMoving +=zoomBorder_OnMoving;
        }

        private void zoomBorder_OnMoving(string position)
        {
            this.statusBarText.Text = position;
        }

        private void zoomBorder_EndRotateFestureFollowing()
        {
            counterRotate = 0;
            isRotateGestureActive = false;
        }

        private void zoomBorder_StartRotateFestureFollowing()
        {
            isRotateGestureActive = true;
        }

        private void zoomBorder_EndStretchGestureFollowing()
        {
            isStretchGestureActive = false;
            counterStretch = 0;
        }

        private void border_StartStretchGestureFollowing()
        {
            isStretchGestureActive = true;
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
                    //e.OldSensor.ColorFrameReady -= sensor_ColorFrameReady;
                    //e.OldSensor.DepthFrameReady -= sensor_DepthFrameReady;
                    //e.OldSensor.AllFramesReady -= sensor_AllFramesReady;
                    //this.SkeletonViewerControl.KinectDevice = null;
                    //this.kinectRegion.KinectSensor = null;
                    this.sensor = null;
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
                    this.sensor.SkeletonStream.Enable(parameters);

                    this.sensor.SkeletonFrameReady += sensor_SkeletonFrameReady;
                    //this.sensor.ColorFrameReady += sensor_ColorFrameReady;
                    //this.sensor.DepthFrameReady += sensor_DepthFrameReady;
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
                    this.statusBarText.Text = Properties.Resources.NoKinectReady;
                }
                if (this.sensor != null) //w miedzyczasie ktos mogl odlaczyc kinecta
                {
                    try
                    {
                        this.sensor.Start();
                        this.medicalImage.Visibility = Visibility.Visible;
                        this.statusBarText.Text = Properties.Resources.KinectReady;
                        InitializeGestures();
                        InitializePostures();
                    }
                    catch { }
                }

            }
            else
                this.statusBarText.Text = Properties.Resources.NoKinectReady;
        }

        //private void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        //{
        //}

        //private void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        //{
        //}

        private void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame == null)
                    return;
                Skeleton[] skeletons= new Skeleton[6];
                if (skeletons == null || skeletons.Length != frame.SkeletonArrayLength)
                {
                    skeletons = new Skeleton[frame.SkeletonArrayLength];
                }
                frame.CopySkeletonDataTo(skeletons);

                if (skeletons.All(s => s.TrackingState == SkeletonTrackingState.NotTracked))
                    return;

                ProcessFrame(frame, skeletons);
            }
        }

        private void ProcessFrame(SkeletonFrame frame, Skeleton[] skeletons)
        {
            //Dictionary<int, string> stabilities = new Dictionary<int, string>(); //sluzy do wyswitelania stablilites na liscie - tu nieuzywane!
            var skeletonId = TrackClosestSkeleton(skeletons);

            Skeleton closestSkeleton = skeletons.Where(skel => skel.TrackingId == skeletonId).FirstOrDefault();
            if (closestSkeleton != null && sensor != null)
            {
                if (closestSkeleton.TrackingState != SkeletonTrackingState.Tracked)
                    return;

                contextTracker.Add(closestSkeleton.Position.ToVector3(), closestSkeleton.TrackingId);
                //stabilities.Add(closestSkeleton.TrackingId, contextTracker.IsStableRelativeToCurrentSpeed(closestSkeleton.TrackingId) ? "Stable" : "Non stable");
                if (!contextTracker.IsStableRelativeToCurrentSpeed(closestSkeleton.TrackingId))
                    return;

                algorithmicPostureDetector.TrackPostures(closestSkeleton);
                if (isStretchGestureActive)
                {
                    if (closestSkeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked && closestSkeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
                    {
                        if (counterStretch == 0) //ustaw wartosci poczatkowe przy pierwszej iteracji
                        {
                            stretchGestureDetector.SetStartPosition(closestSkeleton);
                            counterStretch++;
                        }
                        else
                            stretchGestureDetector.Add(closestSkeleton);
                    }
                }
                if (isRotateGestureActive)
                {
                    if (closestSkeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked && closestSkeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
                    {
                        if (counterRotate == 0)
                        {
                            rotationGestureDetector.SetStartPosition(closestSkeleton);
                            counterRotate++;
                        }
                        else
                            rotationGestureDetector.Add(closestSkeleton);
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
                //sensor.DepthFrameReady -= sensor_DepthFrameReady;
                sensor.SkeletonFrameReady -= sensor_SkeletonFrameReady;
                //sensor.ColorFrameReady -= sensor_ColorFrameReady;
                this.sensor.Stop();
                sensor = null;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

    }
}
