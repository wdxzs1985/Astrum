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
            UserSelector thisCtrl = (UserSelector)obj;
            thisCtrl.User.Content = thisCtrl.SelectedUserName;

            thisCtrl.DecreaseIndexButton.IsEnabled = !thisCtrl.IsFirst;
            thisCtrl.IncreaseIndexButton.IsEnabled = !thisCtrl.IsLast;

            thisCtrl.RaiseUserChangedEvent();
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

        public UserSelector()
        {
            InitializeComponent();
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
