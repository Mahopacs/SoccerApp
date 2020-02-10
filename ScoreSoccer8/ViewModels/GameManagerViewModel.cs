using Coding4Fun.Toolkit.Controls;
using ScoreSoccer8.Classes;
using ScoreSoccer8.Cloud;
using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.Resources;
using ScoreSoccer8.Utilities;
using ScoreSoccer8.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ScoreSoccer8.ViewModels
{
    public enum StartingFormation
    {
        Eight,
        Eleven
    }

    public class TodoItem
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public bool Complete { get; set; }
    }

    public class GameManagerViewModel : Notification, IDisposable
    {
        private string scorePicTemplate = "/Assets/flipNumbers/black/{0}.png";

        double _screenWidth = System.Windows.Application.Current.Host.Content.ActualWidth;
        double _screenHeight = System.Windows.Application.Current.Host.Content.ActualHeight;
        private int _homeTeamID;
        private int _visitorTeamID;

        private StartingFormation _startingFormation;

        //Players for subs
        private TeamRosterModel _homePlayer1ToSwap;
        private TeamRosterModel _homePlayer2ToSwap;
        private TeamRosterModel _visitorPlayer1ToSwap;
        private TeamRosterModel _visitorPlayer2ToSwap;
        PopupClock _clockPopup = new Views.PopupClock();

        private int _uniquePlayerIdentifier = 1;

        private string BLANK_PLAYER = "Empty";

        public GameManagerViewModel()
        {
            TestMobileService();

            HeightOfScreen = _screenHeight;
            WidthOfScreen = _widthOfScreen;

            MinWidthOfOneListBox = HeightOfScreen / 12.5;

            StatsMode = true;

            //Home
            RightCol1 = new ObservableCollection<TeamRosterModel>();
            RightCol2 = new ObservableCollection<TeamRosterModel>();
            RightCol3 = new ObservableCollection<TeamRosterModel>();
            RightUnknownPlayer = new ObservableCollection<TeamRosterModel>();
            RightSpaceHolder = new ObservableCollection<TeamRosterModel>();
            RightGoalie = new ObservableCollection<TeamRosterModel>();
            RightSubs = new ObservableCollection<TeamRosterModel>();

            //Visitor
            LeftCol1 = new ObservableCollection<TeamRosterModel>();
            LeftCol2 = new ObservableCollection<TeamRosterModel>();
            LeftCol3 = new ObservableCollection<TeamRosterModel>();
            LeftUnknownPlayer = new ObservableCollection<TeamRosterModel>();
            LeftSpaceHolder = new ObservableCollection<TeamRosterModel>();
            LeftGoalie = new ObservableCollection<TeamRosterModel>();
            LeftSubs = new ObservableCollection<TeamRosterModel>();

            _clockPopup.CloseOrHidePopup += clockPopup_CloseOrHidePopup;
            _clockPopup.Opacity = .8;
            _clockAdjusterPopup.Child = _clockPopup;

            Clock.ClockHasChanged += Clock_ClockHasChanged;
        }

        private async void TestMobileService()
        {
            //TodoItem item = new TodoItem { Text = "Awesome item", Complete = false };
            //await App.MobileService.GetTable<TodoItem>().InsertAsync(item);
        }

        int clockTickCounter = 0;

        private void Clock_ClockHasChanged(object sender, Clock.ClockEventArgs e)
        {
            ClockTime = e.FormattedTime;

            if (Game.Game.ClockUpOrDown.ToUpper().Equals("DOWN"))
            {
                if (ClockTime.Seconds == 0 && ClockTime.Minutes == 0 && ClockTime.Hours == 0)
                {
                    AdvancePeriod(true);
                }
            }

            if (clockTickCounter == 5)
            {
                Task.Factory.StartNew(() => DAL.Instance().UpdateGameCurrentPeriodAndCurrentClock(Game.Game.GameID, Game.Game.CurrentPeriod, ClockTimeUi));
                clockTickCounter = 0;
            }

            clockTickCounter++;
        }

        private int GetSecondsFromCurrentClockString(string clock)
        {
            int seconds = 0;

            string[] mmss = clock.Split(':');
            int mins = Convert.ToInt32(mmss[0]);
            int secs = Convert.ToInt32(mmss[1]);

            int minsToSecs = mins * 60;

            seconds = minsToSecs + secs;

            return seconds;
        }





        private string getScore1(int score)
        {
            return getScore(score, 0, 1);
        }

        private string getScore2(int score)
        {
            return getScore(score, 1, 1);
        }

        private string getScore(int score, int start, int stop)
        {
            string toReturn = string.Format(scorePicTemplate, 0);

            string temp = score.ToString().PadLeft(2, '0');

            int j;
            bool result = Int32.TryParse(temp.Substring(start, stop), out j);

            if (result)
            {
                toReturn = string.Format(scorePicTemplate, j);
            }

            return toReturn;
        }

        private string _rightScore1_picPath;
        public string RightScore1_picPath
        {
            get
            {
                string temp = getScore1(RightTeamScore);
                return temp;
            }
            set { _rightScore1_picPath = value; NotifyPropertyChanged("RightScore1_picPath"); }
        }

        private string _rightScore2_picPath;
        public string RightScore2_picPath
        {
            get
            {
                string temp = getScore2(RightTeamScore);
                return temp;
            }
            set { _rightScore2_picPath = value; NotifyPropertyChanged("RightScore2_picPath"); }
        }


        private string _leftScore1_picPath;
        public string LeftScore1_picPath
        {
            get
            {
                string temp = getScore1(LeftTeamScore);
                return temp;
            }
            set { _leftScore1_picPath = value; NotifyPropertyChanged("LeftScore1_picPath"); }
        }

        private string _leftScore2_picPath;
        public string LeftScore2_picPath
        {
            get
            {
                string temp = getScore2(LeftTeamScore);
                return temp;
            }
            set { _leftScore2_picPath = value; NotifyPropertyChanged("LeftScore2_picPath"); }
        }

        /// <summary>
        /// Initialize the Game.  The event ID is being passed
        /// For players, the screen is laid out as follows:
        ///                                 -
        ///             Visitor             -                 Home
        ///                                 -
        /// 
        /// Unkown4  VCol3    VCol2    VCol1 -   HCol1    HCol2    HCol3    Unkown4
        ///                                 -
        /// Goalie                          -                              Goalie
        ///                                 -
        ///                                 
        /// All of the controls on the screen so far are dynamic List Boxes, evenly spaced depending on how many players are added.
        /// The Goalie and Unknown Listboxes should only EVER have 1 item in them.  Having them dynamic allows us to have a consistent methodology to
        /// code against.
        /// 
        /// Row Will be 0 based, Column is base 1.
        /// Goalie Column 4
        /// Sub Column 5
        /// </summary>
        /// <param name="paramValue"></param>
        internal void Initialize(int eventID)
        {
            //We want to load the Game Object Every time we come into initialize.
            Game = DAL.Instance().GetGame(eventID);
            _visitorTeamID = _game.Game.AwayTeamID;
            _homeTeamID = _game.Game.HomeTeamID;

            GMEnabled = true;

            //ONLY Do this the first time the game is opened.
            if (Game.Game.GameStatus.ToUpper().Trim().Replace(" ", "").Equals("NOTSTARTED"))
            {
                Common.Instance().InitiliazeEventRoster(eventID);
            }

            //IF THE GAME IS LOADED ALREADY FOR THE FIRST TIME, RELOADGAME.
            //The only way the game would have already been loaded is if there are players who have a game position id
            //MARK TO DO
            if (DAL.Instance().GetPlayersPhyiscalAndTeamRosterInfo(eventID).Where(x => x.EventRoster.GMPlayerPositionID != string.Empty).Count() > 0)
            {
                ReloadGameAfterItHasAlreadyBeenLoaded(eventID);
            }
            else
            {
                LoadForTheFirstTime();
            }
        }

        #region firsttimeload

        private void LoadForTheFirstTime()
        {
            InitializeClock();
            LoadStartingFormation(_game);
            //Needs to be called all the time when initializing
            LoadTeamNamesAndScores(Game.Game.GameID);
            InitializePlayersForTheFirstTime();

            //Moving to a different thread.  
            Task.Factory.StartNew(() => SaveGmFieldPositionForAllPlayers());
        }

        private void SaveGmFieldPositionForAllPlayers()
        {
            //Debug.WriteLine("Starting to save roster positions" + DateTime.Now);

            //After everything is loaded, we need to loop through all the players
            //and add their GM Position ID to the event roster for them.
            RightGoalie.ToList().ForEach(x => UpdateGamePositionID(_homeTeamID, x.Player.PlayerID, x.FieldColumn, x.FieldRow));

            RightSubs.ToList().ForEach(x =>
            {
                if (!x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper()))
                {
                    UpdateGamePositionID(_homeTeamID, x.Player.PlayerID, x.FieldColumn, x.FieldRow);
                }
            });
            RightCol3.ToList().ForEach(x =>
            {
                if (!x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper()))
                {
                    UpdateGamePositionID(_homeTeamID, x.Player.PlayerID, x.FieldColumn, x.FieldRow);
                }
            });

            RightCol2.ToList().ForEach(x =>
            {
                if (!x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper()))
                {
                    UpdateGamePositionID(_homeTeamID, x.Player.PlayerID, x.FieldColumn, x.FieldRow);
                }
            });

            RightCol1.ToList().ForEach(x =>
            {
                if (!x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper()))
                {
                    UpdateGamePositionID(_homeTeamID, x.Player.PlayerID, x.FieldColumn, x.FieldRow);
                }
            });

            LeftGoalie.ToList().ForEach(x => UpdateGamePositionID(_visitorTeamID, x.Player.PlayerID, x.FieldColumn, x.FieldRow));

            LeftSubs.ToList().ForEach(x =>
            {
                if (!x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper()))
                {
                    UpdateGamePositionID(_visitorTeamID, x.Player.PlayerID, x.FieldColumn, x.FieldRow);
                }
            });

            LeftCol3.ToList().ForEach(x =>
            {
                if (!x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper()))
                {
                    UpdateGamePositionID(_visitorTeamID, x.Player.PlayerID, x.FieldColumn, x.FieldRow);
                }
            });

            LeftCol2.ToList().ForEach(x =>
            {
                if (!x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper()))
                {
                    UpdateGamePositionID(_visitorTeamID, x.Player.PlayerID, x.FieldColumn, x.FieldRow);
                }
            });

            LeftCol1.ToList().ForEach(x =>
            {
                if (!x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper()))
                {
                    UpdateGamePositionID(_visitorTeamID, x.Player.PlayerID, x.FieldColumn, x.FieldRow);
                }
            });

            PrintPlayers();

            //Debug.WriteLine("Done saving roster positions" + DateTime.Now);
        }

        private void PrintPlayers()
        {
            var homePlayers = new List<TeamRosterModel>(DAL.Instance().GetPlayersPhyiscalAndTeamRosterInfo(Game.Game.GameID).Where(x => x.Team.TeamID == _homeTeamID)).OrderByDescending(x => x.EventRoster.Starter).ToList();
            var visitorPlayers = new List<TeamRosterModel>(DAL.Instance().GetPlayersPhyiscalAndTeamRosterInfo(Game.Game.GameID).Where(x => x.Team.TeamID == _visitorTeamID)).OrderByDescending(x => x.EventRoster.Starter).ToList();


            //foreach (var row in homePlayers)
            //{
            //    Debug.WriteLine(row.Team.TeamName + " " +  row.Player.FirstName + " " + row.Player.LastName + " : " + row.EventRoster.GMPlayerPositionID);
            //}

            //foreach (var row in homePlayers)
            //{
            //    Debug.WriteLine(row.Team.TeamName + " " + row.Player.FirstName + " " + row.Player.LastName + " : " + row.EventRoster.GMPlayerPositionID);
            //}

            //var homeDups = homePlayers.GroupBy(item => item.EventRoster.GMPlayerPositionID)
            //    .SelectMany(grp => grp.Skip(1));

            //if (homeDups.Count() > 0)
            //{
            //    Debug.WriteLine("Home Duplicate Found: ");
            //    foreach (var row in homeDups)
            //    {
            //        Debug.WriteLine(row.Team.TeamName + " " + row.Player.FirstName + " " + row.Player.LastName + " : " + row.EventRoster.GMPlayerPositionID);
            //    }
            //}

            //var visDups = visitorPlayers.GroupBy(item => item.EventRoster.GMPlayerPositionID)
            //    .SelectMany(grp => grp.Skip(1));

            //if (visDups.Count() > 0)
            //{
            //    Debug.WriteLine("Visitor Duplicate Found: ");
            //    foreach (var row in visDups)
            //    {
            //        Debug.WriteLine(row.Team.TeamName + " " + row.Player.FirstName + " " + row.Player.LastName + " : " + row.EventRoster.GMPlayerPositionID);
            //    }
            //}
        }

        private void InitializePlayersForTheFirstTime()
        {
            string homeImagePath = DAL.Instance().GetJerseyByJerseyId(_game.HomeTeam.JerseyID).ImagePath;
            string awayImagePath = DAL.Instance().GetJerseyByJerseyId(_game.AwayTeam.JerseyID).ImagePath;

            AllHomePlayers = new List<TeamRosterModel>(DAL.Instance().GetPlayersPhyiscalAndTeamRosterInfo(Game.Game.GameID).Where(x => x.Team.TeamID == _homeTeamID)).OrderByDescending(x => x.EventRoster.Starter).ToList();
            AllVisitorPlayers = new List<TeamRosterModel>(DAL.Instance().GetPlayersPhyiscalAndTeamRosterInfo(Game.Game.GameID).Where(x => x.Team.TeamID == _visitorTeamID)).OrderByDescending(x => x.EventRoster.Starter).ToList();

            AllHomePlayers.ForEach(x =>
            {
                x.UniqueIdentifier = _uniquePlayerIdentifier;
                _uniquePlayerIdentifier += 1;
            });

            AllVisitorPlayers.ForEach(x =>
            {
                x.UniqueIdentifier = _uniquePlayerIdentifier;
                _uniquePlayerIdentifier += 1;
            });

            //Load Right Goalie, Home is always on the right first.
            if (AllHomePlayers.Count() >= 1)
            {
                TeamRosterModel p = new TeamRosterModel(AllHomePlayers[0]);
                p.PlayerTappedByUser += PlayerTappedByUser;
                p.JerseySource = homeImagePath;
                p.FieldColumn = 4;
                p.FieldRow = 0;
                p.Home = true;
                AllHomePlayers.Remove(AllHomePlayers[0]);
                RightGoalie.Add(p);
            }
            //If there is no one on the team, add a blank player to the goalie.
            else
            {
                TeamRosterModel player = GetBlankPlayer(_homeTeamID, 4);
                player.FieldColumn = 4;
                player.FieldRow = 0;
                RightGoalie.Add(player);
            }

            //LOAD Right Unkown Player.
            TeamRosterModel rightUnkPlayer = new TeamRosterModel();
            rightUnkPlayer.Player = new DataObjects.DbClasses.Player() { FirstName = "Unknown", PlayerID = -1 };
            rightUnkPlayer.Team = new DataObjects.DbClasses.Team() { TeamID = _homeTeamID };
            rightUnkPlayer.TeamRoster = new DataObjects.DbClasses.TeamRoster() { UniformNumber = "?" };
            rightUnkPlayer.JerseySource = homeImagePath;
            rightUnkPlayer.PlayerTappedByUser += PlayerTappedByUser;
            RightUnknownPlayer.Add(rightUnkPlayer);

            TeamRosterModel rightSpaceHolder = new TeamRosterModel();
            rightSpaceHolder.Player = new DataObjects.DbClasses.Player() { FirstName = "Unknown", PlayerID = -1 };
            rightSpaceHolder.Team = new DataObjects.DbClasses.Team() { TeamID = _homeTeamID };
            rightSpaceHolder.TeamRoster = new DataObjects.DbClasses.TeamRoster() { UniformNumber = "?" };
            rightSpaceHolder.JerseySource = homeImagePath;
            rightSpaceHolder.PlayerOpacity = 0;
            RightSpaceHolder.Clear();
            RightSpaceHolder.Add(rightSpaceHolder);

            //Home Player (Right side of the screen setup)
            // Col 1, 2, 3, 4
            //For 11 v 11, we will go with the 4x4x2 formation
            //For 8 v 8, we will go with the 2x3x2 to start.
            //We will always start by adding the row of players closest to the goalie first (in case there aren't enough players on the team)

            //Load Column 3, always should be loaded first.  Column 3 will always have 2 players in it.

            //Always 2
            var column3RightPlayers = GetColumn3(AllHomePlayers, _homeTeamID).OrderBy(x => x.Player.PlayerID).ToList();
            //If 8 v 8, 3 players.  If 11 v 11, 4 players
            var column2RightPlayers = GetColumn2(AllHomePlayers, _homeTeamID).OrderBy(x => x.Player.PlayerID).ToList();
            //If 8 v 8 2 players.  If 11 v 11, 4 players
            var column1RightPlayers = GetColumn1(AllHomePlayers, _homeTeamID).OrderBy(x => x.Player.PlayerID).ToList();

            int col3Row = 0;
            column3RightPlayers.ForEach(x =>
            {
                if (!x.Player.FirstName.Equals(BLANK_PLAYER))
                {
                    x.PlayerTappedByUser += PlayerTappedByUser;
                }
                x.FieldColumn = 3;
                x.FieldRow = col3Row;
                x.Home = true;

                if (x.JerseySource == null)
                {
                    x.JerseySource = homeImagePath;
                }

                RightCol3.Add(x);

                col3Row += 1;
            });

            int col2Row = 0;
            column2RightPlayers.ForEach(x =>
            {
                if (!x.Player.FirstName.Equals(BLANK_PLAYER))
                {
                    x.PlayerTappedByUser += PlayerTappedByUser;
                }

                x.FieldColumn = 2;
                x.FieldRow = col2Row;
                x.Home = true;

                if (x.JerseySource == null)
                {
                    x.JerseySource = homeImagePath;
                }

                RightCol2.Add(x);
                col2Row += 1;
            });

            int col1Row = 0;
            column1RightPlayers.ForEach(x =>
            {
                if (!x.Player.FirstName.Equals(BLANK_PLAYER))
                {
                    x.PlayerTappedByUser += PlayerTappedByUser;
                }

                x.FieldColumn = 1;
                x.FieldRow = col1Row;
                x.Home = true;

                if (x.JerseySource == null)
                {
                    x.JerseySource = homeImagePath;
                }

                RightCol1.Add(x);
                col1Row += 1;
            });

            int homeSubsRow = 0;
            //Home Subs
            for (int i = 0; i < AllHomePlayers.Count; i++)
            {
                TeamRosterModel p = new TeamRosterModel(AllHomePlayers[i]);
                p.PlayerTappedByUser += PlayerTappedByUser;
                p.Home = true;
                p.FieldColumn = 5;
                p.FieldRow = homeSubsRow;
                p.Home = true;
                p.JerseySource = homeImagePath;
                RightSubs.Add(p);

                homeSubsRow += 1;
            }

            TeamRosterModel rightSubBlank = GetBlankPlayer(_homeTeamID, 5);
            rightSubBlank.FieldColumn = 5;
            rightSubBlank.FieldRow = homeSubsRow + 1;
            //Home Sub Blank player
            RightSubs.Add(rightSubBlank);


            //Left / Visitor loading
            //Unkown4  VCol3    VCol2    VCol1
            //For 11 v 11, we will go with the 4x4x2 formation
            //For 8 v 8, we will go with the 2x3x2 to start.

            //Always load unkown player.
            TeamRosterModel leftUnkPlayer = new TeamRosterModel();
            leftUnkPlayer.Player = new DataObjects.DbClasses.Player() { FirstName = "Unknown", PlayerID = -1 };
            leftUnkPlayer.Team = new DataObjects.DbClasses.Team() { TeamID = _visitorTeamID };
            leftUnkPlayer.TeamRoster = new DataObjects.DbClasses.TeamRoster() { UniformNumber = "?" };
            leftUnkPlayer.JerseySource = awayImagePath;
            leftUnkPlayer.PlayerTappedByUser += PlayerTappedByUser;
            LeftUnknownPlayer.Add(leftUnkPlayer);

            LeftSpaceHolder.Clear();
            TeamRosterModel leftSpaceHolder = new TeamRosterModel();
            leftSpaceHolder.Player = new DataObjects.DbClasses.Player() { FirstName = "Unknown", PlayerID = -1 };
            leftSpaceHolder.Team = new DataObjects.DbClasses.Team() { TeamID = _visitorTeamID };
            leftSpaceHolder.TeamRoster = new DataObjects.DbClasses.TeamRoster() { UniformNumber = "?" };
            leftSpaceHolder.JerseySource = awayImagePath;
            leftSpaceHolder.PlayerOpacity = 0;
            LeftSpaceHolder.Add(leftSpaceHolder);

            //Always load goalie.
            if (AllVisitorPlayers.Count() > 0)
            {
                TeamRosterModel vGoalie = new TeamRosterModel(AllVisitorPlayers[0]);
                vGoalie.PlayerTappedByUser += PlayerTappedByUser;
                vGoalie.FieldColumn = 4;
                vGoalie.FieldRow = 1;
                vGoalie.Home = false;
                vGoalie.JerseySource = awayImagePath;
                LeftGoalie.Add(vGoalie);
                AllVisitorPlayers.Remove(AllVisitorPlayers[0]);
            }
            else
            {
                TeamRosterModel homeBlankGoalie = GetBlankPlayer(_visitorTeamID, 4);
                homeBlankGoalie.FieldColumn = 4;
                homeBlankGoalie.FieldRow = 1;
                LeftGoalie.Add(homeBlankGoalie);
            }

            //Always 2
            var column3LeftPlayers = GetColumn3(AllVisitorPlayers, _visitorTeamID).OrderBy(x => x.Player.PlayerID).ToList();
            //If 8 v 8, 3 players.  If 11 v 11, 4 players
            var column2LeftPlayers = GetColumn2(AllVisitorPlayers, _visitorTeamID).OrderBy(x => x.Player.PlayerID).ToList();
            //If 8 v 8 2 players.  If 11 v 11, 4 players
            var column1LeftPlayers = GetColumn1(AllVisitorPlayers, _visitorTeamID).OrderBy(x => x.Player.PlayerID).ToList();

            int leftCol3Row = 0;
            column3LeftPlayers.ForEach(x =>
            {
                if (!x.Player.FirstName.Equals(BLANK_PLAYER))
                {
                    x.PlayerTappedByUser += PlayerTappedByUser;
                }

                x.FieldColumn = 3;
                x.FieldRow = leftCol3Row;
                x.Home = false;

                if (x.JerseySource == null)
                {
                    x.JerseySource = awayImagePath;
                }

                LeftCol3.Add(x);
                leftCol3Row += 1;
            });

            int leftCol2Row = 0;
            column2LeftPlayers.ForEach(x =>
            {
                if (!x.Player.FirstName.Equals(BLANK_PLAYER))
                {
                    x.PlayerTappedByUser += PlayerTappedByUser;
                }

                x.FieldColumn = 2;
                x.FieldRow = leftCol2Row;
                x.Home = false;

                if (x.JerseySource == null)
                {
                    x.JerseySource = awayImagePath;
                }

                LeftCol2.Add(x);
                leftCol2Row += 1;
            });

            int leftCol1Row = 0;
            column1LeftPlayers.ForEach(x =>
            {
                if (!x.Player.FirstName.Equals(BLANK_PLAYER))
                {
                    x.PlayerTappedByUser += PlayerTappedByUser;
                }

                x.FieldColumn = 1;
                x.FieldRow = leftCol1Row;
                x.Home = false;

                if (x.JerseySource == null)
                {
                    x.JerseySource = awayImagePath;
                }

                LeftCol1.Add(x);
                leftCol1Row += 1;
            });

            int leftSubsRow = 0;
            for (int i = 0; i < AllVisitorPlayers.Count; i++)
            {
                TeamRosterModel p = new TeamRosterModel(AllVisitorPlayers[i]);
                p.PlayerTappedByUser += PlayerTappedByUser;
                p.FieldColumn = 5;
                p.FieldRow = leftSubsRow;
                p.Home = false;
                p.JerseySource = awayImagePath;

                LeftSubs.Add(p);
                leftSubsRow += 1;
            }

            TeamRosterModel leftSubBlankPlayer = GetBlankPlayer(_visitorTeamID, 5);
            leftSubBlankPlayer.FieldColumn = 5;
            leftSubBlankPlayer.FieldRow = leftSubsRow + 1;
            LeftSubs.Add(leftSubBlankPlayer);
        }

        #endregion firsttimeload

        #region reload

        private void ReloadGameAfterItHasAlreadyBeenLoaded(int eventID)
        {
            Game = DAL.Instance().GetGame(eventID);

            //1. Initialize Clock to what it was
            InitializeClock();
            //2. Load Starting Formation
            LoadStartingFormation(Game);
            //3. Set Up namesOnLeftAndRight
            LoadTeamNamesAndScores(eventID);
            //4. Reload Players
            ReloadPlayers();
        }

        public void ReloadOnlyScore()
        {
            //Reload game
            Game = DAL.Instance().GetGame(Game.Game.GameID);
            //Load Home and away scores
            LoadTeamNamesAndScores(Game.Game.GameID);
        }

        private void LoadTeamNamesAndScores(int eventID)
        {
            if (Game.Game.HomeTeamSideOfField.ToUpper().Equals("RIGHT"))
            {
                RightTeamName = Game.HomeTeam.TeamName;
                RightTeamScore = Game.Game.HomeTeamScore;
                RightTeamShootOutGoals = Game.Game.HomeTeamShootOutGoals;

                LeftTeamName = Game.AwayTeam.TeamName;
                LeftTeamScore = Game.Game.AwayTeamScore;
                LeftTeamShootOutGoals = Game.Game.AwayTeamShootOutGoals;
            }
            else
            {
                //Right Team is Away team
                RightTeamName = Game.AwayTeam.TeamName;
                RightTeamScore = Game.Game.AwayTeamScore;
                RightTeamShootOutGoals = Game.Game.AwayTeamShootOutGoals;

                LeftTeamName = Game.HomeTeam.TeamName;
                LeftTeamScore = Game.Game.HomeTeamScore;
                LeftTeamShootOutGoals = Game.Game.HomeTeamShootOutGoals;
            }

            RightScore1_picPath = getScore1(RightTeamScore);
            RightScore2_picPath = getScore2(RightTeamScore);
            LeftScore1_picPath = getScore1(LeftTeamScore);
            LeftScore2_picPath = getScore2(LeftTeamScore);


            ShootOutOpps = Game.Game.HomeTeamShootOutGoalOpp + Game.Game.AwayTeamShootOutGoalOpp;
        }

        public void ReloadPlayers()
        {
            var homePlayers = DAL.Instance().GetPlayersPhyiscalAndTeamRosterInfo(Game.Game.GameID).Where(x => x.Team.TeamID == Game.Game.HomeTeamID);
            var awayPlayers = DAL.Instance().GetPlayersPhyiscalAndTeamRosterInfo(Game.Game.GameID).Where(x => x.Team.TeamID == Game.Game.AwayTeamID);
            string homeImagePath = DAL.Instance().GetJerseyByJerseyId(_game.HomeTeam.JerseyID).ImagePath;
            string awayImagePath = DAL.Instance().GetJerseyByJerseyId(_game.AwayTeam.JerseyID).ImagePath;

            //UnSubscribe From all events
            UnloadAllEventsAndClearPlayers();

            homePlayers.ToList().ForEach(x =>
            {
                if (x.EventRoster.GMPlayerPositionID != string.Empty)
                {
                    string[] columnAndRow = x.EventRoster.GMPlayerPositionID.Split(',');
                    x.FieldColumn = Convert.ToInt32(columnAndRow[0]);
                    x.FieldRow = Convert.ToInt32(columnAndRow[1]);
                    x.Home = true;
                    x.JerseySource = homeImagePath;
                }
                else
                {
                    x.Home = true;
                    x.JerseySource = homeImagePath;
                    x.FieldColumn = 5;
                    x.FieldRow = 99;
                }
            });

            awayPlayers.ToList().ForEach(x =>
            {
                if (x.EventRoster.GMPlayerPositionID != string.Empty)
                {
                    string[] columnAndRow = x.EventRoster.GMPlayerPositionID.Split(',');
                    x.FieldColumn = Convert.ToInt32(columnAndRow[0]);
                    x.FieldRow = Convert.ToInt32(columnAndRow[1]);
                    x.Home = false;
                    x.JerseySource = awayImagePath;
                }
                else
                {
                    x.Home = false;
                    x.JerseySource = awayImagePath;
                    x.FieldColumn = 5;
                    x.FieldRow = 99;
                }
            });

            if (Game.Game.HomeTeamSideOfField.ToUpper().Equals("RIGHT"))
            {
                LoadRightGoalie(homePlayers.ToList(), Game.Game.HomeTeamID);
                LoadLeftGoalie(awayPlayers.ToList(), Game.Game.AwayTeamID);

                //home players on right
                LoadCol1(homePlayers.ToList(), Game.Game.HomeTeamID, true);
                LoadCol2(homePlayers.ToList(), Game.Game.HomeTeamID, true);
                LoadCol3(homePlayers.ToList(), Game.Game.HomeTeamID, true);
                LoadSubs(homePlayers.ToList(), Game.Game.HomeTeamID, true);
                LoadUnkPlayers(Game.Game.HomeTeamID, true);

                var allHomePlayersOnField = RightCol1.Union(RightCol2).Union(RightCol3).Union(RightGoalie).Union(RightSubs).ToList();
                var missingHomePlayers = homePlayers.Where(x => !allHomePlayersOnField.Exists(pof => pof.Player.PlayerID == x.Player.PlayerID));

                if (missingHomePlayers.Count() > 0)
                {
                    foreach (var row in missingHomePlayers)
                    {
                        row.FieldColumn = 5;
                        row.FieldRow = 99;
                        RightSubs.Add(row);

                        UpdateGamePositionID(_homeTeamID, row.Player.PlayerID, row.FieldColumn, row.FieldRow);
                        LogError("Player Duplicate Issue", "Reload Players");
                    }
                }

                //away players on left
                LoadCol1(awayPlayers.ToList(), Game.Game.AwayTeamID, false);
                LoadCol2(awayPlayers.ToList(), Game.Game.AwayTeamID, false);
                LoadCol3(awayPlayers.ToList(), Game.Game.AwayTeamID, false);
                LoadSubs(awayPlayers.ToList(), Game.Game.AwayTeamID, false);
                LoadUnkPlayers(Game.Game.AwayTeamID, false);

                var allAwayPlayersOnField = LeftCol1.Union(LeftCol2).Union(LeftCol3).Union(LeftGoalie).Union(LeftSubs).ToList();
                var missingAwayPlayers = awayPlayers.Where(x => !allAwayPlayersOnField.Exists(pof => pof.Player.PlayerID == x.Player.PlayerID));

                if (missingAwayPlayers.Count() > 0)
                {
                    foreach (var row in missingAwayPlayers)
                    {
                        row.FieldColumn = 5;
                        row.FieldRow = 99;
                        LeftSubs.Add(row);

                        UpdateGamePositionID(_visitorTeamID, row.Player.PlayerID, row.FieldColumn, row.FieldRow);
                        LogError("Player Duplicate Issue", "Reload Players");
                    }
                }
            }
            else
            {
                LoadRightGoalie(awayPlayers.ToList(), Game.Game.AwayTeamID);
                LoadLeftGoalie(homePlayers.ToList(), Game.Game.HomeTeamID);

                //Away players on the right
                LoadCol1(awayPlayers.ToList(), Game.Game.AwayTeamID, true);
                LoadCol2(awayPlayers.ToList(), Game.Game.AwayTeamID, true);
                LoadCol3(awayPlayers.ToList(), Game.Game.AwayTeamID, true);
                LoadSubs(awayPlayers.ToList(), Game.Game.AwayTeamID, true);
                LoadUnkPlayers(Game.Game.AwayTeamID, true);

                var allAwayPlayersOnField = RightCol1.Union(RightCol2).Union(RightCol3).Union(RightGoalie).Union(RightSubs).ToList();
                var missingAwayPlayers = awayPlayers.Where(x => !allAwayPlayersOnField.Exists(pof => pof.Player.PlayerID == x.Player.PlayerID));

                if (missingAwayPlayers.Count() > 0)
                {
                    foreach (var row in missingAwayPlayers)
                    {
                        row.FieldColumn = 5;
                        row.FieldRow = 99;
                        RightSubs.Add(row);

                        UpdateGamePositionID(_visitorTeamID, row.Player.PlayerID, row.FieldColumn, row.FieldRow);
                        LogError("Player Duplicate Issue", "Reload Players");
                    }
                }

                //Home Players on the left
                LoadCol1(homePlayers.ToList(), Game.Game.HomeTeamID, false);
                LoadCol2(homePlayers.ToList(), Game.Game.HomeTeamID, false);
                LoadCol3(homePlayers.ToList(), Game.Game.HomeTeamID, false);
                LoadSubs(homePlayers.ToList(), Game.Game.HomeTeamID, false);
                LoadUnkPlayers(Game.Game.HomeTeamID, false);

                var allHomePlayersOnField = LeftCol1.Union(LeftCol2).Union(LeftCol3).Union(LeftGoalie).Union(LeftSubs).ToList();
                var missingHomePlayers = homePlayers.Where(x => !allHomePlayersOnField.Exists(pof => pof.Player.PlayerID == x.Player.PlayerID));

                if (missingHomePlayers.Count() > 0)
                {
                    foreach (var row in missingHomePlayers)
                    {
                        row.FieldColumn = 5;
                        row.FieldRow = 99;
                        LeftSubs.Add(row);

                        //Then Update his Game Position ID
                        UpdateGamePositionID(_homeTeamID, row.Player.PlayerID, row.FieldColumn, row.FieldRow);
                        LogError("Player Duplicate Issue", "Reload Players");
                    }
                }
            }

        }

        private void LogError(string message, string method)
        {
            ErrorLogConnection er = new ErrorLogConnection();
            er.UpdateErrorLog(method, message);
        }


        private void UnloadAllEventsAndClearPlayers()
        {
            RightGoalie.ToList().ForEach(x =>
            {
                x.PlayerTappedByUser -= PlayerTappedByUser;
            });

            RightCol1.ToList().ForEach(x =>
            {
                x.PlayerTappedByUser -= PlayerTappedByUser;
            });

            RightCol2.ToList().ForEach(x =>
            {
                x.PlayerTappedByUser -= PlayerTappedByUser;
            });

            RightCol3.ToList().ForEach(x =>
            {
                x.PlayerTappedByUser -= PlayerTappedByUser;
            });

            RightSubs.ToList().ForEach(x =>
            {
                x.PlayerTappedByUser -= PlayerTappedByUser;
            });

            RightUnknownPlayer.ToList().ForEach(x =>
            {
                x.PlayerTappedByUser -= PlayerTappedByUser;
            });

            LeftGoalie.ToList().ForEach(x =>
            {
                x.PlayerTappedByUser -= PlayerTappedByUser;
            });

            LeftCol1.ToList().ForEach(x =>
            {
                x.PlayerTappedByUser -= PlayerTappedByUser;
            });

            LeftCol2.ToList().ForEach(x =>
            {
                x.PlayerTappedByUser -= PlayerTappedByUser;
            });

            LeftCol3.ToList().ForEach(x =>
            {
                x.PlayerTappedByUser -= PlayerTappedByUser;
            });

            LeftSubs.ToList().ForEach(x =>
            {
                x.PlayerTappedByUser -= PlayerTappedByUser;
            });

            LeftUnknownPlayer.ToList().ForEach(x =>
            {
                x.PlayerTappedByUser -= PlayerTappedByUser;
            });

            //Clear all of the players
            RightGoalie.Clear();
            RightCol1.Clear();
            RightCol2.Clear();
            RightCol3.Clear();
            RightSubs.Clear();
            RightUnknownPlayer.Clear();
            LeftGoalie.Clear();
            LeftCol1.Clear();
            LeftCol2.Clear();
            LeftCol3.Clear();
            LeftSubs.Clear();
            LeftUnknownPlayer.Clear();
        }

        private void LoadLeftGoalie(List<TeamRosterModel> players, int teamID)
        {
            var leftGoalie = players.Where(x => x.FieldColumn == 4).ToList();

            //we didn't find the goalie
            if (leftGoalie.Count() == 0)
            {
                TeamRosterModel player = GetBlankPlayer(teamID, 4);
                player.FieldColumn = 4;
                player.FieldRow = 1;
                LeftGoalie.Add(player);
            }
            else
            {
                //We found the goalie...
                TeamRosterModel p = new TeamRosterModel(leftGoalie[0]);
                p.PlayerTappedByUser += PlayerTappedByUser;
                p.FieldColumn = 4;
                p.UniqueIdentifier = _uniquePlayerIdentifier;
                _uniquePlayerIdentifier += 1;
                p.FieldRow = 1;
                p.Home = teamID == Game.Game.HomeTeamID ? true : false;
                LeftGoalie.Add(p);
            }
        }

        private void LoadRightGoalie(List<TeamRosterModel> players, int teamID)
        {
            var rightGoalie = players.Where(x => x.FieldColumn == 4).ToList();

            //we didn't find the goalie
            if (rightGoalie.Count() == 0)
            {
                TeamRosterModel player = GetBlankPlayer(teamID, 4);
                player.FieldColumn = 4;
                player.FieldRow = 1;
                RightGoalie.Add(player);
            }
            else
            {
                //We found the goalie...
                TeamRosterModel p = new TeamRosterModel(rightGoalie[0]);
                p.PlayerTappedByUser += PlayerTappedByUser;
                p.FieldColumn = 4;
                p.FieldRow = 1;
                p.UniqueIdentifier = _uniquePlayerIdentifier;
                _uniquePlayerIdentifier += 1;
                p.Home = teamID == Game.Game.HomeTeamID ? true : false;
                RightGoalie.Add(p);
            }
        }

        private void LoadCol1(List<TeamRosterModel> players, int teamID, bool right)
        {
            //all of the players that need to be added to column 1 on the right side
            var playersToAddCol1 = players.Where(x => x.FieldColumn == 1).ToList();

            //We should always 5 spaces 
            for (int i = 0; i <= 3; i++)
            {
                if (playersToAddCol1.Where(x => x.FieldRow == i).Count() > 0)
                {
                    var player = playersToAddCol1.Where(x => x.FieldRow == i).FirstOrDefault();

                    if (player != null)
                    {
                        player.UniqueIdentifier = _uniquePlayerIdentifier;
                        _uniquePlayerIdentifier += 1;
                        player.PlayerTappedByUser += PlayerTappedByUser;
                        player.Home = teamID == Game.Game.HomeTeamID ? true : false;
                        if (right)
                        {
                            RightCol1.Add(player);
                        }
                        else
                        {
                            LeftCol1.Add(player);
                        }
                    }
                }
                else
                {
                    TeamRosterModel blankPlayer = GetBlankPlayer(teamID, 1);
                    blankPlayer.FieldRow = i;

                    if (right)
                    {
                        RightCol1.Add(blankPlayer);
                    }
                    else
                    {
                        LeftCol1.Add(blankPlayer);
                    }
                }
            }
        }

        private void LoadCol2(List<TeamRosterModel> players, int teamID, bool right)
        {
            //all of the players that need to be added to column 1 on the right side
            var playersToAddCol2 = players.Where(x => x.FieldColumn == 2).ToList();

            //We should always 5 spaces 
            for (int i = 0; i <= 3; i++)
            {
                if (playersToAddCol2.Where(x => x.FieldRow == i).Count() > 0)
                {
                    var player = playersToAddCol2.Where(x => x.FieldRow == i).FirstOrDefault();

                    if (player != null)
                    {
                        player.UniqueIdentifier = _uniquePlayerIdentifier;
                        _uniquePlayerIdentifier += 1;
                        player.PlayerTappedByUser += PlayerTappedByUser;
                        player.Home = teamID == Game.Game.HomeTeamID ? true : false;
                        if (right)
                        {
                            RightCol2.Add(player);
                        }
                        else
                        {
                            LeftCol2.Add(player);
                        }
                    }
                }
                else
                {
                    TeamRosterModel blankPlayer = GetBlankPlayer(teamID, 2);
                    blankPlayer.FieldRow = i;

                    if (right)
                    {
                        RightCol2.Add(blankPlayer);
                    }
                    else
                    {
                        LeftCol2.Add(blankPlayer);
                    }
                }
            }
        }

        private void LoadCol3(List<TeamRosterModel> players, int teamID, bool right)
        {
            //all of the players that need to be added to column 1 on the right side
            var playersToAddCol3 = players.Where(x => x.FieldColumn == 3).ToList();

            //We should always 5 spaces 
            for (int i = 0; i <= 3; i++)
            {
                if (playersToAddCol3.Where(x => x.FieldRow == i).Count() > 0)
                {
                    var player = playersToAddCol3.Where(x => x.FieldRow == i).FirstOrDefault();

                    if (player != null)
                    {
                        player.UniqueIdentifier = _uniquePlayerIdentifier;
                        _uniquePlayerIdentifier += 1;
                        player.PlayerTappedByUser += PlayerTappedByUser;
                        player.Home = teamID == Game.Game.HomeTeamID ? true : false;
                        if (right)
                        {
                            RightCol3.Add(player);
                        }
                        else
                        {
                            LeftCol3.Add(player);
                        }
                    }
                }
                else
                {
                    TeamRosterModel blankPlayer = GetBlankPlayer(teamID, 3);
                    blankPlayer.FieldRow = i;

                    if (right)
                    {
                        RightCol3.Add(blankPlayer);
                    }
                    else
                    {
                        LeftCol3.Add(blankPlayer);
                    }
                }
            }
        }

        private void LoadSubs(List<TeamRosterModel> players, int teamID, bool right)
        {
            //all of the players that need to be added to column 1 on the right side
            var playersToAddCol5 = players.Where(x => x.FieldColumn == 5).ToList();

            foreach (var player in playersToAddCol5.OrderBy(x => x.FieldRow))
            {
                player.UniqueIdentifier = _uniquePlayerIdentifier;
                _uniquePlayerIdentifier += 1;
                player.PlayerTappedByUser += PlayerTappedByUser;
                player.Home = teamID == Game.Game.HomeTeamID ? true : false;
                if (right)
                {
                    RightSubs.Add(player);
                }
                else
                {
                    LeftSubs.Add(player);
                }
            }

            TeamRosterModel blankPlayer = GetBlankPlayer(teamID, 5);

            //Always add 1 sub
            if (right)
            {
                blankPlayer.FieldRow = RightSubs.Count() + 1;
                RightSubs.Add(blankPlayer);
            }
            else
            {
                blankPlayer.FieldRow = LeftSubs.Count() + 1;
                LeftSubs.Add(blankPlayer);
            }
        }

        private void LoadUnkPlayers(int teamID, bool right)
        {
            string homeImagePath = DAL.Instance().GetJerseyByJerseyId(_game.HomeTeam.JerseyID).ImagePath;
            string awayImagePath = DAL.Instance().GetJerseyByJerseyId(_game.AwayTeam.JerseyID).ImagePath;

            TeamRosterModel unkPlayer = new TeamRosterModel();
            unkPlayer.Player = new DataObjects.DbClasses.Player() { FirstName = "Unknown", PlayerID = -1 };
            unkPlayer.Team = new DataObjects.DbClasses.Team() { TeamID = teamID };
            unkPlayer.TeamRoster = new DataObjects.DbClasses.TeamRoster() { UniformNumber = "?" };
            unkPlayer.JerseySource = teamID == Game.Game.HomeTeamID ? homeImagePath : awayImagePath;
            unkPlayer.PlayerTappedByUser += PlayerTappedByUser;

            if (right)
            {
                RightUnknownPlayer.Add(unkPlayer);
            }
            else
            {
                LeftUnknownPlayer.Add(unkPlayer);
            }

            //Also loading Spaceholders Here.
            TeamRosterModel spaceHolder = new TeamRosterModel();
            spaceHolder.Player = new DataObjects.DbClasses.Player() { FirstName = String.Empty, PlayerID = -1 };
            spaceHolder.Team = new DataObjects.DbClasses.Team() { TeamID = teamID };
            spaceHolder.TeamRoster = new DataObjects.DbClasses.TeamRoster() { UniformNumber = "?" };
            spaceHolder.JerseySource = teamID == Game.Game.HomeTeamID ? homeImagePath : awayImagePath;
            spaceHolder.PlayerOpacity = 0;

            if (right)
            {
                RightSpaceHolder.Clear();
                RightSpaceHolder.Add(spaceHolder);
            }
            else
            {
                LeftSpaceHolder.Clear();
                LeftSpaceHolder.Add(spaceHolder);
            }
        }

        #endregion reload

        private void InitializeClock()
        {
            //set clock count up or down
            if (Game.Game.ClockUpOrDown.ToUpper().Equals("UP"))
            {
                Clock.CountUp = true;
            }
            else
            {
                Clock.CountUp = false;
            }

            if (Game.Game.GameStatus.ToUpper().Trim().Replace(" ", "").Equals("NOTSTARTED"))
            {
                int minsToStartFrom = Game.Game.ClockUpOrDown.ToUpper().Equals("UP") ? 0 : Game.Game.PeriodLength;

                Clock.SetClock(TimeSpan.FromMinutes(minsToStartFrom));
                ClockTime = TimeSpan.FromMinutes(minsToStartFrom);
                ClockButtonSource = "/Assets/Play.png";
            }
            //This means that the clock has not been set yet
            else if (ClockTimeUi == string.Empty || ClockTimeUi == null)
            {
                try
                {
                    //Get Minutes and Seconds, convert to TimeSpan
                    string[] mmss = Game.Game.CurrentClock.Replace(" ", string.Empty).Split(':');
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

                    TimeSpan timeOfGame = TimeSpan.FromSeconds(seconds) + TimeSpan.FromMinutes(minutes);

                    Clock.StopClock();
                    Clock.SetClock(timeOfGame);
                    ClockButtonSource = "/Assets/Play.png";
                    ClockTime = timeOfGame;
                }
                catch (Exception ex)
                {
                    LogError(ex.ToString() + " Time that bombed out:" + Game.Game.CurrentClock, "InitializeClock");
                }
            }
            //else, don't initialize the clock
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameID"></param>
        /// <param name="teamID"></param>
        /// <param name="playerID"></param>
        /// <param name="positionID">Field Column, Field Row</param>
        private void UpdateGamePositionID(int teamID, int playerID, int fieldColumn, int fieldRow)
        {
            DAL.Instance().UpdatePlayersGMPlayerPositionID(Game.Game.GameID, teamID, playerID, fieldColumn + "," + fieldRow);
        }

        /// <summary>
        ///  Col 1, 2, 3, 4
        /// For 11 v 11, we need 4 players
        /// For 8 v 8, we need 2 players
        /// </summary>
        /// <param name="homePlayers"></param>
        /// <returns></returns>
        private List<TeamRosterModel> GetColumn1(List<TeamRosterModel> players, int teamID)
        {
            List<TeamRosterModel> returnValue = new List<TeamRosterModel>();

            if (_startingFormation == StartingFormation.Eight)
            {
                //2 players
                for (int i = 0; i < 2; i++)
                {
                    if (players.Count() != 0)
                    {
                        returnValue.Add(players[0]);
                        players.Remove(players[0]);
                    }
                }
            }
            else
            {
                //4 players
                for (int i = 0; i < 4; i++)
                {
                    if (players.Count() != 0)
                    {
                        returnValue.Add(players[0]);
                        players.Remove(players[0]);
                    }
                }
            }

            //Always return 5 so you can sub.
            int blankPlayersNeeded = 4 - returnValue.Count;
            for (int i = 0; i < blankPlayersNeeded; i++)
            {
                returnValue.Add(GetBlankPlayer(teamID, 1));
            }

            return returnValue;
        }

        /// <summary>
        /// If 8 v 8, 3 players.  
        /// If 11 v 11, 4 players.  
        /// Always return 5 including blanks.
        /// </summary>
        /// <param name="homePlayers"></param>
        /// <returns></returns>
        private List<TeamRosterModel> GetColumn2(List<TeamRosterModel> players, int teamID)
        {
            List<TeamRosterModel> returnValue = new List<TeamRosterModel>();

            if (_startingFormation == StartingFormation.Eight)
            {
                //3 players
                for (int i = 0; i < 3; i++)
                {
                    if (players.Count() != 0)
                    {
                        returnValue.Add(players[0]);
                        players.Remove(players[0]);
                    }
                }
            }
            else
            {
                //4 players
                for (int i = 0; i < 4; i++)
                {
                    if (players.Count() != 0)
                    {
                        returnValue.Add(players[0]);
                        players.Remove(players[0]);
                    }
                }
            }

            //Always return 5 so you can sub.
            int blankPlayersNeeded = 4 - returnValue.Count;
            for (int i = 0; i < blankPlayersNeeded; i++)
            {
                returnValue.Add(GetBlankPlayer(teamID, 2));
            }

            return returnValue;
        }

        /// <summary>
        /// Always need 2 players, but 5 with blanks.
        /// </summary>
        /// <param name="homePlayers"></param>
        /// <returns></returns>
        private List<TeamRosterModel> GetColumn3(List<TeamRosterModel> players, int teamID)
        {
            List<TeamRosterModel> returnValue = new List<TeamRosterModel>();

            //2 players
            for (int i = 0; i < 2; i++)
            {
                if (players.Count() != 0)
                {
                    returnValue.Add(players[0]);
                    players.Remove(players[0]);
                }
            }

            //Always return 5 so you can sub.
            int blankPlayersNeeded = 4 - returnValue.Count;
            for (int i = 0; i < blankPlayersNeeded; i++)
            {
                returnValue.Add(GetBlankPlayer(teamID, 3));
            }

            return returnValue;
        }

        private void LoadStartingFormation(GameModel game)
        {
            if (game.Game.PlayersPerTeam == 8)
            {
                _startingFormation = StartingFormation.Eight;
            }
            else
            {
                _startingFormation = StartingFormation.Eleven;
            }
        }

        private TeamRosterModel GetBlankPlayer(int teamID, int fieldColumn)
        {
            TeamRosterModel p = new TeamRosterModel();
            p.Player.FirstName = BLANK_PLAYER;
            p.Team.TeamID = teamID;
            // Column 5 is the bench
            p.FieldColumn = fieldColumn;
            p.PlayerTappedByUser += PlayerTappedByUser;
            p.PlayerOpacity = SubMode ? .5 : 0;
            p.UniqueIdentifier = _uniquePlayerIdentifier;
            p.PlayerVisibility = SubMode ? Visibility.Visible : Visibility.Collapsed;
            _uniquePlayerIdentifier += 1;
            p.JerseySource = "/Assets/YellowSquare2.png";
            p.Home = (teamID == _homeTeamID) ? true : false;

            return p;
        }

        /// <summary>
        /// Someone Has clicked on a player.  We need to determine what mode the UI is in in order to figure out what to do
        /// as an action.  Right now there are only 2 modes (stats / sub) mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerTappedByUser(object sender, EventArgs e)
        {
            //For Now, just get subs working.
            TeamRosterModel playerToSwap = sender as TeamRosterModel;

            if (SubMode)
            {
                ApplySubLogic(playerToSwap);
            }
            else
            {
                if (!Game.Game.GameStatus.ToUpper().Trim().Replace(" ", "").Equals("FINAL"))
                {
                    if (!Clock.IsClockRunning())
                    {
                        //The game has not started yet.  Prompt the user that the game has not started, and ask them whether or not they want to store the stats anyway.
                        //We are only going to do this in stats mode, as the user needs to be able to move players around the screen before the game starts.
                        MessageBoxResult res = MessageBox.Show(AppResources.PeriodNotStarted, AppResources.NotStarted, MessageBoxButton.OKCancel);
                        if (res == MessageBoxResult.Cancel)
                        {
                            return;
                        }
                    }
                }

                int otherTeamGoalieId = 0;
                if (playerToSwap.Team.TeamID == _homeTeamID)
                {
                    otherTeamGoalieId = LeftGoalie[0].Player.PlayerID;
                }
                else
                {
                    otherTeamGoalieId = RightGoalie[0].Player.PlayerID;
                }
                try
                {
                    //Call into stats engine
                    (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/StatsPicker.xaml?gameID=" + Game.Game.GameID + "&teamID=" + playerToSwap.Team.TeamID +
                        "&playerID=" + playerToSwap.Player.PlayerID + "&period=" + Game.Game.CurrentPeriod + "&gameTime=" + GetClockValueFinalIntoAcct() +
                        "&playerPosition=" + GetGmCommaPosition(playerToSwap) + "&playerName=" + playerToSwap.Player.FirstName + " " + playerToSwap.Player.LastName + "&otherTeamGoalieID=" + otherTeamGoalieId, UriKind.Relative));
                }
                catch (Exception ex)
                {

                }
            }
        }

        private string GetClockValueFinalIntoAcct()
        {
            string clk = string.Empty;

            if (Game.Game.GameStatus.ToUpper().Equals("FINAL") && Game.Game.ClockUpOrDown.ToUpper().Equals("DOWN"))
            {
                clk = "00:01";
            }
            else if (Game.Game.GameStatus.ToUpper().Equals("FINAL") && Game.Game.ClockUpOrDown.ToUpper().Equals("UP"))
            {
                clk = Game.Game.CurrentClock;
            }
            else
            {
                clk = ClockTime.ToString("mm\\:ss");
            }

            return clk;
        }

        private void ApplySubLogic(TeamRosterModel playerToSwap)
        {
            if (playerToSwap.Team.TeamID == _homeTeamID)
            {
                if (_visitorPlayer1ToSwap != null)
                {
                    return;
                }

                if (_homePlayer1ToSwap == null)
                {
                    _homePlayer1ToSwap = playerToSwap;
                    SetPlayerToSubMode(_homePlayer1ToSwap, true);
                }
                else
                {
                    if (_homePlayer1ToSwap.Player.PlayerID == playerToSwap.Player.PlayerID)
                    {
                        SetPlayerToSubMode(_homePlayer1ToSwap, false);
                        _homePlayer1ToSwap = null;
                    }
                    else
                    {
                        _homePlayer2ToSwap = playerToSwap;

                        //Hook to actually call into sub play.  If one player is on the bench, and one is on the field..
                        if ((_homePlayer1ToSwap.FieldColumn != 5 && _homePlayer2ToSwap.FieldColumn == 5) || (_homePlayer1ToSwap.FieldColumn == 5 && _homePlayer2ToSwap.FieldColumn != 5))
                        {
                            int playerOut = _homePlayer1ToSwap.FieldColumn == 5 ? _homePlayer2ToSwap.Player.PlayerID : _homePlayer1ToSwap.Player.PlayerID;
                            int playerIn = _homePlayer1ToSwap.FieldColumn == 5 ? _homePlayer1ToSwap.Player.PlayerID : _homePlayer2ToSwap.Player.PlayerID;

                            var actualPlayerIn = _homePlayer1ToSwap.FieldColumn == 5 ? _homePlayer1ToSwap : _homePlayer2ToSwap;
                            var actualPlayerOut = _homePlayer1ToSwap.FieldColumn == 5 ? _homePlayer2ToSwap : _homePlayer1ToSwap;

                            //Actual Sub
                            DAL.Instance().SaveSubstitutionPlay("GM", Game.Game.GameID, Game.Game.HomeTeamID, Game.Game.CurrentPeriod, TimeOfSubstitution, playerIn, GetGmCommaPosition(actualPlayerIn), playerOut, GetGmCommaPosition(actualPlayerOut));
                        }
                        else
                        {
                            //Otherwise, players are just swapping on the field
                            DAL.Instance().SaveMovePlay(Game.Game.GameID, Game.Game.HomeTeamID, Game.Game.CurrentPeriod, TimeOfSubstitution, _homePlayer1ToSwap.Player.PlayerID, GetGmCommaPosition(_homePlayer1ToSwap),
                              _homePlayer2ToSwap.Player.PlayerID, GetGmCommaPosition(_homePlayer2ToSwap));
                        }

                        SetPlayerToSubMode(_homePlayer2ToSwap, true);


                        SwapPlayerHome();
                        ResetHomeAndVisitorBlankSub();
                    }
                }
            }
            else
            {
                if (_homePlayer1ToSwap != null)
                {
                    return;
                }

                if (_visitorPlayer1ToSwap == null)
                {
                    _visitorPlayer1ToSwap = playerToSwap;
                    SetPlayerToSubMode(_visitorPlayer1ToSwap, true);
                }
                else
                {
                    if (_visitorPlayer1ToSwap.Player.PlayerID == playerToSwap.Player.PlayerID)
                    {
                        SetPlayerToSubMode(_visitorPlayer1ToSwap, false);
                        _visitorPlayer1ToSwap = null;
                    }
                    else
                    {
                        _visitorPlayer2ToSwap = playerToSwap;

                        //Hook to actually call into sub play.  If one player is on the bench, and one is on the field..
                        if ((_visitorPlayer1ToSwap.FieldColumn != 5 && _visitorPlayer2ToSwap.FieldColumn == 5) || (_visitorPlayer1ToSwap.FieldColumn == 5 && _visitorPlayer2ToSwap.FieldColumn != 5))
                        {
                            int playerOut = _visitorPlayer1ToSwap.FieldColumn == 5 ? _visitorPlayer2ToSwap.Player.PlayerID : _visitorPlayer1ToSwap.Player.PlayerID;
                            int playerIn = _visitorPlayer1ToSwap.FieldColumn == 5 ? _visitorPlayer1ToSwap.Player.PlayerID : _visitorPlayer2ToSwap.Player.PlayerID;

                            var actualPlayerIn = _visitorPlayer1ToSwap.FieldColumn == 5 ? _visitorPlayer1ToSwap : _visitorPlayer2ToSwap;
                            var actualPlayerOut = _visitorPlayer1ToSwap.FieldColumn == 5 ? _visitorPlayer2ToSwap : _visitorPlayer1ToSwap;

                            //Debug.WriteLine(actualPlayerIn.Player.FirstName + ": " + GetGmCommaPosition(actualPlayerIn));
                            //Debug.WriteLine(actualPlayerOut.Player.FirstName + ": " + GetGmCommaPosition(actualPlayerOut));

                            //Actual Sub
                            DAL.Instance().SaveSubstitutionPlay("GM", Game.Game.GameID, Game.Game.AwayTeamID, Game.Game.CurrentPeriod, TimeOfSubstitution, playerIn, GetGmCommaPosition(actualPlayerIn), playerOut, GetGmCommaPosition(actualPlayerOut));
                        }
                        else
                        {
                            //Otherwise, players are just swapping on the field
                            DAL.Instance().SaveMovePlay(Game.Game.GameID, Game.Game.AwayTeamID, Game.Game.CurrentPeriod, TimeOfSubstitution, _visitorPlayer1ToSwap.Player.PlayerID, GetGmCommaPosition(_visitorPlayer1ToSwap),
                               _visitorPlayer2ToSwap.Player.PlayerID, GetGmCommaPosition(_visitorPlayer2ToSwap));
                        }

                        SetPlayerToSubMode(_visitorPlayer2ToSwap, true);
                        SwapPlayerVisitor();
                        ResetHomeAndVisitorBlankSub();
                    }
                }
            }

            PrintPlayers();
            //Prompt users if there are too many guys on the field...
        }

        private string GetGmCommaPosition(TeamRosterModel player)
        {
            string returnValue = string.Empty;

            returnValue = player.FieldColumn + "," + player.FieldRow;

            return returnValue;
        }

        private void ResetHomeAndVisitorBlankSub()
        {
            try
            {

                //ReOrder Home Subs, placing only 1 empty sub last.
                var copyOfRightSubs = new ObservableCollection<TeamRosterModel>(RightSubs);
                RightSubs.Clear();

                foreach (var row in copyOfRightSubs.Where(x => x.Player.FirstName != BLANK_PLAYER))
                {
                    RightSubs.Add(row);
                }

                //add 1 Blank slot on the bench.
                TeamRosterModel rightBlank = GetBlankPlayer(_homeTeamID, 5);
                rightBlank.FieldRow = RightSubs.Count();
                RightSubs.Add(rightBlank);

                //ReOrder Home Subs, placing only 1 empty sub last.
                var copyOfVisitoreSubs = new ObservableCollection<TeamRosterModel>(LeftSubs);
                LeftSubs.Clear();

                foreach (var row in copyOfVisitoreSubs.Where(x => x.Player.FirstName != BLANK_PLAYER))
                {
                    LeftSubs.Add(row);
                }

                //add 1 Blank slot on the bench.
                TeamRosterModel leftBlank = GetBlankPlayer(_visitorTeamID, 5);
                leftBlank.FieldRow = LeftSubs.Count();
                LeftSubs.Add(leftBlank);
            }
            catch (Exception ex)
            {

            }
        }

        private void SetPlayerToSubMode(TeamRosterModel p, bool subMode)
        {
            try
            {
                if (LeftCol1.Where(x => x.UniqueIdentifier == p.UniqueIdentifier).Count() > 0)
                {
                    LeftCol1.Where(x => x.UniqueIdentifier == p.UniqueIdentifier).FirstOrDefault().ToggledForSub = subMode;
                    return;
                }

                if (LeftCol2.Where(x => x.UniqueIdentifier == p.UniqueIdentifier).Count() > 0)
                {
                    LeftCol2.Where(x => x.UniqueIdentifier == p.UniqueIdentifier).FirstOrDefault().ToggledForSub = subMode;
                    return;
                }

                if (LeftCol3.Where(x => x.UniqueIdentifier == p.UniqueIdentifier).Count() > 0)
                {
                    LeftCol3.Where(x => x.UniqueIdentifier == p.UniqueIdentifier).FirstOrDefault().ToggledForSub = subMode;
                    return;
                }

                if (LeftGoalie.Where(x => x.UniqueIdentifier == p.UniqueIdentifier).Count() > 0)
                {
                    LeftGoalie.Where(x => x.UniqueIdentifier == p.UniqueIdentifier).FirstOrDefault().ToggledForSub = subMode;
                    return;
                }

                if (LeftSubs.Where(x => x.UniqueIdentifier == p.UniqueIdentifier).Count() > 0)
                {
                    LeftSubs.Where(x => x.UniqueIdentifier == p.UniqueIdentifier).FirstOrDefault().ToggledForSub = subMode;
                    return;
                }

                if (RightCol1.Where(x => x.UniqueIdentifier == p.UniqueIdentifier).Count() > 0)
                {
                    RightCol1.Where(x => x.UniqueIdentifier == p.UniqueIdentifier).FirstOrDefault().ToggledForSub = subMode;
                    return;
                }

                if (RightCol2.Where(x => x.UniqueIdentifier == p.UniqueIdentifier).Count() > 0)
                {
                    RightCol2.Where(x => x.UniqueIdentifier == p.UniqueIdentifier).FirstOrDefault().ToggledForSub = subMode;
                    return;
                }

                if (RightCol3.Where(x => x.UniqueIdentifier == p.UniqueIdentifier).Count() > 0)
                {
                    RightCol3.Where(x => x.UniqueIdentifier == p.UniqueIdentifier).FirstOrDefault().ToggledForSub = subMode;
                    return;
                }

                if (RightGoalie.Where(x => x.UniqueIdentifier == p.UniqueIdentifier).Count() > 0)
                {
                    RightGoalie.Where(x => x.UniqueIdentifier == p.UniqueIdentifier).FirstOrDefault().ToggledForSub = subMode;
                    return;
                }

                if (RightSubs.Where(x => x.UniqueIdentifier == p.UniqueIdentifier).Count() > 0)
                {
                    RightSubs.Where(x => x.UniqueIdentifier == p.UniqueIdentifier).FirstOrDefault().ToggledForSub = subMode;
                    return;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void SwapPlayerHome()
        {
            TeamRosterModel copyOfPlayer1 = new TeamRosterModel(_homePlayer1ToSwap);
            TeamRosterModel copyOfPlayer2 = new TeamRosterModel(_homePlayer2ToSwap);

            if (Game.Game.HomeTeamSideOfField.ToUpper().Equals("RIGHT"))
            {
                switch (copyOfPlayer1.FieldColumn)
                {
                    case 1:
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer2.Player.FirstName;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer2.Player.LastName;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer2.Player.Height;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer2.Player.Weight;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer2.Player.Kicks;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer2.TeamRoster.UniformNumber;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer2.Player.PlayerID;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer2.JerseySource;
                        //RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer2.FieldRow;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer2.PlayerOpacity;
                        break;
                    case 2:
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer2.Player.FirstName;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer2.Player.LastName;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer2.Player.Height;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer2.Player.Weight;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer2.Player.Kicks;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer2.TeamRoster.UniformNumber;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer2.Player.PlayerID;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer2.JerseySource;
                        //RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer2.FieldRow;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer2.PlayerOpacity;
                        break;
                    case 3:
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer2.Player.FirstName;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer2.Player.LastName;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer2.Player.Height;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer2.Player.Weight;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer2.Player.Kicks;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer2.TeamRoster.UniformNumber;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer2.Player.PlayerID;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer2.JerseySource;
                        //RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer2.FieldRow;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer2.PlayerOpacity;
                        break;
                    case 4:
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer2.Player.FirstName;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer2.Player.LastName;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer2.Player.Height;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer2.Player.Weight;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer2.Player.Kicks;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer2.TeamRoster.UniformNumber;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer2.Player.PlayerID;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer2.JerseySource;
                        //RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer2.FieldRow;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer2.PlayerOpacity;
                        break;
                    case 5:
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer2.Player.FirstName;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer2.Player.LastName;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer2.Player.Height;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer2.Player.Weight;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer2.Player.Kicks;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer2.TeamRoster.UniformNumber;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer2.Player.PlayerID;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer2.JerseySource;
                        //RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer2.FieldRow;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer2.PlayerOpacity;
                        break;
                }

                //Player2
                switch (copyOfPlayer2.FieldColumn)
                {
                    case 1:
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer1.Player.FirstName;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer1.Player.LastName;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer1.Player.Height;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer1.Player.Weight;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer1.Player.Kicks;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer1.TeamRoster.UniformNumber;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer1.Player.PlayerID;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer1.JerseySource;
                        //RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer1.FieldRow;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer1.PlayerOpacity;
                        break;
                    case 2:
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer1.Player.FirstName;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer1.Player.LastName;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer1.Player.Height;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer1.Player.Weight;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer1.Player.Kicks;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer1.TeamRoster.UniformNumber;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer1.Player.PlayerID;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer1.JerseySource;
                        //RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer1.FieldRow;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer1.PlayerOpacity;
                        break;
                    case 3:
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer1.Player.FirstName;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer1.Player.LastName;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer1.Player.Height;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer1.Player.Weight;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer1.Player.Kicks;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer1.TeamRoster.UniformNumber;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer1.Player.PlayerID;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer1.JerseySource;
                        //RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer1.FieldRow;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer1.PlayerOpacity;
                        break;
                    case 4:
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer1.Player.FirstName;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer1.Player.LastName;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer1.Player.Height;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer1.Player.Weight;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer1.Player.Kicks;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer1.TeamRoster.UniformNumber;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer1.Player.PlayerID;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer1.JerseySource;
                        //RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer1.FieldRow;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer1.PlayerOpacity;
                        break;
                    case 5:
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer1.Player.FirstName;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer1.Player.LastName;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer1.Player.Height;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer1.Player.Weight;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer1.Player.Kicks;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer1.TeamRoster.UniformNumber;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer1.Player.PlayerID;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer1.JerseySource;
                        //RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer1.FieldRow;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer1.PlayerOpacity;
                        break;
                }
            }
            else
            {
                //Player2
                switch (copyOfPlayer2.FieldColumn)
                {
                    case 1:
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer1.Player.FirstName;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer1.Player.LastName;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer1.Player.Height;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer1.Player.Weight;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer1.Player.Kicks;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer1.TeamRoster.UniformNumber;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer1.Player.PlayerID;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer1.JerseySource;
                        //LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer1.FieldRow;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer1.PlayerOpacity;
                        break;
                    case 2:
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer1.Player.FirstName;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer1.Player.LastName;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer1.Player.Height;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer1.Player.Weight;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer1.Player.Kicks;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer1.TeamRoster.UniformNumber;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer1.Player.PlayerID;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer1.JerseySource;
                        //LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer1.FieldRow;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer1.PlayerOpacity;
                        break;
                    case 3:
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer1.Player.FirstName;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer1.Player.LastName;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer1.Player.Height;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer1.Player.Weight;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer1.Player.Kicks;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer1.TeamRoster.UniformNumber;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer1.Player.PlayerID;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer1.JerseySource;
                        //LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer1.FieldRow;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer1.PlayerOpacity;
                        break;
                    case 4:
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer1.Player.FirstName;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer1.Player.LastName;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer1.Player.Height;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer1.Player.Weight;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer1.Player.Kicks;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer1.TeamRoster.UniformNumber;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer1.Player.PlayerID;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer1.JerseySource;
                        //LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer1.FieldRow;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer1.PlayerOpacity;
                        break;
                    case 5:
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer1.Player.FirstName;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer1.Player.LastName;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer1.Player.Height;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer1.Player.Weight;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer1.Player.Kicks;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer1.TeamRoster.UniformNumber;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer1.Player.PlayerID;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer1.JerseySource;
                        //LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer1.FieldRow;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer1.PlayerOpacity;
                        break;
                }

                //Player1
                switch (copyOfPlayer1.FieldColumn)
                {
                    case 1:
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer2.Player.FirstName;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer2.Player.LastName;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer2.Player.Height;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer2.Player.Weight;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer2.Player.Kicks;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer2.TeamRoster.UniformNumber;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer2.Player.PlayerID;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer2.JerseySource;
                        //LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer2.FieldRow;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer2.PlayerOpacity;
                        break;
                    case 2:
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer2.Player.FirstName;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer2.Player.LastName;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer2.Player.Height;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer2.Player.Weight;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer2.Player.Kicks;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer2.TeamRoster.UniformNumber;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer2.Player.PlayerID;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer2.JerseySource;
                        //LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer2.FieldRow;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer2.PlayerOpacity;
                        break;
                    case 3:
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer2.Player.FirstName;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer2.Player.LastName;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer2.Player.Height;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer2.Player.Weight;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer2.Player.Kicks;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer2.TeamRoster.UniformNumber;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer2.Player.PlayerID;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer2.JerseySource;
                        //LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer2.FieldRow;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer2.PlayerOpacity;
                        break;
                    case 4:
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer2.Player.FirstName;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer2.Player.LastName;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer2.Player.Height;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer2.Player.Weight;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer2.Player.Kicks;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer2.TeamRoster.UniformNumber;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer2.Player.PlayerID;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer2.JerseySource;
                        //LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer2.FieldRow;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer2.PlayerOpacity;
                        break;
                    case 5:
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer2.Player.FirstName;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer2.Player.LastName;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer2.Player.Height;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer2.Player.Weight;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer2.Player.Kicks;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer2.TeamRoster.UniformNumber;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer2.Player.PlayerID;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer2.JerseySource;
                        //LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer2.FieldRow;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer2.PlayerOpacity;
                        break;
                }
            }


            SetPlayerToSubMode(_homePlayer1ToSwap, false);
            SetPlayerToSubMode(_homePlayer2ToSwap, false);

            _homePlayer1ToSwap = null;
            _homePlayer2ToSwap = null;
        }

        private void SwapPlayerVisitor()
        {
            TeamRosterModel copyOfPlayer1 = new TeamRosterModel(_visitorPlayer1ToSwap);
            TeamRosterModel copyOfPlayer2 = new TeamRosterModel(_visitorPlayer2ToSwap);

            if (Game.Game.AwayTeamSideOfField.ToUpper().Equals("LEFT"))
            {
                //Player2
                switch (copyOfPlayer2.FieldColumn)
                {
                    case 1:
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer1.Player.FirstName;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer1.Player.LastName;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer1.Player.Height;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer1.Player.Weight;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer1.Player.Kicks;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer1.TeamRoster.UniformNumber;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer1.Player.PlayerID;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer1.JerseySource;
                        //LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer1.FieldRow;
                        //LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldColumn = copyOfPlayer1.FieldColumn;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer1.PlayerOpacity;
                        break;
                    case 2:
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer1.Player.FirstName;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer1.Player.LastName;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer1.Player.Height;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer1.Player.Weight;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer1.Player.Kicks;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer1.TeamRoster.UniformNumber;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer1.Player.PlayerID;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer1.JerseySource;
                        //LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer1.FieldRow;
                        //LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldColumn = copyOfPlayer1.FieldColumn;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer1.PlayerOpacity;
                        break;
                    case 3:
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer1.Player.FirstName;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer1.Player.LastName;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer1.Player.Height;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer1.Player.Weight;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer1.Player.Kicks;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer1.TeamRoster.UniformNumber;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer1.Player.PlayerID;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer1.JerseySource;
                        //LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer1.FieldRow;
                        //LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldColumn = copyOfPlayer1.FieldColumn;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer1.PlayerOpacity;
                        break;
                    case 4:
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer1.Player.FirstName;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer1.Player.LastName;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer1.Player.Height;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer1.Player.Weight;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer1.Player.Kicks;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer1.TeamRoster.UniformNumber;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer1.Player.PlayerID;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer1.JerseySource;
                        //LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer1.FieldRow;
                        //LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldColumn = copyOfPlayer1.FieldColumn;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer1.PlayerOpacity;
                        break;
                    case 5:
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer1.Player.FirstName;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer1.Player.LastName;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer1.Player.Height;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer1.Player.Weight;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer1.Player.Kicks;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer1.TeamRoster.UniformNumber;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer1.Player.PlayerID;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer1.JerseySource;
                        //LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer1.FieldRow;
                        //LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldColumn = copyOfPlayer1.FieldColumn;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer1.PlayerOpacity;
                        break;
                }

                //Player1
                switch (copyOfPlayer1.FieldColumn)
                {
                    case 1:
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer2.Player.FirstName;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer2.Player.LastName;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer2.Player.Height;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer2.Player.Weight;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer2.Player.Kicks;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer2.TeamRoster.UniformNumber;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer2.Player.PlayerID;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer2.JerseySource;
                        //LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer2.FieldRow;
                        //LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldColumn = copyOfPlayer2.FieldColumn;
                        LeftCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer2.PlayerOpacity;
                        break;
                    case 2:
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer2.Player.FirstName;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer2.Player.LastName;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer2.Player.Height;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer2.Player.Weight;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer2.Player.Kicks;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer2.TeamRoster.UniformNumber;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer2.Player.PlayerID;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer2.JerseySource;
                        //LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer2.FieldRow;
                        //LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldColumn = copyOfPlayer2.FieldColumn;
                        LeftCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer2.PlayerOpacity;
                        break;
                    case 3:
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer2.Player.FirstName;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer2.Player.LastName;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer2.Player.Height;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer2.Player.Weight;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer2.Player.Kicks;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer2.TeamRoster.UniformNumber;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer2.Player.PlayerID;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer2.JerseySource;
                        //LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer2.FieldRow;
                        //LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldColumn = copyOfPlayer2.FieldColumn;
                        LeftCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer2.PlayerOpacity;
                        break;
                    case 4:
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer2.Player.FirstName;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer2.Player.LastName;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer2.Player.Height;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer2.Player.Weight;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer2.Player.Kicks;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer2.TeamRoster.UniformNumber;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer2.Player.PlayerID;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer2.JerseySource;
                        //LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer2.FieldRow;
                        //LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldColumn = copyOfPlayer2.FieldColumn;
                        LeftGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer2.PlayerOpacity;
                        break;
                    case 5:
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer2.Player.FirstName;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer2.Player.LastName;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer2.Player.Height;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer2.Player.Weight;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer2.Player.Kicks;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer2.TeamRoster.UniformNumber;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer2.Player.PlayerID;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer2.JerseySource;
                        //LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer2.FieldRow;
                        //LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldColumn = copyOfPlayer2.FieldColumn;
                        LeftSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer2.PlayerOpacity;
                        break;
                }
            }
            else
            {
                switch (copyOfPlayer1.FieldColumn)
                {
                    case 1:
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer2.Player.FirstName;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer2.Player.LastName;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer2.Player.Height;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer2.Player.Weight;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer2.Player.Kicks;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer2.TeamRoster.UniformNumber;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer2.Player.PlayerID;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer2.JerseySource;
                        //RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer2.FieldRow;
                        //RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldColumn = copyOfPlayer2.FieldColumn;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer2.PlayerOpacity;
                        break;
                    case 2:
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer2.Player.FirstName;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer2.Player.LastName;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer2.Player.Height;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer2.Player.Weight;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer2.Player.Kicks;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer2.TeamRoster.UniformNumber;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer2.Player.PlayerID;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer2.JerseySource;
                        //RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer2.FieldRow;
                        //RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldColumn = copyOfPlayer2.FieldColumn;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer2.PlayerOpacity;
                        break;
                    case 3:
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer2.Player.FirstName;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer2.Player.LastName;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer2.Player.Height;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer2.Player.Weight;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer2.Player.Kicks;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer2.TeamRoster.UniformNumber;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer2.Player.PlayerID;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer2.JerseySource;
                        //RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer2.FieldRow;
                        //RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldColumn = copyOfPlayer2.FieldColumn;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer2.PlayerOpacity;
                        break;
                    case 4:
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer2.Player.FirstName;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer2.Player.LastName;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer2.Player.Height;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer2.Player.Weight;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer2.Player.Kicks;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer2.TeamRoster.UniformNumber;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer2.Player.PlayerID;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer2.JerseySource;
                        //RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer2.FieldRow;
                        // RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldColumn = copyOfPlayer2.FieldColumn;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer2.PlayerOpacity;
                        break;
                    case 5:
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer2.Player.FirstName;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer2.Player.LastName;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer2.Player.Height;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer2.Player.Weight;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer2.Player.Kicks;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer2.TeamRoster.UniformNumber;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer2.Player.PlayerID;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer2.JerseySource;
                        //RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer2.FieldRow;
                        //RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().FieldColumn = copyOfPlayer2.FieldColumn;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer1.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer2.PlayerOpacity;
                        break;
                }

                //Player2
                switch (copyOfPlayer2.FieldColumn)
                {
                    case 1:
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer1.Player.FirstName;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer1.Player.LastName;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer1.Player.Height;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer1.Player.Weight;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer1.Player.Kicks;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer1.TeamRoster.UniformNumber;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer1.Player.PlayerID;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer1.JerseySource;
                        //RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer1.FieldRow;
                        //RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldColumn = copyOfPlayer1.FieldColumn;
                        RightCol1.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer1.PlayerOpacity;
                        break;
                    case 2:
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer1.Player.FirstName;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer1.Player.LastName;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer1.Player.Height;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer1.Player.Weight;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer1.Player.Kicks;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer1.TeamRoster.UniformNumber;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer1.Player.PlayerID;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer1.JerseySource;
                        //RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer1.FieldRow;
                        //RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldColumn = copyOfPlayer1.FieldColumn;
                        RightCol2.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer1.PlayerOpacity;
                        break;
                    case 3:
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer1.Player.FirstName;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer1.Player.LastName;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer1.Player.Height;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer1.Player.Weight;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer1.Player.Kicks;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer1.TeamRoster.UniformNumber;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer1.Player.PlayerID;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer1.JerseySource;
                        // RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer1.FieldRow;
                        //RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldColumn = copyOfPlayer1.FieldColumn;
                        RightCol3.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer1.PlayerOpacity;
                        break;
                    case 4:
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer1.Player.FirstName;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer1.Player.LastName;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer1.Player.Height;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer1.Player.Weight;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer1.Player.Kicks;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer1.TeamRoster.UniformNumber;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer1.Player.PlayerID;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer1.JerseySource;
                        //RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer1.FieldRow;
                        //RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldColumn = copyOfPlayer1.FieldColumn;
                        RightGoalie.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer1.PlayerOpacity;
                        break;
                    case 5:
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.FirstName = copyOfPlayer1.Player.FirstName;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.LastName = copyOfPlayer1.Player.LastName;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Height = copyOfPlayer1.Player.Height;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Weight = copyOfPlayer1.Player.Weight;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.Kicks = copyOfPlayer1.Player.Kicks;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().TeamRoster.UniformNumber = copyOfPlayer1.TeamRoster.UniformNumber;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().ToggledForSub = false;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().Player.PlayerID = copyOfPlayer1.Player.PlayerID;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().JerseySource = copyOfPlayer1.JerseySource;
                        //RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldRow = copyOfPlayer1.FieldRow;
                        //RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().FieldColumn = copyOfPlayer1.FieldColumn;
                        RightSubs.Where(x => x.UniqueIdentifier == copyOfPlayer2.UniqueIdentifier).FirstOrDefault().PlayerOpacity = copyOfPlayer1.PlayerOpacity;
                        break;
                }
            }

            SetPlayerToSubMode(_visitorPlayer1ToSwap, false);
            SetPlayerToSubMode(_visitorPlayer2ToSwap, false);

            _visitorPlayer1ToSwap = null;
            _visitorPlayer2ToSwap = null;
        }

        private string GetClockUiTime(TimeSpan ts)
        {
            string returnValue = string.Empty;
            int actualMinutes = ClockTime.Minutes + (ClockTime.Hours * 60);
            int actualSeconds = ClockTime.Seconds;

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

            if (Game.Game.GameStatus.ToUpper().Equals("FINAL"))
            {
                returnValue = "Final";
            }
            else
            {
                returnValue = minutesString + ":" + secondsString;
            }

            return returnValue;
        }

        #region properties

        private string _clockButtonSource;
        public string ClockButtonSource
        {
            get { return _clockButtonSource; }
            set { _clockButtonSource = value; NotifyPropertyChanged("ClockButtonSource"); }
        }

        private TimeSpan _clockAdjusterTime;
        public TimeSpan ClockAdjusterTime
        {
            get { return _clockAdjusterTime; }
            set { _clockAdjusterTime = value; NotifyPropertyChanged("ClockAdjusterTime"); }
        }


        private double _minWidthOfOneListBox;
        public double MinWidthOfOneListBox
        {
            get { return _minWidthOfOneListBox; }
            set { _minWidthOfOneListBox = value; NotifyPropertyChanged("MinWidthOfOneListBox"); }
        }


        private bool _gMEnabled = true;
        public bool GMEnabled
        {
            get { return _gMEnabled; }
            set { _gMEnabled = value; NotifyPropertyChanged("GMEnabled"); }
        }


        private GameModel _game;
        public GameModel Game
        {
            get { return _game; }
            set { _game = value; NotifyPropertyChanged("Game"); }
        }

        private List<TeamRosterModel> _allHomePlayers;
        public List<TeamRosterModel> AllHomePlayers
        {
            get { return _allHomePlayers; }
            set { _allHomePlayers = value; NotifyPropertyChanged("AllHomePlayers"); }
        }

        private List<TeamRosterModel> _allVisitorPlayers;
        public List<TeamRosterModel> AllVisitorPlayers
        {
            get { return _allVisitorPlayers; }
            set { _allVisitorPlayers = value; NotifyPropertyChanged("AllVisitorPlayers"); }
        }

        private TimeSpan _clockTime;
        public TimeSpan ClockTime
        {
            get { return _clockTime; }
            set
            {
                _clockTime = value;
                NotifyPropertyChanged("ClockTime");
                ClockTimeUi = GetClockUiTime(ClockTime);
            }
        }

        private string _clockTimeUi;
        public string ClockTimeUi
        {
            get { return _clockTimeUi; }
            set { _clockTimeUi = value; NotifyPropertyChanged("ClockTimeUi"); }
        }

        private string _homeTeamName;
        public string HomeTeamName
        {
            get { return _homeTeamName; }
            set { _homeTeamName = value; NotifyPropertyChanged("HomeTeamName"); }
        }

        private string _visitorTeamName;
        public string VisitorTeamName
        {
            get { return _visitorTeamName; }
            set { _visitorTeamName = value; NotifyPropertyChanged("VisitorTeamName"); }
        }

        private bool _statsMode;
        public bool StatsMode
        {
            get { return _statsMode; }
            set
            {
                _statsMode = value;
                NotifyPropertyChanged("StatsMode");
            }
        }

        private bool _subMode;
        public bool SubMode
        {
            get { return _subMode; }
            set
            {
                _subMode = value;
                NotifyPropertyChanged("SubMode");
            }
        }

        private double _heightOfScreen;
        public double HeightOfScreen
        {
            get { return _heightOfScreen; }
            set { _heightOfScreen = value; NotifyPropertyChanged("HeightOfScreen"); }
        }

        private double _widthOfScreen;
        public double WidthOfScreen
        {
            get { return _widthOfScreen; }
            set { _widthOfScreen = value; NotifyPropertyChanged("WidthOfScreen"); }
        }

        private ObservableCollection<TeamRosterModel> _rightCol1;
        public ObservableCollection<TeamRosterModel> RightCol1
        {
            get { return _rightCol1; }
            set { _rightCol1 = value; base.NotifyPropertyChanged("RightCol1"); }
        }

        private ObservableCollection<TeamRosterModel> _rightCol2;
        public ObservableCollection<TeamRosterModel> RightCol2
        {
            get { return _rightCol2; }
            set { _rightCol2 = value; base.NotifyPropertyChanged("RightCol2"); }
        }

        private ObservableCollection<TeamRosterModel> _rightCol3;
        public ObservableCollection<TeamRosterModel> RightCol3
        {
            get { return _rightCol3; }
            set { _rightCol3 = value; base.NotifyPropertyChanged("RightCol3"); }
        }

        private ObservableCollection<TeamRosterModel> _rightUnknownPlayer;
        public ObservableCollection<TeamRosterModel> RightUnknownPlayer
        {
            get { return _rightUnknownPlayer; }
            set { _rightUnknownPlayer = value; base.NotifyPropertyChanged("RightUnknownPlayer"); }
        }

        private ObservableCollection<TeamRosterModel> _rightSpaceHolder;
        public ObservableCollection<TeamRosterModel> RightSpaceHolder
        {
            get { return _rightSpaceHolder; }
            set { _rightSpaceHolder = value; base.NotifyPropertyChanged("RightSpaceHolder"); }
        }

        private ObservableCollection<TeamRosterModel> _leftSpaceHolder;
        public ObservableCollection<TeamRosterModel> LeftSpaceHolder
        {
            get { return _leftSpaceHolder; }
            set { _leftSpaceHolder = value; base.NotifyPropertyChanged("LeftSpaceHolder"); }
        }



        private ObservableCollection<TeamRosterModel> _rightGoalie;
        public ObservableCollection<TeamRosterModel> RightGoalie
        {
            get { return _rightGoalie; }
            set { _rightGoalie = value; }
        }

        private ObservableCollection<TeamRosterModel> _leftCol1;
        public ObservableCollection<TeamRosterModel> LeftCol1
        {
            get { return _leftCol1; }
            set { _leftCol1 = value; base.NotifyPropertyChanged("LeftCol1"); }
        }

        private ObservableCollection<TeamRosterModel> _leftCol2;
        public ObservableCollection<TeamRosterModel> LeftCol2
        {
            get { return _leftCol2; }
            set { _leftCol2 = value; base.NotifyPropertyChanged("LeftCol2"); }
        }

        private ObservableCollection<TeamRosterModel> _leftCol3;
        public ObservableCollection<TeamRosterModel> LeftCol3
        {
            get { return _leftCol3; }
            set { _leftCol3 = value; base.NotifyPropertyChanged("LeftCol3"); }
        }

        private ObservableCollection<TeamRosterModel> _leftUnknownPlayer;
        public ObservableCollection<TeamRosterModel> LeftUnknownPlayer
        {
            get { return _leftUnknownPlayer; }
            set { _leftUnknownPlayer = value; base.NotifyPropertyChanged("LeftUnknownPlayer"); }
        }

        private ObservableCollection<TeamRosterModel> _leftGoalie;
        public ObservableCollection<TeamRosterModel> LeftGoalie
        {
            get { return _leftGoalie; }
            set { _leftGoalie = value; NotifyPropertyChanged("LeftGoalie"); }
        }

        private ObservableCollection<TeamRosterModel> _rightSubs;
        public ObservableCollection<TeamRosterModel> RightSubs
        {
            get { return _rightSubs; }
            set { _rightSubs = value; NotifyPropertyChanged("RightSubs"); }
        }

        private ObservableCollection<TeamRosterModel> _leftSubs;
        public ObservableCollection<TeamRosterModel> LeftSubs
        {
            get { return _leftSubs; }
            set { _leftSubs = value; NotifyPropertyChanged("LeftSubs"); }
        }

        private string _rightTeamName;
        public string RightTeamName
        {
            get { return _rightTeamName; }
            set { _rightTeamName = value; NotifyPropertyChanged("RightTeamName"); }
        }

        private int _rightTeamScore;
        public int RightTeamScore
        {
            get { return _rightTeamScore; }
            set { _rightTeamScore = value; NotifyPropertyChanged("RightTeamScore"); }
        }

        private int _rightTeamShootOutGoals;
        public int RightTeamShootOutGoals
        {
            get { return _rightTeamShootOutGoals; }
            set { _rightTeamShootOutGoals = value; NotifyPropertyChanged("RightTeamShootOutGoals"); }
        }

        private string _leftTeamName;
        public string LeftTeamName
        {
            get { return _leftTeamName; }
            set { _leftTeamName = value; NotifyPropertyChanged("LeftTeamName"); }
        }

        private int _leftTeamScore;
        public int LeftTeamScore
        {
            get { return _leftTeamScore; }
            set { _leftTeamScore = value; NotifyPropertyChanged("LeftTeamScore"); }
        }

        private int _leftTeamShootOutGoals;
        public int LeftTeamShootOutGoals
        {
            get { return _leftTeamShootOutGoals; }
            set { _leftTeamShootOutGoals = value; NotifyPropertyChanged("LeftTeamShootOutGoals"); }
        }

        private int _shootOutOpps;
        public int ShootOutOpps
        {
            get { return _shootOutOpps; }
            set { _shootOutOpps = value; NotifyPropertyChanged("ShootOutOpps"); }
        }

        private string _timeOfSubstitution;
        public string TimeOfSubstitution
        {
            get { return _timeOfSubstitution; }
            set { _timeOfSubstitution = value; NotifyPropertyChanged("TimeOfSubstitution"); }
        }


        #endregion properties

        #region commands

        #region advanceperiodcommand

        private ICommand _advancePeriodCommand;
        public ICommand AdvancePeriodCommand
        {
            get
            {
                if (_advancePeriodCommand == null)
                {
                    _advancePeriodCommand = new DelegateCommand(param => this.AdvancePeriod(false), param => true);
                }

                return _advancePeriodCommand;
            }
        }

        public void AdvancePeriod(bool automatedCall)
        {
            //If the game is final, don't allow the user to advance the period.
            if (!Game.Game.GameStatus.ToUpper().Equals("FINAL"))
            {
                if (IsGameOver())
                {
                    //If it is not an automated call, prompt the user
                    if (!automatedCall)
                    {
                        MessageBoxResult res = MessageBox.Show("Do you wish to end the game?", "End Game?", MessageBoxButton.OKCancel);

                        if (res == MessageBoxResult.OK)
                        {
                            //Set the game to final
                            DAL.Instance().EndGame(Game.Game.GameID, ClockTimeUi);

                            ClockTimeUi = "Final";
                            StopClock();
                        }
                    }
                    else
                    {
                        //Set the game to final
                        DAL.Instance().EndGame(Game.Game.GameID, ClockTimeUi);

                        ClockTimeUi = "Final";
                        StopClock();
                    }
                }
                else
                {
                    if (!automatedCall)
                    {
                        MessageBoxResult res = MessageBox.Show(AppResources.AdvanceToNextPeriod, AppResources.Advance, MessageBoxButton.OKCancel);
                        if (res == MessageBoxResult.OK)
                        {
                            //Advance the period
                            DAL.Instance().AdvancePeriod(Game.Game.GameID, Game.Game.CurrentPeriod, ClockTimeUi);

                            Game = DAL.Instance().GetGame(Game.Game.GameID);
                            //Set the NEW Clock Time
                            StopClock();
                            ClockTime = TimeSpan.FromSeconds(GetSecondsFromCurrentClockString(Game.Game.CurrentClock));
                            Clock.SetClock(TimeSpan.FromSeconds(GetSecondsFromCurrentClockString(Game.Game.CurrentClock)));
                        }
                    }
                    else
                    {
                        //Advance the period
                        DAL.Instance().AdvancePeriod(Game.Game.GameID, Game.Game.CurrentPeriod, ClockTimeUi);

                        Game = DAL.Instance().GetGame(Game.Game.GameID);
                        //Set the NEW Clock Time
                        StopClock();
                        ClockTime = TimeSpan.FromSeconds(GetSecondsFromCurrentClockString(Game.Game.CurrentClock));
                        Clock.SetClock(TimeSpan.FromSeconds(GetSecondsFromCurrentClockString(Game.Game.CurrentClock)));
                    }
                }
            }

            Game = DAL.Instance().GetGame(Game.Game.GameID);

        }

        private bool IsGameOver()
        {
            bool returnValue = false;

            //end of regular periods, game is not tied
            if (Game.Game.Periods == Game.Game.CurrentPeriod && Game.Game.HomeTeamScore != Game.Game.AwayTeamScore)
            {
                returnValue = true;
            }

            //end of regulation, game tied, but no overtimes
            if (Game.Game.Periods == Game.Game.CurrentPeriod && !Game.Game.HasOverTime)
            {
                returnValue = true;
            }

            //End of OT 1, Game is not tied
            if (Game.Game.Periods + 1 == Game.Game.CurrentPeriod && Game.Game.HomeTeamScore != Game.Game.AwayTeamScore)
            {
                returnValue = true;
            }

            //End of OT 2, Game is over
            if (Game.Game.Periods + 2 == Game.Game.CurrentPeriod)
            {
                returnValue = true;
            }

            return returnValue;
        }

        #endregion advanceperiodcommand

        #region addhomeplayer

        private ICommand _addHomePlayerCommand;
        public ICommand AddHomePlayerCommand
        {
            get
            {
                if (_addHomePlayerCommand == null)
                {
                    _addHomePlayerCommand = new DelegateCommand(param => this.AddRightPlayer(), param => true);
                }

                return _addHomePlayerCommand;
            }
        }

        public void AddRightPlayer()
        {
            int teamId = Game.Game.AwayTeamSideOfField.ToUpper().Equals("RIGHT") ? Game.Game.AwayTeamID : Game.Game.HomeTeamID;
            string teamName = Game.Game.AwayTeamSideOfField.ToUpper().Equals("RIGHT") ? Game.AwayTeam.TeamName : Game.HomeTeam.TeamName;

            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/PlayerList.xaml?gameID=" + Game.Game.GameID + "&teamID=" + teamId + "&teamName=" + teamName, UriKind.Relative));
        }

        #endregion addhomeplayer

        #region addvisitorplayer

        private ICommand _addVisitorPlayerCommand;
        public ICommand AddVisitorPlayerCommand
        {
            get
            {
                if (_addVisitorPlayerCommand == null)
                {
                    _addVisitorPlayerCommand = new DelegateCommand(param => this.AddLeftPlayer(), param => true);
                }

                return _addVisitorPlayerCommand;
            }
        }

        public void AddLeftPlayer()
        {
            int teamId = Game.Game.AwayTeamSideOfField.ToUpper().Equals("LEFT") ? Game.Game.AwayTeamID : Game.Game.HomeTeamID;
            string teamName = Game.Game.AwayTeamSideOfField.ToUpper().Equals("LEFT") ? Game.AwayTeam.TeamName : Game.HomeTeam.TeamName;

            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/PlayerList.xaml?gameID=" + Game.Game.GameID + "&teamID=" + teamId + "&teamName=" + teamName, UriKind.Relative));
        }

        #endregion addvisitorplayer

        #region modecommand

        private ICommand _modeCommand;
        public ICommand ModeCommand
        {
            get
            {
                if (_modeCommand == null)
                {
                    _modeCommand = new DelegateCommand(param => this.ModeCommandClicked(), param => true);
                }

                return _modeCommand;
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
        private void GoHelpScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Help_Buttons.xaml?screenId=" + Enums.Screen.GameManager, UriKind.Relative));
        }

        public void ModeCommandClicked()
        {
            if (SubMode)
            {
                TimeOfSubstitution = GetClockValueFinalIntoAcct();

                RightCol1.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerOpacity = .5);
                RightCol2.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerOpacity = .5);
                RightCol3.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerOpacity = .5);
                RightGoalie.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerOpacity = .5);
                RightSubs.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerOpacity = .5);

                RightCol1.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerVisibility = Visibility.Visible);
                RightCol2.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerVisibility = Visibility.Visible);
                RightCol3.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerVisibility = Visibility.Visible);
                RightGoalie.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerVisibility = Visibility.Visible);
                RightSubs.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerVisibility = Visibility.Visible);

                LeftCol1.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerVisibility = Visibility.Visible);
                LeftCol2.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerVisibility = Visibility.Visible);
                LeftCol3.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerVisibility = Visibility.Visible);
                LeftGoalie.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerVisibility = Visibility.Visible);
                LeftSubs.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerVisibility = Visibility.Visible);

                LeftCol1.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerOpacity = .5);
                LeftCol2.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerOpacity = .5);
                LeftCol3.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerOpacity = .5);
                LeftGoalie.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerOpacity = .5);
                LeftSubs.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerOpacity = .5);

            }
            else
            {
                RightCol1.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerOpacity = 0);
                RightCol2.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerOpacity = 0);
                RightCol3.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerOpacity = 0);
                RightGoalie.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerOpacity = 0);
                RightSubs.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerOpacity = 0);

                RightCol1.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerVisibility = Visibility.Collapsed);
                RightCol2.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerVisibility = Visibility.Collapsed);
                RightCol3.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerVisibility = Visibility.Collapsed);
                RightGoalie.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerVisibility = Visibility.Collapsed);
                RightSubs.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerVisibility = Visibility.Collapsed);

                RightCol1.ToList().ForEach(x => x.ToggledForSub = false);
                RightCol2.ToList().ForEach(x => x.ToggledForSub = false);
                RightCol3.ToList().ForEach(x => x.ToggledForSub = false);
                RightGoalie.ToList().ForEach(x => x.ToggledForSub = false);
                RightSubs.ToList().ForEach(x => x.ToggledForSub = false);

                LeftCol1.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerOpacity = 0);
                LeftCol2.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerOpacity = 0);
                LeftCol3.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerOpacity = 0);
                LeftGoalie.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerOpacity = 0);
                LeftSubs.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerOpacity = 0);

                LeftCol1.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerVisibility = Visibility.Collapsed);
                LeftCol2.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerVisibility = Visibility.Collapsed);
                LeftCol3.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerVisibility = Visibility.Collapsed);
                LeftGoalie.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerVisibility = Visibility.Collapsed);
                LeftSubs.Where(x => x.Player.FirstName.ToUpper().Equals(BLANK_PLAYER.ToUpper())).ToList().ForEach(x => x.PlayerVisibility = Visibility.Collapsed);

                LeftCol1.ToList().ForEach(x => x.ToggledForSub = false);
                LeftCol2.ToList().ForEach(x => x.ToggledForSub = false);
                LeftCol3.ToList().ForEach(x => x.ToggledForSub = false);
                LeftGoalie.ToList().ForEach(x => x.ToggledForSub = false);
                LeftSubs.ToList().ForEach(x => x.ToggledForSub = false);
            }
        }

        #endregion modecommand

        #region switchsidescommand

        private ICommand _switchSidesCommand;
        public ICommand SwitchSidesCommand
        {
            get
            {
                if (_switchSidesCommand == null)
                {
                    _switchSidesCommand = new DelegateCommand(param => this.SwitchSidesOfField(), param => true);
                }

                return _switchSidesCommand;
            }
        }

        public void SwitchSidesOfField()
        {
            DAL.Instance().SetHomeTeamSideOfField(Game.Game.GameID, Game.Game.HomeTeamSideOfField.ToUpper().Equals("RIGHT") ? "LEFT" : "RIGHT");
            Game = DAL.Instance().GetGame(Game.Game.GameID);

            LoadTeamNamesAndScores(Game.Game.GameID);

            //RightCol1 = LeftCol1
            //RightCol2 = LeftCol2
            //RightCol3 = LeftCol3
            //RightSubs = LeftSubs
            //RightUnkPlayer = LeftUnkPlayer
            //RightTeamName = LeftTeamName

            //Make Copies of everything
            List<TeamRosterModel> copyRightCol1 = new List<TeamRosterModel>(RightCol1);
            List<TeamRosterModel> copyRightCol2 = new List<TeamRosterModel>(RightCol2);
            List<TeamRosterModel> copyRightCol3 = new List<TeamRosterModel>(RightCol3);
            List<TeamRosterModel> copyOfRightSubs = new List<TeamRosterModel>(RightSubs);
            List<TeamRosterModel> copyOfRightUnkPlayer = new List<TeamRosterModel>(RightUnknownPlayer);
            List<TeamRosterModel> copyOfRightGoalie = new List<TeamRosterModel>(RightGoalie);

            List<TeamRosterModel> copyLeftCol1 = new List<TeamRosterModel>(LeftCol1);
            List<TeamRosterModel> copyLeftCol2 = new List<TeamRosterModel>(LeftCol2);
            List<TeamRosterModel> copyLeftCol3 = new List<TeamRosterModel>(LeftCol3);
            List<TeamRosterModel> copyOfLeftSubs = new List<TeamRosterModel>(LeftSubs);
            List<TeamRosterModel> copyOfLeftUnkPlayer = new List<TeamRosterModel>(LeftUnknownPlayer);
            List<TeamRosterModel> copyOfLeftGoalie = new List<TeamRosterModel>(LeftGoalie);

            //Clear Everything out
            RightCol1.Clear();
            RightCol2.Clear();
            RightCol3.Clear();
            RightSubs.Clear();
            RightUnknownPlayer.Clear();
            RightGoalie.Clear();

            LeftCol1.Clear();
            LeftCol2.Clear();
            LeftCol3.Clear();
            LeftSubs.Clear();
            LeftUnknownPlayer.Clear();
            LeftGoalie.Clear();

            //Swap em
            copyRightCol1.ForEach(x => LeftCol1.Add(x));
            copyRightCol2.ForEach(x => LeftCol2.Add(x));
            copyRightCol3.ForEach(x => LeftCol3.Add(x));
            copyOfRightSubs.ForEach(x => LeftSubs.Add(x));
            copyOfRightUnkPlayer.ForEach(x => LeftUnknownPlayer.Add(x));
            copyOfRightGoalie.ForEach(x => LeftGoalie.Add(x));

            copyLeftCol1.ForEach(x => RightCol1.Add(x));
            copyLeftCol2.ForEach(x => RightCol2.Add(x));
            copyLeftCol3.ForEach(x => RightCol3.Add(x));
            copyOfLeftSubs.ForEach(x => RightSubs.Add(x));
            copyOfLeftUnkPlayer.ForEach(x => RightUnknownPlayer.Add(x));
            copyOfLeftGoalie.ForEach(x => RightGoalie.Add(x));
        }

        #endregion switchsidescommand

        #region clockclickedcommand

        private bool _clockEnabled = true;
        private ICommand _clockClickedCommand;
        public ICommand ClockClickedCommand
        {
            get
            {
                if (_clockClickedCommand == null)
                {
                    _clockClickedCommand = new DelegateCommand(param => this.ClockClicked(), param => _clockEnabled);
                }

                return _clockClickedCommand;
            }
        }

        public void ClockClicked()
        {
            //If the game is final, don't allow the user to do start the clock.
            if (!Game.Game.GameStatus.ToUpper().Equals("FINAL"))
            {
                if (!Clock.IsClockRunning())
                {
                    StartClock();
                }
                else
                {
                    StopClock();
                }

                if (Game.Game.GameStatus.ToUpper().Trim().Replace(" ", "").Equals("NOTSTARTED"))
                {
                    DAL.Instance().StartGame(Game.Game.GameID);
                    Task.Factory.StartNew(() => DAL.Instance().UpdateGameStartedStatForGame(Game.Game.GameID));
                }

                Game = DAL.Instance().GetGame(Game.Game.GameID);
            }
        }

        private void StartClock()
        {
            Clock.StartClock();
            ClockButtonSource = "/Assets/Pause.png";
        }

        private void StopClock()
        {
            Clock.StopClock();
            ClockButtonSource = "/Assets/Play.png";
        }

        private bool _clockWasPreviouslyRunningBeforeBeingAdjusted;
        private bool _clockIsBeingAdjusted = false;
        //This will be called when we adjust the clock.
        public void ClockIsBeingAdjusted()
        {
            _clockIsBeingAdjusted = true;

            if (Clock.IsClockRunning())
            {
                _clockWasPreviouslyRunningBeforeBeingAdjusted = true;
                //We need to stop the clock to allow the user to adjust the clock
                Clock.StopClock();
            }
            else
            {
                _clockWasPreviouslyRunningBeforeBeingAdjusted = false;
            }
        }

        public void ClockIsDoneBeingAdjusted(TimeSpan newTime)
        {
            Clock.SetClock(newTime);
            ClockTime = newTime;
            ClockTimeUi = GetClockUiTime(newTime);

            if (_clockWasPreviouslyRunningBeforeBeingAdjusted)
            {
                Clock.StartClock();
            }

            _clockIsBeingAdjusted = false;
        }

        #endregion clockclickedcommand

        #region adjustclockcommand

        private ICommand _adjustClockCommand;
        public ICommand AdjustClockCommand
        {
            get
            {
                if (_adjustClockCommand == null)
                {
                    _adjustClockCommand = new DelegateCommand(param => AdjustClock(), param => true);
                }

                return _adjustClockCommand;
            }
        }

        Popup _clockAdjusterPopup = new Popup();

        private void clockPopup_CloseOrHidePopup(object sender, EventArgs e)
        {
            //Enable the whole screen
            //GMEnabled = true;
            //_clockAdjusterPopup.IsOpen = false;
        }

        private void tsPickerUnloaded(object sender, RoutedEventArgs e)
        {

        }

        public void AdjustClock()
        {


            //_clockAdjusterPopup.IsOpen = true;

            ////Disable the whole screen
            //GMEnabled = false;

            //_clockPopup.Visibility = Visibility.Visible;
            //_clockPopup.ResetClockDefault();
            //_clockAdjusterPopup.VerticalOffset = 500;

            //_clockAdjusterPopup.HorizontalOffset = _screenWidth - (_screenWidth * .2);
        }

        #endregion adjustclockcommand

        #region editplaycommand

        private ICommand _editPlayCommand;
        public ICommand EditPlayCommand
        {
            get
            {
                if (_editPlayCommand == null)
                {
                    _editPlayCommand = new DelegateCommand(param => EditPlay(), param => true);
                }

                return _editPlayCommand;
            }
        }

        private void EditPlay()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/PlayList.xaml?gameID=" + Game.Game.GameID, UriKind.Relative));

        }

        #endregion adjustclockcommand

        #region undocommand

        private ICommand _undoCommand;
        public ICommand UndoCommand
        {
            get
            {
                if (_undoCommand == null)
                {
                    _undoCommand = new DelegateCommand(param => Undo(), param => true);
                }

                return _undoCommand;
            }
        }

        private void Undo()
        {
            string playTextDeleted = DAL.Instance().UndoLastPlay(Game.Game.GameID);
            if (playTextDeleted != string.Empty)
            {
                MessageBox.Show(playTextDeleted + AppResources.WasUndone, AppResources.Undo, MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show(AppResources.NoPlaysToUndo, AppResources.Undo, MessageBoxButton.OK);
            }
            ReloadGameAfterItHasAlreadyBeenLoaded(Game.Game.GameID);
        }

        #endregion adjustclockcommand

        #region gamestatscommand

        private ICommand _gameStatsCommand;
        public ICommand GameStatsCommand
        {
            get
            {
                if (_gameStatsCommand == null)
                {
                    _gameStatsCommand = new DelegateCommand(param => GameStats(), param => true);
                }

                return _gameStatsCommand;
            }
        }

        private void GameStats()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/PickStatisticsScreen.xaml?gameid=" + Game.Game.GameID, UriKind.Relative));
        }

        #endregion adjustclockcommand

        #endregion commands

        public void Dispose()
        {
            RightCol1.ToList().ForEach(x => x.PlayerTappedByUser -= PlayerTappedByUser);
            RightCol2.ToList().ForEach(x => x.PlayerTappedByUser -= PlayerTappedByUser);
            RightCol3.ToList().ForEach(x => x.PlayerTappedByUser -= PlayerTappedByUser);
            RightGoalie.ToList().ForEach(x => x.PlayerTappedByUser -= PlayerTappedByUser);
            RightSubs.ToList().ForEach(x => x.PlayerTappedByUser -= PlayerTappedByUser);

            LeftCol1.ToList().ForEach(x => x.PlayerTappedByUser -= PlayerTappedByUser);
            LeftCol2.ToList().ForEach(x => x.PlayerTappedByUser -= PlayerTappedByUser);
            LeftCol3.ToList().ForEach(x => x.PlayerTappedByUser -= PlayerTappedByUser);
            LeftGoalie.ToList().ForEach(x => x.PlayerTappedByUser -= PlayerTappedByUser);
            LeftSubs.ToList().ForEach(x => x.PlayerTappedByUser -= PlayerTappedByUser);

            Clock.ClockHasChanged -= Clock_ClockHasChanged;
        }
    }
}
