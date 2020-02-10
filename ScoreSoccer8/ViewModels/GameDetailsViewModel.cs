using ScoreSoccer8.Classes;
using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.DataObjects.UiClasses;
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

namespace ScoreSoccer8.ViewModels
{
    public class GameDetailsViewModel : Notification
    {
        public GameDetailsViewModel()
        {
            GameDetails = new GameModel();
            TeamsList = new ObservableCollection<TeamModel>();              
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

        #region "Properties"

        private void GoHelpScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Help_Buttons.xaml?screenId=" + Enums.Screen.GameDetails, UriKind.Relative));
        }

        private GameModel _gameDetails;
        public GameModel GameDetails
        {
            get { return _gameDetails; }
            set { _gameDetails = value; NotifyPropertyChanged("GameDetails"); }
        }

        private ObservableCollection<TeamModel> _teamsList;
        public ObservableCollection<TeamModel> TeamsList
        {
            get { return _teamsList; }
            set { _teamsList = value; NotifyPropertyChanged("TeamsList"); }
        }

        private TeamModel _selectedHomeTeam;
        public TeamModel SelectedHomeTeam
        {
            get { return _selectedHomeTeam; }
            set { _selectedHomeTeam = value; NotifyPropertyChanged("SelectedHomeTeam"); }
        }

        private TeamModel _selectedAwayTeam;
        public TeamModel SelectedAwayTeam
        {
            get { return _selectedAwayTeam; }
            set { _selectedAwayTeam = value; NotifyPropertyChanged("SelectedAwayTeam"); }
        }

        private DateTime _gameDate;
        public DateTime GameDate
        {
            get { return _gameDate; }
            set { _gameDate = value; NotifyPropertyChanged("GameDate"); }
        }

        private DateTime _gameTime;
        public DateTime GameTime
        {
            get { return _gameTime; }
            set { _gameTime = value; NotifyPropertyChanged("GameTime"); }
        }

        private int? _periodLength;
        public int? PeriodLength
        {
            get { return _periodLength; }
            set { _periodLength = value; NotifyPropertyChanged("PeriodLength"); }
        }

        private int? _overTimeLength;
        public int? OverTimeLength
        {
            get { return _overTimeLength; }
            set { _overTimeLength = value; NotifyPropertyChanged("OverTimeLength"); }
        }

        private bool _eightPlayersChecked;
        public bool EightPlayersChecked
        {
            get { return _eightPlayersChecked; }
            set
            {
                _eightPlayersChecked = value;
                GameDetails.Game.PlayersPerTeam = 8;
                NotifyPropertyChanged("EightPlayersChecked");
            }
        }

        private bool _elevenPlayersChecked;
        public bool ElevenPlayersChecked
        {
            get { return _elevenPlayersChecked; }
            set
            {
                _elevenPlayersChecked = value;
                GameDetails.Game.PlayersPerTeam = 11;
                NotifyPropertyChanged("ElevenPlayersChecked");
            }
        }

        private bool _twoPeriodsChecked;
        public bool TwoPeriodsChecked
        {
            get { return _twoPeriodsChecked; }
            set
            {
                _twoPeriodsChecked = value;
                GameDetails.Game.Periods = 2;
                NotifyPropertyChanged("TwoPeriodsChecked");
            }
        }

        private bool _fourPeriodsChecked;
        public bool FourPeriodsChecked
        {
            get { return _fourPeriodsChecked; }
            set
            {
                _fourPeriodsChecked = value;
                GameDetails.Game.Periods = 4;
                NotifyPropertyChanged("FourPeriodsChecked");
            }
        }

        private bool _otYes;
        public bool OTYes
        {
            get { return _otYes; }
            set
            {
                _otYes = value;
                GameDetails.Game.HasOverTime = true;
                NotifyPropertyChanged("OTYes");
            }
        }

        private bool _otNo;
        public bool OTNo
        {
            get { return _otNo; }
            set
            {
                _otNo = value;
                GameDetails.Game.HasOverTime = false;
                NotifyPropertyChanged("OTNo");
            }
        }

        private bool _clockUp;
        public bool ClockUp
        {
            get { return _clockUp; }
            set
            {
                _clockUp = value;
                GameDetails.Game.ClockUpOrDown = "UP";
                NotifyPropertyChanged("ClockUp");
            }
        }

        private bool _clockDown;
        public bool ClockDown
        {
            get { return _clockDown; }
            set
            {
                _clockDown = value;
                GameDetails.Game.ClockUpOrDown = "DOWN";
                NotifyPropertyChanged("ClockDown");
            }
        }

        private bool _fieldsEnabled;
        public bool FieldsEnabled
        {
            get { return _fieldsEnabled; }
            set
            {
                _fieldsEnabled = value;
                if (value == true) { DisplayScreenDisabledMessage = Visibility.Collapsed; } else { DisplayScreenDisabledMessage = Visibility.Visible; }
                NotifyPropertyChanged("FieldsEnabled");
            }
        }

        private Visibility _displayScreenDisabledMessage;
        public Visibility DisplayScreenDisabledMessage
        {
            get { return _displayScreenDisabledMessage; }
            set { _displayScreenDisabledMessage = value; NotifyPropertyChanged("DisplayScreenDisabledMessage"); }
        }

        #endregion "Properties"

        #region "Methods"

        public void Initialize(int gameID)
        {
            if (gameID == 0)
            {            
                TeamsList = PopulateTeamsList("Y"); //Include Select Team in drop down
                SetDefaultValuesForAddGame();
            }
            else
            {
                TeamsList = PopulateTeamsList("N");
                GameDetails = DAL.Instance().GetGame(gameID);
                SetSelectedValues();
            }
        }

        private ObservableCollection<TeamModel> PopulateTeamsList(string addSelectTeam)
        {
            ObservableCollection<TeamModel> teamsList = new ObservableCollection<TeamModel>(DAL.Instance().GetTeams("N", addSelectTeam).Where(x => x.Team.SampleData.ToUpper().Equals("N")));

            return teamsList;      
        }

        private void SetDefaultValuesForAddGame()
        {
            FieldsEnabled = true;
            GameDate = DateTime.Now;
            GameTime = DateTime.Now;
            PeriodLength = 30;
            GameDetails.Game.Periods = 2;

            EightPlayersChecked = true;
            TwoPeriodsChecked = true;
            OTNo = true;
            ClockDown = true;

            //set home and away teams
            foreach (var item in TeamsList)
            {
                SelectedAwayTeam = item;
                SelectedHomeTeam = item;
                break;
            }
        }

        private void SetSelectedValues()
        {
            try
            {
                //Set game date and game time
                GameDate = GameDetails.Game.GameDate;
                GameTime = GameDetails.Game.GameTime;
                PeriodLength = GameDetails.Game.PeriodLength;
                OverTimeLength = GameDetails.Game.OverTimeLength;

                //set home and away teams
                foreach (var item in TeamsList)
                {
                    if (GameDetails.Game.AwayTeamID == item.Team.TeamID)
                    {
                        SelectedAwayTeam = item;
                    }
                    if (GameDetails.Game.HomeTeamID == item.Team.TeamID)
                    {
                        SelectedHomeTeam = item;
                    }
                }

                //Set 8 or 11 players
                if (GameDetails.Game.PlayersPerTeam == 8)
                {
                    EightPlayersChecked = true;
                }
                else
                {
                    ElevenPlayersChecked = true;
                }

                //Set 2 or 4 periods
                if (GameDetails.Game.Periods == 2)
                {

                    TwoPeriodsChecked = true;
                }
                else
                {
                    FourPeriodsChecked = true;
                }

                //Has OT Yes or No
                if (GameDetails.Game.HasOverTime == true)
                {
                    OTYes = true;
                }
                else
                {
                    OTNo = true;
                }

                //Clock Up or Down
                if (GameDetails.Game.ClockUpOrDown.ToUpper() == "UP")
                {
                    ClockUp = true;
                }
                else
                {
                    ClockDown = true;
                }

                if (GameDetails.Game.GameStatus == "NOT STARTED")
                {
                    FieldsEnabled = true;
                }
                else
                {
                    FieldsEnabled = false;
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void SaveToDatabase()
        {
            DAL.Instance().UpsertGame(this.GameDetails.Game);
        }

        #endregion "Methods"
    }
}
