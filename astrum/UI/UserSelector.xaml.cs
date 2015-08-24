using System;
using System.Collections.Generic;
using System.ComponentModel;
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

using Astrum.Json;

namespace Astrum.UI
{
    /// <summary>
    /// Interaction logic for UserSelector.xaml
    /// </summary>
    public partial class UserSelector : UserControl
    {

        public static readonly DependencyProperty UserListProperty = DependencyProperty.Register(
        "UserList",
        typeof(List<LoginUser>),
        typeof(UserSelector),
        new FrameworkPropertyMetadata(
                new PropertyChangedCallback(UserSelector.OnUserChanged)
        )
);

        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(
        "SelectedIndex",
        typeof(int),
        typeof(UserSelector),
        new FrameworkPropertyMetadata(
                new PropertyChangedCallback(UserSelector.OnUserChanged)
        )
);

        private static void OnUserChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            UserSelector self = (UserSelector)obj;
            self.User.Content = self.SelectedUserName;

            if(self.SelectedUserLeader != null && self.SelectedUserLeader != "")
            {
                var path = String.Format("./cache/{0}-avatar.png", self.SelectedUser.leader);

                ImageHelper.LoadImage(path, self.Avatar, "Images/ic_account_circle_white_48dp.png");
            }
            else
            {
                self.Avatar.Source = self._accountImage;
            }

            self.DecreaseIndexButton.IsEnabled = !self.IsFirst;
            self.IncreaseIndexButton.IsEnabled = !self.IsLast;

            self.RaiseUserChangedEvent();
        }

        public static readonly RoutedEvent UserChangedEvent = EventManager.RegisterRoutedEvent(
        "UserChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UserSelector));

        public event RoutedEventHandler UserChanged
        {
            add { AddHandler(UserChangedEvent, value); }
            remove { RemoveHandler(UserChangedEvent, value); }
        }

        void RaiseUserChangedEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(UserSelector.UserChangedEvent);
            RaiseEvent(newEventArgs);
        }

        private ImageSource _accountImage = null;

        public UserSelector()
        {
            InitializeComponent();

            _accountImage = Avatar.Source;
        }
        
        public List<LoginUser> UserList
        {
            get
            {
                return (List<LoginUser>)GetValue(UserListProperty);
            }
            set
            {
                SetValue(UserListProperty, value);
            }
        }

        [Browsable(true)]
        public int SelectedIndex
        {
            get
            {
                return (int)GetValue(SelectedIndexProperty);
            }
            set
            {
                SetValue(SelectedIndexProperty, value);
            }
        }

        public LoginUser SelectedUser
        {
            get
            {
                if (IsEmpty)
                {
                    return null;
                }
                return UserList[SelectedIndex];
            }
        }


        public string SelectedUserName
        {
            get
            {
                if(IsEmpty)
                {
                    return "";
                }
                return UserList[SelectedIndex].name;
            }
        }


        public string SelectedUserLeader
        {
            get
            {
                if (IsEmpty)
                {
                    return "";
                }
                return UserList[SelectedIndex].leader;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return UserList == null || UserList.Count == 0;
            }
        }

        public bool IsFirst
        {
            get
            {
                if (IsEmpty)
                {
                    return true;
                }
                return SelectedIndex <= 0;
            }
        }

        public bool IsLast
        {
            get
            {
                if (IsEmpty)
                {
                   return true;
                }
                return SelectedIndex >= UserList.Count - 1;
            }
        }

        private void DecreaseIndexButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedIndex <= 0)
            {
                SelectedIndex = 0;
            }
            else
            {
                SelectedIndex--;
            }
        }

        private void IncreaseIndexButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedIndex >= UserList.Count - 1)
            {
                SelectedIndex = UserList.Count - 1;
            }
            else
            {
                SelectedIndex++;
            }
        }
    }

    
}
