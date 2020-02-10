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
    public class TeamRosterModel : Notification
    {
        public event EventHandler PlayerRosterDeleted;

        public delegate void PlayerTapped(object sender, EventArgs e);
        public event PlayerTapped PlayerTappedByUser;

        public TeamRosterModel()
        {
            PlayerOpacity = 1;
            Player = new DbClasses.Player();
            TeamRoster = new DbClasses.TeamRoster();
            Team = new DbClasses.Team();
        }

        public TeamRosterModel(TeamRosterModel p)
        {
            if (Player == null)
            {
                Player = new Player();
            }

            Player.PlayerID = p.Player.PlayerID;
            Player.FirstName = p.Player.FirstName;
            Player.LastName = p.Player.LastName;
            Player.Height = p.Player.Height;
            Player.Weight = p.Player.Weight;
            Player.Kicks = p.Player.Kicks;
            FieldColumn = p.FieldColumn;
            FieldRow = p.FieldRow;
            UniqueIdentifier = p.UniqueIdentifier;
            JerseySource = p.JerseySource;

            if (Team == null)
            {
                Team = new Team();
            }

            Team.Coach = p.Team.Coach;
            Team.Color = p.Team.Color;
            Team.ContactNumber = p.Team.ContactNumber;
            Team.Flag = p.Team.Flag;
            Team.JerseyID = p.Team.JerseyID;
            Team.Notes = p.Team.Notes;
            Team.TeamID = p.Team.TeamID;
            Team.TeamName = p.Team.TeamName;
            Team.TeamShortName = p.Team.TeamShortName;


            if (TeamRoster == null)
            {
                TeamRoster = new TeamRoster();
            }

            TeamRoster.Active = p.TeamRoster.Active;
            TeamRoster.PlayerID = p.TeamRoster.PlayerID;
            //   TeamRoster.RosterDisplayText = p.TeamRoster.RosterDisplayText;

            TeamRoster.TeamID = p.TeamRoster.TeamID;
            TeamRoster.UniformNumber = p.TeamRoster.UniformNumber;
            TeamRoster.Visible = p.TeamRoster.Visible;

            Home = p.Home;
            PlayerOpacity = p.PlayerOpacity;
        }

        /// <summary>
        /// Load from the database.
        /// </summary>
        /// <param name="team"></param>
        /// <param name="player"></param>
        /// <param name="roster"></param>
        public TeamRosterModel(Team team, Player player, TeamRoster roster, EventRoster eventRoster)
        {
            Team = team;
            Player = player;
            TeamRoster = roster;
            EventRoster = eventRoster;
            RosterDisplayText = TeamRoster.UniformNumber.PadRight(4, ' ') + " " + Player.FirstName + " " + Player.LastName;
        }

        public TeamRosterModel(Team team, Player player, TeamRoster roster)
        {
            Team = team;
            Player = player;
            TeamRoster = roster;
            RosterDisplayText = TeamRoster.UniformNumber.PadRight(4, ' ') + " " + Player.FirstName + " " + Player.LastName;
        }

        private Team _team;
        public Team Team
        {
            get { return _team; }
            set { _team = value; NotifyPropertyChanged("Team"); }
        }

        private Player _player;
        public Player Player
        {
            get { return _player; }
            set { _player = value; NotifyPropertyChanged("Player"); }
        }

        private EventRoster _eventRoster;
        public EventRoster EventRoster
        {
            get { return _eventRoster; }
            set { _eventRoster = value; NotifyPropertyChanged("EventRoster"); }
        }

        private TeamRoster _teamRoster;
        public TeamRoster TeamRoster
        {
            get { return _teamRoster; }
            set { _teamRoster = value; NotifyPropertyChanged("TeamRoster"); }
        }

        private string _rosterDisplayText;
        public string RosterDisplayText
        {
            get { return _rosterDisplayText; }
            set { _rosterDisplayText = value; NotifyPropertyChanged("RosterDisplayText"); }
        }

        private double _playerOpacity;
        public double PlayerOpacity
        {
            get { return _playerOpacity; }
            set { _playerOpacity = value; NotifyPropertyChanged("PlayerOpacity"); }
        }

        public int FieldColumn { get; set; }
        public int FieldRow { get; set; }
        public bool Home { get; set; }
        public int UniqueIdentifier { get; set; }

        private string _jerseySource;
        public string JerseySource
        {
            get { return _jerseySource; }
            set { _jerseySource = value; NotifyPropertyChanged("JerseySource"); }
        }

        private Visibility _playerVisibility;
        public Visibility PlayerVisibility
        {
            get { return _playerVisibility; }
            set { _playerVisibility = value; NotifyPropertyChanged("PlayerVisibility"); }
        }

        private bool _toggledForSub;
        public bool ToggledForSub
        {
            get { return _toggledForSub; }
            set { _toggledForSub = value; NotifyPropertyChanged("ToggledForSub"); }
        }

        private int _uniformNumberInt;
        public int UniformNumberInt
        {
            get { return _uniformNumberInt; }
            set
            { _uniformNumberInt = value; NotifyPropertyChanged("UniformNumberInt"); }
        }

        #region "Commands"


        private ICommand _goToDeletePlayerRosterCommand;
        public ICommand GoToDeletePlayerRosterCommand
        {
            get
            {
                if (_goToDeletePlayerRosterCommand == null)
                {
                    _goToDeletePlayerRosterCommand = new DelegateCommand(param => this.GoToDeletePlayerRoster(), param => true);
                }

                return _goToDeletePlayerRosterCommand;
            }
        }

        private ICommand _goToPlayerDetailsCommand;
        public ICommand GoToPlayerDetailsCommand
        {
            get
            {
                if (_goToPlayerDetailsCommand == null)
                {
                    _goToPlayerDetailsCommand = new DelegateCommand(param => this.GoToPlayerDetailsScreen(), param => true);
                }

                return _goToPlayerDetailsCommand;
            }
        }

        /// <summary>
        /// This command is only to be used by the Game Manager Screen.
        /// </summary>
        private ICommand _playerTappedCommand;
        public ICommand PlayerTappedCommand
        {
            get
            {
                if (_playerTappedCommand == null)
                {
                    _playerTappedCommand = new DelegateCommand(param => this.PlayerTappedInGameManagerScreen(), param => true);
                }

                return _playerTappedCommand;
            }
        }

        public void PlayerTappedInGameManagerScreen()
        {
            if (PlayerTappedByUser != null)
            {
                PlayerTappedByUser(this, new EventArgs());
            }
        }

        #endregion "Commands"

        #region "Events"

        protected virtual void OnPlayerRosterDeleted(EventArgs e)
        {
            EventHandler handler = PlayerRosterDeleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion "Events"

        #region "Methods"

        private void GoToDeletePlayerRoster()
        {
            MessageBoxResult result = MessageBox.Show(AppResources.Delete + " '" + Player.FirstName + " " + Player.LastName + "'" + AppResources.FromRoster + "?", AppResources.DeletePlayerFromRoster, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                TeamRoster teamRoster = new TeamRoster();
                teamRoster.TeamID = Team.TeamID;
                teamRoster.PlayerID = Player.PlayerID;

                DAL.Instance().DeleteTeamRoster(teamRoster);
                OnPlayerRosterDeleted(EventArgs.Empty);
            }
        }

        private void GoToPlayerDetailsScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/PlayerDetails.xaml?gameID=0" + "&teamID=" + Team.TeamID + "&playerID=" + Player.PlayerID, UriKind.Relative));
        }

        #endregion "Methods"

    }
}
