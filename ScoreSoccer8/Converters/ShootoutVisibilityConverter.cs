using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ScoreSoccer8.Converters
{
    public class ShootoutVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility returnValue = Visibility.Collapsed;
            int? shootOutOpps = System.Convert.ToInt32(value);

            if (shootOutOpps > 0)
            {
                returnValue = Visibility.Visible;
            }

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
