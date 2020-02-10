using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ScoreSoccer8.Converters
{
    public class SubColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool visible = (bool)value;

            //return white normally
            SolidColorBrush returnValue = new SolidColorBrush(Color.FromArgb(Colors.White.A, Colors.White.R, Colors.White.G, Colors.White.B));

            if (visible)
            {
                returnValue = new SolidColorBrush(Color.FromArgb(Colors.Red.A, Colors.Red.R, Colors.Red.G, Colors.Red.B));
            }

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
