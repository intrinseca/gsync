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
using System.Windows.Shapes;
using System.IO;

namespace GSync
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        private GoogleCalendar google;
        private SyncEngine syncEngine;

        public ConfigurationWindow(GoogleCalendar _google, SyncEngine _syncEngine)
        {
            InitializeComponent();

            google = _google;
            syncEngine = _syncEngine;

            checkGoogleStatus();
        }

        private void checkGoogleStatus()
        {
            if (google.Authenticated)
            {
                btnSignIn.Visibility = System.Windows.Visibility.Collapsed;
                btnSignOut.Visibility = System.Windows.Visibility.Visible;

                lstCalendars.ItemsSource = google.GetCalendarList();
            }
            else
            {
                btnSignIn.Visibility = System.Windows.Visibility.Visible;
                btnSignOut.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private string getAuthorization(Uri authUri)
        {
            AuthenticationWindow authWindow = new AuthenticationWindow();
            authWindow.Browser.Navigate(authUri);
            if (authWindow.ShowDialog() == true)
            {
                // Retrieve the access token by using the authorization code:
                return authWindow.AuthResult;
            }
            else
            {
                return null;
            }
        }

        private void btnSignOut_Click(object sender, RoutedEventArgs e)
        {
            google.Deauthorise();
            checkGoogleStatus();
        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            google.Authorise(getAuthorization);
            checkGoogleStatus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
