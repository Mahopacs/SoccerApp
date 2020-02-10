using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ScoreSoccer8.Converters
{
    public class ImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string fileName = value as string;
            ImageSource imgSource = null;

            try
            {
                Uri uri = new Uri(fileName, UriKind.RelativeOrAbsolute);
                 imgSource = new BitmapImage(uri);
            }
            catch (Exception ex)
            {

            }

            

            return imgSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
