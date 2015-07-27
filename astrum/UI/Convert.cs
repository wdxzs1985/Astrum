using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Astrum.UI
{
    class Convert
    {
    }

    public class ProgressBarForegoundConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int target = (int) value;

            var foreground = new SolidColorBrush(Color.FromRgb(1, 211, 40));

            if (target <= 10)
            {
                foreground = new SolidColorBrush(Color.FromRgb(244, 67, 54));
            }
            else if (target <= 40)
            {
                foreground = new SolidColorBrush(Color.FromRgb(255, 152, 0));
            }

            return foreground;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool state = (bool)value;

            return state ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
