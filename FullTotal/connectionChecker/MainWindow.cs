using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormControls;


namespace connectionChecker
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Error er = new Error("Wystąpił błąd! Spradź poprawność loginu i hasła do bazy danych \n");
            er.ShowDialog();
        }
    }
}
