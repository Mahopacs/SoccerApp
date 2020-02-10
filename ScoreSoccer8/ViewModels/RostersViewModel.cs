using ScoreSoccer8.Classes;
using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.Resources;
using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ScoreSoccer8.ViewModels
{

    public class RostersViewModel : Notification
    {

        public RostersViewModel()
        {
            RosterList = new ObservableCollection<TeamRosterModel>();                     
        }

        #region "Properties"

        private ObservableCollection<TeamRosterModel> _rosterList;
        public ObservableCollection<TeamRosterModel> RosterList
        {
            get { return _rosterList; }
            set { _rosterList = value; NotifyPropertyChanged("RosterList"); }
        }

        private string _pageTitle;
        public string PageTitle
        {
            get { return _pageTitle; }
            set { _pageTitle = value; NotifyPropertyChanged("PageTitle"); }
        }
     
        private int _teamID;  
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

        #endregion "Properties"

        #region Commands

        private ICommand _addPlayerClickCommand;
        public ICommand AddPlayerClickCommand
        {
            get
            {
                if (_addPlayerClickCommand == null)
                {
                    _addPlayerClickCommand = new DelegateCommand(param => this.GoToAddPlayersScreen(), param => true);
                }

                return _addPlayerClickCommand;
            }
        }

        private ICommand _helpClickCommand;
        public ICommand HelpClickCommand
        {
            get
            {
                if (_helpClickCommand == null)
                {
                    _helpClickCommand = new DelegateCommand(param => this.GoHelpScreen(), param => true);
                }

                return _helpClickCommand;
            }
        }

        #endregion  Commands

        #region "Methods"

        private void GoHelpScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Help_Buttons.xaml?screenId=" + Enums.Screen.Rosters, UriKind.Relative));
        }

        private void GoToAddPlayersScreen()
        {
              (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/PlayerList.xaml?gameID=0 " + "&teamID=" + TeamID + "&teamName=" + TeamName, UriKind.Relative));         
        }

        public void Initialize(int teamID)
        {
            Team team = BaseTableDataAccess.Instance().GetTeamByTeamID(teamID);
            TeamID = teamID;
            TeamName = team.TeamName;
            PageTitle = TeamName;
            PopulateRosterList();         
        }
  
        private void PopulateRosterList()
        {
            string activeStatus = string.Empty;
            RosterList = BaseTableDataAccess.Instance().GetTeamRoster(TeamID);
          
            foreach (var item in RosterList)
            {
                item.PlayerRosterDeleted += item_PlayerRosterDeleted;

                Player player = new Player();
                player = DAL.Instance().GetPlayer(item.Player.PlayerID);

                if (item.TeamRoster.Active == "Y")
                {
                    activeStatus = AppResources.Active.PadRight(7);
                }
                else
                {
                    activeStatus = AppResources.InActive;
                }
            
                item.RosterDisplayText = item.TeamRoster.UniformNumber.PadRight(4, ' ') + activeStatus + " " + player.FirstName + " "  + player.LastName;   
            }
        }

        void item_PlayerRosterDeleted(object sender, EventArgs e)
        {
            PopulateRosterList();
        }
        
        #endregion "Methods"

    }
}
