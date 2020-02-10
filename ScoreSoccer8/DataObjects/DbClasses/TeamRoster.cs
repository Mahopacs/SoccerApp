using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.DataObjects.DbClasses
{
    public class TeamRoster: Notification
    {
        #region "Properties"

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

        private string _uniformNumber;
        public string UniformNumber
        {
            get { return _uniformNumber; }
            set { _uniformNumber = value; NotifyPropertyChanged("UniformNumber"); }
        }
      
        private string _active;
        public string Active
        {
            get { return _active; }
            set { _active = value; NotifyPropertyChanged("Active"); }
        }

        //Instead of deleting a player from the team roster we mark it invisible, we do this with teams and players too
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
