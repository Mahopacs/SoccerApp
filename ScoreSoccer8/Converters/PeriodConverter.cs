using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ScoreSoccer8.Converters
{
    public class PeriodConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int period = System.Convert.ToInt32(value);
            string returnValue = string.Empty;

            if (period == 1)
            {
                returnValue = "1st";
            }

            if (period == 2)
            {
                returnValue = "2nd";
            }

            if (period == 3)
            {
                returnValue = "3rd";
            }

            if (period >= 4)
            {
                returnValue = period.ToString() + "th";
            }

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
