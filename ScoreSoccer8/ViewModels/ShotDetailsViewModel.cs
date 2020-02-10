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
    public class ShotDetailsViewModel : Notification
    {
        public ShotDetailsViewModel()
        {
            StatShotPlay = new Play();
        }

        #region "Properties"

        private Play _statShotPlay;
        public Play StatShotPlay
        {
            get { return _statShotPlay; }
            set { _statShotPlay = value; NotifyPropertyChanged("StatShotPlay"); }
        }

        private bool _shotMissChecked;
        public bool ShotMissChecked
        {
            get { return _shotMissChecked; }
            set
            {
                _shotMissChecked = value;
                IsShotOnGoalVisible = Visibility.Collapsed;
                AssistOrBlockLabelCaption = "";
                IsAssistOrBlockLabelvisible = Visibility.Collapsed;
                IsPlayerDropDownvisible = Visibility.Collapsed;
                ShotOnGoalChecked = false;
                ShotNotOnGoalChecked = true;
                NotifyPropertyChanged("ShotMissChecked");
            }
        }

        private bool _shotHitPostChecked;
        public bool ShotHitPostChecked
        {
            get { return _shotHitPostChecked; }
            set
            {
                //On penalty kicks, shootout kicks, and direct kicks there is no shot on goal so do not show shot on goal
                if ((StatCategoryName == "Penalty Kick") || (StatCategoryName == "Shootout Kick") 
                    || (StatCategoryName == "Direct Free Kick") || (StatCategoryName == "Corner Kick"))
                {
                    IsShotOnGoalVisible = Visibility.Collapsed;
                }
                else
                {
                    IsShotOnGoalVisible = Visibility.Visible;
                }

                _shotHitPostChecked = value;
                AssistOrBlockLabelCaption = "";
                IsAssistOrBlockLabelvisible = Visibility.Collapsed;
                IsPlayerDropDownvisible = Visibility.Collapsed;
                ShotOnGoalChecked = false;
                ShotNotOnGoalChecked = true;
                NotifyPropertyChanged("ShotHitPostChecked");
            }
        }

        private bool _shotBlockedChecked;
        public bool ShotBlockedChecked
        {
            get { return _shotBlockedChecked; }
            set
            {
                _shotBlockedChecked = value;

                //On penalty kicks, shootout kicks, and direct kicks there is no shot on goal so do not show shot on goal
                if ((StatCategoryName == "Penalty Kick") || (StatCategoryName == "Shootout Kick") || 
                    (StatCategoryName == "Direct Free Kick") || (StatCategoryName == "Corner Kick"))
                {
                    IsShotOnGoalVisible = Visibility.Collapsed;
                }
                else
                {
                    IsShotOnGoalVisible = Visibility.Visible;
                }
                            
                AssistOrBlockLabelCaption = AppResources.BlockedBy;
                IsAssistOrBlockLabelvisible = Visibility.Visible;
                IsPlayerDropDownvisible = Visibility.Visible;
                PlayerList = DAL.Instance().GetPlayersPhyiscalAndTeamRosterInfo(StatShotPlay.GameID, NonShotTeamID, true, true);
                SetBlockerDefault();
                ShotOnGoalChecked = true;
                ShotNotOnGoalChecked = false;
                NotifyPropertyChanged("ShotBlockedChecked");
            }
        }

        private bool _shotGoalChecked;
        public bool ShotGoalChecked
        {
            get { return _shotGoalChecked; }
            set
            {
                _shotGoalChecked = value;
               
                //On penalty kicks, shootout kicks, and direct kicks there is no assist so do not show assist drop down
                if ((StatCategoryName == "Penalty Kick") || (StatCategoryName == "Shootout Kick") || 
                    (StatCategoryName == "Direct Free Kick") || (StatCategoryName == "Corner Kick"))
                {
                    AssistOrBlockLabelCaption = "";
                    IsAssistOrBlockLabelvisible = Visibility.Collapsed;
                    IsPlayerDropDownvisible = Visibility.Collapsed;
                    IsShotOnGoalVisible = Visibility.Visible;

                    if (value == true) 
                    {
                        ShotOnGoalChecked = true;
                    }
                    else
                    {
                        ShotNotOnGoalChecked = true;
                    }      
                }
                else
                {
                    AssistOrBlockLabelCaption = AppResources.Assist;
                    IsAssistOrBlockLabelvisible = Visibility.Visible;
                    IsPlayerDropDownvisible = Visibility.Visible;
                    PlayerList = DAL.Instance().GetPlayersPhyiscalAndTeamRosterInfo(StatShotPlay.GameID, StatShotPlay.TeamID, true, true); 
                    IsShotOnGoalVisible = Visibility.Visible;
                    ShotOnGoalChecked = true;
                    ShotNotOnGoalChecked = false;
                }

                NotifyPropertyChanged("ShotGoalChecked");
            }
        }

        private Visibility _isShotOnGoalVisible;
        public Visibility IsShotOnGoalVisible
        {
            get { return _isShotOnGoalVisible; }
            set { _isShotOnGoalVisible = value; NotifyPropertyChanged("IsShotOnGoalVisible"); }
        }

        private string _assistOrBlockLabelCaption;
        public string AssistOrBlockLabelCaption
        {
            get { return _assistOrBlockLabelCaption; }
            set { _assistOrBlockLabelCaption = value; NotifyPropertyChanged("AssistOrBlockLabelCaption"); }
        }

        private Visibility _isAssistOrBlockLabelvisible;
        public Visibility IsAssistOrBlockLabelvisible
        {
            get { return _isAssistOrBlockLabelvisible; }
            set
            {
                _isAssistOrBlockLabelvisible = value; NotifyPropertyChanged("IsAssistOrBlockLabelvisible");
            }
        }

        private Visibility _isPlayerDropDownvisible;
        public Visibility IsPlayerDropDownvisible
        {
            get { return _isPlayerDropDownvisible; }
            set { _isPlayerDropDownvisible = value; NotifyPropertyChanged("IsPlayerDropDownvisible"); }
        }

        private bool _leftFootShotChecked;
        public bool LeftFootShotChecked
        {
            get { return _leftFootShotChecked; }
            set
            {
                _leftFootShotChecked = value;
                NotifyPropertyChanged("LeftFootShotChecked");
            }
        }

        private bool _rightFootShotChecked;
        public bool RightFootShotChecked
        {
            get { return _rightFootShotChecked; }
            set
            {
                _rightFootShotChecked = value;
                NotifyPropertyChanged("RightFootShotChecked");
            }
        }

        private bool _headedShotChecked;
        public bool HeadedShotChecked
        {
            get { return _headedShotChecked; }
            set
            {
                _headedShotChecked = value;
                NotifyPropertyChanged("HeadedShotChecked");
            }
        }

        private bool _shotOnGoalChecked;
        public bool ShotOnGoalChecked
        {
            get { return _shotOnGoalChecked; }
            set
            {
                _shotOnGoalChecked = value;
                NotifyPropertyChanged("ShotOnGoalChecked");
            }
        }

        private bool _shotNotOnGoalChecked;
        public bool ShotNotOnGoalChecked
        {
            get { return _shotNotOnGoalChecked; }
            set
            {
                _shotNotOnGoalChecked = value;
                NotifyPropertyChanged("ShotNotOnGoalChecked");
            }
        }

        private TeamRosterModel _selectedPlayer;
        public TeamRosterModel SelectedPlayer
        {
            get { return _selectedPlayer; }
            set { _selectedPlayer = value; NotifyPropertyChanged("SelectedPlayer"); }
        }

        private ObservableCollection<TeamRosterModel> _playerList;
        public ObservableCollection<TeamRosterModel> PlayerList
        {
            get { return _playerList; }
            set { _playerList = value; NotifyPropertyChanged("PlayerList"); }
        }

        private int _nonShotTeamID;
        public int NonShotTeamID
        {
            get { return _nonShotTeamID; }
            set { _nonShotTeamID = value; NotifyPropertyChanged("NonShotTeamID"); }
        }

        private string _statCategoryName;
        public string StatCategoryName
        {
            get { return _statCategoryName; }
            set { _statCategoryName = value; NotifyPropertyChanged("StatCategoryName"); }
        }

        private string _statDescription;
        public string StatDescription
        {
            get { return _statDescription; }
            set { _statDescription = value; NotifyPropertyChanged("StatDescription"); }
        }

        private bool _cancelButtonClicked;
        public bool CancelButtonClicked
        {
            get { return _cancelButtonClicked; }
            set { _cancelButtonClicked = value; NotifyPropertyChanged("CancelButtonClicked"); }
        }

        private bool _okButtonClicked;
        public bool OkButtonClicked
        {
            get { return _okButtonClicked; }
            set { _okButtonClicked = value; NotifyPropertyChanged("OkButtonClicked"); }
        }

        #endregion "Properties"

        #region "Commands"

        private ICommand _okClickCommand;
        public ICommand OkClickCommand
        {
            get
            {
                if (_okClickCommand == null)
                {
                    _okClickCommand = new DelegateCommand(param => this.OkClicked(), param => true);
                }

                return _okClickCommand;
            }
        }

        private ICommand _cancelClickCommand;
        public ICommand CancelClickCommand
        {
            get
            {
                if (_cancelClickCommand == null)
                {
                    _cancelClickCommand = new DelegateCommand(param => this.CancelClicked(), param => true);
                }

                return _cancelClickCommand;
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
        #endregion "Commands"

        #region "Methods"

        private void GoHelpScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Help_Buttons.xaml?screenId=" + Enums.Screen.ShotDetails, UriKind.Relative));
        }

        public void Initialize(int gameID, int playerID, int teamID, int period, string gameTime, string GMPlayer1PositionID, int otherTeamGoalieID, int statCategoryID, int statDescriptionID)
        {
            StatCategoryName = DAL.Instance().GetStatCategoryNameById(statCategoryID);
            StatDescription = DAL.Instance().GetStatDescriptionNameById(statDescriptionID);

            StatShotPlay.GameID = gameID;
            StatShotPlay.Period = period;
            StatShotPlay.GameTime = gameTime;
            StatShotPlay.Player1ID = playerID;
            StatShotPlay.TeamID = teamID;
            StatShotPlay.GMPlayer1PositionID = GMPlayer1PositionID;
            StatShotPlay.OtherTeamGoalieID = otherTeamGoalieID;
            NonShotTeamID = Common.Instance().GetOtherTeamID(gameID, teamID);

            //Set Default Values
            RightFootShotChecked = true;

            if ((StatDescription == "Goal") || (StatDescription == "For Goal"))
            {
                ShotGoalChecked = true;
                ShotOnGoalChecked = true;                
            }
            else
            {
                ShotMissChecked = true;
                ShotNotOnGoalChecked = true;
            }        
        }
      
        private string GetShotOnGoalValue()
        {
            string returnValue = "N";

            try
            {
                if (ShotOnGoalChecked == true)
                {
                    returnValue = "Y";
                }

                return returnValue;
            }
            catch (Exception)
            {
                return returnValue;
            }
        }

        private int? GetShotTypeIdValue()
        {
            int? shotTypeID;
            string shotTypeDesc = "Right";

            try
            {
                if (LeftFootShotChecked == true)
                {
                    shotTypeDesc = "Left";
                }
                else if (RightFootShotChecked == true)
                {
                    shotTypeDesc = "Right";
                }
                else
                {
                    shotTypeDesc = "Headed";
                }

                shotTypeID = DAL.Instance().GetStatDescriptionByName(shotTypeDesc).StatDescription.StatDescriptionID;

                return shotTypeID;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private int? GetShotDescriptionValue()
        {
            int? shotDescID;
            string shotDesc = "Miss";

            try
            {
                if (ShotMissChecked == true)
                {
                    shotDesc = "Miss";
                }
                else if (ShotHitPostChecked == true)
                {
                    shotDesc = "Hit Post";
                }
                else if (ShotBlockedChecked == true)
                {
                    shotDesc = "Blocked";
                }
                else
                {
                    shotDesc = "Goal";
                }

                shotDescID = DAL.Instance().GetStatDescriptionByName(shotDesc).StatDescription.StatDescriptionID;

                return shotDescID;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private int? GetShotBlockedByIdValue()
        {
            int? shotBlockedByID = null;

            try
            {
                if (ShotBlockedChecked == true)
                {
                    shotBlockedByID = SelectedPlayer.Player.PlayerID;
                }

                return shotBlockedByID;
            }
            catch (Exception)
            {
                return shotBlockedByID;
            }
        }

        private int? GetAssistIdValue()
        {
            int? shotAssistID = null;

            try
            {
                if (ShotGoalChecked == true)
                {
                    if (SelectedPlayer != null)
                    {
                        shotAssistID = SelectedPlayer.Player.PlayerID;
                    }
                }

                return shotAssistID;
            }
            catch (Exception)
            {
                return shotAssistID;
            }
        }

        private void OkClicked()
        {
            OkButtonClicked = true;
            SaveToDatabase();
            (Application.Current.RootVisual as Frame).GoBack();
        }

        public void SaveToDatabase()
        {
            StatShotPlay.StatCategoryID = DAL.Instance().GetStatCategoryIDByName(StatCategoryName);
            StatShotPlay.StatDescriptionID = GetShotDescriptionValue(); //this is the ID for Miss, Hit Post, Blocked, Goal
            StatShotPlay.ShotTypeID = GetShotTypeIdValue(); //this is the ID for left, right, or headed shot
            StatShotPlay.ShotOnGoal = GetShotOnGoalValue(); //this is Y or N for shot on goal
            StatShotPlay.ShotBlockedByID = GetShotBlockedByIdValue();
            StatShotPlay.AssistID = GetAssistIdValue();

            DAL.Instance().UpsertPlay(StatShotPlay, "GM");
        }
      
        private void CancelClicked()
        {
            CancelButtonClicked = true;
            (Application.Current.RootVisual as Frame).GoBack();
        }
       
        private void SetBlockerDefault()
        {
            //set blocker to the other teams goalie as the default
            foreach (var item in PlayerList)
            {
                if (StatShotPlay.OtherTeamGoalieID == item.Player.PlayerID)
                {
                    SelectedPlayer = item;
                }
            }
        }

        #endregion "Methods"
    }
}
