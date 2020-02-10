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
    public class PlayDetailsViewModel : Notification
    {
        private string _messageBoxTitle = AppResources.PlayDetails;

        public PlayDetailsViewModel()
        {
            TeamsList = new ObservableCollection<Team>();
            PeriodsList = new ObservableCollection<string>();
            StatCategories = new ObservableCollection<StatCategoryModel>();
            StatCategoryDescriptions = new ObservableCollection<StatDescription>();
        }

        #region "Properties"

        private PlayModel _playDetails;
        public PlayModel PlayDetails
        {
            get { return _playDetails; }
            set { _playDetails = value; NotifyPropertyChanged("PlayDetails"); }
        }

        private ObservableCollection<Team> _teamsList;
        public ObservableCollection<Team> TeamsList
        {
            get { return _teamsList; }
            set { _teamsList = value; NotifyPropertyChanged("TeamsList"); }
        }

        private Team _selectedTeam;
        public Team SelectedTeam
        {
            get { return _selectedTeam; }
            set
            {
                _selectedTeam = value;
                PlayerList = DAL.Instance().GetPlayersPhyiscalAndTeamRosterInfo(Game.Game.GameID, _selectedTeam.TeamID, false, true);
                NonShotTeamID = Common.Instance().GetOtherTeamID(Game.Game.GameID, _selectedTeam.TeamID);
                PopulateSubInAndOutDropDowns();
                NotifyPropertyChanged("SelectedTeam");
            }
        }

        private ObservableCollection<TeamRosterModel> _playerList;
        public ObservableCollection<TeamRosterModel> PlayerList
        {
            get { return _playerList; }
            set { _playerList = value; NotifyPropertyChanged("PlayerList"); }
        }

        private TeamRosterModel _selectedPlayer;
        public TeamRosterModel SelectedPlayer
        {
            get { return _selectedPlayer; }
            set { _selectedPlayer = value; NotifyPropertyChanged("SelectedPlayer"); }
        }

        private TeamRosterModel _selectedPlayerIn;
        public TeamRosterModel SelectedPlayerIn
        {
            get { return _selectedPlayerIn; }
            set { _selectedPlayerIn = value; NotifyPropertyChanged("SelectedPlayerIn"); }
        }

        private TeamRosterModel _selectedPlayerOut;
        public TeamRosterModel SelectedPlayerOut
        {
            get { return _selectedPlayerOut; }
            set { _selectedPlayerOut = value; NotifyPropertyChanged("SelectedPlayerOut"); }
        }

        private ObservableCollection<string> _periodsList;
        public ObservableCollection<string> PeriodsList
        {
            get { return _periodsList; }
            set { _periodsList = value; NotifyPropertyChanged("PeriodsList"); }
        }

        private string _selectedPeriod;
        public string SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                _selectedPeriod = value;
                PopulateSubInAndOutDropDowns();
                NotifyPropertyChanged("SelectedPeriod");
            }
        }

        private string _clock;
        public string Clock
        {
            get { return _clock; }
            set
            {
                _clock = value;
                PopulateSubInAndOutDropDowns();
                NotifyPropertyChanged("Clock");
            }
        }

        private TimeSpan _clockTimeSpan;
        public TimeSpan ClockTimeSpan
        {
            get { return _clockTimeSpan; }
            set
            {
                _clockTimeSpan = value;
                NotifyPropertyChanged("ClockTime");

                string returnValue = string.Empty;
                int actualMinutes = ClockTimeSpan.Minutes + (ClockTimeSpan.Hours * 60);
                int actualSeconds = ClockTimeSpan.Seconds;

                string minutesString;
                string secondsString;

                if (actualMinutes.ToString().Length == 0)
                {
                    minutesString = "00";
                }
                else if (actualMinutes.ToString().Length == 1)
                {
                    minutesString = "0" + actualMinutes.ToString();
                }
                else
                {
                    minutesString = actualMinutes.ToString();
                }

                if (actualSeconds.ToString().Length == 0)
                {
                    secondsString = "00";
                }
                else if (actualSeconds.ToString().Length == 1)
                {
                    secondsString = "0" + actualSeconds.ToString();
                }
                else
                {
                    secondsString = actualSeconds.ToString();
                }

                Clock = minutesString + ":" + secondsString;
            }
        }

        private ObservableCollection<StatCategoryModel> _statCategories;
        public ObservableCollection<StatCategoryModel> StatCategories
        {
            get { return _statCategories; }
            set { _statCategories = value; NotifyPropertyChanged("StatCategories"); }
        }

        private StatCategoryModel _selectedStatCategory;
        public StatCategoryModel SelectedStatCategory
        {
            get { return _selectedStatCategory; }
            set
            {
                _selectedStatCategory = value;

                //Based on selected category, need to load the appropriate description for that Stat Category              
                FilterStatCategoryDescriptionsMasterList(_selectedStatCategory.StatCategory.StatCategoryName);

                if ((_selectedStatCategory.StatCategory.StatCategoryName == AppResources.Shot) ||
                    (_selectedStatCategory.StatCategory.StatCategoryName == AppResources.ShootoutKick) ||
                    (_selectedStatCategory.StatCategory.StatCategoryName == AppResources.PenaltyKick))
                {
                    IsShotOnGoalVisible = Visibility.Collapsed;
                    AssistOrBlockLabelCaption = "";
                    IsLeftRightHeadedVisible = Visibility.Visible;
                    IsAssistOrBlockLabelVisible = Visibility.Collapsed;
                    IsAssistOrBlockPlayerDropDownVisible = Visibility.Collapsed;
                    PlayerEnabled = true;
                    SubInputVisibility = Visibility.Collapsed;
                    IsOtherTeamGoalieVisible = Visibility.Collapsed;
                    IsOtherTeamGoalieDropDownVisible = Visibility.Collapsed;
                }
                else if ((_selectedStatCategory.StatCategory.StatCategoryName == AppResources.CornerKick) || (_selectedStatCategory.StatCategory.StatCategoryName == AppResources.DirectFreeKick))
                {
                    PlayerEnabled = true;
                    SubInputVisibility = Visibility.Collapsed;

                    if (SelectedStatCategoryDescription.StatDescriptionName == AppResources.ForGoal)
                    {
                        AssistOrBlockLabelCaption = "";
                        IsAssistOrBlockLabelVisible = Visibility.Collapsed;
                        IsAssistOrBlockPlayerDropDownVisible = Visibility.Collapsed;
                        IsShotOnGoalVisible = Visibility.Visible;
                        IsLeftRightHeadedVisible = Visibility.Visible;
                        IsOtherTeamGoalieVisible = Visibility.Visible;
                        IsOtherTeamGoalieDropDownVisible = Visibility.Visible;
                        OtherTeamGoaliePlayerList = DAL.Instance().GetPlayersPhyiscalAndTeamRosterInfo(Game.Game.GameID, NonShotTeamID, true, true);
                        SetOtherTeamGoalieDefault();
                    }
                    else
                    {
                        IsShotOnGoalVisible = Visibility.Collapsed;
                        AssistOrBlockLabelCaption = "";
                        IsLeftRightHeadedVisible = Visibility.Collapsed;
                        IsAssistOrBlockLabelVisible = Visibility.Collapsed;
                        IsAssistOrBlockPlayerDropDownVisible = Visibility.Collapsed;
                        IsOtherTeamGoalieVisible = Visibility.Collapsed;
                        IsOtherTeamGoalieDropDownVisible = Visibility.Collapsed;
                    }
                }
                else if (_selectedStatCategory.StatCategory.StatCategoryName == AppResources.Substitution)
                {
                    IsShotOnGoalVisible = Visibility.Collapsed;
                    AssistOrBlockLabelCaption = "";
                    IsLeftRightHeadedVisible = Visibility.Collapsed;
                    IsAssistOrBlockLabelVisible = Visibility.Collapsed;
                    IsAssistOrBlockPlayerDropDownVisible = Visibility.Collapsed;
                    SubInputVisibility = Visibility.Visible;
                    IsOtherTeamGoalieVisible = Visibility.Collapsed;
                    IsOtherTeamGoalieDropDownVisible = Visibility.Collapsed;
                    PlayerEnabled = false;
                    PopulateSubInAndOutDropDowns();
                }
                else
                {
                    IsShotOnGoalVisible = Visibility.Collapsed;
                    AssistOrBlockLabelCaption = "";
                    IsLeftRightHeadedVisible = Visibility.Collapsed;
                    IsAssistOrBlockLabelVisible = Visibility.Collapsed;
                    IsAssistOrBlockPlayerDropDownVisible = Visibility.Collapsed;
                    PlayerEnabled = true;
                    SubInputVisibility = Visibility.Collapsed;
                    IsOtherTeamGoalieVisible = Visibility.Collapsed;
                    IsOtherTeamGoalieDropDownVisible = Visibility.Collapsed;
                }
                NotifyPropertyChanged("SelectedStatCategory");
            }
        }

        private ObservableCollection<StatDescription> _statCategoryDescriptionsMasterList;
        public ObservableCollection<StatDescription> StatCategoryDescriptionsMasterList
        {
            get { return _statCategoryDescriptionsMasterList; }
            set { _statCategoryDescriptionsMasterList = value; NotifyPropertyChanged("StatCategoryDescriptionsMasterList"); }
        }

        private ObservableCollection<StatDescription> _statCategoryDescriptions;
        public ObservableCollection<StatDescription> StatCategoryDescriptions
        {
            get { return _statCategoryDescriptions; }
            set { _statCategoryDescriptions = value; NotifyPropertyChanged("StatCategoryDescriptions"); }
        }

        private StatDescription _selectedStatCategoryDescription;
        public StatDescription SelectedStatCategoryDescription
        {
            get { return _selectedStatCategoryDescription; }
            set
            {
                _selectedStatCategoryDescription = value;

                if (_selectedStatCategoryDescription != null)
                {
                    if (_selectedStatCategoryDescription.StatDescriptionName == AppResources.HitPost)
                    {

                    }

                    else if (_selectedStatCategoryDescription.StatDescriptionName == AppResources.Miss)
                    {
                        IsShotOnGoalVisible = Visibility.Collapsed;
                        AssistOrBlockLabelCaption = "";
                        IsLeftRightHeadedVisible = Visibility.Visible;
                        IsAssistOrBlockLabelVisible = Visibility.Collapsed;
                        IsAssistOrBlockPlayerDropDownVisible = Visibility.Collapsed;

                        IsOtherTeamGoalieVisible = Visibility.Collapsed;
                        IsOtherTeamGoalieDropDownVisible = Visibility.Collapsed;
                    }

                    else if (_selectedStatCategoryDescription.StatDescriptionName == AppResources.Blocked)
                    {
                        IsShotOnGoalVisible = Visibility.Visible;
                        AssistOrBlockLabelCaption = AppResources.BlockedBy;
                        IsLeftRightHeadedVisible = Visibility.Visible;
                        IsAssistOrBlockLabelVisible = Visibility.Visible;
                        IsAssistOrBlockPlayerDropDownVisible = Visibility.Visible;
                        AssistOrBlockPlayerList = DAL.Instance().GetPlayersPhyiscalAndTeamRosterInfo(Game.Game.GameID, NonShotTeamID, true, true);
                        SetBlockerDefault();

                        IsOtherTeamGoalieVisible = Visibility.Collapsed;
                        IsOtherTeamGoalieDropDownVisible = Visibility.Collapsed;
                    }

                    else if (_selectedStatCategoryDescription.StatDescriptionName == AppResources.Goal)
                    {
                        //All goal plays default to shot on goal except shootout kicks
                        if (SelectedStatCategory.StatCategory.StatCategoryName == AppResources.ShootoutKick)
                        {
                            ShotOnGoalChecked = false;
                            ShotNotOnGoalChecked = true;
                        }
                        else
                        {
                            ShotOnGoalChecked = true;
                            ShotNotOnGoalChecked = false;
                        }

                        if ((SelectedStatCategory.StatCategory.StatCategoryName == AppResources.ShootoutKick) ||
                           (SelectedStatCategory.StatCategory.StatCategoryName == AppResources.PenaltyKick))
                        {
                            AssistOrBlockLabelCaption = "";
                            IsAssistOrBlockLabelVisible = Visibility.Collapsed;
                            IsAssistOrBlockPlayerDropDownVisible = Visibility.Collapsed;
                        }
                        else
                        {
                            AssistOrBlockLabelCaption = AppResources.Assist;
                            IsAssistOrBlockLabelVisible = Visibility.Visible;
                            IsAssistOrBlockPlayerDropDownVisible = Visibility.Visible;
                            AssistOrBlockPlayerList = DAL.Instance().GetPlayersPhyiscalAndTeamRosterInfo(Game.Game.GameID, SelectedTeam.TeamID, true, true);
                        }

                        IsShotOnGoalVisible = Visibility.Visible;
                        IsLeftRightHeadedVisible = Visibility.Visible;
                        IsOtherTeamGoalieVisible = Visibility.Visible;
                        IsOtherTeamGoalieDropDownVisible = Visibility.Visible;
                        OtherTeamGoaliePlayerList = DAL.Instance().GetPlayersPhyiscalAndTeamRosterInfo(Game.Game.GameID, NonShotTeamID, true, true);
                        SetOtherTeamGoalieDefault();
                    }

                    else if (_selectedStatCategoryDescription.StatDescriptionName == AppResources.ForGoal) //Corner Kick, Direct Free Kick
                    {
                        ShotOnGoalChecked = true;
                        ShotNotOnGoalChecked = false;

                        AssistOrBlockLabelCaption = "";
                        IsAssistOrBlockLabelVisible = Visibility.Collapsed;
                        IsAssistOrBlockPlayerDropDownVisible = Visibility.Collapsed;
                        IsShotOnGoalVisible = Visibility.Visible;
                        IsLeftRightHeadedVisible = Visibility.Visible;
                        IsOtherTeamGoalieVisible = Visibility.Visible;
                        IsOtherTeamGoalieDropDownVisible = Visibility.Visible;
                        OtherTeamGoaliePlayerList = DAL.Instance().GetPlayersPhyiscalAndTeamRosterInfo(Game.Game.GameID, NonShotTeamID, true, true);
                        SetOtherTeamGoalieDefault();
                    }

                    else
                    {
                        IsShotOnGoalVisible = Visibility.Collapsed;
                        AssistOrBlockLabelCaption = "";
                        IsLeftRightHeadedVisible = Visibility.Collapsed;
                        IsAssistOrBlockLabelVisible = Visibility.Collapsed;
                        IsAssistOrBlockPlayerDropDownVisible = Visibility.Collapsed;
                        IsOtherTeamGoalieVisible = Visibility.Collapsed;
                        IsOtherTeamGoalieDropDownVisible = Visibility.Collapsed;
                    }
                }
                NotifyPropertyChanged("SelectedStatCategoryDescription");
            }
        }

        private GameModel _game;
        public GameModel Game
        {
            get { return _game; }
            set { _game = value; NotifyPropertyChanged("Game"); }
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

        #region "Shot Details Properties"

        private ObservableCollection<TeamRosterModel> _assistOrBlockPlayerList;
        public ObservableCollection<TeamRosterModel> AssistOrBlockPlayerList
        {
            get { return _assistOrBlockPlayerList; }
            set { _assistOrBlockPlayerList = value; NotifyPropertyChanged("AssistOrBlockPlayerList"); }
        }

        private ObservableCollection<TeamRosterModel> _otherTeamGoaliePlayerList;
        public ObservableCollection<TeamRosterModel> OtherTeamGoaliePlayerList
        {
            get { return _otherTeamGoaliePlayerList; }
            set { _otherTeamGoaliePlayerList = value; NotifyPropertyChanged("OtherTeamGoaliePlayerList"); }
        }

        private ObservableCollection<TeamRosterModel> _subPlayerInList;
        public ObservableCollection<TeamRosterModel> SubPlayerInList
        {
            get { return _subPlayerInList; }
            set { _subPlayerInList = value; NotifyPropertyChanged("SubPlayerInList"); }
        }

        private ObservableCollection<TeamRosterModel> _subPlayerOutList;
        public ObservableCollection<TeamRosterModel> SubPlayerOutList
        {
            get { return _subPlayerOutList; }
            set { _subPlayerOutList = value; NotifyPropertyChanged("SubPlayerOutList"); }
        }

        private TeamRosterModel _selectedAssistOrBlockPlayer;
        public TeamRosterModel SelectedAssistOrBlockPlayer
        {
            get { return _selectedAssistOrBlockPlayer; }
            set { _selectedAssistOrBlockPlayer = value; NotifyPropertyChanged("SelectedAssistOrBlockPlayer"); }
        }

        private TeamRosterModel _selectedOtherTeamGoaliePlayer;
        public TeamRosterModel SelectedOtherTeamGoaliePlayer
        {
            get { return _selectedOtherTeamGoaliePlayer; }
            set { _selectedOtherTeamGoaliePlayer = value; NotifyPropertyChanged("SelectedOtherTeamGoaliePlayer"); }
        }

        private Visibility _isShotOnGoalVisible;
        public Visibility IsShotOnGoalVisible
        {
            get { return _isShotOnGoalVisible; }
            set { _isShotOnGoalVisible = value; NotifyPropertyChanged("IsShotOnGoalVisible"); }
        }

        private Visibility _isOtherTeamGoalieVisible;
        public Visibility IsOtherTeamGoalieVisible
        {
            get { return _isOtherTeamGoalieVisible; }
            set { _isOtherTeamGoalieVisible = value; NotifyPropertyChanged("IsOtherTeamGoalieVisible"); }
        }

        private string _assistOrBlockLabelCaption;
        public string AssistOrBlockLabelCaption
        {
            get { return _assistOrBlockLabelCaption; }
            set { _assistOrBlockLabelCaption = value; NotifyPropertyChanged("AssistOrBlockLabelCaption"); }
        }

        private Visibility _isAssistOrBlockLabelVisible;
        public Visibility IsAssistOrBlockLabelVisible
        {
            get { return _isAssistOrBlockLabelVisible; }
            set { _isAssistOrBlockLabelVisible = value; NotifyPropertyChanged("IsAssistOrBlockLabelVisible"); }
        }

        private Visibility _isAssistOrBlockPlayerDropDownVisible;
        public Visibility IsAssistOrBlockPlayerDropDownVisible
        {
            get { return _isAssistOrBlockPlayerDropDownVisible; }
            set { _isAssistOrBlockPlayerDropDownVisible = value; NotifyPropertyChanged("IsAssistOrBlockPlayerDropDownVisible"); }
        }

        private Visibility _isOtherTeamGoalieDropDownVisible;
        public Visibility IsOtherTeamGoalieDropDownVisible
        {
            get { return _isOtherTeamGoalieDropDownVisible; }
            set { _isOtherTeamGoalieDropDownVisible = value; NotifyPropertyChanged("IsOtherTeamGoalieDropDownVisible"); }
        }

        private Visibility _isLeftRightHeadedVisible;
        public Visibility IsLeftRightHeadedVisible
        {
            get { return _isLeftRightHeadedVisible; }
            set { _isLeftRightHeadedVisible = value; NotifyPropertyChanged("IsLeftRightHeadedVisible"); }
        }

        private Visibility _subInputVisibility;
        public Visibility SubInputVisibility
        {
            get { return _subInputVisibility; }
            set { _subInputVisibility = value; NotifyPropertyChanged("SubInputVisibility"); }
        }

        private bool _playerEnabled;
        public bool PlayerEnabled
        {
            get { return _playerEnabled; }
            set { _playerEnabled = value; NotifyPropertyChanged("PlayerEnabled"); }
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

        private int _nonShotTeamID;
        public int NonShotTeamID
        {
            get { return _nonShotTeamID; }
            set { _nonShotTeamID = value; NotifyPropertyChanged("NonShotTeamID"); }
        }

        private int? _otherTeamGoalieID;
        public int? OtherTeamGoalieID
        {
            get { return _otherTeamGoalieID; }
            set { _otherTeamGoalieID = value; NotifyPropertyChanged("OtherTeamGoalieID"); }
        }

        private int _playID;
        public int PlayID
        {
            get { return _playID; }
            set { _playID = value; NotifyPropertyChanged("PlayID"); }
        }

        #endregion "Shot Details Properties"

        #region Commands

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
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Help_Buttons.xaml?screenId=" + Enums.Screen.PlayDetails, UriKind.Relative));
        }

        public void Initialize(int gameID, int playID)
        {
            PlayID = playID;
            Game = DAL.Instance().GetGame(gameID);

            PopulateTeamsList();
            PopulatePeriodsList();
            PopulateStatCategories();

            if (PlayID == 0)
            {
                SetDefaultValues();
            }
            else
            {
                PlayDetails = DAL.Instance().GetPlay(gameID, playID);

                SetSelectedValues();
            }
        }

        private void SetDefaultValues()
        {
            RightFootShotChecked = true;
            ShotNotOnGoalChecked = true;

            //Set team
            foreach (var item in TeamsList)
            {
                SelectedTeam = item;
                break;
            }

            //Set player
            foreach (var item in PlayerList)
            {
                SelectedPlayer = item;
                break;
            }

            //Set Period to current period
            foreach (var item in PeriodsList)
            {
                if (Game.Game.CurrentPeriod.ToString() == item)
                    SelectedPeriod = item;
                break;
            }

            //Set Category to default to Shot
            foreach (var item in StatCategories)
            {
                if (item.StatCategory.StatCategoryName == AppResources.Shot)
                {
                    SelectedStatCategory = item;
                    break;
                }
            }

            //Set Description to default to Good
            foreach (var item in StatCategoryDescriptions)
            {
                if (item.StatDescriptionName == AppResources.Excellent)
                {
                    SelectedStatCategoryDescription = item;
                    break;
                }
            }

            SetClockTimeSpan(Game.Game.CurrentClock);
        }

        private void PopulateTeamsList()
        {
            TeamsList.Add(Game.AwayTeam);
            TeamsList.Add(Game.HomeTeam);
        }

        private void PopulatePeriodsList()
        {
            if (Game.Game.Periods == 2)
            {
                PeriodsList.Add("1");
                PeriodsList.Add("2");
            }
            else
            {
                PeriodsList.Add("1");
                PeriodsList.Add("2");
                PeriodsList.Add("3");
                PeriodsList.Add("4");
            }
        }

        private void PopulateStatCategories()
        {
            ObservableCollection<StatCategoryModel> statList = new ObservableCollection<StatCategoryModel>();

            statList = DAL.Instance().GetVisibleStats(true, true);

            foreach (var item in statList)
            {
                //If this is the paid version then all stats are available, otherwise for the free version only the Active stats are (i.e. shots only)
                if ((App.DoesUserHaveAbilityToTrackAllStats() == true) || (App.DoesUserHaveAbilityToTrackAllStats() == false) && (item.StatCategory.Active == "Y"))
                {
                    Common.Instance().GlobalizeStatCatAndDescription(item);
                    StatCategories.Add(item);
                }
            }
        }

        private void FilterStatCategoryDescriptionsMasterList(string statCategoryName)
        {
            StatCategoryDescriptions = new ObservableCollection<StatDescription>();
            foreach (var statCat in App.gStatsList)
            {
                if (statCat.StatCategory.StatCategoryName == statCategoryName)
                {
                    if ((statCategoryName == AppResources.Shot) || (statCategoryName == AppResources.ShootoutKick) || (statCategoryName == AppResources.PenaltyKick))
                    {
                        StatDescription statDescMiss = new StatDescription { StatDescriptionName = AppResources.Miss, StatDescriptionID = 1 };
                        StatCategoryDescriptions.Add(statDescMiss);

                        StatDescription statDescHitPost = new StatDescription { StatDescriptionName = AppResources.HitPost, StatDescriptionID = 2 };
                        StatCategoryDescriptions.Add(statDescHitPost);

                        StatDescription statDescBlocked = new StatDescription { StatDescriptionName = AppResources.Blocked, StatDescriptionID = 3 };
                        StatCategoryDescriptions.Add(statDescBlocked);

                        StatDescription statDescGoal = new StatDescription { StatDescriptionName = AppResources.Goal, StatDescriptionID = 4 };
                        StatCategoryDescriptions.Add(statDescGoal);
                    }
                    else
                    {
                        if (statCat.Descriptions != null)
                        {
                            foreach (var item in statCat.Descriptions)
                            {
                                StatCategoryDescriptions.Add(item.StatDescription);
                            }
                        }
                    }

                    //set default value in dropdown
                    foreach (var item in StatCategoryDescriptions)
                    {
                        SelectedStatCategoryDescription = item;
                        break;
                    }
                    break;
                }
            }
        }

        private void SetSelectedValues()
        {
            //set team
            foreach (var item in TeamsList)
            {
                if (PlayDetails.Play.TeamID == item.TeamID)
                {
                    SelectedTeam = item;
                    break;
                }
            }

            //set player
            foreach (var item in PlayerList)
            {
                if (PlayDetails.Play.Player1ID == item.Player.PlayerID)
                {
                    SelectedPlayer = item;
                    break;
                }
            }

            //set Period
            foreach (var item in PeriodsList)
            {
                if (PlayDetails.Play.Period.ToString() == item)
                {
                    SelectedPeriod = item;
                    break;
                }
            }

            //set category name
            foreach (var item in StatCategories)
            {
                if (PlayDetails.Play.StatCategoryID == item.StatCategory.StatCategoryID)
                {
                    SelectedStatCategory = item;
                    break;
                }
            }

            //if we were not able to find the category then this is a category we do not supported editing for (i.e. move play)
            if (SelectedStatCategory == null)
            {
                MessageBox.Show(AppResources.NotASupportedPlayTypeForPBPEdit, _messageBoxTitle, MessageBoxButton.OK);
                CancelButtonClicked = true;
                (Application.Current.RootVisual as Frame).GoBack();
                return;
            }

            //set category description
            foreach (var item in StatCategoryDescriptions)
            {
                if (PlayDetails.Play.StatDescriptionID == item.StatDescriptionID)
                {
                    SelectedStatCategoryDescription = item;
                    break;
                }
            }

            SetClockTimeSpan(PlayDetails.Play.GameTime);

            //set other team goalie
            OtherTeamGoalieID = PlayDetails.Play.OtherTeamGoalieID;
            if (Common.Instance().IsThisAShotPlay(PlayDetails.Play))
            {
                SetSelectedValuesForShotPlays();
            }
        }

        //set clock, convert to TimeSpan      
        private void SetClockTimeSpan(string clockTime)
        {
            //set clock   
            //Get Minutes and Seconds, convert to TimeSpan
            if (clockTime == null)
            {
                ClockTimeSpan = TimeSpan.FromSeconds(0) + TimeSpan.FromMinutes(0);
            }
            else
            {
                string[] mmss = clockTime.Replace(" ", string.Empty).Split(':');
                int minutes = 0;
                int seconds = 0;

                if (mmss.Count() == 2)
                {
                    minutes = Convert.ToInt32(mmss[0]);
                    seconds = Convert.ToInt32(mmss[1]);
                }
                if (mmss.Count() == 1)
                {
                    seconds = Convert.ToInt32(mmss[0]);
                }

                ClockTimeSpan = TimeSpan.FromSeconds(seconds) + TimeSpan.FromMinutes(minutes);
            }
        }

        private void SetSelectedValuesForShotPlays()
        {
            //set left/right/headed selected value
            string shotTypeDesc;
            shotTypeDesc = DAL.Instance().GetStatDescriptionNameById(PlayDetails.Play.ShotTypeID);

            if (shotTypeDesc == "Left")
            {
                LeftFootShotChecked = true;
                RightFootShotChecked = false;
                HeadedShotChecked = false;
            }
            else if (shotTypeDesc == "Right")
            {
                LeftFootShotChecked = false;
                RightFootShotChecked = true;
                HeadedShotChecked = false;
            }
            else if (shotTypeDesc == "Headed")
            {
                LeftFootShotChecked = false;
                RightFootShotChecked = false;
                HeadedShotChecked = true;
            }

            //set shot on goal
            if (PlayDetails.Play.ShotOnGoal == "Y")
            {
                ShotOnGoalChecked = true;
                ShotNotOnGoalChecked = false;
            }
            else
            {
                ShotOnGoalChecked = false;
                ShotNotOnGoalChecked = true;
            }

            //set assist or blocked player
            string playDesc;
            playDesc = DAL.Instance().GetStatDescriptionNameById(PlayDetails.Play.StatDescriptionID);

            //Not all goal plays have an assist (i.e. penalty kicks, direct free kicks, shootout kicks) so need to make sure AssistOrBlockPlayerList is populated
            if (AssistOrBlockPlayerList != null)
            {
                if (playDesc == "Goal" | playDesc == "Blocked")
                {
                    foreach (var item in AssistOrBlockPlayerList)
                    {
                        if ((PlayDetails.Play.AssistID == item.Player.PlayerID) || (PlayDetails.Play.ShotBlockedByID == item.Player.PlayerID))
                        {
                            SelectedAssistOrBlockPlayer = item;
                            break;
                        }
                    }

                    //if there is no assist or blocked by then set to NONE player (-2)
                    if (SelectedAssistOrBlockPlayer == null)
                    {
                        foreach (var item in AssistOrBlockPlayerList)
                        {
                            if (item.Player.PlayerID == -2)
                            {
                                SelectedAssistOrBlockPlayer = item;
                                break;
                            }
                        }
                    }
                }
            }
            //set other team goalie
            //On GM every play has the other team goalie property set.  So we save it here to OtherTeamGoalieID property
            //But only a goal play on this screen requires the other teams goalie drop down to be displayed 
            //Therefore is this the collection is empty this is not a goal play and we do not need to set the SelectedOtherTeamGoaliePlayer

            OtherTeamGoalieID = PlayDetails.Play.OtherTeamGoalieID;
            if (OtherTeamGoaliePlayerList != null)
            {
                foreach (var item in OtherTeamGoaliePlayerList)
                {
                    if (PlayDetails.Play.OtherTeamGoalieID == item.Player.PlayerID)
                    {
                        SelectedOtherTeamGoaliePlayer = item;
                        break;
                    }
                }
            }
        }


        public void OkClicked()
        {

            OkButtonClicked = true;

            SaveToDatabase();
            (Application.Current.RootVisual as Frame).GoBack();
        }

        //If an edit need to delete old play and then insert new play, if only an add then insert new play   
        public void SaveToDatabase()
        {
            string statCategoryName;

            if (ValidateScreen() == true)
            {
                Play play = new Play();

                if (PlayID != 0)
                {
                    Play playDetails = new Play();
                    playDetails = BaseTableDataAccess.Instance().GetPlay(Game.Game.GameID, PlayID);
                    DAL.Instance().BackOutStatsForAPlay(Game.Game.GameID, playDetails);

                    //Since this is an edit, we need to reuse the same playid
                    play.PlayID = PlayID;
                }

                play.GameID = Game.Game.GameID;
                play.TeamID = SelectedTeam.TeamID;
                play.StatCategoryID = SelectedStatCategory.StatCategory.StatCategoryID;

                if (SelectedStatCategoryDescription != null)
                {
                    play.StatDescriptionID = SelectedStatCategoryDescription.StatDescriptionID;
                }

                if (play.StatCategoryID == 22)  //Substitution (22)
                {
                    play.Player1ID = SelectedPlayerIn.Player.PlayerID;
                    play.Player2ID = SelectedPlayerOut.Player.PlayerID;
                }
                else
                {
                    play.Player1ID = SelectedPlayer.Player.PlayerID;
                    play.Player2ID = null;
                }

                play.Period = Convert.ToInt32(SelectedPeriod);
                play.PlayerPosition = "";
                play.GameTime = Clock;
                play.OtherTeamGoalieID = GetOtherTeamGoalieIdValue();

                //Check to see if this is a shot type play
                statCategoryName = DAL.Instance().GetStatCategoryNameById(play.StatCategoryID);
                if ((statCategoryName == "Shot") || (statCategoryName == "Shootout Kick") || (statCategoryName == "Penalty Kick") ||
                    (statCategoryName == "Direct Free Kick") || (statCategoryName == "Corner Kick"))
                {
                    play.AssistID = GetAssistIdValue();
                    play.ShotBlockedByID = GetShotBlockedByIdValue();
                    play.ShotOnGoal = GetShotOnGoalValue();
                    play.ShotTypeID = GetShotTypeIdValue();
                }
                else //Not a shot play
                {
                    play.AssistID = null;
                    play.ShotBlockedByID = null;
                    play.ShotOnGoal = "N";
                    play.ShotTypeID = null;
                }

                if (statCategoryName == "Substitution")
                {
                    DAL.Instance().SaveSubstitutionPlay("PBPSCREEN",play.GameID, play.TeamID, play.Period, play.GameTime, play.Player1ID, play.GMPlayer1PositionID, play.Player2ID, play.GMPlayer2PositionID);
                }
                else
                {
                    DAL.Instance().UpsertPlay(play, "PBPSCREEN");
                }
            }
        }

        public bool ValidateScreen()
        {
            bool rtnValue = true;
            int elapsedTimeInGameInSeconds;

            try
            {
                elapsedTimeInGameInSeconds = StatCalculationsModule.CalculateTimeElapsedInGameInSeconds(Game.Game, Game.Game.CurrentPeriod, Game.Game.CurrentClock);

                if (Clock == string.Empty || Clock == null)
                {
                    MessageBox.Show(AppResources.ClockNotEntered, _messageBoxTitle, MessageBoxButton.OK);
                    rtnValue = false;
                }
                else
                {
                    //Make sure this is not a period/clock time that is in the future, that is what Game Manager is for.
                    int elapsedTimeOfThisPlayInSeconds = StatCalculationsModule.CalculateTimeElapsedInGameInSeconds(Game.Game, Convert.ToInt32(SelectedPeriod), Clock);

                    if (elapsedTimeOfThisPlayInSeconds > elapsedTimeInGameInSeconds)
                    {
                        MessageBox.Show(AppResources.PeriodClockValueInFuture, _messageBoxTitle, MessageBoxButton.OK);
                        rtnValue = false;
                    }
                }

                return rtnValue;
            }
            catch (Exception ex)
            {
                return rtnValue;
            }
        }

        private void CancelClicked()
        {
            CancelButtonClicked = true;
            (Application.Current.RootVisual as Frame).GoBack();
        }

        private void PopulateSubInAndOutDropDowns()
        {
            if (Clock != null)
            {
                if (SelectedStatCategory.StatCategory.StatCategoryName == AppResources.Substitution)
                {
                    int elapsedTimeInGameInSeconds = StatCalculationsModule.CalculateTimeElapsedInGameInSeconds(Game.Game, Convert.ToInt32(SelectedPeriod), Clock);

                    //The players IN the game at this time should appear in the dropdown PLAYER OUT (i.e. if they are in the game, they can come OUT) (vice versa)
                    SubPlayerOutList = DAL.Instance().GetPlayersInAndOutAtAGivenPointInGame(Game.Game.GameID, SelectedTeam.TeamID, elapsedTimeInGameInSeconds, "IN");
                    SubPlayerInList = DAL.Instance().GetPlayersInAndOutAtAGivenPointInGame(Game.Game.GameID, SelectedTeam.TeamID, elapsedTimeInGameInSeconds, "OUT");
                }
            }
            else
            {
                SubPlayerInList = new ObservableCollection<TeamRosterModel>();
                SubPlayerOutList = new ObservableCollection<TeamRosterModel>();
            }

            //Set Sub Player In
            foreach (var item in SubPlayerInList)
            {
                SelectedPlayerIn = item;
                break;
            }

            //Set Sub Player Out
            foreach (var item in SubPlayerOutList)
            {
                SelectedPlayerOut = item;
                break;
            }
        }

        #endregion "Methods"

        #region "Shot Details Methods"

        private void SetBlockerDefault()
        {
            //set blocker to the other teams goalie as the default
            foreach (var item in AssistOrBlockPlayerList)
            {
                if (OtherTeamGoalieID == item.Player.PlayerID)
                {
                    SelectedAssistOrBlockPlayer = item;
                }
            }

            //If no other team goalie then set to player NONE
            if (SelectedAssistOrBlockPlayer == null)
            {
                foreach (var item in AssistOrBlockPlayerList)
                {
                    if (item.RosterDisplayText == AppResources.None)
                    {
                        SelectedAssistOrBlockPlayer = item;
                    }
                }
            }
        }

        private void SetOtherTeamGoalieDefault()
        {
            //set blocker to the other teams goalie as the default
            foreach (var item in OtherTeamGoaliePlayerList)
            {
                if (OtherTeamGoalieID == item.Player.PlayerID)
                {
                    SelectedOtherTeamGoaliePlayer = item;
                }
            }

            //If no other team goalie then set to player NONE
            if (SelectedOtherTeamGoaliePlayer == null)
            {
                foreach (var item in OtherTeamGoaliePlayerList)
                {
                    if (item.RosterDisplayText == AppResources.None)
                    {
                        SelectedOtherTeamGoaliePlayer = item;
                    }
                }
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
                    shotTypeDesc = "Left";
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

        private int? GetShotBlockedByIdValue()
        {
            int? shotBlockedByID = null;

            try
            {
                if (SelectedStatCategoryDescription.StatDescriptionName == "Blocked")
                {
                    shotBlockedByID = SelectedAssistOrBlockPlayer.Player.PlayerID;
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
                if (SelectedAssistOrBlockPlayer != null)
                {
                    if (SelectedAssistOrBlockPlayer.Player.PlayerID != null)
                    {
                        if (SelectedStatCategoryDescription.StatDescriptionName == "Goal")
                        {
                            shotAssistID = SelectedAssistOrBlockPlayer.Player.PlayerID;
                        }
                    }
                }

                return shotAssistID;
            }
            catch (Exception)
            {
                return shotAssistID;
            }
        }

        private int? GetOtherTeamGoalieIdValue()
        {
            int? otherTeamGoalieID = null;

            try
            {
                //If OtherTeamGoalieID property was set from the original play, start with that as the value
                otherTeamGoalieID = OtherTeamGoalieID;

                //If this was a play that involved setting the other team goalie (i.e. a goal play) then update the other team goalie
                if (SelectedOtherTeamGoaliePlayer != null)
                {
                    if (SelectedOtherTeamGoaliePlayer.Player.PlayerID == -2)
                    {
                        otherTeamGoalieID = null;
                    }
                    else
                    {
                        otherTeamGoalieID = SelectedOtherTeamGoaliePlayer.Player.PlayerID;
                    }
                }
                return otherTeamGoalieID;
            }
            catch (Exception ex)
            {
                return otherTeamGoalieID;
            }
        }

        #endregion "Shot Details Methods"

    }
}
