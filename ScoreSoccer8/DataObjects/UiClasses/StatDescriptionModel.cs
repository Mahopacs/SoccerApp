using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ScoreSoccer8.DataObjects.UiClasses
{
    public class StatDescriptionModel : Notification
    {
        public delegate void SaveStatDescription(object sender, EventArgs e);
        public event SaveStatDescription SaveStatDesc;

        #region "Properties"

        private StatDescription statDescription;
        public StatDescription StatDescription
        {
            get { return statDescription; }
            set { statDescription = value; NotifyPropertyChanged("StatDescription"); }
        }

        private int _otherTeamGoalieID;
        public int OtherTeamGoalieID
        {
            get { return _otherTeamGoalieID; }
            set { _otherTeamGoalieID = value; NotifyPropertyChanged("OtherTeamGoalieID"); }
        }

        private int _playerTeamID;
        public int PlayerTeamID
        {
            get { return _playerTeamID; }
            set { _playerTeamID = value; NotifyPropertyChanged("PlayerTeamID"); }
        }

        private int _playID;
        public int PlayID
        {
            get { return _playID; }
            set { _playID = value; NotifyPropertyChanged("PlayID"); }
        }

        private int _playerID;
        public int PlayerID
        {
            get { return _playerID; }
            set { _playerID = value; NotifyPropertyChanged("PlayerID"); }
        }

        private int _gameID;
        public int GameID
        {
            get { return _gameID; }
            set { _gameID = value; NotifyPropertyChanged("GameID"); }
        }

        #endregion "Properties"

        #region "Commands"

        private ICommand _goToSaveStatAfterDescriptionClickCommand;
        public ICommand GoToSaveStatAfterDescriptionClickCommand
        {
            get
            {
                if (_goToSaveStatAfterDescriptionClickCommand == null)
                {
                    _goToSaveStatAfterDescriptionClickCommand = new DelegateCommand(param => this.SaveStatPlayToDatabase(), param => true);
                }

                return _goToSaveStatAfterDescriptionClickCommand;
            }
        }

        #endregion "Commands"

        #region "Methods"

        private void SaveStatPlayToDatabase()
        {
            if (SaveStatDesc != null)
            {
                SaveStatDesc(this, new EventArgs());
            }
        }

        

        #endregion "Methods"
    }
}
