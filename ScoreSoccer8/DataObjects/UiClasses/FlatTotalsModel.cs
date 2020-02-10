using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.DataObjects.UiClasses
{
    public class FlatTotalsModel: Notification
    {

        public FlatTotalsModel()
        {
            _backgroundOpacity = 0.3;
            _backgroundColor = "Transparent";
        }

        #region "Properties"

        private double _backgroundOpacity;
        public double BackgroundOpacity
        {
            get { return _backgroundOpacity; }
            set { _backgroundOpacity = value; NotifyPropertyChanged("BackgroundOpacity"); }
        }

        private string _backgroundColor;
        public string BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; NotifyPropertyChanged("BackgroundColor"); }
        }

        private FlatTotals _flatTotals;
        public FlatTotals FlatTotals
        {
            get { return _flatTotals; }
            set { _flatTotals = value; NotifyPropertyChanged("FlatTotals"); }
        }

        private Player _player;
        public Player Player
        {
            get { return _player; }
            set { _player = value; NotifyPropertyChanged("Player"); }
        }

        private Team _team;
        public Team Team
        {
            get { return _team; }
            set { _team = value; NotifyPropertyChanged("Team"); }
        }

        private TeamRoster _teamRoster;
        public TeamRoster TeamRoster
        {
            get { return _teamRoster; }
            set { _teamRoster = value; NotifyPropertyChanged("TeamRoster"); }
        }

        private string _rosterDisplayText;
        public string RosterDisplayText
        {
            get { return _rosterDisplayText; }
            set { _rosterDisplayText = value; NotifyPropertyChanged("RosterDisplayText"); }
        }

     

        #endregion "Properties"
    }
}
