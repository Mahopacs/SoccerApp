using Coding4Fun.Toolkit.Controls;
using ScoreSoccer8.Classes;
using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.Resources;
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
    public class PlayerListViewModel : Notification
    {
        private string _messageBoxTitle = AppResources.PlayerList;

        public PlayerListViewModel()
        {
            SearchCharacter = string.Empty;
        }

        #region "Properties"

        private ObservableCollection<PlayerModel> _playersList;
        public ObservableCollection<PlayerModel> PlayersList
        {
            get { return _playersList; }
            set { _playersList = value; NotifyPropertyChanged("PlayersList"); }
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

        private string _teamName;
        public string TeamName
        {
            get { return _teamName; }
            set { _teamName = value; NotifyPropertyChanged("TeamName"); }
        }

        private string _searchCharacter;
        public string SearchCharacter
        {
            get { return _searchCharacter; }
            set { _searchCharacter = value; NotifyPropertyChanged("SearchCharacter"); }
        }

        private bool _displayDeletedPlayers;
        public bool DisplayDeletedPlayers
        {
            get { return _displayDeletedPlayers; }
            set
            {
                _displayDeletedPlayers = value;
                NotifyPropertyChanged("DisplayDeletedPlayers");
                DisplayDeletedPlayersClicked();
            }
        }

        private Visibility _isAddCaptionVisible;
        public Visibility IsAddCaptionVisible
        {
            get { return _isAddCaptionVisible; }
            set { _isAddCaptionVisible = value; NotifyPropertyChanged("IsAddCaptionVisible"); }
        }

        #endregion "Properties"

        #region "Commands"

        private ICommand _addPlayerClickCommand;
        public ICommand AddPlayerClickCommand
        {
            get
            {
                if (_addPlayerClickCommand == null)
                {
                    _addPlayerClickCommand = new DelegateCommand(param => this.GoToPlayerDetailsScreen(), param => true);
                }

                return _addPlayerClickCommand;
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

        #region "Method"

        private void GoHelpScreen()
        {
            if (TeamID == 0)
            {
                (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Help_Buttons.xaml?screenId=" + Enums.Screen.Players, UriKind.Relative));
            }
            else
            {
                (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Help_Buttons.xaml?screenId=" + Enums.Screen.PlayersRosters, UriKind.Relative));
            }
        }

        public void Initialize(int gameID, int teamID, string teamName)
        {
            GameID = gameID;
            TeamID = teamID;
            TeamName = teamName;

            if (_displayDeletedPlayers == true)
            {
                PopulatePlayersList("Y", SearchCharacter);
            }
            else
            {
                PopulatePlayersList("N", SearchCharacter);
            }
        }

        public void ReDisplaySorted(int gameID, int teamID, string teamName, string searchCharacter)
        {
            TeamID = teamID;
            TeamName = teamName;
            SearchCharacter = searchCharacter;

            if (_displayDeletedPlayers == true)
            {
                PopulatePlayersList("Y", SearchCharacter);
            }
            else
            {
                PopulatePlayersList("N", SearchCharacter);
            }
        }

        void item_PlayerDeleted(object sender, EventArgs e)
        {
            if (_displayDeletedPlayers == true)
            {
                PopulatePlayersList("Y", SearchCharacter);
            }
            else
            {
                PopulatePlayersList("N", SearchCharacter);
            }
        }

        //TJY TO DO, Any players in RosterList = DAL.Instance().GetTeamRoster(TeamID) we do not want to include from PlayersList = DAL.Instance().GetAllPlayers below
        private void PopulatePlayersList(string displayDeletedPlayers, string searchCharacter)
        {
            ObservableCollection<TeamRosterModel> teamRosterList = BaseTableDataAccess.Instance().GetTeamRoster(TeamID);
            ObservableCollection<PlayerModel> globalPlayerList = new ObservableCollection<PlayerModel>(DAL.Instance().GetAllPlayers(displayDeletedPlayers, searchCharacter).Where(x => x.Player.SampleData.ToUpper().Equals("N")));
            PlayersList = new ObservableCollection<PlayerModel>();

            foreach (var globalplayerEntry in globalPlayerList)
            {
                if (Common.Instance().IsPlayerInList(teamRosterList, globalplayerEntry.Player.PlayerID) == false)
                {
                    globalplayerEntry.PlayerDeleted += item_PlayerDeleted;
                    globalplayerEntry.PlayerAddedToRoster += item_PlayerAddedToRoster;

                    PlayersList.Add(globalplayerEntry);
                }
            }

            //If this is simply the displaying of the Players screen (i.e. not adding a player to a team, then no need to show the add button)
            if (TeamID == 0)
            {
                PlayersList.ToList().ForEach(x => x.IsAddButtonVisible = Visibility.Collapsed);
                IsAddCaptionVisible = Visibility.Collapsed;
            }
            else
            {
                PlayersList.ToList().ForEach(x => x.IsAddButtonVisible = Visibility.Visible);
                IsAddCaptionVisible = Visibility.Visible;
            }
        }

        void item_PlayerAddedToRoster(object sender, EventArgs e)
        {
            PlayerModel player = (PlayerModel)sender;
            TeamRoster teamRoster = new TeamRoster();

            teamRoster.TeamID = TeamID;
            teamRoster.PlayerID = player.Player.PlayerID;
            teamRoster.UniformNumber = "0";
            teamRoster.Active = "Y";
            BaseTableDataAccess.Instance().UpsertTeamRoster(teamRoster);

            Common.Instance().SetTeamRosterPromptForJersey(TeamID, TeamName, player.Player.PlayerID);

            if (GameID != 0)
            {
                if (App.gPromptForJersey == true)
                {
                    InputPrompt input = new InputPrompt();


                    InputScope scope = new InputScope();
                    InputScopeName name = new InputScopeName();

                    name.NameValue = InputScopeNameValue.Number;
                    scope.Names.Add(name);

                    input.Completed += input_Completed;
                    input.Title = AppResources.JerseyNumber;
                    input.Message = AppResources.EnterPlayersJerseyNumber;
                    input.InputScope = scope;
                    input.Show();
                }
                App.gPromptForJersey = false;
            }
            else  //Player List was called from Rosters screen so go back to that screen to get jersey number
            {
                (Application.Current.RootVisual as Frame).GoBack();
            }
        }


        void input_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            int number;
            bool parsed = int.TryParse(e.Result, out number);

            if (e.Result.Length > 3)
            {
                MessageBox.Show(AppResources.JerseyMustBeLessThan3Characters, _messageBoxTitle, MessageBoxButton.OK);
            }
            else if (parsed == false)
            {
                MessageBox.Show(AppResources.JerseyMustBeANumber, _messageBoxTitle, MessageBoxButton.OK);
            }
            else
            {
                TeamRoster teamRoster = new TeamRoster();

                teamRoster.TeamID = App.gPromptForJerseyTeamID;
                teamRoster.PlayerID = App.gPromptForJerseyPlayerID;
                teamRoster.UniformNumber = e.Result.ToString();
                teamRoster.Active = "Y";
                BaseTableDataAccess.Instance().UpsertTeamRoster(teamRoster);

                //Game Manager is calling AddPlayerToRoster via the + button, so also need to save this player to event roster/isonfield = false and return
                AddPlayerToEventRoster();
                (Application.Current.RootVisual as Frame).GoBack();
            }
        }

        private void AddPlayerToEventRoster()
        {
            EventRoster eventRosterEntry = new EventRoster();
            EventRoster checkForExists = new EventRoster();
            try
            {
                checkForExists = BaseTableDataAccess.Instance().GetEventRosterByGameTeamPlayer(GameID, App.gPromptForJerseyTeamID, App.gPromptForJerseyPlayerID);

                //Should never get to this point, but just in case, we do not want to insert a player on the roster more than once
                if (checkForExists == null)
                {
                    eventRosterEntry.GameID = GameID;
                    eventRosterEntry.TeamID = App.gPromptForJerseyTeamID;
                    eventRosterEntry.PlayerID = App.gPromptForJerseyPlayerID;
                    eventRosterEntry.Starter = "N";
                    DAL.Instance().UpdatePlayersGameStartedStat(GameID, App.gPromptForJerseyTeamID, App.gPromptForJerseyPlayerID, false);
                    eventRosterEntry.IsPlayerOnField = "N";
                    eventRosterEntry.GMPlayerPositionID = "";

                    BaseTableDataAccess.Instance().InsertEventRoster(eventRosterEntry);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void GoToPlayerDetailsScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/PlayerDetails.xaml?gameID=0&teamID=0&playerID=0", UriKind.Relative));
        }

        private void DisplayDeletedPlayersClicked()
        {
            if (_displayDeletedPlayers == true)
            {
                PopulatePlayersList("Y", SearchCharacter);
            }
            else
            {
                PopulatePlayersList("N", SearchCharacter);
            }
        }

        #endregion "Method"
    }
}
