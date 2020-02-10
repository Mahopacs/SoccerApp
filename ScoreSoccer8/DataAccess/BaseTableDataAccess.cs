using ScoreSoccer8.Cloud;
using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.DataObjects.UiClasses;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreSoccer8.DataAccess
{
    public class BaseTableDataAccess
    {

        public static BaseTableDataAccess 
            ()
        {
            return new BaseTableDataAccess();
        }

        #region "Players"

        public Player GetPlayerByPlayerID(int? playerID)
        {
            Player returnValue = new Player();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Player>("Select * from player p where p.playerID = " + playerID).FirstOrDefault();
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public Player GetPlayerByPlayerFirstName(string firstName)
        {
            Player returnValue = new Player();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Player>("Select * from player p where p.firstname = '" + firstName + "'").FirstOrDefault();
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public void DeletePlayer(int playerID)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                         {
                             db.Delete<Player>(playerID);
                             success = true;
                         });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public List<Player> GetAllPlayersList()
        {
            List<Player> returnValue = new List<Player>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Player>("Select * from player p order by visible DESC, upper(lastname)");
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public Dictionary<int, Player> GetAllPlayersDictionary()
        {
            Dictionary<int, Player> returnValue = new Dictionary<int, Player>();

            GetAllPlayersList().ForEach(x => returnValue.Add(x.PlayerID, x));

            return returnValue;
        }

        public void UpsertPlayer(Player player, int gameID, int teamID = 0)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            if (player.PlayerID == 0)
                            {
                                player.Visible = "Y";

                                if (player.SampleData != "Y")
                                {
                                    player.SampleData = "N";
                                }

                                if (player.BirthDate.Year < 1900)
                                {
                                    player.BirthDate = DateTime.Now;
                                }

                                db.Insert(player);

                                //if this is a game, insert player into Event Roster too
                                if (gameID != 0)
                                {
                                    EventRoster eventRosterEntry = new EventRoster();
                                    eventRosterEntry.GameID = gameID;
                                    eventRosterEntry.PlayerID = player.PlayerID;
                                    eventRosterEntry.Starter = "N";
                                    DAL.Instance().UpdatePlayersGameStartedStat(gameID, teamID, player.PlayerID, false);
                                    db.Insert(eventRosterEntry);
                                }
                            }
                            else
                            {
                                db.Update(player);
                            }
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }

        }



        #endregion "Players"

        #region "TeamRoster"

        public List<TeamRoster> GetTeamRosterByTeamList(int teamID)
        {
            List<TeamRoster> returnValue = new List<TeamRoster>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<TeamRoster>("Select * from teamroster t where t.teamid = " + teamID + " AND visible = 'Y' order by active DESC, CAST(uniformnumber as integer)");
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public TeamRoster GetPlayerTeamRosterEntry(int teamID, int playerID)
        {
            TeamRoster returnValue = new TeamRoster();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<TeamRoster>("Select * from TeamRoster Where teamID = " + teamID + " and playerID = " + playerID).FirstOrDefault();
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public Dictionary<int, TeamRoster> GetTeamRosterByTeamDictionary(int teamID)
        {
            Dictionary<int, TeamRoster> returnValue = new Dictionary<int, TeamRoster>();

            GetTeamRosterByTeamList(teamID).ForEach(x => returnValue.Add(x.TeamID + x.PlayerID, x));

            return returnValue;
        }

        public List<TeamRoster> GetAllTeamRosterList()
        {
            List<TeamRoster> returnValue = new List<TeamRoster>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<TeamRoster>("Select * from TeamRoster t");
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Key is Team ID + Player ID
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, TeamRoster> GetAllTeamRosterDictionary()
        {
            Dictionary<string, TeamRoster> returnValue = new Dictionary<string, TeamRoster>();

            GetAllTeamRosterList().ForEach(x => returnValue.Add(x.TeamID.ToString() + x.PlayerID.ToString(), x));

            return returnValue;
        }

        public void DeletePlayerFromAnyAllTeamRosters(int playerID)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<TeamRoster>("Update TeamRoster set visible = 'N' Where playerID = " + playerID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void DeleteTeamRoster(int teamID, int playerID)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            //8/31/14 TJY Do not want to delete from team roster, otherwise GM blows up because GetPlayersPhyiscalAndTeamRosterInfo blows up
                            //because the player is in eventRoster but not in teamRoster, so just set to visible = 'N'
                            //db.Query<TeamRoster>("Delete from TeamRoster Where teamID = " + teamID + " AND playerID = " + playerID);
                            db.Query<TeamRoster>("Update TeamRoster set Visible = 'N' Where teamID = " + teamID + " AND playerID = " + playerID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public ObservableCollection<TeamRosterModel> GetTeamRoster(int teamID)
        {
            ObservableCollection<TeamRosterModel> returnValue = new ObservableCollection<TeamRosterModel>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            var playerDictionary = BaseTableDataAccess.Instance().GetAllPlayersDictionary();
                            var teamRosterTable = BaseTableDataAccess.Instance().GetTeamRosterByTeamList(teamID);
                            var teamThatIsBeingLoaded = BaseTableDataAccess.Instance().GetTeamByTeamID(teamID);

                            //Build Team Roster Model and add to results
                            foreach (var row in teamRosterTable)
                            {
                                returnValue.Add(new TeamRosterModel(teamThatIsBeingLoaded, playerDictionary[row.PlayerID], row));
                            }
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        //Since TeamRoster does not have primary key need to upsert this way because db.update is not available

        //public void DeleteInsertTeamRoster(TeamRoster teamRoster)
        //{
        //    bool success = false;
        //    int tries = 0;
        //    TeamRoster playerTeamRosterEntry = new TeamRoster();

        //    while ((success == false) && (tries < App.gDBRetryLimit))
        //    {
        //        try
        //        {
        //            using (var db = new SQLiteConnection(App.gDBPath))
        //            {
        //                db.RunInTransaction(() =>
        //                {
        //                    DAL.Instance().DeleteTeamRoster(teamRoster);

        //                    playerTeamRosterEntry = BaseTableDataAccess.Instance().GetPlayerTeamRosterEntry(teamRoster.TeamID, teamRoster.PlayerID);

        //                    if (playerTeamRosterEntry == null)  //If null then this is the first time this player has been added to this team, simply insert them
        //                    {
        //                        teamRoster.Visible = "Y";
        //                        db.Insert(teamRoster);
        //                    }
        //                    else
        //                    {
        //                        teamRoster.Visible = "Y";
        //                        BaseTableDataAccess.Instance().UpdatePlayersTeamRosterEntry(teamRoster.TeamID, teamRoster.PlayerID, teamRoster.Active, teamRoster.UniformNumber, teamRoster.Visible); 
        //                    }
        //                    success = true;
        //                });
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            tries = tries + 1;
        //            Thread.Sleep(App.gDBRetrySleepTime);
        //        }
        //    }
        //}

        public void UpsertTeamRoster(TeamRoster teamRoster)
        {
            bool success = false;
            int tries = 0;
            TeamRoster playerTeamRosterEntry = new TeamRoster();

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {  
                            playerTeamRosterEntry = BaseTableDataAccess.Instance().GetPlayerTeamRosterEntry(teamRoster.TeamID, teamRoster.PlayerID);

                            teamRoster.Visible = "Y";

                            if (playerTeamRosterEntry == null)  //If null then this is the first time this player has been added to this team, simply insert them
                            {                               
                                db.Insert(teamRoster);
                            }
                            else
                            {                                
                                BaseTableDataAccess.Instance().UpdatePlayersTeamRosterEntry(teamRoster.TeamID, teamRoster.PlayerID, teamRoster.Active, teamRoster.UniformNumber, teamRoster.Visible);
                            }
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }



        public void UpdatePlayersTeamRosterEntry(int teamID, int playerID, string active, string uniformNumber, string visible)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<TeamRoster>("Update TeamRoster set uniformNumber = '" + uniformNumber + "', active = '" + active + "', visible = '" + visible + "' where teamID = " + teamID + " and playerID = " + playerID);
                            success = true;
                        });
                    }
                }
                catch (Exception ex)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        #endregion "TeamRoster"

        #region "Teams"

        public Team GetTeamByTeamID(int teamID)
        {
            Team returnValue = new Team();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Team>("Select * from team t where t.teamid = " + teamID).FirstOrDefault();
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public Team GetTeamByTeamName(string teamName)
        {
            Team returnValue = new Team();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Team>("Select * from team t where upper(t.teamName) = '" + teamName.ToUpper() + "'").FirstOrDefault();
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public Team GetTeamByTeamShortName(string teamShortName)
        {
            Team returnValue = new Team();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Team>("Select * from team t where upper(t.teamShortName) = '" + teamShortName.ToUpper() + "'").FirstOrDefault();
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public List<Team> GetAllTeamsList()
        {
            List<Team> returnValue = new List<Team>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Team>("Select * from team t order by upper(teamname)");
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public Dictionary<int, Team> GetAllTeamsDictionary()
        {
            Dictionary<int, Team> returnValue = new Dictionary<int, Team>();

            GetAllTeamsList().ForEach(x => returnValue.Add(x.TeamID, x));

            return returnValue;
        }

        public void UpsertTeam(Team team)
        {
            bool _newTeam = false;
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            if (team.TeamID == 0)
                            {
                                if (team.SampleData != "Y")
                                {
                                    team.SampleData = "N";
                                }

                                _newTeam = true;
                                team.Visible = "Y";
                                db.Insert(team);
                            }
                            else
                            {
                                _newTeam = false;
                                db.Update(team);
                            }
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }





        #endregion "Teams"

        #region "Leagues"

        public List<League> GetAllLeaguesList()
        {
            List<League> returnValue = new List<League>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<League>("Select * from league l Order By l.Visible ASC, upper(l.leagueName) ASC");
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public League GetLeagueByLeagueID(int leagueID)
        {
            League returnValue = new League();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<League>("Select * from league l where l.leagueID = " + leagueID).FirstOrDefault();
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public void UpsertLeague(League league)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            if (league.LeagueID == 0)
                            {
                                league.Visible = "Y";
                                db.Insert(league);
                            }
                            else
                            {
                                db.Update(league);
                            }
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        #endregion "Leagues"

        #region "Games"

        public List<Game> GetAllGamesList()
        {
            List<Game> returnValue = new List<Game>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Game>("Select * from game order by gamedate DESC");
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public List<Game> GetGamesForTeam(int teamID)
        {
            List<Game> returnValue = new List<Game>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Game>("Select * from Game where (hometeamID = " + teamID + " OR awayteamID = " + teamID + ") and gameStatus NOT IN ('NOT STARTED') Order By gamedate DESC");
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public List<Game> GetAllGamesForTeamOrderByGameDateASC(int teamID)
        {
            List<Game> returnValue = new List<Game>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Game>("Select * from Game where (hometeamID = " + teamID + " OR awayteamID = " + teamID + ") Order By gamedate ASC");
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public Game GetGameByGameID(int gameID)
        {
            Game returnValue = new Game();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Game>("Select * from game g where g.gameID = " + gameID).FirstOrDefault();
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public void DeleteGame(int gameID)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Delete<Game>(gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpsertGame(Game game)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            if (game.GameID == 0)
                            {
                                if (game.SampleData != "Y")
                                {
                                    game.SampleData = "N";
                                }

                                db.Insert(game);
                            }
                            else
                            {
                                db.Update(game);
                            }
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdateHomeTeamSideOfField(int gameID, string sideOfField)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update game set HomeTeamSideOfField = '" + sideOfField + "' where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdateAwayTeamSideOfField(int gameID, string sideOfField)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update game set AwayTeamSideOfField = '" + sideOfField + "' where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdateGameCurrentPeriodAndCurrentClock(int gameID, int currentPeriod, string currentClock, int gameElapsedTimeInSeconds)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update game set currentElapsedTimeInSeconds = " + gameElapsedTimeInSeconds + ", currentperiod = " + currentPeriod + ", currentClock = '" + currentClock + "' where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdateGameCurrentClock(int gameID, string currentClock)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update game set currentClock = '" + currentClock + "' where gameID = " + gameID); success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdateGamesPeriod1ActualLength(int gameID, string actualPeriodLength)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update game set period1actuallength = '" + actualPeriodLength + "' where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }

        }

        public void UpdateGamesPeriod2ActualLength(int gameID, string actualPeriodLength)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update game set period2actuallength = '" + actualPeriodLength + "' where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdateGamesPeriod3ActualLength(int gameID, string actualPeriodLength)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update game set period3actuallength = '" + actualPeriodLength + "' where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdateGamesPeriod4ActualLength(int gameID, string actualPeriodLength)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update game set period4actuallength = '" + actualPeriodLength + "' where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdateGamesPeriod5ActualLength(int gameID, string actualPeriodLength)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update game set period5actuallength = '" + actualPeriodLength + "' where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdateGamesPeriod6ActualLength(int gameID, string actualPeriodLength)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update game set period6actuallength = '" + actualPeriodLength + "' where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdateGamesOT1ActualLength(int gameID, string actualPeriodLength)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update game set ot1actuallength = '" + actualPeriodLength + "' where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdateGamesOT2ActualLength(int gameID, string actualPeriodLength)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update game set ot2actuallength = '" + actualPeriodLength + "' where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdateGameHomeTeamScore(int gameID, int goalTotal)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update game set hometeamscore = " + goalTotal + " where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdateGameHomeTeamShootoutScore(int gameID, int goalTotal)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update game set homeTeamShootOutGoals = " + goalTotal + " where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdateGameHomeTeamShootoutOpp(int gameID, int oppTotal)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update game set homeTeamShootOutGoalOpp = " + oppTotal + " where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdateGameAwayTeamScore(int gameID, int goalTotal)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update game set awayteamscore = " + goalTotal + " where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdateGameAwayTeamShootoutScore(int gameID, int goalTotal)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update game set awayTeamShootOutGoals = " + goalTotal + " where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdateGameAwayTeamShootoutOpp(int gameID, int oppTotal)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update game set awayTeamShootOutGoalOpp = " + oppTotal + " where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdateGameStatus(int gameID, string gameStatus)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update game set gameStatus = '" + gameStatus + "' where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdateGameTeamDecisions(int gameID, string homeTeamDecision, string awayTeamDecision)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update game set homeTeamDecision = '" + homeTeamDecision + "' and awayTeamDecision = '" + awayTeamDecision + "' where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        #endregion "Games"

        #region "Plays"

        public List<Play> GetAllPlaysForGame(int gameID, string sortByElapsedTimeAscOrDesc)
        {
            List<Play> returnValue = new List<Play>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            if (sortByElapsedTimeAscOrDesc == "ASC")
                            {
                                returnValue = db.Query<Play>("Select * from play p  where p.gameID = " + gameID + " order by ElapsedTimeInSeconds ASC, playID ASC");
                            }
                            else
                            {
                                returnValue = db.Query<Play>("Select * from play p  where p.gameID = " + gameID + " order by ElapsedTimeInSeconds DESC, playID DESC");
                            }
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public List<Play> GetAllSubPlaysForGame(int gameID)
        {
            List<Play> returnValue = new List<Play>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Play>("Select * from play p where p.statcategoryID = 22 and p.gameID = " + gameID + " order by p.playID");
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public List<Play> GetPlaysForEntireApp()
        {
            List<Play> returnValue = new List<Play>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Play>("Select * from play p");
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public List<Play> GetPlaysForGamePriorToElapsedTime(int gameID, int elapsedTimeInSeconds)
        {
            List<Play> returnValue = new List<Play>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Play>("Select * from play p  where gameID = " + gameID + " AND elapsedTimeInSeconds <= " + elapsedTimeInSeconds + " order by ElapsedTimeInSeconds DESC, playID DESC");
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;

        }

        public List<Play> GetPlaysForGameAfterEqualToElapsedTime(int gameID, int elapsedTimeInSeconds)
        {
            List<Play> returnValue = new List<Play>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Play>("Select * from play p  where gameID = " + gameID + " AND elapsedTimeInSeconds >= " + elapsedTimeInSeconds + " order by ElapsedTimeInSeconds DESC");
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public void SubtractOneFromHomePlayScoreForPlaysForGameAfterDeletedGoal(int gameID, int elapsedTimeInSeconds)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Play>("Update Play Set homescore = homescore - 1 Where gameID = " + gameID + " AND elapsedTimeInSeconds > " + elapsedTimeInSeconds);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void SubtractOneFromAwayPlayScoreForPlaysForGameAfterDeletedGoal(int gameID, int elapsedTimeInSeconds)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Play>("Update Play Set awayscore = awayscore - 1 Where gameID = " + gameID + " AND elapsedTimeInSeconds > " + elapsedTimeInSeconds);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void AddOneToHomePlayScoreForPlaysForGameAfterDeletedGoal(int gameID, int elapsedTimeInSeconds)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Play>("Update Play Set homescore = homescore + 1 Where gameID = " + gameID + " AND elapsedTimeInSeconds > " + elapsedTimeInSeconds);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void AddOneToAwayPlayScoreForPlaysForGameAfterDeletedGoal(int gameID, int elapsedTimeInSeconds)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Play>("Update Play Set awayscore = awayscore + 1 Where gameID = " + gameID + " AND elapsedTimeInSeconds > " + elapsedTimeInSeconds);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public List<Play> GetAllSubPlaysForGameByPlayerID(int gameID, int playerID, int teamID)
        {
            List<Play> returnValue = new List<Play>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Play>("Select * from play p where p.statcategoryID = 22 and p.gameID = " + gameID + " and p.teamID = " + teamID + " and (p.player1ID = " + playerID + " or p.player2ID = " + playerID + ") order by p.playID");
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public void UpsertPlay(Play play)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            if (play.PlayID == 0)
                            {
                                db.Insert(play);
                            }
                            else
                            {
                                db.Update(play);
                            }
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public Play GetPlay(int gameID, int playID)
        {
            Play returnValue = new Play();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Play>("Select * from play p where p.gameID = " + gameID + " AND p.playID = " + playID).FirstOrDefault();
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public void DeletePlay(int gameID, int playID)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Play>("Delete from Play Where gameID = " + gameID + " AND playID = " + playID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdatePlayHomeTeamScore(int gameID, int playID, int goalTotal)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Play>("Update play set homescore = " + goalTotal + " where gameID = " + gameID + " AND playID = " + playID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdatePlayAwayTeamScore(int gameID, int playID, int goalTotal)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Play>("Update play set awayscore = " + goalTotal + " where gameID = " + gameID + " AND playID = " + playID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        #endregion "Plays"

        #region "Jerseys"

        public void UpsertJersey(Jersey jersey)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            if (jersey.JerseyID == 0)
                            {
                                db.Insert(jersey);
                            }
                            else
                            {
                                db.Update(jersey);
                            }
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public List<Jersey> GetAllJerseysList()
        {
            List<Jersey> returnValue = new List<Jersey>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Jersey>("Select * from jersey");
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public Jersey GetJerseyByJerseyID(int jerseyID)
        {
            Jersey returnValue = new Jersey();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Jersey>("Select * from jersey j where j.jerseyID = " + jerseyID).FirstOrDefault();
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        #endregion "Jerseys"

        #region "Formations"

        public void UpsertFormation(Formation formation)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            if (formation.FormationID == 0)
                            {
                                db.Insert(formation);
                            }
                            else
                            {
                                db.Update(formation);
                            }
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public Formation GetFormationByFormationID(int formationID)
        {
            Formation returnValue = new Formation();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Formation>("Select * from formation f where f.formationID = " + formationID).FirstOrDefault();
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public List<Formation> GetFormationsByNumberOfPlayers(int playerCount)
        {
            List<Formation> returnValue = new List<Formation>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<Formation>("Select * from formation f where f.formationCount= " + playerCount);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        #endregion "Formatations"

        #region "StatDescritption"

        public List<StatDescription> GetStatDescriptions()
        {
            List<StatDescription> returnValue = new List<StatDescription>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<StatDescription>("Select * from statDescription where visible = 'Y' order by statDescriptionName");
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public void UpsertStatDescription(StatDescription statDescription)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            if (statDescription.StatDescriptionID == 0)
                            {
                                db.Insert(statDescription);
                            }
                            else
                            {
                                db.Update(statDescription);
                            }
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public StatDescription GetStatDescriptionByName(string statDescriptionName)
        {
            StatDescription returnValue = new StatDescription();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<StatDescription>("Select * from statDescription s where s.StatDescriptionName = '" + statDescriptionName + "'").FirstOrDefault();
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public StatDescription GetStatDescriptionByID(int? statDescriptionID)
        {
            StatDescription returnValue = new StatDescription();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<StatDescription>("Select * from statDescription s where s.StatDescriptionID = " + statDescriptionID).FirstOrDefault();
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        #endregion "StatDescription"

        #region "StatCategory"

        public StatCategory GetStatCategoryByName(string statCategoryName)
        {
            StatCategory returnValue = new StatCategory();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<StatCategory>("Select * from statCategory s where s.statCategoryName = '" + statCategoryName + "'").FirstOrDefault();
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public StatCategory GetStatCategoryByID(int statCategoryID)
        {
            StatCategory returnValue = new StatCategory();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<StatCategory>("Select * from statCategory s where s.statCategoryID = " + statCategoryID).FirstOrDefault();
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public void UpsertStatCategory(StatCategory statCategory)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            if (statCategory.StatCategoryID == 0)
                            {
                                db.Insert(statCategory);
                            }
                            else
                            {
                                db.Update(statCategory);
                            }
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public List<StatCategory> GetAllStatCategoriesList(bool orderByName)
        {
            List<StatCategory> returnValue = new List<StatCategory>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            if (orderByName == true)
                            {
                                returnValue = db.Query<StatCategory>("Select * from StatCategory t order by StatCategoryName");
                            }
                            else
                            {
                                returnValue = db.Query<StatCategory>("Select * from StatCategory t order by sortorder");
                            }
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public void UpdateStatCategorySetShotToActiveAllOtherStatsToNotActive()
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<StatCategory>("Update statcategory set active = 'Y' where statcategoryId = 1");
                            db.Query<StatCategory>("Update statcategory set active = 'N' where statcategoryId <> 1");
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdateStatCategorySetAllStatsToActive()
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<StatCategory>("Update statcategory set active = 'Y'");
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        #endregion "StatCategory"

        #region "EventRoster"

        public Dictionary<string, EventRoster> GetEventRosterByEventIDDictionary(int gameID)
        {
            Dictionary<string, EventRoster> returnValue = new Dictionary<string, EventRoster>();

            GetEventRosterByGameID(gameID).ForEach(x => returnValue.Add(x.PlayerID.ToString() + x.TeamID.ToString(), x));

            return returnValue;
        }

        public List<EventRoster> GetEventRoster(int gameID, int teamID)
        {
            List<EventRoster> returnValue = new List<EventRoster>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<EventRoster>("Select * from EventRoster Where gameID = " + gameID + " AND teamID = " + teamID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public EventRoster GetEventRosterByGameTeamPlayer(int gameID, int teamID, int playerID)
        {
            EventRoster returnValue = new EventRoster();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<EventRoster>("Select * from EventRoster Where gameID = " + gameID + " AND teamID = " + teamID + " AND playerID = " + playerID).FirstOrDefault();
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public List<EventRoster> GetEventRosterByGameID(int gameID)
        {
            List<EventRoster> returnValue = new List<EventRoster>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<EventRoster>("Select * from EventRoster Where gameID = " + gameID + " order by teamID, playerID");
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public List<EventRoster> GetPlayersListOfGames(int playerID)
        {
            List<EventRoster> returnValue = new List<EventRoster>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            //TJY JOIN USED
                            returnValue = db.Query<EventRoster>("Select * from EventRoster e, Game g Where e.gameID = g.gameID and  e.playerID = " + playerID + " order by g.gameDate");
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public void DeleteEventRoster(int gameID, int teamID)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<EventRoster>("Delete from EventRoster Where gameID = " + gameID + " AND teamID = " + teamID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public List<EventRoster> GetListOfPlayersOnTheField(int gameID, int teamID)
        {
            List<EventRoster> returnValue = new List<EventRoster>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<EventRoster>("Select * from EventRoster Where isplayeronfield = 'Y' AND gameID = " + gameID + " AND teamID = " + teamID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public void UpdatePlayersOnFieldStatus(int gameID, int teamID, int? playerID, string isPlayerOnField)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update eventroster set isplayeronfield = '" + isPlayerOnField + "' where gameID = " + gameID + " and teamID = " + teamID + " and playerID = " + playerID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdatePlayersGMPlayerPositionID(int gameID, int teamID, int? playerID, string gmPlayerPositionID)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update eventroster set GMPlayerPositionID = '" + gmPlayerPositionID + "' where gameID = " + gameID + " and teamID = " + teamID + " and playerID = " + playerID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpdatePlayersStarterStatus(int gameID, int teamID, int? playerID, string isStarter)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<Game>("Update eventroster set starter = '" + isStarter + "' where gameID = " + gameID + " and teamID = " + teamID + " and playerID = " + playerID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void UpsertEventRoster(ObservableCollection<EventRoster> eventRosterList)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            DeleteEventRoster(eventRosterList[0].GameID, eventRosterList[0].TeamID);
                            db.InsertAll(eventRosterList);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }
     
        public void InsertEventRoster(EventRoster eventRosterEntry)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Insert(eventRosterEntry);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }
       
        #endregion "EventRoster"

        #region "FlatTotals"

        public FlatTotals GetTeamsFlatStatsForGame(int gameID, int teamID)
        {
            FlatTotals returnValue = new FlatTotals();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<FlatTotals>("select teamID, sum(ShotTotal) ShotTotal, sum(SaveTotal) SaveTotal, sum(OffsidesTotal) OffsidesTotal, sum(CornerKickTotal) CornerKickTotal, sum(FoulCommittedTotal) FoulCommittedTotal, sum(ShotOnGoalTotal) ShotOnGoalTotal, sum(YellowCardTotal) YellowCardTotal, sum(RedCardTotal) RedCardTotal from flattotals f where f.gameID = " + gameID + " and f.teamID = " + teamID + " group by teamID").FirstOrDefault();
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public FlatTotals GetPlayersFlatTotalsRecord(int gameID, int teamID, int playerID)
        {
            FlatTotals returnValue = new FlatTotals();
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<FlatTotals>("select * from flattotals f where f.gameID = " + gameID + " and f.teamID = " + teamID + " and f.playerID = " + playerID).FirstOrDefault();
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public List<FlatTotals> GetGamesPlayerFlats(int gameID, int teamID)
        {
            List<FlatTotals> returnValue = new List<FlatTotals>();

            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            returnValue = db.Query<FlatTotals>("select * from flattotals f where f.gameID = " + gameID + " and f.teamID = " + teamID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
            return returnValue;
        }

        public void UpdatePlayersFlatTotalsRecord(int gameID, int playerID, int teamID, string statColumnToUpdate, int statValue)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                         {
                             db.Query<FlatTotals>("Update flattotals set " + statColumnToUpdate + " = " + statValue + " where gameID = " + gameID + " and teamID = " + teamID + " and playerID = " + playerID);
                             success = true;
                         });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        //This did not work when running in a task, NOT using
        public void UpdatePlayersFlatTotalsRecordForSecondsAndMinutes(int gameID, int teamID, int playerID, int seconds, int minutes)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<FlatTotals>("Update flattotals set secondsplayedtotal = " + seconds + ", minutesplayedtotal = " + minutes + " where gameID = " + gameID + " and teamID = " + teamID + " and playerID = " + playerID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }


        public void InsertFlatTotals(FlatTotals flatTotals)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Insert(flatTotals);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void DeleteEntireGamesStats(int gameID)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<FlatTotals>("Delete from FlatTotals where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        public void DeleteEntireGamesPlusMinusStats(int gameID)
        {
            bool success = false;
            int tries = 0;

            while ((success == false) && (tries < App.gDBRetryLimit))
            {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<FlatTotals>("Update FlatTotals set plusMinusTotal = 0 where gameID = " + gameID);
                            success = true;
                        });
                    }
                }
                catch (Exception)
                {
                    tries = tries + 1;
                    Thread.Sleep(App.gDBRetrySleepTime);
                }
            }
        }

        #endregion "FlatTotals"

        #region "Alter Tables Add Oncloud column"

        //Because we want this to be an all or nothing not using the retry here
        public void AddOnCloudColumnToDatabaseTables()
        {
         {
                try
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.RunInTransaction(() =>
                        {
                            db.Query<EventRoster>("ALTER TABLE EventRoster ADD COLUMN OnCloud string");
                            db.Query<FlatTotals>("ALTER TABLE FlatTotals ADD COLUMN OnCloud string");
                            db.Query<Formation>("ALTER TABLE Formation ADD COLUMN OnCloud string");
                            db.Query<Game>("ALTER TABLE Game ADD COLUMN OnCloud string");
                            db.Query<Jersey>("ALTER TABLE Jersey ADD COLUMN OnCloud string");
                            db.Query<League>("ALTER TABLE League ADD COLUMN OnCloud string");
                            db.Query<PeriodByPeriodScore>("ALTER TABLE PeriodByPeriodScore ADD COLUMN OnCloud string");
                            db.Query<Play>("ALTER TABLE Play ADD COLUMN OnCloud string");
                            db.Query<Player>("ALTER TABLE Player ADD COLUMN OnCloud string");
                            db.Query<StatCategory>("ALTER TABLE StatCategory ADD COLUMN OnCloud string");
                            db.Query<StatDescription>("ALTER TABLE StatDescription ADD COLUMN OnCloud string");
                            db.Query<Team>("ALTER TABLE Team ADD COLUMN OnCloud string");
                            db.Query<TeamRoster>("ALTER TABLE TeamRoster ADD COLUMN OnCloud string");                           
                        });
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }


        #endregion "Alter Tables Add Oncloud column"
    }
}
