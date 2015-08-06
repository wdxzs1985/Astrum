using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;

using Newtonsoft.Json;
using Astrum.Http;
using Astrum.Json;
using System.Drawing;
using System.Windows.Forms;
using Astrum.UI;
using Astrum.Json.Card;

namespace Astrum
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string USER_LIST_FILE = "./userlist.json";
        private NotifyIcon notifyIcon = null;

        public MainWindow()
        {
            InitializeComponent();

            client = new AstrumClient();

            this.DataContext = client.ViewModel;
            InitialTray();

            initLoginPanel();
        }

        private void InitialTray()
        {
            //设置托盘的各个属性
            notifyIcon = new NotifyIcon();
            notifyIcon.Text = client.ViewModel.WindowTitle;

            notifyIcon.Icon = new Icon("./Astrum.ico");
            notifyIcon.Visible = false;
            notifyIcon.MouseClick += NotifyIcon_MouseClick;
            
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            //ToolStripMenuItem stopItem = new ToolStripMenuItem("停止", null, StopItem_Click);
            ToolStripMenuItem exitItem = new ToolStripMenuItem("退出", null, ExitItem_Click);
            //notifyIcon.ContextMenuStrip.Items.Add(stopItem);
            notifyIcon.ContextMenuStrip.Items.Add(exitItem);
            
            //窗体状态改变时候触发
            this.StateChanged += MainWindow_StateChanged;
        }

        private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void StopItem_Click(object sender, EventArgs e)
        {
            client.ViewModel.IsRunning = false;
        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                notifyIcon.Visible = true;
                ShowInTaskbar = false;
                notifyIcon.ShowBalloonTip(1000, client.ViewModel.WindowTitle, "少女隐身中...", ToolTipIcon.None);
            }
            else
            {
                notifyIcon.Visible = false;
                this.ShowInTaskbar = true;
            }
        }

        private void initLoginPanel()
        {
            this.UsernameBox.Text = "";
            this.PasswordBox.Password = "";
            
            LoginPanel.Visibility = Visibility.Visible;
            StatusPanel.Visibility = Visibility.Hidden;
            LoginButton.Content = "登陆";
            LoginButton.IsEnabled = true;
            //LoginUserComboBox.IsEnabled = true;
            UserSelector.IsEnabled = true;

            LoadUserList();
        }

        private AstrumClient client;

        private LoginUser nowUser;

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("login start");
            LoginButton.Content = "少女祈祷中";
            LoginButton.IsEnabled = false;
            //LoginUserComboBox.IsEnabled = false;
            UserSelector.IsEnabled = false;

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
                Tabs.IsEnabled = false;

                try
                {
                    await Task.Run(() =>
                    {
                        if (client.ViewModel.IsQuestEnable)
                        {
                            var result = client.StartQuest();
                            client.ViewModel.IsReady = result;
                            return result;
                        }
                        else if (client.ViewModel.IsGuildBattleEnable)
                        {
                            var result = client.StartGuildBattle();
                            client.ViewModel.IsReady = result;
                            return result;
                        }
                        else if (client.ViewModel.IsGachaEnable)
                        {
                            var result = client.StartGacha();
                            return result;
                        }
                        return false;
                    });

                    nowUser = new LoginUser { username = username, password = password };
                    //save user
                    SaveUserList();
                }
                catch (Exception ex)
                {
                    initLoginPanel();
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Tabs.IsEnabled = true;
                }
            }
            else
            {
                initLoginPanel();
                MessageBoxResult result = System.Windows.MessageBox.Show("登入失败");
            }
        }

        private async void LoadUserList()
        {
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
            UserSelector.SelectedIndex = 0;
        }

        private async void SaveUserList()
        {
            await Task.Run(() =>
            {
                var userList = client.ViewModel.LoginUserList.Where(user => user.username != null && !nowUser.username.Equals(user.username)).ToList();

                nowUser.name = client.ViewModel.Name;
                nowUser.minstaminastock = client.ViewModel.MinStaminaStock;
                nowUser.minbpstock = client.ViewModel.MinBpStock;
                nowUser.keepstamina = client.ViewModel.KeepStamina;

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
            Tabs.IsEnabled = false;
            //QuestButton.IsEnabled = false;
            //GuildBattleButton.IsEnabled = false;


            if (client.ViewModel.IsRunning == false)
            {
                SaveUserList();

                StartButton.Content = "Stop";
                StartButton.IsEnabled = true;

                client.ViewModel.IsRunning = true;
                client.ViewModel.IsStaminaEmpty = false;

                bool result = await Task.Run(() =>
                {
                    try
                    {

                        while (client.ViewModel.IsRunning)
                        {
                            Console.WriteLine("Start Loop");
                            if (client.ViewModel.IsQuestEnable)
                            {
                                client.StartQuest();
                                client.Quest();
                                
                                client.CountDown(client.ViewModel.Fever ? AstrumClient.SECOND * 10 : AstrumClient.MINUTE);

                            }
                            else if (client.ViewModel.IsGuildBattleEnable)
                            {
                                client.GuildBattle();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return false;
                    }
                    return true;
                });

                if(result)
                {
                    StartButton.Content = "Start";
                    StartButton.IsEnabled = true;
                    Tabs.IsEnabled = true;
                    //QuestButton.IsEnabled = true;
                    //GuildBattleButton.IsEnabled = true;
                }
                else
                {
                    client.ViewModel.IsRunning = false;
                    client.ViewModel.IsReady = false;
                    initLoginPanel();
                }
            }
            else
            {
                client.ViewModel.IsRunning = false;
                StartButton.IsEnabled = false;
            }
        }




        private void UserSelector_UserChanged(object sender, RoutedEventArgs e)
        {
            LoginUser user = UserSelector.SelectedUser;

            if (user != null)
            {
                UsernameBox.Text = user.username;
                PasswordBox.Password = user.password;

                client.ViewModel.MinStaminaStock = user.minstaminastock;
                client.ViewModel.MinBpStock = user.minbpstock;
                client.ViewModel.KeepStamina = user.keepstamina;

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
                try
                {
                    if (client.ViewModel.StaminaHalfStock > 0)
                    {
                        client.UseItem(AstrumClient.ITEM_STAMINA, AstrumClient.INSTANT_HALF_STAMINA, 1);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        private async void UseStaminaButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (client.ViewModel.StaminaStock > 0)
                    {
                        client.UseItem(AstrumClient.ITEM_STAMINA, AstrumClient.INSTANT_STAMINA, 1);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        private async void UseMiniBpButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (client.ViewModel.BpMiniStock > 0)
                    {
                        client.UseItem(AstrumClient.ITEM_BP, AstrumClient.INSTANT_MINI_BP, 1);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        private async void UseBpButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (client.ViewModel.BpStock > 0)
                    {
                        client.UseItem(AstrumClient.ITEM_BP, AstrumClient.INSTANT_BP, 1);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }
   
        private async void QuestButton_Checked(object sender, RoutedEventArgs e)
        {
            if (client != null && client.ViewModel.IsLogin)
            {
                client.ViewModel.IsReady = false;
                Tabs.IsEnabled = false;

                try
                {
                    var result = await Task.Run(() =>
                    {
                        return client.StartQuest();
                    });

                    if (result)
                    {
                        client.ViewModel.IsReady = true;
                    }
                    else
                    {
                        client.ViewModel.IsRunning = false;
                        client.ViewModel.IsReady = false;
                    }
                }
                catch (Exception ex)
                {
                    initLoginPanel();
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Tabs.IsEnabled = true;
                }
            }
        }

        private async void GuildBattleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (client != null && client.ViewModel.IsLogin)
            {
                client.ViewModel.IsReady = false;
                Tabs.IsEnabled = false;

                try
                {
                    var result = await Task.Run(() =>
                    {
                        return client.StartGuildBattle();
                    });
                    if (result)
                    {
                        client.ViewModel.IsReady = true;
                    }
                    else
                    {
                        client.ViewModel.IsRunning = false;
                        client.ViewModel.IsReady = false;
                    }
                }
                catch (Exception ex)
                {
                    initLoginPanel();
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Tabs.IsEnabled = true;
                }
            }
        }

        private async void GachaButton_Checked(object sender, RoutedEventArgs e)
        {
            if (client != null && client.ViewModel.IsLogin)
            {
                client.ViewModel.IsReady = false;
                client.ViewModel.IsTrainingEnable = false;
                client.ViewModel.IsTrainingBaseEnable = false;
                Tabs.IsEnabled = false;

                try
                {
                    var result = await Task.Run(() =>
                    {
                        return client.StartGacha();
                    });
                }
                catch (Exception ex)
                {
                    initLoginPanel();
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Tabs.IsEnabled = true;
                }
            }
        }

        private async void UseGuildBattleTpNormal(object sender, RoutedEventArgs e)
        {
            if (client.ViewModel.IsReady)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        client.GuildBattleTpNormal();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
                
            }
        }

        private async void UseGuildBattleTpChat(object sender, RoutedEventArgs e)
        {
            if (client.ViewModel.IsReady)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        client.GuildBattleTpChat();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });

            }
        }

        private async void UseGuildBattleTpRoulette(object sender, RoutedEventArgs e)
        {
            if (client.ViewModel.IsReady)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        client.GuildBattleTpRoulette();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
            }
        }
        
        private async void GachaListView_Gacha(object sender, RoutedEventArgs e)
        {
            GachaPanel.IsEnabled = false;

            await Task.Run(() =>
            {
                try
                {
                    var ge = (GachaEventArgs)e;
                    client.Gacha(ge.GachaId, ge.Sequence);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });

            GachaPanel.IsEnabled = true;
        }

        private async void TrainingButton_Click(object sender, RoutedEventArgs e)
        {
            TrainingPanel.IsEnabled = false;
            client.ViewModel.IsTrainingBaseEnable = false;

            if (client.ViewModel.IsTrainingEnable)
            {
                client.ViewModel.IsTrainingEnable = false;
            }
            else
            {
                client.ViewModel.IsTrainingEnable = true;

                await Task.Run(() =>
                {
                    try
                    {
                        client.StartTraining();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
                TrainingPanel.IsEnabled = true;
            }
        }
        
        private async void ExecuteRaiseNormal_Click(object sender, RoutedEventArgs e)
        {
            TrainingPanel.IsEnabled = false;

            await Task.Run(() =>
            {
                try
                {
                    var result = client.ExecuteRaiseNormal();
                    if (result)
                    {
                        client.ViewModel.History = "Success";
                    }
                    else
                    {
                        client.ViewModel.History = "No Card";
                    }
                    client.StartTraining();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });

            TrainingPanel.IsEnabled = true;
        }

        private async void ExecuteRaiseRare_Click(object sender, RoutedEventArgs e)
        {
            TrainingPanel.IsEnabled = false;

            await Task.Run(() =>
            {
                try
                {
                    var result = client.ExecuteRaiseRare();
                    if (result)
                    {
                        client.ViewModel.History = "Success";
                    }
                    else
                    {
                        client.ViewModel.History = "No Card";
                    }
                    client.StartTraining();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });

            TrainingPanel.IsEnabled = true;
        }

        private async void TraningChangeBase_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    client.TrainingBase();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
            
        }

        private async void TrainingBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                var baseId = ((CardInfo)TrainingBase.SelectedItem)._id;
                await Task.Run(() =>
                {
                    try
                    {
                        client.StartTraining(baseId);
                        client.ViewModel.IsTrainingBaseEnable = false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
            }
        }

        private async void UseStrengthStatueBronze_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    var itemId = AstrumClient.INSTANT_STRENGTH_STATUE_BRONZE;
                    var quantity = client.ViewModel.StrengthStatueBronzeAvailable;
                    var result =client.ExecuteRaiseItem(itemId, quantity);
                    if (result)
                    {
                        client.ViewModel.History = "Success";
                    }
                    client.StartTraining();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        private async void UseStrengthStatueSilver_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    var itemId = AstrumClient.INSTANT_STRENGTH_STATUE_SILVER;
                    var quantity = client.ViewModel.StrengthStatueSilverAvailable;
                    var result = client.ExecuteRaiseItem(itemId, quantity);
                    if (result)
                    {
                        client.ViewModel.History = "Success";
                    }
                    client.StartTraining();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        private async void UseStrengthStatueGold_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    var itemId = AstrumClient.INSTANT_STRENGTH_STATUE_GOLD;
                    var quantity = client.ViewModel.StrengthStatueGoldAvailable;
                    var result = client.ExecuteRaiseItem(itemId, quantity);
                    if (result)
                    {
                        client.ViewModel.History = "Success";
                    }
                    client.StartTraining();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        private async void UseAbilityBookBronze_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    var itemId = AstrumClient.INSTANT_ABILITY_BOOK_BRONZE;
                    var quantity = client.ViewModel.AbilityBookBronzeAvailable;
                    var result = client.ExecuteRaiseItem(itemId, quantity);
                    if (result)
                    {
                        client.ViewModel.History = "Success";
                    }
                    client.StartTraining();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        private async void UseAbilityBookSilver_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    var itemId = AstrumClient.INSTANT_ABILITY_BOOK_SILVER;
                    var quantity = client.ViewModel.AbilityBookSilverAvailable;
                    var result = client.ExecuteRaiseItem(itemId, quantity);
                    if (result)
                    {
                        client.ViewModel.History = "Success";
                    }
                    client.StartTraining();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        private async void UseAbilityBookGold_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    var itemId = AstrumClient.INSTANT_ABILITY_BOOK_GOLD;
                    var quantity = client.ViewModel.AbilityBookGoldAvailable;
                    var result = client.ExecuteRaiseItem(itemId, quantity);
                    if (result)
                    {
                        client.ViewModel.History = "Success";
                    }
                    client.StartTraining();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }
    }
}
