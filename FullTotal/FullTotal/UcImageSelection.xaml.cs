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
    /// Interaction logic for UcImageSelection.xaml
    /// </summary>
    public partial class UcImageSelection : UserControl
    {
        private const double ScrollErrorMargin = 0.001;

        private const int PixelScrollByAmount = 20;

        List<ImagePath> imagesList = new List<ImagePath>();
        ImagePath selectedImagePath = new ImagePath();

        public delegate void ControlEnd();
        public event ControlEnd ImageSuccessfullySelected;

        public UcImageSelection(List<ImagePath> imagesPathsList)
        {
            InitializeComponent();
            imagesList = imagesPathsList;

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
                var button = new KinectTileButton { Label = path, Content = image };
                button.BorderBrush = null;
                this.wrapPanel.Children.Add(button);
            }

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

      
        private void PageLeftButtonClick(object sender, RoutedEventArgs e)
        {
            KinectTileButton selectedButton = (from KinectTileButton x in this.wrapPanel.Children where x.BorderBrush != null select x).FirstOrDefault();
            if (selectedButton != null)
            {
                selectedImagePath = (ImagePath)selectedButton.Label;

                if (ImageSuccessfullySelected != null)
                    ImageSuccessfullySelected();
            }
        }

        public ImagePath GetSelectedImagePath()
        {
            return selectedImagePath;
        }
    }
}
