using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication2
{
    public class Customer :INotifyPropertyChanged
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
                    NotifyPropertyChanged("Name");
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
                    NotifyPropertyChanged("Headquaters");
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
                    NotifyPropertyChanged("AccountState");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
