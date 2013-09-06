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
using System.Diagnostics;

namespace GSync
{
    /// <summary>
    /// Interaction logic for AuthenticationWindow.xaml
    /// </summary>
    public partial class AuthenticationWindow : Window
    {
        public string AuthResult { get; set; }

        public AuthenticationWindow()
        {
            InitializeComponent();
        }

        private void Browser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            dynamic doc = Browser.Document;
            string title = doc.title;
            Debug.Print(title);

            if (title.StartsWith("Success code="))
            {
                AuthResult = title.Substring(title.IndexOf('=') + 1);
                Debug.Print(AuthResult);
                DialogResult = true;
                this.Close();
            }
        }
    }
}
