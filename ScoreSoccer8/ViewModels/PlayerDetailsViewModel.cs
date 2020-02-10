using ScoreSoccer8.Classes;
using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ScoreSoccer8.ViewModels
{
    public class PlayerDetailsViewModel : Notification
    {

        public PlayerDetailsViewModel()
        {
            PlayerDetails = new Player();
        }

        #region "Properties"

        private Player _playerDetails;
        public Player PlayerDetails
        {
            get { return _playerDetails; }
            set { _playerDetails = value; NotifyPropertyChanged("PlayerDetails"); }
        }

        private int _gameID;
        public int GameID
        {
            get { return _gameID; }
            set { _gameID = value; NotifyPropertyChanged("GameID"); }
        }

        private int _teamID;
        public int TeamID
        {
            get { return _teamID; }
            set { _teamID = value; NotifyPropertyChanged("TeamID"); }
        }

        private int _playerID;
        public int PlayerID
        {
            get { return _playerID; }
            set { _playerID = value; NotifyPropertyChanged("PlayerID"); }
        }

        private bool _kicksLeftChecked;
        public bool KicksLeftChecked
        {
            get { return _kicksLeftChecked; }
            set
            {
                _kicksLeftChecked = value;
                PlayerDetails.Kicks = "Left";
                NotifyPropertyChanged("KicksLeftChecked");
            }
        }

        private bool _kicksRightChecked;
        public bool KicksRightChecked
        {
            get { return _kicksRightChecked; }
            set
            {
                _kicksRightChecked = value;
                PlayerDetails.Kicks = "Right";
                NotifyPropertyChanged("KicksRightChecked");
            }
        }

        private bool _activeChecked;
        public bool ActiveChecked
        {
            get { return _activeChecked; }
            set
            {
                _activeChecked = value;
                Active = "Y";
                NotifyPropertyChanged("ActiveChecked");
            }
        }

        private bool _inActiveChecked;
        public bool InActiveChecked
        {
            get { return _inActiveChecked; }
            set
            {
                _inActiveChecked = value;
                Active = "N";
                NotifyPropertyChanged("InActiveChecked");
            }
        }

        private Visibility _isVisible;
        public Visibility IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; NotifyPropertyChanged("IsVisible"); }
        }

        private Visibility _isDeleted;
        public Visibility IsDeleted
        {
            get { return _isDeleted; }
            set { _isDeleted = value; NotifyPropertyChanged("IsDeleted"); }
        }

        private string _uniformNumber;
        public string UniformNumber
        {
            get { return _uniformNumber; }
            set { _uniformNumber = value; NotifyPropertyChanged("UniformNumber"); }
        }

        private string _active;
        public string Active
        {
            get { return _active; }
            set { _active = value; NotifyPropertyChanged("Active"); }
        }

        private DateTime _birthDate;
        public DateTime BirthDate
        {
            get { return _birthDate; }
            set { _birthDate = value; NotifyPropertyChanged("BirthDate"); }
        }
  
        #endregion "Properties"

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

        #region "Methods"

        private void GoHelpScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Help_Buttons.xaml?screenId=" + Enums.Screen.PlayerDetails, UriKind.Relative));
        }

        public void Initialize(int gameID, int teamID, int playerID)
        {
            GameID = gameID;
            TeamID = teamID;
            PlayerID = playerID;

            if (teamID == 0)
            {
                IsVisible = Visibility.Collapsed;
            }
            else
            {
                IsVisible=Visibility.Visible;
                TeamRoster teamRosterEntry = new TeamRoster();
                teamRosterEntry = DAL.Instance().GetPlayerTeamRosterEntry(TeamID, playerID);

               
                UniformNumber = teamRosterEntry.UniformNumber;
                if (teamRosterEntry.Active == "Y")
                {
                    ActiveChecked = true;
                }
                else
                {
                    InActiveChecked = true;
                }
            }

            if (playerID == 0)
            {
                PlayerDetails.PlayerID = playerID;
                KicksRightChecked = true;
                IsDeleted = Visibility.Collapsed;
                BirthDate = DateTime.Now.AddDays(-7000);
            }
            else
            {
                PlayerDetails = DAL.Instance().GetPlayer(playerID);

                BirthDate = PlayerDetails.BirthDate;

                if (BirthDate.Year < 1900)
                {
                    BirthDate = DateTime.Now;
                }

                if (PlayerDetails.Kicks == "Left")
                {
                    KicksLeftChecked = true;
                }
                else
                {
                    KicksRightChecked = true;
                }

                //This functionality is not in place now
                //If this is a deleted player (i.e. visible = 'N') then we need to show label on bottom of screen that this is a deleted player
                if (PlayerDetails.Visible == "Y")
                {
                    IsDeleted = Visibility.Collapsed;
                }
                else
                {
                    IsDeleted = Visibility.Visible;
                }
            }       
        }
   
        public void SaveToDatabase(int teamID)
        {
            BaseTableDataAccess.Instance().UpsertPlayer(this.PlayerDetails, GameID, teamID);

            if (teamID != 0)
            {
                TeamRoster teamRoster = new TeamRoster();
                teamRoster.PlayerID = this.PlayerDetails.PlayerID;
                teamRoster.TeamID = teamID;
                teamRoster.UniformNumber = UniformNumber;
                teamRoster.Active = Active;
                BaseTableDataAccess.Instance().UpsertTeamRoster(teamRoster);
            }
        }

        #endregion "Methods"
    }
}
