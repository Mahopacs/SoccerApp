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
    public static class PBPModule
    {

        #region "Methods"

       
        public static void GoalWasDeletedBackOutGoalFromGameScoreAndUpdatePBPRunningScore(Play play, int goalScoringTeamID)
        {
            try
            {
                //Need to get the current home and away score so we can update it
                Game game = new Game();
                game = BaseTableDataAccess.Instance().GetGameByGameID(play.GameID);

                if (game.HomeTeamID == goalScoringTeamID)
                {
                    game.HomeTeamScore = game.HomeTeamScore - 1;
                }
                else
                {
                    game.AwayTeamScore = game.AwayTeamScore - 1;
                }

                BaseTableDataAccess.Instance().UpdateGameAwayTeamScore(play.GameID, game.AwayTeamScore);
                BaseTableDataAccess.Instance().UpdateGameHomeTeamScore(play.GameID, game.HomeTeamScore);

                //update the running score of the game
                RebuildRunningScoreDueToScoringPlayChangedNEW(play, "DELETE");
               // RebuildRunningScoreDueToScoringPlayChangedOLD(play);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static void ShootOutGoalWasDeletedBackOutGoalFromGameShootoutScore(int gameID, int goalScoringTeamID)
        {
            try
            {
                //Need to get the current home and away score so we can update it
                Game game = new Game();
                game = BaseTableDataAccess.Instance().GetGameByGameID(gameID);

                if (game.HomeTeamID == goalScoringTeamID)
                {
                    game.HomeTeamShootOutGoals = game.HomeTeamShootOutGoals - 1;
                }
                else
                {
                    game.AwayTeamShootOutGoals = game.AwayTeamShootOutGoals - 1;
                }

                BaseTableDataAccess.Instance().UpdateGameAwayTeamShootoutScore(gameID, game.AwayTeamShootOutGoals);
                BaseTableDataAccess.Instance().UpdateGameHomeTeamShootoutScore(gameID, game.HomeTeamShootOutGoals);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static void BackOutTeamShootOutOpp(int gameID, int goalScoringTeamID)
        {
            try
            {
                //Need to get the current home and away score so we can update it
                Game game = new Game();
                game = BaseTableDataAccess.Instance().GetGameByGameID(gameID);

                if (game.HomeTeamID == goalScoringTeamID)
                {
                    game.HomeTeamShootOutGoalOpp = game.HomeTeamShootOutGoalOpp - 1;
                }
                else
                {
                    game.AwayTeamShootOutGoalOpp = game.AwayTeamShootOutGoalOpp - 1;
                }

                BaseTableDataAccess.Instance().UpdateGameAwayTeamShootoutOpp(gameID, game.AwayTeamShootOutGoalOpp);
                BaseTableDataAccess.Instance().UpdateGameHomeTeamShootoutOpp(gameID, game.HomeTeamShootOutGoalOpp);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static void RebuildRunningScoreDueToScoringPlayChangedNEW(Play play, string addOrDeletedGoal)
        {
            string homeOrAwayTeamGoal = string.Empty;

            try
            {
              //  Debug.WriteLine(DateTime.Now + " - Start - RebuildRunningScoreDueToScoringPlayChangedNEW");

                GameModel game = new GameModel();
                game = DAL.Instance().GetGame(play.GameID);
                
                //Determine the team id of the goalscoring team, determine if they are home or away (do not count own goals or shootout goals)       
                if (Common.Instance().IsThisAGCoalScoredPlay(play, false, false))
                {
                    if (game.Game.HomeTeamID == play.TeamID) { homeOrAwayTeamGoal = "Home"; } else { homeOrAwayTeamGoal = "Away"; }
                }
                else if (play.StatCategoryID == 12) //Own Goal (12)
                {
                    if (game.Game.AwayTeamID == play.TeamID) { homeOrAwayTeamGoal = "Home"; } else { homeOrAwayTeamGoal = "Away"; }
                }

                if (homeOrAwayTeamGoal == "Home")
                {
                    if (addOrDeletedGoal.ToUpper() == "ADD")
                    {
                        BaseTableDataAccess.Instance().AddOneToHomePlayScoreForPlaysForGameAfterDeletedGoal(play.GameID, play.ElapsedTimeInSeconds);
                    }
                    else
                    {
                        BaseTableDataAccess.Instance().SubtractOneFromHomePlayScoreForPlaysForGameAfterDeletedGoal(play.GameID, play.ElapsedTimeInSeconds);
                    }
                }
                else if (homeOrAwayTeamGoal == "Away")
                {
                    if (addOrDeletedGoal.ToUpper() == "ADD")
                    {
                        BaseTableDataAccess.Instance().AddOneToAwayPlayScoreForPlaysForGameAfterDeletedGoal(play.GameID, play.ElapsedTimeInSeconds);
                    }
                    else
                    {
                        BaseTableDataAccess.Instance().SubtractOneFromAwayPlayScoreForPlaysForGameAfterDeletedGoal(play.GameID, play.ElapsedTimeInSeconds);
                    }
                }
               // Debug.WriteLine(DateTime.Now + " - End - RebuildRunningScoreDueToScoringPlayChangedNEW");
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        //This routine will rebuild the games running score by starting with the first play of the game and looking for goals scored and update the running score as it finds goals scored
        //public static void RebuildRunningScoreDueToScoringPlayChangedOLD(Play play)
        //{
        //    int currentHomeScore = 0;
        //    int currentAwayScore = 0;

        //    try
        //    {
        //        //Needed so we know the home and away team ids, this way we know which team to update score for when a goal is scored
        //        GameModel game = new GameModel();
        //        game = DAL.Instance().GetGame(play.GameID);

        //        //Get list of all plays for the game, If goal scored then update the play home and away score (i.e. running score)
        //        ObservableCollection<PlayModel> playByPlayList = new ObservableCollection<PlayModel>();
        //        playByPlayList = DAL.Instance().GetPlaysForGame(play.GameID, "ASC");

        //        foreach (var playEntry in playByPlayList)
        //        {
        //            if (((DAL.Instance().GetStatDescriptionNameById(playEntry.Play.StatDescriptionID) == "Goal") ||
        //                (DAL.Instance().GetStatDescriptionNameById(playEntry.Play.StatDescriptionID) == "For Goal")) && (DAL.Instance().GetStatCategoryNameById(playEntry.Play.StatCategoryID) != "Shootout Kick"))
        //            {

        //                if (game.Game.HomeTeamID == playEntry.Play.TeamID)
        //                {
        //                    currentHomeScore = currentHomeScore + 1;
        //                }
        //                else
        //                {
        //                    currentAwayScore = currentAwayScore + 1;
        //                }
        //            }

        //            if (playEntry.Play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Own Goal"))
        //            {
        //                if (game.Game.AwayTeamID == playEntry.Play.TeamID)
        //                {
        //                    currentHomeScore = currentHomeScore + 1;
        //                }
        //                else
        //                {
        //                    currentAwayScore = currentAwayScore + 1;
        //                }
        //            }

        //            //Finished the play, now update the play with the "new" play home and away score
        //            BaseTableDataAccess.UpdatePlayAwayTeamScore(play.GameID, playEntry.Play.PlayID, currentAwayScore);
        //            BaseTableDataAccess.UpdatePlayHomeTeamScore(play.GameID, playEntry.Play.PlayID, currentHomeScore);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

        //UPDATE We are no longer rebuilding plus minus during scoring changes, instead it is rebuild only when needed (i.e. on demand)
        //We will be updating running score, and this is now in RebuildRunningScoreDueToScoringPlayerChanged
        //Every play has a HomeScore and AwayScore value (i.e. running score), when a scoring play (i.e. goal) is deleted
        //we need to go through the entire pbp list and re-calculate the play home and away score (i.e. running score) for every play
        //then once done update the game score
        //public static void RebuildPlusMinusAndPlayScoresFromPBPDueToScoringPlayChanged(int gameID)
        //{
        //    GameModel game = new GameModel();
        //    game = DAL.Instance().GetGame(gameID);

        //    //Reset event roster to how it was when game started so subs plays are processed correct
        //    //To do this we need to set anyone who was a starter to on the field and anyone who was not to not on the field
        //    List<EventRoster> eventRosterList = new List<EventRoster>();
        //    eventRosterList = BaseTableDataAccess.GetEventRosterByGameID(gameID);

        //    foreach (var eventRosterEntry in eventRosterList)
        //    {
        //        if (eventRosterEntry.Starter == "Y")
        //        {
        //            BaseTableDataAccess.UpdatePlayersOnFieldStatus(gameID, eventRosterEntry.TeamID, eventRosterEntry.PlayerID, "Y");
        //        }
        //        else
        //        {
        //            BaseTableDataAccess.UpdatePlayersOnFieldStatus(gameID, eventRosterEntry.TeamID, eventRosterEntry.PlayerID, "N");
        //        }
        //    }

        //    //Get list of all plays for the game
        //    //For each play:
        //    //1) If sub, update players on field (so it can be used for plus minus calculation if/when a goal is scored
        //    //2) If goal, update the play home and away score (i.e. running score), update the game score, and update plus minus stats for players on the field 
        //    ObservableCollection<PlayModel> playByPlayList = new ObservableCollection<PlayModel>();
        //    playByPlayList = DAL.Instance().GetPlaysForGame(gameID, "ASC");

        //    //Set game score to 0-0 so can rebuild running score on all plays
        //    BaseTableDataAccess.UpdateGameAwayTeamScore(gameID, 0);
        //    BaseTableDataAccess.UpdateGameHomeTeamScore(gameID, 0);
        //    BaseTableDataAccess.DeleteEntireGamesPlusMinusStats(gameID);

        //    foreach (var playEntry in playByPlayList)
        //    {
        //        //Prior to each play get the game score
        //        game = DAL.Instance().GetGame(playEntry.Play.GameID);

        //        if (playEntry.Play.StatCategoryID == 22)
        //        {
        //            BaseTableDataAccess.UpdatePlayersOnFieldStatus(playEntry.Play.GameID, playEntry.Play.TeamID, playEntry.Play.Player1ID, "Y");
        //            BaseTableDataAccess.UpdatePlayersOnFieldStatus(playEntry.Play.GameID, playEntry.Play.TeamID, playEntry.Play.Player2ID, "N");
        //        }

        //        if (DAL.Instance().GetStatDescriptionNameById(playEntry.Play.StatDescriptionID) == "Goal")
        //        {
        //            //Calculate/Update PlusMinus Stats
        //            int otherTeamID = Common.Instance().GetOtherTeamID(game.Game.GameID, playEntry.Play.TeamID);
        //            StatCalculationsModule.AdjustPlusMinus(playEntry.Play.GameID, playEntry.Play.TeamID, otherTeamID);

        //            if (game.Game.HomeTeamID == playEntry.Play.TeamID)
        //            {
        //                game.Game.HomeTeamScore = game.Game.HomeTeamScore + 1;
        //                BaseTableDataAccess.UpdateGameHomeTeamScore(game.Game.GameID, game.Game.HomeTeamScore);
        //            }
        //            else
        //            {
        //                game.Game.AwayTeamScore = game.Game.AwayTeamScore + 1;
        //                BaseTableDataAccess.UpdateGameAwayTeamScore(game.Game.GameID, game.Game.AwayTeamScore);
        //            }
        //        }

        //        //Finished the play, now update the play with the "new" play home and away score, also update game current score
        //        BaseTableDataAccess.UpdatePlayAwayTeamScore(game.Game.GameID, playEntry.Play.PlayID, game.Game.AwayTeamScore);
        //        BaseTableDataAccess.UpdatePlayHomeTeamScore(game.Game.GameID, playEntry.Play.PlayID, game.Game.HomeTeamScore);
        //    }
        //}

        //7/16/14 UPDATE We are no longer calculating plus minus or player minutes for each sub, only when needed (i.e on demand)
        //Go through every play and rebuild plus minus and calculate player minutes at end
        //public static void RebuildPlusMinusAndMinutesPlayedFromPBPDueToSubstitutionPlayChanged(int gameID)
        //{
        //    //Reset event roster to how it was when game started so subs plays are processed correct
        //    //To do this we need to set anyone who was a starter to on the field and anyone who was not to not on the field
        //    List<EventRoster> eventRosterList = new List<EventRoster>();
        //    eventRosterList = BaseTableDataAccess.GetEventRosterByGameID(gameID);

        //    foreach (var eventRosterEntry in eventRosterList)
        //    {
        //        if (eventRosterEntry.Starter == "Y")
        //        {
        //            BaseTableDataAccess.UpdatePlayersOnFieldStatus(gameID, eventRosterEntry.TeamID, eventRosterEntry.PlayerID, "Y");
        //        }
        //        else
        //        {
        //            BaseTableDataAccess.UpdatePlayersOnFieldStatus(gameID, eventRosterEntry.TeamID, eventRosterEntry.PlayerID, "N");
        //        }
        //    }

        //    //Get list of all plays for the game.  For each play:
        //    //1) If sub, update players on field (so it can be used for plus minus calculation if/when a goal is scored
        //    //2) If goal, recalculate plus minus
        //    ObservableCollection<PlayModel> playByPlayList = new ObservableCollection<PlayModel>();
        //    playByPlayList = DAL.Instance().GetPlaysForGame(gameID, "ASC");

        //    BaseTableDataAccess.DeleteEntireGamesPlusMinusStats(gameID);

        //    foreach (var playEntry in playByPlayList)
        //    {
        //        if (playEntry.Play.StatCategoryID == 22)
        //        {
        //            BaseTableDataAccess.UpdatePlayersOnFieldStatus(playEntry.Play.GameID, playEntry.Play.TeamID, playEntry.Play.Player1ID, "Y");
        //            BaseTableDataAccess.UpdatePlayersOnFieldStatus(playEntry.Play.GameID, playEntry.Play.TeamID, playEntry.Play.Player2ID, "N");
        //        }

        //        if (DAL.Instance().GetStatDescriptionNameById(playEntry.Play.StatDescriptionID) == "Goal")
        //        {
        //            //Calculate/Update PlusMinus Stats
        //            int otherTeamID = Common.Instance().GetOtherTeamID(gameID, playEntry.Play.TeamID);
        //            StatCalculationsModule.AdjustPlusMinus(playEntry.Play.GameID, playEntry.Play.TeamID, otherTeamID);
        //        }
        //    }
        //    StatCalculationsModule.CalculateALLPlayerMinutes(gameID);
        //}



        #endregion "Methods"
    }
}
