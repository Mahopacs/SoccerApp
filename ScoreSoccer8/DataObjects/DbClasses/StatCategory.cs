using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.DataObjects.DbClasses
{
    public class StatCategory : Notification
    {

        #region "Properties"

        private int _statCategoryID;
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int StatCategoryID
        {
            get { return _statCategoryID; }
            set { _statCategoryID = value; NotifyPropertyChanged("StatCategoryID"); }
        }

        private string _statCategoryName;
        public string StatCategoryName
        {
            get { return _statCategoryName; }
            set { _statCategoryName = value; NotifyPropertyChanged("StatCategoryName"); }
        }

        // this property represents if the stat is visible to the user (this value is set by the code and is NEVER changed)
        // whereas UserSelectedVisible is what the user can choose to display or not display for stats (i.e. they will never have the
        // the option of displaying a non visible stats (i.e. like PlusMinus or Player Minutes)
        private string _visible;
        public string Visible
        {
            get { return _visible; }
            set { _visible = value; NotifyPropertyChanged("Visible"); }
        }

        private string _userSelectedVisible;
        public string UserSelectedVisible
        {
            get { return _userSelectedVisible; }
            set { _userSelectedVisible = value; NotifyPropertyChanged("UserSelectedVisible"); }
        }

        //Indicates whether the stat is available to the user (i.e. Free or Paid version, for Free Version only Shots are active)
        private string _active;
        public string Active
        {
            get { return _active; }
            set { _active = value; NotifyPropertyChanged("Active"); }
        }

        private int _sortOrder;
        public int SortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; NotifyPropertyChanged("SortOrder"); }
        }

        private string _descriptions;
        public string Descriptions
        {
            get { return _descriptions; }
            set { _descriptions = value; NotifyPropertyChanged("Descriptions"); }
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
