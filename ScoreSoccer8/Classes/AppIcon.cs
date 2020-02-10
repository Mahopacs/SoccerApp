using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScoreSoccer8.Utilities;

namespace ScoreSoccer8.Classes
{
    public class AppIcon
    {
        public AppIcon(Enums.IconButtonType type, string text, string imagePath, List<Enums.Screen> screens) 
        {
            Type = type;
            Text = text;
            ImagePath = imagePath;
            Screens = screens;
        }

        public Enums.IconButtonType Type { get; set; }

        public List<Enums.Screen> Screens { get; set; }

        private string _imagePath = "/Assets/na.png";
        public string ImagePath 
        { 
            get { return _imagePath; }
            set { if(!string.IsNullOrEmpty(value)){_imagePath = value;} } 
        }

        public string Text { get; set; }
    }
}
