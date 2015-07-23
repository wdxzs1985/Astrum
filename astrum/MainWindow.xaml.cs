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
using Astrum.Json;


namespace Astrum
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string USER_LIST_FILE = "./userlist.json";

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

            LoadUserList();
        }

        private AstrumClient client;

        private LoginUser nowUser;

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("login start");
            LoginButton.IsEnabled = false;
            LoginButton.Content = "少女祈祷中";
            LoginUserComboBox.IsEnabled = false;

            var username = UsernameBox.Text;
            var password = this.PasswordBox.Password;

            var login = false;
            if (!"".Equals(username) && !"".Equals(password))
            {
                login = await Task.Run(() =>
                {
                    try
                    {
                        return client.Login(username, password);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return false;
                    }

                });
            }

            if (login)
            {

                LoginPanel.Visibility = Visibility.Hidden;
                StatusPanel.Visibility = Visibility.Visible;

                Controller.Visibility = Visibility.Hidden;

                await Task.Run(() =>
                {
                    client.ViewModel.IsRunning = false;
                    client.OnStart();
                });

                Controller.Visibility = Visibility.Visible;

                nowUser = new LoginUser { username = username, password = password };
                //save user
                SaveUserList();

            }
            else
            {
                MessageBoxResult result = MessageBox.Show("登入失败");
                LoginButton.IsEnabled = true;
                LoginButton.Content = "登陆";
                LoginUserComboBox.IsEnabled = true;
            }
        }

        private async void LoadUserList()
        {
            int index = 0;
            List<LoginUser> loginUserList = await Task.Run(() =>
            {
                loginUserList = new List<LoginUser>();
                FileStream fs = null;
                StreamReader sr = null;
                try
                {
                    fs = new FileStream(USER_LIST_FILE, FileMode.Open);
                    sr = new StreamReader(fs);

                    string text = sr.ReadToEnd();

                    LoginUserList userList = JsonConvert.DeserializeObject<LoginUserList>(text);

                    loginUserList = userList.list;

                }
                catch
                {
                }
                finally
                {
                    if (sr != null)
                    {
                        sr.Close();
                    }
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }

                loginUserList.Add(new LoginUser { name = "New User" });

                return loginUserList;
            });

            client.ViewModel.LoginUserList = loginUserList;
            LoginUserComboBox.SelectedIndex = index;
        }

        private async void SaveUserList()
        {
            await Task.Run(() =>
            {
                var userList = client.ViewModel.LoginUserList.Where(user => user.username != null && !nowUser.username.Equals(user.username)).ToList();

                nowUser.name = client.ViewModel.Name;
                nowUser.minstaminastock = client.ViewModel.MinStaminaStock;
                nowUser.minbpstock = client.ViewModel.MinBpStock;

                userList.Insert(0, nowUser);

                client.ViewModel.LoginUserList = userList;

                FileStream fs = null;
                StreamWriter sw = null;
                try
                {
                    LoginUserList values = new LoginUserList();
                    values.list = userList;

                    string json = JsonConvert.SerializeObject(values);

                    fs = new FileStream(USER_LIST_FILE, FileMode.Create);
                    sw = new StreamWriter(fs);
                    sw.WriteLine(json);
                }
                catch
                {

                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Close();
                    }
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }

            });
        }
        
        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            QuestButton.IsEnabled = false;
            GuildBattleButton.IsEnabled = false;


            if (client.ViewModel.IsRunning == false)
            {
                SaveUserList();

                StartButton.Content = "Stop";
                StartButton.IsEnabled = true;

                client.ViewModel.IsRunning = true;

                bool result = await Task.Run(() =>
                {
                    while (client.ViewModel.IsRunning)
                    {
                        Console.WriteLine("Start Loop");

                        try
                        {
                            client.OnStart();

                            //client.ViewModel.IsRunning = false;

                            if (client.ViewModel.IsQuestEnable)
                            {
                                //client.ViewModel.IsRunning = true;
                                client.Quest();
                            }

                            if (client.ViewModel.IsGuildBattleEnable)
                            {
                                //client.ViewModel.IsRunning = true;
                                client.GuildBattle();
                            }

                            client.CountDown(AstrumClient.MINUTE);
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
                GuildBattleButton.IsEnabled = true;

                if (!result)
                {
                    LoginPanel.Visibility = Visibility.Visible;
                    StatusPanel.Visibility = Visibility.Hidden;
                    LoginButton.IsEnabled = true;
                    LoginUserComboBox.IsEnabled = true;
                }
            }
            else
            {
                client.ViewModel.IsRunning = false;
                StartButton.IsEnabled = false;
            }
        }


        private void LoginUserComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoginUser user = (LoginUser)LoginUserComboBox.SelectedItem;

            if (user != null)
            {
                UsernameBox.Text = user.username;
                PasswordBox.Password = user.password;

                client.ViewModel.MinStaminaStock = user.minstaminastock;

                if (user.username != null)
                {
                    UsernameBox.Visibility = Visibility.Hidden;
                    PasswordBox.Visibility = Visibility.Hidden;
                }
                else
                {
                    UsernameBox.Visibility = Visibility.Visible;
                    PasswordBox.Visibility = Visibility.Visible;
                }
            }
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var p = (ProgressBar)sender;

            var progressBefore = (e.OldValue / p.Maximum) * 100;
            var progressAfter = (e.NewValue / p.Maximum) * 100;

            Console.WriteLine("{0}: {1}% -> {2}%", p.Name, progressBefore, progressAfter);


            var foreground = new SolidColorBrush(Color.FromRgb(1,211,40));

            if (progressAfter <= 10d)
            {
                foreground = Brushes.Red;
            }
            else if (progressAfter <= 40d)
            {
                foreground = Brushes.Orange;
            }


            p.Foreground = foreground;
        }
    }
}
