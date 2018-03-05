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
using System.Windows.Shapes;

namespace QuickAuth
{
    /// <summary>
    /// Interaction logic for LoginCreate.xaml
    /// </summary>
    public partial class LoginCreate : Window
    {
        AppSettings settings;

        public LoginCreate()
        {
            settings = AppSettings.LoadedSettings;

            InitializeComponent();

            this.Title = settings.LoginPage["Title"];
            this.BindLUsername.Content = settings.LoginPage["LabelUsername"];
            this.BindLPassword.Content = settings.LoginPage["LabelPassword"];
            this.BindBtnConnect.Content = settings.LoginPage["LabelConnect"];

            this.BindBtnConnect.Click += (x, y) =>
            {
                this.Username = this.BindUsername.Text;
                this.Password = this.BindPassword.Password;
                this.Close();
            };

            this.BindUsername.Focus();
        }

        public string Username { get; private set; }
        public string Password { get; private set; }
    }
}
