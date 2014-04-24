using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBConnection;

namespace connectionChecker
{
    public partial class Form1 : Form
    {
        Connection myConnection;
        public Form1()
        {
            InitializeComponent();
            myConnection = new Connection("sa", "mojeHaslo123");
           
            LoadDataFromDatabase();
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                myConnection.OpenConnection();
                dataGridView1.DataSource = myConnection.ExecuteQuery("SELECT * FROM Table_1");
                myConnection.CloseConnetcion();
            }
            catch
            {
                MessageBox.Show("Ups! Coś się nie powiodło!");
            }
        }
    }
}
