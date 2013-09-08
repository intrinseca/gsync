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
using System.IO;

namespace GSync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OutlookReader outlook;
        GoogleCalendar google;
        SyncEngine syncEngine;

        public const string SYNCED_ENTRIES_FILE = "SyncedEntries.bin";

        public MainWindow()
        {
            InitializeComponent();

            outlook = new OutlookReader();

            google = new GoogleCalendar();
            google.ActiveCalendar = "intrinseca.me.uk_37m9th328t55luubdrqvlf6tdc@group.calendar.google.com";
            syncEngine = new SyncEngine(outlook, google);

            if (File.Exists(SYNCED_ENTRIES_FILE))
            {
                Stream loadStream = File.OpenRead(SYNCED_ENTRIES_FILE);
                syncEngine.LoadSyncedEntries(loadStream);
                loadStream.Close();
            }
        }

        private void btnConfigure_Click(object sender, RoutedEventArgs e)
        {
            var conf = new ConfigurationWindow(google, syncEngine);
            conf.Owner = this;
            conf.ShowDialog();
        }

        private void btnSyncNow_Click(object sender, RoutedEventArgs e)
        {
            syncEngine.Sync();

            Stream writeStream = File.OpenWrite(MainWindow.SYNCED_ENTRIES_FILE);
            syncEngine.SaveSyncedEntries(writeStream);
            writeStream.Close();
        }
    }
}
