using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScoreSoccer8.Utilities;
using System.Collections.ObjectModel;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.Resources;
using ScoreSoccer8.DataAccess;
using System.Diagnostics;
using System.Windows.Input;
using System.Globalization;

namespace ScoreSoccer8.ViewModels
{
    public class StatPickerHelpViewModel : Notification
    {

        public void GenerateList()
        {

            ObservableCollection<StatCategoryModel> statList = new ObservableCollection<StatCategoryModel>();
            statList = DAL.Instance().GetVisibleStats(false, false);

            foreach (var item in statList)
            {
                //Debug.WriteLine(item.StatCategory.StatCategoryName);

                //if (item.Descriptions != null)
                //{
                //    foreach (var descriptions in item.Descriptions)
                //    {
                //        //Debug.WriteLine(item.StatCategory.StatCategoryName + "," + descriptions.StatDescription.StatDescriptionName);
                //    }
                //}
            }

        }




        public StatPickerHelpViewModel()
        {

            LocalLanguage = System.Globalization.CultureInfo.CurrentUICulture.DisplayName;

            ObservableCollection<Legend> legendList = new ObservableCollection<Legend>()
            {

                    new Legend { ShortName = "Shot ", LongName = AppResources.Shot },
                    new Legend { ShortName = "- Shot Details", LongName = AppResources.ShotDetails },
                    new Legend { ShortName = "Pass ", LongName = AppResources.Pass },
                    new Legend { ShortName = "- Excellent", LongName = AppResources.PassesExcellent },
                    new Legend { ShortName = "- Good", LongName = AppResources.PassesGood },
                    new Legend { ShortName = "- Poor", LongName = AppResources.PassesPoor },
                    new Legend { ShortName = "Turnover ", LongName = AppResources.Turnover },
                    new Legend { ShortName = "- Illegal Throw In", LongName = AppResources.IllegalThrowIn },
                    new Legend { ShortName = "- Lost Dribble", LongName = AppResources.LostDribble },
                    new Legend { ShortName = "Offsides ", LongName = AppResources.Offsides },
                    new Legend { ShortName = "Foul Committed ", LongName = AppResources.FoulCommitted },
                    new Legend { ShortName = "- Kicking", LongName = AppResources.Kicking },
                    new Legend { ShortName = "- Tripping", LongName = AppResources.Tripping },
                    new Legend { ShortName = "- Charging", LongName = AppResources.Charging },
                    new Legend { ShortName = "- Pushing", LongName = AppResources.Pushing },
                    new Legend { ShortName = "- Holding", LongName = AppResources.Holding },
                    new Legend { ShortName = "- Illegal Tackle", LongName = AppResources.IllegalTackle },
                    new Legend { ShortName = "Out Of Bounds ", LongName = AppResources.OutOfBounds },
                    new Legend { ShortName = "Cross ", LongName = AppResources.Cross },
                    new Legend { ShortName = "- Excellent", LongName = AppResources.CrossesExcellent },
                    new Legend { ShortName = "- Good", LongName = AppResources.CrossesGood },
                    new Legend { ShortName = "- Poor", LongName = AppResources.CrossesPoor },
                    new Legend { ShortName = "Throw In ", LongName = AppResources.ThrowIn },
                    new Legend { ShortName = "Corner Kick ", LongName = AppResources.CornerKick },
                    new Legend { ShortName = "- Excellent", LongName = AppResources.CornersExcellent },
                    new Legend { ShortName = "- Good", LongName = AppResources.CornersGood },
                    new Legend { ShortName = "- Poor", LongName = AppResources.CornersPoor },
                    new Legend { ShortName = "- For Goal", LongName = AppResources.ForGoal },
                    new Legend { ShortName = "Tackle ", LongName = AppResources.Tackle },
                    new Legend { ShortName = "Goalie Kick ", LongName = AppResources.GoalieKick },
                    new Legend { ShortName = "Own Goal ", LongName = AppResources.OwnGoal },
                    new Legend { ShortName = "Foul Drawn ", LongName = AppResources.FoulDrawn },
                    new Legend { ShortName = "Direct Free Kick ", LongName = AppResources.DirectFreeKick },
                    new Legend { ShortName = "- For Goal", LongName = AppResources.ForGoal },
                    new Legend { ShortName = "- Not For Goal", LongName = AppResources.NotForGoal },
                    new Legend { ShortName = "Indirect Free Kick ", LongName = AppResources.IndirectFreeKick },
                    new Legend { ShortName = "Penalty Kick ", LongName = AppResources.PenaltyKick },
                    new Legend { ShortName = "- Shot Details", LongName = AppResources.ShotDetails },
                    new Legend { ShortName = "Yellow Card ", LongName = AppResources.YellowCard },
                    new Legend { ShortName = "- Unsportsmanlike Conduct", LongName = AppResources.UnsportsmanlikeConduct },
                    new Legend { ShortName = "- Delaying Restart Of Play", LongName = AppResources.DelayingRestartOfPlay },
                    new Legend { ShortName = "Red Card ", LongName = AppResources.RedCard },
                    new Legend { ShortName = "- Foul Play", LongName = AppResources.FoulPlay },
                    new Legend { ShortName = "- Violent Conduct", LongName = AppResources.ViolentConduct },
                    new Legend { ShortName = "- Illegal Hands", LongName = AppResources.NotForGoal },
                    new Legend { ShortName = "- Second Yellow Card", LongName = AppResources.SecondYellowCard },
                    new Legend { ShortName = "Drop Kick ", LongName = AppResources.DropKick },
                    new Legend { ShortName = "- Excellent", LongName = AppResources.DropKicksExcellent },
                    new Legend { ShortName = "- Good", LongName = AppResources.DropKicksGood },
                    new Legend { ShortName = "- Poor", LongName = AppResources.DropKicksPoor },
                    new Legend { ShortName = "Dribble ", LongName = AppResources.Dribble },
                    new Legend { ShortName = "Shootout Kick ", LongName = AppResources.ShootoutKick },
                    new Legend { ShortName = "- Shot Details", LongName = AppResources.ShotDetails },

            };

            int i = 0;
            foreach (Legend item in legendList)
            {
                if (i % 2 != 0)
                {
                    item.BackgroundColor = "Black";
                    item.BackgroundOpacity = 0.2;
                }

                i++;
            }

            LegendList = legendList;
        }

        private string _localLanguage = "";
        public string LocalLanguage
        {
            get { return _localLanguage; }
            set { _localLanguage = value; NotifyPropertyChanged("LocalLanguage"); }
        }

        private ObservableCollection<Legend> _legendList = new ObservableCollection<Legend>();
        public ObservableCollection<Legend> LegendList
        {
            get { return _legendList; }
            set { _legendList = value; NotifyPropertyChanged("LegendList"); }
        }

    }
}
