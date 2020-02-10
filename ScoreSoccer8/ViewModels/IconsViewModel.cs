using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScoreSoccer8.Utilities;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.DbClasses;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using ScoreSoccer8.Classes;
namespace ScoreSoccer8.ViewModels
{
    public class IconsViewModel : Notification
    {

        public IconsViewModel()
        {
            Icons i = new Icons();
            IconList = i.IconList;
        }

        public void Initialize(Enums.Screen screen)
        {

            List<AppIcon> temp = new List<AppIcon>();

            foreach (AppIcon item in IconList)
            {
                if (item.Screens.Contains(screen))
                {
                    temp.Add(item);
                }
            }

            IconList = temp;
        }

        private List<AppIcon> _iconList;
        public List<AppIcon> IconList
        {
            get { return _iconList; }
            set { _iconList = value; NotifyPropertyChanged("IconList"); }
        }

    }
}
