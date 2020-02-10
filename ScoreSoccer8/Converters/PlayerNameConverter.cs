using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ScoreSoccer8.Converters
{
    public class PlayerNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string firstName = value as string;

            string returnValue = string.Empty;

            if (firstName.Length > 6)
            {
                returnValue = firstName.Substring(0, 6);
            }
            else
            {
                returnValue = firstName;
            }

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
