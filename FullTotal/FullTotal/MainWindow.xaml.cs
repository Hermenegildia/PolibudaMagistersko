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
        //readonly ContextTracker contextTracker = new ContextTracker();
        //StretchGestureDetector stretchGestureDetector;
        //RotationGestureDetector rotationGestureDetector;
        //bool isStretchGestureActive;
        //bool isRotateGestureActive;
        //AlgorithmicPostureDetector algorithmicPostureDetector = new AlgorithmicPostureDetector();
        //int counterStretch = 0;
        //int counterRotate = 0;

        List<ImagePath> imagesList;
        ImagePath selected = new ImagePath();

        bool isStarting = true;
        UserControl currentControl;
        UcImageSelection ucImageSelection;
        MainControl mainControl;

        public MainWindow(List< ImagePath> imagesList)
        {
            InitializeComponent();

            this.imagesList = imagesList;
            //this.medicalImage.Source = GetImageFormPath(imagesList[0].Path);
         

            //MainControl
            //isStretchGestureActive = false;
            //isRotateGestureActive = false;
            
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
            
            InitializeKinect();
            //if (isStarting)
            //{
            //    mainControl = new MainControl(this.kinectRegion, this.kinectSensorChooser.Kinect, GetImageFromPath(imagesList[0].Path));
            //    mainControl.OpenUcImageSelection += mainControl_OpenUcImageSelection;
            //    this.kinectRegion.Content = mainControl;
            //    isStarting = false;
            //}
           
            //MainControl
            //InitializeZoomBorder();
        }

        private void mainControl_OpenUcImageSelection()
        {
            ucImageSelection = new UcImageSelection(imagesList);
            ucImageSelection.ImageSuccessfullySelected += ucImageSelection_ImageSuccessfullySelected;
            this.kinectRegion.Content = ucImageSelection;
        }

        private void ucImageSelection_ImageSuccessfullySelected()
        {
            selected = ucImageSelection.GetSelectedImagePath();
            mainControl = new MainControl(this.kinectRegion, this.kinectSensorChooser.Kinect, GetImageFromPath(selected.Path));
            //usuń ewentualne stare powiązania i dodaj nowe
            mainControl.OpenUcImageSelection -= mainControl_OpenUcImageSelection;
            mainControl.OpenUcImageSelection += mainControl_OpenUcImageSelection;
            mainControl.Loaded -= mainControl_Loaded;
            mainControl.Loaded +=mainControl_Loaded;
            this.kinectRegion.Content = mainControl;
         
           
            //this.medicalImage.Source = GetImageFormPath(selected.Path);
        }

        private void mainControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.mainControl.InitializeGestures();
            this.mainControl.InitializePostures();
        }

        private Image GetImageFromPath(string path)
        {
            Image result = new Image();
            result.Source = GetImageSourceFormPath(path);
            return result;
        }

        private ImageSource GetImageSourceFormPath(string p)
        {
          
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(p);
            bi.EndInit();
           
            return bi;
        }

        //MainControl
        //private void InitializeGestures()
        //{
        //    var originalSize = this.zoomBorder.PointToScreen(new Point(this.zoomBorder.ActualWidth, this.zoomBorder.ActualHeight)) - this.zoomBorder.PointToScreen(new Point(0, 0));
        //    stretchGestureDetector = new StretchGestureDetector(this.sensor, this.zoomBorder, originalSize);
        //    stretchGestureDetector.MinimalPeriodBetweenGestures = 50;
        //    stretchGestureDetector.OnGestureWithDistanceDetected += stretchGestureDetector_OnGestureWithDistanceDetected;

        //    rotationGestureDetector = new RotationGestureDetector(this.sensor, this.zoomBorder, originalSize);
        //    //rotationGestureDetector.MinimalPeriodBetweenGestures = 50;
        //    rotationGestureDetector.OnGestureWithAngleDetected +=rotationGestureDetector_OnGestureWithAngleDetected;
        //}

        //MainControl
        //private void rotationGestureDetector_OnGestureWithAngleDetected(string gestureName, double angle)
        //{
        //    statusBarText.Text = gestureName + " " + angle.ToString();
        //    zoomBorder.SetRotationAngle(angle);
        //}

        //MainControl
        //private void InitializePostures()
        //{
        //    algorithmicPostureDetector.PostureDetected +=algorithmicPostureDetector_PostureDetected;
        //}

        //MainControl
        //private void algorithmicPostureDetector_PostureDetected(string posture)
        //{
        //    this.NameTextBlock.Text = posture;
        //}

        //MainControl
        //private void stretchGestureDetector_OnGestureWithDistanceDetected(string gestureName, double totalRatio)
        //{
        //    //statusBarText.Text = gestureName + " " + totalRatio.ToString();
        //    this.zoomBorder.SetZoomFactor(totalRatio);
        //}

      
        private void InitializeKinect()
        {
            kinectSensorChooser = new KinectSensorChooser();
            kinectSensorChooser.KinectChanged += kinectSencorChooser_KinectChanged;
            kinectSensorChooserUI.KinectSensorChooser = this.kinectSensorChooser;
            kinectSensorChooser.Start();

            //przypisz wlasciwosc "Kinect" sensorChooser'a do wlasciwosci "KinectSensorProperty" w this.kinectRegion
            var regionSensorBinding = new Binding("Kinect") { Source = this.kinectSensorChooser };
            BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);

            
        }

        //MainControl
        //private void InitializeZoomBorder()
        //{
        //    this.zoomBorder.AssignKinectRegion(this.kinectRegion);
        //    this.zoomBorder.OnStartStretchGestureFollowing += border_StartStretchGestureFollowing;
        //    this.zoomBorder.OnEndStretchGestureFollowing += zoomBorder_EndStretchGestureFollowing;
        //    this.zoomBorder.OnStartRotateFestureFollowing += zoomBorder_StartRotateFestureFollowing;
        //    this.zoomBorder.OnEndRotateFestureFollowing += zoomBorder_EndRotateFestureFollowing;
        //    this.zoomBorder.OnMoving += zoomBorder_OnMoving;
        //}

        //MainControl
        //private void zoomBorder_OnMoving(string position)
        //{
        //    //this.statusBarText.Text = position;
        //}

        //MainControl
        //private void zoomBorder_EndRotateFestureFollowing()
        //{
        //    counterRotate = 0;
        //    isRotateGestureActive = false;
        //}

        //MainControl
        //private void zoomBorder_StartRotateFestureFollowing()
        //{
        //    isRotateGestureActive = true;
        //}

        //MainControl
        //private void zoomBorder_EndStretchGestureFollowing()
        //{
        //    isStretchGestureActive = false;
        //    counterStretch = 0;
        //}

        //MainControl
        //private void border_StartStretchGestureFollowing()
        //{
        //    isStretchGestureActive = true;
        //}

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
                        //MainControl --> wywalone?
                        //this.medicalImage.Visibility = Visibility.Visible;
                        this.statusBarText.Text = Properties.Resources.KinectReady;

                        if (isStarting)
                        {
                            mainControl = new MainControl(this.kinectRegion, this.kinectSensorChooser.Kinect, GetImageFromPath(imagesList[0].Path));
                            mainControl.OpenUcImageSelection += mainControl_OpenUcImageSelection;
                            this.kinectRegion.Content = mainControl;
                            //MainControl
                            mainControl.Loaded += mainControl_Loaded;
                            isStarting = false;
                        }

                        
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
                //MainControl
                this.mainControl.MyContextTracker.Add(closestSkeleton.Position.ToVector3(), closestSkeleton.TrackingId);
                //stabilities.Add(closestSkeleton.TrackingId, contextTracker.IsStableRelativeToCurrentSpeed(closestSkeleton.TrackingId) ? "Stable" : "Non stable");
                if (!this.mainControl.MyContextTracker.IsStableRelativeToCurrentSpeed(closestSkeleton.TrackingId))
                    return;

                //MainControl
                this.mainControl.MyAlgorithmicPostureDetector.TrackPostures(closestSkeleton);
                if (this.mainControl.IsStretchGestureActive)
                {
                    if (closestSkeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked && closestSkeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
                    {
                        if (this.mainControl.CounterStretch == 0) //ustaw wartosci poczatkowe przy pierwszej iteracji
                        {
                            //HandPointer leftPointer = kinectRegion.HandPointers.FirstOrDefault(x => x.HandType == HandType.Left);
                            //HandPointer rightPointer = kinectRegion.HandPointers.FirstOrDefault(x => x.HandType == HandType.Right);
                            //stretchGestureDetector.SetStartPosition(leftPointer.GetPosition(this.kinectRegion, this.kinectRegion);
                            
                            this.mainControl.MyStretchGestureDetector.SetStartPosition(closestSkeleton);
                            this.mainControl.CounterStretch++;
                        }
                        else
                            this.mainControl.MyStretchGestureDetector.Add(closestSkeleton);
                    }
                }
                if (this.mainControl.IsRotateGestureActive)
                {
                    if (closestSkeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked && closestSkeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
                    {
                        if (this.mainControl.CounterRotate == 0)
                        {
                            this.mainControl.MyRotationGestureDetector.SetStartPosition(closestSkeleton);
                            this.mainControl.CounterRotate++;
                        }
                        else
                            this.mainControl.MyRotationGestureDetector.Add(closestSkeleton);
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (mainControl != null)
            {
                this.mainControl.InitializeGestures();
                this.mainControl.InitializePostures();
            }
        }

       //MainControl
        //private void KinectCircleButton_Click(object sender, RoutedEventArgs e)
        //{
        //    //ImageSelection imageSelection = new ImageSelection(this.kinectSensorChooser, imagesList);
        //    //imageSelection.ShowDialog();

        //    ucImageSelection = new UcImageSelection(imagesList);
        //    ucImageSelection.ImageSuccessfullySelected +=ucImageSelection_ImageSuccessfullySelected;
        //    this.kinectRegion.Content = ucImageSelection;
       
        //    //selected = imageSelection.GetSelectedImagePath();

        //    //selected = ucImageSelection.GetSelectedImagePath();
        //    //this.medicalImage.Source = GetImageFormPath(selected.Path);
        //    //InitializeComponent();
        //}

       

        
     
        //MainControl
        //private void KinectCircleButton_Click_ResetZoomable(object sender, RoutedEventArgs e)
        //{
        //    this.zoomBorder.Reset();
        //}

        //MainControl
        //private void KinectCircleButton_Click_SwitchStretchAndRotate(object sender, RoutedEventArgs e)
        //{
        //    if (this.stretchRotateButton.Label.ToString().Contains("Przybliż"))
        //    {
        //        this.zoomBorder.AssignGestureDetectionType(false);
        //        this.stretchRotateButton.Label = "Obrót";
        //    }
        //            else if (this.stretchRotateButton.Label.ToString().Contains("Obrót"))
        //    {

        //        this.zoomBorder.AssignGestureDetectionType(true);
        //        this.stretchRotateButton.Label = "Przybliż/oddal";
        //            }
        //}

    }
}
