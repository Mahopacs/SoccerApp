using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.Resources;
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
    public class PlayerModel : Notification
    {
        public delegate void PlayerTapped(object sender, EventArgs e);
        public event PlayerTapped PlayerTappedByUser;

        public event EventHandler PlayerDeleted;
        public event EventHandler PlayerAddedToRoster;

        public PlayerModel()
        {
            PlayerOpacity = 1;           
        }

        //        public PlayerModel(Player p)
        //        {
        //            Player.PlayerID = p.PlayerID;
        //            Player.FirstName = p.FirstName;
        //            Player.LastName = p.LastName;
        //            Player.Height = p.Height;
        //            Player.Weight = p.Weight;
        //            Player.Kicks = p.Kicks;
        //            PlayerOpacity = 1;
        //         //   FieldColumn = p.FieldColumn;
        ////UniqueIdentifier = p.UniqueIdentifier;
        //              FieldColumn = p.FieldColumn;
        //UniqueIdentifier = p.UniqueIdentifier;
        //        }

        #region "Events"

        protected virtual void OnPlayerDeleted(EventArgs e)
        {
            EventHandler handler = PlayerDeleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnPlayerAddedToRoster(EventArgs e)
        {
            EventHandler handler = PlayerAddedToRoster;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion "Events"

        #region "Properties"

        private Player _player;
        public Player Player
        {
            get { return _player; }
            set { _player = value; NotifyPropertyChanged("Player"); }
        }

        private string _playerName;
        public string PlayerName
        {
            get { return _playerName; }
            set { _playerName = value; NotifyPropertyChanged("PlayerName"); }
        }

        private bool _toggledForSub;
        public bool ToggledForSub
        {
            get { return _toggledForSub; }
            set { _toggledForSub = value; NotifyPropertyChanged("ToggledForSub"); }
        }

        private int _playerOpacity;
        public int PlayerOpacity
        {
            get { return _playerOpacity; }
            set { _playerOpacity = value; NotifyPropertyChanged("PlayerOpacity"); }
        }

        public int FieldColumn { get; set; }
        public bool Home { get; set; }
        public int UniqueIdentifier { get; set; }

        private Visibility _isAddButtonVisible;
        public Visibility IsAddButtonVisible
        {
            get { return _isAddButtonVisible; }
            set { _isAddButtonVisible = value; NotifyPropertyChanged("IsAddButtonVisible"); }
        }

        #endregion "Properties"

        #region Commands

        private ICommand _goToPlayerDetailsCommand;
        public ICommand GoToPlayerDetailsCommand
        {
            get
            {
                if (_goToPlayerDetailsCommand == null)
                {
                    _goToPlayerDetailsCommand = new DelegateCommand(param => this.GoToPlayerDetailsScreen(), param => true);
                }

                return _goToPlayerDetailsCommand;
            }
        }

        private ICommand _goToDeletePlayerCommand;
        public ICommand GoToDeletePlayerCommand
        {
            get
            {
                if (_goToDeletePlayerCommand == null)
                {
                    _goToDeletePlayerCommand = new DelegateCommand(param => this.GoToDeletePlayer(), param => true);
                }

                return _goToDeletePlayerCommand;
            }
        }

        private ICommand _goToAddPlayerToRosterCommand;
        public ICommand GoToAddPlayerToRosterCommand
        {
            get
            {
                if (_goToAddPlayerToRosterCommand == null)
                {
                    _goToAddPlayerToRosterCommand = new DelegateCommand(param => this.GoToAddPlayerToRoster(), param => true);
                }

                return _goToAddPlayerToRosterCommand;
            }
        }


        /// <summary>
        /// This command is only to be used by the Game Manager Screen.
        /// </summary>
        private ICommand _playerTappedCommand;
        public ICommand PlayerTappedCommand
        {
            get
            {
                if (_playerTappedCommand == null)
                {
                    _playerTappedCommand = new DelegateCommand(param => this.PlayerTappedInGameManagerScreen(), param => true);
                }

                return _playerTappedCommand;
            }
        }

        #endregion Commands

        #region "Methods"

        public void PlayerTappedInGameManagerScreen()
        {
            if (PlayerTappedByUser != null)
            {
                PlayerTappedByUser(this, new EventArgs());
            }
        }

        private void GoToPlayerDetailsScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/PlayerDetails.xaml?gameID=0" + "&teamID=0" + "&playerID=" + Player.PlayerID, UriKind.Relative));
        }

        private void GoToDeletePlayer()
        {
             MessageBoxResult  result= MessageBox.Show(AppResources.Delete + " '" + Player.FirstName + " " + Player.LastName + "'" + AppResources.FromPlayerList + "?", AppResources.DeletePlayer, MessageBoxButton.OKCancel);
             if (result == MessageBoxResult.OK)
             {
                 this.Player.Visible = "N";
                 BaseTableDataAccess.Instance().UpsertPlayer(this.Player, 0, 0);
                 //Also need to delete player from ANY team roster
                 DAL.Instance().DeletePlayerFromAnyAllTeamRosters(Player.PlayerID);
                 OnPlayerDeleted(EventArgs.Empty);
             }
        }

        private void GoToAddPlayerToRoster()
        {
            OnPlayerAddedToRoster(EventArgs.Empty);
        }

        #endregion "Methods"

    }
}

