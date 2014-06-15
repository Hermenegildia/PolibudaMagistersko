using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApplication2
{
    public class CustomerViewModel : DependencyObject
    {
        Customer myCustomer;

        public CustomerViewModel()
        {
            myCustomer = new Customer("Drzymisławowow", "Jasienice", 157.87);
        }

        
        public static readonly DependencyProperty CustomerProperty =
            DependencyProperty.Register(
            "Customer",
            typeof(Customer),
            typeof(CustomerViewModel),
            new PropertyMetadata(null));

        //public Customer Customer
        //{
        //    get { return (Customer)GetValue(CustomerProperty); }
        //    set { SetValue(CustomerProperty, value); }
        //}

        public Customer Customer
        {
            get { return myCustomer; }
            set { myCustomer = value; }
        }

        public string CustomerName
        {
            get { return Customer.Name; }
            set { Customer.Name = value; }
        }
    }
}
