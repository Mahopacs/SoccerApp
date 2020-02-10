using ScoreSoccer8.Classes;
using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Globalization;

//Create a proc to validate the transaction data.
//If player started, then their trans should be out,in,out,in,out,in.....
//If player did not start then their trans should be in,out,in,out,in,out...

//Get all players from event roster for a game
//for each one, get all of their sub transactions.
//if starter, then loop from 1 to transcount...odd ones should be out (i.e. their 1st trans should be out), even ones should be in
//if not starter, then loop from 1 to transcount...odd ones should be in (i.e. their 1st trans should be in), even ones should be out

//If we find a trans that is not valid, add it to a list that will be displayed after proc is complete running.

namespace ScoreSoccer8.Utilities
{
    public class Common
    {

        private static Dictionary<Enums.IconButtonType, string> _iconText;

        public static Common Instance()
        {
            return new Common();
        }

        public static void IconText()
        {
            _iconText.Add(Enums.IconButtonType.OK, AppResources.Ok);
            _iconText.Add(Enums.IconButtonType.Cancel, AppResources.Cancel);
            _iconText.Add(Enums.IconButtonType.Plus, AppResources.Add);
            _iconText.Add(Enums.IconButtonType.Edit, AppResources.Edit);
            _iconText.Add(Enums.IconButtonType.GM, AppResources.GameManager);
            _iconText.Add(Enums.IconButtonType.Stats, AppResources.Statistics);
            _iconText.Add(Enums.IconButtonType.Delete, AppResources.Delete);
            _iconText.Add(Enums.IconButtonType.LeftFoot, AppResources.ShootsLeftFoot);
            _iconText.Add(Enums.IconButtonType.RightFoot, AppResources.ShootsRightFoot);

            _iconText.Add(Enums.IconButtonType.Roster, AppResources.Roster);

            _iconText.Add(Enums.IconButtonType.AddGame, AppResources.AddGame);
            _iconText.Add(Enums.IconButtonType.DeleteGame, AppResources.DeleteGame);
            _iconText.Add(Enums.IconButtonType.EditGame, AppResources.EditGame);

            _iconText.Add(Enums.IconButtonType.AddPlayer, AppResources.AddPlayerToTeamRoster);
            _iconText.Add(Enums.IconButtonType.DeletePlayer, AppResources.DeletePlayerFromRoster);
            _iconText.Add(Enums.IconButtonType.DeletePlayerFromPlayerList, AppResources.DeletePlayerFromPlayerList);

            _iconText.Add(Enums.IconButtonType.AddLeague, AppResources.AddLeague);
            _iconText.Add(Enums.IconButtonType.DeleteLeauge, AppResources.DeleteLeague);

            _iconText.Add(Enums.IconButtonType.ClockUp, AppResources.ClockUp);
            _iconText.Add(Enums.IconButtonType.ClockDown, AppResources.ClockDown);

            _iconText.Add(Enums.IconButtonType.AddNewPlayerToPlayerList, AppResources.AddNewPlayerToPlayerList);
            _iconText.Add(Enums.IconButtonType.AddPlayerToBench, AppResources.AddPlayerToBench);

            _iconText.Add(Enums.IconButtonType.AddTeams, AppResources.AddTeam);
            _iconText.Add(Enums.IconButtonType.DelteTeams, AppResources.DeleteTeam);
            _iconText.Add(Enums.IconButtonType.EditPlayByPlay, AppResources.EditPlayByPlay);

            _iconText.Add(Enums.IconButtonType.Substitution, AppResources.Substitution);
            _iconText.Add(Enums.IconButtonType.Goal, AppResources.Goal);
            _iconText.Add(Enums.IconButtonType.Header, AppResources.Header);
            _iconText.Add(Enums.IconButtonType.HitPost, AppResources.HitPost);
            _iconText.Add(Enums.IconButtonType.Legend, AppResources.Legend);
            _iconText.Add(Enums.IconButtonType.Miss, AppResources.Miss);
            _iconText.Add(Enums.IconButtonType.No, AppResources.No);
            _iconText.Add(Enums.IconButtonType.Yes, AppResources.Yes);
            _iconText.Add(Enums.IconButtonType.PenaltyGoal, AppResources.PenaltyGoal);
            _iconText.Add(Enums.IconButtonType.Period, AppResources.Period);
            _iconText.Add(Enums.IconButtonType.RedCard, AppResources.RedCard);
            _iconText.Add(Enums.IconButtonType.YellowCard, AppResources.YellowCard);
            _iconText.Add(Enums.IconButtonType.Search, AppResources.Search);
            _iconText.Add(Enums.IconButtonType.StatTimeline, AppResources.Timeline);
            _iconText.Add(Enums.IconButtonType.StatGameStats, AppResources.GameStatistics);
            _iconText.Add(Enums.IconButtonType.StatAwayBoxscore, AppResources.AwayBoxscore);
            _iconText.Add(Enums.IconButtonType.StatHomeBoxscore, AppResources.HomeBoxscore);
            _iconText.Add(Enums.IconButtonType.StatPlayByPlay, AppResources.PlayByPlay);
            _iconText.Add(Enums.IconButtonType.StatShare, AppResources.Share);

            _iconText.Add(Enums.IconButtonType.Teams, AppResources.Teams);
            _iconText.Add(Enums.IconButtonType.Leagues, AppResources.Leagues);
            _iconText.Add(Enums.IconButtonType.Players, AppResources.Players);
            _iconText.Add(Enums.IconButtonType.Games, AppResources.Games);

            _iconText.Add(Enums.IconButtonType.AddPlay, AppResources.AddPlay);
            _iconText.Add(Enums.IconButtonType.DeltePlay, AppResources.DeletePlay);

            _iconText.Add(Enums.IconButtonType.ShotOnGoal, AppResources.ShotOnGoal);
            _iconText.Add(Enums.IconButtonType.ShotNotOnGoal, AppResources.ShotNotOnGoal);
            _iconText.Add(Enums.IconButtonType.ShotWithLeftFoot, AppResources.ShotWithLeftFoot);
            _iconText.Add(Enums.IconButtonType.ShotWithRightFoot, AppResources.ShotWithRightFoot);
            _iconText.Add(Enums.IconButtonType.Blocked, AppResources.Blocked);

            _iconText.Add(Enums.IconButtonType.ShotGoal, AppResources.ShotGoal);

            _iconText.Add(Enums.IconButtonType.Undo, AppResources.Undo);
            _iconText.Add(Enums.IconButtonType.GMPeriod, AppResources.ClickToAdjustPeriod);
            _iconText.Add(Enums.IconButtonType.GMClock, AppResources.ClickToAdjustClock);
            _iconText.Add(Enums.IconButtonType.GMStartStop, AppResources.ClickToStartOrStopGameTime);

        }

        public static string GetIconText(Enums.IconButtonType iconType)
        {
            if (_iconText == null)
            {
                _iconText = new Dictionary<Enums.IconButtonType, string>();
                IconText();
            }

            return _iconText.ContainsKey(iconType) ? _iconText[iconType] : "N/A";
        }

        #region "Legend"

        public ObservableCollection<Legend> GetLegendList()
        {
            ObservableCollection<Legend> legendList = new ObservableCollection<Legend>()
            {
                new Legend { ShortName = "#", LongName = "Uniform Number" },
                new Legend { ShortName = "Name", LongName = "Player Name" },
                new Legend { ShortName = "GS", LongName = "Game Started" },
                new Legend { ShortName = "MIN", LongName = "Minutes Played " },
                new Legend { ShortName = "PM", LongName = "Plus Minus" },
                new Legend { ShortName = "GOL", LongName = "Goals" },
                new Legend { ShortName = "AST", LongName = "Assists " },
                new Legend { ShortName = "SOG", LongName = "Shots On Goal" },
                new Legend { ShortName = "2FG", LongName = "2P Field Goal Made" },
                new Legend { ShortName = "SHm", LongName = "Shots Miss" },
                new Legend { ShortName = "SHp", LongName = "Shots Hit Post" },
                new Legend { ShortName = "SHb", LongName = "Shots Blocked" },
                new Legend { ShortName = "FTM", LongName = "FT Made" },
                new Legend { ShortName = "PS+", LongName = "Passes Good" },
                new Legend { ShortName = "PS*", LongName = "Passes Excellent" },
                new Legend { ShortName = "PS-", LongName = "Passes Poor" },
                new Legend { ShortName = "REB", LongName = "Rebounds" },
                new Legend { ShortName = "TOt", LongName = "Turnovers Illegal Throw In" },
                new Legend { ShortName = "TOd", LongName = "Turnovers Lost Dribble" },
                new Legend { ShortName = "STL", LongName = "Offsides " },
                new Legend { ShortName = "TO", LongName = "Fouls Committed " },
                new Legend { ShortName = "FCk", LongName = "Fouls Committed Kicking" },
                new Legend { ShortName = "FCt", LongName = "Fouls Committed Tripping" },
                new Legend { ShortName = "FCc", LongName = "Fouls Committed Charging" },
                new Legend { ShortName = "FCp", LongName = "Fouls Committed Pushing" },
                new Legend { ShortName = "FCh", LongName = "Fouls Committed Holding" },
                new Legend { ShortName = "FCi", LongName = "Fouls Committed Illegal Tackle" },
                new Legend { ShortName = "AST", LongName = "Out Of Bounds" },
                new Legend { ShortName = "BLK", LongName = "Crosses " },
                new Legend { ShortName = "CRS*", LongName = "Cross Excellent" },
                new Legend { ShortName = "CRS+", LongName = "Cross Good" },
                new Legend { ShortName = "CRS-", LongName = "Cross Poor" },
                new Legend { ShortName = "FOU", LongName = "Throw Ins " },
                new Legend { ShortName = "3FG", LongName = "Corners " },
                new Legend { ShortName = "CRN*", LongName = "Corners Excellent" },
                new Legend { ShortName = "CRN+", LongName = "Corners Good" },
                new Legend { ShortName = "CRN-", LongName = "Corners Poor" },
                new Legend { ShortName = "CRNg", LongName = "Corners For Goal" },
                new Legend { ShortName = "2FGA", LongName = "Tackles " },
                new Legend { ShortName = "3FGA", LongName = "Goal Kicks " },
                new Legend { ShortName = "FTA", LongName = "Made Own Goal" },
                new Legend { ShortName = "FD", LongName = "Fouls Drawn" },
                new Legend { ShortName = "DFK", LongName = "Direct Free Kicks " },
                new Legend { ShortName = "DFKn", LongName = "Direct Free Kicks Not For Goal" },
                new Legend { ShortName = "DFKp", LongName = "Direct Free Kicks For Goal" },
                new Legend { ShortName = "IFK", LongName = "Indirect Free Kicks " },
                new Legend { ShortName = "PK", LongName = "Penalty Kicks " },
                new Legend { ShortName = "PKm", LongName = "Penalty Kicks Missed" },
                new Legend { ShortName = "PKp", LongName = "Penalty Kicks Hit Post" },
                new Legend { ShortName = "PKb", LongName = "Penalty Kicks Blocked" },
                new Legend { ShortName = "PKg", LongName = "Penalty Kicks Goal" },
                new Legend { ShortName = "YC", LongName = "Yellow Cards" },
                new Legend { ShortName = "YCu", LongName = "Yellow Cards Unsportsmanship" },
                new Legend { ShortName = "YCd", LongName = "Yellow Cards Delaying Play" },
                new Legend { ShortName = "RC", LongName = "Red Cards" },
                new Legend { ShortName = "RCf", LongName = "Red Cards Foul Play" },
                new Legend { ShortName = "RCv", LongName = "Red Cards Violent Conduct" },
                new Legend { ShortName = "RCh", LongName = "Red Cards Illegal Hands" },
                new Legend { ShortName = "RCy", LongName = "Red Cards Second Yellow Card" },
                new Legend { ShortName = "DK", LongName = "Drop Kicks" },
                new Legend { ShortName = "DK*", LongName = "Drop Kicks Excellent" },
                new Legend { ShortName = "DK+", LongName = "Drop Kicks Good" },
                new Legend { ShortName = "DK-", LongName = "Drop Kicks Poor" },
                new Legend { ShortName = "DB", LongName = "Dribble" },
                new Legend { ShortName = "SK", LongName = "Shootout Kick" },
                new Legend { ShortName = "SKm", LongName = "Shootout Kick Missed" },
                new Legend { ShortName = "SKp", LongName = "Shootout Kick Hit Post" },
                new Legend { ShortName = "SKb", LongName = "Shootout Kick Blocked" },
                new Legend { ShortName = "SKg", LongName = "Shootout Kick Goal" },
                new Legend { ShortName = "BLO", LongName = "Blocks" },
                new Legend { ShortName = "ALW", LongName = "Goals Allowed" },
                new Legend { ShortName = "SV", LongName = "Saves" },
                new Legend { ShortName = "SUB", LongName = "Substitutions" },
                new Legend { ShortName = "MV", LongName = "Moves" },



            };

            int i = 0;
            foreach (Legend item in legendList)
            {
                if (i % 2 != 0)
                {
                    item.BackgroundColor = "Black";
                    item.BackgroundOpacity = 0.2;
                }

                i++;
            }

            return legendList;

        }

        #endregion

        #region "Time"

        public string GetTimelineTime(Play play)
        {
            string toReturn = string.Empty;
            string convertedTime = GetAlternatePlayTime(play.GameID, play.Period, play.GameTime);

            //return convertedTime;


            int i = convertedTime.IndexOf(":");
            int number = 0;
            int number2 = 0;

            if (i > 0)
            {
                int.TryParse(convertedTime.Substring(0, i), out number);
                number += 1;
            }

            int j = convertedTime.IndexOf("+");

            if (j > 0)
            {
                int k = convertedTime.IndexOf(")");
                int.TryParse(convertedTime.Substring(j, k), out number2);
            }

            if (number2 > 0)
            {
                toReturn = number + "+" + number2;
            }
            else
            {
                toReturn = number + "'";
            }

            return toReturn;
        }

        #endregion

        #region "Misc"

        public string ConvertSecondsToClockValue(int seconds)
        {
            try
            {
                string returnValue = string.Empty;
                int actualMinutes = seconds / 60;
                int actualSeconds = seconds % (actualMinutes * 60);

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

                returnValue = minutesString + ":" + secondsString;
                return returnValue;
            }

            catch (Exception)
            {
                return string.Empty;
            }
        }

        public int ConvertStringToNumber(string value)
        {
            int returnValue = 0;

            try
            {
                returnValue = Convert.ToInt32(value);

                return returnValue;
            }

            catch (Exception)
            {
                return returnValue;
            }
        }

        //TJY TO DO, Mark, this could be replaced by LINQ for performance improvement
        public bool IsPlayerInList(List<int?> list, int? playerID)
        {
            bool returnValue = false;

            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (playerID == list[i])
                    {
                        returnValue = true;
                        break;
                    }
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                return returnValue;
            }
        }

        //TJY TO DO, Mark, this could be replaced by LINQ for performance improvement    
        public bool IsPlayerInList(ObservableCollection<TeamRosterModel> list, int? playerID)
        {
            bool returnValue = false;

            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (playerID == list[i].Player.PlayerID)
                    {
                        returnValue = true;
                        break;
                    }
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                return returnValue;
            }
        }

        public int GetStatusGraphNumber(int num1, int num2)
        {
            int toReturn = 0;
            int tempNum = num1;

            if (num2 > num1) tempNum = num2;

            if (tempNum == 0) return 5;

            if (tempNum < 30)
            {
                toReturn = (int)(Math.Ceiling((decimal)tempNum / 5) * 5);
            }

            if (tempNum > 30)
            {
                toReturn = (int)(Math.Ceiling((decimal)tempNum / 10) * 10);
            }

            return toReturn;
        }

        //This is not being used, needs to be tested
        //public  void RebuildGameStatsFromPBPTJYNOTSUREIFWORKINGFORALLSCORINGPLAYS(int gameID)
        //{
        //    GameModel game = new GameModel();
        //    game = DAL.Instance().GetGame(gameID);

        //    //Reset event roster to how it was when game started so subs plays are processed correct
        //    //To do this we need to set anyone who was a starter to on the field and anyone who was not to not on the field
        //    List<EventRoster> eventRosterList = new List<EventRoster>();
        //    eventRosterList = BaseTableDataAccess.Instance().GetEventRosterByGameID(gameID);

        //    foreach (var eventRosterEntry in eventRosterList)
        //    {
        //        if (eventRosterEntry.Starter == "Y")
        //        {
        //            BaseTableDataAccess.Instance().UpdatePlayersOnFieldStatus(gameID, eventRosterEntry.TeamID, eventRosterEntry.PlayerID, "Y");
        //        }
        //        else
        //        {
        //            BaseTableDataAccess.Instance().UpdatePlayersOnFieldStatus(gameID, eventRosterEntry.TeamID, eventRosterEntry.PlayerID, "N");
        //        }
        //    }

        //    //delete all stats for game, we are going to rebuild all the stats from the plays
        //    BaseTableDataAccess.Instance().DeleteEntireGamesStats(gameID);

        //    //Get list of all plays for the game
        //    ObservableCollection<PlayModel> playByPlayList = new ObservableCollection<PlayModel>();
        //    playByPlayList = DAL.Instance().GetPlaysForGame(gameID, "ASC");

        //    //for each play we want to do a light version of UpsertPlay (which is sub processing, shot processing, saving of the stats, and player minutes below)
        //    //by light I mean we do not want to do anything that involves updating game or play table (other than play home and away score)
        //    //we would only want to be updating stat totals

        //    //Set game score to 0-0 so can rebuild running score on all plays
        //    BaseTableDataAccess.Instance().UpdateGameAwayTeamScore(gameID, 0);
        //    BaseTableDataAccess.Instance().UpdateGameHomeTeamScore(gameID, 0);

        //    foreach (var playEntry in playByPlayList)
        //    {
        //        game = DAL.Instance().GetGame(playEntry.Play.GameID);
        //        BaseTableDataAccess.Instance().UpdatePlayAwayTeamScore(game.Game.GameID, playEntry.Play.PlayID, game.Game.AwayTeamScore);
        //        BaseTableDataAccess.Instance().UpdatePlayHomeTeamScore(game.Game.GameID, playEntry.Play.PlayID, game.Game.HomeTeamScore);

        //        if ((playEntry.Play.StatCategoryID == 1) ||
        //          (playEntry.Play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Penalty Kick")) ||
        //          (playEntry.Play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Direct Free Kick")) ||
        //          (playEntry.Play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Corner Kick")) && (DAL.Instance().GetStatDescriptionNameById(playEntry.Play.StatDescriptionID) == "For Goal"))
        //        {
        //            Common.Instance().ShotProcessing(game, playEntry.Play);
        //        }
        //        else if (playEntry.Play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Shootout Kick"))
        //        {
        //            Common.Instance().ShootoutProcessing(game, playEntry.Play);
        //        }
        //        else if (playEntry.Play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Own Goal"))
        //        {
        //            //Update the score in the game table and in the play table              
        //            DAL.Instance().UpdateGameAndPlayScore(playEntry.Play);
        //        }

        //        if (playEntry.Play.StatCategoryID == 22)
        //        {
        //            Common.Instance().SubstitutionProcessing(gameID, playEntry.Play, false);
        //        }

        //        //Now we need to save this stat to the database
        //        DAL.Instance().UpsertStatPlayToFlatTotals(playEntry.Play, false);
        //    }

        //    StatCalculationsModule.CalculateALLPlayerMinutes(game.Game.GameID);
        //    StatCalculationsModule.CalculateALLPlayerPlusMinus(game.Game.GameID);
        //}

        //this will convert 47:08 to 45:00(+2) for a clock up game with a period length of 45 (i.e. it converts to readable time with extra time indicated)
        //47:08 -> 45:00 (+2), 48:45 -> 45:00 (+3), 91:23 -> 90:00 (+1), period 3 (i.e. OT1), clock of 107:23 be -> 105:00 (+2), period 4 (i.e. OT2), clock of 124:54 be -> 120:00 (+4)

        public string GetAlternatePlayTime(int gameID, int period, string clock)
        {
            int extraTimeValue;
            int periodLength;
            int elapsedClockValue;
            int elapsedClockValueBase;
            Game game = new Game();

            try
            {
                game = BaseTableDataAccess.Instance().GetGameByGameID(gameID);

                //If clock down then simply return what was passed in (i.e. there is no concept of extra time so no alternate time to convert to
                if (game.ClockUpOrDown.ToUpper() == "DOWN")
                {
                    return clock;
                }
                else
                {
                    elapsedClockValue = StatCalculationsModule.CalculateTimeElapsedInGameInSeconds(game, period, clock);

                    //Now need to get elsapsed clock time for if the period did not go to extra time (i.e. 1/45:00, 2/90:00, 3/105:00)
                    if (period <= game.Periods)
                    {
                        periodLength = (game.PeriodLength * period);
                    }
                    else
                    {
                        periodLength = (game.PeriodLength * (period - 1)) + game.OverTimeLength;
                    }

                    elapsedClockValueBase = StatCalculationsModule.CalculateTimeElapsedInGameInSeconds(game, period, periodLength + ":00");

                    extraTimeValue = (elapsedClockValue - elapsedClockValueBase) / 60;

                    if (extraTimeValue > 0)
                    {
                        return periodLength + ":00" + " (+" + extraTimeValue.ToString() + ")";
                    }
                    else
                    {
                        return clock;
                    }

                }
            }
            catch (Exception ex)
            {
                return clock;
            }
        }

        public string CreatePlayText(Play play)
        {
            string rtnPlayText = string.Empty;
            string extraShotInfo = string.Empty;
            string categoryName;
            string categoryDescription;
            Player player1;
            Player player2;
            Game game = new Game();

            try
            {
                game = BaseTableDataAccess.Instance().GetGameByGameID(play.GameID);

                player1 = DAL.Instance().GetPlayer(play.Player1ID);

                if (player1.FirstName == AppResources.Unknown)
                {
                    if (play.TeamID == game.HomeTeamID)
                    {
                        player1.FirstName = player1.FirstName + " (" + AppResources.Home + ")";
                    }
                    else
                    {
                        player1.FirstName = player1.FirstName + " (" + AppResources.Away + ")";
                    }
                }

                categoryName = BaseTableDataAccess.Instance().GetStatCategoryByID(play.StatCategoryID).StatCategoryName;
                categoryDescription = DAL.Instance().GetStatDescriptionNameById(play.StatDescriptionID);

                switch (categoryName)
                {
                    case "Shot":
                        string shotType = string.Empty; //left, right, or headed;
                        string shotOnGoal = string.Empty; //Y or N;
                        string shotBlockedByName = string.Empty;
                        string shotAssistName = string.Empty;

                        shotType = DAL.Instance().GetStatDescriptionNameById(play.ShotTypeID).ToUpper();
                        shotOnGoal = play.ShotOnGoal;
                        if (play.ShotBlockedByID != null)
                        {
                            shotBlockedByName = DAL.Instance().GetPlayer((int)play.ShotBlockedByID).FirstName;
                        }

                        if (play.AssistID != null)
                        {
                            shotAssistName = DAL.Instance().GetPlayer((int)play.AssistID).FirstName;
                        }

                        if ((categoryDescription == string.Empty) || (categoryDescription == "Miss"))
                        {
                            //if not show type then user simply clicked on shot, need to set category description to miss and shottype to players kicks value
                            if ((shotType == string.Empty) || (categoryDescription == string.Empty))
                            {
                                play.StatDescriptionID = 1;

                                if (player1.Kicks.ToUpper() == "LEFT")
                                {
                                    play.ShotTypeID = DAL.Instance().GetStatDescriptionIDByName("Left");
                                    shotType = AppResources.Left;
                                }
                                else
                                {
                                    play.ShotTypeID = DAL.Instance().GetStatDescriptionIDByName("Right");
                                    shotType = AppResources.Right;
                                }
                            }
                            rtnPlayText = player1.FirstName + AppResources.TakesAShotAndMisses;
                        }
                        else if (categoryDescription == "Hit Post")
                        {
                            rtnPlayText = player1.FirstName + AppResources.TakesAShotAndHitsThePost;
                        }
                        else if (categoryDescription == "Blocked")
                        {
                            rtnPlayText = player1.FirstName + AppResources.TakesAShotAndItIsBlocked;

                            if (shotBlockedByName != string.Empty)
                            {
                                rtnPlayText = rtnPlayText + AppResources.By + shotBlockedByName;
                            }
                        }
                        else if (categoryDescription == "Goal")
                        {
                            rtnPlayText = player1.FirstName + AppResources.TakesAShotAndScores;

                            if (shotAssistName != string.Empty)
                            {
                                rtnPlayText = rtnPlayText + AppResources.AssistedBy + shotAssistName;
                            }
                        }

                        if (shotType != string.Empty)
                        {
                            if (shotType == "HEADED")
                            {
                                extraShotInfo = " (" + AppResources.Headed;
                            }
                            else if (shotType == "LEFT")
                            {
                                extraShotInfo = " (" + AppResources.LeftFoot;
                            }
                            else
                            {
                                extraShotInfo = " (" + AppResources.RightFoot;
                            }
                        }

                        if (shotOnGoal == "Y")
                        {
                            if (extraShotInfo == string.Empty)
                            {
                                extraShotInfo = " (" + AppResources.ShotOnGoal.ToLower() + ")";
                            }
                            else
                            {
                                extraShotInfo = extraShotInfo + ", " + AppResources.ShotOnGoal.ToLower() + ")";
                            }
                        }
                        else
                        {
                            if (extraShotInfo != string.Empty)
                            {
                                extraShotInfo = extraShotInfo + ")";
                            }
                        }

                        rtnPlayText = rtnPlayText + extraShotInfo;

                        break;
                    case "Pass":
                        if ((categoryDescription == string.Empty) || (categoryDescription == "Good"))
                        {
                            if (categoryDescription == string.Empty)
                            {
                                play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                            }

                            rtnPlayText = player1.FirstName + AppResources.MakesAGoodPass;
                        }
                        else if (categoryDescription == "Excellent")
                        {
                            rtnPlayText = player1.FirstName + AppResources.MakesAnExcellentPass;
                        }

                        else if (categoryDescription == "Poor")
                        {
                            rtnPlayText = player1.FirstName + AppResources.MakesAPoorPass;
                        }
                        break;
                    case "Turnover":
                        if ((categoryDescription == string.Empty) || (categoryDescription == "Lost Dribble"))
                        {
                            if (categoryDescription == string.Empty)
                            {
                                play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Lost Dribble");
                            }

                            rtnPlayText = player1.FirstName + AppResources.LostTheDribble;
                        }
                        else if (categoryDescription == "Illegal Throw In")
                        {
                            rtnPlayText = player1.FirstName + AppResources.HasAnIllegalThrowIn;
                        }
                        break;
                    case "Offsides":
                        rtnPlayText = player1.FirstName + AppResources.IsOffsides;
                        break;
                    case "Foul Committed":
                        if ((categoryDescription == string.Empty) || (categoryDescription == "Tripping"))
                        {
                            if (categoryDescription == string.Empty)
                            {
                                play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Tripping");
                            }

                            rtnPlayText = player1.FirstName + AppResources.CommittsATrippingFoul;
                        }
                        else if (categoryDescription == "Kicking")
                        {
                            rtnPlayText = player1.FirstName + AppResources.CommittsAKickingFoul;
                        }
                        else if (categoryDescription == "Charging")
                        {
                            rtnPlayText = player1.FirstName + AppResources.CommittsAChargingFoul;
                        }
                        else if (categoryDescription == "Pushing")
                        {
                            rtnPlayText = player1.FirstName + AppResources.CommittsAPushingFoul;
                        }
                        else if (categoryDescription == "Holding")
                        {
                            rtnPlayText = player1.FirstName + AppResources.CommittsAHoldingFoul;
                        }
                        else if (categoryDescription == "Illegal Tackle")
                        {
                            rtnPlayText = player1.FirstName + AppResources.CommittsAnIllegalTackleFoul;
                        }
                        break;
                    case "Out Of Bounds":
                        rtnPlayText = player1.FirstName + AppResources.KicksTheBallOutOfBounds;
                        break;
                    case "Cross":
                        if ((categoryDescription == string.Empty) || (categoryDescription == "Good"))
                        {
                            if (categoryDescription == string.Empty)
                            {
                                play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                            }

                            rtnPlayText = player1.FirstName + AppResources.MakesAGoodCross;
                        }
                        else if (categoryDescription == "Excellent")
                        {
                            rtnPlayText = player1.FirstName + AppResources.MakesAnExcellentCross;
                        }
                        else if (categoryDescription == "Poor")
                        {
                            rtnPlayText = player1.FirstName + AppResources.MakesAPoorCross;
                        }
                        break;
                    case "Throw In":
                        rtnPlayText = player1.FirstName + AppResources.HasAThrowIn;
                        break;
                    case "Corner Kick":
                        if ((categoryDescription == string.Empty) || (categoryDescription == "Good"))
                        {
                            if (categoryDescription == string.Empty)
                            {
                                play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                            }

                            rtnPlayText = player1.FirstName + AppResources.TakesAGoodCornerKick;
                        }
                        else if (categoryDescription == "Excellent")
                        {
                            rtnPlayText = player1.FirstName + AppResources.TakesAnExcellentCornerKick;
                        }
                        else if (categoryDescription == "Poor")
                        {
                            rtnPlayText = player1.FirstName + AppResources.TakesAPoorCornerKick;
                        }
                        else if ((categoryDescription == "For Goal") || (categoryDescription == "Goal"))
                        {
                            rtnPlayText = player1.FirstName + AppResources.TakesACornerKickAndScores;
                        }
                        break;
                    case "Tackle":
                        rtnPlayText = player1.FirstName + AppResources.HasATackle;
                        break;
                    case "Goalie Kick":
                        rtnPlayText = player1.FirstName + AppResources.TakesAGoalKick;
                        break;
                    case "Own Goal":
                        rtnPlayText = player1.FirstName + AppResources.KicksBallIntoOwnGoal;
                        break;
                    case "Foul Drawn":
                        rtnPlayText = player1.FirstName + AppResources.DrawsAFoul;
                        break;
                    case "Direct Free Kick":
                        if ((categoryDescription == string.Empty) || (categoryDescription == "Not For Goal"))
                        {
                            if (categoryDescription == string.Empty)
                            {
                                play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Not For Goal");
                            }

                            rtnPlayText = player1.FirstName + AppResources.TakesADirectFreeKick;
                        }
                        else if ((categoryDescription == "For Goal") || (categoryDescription == "Goal"))
                        {
                            rtnPlayText = player1.FirstName + AppResources.TakesADirectFreeKickAndScores;
                        }
                        break;
                    case "Indirect Free Kick":
                        rtnPlayText = player1.FirstName + AppResources.TakesAnIndirectFreeKick;
                        break;
                    case "Penalty Kick":
                        if ((categoryDescription == string.Empty) || (categoryDescription == "Miss"))
                        {
                            if (categoryDescription == string.Empty)
                            {
                                play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Miss");
                            }

                            rtnPlayText = player1.FirstName + AppResources.TakesAPenaltyKickAndMisses;
                        }
                        else if (categoryDescription == "Hit Post")
                        {
                            rtnPlayText = player1.FirstName + AppResources.TakesAPenaltyKickAndHitsThePost;
                        }
                        else if (categoryDescription == "Blocked")
                        {
                            rtnPlayText = player1.FirstName + AppResources.TakesAPenaltyKickAndItIsBlocked;
                        }
                        else if (categoryDescription == "Goal")
                        {
                            rtnPlayText = player1.FirstName + AppResources.TakesAPenaltyKickAndScores;
                        }
                        break;
                    case "Yellow Card":
                        if ((categoryDescription == string.Empty) || (categoryDescription == "Unsportsmanlike Conduct"))
                        {
                            if (categoryDescription == string.Empty)
                            {
                                play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Unsportsmanlike Conduct");
                            }

                            rtnPlayText = player1.FirstName + AppResources.GetsAYellowCardForUnsportsmanlikeConduct;
                        }
                        else if (categoryDescription == "Delaying Restart Of Play")
                        {
                            rtnPlayText = player1.FirstName + AppResources.GetsAYellowCardForDelayingRestartOfPlay;
                        }
                        break;
                    case "Red Card":
                        if ((categoryDescription == string.Empty) || (categoryDescription == "Violent Conduct"))
                        {
                            if (categoryDescription == string.Empty)
                            {
                                play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Violent Conduct");
                            }

                            rtnPlayText = player1.FirstName + AppResources.GetsARedCardForViolentConduct;
                        }
                        else if (categoryDescription == "Illegal Hands")
                        {
                            rtnPlayText = player1.FirstName + AppResources.GetsARedCardForIllegalHands;
                        }
                        else if (categoryDescription == "Second Yellow Card")
                        {
                            rtnPlayText = player1.FirstName + AppResources.GetsARedCardForSecondYellowCard;
                        }
                        else if (categoryDescription == "Foul Play")
                        {
                            rtnPlayText = player1.FirstName + AppResources.GetsARedCardForFoulPlay;
                        }
                        break;
                    case "Drop Kick":
                        if ((categoryDescription == string.Empty) || (categoryDescription == "Good"))
                        {
                            if (categoryDescription == string.Empty)
                            {
                                play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                            }

                            rtnPlayText = player1.FirstName + AppResources.MakesAGoodDropKick;
                        }
                        else if (categoryDescription == "Excellent")
                        {
                            rtnPlayText = player1.FirstName + AppResources.MakesAnExcellentDropKick;
                        }
                        else if (categoryDescription == "Poor")
                        {
                            rtnPlayText = player1.FirstName + AppResources.MakesAPoorDropKick;
                        }
                        break;
                    case "Dribble":
                        rtnPlayText = player1.FirstName + AppResources.Dribbles;
                        break;
                    case "Shootout Kick":
                        if ((categoryDescription == string.Empty) || (categoryDescription == "Miss"))
                        {
                            if (categoryDescription == string.Empty)
                            {
                                play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Miss");
                            }

                            rtnPlayText = player1.FirstName + AppResources.TakesAShootoutKickAndMisses;
                        }
                        else if (categoryDescription == "Hit Post")
                        {
                            rtnPlayText = player1.FirstName + AppResources.TakesAShootoutKickAndHitsThePost;
                        }
                        else if (categoryDescription == "Blocked")
                        {
                            rtnPlayText = player1.FirstName + AppResources.TakesAShootoutKickAndItIsBlocked;
                        }
                        else if (categoryDescription == "Goal")
                        {
                            rtnPlayText = player1.FirstName + AppResources.TakesAShootoutKickAndScores;
                        }
                        break;
                    case "Substitution":
                        player2 = DAL.Instance().GetPlayer(play.Player2ID);

                        if ((player2.PlayerID != 0) && (player1.PlayerID != 0))
                        {
                            rtnPlayText = AppResources.Substitution + " - " + player1.FirstName + " " + player1.LastName + " " + AppResources.In.ToUpper() + ", " +
                                                              player2.FirstName + " " + player2.LastName + " " + AppResources.Out.ToUpper();
                        }
                        else if (player1.PlayerID != 0)
                        {
                            rtnPlayText = AppResources.Substitution + " - " + player1.FirstName + " " + player1.LastName + " " + AppResources.In.ToUpper();
                        }
                        else if (player2.PlayerID != 0)
                        {
                            rtnPlayText = AppResources.Substitution + " - " + player2.FirstName + " " + player2.LastName + " " + AppResources.Out.ToUpper();
                        }

                        break;
                    case "Move":
                        player2 = DAL.Instance().GetPlayer(play.Player2ID);

                        //rtnPlayText = AppResources.Move + " - " + player1.FirstName + " " + player1.LastName + "(" + play.GMPlayer1PositionID + ") " + AppResources.SwitchedPositionsWith +
                        //                                 player2.FirstName + " " + player2.LastName + "(" + play.GMPlayer2PositionID + ")";

                        rtnPlayText = AppResources.Move + " - " + player1.FirstName + " " + player1.LastName + AppResources.SwitchedPositionsWith + player2.FirstName + " " + player2.LastName;
                        break;
                    case "Clock":
                        rtnPlayText = AppResources.ClockAdjustedToPeriod + " = " + play.Period + " " + AppResources.Clock.ToLower() + " = " + play.GameTime;
                        break;
                }
                return rtnPlayText;
            }
            catch (Exception ex)
            {
                rtnPlayText = "Error creating play text.  Error = " + ex.Message;
                return rtnPlayText;
            }
        }

        //use by Game Manager when it is opened for a game to make sure eventroster is intiially loaded correctly
        public void InitiliazeEventRoster(int gameID)
        {
            try
            {
                GameModel _gameDetails = DAL.Instance().GetGame(gameID);

                DAL.Instance().InitiliazeEventRoster(_gameDetails.Game.GameID, _gameDetails.Game.HomeTeamID);
                DAL.Instance().InitiliazeEventRoster(_gameDetails.Game.GameID, _gameDetails.Game.AwayTeamID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing event roster. Error = " + ex.Message);
            }
        }

        public void SetTeamRosterPromptForJersey(int teamID, string teamName, int playerID)
        {
            App.gPromptForJersey = true;
            App.gPromptForJerseyTeamID = teamID;
            App.gPromptForJerseyPlayerID = playerID;
        }

        public void ClearTeamRosterPromptForJersey()
        {
            App.gPromptForJersey = false;
            App.gPromptForJerseyTeamID = 0;
            App.gPromptForJerseyPlayerID = 0;
        }

        public int GetOtherTeamID(int gameID, int teamID)
        {
            Game game = new Game();
            int returnValue = -1;

            try
            {
                game = BaseTableDataAccess.Instance().GetGameByGameID(gameID);

                if (game.HomeTeamID == teamID)
                {
                    returnValue = game.AwayTeamID;
                }
                else
                {
                    returnValue = game.HomeTeamID;
                }

                return returnValue;
            }
            catch (Exception)
            {
                return returnValue;
            }
        }

        //Shot, Penalty Kick, Direct Free Kick, and Corner Kick for goal are all shot plays
        public bool IsThisAShotPlay(Play play)
        {
            bool returnValue = false;

            //if ((play.StatCategoryID == 1) ||
            //    (play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Penalty Kick")) ||
            //    (play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Direct Free Kick")) ||
            //    (play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Corner Kick")) && (DAL.Instance().GetStatDescriptionNameById(play.StatDescriptionID) == "For Goal"))

            if ((play.StatCategoryID == 1) || (play.StatCategoryID == 16) || (play.StatCategoryID == 14) || ((play.StatCategoryID == 9) && (play.StatDescriptionID == 17)))
            {
                returnValue = true;
            }
            else
            {
                returnValue = false;
            }
            return returnValue;
        }


        public string IsThisShootoutGoal(Play play)
        {
            string imagePath = "/Assets/SoccerBall2.png";

            //if (DAL.Instance().GetStatCategoryNameById(play.StatCategoryID) == "Shootout Kick")
            if (play.StatCategoryID == 21)
            {
                imagePath = "/Assets/goalP.png";
            }

            return imagePath;
        }

        public bool IsThisAGCoalScoredPlay(Play play, bool includeOwnGoal, bool includeShootGoal)
        {
            bool returnValue = false;

            //if ((DAL.Instance().GetStatDescriptionNameById(play.StatDescriptionID) == "Goal") || (DAL.Instance().GetStatCategoryNameById(play.StatCategoryID) == "Own Goal") ||
            //((DAL.Instance().GetStatDescriptionNameById(play.StatDescriptionID) == "For Goal")) && (DAL.Instance().GetStatCategoryNameById(play.StatCategoryID) != "Shootout Kick"))

            if (play.StatDescriptionID == 17)   //17 is FOR GOAL
            {
                returnValue = true;
            }
            else if (play.StatCategoryID == 12) //12 is OWN GOAL
                if (includeOwnGoal == true)
                {
                    returnValue = true;
                }
                else
                {
                    returnValue = false;
                }
            else if (play.StatDescriptionID == 4)   //4 is GOAL
            {
                if ((play.StatCategoryID == 21) && (includeShootGoal == false)) //21 is SHOOTOUT KICK
                {
                    returnValue = false;
                }
                else
                {
                    returnValue = true;
                }
            }

            return returnValue;
        }

        //Any description of Goal or For Goal or a stat cat of Own Goal
        //public  bool IsThisAGCoalScoredPlayUseingDALCAL(Play play, bool includeOwnGoal, bool includeShootGoal)
        //{
        //    bool returnValue = false;
        //    string statCategoryName;
        //    string statCategoryDescription;

        //    statCategoryName = DAL.Instance().GetStatCategoryNameById(play.StatCategoryID);
        //    statCategoryDescription = DAL.Instance().GetStatDescriptionNameById(play.StatDescriptionID);

        //    //if ((DAL.Instance().GetStatDescriptionNameById(play.StatDescriptionID) == "Goal") || (DAL.Instance().GetStatCategoryNameById(play.StatCategoryID) == "Own Goal") ||
        //    //((DAL.Instance().GetStatDescriptionNameById(play.StatDescriptionID) == "For Goal")) && (DAL.Instance().GetStatCategoryNameById(play.StatCategoryID) != "Shootout Kick"))

        //    if (statCategoryDescription == "For Goal")
        //    {
        //        returnValue = true;
        //    }
        //    else if (statCategoryName == "Own Goal")
        //        if (includeOwnGoal == true)
        //        {
        //            returnValue = true;
        //        }
        //        else
        //        {
        //            returnValue = false;
        //        }
        //    else if (statCategoryDescription == "Goal")
        //    {
        //        if ((statCategoryName == "Shootout Kick") && (includeShootGoal == false))
        //        {
        //            returnValue = false;
        //        }
        //        else
        //        {
        //            returnValue = true;
        //        }
        //    }

        //    return returnValue;
        //}

        #endregion "Misc"

        #region "Upsert Play Extended Processing"

        //If this is an undoPlay, then we are backing out the substitution, otherwise it is simply a normal sub (i.e. Player1 IN, Player2 OUT)
        public void SubstitutionProcessing(int gameID, Play play, bool undoPlay)
        {
            Game game = new Game();

            try
            {
                game = BaseTableDataAccess.Instance().GetGameByGameID(gameID);

                if (undoPlay == false)
                {
                    //Update players on field status
                    BaseTableDataAccess.Instance().UpdatePlayersOnFieldStatus(play.GameID, play.TeamID, play.Player1ID, "Y");
                    BaseTableDataAccess.Instance().UpdatePlayersOnFieldStatus(play.GameID, play.TeamID, play.Player2ID, "N");

                    //Update players GM position
                    BaseTableDataAccess.Instance().UpdatePlayersGMPlayerPositionID(play.GameID, play.TeamID, play.Player1ID, play.GMPlayer2PositionID);
                    BaseTableDataAccess.Instance().UpdatePlayersGMPlayerPositionID(play.GameID, play.TeamID, play.Player2ID, play.GMPlayer1PositionID);

                    //If game has not started then this is a pre game change of starters
                    if (game.GameStatus == "NOT STARTED")
                    {
                        BaseTableDataAccess.Instance().UpdatePlayersStarterStatus(play.GameID, play.TeamID, play.Player1ID, "Y");
                        BaseTableDataAccess.Instance().UpdatePlayersStarterStatus(play.GameID, play.TeamID, play.Player2ID, "N");

                        //Now callling DAL.UpdateGameStartedStatForGame once the game starts so no need to do this here
                       // DAL.Instance().UpdatePlayersGameStartedStat(gameID, play.TeamID, play.Player1ID, true);
                       // DAL.Instance().UpdatePlayersGameStartedStat(gameID, play.TeamID, (int)play.Player2ID, false);
                    }
                }
                else   // We are undoing this substitution
                //Player1ID WAS the player who was coming IN, since we are undoing this play, we need to move them OUT now
                //Player2ID WAS the player who was coming OUT, since we are undoing this play, we need to move them IN now
                {
                    BaseTableDataAccess.Instance().UpdatePlayersOnFieldStatus(gameID, play.TeamID, play.Player1ID, "N");
                    BaseTableDataAccess.Instance().UpdatePlayersOnFieldStatus(gameID, play.TeamID, play.Player2ID, "Y");

                    BaseTableDataAccess.Instance().UpdatePlayersGMPlayerPositionID(play.GameID, play.TeamID, play.Player1ID, play.GMPlayer1PositionID);
                    BaseTableDataAccess.Instance().UpdatePlayersGMPlayerPositionID(play.GameID, play.TeamID, play.Player2ID, play.GMPlayer2PositionID);

                    if (game.GameStatus == "NOT STARTED")
                    {
                        BaseTableDataAccess.Instance().UpdatePlayersStarterStatus(play.GameID, play.TeamID, play.Player1ID, "N");
                        BaseTableDataAccess.Instance().UpdatePlayersStarterStatus(play.GameID, play.TeamID, play.Player2ID, "Y");

                        //Now callling DAL.UpdateGameStartedStatForGame once the game starts so no need to do this here                    
                        //DAL.Instance().UpdatePlayersGameStartedStat(gameID, play.TeamID, play.Player1ID, false);
                        //DAL.Instance().UpdatePlayersGameStartedStat(gameID, play.TeamID, (int) play.Player2ID, true);
                   }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        //This is where we will:
        //1) If goal is scored update the score of the game (i.e. add a goal to either hometeamscore or awayteamscore
        //2) If goal is scored update stat totals with a goal allowed for the goalie
        //3) If goal is scored update the plusminus for all players on the field
        //4) If NOT goal then if shot on goal update stat totals with a save for the goalie
        //5) If NOT goal then if not shot on goal update stat totals with a block for the blocker

        //No longer calculating plus minus as plays entered, instead doing it on demand
        public void ShotProcessing(GameModel game, Play play)
        {
            int otherTeamID;

            try
            {
                otherTeamID = Common.Instance().GetOtherTeamID(play.GameID, play.TeamID);

                //If this is a goal need to do goal processing (we do not include own goals or shootout goals)
                //if (((DAL.Instance().GetStatDescriptionNameById(play.StatDescriptionID) == "Goal") ||
                //    (DAL.Instance().GetStatDescriptionNameById(play.StatDescriptionID) == "For Goal")) && (DAL.Instance().GetStatCategoryNameById(play.StatCategoryID) != "Shootout Kick"))
                //if (((play.StatDescriptionID == 4) || (play.StatDescriptionID == 17)) && (play.StatCategoryID != 21))
                //is this a goal scored play (we do not include OWN GOALS or SHOOTOUT GOALS
                if (Common.Instance().IsThisAGCoalScoredPlay(play, false, false))
                {
                    //Charge goalie with a goal allowed 
                    if (play.OtherTeamGoalieID != null)
                    {
                        DAL.Instance().UpsertFlatTotalsStat(play.GameID, play.OtherTeamGoalieID, otherTeamID, "Goal Allowed", false);
                    }

                    //If play had an assist update players assist stat
                    if (play.AssistID != null)
                    {
                        DAL.Instance().UpsertFlatTotalsStat(play.GameID, play.AssistID, play.TeamID, "Assist", false);
                    }
                }
                else //this was a shot but not a goal scored, process goalie stats
                {
                    //If the shot was blocked it is either a save (if shot on goal) or a block (if shot not on goal)
                    if (play.ShotBlockedByID != null)
                    {
                        string statName;
                        if (play.ShotOnGoal == "Y") { statName = "Save"; } else { statName = "Blocked"; }

                        DAL.Instance().UpsertFlatTotalsStat(play.GameID, play.ShotBlockedByID, otherTeamID, statName, false);
                    }
                }

                //If shot on goal save stat
                if (play.ShotOnGoal == "Y")
                {
                    DAL.Instance().UpsertFlatTotalsStat(play.GameID, play.Player1ID, play.TeamID, "Shot On Goal", false);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        //TJY TO DO, need to update attemps and goals for player (I think, need to test first to confirm)
        public void ShootoutProcessing(GameModel game, Play play)
        {
            int opps;
            int goals;
            try
            {
                //save the opp
                if (game.Game.HomeTeamID == play.TeamID)
                {
                    opps = game.Game.HomeTeamShootOutGoalOpp + 1;
                    BaseTableDataAccess.Instance().UpdateGameHomeTeamShootoutOpp(play.GameID, opps);
                }
                else
                {
                    opps = game.Game.AwayTeamShootOutGoalOpp + 1;
                    BaseTableDataAccess.Instance().UpdateGameAwayTeamShootoutOpp(play.GameID, opps);
                }

                //If this is a goal save team shootout goal and opp
                if (play.StatDescriptionID == 4)    //Goal (4)
                {
                    if (game.Game.HomeTeamID == play.TeamID)
                    {
                        goals = game.Game.HomeTeamShootOutGoals + 1;
                        BaseTableDataAccess.Instance().UpdateGameHomeTeamShootoutScore(play.GameID, goals);
                    }
                    else
                    {
                        goals = game.Game.AwayTeamShootOutGoals + 1;
                        BaseTableDataAccess.Instance().UpdateGameAwayTeamShootoutScore(play.GameID, goals);
                    }
                }

                if (play.ShotOnGoal == "Y")
                {
                    DAL.Instance().UpsertFlatTotalsStat(play.GameID, play.Player1ID, play.TeamID, "Shot On Goal", false);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion "Upsert Play Extended Processing"

        #region "Globalization"

        public void GlobalizeStatCatAndDescription(StatCategoryModel statCatModel)
        {
            switch (statCatModel.StatCategory.StatCategoryName)
            {
                case "Shot":
                    statCatModel.StatCategory.StatCategoryName = AppResources.Shot;
                    break;
                case "Pass":
                    statCatModel.StatCategory.StatCategoryName = AppResources.Pass;
                    break;
                case "Turnover":
                    statCatModel.StatCategory.StatCategoryName = AppResources.Turnover;
                    break;
                case "Offsides":
                    statCatModel.StatCategory.StatCategoryName = AppResources.Offsides;
                    break;
                case "Foul Committed":
                    statCatModel.StatCategory.StatCategoryName = AppResources.FoulCommitted;
                    break;
                case "Out Of Bounds":
                    statCatModel.StatCategory.StatCategoryName = AppResources.OutOfBounds;
                    break;
                case "Cross":
                    statCatModel.StatCategory.StatCategoryName = AppResources.Cross;
                    break;
                case "Throw In":
                    statCatModel.StatCategory.StatCategoryName = AppResources.ThrowIn;
                    break;
                case "Corner Kick":
                    statCatModel.StatCategory.StatCategoryName = AppResources.CornerKick;
                    break;
                case "Tackle":
                    statCatModel.StatCategory.StatCategoryName = AppResources.Tackle;
                    break;
                case "Goalie Kick":
                    statCatModel.StatCategory.StatCategoryName = AppResources.GoalieKick;
                    break;
                case "Own Goal":
                    statCatModel.StatCategory.StatCategoryName = AppResources.OwnGoal;
                    break;
                case "Foul Drawn":
                    statCatModel.StatCategory.StatCategoryName = AppResources.FoulDrawn;
                    break;
                case "Direct Free Kick":
                    statCatModel.StatCategory.StatCategoryName = AppResources.DirectFreeKick;
                    break;
                case "Indirect Free Kick":
                    statCatModel.StatCategory.StatCategoryName = AppResources.IndirectFreeKick;
                    break;
                case "Penalty Kick":
                    statCatModel.StatCategory.StatCategoryName = AppResources.PenaltyKick;
                    break;
                case "Yellow Card":
                    statCatModel.StatCategory.StatCategoryName = AppResources.YellowCard;
                    break;
                case "Red Card":
                    statCatModel.StatCategory.StatCategoryName = AppResources.RedCard;
                    break;
                case "Drop Kick":
                    statCatModel.StatCategory.StatCategoryName = AppResources.DropKick;
                    break;
                case "Dribble":
                    statCatModel.StatCategory.StatCategoryName = AppResources.Dribble;
                    break;
                case "Shootout Kick":
                    statCatModel.StatCategory.StatCategoryName = AppResources.ShootoutKick;
                    break;
                default:
                    break;
            }

            if (statCatModel.Descriptions != null)
            {
                foreach (var descriptions in statCatModel.Descriptions)
                {
                    switch (descriptions.StatDescription.StatDescriptionName)
                    {
                        case "Miss":
                            descriptions.StatDescription.StatDescriptionName = AppResources.Miss;
                            break;
                        case "Hit Post":
                            descriptions.StatDescription.StatDescriptionName = AppResources.HitPost;
                            break;
                        case "Blocked":
                            descriptions.StatDescription.StatDescriptionName = AppResources.Blocked;
                            break;
                        case "Goal":
                            descriptions.StatDescription.StatDescriptionName = AppResources.Goal;
                            break;
                        case "Excellent":
                            descriptions.StatDescription.StatDescriptionName = AppResources.Excellent;
                            break;
                        case "Poor":
                            descriptions.StatDescription.StatDescriptionName = AppResources.Poor;
                            break;
                        case "Illegal Throw In":
                            descriptions.StatDescription.StatDescriptionName = AppResources.IllegalThrowIn;
                            break;
                        case "Bad Pass":
                            descriptions.StatDescription.StatDescriptionName = AppResources.BadPass;
                            break;
                        case "Lost Dribble":
                            descriptions.StatDescription.StatDescriptionName = AppResources.LostDribble;
                            break;
                        case "Kicking":
                            descriptions.StatDescription.StatDescriptionName = AppResources.Kicking;
                            break;
                        case "Tripping":
                            descriptions.StatDescription.StatDescriptionName = AppResources.Tripping;
                            break;
                        case "Charging":
                            descriptions.StatDescription.StatDescriptionName = AppResources.Charging;
                            break;
                        case "Pushing":
                            descriptions.StatDescription.StatDescriptionName = AppResources.Pushing;
                            break;
                        case "Holding":
                            descriptions.StatDescription.StatDescriptionName = AppResources.Holding;
                            break;
                        case "Illegal Tackle":
                            descriptions.StatDescription.StatDescriptionName = AppResources.IllegalTackle;
                            break;
                        case "For Goal":
                            descriptions.StatDescription.StatDescriptionName = AppResources.ForGoal;
                            break;
                        case "Not For Goal":
                            descriptions.StatDescription.StatDescriptionName = AppResources.NotForGoal;
                            break;
                        case "Unsportsmanlike Conduct":
                            descriptions.StatDescription.StatDescriptionName = AppResources.UnsportsmanlikeConduct;
                            break;
                        case "Delaying Restart Of Play":
                            descriptions.StatDescription.StatDescriptionName = AppResources.DelayingRestartOfPlay;
                            break;
                        case "Foul Play":
                            descriptions.StatDescription.StatDescriptionName = AppResources.FoulPlay;
                            break;
                        case "Violent Conduct":
                            descriptions.StatDescription.StatDescriptionName = AppResources.ViolentConduct;
                            break;
                        case "Second Yellow Card":
                            descriptions.StatDescription.StatDescriptionName = AppResources.SecondYellowCard;
                            break;
                        case "In":
                            descriptions.StatDescription.StatDescriptionName = AppResources.In;
                            break;
                        case "Out":
                            descriptions.StatDescription.StatDescriptionName = AppResources.Out;
                            break;
                        case "Left":
                            descriptions.StatDescription.StatDescriptionName = AppResources.Left;
                            break;
                        case "Right":
                            descriptions.StatDescription.StatDescriptionName = AppResources.Right;
                            break;
                        case "Headed":
                            descriptions.StatDescription.StatDescriptionName = AppResources.Headed;
                            break;
                        case "Shot Details":
                            descriptions.StatDescription.StatDescriptionName = AppResources.ShotDetails;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        #endregion "Globalization"

        #region "FlatTotals"


        public int GetFlatTotalsColumnValue(FlatTotals flattotals, string flatTotalsColumn)
        {
            int returnValue = 0;

            try
            {
                if (flattotals == null)
                {
                    return 0;
                }

                switch (flatTotalsColumn)
                {
                    case "ShotMiss":
                        returnValue = flattotals.ShotMiss;
                        break;
                    case "ShotHitPost":
                        returnValue = flattotals.ShotHitPost;
                        break;
                    case "ShotBlocked":
                        returnValue = flattotals.ShotBlocked;
                        break;
                    case "ShotGoal":
                        returnValue = flattotals.ShotGoal;
                        break;
                    case "PassExcellent":
                        returnValue = flattotals.PassExcellent;
                        break;
                    case "PassGood":
                        returnValue = flattotals.PassGood;
                        break;
                    case "PassPoor":
                        returnValue = flattotals.PassPoor;
                        break;
                    case "TurnoverIllegalThrowIn":
                        returnValue = flattotals.TurnoverIllegalThrowIn;
                        break;
                    case "TurnoverLostDribble":
                        returnValue = flattotals.TurnoverLostDribble;
                        break;
                    case "OffsidesTotal":
                        returnValue = flattotals.OffsidesTotal;
                        break;
                    case "FoulCommittedTripping":
                        returnValue = flattotals.FoulCommittedTripping;
                        break;
                    case "FoulCommittedCharging":
                        returnValue = flattotals.FoulCommittedCharging;
                        break;
                    case "FoulCommittedHolding":
                        returnValue = flattotals.FoulCommittedHolding;
                        break;
                    case "FoulCommittedIllegalTackle":
                        returnValue = flattotals.FoulCommittedIllegalTackle;
                        break;
                    case "FoulCommittedKicking":
                        returnValue = flattotals.FoulCommittedKicking;
                        break;
                    case "FoulCommittedPushing":
                        returnValue = flattotals.FoulCommittedPushing;
                        break;
                    case "OutOfBoundsTotal":
                        returnValue = flattotals.OutOfBoundsTotal;
                        break;
                    case "CrossExcellent":
                        returnValue = flattotals.CrossExcellent;
                        break;
                    case "CrossGood":
                        returnValue = flattotals.CrossGood;
                        break;
                    case "CrossPoor":
                        returnValue = flattotals.CrossPoor;
                        break;
                    case "ThrowInTotal":
                        returnValue = flattotals.ThrowInTotal;
                        break;
                    case "CornerKickExcellent":
                        returnValue = flattotals.CornerKickExcellent;
                        break;
                    case "CornerKickGood":
                        returnValue = flattotals.CornerKickGood;
                        break;
                    case "CornerKickPoor":
                        returnValue = flattotals.CornerKickPoor;
                        break;
                    case "CornerKickForGoal":
                        returnValue = flattotals.CornerKickForGoal;
                        break;
                    case "TackleTotal":
                        returnValue = flattotals.TackleTotal;
                        break;
                    case "GoalieKickTotal":
                        returnValue = flattotals.GoalieKickTotal;
                        break;
                    case "OwnGoalTotal":
                        returnValue = flattotals.OwnGoalTotal;
                        break;
                    case "FoulDrawnTotal":
                        returnValue = flattotals.FoulDrawnTotal;
                        break;
                    case "DirectFreeKickNotForGoal":
                        returnValue = flattotals.DirectFreeKickNotForGoal;
                        break;
                    case "DirectFreeKickForGoal":
                        returnValue = flattotals.DirectFreeKickForGoal;
                        break;
                    case "IndirectFreeKickTotal":
                        returnValue = flattotals.IndirectFreeKickTotal;
                        break;
                    case "PenaltyKickMiss":
                        returnValue = flattotals.PenaltyKickMiss;
                        break;
                    case "PenaltyKickHitPost":
                        returnValue = flattotals.PenaltyKickHitPost;
                        break;
                    case "PenaltyKickBlocked":
                        returnValue = flattotals.PenaltyKickBlocked;
                        break;
                    case "PenaltyKickGoal":
                        returnValue = flattotals.PenaltyKickGoal;
                        break;
                    case "YellowCardUnsportsmanLikeConduct":
                        returnValue = flattotals.YellowCardUnsportsmanLikeConduct;
                        break;
                    case "YellowCardDelayingRestartOfPlay":
                        returnValue = flattotals.YellowCardDelayingRestartOfPlay;
                        break;
                    case "RedCardFoulPlay":
                        returnValue = flattotals.RedCardFoulPlay;
                        break;
                    case "RedCardIllegalHands":
                        returnValue = flattotals.RedCardIllegalHands;
                        break;
                    case "RedCardSecondYellowCard":
                        returnValue = flattotals.RedCardSecondYellowCard;
                        break;
                    case "RedCardViolentConduct":
                        returnValue = flattotals.RedCardViolentConduct;
                        break;
                    case "DropKickExcellent":
                        returnValue = flattotals.DropKickExcellent;
                        break;
                    case "DropKickGood":
                        returnValue = flattotals.DropKickGood;
                        break;
                    case "DropKickPoor":
                        returnValue = flattotals.DropKickPoor;
                        break;
                    case "DribbleTotal":
                        returnValue = flattotals.DribbleTotal;
                        break;
                    case "ShootoutKickMiss":
                        returnValue = flattotals.ShootoutKickMiss;
                        break;
                    case "ShootoutKickHitPost":
                        returnValue = flattotals.ShootoutKickHitPost;
                        break;
                    case "ShootoutKickBlocked":
                        returnValue = flattotals.ShootoutKickBlocked;
                        break;
                    case "ShootoutKickGoal":
                        returnValue = flattotals.ShootoutKickGoal;
                        break;
                    case "SecondsPlayedTotal":
                        returnValue = flattotals.SecondsPlayedTotal;
                        break;
                    case "MinutesPlayedTotal":
                        returnValue = flattotals.MinutesPlayedTotal;
                        break;
                    case "PlusMinusTotal":
                        returnValue = flattotals.PlusMinusTotal;
                        break;
                    case "BlockedTotal":
                        returnValue = flattotals.BlockedTotal;
                        break;
                    case "GoalAllowedTotal":
                        returnValue = flattotals.GoalAllowedTotal;
                        break;
                    case "SaveTotal":
                        returnValue = flattotals.SaveTotal;
                        break;
                    case "ShotOnGoalTotal":
                        returnValue = flattotals.ShotOnGoalTotal;
                        break;
                    case "AssistTotal":
                        returnValue = flattotals.AssistTotal;
                        break;
                    default:
                        returnValue = 0;
                        break;
                }

                return returnValue;
            }

            catch (Exception ex)
            {
                return 0;
            }
        }

        public string GetFlatTotalsColumnName(int statCategoryID, int? statCategoryDescriptionID)
        {
            string categoryName;
            string categoryDescription;
            string flatTotalsColumn = string.Empty;

            try
            {
                categoryName = DAL.Instance().GetStatCategoryNameById(statCategoryID);
                categoryDescription = DAL.Instance().GetStatDescriptionNameById(statCategoryDescriptionID);

                switch (categoryName)
                {
                    case "Shot":
                        if (categoryDescription == "Miss") { flatTotalsColumn = "ShotMiss"; }
                        else if (categoryDescription == "Hit Post") { flatTotalsColumn = "ShotHitPost"; }
                        else if (categoryDescription == "Blocked") { flatTotalsColumn = "ShotBlocked"; }
                        else if (categoryDescription == "Goal") { flatTotalsColumn = "ShotGoal"; }
                        break;
                    case "Pass":
                        if (categoryDescription == "Excellent") { flatTotalsColumn = "PassExcellent"; }
                        else if (categoryDescription == "Good") { flatTotalsColumn = "PassGood"; }
                        else if (categoryDescription == "Poor") { flatTotalsColumn = "PassPoor"; }
                        break;
                    case "Turnover":
                        if (categoryDescription == "Illegal Throw In") { flatTotalsColumn = "TurnoverIllegalThrowIn"; }
                        else if (categoryDescription == "Lost Dribble") { flatTotalsColumn = "TurnoverLostDribble"; }
                        break;
                    case "Offsides":
                        flatTotalsColumn = "OffsidesTotal";
                        break;
                    case "Foul Committed":
                        if (categoryDescription == "Charging") { flatTotalsColumn = "FoulCommittedCharging"; }
                        else if (categoryDescription == "Holding") { flatTotalsColumn = "FoulCommittedHolding"; }
                        else if (categoryDescription == "Illegal Tackle") { flatTotalsColumn = "FoulCommittedIllegalTackle"; }
                        else if (categoryDescription == "Kicking") { flatTotalsColumn = "FoulCommittedKicking"; }
                        else if (categoryDescription == "Pushing") { flatTotalsColumn = "FoulCommittedPushing"; }
                        else if (categoryDescription == "Tripping") { flatTotalsColumn = "FoulCommittedTripping"; }
                        break;
                    case "Out Of Bounds":
                        flatTotalsColumn = "OutOfBoundsTotal";
                        break;
                    case "Cross":
                        if (categoryDescription == "Excellent") { flatTotalsColumn = "CrossExcellent"; }
                        else if (categoryDescription == "Good") { flatTotalsColumn = "CrossGood"; }
                        else if (categoryDescription == "Poor") { flatTotalsColumn = "CrossPoor"; }
                        break;
                    case "Throw In":
                        flatTotalsColumn = "ThrowInTotal";
                        break;
                    case "Corner Kick":
                        if (categoryDescription == "Excellent") { flatTotalsColumn = "CornerKickExcellent"; }
                        else if (categoryDescription == "Good") { flatTotalsColumn = "CornerKickGood"; }
                        else if (categoryDescription == "Poor") { flatTotalsColumn = "CornerKickPoor"; }
                        else if (categoryDescription == "For Goal") { flatTotalsColumn = "CornerKickForGoal"; }
                        break;
                    case "Tackle":
                        flatTotalsColumn = "TackleTotal";
                        break;
                    case "Goalie Kick":
                        flatTotalsColumn = "GoalieKickTotal";
                        break;
                    case "Own Goal":
                        flatTotalsColumn = "OwnGoalTotal";
                        break;
                    case "Foul Drawn":
                        flatTotalsColumn = "FoulDrawnTotal";
                        break;
                    case "Direct Free Kick":
                        if (categoryDescription == "Not For Goal") { flatTotalsColumn = "DirectFreeKickNotForGoal"; }
                        else if (categoryDescription == "For Goal") { flatTotalsColumn = "DirectFreeKickForGoal"; }
                        break;
                    case "Indirect Free Kick":
                        flatTotalsColumn = "IndirectFreeKickTotal";
                        break;
                    case "Penalty Kick":
                        if (categoryDescription == "Miss") { flatTotalsColumn = "PenaltyKickMiss"; }
                        else if (categoryDescription == "Hit Post") { flatTotalsColumn = "PenaltyKickHitPost"; }
                        else if (categoryDescription == "Blocked") { flatTotalsColumn = "PenaltyKickBlocked"; }
                        else if (categoryDescription == "Goal") { flatTotalsColumn = "PenaltyKickGoal"; }
                        break;
                    case "Yellow Card":
                        if (categoryDescription == "Unsportsmanlike Conduct") { flatTotalsColumn = "YellowCardUnsportsmanLikeConduct"; }
                        else if (categoryDescription == "Delaying Restart Of Play") { flatTotalsColumn = "YellowCardDelayingRestartOfPlay"; }
                        break;
                    case "Red Card":
                        if (categoryDescription == "Foul Play") { flatTotalsColumn = "RedCardFoulPlay"; }
                        else if (categoryDescription == "Violent Conduct") { flatTotalsColumn = "RedCardIllegalHands"; }
                        else if (categoryDescription == "Illegal Hands") { flatTotalsColumn = "RedCardSecondYellowCard"; }
                        else if (categoryDescription == "Second Yellow Card") { flatTotalsColumn = "RedCardViolentConduct"; }
                        break;
                    case "Drop Kick":
                        if (categoryDescription == "Excellent") { flatTotalsColumn = "DropKickExcellent"; }
                        else if (categoryDescription == "Good") { flatTotalsColumn = "DropKickGood"; }
                        else if (categoryDescription == "Poor") { flatTotalsColumn = "DropKickPoor"; }
                        break;
                    case "Dribble":
                        flatTotalsColumn = "DribbleTotal";
                        break;
                    case "Shootout Kick":
                        if (categoryDescription == "Miss") { flatTotalsColumn = "ShootoutKickMiss"; }
                        else if (categoryDescription == "Hit Post") { flatTotalsColumn = "ShootoutKickHitPost"; }
                        else if (categoryDescription == "Blocked") { flatTotalsColumn = "ShootoutKickBlocked"; }
                        else if (categoryDescription == "Goal") { flatTotalsColumn = "ShootoutKickGoal"; }
                        break;
                    case "Substitution":
                        break;
                    case "Move":
                        break;
                    case "Clock":
                        break;
                    case "SecondsPlayed":
                        flatTotalsColumn = "SecondsPlayedTotal";
                        break;
                    case "MinutesPlayed":
                        flatTotalsColumn = "MinutesPlayedTotal";
                        break;
                    case "PlusMinus":
                        flatTotalsColumn = "PlusMinusTotal";
                        break;
                    case "Blocked":
                        flatTotalsColumn = "BlockedTotal";
                        break;
                    case "Goal Allowed":
                        flatTotalsColumn = "GoalAllowedTotal";
                        break;
                    case "Save":
                        flatTotalsColumn = "SaveTotal";
                        break;
                    case "Shot On Goal":
                        flatTotalsColumn = "ShotOnGoalTotal";
                        break;
                    case "Assist":
                        flatTotalsColumn = "AssistTotal";
                        break;
                }

                return flatTotalsColumn;
            }
            catch (Exception ex)
            {
                return flatTotalsColumn;
            }
        }

        #endregion"

    }
}


