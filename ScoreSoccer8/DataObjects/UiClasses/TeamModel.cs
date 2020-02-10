using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.Resources;
using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ScoreSoccer8.DataObjects.UiClasses
{
    public class TeamModel : Notification
    {
        public event EventHandler TeamDeleted;

        public TeamModel()
        {
            Team = new Team();
        }

        #region "Properties"

        private Team _team;
        public Team Team
        {
            get { return _team; }
            set { _team = value; NotifyPropertyChanged("Team"); }
        }

        private string _jerseySource;
        public string JerseySource
        {
            get { return _jerseySource; }
            set { _jerseySource = value; NotifyPropertyChanged("JerseySource"); }
        }

        #endregion "Properties"

        #region "Events"

        protected virtual void OnTeamDeleted(EventArgs e)
        {
            EventHandler handler = TeamDeleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion "Events"

        #region Commands

        private ICommand _goToTeamDetailsCommand;
        public ICommand GoToTeamDetailsCommand
        {
            get
            {
                if (_goToTeamDetailsCommand == null)
                {
                    _goToTeamDetailsCommand = new DelegateCommand(param => this.GoToTeamDetailsScreen(), param => true);
                }

                return _goToTeamDetailsCommand;
            }
        }

        private ICommand _gotoRostersCommand;
        public ICommand GoToRostersCommand
        {
            get
            {
                if (_gotoRostersCommand == null)
                {
                    _gotoRostersCommand = new DelegateCommand(param => this.GoToRostersScreen(), param => true);
                }

                return _gotoRostersCommand;
            }
        }

        private ICommand _goToDeleteTeamsCommand;
        public ICommand GoToDeleteTeamsCommand
        {
            get
            {
                if (_goToDeleteTeamsCommand == null)
                {
                    _goToDeleteTeamsCommand = new DelegateCommand(param => this.GoToDeleteTeam(), param => true);
                }

                return _goToDeleteTeamsCommand;
            }
        }

        #endregion Commands

        #region "Methods"

        private void GoToTeamDetailsScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/TeamDetails.xaml?parameter=" + Team.TeamID, UriKind.Relative));
        }

        private void GoToRostersScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Rosters.xaml?teamID=" + Team.TeamID, UriKind.Relative));
        }

        private void GoToDeleteTeam()
        {
            MessageBoxResult result = MessageBox.Show(AppResources.Delete + " '" + Team.TeamName + "' " + AppResources.Team + "?", AppResources.DeleteTeam, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                this.Team.Visible = "N";
                BaseTableDataAccess.Instance().UpsertTeam(this.Team);
                OnTeamDeleted(EventArgs.Empty);
            }
        }
        #endregion "Methods"

    }
}
