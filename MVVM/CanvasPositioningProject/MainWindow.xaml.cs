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

namespace CanvasPositioningProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DoStuff();
        }

        private void DoStuff()
        {
            // Add a Border
            Border myBorder = new Border();
            myBorder.HorizontalAlignment = HorizontalAlignment.Left;
            myBorder.VerticalAlignment = VerticalAlignment.Top;
            myBorder.BorderBrush = Brushes.Black;
            myBorder.BorderThickness = new Thickness(2);

            // Create the Canvas
            Canvas myCanvas = new Canvas();
            myCanvas.Background = Brushes.LightBlue;
            myCanvas.Width = 400;
            myCanvas.Height = 400;

            // Create the child Button elements
            Button myButton1 = new Button();
            Button myButton2 = new Button();
            Button myButton3 = new Button();
            Button myButton4 = new Button();

            // Set Positioning attached properties on Button elements
            Canvas.SetTop(myButton1, 50);
            myButton1.Content = "Canvas.Top=50";
            Canvas.SetBottom(myButton2, 50);
            myButton2.Content = "Canvas.Bottom=50";
            Canvas.SetLeft(myButton3, 50);
            myButton3.Content = "Canvas.Left=50";
            Canvas.SetRight(myButton4, 50);
            myButton4.Content = "Canvas.Right=50";


            // Add Buttons to the Canvas' Children collection
            myCanvas.Children.Add(myButton1);
            myCanvas.Children.Add(myButton2);
            myCanvas.Children.Add(myButton3);
            myCanvas.Children.Add(myButton4);

            // Add the Canvas as the lone Child of the Border
            myBorder.Child = myCanvas;

            // Add the Border as the Content of the Parent Window Object
            mainWindow.Content = myBorder;
            mainWindow.Show();

        }
    }
}
