using ScoreSoccer8.Classes;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ScoreSoccer8.ViewModels
{
    public class FlatTotalsViewModel : Notification
    {

        public FlatTotalsViewModel()
        {
            OutputWindowResults = new ObservableCollection<string>();
        }

        #region "Properties"

        private ObservableCollection<string> _outputWindowResults;
        public ObservableCollection<string> OutputWindowResults
        {
            get { return _outputWindowResults; }
            set { _outputWindowResults = value; NotifyPropertyChanged("OutputWindowResults"); }
        }

        private ObservableCollection<FlatTotalsModel> _flatStats;
        public ObservableCollection<FlatTotalsModel> FlatStats
        {
            get { return _flatStats; }
            set { _flatStats = value; NotifyPropertyChanged("FlatStats"); }
        }

        private ObservableCollection<FlatTotalsModel> _homeFlatStats;
        public ObservableCollection<FlatTotalsModel> HomeFlatStats
        {
            get { return _homeFlatStats; }
            set { _homeFlatStats = value; NotifyPropertyChanged("HomeFlatStats"); }
        }

        private ObservableCollection<FlatTotalsModel> _awayFlatStats;
        public ObservableCollection<FlatTotalsModel> AwayFlatStats
        {
            get { return _awayFlatStats; }
            set { _awayFlatStats = value; NotifyPropertyChanged("AwayFlatStats"); }
        }

        #endregion "Properties"

        #region "Commands"

        private ICommand _runAutomatedTest;
        public ICommand RunAutomatedTest
        {
            get
            {
                if (_runAutomatedTest == null)
                {
                    _runAutomatedTest = new DelegateCommand(param => this.RunAutomatedTesting(), param => true);
                }

                return _runAutomatedTest;
            }
        }

        private ICommand _miscProcessing;
        public ICommand MiscProcessing
        {
            get
            {
                if (_miscProcessing == null)
                {
                    _miscProcessing = new DelegateCommand(param => this.GoToMiscProcessing(), param => true);
                }

                return _miscProcessing;
            }
        }

        private ICommand _playByPlayClickCommand;
        public ICommand PlayByPlayClickCommand
        {
            get
            {
                if (_playByPlayClickCommand == null)
                {
                    _playByPlayClickCommand = new DelegateCommand(param => this.GoToPlayByPlayScreen(), param => true);
                }

                return _playByPlayClickCommand;
            }
        }

        #endregion "Commands"

        #region "Automated Testing"

        private void RunAutomatedTesting()
        {
            OutputWindowResults.Clear();

            OutputWindowResults.Add("Automated Testing Started");
            PlayerStatsAutomatedTest();
          //  TeamStatsAutomatedTest();
          //  PlayerMinutesClockDownAutomatedTest();
          //  PlusMinusAutomatedTest();

            OutputWindowResults.Add("Automated Testing Completed");
        }

        //Game 1 has every possible play/stat 
        private void PlayerStatsAutomatedTest()
        {
            bool didTestPass = true;
            Game game = new Game();

            game = BaseTableDataAccess.Instance().GetGameByGameID(1);

            //Just get away player 2 stats, that is the player with all the stats

            FlatTotalsModel playerFlatsRecord = DAL.Instance().GetPlayersFlatTotalsForGame(game.GameID, game.AwayTeamID, 2);

            if (playerFlatsRecord.FlatTotals.CornerKickExcellent != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " corner kick excellent total = " + playerFlatsRecord.FlatTotals.CornerKickExcellent + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.CornerKickGood != 2)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " corner kick good total = " + playerFlatsRecord.FlatTotals.CornerKickGood + " it should equal 2");
            }

            if (playerFlatsRecord.FlatTotals.CornerKickPoor != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " corner kick poor total = " + playerFlatsRecord.FlatTotals.CornerKickPoor + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.CornerKickForGoal != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " corner kick goal total = " + playerFlatsRecord.FlatTotals.CornerKickForGoal + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.CornerKickTotal != 5)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " corner kick total = " + playerFlatsRecord.FlatTotals.CornerKickTotal + " it should equal 5");
            }

            if (playerFlatsRecord.FlatTotals.CrossExcellent != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " cross excellent total = " + playerFlatsRecord.FlatTotals.CrossExcellent + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.CrossGood != 2)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " cross good total = " + playerFlatsRecord.FlatTotals.CrossGood + " it should equal 2");
            }

            if (playerFlatsRecord.FlatTotals.CrossPoor != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " cross poor total = " + playerFlatsRecord.FlatTotals.CrossPoor + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.CrossTotal != 4)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " cross total = " + playerFlatsRecord.FlatTotals.CrossTotal + " it should equal 4");
            }

            if (playerFlatsRecord.FlatTotals.PassExcellent != 2)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " pass excellent total = " + playerFlatsRecord.FlatTotals.PassExcellent + " it should equal 2");
            }

            if (playerFlatsRecord.FlatTotals.PassGood != 3)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " pass good total = " + playerFlatsRecord.FlatTotals.PassGood + " it should equal 3");
            }

            if (playerFlatsRecord.FlatTotals.PassPoor != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " pass poor total = " + playerFlatsRecord.FlatTotals.PassPoor + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.PassTotal != 6)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " pass total = " + playerFlatsRecord.FlatTotals.PassTotal + " it should equal 6");
            }

            if (playerFlatsRecord.FlatTotals.FoulCommittedHolding != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " foul committed holding total = " + playerFlatsRecord.FlatTotals.FoulCommittedHolding + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.FoulCommittedCharging != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " foul committed charging total = " + playerFlatsRecord.FlatTotals.FoulCommittedCharging + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.FoulCommittedPushing != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " foul committed pushing total = " + playerFlatsRecord.FlatTotals.FoulCommittedPushing + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.FoulCommittedTripping != 2)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " foul committed tripping total = " + playerFlatsRecord.FlatTotals.FoulCommittedTripping + " it should equal 2");
            }

            if (playerFlatsRecord.FlatTotals.FoulCommittedKicking != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " foul committed kicking total = " + playerFlatsRecord.FlatTotals.FoulCommittedKicking + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.FoulCommittedIllegalTackle != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " foul committed illegal tackle total = " + playerFlatsRecord.FlatTotals.FoulCommittedIllegalTackle + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.FoulCommittedTotal != 7)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " foul committed total = " + playerFlatsRecord.FlatTotals.FoulCommittedTotal + " it should equal 7");
            }

            if (playerFlatsRecord.FlatTotals.OutOfBoundsTotal != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " out of bounds total = " + playerFlatsRecord.FlatTotals.OutOfBoundsTotal + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.OffsidesTotal != 2)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " offsides total = " + playerFlatsRecord.FlatTotals.OffsidesTotal + " it should equal 2");
            }

            if (playerFlatsRecord.FlatTotals.ThrowInTotal != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " throw in total = " + playerFlatsRecord.FlatTotals.ThrowInTotal + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.TurnoverIllegalThrowIn != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " turnover illegal throw in total = " + playerFlatsRecord.FlatTotals.TurnoverIllegalThrowIn + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.TurnoverLostDribble != 3)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " turnover lost dribble total = " + playerFlatsRecord.FlatTotals.TurnoverLostDribble + " it should equal 3");
            }

            if (playerFlatsRecord.FlatTotals.TurnoverTotal != 4)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " turnover total = " + playerFlatsRecord.FlatTotals.TurnoverTotal + " it should equal 4");
            }

            if (playerFlatsRecord.FlatTotals.ShotMiss != 4)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " shot miss total = " + playerFlatsRecord.FlatTotals.ShotMiss + " it should equal 4");
            }

            if (playerFlatsRecord.FlatTotals.ShotHitPost != 9)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " shot hit post total = " + playerFlatsRecord.FlatTotals.ShotHitPost + " it should equal 9");
            }

            if (playerFlatsRecord.FlatTotals.ShotBlocked != 9)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " shot blocked total = " + playerFlatsRecord.FlatTotals.ShotBlocked + " it should equal 9");
            }

            if (playerFlatsRecord.FlatTotals.ShotGoal != 4)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " shot goal total = " + playerFlatsRecord.FlatTotals.ShotGoal + " it should equal 4");
            }

            if (playerFlatsRecord.FlatTotals.ShotTotal != 33)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " shot total = " + playerFlatsRecord.FlatTotals.ShotTotal + " it should equal 33");
            }

            if (playerFlatsRecord.FlatTotals.ShotOnGoalTotal != 13)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " shot on goal total = " + playerFlatsRecord.FlatTotals.ShotOnGoalTotal + " it should equal 13");
            }

            if (playerFlatsRecord.FlatTotals.TackleTotal != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " tackle total = " + playerFlatsRecord.FlatTotals.TackleTotal + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.GoalieKickTotal != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " goalie kick total = " + playerFlatsRecord.FlatTotals.GoalieKickTotal + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.FoulDrawnTotal != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " foul drawn total = " + playerFlatsRecord.FlatTotals.FoulDrawnTotal + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.YellowCardDelayingRestartOfPlay != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " yellow card delaying restart of play total = " + playerFlatsRecord.FlatTotals.YellowCardDelayingRestartOfPlay + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.YellowCardUnsportsmanLikeConduct != 2)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " yellow card unsportsmanlike conduct total = " + playerFlatsRecord.FlatTotals.YellowCardUnsportsmanLikeConduct + " it should equal 2");
            }

            if (playerFlatsRecord.FlatTotals.YellowCardTotal != 3)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " yellow card total = " + playerFlatsRecord.FlatTotals.YellowCardTotal + " it should equal 3");
            }

            if (playerFlatsRecord.FlatTotals.RedCardFoulPlay != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " red card foul play total = " + playerFlatsRecord.FlatTotals.RedCardFoulPlay + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.RedCardIllegalHands != 2)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " red card illegal hands total = " + playerFlatsRecord.FlatTotals.RedCardIllegalHands + " it should equal 2");
            }

            if (playerFlatsRecord.FlatTotals.RedCardViolentConduct != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " red card violent conduct total = " + playerFlatsRecord.FlatTotals.RedCardViolentConduct + " it should equal 2");
            }

            if (playerFlatsRecord.FlatTotals.RedCardTotal != 5)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " red card total = " + playerFlatsRecord.FlatTotals.RedCardTotal + " it should equal 5");
            }

            if (playerFlatsRecord.FlatTotals.PenaltyKickMiss != 2)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " penalty kick miss total = " + playerFlatsRecord.FlatTotals.PenaltyKickMiss + " it should equal 2");
            }

            if (playerFlatsRecord.FlatTotals.PenaltyKickHitPost != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " penalty kick hit post total = " + playerFlatsRecord.FlatTotals.PenaltyKickHitPost + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.PenaltyKickBlocked != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " penalty kick blocked total = " + playerFlatsRecord.FlatTotals.PenaltyKickBlocked + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.PenaltyKickGoal != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " penalty kick goal total = " + playerFlatsRecord.FlatTotals.PenaltyKickGoal + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.PenaltyKickTotal != 5)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " penalty kick total = " + playerFlatsRecord.FlatTotals.PenaltyKickTotal + " it should equal 5");
            }

            if (playerFlatsRecord.FlatTotals.IndirectFreeKickTotal != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " indirect free kick total = " + playerFlatsRecord.FlatTotals.IndirectFreeKickTotal + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.DropKickExcellent != 2)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " drop kick excellent total = " + playerFlatsRecord.FlatTotals.DropKickExcellent + " it should equal 2");
            }

            if (playerFlatsRecord.FlatTotals.DropKickGood != 3)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " drop kick good total = " + playerFlatsRecord.FlatTotals.DropKickGood + " it should equal 3");
            }

            if (playerFlatsRecord.FlatTotals.DropKickPoor != 2)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " drop kick poor total = " + playerFlatsRecord.FlatTotals.DropKickPoor + " it should equal 2");
            }

            if (playerFlatsRecord.FlatTotals.DropKickTotal != 7)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " drop kick total = " + playerFlatsRecord.FlatTotals.DropKickTotal + " it should equal 7");
            }

            if (playerFlatsRecord.FlatTotals.DirectFreeKickForGoal != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " direct free kick for goal total = " + playerFlatsRecord.FlatTotals.DirectFreeKickForGoal + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.DirectFreeKickNotForGoal != 2)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " direct free kick not for goal total = " + playerFlatsRecord.FlatTotals.DirectFreeKickNotForGoal + " it should equal 2");
            }

            if (playerFlatsRecord.FlatTotals.DirectFreeKickTotal != 3)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " direct free kick total = " + playerFlatsRecord.FlatTotals.DirectFreeKickTotal + " it should equal 3");
            }

            if (playerFlatsRecord.FlatTotals.ShootoutKickMiss != 2)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " shootout kick miss total = " + playerFlatsRecord.FlatTotals.ShootoutKickMiss + " it should equal 2");
            }

            if (playerFlatsRecord.FlatTotals.ShootoutKickHitPost != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " shootout kick hit post total = " + playerFlatsRecord.FlatTotals.ShootoutKickHitPost + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.ShootoutKickBlocked != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " shootout kick blocked total = " + playerFlatsRecord.FlatTotals.ShootoutKickBlocked + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.ShootoutKickGoal != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " shootout kick goal total = " + playerFlatsRecord.FlatTotals.ShootoutKickGoal + " it should equal 1");
            }

            if (playerFlatsRecord.FlatTotals.ShootoutKickTotal != 5)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " shootout kick total = " + playerFlatsRecord.FlatTotals.ShootoutKickTotal + " it should equal 5");
            }

            if (playerFlatsRecord.FlatTotals.OwnGoalTotal != 1)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + playerFlatsRecord.Player.FirstName + " own goal total = " + playerFlatsRecord.FlatTotals.OwnGoalTotal + " it should equal 1");
            }

            if (didTestPass == true)
            {
                OutputWindowResults.Add("Automated Test passed for " + playerFlatsRecord.Player.FirstName + " game stats Game 1, Away Team.");
            }
            else
            {
                OutputWindowResults.Add("Automated Test failed for " + playerFlatsRecord.Player.FirstName + " game stats Game 1, Away Team.");
            }
        }

        //Game 1 has every possible play/stat 
        private void TeamStatsAutomatedTest()
        {
            bool didTestPass = true;
            Game game = new Game();

            game = BaseTableDataAccess.Instance().GetGameByGameID(1);

            FlatTotalsModel awayTeamFlatStats = DAL.Instance().GetTeamsFlatStatsForGame(game.GameID, game.AwayTeamID);
            FlatTotalsModel homeTeamFlatStats = DAL.Instance().GetTeamsFlatStatsForGame(game.GameID, game.HomeTeamID);

            if (awayTeamFlatStats.FlatTotals.YellowCardTotal != 3)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + awayTeamFlatStats.Team.TeamName + " yellow card total = " + awayTeamFlatStats.FlatTotals.YellowCardTotal + " it should equal 3");
            }

            if (awayTeamFlatStats.FlatTotals.RedCardTotal != 5)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + awayTeamFlatStats.Team.TeamName + " red card total = " + awayTeamFlatStats.FlatTotals.RedCardTotal + " it should equal 5");
            }

            if (awayTeamFlatStats.FlatTotals.CornerKickTotal != 5)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + awayTeamFlatStats.Team.TeamName + " cornerkick total = " + awayTeamFlatStats.FlatTotals.CornerKickTotal + " it should equal 5");
            }

            if (awayTeamFlatStats.FlatTotals.FoulCommittedTotal != 7)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + awayTeamFlatStats.Team.TeamName + " foul committed total = " + awayTeamFlatStats.FlatTotals.FoulCommittedTotal + " it should equal 7");
            }

            if (awayTeamFlatStats.FlatTotals.OffsidesTotal != 3)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + awayTeamFlatStats.Team.TeamName + " offsides total = " + awayTeamFlatStats.FlatTotals.OffsidesTotal + " it should equal 3");
            }

            if (awayTeamFlatStats.FlatTotals.ShotOnGoalTotal != 13)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + awayTeamFlatStats.Team.TeamName + " shots on goal total = " + awayTeamFlatStats.FlatTotals.ShotOnGoalTotal + " it should equal 13");
            }

            if (awayTeamFlatStats.FlatTotals.ShotTotal != 33)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + awayTeamFlatStats.Team.TeamName + " shots total = " + awayTeamFlatStats.FlatTotals.ShotTotal + " it should equal 33");
            }

            //For saves need to check the home team stats as this game had all the plays for the home team (i.e. Jared on Farmington)
            if (homeTeamFlatStats.FlatTotals.SaveTotal != 4)
            {
                didTestPass = false;
                OutputWindowResults.Add("Failed Test -> " + homeTeamFlatStats.Team.TeamName + " save total = " + homeTeamFlatStats.FlatTotals.SaveTotal + " it should equal 4");
            }

            if (didTestPass == true)
            {
                OutputWindowResults.Add("Automated Test passed for Team Stats Game 1, Away Team.");
            }
            else
            {
                OutputWindowResults.Add("Automated Test failed for Team Stats Game 1, Away Team.  Please check output window for details.");
            }
        }

        //Game ID 5 has clock down player minutes testing
        private void PlayerMinutesClockDownAutomatedTest()
        {
            int expectedResult = 0;
            bool didTestPass = true;
            Game game = new Game();

            game = BaseTableDataAccess.Instance().GetGameByGameID(5);

          //  StatCalculationsModule.CalculateALLPlayerMinutes(game.GameID);
         
            HomeFlatStats = DAL.Instance().GetGamesPlayerFlatStats(game.GameID, game.HomeTeamID);
            AwayFlatStats = DAL.Instance().GetGamesPlayerFlatStats(game.GameID, game.AwayTeamID);

            foreach (var item in HomeFlatStats)
            {
                switch (item.Player.FirstName.ToString())
                {
                    case "Ted": expectedResult = 16; break;
                    case "Bill": expectedResult = 18; break;
                    case "John": expectedResult = 22; break;
                    case "Steve": expectedResult = 12; break;
                    case "Bob": expectedResult = 11; break;
                    case "Lou": expectedResult = 8; break;
                    case "Chris": expectedResult = 13; break;
                    default:
                        break;
                }

                if (item.Player.FirstName.ToString() == AppResources.Unknown)
                {
                    expectedResult = 0;
                }

                if (item.FlatTotals.MinutesPlayedTotal != expectedResult)
                {
                    didTestPass = false;
                    OutputWindowResults.Add("Failed Test -> " + item.Player.FirstName + " minutes = " + item.FlatTotals.MinutesPlayedTotal + " it should equal " + expectedResult);
                }
            }

            if (didTestPass == true)
            {
                OutputWindowResults.Add("Automated Test passed for PlayerMinutes Game 6, Home Team.");
            }
            else
            {
                OutputWindowResults.Add("Automated Test failed for PlayerMinutes Game 6, Home Team.  Please check output window for details.");
            }
        }

        //Game ID 1 has 8 goals scored for good plus minus testing
        private void PlusMinusAutomatedTest()
        {
            int expectedResult = 0;
            bool didTestPass = true;
            Game game = new Game();

            game = BaseTableDataAccess.Instance().GetGameByGameID(1);

            //StatCalculationsModule.CalculateALLPlayerPlusMinus(1);
           
            HomeFlatStats = DAL.Instance().GetGamesPlayerFlatStats(game.GameID, game.HomeTeamID);
            AwayFlatStats = DAL.Instance().GetGamesPlayerFlatStats(game.GameID, game.AwayTeamID);

            foreach (var item in HomeFlatStats)
            {
                switch (item.Player.FirstName.ToString())
                {
                    case "Bob": expectedResult = -6; break;
                    case "Lebron": expectedResult = -6; break;
                    case "Gregg": expectedResult = -6; break;
                    case "Cole": expectedResult = -6; break;
                    case "Joseph": expectedResult = -6; break;
                    case "Steve": expectedResult = -6; break;
                    case "Brian": expectedResult = -6; break;
                    case "Lee": expectedResult = -3; break;
                    case "Dominic": expectedResult = 0; break;
                    case "John": expectedResult = 0; break;
                    case "Pete": expectedResult = 0; break;
                    case "Caden": expectedResult = 0; break;
                    case "Mark": expectedResult = -3; break;
                    case "Dave": expectedResult = 0; break;
                    case "Jared": expectedResult = 0; break;
                    case "Unknown": expectedResult = 0; break;
                    default:
                        break;
                }

                if (item.FlatTotals.PlusMinusTotal != expectedResult)
                {
                    didTestPass = false;
                    OutputWindowResults.Add("Failed Test -> " + item.Player.FirstName + " plusminus = " + item.FlatTotals.PlusMinusTotal + " it should equal " + expectedResult);
                }
            }

            if (didTestPass == true)
            {
                OutputWindowResults.Add("Automated Test passed for PlusMinus Game 1, Home Team.");
            }
            else
            {
                OutputWindowResults.Add("Automated Test failed for PlusMinus Game 1, Home Team.  Please check output window for details.");
            }
        }
        //1 Lebron plusminus = -2
        //1 Ben plusminus = -2
        //1 Mark plusminus = -2
        //1 Joseph plusminus = -2
        //1 Brenden plusminus = -2
        //1 Cole plusminus = -2
        //1 Jared plusminus = -2
        //1 Caden plusminus = -1
        //1 Caden plusminus = 0
        //1 Sevie plusminus = 0
        //1 Cam plusminus = 0
        //1 Graham plusminus = 0
        //1 Mathew plusminus = 0
        //1 Unknown plusminus = 0

        //        2 Bob plusminus = 2
        //2 Lebron plusminus = 2
        //2 Gregg plusminus = 2
        //2 Cole plusminus = 2
        //2 Joseph plusminus = 2
        //2 Steve plusminus = 2
        //2 Brian plusminus = 2
        //2 Lee plusminus = 1
        //2 Dominic plusminus = 0
        //2 John plusminus = 0
        //2 Pete plusminus = 0
        //2 Caden plusminus = 0
        //2 Mark plusminus = 1
        //2 Dave plusminus = 0
        //2 Jared plusminus = 0
        //2 Unknown plusminus = 0

        #endregion "Automated Testing"

        #region "Methods"

        private void DeleteAddPlays()
        {

        }

        private void GoToMiscProcessing()
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////// 
            int test;

            TestAddDeletePlayLogic();
            return;

            //   test = DAL.Instance().AdvancePeriod(4, 1, "47:23");
            //   test = DAL.Instance().AdvancePeriod(4, 2, "92:23");
            //   test = DAL.Instance().AdvancePeriod(4, 3, "105:23");
            //   test = DAL.Instance().AdvancePeriod(4, 4, "121:23");
       //     test = DAL.Instance().AdvancePeriod(1, 1, "");
            /////////////////////////////////////////////////////////////////////////////////////////////////////////     
        }

        private void GoToPlayByPlayScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/PlayList.xaml?gameID=1", UriKind.Relative));            
        }

        //TestAddDeletePlayLogic adds and then deletes one of each type of play for game 1 (the game with the 90+ plays).
        //After running TestAddDeletePlayLogic ALL stats should AGAIN pass validation as net result should be unchanged.
        //This would mean our add/undo (i.e. delete) stat processing works.
        private void TestAddDeletePlayLogic()
        {
            //Need to set game to 'IN PROGRESS' otherwise sub transactions will result in change of starters 
    
            for (int i = 1; i <= 100; i++)
            {
                Play play = new Play();
                play.GameID = 1;
             
                switch (i)
                {
                    case 1: //Jared/Shot/Hit Post/Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 2: //Jared/Shot/Hit Post/Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 3: //Jared/Shot/Miss/Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 1;
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 4: //Jared/Yellow Card/Unsportsmanlike Conduct
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Yellow Card"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Unsportsmanlike Conduct");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 5: //Jared/Turnover/Lost Dribble
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Turnover"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Lost Dribble");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 6: //Jared/Shot/GOAL/Shot On Goal/Left (Pete Izzo allowed goal)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 4;
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27;
                        //To simulate realtime data entry (i.e. user from GM screen)
                        Thread.Sleep(2000);
                        break;
                    case 7: //Jared/Shot/Blocked/Shot On Goal/Right/Blocked By other team goalie (Pete save)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "Y"; play.ShotTypeID = 28; play.ShotBlockedByID = 23;
                        break;
                    case 8: //Jared/Shot/Hit Post/Shot Not On Goal/Headed
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 9: //Jared/Corner Kick/Good
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Corner Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 10: //Jared/Shot/Blocked/Shot Not On Goal/Right/Blocked By other team goalie (Pete Izzo blocked)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "N"; play.ShotTypeID = 28; play.ShotBlockedByID = 23;
                        break;
                    case 11: //Jared/Shot/Miss/Shot Not On Goal/Right
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 1;
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "N"; play.ShotTypeID = 28;
                        break;
                    case 12: //Jared/Shot/Blocked/Shot On Goal/Right
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "Y"; play.ShotTypeID = 28;
                        break;
                    case 13: //Jared/Shot/Blocked/Shot On Goal/Right
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "Y"; play.ShotTypeID = 28;
                        break;
                    case 14: //Jared/Shot/Miss/Shot Not On Goal/Headed
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 1;
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "N"; play.ShotTypeID = 29;
                        break;
                    case 15: //Jared/Shot/Hit Post/Shot Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 16: //Jared/Shot/GOAL/Shot On Goal/Left/Assist LEbron (Pete Izzo goal allowed)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 4;
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "Y"; play.AssistID = 2; play.ShotTypeID = 27;
                        //To simulate realtime data entry (i.e. user from GM screen)
                        Thread.Sleep(2000);
                        break;
                    case 17: //Jared/Tackle
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Tackle"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 18: //Jared/Foul Drawn
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Drawn"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 19: //Jared/Shot/GOAL/Shot On Goal/Left (Pete Izzo goal allowed)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 4;
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27;
                        //To simulate realtime data entry (i.e. user from GM screen)
                        Thread.Sleep(2000);
                        break;             
                    case 23: //Jared/Shot/Blocked/Shot On Goal/Left/Blocked by other team goalie (Pete Izzo save)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27; play.ShotBlockedByID = 23;
                        break;
                    case 24: //Jared/Shot/Hit Post/Shot Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;              
                    case 26: //Jared/Indirect Free Kick
                        play.TeamID = 1; play.OtherTeamGoalieID = 11; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Indirect Free Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 27: //Jared/Foul/Holding
                        play.TeamID = 1; play.OtherTeamGoalieID = 11; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Holding");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 28: //Jared/Shot/Hit Post/Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 29: //Jared/Shot/Hit Post/Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 30: //Jared/Shot/Hit Post/Shot On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27;
                        break;
                    case 31: //Jared/Shot/Hit Post/Shot On Goal/Right
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "Y"; play.ShotTypeID = 28;
                        break;
                    case 32: //Jared/Shot/Blocked/Shot On Goal/Right/Blocked By other team goalie (Pete Izzo save)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "Y"; play.ShotTypeID = 28; play.ShotBlockedByID = 23;
                        break;
                    case 33: //Jared/Shot/Blocked/Shot On Goal/Right/Blocked By other team goalie (Pete Izzo save)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "Y"; play.ShotTypeID = 28; play.ShotBlockedByID = 23;
                        break;
                    case 34: //Jared/Shot/Blocked/Shot Not On Goal/Right/Blocked By other team goalie (Pete Izzo blocked)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "N"; play.ShotTypeID = 28; play.ShotBlockedByID = 23;
                        break;
                    case 35: //Jared/Shot/Blocked/Shot Not On Goal/Right/Blocked By other team goalie (Pete Izzo blocked)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "N"; play.ShotTypeID = 28; play.ShotBlockedByID = 23;
                        break;
                    case 36: //Jared/Shot/GOAL/Shot On Goal/Left/Assist Lebron (Pete Izzo goal allowed)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 4;
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "Y"; play.AssistID = 1; play.ShotTypeID = 27;
                        //To simulate realtime data entry (i.e. user from GM screen)
                        Thread.Sleep(2000);
                        break;
                    case 37: //Jared/Turnover/Lost Dribble
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Turnover"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Lost Dribble");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 38: //Unknown/Offsides
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = -1;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Offsides"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 39: //Jared/Offsides
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Offsides"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 40: //Jared/Offsides
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Offsides"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "05:12";
                        break;                  
                    case 42: //Jared/Turnover/Illegal Throw In
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Turnover"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Illegal Throw In");
                        play.Period = 1; play.GameTime = "05:12";
                        break;               
                    case 44: //Jared/Pass/Excellent
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 2; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Excellent");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 45: //Jared/Pass/Good
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 2; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 46: //Jared/Pass/Poor
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 2; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Poor");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 47: //Jared/Foul Committed/Kicking
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Kicking");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 48: //Jared/Foul Committed/Tripping
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Tripping");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 49: //Jared/Foul Committed/Charging
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Charging");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 50: //Jared/Foul Committed/Pushing
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Pushing");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 51: //Jared/Foul Committed/Illegal Tackle
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Illegal Tackle");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 52: //Jared/Out Of Bounds
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Out Of Bounds"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 53: //Jared/Cross/Excellent
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Cross"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Excellent");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 54: //Jared/Cross/Good
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Cross"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 55: //Jared/Cross/Poor
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Cross"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Poor");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 56: //Jared/Throw IN
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Throw In"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Poor");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 57: //Jared/Corner Kick/Excellent
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Corner Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Excellent");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 58: //Jared/Corner Kick/Poor
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Corner Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Poor");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 59: //Jared/Corner Kick/For Goal
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Corner Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("For Goal");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 60: //Jared/Goalie Kick
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Goalie Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 61: //Jared/Drop Kick/Excellent
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Excellent");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 62: //Jared/Drop Kick//Good
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 63: //Jared/Drop Kick/Poor
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Poor");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 64: //Jared/Direct Free Kick/Not For Goal
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Direct Free Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Not For Goal");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 65: //Jared/Direct Free Kick/For Goal
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Direct Free Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("For Goal");
                        play.Period = 1; play.GameTime = "05:12"; play.ShotOnGoal = "Y";
                        break;
                    case 68: //Jared/Penalty Kick/Miss
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Penalty Kick"); play.StatDescriptionID = 1;
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 69: //Jared/Penalty Kick/Hit Post
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Penalty Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 70: //Jared/Penalty Kick/Blocked
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Penalty Kick"); play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 71: //Jared/Penalty Kick/Goal
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Penalty Kick"); play.StatDescriptionID = 4;
                        play.Period = 1; play.GameTime = "05:12";
                        //To simulate realtime data entry (i.e. user from GM screen)
                        Thread.Sleep(2000);
                        break;
                    case 72: //Jared/Yellow Card/Delaying Start Of Play
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Yellow Card"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Delaying Restart Of Play");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 73: //Jared/Red Card/Foul Play
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Red Card"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Foul Play");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 74: //Jared/Red Card/Foul Play
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Red Card"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Violent Conduct");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 75: //Jared/Red Card/Foul Play
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Red Card"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Illegal Hands");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 76: //Jared/Red Card/Foul Play
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Red Card"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Second Yellow Card");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 77: //Jared/Drop Kick/Excellent
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Excellent");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 78: //Jared/Drop Kick/Good
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 79: //Jared/Drop Kick/Poor
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Poor");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 80: //Jared/Shootout Kick/Miss
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Shootout Kick"); play.StatDescriptionID = 1;
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 81: //Jared/Shootout Kick/Hit Post
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Shootout Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 82: //Jared/Shootout Kick/Blocked
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Shootout Kick"); play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 83: //Jared/Shootout Kick/Goal
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Shootout Kick"); play.StatDescriptionID = 4;
                        play.Period = 1; play.GameTime = "05:12";
                        //To simulate realtime data entry (i.e. user from GM screen)
                        Thread.Sleep(2000);
                        break;
                    case 84: //Jared/Pass/Good
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 2; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 85:
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Own Goal"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 86: //Jared/Pass/Excellent
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 2; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Excellent");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 87: //Jared/Shot/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 88: //Jared/Pass/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 2; play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 89: //Jared/Turnover/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Turnover"); play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 90: //Jared/Foul Committed/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 91: //Jared/Cross/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Cross"); play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 92: //Jared/Corner Kick/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Corner Kick"); play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 93: //Jared/Direct Free Kick/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Direct Free Kick"); play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 94: //Jared/Penalty Kick/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Penalty Kick"); play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 95: //Jared/Yellow Card/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Yellow Card"); play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 96: //Jared/Red Card/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Red Card"); play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 97: //Jared/Drop Kick/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 98: //Jared/Shootout Kick/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Shootout Kick"); play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    default:
                        break;
                }

                //To simulate realtime data entry (i.e. user from GM screen)
                //If we do not have this the stats update to quickly due to being upsertplay uses a thread and stats will not calculate correctly.
                Thread.Sleep(300);

                if (play.Player1ID != 0)
                {               
                        DAL.Instance().UpsertPlay(play, "GM");
                        Thread.Sleep(300);
                        DAL.Instance().UndoLastPlay(1);                 
                }
           
            }
            MessageBox.Show("AddDeleteRoutineCompleted");
        }

        #endregion "Methods"
    }
}
