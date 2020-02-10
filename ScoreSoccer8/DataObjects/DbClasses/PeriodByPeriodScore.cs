using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.DataObjects.DbClasses
{
    //This class/table is used to store the games period by period score 
    public class PeriodByPeriodScore: Notification
    {

        #region "Properties"

        private int _gameID;
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int GameID
        {
            get { return _gameID; }
            set { _gameID = value; NotifyPropertyChanged("GameID"); }
        }

        private int _period;
        public int Period
        {
            get { return _period; }
            set { _period = value; NotifyPropertyChanged("Period"); }
        }

        private int _homeTeamScore;
        public int HomeTeamScore
        {
            get { return _homeTeamScore; }
            set { _homeTeamScore = value; NotifyPropertyChanged("HomeTeamScore"); }
        }

        private int _awayTeamScore;
        public int AwayTeamScore
        {
            get { return _awayTeamScore; }
            set { _awayTeamScore = value; NotifyPropertyChanged("AwayTeamScore"); }
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
