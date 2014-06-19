using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication2
{
    public class Customer
    {
    
        public Customer(string customerName, string customerHeadquaters, double customerAccountState)
        {
            this.Name = customerName;
            this.Headquaters = customerHeadquaters;
            this.accountState = customerAccountState;
        }

        string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                
                }
            }
        }

        string headquaters;
        public string Headquaters
        {
            get { return headquaters; }
            set
            {
                if (headquaters != value)
                {
                    headquaters = value;
                  
                }
            }
        }

        double accountState;
        public double AccountState
        {
            get { return accountState; }
            set
            {
                if (accountState != value)
                {
                    accountState = value;
                   
                }
            }
        }

      


    }
}
