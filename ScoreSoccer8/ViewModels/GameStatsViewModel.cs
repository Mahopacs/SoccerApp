using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScoreSoccer8.Utilities;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.DbClasses;

namespace ScoreSoccer8.ViewModels
{
    public class GameStatsViewModel : Notification
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

            LoadGameStats(_gameId);
        }

        private void LoadGameStats(int gameId)
        {

            FlatTotalsModel awayTotals = DAL.Instance().GetTeamsFlatStatsForGame(gameId, _game.Game.AwayTeamID);
            FlatTotalsModel homeTotals = DAL.Instance().GetTeamsFlatStatsForGame(gameId, _game.Game.HomeTeamID);

            TotalShots_Away = awayTotals.FlatTotals.ShotTotal;
            ShotsOnGoal_Away = awayTotals.FlatTotals.ShotOnGoalTotal;
            Fouls_Away = awayTotals.FlatTotals.FoulCommittedTotal;
            Corners_Away = awayTotals.FlatTotals.CornerKickTotal;
            Saves_Away = awayTotals.FlatTotals.SaveTotal;
            Offsides_Away = awayTotals.FlatTotals.OffsidesTotal;
            YellowCard_Away = awayTotals.FlatTotals.YellowCardTotal;
            RedCard_Away = awayTotals.FlatTotals.RedCardTotal;

            TotalShots_Home = homeTotals.FlatTotals.ShotTotal;
            ShotsOnGoal_Home = homeTotals.FlatTotals.ShotOnGoalTotal;
            Fouls_Home = homeTotals.FlatTotals.FoulCommittedTotal;
            Corners_Home = homeTotals.FlatTotals.CornerKickTotal;
            Saves_Home = homeTotals.FlatTotals.SaveTotal;
            Offsides_Home = homeTotals.FlatTotals.OffsidesTotal;
            YellowCard_Home = homeTotals.FlatTotals.YellowCardTotal;
            RedCard_Home = homeTotals.FlatTotals.RedCardTotal;

            TotalShots_Max = Common.Instance().GetStatusGraphNumber(TotalShots_Away, TotalShots_Home);
            ShotsOnGoal_Max = Common.Instance().GetStatusGraphNumber(ShotsOnGoal_Away, ShotsOnGoal_Home);
            Fouls_Max = Common.Instance().GetStatusGraphNumber(Fouls_Away, Fouls_Home);
            Corners_Max = Common.Instance().GetStatusGraphNumber(Corners_Away, Corners_Home);
            Saves_Max = Common.Instance().GetStatusGraphNumber(Saves_Away, Saves_Home);
            Offsides_Max = Common.Instance().GetStatusGraphNumber(Offsides_Away, Offsides_Home);
            YellowCards_Max = Common.Instance().GetStatusGraphNumber(YellowCard_Away, YellowCard_Home);
            RedCards_Max = Common.Instance().GetStatusGraphNumber(RedCard_Away, RedCard_Home);

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
        
        private GameModel _game;
        public GameModel Game
        {
            get { return _game; }
            set { _game = value; NotifyPropertyChanged("Game"); }
        }

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

        #region Game Stats Properties

        private int _totalShots_Max;
        public int TotalShots_Max
        {
            get { return _totalShots_Max; }
            set { _totalShots_Max = value; NotifyPropertyChanged("TotalShots_Max"); }
        }

        private int _shotsOnGoal_Max;
        public int ShotsOnGoal_Max
        {
            get { return _shotsOnGoal_Max; }
            set { _shotsOnGoal_Max = value; NotifyPropertyChanged("ShotsOnGoal_Max"); }
        }

        private int _fouls_Max;
        public int Fouls_Max
        {
            get { return _fouls_Max; }
            set { _fouls_Max = value; NotifyPropertyChanged("Fouls_Max"); }
        }

        private int _corners_Max;
        public int Corners_Max
        {
            get { return _corners_Max; }
            set { _corners_Max = value; NotifyPropertyChanged("Corners_Max"); }
        }

        private int _saves_Max;
        public int Saves_Max
        {
            get { return _saves_Max; }
            set { _saves_Max = value; NotifyPropertyChanged("Saves_Max"); }
        }

        private int _offsides_Max;
        public int Offsides_Max
        {
            get { return _offsides_Max; }
            set { _offsides_Max = value; NotifyPropertyChanged("Offsides_Max"); }
        }

        private int _yellowCards_Max;
        public int YellowCards_Max
        {
            get { return _yellowCards_Max; }
            set { _yellowCards_Max = value; NotifyPropertyChanged("YellowCards_Max"); }
        }

        private int _redCards_Max;
        public int RedCards_Max
        {
            get { return _redCards_Max; }
            set { _redCards_Max = value; NotifyPropertyChanged("RedCards_Max"); }
        }




        private int _totalShots_Away;
        public int TotalShots_Away
        {
            get { return _totalShots_Away; }
            set { _totalShots_Away = value; NotifyPropertyChanged("TotalShots_Away"); }
        }

        private int _shotsOnGoal_Away;
        public int ShotsOnGoal_Away
        {
            get { return _shotsOnGoal_Away; }
            set { _shotsOnGoal_Away = value; NotifyPropertyChanged("ShotsOnGoal_Away"); }
        }

        private int _fouls_Away;
        public int Fouls_Away
        {
            get { return _fouls_Away; }
            set { _fouls_Away = value; NotifyPropertyChanged("Fouls_Away"); }
        }

        private int _corners_Away;
        public int Corners_Away
        {
            get { return _corners_Away; }
            set { _corners_Away = value; NotifyPropertyChanged("Corners_Away"); }
        }

        private int _saves_Away;
        public int Saves_Away
        {
            get { return _saves_Away; }
            set { _saves_Away = value; NotifyPropertyChanged("Saves_Away"); }
        }

        private int _offsides_Away;
        public int Offsides_Away
        {
            get { return _offsides_Away; }
            set { _offsides_Away = value; NotifyPropertyChanged("Offsides_Away"); }
        }

        private int _yellowCard_Away;
        public int YellowCard_Away
        {
            get { return _yellowCard_Away; }
            set { _yellowCard_Away = value; NotifyPropertyChanged("YellowCard_Away"); }
        }

        private int _redCard_Away;
        public int RedCard_Away
        {
            get { return _redCard_Away; }
            set { _redCard_Away = value; NotifyPropertyChanged("RedCard_Away"); }
        }


        private int _totalShots_Home;
        public int TotalShots_Home
        {
            get { return _totalShots_Home; }
            set { _totalShots_Home = value; NotifyPropertyChanged("TotalShots_Home"); }
        }

        private int _shotsOnGoal_Home;
        public int ShotsOnGoal_Home
        {
            get { return _shotsOnGoal_Home; }
            set { _shotsOnGoal_Home = value; NotifyPropertyChanged("ShotsOnGoal_Home"); }
        }

        private int _fouls_Home;
        public int Fouls_Home
        {
            get { return _fouls_Home; }
            set { _fouls_Home = value; NotifyPropertyChanged("Fouls_Home"); }
        }

        private int _corners_Home;
        public int Corners_Home
        {
            get { return _corners_Home; }
            set { _corners_Home = value; NotifyPropertyChanged("Corners_Home"); }
        }

        private int _saves_Home;
        public int Saves_Home
        {
            get { return _saves_Home; }
            set { _saves_Home = value; NotifyPropertyChanged("Saves_Home"); }
        }

        private int _offsides_Home;
        public int Offsides_Home
        {
            get { return _offsides_Home; }
            set { _offsides_Home = value; NotifyPropertyChanged("Offsides_Home"); }
        }

        private int _yellowCard_Home;
        public int YellowCard_Home
        {
            get { return _yellowCard_Home; }
            set { _yellowCard_Home = value; NotifyPropertyChanged("YellowCard_Home"); }
        }

        private int _redCard_Home;
        public int RedCard_Home
        {
            get { return _redCard_Home; }
            set { _redCard_Home = value; NotifyPropertyChanged("RedCard_Home"); }
        }

        #endregion  Game Stats Properties


    }

}
