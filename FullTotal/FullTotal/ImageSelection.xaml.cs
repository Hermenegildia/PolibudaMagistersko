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
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System.Globalization;
using System.IO;

namespace FullTotal
{
    /// <summary>
    /// Interaction logic for UcImageSelection.xaml
    /// </summary>
    public partial class ImageSelection : Window
    {

        public static readonly DependencyProperty PageLeftEnabledProperty = DependencyProperty.Register(
           "PageLeftEnabled", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty PageRightEnabledProperty = DependencyProperty.Register(
            "PageRightEnabled", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        private const double ScrollErrorMargin = 0.001;

        private const int PixelScrollByAmount = 20;

        private readonly KinectSensorChooser sensorChooser;

        List<ImagePath> imagesList = new List<ImagePath>();
        ImagePath selectedImagePath = new ImagePath();

        public ImageSelection(KinectSensorChooser sensorChooser, List<ImagePath> imagesPathsList)
        {
            InitializeComponent();
            imagesList = imagesPathsList;
            // initialize the sensor chooser and UI

            this.sensorChooser = sensorChooser;
            //this.sensorChooser.RequiredConnectionId = sensorChooser.RequiredConnectionId;
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            this.sensorChooser.Start();

            // Bind the sensor chooser's current sensor to the KinectRegion
            var regionSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);

            // Clear out placeholder content
            this.wrapPanel.Children.Clear();

            // Add in display content
            //for (var index = 0; index < 30; ++index)
            //{
            //    var button = new KinectTileButton { Label = (index + 1).ToString(CultureInfo.CurrentCulture) };
            //    this.wrapPanel.Children.Add(button);
            //}
            foreach (ImagePath path in imagesList)
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(path.Path);
                bi.EndInit();
                Image image = new Image();
                image.Source = bi;
                var button = new KinectTileButton { Label = path, Content = image};
                button.BorderBrush = null;
                this.wrapPanel.Children.Add(button);
            }

            // Bind listner to scrollviwer scroll position change, and check scroll viewer position
            this.UpdatePagingButtonState();
            scrollViewer.ScrollChanged += (o, e) => this.UpdatePagingButtonState();
        }

          

        /// <summary>
        /// CLR Property Wrappers for PageLeftEnabledProperty
        /// </summary>
        public bool PageLeftEnabled
        {
            get
            {
                return (bool)GetValue(PageLeftEnabledProperty);
            }

            set
            {
                this.SetValue(PageLeftEnabledProperty, value);
            }
        }

        /// <summary>
        /// CLR Property Wrappers for PageRightEnabledProperty
        /// </summary>
        public bool PageRightEnabled
        {
            get
            {
                return (bool)GetValue(PageRightEnabledProperty);
            }

            set
            {
                this.SetValue(PageRightEnabledProperty, value);
            }
        }

        /// <summary>
        /// Called when the KinectSensorChooser gets a new sensor
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="args">event arguments</param>
        private static void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }

            if (args.NewSensor != null)
            {
                try
                {
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();

                    try
                    {
                        args.NewSensor.DepthStream.Range = DepthRange.Near;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                    }
                    catch (InvalidOperationException)
                    {
                        // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    }
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }
        }


        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.sensorChooser.Stop();
        }

        private void KinectTileButtonClick(object sender, RoutedEventArgs e)
        {
            //wyczyść poprzednie ramki
            foreach (KinectTileButton but in wrapPanel.Children)
                but.BorderBrush = null;

            var button = (KinectTileButton)e.OriginalSource;
            //var selectionDisplay = new SelectionDisplay(button.Label as string);
            //this.kinectRegionGrid.Children.Add(selectionDisplay);
            button.BorderBrush = Brushes.Orange;
            button.BorderThickness = new Thickness(10);
            
            e.Handled = true;
        }

        /// <summary>
        /// Change button state depending on scroll viewer position
        /// </summary>
        private void UpdatePagingButtonState()
        {
            this.PageLeftEnabled = scrollViewer.HorizontalOffset > ScrollErrorMargin;
            this.PageRightEnabled = scrollViewer.HorizontalOffset < scrollViewer.ScrollableWidth - ScrollErrorMargin;
        }

        private void PageLeftButtonClick(object sender, RoutedEventArgs e)
        {
            KinectTileButton selectedButton = (from KinectTileButton x in this.wrapPanel.Children where x.BorderBrush != null select x).FirstOrDefault();
            if (selectedButton != null)
            {
                selectedImagePath = (ImagePath)selectedButton.Label;
                this.Close();
            }
        }

        public ImagePath GetSelectedImagePath()
        {
            return selectedImagePath;
        }
    }
}
