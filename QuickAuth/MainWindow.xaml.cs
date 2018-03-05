using QuickAuthLib;
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

namespace QuickAuth
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            AppSettings settings = AppSettings.load("Settings.json");

            ConnectivityTestResults results;
            bool isConnected = ConnetivityTest.Check(settings.App["ConnectionCheck"], out results);

            if (!isConnected)
            {
                LoginPage login = LoginPage.Load(results.ResponseURL);
            }

            InitializeComponent();
        }
    }
}
