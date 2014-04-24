using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration; 
using DBConnection;
using CustomControls;
using System.Xaml;
using System.Collections;

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

      


        private void button1_Click(object sender, EventArgs e)
        {
            LoadDataFromDatabase();

            var wpfwindow = new MyDllWindow();
            ElementHost.EnableModelessKeyboardInterop(wpfwindow);
            wpfwindow.ShowWindow();
            
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            InsertRecordForm insertRecordForm = new InsertRecordForm(myConnection);
            if (insertRecordForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dataGridView1.DataSource = insertRecordForm.GetDataTable();
                LoadDataFromDatabase();
            }
        }
    }
}