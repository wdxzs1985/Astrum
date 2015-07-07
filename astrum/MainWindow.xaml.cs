using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Net;
using System.IO;
using System.Collections;

using Newtonsoft.Json;

namespace Astrum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            __client = new AstrumClient();
            __client.Username = "";
            __client.Password = "";

            this.DataContext = Client;
            this.PasswordBox.Password = __client.Password;

            LoginPanel.Visibility = System.Windows.Visibility.Visible;
            StatusPanel.Visibility = Visibility.Hidden;
        }

        private AstrumClient __client;
        public AstrumClient Client { get { return __client; } }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("login start");
            LoginButton.IsEnabled = false;
            __client.Password = this.PasswordBox.Password;

            var login = await Task.Run(() =>
            {
                return __client.Login();
            });
            if (login)
            {
                Console.WriteLine("login success");
                LoginPanel.Visibility = Visibility.Hidden;
                StatusPanel.Visibility = Visibility.Visible;
                __client.Token();
                __client.Mypage();
            }
            else
            {
                Console.WriteLine("login failed");
                LoginButton.IsEnabled = true;
            }
        }

        private async void QuestButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("quest start");
            QuestButton.IsEnabled = false;
            QuestButton.Content = "Runing...";
            var result = await Task.Run(() =>
            {
                __client.Mypage();
                __client.Quest();
                return true;
            });
            QuestButton.IsEnabled = true;
            QuestButton.Content = "Q";
            Console.WriteLine("quest end");
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {

            var values = new Dictionary<string, string>
                {
                   { "areaId", "chapter1-1" }
                };
            string param = JsonConvert.SerializeObject(values);
            Console.WriteLine(param);
        }

        private async void RaidButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("raid start");
            RaidButton.IsEnabled = false;
            RaidButton.Content = "Runing...";
            var result = await Task.Run(() =>
            {
                __client.Mypage();
                __client.Raid();
                return true;
            });
            RaidButton.IsEnabled = true;
            RaidButton.Content = "R";
            Console.WriteLine("raid end");
        }

    }


}
