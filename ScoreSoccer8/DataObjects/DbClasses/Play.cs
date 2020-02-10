using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ScoreSoccer8.DataObjects.DbClasses
{
    public class Play : Notification
    {
        #region "Properties"

        private int _playID;
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int PlayID
        {
            get { return _playID; }
            set { _playID = value; NotifyPropertyChanged("PlayID"); }
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

        private int _player1ID;
        public int Player1ID
        {
            get { return _player1ID; }
            set { _player1ID = value; NotifyPropertyChanged("Player1ID"); }
        }

        //Used to store the player OUT part of a substitution 
        //(PLAYER1ID is the player IN for a substitution, and for NON SUB plays PLAYER1ID is simply the player getting the stat
        private int? _player2ID;
        public int? Player2ID
        {
            get { return _player2ID; }
            set { _player2ID = value; NotifyPropertyChanged("Player2ID"); }
        }

        private int? _assistID;
        public int? AssistID
        {
            get { return _assistID; }
            set { _assistID = value; NotifyPropertyChanged("AssistID"); }
        }

        private int? _shotBlockedByID;
        public int? ShotBlockedByID
        {
            get { return _shotBlockedByID; }
            set { _shotBlockedByID = value; NotifyPropertyChanged("ShotBlockedByID"); }
        }

        private int? _otherTeamGoalieID;
        public int? OtherTeamGoalieID
        {
            get { return _otherTeamGoalieID; }
            set { _otherTeamGoalieID = value; NotifyPropertyChanged("OtherTeamGoalieID"); }
        }

        private string _playerPosition;
        public string PlayerPosition
        {
            get { return _playerPosition; }
            set { _playerPosition = value; NotifyPropertyChanged("PlayerPosition"); }
        }

        private string _gMPlayer1PositionID;
        public string GMPlayer1PositionID
        {
            get { return _gMPlayer1PositionID; }
            set { _gMPlayer1PositionID = value; NotifyPropertyChanged("GMPlayer1PositionID"); }
        }

        private string _gMPlayer2PositionID;
        public string GMPlayer2PositionID
        {
            get { return _gMPlayer2PositionID; }
            set { _gMPlayer2PositionID = value; NotifyPropertyChanged("GMPlayer2PositionID"); }
        }

        private int _period;
        public int Period
        {
            get { return _period; }
            set { _period = value; NotifyPropertyChanged("Period"); }
        }

        private string _topInfo;
        public string TopInfo
        {
            get { return "Period: " + _period + ",  " + _gameTime + ",  " + _awayScore + " : " + _homeScore; }
            set { _topInfo = value; NotifyPropertyChanged("TopInfo"); }
        }

        private string _gameTime;
        public string GameTime
        {
            get { return _gameTime; }
            set { _gameTime = value; NotifyPropertyChanged("GameTime"); }
        }

        private int _elapsedTimeInSeconds;
        public int ElapsedTimeInSeconds
        {
            get { return _elapsedTimeInSeconds; }
            set { _elapsedTimeInSeconds = value; NotifyPropertyChanged("ElapsedTimeInSeconds"); }
        }

        private int _homeScore;
        public int HomeScore
        {
            get { return _homeScore; }
            set { _homeScore = value; NotifyPropertyChanged("HomeScore"); }
        }

        private int _awayScore;
        public int AwayScore
        {
            get { return _awayScore; }
            set { _awayScore = value; NotifyPropertyChanged("AwayScore"); }
        }

        private string _playText;
        public string PlayText
        {
            get { return _playText; }
            set { _playText = value; NotifyPropertyChanged("PlayText"); }
        }

        private int _statCategoryID;
        public int StatCategoryID
        {
            get { return _statCategoryID; }
            set { _statCategoryID = value; NotifyPropertyChanged("StatCategoryID"); }
        }

        private int? _statDescriptionID;
        public int? StatDescriptionID
        {
            get { return _statDescriptionID; }
            set { _statDescriptionID = value; NotifyPropertyChanged("StatDescriptionID"); }
        }

        //This is used to store left, right, or headed (ID equivalent)
        private int? _shotTypeID;
        public int? ShotTypeID
        {
            get { return _shotTypeID; }
            set { _shotTypeID = value; NotifyPropertyChanged("ShotTypeID"); }
        }

        //Indicated whether shot was on goal (Y or N value)
        private string _shotOnGoal;
        public string ShotOnGoal
        {
            get { return _shotOnGoal; }
            set { _shotOnGoal = value; NotifyPropertyChanged("ShotOnGoal"); }
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
