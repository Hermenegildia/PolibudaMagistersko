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
using System.Windows.Forms;
using Microsoft.Kinect;

namespace imageViewerALa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> fileNames;
        string path;
        int iterator;
        Point currentPoint;
        Point startPoint;
        Rectangle rectangle;

        public MainWindow()
        {
            InitializeComponent();
            fileNames = new List<string>();
            InitializeFilesPath();
        }

        private void InitializeFilesPath()
        {
            //System.Windows.MessageBox.Show("Witaj w programie! Wybierz lokalizację plików do wyświetlenia");
            //FolderBrowserDialog dialog = new FolderBrowserDialog();
            //if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    path = dialog.SelectedPath;
            //    labelPath.Content = path;
            path = @"C:\Users\alA\Desktop";
            string[] buf = System.IO.Directory.GetFiles(path, "*.jpg");
            for (int i = 0; i < buf.Length; i++)
                fileNames.Add(buf[i]);
            iterator = 0;
            ShowPicture(fileNames[iterator]);
            this.Topmost = true;
            //}
        }

        private void ShowPicture(string p)
        {
            imagePicture.Source = new BitmapImage(new Uri(p));
            labelPath.Content = p;
         
        }

       
       
        private void imagePicture_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if (iterator < fileNames.Count-1)
            //    iterator++;
            //else
            //    iterator = 0;
            //ShowPicture(fileNames[iterator]);
            CanvasControl.Children.Clear();
            startPoint = e.GetPosition(CanvasControl);
            rectangle = new Rectangle();
            rectangle.Stroke = Brushes.OrangeRed;
            rectangle.StrokeThickness = 3;

            Canvas.SetLeft(rectangle, startPoint.X);
            Canvas.SetTop(rectangle, startPoint.X);
            CanvasControl.Children.Add(rectangle);
            //if (e.ButtonState == MouseButtonState.Pressed)
            //    currentPoint = e.GetPosition(this);
        }

        private void imagePicture_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released || rectangle == null)
                return;

            var pos = e.GetPosition(CanvasControl);

            var x = Math.Min(pos.X, startPoint.X);
            var y = Math.Min(pos.Y, startPoint.Y);

            var w = Math.Max(pos.X, startPoint.X) - x;
            var h = Math.Max(pos.Y, startPoint.Y) - y;

            rectangle.Width = w;
            rectangle.Height = h;

            Canvas.SetLeft(rectangle, x);
            Canvas.SetTop(rectangle, y);

            //if (e.LeftButton == MouseButtonState.Pressed)
            //{   
            //    Line line = new Line();

            //    line.Stroke = SystemColors.WindowFrameBrush;
            //    line.X1 = currentPoint.X;
            //    line.Y1 = currentPoint.Y;
            //    line.X2 = e.GetPosition(this).X;
            //    line.Y2 = e.GetPosition(this).Y;

            //    currentPoint = e.GetPosition(this);

            //    CanvasControl.Children.Add(line);
            //}
        }

        private void imagePicture_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UIElement uie = CanvasControl.Children[0];
            
            rectangle = null;
        }

        
   
    }
}
