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

        //kinectRegion properties
        bool isRightGripInteraction = false;
        bool isLeftGripInteraction = false;
        object lastLeftControl;
        object lastRightControl;

        delegate void HandGripHandler(HandPointer handPointer);
        event HandGripHandler OnHandGripRelease;
        event HandGripHandler OnHandGrip;

        public MainWindow()
        {
            InitializeComponent();


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

            var regionSensorBinding = new Binding("Kinect") { Source = this.kinectSensorChooser };
            BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);

            KinectRegion.AddQueryInteractionStatusHandler(this.medicalImage, OnQuery); //obsluga medicalImage przez kinectRegion
            KinectRegion.AddQueryInteractionStatusHandler(this.border, OnQuery); //obsluga zoomborder przez kinectRegion

            this.OnHandGripRelease +=MainWindow_OnHandGripRelease; //zapis na moje zdarzenie, ze puszczona reka
            this.OnHandGrip +=MainWindow_OnHandGrip;
            //KinectRegion.AddHandPointerLeaveHandler(this.medicalImage, OnPointerLeave); 
        }

        private void MainWindow_OnHandGrip(HandPointer handPointer)
        {
            if (handPointer.HandType == HandType.Right)
            {
                //this.medicalImage.CaptureMouse();
                
            }
        }


        //gdy reka wyjezdza za obrazek
        //private void OnPointerLeave(object sender, HandPointerEventArgs e)
        //{
        //    if (e.HandPointer.HandType == HandType.Left)
        //        isLeftGripInteraction = false;
        //    else if (e.HandPointer.HandType == HandType.Right)
        //        isRightGripInteraction = false;
        //}

        private void MainWindow_OnHandGripRelease(HandPointer handPointer)
        {
            if (handPointer.HandType == HandType.Left)
            {
                var position = handPointer.GetPosition(this);
                this.border.RotateLeft(-5, position.X, position.Y);
            }
                //this.border.Reset();
            //else if (handType == HandType.Right)
                //this.medicalImage.ReleaseMouseCapture();
        }

        private void OnQuery(object sender, QueryInteractionStatusEventArgs e)
        {
            if (e.HandPointer.HandType == HandType.Right)
            {

                //If a grip detected change the cursor image to grip
                if (e.HandPointer.HandEventType == HandEventType.Grip)
                {
                    isRightGripInteraction = true;
                    e.IsInGripInteraction = true;

                    if (OnHandGrip != null)
                        OnHandGrip(e.HandPointer);
                }

                //If Grip Release detected change the cursor image to open
                else if (e.HandPointer.HandEventType == HandEventType.GripRelease)
                {
                    isRightGripInteraction = false;
                    e.IsInGripInteraction = false;
                }

                //If no change in state do not change the cursor
                else if (e.HandPointer.HandEventType == HandEventType.None)
                {
                    e.IsInGripInteraction = isRightGripInteraction;
                }

                lastRightControl = e.Source;
            }
            else if (e.HandPointer.HandType == HandType.Left)
            {

                if (e.HandPointer.HandEventType == HandEventType.Grip)
                {
                    isLeftGripInteraction = true;
                    e.IsInGripInteraction = true;
                }

                //If Grip Release detected change the cursor image to open
                else if (e.HandPointer.HandEventType == HandEventType.GripRelease)
                {
                    isLeftGripInteraction = false;
                    e.IsInGripInteraction = false;

                    if (e.Source.GetType() == typeof(ZoomBorder) || e.Source.GetType()==typeof(Image))
                    {  
                            if (OnHandGripRelease != null)
                                OnHandGripRelease(e.HandPointer);
                    }
                }
                //If no change in state do not change the cursor
                else if (e.HandPointer.HandEventType == HandEventType.None)
                {
                    e.IsInGripInteraction = isLeftGripInteraction;
                }
            }
            //this.statusBarText.Text = (e.HandPointer.GetPosition(this.medicalImage)).Y.ToString();
            e.Handled = true;
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
                    this.sensor.SkeletonStream.Enable(parameters);

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
                sensor.DepthFrameReady -= sensor_DepthFrameReady;
                sensor.SkeletonFrameReady -= sensor_SkeletonFrameReady;
                sensor.ColorFrameReady -= sensor_ColorFrameReady;
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
