using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.DataObjects.UiClasses
{
    public class Legend
    {
        private string legendNameTemplate = "{0} - {1}";

        public Legend() 
        {
            BackgroundColor = "Transparent";
            BackgroundOpacity = 1;
        }

        public Legend(string shortName, string longName)
        {
            ShortName = shortName;
            LongName = longName;

            BackgroundColor = "Transparent";
            BackgroundOpacity = 1;
        }

        public string BackgroundColor { get; set; }
        public double BackgroundOpacity { get; set; }

        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string LegendName 
        { 
            get
            {
                return string.Format(legendNameTemplate, ShortName, LongName);
            }
        }
    }
}
