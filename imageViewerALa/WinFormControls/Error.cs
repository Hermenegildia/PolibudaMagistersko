using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormControls
{
    public partial class Error : Form
    {
        public Error()
        {
            InitializeComponent();
        }

        public Error(string errorText)
        {
            InitializeComponent();
            this.tbText.Text = errorText;
            this.tbText.Visible = true;
            //this.tbText.ReadOnly = true;
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
