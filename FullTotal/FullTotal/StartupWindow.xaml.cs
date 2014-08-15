
using Microsoft.Win32;
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
using System.Collections;

namespace FullTotal
{
    /// <summary>
    /// Interaction logic for StartupWindow.xaml
    /// </summary>
    public partial class StartupWindow : Window
    {
        

        List<ImagePath> imagesList = new List<ImagePath>();

        public StartupWindow()
        {
            InitializeComponent();
            this.imagesListBox.DataContext = imagesList;
        }

        

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            ofd.Filter = "jpg files (*.jpg)|*.jpg|All files (*.*)|*.*";
            ofd.RestoreDirectory = true;
            var dialogResult = ofd.ShowDialog();
            if (dialogResult!= null && dialogResult.Value)
            {
                if (ofd.CheckPathExists)
                {
                    List<string> fileNamesList = ofd.FileNames.ToList<string>();
                    foreach (string fileName in fileNamesList)
                    {
                        var imagePath = new ImagePath(fileName);
                        imagesList.Add(imagePath);
                        this.imagesListBox.Items.Add(imagePath);
                    }
                }
            }
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            if (imagesListBox.SelectedItem != null)
            {
                this.imagesList.Remove((ImagePath)imagesListBox.SelectedItem);
                this.imagesListBox.Items.RemoveAt(imagesListBox.SelectedIndex);
            }
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            if (this.imagesListBox.Items.Count == 0)
                MessageBox.Show("Musisz wybrać co najmniej jeden obraz!");
            else
            {
                MainWindow mainWindow = new MainWindow(this.imagesList);
                this.Hide();
                var result = mainWindow.ShowDialog();
               this.Close();
            }
        }
    }
}
