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
    public class GameModel : Notification
    {
        public event EventHandler GameDeleted;
        private string scorePicTemplate = "/Assets/flipNumbers/black/{0}.png";
        public GameModel()
        {           
            Game = new Game();
            AwayTeam = new Team();
            HomeTeam = new Team();
            GameMinutes = Game.CurrentClock;

        }

        #region "Events"

        private string getScore1(int score)
        {
            return getScore(score, 0, 1);
        }

        private string getScore2(int score)
        {
            return getScore(score, 1, 1);
        }

        private string getScore(int score, int start, int stop)
        {
            string toReturn = string.Format(scorePicTemplate, 0);

            string temp = score.ToString().PadLeft(2, '0');

            int j;
            bool result = Int32.TryParse(temp.Substring(start, stop), out j);

            if (result)
            {
                toReturn = string.Format(scorePicTemplate, j);
            }

            return toReturn;
        }

        protected virtual void OnGameDeleted(EventArgs e)
        {
            EventHandler handler = GameDeleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion "Events"

        #region "Properties"

        private string _homeScore1_picPath;
        public string HomeScore1_picPath
        {
            get 
            {
                return getScore1(Game.HomeTeamScore); ; 
            }
            set { _homeScore1_picPath = value; NotifyPropertyChanged("HomeScore1_picPath"); }
        }

        private string _homeScore2_picPath;
        public string HomeScore2_picPath
        {
            get
            {
                return getScore2(Game.HomeTeamScore); ;
            }
            set { _homeScore2_picPath = value; NotifyPropertyChanged("HomeScore2_picPath"); }
        }

        private string _awayScore1_picPath;
        public string AwayScore1_picPath
        {
            get
            {
                return getScore1(Game.AwayTeamScore); ;
            }
            set { _awayScore1_picPath = value; NotifyPropertyChanged("AwayScore1_picPath"); }
        }

        private string _awayScore2_picPath;
        public string AwayScore2_picPath
        {
            get
            {
                return getScore2(Game.AwayTeamScore); ;
            }
            set { _awayScore2_picPath = value; NotifyPropertyChanged("AwayScore2_picPath"); }
        }

        private Game _game;
        public Game Game
        {
            get { return _game; }
            set { _game = value; NotifyPropertyChanged("Game"); }
        }

        private String _gameMinutes;
        public String GameMinutes
        {
            get { return _gameMinutes; }
            set { _gameMinutes = value; NotifyPropertyChanged("GameMinutes"); }
        }

        private Team _awayteam;
        public Team AwayTeam
        {
            get { return _awayteam; }
            set { _awayteam = value; NotifyPropertyChanged("AwayTeam"); }
        }

        private Team _hometeam;
        public Team HomeTeam
        {
            get { return _hometeam; }
            set { _hometeam = value; NotifyPropertyChanged("HomeTeam"); }
        }

        private string _gameTitle;
        public string GameTitle
        {
            get { return _gameTitle; }
            set { _gameTitle = value; NotifyPropertyChanged("GameTitle"); }
        }

        private string _gameTitleWithDate;
        public string GameTitleWithDate
        {
            get { return _gameTitleWithDate; }
            set { _gameTitleWithDate = value; NotifyPropertyChanged("GameTitleWithDate"); }
        }
   
        #endregion "Properties"

        #region Commands

        private ICommand _goToGameDetailsCommand;
        public ICommand GoToGameDetailsCommand
        {
            get
            {
                if (_goToGameDetailsCommand == null)
                {
                    _goToGameDetailsCommand = new DelegateCommand(param => this.GoToGameDetailsScreen(), param => true);
                }

                return _goToGameDetailsCommand;
            }
        }

        private ICommand _goToDeleteGamesCommand;
        public ICommand GoToDeleteGamesCommand
        {
            get
            {
                if (_goToDeleteGamesCommand == null)
                {
                    _goToDeleteGamesCommand = new DelegateCommand(param => this.GoToDeleteGame(), param => true);
                }

                return _goToDeleteGamesCommand;
            }
        }

        private ICommand _goToGameManagerCommand;
        public ICommand GoToGameManagerCommand
        {
            get
            {
                if (_goToGameManagerCommand == null)
                {
                    _goToGameManagerCommand = new DelegateCommand(param => this.GoToGameManager(), param => true);
                }

                return _goToGameManagerCommand;
            }
        }

        private ICommand _goToStatsScreenCommand;
        public ICommand GoToStatsScreenCommand
        {
            get
            {
                if (_goToStatsScreenCommand == null)
                {
                    _goToStatsScreenCommand = new DelegateCommand(param => this.GoToBoxScore(), param => true);
                }

                return _goToStatsScreenCommand;
            }
        }

        #endregion Commands

        #region "Methods"

        private void GoToGameDetailsScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/GameDetails.xaml?parameter=" + Game.GameID, UriKind.Relative));

        }

        private void GoToDeleteGame()
        {
            MessageBoxResult result = MessageBox.Show(AppResources.Delete + " '" + GameTitle + "' " + AppResources.Game + "?", AppResources.DeleteGame, MessageBoxButton.OKCancel);
             if (result == MessageBoxResult.OK)
             {
                 this.Game.Visible = "N";
                 DAL.Instance().UpsertGame(this.Game);            
                 OnGameDeleted(EventArgs.Empty);
             }
        }

        private void GoToBoxScore()
        {
            //(Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/BoxScore.xaml?gameid=" + Game.GameID, UriKind.Relative));
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/PickStatisticsScreen.xaml?gameid=" + Game.GameID, UriKind.Relative));
        }

        public void GoToGameManager()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/GameManagerView.xaml?gameid=" + Game.GameID, UriKind.Relative));
            // (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/LoadingPopup.xaml?gameid=" + Game.GameID, UriKind.Relative));
        }


        #endregion "Methods"
    }
}
