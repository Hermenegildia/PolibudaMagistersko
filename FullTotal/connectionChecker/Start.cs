using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace connectionChecker
{
    public partial class Start : Form
    {
        List<string> imagePaths = new List<string>();

        public Start()
        {
            InitializeComponent();
           
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            ofd.Filter = "jpg files (*.jpg)|*.jpg|All files (*.*)|*.*";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (ofd.CheckPathExists)
                {
                    List<string> fileNamesList = ofd.FileNames.ToList<string>();
                    foreach (string fileName in fileNamesList)
                        imagePaths.Add(fileName);
                    lbPhotos.DataSource = imagePaths;
                    lbPhotos.Refresh();
                }
            }
        }

      
    }
}
