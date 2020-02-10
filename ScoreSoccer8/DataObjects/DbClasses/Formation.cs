using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.DataObjects.DbClasses
{
    //This class/table is currently not being used as we are allowing the user to create whatever formation they want to on Game Manager screen.
    //If at some point we use this the data will be like -> FormationCount = 8, FormationName = "2-3-2"
    public class Formation: Notification
    {
        #region "Properties"

        private int _formationID;
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int FormationID
        {
            get { return _formationID; }
            set { _formationID = value; NotifyPropertyChanged("FormationID"); }
        }

        private int _formationCount;
        public int FormationCount
        {
            get { return _formationCount; }
            set { _formationCount = value; NotifyPropertyChanged("FormationCount"); }
        }

        private string _formationName;
        public string FormationName
        {
            get { return _formationName; }
            set { _formationName = value; NotifyPropertyChanged("FormationName"); }
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
