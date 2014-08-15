using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FullTotal
{
    /// <summary>
    /// Interaction logic for SelectionDisplay.xaml
    /// </summary>
    public partial class SelectionDisplay : UserControl
    {
        public SelectionDisplay(string itemId)
        {
            InitializeComponent();
            this.messageTextBlock.Text = string.Format(CultureInfo.CurrentCulture, Properties.Resources.SelectedMessage, itemId);
        }

        /// <summary>
        /// Called when the OnLoaded storyboard completes.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void OnLoadedStoryboardCompleted(object sender, System.EventArgs e)
        {
            var parent = (Panel)this.Parent;
            parent.Children.Remove(this);
        }
    }
}