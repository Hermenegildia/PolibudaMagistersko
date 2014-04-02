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

        public MainWindow()
        {
            InitializeComponent();
            fileNames = new List<string>();
            InitializeFilesPath();
        }

        private void InitializeFilesPath()
        {
            System.Windows.MessageBox.Show("Witaj w programie! Wybierz lokalizację plików do wyświetlenia");
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = dialog.SelectedPath;
                labelPath.Content = path;
                string[] buf = System.IO.Directory.GetFiles(path, "*.jpg");
                for (int i = 0; i < buf.Length; i++)
                    fileNames.Add(buf[i]);
                iterator = 0;
                ShowPicture(fileNames[iterator]);
                
            }
        }

        private void ShowPicture(string p)
        {
            imagePicture.Source = new BitmapImage(new Uri(p));
            labelPath.Content = p;
        }

       
        private void imagePicture_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (iterator < fileNames.Count-1)
                iterator++;
            else
                iterator = 0;
            ShowPicture(fileNames[iterator]);
        }

        
   
    }
}
