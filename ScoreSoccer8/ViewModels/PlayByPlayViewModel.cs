using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ScoreSoccer8.Utilities;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.DbClasses;

namespace ScoreSoccer8.ViewModels
{
    class PlayByPlayViewModel : Notification
    {
        public int _gameId;
        private string scorePicTemplate = "/Assets/flipNumbers/black/{0}.png";

        public void Initialize(int gameId)
        {
            _gameId = gameId;
            _game = DAL.Instance().GetGame(_gameId);

            AwayTeamName = _game.AwayTeam.TeamName;
            HomeTeamName = _game.HomeTeam.TeamName;

            AwayTeamShortName = Game.AwayTeam.TeamShortName;
            HomeTeamShortName = Game.HomeTeam.TeamShortName;

            AwayTeamJerseyPath = BaseTableDataAccess.Instance().GetJerseyByJerseyID(Game.AwayTeam.JerseyID).ImagePath;
            HomeTeamJerseyPath = BaseTableDataAccess.Instance().GetJerseyByJerseyID(Game.HomeTeam.JerseyID).ImagePath;

            GameDate_NoTime = _game.Game.GameDate_NoTime;

            PlayByPlayList = DAL.Instance().GetPlaysForGame(_gameId, "DESC");
            PlayByPlayList.Reverse();

        }

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

        #region Properties

        private string _gameDate_NoTime;
        public string GameDate_NoTime
        {
            get { return _gameDate_NoTime; }
            set { _gameDate_NoTime = value; NotifyPropertyChanged("GameDate_NoTime"); }
        }

        private string _homeScore1_picPath;
        public string HomeScore1_picPath
        {
            get
            {
                return getScore1(Game.Game.HomeTeamScore); ;
            }
            set { _homeScore1_picPath = value; NotifyPropertyChanged("HomeScore1_picPath"); }
        }

        private string _homeScore2_picPath;
        public string HomeScore2_picPath
        {
            get
            {
                return getScore2(Game.Game.HomeTeamScore); ;
            }
            set { _homeScore2_picPath = value; NotifyPropertyChanged("HomeScore2_picPath"); }
        }

        private string _awayScore1_picPath;
        public string AwayScore1_picPath
        {
            get
            {
                return getScore1(Game.Game.AwayTeamScore); ;
            }
            set { _awayScore1_picPath = value; NotifyPropertyChanged("AwayScore1_picPath"); }
        }

        private string _awayScore2_picPath;
        public string AwayScore2_picPath
        {
            get
            {
                return getScore2(Game.Game.AwayTeamScore); ;
            }
            set { _awayScore2_picPath = value; NotifyPropertyChanged("AwayScore2_picPath"); }
        }

        private ObservableCollection<PlayModel> _playByPlayList = new ObservableCollection<PlayModel>();
        public ObservableCollection<PlayModel> PlayByPlayList
        {
            get { return _playByPlayList; }
            set { _playByPlayList = value; NotifyPropertyChanged("PlayByPlayList"); }
        }

        private List<Timeline> _timelineList;
        public List<Timeline> TimelineList
        {
            get { return _timelineList; }
            set { _timelineList = value; NotifyPropertyChanged("TimelineList"); }
        }

        private GameModel _game;
        public GameModel Game
        {
            get { return _game; }
            set { _game = value; NotifyPropertyChanged("Game"); }
        }

        private string _awayTeamName;
        public string AwayTeamName
        {
            get { return _awayTeamName; }
            set { _awayTeamName = value; NotifyPropertyChanged("AwayTeamName"); }
        }

        private string _homeTeamName;
        public string HomeTeamName
        {
            get { return _homeTeamName; }
            set { _homeTeamName = value; NotifyPropertyChanged("HomeTeamName"); }
        }

        private string _awayTeamShortName;
        public string AwayTeamShortName
        {
            get { return _awayTeamShortName; }
            set { _awayTeamShortName = value; NotifyPropertyChanged("AwayTeamShortName"); }
        }

        private string _homeTeamShortName;
        public string HomeTeamShortName
        {
            get { return _homeTeamShortName; }
            set { _homeTeamShortName = value; NotifyPropertyChanged("HomeTeamShortName"); }
        }

        private string _awayTeamJerseyPath;
        public string AwayTeamJerseyPath
        {
            get { return _awayTeamJerseyPath; }
            set { _awayTeamJerseyPath = value; NotifyPropertyChanged("AwayTeamJerseyPath"); }
        }

        private string _homeTeamJerseyPath;
        public string HomeTeamJerseyPath
        {
            get { return _homeTeamJerseyPath; }
            set { _homeTeamJerseyPath = value; NotifyPropertyChanged("HomeTeamJerseyPath"); }
        }

        #endregion Properties
    }
}
