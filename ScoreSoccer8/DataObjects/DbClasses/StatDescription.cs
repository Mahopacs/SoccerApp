using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.DataObjects.DbClasses
{
    public class StatDescription : Notification
    {
        #region "Properties"

        private int _statDescriptionID;
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int StatDescriptionID
        {
            get { return _statDescriptionID; }
            set { _statDescriptionID = value; NotifyPropertyChanged("StatDescriptionID"); }
        }

        private string _statDescriptionName;
        public string StatDescriptionName
        {
            get { return _statDescriptionName; }
            set { _statDescriptionName = value; NotifyPropertyChanged("StatDescriptionName"); }
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
