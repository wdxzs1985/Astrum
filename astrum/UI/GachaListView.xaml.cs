using Astrum.Json.Gacha;
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

namespace Astrum.UI
{
    /// <summary>
    /// Interaction logic for GachaList.xaml
    /// </summary>
    public partial class GachaListView : UserControl
    {

        public static readonly DependencyProperty GachaListProperty = DependencyProperty.Register(
                "GachaList",
                typeof(List<GachaInfo>),
                typeof(GachaListView),
                new FrameworkPropertyMetadata(
                        new PropertyChangedCallback(GachaListView.OnDataUpdated)
                )
        );

        private static void OnDataUpdated(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            GachaListView self = (GachaListView)obj;
            self.Root.ItemsSource = self.GachaList;
        }

        public static readonly RoutedEvent GachaEvent = EventManager.RegisterRoutedEvent(
        "Gacha", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GachaListView));
        
        public event RoutedEventHandler Gacha
        {
            add { AddHandler(GachaEvent, value); }
            remove { RemoveHandler(GachaEvent, value); }
        }


        void RaiseGachaEvent(string gachaId, bool sequence)
        {
            GachaEventArgs newEventArgs = new GachaEventArgs(GachaEvent, gachaId, sequence);
            RaiseEvent(newEventArgs);
        }

        public GachaListView()
        {
            InitializeComponent();
        }


        public List<GachaInfo> GachaList
        {
            get
            {
                return (List<GachaInfo>)GetValue(GachaListProperty);
            }
            set
            {
                SetValue(GachaListProperty, value);
            }
        }


        private void GachaButton_Click(object sender, RoutedEventArgs e)
        {
            var gachaId = (string)((Button)sender).Tag;
            RaiseGachaEvent(gachaId, false);
        }

        private void GachaSequenceButton_Click(object sender, RoutedEventArgs e)
        {
            var gachaId = (string)((Button)sender).Tag;
            RaiseGachaEvent(gachaId, true);
        }
    }

    public class GachaEventArgs : RoutedEventArgs
    {
        public GachaEventArgs(RoutedEvent routedEvent, string gachaId) : base(routedEvent)
        {
            _gachaId = gachaId;
            _sequence = false;
        }

        public GachaEventArgs(RoutedEvent routedEvent, string gachaId, bool sequence) : base(routedEvent)
        {
            _gachaId = gachaId;
            _sequence = sequence;
        }


        private readonly string _gachaId;
        private readonly bool _sequence;

        public string GachaId
        {
            get { return _gachaId; }
        }


        public bool Sequence
        {
            get { return _sequence; }
        }

    }
}
