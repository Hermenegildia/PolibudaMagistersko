﻿using System;
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
        ErrorProvider ep1 = new ErrorProvider();
        ErrorProvider ep2 = new ErrorProvider();

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
                myConnection.CloseConnection();
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
                ep1.SetError(tbName, String.Empty);
                ep2.SetError(tbLastName,String.Empty);
                if (tbName.Text != string.Empty && tbLastName.Text != string.Empty)
                {
                    string name = tbName.Text;
                    string lastName = tbLastName.Text;
                    myTable = InsertDataToDatabase(name, lastName);
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                {
                    if (tbName.Text == String.Empty)
                    {
                        ep1.BlinkStyle = ErrorBlinkStyle.NeverBlink;
                        ep1.SetIconAlignment(tbName, ErrorIconAlignment.MiddleRight);
                        ep1.SetError(tbName, "Uzupełnij imię!");
                    }

                    if (tbLastName.Text == String.Empty)
                    {
                        ep2.BlinkStyle = ErrorBlinkStyle.NeverBlink;
                        ep2.SetIconAlignment(tbLastName, ErrorIconAlignment.MiddleRight);
                        ep2.SetError(tbLastName, "Uzupełnij nazwisko!");
                    }
                    return;
                }
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

      
        private void tbLastName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                btOK.PerformClick();
            }
        }

        private void tbName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                btOK.PerformClick();
            }
        }

    }
}
