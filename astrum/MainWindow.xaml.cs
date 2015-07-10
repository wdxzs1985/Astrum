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
using Astrum.Http;
using System.Threading;


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

            this.DataContext = client.ViewModel;

            this.UsernameBox.Text = "";
            this.PasswordBox.Password = "";

            LoginPanel.Visibility = Visibility.Visible;
            StatusPanel.Visibility = Visibility.Hidden;

            LoginButton.IsEnabled = true;
        }

        private AstrumClient client;

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("login start");
            LoginButton.IsEnabled = false;

            var username = UsernameBox.Text;
            var password = this.PasswordBox.Password;

            var login = await Task.Run(() =>
            {
                return client.Login(username, password);
            });
            if (login)
            {
                Console.WriteLine("login success");
                LoginPanel.Visibility = Visibility.Hidden;
                StatusPanel.Visibility = Visibility.Visible;

                client.Token();
                client.Mypage();

                client.ViewModel.IsRunning = false;
            }
            else
            {
                Console.WriteLine("login failed");
                LoginButton.IsEnabled = true;
            }
        }
        
        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            QuestButton.IsEnabled = false;
            RaidButton.IsEnabled = false;
            GuildBattleButton.IsEnabled = false;

            if (client.ViewModel.IsRunning == false)
            {
                client.ViewModel.IsRunning = true;

                StartButton.Content = "Stop";
                StartButton.IsEnabled = true;
                
                bool result = await Task.Run(() =>
                {
                    client.Mypage();
                    while (client.ViewModel.IsRunning)
                    {
                        Console.WriteLine("start loop");
                        client.ViewModel.IsRunning = false;

                        try{
                            if (client.ViewModel.IsQuestEnable)
                            {
                                client.ViewModel.IsRunning = true;
                                client.Quest();
                            }

                            if (client.ViewModel.IsRaidEnable)
                            {
                                client.ViewModel.IsRunning = true;
                                client.Raid();
                            }

                            if (client.ViewModel.IsGuildBattleEnable)
                            {
                                client.ViewModel.IsRunning = true;
                                client.GuildBattle();
                            }

                            var countDown = AstrumClient.MINUTE;
                            for (var i = 0; i < AstrumClient.MINUTE; i += 100)
                            {
                                Thread.Sleep(100);
                                if (i % 1000 == 0)
                                {
                                    Console.WriteLine("wait {0} second", (countDown - i) / 1000);
                                }
                                if (!client.ViewModel.IsRunning)
                                {
                                    break;
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            client.ViewModel.IsRunning = false;
                            return false;
                        }
                    }
                    return true;
                });


                StartButton.Content = "Start";
                StartButton.IsEnabled = true;
                QuestButton.IsEnabled = true;
                RaidButton.IsEnabled = true;
                GuildBattleButton.IsEnabled = true;

                if (!result)
                {
                    LoginPanel.Visibility = Visibility.Visible;
                    StatusPanel.Visibility = Visibility.Hidden;
                    LoginButton.IsEnabled = true;
                }
            }
            else
            {
                client.ViewModel.IsRunning = false;
                StartButton.IsEnabled = false;
            }
        }
    }
}
