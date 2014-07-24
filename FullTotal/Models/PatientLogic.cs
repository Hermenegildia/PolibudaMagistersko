using DBConnection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Models
{
    public class PatientLogic
    {
        Connection connection;

        public PatientLogic(Connection connection)
        {
            this.connection = connection;
        }

        public DataTable InsertDataToDatabase(Patient patient)
        {
            try
            {
                Hashtable param = new Hashtable();
                param["name"] = patient.FirstName;
                param["last_name"] = patient.LastName;
                param["PESEL"] = patient.PESEL;
                connection.OpenConnection();
                DataTable result = connection.ExecuteQuery("INSERT INTO patients (name, last_name, PESEL) VALUES (@name, @last_name, @PESEL)", param);
                connection.CloseConnection();
                return result;
            }
            catch
            {   
                MessageBox.Show("Ups! Coś się nie powiodło!");
                return new DataTable();
            }
        }

    }
}
