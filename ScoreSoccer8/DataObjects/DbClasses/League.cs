using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.DataObjects.DbClasses
{
    public class League: Notification
    {
        #region "Properties"

        private int _leagueID;
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int LeagueID
        {
            get { return _leagueID; }
            set { _leagueID = value; NotifyPropertyChanged("LeagueID"); }
        }

        private string _leagueName;
        public string LeagueName
        {
            get { return _leagueName; }
            set { _leagueName = value; NotifyPropertyChanged("LeagueName"); }
        }

        private string _leagueContactName;
        public string LeagueContactName
        {
            get { return _leagueContactName; }
            set { _leagueContactName = value; NotifyPropertyChanged("LeagueContactName"); }
        }

        private string _leagueContactNumber;
        public string LeagueContactNumber
        {
            get { return _leagueContactNumber; }
            set { _leagueContactNumber = value; NotifyPropertyChanged("LeagueContactNumber"); }
        }

        private string _visible;
        public string Visible
        {
            get { return _visible; }
            set { _visible = value; NotifyPropertyChanged("Visible"); }
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
