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
using WinFormControls;


namespace connectionChecker
{
    public partial class StartupWindow : Form
    {
        Connection myConnection;

        public StartupWindow()
        {
            InitializeComponent();
            tbLogin.Text = Properties.Settings.Default.dbLogin;
            tbPassword.Text = Properties.Settings.Default.dbPssword;
        }

       

        private void button1_Click(object sender, EventArgs e)
        {
            //ReadLoginParams();
            //ShowHideLabels();
            //try
            //{
            //    myConnection = new Connection(Properties.Settings.Default.dbLogin, Properties.Settings.Default.dbPssword);
            //    int i = 0;
            //    while (i < 10000)
            //    {
            //        progressBar1.PerformStep();
            //        i++;
            //    }
                //Form1 mainWindow = new Form1(myConnection);
            Form1 mainWindow = new Form1(); //bez bazy, żeby było szybciej

                progressBar1.PerformStep();
                mainWindow.FormClosed += new FormClosedEventHandler(mainWindow_Closed);

                this.Hide();
                mainWindow.Show();
            //}
            //catch (Exception ex)
            //{
            //    Error er = new Error("Wystąpił błąd! Spradź poprawność loginu i hasła do bazy danych \n" + ex);
            //    er.ShowDialog();
            //}
        }

        private void ReadLoginParams()
        {
            Properties.Settings.Default.dbLogin = tbLogin.Text;
            Properties.Settings.Default.dbPssword = tbPassword.Text;
            Properties.Settings.Default.Save();
        }

        private void ShowHideLabels()
        {
            lbInfo.Visible = false;
            lbWait.Visible = true;
            lbTitle.Visible = true;
            lbWait.Refresh();
            lbTitle.Refresh();
        }

        private void mainWindow_Closed(object sender, FormClosedEventArgs e)
        {
            this.Close();
        }

        private void tbLogin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                btLogIn.PerformClick();
            }
        }

        

     
      
       
    }
}
