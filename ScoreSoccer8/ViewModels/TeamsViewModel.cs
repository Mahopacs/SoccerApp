using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScoreSoccer8.Classes;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using ScoreSoccer8.Utilities;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.DbClasses;

namespace ScoreSoccer8.ViewModels
{
    public class TeamsViewModel : Notification
    {
        public TeamsViewModel()
        {
            
        }

        #region "Properties"

        private ObservableCollection<TeamModel> _teamsList;
        public ObservableCollection<TeamModel> TeamsList
        {
            get { return _teamsList; }
            set { _teamsList = value; NotifyPropertyChanged("TeamsList"); }
        }

        private bool _displayDeletedTeams;
        public bool DisplayDeletedTeams
        {
            get { return _displayDeletedTeams; }
            set
            {
                _displayDeletedTeams = value;
                NotifyPropertyChanged("DisplayDeletedTeams");
                DisplayDeletedTeamsClicked();
            }
        }

        #endregion "Properties"

        #region "Commands"

        private ICommand _addTeamsClickCommand;
        public ICommand AddTeamClickCommand
        {
            get
            {
                if (_addTeamsClickCommand == null)
                {
                    _addTeamsClickCommand = new DelegateCommand(param => this.GoToTeamDetailsScreen(), param => true);
                }

                return _addTeamsClickCommand;
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
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Help_Buttons.xaml?screenId=" + Enums.Screen.Teams, UriKind.Relative));
        }

        public void Initialize()
        {
            PopulateTeamsList("N");
        }

        void item_TeamDeleted(object sender, EventArgs e)
        {
            if (_displayDeletedTeams == true)
            {
                PopulateTeamsList("Y");
            }
            else
            {
                PopulateTeamsList("N");
            }
        }

        private void PopulateTeamsList(string displayDeletedTeams)
        {
            TeamsList = new ObservableCollection<TeamModel>();

            ObservableCollection<TeamModel> teamsList = new ObservableCollection<TeamModel>(DAL.Instance().GetTeams(displayDeletedTeams, "N").Where(x => x.Team.SampleData.ToUpper().Equals("N")));

            foreach (var item in teamsList)
            {
                item.TeamDeleted += item_TeamDeleted;
                TeamsList.Add(item);
            }
        }
       
        private void GoToTeamDetailsScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/TeamDetails.xaml?parameter=0", UriKind.Relative));
        }

        private void DisplayDeletedTeamsClicked()
        {
            if (_displayDeletedTeams == true)
            {
                PopulateTeamsList("Y");
            }
            else
            {
                PopulateTeamsList("N");
            }
        }

        #endregion "Methods"

    }
}
