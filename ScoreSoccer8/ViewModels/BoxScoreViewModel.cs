using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScoreSoccer8.DataAccess;
using ScoreSoccer8.Classes;
using ScoreSoccer8.Utilities;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.DataObjects.DbClasses;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace ScoreSoccer8.ViewModels
{
    public class BoxScoreViewModel : Notification
    {
        public int _gameId;
        private int fontSize = 14;
        private int rowSpam = 2;

        // private string playerName = "{0} {1} (#{2})";
        private string playerNameTemplate = "{0} {1}";
        private string scoreTextTemplate = "{0} - {1} {2}";
        private string gameTitleTemplate = "{0} {1} : {2} {3}";
        private string timeTemplate = "{0}";

        public void Initialize(int gameId)
        {
            _gameId = gameId;
            _game = DAL.Instance().GetGame(_gameId);

            GameDate = _game.Game.GameDate.ToString("d");

            AwayTeamName = _game.AwayTeam.TeamName;
            HomeTeamName = _game.HomeTeam.TeamName;

            BoxscoreAwayItems = new ObservableCollection<FlatTotalsModel>();
            BoxscoreHomeItems = new ObservableCollection<FlatTotalsModel>();

            GameTitle = string.Format(gameTitleTemplate, Game.AwayTeam.TeamName, _game.Game.AwayTeamScore, _game.Game.HomeTeamScore, Game.HomeTeam.TeamName);
            AwayBoxscore = Game.AwayTeam.TeamShortName + " Boxscore";
            HomeBoxscore = Game.HomeTeam.TeamShortName + " Boxscore";

            AwayTeamShortName = Game.AwayTeam.TeamShortName;
            HomeTeamShortName = Game.HomeTeam.TeamShortName;

            AwayTeamJerseyPath = BaseTableDataAccess.Instance().GetJerseyByJerseyID(Game.AwayTeam.JerseyID).ImagePath;
            HomeTeamJerseyPath = BaseTableDataAccess.Instance().GetJerseyByJerseyID(Game.HomeTeam.JerseyID).ImagePath;

            ObservableCollection<StatCategoryModel> allStats = DAL.Instance().GetVisibleStats(false, false);

            PlayByPlayList = DAL.Instance().GetPlaysForGame(_gameId, "DESC");
            PlayByPlayList.Reverse();

            //StatCalculationsModule.CalculateALLPlayerMinutes(_gameId);
            //StatCalculationsModule.CalculateALLPlayerPlusMinus(_gameId);
  
            BoxscoreAwayItems = LoadPlayerSats(_game.AwayTeam.TeamID);
            BoxscoreHomeItems = LoadPlayerSats(_game.HomeTeam.TeamID);

            LoadTimeline();
            LoadGameStats(_gameId);
            LoadLegend();
        }

        private void LoadLegend()
        {
            LegendList = Common.Instance().GetLegendList();
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

        private void LoadTimeline()
        {

            List<PlayModel> playCollection = DAL.Instance().GetPlaysForGameWithPlayerStatObjectFilledIn(_gameId, "ASC").ToList();

            TimelineList = new List<Timeline>();
            List<Timeline> tempTimelineLIst = new List<Timeline>();
            Timeline timeline = new Timeline();
            Timeline previewsTimeline = new Timeline();

            if (playCollection.Count < 1)
            {
                timeline = new Timeline();
                timeline.GameMarks = "(no events)";
                timeline.ShowGameMarks = "Visible";
                TimelineList.Add(timeline);

                return;
            }

            int numberOfPeriods = _game.Game.Periods;
            int halfTime = numberOfPeriods / 2;
            int allPeriods = playCollection.Where(x => x.Play != null).ToList().Max(x => x.Play.Period);
            int overtime = 0;

            if (allPeriods > numberOfPeriods)
            {
                overtime = allPeriods - numberOfPeriods;
            }

            foreach (PlayModel item in playCollection)
            {

                Play play = new Play();
                play = item.Play;

                if (Common.Instance().IsThisAGCoalScoredPlay(item.Play, true, false))
                {
                    Timeline tl = new Timeline();
                    tl.Time = Common.Instance().GetTimelineTime(play);
                    tl.Period = play.Period;
                    tl.ImageHeight = 30;
                    tl.ImageWidth = 30;
                    tl.FontSize = fontSize;

                    if (_game.AwayTeam.TeamID == play.TeamID)
                    {
                        tl.AwayPlayerName = string.Format(playerNameTemplate, item.Player1.FirstName, item.Player1.LastName, item.Player1.PlayerID);
                        tl.AwayPlayerSecondName = string.Format(scoreTextTemplate, play.AwayScore, play.HomeScore, Game.AwayTeam.TeamName);
                        tl.FontSize = fontSize;
                        tl.AwayImagePath = Common.Instance().IsThisShootoutGoal(play);
                    }
                    else
                    {
                        tl.HomePlayerName = string.Format(playerNameTemplate, item.Player1.FirstName, item.Player1.LastName, item.Player1.PlayerID);
                        tl.HomePlayerSecondName = string.Format(scoreTextTemplate, play.AwayScore, play.HomeScore, Game.HomeTeam.TeamName);
                        tl.FontSize = fontSize;
                        tl.HomeImagePath = Common.Instance().IsThisShootoutGoal(play);
                    }

                    tempTimelineLIst.Add(tl);
                }

                if (item.StatCategory.StatCategoryName == "Yellow Card")
                {

                    Timeline tl = new Timeline();
                    tl.Time = Common.Instance().GetTimelineTime(play);
                    tl.Period = play.Period;
                    tl.RowSpam = rowSpam;
                    tl.FontSize = fontSize;

                    if (_game.AwayTeam.TeamID == play.TeamID)
                    {
                        tl.AwayPlayerName = string.Format(playerNameTemplate, item.Player1.FirstName, item.Player1.LastName, item.Player1.PlayerID);
                        tl.AwayImagePath = "/Assets/yellowCard.png";

                        if (item.StatDescription.StatDescriptionName == "Second Yellow Card")
                        {
                            tl.AwaySecondYellow = "Visible";
                        }

                    }
                    else
                    {
                        tl.HomePlayerName = string.Format(playerNameTemplate, item.Player1.FirstName, item.Player1.LastName, item.Player1.PlayerID);
                        tl.HomeImagePath = "/Assets/yellowCard.png";

                        if (item.StatDescription.StatDescriptionName == "Second Yellow Card")
                        {
                            tl.HomeSecondYellow = "Visible";
                        }
                    }

                    tempTimelineLIst.Add(tl);
                }

                if (item.StatCategory.StatCategoryName == "Red Card")
                {

                    Timeline tl = new Timeline();
                    tl.Time = Common.Instance().GetTimelineTime(play);
                    tl.Period = play.Period;
                    tl.RowSpam = rowSpam;
                    tl.FontSize = fontSize;

                    if (_game.AwayTeam.TeamID == play.TeamID)
                    {
                        tl.AwayPlayerName = string.Format(playerNameTemplate, item.Player1.FirstName, item.Player1.LastName, item.Player1.PlayerID);
                        tl.AwayImagePath = "/Assets/redCard.png";
                    }
                    else
                    {
                        tl.HomePlayerName = string.Format(playerNameTemplate, item.Player1.FirstName, item.Player1.LastName, item.Player1.PlayerID);
                        tl.HomeImagePath = "/Assets/redCard.png";
                    }
                    tempTimelineLIst.Add(tl);
                }


                if (item.StatCategory.StatCategoryName == "Substitution")
                {

                    Timeline tl = new Timeline();
                    tl.Time = Common.Instance().GetTimelineTime(play);
                    tl.Period = play.Period;
                    tl.ImageHeight = 30;
                    tl.ImageWidth = 20;
                    tl.FontSize = fontSize;

                    Player secondPlayer = DAL.Instance().GetPlayer(item.Play.Player2ID);


                    if (_game.AwayTeam.TeamID == play.TeamID)
                    {
                        tl.AwayPlayerName = string.Format(playerNameTemplate, item.Player1.FirstName, item.Player1.LastName, item.Player1.PlayerID);
                        tl.AwayPlayerSecondName = string.Format(playerNameTemplate, secondPlayer.FirstName, secondPlayer.LastName, secondPlayer.PlayerID);
                        tl.AwayImagePath = "/Assets/sub.png";
                    }
                    else
                    {
                        tl.HomePlayerName = string.Format(playerNameTemplate, item.Player1.FirstName, item.Player1.LastName, item.Player1.PlayerID);
                        tl.HomePlayerSecondName = string.Format(playerNameTemplate, secondPlayer.FirstName, secondPlayer.LastName, secondPlayer.PlayerID);
                        tl.HomeImagePath = "/Assets/sub.png";
                    }

                    tempTimelineLIst.Add(tl);
                }


            }



            // TimelineList.AddRange(tempTimelineLIst.Where(x => x.Period == 1).ToList());

            timeline = new Timeline();
            timeline.GameMarks = "KICKOFF";
            timeline.ShowGameMarks = "Visible";
            TimelineList.Add(timeline);

            if (numberOfPeriods == 2)
            {
                if (tempTimelineLIst.Where(x => x.Period == 1).ToList().Count < 1)
                {
                    timeline = new Timeline();
                    timeline.GameMarks = "(no events)";
                    timeline.ShowGameMarks = "Visible";
                    TimelineList.Add(timeline);
                }
                else
                {
                    TimelineList.AddRange(tempTimelineLIst.Where(x => x.Period == 1).ToList());
                }

                timeline = new Timeline();
                timeline.GameMarks = "HALF-TIME";
                timeline.ShowGameMarks = "Visible";
                TimelineList.Add(timeline);

                if (tempTimelineLIst.Where(x => x.Period == 2).ToList().Count < 1)
                {
                    timeline = new Timeline();
                    timeline.GameMarks = "(no events)";
                    timeline.ShowGameMarks = "Visible";
                    TimelineList.Add(timeline);
                }
                else
                {
                    TimelineList.AddRange(tempTimelineLIst.Where(x => x.Period == 2).ToList());
                }

                timeline = new Timeline();
                timeline.GameMarks = "FULL-TIME";
                timeline.ShowGameMarks = "Visible";
                TimelineList.Add(timeline);


                if (overtime > 1)
                {
                    if (tempTimelineLIst.Where(x => x.Period == 3).ToList().Count < 1)
                    {
                        timeline = new Timeline();
                        timeline.GameMarks = "(no events)";
                        timeline.ShowGameMarks = "Visible";
                        TimelineList.Add(timeline);
                    }
                    else
                    {
                        TimelineList.AddRange(tempTimelineLIst.Where(x => x.Period == 3).ToList());
                    }

                    timeline = new Timeline();
                    timeline.GameMarks = "EXTRA-TIME HALF-TIME";
                    timeline.ShowGameMarks = "Visible";
                    TimelineList.Add(timeline);

                    if (tempTimelineLIst.Where(x => x.Period == 4).ToList().Count < 1)
                    {
                        timeline = new Timeline();
                        timeline.GameMarks = "(no events)";
                        timeline.ShowGameMarks = "Visible";
                        TimelineList.Add(timeline);
                    }
                    else
                    {
                        TimelineList.AddRange(tempTimelineLIst.Where(x => x.Period == 4).ToList());
                    }

                    timeline = new Timeline();
                    timeline.GameMarks = "EXTRA-TIME FULL-TIME";
                    timeline.ShowGameMarks = "Visible";
                    TimelineList.Add(timeline);


                }


            }
            else
            {
                for (int i = 1; i <= numberOfPeriods; i++)
                {
                    if (tempTimelineLIst.Where(x => x.Period == i).ToList().Count < 1)
                    {
                        timeline = new Timeline();
                        timeline.GameMarks = "(no events)";
                        timeline.ShowGameMarks = "Visible";
                        TimelineList.Add(timeline);
                    }
                    else
                    {
                        TimelineList.AddRange(tempTimelineLIst.Where(x => x.Period == i).ToList());
                    }

                    if (numberOfPeriods / 2 == i)
                    {
                        timeline = new Timeline();
                        timeline.GameMarks = "HALF-TIME";
                        timeline.ShowGameMarks = "Visible";
                        TimelineList.Add(timeline);
                    }
                    else
                    {
                        if (numberOfPeriods == i)
                        {
                            timeline = new Timeline();
                            timeline.GameMarks = "FULL-TIME";
                            timeline.ShowGameMarks = "Visible";
                            TimelineList.Add(timeline);
                        }
                        else
                        {
                            timeline = new Timeline();
                            timeline.GameMarks = "End Period " + i;
                            timeline.ShowGameMarks = "Visible";
                            TimelineList.Add(timeline);
                        }
                    }

                    if (overtime > 1)
                    {
                        if (overtime / 2 == i)
                        {
                            timeline = new Timeline();
                            timeline.GameMarks = "EXTRA-TIME HALF-TIME";
                            timeline.ShowGameMarks = "Visible";
                            TimelineList.Add(timeline);
                        }

                        if (overtime == i)
                        {
                            timeline = new Timeline();
                            timeline.GameMarks = "EXTRA-TIME FULL-TIME";
                            timeline.ShowGameMarks = "Visible";
                            TimelineList.Add(timeline);
                        }
                    }


                }

            }



            TimelineList.Reverse();

            tempTimelineLIst.Add(timeline);

        }

        private ObservableCollection<FlatTotalsModel> LoadPlayerSats(int teamId)
        {

            ObservableCollection<FlatTotalsModel> players = DAL.Instance().GetGamesPlayerFlatStats(_gameId, teamId);

            int i = 0;
            foreach (FlatTotalsModel item in players)
            {   
                if (i % 2 != 0)
                {
                    item.BackgroundColor = "White";
                    item.BackgroundOpacity = 0.3;
                }

                i++;
            }

            return players;

        }
        
        private ICommand _showLegendCommand;
        public ICommand ShowLegendCommand
        {
            get
            {
                if (_showLegendCommand == null)
                {
                    _showLegendCommand = new DelegateCommand(param => this.ToShowPopup(), param => true);
                }

                return _showLegendCommand;
            }
        }

        private void ToShowPopup()
        {
            if (ShowPopup == "Visible")
            {
                ShowPopup = "Collapsed";
            }
            else
            {
                ShowPopup = "Visible";
            }
        }

        #region "Properties"


        #region "Data Grid"


        #endregion

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

        private string _showPopup = "Collapsed";
        public string ShowPopup
        {
            get { return _showPopup; }
            set { _showPopup = value; NotifyPropertyChanged("ShowPopup"); }
        }

        private ObservableCollection<Legend> _legendList = new ObservableCollection<Legend>();
        public ObservableCollection<Legend> LegendList
        {
            get { return _legendList; }
            set { _legendList = value; NotifyPropertyChanged("LegendList"); }
        }

        private ObservableCollection<PlayModel> _playByPlayList = new ObservableCollection<PlayModel>();
        public ObservableCollection<PlayModel> PlayByPlayList
        {
            get { return _playByPlayList; }
            set { _playByPlayList = value; NotifyPropertyChanged("PlayByPlayList"); }
        }


        private ObservableCollection<FlatTotalsModel> _boxscoreAwayItems = new ObservableCollection<FlatTotalsModel>();
        public ObservableCollection<FlatTotalsModel> BoxscoreAwayItems
        {
            get { return _boxscoreAwayItems; }
            set { _boxscoreAwayItems = value; NotifyPropertyChanged("BoxscoreAwayItems"); }
        }

        private ObservableCollection<FlatTotalsModel> _boxscoreHomeItems = new ObservableCollection<FlatTotalsModel>();
        public ObservableCollection<FlatTotalsModel> BoxscoreHomeItems
        {
            get { return _boxscoreHomeItems; }
            set { _boxscoreHomeItems = value; NotifyPropertyChanged("BoxscoreHomeItems"); }
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

        private String _gameDate;
        public String GameDate
        {
            get { return _gameDate; }
            set { _gameDate = value; NotifyPropertyChanged("GameDate"); }
        }

        private string _awayBoxscore;
        public string AwayBoxscore
        {
            get { return _awayBoxscore; }
            set { _awayBoxscore = value; NotifyPropertyChanged("AwayBoxscore"); }
        }

        private string _homeBoxscore;
        public string HomeBoxscore
        {
            get { return _homeBoxscore; }
            set { _homeBoxscore = value; NotifyPropertyChanged("HomeBoxscore"); }
        }

        private string _gameTitle;
        public string GameTitle
        {
            get { return _gameTitle; }
            set { _gameTitle = value; NotifyPropertyChanged("GameTitle"); }
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



        #region Game Stats

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

        #endregion

        #endregion
    }
}
