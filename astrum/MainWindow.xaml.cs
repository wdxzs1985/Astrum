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

            client = new AstrumClient();

            client.Username = "";
            client.Password = "";

            this.DataContext = client;
            this.PasswordBox.Password = client.Password;

            LoginPanel.Visibility = Visibility.Visible;
            StatusPanel.Visibility = Visibility.Hidden;
            LoginButton.IsEnabled = true;
        }

        private AstrumClient client;
        //public AstrumClient Client { get { return __client; } }
             
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("login start");
            LoginButton.IsEnabled = false;
            client.Password = this.PasswordBox.Password;

            var login = await Task.Run(() =>
            {
                return client.Login();
            });
            if (login)
            {
                Console.WriteLine("login success");
                LoginPanel.Visibility = Visibility.Hidden;
                StatusPanel.Visibility = Visibility.Visible;
                client.Token();
                client.Mypage();
            }
            else
            {
                Console.WriteLine("login failed");
                LoginButton.IsEnabled = true;
            }
        }

        private bool isRunning = false;

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (isRunning == false)
            {
                StartButton.Content = "Stop";
                isRunning = true;               

                bool result = await Task.Run(() =>
                {
                    client.Mypage();
                    while (isRunning)
                    {
                        try{
                            if (client.IsQuestEnable)
                            {
                                client.Quest();
                            }

                            if (client.IsRaidEnable)
                            {
                                client.Raid();
                            }

                            Console.WriteLine("wait 1 minute...");
                            client.Delay(AstrumClient.MINUTE);
                        }
                        catch
                        {
                            return false;
                        }
                        
                    }
                    return true;
                });

                isRunning = false;

                if (!result)
                {
                    LoginPanel.Visibility = Visibility.Visible;
                    StatusPanel.Visibility = Visibility.Hidden;
                    LoginButton.IsEnabled = true;
                }
            }
            else
            {
                isRunning = false;

                QuestCheckBox.IsChecked = false;
                RaidCheckBox.IsChecked = false;

                StartButton.Content = "Run";
            }
        }
    }
}
