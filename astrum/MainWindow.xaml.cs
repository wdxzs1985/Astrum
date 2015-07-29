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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string USER_LIST_FILE = "./userlist.json";

        public MainWindow()
        {
            InitializeComponent();

            client = new AstrumClient();

            this.DataContext = client.ViewModel;

            initLoginPanel();
        }

        private void initLoginPanel()
        {
            this.UsernameBox.Text = "";
            this.PasswordBox.Password = "";
            
            LoginPanel.Visibility = Visibility.Visible;
            StatusPanel.Visibility = Visibility.Hidden;
            LoginButton.Content = "登陆";
            LoginButton.IsEnabled = true;
            LoginUserComboBox.IsEnabled = true;

            LoadUserList();
        }

        private AstrumClient client;

        private LoginUser nowUser;

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("login start");
            LoginButton.Content = "少女祈祷中";
            LoginButton.IsEnabled = false;
            LoginUserComboBox.IsEnabled = false;

            var username = UsernameBox.Text;
            var password = PasswordBox.Password;

            client.ViewModel.IsLogin = false;
            if (!"".Equals(username) && !"".Equals(password))
            {
                client.ViewModel.IsLogin = await Task.Run(() =>
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

            if (client.ViewModel.IsLogin)
            {
                
                client.ViewModel.IsReady = false;
                client.ViewModel.IsRunning = false;

                LoginPanel.Visibility = Visibility.Hidden;
                StatusPanel.Visibility = Visibility.Visible;

                await Task.Run(() =>
                {
                    client.StartQuest();
                });

                client.ViewModel.IsReady = true;

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

        private async void StartQuestButton_Click(object sender, RoutedEventArgs e)
        {
            StartQuestButton.IsEnabled = false;
            QuestButton.IsEnabled = false;
            GuildBattleButton.IsEnabled = false;


            if (client.ViewModel.IsRunning == false)
            {
                SaveUserList();

                StartQuestButton.Content = "Stop";
                StartQuestButton.IsEnabled = true;

                client.ViewModel.IsRunning = true;
                client.ViewModel.IsStaminaEmpty = false;

                bool result = await Task.Run(() =>
                {
                    while (client.ViewModel.IsRunning)
                    {
                        Console.WriteLine("Start Loop");

                        try
                        {
                            client.StartQuest();
                            
                            client.Quest();

                            if(client.ViewModel.Fever)
                            {
                                client.CountDown(AstrumClient.SECOND * 10);
                            }
                            else
                            {
                                client.CountDown(AstrumClient.MINUTE);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            client.ViewModel.IsRunning = false;
                            client.ViewModel.IsReady = false;
                            return false;
                        }
                    }
                    return true;
                });


                StartQuestButton.Content = "Start";
                StartQuestButton.IsEnabled = true;
                QuestButton.IsEnabled = true;
                GuildBattleButton.IsEnabled = true;

                if (!result)
                {
                    initLoginPanel();
                }
            }
            else
            {
                client.ViewModel.IsRunning = false;
                StartQuestButton.IsEnabled = false;
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
                client.ViewModel.MinBpStock = user.minbpstock;

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
		
		public void DragWindow(object sender,MouseButtonEventArgs args)
		{
			this.DragMove();
		}
		
		public void CloseWindow(object sender,RoutedEventArgs args)
		{
			this.Close();
		}
		
		public void MiniWindow(object sender,RoutedEventArgs args)
		{
			this.WindowState = WindowState.Minimized;
		}

        private async void UseHalfStaminaButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                if (client.ViewModel.StaminaHalfStock > 0)
                {
                    client.UseItem(AstrumClient.ITEM_STAMINA, AstrumClient.INSTANT_HALF_STAMINA, 1);
                }
            });
        }

        private async void UseStaminaButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                if (client.ViewModel.StaminaStock > 0)
                {
                    client.UseItem(AstrumClient.ITEM_STAMINA, AstrumClient.INSTANT_STAMINA, 1);
                }
            });
        }

        private async void UseMiniBpButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                if (client.ViewModel.BpMiniStock > 0)
                {
                    client.UseItem(AstrumClient.ITEM_BP, AstrumClient.INSTANT_MINI_BP, 1);
                }
            });
        }

        private async void UseBpButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                if (client.ViewModel.BpStock > 0)
                {
                    client.UseItem(AstrumClient.ITEM_BP, AstrumClient.INSTANT_BP, 1);
                }
            });
        }

        private async void StartGuildBattleButton_Click(object sender, RoutedEventArgs e)
        {
            StartGuildBattleButton.IsEnabled = false;
            QuestButton.IsEnabled = false;
            GuildBattleButton.IsEnabled = false;
            
            if (client.ViewModel.IsRunning == false)
            {
                StartGuildBattleButton.Content = "Stop";
                StartGuildBattleButton.IsEnabled = true;

                client.ViewModel.IsRunning = true;
                client.ViewModel.IsStaminaEmpty = false;

                bool result = await Task.Run(() =>
                {
                        try
                        {
                            client.GuildBattle();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            client.ViewModel.IsRunning = false;
                            return false;
                        }
                    return true;
                });


                StartGuildBattleButton.Content = "Start";
                StartGuildBattleButton.IsEnabled = true;
                QuestButton.IsEnabled = true;
                GuildBattleButton.IsEnabled = true;

                if (!result)
                {
                    initLoginPanel();
                }
            }
            else
            {
                client.ViewModel.IsRunning = false;
                StartGuildBattleButton.IsEnabled = false;
            }
        }

        private async void QuestButton_Checked(object sender, RoutedEventArgs e)
        {
            if (client != null && client.ViewModel.IsLogin)
            {
                client.ViewModel.IsReady = false;
                await Task.Run(() =>
                {
                    client.StartQuest();
                });

                client.ViewModel.IsReady = true;
            }
        }

        private async void GuildBattleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (client != null && client.ViewModel.IsLogin)
            {
                client.ViewModel.IsReady = false;
                await Task.Run(() =>
                {
                    client.StartGuildBattle();
                });

                if (client.ViewModel.GuildBattleId !=null)
                {
                    client.ViewModel.IsReady = true;
                }
            }
        }
    }
}
