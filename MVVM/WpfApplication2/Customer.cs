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
    public class Customer : DependencyObject
    {
    
        public Customer(string customerName, string customerHeadquaters, double customerAccountState)
        {
            this.Name = customerName;
            this.Headquaters = customerHeadquaters;
            this.AccountState = customerAccountState;
        }

        public Customer()
        {
        }

        //string name;

        public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
            "Name", typeof(string), typeof(Customer), new FrameworkPropertyMetadata("Robercik"));

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty HeadquatersProperty = DependencyProperty.Register(
            "Headquaters", typeof(string), typeof(Customer), new FrameworkPropertyMetadata("Warszawka"));

        public string Headquaters
        {
            get { return (string)GetValue(HeadquatersProperty); }
            set { SetValue(HeadquatersProperty, value); }
        }

        public static readonly DependencyProperty AccountStateProperty = DependencyProperty.Register(
          "AccountState", typeof(double), typeof(Customer), new FrameworkPropertyMetadata(100000.00), Validate);

        private static bool Validate(object value)
        {
            if (value == null)
                return false;
            if (value.GetType() != typeof(double))
                return false;

            return true;
        }

        public double AccountState
        {
            get { return (double)GetValue(AccountStateProperty); }
            set
            {
                try { SetValue(AccountStateProperty, value); }
                catch
                {
                   
                }
            }
        }

        //public string Name
        //{
        //    get { return name; }
        //    set
        //    {
        //        if (name != value)
        //        {
        //            name = value;
        //            NotifyPropertyChanged();
        //        }
        //    }
        //}

        //string headquaters;
        //public string Headquaters
        //{
        //    get { return headquaters; }
        //    set
        //    {
        //        if (headquaters != value)
        //        {
        //            headquaters = value;
        //            NotifyPropertyChanged();
        //        }
        //    }
        //}

        //double accountState;
        //public double AccountState
        //{
        //    get { return accountState; }
        //    set
        //    {
        //        if (accountState != value)
        //        {
        //            accountState = value;
        //            NotifyPropertyChanged();
        //        }
        //    }
        //}


        //public event PropertyChangedEventHandler PropertyChanged;

        //private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        //{
        //    if (PropertyChanged != null)
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //}

    }
}
