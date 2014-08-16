using Kinect.Toolbox;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FullTotal
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    public partial class MainControl : UserControl
    {
        //gestures and postures
        public readonly ContextTracker MyContextTracker = new ContextTracker();
        public StretchGestureDetector MyStretchGestureDetector;
        public RotationGestureDetector MyRotationGestureDetector;
        public bool IsStretchGestureActive;
        public bool IsRotateGestureActive;
        public AlgorithmicPostureDetector MyAlgorithmicPostureDetector = new AlgorithmicPostureDetector();
        public int CounterStretch = 0;
        public int CounterRotate = 0;

        public delegate void MyVoidDelegateForEvents();
        public event MyVoidDelegateForEvents OpenUcImageSelection;
        
        KinectSensor sensor;

        public MainControl(KinectRegion kinectRegion, KinectSensor sensor, UIElement zoomBorderChild)
        {
            InitializeComponent();

            IsStretchGestureActive = false;
            IsRotateGestureActive = false;

            InitializeZoomBorder(kinectRegion, zoomBorderChild);
            this.sensor = sensor;

            //InitializePostures();
            //InitializeGestures();
            
        }

        private void InitializeZoomBorder(KinectRegion region, UIElement child)
        {
            this.zoomBorder.Child = child;
            this.zoomBorder.AssignKinectRegion(region);
            this.zoomBorder.OnStartStretchGestureFollowing += border_StartStretchGestureFollowing;
            this.zoomBorder.OnEndStretchGestureFollowing += zoomBorder_EndStretchGestureFollowing;
            this.zoomBorder.OnStartRotateFestureFollowing += zoomBorder_StartRotateFestureFollowing;
            this.zoomBorder.OnEndRotateFestureFollowing += zoomBorder_EndRotateFestureFollowing;
            //this.zoomBorder.OnMoving += zoomBorder_OnMoving;
        }

        private void border_StartStretchGestureFollowing()
        {
            IsStretchGestureActive = true;
        }
       
        private void zoomBorder_EndStretchGestureFollowing()
        {
            IsStretchGestureActive = false;
            CounterStretch = 0;
        }

        private void zoomBorder_StartRotateFestureFollowing()
        {
            IsRotateGestureActive = true;
        }

        private void zoomBorder_EndRotateFestureFollowing()
        {
            CounterRotate = 0;
            IsRotateGestureActive = false;
        }

        public void InitializeGestures()
        {
            var originalSize = this.zoomBorder.PointToScreen(new Point(this.zoomBorder.ActualWidth, this.zoomBorder.ActualHeight)) - this.zoomBorder.PointToScreen(new Point(0, 0));
            MyStretchGestureDetector = new StretchGestureDetector(this.sensor, this.zoomBorder, originalSize);
            MyStretchGestureDetector.MinimalPeriodBetweenGestures = 50;
            MyStretchGestureDetector.OnGestureWithDistanceDetected += stretchGestureDetector_OnGestureWithDistanceDetected;

            MyRotationGestureDetector = new RotationGestureDetector(this.sensor, this.zoomBorder, originalSize);
            //rotationGestureDetector.MinimalPeriodBetweenGestures = 50;
            MyRotationGestureDetector.OnGestureWithAngleDetected += rotationGestureDetector_OnGestureWithAngleDetected;
        }

        public void InitializePostures()
        {
            MyAlgorithmicPostureDetector.PostureDetected += algorithmicPostureDetector_PostureDetected;
        }

        private void algorithmicPostureDetector_PostureDetected(string posture)
        {
           //todo: Ala wykorzystać do blookowania i odblokowania ekranu!
        }

        private void stretchGestureDetector_OnGestureWithDistanceDetected(string gestureName, double totalRatio)
        {
            this.zoomBorder.SetZoomFactor(totalRatio);
        }

        private void rotationGestureDetector_OnGestureWithAngleDetected(string gestureName, double angle)
        {
            zoomBorder.SetRotationAngle(angle);
        }

        private void KinectCircleButton_Click_ResetZoomable(object sender, RoutedEventArgs e)
        {
            this.zoomBorder.Reset();
        }

        private void KinectCircleButton_Click_SwitchStretchAndRotate(object sender, RoutedEventArgs e)
        {
            if (this.stretchRotateButton.Label.ToString().Contains("Przybliż"))
            {
                this.zoomBorder.AssignGestureDetectionType(false);
                this.stretchRotateButton.Label = "Obróć obraz";
            }
            else if (this.stretchRotateButton.Label.ToString().Contains("Obróć"))
            {

                this.zoomBorder.AssignGestureDetectionType(true);
                this.stretchRotateButton.Label = "Przybliż/oddal";
            }
        }

        private void KinectCircleButton_Click(object sender, RoutedEventArgs e)
        {
            if (OpenUcImageSelection != null)
                OpenUcImageSelection();
        }

      

       
    }
}
