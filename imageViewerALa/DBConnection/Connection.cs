using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;
using System.Collections;


namespace DBConnection
{
    public class Connection
    {
        SqlConnection myConnection;
        SqlTransaction myTransaction;
        string dbUserName;
        string dbPassword;

        public Connection(string db_us_name, string db_pass)
        {
            dbUserName = db_us_name;
            dbPassword = db_pass;
            CreateConnection();
        }

        private void CreateConnection()
        {
            myConnection = new SqlConnection("User ID=" + dbUserName +
                                            ";Password=" + dbPassword +
                                            ";Server=ALICJA-HP\\SQLEXPRESS" +
                                            //";Database=bazka" +
                                            ";Trusted_Connection=false" +
                                            ";connection timeout=0");

            OpenConnection();
            DataTable databases = ExecuteQuery("SELECT * FROM master.dbo.sysdatabases WHERE name = 'bazia'");
            if (databases.Rows.Count < 1)
            {
                CloseConnetcion();
                myConnection.Open();

                ExecuteNonQuery(Queries.CreateDatabase("bazia"));
                CloseConnetcion();
                //todo: bazia stworzona, teraz podłączyć się do bazi i w bazi stworzyć tabelę!
                ExecuteNonQuery(Queries.CreateTable("Table_1"));

            }
            CloseConnetcion();
            myConnection = new SqlConnection("User ID=" + dbUserName +
                                              ";Password=" + dbPassword +
                                              ";Server=ALICJA-HP\\SQLEXPRESS" +
                                              ";Database=bazia" +
                                              ";Trusted_Connection=false" +
                                              ";connection timeout=0");
     
            
        }

        public void OpenConnection()
        {
            try
            {
                myConnection.Close();
                myConnection.Open();
                myTransaction = myConnection.BeginTransaction();
            }
            catch (Exception ex)
            {
                ErrorConnection();
                MessageBox.Show("Wystąpił błąd podczas łączenia z bazą danych! " + ex.Message);
            }
        }

        public void CloseConnetcion()
        {
            try
            {
                myTransaction.Commit();
            }
            catch { }
            myConnection.Close();
        }

        public void ErrorConnection()
        {
            myTransaction.Rollback();
            myConnection.Close();
        }

        public DataTable ExecuteQuery(string query)
        {
            DataTable result = new DataTable();
            try
            {
                SqlCommand myCommand = new SqlCommand(query, myConnection, myTransaction);
                SqlDataAdapter adapter = new SqlDataAdapter(myCommand);
                adapter.Fill(result);
            }
            catch (Exception ex)
            { }
            return result;
        }

        public void ExecuteNonQuery(string query)
        {
            
            try
            {
                SqlCommand myCommand = new SqlCommand(query, myConnection);
                myCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            { }
         
        }

        public DataTable ExecuteQuery(string query, Hashtable param)
        {
          
            SqlCommand myCommand = new SqlCommand(query, myConnection, myTransaction);
            foreach (DictionaryEntry parameterEntry in param)
            {
                myCommand.Parameters.AddWithValue((string)parameterEntry.Key, parameterEntry.Value) ;
            }

            SqlDataAdapter adapter = new SqlDataAdapter(myCommand);
            DataTable result = new DataTable();
            adapter.Fill(result);
            return result;
        }
    }
}
