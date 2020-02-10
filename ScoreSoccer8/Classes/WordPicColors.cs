using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ScoreSoccer8.Classes
{
    public class WordPicColors : Notification
    {
        public WordPicColors(SolidColorBrush brush, string colorName)
        {
            ColorToBindTo = brush;
            ColorName = colorName;
        }

        private string _colorName;
        public string ColorName
        {
            get { return _colorName; }
            set { _colorName = value; NotifyPropertyChanged("ColorName"); }
        }


        private SolidColorBrush _colorToBindTo;
        public SolidColorBrush ColorToBindTo
        {
            get { return _colorToBindTo; }
            set { _colorToBindTo = value; NotifyPropertyChanged("ColorToBindTo"); }
        }    
    }
}
