using QuickAuthLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace QuickAuth
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppSettings settings;
        private DispatcherTimer connectionTimer;
        private AppSettings.ConnectionStatusValue connectionStatus;
        private ConnectivityTestResults connectivityStatus;

        private LoginCreate loginScreen;

        bool isTestingConnection = false;

        public MainWindow()
        {
            this.settings = AppSettings.Load("Settings.json");

            NetworkChange.NetworkAddressChanged += (x, y) => { this.RunConnectionTest(); };
            NetworkChange.NetworkAvailabilityChanged += (x, y) => { this.RunConnectionTest(); };

            InitializeComponent();

            this.SetConnectionStatus(settings.ConnectionStatus["Connecting"]);
            this.Loaded += (x, y) => {
                this.RunConnectionTest();

                connectionTimer = new DispatcherTimer();
                connectionTimer.Interval = new TimeSpan(0, 0, 15);
                connectionTimer.Tick += (Tx, Ty) =>
                {
                    this.RunConnectionTest();
                };
                connectionTimer.Start();
            };
        }

        public void CallUI(ThreadStart fn)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, fn);
        }

        public void SetConnectionStatus(AppSettings.ConnectionStatusValue status)
        {
            this.CallUI(new ThreadStart(delegate
            {
                this.connectionStatus = status;
                BindConnectionStatusBackground.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(status.StatusColor));
                BindConnectionStatusText.Content = status.StatusText.ToUpper();
            }));
        }

        public void RunConnectionTest ()
        {
            // Are we already running a test?
            if (this.isTestingConnection)
                return;

            // If not, test connection
            this.isTestingConnection = true;

            ConnectivityTestResults results;
            bool isConnected = ConnetivityTest.Check(settings.App["ConnectionCheck"], out results);

            this.connectivityStatus = results;

            // Did we pass the test?
            if (results.HasPassedTest)
            {
                this.SetConnectionStatus(settings.ConnectionStatus["Connected"]);
            }

            // Ok, we have no internet connection, so just stop here.
            else if (!results.HasInternet)
            {
                this.SetConnectionStatus(settings.ConnectionStatus["NoInternet"]);
            }

            // What if we have internet, but hit a login page?
            else if (results.HasRedirected)
            {
                this.SetConnectionStatus(settings.ConnectionStatus["RequestLogin"]);

                if (this.settings.SavedNetworks.ContainsKey(results.ResponseURL))
                {
                    AppSettings.SavedNetwork network = this.settings.SavedNetworks[results.ResponseURL];
                    this.RunLogin(results.ResponseURL, network.Username, network.Password);
                }
                else
                {
                    loginScreen = new LoginCreate();
                    loginScreen.Owner = this;
                    loginScreen.ShowDialog();

                    if (loginScreen.Username != null && loginScreen.Username.Length > 0)
                        this.RunLogin(results.ResponseURL, loginScreen.Username, loginScreen.Password);
                }
            }

            // Finally, allow tests to run again
            this.isTestingConnection = false;
        }

        public void RunLogin (string url, string username, string password)
        {
            this.SetConnectionStatus(settings.ConnectionStatus["ConnectLogin"]);

            new Thread(() =>
            {
                try
                {
                    // Run login function
                    LoginPage login = LoginPage.Load(url);
                    login.Login(username, password);

                // After running the login function, send a connection test to our UI thread
                    this.CallUI(new ThreadStart(delegate
                    {
                        this.RunConnectionTest();

                        // If login has worked...
                        if (this.connectivityStatus.HasPassedTest)
                        {
                            // Save current settings
                            AppSettings.SavedNetwork network = new AppSettings.SavedNetwork();
                            network.Username = username;
                            network.Password = password;

                            // Save current login-password combo with this login URL
                            if (!this.settings.SavedNetworks.ContainsKey(url))
                                this.settings.SavedNetworks.Add(url, network);

                            this.settings.Save();
                        }

                        // If didn't and we were connected, unsave it
                        else if (this.connectivityStatus.HasInternet && this.settings.SavedNetworks.ContainsKey(url))
                            this.settings.SavedNetworks.Remove(url);
                    }));
                }
                catch
                {
                    // If some error happens while logging in, update UI
                    this.CallUI(new ThreadStart(delegate
                    {
                        this.SetConnectionStatus(settings.ConnectionStatus["Error"]);
                    }));
                }

            }).Start();
        }
    }
}
