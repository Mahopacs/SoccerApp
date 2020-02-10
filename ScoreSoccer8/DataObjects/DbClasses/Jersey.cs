using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.DataObjects.DbClasses
{
    //This class/table is used to store jersey information.  The ID, name, and image path.  Upon initialization of application, when database is created
    //we load this data with our jersey information, see DAL.PopulateJerseyTable
    public class Jersey: Notification
    {
        #region "Properties"

        private int _jerseyID;
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int JerseyID
        {
            get { return _jerseyID; }
            set { _jerseyID = value; NotifyPropertyChanged("JerseyID"); }
        }

        private string _imagePath;
        public string ImagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; NotifyPropertyChanged("ImagePath"); }
        }

        private string _jerseyName;
        public string JerseyName
        {
            get { return _jerseyName; }
            set { _jerseyName = value; NotifyPropertyChanged("JerseyName"); }
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
