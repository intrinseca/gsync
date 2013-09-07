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
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;

namespace GSync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OutlookReader outlook;
        GoogleCalendar google;

        public MainWindow()
        {
            InitializeComponent();

            outlook = new OutlookReader();
            lstItems.ItemsSource = outlook.Entries;

            google = new GoogleCalendar();
        }

        private void btnRefreshOutlook_Click(object sender, RoutedEventArgs e)
        {
            outlook.Refresh();

            foreach (var entry in outlook.Entries)
            {
                google.AddEvent(entry, "intrinseca.me.uk_37m9th328t55luubdrqvlf6tdc@group.calendar.google.com");
            }
        }

        private void btnCredentials_Click(object sender, RoutedEventArgs e)
        {
            lstItems.ItemsSource = google.GetCalendarList();
            lstItems.DisplayMemberPath = "Summary";
        }

        private void btnConfigure_Click(object sender, RoutedEventArgs e)
        {
            var conf = new ConfigurationWindow(google);
            conf.Owner = this;
            conf.ShowDialog();
        }
    }
}
