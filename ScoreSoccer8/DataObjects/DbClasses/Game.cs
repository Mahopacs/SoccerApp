using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.DataObjects.DbClasses
{
    public class Game : Notification
    {

        #region "Properties"

        private int _gameID;
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int GameID
        {
            get { return _gameID; }
            set { _gameID = value; NotifyPropertyChanged("GameID"); }
        }

        private int _homeTeamID;
        public int HomeTeamID
        {
            get { return _homeTeamID; }
            set { _homeTeamID = value; NotifyPropertyChanged("HomeTeamID"); }
        }

        private int _awayTeamID;
        public int AwayTeamID
        {
            get { return _awayTeamID; }
            set { _awayTeamID = value; NotifyPropertyChanged("AwayTeamID"); }
        }

        private DateTime _gameDate;
        public DateTime GameDate
        {
            get { return _gameDate; }
            set 
            { 
                _gameDate = value;
                _gameDate_NoTime = _gameDate.ToString("d");
                NotifyPropertyChanged("GameDate"); 
            }
        }

        private string _gameDate_NoTime;
        public string GameDate_NoTime
        {
            get { return _gameDate_NoTime; }
            set { _gameDate_NoTime = value; NotifyPropertyChanged("GameDate_NoTime"); }
        }

        private DateTime _gameTime;
        public DateTime GameTime
        {
            get { return _gameTime; }
            set { _gameTime = value; NotifyPropertyChanged("GameTime"); }
        }

        private int _playersPerTeam;
        public int PlayersPerTeam
        {
            get { return _playersPerTeam; }
            set { _playersPerTeam = value; NotifyPropertyChanged("PlayersPerTeam"); }
        }

        private int _periods;
        public int Periods
        {
            get { return _periods; }
            set { _periods = value; NotifyPropertyChanged("Periods"); }
        }

        private int _periodLength;
        public int PeriodLength
        {
            get { return _periodLength; }
            set { _periodLength = value; NotifyPropertyChanged("PeriodLength"); }
        }

        private bool _hasOverTime;
        public bool HasOverTime
        {
            get { return _hasOverTime; }
            set { _hasOverTime = value; NotifyPropertyChanged("HasOverTime"); }
        }

        private int _overTimeLength;
        public int OverTimeLength
        {
            get { return _overTimeLength; }
            set { _overTimeLength = value; NotifyPropertyChanged("OverTimeLength"); }
        }

        private int _homeFormationID;
        public int HomeFormationID
        {
            get { return _homeFormationID; }
            set { _homeFormationID = value; NotifyPropertyChanged("HomeFormationID"); }
        }

        private int _awayFormationID;
        public int AwayFormationID
        {
            get { return _awayFormationID; }
            set { _awayFormationID = value; NotifyPropertyChanged("AwayFormationID"); }
        }

        private string _clockUpOrDown;
        public string ClockUpOrDown
        {
            get { return _clockUpOrDown; }
            set { _clockUpOrDown = value; NotifyPropertyChanged("ClockUpOrDown"); }
        }

        private string _gameStatus;
        public string GameStatus
        {
            get { return _gameStatus; }
            set { _gameStatus = value; NotifyPropertyChanged("GameStatus"); }
        }

        private int _currentPeriod;
        public int CurrentPeriod
        {
            get { return _currentPeriod; }
            set { _currentPeriod = value; NotifyPropertyChanged("CurrentPeriod"); }
        }

        private string _currentClock;
        public string CurrentClock
        {
            get { return _currentClock; }
            set { _currentClock = value; NotifyPropertyChanged("CurrentClock"); }
        }

        private int _currentElapsedTimeInSeconds;
        public int CurrentElapsedTimeInSeconds
        {
            get { return _currentElapsedTimeInSeconds; }
            set { _currentElapsedTimeInSeconds = value; NotifyPropertyChanged("CurrentElapsedTimeInSeconds"); }
        }

        private int _homeTeamScore;
        public int HomeTeamScore
        {
            get { return _homeTeamScore; }
            set { _homeTeamScore = value; NotifyPropertyChanged("HomeTeamScore"); }
        }

        private int _homeTeamShootOutGoals;
        public int HomeTeamShootOutGoals
        {
            get { return _homeTeamShootOutGoals; }
            set { _homeTeamShootOutGoals = value; NotifyPropertyChanged("HomeTeamShootOutGoals"); }
        }

        private int _homeTeamShootOutGoalOpp;
        public int HomeTeamShootOutGoalOpp
        {
            get { return _homeTeamShootOutGoalOpp; }
            set { _homeTeamShootOutGoalOpp = value; NotifyPropertyChanged("HomeTeamShootOutGoalOpp"); }
        }

        private int _awayTeamScore;
        public int AwayTeamScore
        {
            get { return _awayTeamScore; }
            set { _awayTeamScore = value; NotifyPropertyChanged("AwayTeamScore"); }
        }

        private int _awayTeamShootOutGoals;
        public int AwayTeamShootOutGoals
        {
            get { return _awayTeamShootOutGoals; }
            set { _awayTeamShootOutGoals = value; NotifyPropertyChanged("AwayTeamShootOutGoals"); }
        }

        private int _awayTeamShootOutGoalOpp;
        public int AwayTeamShootOutGoalOpp
        {
            get { return _awayTeamShootOutGoalOpp; }
            set { _awayTeamShootOutGoalOpp = value; NotifyPropertyChanged("AwayTeamShootOutGoalOpp"); }
        }

        private string _visible;
        public string Visible
        {
            get { return _visible; }
            set { _visible = value; NotifyPropertyChanged("Visible"); }
        }

        private string _homeTeamSideOfField;
        public string HomeTeamSideOfField
        {
            get { return _homeTeamSideOfField; }
            set { _homeTeamSideOfField = value; NotifyPropertyChanged("HomeTeamSideOfField"); }
        }

        private string _awayTeamSideOfField;
        public string AwayTeamSideOfField
        {
            get { return _awayTeamSideOfField; }
            set { _awayTeamSideOfField = value; NotifyPropertyChanged("AwayTeamSideOfField"); }
        }

        //Period1ActualLength to Period6ActualLength are used for CLOCK UP games so if period goes to extra time we know exactly how long
        //the period went, this is used when we calculate the player minutes
        private string _period1ActualLength;
        public string Period1ActualLength
        {
            get { return _period1ActualLength; }
            set { _period1ActualLength = value; NotifyPropertyChanged("Period1ActualLength"); }
        }

        private string _period2ActualLength;
        public string Period2ActualLength
        {
            get { return _period2ActualLength; }
            set { _period2ActualLength = value; NotifyPropertyChanged("Period2ActualLength"); }
        }

        private string _period3ActualLength;
        public string Period3ActualLength
        {
            get { return _period3ActualLength; }
            set { _period3ActualLength = value; NotifyPropertyChanged("Period3ActualLength"); }
        }

        private string _period4ActualLength;
        public string Period4ActualLength
        {
            get { return _period4ActualLength; }
            set { _period4ActualLength = value; NotifyPropertyChanged("Period4ActualLength"); }
        }

        private string _period5ActualLength;
        public string Period5ActualLength
        {
            get { return _period5ActualLength; }
            set { _period5ActualLength = value; NotifyPropertyChanged("Period5ActualLength"); }
        }

        private string _period6ActualLength;
        public string Period6ActualLength
        {
            get { return _period6ActualLength; }
            set { _period6ActualLength = value; NotifyPropertyChanged("Period6ActualLength"); }
        }

        private string _homeTeamDecision;
        public string HomeTeamDecision
        {
            get { return _homeTeamDecision; }
            set { _homeTeamDecision = value; NotifyPropertyChanged("HomeTeamDecision"); }
        }

        private string _awayTeamDecision;
        public string AwayTeamDecision
        {
            get { return _awayTeamDecision; }
            set { _awayTeamDecision = value; NotifyPropertyChanged("AwayTeamDecision"); }
        }

        private string _sampleData;
        public string SampleData
        {
            get { return _sampleData; }
            set { _sampleData = value; NotifyPropertyChanged("SampleData"); }
        }

        private string _onCloud;
        public string OnCloud
        {
            get { return _onCloud; }
            set { _onCloud = value; NotifyPropertyChanged("OnCloud"); }
        }

        #endregion "Properties"
    }
}
