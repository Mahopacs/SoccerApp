using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.DataObjects.DbClasses
{
    //This class/table is used to store event roster information.  This will tell you all the players that were on the team for that game, who 
    //started, and at the time of querying the table whether the player is on the field or not. 
    public class EventRoster: Notification
    {

        #region "Properties"

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

        private int _playerID;
        public int PlayerID
        {
            get { return _playerID; }
            set { _playerID = value; NotifyPropertyChanged("PlayerID"); }
        }
    
        private string _starter;
        public string Starter
        {
            get { return _starter; }
            set { _starter = value; NotifyPropertyChanged("Starter"); }
        }

        private string _isPlayerOnField;
        public string IsPlayerOnField
        {
            get { return _isPlayerOnField; }
            set { _isPlayerOnField = value; NotifyPropertyChanged("IsPlayerOnField"); }
        }

        private string _gMPlayerPositionID;
        public string GMPlayerPositionID
        {
            get { return _gMPlayerPositionID; }
            set { _gMPlayerPositionID = value; NotifyPropertyChanged("GMPlayerPositionID"); }
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
