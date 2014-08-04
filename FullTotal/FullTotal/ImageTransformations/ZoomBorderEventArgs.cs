using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace FullTotal.ImageTransformations
{
    public class ZoomBorderEventArgs: RoutedEventArgs
    {
        //public 

        public ZoomBorderEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
        }
    }
}
