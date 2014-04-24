using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;


namespace DBConnection
{
    public class Connection
    {
        SqlConnection myConnection;
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
                                            ";Server=localhost" +
                                            ";Trusted_Connection=false" +
                                            ";connection timeout=30");

        }


    }
}
