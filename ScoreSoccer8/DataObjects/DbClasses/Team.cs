using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.DataObjects.DbClasses
{
    public class Team : Notification
    {
        public Team()
        {

        }

        private int _teamID;
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int TeamID
        {
            get { return _teamID; }
            set { _teamID = value; NotifyPropertyChanged("TeamID"); }
        }

        private string _teamName;
        public string TeamName
        {
            get { return _teamName; }
            set { _teamName = value; NotifyPropertyChanged("TeamName"); }
        }

        private string _teamShortName;
        public string TeamShortName
        {
            get { return _teamShortName; }
            set { _teamShortName = value; NotifyPropertyChanged("TeamShortName"); }
        }

        private int _jerseyID;
        public int JerseyID
        {
            get { return _jerseyID; }
            set { _jerseyID = value; NotifyPropertyChanged("JerseyID"); }
        }

        private int _flag;
        public int Flag
        {
            get { return _flag; }
            set { _flag = value; NotifyPropertyChanged("Flag"); }
        }

        private string _color;
        public string Color
        {
            get { return _color; }
            set { _color = value; NotifyPropertyChanged("Color"); }
        }

        private string _coach;
        public string Coach
        {
            get { return _coach; }
            set { _coach = value; NotifyPropertyChanged("Coach"); }
        }

        private string _contactNumber;
        public string ContactNumber
        {
            get { return _contactNumber; }
            set { _contactNumber = value; NotifyPropertyChanged("ContactNumber"); }
        }

        private string _notes;
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; NotifyPropertyChanged("Notes"); }
        }

        private string _visible;
        public string Visible
        {
            get { return _visible; }
            set { _visible = value; NotifyPropertyChanged("Visible"); }
        }

        private int _leagueID;
        public int LeagueID
        {
            get { return _leagueID; }
            set { _leagueID = value; NotifyPropertyChanged("LeagueID"); }
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

    }
}
