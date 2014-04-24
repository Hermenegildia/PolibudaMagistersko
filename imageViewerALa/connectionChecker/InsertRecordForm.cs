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
using System.Collections;

namespace connectionChecker
{
    public partial class InsertRecordForm : Form
    {
        Connection myConnection;
        DataTable myTable;

        public InsertRecordForm(Connection conn)
        {
            InitializeComponent();
            myConnection = conn;
        }

        private DataTable InsertDataToDatabase(string name, string last_name)
        {
            try
            {
                Hashtable param = new Hashtable();
                param["name"] = name;
                param["last_name"] = last_name;
                myConnection.OpenConnection();
                DataTable result = myConnection.ExecuteQuery("INSERT INTO Table_1 (name, last_name) VALUES (@name, @last_name)", param);
                myConnection.CloseConnetcion();
                return result;
            }
            catch
            {
                MessageBox.Show("Ups! Coś się nie powiodło!");
                return new DataTable();
            }
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            try
            {
                string name = tbName.Text;
                string lastName = tbLastName.Text;
                myTable = InsertDataToDatabase(name, lastName);
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            catch 
            {
                DialogResult = System.Windows.Forms.DialogResult.Abort;
            }
        }

        public DataTable GetDataTable()
        {
            return myTable;
        }

    }
}
