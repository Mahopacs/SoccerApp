using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.DataObjects.DbClasses
{
    public class Player : Notification
    {
        public Player()
        {

        }

        private int _playerID;
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int PlayerID
        {
            get { return _playerID; }
            set { _playerID = value; NotifyPropertyChanged("PlayerID"); }
        }

        public string FullName
        {
            get { return _firstName + " " + _lastName; }

        }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; NotifyPropertyChanged("FirstName"); }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; NotifyPropertyChanged("LastName"); }
        }

        private string _height;
        public string Height
        {
            get { return _height; }
            set { _height = value; NotifyPropertyChanged("Height"); }
        }

        private int? _weight;
        public int? Weight
        {
            get { return _weight; }
            set { _weight = value; NotifyPropertyChanged("Weight"); }
        }

        private string _kicks;
        public string Kicks
        {
            get { return _kicks; }
            set { _kicks = value; NotifyPropertyChanged("Kicks"); }
        }

        private string _visible;
        public string Visible
        {
            get { return _visible; }
            set { _visible = value; NotifyPropertyChanged("Visible"); }
        }

        private string _sampleData;
        public string SampleData
        {
            get { return _sampleData; }
            set { _sampleData = value; NotifyPropertyChanged("SampleData"); }
        }

        private DateTime _birthDate;
        public DateTime BirthDate
        {
            get { return _birthDate; }
            set { _birthDate = value; NotifyPropertyChanged("BirthDate"); }
        }

        private string _onCloud;
        public string OnCloud
        {
            get { return _onCloud; }
            set { _onCloud = value; NotifyPropertyChanged("OnCloud"); }
        }
    }
}
