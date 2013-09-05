using System;
using System.Collections.Generic;
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

namespace GSync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OutlookReader outlook;

        public MainWindow()
        {
            InitializeComponent();

            outlook = new OutlookReader();
            lstItems.ItemsSource = outlook.Entries;
        }

        private void btnRefreshOutlook_Click(object sender, RoutedEventArgs e)
        {
            outlook.Refresh();
        }

        private void btnCredentials_Click(object sender, RoutedEventArgs e)
        {
            var goo = new GoogleCalendar();
        }
    }
}
