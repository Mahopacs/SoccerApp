using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.Resources;
using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ScoreSoccer8.ViewModels
{
    public class StatsReportsViewModel : Notification
    {
        public StatsReportsViewModel()
        {
            PopulateReportTypes();
            PopulateGamesList("N");
            PopulateTeamsList("N");
            PopulatePlayers("N");
            SelectedReportType = AppResources.GameStats;
            StartDate = DateTime.Now.AddDays(-30);
            EndDate = DateTime.Now;
        }

        #region "Properties"

        private ObservableCollection<string> _reportsList;
        public ObservableCollection<string> ReportsList
        {
            get { return _reportsList; }
            set { _reportsList = value; NotifyPropertyChanged("ReportsList"); }
        }

        private ObservableCollection<GameModel> _gamesList;
        public ObservableCollection<GameModel> GamesList
        {
            get { return _gamesList; }
            set { _gamesList = value; NotifyPropertyChanged("GamesList"); }
        }

        private ObservableCollection<TeamModel> _teamsList;
        public ObservableCollection<TeamModel> TeamsList
        {
            get { return _teamsList; }
            set { _teamsList = value; NotifyPropertyChanged("TeamsList"); }
        }

        private ObservableCollection<PlayerModel> _playersList;
        public ObservableCollection<PlayerModel> PlayersList
        {
            get { return _playersList; }
            set { _playersList = value; NotifyPropertyChanged("PlayersList"); }
        }

        private string _selectedReportType;
        public string SelectedReportType
        {
            get { return _selectedReportType; }
            set
            {
                _selectedReportType = value;
                if (_selectedReportType == AppResources.PlayList)
                {
                    GamesEnabled = true;
                    TeamsEnabled = false;
                    PlayersEnabled = false;
                    StartDateEnabled = false;
                    EndDateEnabled = false;
                }
                else if (_selectedReportType == AppResources.GameStats)
                {
                    GamesEnabled = true;
                    TeamsEnabled = false;
                    PlayersEnabled = false;
                    StartDateEnabled = false;
                    EndDateEnabled = false;
                }
                else if (_selectedReportType == AppResources.TeamStats)
                {
                    GamesEnabled = false;
                    TeamsEnabled = true;
                    PlayersEnabled = false;
                    StartDateEnabled = true;
                    EndDateEnabled = true;
                }
                else //Player Stats
                {
                    GamesEnabled = false;
                    TeamsEnabled = false;
                    PlayersEnabled = true;
                    StartDateEnabled = true;
                    EndDateEnabled = true;
                }

                SetDefaultValues(_selectedReportType);
                NotifyPropertyChanged("SelectedReportType");
            }
        }

        private GameModel _selectedGame;
        public GameModel SelectedGame
        {
            get { return _selectedGame; }
            set
            {
                _selectedGame = value;
                NotifyPropertyChanged("SelectedGame");
            }
        }

        private TeamModel _selectedTeam;
        public TeamModel SelectedTeam
        {
            get { return _selectedTeam; }
            set
            {
                _selectedTeam = value;
                GameModel game = DAL.Instance().GetTeamsFirstGame(_selectedTeam.Team.TeamID);
                StartDate = game.Game.GameDate;
                NotifyPropertyChanged("SelectedTeam");
            }
        }

        private PlayerModel _selectedPlayer;
        public PlayerModel SelectedPlayer
        {
            get { return _selectedPlayer; }
            set
            {
                _selectedPlayer = value;
                GameModel game = DAL.Instance().GetPlayersFirstGame(_selectedPlayer.Player.PlayerID);
                StartDate = game.Game.GameDate;
                NotifyPropertyChanged("SelectedPlayer");
            }
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; NotifyPropertyChanged("StartDate"); }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; NotifyPropertyChanged("EndDate"); }
        }

        private bool _gamesEnabled;
        public bool GamesEnabled
        {
            get { return _gamesEnabled; }
            set { _gamesEnabled = value; NotifyPropertyChanged("GamesEnabled"); }
        }

        private bool _teamsEnabled;
        public bool TeamsEnabled
        {
            get { return _teamsEnabled; }
            set { _teamsEnabled = value; NotifyPropertyChanged("TeamsEnabled"); }
        }

        private bool _playersEnabled;
        public bool PlayersEnabled
        {
            get { return _playersEnabled; }
            set { _playersEnabled = value; NotifyPropertyChanged("PlayersEnabled"); }
        }

        private bool _startDateEnabled;
        public bool StartDateEnabled
        {
            get { return _startDateEnabled; }
            set { _startDateEnabled = value; NotifyPropertyChanged("StartDateEnabled"); }
        }

        private bool _endDateEnabled;
        public bool EndDateEnabled
        {
            get { return _endDateEnabled; }
            set { _endDateEnabled = value; NotifyPropertyChanged("EndDateEnabled"); }
        }

        #endregion "Properties"

        #region Commands

        private ICommand _generateReportCommand;
        public ICommand GenerateReportCommand
        {
            get
            {
                if (_generateReportCommand == null)
                {
                    _generateReportCommand = new DelegateCommand(param => this.GenerateReport(), param => true);
                }

                return _generateReportCommand;
            }
        }

        #endregion "Commands"

        #region "Methods"

        private void PopulateReportTypes()
        {
            ReportsList = new ObservableCollection<string>();
            ReportsList.Add(AppResources.PlayList);
            ReportsList.Add(AppResources.GameStats);
            ReportsList.Add(AppResources.TeamStats);
            ReportsList.Add(AppResources.PlayerStats);
        }

        private void PopulateGamesList(string displayDeletedGames)
        {
            GamesList = DAL.Instance().GetGames(displayDeletedGames);
        }

        private void PopulateTeamsList(string displayDeletedTeams)
        {
            TeamsList = DAL.Instance().GetTeams(displayDeletedTeams,"N");
        }

        private void PopulatePlayers(string displayDeletePlayers)
        {
            PlayersList = DAL.Instance().GetAllPlayers(displayDeletePlayers, "");
        }

        private void GenerateReport()
        {
            if (_selectedReportType == AppResources.PlayList)
            {

            }
            else if (_selectedReportType == AppResources.GameStats)
            {
                //If game selected is in progress, then first need to  =>StatCalculationsModule.CalculatePlayerMinutes(game.Game.GameID);
            }
            else if (_selectedReportType == AppResources.TeamStats)
            {

            }
            else //Player Stats
            {

            }
        }

        private void SetDefaultValues(string reportType)
        {
            if (reportType == AppResources.GameStats)
            {
                foreach (var item in GamesList)
                {
                    SelectedGame = item;
                    break;
                }
            }
            else if (reportType == AppResources.TeamStats)
            {
                foreach (var item in TeamsList)
                {
                    SelectedTeam = item;
                    break;
                }
            }
            else if (reportType == AppResources.PlayerStats)
            {
                foreach (var item in PlayersList)
                {
                    SelectedPlayer = item;
                    break;
                }
            }

        }

        #endregion "Methods"
    }
}
