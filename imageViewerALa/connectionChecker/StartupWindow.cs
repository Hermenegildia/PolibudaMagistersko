using DBConnection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace connectionChecker
{
    public partial class StartupWindow : Form
    {
        Connection myConnection;

        public StartupWindow()
        {
            InitializeComponent();
          
        }

        private void StartupWindow_Shown(object sender, EventArgs e)
        {
           button1_Click(this, new EventArgs());

        }

      

        private void button1_Click(object sender, EventArgs e)
        {
            lbWait.Refresh();
            lbTitle.Refresh();
            myConnection = new Connection("sa", "mojeHaslo123");
            int i = 0;
            while (i < 10000)
            {
                progressBar1.PerformStep();
                i++;
            }
            Form1 mainWindow = new Form1(myConnection);
            progressBar1.PerformStep();
            mainWindow.FormClosed += new FormClosedEventHandler(mainWindow_Closed);
            this.Hide();
            mainWindow.Show();
        }

        private void mainWindow_Closed(object sender, FormClosedEventArgs e)
        {
            this.Close();
        }

       
    }
}
