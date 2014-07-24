using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Patient
    {
        public Patient()
        {
        }

        public Patient(string firstName, string lastName, int PESEL)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.PESEL = PESEL;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PESEL { get; set; }

        public override string ToString()
        {
            return this.LastName + " " + this.FirstName;
        }
    }
}
