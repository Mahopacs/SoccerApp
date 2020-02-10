using ScoreSoccer8.Cloud;
using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.DataObjects.UiClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.Utilities
{
    public static class StatCalculationsModule
    {
        #region "Routines to calculate player minutes/seconds played"

        //With this approach we are not using clock tick events.
        //As a result our database does not reflect an accurate depiction of minutes/seconds played at any given time
        //Which is fine since we will be doing our recalculation based on when the user clicks on a stat report generation
        //(OR we could decide to run this behind the scenes every X seconds)

        //For EVERY player in event roster for BOTH teams, we need to determine each players minutes/seconds played

        //public static void CalculateALLPlayerMinutesOLD(int gameID)
        //{
        //    int inPeriod;
        //    string inClock;
        //    int outPeriod;
        //    string outClock;
        //    int playerSecondsPlayed;
        //    string playersMostRecentTrans = string.Empty;
        //    Game game = new Game();
        //    List<EventRoster> eventRosterList = new List<EventRoster>();

        //    try
        //    {
        //        Debug.WriteLine(DateTime.Now + " - Start - CalculatePlayerMinutes");
        //        //Get game info -> periods, period length, OT, OT length, current period, current clock are all needed
        //        game = BaseTableDataAccess.Instance().GetGameByGameID(gameID);

        //        //Get list of all players for both teams who were on roster for this game
        //        eventRosterList = BaseTableDataAccess.Instance().GetEventRosterByGameID(gameID);

        //        //Go through all players and determine their minutes/seconds played
        //        foreach (var eventRosterEntry in eventRosterList)
        //        {
        //            playerSecondsPlayed = 0;

        //            //get this players sub transactions/plays
        //            List<Play> playersSubTransactionsList = new List<Play>();
        //            playersSubTransactionsList = BaseTableDataAccess.Instance().GetAllSubPlaysForGameByPlayerID(gameID, eventRosterEntry.PlayerID, eventRosterEntry.TeamID);

        //            //If this player had no sub transations then they never left the game
        //            //If they started we need to determine their minutes played (which is simply time elapsed in game)  
        //            //If they did not start then they never got in game and minutes played = 0
        //            if (playersSubTransactionsList.Count == 0)
        //            {
        //                if (eventRosterEntry.Starter == "Y")
        //                {
        //                    playerSecondsPlayed = CalculateTimeElapsedInGameInSeconds(game, game.CurrentPeriod, game.CurrentClock);
        //                }
        //                else //player did not start and has no sub transactions so this player has never appeared in the game
        //                {
        //                    playerSecondsPlayed = 0;
        //                }
        //            }
        //            else //this player has sub transactions so need to go through them to determine players minutes played
        //            {
        //                //If the player was a starter then their first sub will be an OUT, so we can initiailize their IN varialbles to the 1st period and period length (i.e. time period started)
        //                if (eventRosterEntry.Starter == "Y")
        //                {
        //                    playersMostRecentTrans = "IN";
        //                    if (game.ClockUpOrDown.ToUpper() == "UP")
        //                    {
        //                        inPeriod = 1; inClock = "00:00"; outPeriod = -99; outClock = "-99";
        //                    }
        //                    else
        //                    {
        //                        inPeriod = 1; inClock = game.PeriodLength.ToString() + ":00"; outPeriod = -99; outClock = "-99";
        //                    }
        //                }
        //                else //We do not need to initialize the IN varaibles if this player did not start, as their first sub will be an IN, BUT .NET requires variables to be assigned if their first use is in if statment logic
        //                {
        //                    playersMostRecentTrans = "OUT";
        //                    inPeriod = -99; inClock = "-99"; outPeriod = -99; outClock = "-99";
        //                }

        //                foreach (var playerSubTrans in playersSubTransactionsList)
        //                {
        //                    //Check if this is an IN (their ID is in player1ID) or OUT (their ID is in player2ID) sub transaction for the player
        //                    if (eventRosterEntry.PlayerID == playerSubTrans.Player1ID)
        //                    {
        //                        //this is an IN sub for player
        //                        if (playersMostRecentTrans != "IN")
        //                        {
        //                            inPeriod = playerSubTrans.Period; inClock = playerSubTrans.GameTime;
        //                            outPeriod = 0; outClock = "0";
        //                            playersMostRecentTrans = "IN";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //this is an OUT sub for player
        //                        if (playersMostRecentTrans != "OUT")
        //                        {
        //                            outPeriod = playerSubTrans.Period; outClock = playerSubTrans.GameTime;
        //                            playersMostRecentTrans = "OUT";
        //                            playerSecondsPlayed = playerSecondsPlayed + HowLongHasPlayerBeenInGame(game, inPeriod, outPeriod, inClock, outClock);
        //                        }
        //                    }
        //                } // next trans for player

        //                //If outPeriod = 0, then player is still on field (as we have not received an OUT sub for them), 
        //                //so need to add time since they came in to their total time (i.e. inClock - currentClock)
        //                if (outPeriod == 0)
        //                {
        //                    playerSecondsPlayed = playerSecondsPlayed + HowLongHasPlayerBeenInGame(game, inPeriod, game.CurrentPeriod, inClock, game.CurrentClock);
        //                }
        //            }
        //            //for this player write their seconds and minutes played to Stat totals, then we can move on to next player
        //            SavePlayersTimePlayedToStatTotals(gameID, eventRosterEntry.TeamID, eventRosterEntry.PlayerID, playerSecondsPlayed);
        //        } // next player
        //        Debug.WriteLine(DateTime.Now + " - End - CalculatePlayerMinutes");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }

        //}

        public static void CalculateALLPlayerMinutes(int gameID)
        {
            int inPeriod;
            string inClock;
            int outPeriod;
            string outClock;
            int playerSecondsPlayed;
            int gamesElapsedTimeInSeconds;
            string playersMostRecentTrans = string.Empty;
            Game game = new Game();
            List<EventRoster> eventRosterList = new List<EventRoster>();
            List<Play> gamesSubTransactionsList = new List<Play>(); ;

            try
            {
                if (App.DoesUserHaveAbilityToTrackAllStats() == false)
                {
                    return;
                }

                // Debug.WriteLine(DateTime.Now.ToLongTimeString() + " - Start - CalculatePlayerMinutes");

                //Get game info -> periods, period length, OT, OT length, current period, current clock are all needed
                game = BaseTableDataAccess.Instance().GetGameByGameID(gameID);

                gamesElapsedTimeInSeconds = CalculateTimeElapsedInGameInSeconds(game, game.CurrentPeriod, game.CurrentClock);

                //Get list of all players for both teams who were on roster for this game
                eventRosterList = BaseTableDataAccess.Instance().GetEventRosterByGameID(gameID);

                gamesSubTransactionsList = BaseTableDataAccess.Instance().GetAllSubPlaysForGame(gameID);

                //Go through all players and determine their minutes/seconds played
                foreach (var eventRosterEntry in eventRosterList)
                {
                    playerSecondsPlayed = 0;

                    //get this players sub transactions/plays
                    var playersSubTransactionsList = gamesSubTransactionsList.Where(x => (x.Player1ID == eventRosterEntry.PlayerID || x.Player2ID == eventRosterEntry.PlayerID)
                                                 && x.TeamID == eventRosterEntry.TeamID).ToList();

                    //If this player had no sub transations then they never left the game
                    //If they started we need to determine their minutes played (which is simply time elapsed in game)  
                    //If they did not start then they never got in game and minutes played = 0
                    if (playersSubTransactionsList.Count == 0)
                    {
                        if (eventRosterEntry.Starter == "Y")
                        {
                            playerSecondsPlayed = gamesElapsedTimeInSeconds;
                        }
                        else //player did not start and has no sub transactions so this player has never appeared in the game
                        {
                            playerSecondsPlayed = 0;
                        }
                    }
                    else //this player has sub transactions so need to go through them to determine players minutes played
                    {
                        //If the player was a starter then their first sub will be an OUT, so we can initiailize their IN varialbles to the 1st period and period length (i.e. time period started)
                        if (eventRosterEntry.Starter == "Y")
                        {
                            playersMostRecentTrans = "IN";
                            if (game.ClockUpOrDown.ToUpper() == "UP")
                            {
                                inPeriod = 1; inClock = "00:00"; outPeriod = -99; outClock = "-99";
                            }
                            else
                            {
                                inPeriod = 1; inClock = game.PeriodLength.ToString() + ":00"; outPeriod = -99; outClock = "-99";
                            }
                        }
                        else //We do not need to initialize the IN varaibles if this player did not start, as their first sub will be an IN, BUT .NET requires variables to be assigned if their first use is in if statment logic
                        {
                            playersMostRecentTrans = "OUT";
                            inPeriod = -99; inClock = "-99"; outPeriod = -99; outClock = "-99";
                        }

                        foreach (var playerSubTrans in playersSubTransactionsList)
                        {
                            //Check if this is an IN (their ID is in player1ID) or OUT (their ID is in player2ID) sub transaction for the player
                            if (eventRosterEntry.PlayerID == playerSubTrans.Player1ID)
                            {
                                //this is an IN sub for player
                                if (playersMostRecentTrans != "IN")
                                {
                                    inPeriod = playerSubTrans.Period; inClock = playerSubTrans.GameTime;
                                    outPeriod = 0; outClock = "0";
                                    playersMostRecentTrans = "IN";
                                }
                            }
                            else
                            {
                                //this is an OUT sub for player
                                if (playersMostRecentTrans != "OUT")
                                {
                                    outPeriod = playerSubTrans.Period; outClock = playerSubTrans.GameTime;
                                    playersMostRecentTrans = "OUT";
                                    playerSecondsPlayed = playerSecondsPlayed + HowLongHasPlayerBeenInGame(game, inPeriod, outPeriod, inClock, outClock);
                                }
                            }
                        } // next trans for player

                        //If outPeriod = 0, then player is still on field (as we have not received an OUT sub for them), 
                        //so need to add time since they came in to their total time (i.e. inClock - currentClock)
                        if (outPeriod == 0)
                        {
                            playerSecondsPlayed = playerSecondsPlayed + HowLongHasPlayerBeenInGame(game, inPeriod, game.CurrentPeriod, inClock, game.CurrentClock);
                        }
                    }
                    //for this player write their seconds and minutes played to Stat totals, then we can move on to next player
                    SavePlayersTimePlayedToStatTotals(gameID, eventRosterEntry.TeamID, eventRosterEntry.PlayerID, playerSecondsPlayed);
                } // next player
                // Debug.WriteLine(DateTime.Now.ToLongTimeString() + " - End - CalculatePlayerMinutes");
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static int CalculateTimeElapsedInGameInSeconds(Game game, int currentPeriod, string currentClock)
        {
            int elapsedTimeInSeconds;

            try
            {
                if (game.ClockUpOrDown.ToUpper() == "DOWN")
                {
                    elapsedTimeInSeconds = CalculateTimeElapsedInGameInSecondsForCLOCKDOWN(game, currentPeriod, currentClock);
                }
                else
                {
                    elapsedTimeInSeconds = CalculateTimeElapsedInGameInSecondsForCLOCKUP(game, currentPeriod, currentClock);
                }

                return elapsedTimeInSeconds;
            }
            catch (Exception ex)
            {
                ErrorLogConnection cloud = new ErrorLogConnection();
                cloud.UpdateErrorLog("StatCalculationModule.CalculateTimeElapsedInGameInSeconds", ex.Message.ToString());
                return 0;
            }
        }

        public static int CalculateTimeElapsedInGameInSecondsForCLOCKDOWN(Game game, int currentPeriod, string currentClock)
        {
            int periodLength;
            int secondsElapsed = 0;

            try
            {
                if (currentClock == null)
                {
                    return 0;
                }

                for (int i = 1; i <= currentPeriod; i++)
                {
                    if (i <= game.Periods)
                    {
                        periodLength = game.PeriodLength;
                    }
                    else
                    {
                        periodLength = game.OverTimeLength;
                    }

                    int periodLengthInSeconds = periodLength * 60;

                    if (i == currentPeriod)
                    {
                        int currentClockInSeconds = ConvertClockStringValueToSeconds(currentClock);

                        //clock goes down so if period length was 10:00, and we are at 7:00, we are 10-7, or 3 minutes into period                     
                        secondsElapsed = secondsElapsed + (periodLengthInSeconds - currentClockInSeconds);
                    }
                    else
                    {
                        secondsElapsed = secondsElapsed + periodLengthInSeconds;
                    }
                }

                return secondsElapsed;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static int CalculateTimeElapsedInGameInSecondsForCLOCKUP(Game game, int currentPeriod, string currentClock)
        {
            int secondsElapsed = 0;
            int periodLengthInSeconds;

            try
            {
                if (currentClock == null)
                {
                    return 0;
                }

                for (int i = 1; i <= currentPeriod; i++)
                {
                    //Since clock up can have extra time, we can not user period length or OT length, we have to use the Period1ActualLength...Period6ActualLength
                    switch (i)
                    {
                        case 1:
                            if (game.Period1ActualLength == null)
                            {
                                periodLengthInSeconds = ConvertClockStringValueToSeconds(game.PeriodLength + ":00");
                            }
                            else
                            {
                                periodLengthInSeconds = ConvertClockStringValueToSeconds(game.Period1ActualLength);
                            }
                            break;
                        case 2:
                            if (game.Period2ActualLength == null)
                            {
                                periodLengthInSeconds = ConvertClockStringValueToSeconds(game.PeriodLength + ":00");
                            }
                            else
                            {
                                periodLengthInSeconds = ConvertClockStringValueToSeconds(game.Period2ActualLength);
                            }
                            break;
                        case 3:
                            if (game.Period3ActualLength == null)
                            {
                                if (currentPeriod > game.Periods)
                                {
                                    periodLengthInSeconds = ConvertClockStringValueToSeconds(game.OverTimeLength + ":00");
                                }
                                else
                                {
                                    periodLengthInSeconds = ConvertClockStringValueToSeconds(game.PeriodLength + ":00");
                                }
                            }
                            else
                            {
                                periodLengthInSeconds = ConvertClockStringValueToSeconds(game.Period3ActualLength);
                            }
                            break;
                        case 4:
                            if (game.Period4ActualLength == null)
                            {
                                if (currentPeriod > game.Periods)
                                {
                                    periodLengthInSeconds = ConvertClockStringValueToSeconds(game.OverTimeLength + ":00");
                                }
                                else
                                {
                                    periodLengthInSeconds = ConvertClockStringValueToSeconds(game.PeriodLength + ":00");
                                }
                            }
                            else
                            {
                                periodLengthInSeconds = ConvertClockStringValueToSeconds(game.Period4ActualLength);
                            }
                            break;
                        case 5:
                            if (game.Period5ActualLength == null)
                            {
                                periodLengthInSeconds = ConvertClockStringValueToSeconds(game.OverTimeLength + ":00");
                            }
                            else
                            {
                                periodLengthInSeconds = ConvertClockStringValueToSeconds(game.Period5ActualLength);
                            }
                            break;
                        case 6:
                            if (game.Period6ActualLength == null)
                            {
                                periodLengthInSeconds = ConvertClockStringValueToSeconds(game.OverTimeLength + ":00");
                            }
                            else
                            {
                                periodLengthInSeconds = ConvertClockStringValueToSeconds(game.Period6ActualLength);
                            }
                            break;
                        default:
                            periodLengthInSeconds = game.OverTimeLength * 60;
                            break;
                    }


                    if (i == currentPeriod)
                    {
                        //Since this is the current period we need to take the current clock value and subtract from it when the period started.
                        //i.e. 90:00 - 45:00)
                        int periodStartTimeInSeconds = DAL.Instance().GetPeriodStartTimeInSeconds(game, i);
                        int currentClockInSeconds = ConvertClockStringValueToSeconds(currentClock);

                        secondsElapsed = secondsElapsed + (currentClockInSeconds - periodStartTimeInSeconds);
                    }
                    else
                    {
                        secondsElapsed = secondsElapsed + periodLengthInSeconds;
                    }
                }

                return secondsElapsed;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //This will convert 90:16 to 45:16 for a 45 minute period for 2nd period
        public static int CalculateTimeElapsedInPeriodInSecondsForCLOCKUP(Game game, int currentPeriod, string currentClock)
        {
            int secondsElapsed = 0;
            int periodLengthInSeconds;

            try
            {
                if (currentClock == null)
                {
                    return 0;
                }

                for (int i = 1; i <= currentPeriod; i++)
                {
                    //Since clock up can have extra time, we can not user period length or OT length, we have to use the Period1ActualLength...Period6ActualLength
                    switch (i)
                    {
                        case 1:
                            periodLengthInSeconds = ConvertClockStringValueToSeconds(game.PeriodLength + ":00");
                            break;
                        case 2:

                            periodLengthInSeconds = ConvertClockStringValueToSeconds(game.PeriodLength + ":00");
                            break;
                        case 3:

                            if (currentPeriod > game.Periods)
                            {
                                periodLengthInSeconds = ConvertClockStringValueToSeconds(game.OverTimeLength + ":00");
                            }
                            else
                            {
                                periodLengthInSeconds = ConvertClockStringValueToSeconds(game.PeriodLength + ":00");
                            }
                            break;
                        case 4:
                            if (currentPeriod > game.Periods)
                            {
                                periodLengthInSeconds = ConvertClockStringValueToSeconds(game.OverTimeLength + ":00");
                            }
                            else
                            {
                                periodLengthInSeconds = ConvertClockStringValueToSeconds(game.PeriodLength + ":00");
                            }
                            break;
                        case 5:
                            periodLengthInSeconds = ConvertClockStringValueToSeconds(game.OverTimeLength + ":00");
                            break;
                        case 6:
                            periodLengthInSeconds = ConvertClockStringValueToSeconds(game.OverTimeLength + ":00");
                            break;
                        default:
                            periodLengthInSeconds = game.OverTimeLength * 60;
                            break;
                    }

                    if (i == currentPeriod)
                    {
                        //Since this is the current period we need to take the current clock value and subtract from it when the period started.
                        //i.e. 90:00 - 45:00)
                        int periodStartTimeInSeconds = DAL.Instance().GetPeriodStartTimeInSeconds(game, i);
                        int currentClockInSeconds = ConvertClockStringValueToSeconds(currentClock);

                        secondsElapsed = secondsElapsed + (currentClockInSeconds - periodStartTimeInSeconds);
                    }
                    else
                    {
                        secondsElapsed = secondsElapsed + periodLengthInSeconds;
                    }
                }

                return secondsElapsed;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static int CalculateRegulationTimeElapsedInPeriodInSecondsForCLOCKUP(Game game, int period)
        {
            int periodLengthInSeconds;
            int periodOTLengthInSeconds;

            try
            {
                if (period <= game.Periods)
                {
                    periodLengthInSeconds = ConvertClockStringValueToSeconds(game.PeriodLength + ":00") * period;
                }
                else   //OT Period
                {
                    periodLengthInSeconds = ConvertClockStringValueToSeconds(game.PeriodLength + ":00") * game.Periods;
                    periodOTLengthInSeconds = ConvertClockStringValueToSeconds(game.OverTimeLength + ":00") * (period - game.Periods);
                    periodLengthInSeconds = periodLengthInSeconds + periodOTLengthInSeconds;
                }

                return periodLengthInSeconds;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static void SavePlayersTimePlayedToStatTotals(int gameID, int teamID, int playerID, int secondsPlayed)
        {
            int minutes;

            try
            {
                minutes = secondsPlayed / 60;

                //This is NOT working, so commented out (I think it has to do with threading, NOT sure though)
                //BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecordForSecondsAndMinutes(gameID, teamID, playerID, secondsPlayed, minutes);

                DAL.Instance().UpsertFlatTotals(gameID, teamID, playerID, 23, 0, secondsPlayed);
                DAL.Instance().UpsertFlatTotals(gameID, teamID, playerID, 24, 0, minutes);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static int ConvertClockStringValueToSeconds(string clockStringValue)
        {
            int returnValue = 0;
            try
            {
                if (clockStringValue.Contains(":") == false)
                {
                    returnValue = -999; //EVERY clock value should contain a :
                }
                else
                {
                    int colonLocation = clockStringValue.IndexOf(":");
                    int minutesPart = Convert.ToInt16(clockStringValue.Substring(0, colonLocation));
                    int secondsPart = Convert.ToInt16(clockStringValue.Substring(colonLocation + 1));

                    returnValue = (minutesPart * 60) + secondsPart;
                }
                return returnValue;
            }

            catch (Exception ex)
            {

                return returnValue;
            }
        }

        public static int HowLongHasPlayerBeenInGame(Game game, int inPeriod, int outPeriod, string inClock, string outClock)
        {
            int playerSecondsPlayed = 0;
            int periodLength;

            try
            {
                //player was in an out in same period so all we need to do is subtract times to get player minutes/seconds
                if (inPeriod == outPeriod)
                {
                    if (game.ClockUpOrDown.ToUpper() == "UP")
                    {
                        playerSecondsPlayed = playerSecondsPlayed + ConvertClockStringValueToSeconds(outClock) - ConvertClockStringValueToSeconds(inClock);
                    }
                    else
                    {
                        playerSecondsPlayed = playerSecondsPlayed + ConvertClockStringValueToSeconds(inClock) - ConvertClockStringValueToSeconds(outClock);
                    }
                }
                else  //player was OUT in a different period than the period he started (could even be several periods, i.e. was in 1st period and did not come out until 4th period (i.e. 2nd OT)
                {
                    for (int i = inPeriod; i <= outPeriod; i++)
                    {
                        //if OT then need to make sure we adjust the period length using in calculation
                        if (i <= game.Periods) { periodLength = game.PeriodLength; } else { periodLength = game.OverTimeLength; }

                        if (i < outPeriod)
                        {
                            if (i == inPeriod)
                            {
                                if (game.ClockUpOrDown.ToUpper() == "UP")
                                {
                                    playerSecondsPlayed = playerSecondsPlayed + (periodLength * 60) - ConvertClockStringValueToSeconds(inClock);
                                }
                                else
                                {
                                    playerSecondsPlayed = playerSecondsPlayed + ConvertClockStringValueToSeconds(inClock);
                                }
                            }
                            else //this is if player in/out spanned 2 periods, if so, then add that entire periods minutes to his minutes since he played that entire period
                            {
                                playerSecondsPlayed = playerSecondsPlayed + (periodLength * 60);
                            }
                        }
                        else  //this is the period he came out, so simply add the time they came out to their minutes played
                        {
                            if (game.ClockUpOrDown.ToUpper() == "UP")
                            {
                                playerSecondsPlayed = playerSecondsPlayed + ConvertClockStringValueToSeconds(outClock);
                            }
                            else
                            {
                                playerSecondsPlayed = playerSecondsPlayed + (periodLength * 60) - ConvertClockStringValueToSeconds(outClock);
                            }
                        }
                    } //next period
                }
                return playerSecondsPlayed;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion "Routines to calculate player minutes/seconds played"

        #region "PlusMinus"

        //this should only be called for a goal scored play (shootout goals do not effect plus minus)
        public static void AdjustPlusMinus(Play play, string addOrDeletedGoal, string whosCalling)
        {
            int goalScoringTeamID;
            int goalAllowedTeamID;
            List<EventRoster> eventRosterList = new List<EventRoster>();
            ObservableCollection<PlayModel> playByPlayList = new ObservableCollection<PlayModel>();

            try
            {
                if (App.DoesUserHaveAbilityToTrackAllStats() == false)
                {
                    return;
                }

                // Debug.WriteLine(DateTime.Now + " - Start - AdjustPlusMinus. Play = " + play.GameTime + " " + play.PlayText);

                //if goal scored play (own goals count, shootout goals do not)             
                if (Common.Instance().IsThisAGCoalScoredPlay(play, true, false))
                {
                    if (play.StatCategoryID == 12)  //Own Goal (12)
                    {
                        goalAllowedTeamID = play.TeamID;
                        goalScoringTeamID = Common.Instance().GetOtherTeamID(play.GameID, play.TeamID);
                    }
                    else
                    {
                        goalScoringTeamID = play.TeamID;
                        goalAllowedTeamID = Common.Instance().GetOtherTeamID(play.GameID, play.TeamID);
                    }

                    //get list of players on the field at the time of the goal scored for the team that SCORED the goal
                    //If GM is calling then we can simply take the players on the field, otherwise call into proc to determine players on field based on the time of the play
                    if (whosCalling.ToUpper() == "GM")
                    {
                        eventRosterList = BaseTableDataAccess.Instance().GetListOfPlayersOnTheField(play.GameID, goalScoringTeamID);
                    }
                    else
                    {
                        eventRosterList = DAL.Instance().GetPlayersInGivenPointInGame(play.GameID, goalScoringTeamID, play.ElapsedTimeInSeconds);
                    }

                    //for each one of these players add 1 to their plus minus value
                    foreach (var playerInGame in eventRosterList)
                    {
                        Play plusMinus = new Play();

                        plusMinus.GameID = play.GameID;
                        plusMinus.Player1ID = (int)playerInGame.PlayerID;
                        plusMinus.TeamID = goalScoringTeamID;
                        plusMinus.StatCategoryID = 25;  //PlusMinus (25)
                        plusMinus.StatDescriptionID = 0;

                        if (addOrDeletedGoal.ToUpper() == "ADD")
                        {
                            plusMinus.PlayText = "Plus";
                        }
                        else
                        {
                            plusMinus.PlayText = "Minus";
                        }

                        DAL.Instance().UpsertStatPlayToFlatTotals(plusMinus, false);
                    }

                    //get list of players on the field at the time of the goal scored for the team that ALLOWED the goal
                    //If GM is calling then we can simply take the players on the field, otherwise call into proc to determine players on field based on the time of the play
                    if (whosCalling.ToUpper() == "GM")
                    {
                        eventRosterList = BaseTableDataAccess.Instance().GetListOfPlayersOnTheField(play.GameID, goalAllowedTeamID);
                    }
                    else
                    {
                        eventRosterList = DAL.Instance().GetPlayersInGivenPointInGame(play.GameID, goalAllowedTeamID, play.ElapsedTimeInSeconds);
                    }

                    //for each one of these players add -1 to their plus minus value
                    foreach (var playerInGame in eventRosterList)
                    {
                        Play plusMinus = new Play();

                        plusMinus.GameID = play.GameID;
                        plusMinus.Player1ID = (int)playerInGame.PlayerID;
                        plusMinus.TeamID = goalAllowedTeamID;
                        plusMinus.StatCategoryID = 25;  //PlusMinus (25)
                        plusMinus.StatDescriptionID = 0;

                        if (addOrDeletedGoal.ToUpper() == "ADD")
                        {
                            plusMinus.PlayText = "Minus";
                        }
                        else
                        {
                            plusMinus.PlayText = "Plus";
                        }

                        DAL.Instance().UpsertStatPlayToFlatTotals(plusMinus, false);
                    }
                    //   Debug.WriteLine(DateTime.Now + " - End - AdjustPlusMinus. Play = " + play.GameTime + " " + play.PlayText);
                }
            }

            catch (Exception ex)
            {
                throw;
            }

        }

        public static void CalculateALLPlayerPlusMinus(int gameID)
        {
            int goalScoringTeamID;
            int goalAllowedTeamID;
            int elapsedTimeInGameAtTimeOfGoalScored;
            ObservableCollection<PlayModel> playByPlayList = new ObservableCollection<PlayModel>();
            List<EventRoster> subPlayerINListForGoalScoringTeam = new List<EventRoster>();
            List<EventRoster> subPlayerINListForGoalAllowedTeam = new List<EventRoster>();

            try
            {
                //  Debug.WriteLine(DateTime.Now + " - Start - CalculateALLPlayerPlusMinus. GameID =" + gameID);

                BaseTableDataAccess.Instance().DeleteEntireGamesPlusMinusStats(gameID);

                //Get list of all plays for the game          
                playByPlayList = DAL.Instance().GetPlaysForGame(gameID, "ASC");

                foreach (var playEntry in playByPlayList)
                {
                    //for every goal scored play in game   (own goals count, shootout goals do not)               
                    if (Common.Instance().IsThisAGCoalScoredPlay(playEntry.Play, true, false) == true)
                    {
                        if (playEntry.Play.StatCategoryID == 12)    //Own Goal (12)
                        {
                            goalAllowedTeamID = playEntry.Play.TeamID;
                            goalScoringTeamID = Common.Instance().GetOtherTeamID(playEntry.Play.GameID, playEntry.Play.TeamID);
                        }
                        else
                        {
                            goalScoringTeamID = playEntry.Play.TeamID;
                            goalAllowedTeamID = Common.Instance().GetOtherTeamID(playEntry.Play.GameID, playEntry.Play.TeamID);
                        }

                        elapsedTimeInGameAtTimeOfGoalScored = playEntry.Play.ElapsedTimeInSeconds;

                        //get list of players on the field at the time of the goal scored for the team that SCORED the goal
                        subPlayerINListForGoalScoringTeam = DAL.Instance().GetPlayersInGivenPointInGame(gameID, goalScoringTeamID, elapsedTimeInGameAtTimeOfGoalScored);

                        //for each one of these players add 1 to their plus minus value
                        foreach (var playerInGame in subPlayerINListForGoalScoringTeam)
                        {
                            Play plusMinus = new Play();

                            plusMinus.GameID = gameID;
                            plusMinus.Player1ID = playerInGame.PlayerID;
                            plusMinus.TeamID = goalScoringTeamID;
                            plusMinus.StatCategoryID = 25;  //PlusMinus (25)
                            plusMinus.StatDescriptionID = 0;
                            plusMinus.PlayText = "Plus";
                            DAL.Instance().UpsertStatPlayToFlatTotals(plusMinus, false);
                        }

                        //get list of players on the field at the time of the goal scored for the team that ALLOWED the goal
                        subPlayerINListForGoalAllowedTeam = DAL.Instance().GetPlayersInGivenPointInGame(gameID, goalAllowedTeamID, elapsedTimeInGameAtTimeOfGoalScored);

                        //for each one of these players add -1 to their plus minus value
                        foreach (var playerInGame in subPlayerINListForGoalAllowedTeam)
                        {
                            Play plusMinus = new Play();

                            plusMinus.GameID = gameID;
                            plusMinus.Player1ID = playerInGame.PlayerID;
                            plusMinus.TeamID = goalAllowedTeamID;
                            plusMinus.StatCategoryID = 25;   //PlusMinus (25)
                            plusMinus.StatDescriptionID = 0;
                            plusMinus.PlayText = "Minus";
                            DAL.Instance().UpsertStatPlayToFlatTotals(plusMinus, false);
                        }
                    }
                }
                //     Debug.WriteLine(DateTime.Now + " - End - CalculateALLPlayerPlusMinus. GameID =" + gameID);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        #endregion "PlusMinus"

    }
}
