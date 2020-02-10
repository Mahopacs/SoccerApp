using ScoreSoccer8.Classes;
using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.Resources;
using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

// We are not using Colors, we deciced to use Jerseys
namespace ScoreSoccer8.ViewModels
{
    public class TeamDetailsViewModel : Notification
    {

        public TeamDetailsViewModel()
        {
            TeamDetails = new TeamModel();
            LoadJerseys();
           // LoadColors();
            LeaguesList = new ObservableCollection<LeagueModel>();
            LeaguesList = DAL.Instance().GetLeagues(false,true);
        }

        #region "Properties"

        private TeamModel _teamDetails;
        public TeamModel TeamDetails
        {
            get { return _teamDetails; }
            set { _teamDetails = value; NotifyPropertyChanged("TeamDetails"); }
        }

        private ObservableCollection<LeagueModel> _leaguesList;
        public ObservableCollection<LeagueModel> LeaguesList
        {
            get { return _leaguesList; }
            set { _leaguesList = value; NotifyPropertyChanged("LeaguesList"); }
        }

        private ObservableCollection<WordPicColors> _wpColors;
        public ObservableCollection<WordPicColors> WpColors
        {
            get { return _wpColors; }
            set { _wpColors = value; NotifyPropertyChanged("WpColors"); }
        }

        private ObservableCollection<Jersey> _jerseyList;
        public ObservableCollection<Jersey> JerseyList
        {
            get { return _jerseyList; }
            set { _jerseyList = value; NotifyPropertyChanged("JerseyList"); }
        }

        private Jersey _selectedJersey;
        public Jersey SelectedJersey
        {
            get { return _selectedJersey; }
            set { _selectedJersey = value; NotifyPropertyChanged("SelectedJersey"); }
        }

        private WordPicColors _selectedColor;
        public WordPicColors SelectedColor
        {
            get { return _selectedColor; }
            set { _selectedColor = value; NotifyPropertyChanged("SelectedColor"); }
        }

        private LeagueModel _selectedLeague;
        public LeagueModel SelectedLeague
        {
            get { return _selectedLeague; }
            set { _selectedLeague = value; NotifyPropertyChanged("SelectedLeague"); }
        }

        #endregion "Properties"

        #region "Methods"

        public void Initialize(int teamID)
        {
            if (teamID == 0)
            {
             //   SetDefaultSelectedColor();
                SetDefaultSelectedJersey();
                SetDefaultSelectedLeague();
            }
            else
            {
                TeamDetails = DAL.Instance().GetTeam(teamID);
             //   SetSelectedColor();
                SetSelectedJersey();
                SetSelectedLeague();
            }
        }

        public void SaveToDatabase()
        {
            BaseTableDataAccess.Instance().UpsertTeam(this.TeamDetails.Team);
        }


        private void SetSelectedColor()
        {
            foreach (var row in WpColors)
            {
                if (row.ColorName.ToUpper().Equals(TeamDetails.Team.Color.ToUpper()))
                {
                    SelectedColor = row;
                }
            }
        }

        private void SetSelectedJersey()
        {
            foreach (var row in JerseyList)
            {
                if (row.JerseyID.Equals(TeamDetails.Team.JerseyID))
                {
                    SelectedJersey = row;
                }
            }
        }

        private void SetSelectedLeague()
        {
            foreach (var row in LeaguesList)
            {
                if (row.League.LeagueID.Equals(TeamDetails.Team.LeagueID))
                {
                    SelectedLeague = row;
                }
            }
        }

        private void SetDefaultSelectedColor()
        {
            foreach (var row in WpColors)
            {
                SelectedColor = row;
            }
        }

        private void SetDefaultSelectedJersey()
        {
            foreach (var row in JerseyList)
            {
                SelectedJersey = row;
            }
        }

        private void SetDefaultSelectedLeague()
        {
            foreach (var row in LeaguesList)
            {
                if (row.League.LeagueName == AppResources.NoLeague)
                {
                    SelectedLeague = row;
                    break;
                }
            }
        }

        private void LoadJerseys()
        {
            JerseyList = new ObservableCollection<Jersey>();
            JerseyList = DAL.Instance().GetJerseys();
         }

        //Not used (decided to go with Jerseys)
        private void LoadColors()
        {
            WpColors = new ObservableCollection<WordPicColors>();
            WpColors.Add(new WordPicColors(new SolidColorBrush(Colors.Black), "Black"));
            WpColors.Add(new WordPicColors(new SolidColorBrush(Colors.Blue), "Blue"));
            WpColors.Add(new WordPicColors(new SolidColorBrush(Colors.Brown), "Brown"));
            WpColors.Add(new WordPicColors(new SolidColorBrush(Colors.Cyan), "Cyan"));

            WpColors.Add(new WordPicColors(new SolidColorBrush(Colors.DarkGray), "Dark Gray"));
            WpColors.Add(new WordPicColors(new SolidColorBrush(Colors.Gray), "Gray"));
            WpColors.Add(new WordPicColors(new SolidColorBrush(Colors.Green), "Green"));
            WpColors.Add(new WordPicColors(new SolidColorBrush(Colors.LightGray), "Light Gray"));

            WpColors.Add(new WordPicColors(new SolidColorBrush(Colors.Magenta), "Magenta"));
            WpColors.Add(new WordPicColors(new SolidColorBrush(Colors.Orange), "Orange"));
            WpColors.Add(new WordPicColors(new SolidColorBrush(Colors.Purple), "Purple"));
            WpColors.Add(new WordPicColors(new SolidColorBrush(Colors.Red), "Red"));

            WpColors.Add(new WordPicColors(new SolidColorBrush(Colors.White), "White"));
            WpColors.Add(new WordPicColors(new SolidColorBrush(Colors.Yellow), "Yellow"));
        }

        #endregion "Methods"
    }
}
