using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApplication2
{
    public class CustomerViewModel :  INotifyPropertyChanged
    {
        Customer myCustomer;

        public CustomerViewModel()
        {
            myCustomer = new Customer();//"Drzymisławowow", "Jasienice", 157.87);
        }


        //public static readonly DependencyProperty CustomerProperty =
        //    DependencyProperty.Register(
        //    "Customer",
        //    typeof(Customer),
        //    typeof(CustomerViewModel),
        //    new PropertyMetadata(null));

        //public Customer Customer
        //{
        //    get { return (Customer)GetValue(CustomerProperty); }
        //    set { SetValue(CustomerProperty, value); }
        //}

        public Customer Customer
        {
            get { return myCustomer; }
            set
            {
                if (myCustomer != value)
                {
                    myCustomer = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //public string CustomerName
        //{
        //    get { return Customer.Name; }
        //    set {
        //        if (Customer.Name != value)
        //        Customer.Name = value;
        //        NotifyPropertyChanged();
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
