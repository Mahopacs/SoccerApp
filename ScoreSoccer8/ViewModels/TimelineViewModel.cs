using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScoreSoccer8.Classes;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using ScoreSoccer8.Utilities;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.DbClasses;
using System.Diagnostics;
using ScoreSoccer8;
using ScoreSoccer8.Resources;

namespace ScoreSoccer8.ViewModels
{
    public class TimelineViewModel : Notification
    {
        public int _gameId;
        private int _fontSize = 14;
        private int _rowSpam = 2;

        private string _playerNameTemplate = "{0} {1}";
        private string _scoreTextTemplate = "{0} - {1} {2}";
        
        private string scorePicTemplate = "/Assets/flipNumbers/black/{0}.png";
        
        public TimelineViewModel() { }

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

            LoadTimeline();
        }


        private void LoadTimeline()
        {

            List<PlayModel> playCollection = DAL.Instance().GetPlaysForGame(_gameId, "ASC").ToList();

            TimelineList = new List<Timeline>();
            List<Timeline> tempTimelineLIst = new List<Timeline>();
            Timeline timeline = new Timeline();
            Timeline previewsTimeline = new Timeline();

            if (playCollection.Count < 1)
            {
                timeline = new Timeline();
                timeline.GameMarks = "(" + AppResources.NoEvents + ")";
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
                Player player = new Player();
                Play play = new Play();
                play = item.Play;
              
                if (Common.Instance().IsThisAGCoalScoredPlay(item.Play, true, true))
                {

                    Timeline tl = new Timeline();
                    tl.Time = Common.Instance().GetTimelineTime(play);
                    tl.Period = play.Period;
                    tl.ImageHeight = 30;
                    tl.ImageWidth = 30;
                    tl.FontSize = _fontSize;

                    if (_game.AwayTeam.TeamID == play.TeamID)
                    {
                        player = DAL.Instance().GetPlayer(item.Play.Player1ID);
                        tl.AwayPlayerName = string.Format(_playerNameTemplate, player.FirstName, player.LastName, item.Play.Player1ID);
                        tl.AwayPlayerSecondName = string.Format(_scoreTextTemplate, play.AwayScore, play.HomeScore, Game.AwayTeam.TeamName);
                        tl.FontSize = _fontSize;
                        tl.AwayImagePath = Common.Instance().IsThisShootoutGoal(play);
                    }
                    else
                    {
                        player = DAL.Instance().GetPlayer(item.Play.Player1ID);
                        tl.HomePlayerName = string.Format(_playerNameTemplate, player.FirstName, player.LastName, item.Play.Player1ID);
                        tl.HomePlayerSecondName = string.Format(_scoreTextTemplate, play.AwayScore, play.HomeScore, Game.HomeTeam.TeamName);
                        tl.FontSize = _fontSize;
                        tl.HomeImagePath = Common.Instance().IsThisShootoutGoal(play);
                    }

                    tempTimelineLIst.Add(tl);
                }

                if (item.Play.StatCategoryID == 17) //Yellow Card
                {

                    Timeline tl = new Timeline();
                    tl.Time = Common.Instance().GetTimelineTime(play);
                    tl.Period = play.Period;
                    tl.RowSpam = _rowSpam;
                    tl.FontSize = _fontSize;

                    if (_game.AwayTeam.TeamID == play.TeamID)
                    {
                        player = DAL.Instance().GetPlayer(item.Play.Player1ID);
                        tl.AwayPlayerName = string.Format(_playerNameTemplate, player.FirstName, player.LastName, item.Play.Player1ID);
                        tl.AwayImagePath = "/Assets/yellowCard.png";

                        if (item.Play.StatDescriptionID == 24) //Second Yellow Card
                        {
                            tl.AwaySecondYellow = "Visible";
                        }

                    }
                    else
                    {
                        player = DAL.Instance().GetPlayer(item.Play.Player1ID);
                        tl.HomePlayerName = string.Format(_playerNameTemplate, player.FirstName, player.LastName, item.Play.Player1ID);
                        tl.HomeImagePath = "/Assets/yellowCard.png";

                        if (item.Play.StatDescriptionID == 24)   //Second Yellow Card
                        {
                            tl.HomeSecondYellow = "Visible";
                        }
                    }

                    tempTimelineLIst.Add(tl);
                }

                if (item.Play.StatCategoryID == 18)   //Red Card
                {

                    Timeline tl = new Timeline();
                    tl.Time = Common.Instance().GetTimelineTime(play);
                    tl.Period = play.Period;
                    tl.RowSpam = _rowSpam;
                    tl.FontSize = _fontSize;

                    if (_game.AwayTeam.TeamID == play.TeamID)
                    {
                        player = DAL.Instance().GetPlayer(item.Play.Player1ID);
                        tl.AwayPlayerName = string.Format(_playerNameTemplate, player.FirstName, player.LastName, item.Play.Player1ID);
                        tl.AwayImagePath = "/Assets/redCard.png";
                    }
                    else
                    {
                        player = DAL.Instance().GetPlayer(item.Play.Player1ID);
                        tl.HomePlayerName = string.Format(_playerNameTemplate, player.FirstName, player.LastName, item.Play.Player1ID);
                        tl.HomeImagePath = "/Assets/redCard.png";
                    }
                    tempTimelineLIst.Add(tl);
                }


                if (item.Play.StatCategoryID == 22)   //Substitution
                {

                    Timeline tl = new Timeline();
                    tl.Time = Common.Instance().GetTimelineTime(play);
                    tl.Period = play.Period;
                    tl.ImageHeight = 30;
                    //tl.ImageWidth = 20;
                    tl.FontSize = _fontSize;

                    Player secondPlayer = DAL.Instance().GetPlayer(item.Play.Player2ID);


                    if (_game.AwayTeam.TeamID == play.TeamID)
                    {
                        player = DAL.Instance().GetPlayer(item.Play.Player1ID);
                        tl.AwayPlayerName = string.Format(_playerNameTemplate, player.FirstName, player.LastName, item.Play.Player1ID);
                        tl.AwayPlayerSecondName = string.Format(_playerNameTemplate, secondPlayer.FirstName, secondPlayer.LastName, secondPlayer.PlayerID);
                        tl.AwayImagePath = "/Assets/sub.png";
                    }
                    else
                    {
                        player = DAL.Instance().GetPlayer(item.Play.Player1ID);
                        tl.HomePlayerName = string.Format(_playerNameTemplate, player.FirstName, player.LastName, item.Play.Player1ID);
                        tl.HomePlayerSecondName = string.Format(_playerNameTemplate, secondPlayer.FirstName, secondPlayer.LastName, secondPlayer.PlayerID);
                        tl.HomeImagePath = "/Assets/sub.png";
                    }

                    tempTimelineLIst.Add(tl);
                }


            }

            timeline = new Timeline();
            timeline.GameMarks = AppResources.Kickoff;
            timeline.ShowGameMarks = "Visible";
            TimelineList.Add(timeline);

            if (numberOfPeriods == 2)
            {
                if (tempTimelineLIst.Where(x => x.Period == 1).ToList().Count < 1)
                {
                    timeline = new Timeline();
                    timeline.GameMarks = "(" + AppResources.NoEvents + ")";
                    timeline.ShowGameMarks = "Visible";
                    TimelineList.Add(timeline);
                }
                else
                {
                    TimelineList.AddRange(tempTimelineLIst.Where(x => x.Period == 1).ToList());
                }

                timeline = new Timeline();
                timeline.GameMarks = AppResources.HalfTime;
                timeline.ShowGameMarks = "Visible";
                TimelineList.Add(timeline);

                if (tempTimelineLIst.Where(x => x.Period == 2).ToList().Count < 1)
                {
                    timeline = new Timeline();
                    timeline.GameMarks = "(" + AppResources.NoEvents + ")";
                    timeline.ShowGameMarks = "Visible";
                    TimelineList.Add(timeline);
                }
                else
                {
                    TimelineList.AddRange(tempTimelineLIst.Where(x => x.Period == 2).ToList());
                }

                timeline = new Timeline();
                timeline.GameMarks =AppResources.FullTime;
                timeline.ShowGameMarks = "Visible";
                TimelineList.Add(timeline);


                if (overtime > 1)
                {
                    if (tempTimelineLIst.Where(x => x.Period == 3).ToList().Count < 1)
                    {
                        timeline = new Timeline();
                        timeline.GameMarks = "(" + AppResources.NoEvents + ")";
                        timeline.ShowGameMarks = "Visible";
                        TimelineList.Add(timeline);
                    }
                    else
                    {
                        TimelineList.AddRange(tempTimelineLIst.Where(x => x.Period == 3).ToList());
                    }

                    timeline = new Timeline();
                    timeline.GameMarks = AppResources.ExtraTimeHalfTime;
                    timeline.ShowGameMarks = "Visible";
                    TimelineList.Add(timeline);

                    if (tempTimelineLIst.Where(x => x.Period == 4).ToList().Count < 1)
                    {
                        timeline = new Timeline();
                        timeline.GameMarks = "(" + AppResources.NoEvents + ")";
                        timeline.ShowGameMarks = "Visible";
                        TimelineList.Add(timeline);
                    }
                    else
                    {
                        TimelineList.AddRange(tempTimelineLIst.Where(x => x.Period == 4).ToList());
                    }

                    timeline = new Timeline();
                    timeline.GameMarks = AppResources.ExtraTimeFullTime;
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
                        timeline.GameMarks = "(" + AppResources.NoEvents + ")";
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
                        timeline.GameMarks = AppResources.HalfTime;
                        timeline.ShowGameMarks = "Visible";
                        TimelineList.Add(timeline);
                    }
                    else
                    {
                        if (numberOfPeriods == i)
                        {
                            timeline = new Timeline();
                            timeline.GameMarks = AppResources.FullTime;
                            timeline.ShowGameMarks = "Visible";
                            TimelineList.Add(timeline);
                        }
                        else
                        {
                            timeline = new Timeline();
                            timeline.GameMarks = AppResources.EndPeriod + i;
                            timeline.ShowGameMarks = "Visible";
                            TimelineList.Add(timeline);
                        }
                    }

                    if (overtime > 1)
                    {
                        if (overtime / 2 == i)
                        {
                            timeline = new Timeline();
                            timeline.GameMarks = AppResources.ExtraTimeHalfTime;
                            timeline.ShowGameMarks = "Visible";
                            TimelineList.Add(timeline);
                        }

                        if (overtime == i)
                        {
                            timeline = new Timeline();
                            timeline.GameMarks = AppResources.ExtraTimeFullTime;
                            timeline.ShowGameMarks = "Visible";
                            TimelineList.Add(timeline);
                        }
                    }


                }

            }


            TimelineList.Reverse();

            tempTimelineLIst.Add(timeline);

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

    }
}
