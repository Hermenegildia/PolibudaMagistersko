using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace connectionChecker.AppPreparation
{
    public partial class ChooseDoc : Form
    {
        string path = string.Empty;
        public ChooseDoc()
        {
            InitializeComponent();
        }

        private void btPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog.SafeFileName;      
            }

        }

        public string GetSelectedPath()
        {
            return path;
        }
    }
}
