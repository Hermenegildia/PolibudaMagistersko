using Gestures;
using Gestures.Wave;
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
using System.Windows.Shapes;


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

        public WindowWithSkeletonViewer()
        {
            InitializeComponent();
            waveGesture = new WaveGesture();
            waveGesture.GestureDetected += new EventHandler(waveGesture_gestureDetected);
        }

        private void waveGesture_gestureDetected(object sender, EventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Kiwka była!");
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
                   this.sensor.SkeletonStream.Enable();
                   this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;
                   this.sensor.ColorFrameReady += sensor_ColorFrameReady;
                   

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

