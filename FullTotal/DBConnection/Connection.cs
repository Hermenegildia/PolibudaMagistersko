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
        SqlConnection mySqlConnection;
        SqlTransaction mySqlTransaction;
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
            mySqlConnection = new SqlConnection("User ID=" + dbUserName +
                                            ";Password=" + dbPassword +
                                            ";Server=ALICJA-HP\\SQLEXPRESS" +
                                            //";Database=bazka" + //nie laczymy z baza, bo nie wiemy czy istnieje!
                                            ";Trusted_Connection=false" +
                                            ";connection timeout=0");

            OpenConnection();
            DataTable databases = ExecuteQuery("SELECT * FROM master.dbo.sysdatabases WHERE name = 'bazka'");
            if (databases.Rows.Count < 1) //todo: w pracy opisać dlaczego tworzenie bazy danych bez transakcji - szukaj po wyjatku
            //CREATE DATABASE statement not allowed within multi-statement transaction.
            {
                CloseConnection();
                mySqlConnection.Open();
                
                ExecuteNonQueryNoTransaction(Queries.CreateDatabase("bazka"));
                mySqlConnection.Close();
                mySqlConnection = new SqlConnection("User ID=" + dbUserName +
                                                  ";Password=" + dbPassword +
                                                  ";Server=ALICJA-HP\\SQLEXPRESS" +
                                                  ";Database=bazka" +
                                                  ";Trusted_Connection=false" +
                                                  ";connection timeout=0");
                OpenConnection();
                ExecuteNonQuery(Queries.CreateTablePatients());
                ExecuteNonQuery(Queries.CreateTableDocumentation());
                CloseConnection();
            }
            mySqlConnection = new SqlConnection("User ID=" + dbUserName +
                                                  ";Password=" + dbPassword +
                                                  ";Server=ALICJA-HP\\SQLEXPRESS" +
                                                  ";Database=bazka" +
                                                  ";Trusted_Connection=false" +
                                                  ";connection timeout=0");
            
        }

        public void OpenConnection()
        {
            try
            {
                mySqlConnection.Close();
                mySqlConnection.Open();
                mySqlTransaction = mySqlConnection.BeginTransaction();
            }
            catch (Exception ex)
            {
                ErrorConnection();
                MessageBox.Show("Wystąpił błąd podczas łączenia z bazą danych! " + ex.Message);
            }
        }

        public void CloseConnection()
        {
            try
            {
                mySqlTransaction.Commit();
            }
            catch 
            {
                ErrorConnection();
            }
            mySqlConnection.Close();
        }

        public void ErrorConnection()
        {
            mySqlTransaction.Rollback();
            mySqlConnection.Close();
        }

        public DataTable ExecuteQuery(string query)
        {
            DataTable result = new DataTable();
            try
            {
                SqlCommand myCommand = new SqlCommand(query, mySqlConnection, mySqlTransaction);
                SqlDataAdapter adapter = new SqlDataAdapter(myCommand);
                adapter.Fill(result);
            }
            catch (Exception ex)
            { }
            return result;
        }

        public void ExecuteNonQueryNoTransaction(string query)
        {
           try
            {
                SqlCommand myCommand = new SqlCommand(query, mySqlConnection);
                myCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            { }
        }

        public void ExecuteNonQuery(string query)
        {
            try
            {
                SqlCommand myCommand = new SqlCommand(query, mySqlConnection, mySqlTransaction);
                myCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            { }

        }

        public DataTable ExecuteQuery(string query, Hashtable param)
        {
          
            SqlCommand myCommand = new SqlCommand(query, mySqlConnection, mySqlTransaction);
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
