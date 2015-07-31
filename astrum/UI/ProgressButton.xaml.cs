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

namespace Astrum.UI
{
    /// <summary>
    /// Interaction logic for ProgressButton.xaml
    /// </summary>
    public partial class ProgressButton : UserControl
    {
        //public event EventHandler Click;
        public event RoutedEventHandler Click;

        public ProgressButton()
        {
            InitializeComponent();
        }

        public static DependencyProperty ButtonContentProperty = 
            DependencyProperty.Register("ButtonContent", typeof(object), typeof(ProgressButton));


        public object ButtonContent
        {
            get { return GetValue(ButtonContentProperty); }
            set { SetValue(ButtonContentProperty, value); }
        }

        public static DependencyProperty ShowProgressProperty =
            DependencyProperty.Register("IsShowProgress", typeof(bool), typeof(ProgressButton), new UIPropertyMetadata(false));

        [BindableAttribute(true)]
        public bool IsShowProgress
        {
            get { return (bool) GetValue(ShowProgressProperty); }
            set { SetValue(ShowProgressProperty, value); }
        }

        public static DependencyProperty ArcThicknessProperty =
            DependencyProperty.Register("ArcThickness", typeof(double), typeof(ProgressButton), new UIPropertyMetadata(0.0d));

        [BindableAttribute(true)]
        public double ArcThickness
        {
            get { return (double) GetValue(ArcThicknessProperty); }
            set { SetValue(ArcThicknessProperty, value); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool _isShowProgress = (bool) this.IsShowProgress;
            this.IsShowProgress = !_isShowProgress;

            if (this.Click != null)
            {
                this.Click(this, e);
            }
        }
    }
}
