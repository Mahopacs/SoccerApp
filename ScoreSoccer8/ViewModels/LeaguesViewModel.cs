using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ScoreSoccer8.ViewModels
{
    public class LeaguesViewModel : Notification
    {
        public LeaguesViewModel()
        {
            LeaguesList = new ObservableCollection<LeagueModel>();
        }

        #region "Properties"

        private ObservableCollection<LeagueModel> _leaguesList;
        public ObservableCollection<LeagueModel> LeaguesList
        {
            get { return _leaguesList; }
            set { _leaguesList = value; NotifyPropertyChanged("LeaguesList"); }
        }

        private bool _displayDeletedLeagues;
        public bool DisplayDeletedLeagues
        {
            get { return _displayDeletedLeagues; }
            set
            {
                _displayDeletedLeagues = value;
                NotifyPropertyChanged("DisplayDeletedLeagues");
                DisplayDeletedLeaguesClicked();
            }
        }

        #endregion "Properties"

        #region "Commands"

        private ICommand _addLeaguesClickCommand;
        public ICommand AddLeagueClickCommand
        {
            get
            {
                if (_addLeaguesClickCommand == null)
                {
                    _addLeaguesClickCommand = new DelegateCommand(param => this.GoToLeagueDetailsScreen(), param => true);
                }

                return _addLeaguesClickCommand;
            }
        }

        private ICommand _helpClickCommand;
        public ICommand HelpClickCommand
        {
            get
            {
                if (_helpClickCommand == null)
                {
                    _helpClickCommand = new DelegateCommand(param => this.GoHelpScreen(), param => true);
                }

                return _helpClickCommand;
            }
        }

        #endregion "Commands"

        #region "Methods"

        private void GoHelpScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Help_Buttons.xaml?screenId=" + Enums.Screen.Leagues, UriKind.Relative));
        }

        public void Initialize()
        {
            PopulateLeaguesList(false);
        }

        void item_LeagueDeleted(object sender, EventArgs e)
        {
            if (_displayDeletedLeagues == true)
            {
                PopulateLeaguesList(true);
            }
            else
            {
                PopulateLeaguesList(false);
            }
        }

        private void PopulateLeaguesList(bool displayDeletedLeagues)
        {
            LeaguesList = DAL.Instance().GetLeagues(displayDeletedLeagues, false);
       
            foreach (var item in LeaguesList)
            {
                item.LeagueDeleted += item_LeagueDeleted;
            }
        }

        private void GoToLeagueDetailsScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/LeagueDetails.xaml?parameter=0", UriKind.Relative));
        }

        private void DisplayDeletedLeaguesClicked()
        {
            if (_displayDeletedLeagues == true)
            {
                PopulateLeaguesList(true);
            }
            else
            {
                PopulateLeaguesList(false);
            }
        }

        #endregion "Methods"

    }
}
