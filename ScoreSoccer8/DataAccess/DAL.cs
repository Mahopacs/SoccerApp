using ScoreSoccer8.Cloud;
using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.Resources;
using ScoreSoccer8.Utilities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace ScoreSoccer8.DataAccess
{
    public class DAL
    {

        public static DAL Instance()
        {
            return new DAL();
        }

        #region "Misc"

        //We used to use if (!FileExists(App.gDBName).Result == false) to determine if the database was created, we know check the setting in isolated storage
        public void InitializeApplication()
        {
            string isDatabaseCreated = "NO";

            try
            {
                Rate.UpdateOpenedCount();
                App.gAppOpenedCount = Rate.GetAppOpenedCount();
                App.gHasAppBeenRated = Rate.HasAppBeenRated();

                isDatabaseCreated = IS.GetSettingStringValue("DatabaseCreated").ToUpper();

                if (isDatabaseCreated.ToUpper() != "YES")
                {
                    using (var db = new SQLiteConnection(App.gDBPath))
                    {
                        db.CreateTable<Team>();
                        db.CreateTable<League>();
                        db.CreateTable<Player>();
                        db.CreateTable<TeamRoster>();
                        db.CreateTable<Game>();
                        db.CreateTable<PeriodByPeriodScore>();
                        db.CreateTable<Jersey>();
                        db.CreateTable<StatDescription>();
                        db.CreateTable<StatCategory>();
                        db.CreateTable<EventRoster>();
                        db.CreateTable<Formation>();
                        db.CreateTable<Play>();
                        db.CreateTable<FlatTotals>();
                        PopulateJerseyTable();
                        PopulateStatDescriptionTable();
                        PopulateStatCategoryTable();

                        BaseTableDataAccess.Instance().UpsertLeague(new League { LeagueName = AppResources.NoLeague, LeagueContactName = "", LeagueContactNumber = "", Visible = "N" });

                        if (App.gIsDeveloperDebugMode == true)
                        {
                            DAL.Instance().CreateSampleDataForDevelopers(false);
                            
                        }

                        DAL.Instance().CreateSampleDataForUsers();
                        DAL.Instance().CreateSampleDataForTed();
            
                        IS.SaveSetting("DatabaseCreated", "YES");
                    }
                }
                else   //Database already exists (line below is needed for users that got the app prior to adding 'OnCloud' column
                {
                    BaseTableDataAccess.Instance().AddOnCloudColumnToDatabaseTables();
                }
            }
            catch (Exception ex)
            {
                ErrorLogConnection cloud = new ErrorLogConnection();
                cloud.UpdateErrorLog("DAL.InitializeApplication", ex.Message.ToString());
            }

            //This is always done regardless of if this is the first or 100th time the user is opening the application
            SetUpStatsForApp();
        }

        public void SetUpStatsForApp()
        {
            try
            {
                //We only load the Stat Object once, if we ever get to the point we allow the user to hide or show certain stats in the StatsPicker
                //then we need to call this again to rebuild the global stat list
                if (App.DoesUserHaveAbilityToTrackAllStats() == true)
                {
                    BaseTableDataAccess.Instance().UpdateStatCategorySetAllStatsToActive();
                }
                else //This is the free version so only the shot category is active
                {
                    BaseTableDataAccess.Instance().UpdateStatCategorySetShotToActiveAllOtherStatsToNotActive();
                }

                DAL.Instance().PopulateStatGlobalClass(false);
            }
            catch (Exception ex)
            {
                ErrorLogConnection cloud = new ErrorLogConnection();
                cloud.UpdateErrorLog("DAL.SetupStatsForApp", ex.Message.ToString());
            }
        }

        public void PopulateStatGlobalClass(bool orderByName)
        {
            try
            {
                StatDescriptionModel d1 = GetStatDescriptionByName("Miss");     //1
                StatDescriptionModel d2 = GetStatDescriptionByName("Hit Post"); //2
                StatDescriptionModel d3 = GetStatDescriptionByName("Blocked");      //3
                StatDescriptionModel d4 = GetStatDescriptionByName("Goal");     //4
                StatDescriptionModel d7 = GetStatDescriptionByName("Excellent");    //5
                StatDescriptionModel d8 = GetStatDescriptionByName("Good"); //6
                StatDescriptionModel d9 = GetStatDescriptionByName("Poor"); //7
                StatDescriptionModel d10 = GetStatDescriptionByName("Illegal Throw In");    //8
                StatDescriptionModel d11 = GetStatDescriptionByName("Bad Pass");    //9
                StatDescriptionModel d12 = GetStatDescriptionByName("Lost Dribble");    //10
                StatDescriptionModel d13 = GetStatDescriptionByName("Kicking");     //11
                StatDescriptionModel d14 = GetStatDescriptionByName("Tripping");    //12
                StatDescriptionModel d15 = GetStatDescriptionByName("Charging");    //13
                StatDescriptionModel d16 = GetStatDescriptionByName("Pushing");     //14
                StatDescriptionModel d17 = GetStatDescriptionByName("Holding");     //15
                StatDescriptionModel d18 = GetStatDescriptionByName("Illegal Tackle");  //16
                StatDescriptionModel d19 = GetStatDescriptionByName("For Goal");        //17
                StatDescriptionModel d20 = GetStatDescriptionByName("Not For Goal");    //18
                StatDescriptionModel d21 = GetStatDescriptionByName("Unsportsmanlike Conduct"); //19
                StatDescriptionModel d22 = GetStatDescriptionByName("Delaying Restart Of Play");    //20
                StatDescriptionModel d23 = GetStatDescriptionByName("Foul Play");       //21
                StatDescriptionModel d24 = GetStatDescriptionByName("Violent Conduct"); //22
                StatDescriptionModel d25 = GetStatDescriptionByName("Illegal Hands");   //23
                StatDescriptionModel d26 = GetStatDescriptionByName("Second Yellow Card");  //24
                StatDescriptionModel d27 = GetStatDescriptionByName("Shot Details");    //25

                //Descriptions for Shot
                ObservableCollection<StatDescriptionModel> shotDescriptions = new ObservableCollection<StatDescriptionModel>();
                shotDescriptions.Add(GetStatDescriptionByName("Shot Details"));

                ObservableCollection<StatDescriptionModel> penaltyKickDescriptions = new ObservableCollection<StatDescriptionModel>();
                penaltyKickDescriptions.Add(GetStatDescriptionByName("Shot Details"));

                ObservableCollection<StatDescriptionModel> shootoutKickDescriptions = new ObservableCollection<StatDescriptionModel>();
                shootoutKickDescriptions.Add(GetStatDescriptionByName("Shot Details"));

                //Descriptions for Excellent, Good, Poor
                ObservableCollection<StatDescriptionModel> excellentGoodPoorDescriptions = new ObservableCollection<StatDescriptionModel>();
                excellentGoodPoorDescriptions.Add(GetStatDescriptionByName("Excellent"));
                excellentGoodPoorDescriptions.Add(GetStatDescriptionByName("Good"));
                excellentGoodPoorDescriptions.Add(GetStatDescriptionByName("Poor"));

                ObservableCollection<StatDescriptionModel> crossExcellentGoodPoorDescriptions = new ObservableCollection<StatDescriptionModel>();
                crossExcellentGoodPoorDescriptions.Add(GetStatDescriptionByName("Excellent"));
                crossExcellentGoodPoorDescriptions.Add(GetStatDescriptionByName("Good"));
                crossExcellentGoodPoorDescriptions.Add(GetStatDescriptionByName("Poor"));

                ObservableCollection<StatDescriptionModel> dropKickExcellentGoodPoorDescriptions = new ObservableCollection<StatDescriptionModel>();
                dropKickExcellentGoodPoorDescriptions.Add(GetStatDescriptionByName("Excellent"));
                dropKickExcellentGoodPoorDescriptions.Add(GetStatDescriptionByName("Good"));
                dropKickExcellentGoodPoorDescriptions.Add(GetStatDescriptionByName("Poor"));

                ObservableCollection<StatDescriptionModel> cornerKickDescriptions = new ObservableCollection<StatDescriptionModel>();
                cornerKickDescriptions.Add(GetStatDescriptionByName("Excellent"));
                cornerKickDescriptions.Add(GetStatDescriptionByName("Good"));
                cornerKickDescriptions.Add(GetStatDescriptionByName("Poor"));
                cornerKickDescriptions.Add(GetStatDescriptionByName("For Goal"));

                //Descriptions for Turnover
                ObservableCollection<StatDescriptionModel> turnoverDescriptions = new ObservableCollection<StatDescriptionModel>();
                turnoverDescriptions.Add(d10);
                turnoverDescriptions.Add(d12);

                //Descriptions for Foul Commited
                ObservableCollection<StatDescriptionModel> foulCommitedDescriptions = new ObservableCollection<StatDescriptionModel>();
                foulCommitedDescriptions.Add(d13);
                foulCommitedDescriptions.Add(d14);
                foulCommitedDescriptions.Add(d15);
                foulCommitedDescriptions.Add(d16);
                foulCommitedDescriptions.Add(d17);
                foulCommitedDescriptions.Add(d18);

                //Descriptions for Direct Free Kick 
                ObservableCollection<StatDescriptionModel> directFreeKickDescriptions = new ObservableCollection<StatDescriptionModel>();
                directFreeKickDescriptions.Add(d19);
                directFreeKickDescriptions.Add(d20);

                //Descriptions for Yellow Card
                ObservableCollection<StatDescriptionModel> yellowCardDescriptions = new ObservableCollection<StatDescriptionModel>();
                yellowCardDescriptions.Add(d21);
                yellowCardDescriptions.Add(d22);

                //Descriptions for Red Card
                ObservableCollection<StatDescriptionModel> redCardDescriptions = new ObservableCollection<StatDescriptionModel>();
                redCardDescriptions.Add(d23);
                redCardDescriptions.Add(d24);
                redCardDescriptions.Add(d25);
                redCardDescriptions.Add(d26);

                App.gStatsList.Clear();

                List<StatCategory> listStatCategories = new List<StatCategory>();
                listStatCategories = BaseTableDataAccess.Instance().GetAllStatCategoriesList(orderByName);
                foreach (var item in listStatCategories)
                {
                    StatCategory statCat = new StatCategory();
                    StatCategoryModel statCatModel = new StatCategoryModel();

                    statCatModel.StatCategory.StatCategoryID = item.StatCategoryID;
                    statCatModel.StatCategory.StatCategoryName = item.StatCategoryName;
                    statCatModel.StatCategory.SortOrder = item.SortOrder;
                    statCatModel.StatCategory.Visible = item.Visible;
                    statCatModel.StatCategory.Active = item.Active;

                    switch (item.StatCategoryName)
                    {
                        case "Shot":
                            statCatModel.Descriptions = shotDescriptions;
                            break;
                        case "Pass":
                            statCatModel.Descriptions = excellentGoodPoorDescriptions;
                            break;
                        case "Turnover":
                            statCatModel.Descriptions = turnoverDescriptions;
                            break;
                        case "Offsides":
                            statCatModel.Descriptions = null;
                            break;
                        case "Foul Committed":
                            statCatModel.Descriptions = foulCommitedDescriptions;
                            break;
                        case "Out Of Bounds":
                            statCatModel.Descriptions = null;
                            break;
                        case "Cross":
                            statCatModel.Descriptions = crossExcellentGoodPoorDescriptions;
                            break;
                        case "Throw In":
                            statCatModel.Descriptions = null;
                            break;
                        case "Corner Kick":
                            statCatModel.Descriptions = cornerKickDescriptions;
                            break;
                        case "Tackle":
                            statCatModel.Descriptions = null;
                            break;
                        case "Goalie Kick":
                            statCatModel.Descriptions = null;
                            break;
                        case "Own Goal":
                            statCatModel.Descriptions = null;
                            break;
                        case "Foul Drawn":
                            statCatModel.Descriptions = null;
                            break;
                        case "Direct Free Kick":
                            statCatModel.Descriptions = directFreeKickDescriptions;
                            break;
                        case "Indirect Free Kick":
                            statCatModel.Descriptions = null; ;
                            break;
                        case "Penalty Kick":
                            statCatModel.Descriptions = penaltyKickDescriptions;
                            break;
                        case "Yellow Card":
                            statCatModel.Descriptions = yellowCardDescriptions;
                            break;
                        case "Red Card":
                            statCatModel.Descriptions = redCardDescriptions;
                            break;
                        case "Drop Kick":
                            statCatModel.Descriptions = dropKickExcellentGoodPoorDescriptions;
                            break;
                        case "Dribble":
                            statCatModel.Descriptions = null;
                            break;
                        case "Shootout Kick":
                            statCatModel.Descriptions = shootoutKickDescriptions;
                            break;
                        case "Substitution":
                            statCatModel.Descriptions = null;
                            break;
                        default:
                            statCatModel.Descriptions = null;
                            break;
                    }
                    App.gStatsList.Add(statCatModel);
                }
            }
            catch (Exception ex)
            {
                ErrorLogConnection cloud = new ErrorLogConnection();
                cloud.UpdateErrorLog("DAL.PopulateStatGlobalClass", ex.Message.ToString());
            }
        }

        public async Task<bool> FileExists(string fileName)
        {
            var result = false;
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                result = true;
            }
            catch
            {
            }

            return result;
        }

        //Used by Ted for Farmington Fall 2014 roster
        public void CreateSampleDataForTed()
        {
            try
            {
                //Create Leagues
                BaseTableDataAccess.Instance().UpsertLeague(new League { LeagueName = "Farmington Valley", LeagueContactName = "Bob Smith", LeagueContactNumber = "860-123-4567", Visible = "Y" });

                //Create Teams
                BaseTableDataAccess.Instance().UpsertTeam(new Team { TeamName = "Farmington", TeamShortName = "FU", Coach = "Chris Caccamo", Color = "White", ContactNumber = "860-123-1234", Visible = "Y", JerseyID = 3 });
                BaseTableDataAccess.Instance().UpsertTeam(new Team { TeamName = "Avon", TeamShortName = "AV", Coach = "Joe Smith", Color = "Blue", ContactNumber = "860-456-4312", Visible = "Y", JerseyID = 4 });

                //Create the players (the physical list of players)
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Jared", LastName = "Young", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //2
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Mathew", LastName = "Cence", Height = "4'1", Weight = 82, Kicks = "Left", Visible = "Y" }, 0);  //3
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Caden", LastName = "Gallagher", Height = "4'10", Weight = 90, Kicks = "Right", Visible = "Y" }, 0);  //4
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Cole", LastName = "Caccamo", Height = "5'2", Weight = 70, Kicks = "Right", Visible = "Y" }, 0);  //5
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Cam", LastName = "Barrone", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //7
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Jacob", LastName = "Perry", Height = "5'0", Weight = 74, Kicks = "Right", Visible = "Y" }, 0);  //8
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Ben", LastName = "Cosentino", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //9
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Kaden", LastName = "Macaroon", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0); //10
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Mark", LastName = "Cherynak", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0); //11
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Brenden", LastName = "Occhino", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0); //12
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Isaac", LastName = "Coello", Height = "5'0", Weight = 80, Kicks = "Left", Visible = "Y" }, 0);  //13
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Dominic", LastName = "Gallo", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //14

                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Ben", LastName = "Villa", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //15
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "EJ", LastName = "Sanchez", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //15
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Eoin", LastName = "Chekas", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //15
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Graham", LastName = "Peterson", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //15
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Ricky", LastName = "Duan", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //15
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Ryan", LastName = "Kushnir", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //15

                //Create a roster for Farmington
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 31, Active = "Y", UniformNumber = "20", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 32, Active = "Y", UniformNumber = "7", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 33, Active = "Y", UniformNumber = "19", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 34, Active = "Y", UniformNumber = "11", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 35, Active = "Y", UniformNumber = "00", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 36, Active = "Y", UniformNumber = "3", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 37, Active = "Y", UniformNumber = "8", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 38, Active = "Y", UniformNumber = "13", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 39, Active = "Y", UniformNumber = "17", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 40, Active = "Y", UniformNumber = "14", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 41, Active = "Y", UniformNumber = "6", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 42, Active = "Y", UniformNumber = "9", Visible = "Y" });

                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 43, Active = "N", UniformNumber = "0", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 44, Active = "N", UniformNumber = "0", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 45, Active = "N", UniformNumber = "4", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 46, Active = "N", UniformNumber = "2", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 47, Active = "N", UniformNumber = "23", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 48, Active = "N", UniformNumber = "16", Visible = "Y" });


                //Create Games
                UpsertGame(new Game { AwayTeamID = 4, HomeTeamID = 3, PlayersPerTeam = 8, Periods = 2, PeriodLength = 40, HasOverTime = false, OverTimeLength = 0, GameDate = DateTime.Now.AddDays(+5), GameTime = DateTime.Now, HomeFormationID = 1, AwayFormationID = 1, ClockUpOrDown = "DOWN", Visible = "Y" });
            }
            catch (Exception)
            {
            }
        }

        public void CreateSampleDataForDevelopers(bool samplePlays)
        {
            try
            {
                //Create Leagues
                BaseTableDataAccess.Instance().UpsertLeague(new League { LeagueName = "Farmington Vall Socc", LeagueContactName = "Bob Smith", LeagueContactNumber = "860-123-4567", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertLeague(new League { LeagueName = "FSA Premier", LeagueContactName = "John Jones", LeagueContactNumber = "203-892-4381", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertLeague(new League { LeagueName = "This Max League Name", LeagueContactName = "Max Contact Name 123", LeagueContactNumber = "203-892-4381", Visible = "Y" });

                //Create Teams
                BaseTableDataAccess.Instance().UpsertTeam(new Team { TeamName = "Farmington", TeamShortName = "FU", Coach = "Chris Caccamo", Color = "White", ContactNumber = "860-123-1234", Visible = "Y", JerseyID = 3 });
                BaseTableDataAccess.Instance().UpsertTeam(new Team { TeamName = "Avon", TeamShortName = "AV", Coach = "Joe Smith", Color = "Blue", ContactNumber = "860-456-4312", Visible = "Y", JerseyID = 4 });
                BaseTableDataAccess.Instance().UpsertTeam(new Team { TeamName = "West Hartford", TeamShortName = "WH", Coach = "Steve Stokoe", Color = "Red", ContactNumber = "203-789-9806", Visible = "Y", JerseyID = 5 });
                BaseTableDataAccess.Instance().UpsertTeam(new Team { TeamName = "Bristol", TeamShortName = "BR", Coach = "John Williams", Color = "Red", ContactNumber = "456-423-7182", Visible = "Y", JerseyID = 6 });
                BaseTableDataAccess.Instance().UpsertTeam(new Team { TeamName = "Team PM Clock Down", TeamShortName = "PMD", Coach = "John Played", Color = "Red", ContactNumber = "456-423-7182", Visible = "Y", JerseyID = 8 });
                BaseTableDataAccess.Instance().UpsertTeam(new Team { TeamName = "Team PM Clock Up", TeamShortName = "PMU", Coach = "John Played", Color = "Red", ContactNumber = "456-423-7182", Visible = "Y", JerseyID = 10 });
                BaseTableDataAccess.Instance().UpsertTeam(new Team { TeamName = "This Max Team Name12", TeamShortName = "Max1", Coach = "This Max Coach Name1", Color = "Red", ContactNumber = "456-423-7182", Visible = "Y", JerseyID = 11 });

                //Create the players (the physical list of players)
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Lebron", LastName = "James", Height = "7'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0); //1
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Jared", LastName = "Young", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //2
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Mathew", LastName = "Cence", Height = "4'1", Weight = 82, Kicks = "Left", Visible = "Y" }, 0);  //3
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Caden", LastName = "Gallagher", Height = "4'10", Weight = 90, Kicks = "Right", Visible = "Y" }, 0);  //4
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Cole", LastName = "Caccamo", Height = "5'2", Weight = 70, Kicks = "Right", Visible = "Y" }, 0);  //5
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Sevie", LastName = "Jones", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //6
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Cam", LastName = "Barrone", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //7
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Joseph", LastName = "Abreu", Height = "5'0", Weight = 74, Kicks = "Right", Visible = "Y" }, 0);  //8
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Ben", LastName = "Cosentino", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //9
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Caden", LastName = "Macaroon", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0); //10
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Mark", LastName = "Smith", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0); //11
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Brenden", LastName = "Ochino", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0); //12
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Graham", LastName = "Peterson", Height = "5'0", Weight = 80, Kicks = "Left", Visible = "Y" }, 0);  //13
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Dominic", LastName = "Gallo", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //14
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Steve", LastName = "Jones", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //15
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Bob", LastName = "Evans", Height = "4'1", Weight = 82, Kicks = "Left", Visible = "Y" }, 0);  //16
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "John", LastName = "Heine", Height = "4'10", Weight = 90, Kicks = "Right", Visible = "Y" }, 0);  //17
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Russ", LastName = "Vetrano", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0); //18
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Frank", LastName = "Schmidt", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0); //19
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Dave", LastName = "Santoro", Height = "5'0", Weight = 74, Kicks = "Right", Visible = "Y" }, 0);  //20
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Gregg", LastName = "Lafferty", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0); //21
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Brian", LastName = "Allen", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //22
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Pete", LastName = "Izzo", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0); //23
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Lee", LastName = "Johnson", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0); //24
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Dave", LastName = "Lusk", Height = "5'0", Weight = 80, Kicks = "Left", Visible = "Y" }, 0);  //25
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Rich", LastName = "Gravelin", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //26
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Ted", LastName = "Ted", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //27
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Bill", LastName = "Bill", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0); //28
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "John", LastName = "John", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //29
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Steve", LastName = "Steve", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //30
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Bob", LastName = "Bob", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //31
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Lou", LastName = "Lou", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //32
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Chris", LastName = "Chris", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0);  //33
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = "Max Name1234", LastName = "Max Name1234567", Height = "5'0", Weight = 80, Kicks = "Right", Visible = "Y" }, 0); //34

                //Create a roster for Farmington
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 1, PlayerID = 1, Active = "Y", UniformNumber = "2", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 1, PlayerID = 2, Active = "Y", UniformNumber = "10", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 1, PlayerID = 3, Active = "Y", UniformNumber = "20", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 1, PlayerID = 4, Active = "Y", UniformNumber = "11", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 1, PlayerID = 5, Active = "Y", UniformNumber = "8", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 1, PlayerID = 6, Active = "Y", UniformNumber = "13", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 1, PlayerID = 7, Active = "Y", UniformNumber = "18", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 1, PlayerID = 8, Active = "Y", UniformNumber = "5", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 1, PlayerID = 9, Active = "Y", UniformNumber = "3", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 1, PlayerID = 10, Active = "Y", UniformNumber = "12", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 1, PlayerID = 11, Active = "Y", UniformNumber = "4", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 1, PlayerID = 12, Active = "Y", UniformNumber = "7", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 1, PlayerID = 13, Active = "Y", UniformNumber = "19", Visible = "Y" });

                //Create a roster for Avon
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 2, PlayerID = 21, Active = "Y", UniformNumber = "2" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 2, PlayerID = 22, Active = "Y", UniformNumber = "10" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 2, PlayerID = 23, Active = "Y", UniformNumber = "20" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 2, PlayerID = 24, Active = "Y", UniformNumber = "11" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 2, PlayerID = 25, Active = "N", UniformNumber = "8" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 2, PlayerID = 16, Active = "Y", UniformNumber = "0" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 2, PlayerID = 17, Active = "Y", UniformNumber = "18" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 2, PlayerID = 8, Active = "Y", UniformNumber = "5" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 2, PlayerID = 1, Active = "Y", UniformNumber = "0" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 2, PlayerID = 2, Active = "N", UniformNumber = "12" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 2, PlayerID = 5, Active = "Y", UniformNumber = "4" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 2, PlayerID = 14, Active = "Y", UniformNumber = "14" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 2, PlayerID = 15, Active = "Y", UniformNumber = "7" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 2, PlayerID = 10, Active = "Y", UniformNumber = "34" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 2, PlayerID = 11, Active = "Y", UniformNumber = "44" });

                //Create a roster for West Hartford
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 1, Active = "Y", UniformNumber = "9" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 11, Active = "Y", UniformNumber = "15" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 5, Active = "Y", UniformNumber = "1" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 8, Active = "Y", UniformNumber = "2" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 12, Active = "Y", UniformNumber = "3" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 13, Active = "Y", UniformNumber = "5" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 17, Active = "Y", UniformNumber = "8" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 24, Active = "Y", UniformNumber = "11" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 25, Active = "Y", UniformNumber = "14" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 26, Active = "Y", UniformNumber = "20" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 9, Active = "Y", UniformNumber = "3" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 4, Active = "Y", UniformNumber = "15" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 3, PlayerID = 10, Active = "Y", UniformNumber = "4" });

                //Create a roster for Bristol
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 4, PlayerID = 4, Active = "Y", UniformNumber = "1" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 4, PlayerID = 8, Active = "Y", UniformNumber = "3" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 4, PlayerID = 12, Active = "Y", UniformNumber = "5" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 4, PlayerID = 16, Active = "Y", UniformNumber = "7" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 4, PlayerID = 20, Active = "Y", UniformNumber = "9" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 4, PlayerID = 24, Active = "Y", UniformNumber = "11" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 4, PlayerID = 5, Active = "Y", UniformNumber = "13" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 4, PlayerID = 9, Active = "Y", UniformNumber = "15" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 4, PlayerID = 13, Active = "Y", UniformNumber = "17" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 4, PlayerID = 17, Active = "Y", UniformNumber = "19" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 4, PlayerID = 21, Active = "Y", UniformNumber = "21" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 4, PlayerID = 25, Active = "Y", UniformNumber = "23" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 4, PlayerID = 18, Active = "Y", UniformNumber = "25" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 4, PlayerID = 34, Active = "Y", UniformNumber = "77" });

                //Create a roster for Team PM Clock Down
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 5, PlayerID = 27, Active = "Y", UniformNumber = "1" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 5, PlayerID = 28, Active = "Y", UniformNumber = "3" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 5, PlayerID = 29, Active = "Y", UniformNumber = "5" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 5, PlayerID = 30, Active = "Y", UniformNumber = "7" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 5, PlayerID = 31, Active = "N", UniformNumber = "9" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 5, PlayerID = 32, Active = "N", UniformNumber = "11" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 5, PlayerID = 33, Active = "N", UniformNumber = "13" });

                //Create a roster for Team PM Clock Uo
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 6, PlayerID = 27, Active = "Y", UniformNumber = "1" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 6, PlayerID = 28, Active = "Y", UniformNumber = "3" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 6, PlayerID = 29, Active = "Y", UniformNumber = "5" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 6, PlayerID = 30, Active = "Y", UniformNumber = "7" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 6, PlayerID = 31, Active = "N", UniformNumber = "9" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 6, PlayerID = 32, Active = "N", UniformNumber = "11" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = 6, PlayerID = 33, Active = "N", UniformNumber = "13" });

                //Create Games
                UpsertGame(new Game { AwayTeamID = 1, HomeTeamID = 2, PlayersPerTeam = 8, Periods = 2, PeriodLength = 30, HasOverTime = false, OverTimeLength = 0, GameDate = DateTime.Now.AddDays(-1), GameTime = DateTime.Now, HomeFormationID = 1, AwayFormationID = 1, ClockUpOrDown = "DOWN", Visible = "Y" });
                UpsertGame(new Game { AwayTeamID = 1, HomeTeamID = 2, PlayersPerTeam = 8, Periods = 2, PeriodLength = 45, HasOverTime = true, OverTimeLength = 0, GameDate = DateTime.Now.AddDays(-55), GameTime = DateTime.Now, HomeFormationID = 1, AwayFormationID = 1, ClockUpOrDown = "DOWN", Visible = "Y" });
                UpsertGame(new Game { AwayTeamID = 1, HomeTeamID = 2, PlayersPerTeam = 8, Periods = 2, PeriodLength = 30, HasOverTime = true, OverTimeLength = 0, GameDate = DateTime.Now.AddDays(-110), GameTime = DateTime.Now, HomeFormationID = 1, AwayFormationID = 1, ClockUpOrDown = "DOWN", Visible = "Y" });
                UpsertGame(new Game { AwayTeamID = 1, HomeTeamID = 2, PlayersPerTeam = 8, Periods = 2, PeriodLength = 45, HasOverTime = true, OverTimeLength = 15, GameDate = DateTime.Now.AddDays(-45), GameTime = DateTime.Now, HomeFormationID = 1, AwayFormationID = 1, ClockUpOrDown = "UP", Visible = "Y" });
                UpsertGame(new Game { AwayTeamID = 2, HomeTeamID = 5, PlayersPerTeam = 4, Periods = 2, PeriodLength = 10, HasOverTime = true, OverTimeLength = 5, GameDate = DateTime.Now.AddDays(-32), GameTime = DateTime.Now, HomeFormationID = 1, AwayFormationID = 1, ClockUpOrDown = "DOWN", Visible = "Y" });
                UpsertGame(new Game { AwayTeamID = 2, HomeTeamID = 6, PlayersPerTeam = 4, Periods = 2, PeriodLength = 10, HasOverTime = true, OverTimeLength = 5, GameDate = DateTime.Now.AddDays(-12), GameTime = DateTime.Now, HomeFormationID = 1, AwayFormationID = 1, ClockUpOrDown = "UP", Visible = "Y" });
                UpsertGame(new Game { AwayTeamID = 1, HomeTeamID = 4, PlayersPerTeam = 8, Periods = 2, PeriodLength = 30, HasOverTime = true, OverTimeLength = 15, GameDate = DateTime.Now.AddDays(-120), GameTime = DateTime.Now, HomeFormationID = 1, AwayFormationID = 1, ClockUpOrDown = "DOWN", Visible = "Y" });

                if (samplePlays == true)
                {
                    CreateSamplePlaysForClockDownGame();    //This game has lots of plays, purpose is to make sure stats total correctly.
                    //CreateSamplePlaysForClockDownGameLOTSOFSUBS();
                    //CreateSamplePlaysForClockUpGame();
                    //CreateSamplePlaysForGameWithPlaysWithSameTimeEditTestGameID7();
                    CreateSampleSubsClockDown();
                    //  CreateSampleSubsClockUp();
                }
            }
            catch (Exception)
            {
            }
        }

        public void CreateSampleDataForUsers()
        {
            try
            {
                int team1ID;
                int team2ID;

                string team1Name = AppResources.OurTeam + " (" + AppResources.Demo + ")";
                string team1ShortName = team1Name.Substring(0, 3);
                string team2Name = AppResources.OpponentTeam + " (" + AppResources.Demo + ")";
                string team2ShortName = team2Name.Substring(0, 3);

                string player1Name = AppResources.Player1;
                string player2Name = AppResources.Player2;
                string player3Name = AppResources.Player3;
                string player4Name = AppResources.Player4;
                string player5Name = AppResources.Player5;
                string player6Name = AppResources.Player6;
                string player7Name = AppResources.Player7;
                string player8Name = AppResources.Player8;
                string player9Name = AppResources.Player9;
                string player10Name = AppResources.Player10;
                string player11Name = AppResources.Player11;
                string player12Name = AppResources.Player12;
                string player13Name = AppResources.Player13;
                string player14Name = AppResources.Player14;
                string player15Name = AppResources.Player15;
                string player16Name = AppResources.Player16;
                string player17Name = AppResources.Player17;
                string player18Name = AppResources.Player18;
                string player19Name = AppResources.Player19;
                string player20Name = AppResources.Player20;
                string player21Name = AppResources.Player21;
                string player22Name = AppResources.Player22;
                string player23Name = AppResources.Player23;
                string player24Name = AppResources.Player24;
                string player25Name = AppResources.Player25;
                string player26Name = AppResources.Player26;
                string player27Name = AppResources.Player27;
                string player28Name = AppResources.Player28;
                string player29Name = AppResources.Player29;
                string player30Name = AppResources.Player30;

                //Create Teams
                BaseTableDataAccess.Instance().UpsertTeam(new Team { TeamName = team1Name, TeamShortName = team1Name.Substring(0, 3), Coach = AppResources.Coach, Color = "White", ContactNumber = "123-456-7890", Visible = "Y", JerseyID = 1, SampleData = "Y" });
                BaseTableDataAccess.Instance().UpsertTeam(new Team { TeamName = team2Name, TeamShortName = team2Name.Substring(0, 3), Coach = AppResources.Coach, Color = "Blue", ContactNumber = "123-456-7890", Visible = "Y", JerseyID = 2, SampleData = "Y" });

                //Create the players (the physical list of players)
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player1, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player2, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player3, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player4, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player5, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player6, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player7, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player8, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player9, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player10, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player11, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player12, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player13, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player14, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player15, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player16, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player17, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player18, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player19, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player20, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player21, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player22, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player23, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player24, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player25, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player26, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player27, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player28, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player29, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);
                BaseTableDataAccess.Instance().UpsertPlayer(new Player { FirstName = AppResources.Player30, LastName = "", Height = "6' 0'", Weight = 180, Kicks = "Right", Visible = "Y", SampleData = "Y" }, 0);

                //Create a roster for Our Team 
                team1ID = BaseTableDataAccess.Instance().GetTeamByTeamShortName(team1ShortName).TeamID;

                int player1ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player1Name).PlayerID;
                int player2ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player2Name).PlayerID;
                int player3ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player3Name).PlayerID;
                int player4ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player4Name).PlayerID;
                int player5ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player5Name).PlayerID;
                int player6ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player6Name).PlayerID;
                int player7ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player7Name).PlayerID;
                int player8ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player8Name).PlayerID;
                int player9ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player9Name).PlayerID;
                int player10ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player10Name).PlayerID;
                int player11ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player11Name).PlayerID;
                int player12ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player12Name).PlayerID;
                int player13ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player13Name).PlayerID;
                int player14ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player14Name).PlayerID;
                int player15ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player15Name).PlayerID;

                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team1ID, PlayerID = player1ID, Active = "Y", UniformNumber = "1", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team1ID, PlayerID = player2ID, Active = "Y", UniformNumber = "2", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team1ID, PlayerID = player3ID, Active = "Y", UniformNumber = "3", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team1ID, PlayerID = player4ID, Active = "Y", UniformNumber = "4", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team1ID, PlayerID = player5ID, Active = "Y", UniformNumber = "5", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team1ID, PlayerID = player6ID, Active = "Y", UniformNumber = "6", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team1ID, PlayerID = player7ID, Active = "Y", UniformNumber = "7", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team1ID, PlayerID = player8ID, Active = "Y", UniformNumber = "8", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team1ID, PlayerID = player9ID, Active = "Y", UniformNumber = "9", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team1ID, PlayerID = player10ID, Active = "Y", UniformNumber = "10", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team1ID, PlayerID = player11ID, Active = "Y", UniformNumber = "11", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team1ID, PlayerID = player12ID, Active = "Y", UniformNumber = "12", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team1ID, PlayerID = player13ID, Active = "Y", UniformNumber = "13", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team1ID, PlayerID = player14ID, Active = "Y", UniformNumber = "14", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team1ID, PlayerID = player15ID, Active = "Y", UniformNumber = "15", Visible = "Y" });

                //Create a roster for Opponent Team
                team2ID = BaseTableDataAccess.Instance().GetTeamByTeamShortName(team2ShortName).TeamID;

                int player16ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player16Name).PlayerID;
                int player17ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player17Name).PlayerID;
                int player18ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player18Name).PlayerID;
                int player19ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player19Name).PlayerID;
                int player20ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player20Name).PlayerID;
                int player21ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player21Name).PlayerID;
                int player22ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player22Name).PlayerID;
                int player23ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player23Name).PlayerID;
                int player24ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player24Name).PlayerID;
                int player25ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player25Name).PlayerID;
                int player26ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player26Name).PlayerID;
                int player27ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player27Name).PlayerID;
                int player28ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player28Name).PlayerID;
                int player29ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player29Name).PlayerID;
                int player30ID = BaseTableDataAccess.Instance().GetPlayerByPlayerFirstName(player30Name).PlayerID;

                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team2ID, PlayerID = player16ID, Active = "Y", UniformNumber = "16", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team2ID, PlayerID = player17ID, Active = "Y", UniformNumber = "17", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team2ID, PlayerID = player18ID, Active = "Y", UniformNumber = "18", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team2ID, PlayerID = player19ID, Active = "Y", UniformNumber = "19", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team2ID, PlayerID = player20ID, Active = "Y", UniformNumber = "20", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team2ID, PlayerID = player21ID, Active = "Y", UniformNumber = "21", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team2ID, PlayerID = player22ID, Active = "Y", UniformNumber = "22", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team2ID, PlayerID = player23ID, Active = "Y", UniformNumber = "23", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team2ID, PlayerID = player24ID, Active = "Y", UniformNumber = "24", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team2ID, PlayerID = player25ID, Active = "Y", UniformNumber = "25", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team2ID, PlayerID = player26ID, Active = "Y", UniformNumber = "26", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team2ID, PlayerID = player27ID, Active = "Y", UniformNumber = "27", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team2ID, PlayerID = player28ID, Active = "Y", UniformNumber = "28", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team2ID, PlayerID = player29ID, Active = "Y", UniformNumber = "29", Visible = "Y" });
                BaseTableDataAccess.Instance().UpsertTeamRoster(new TeamRoster { TeamID = team2ID, PlayerID = player30ID, Active = "Y", UniformNumber = "30", Visible = "Y" });

                //Create Game
                UpsertGame(new Game { AwayTeamID = team2ID, HomeTeamID = team1ID, PlayersPerTeam = 8, Periods = 2, PeriodLength = 30, HasOverTime = false, OverTimeLength = 0, GameDate = DateTime.Now.AddDays(-1), GameTime = DateTime.Now, HomeFormationID = 1, AwayFormationID = 1, ClockUpOrDown = "DOWN", Visible = "Y", SampleData = "Y" });

            }
            catch (Exception ex)
            {
                ErrorLogConnection cloud = new ErrorLogConnection();
                cloud.UpdateErrorLog("DAL.CreateSampleDataForUsers", ex.Message.ToString());
            }
        }


        //Sample data for Game 1 (Farmington at Avon)

        //Farmington Roster (1) -> Lebron (1), Jared (2), Mathew (3), Caden G (4), Cole (5), Sevie (6), Cam (7), Joseph (8), Ben (9), Caden M (10), Mark (11), Brenden (12), Graham (13)
        //Avon Roster (2) -> Gregg (21), Brian (22), Pete (23), Lee (24), Dave (25), Bob (16), John (17), Joseph (8), Lebron (1), Jared (2), Cole (5)

        //Stat Category -> Shot (1*), Pass (2*), Turnover (3*), Offsides (4), Foul Committed (5*), Out Of Bounds (6), Cross (7*), Throw In (8), 
        //Corner Kick (9*), Tackle (10), Goalie Kick (11), Own Goal (12), Foul Drawn (13), Direct Free Kick (14*), Indirect Free Kick (15), 
        //Penalty Kick (16*), Yellow Card (17*), Red Card (18*), Drop Kick (19*), Dribble (20), Shootout Kick (21*), Sub (22)

        //Miss (1), Hit Post (2), Block (3), Goal (4), Excellent (5), Good (6), Poor (7), Illegal Throw In (8), Bad Pass (9), Lost Dribble (10), Kicking (11),
        //Tripping (12), Charging (13), Pushing (14), Holding (15), Illegal Tackle (16), For Goal (17), Not For Goal (18), Unsportsmanlike Conduct (19),
        //Delaying Restart of Play (20), Foul Play (21), Violent Conduct (22), Illegal Hands (23), Second Yellow Card (24), In (25), Out (26)



        //This game has lots of plays, purpose is to make sure stats total correctly.
        private void CreateSamplePlaysForClockDownGame()
        {
            //Need to set game to 'IN PROGRESS' otherwise sub transactions will result in change of starters 
            BaseTableDataAccess.Instance().UpdateGameStatus(1, "IN PROGRESS");
            Common.Instance().InitiliazeEventRoster(1);

            for (int i = 1; i <= 100; i++)
            {
                Play play = new Play();
                play.GameID = 1;
                play.PlayerPosition = "F";

                switch (i)
                {
                    case 1: //Jared/Shot/Hit Post/Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "19:43"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 2: //Jared/Shot/Hit Post/Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "19:21"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 3: //Jared/Shot/Miss/Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 1;
                        play.Period = 1; play.GameTime = "19:11"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 4: //Jared/Yellow Card/Unsportsmanlike Conduct
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Yellow Card"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Unsportsmanlike Conduct");
                        play.Period = 1; play.GameTime = "18:44";
                        break;
                    case 5: //Jared/Turnover/Lost Dribble
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Turnover"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Lost Dribble");
                        play.Period = 1; play.GameTime = "18:30";
                        break;
                    case 6: //Jared/Shot/GOAL/Shot On Goal/Left (Pete Izzo allowed goal)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 4;
                        play.Period = 1; play.GameTime = "18:20"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27;
                        //To simulate realtime data entry (i.e. user from GM screen)
                        Thread.Sleep(2000);
                        break;
                    case 7: //Jared/Shot/Blocked/Shot On Goal/Right/Blocked By other team goalie (Pete save)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "18:11"; play.ShotOnGoal = "Y"; play.ShotTypeID = 28; play.ShotBlockedByID = 23;
                        break;
                    case 8: //Jared/Shot/Hit Post/Shot Not On Goal/Headed
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "18:00"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 9: //Jared/Corner Kick/Good
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Corner Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                        play.Period = 1; play.GameTime = "17:45";
                        break;
                    case 10: //Jared/Shot/Blocked/Shot Not On Goal/Right/Blocked By other team goalie (Pete Izzo blocked)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "17:41"; play.ShotOnGoal = "N"; play.ShotTypeID = 28; play.ShotBlockedByID = 23;
                        break;
                    case 11: //Jared/Shot/Miss/Shot Not On Goal/Right
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 1;
                        play.Period = 1; play.GameTime = "17:11"; play.ShotOnGoal = "N"; play.ShotTypeID = 28;
                        break;
                    case 12: //Jared/Shot/Blocked/Shot On Goal/Right
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "16:28"; play.ShotOnGoal = "Y"; play.ShotTypeID = 28;
                        break;
                    case 13: //Jared/Shot/Blocked/Shot On Goal/Right
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "16:11"; play.ShotOnGoal = "Y"; play.ShotTypeID = 28;
                        break;
                    case 14: //Jared/Shot/Miss/Shot Not On Goal/Headed
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 1;
                        play.Period = 1; play.GameTime = "16:00"; play.ShotOnGoal = "N"; play.ShotTypeID = 29;
                        break;
                    case 15: //Jared/Shot/Hit Post/Shot Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "15:56"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 16: //Jared/Shot/GOAL/Shot On Goal/Left/Assist LEbron (Pete Izzo goal allowed)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 4;
                        play.Period = 1; play.GameTime = "15:23"; play.ShotOnGoal = "Y"; play.AssistID = 2; play.ShotTypeID = 27;
                        //To simulate realtime data entry (i.e. user from GM screen)
                        Thread.Sleep(2000);
                        break;
                    case 17: //Jared/Tackle
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Tackle"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "15:00";
                        break;
                    case 18: //Jared/Foul Drawn
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Drawn"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "14:31";
                        break;
                    case 19: //Jared/Shot/GOAL/Shot On Goal/Left (Pete Izzo goal allowed)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 4;
                        play.Period = 1; play.GameTime = "13:57"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27;
                        //To simulate realtime data entry (i.e. user from GM screen)
                        Thread.Sleep(2000);
                        break;
                    case 20: //Farmington Sub, Brenden in, Caden G out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 12; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "13:56";
                        break;
                    case 21: //Avon sub, Mark Smith in, Lee Johnson out
                        play.TeamID = 2; play.OtherTeamGoalieID = 0; play.Player1ID = 11; play.Player2ID = 24;
                        play.StatCategoryID = 22; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "13:56";
                        break;
                    case 22: //Brenden in, Mathew out
                        //  play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 12; play.Player2ID = 3; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.Period = 1; play.GameTime = "13:56";
                        break;
                    case 23: //Jared/Shot/Blocked/Shot On Goal/Left/Blocked by other team goalie (Pete Izzo save)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "13:24"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27; play.ShotBlockedByID = 23;
                        break;
                    case 24: //Jared/Shot/Hit Post/Shot Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "13:18"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 25: //Jared in, Sevie out
                        // play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 2; play.Player2ID = 6; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.Period = 1; play.GameTime = "12:40";
                        break;
                    case 26: //Jared/Indirect Free Kick
                        play.TeamID = 1; play.OtherTeamGoalieID = 11; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Indirect Free Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "12:17";
                        break;
                    case 27: //Jared/Foul/Holding
                        play.TeamID = 1; play.OtherTeamGoalieID = 11; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Holding");
                        play.Period = 1; play.GameTime = "11:44";
                        break;
                    case 28: //Jared/Shot/Hit Post/Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "11:20"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 29: //Jared/Shot/Hit Post/Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "11:01"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 30: //Jared/Shot/Hit Post/Shot On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "11:00"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27;
                        break;
                    case 31: //Jared/Shot/Hit Post/Shot On Goal/Right
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "10:58"; play.ShotOnGoal = "Y"; play.ShotTypeID = 28;
                        break;
                    case 32: //Jared/Shot/Blocked/Shot On Goal/Right/Blocked By other team goalie (Pete Izzo save)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "10:51"; play.ShotOnGoal = "Y"; play.ShotTypeID = 28; play.ShotBlockedByID = 23;
                        break;
                    case 33: //Jared/Shot/Blocked/Shot On Goal/Right/Blocked By other team goalie (Pete Izzo save)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "10:41"; play.ShotOnGoal = "Y"; play.ShotTypeID = 28; play.ShotBlockedByID = 23;
                        break;
                    case 34: //Jared/Shot/Blocked/Shot Not On Goal/Right/Blocked By other team goalie (Pete Izzo blocked)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "10:38"; play.ShotOnGoal = "N"; play.ShotTypeID = 28; play.ShotBlockedByID = 23;
                        break;
                    case 35: //Jared/Shot/Blocked/Shot Not On Goal/Right/Blocked By other team goalie (Pete Izzo blocked)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "10:35"; play.ShotOnGoal = "N"; play.ShotTypeID = 28; play.ShotBlockedByID = 23;
                        break;
                    case 36: //Jared/Shot/GOAL/Shot On Goal/Left/Assist Lebron (Pete Izzo goal allowed)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 4;
                        play.Period = 1; play.GameTime = "10:23"; play.ShotOnGoal = "Y"; play.AssistID = 1; play.ShotTypeID = 27;
                        //To simulate realtime data entry (i.e. user from GM screen)
                        Thread.Sleep(2000);
                        break;
                    case 37: //Jared/Turnover/Lost Dribble
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Turnover"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Lost Dribble");
                        play.Period = 1; play.GameTime = "10:12";
                        break;
                    case 38: //Unknown/Offsides
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = -1;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Offsides"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "10:11";
                        break;
                    case 39: //Jared/Offsides
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Offsides"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "10:10";
                        break;
                    case 40: //Jared/Offsides
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Offsides"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "10:09";
                        break;
                    case 41: //Jared and Sevie switched positions
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 2; play.Player2ID = 6;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Move"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "10:02";
                        break;
                    case 42: //Jared/Turnover/Illegal Throw In
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Turnover"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Illegal Throw In");
                        play.Period = 1; play.GameTime = "10:00";
                        break;
                    case 43: //Clock Adjusted 
                        play.Period = 1; play.GameTime = "09:58";
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Clock"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        break;
                    case 44: //Jared/Pass/Excellent
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 2; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Excellent");
                        play.Period = 1; play.GameTime = "09:56";
                        break;
                    case 45: //Jared/Pass/Good
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 2; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                        play.Period = 1; play.GameTime = "09:51";
                        break;
                    case 46: //Jared/Pass/Poor
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 2; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Poor");
                        play.Period = 1; play.GameTime = "09:45";
                        break;
                    case 47: //Jared/Foul Committed/Kicking
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Kicking");
                        play.Period = 1; play.GameTime = "09:23";
                        break;
                    case 48: //Jared/Foul Committed/Tripping
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Tripping");
                        play.Period = 1; play.GameTime = "09:20";
                        break;
                    case 49: //Jared/Foul Committed/Charging
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Charging");
                        play.Period = 1; play.GameTime = "09:15";
                        break;
                    case 50: //Jared/Foul Committed/Pushing
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Pushing");
                        play.Period = 1; play.GameTime = "09:12";
                        break;
                    case 51: //Jared/Foul Committed/Illegal Tackle
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Illegal Tackle");
                        play.Period = 1; play.GameTime = "09:09";
                        break;
                    case 52: //Jared/Out Of Bounds
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Out Of Bounds"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "09:01";
                        break;
                    case 53: //Jared/Cross/Excellent
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Cross"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Excellent");
                        play.Period = 1; play.GameTime = "08:56";
                        break;
                    case 54: //Jared/Cross/Good
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Cross"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                        play.Period = 1; play.GameTime = "08:51";
                        break;
                    case 55: //Jared/Cross/Poor
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Cross"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Poor");
                        play.Period = 1; play.GameTime = "08:45";
                        break;
                    case 56: //Jared/Throw IN
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Throw In"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Poor");
                        play.Period = 1; play.GameTime = "08:40";
                        break;
                    case 57: //Jared/Corner Kick/Excellent
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Corner Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Excellent");
                        play.Period = 1; play.GameTime = "08:34";
                        break;
                    case 58: //Jared/Corner Kick/Poor
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Corner Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Poor");
                        play.Period = 1; play.GameTime = "08:33";
                        break;
                    case 59: //Jared/Corner Kick/For Goal
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Corner Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("For Goal");
                        play.Period = 1; play.GameTime = "08:30";
                        break;
                    case 60: //Jared/Goalie Kick
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Goalie Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "08:21";
                        break;
                    case 61: //Jared/Drop Kick/Excellent
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Excellent");
                        play.Period = 1; play.GameTime = "08:18";
                        break;
                    case 62: //Jared/Drop Kick//Good
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                        play.Period = 1; play.GameTime = "08:14";
                        break;
                    case 63: //Jared/Drop Kick/Poor
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Poor");
                        play.Period = 1; play.GameTime = "08:10";
                        break;
                    case 64: //Jared/Direct Free Kick/Not For Goal
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Direct Free Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Not For Goal");
                        play.Period = 1; play.GameTime = "08:00";
                        break;
                    case 65: //Jared/Direct Free Kick/For Goal
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Direct Free Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("For Goal");
                        play.Period = 1; play.GameTime = "07:56"; play.ShotOnGoal = "Y";
                        break;
                    case 68: //Jared/Penalty Kick/Miss
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Penalty Kick"); play.StatDescriptionID = 1;
                        play.Period = 1; play.GameTime = "07:40";
                        break;
                    case 69: //Jared/Penalty Kick/Hit Post
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Penalty Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "07:38";
                        break;
                    case 70: //Jared/Penalty Kick/Blocked
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Penalty Kick"); play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "07:32";
                        break;
                    case 71: //Jared/Penalty Kick/Goal
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Penalty Kick"); play.StatDescriptionID = 4;
                        play.Period = 1; play.GameTime = "07:30";
                        //To simulate realtime data entry (i.e. user from GM screen)
                        Thread.Sleep(2000);
                        break;
                    case 72: //Jared/Yellow Card/Delaying Start Of Play
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Yellow Card"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Delaying Restart Of Play");
                        play.Period = 1; play.GameTime = "07:23";
                        break;
                    case 73: //Jared/Red Card/Foul Play
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Red Card"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Foul Play");
                        play.Period = 1; play.GameTime = "07:20";
                        break;
                    case 74: //Jared/Red Card/Foul Play
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Red Card"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Violent Conduct");
                        play.Period = 1; play.GameTime = "07:18";
                        break;
                    case 75: //Jared/Red Card/Foul Play
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Red Card"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Illegal Hands");
                        play.Period = 1; play.GameTime = "07:14";
                        break;
                    case 76: //Jared/Red Card/Foul Play
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Red Card"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Second Yellow Card");
                        play.Period = 1; play.GameTime = "07:13";
                        break;
                    case 77: //Jared/Drop Kick/Excellent
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Excellent");
                        play.Period = 1; play.GameTime = "07:00";
                        break;
                    case 78: //Jared/Drop Kick/Good
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                        play.Period = 1; play.GameTime = "06:50";
                        break;
                    case 79: //Jared/Drop Kick/Poor
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Poor");
                        play.Period = 1; play.GameTime = "06:40";
                        break;
                    case 80: //Jared/Shootout Kick/Miss
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Shootout Kick"); play.StatDescriptionID = 1;
                        play.Period = 1; play.GameTime = "06:30";
                        break;
                    case 81: //Jared/Shootout Kick/Hit Post
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Shootout Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "06:20";
                        break;
                    case 82: //Jared/Shootout Kick/Blocked
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Shootout Kick"); play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "06:10";
                        break;
                    case 83: //Jared/Shootout Kick/Goal
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Shootout Kick"); play.StatDescriptionID = 4;
                        play.Period = 1; play.GameTime = "06:00";
                        //To simulate realtime data entry (i.e. user from GM screen)
                        Thread.Sleep(2000);
                        break;
                    case 84: //Jared/Pass/Good
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 2; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                        play.Period = 1; play.GameTime = "05:53";
                        break;
                    case 85:
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Own Goal"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "05:40";
                        break;
                    case 86: //Jared/Pass/Excellent
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 2; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Excellent");
                        play.Period = 1; play.GameTime = "05:38";
                        break;
                    case 87: //Jared/Shot/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:35";
                        break;
                    case 88: //Jared/Pass/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 2; play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:33";
                        break;
                    case 89: //Jared/Turnover/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Turnover"); play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:31";
                        break;
                    case 90: //Jared/Foul Committed/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:29";
                        break;
                    case 91: //Jared/Cross/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Cross"); play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:27";
                        break;
                    case 92: //Jared/Corner Kick/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Corner Kick"); play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:25";
                        break;
                    case 93: //Jared/Direct Free Kick/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Direct Free Kick"); play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:23";
                        break;
                    case 94: //Jared/Penalty Kick/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Penalty Kick"); play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:21";
                        break;
                    case 95: //Jared/Yellow Card/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Yellow Card"); play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:19";
                        break;
                    case 96: //Jared/Red Card/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Red Card"); play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:17";
                        break;
                    case 97: //Jared/Drop Kick/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:15";
                        break;
                    case 98: //Jared/Shootout Kick/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Shootout Kick"); play.StatDescriptionID = null;
                        play.Period = 1; play.GameTime = "05:13";
                        break;
                    default:
                        break;
                }

                //To simulate realtime data entry (i.e. user from GM screen)
                //If we do not have this the stats update to quickly due to being upsertplay uses a thread and stats will not calculate correctly.
                Thread.Sleep(1000);

                if (play.Player1ID != 0)
                {
                    if (play.StatCategoryID == 22)
                    {
                        SaveSubstitutionPlay("GM", play.GameID, play.TeamID, play.Period, play.GameTime, play.Player1ID, play.GMPlayer1PositionID, play.Player2ID, string.Empty);
                    }
                    else if (play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Move"))
                    {
                        SaveMovePlay(play.GameID, play.TeamID, play.Period, play.GameTime, play.Player1ID, play.GMPlayer1PositionID, play.Player2ID, play.GMPlayer2PositionID);
                    }
                    else
                    {
                        UpsertPlay(play, "GM");
                    }
                }
                else if (play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Clock"))
                {
                    SaveClockPlay(play.GameID, play.Period, play.GameTime);
                }
            }

            //This is not really sample data, I am calculating player minutes played here
            //BaseTableDataAccess.UpdateGameCurrentPeriodAndCurrentClock(1, 1, "10:00");          
            Task.Factory.StartNew(() => StatCalculationsModule.CalculateALLPlayerMinutes(1));
        }

        //This game has lots of plays, purpose is to make sure stats total correctly.
        private void CreateSamplePlaysForClockDownGameLOTSOFSUBS()
        {
            //Need to set game to 'IN PROGRESS' otherwise sub transactions will result in change of starters 
            BaseTableDataAccess.Instance().UpdateGameStatus(2, "IN PROGRESS");
            Common.Instance().InitiliazeEventRoster(1);

            for (int i = 1; i <= 100; i++)
            {
                Play play = new Play();
                play.GameID = 2;
                play.PlayerPosition = "F";

                switch (i)
                {
                    case 1: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "44:00";
                        break;
                    case 2: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "43:00";
                        break;
                    case 3: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "42:00";
                        break;
                    case 4: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "41:00";
                        break;
                    case 5: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "40:00";
                        break;
                    case 6: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "39:00";
                        break;
                    case 7: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "38:00";
                        break;
                    case 8: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "37:00";
                        break;
                    case 9: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "36:00";
                        break;
                    case 10: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "35:00";
                        break;
                    case 11: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "34:00";
                        break;
                    case 12: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "33:00";
                        break;
                    case 13: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "32:00";
                        break;
                    case 14: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "31:00";
                        break;
                    case 15: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "30:00";
                        break;
                    case 16: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "29:00";
                        break;
                    case 17: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "28:00";
                        break;
                    case 18: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "27:00";
                        break;
                    case 19: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "26:00";
                        break;
                    case 20: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "25:00";
                        break;
                    case 21: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "24:00";
                        break;
                    case 22: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "23:00";
                        break;
                    case 23: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "22:00";
                        break;
                    case 24: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "21:00";
                        break;
                    case 25: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "20:00";
                        break;
                    case 26: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "19:00";
                        break;
                    case 27: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "18:00";
                        break;
                    case 28: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "17:00";
                        break;
                    case 29: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "16:00";
                        break;
                    case 30: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "15:00";
                        break;
                    case 31: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "14:00";
                        break;
                    case 32: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "13:00";
                        break;
                    case 33: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "12:00";
                        break;
                    case 34: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "11:00";
                        break;
                    case 35: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "10:00";
                        break;
                    case 36: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "09:00";
                        break;
                    case 37: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "08:00";
                        break;
                    case 38: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "07:00";
                        break;
                    case 39: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "06:00";
                        break;
                    case 40: //Avon sub, Caden Gallagher in, Brenden out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "05:00";
                        break;
                    case 41: //Jared/Shot/GOAL/Shot On Goal/Left (Pete Izzo allowed goal)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 4;
                        play.Period = 1; play.GameTime = "4:01"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27;
                        break;
                    case 42: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "04:00";
                        break;
                    case 43: //Avon sub, Caden Gallagher in, Brenden out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "03:00";
                        break;
                    case 44: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "02:00";
                        break;
                    case 45: //Avon sub, Caden Gallagher in, Brenden out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 1; play.GameTime = "01:00";
                        break;
                    //2nd half starts
                    case 46: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "44:00";
                        break;
                    case 47: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "43:00";
                        break;
                    case 48: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "42:00";
                        break;
                    case 49: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "41:00";
                        break;
                    case 50: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "40:00";
                        break;
                    case 51: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "39:00";
                        break;
                    case 52: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "38:00";
                        break;
                    case 53: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "37:00";
                        break;
                    case 54: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "36:00";
                        break;
                    case 55: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "35:00";
                        break;
                    case 56: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "34:00";
                        break;
                    case 57: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "33:00";
                        break;
                    case 58: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "32:00";
                        break;
                    case 59: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "31:00";
                        break;
                    case 60: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "30:00";
                        break;
                    case 61: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "29:00";
                        break;
                    case 62: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "28:00";
                        break;
                    case 63: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "27:00";
                        break;
                    case 64: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "26:00";
                        break;
                    case 65: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "25:00";
                        break;
                    case 66: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "24:00";
                        break;
                    case 67: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "23:00";
                        break;
                    case 68: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "22:00";
                        break;
                    case 69: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "21:00";
                        break;
                    case 70: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "20:00";
                        break;
                    case 71: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "19:00";
                        break;
                    case 72: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "18:00";
                        break;
                    case 73: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "17:00";
                        break;
                    case 74: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "16:00";
                        break;
                    case 75: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "15:00";
                        break;
                    case 76: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "14:00";
                        break;
                    case 77: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "13:00";
                        break;
                    case 78: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "12:00";
                        break;
                    case 79: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "11:00";
                        break;
                    case 80: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "10:00";
                        break;
                    case 81: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "09:00";
                        break;
                    case 82: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "08:00";
                        break;
                    case 83: //Avon sub, Caden Gallagher in, Graham Peterson out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "07:00";
                        break;
                    case 84: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "06:00";
                        break;
                    case 85: //Avon sub, Caden Gallagher in, Brenden out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "05:00";
                        break;
                    case 86: //Jared/Shot/GOAL/Shot On Goal/Left (Pete Izzo allowed goal)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 4;
                        play.Period = 2; play.GameTime = "4:01"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27;
                        break;
                    case 87: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "04:00";
                        break;
                    case 88: //Avon sub, Caden Gallagher in, Brenden out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "03:00";
                        break;
                    case 89: //Avon sub, Graham Peterson in, Caden Gallagher out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 13; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "02:00";
                        break;
                    case 90: //Avon sub, Caden Gallagher in, Brenden out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 4; play.Player2ID = 13;
                        play.StatCategoryID = 22; play.StatDescriptionID = 0;
                        play.Period = 2; play.GameTime = "01:00";
                        break;
                    default:
                        break;
                }

                if (play.Player1ID != 0)
                {
                    if (play.StatCategoryID == 22)
                    {
                        SaveSubstitutionPlay("GM", play.GameID, play.TeamID, play.Period, play.GameTime, play.Player1ID, play.GMPlayer1PositionID, play.Player2ID, string.Empty);
                    }
                    else if (play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Move"))
                    {
                        SaveMovePlay(play.GameID, play.TeamID, play.Period, play.GameTime, play.Player1ID, play.GMPlayer1PositionID, play.Player2ID, play.GMPlayer2PositionID);
                    }
                    else
                    {
                        UpsertPlay(play, "GM");
                    }
                }
            }
            // Task.Factory.StartNew(() => StatCalculationsModule.CalculateALLPlayerMinutes(2));
            StatCalculationsModule.CalculateALLPlayerMinutes(2);
        }


        private void CreateSamplePlaysForClockUpGame()
        {
            //Need to set game to 'IN PROGRESS' otherwise sub transactions will result in change of starters 
            BaseTableDataAccess.Instance().UpdateGameStatus(2, "IN PROGRESS");
            Common.Instance().InitiliazeEventRoster(2);

            for (int i = 1; i <= 100; i++)
            {
                Play play = new Play();
                play.GameID = 2;
                play.PlayerPosition = "F";

                switch (i)
                {
                    case 1: //Jared/Shot/Hit Post/Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "00:12"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 2: //Jared/Shot/Hit Post/Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "00:43"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 3: //Jared/Shot/Miss/Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 1;
                        play.Period = 1; play.GameTime = "01:22"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 4: //Jared/Yellow Card/Unsportsmanlike Conduct
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Yellow Card"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Unsportsmanlike Conduct");
                        play.Period = 1; play.GameTime = "01:34";
                        break;
                    case 5: //Jared/Turnover/Lost Dribble
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Turnover"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Lost Dribble");
                        play.Period = 1; play.GameTime = "01:57";
                        break;
                    case 6: //Jared/Shot/GOAL/Shot On Goal/Left (Pete Izzo allowed goal)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 4;
                        play.Period = 1; play.GameTime = "02:12"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27;
                        break;
                    case 7: //Jared/Shot/Blocked/Shot On Goal/Right/Blocked By other team goalie (Pete save)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "02:33"; play.ShotOnGoal = "Y"; play.ShotTypeID = 28; play.ShotBlockedByID = 23;
                        break;
                    case 8: //Jared/Shot/Hit Post/Shot Not On Goal/Headed
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "03:12"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 9: //Jared/Corner Kick/Good
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Corner Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                        play.Period = 1; play.GameTime = "03:22";
                        break;
                    case 10: //Jared/Shot/Blocked/Shot Not On Goal/Right/Blocked By other team goalie (Pete Izzo blocked)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "04:00"; play.ShotOnGoal = "N"; play.ShotTypeID = 28; play.ShotBlockedByID = 23;
                        break;
                    case 11: //Jared/Shot/Miss/Shot Not On Goal/Right
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 1;
                        play.Period = 1; play.GameTime = "04:12"; play.ShotOnGoal = "N"; play.ShotTypeID = 28;
                        break;
                    case 12: //Jared/Shot/Blocked/Shot On Goal/Right
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "04:14"; play.ShotOnGoal = "Y"; play.ShotTypeID = 28;
                        break;
                    case 13: //Jared/Shot/Blocked/Shot On Goal/Right
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "04:21"; play.ShotOnGoal = "Y"; play.ShotTypeID = 28;
                        break;
                    case 14: //Jared/Shot/Miss/Shot Not On Goal/Headed
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 1;
                        play.Period = 1; play.GameTime = "04:33"; play.ShotOnGoal = "N"; play.ShotTypeID = 29;
                        break;
                    case 15: //Jared/Shot/Hit Post/Shot Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "04:59"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 16: //Jared/Shot/GOAL/Shot On Goal/Left/Assist LEbron (Pete Izzo goal allowed)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 4;
                        play.Period = 1; play.GameTime = "05:01"; play.ShotOnGoal = "Y"; play.AssistID = 2; play.ShotTypeID = 27;
                        break;
                    case 17: //Jared/Tackle
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Tackle"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "05:12";
                        break;
                    case 18: //Jared/Foul Drawn
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Drawn"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "05:33";
                        break;
                    case 19: //Jared/Shot/GOAL/Shot On Goal/Left (Pete Izzo goal allowed)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 4;
                        play.Period = 1; play.GameTime = "05:55"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27;
                        break;
                    case 20: //Farmington Sub, Brenden in, Caden G out
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 12; play.Player2ID = 4;
                        play.StatCategoryID = 22; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "06:00";
                        break;
                    case 21: //Avon sub, Mark Smith in, Lee Johnson out
                        play.TeamID = 2; play.OtherTeamGoalieID = 0; play.Player1ID = 11; play.Player2ID = 24;
                        play.StatCategoryID = 22; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "06:00";
                        break;
                    case 22: //Brenden in, Mathew out
                        //  play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 12; play.Player2ID = 3; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.Period = 1; play.GameTime = "13:56";
                        break;
                    case 23: //Jared/Shot/Blocked/Shot On Goal/Left/Blocked by other team goalie (Pete Izzo save)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "06:02"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27; play.ShotBlockedByID = 23;
                        break;
                    case 24: //Jared/Shot/Hit Post/Shot Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "06:23"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 25: //Jared in, Sevie out
                        // play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 2; play.Player2ID = 6; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.Period = 1; play.GameTime = "12:40";
                        break;
                    case 26: //Jared/Indirect Free Kick
                        play.TeamID = 1; play.OtherTeamGoalieID = 11; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Indirect Free Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "06:55";
                        break;
                    case 27: //Jared/Foul/Holding
                        play.TeamID = 1; play.OtherTeamGoalieID = 11; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Holding");
                        play.Period = 1; play.GameTime = "07:00";
                        break;
                    case 28: //Jared/Shot/Hit Post/Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "07:20"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 29: //Jared/Shot/Hit Post/Not On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "07:45"; play.ShotOnGoal = "N"; play.ShotTypeID = 27;
                        break;
                    case 30: //Jared/Shot/Hit Post/Shot On Goal/Left
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "08:00"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27;
                        break;
                    case 31: //Jared/Shot/Hit Post/Shot On Goal/Right
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "08:58"; play.ShotOnGoal = "Y"; play.ShotTypeID = 28;
                        break;
                    case 32: //Jared/Shot/Blocked/Shot On Goal/Right/Blocked By other team goalie (Pete Izzo save)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "09:51"; play.ShotOnGoal = "Y"; play.ShotTypeID = 28; play.ShotBlockedByID = 23;
                        break;
                    case 33: //Jared/Shot/Blocked/Shot On Goal/Right/Blocked By other team goalie (Pete Izzo save)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "10:00"; play.ShotOnGoal = "Y"; play.ShotTypeID = 28; play.ShotBlockedByID = 23;
                        break;
                    case 34: //Jared/Shot/Blocked/Shot Not On Goal/Right/Blocked By other team goalie (Pete Izzo blocked)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "10:38"; play.ShotOnGoal = "N"; play.ShotTypeID = 28; play.ShotBlockedByID = 23;
                        break;
                    case 35: //Jared/Shot/Blocked/Shot Not On Goal/Right/Blocked By other team goalie (Pete Izzo blocked)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "11:35"; play.ShotOnGoal = "N"; play.ShotTypeID = 28; play.ShotBlockedByID = 23;
                        break;
                    case 36: //Jared/Shot/GOAL/Shot On Goal/Left/Assist Lebron (Pete Izzo goal allowed)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = 4;
                        play.Period = 1; play.GameTime = "11:53"; play.ShotOnGoal = "Y"; play.AssistID = 1; play.ShotTypeID = 27;
                        break;
                    case 37: //Jared/Turnover/Lost Dribble
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Turnover"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Lost Dribble");
                        play.Period = 1; play.GameTime = "12:12";
                        break;
                    case 38: //Unknown/Offsides
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = -1;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Offsides"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "13:11";
                        break;
                    case 39: //Jared/Offsides
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Offsides"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "14:10";
                        break;
                    case 40: //Jared/Offsides
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Offsides"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "15:09";
                        break;
                    case 41: //Jared and Sevie switched positions
                        play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 2; play.Player2ID = 6;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Move"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "16:02";
                        break;
                    case 42: //Jared/Turnover/Illegal Throw In
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Turnover"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Illegal Throw In");
                        play.Period = 1; play.GameTime = "16:45";
                        break;
                    case 43: //Clock Adjusted 
                        play.Period = 1; play.GameTime = "17:00";
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Clock"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        break;
                    case 44: //Jared/Pass/Excellent
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 2; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Excellent");
                        play.Period = 1; play.GameTime = "17:56";
                        break;
                    case 45: //Jared/Pass/Good
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 2; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                        play.Period = 1; play.GameTime = "18:51";
                        break;
                    case 46: //Jared/Pass/Poor
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 2; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Poor");
                        play.Period = 1; play.GameTime = "19:45";
                        break;
                    case 47: //Jared/Foul Committed/Kicking
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Kicking");
                        play.Period = 1; play.GameTime = "20:23";
                        break;
                    case 48: //Jared/Foul Committed/Tripping
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Tripping");
                        play.Period = 1; play.GameTime = "21:20";
                        break;
                    case 49: //Jared/Foul Committed/Charging
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Charging");
                        play.Period = 1; play.GameTime = "22:15";
                        break;
                    case 50: //Jared/Foul Committed/Pushing
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Pushing");
                        play.Period = 1; play.GameTime = "23:12";
                        break;
                    case 51: //Jared/Foul Committed/Illegal Tackle
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Illegal Tackle");
                        play.Period = 1; play.GameTime = "24:09";
                        break;
                    case 52: //Jared/Out Of Bounds
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Out Of Bounds"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "25:01";
                        break;
                    case 53: //Jared/Cross/Excellent
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Cross"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Excellent");
                        play.Period = 1; play.GameTime = "26:56";
                        break;
                    case 54: //Jared/Cross/Good
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Cross"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                        play.Period = 1; play.GameTime = "27:51";
                        break;
                    case 55: //Jared/Cross/Poor
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Cross"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Poor");
                        play.Period = 1; play.GameTime = "29:45";
                        break;
                    case 56: //Jared/Throw IN
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Throw In"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Poor");
                        play.Period = 1; play.GameTime = "30:40";
                        break;
                    case 57: //Jared/Corner Kick/Excellent
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Corner Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Excellent");
                        play.Period = 1; play.GameTime = "31:34";
                        break;
                    case 58: //Jared/Corner Kick/Poor
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Corner Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Poor");
                        play.Period = 1; play.GameTime = "32:33";
                        break;
                    case 59: //Jared/Corner Kick/For Goal
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Corner Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("For Goal");
                        play.Period = 1; play.GameTime = "33:30";
                        break;
                    case 60: //Jared/Goalie Kick
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Goalie Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 1; play.GameTime = "34:21";
                        break;
                    case 61: //Jared/Drop Kick/Excellent
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Excellent");
                        play.Period = 1; play.GameTime = "35:18";
                        break;
                    case 62: //Jared/Drop Kick//Good
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                        play.Period = 1; play.GameTime = "36:14";
                        break;
                    case 63: //Jared/Drop Kick/Poor
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Poor");
                        play.Period = 1; play.GameTime = "37:10";
                        break;
                    case 64: //Jared/Direct Free Kick/Not For Goal
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Direct Free Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Not For Goal");
                        play.Period = 1; play.GameTime = "38:00";
                        break;
                    case 65: //Jared/Direct Free Kick/For Goal
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Direct Free Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("For Goal");
                        play.Period = 1; play.GameTime = "39:56"; play.ShotOnGoal = "Y";
                        break;
                    case 68: //Jared/Penalty Kick/Miss
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Penalty Kick"); play.StatDescriptionID = 1;
                        play.Period = 1; play.GameTime = "40:40";
                        break;
                    case 69: //Jared/Penalty Kick/Hit Post
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Penalty Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 1; play.GameTime = "41:38";
                        break;
                    case 70: //Jared/Penalty Kick/Blocked
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Penalty Kick"); play.StatDescriptionID = 3;
                        play.Period = 1; play.GameTime = "44:32";
                        break;
                    case 71: //Jared/Penalty Kick/Goal
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Penalty Kick"); play.StatDescriptionID = 4;
                        play.Period = 2; play.GameTime = "00:30";
                        break;
                    case 72: //Jared/Yellow Card/Delaying Start Of Play
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Yellow Card"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Delaying Restart Of Play");
                        play.Period = 2; play.GameTime = "01:23";
                        break;
                    case 73: //Jared/Red Card/Foul Play
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Red Card"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Foul Play");
                        play.Period = 2; play.GameTime = "02:20";
                        break;
                    case 74: //Jared/Red Card/Foul Play
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Red Card"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Violent Conduct");
                        play.Period = 2; play.GameTime = "03:18";
                        break;
                    case 75: //Jared/Red Card/Foul Play
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Red Card"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Illegal Hands");
                        play.Period = 2; play.GameTime = "04:14";
                        break;
                    case 76: //Jared/Red Card/Foul Play
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Red Card"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Second Yellow Card");
                        play.Period = 2; play.GameTime = "05:13";
                        break;
                    case 77: //Jared/Drop Kick/Excellent
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Excellent");
                        play.Period = 2; play.GameTime = "06:00";
                        break;
                    case 78: //Jared/Drop Kick/Good
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                        play.Period = 2; play.GameTime = "07:50";
                        break;
                    case 79: //Jared/Drop Kick/Poor
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Poor");
                        play.Period = 2; play.GameTime = "08:40";
                        break;
                    case 80: //Jared/Shootout Kick/Miss
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Shootout Kick"); play.StatDescriptionID = 1;
                        play.Period = 2; play.GameTime = "09:30";
                        break;
                    case 81: //Jared/Shootout Kick/Hit Post
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Shootout Kick"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Hit Post");
                        play.Period = 2; play.GameTime = "10:20";
                        break;
                    case 82: //Jared/Shootout Kick/Blocked
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Shootout Kick"); play.StatDescriptionID = 3;
                        play.Period = 2; play.GameTime = "11:10";
                        break;
                    case 83: //Jared/Shootout Kick/Goal
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Shootout Kick"); play.StatDescriptionID = 4;
                        play.Period = 2; play.GameTime = "12:00";
                        break;
                    case 84: //Jared/Pass/Good
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 2; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Good");
                        play.Period = 2; play.GameTime = "13:53";
                        break;
                    case 85:
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Own Goal"); play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("");
                        play.Period = 2; play.GameTime = "14:40";
                        break;
                    case 86: //Jared/Pass/Excellent
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 2; play.StatDescriptionID = DAL.Instance().GetStatDescriptionIDByName("Excellent");
                        play.Period = 2; play.GameTime = "15:38";
                        break;
                    case 87: //Jared/Shot/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 1; play.StatDescriptionID = null;
                        play.Period = 2; play.GameTime = "16:35";
                        break;
                    case 88: //Jared/Pass/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = 2; play.StatDescriptionID = null;
                        play.Period = 2; play.GameTime = "17:33";
                        break;
                    case 89: //Jared/Turnover/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Turnover"); play.StatDescriptionID = null;
                        play.Period = 2; play.GameTime = "18:31";
                        break;
                    case 90: //Jared/Foul Committed/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Foul Committed"); play.StatDescriptionID = null;
                        play.Period = 2; play.GameTime = "19:29";
                        break;
                    case 91: //Jared/Cross/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Cross"); play.StatDescriptionID = null;
                        play.Period = 2; play.GameTime = "20:27";
                        break;
                    case 92: //Jared/Corner Kick/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Corner Kick"); play.StatDescriptionID = null;
                        play.Period = 2; play.GameTime = "21:25";
                        break;
                    case 93: //Jared/Direct Free Kick/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Direct Free Kick"); play.StatDescriptionID = null;
                        play.Period = 2; play.GameTime = "22:23";
                        break;
                    case 94: //Jared/Penalty Kick/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Penalty Kick"); play.StatDescriptionID = null;
                        play.Period = 2; play.GameTime = "23:21";
                        break;
                    case 95: //Jared/Yellow Card/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Yellow Card"); play.StatDescriptionID = null;
                        play.Period = 2; play.GameTime = "24:19";
                        break;
                    case 96: //Jared/Red Card/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Red Card"); play.StatDescriptionID = null;
                        play.Period = 2; play.GameTime = "25:17";
                        break;
                    case 97: //Jared/Drop Kick/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Drop Kick"); play.StatDescriptionID = null;
                        play.Period = 2; play.GameTime = "26:15";
                        break;
                    case 98: //Jared/Shootout Kick/No Description
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2;
                        play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Shootout Kick"); play.StatDescriptionID = null;
                        play.Period = 2; play.GameTime = "27:13";
                        break;
                    default:
                        break;
                }

                if (play.Player1ID != 0)
                {
                    if (play.StatCategoryID == 22)
                    {
                        SaveSubstitutionPlay("GM", play.GameID, play.TeamID, play.Period, play.GameTime, play.Player1ID, play.GMPlayer1PositionID, play.Player2ID, string.Empty);
                    }
                    else if (play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Move"))
                    {
                        SaveMovePlay(play.GameID, play.TeamID, play.Period, play.GameTime, play.Player1ID, play.GMPlayer1PositionID, play.Player2ID, play.GMPlayer2PositionID);
                    }
                    else
                    {
                        UpsertPlay(play, "GM");
                    }
                }
                else if (play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Clock"))
                {
                    SaveClockPlay(play.GameID, play.Period, play.GameTime);
                }
            }

            Task.Factory.StartNew(() => StatCalculationsModule.CalculateALLPlayerMinutes(2));
        }

        //private static void CreateSamplePlaysForClockDownGameGoalProcessingGameID3()
        //{
        //    //Need to set game to 'IN PROGRESS' otherwise sub transactions will result in change of starters 
        //    BaseTableDataAccess.UpdateGameStatus(3, "IN PROGRESS");
        //    Common.Instance().InitiliazeEventRoster(3);

        //    for (int i = 1; i <= 11; i++)
        //    {
        //        Play play = new Play();
        //        play.GameID = 3;
        //        play.PlayerPosition = "F";

        //        switch (i)
        //        {
        //            case 1: //Jared/Pass/Good
        //                play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2; play.StatCategoryID = 2; play.StatDescriptionID = 6;
        //                play.Period = 1; play.GameTime = "19:22";
        //                break;
        //            case 2: //Dave/Cross/Excellent
        //                play.TeamID = 2; play.OtherTeamGoalieID = 11; play.Player1ID = 25; play.StatCategoryID = 7; play.StatDescriptionID = 5;
        //                play.Period = 1; play.GameTime = "19:11";
        //                break;
        //            case 3: //Sevie/Shot/GOAL/Shot On Goal/Left (Pete Izzo allowed goal)
        //                play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 6; play.StatCategoryID = 1; play.StatDescriptionID = 4;
        //                play.Period = 1; play.GameTime = "18:20"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27;

        //                //Sevie/Penalty Kick/Goal
        //                //play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 6; play.StatCategoryID = 16; play.StatDescriptionID = 4;
        //                //play.Period = 1; play.GameTime = "18:20"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27;

        //                //Sevie/Direct Free Kick/Goal
        //                //play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 6; play.StatCategoryID = 14; play.StatDescriptionID = 4;
        //                //play.Period = 1; play.GameTime = "18:20"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27;

        //                //Sevie/Corner Kick/For Goal
        //                //play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 6; play.StatCategoryID = 9; play.StatDescriptionID = 17;
        //                //play.Period = 1; play.GameTime = "18:20"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27;

        //                //Sevie/Shootout Kick/Goal
        //                //play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 6; play.StatCategoryID = 21; play.StatDescriptionID = 4;
        //                //play.Period = 1; play.GameTime = "18:20"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27;

        //                //Sevie/Own Goal
        //                //play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 6; play.StatCategoryID = 12; play.StatDescriptionID = null;
        //                //play.Period = 1; play.GameTime = "18:20";
        //                break;
        //            case 4: //Joseph/Yellow Card/Unsportsmanlike Conduct
        //                play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 8; play.StatCategoryID = 17; play.StatDescriptionID = 19;
        //                play.Period = 1; play.GameTime = "18:14";
        //                break;
        //            case 5: //Clock Adjusted 
        //                play.Period = 1; play.GameTime = "16:58"; play.StatCategoryID = 31;
        //                break;
        //            case 6: //Caden/Cross/Excellent
        //                play.TeamID = 1; play.OtherTeamGoalieID = 11; play.Player1ID = 4; play.StatCategoryID = 3; play.StatDescriptionID = 10;
        //                play.Period = 1; play.GameTime = "16:11";
        //                break;
        //            case 7: //Dave/cross/excellent
        //                play.TeamID = 2; play.OtherTeamGoalieID = 11; play.Player1ID = 25; play.StatCategoryID = 7; play.StatDescriptionID = 5;
        //                play.Period = 1; play.GameTime = "15:44";
        //                break;
        //            case 8: //Joseph/cross/excellent
        //                play.TeamID = 2; play.OtherTeamGoalieID = 11; play.Player1ID = 8; play.StatCategoryID = 7; play.StatDescriptionID = 5;
        //                play.Period = 1; play.GameTime = "15:44";
        //                break;
        //            case 9: //Jared/Turnover/Lost Dribble
        //                play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 1; play.StatCategoryID = 3; play.StatDescriptionID = 10;
        //                play.Period = 1; play.GameTime = "15:44";
        //                break;
        //            case 10: //Caden G/Turnover/Lost Dribble
        //                play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 4; play.StatCategoryID = 3; play.StatDescriptionID = 10;
        //                play.Period = 1; play.GameTime = "15:30";
        //                break;
        //            case 11: //Jared and Sevie switched positions
        //                play.TeamID = 1; play.OtherTeamGoalieID = 0; play.Player1ID = 2; play.Player2ID = 6; play.StatCategoryID = 32;
        //                play.StatDescriptionID = 0; play.Period = 1; play.GameTime = "15:02";
        //                break;
        //            default:
        //                break;
        //        }

        //        if (play.Player1ID != 0)
        //        {
        //            if (play.StatCategoryID == 22)
        //            {
        //                SaveSubstitutionPlay(play.GameID, play.TeamID, play.Period, play.GameTime, play.Player1ID, play.GMPlayer1PositionID, play.Player2ID, string.Empty);
        //            }
        //            else if (play.StatCategoryID == 32)
        //            {
        //                SaveMovePlay(play.GameID, play.TeamID, play.Period, play.GameTime, play.Player1ID, play.GMPlayer1PositionID, play.Player2ID, play.GMPlayer2PositionID);
        //            }
        //            else
        //            {
        //                UpsertPlay(play);
        //            }
        //        }
        //        else if (play.StatCategoryID == 31)
        //        {
        //            SaveClockPlay(play.GameID, play.Period, play.GameTime);
        //        }
        //    }

        //    //This is not really sample data, I am calculating player minutes played here
        //    BaseTableDataAccess.UpdateGameCurrentPeriodAndCurrentClock(3, 1, "10:00");
        //}

        private void CreateSamplePlaysForGameWithPlaysWithSameTimeEditTestGameID7()
        {
            //Need to set game to 'IN PROGRESS' otherwise sub transactions will result in change of starters 
            BaseTableDataAccess.Instance().UpdateGameStatus(7, "IN PROGRESS");
            Common.Instance().InitiliazeEventRoster(7);

            for (int i = 1; i <= 8; i++)
            {
                Play play = new Play();
                play.GameID = 7;
                play.PlayerPosition = "F";

                switch (i)
                {
                    case 1: //Jared/Pass/Good
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 2; play.StatCategoryID = 2; play.StatDescriptionID = 6;
                        play.Period = 1; play.GameTime = "19:47";
                        break;
                    case 2: //Sevie/Shot/GOAL/Shot On Goal/Left (Pete Izzo allowed goal)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 6; play.StatCategoryID = 1; play.StatDescriptionID = 4;
                        play.Period = 1; play.GameTime = "19:20"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27;
                        break;
                    case 3: //Caden/Cross/Excellent
                        play.TeamID = 1; play.OtherTeamGoalieID = 11; play.Player1ID = 4; play.StatCategoryID = 3; play.StatDescriptionID = 10;
                        play.Period = 1; play.GameTime = "18:14";
                        break;
                    case 4: //Dave/cross/excellent
                        play.TeamID = 2; play.OtherTeamGoalieID = 11; play.Player1ID = 25; play.StatCategoryID = 7; play.StatDescriptionID = 5;
                        play.Period = 1; play.GameTime = "18:14";
                        break;
                    case 5: //Joseph/Shot/GOAL/Shot On Goal/Left (Pete Izzo allowed goal)
                        play.TeamID = 1; play.OtherTeamGoalieID = 23; play.Player1ID = 8; play.StatCategoryID = 1; play.StatDescriptionID = 4;
                        play.Period = 1; play.GameTime = "18:14"; play.ShotOnGoal = "Y"; play.ShotTypeID = 27;
                        break;
                    case 6: //Dave/Cross/Excellent
                        play.TeamID = 2; play.OtherTeamGoalieID = 11; play.Player1ID = 25; play.StatCategoryID = 7; play.StatDescriptionID = 5;
                        play.Period = 1; play.GameTime = "18:14";
                        break;
                    case 7: //Caden/Cross/Excellent
                        play.TeamID = 1; play.OtherTeamGoalieID = 11; play.Player1ID = 4; play.StatCategoryID = 3; play.StatDescriptionID = 10;
                        play.Period = 1; play.GameTime = "10:00";
                        break;
                    case 8: //Unknown/Turnover/Lost Dribble
                        play.TeamID = 1; play.OtherTeamGoalieID = 11; play.Player1ID = -1; play.StatCategoryID = 3; play.StatDescriptionID = 10;
                        play.Period = 1; play.GameTime = "09:51";
                        break;
                    default:
                        break;
                }
                UpsertPlay(play, "GM");
            }
        }

        private void CreateSampleSubsClockDown()
        {
            //Need to set game to 'IN PROGRESS' otherwise sub transactions will result in change of starters 
            BaseTableDataAccess.Instance().UpdateGameStatus(5, "IN PROGRESS");
            Common.Instance().InitiliazeEventRoster(5);

            for (int i = 1; i <= 12; i++)
            {
                Play play = new Play();
                play.GameID = 5;
                play.PlayerPosition = "F";

                //Teams Players: Ted 27, Bill 28, John 29, Steve 30, Bob 31, Lou 32, Chris 33			Starters: Ted, Bill, John, Steve	
                //1/8 Chris in John out, 1/6 John in Ted out, 1/4 Ted in Bill out
                //2/10 Bill in Chris out, Lou in Steve out, 2/6 Bob in Ted out, 2/3 Chris in Lou out, 2/1 Ted in Chris out
                //3/5 Steve in Ted out, 3/3 Ted in Bill out Chris in Steve out, 3/1 Lou in John out

                switch (i)
                {
                    case 1: //Chris in, John out
                        play.TeamID = 5; play.Period = 1; play.GameTime = "08:00"; play.Player1ID = 33; play.Player2ID = 29; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 2: //John in, Ted out
                        play.TeamID = 5; play.Period = 1; play.GameTime = "06:00"; play.Player1ID = 29; play.Player2ID = 27; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 3: //Ted in, Bill out
                        play.TeamID = 5; play.Period = 1; play.GameTime = "04:00"; play.Player1ID = 27; play.Player2ID = 28; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 4: //Bill in, Chris out
                        play.TeamID = 5; play.Period = 2; play.GameTime = "10:00"; play.Player1ID = 28; play.Player2ID = 33; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 5: //Lou in, Steve out
                        play.TeamID = 5; play.Period = 2; play.GameTime = "10:00"; play.Player1ID = 32; play.Player2ID = 30; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 6: //Bob in, Ted out
                        play.TeamID = 5; play.Period = 2; play.GameTime = "06:00"; play.Player1ID = 31; play.Player2ID = 27; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 7: //Chris in, Lou out
                        play.TeamID = 5; play.Period = 2; play.GameTime = "03:00"; play.Player1ID = 33; play.Player2ID = 32; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 8: //Ted in, Chris out
                        play.TeamID = 5; play.Period = 2; play.GameTime = "01:00"; play.Player1ID = 27; play.Player2ID = 33; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 9: //Steve in, Ted out
                        play.TeamID = 5; play.Period = 3; play.GameTime = "05:00"; play.Player1ID = 30; play.Player2ID = 27; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 10: //Ted in, Bill out
                        play.TeamID = 5; play.Period = 3; play.GameTime = "03:00"; play.Player1ID = 27; play.Player2ID = 28; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 11: //Chris in, Steve out
                        play.TeamID = 5; play.Period = 3; play.GameTime = "03:00"; play.Player1ID = 33; play.Player2ID = 30; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 12: //Lou in, John out
                        play.TeamID = 5; play.Period = 3; play.GameTime = "01:00"; play.Player1ID = 32; play.Player2ID = 29; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    default:
                        break;
                }

                SaveSubstitutionPlay("GM", play.GameID, play.TeamID, play.Period, play.GameTime, play.Player1ID, play.GMPlayer1PositionID, play.Player2ID, string.Empty);
            }

            //This is not really sample data, I am calculating player minutes played here
            DAL.Instance().UpdateGameCurrentPeriodAndCurrentClock(5, 3, "00:00");
            Task.Factory.StartNew(() => StatCalculationsModule.CalculateALLPlayerMinutes(5));
        }

        private void CreateSampleSubsClockUp()
        {
            //Need to set game to 'IN PROGRESS' otherwise sub transactions will result in ch;ange of starters 
            BaseTableDataAccess.Instance().UpdateGameStatus(6, "IN PROGRESS");
            Common.Instance().InitiliazeEventRoster(6);

            for (int i = 1; i <= 12; i++)
            {
                Play play = new Play();
                play.GameID = 6;
                play.PlayerPosition = "F";

                //Teams Players: Ted 27, Bill 28, John 29, Steve 30, Bob 31, Lou 32, Chris 33			Starters: Ted, Bill, John, Steve	   
                //1/2 Chris in John out, 1/4 John in Ted out, 1/6 Ted in Bill out
                //2/0 Bill in Chris out, Lou in Steve out, 2/4 Bob in Ted out, 2/7 Chris in Lou out, 2/9 Ted in Chris out
                //3/0 Steve in Ted out, 3/2 Ted in Bill out Chris in Steve out, 3/4 Lou in John out

                switch (i)
                {
                    case 1: //Chris in, John out
                        play.TeamID = 6; play.Period = 1; play.GameTime = "02:00"; play.Player1ID = 33; play.Player2ID = 29; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 2: //John in, Ted out
                        play.TeamID = 6; play.Period = 1; play.GameTime = "04:00"; play.Player1ID = 29; play.Player2ID = 27; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 3: //Ted in, Bill out
                        play.TeamID = 6; play.Period = 1; play.GameTime = "06:00"; play.Player1ID = 27; play.Player2ID = 28; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 4: //Bill in, Chris out
                        play.TeamID = 6; play.Period = 2; play.GameTime = "00:00"; play.Player1ID = 28; play.Player2ID = 33; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 5: //Lou in, Steve out
                        play.TeamID = 6; play.Period = 2; play.GameTime = "00:00"; play.Player1ID = 32; play.Player2ID = 30; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 6: //Bob in, Ted out
                        play.TeamID = 6; play.Period = 2; play.GameTime = "04:00"; play.Player1ID = 31; play.Player2ID = 27; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 7: //Chris in, Lou out
                        play.TeamID = 6; play.Period = 2; play.GameTime = "07:00"; play.Player1ID = 33; play.Player2ID = 32; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 8: //Ted in, Chris out
                        play.TeamID = 6; play.Period = 2; play.GameTime = "09:00"; play.Player1ID = 27; play.Player2ID = 33; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 9: //Steve in, Ted out
                        play.TeamID = 6; play.Period = 3; play.GameTime = "00:00"; play.Player1ID = 30; play.Player2ID = 27; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 10: //Ted in, Bill out
                        play.TeamID = 6; play.Period = 3; play.GameTime = "02:00"; play.Player1ID = 27; play.Player2ID = 28; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 11: //Chris in, Steve out
                        play.TeamID = 6; play.Period = 3; play.GameTime = "02:00"; play.Player1ID = 33; play.Player2ID = 30; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    case 12: //Lou in, John out
                        play.TeamID = 6; play.Period = 3; play.GameTime = "04:00"; play.Player1ID = 32; play.Player2ID = 29; play.StatCategoryID = 22; play.StatDescriptionID = 0; play.OtherTeamGoalieID = 0;
                        break;
                    default:
                        break;
                }
                SaveSubstitutionPlay("GM", play.GameID, play.TeamID, play.Period, play.GameTime, play.Player1ID, play.GMPlayer1PositionID, play.Player2ID, string.Empty);
            }

            //This is not really sample data, I am calculating player minutes played here
            DAL.Instance().UpdateGameCurrentPeriodAndCurrentClock(6, 3, "05:00");
            Task.Factory.StartNew(() => StatCalculationsModule.CalculateALLPlayerMinutes(6));
        }

        #endregion "Misc"

        #region "Players"

        //This is the physical player (height, weight, kicks, but NOT team, uniform, or active/reserve
        public Player GetPlayer(int? playerID)
        {
            Player _player = new Player();

            try
            {
                var selectedPlayer = BaseTableDataAccess.Instance().GetPlayerByPlayerID(playerID);

                if (selectedPlayer != null)
                {
                    _player = selectedPlayer;
                }
                else if (playerID == -1)
                {
                    _player.PlayerID = -1;
                    _player.FirstName = AppResources.Unknown;
                }

                return _player;
            }
            catch (Exception ex)
            {
                return _player;
            }
        }

        public ObservableCollection<TeamRosterModel> GetPlayersPhyiscalAndTeamRosterInfo(int gameID)
        {
            ObservableCollection<TeamRosterModel> returnValue = new ObservableCollection<TeamRosterModel>();
            List<TeamRosterModel> teamRosterList = new List<TeamRosterModel>();

            try
            {
                var eventRosterTable = BaseTableDataAccess.Instance().GetEventRosterByEventIDDictionary(gameID);
                var playersTable = BaseTableDataAccess.Instance().GetAllPlayersDictionary();
                var teamsTable = BaseTableDataAccess.Instance().GetAllTeamsDictionary();
                var teamRosterTable = BaseTableDataAccess.Instance().GetAllTeamRosterDictionary();

                foreach (var row in eventRosterTable.Keys)
                {
                    TeamRosterModel rosterRow = new TeamRosterModel();

                    rosterRow.Player = playersTable[eventRosterTable[row].PlayerID];
                    rosterRow.Team = teamsTable[eventRosterTable[row].TeamID];
                    rosterRow.EventRoster = eventRosterTable[eventRosterTable[row].PlayerID.ToString() + eventRosterTable[row].TeamID.ToString()];
                    rosterRow.TeamRoster = teamRosterTable[eventRosterTable[row].TeamID.ToString() + eventRosterTable[row].PlayerID.ToString()];
                    //Debug.WriteLine("Player = " + rosterRow.Player.FirstName + " gmpos id = " + rosterRow.EventRoster.GMPlayerPositionID);
                    teamRosterList.Add(rosterRow);
                }

                teamRosterList = teamRosterList.OrderBy(x => x.TeamRoster.UniformNumber).ToList();
                return new ObservableCollection<TeamRosterModel>(teamRosterList);
            }
            catch (Exception ex)
            {
                return returnValue;
            }
        }

        public ObservableCollection<TeamRosterModel> GetPlayersPhyiscalAndTeamRosterInfo(int gameID, int teamID, bool addNonePlayer, bool addUnknownPlayer)
        {
            ObservableCollection<TeamRosterModel> returnValue = new ObservableCollection<TeamRosterModel>();
            List<TeamRosterModel> teamRosterList = new List<TeamRosterModel>();

            try
            {
                var eventRosterTable = BaseTableDataAccess.Instance().GetEventRosterByEventIDDictionary(gameID);
                var playersTable = BaseTableDataAccess.Instance().GetAllPlayersDictionary();
                var teamsTable = BaseTableDataAccess.Instance().GetAllTeamsDictionary();
                var teamRosterTable = BaseTableDataAccess.Instance().GetAllTeamRosterDictionary();

                foreach (var row in eventRosterTable.Keys)
                {
                    if (eventRosterTable[row].TeamID == teamID)
                    {
                        TeamRosterModel rosterRow = new TeamRosterModel();

                        rosterRow.Player = playersTable[eventRosterTable[row].PlayerID];
                        rosterRow.Team = teamsTable[eventRosterTable[row].TeamID];
                        rosterRow.EventRoster = eventRosterTable[eventRosterTable[row].PlayerID.ToString() + eventRosterTable[row].TeamID.ToString()];
                        rosterRow.TeamRoster = teamRosterTable[eventRosterTable[row].TeamID.ToString() + eventRosterTable[row].PlayerID.ToString()];
                        rosterRow.RosterDisplayText = rosterRow.TeamRoster.UniformNumber.PadRight(4, ' ') + " " + rosterRow.Player.FirstName + " " + rosterRow.Player.LastName;
                        rosterRow.UniformNumberInt = Common.Instance().ConvertStringToNumber((rosterRow.TeamRoster.UniformNumber));
                        teamRosterList.Add(rosterRow);
                    }
                }

                if (addNonePlayer == true)
                {
                    //Now need to add a NONE player selection
                    TeamRosterModel nonePlayer = new TeamRosterModel();
                    nonePlayer.RosterDisplayText = AppResources.None;
                    nonePlayer.Player.PlayerID = -2;
                    nonePlayer.TeamRoster.PlayerID = -1;
                    nonePlayer.UniformNumberInt = -2;

                    teamRosterList.Add(nonePlayer);
                }

                if (addUnknownPlayer == true)
                {
                    //Now need to add a Unknown player selection
                    TeamRosterModel unknownPlayer = new TeamRosterModel();
                    unknownPlayer.RosterDisplayText = "?".PadRight(4, ' ') + " " + AppResources.Unknown;
                    unknownPlayer.Player.PlayerID = -1;
                    unknownPlayer.TeamRoster.UniformNumber = "?";
                    unknownPlayer.TeamRoster.PlayerID = -1;
                    unknownPlayer.UniformNumberInt = -1;

                    teamRosterList.Add(unknownPlayer);
                }

                teamRosterList = teamRosterList.OrderBy(x => x.UniformNumberInt).ToList();
                return new ObservableCollection<TeamRosterModel>(teamRosterList);
            }
            catch (Exception ex)
            {
                return returnValue;
            }
        }

        public TeamRosterModel GetPlayersPhyiscalAndTeamRosterInfo(int gameID, int teamID, int playerID)
        {
            TeamRosterModel returnValue = new TeamRosterModel();

            try
            {
                var eventRosterTable = BaseTableDataAccess.Instance().GetEventRosterByEventIDDictionary(gameID);
                var playersTable = BaseTableDataAccess.Instance().GetAllPlayersDictionary();
                var teamsTable = BaseTableDataAccess.Instance().GetAllTeamsDictionary();
                var teamRosterTable = BaseTableDataAccess.Instance().GetAllTeamRosterDictionary();

                foreach (var row in eventRosterTable.Keys)
                {
                    if ((eventRosterTable[row].TeamID == teamID) && (eventRosterTable[row].PlayerID == playerID))
                    {
                        returnValue.Player = playersTable[eventRosterTable[row].PlayerID];
                        returnValue.Team = teamsTable[eventRosterTable[row].TeamID];
                        returnValue.EventRoster = eventRosterTable[eventRosterTable[row].PlayerID.ToString() + eventRosterTable[row].TeamID.ToString()];
                        returnValue.TeamRoster = teamRosterTable[eventRosterTable[row].TeamID.ToString() + eventRosterTable[row].PlayerID.ToString()];
                        returnValue.RosterDisplayText = returnValue.TeamRoster.UniformNumber.PadRight(4, ' ') + " " + returnValue.Player.FirstName + " " + returnValue.Player.LastName;
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

        public ObservableCollection<PlayerModel> GetAllPlayers(string includeInVisible, string searchCharacter)
        {
            ObservableCollection<PlayerModel> _players = new ObservableCollection<PlayerModel>();

            try
            {
                var list = BaseTableDataAccess.Instance().GetAllPlayersList();
                if (list != null)
                {
                    foreach (var player in list)
                    {
                        if ((includeInVisible == "Y") || (includeInVisible == "N" && player.Visible == "Y"))
                        {
                            if (player.LastName != null)
                            {
                                if ((searchCharacter == string.Empty) ||
                                    player.LastName.ToUpper().StartsWith(searchCharacter.ToUpper()) ||
                                    player.FirstName.ToUpper().StartsWith(searchCharacter.ToUpper()))
                                {
                                    PlayerModel _playerModel = new PlayerModel();
                                    _playerModel.Player = player;
                                    _playerModel.PlayerName = player.FirstName + " " + player.LastName;

                                    _players.Add(_playerModel);
                                }
                            }
                            else   //Last name is null, so do not include it in the check...
                            {
                                if ((searchCharacter == string.Empty) ||
                                 player.FirstName.ToUpper().StartsWith(searchCharacter.ToUpper()))
                                {
                                    PlayerModel _playerModel = new PlayerModel();
                                    _playerModel.Player = player;
                                    _playerModel.PlayerName = player.FirstName + " " + player.LastName;

                                    _players.Add(_playerModel);
                                }
                            }
                        }
                    }
                }
                return _players;
            }
            catch (Exception ex)
            {
                return _players;
            }
        }

        public void DeletePlayerFromAnyAllTeamRosters(int playerID)
        {
            try
            {
                BaseTableDataAccess.Instance().DeletePlayerFromAnyAllTeamRosters(playerID);
            }
            catch (Exception ex)
            {
            }
        }

        public GameModel GetPlayersFirstGame(int playerID)
        {
            GameModel _game = new GameModel();

            try
            {
                var list = BaseTableDataAccess.Instance().GetPlayersListOfGames(playerID);

                if (list != null)
                {
                    foreach (var eventRosterEntry in list)
                    {
                        _game = DAL.Instance().GetGame(eventRosterEntry.GameID);
                        break;
                    }
                }
                return _game;
            }
            catch (Exception ex)
            {
                return _game;
            }
        }

        #endregion "Players"

        #region "Teams"

        public TeamModel GetTeam(int teamID)
        {
            TeamModel _team = new TeamModel();

            try
            {
                var selectedTeam = BaseTableDataAccess.Instance().GetTeamByTeamID(teamID);

                if (selectedTeam != null)
                {
                    _team.Team = selectedTeam;
                }

                return _team;
            }
            catch (Exception ex)
            {
                return _team;
            }
        }

        public ObservableCollection<TeamModel> GetTeams(string includeInVisible, string addSelectTeam)
        {
            List<TeamModel> teams = new List<TeamModel>();

            try
            {
                var list = BaseTableDataAccess.Instance().GetAllTeamsList();
                if (list != null)
                {
                    foreach (var team in list)
                    {
                        if ((includeInVisible == "Y") || (includeInVisible == "N" && team.Visible == "Y"))
                        {
                            TeamModel _teamModel = new TeamModel();
                            _teamModel.Team = team;
                            _teamModel.JerseySource = BaseTableDataAccess.Instance().GetJerseyByJerseyID(team.JerseyID).ImagePath;
                            teams.Add(_teamModel);
                        }
                    }
                }

                if (addSelectTeam.ToUpper() == "Y")
                {
                    //Now need to add a 'SELECT TEAM'  selection
                    TeamModel selectTeam = new TeamModel();
                    selectTeam.Team.TeamID = -1;
                    selectTeam.Team.TeamName = "--" + AppResources.SelectTeam + "--";
                    selectTeam.Team.TeamShortName = "aaa";
                    selectTeam.Team.SampleData = "N";

                    teams.Add(selectTeam);
                    teams = teams.OrderBy(x => x.Team.TeamShortName).ToList();
                }
                return new ObservableCollection<TeamModel>(teams);
            }
            catch (Exception ex)
            {
                return new ObservableCollection<TeamModel>(teams);
            }
        }

        #endregion "Teams"

        #region "Leagues"

        public LeagueModel GetLeague(int leagueID)
        {
            LeagueModel _league = new LeagueModel();

            try
            {
                var selectedLeague = BaseTableDataAccess.Instance().GetLeagueByLeagueID(leagueID);

                if (selectedLeague != null)
                {
                    _league.League = selectedLeague;
                }

                return _league;
            }

            catch (Exception ex)
            {
                return _league;
            }
        }

        //'NO LEAGUE' it is only so user can select it in TEAM DETAILS when setting up a team
        public ObservableCollection<LeagueModel> GetLeagues(bool includeInVisible, bool includeNoLeauge)
        {
            ObservableCollection<LeagueModel> leagues = new ObservableCollection<LeagueModel>();
            string whereClause = string.Empty;
            try
            {
                var list = BaseTableDataAccess.Instance().GetAllLeaguesList();
                if (list != null)
                {
                    foreach (var league in list)
                    {
                        if ((includeInVisible == true && league.LeagueName != AppResources.NoLeague) || (includeInVisible == false && league.Visible == "Y" && league.LeagueName != AppResources.NoLeague) || (includeNoLeauge == true && league.LeagueName == AppResources.NoLeague))
                        {
                            LeagueModel _leagueModel = new LeagueModel();
                            _leagueModel.League = league;
                            leagues.Add(_leagueModel);
                        }
                    }
                }
                return leagues;
            }
            catch (Exception ex)
            {
                return leagues;
            }
        }

        #endregion "Leagues"

        #region "TeamRoster"

        //Since TeamRoster does not have a primary key, I need to do a delete this way and not with db.Delete
        public void DeleteTeamRoster(TeamRoster teamRoster)
        {
            try
            {
                BaseTableDataAccess.Instance().DeleteTeamRoster(teamRoster.TeamID, teamRoster.PlayerID);
            }
            catch (Exception ex)
            {
            }
        }

        public TeamRoster GetPlayerTeamRosterEntry(int teamID, int playerID)
        {
            TeamRoster _teamRoster = new TeamRoster();

            try
            {
                var selectedTeamRoster = BaseTableDataAccess.Instance().GetPlayerTeamRosterEntry(teamID, playerID);

                if (selectedTeamRoster != null)
                {
                    _teamRoster = selectedTeamRoster;
                }
                else if (playerID == -1)
                {
                    _teamRoster.PlayerID = -1;
                    _teamRoster.UniformNumber = "?";
                }

                return _teamRoster;
            }

            catch (Exception ex)
            {
                return _teamRoster;
            }
        }

        #endregion "TeamRoster"

        #region "Game"

        public void UpdateGameAndPlayScore(Play play)
        {
            int goalTotal;
            Game game = new Game();
            Play mostRecentPriorPlay = new Play();

            try
            {
                //Include own goals, but do not include shootout goals as those do not impact the game score
                if (Common.Instance().IsThisAGCoalScoredPlay(play, true, false))
                {
                    game = BaseTableDataAccess.Instance().GetGameByGameID(play.GameID);

                    //Based on the clock time, get the most recent play (i.e. play before this clock time) and the home and away play score and use for this play score    
                    mostRecentPriorPlay = DAL.Instance().GetPlaysForGamePriorToElapsedTime(play.GameID, play.ElapsedTimeInSeconds, play.PlayID);
                    play.AwayScore = mostRecentPriorPlay.AwayScore;
                    play.HomeScore = mostRecentPriorPlay.HomeScore;

                    //Team id is the team that scored, unless it is an own goal then it is the other team that gets a goal
                    if (((game.HomeTeamID == play.TeamID) && play.StatCategoryID != 12) ||
                        (play.StatCategoryID == 12 && game.AwayTeamID == play.TeamID))
                    {
                        goalTotal = game.HomeTeamScore + 1;
                        BaseTableDataAccess.Instance().UpdateGameHomeTeamScore(play.GameID, goalTotal);
                        play.HomeScore = play.HomeScore + 1;
                        BaseTableDataAccess.Instance().UpdatePlayHomeTeamScore(play.GameID, play.PlayID, play.HomeScore);
                    }
                    else
                    {
                        goalTotal = game.AwayTeamScore + 1;
                        BaseTableDataAccess.Instance().UpdateGameAwayTeamScore(play.GameID, goalTotal);
                        play.AwayScore = play.AwayScore + 1;
                        BaseTableDataAccess.Instance().UpdatePlayAwayTeamScore(play.GameID, play.PlayID, play.AwayScore);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void UpsertGame(Game game)
        {
            bool _newGame = false;
            try
            {

                if (game.GameID == 0)
                {
                    _newGame = true;
                    game.GameStatus = "NOT STARTED";
                    game.Visible = "Y";
                    game.CurrentPeriod = 1;
                    game.HomeTeamScore = 0;
                    game.AwayTeamScore = 0;
                    game.ClockUpOrDown = game.ClockUpOrDown.ToUpper();
                    game.HomeTeamSideOfField = "RIGHT";
                    game.AwayTeamSideOfField = "LEFT";

                    //Default game formations based on either 8v8 or 11v11
                    if (game.PlayersPerTeam == 8)
                    {
                        game.HomeFormationID = 1;
                        game.AwayFormationID = 1;
                    }
                    else
                    {
                        game.HomeFormationID = 4;
                        game.AwayFormationID = 4;
                    }
                }

                BaseTableDataAccess.Instance().UpsertGame(game);

                //Whenever we are adding a new game, take all ACTIVE players from that teams Roster and put them on the Event Roster for that team.
                //For starter/reserve, if there is a previous game use starter/reserve values from that, else randomly pick the 8 or 11 starters
                if (_newGame == true)
                {
                    //We decided we will update event roster when user clicks on Game Manager
                    //This way if the user sets up the entire seasons schedule at the beginning of the year we will not initialize event
                    //roster until they actually go into the game (and have some starter history by then and the starters will be more accurate)

                    //Common.Instance().InitiliazeEventRoster(game.GameID);
                }

            }
            catch (Exception ex)
            {
            }
        }

        public ObservableCollection<GameModel> GetGames(string includeInVisible)
        {
            ObservableCollection<GameModel> games = new ObservableCollection<GameModel>();

            try
            {
                var list = BaseTableDataAccess.Instance().GetAllGamesList();
                if (list != null)
                {
                    foreach (var game in list)
                    {
                        if ((includeInVisible == "Y") || (includeInVisible == "N" && game.Visible == "Y"))
                        {
                            GameModel _gameModel = new GameModel();
                            _gameModel.Game = game;
                            _gameModel.HomeTeam = GetTeam(game.HomeTeamID).Team;
                            _gameModel.AwayTeam = GetTeam(game.AwayTeamID).Team;
                            _gameModel.GameTitle = _gameModel.AwayTeam.TeamName + " at " + _gameModel.HomeTeam.TeamName;
                            _gameModel.GameTitleWithDate = _gameModel.Game.GameDate.ToShortDateString() + " " + _gameModel.AwayTeam.TeamShortName + " at " + _gameModel.HomeTeam.TeamShortName;
                            games.Add(_gameModel);
                        }
                    }
                }
                return games;
            }
            catch (Exception ex)
            {
                return games;
            }
        }


        public int GetGamesPlayCount(int gameID)
        {
            int gamesPlayCount = 0;
            List<Play> playList = new List<Play>();

            try
            {
                playList = BaseTableDataAccess.Instance().GetAllPlaysForGame(gameID, "ASC");
                gamesPlayCount = playList.Count();

                return gamesPlayCount;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public GameModel GetGame(int gameID)
        {
            GameModel _game = new GameModel();

            try
            {
                var selectedGame = BaseTableDataAccess.Instance().GetGameByGameID(gameID);

                if (selectedGame != null)
                {
                    _game.Game = selectedGame;
                    _game.HomeTeam = GetTeam(selectedGame.HomeTeamID).Team;
                    _game.AwayTeam = GetTeam(selectedGame.AwayTeamID).Team;
                }

                return _game;
            }
            catch (Exception ex)
            {
                return _game;
            }
        }

        public GameModel GetTeamsFirstGame(int teamID)
        {
            GameModel _game = new GameModel();

            try
            {
                var list = BaseTableDataAccess.Instance().GetAllGamesForTeamOrderByGameDateASC(teamID);

                if (list != null)
                {
                    foreach (var game in list)
                    {
                        _game.Game = game;
                        break;
                    }
                }
                return _game;
            }
            catch (Exception ex)
            {
                return _game;
            }
        }

        public void DeleteGame(int gameID)
        {
            try
            {
                BaseTableDataAccess.Instance().DeleteGame(gameID);
            }
            catch (Exception ex)
            {
            }
        }

        //This proc is called by Game Manager.  It will update the games actual period length for CLOCK UP games. 
        //Then for both clock up and clock down we update the games current period and current clock to the next period/clock (for next clock it calculates the startperiodtime for the next period)

        //This proc returns the next period starting time in seconds

        public int AdvancePeriod(int gameID, int period, string actualPeriodLength)
        {
            Game game = new Game();
            int nextPeriodStartTimeInSeconds;
            string periodStartAsClockStringValueWithColon;

            int elapsedTimeOfActualPeriodLength;
            int elapsedTimeOfAFullPeriodInClockUpGame;
            int periodLength = 0;
            string clockValue;
            int actualPeriodLengthInSeconds;

            try
            {
                game = BaseTableDataAccess.Instance().GetGameByGameID(gameID);

                //If clock up then need to update the period actual length value
                if (game.ClockUpOrDown.ToUpper() == "UP")
                {
                    //If for some reason user advanced period PRIOR to clock reaching the period length, we use period length instead of what was passed
                    //For example, if in a clock up game with 45:00 minute periods, if user clicked advance period at 2:30 into the game, 
                    //we would not use 2:30 but would use 45:00, if user passed in 47:12 for 2nd period, we would use 90:00
                    elapsedTimeOfActualPeriodLength = StatCalculationsModule.CalculateTimeElapsedInPeriodInSecondsForCLOCKUP(game, period, actualPeriodLength);
                    elapsedTimeOfAFullPeriodInClockUpGame = StatCalculationsModule.CalculateRegulationTimeElapsedInPeriodInSecondsForCLOCKUP(game, period);

                    if (elapsedTimeOfActualPeriodLength < elapsedTimeOfAFullPeriodInClockUpGame)
                    {
                        if (period <= game.Periods)
                        {
                            UpdateActualPeriodLengthForClockUpGame(game, period, game.PeriodLength + ":00");
                            periodLength = game.PeriodLength * period;
                        }
                        else
                        {
                            UpdateActualPeriodLengthForClockUpGame(game, period, game.OverTimeLength + ":00");
                            periodLength = (game.PeriodLength * game.Periods) + (game.OverTimeLength * (period - game.Periods));
                        }
                        DAL.Instance().UpdateGameCurrentPeriodAndCurrentClock(gameID, period, periodLength + ":00");
                    }
                    else   //The period ended at the expected time or later (i.e. 45:00, 47:13, 90:00, 94:13, 105:00, 108:17)
                    {
                        if (period <= game.Periods)
                        {
                            //Need to determine what the actual time was for the period and update (i.e. 92:13 = 47:13)
                            actualPeriodLengthInSeconds = elapsedTimeOfActualPeriodLength - (game.PeriodLength * (period - 1) * 60);
                            clockValue = Common.Instance().ConvertSecondsToClockValue(actualPeriodLengthInSeconds);
                        }
                        else
                        {
                            //Need to determine what the actual time was for the period and update
                            actualPeriodLengthInSeconds = elapsedTimeOfActualPeriodLength - (game.PeriodLength * (game.Periods) * 60) - (game.OverTimeLength * (period - (game.Periods + 1)) * 60);
                            clockValue = Common.Instance().ConvertSecondsToClockValue(actualPeriodLengthInSeconds);
                        }
                        UpdateActualPeriodLengthForClockUpGame(game, period, clockValue);
                        DAL.Instance().UpdateGameCurrentPeriodAndCurrentClock(gameID, period, actualPeriodLength);
                    }
                }
                else //clock down game so set clock to 00:00
                {
                    DAL.Instance().UpdateGameCurrentPeriodAndCurrentClock(gameID, period, "00:00");
                }

                Task.Factory.StartNew(() => StatCalculationsModule.CalculateALLPlayerMinutes(gameID));

                periodStartAsClockStringValueWithColon = GetPeriodStartAsClockStringValueWithColon(game, period + 1);
                DAL.Instance().UpdateGameCurrentPeriodAndCurrentClock(gameID, period + 1, periodStartAsClockStringValueWithColon);

                nextPeriodStartTimeInSeconds = GetPeriodStartTimeInSeconds(game, period + 1);
                return nextPeriodStartTimeInSeconds;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void UpdateActualPeriodLengthForClockUpGame(Game game, int period, string actualPeriodLength)
        {
            try
            {
                switch (period)
                {
                    case 1:
                        BaseTableDataAccess.Instance().UpdateGamesPeriod1ActualLength(game.GameID, actualPeriodLength);
                        break;
                    case 2:
                        BaseTableDataAccess.Instance().UpdateGamesPeriod2ActualLength(game.GameID, actualPeriodLength);
                        break;
                    case 3:
                        BaseTableDataAccess.Instance().UpdateGamesPeriod3ActualLength(game.GameID, actualPeriodLength);
                        break;
                    case 4:
                        BaseTableDataAccess.Instance().UpdateGamesPeriod4ActualLength(game.GameID, actualPeriodLength);
                        break;
                    case 5:
                        BaseTableDataAccess.Instance().UpdateGamesPeriod5ActualLength(game.GameID, actualPeriodLength);
                        break;
                    case 6:
                        BaseTableDataAccess.Instance().UpdateGamesPeriod6ActualLength(game.GameID, actualPeriodLength);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public int GetPeriodStartTimeInSeconds(Game game, int period)
        {
            int periodStartTime;
            int periodLength;

            //For clock down games period start time is period length or if in OT then OT length
            if (game.ClockUpOrDown.ToUpper() == "DOWN")
            {
                if (period <= game.Periods)
                {
                    periodStartTime = StatCalculationsModule.ConvertClockStringValueToSeconds(game.PeriodLength + ":00");
                }
                else
                {
                    periodStartTime = StatCalculationsModule.ConvertClockStringValueToSeconds(game.OverTimeLength + ":00");
                }
            }
            else  //This is a clock up game, so clock starts at period length value (i.e. 1st = 0, 2nd = 45, OT1 = 90, OT2 = 105)
            {
                //For all periods
                if (period <= game.Periods + 1)
                {
                    periodLength = (game.PeriodLength * (period - 1));
                    periodStartTime = StatCalculationsModule.ConvertClockStringValueToSeconds(periodLength + ":00");
                }
                else //This is OT2 on start times (i.e. (2*45) + (4-2+1) * 15) = 105
                {
                    periodLength = (game.Periods * game.PeriodLength) + ((period - game.Periods - 1) * (int)game.OverTimeLength);
                    periodStartTime = StatCalculationsModule.ConvertClockStringValueToSeconds(periodLength + ":00");
                }
            }

            return periodStartTime;
        }

        public string GetPeriodStartAsClockStringValueWithColon(Game game, int period)
        {
            int periodLength;
            string periodStartTimeAsClockStringValueWithColon;

            //For clock down games period start time is period length or if in OT then OT length
            if (game.ClockUpOrDown.ToUpper() == "DOWN")
            {
                if (period <= game.Periods)
                {
                    periodStartTimeAsClockStringValueWithColon = game.PeriodLength + ":00";
                }
                else
                {
                    periodStartTimeAsClockStringValueWithColon = game.OverTimeLength + ":00";
                }
            }
            else  //This is a clock up game, so clock starts at period length value (i.e. 1st = 0, 2nd = 45, OT1 = 90, OT2 = 105)
            {
                //For all periods
                if (period <= game.Periods + 1)
                {
                    periodLength = (game.PeriodLength * (period - 1));
                    periodStartTimeAsClockStringValueWithColon = periodLength + ":00";
                }
                else //This is OT2 on start times (i.e. (2*45) + (4-2+1) * 15) = 105
                {
                    periodLength = (game.Periods * game.PeriodLength) + ((period - game.Periods - 1) * (int)game.OverTimeLength);
                    periodStartTimeAsClockStringValueWithColon = periodLength + ":00";
                }
            }

            return periodStartTimeAsClockStringValueWithColon;
        }

        public void UpdateGameCurrentPeriodAndCurrentClock(int gameID, int currentPeriod, string currentClock)
        {
            Game game = new Game();
            int gameElapsedTimeInSeconds;

            try
            {

                game = BaseTableDataAccess.Instance().GetGameByGameID(gameID);
                gameElapsedTimeInSeconds = StatCalculationsModule.CalculateTimeElapsedInGameInSeconds(game, currentPeriod, currentClock);

                BaseTableDataAccess.Instance().UpdateGameCurrentPeriodAndCurrentClock(gameID, currentPeriod, currentClock, gameElapsedTimeInSeconds);
            }
            catch (Exception ex)
            {
            }
        }


        public void UpdateGameStatus(int gameID, string gameStatus)
        {
            try
            {
                BaseTableDataAccess.Instance().UpdateGameStatus(gameID, gameStatus);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Sets game to inprogress, sets period to 1, sets clock to period start time (00:00 for clock up game or period length for clock down game)
        public void StartGame(int gameID)
        {
            Game game = new Game();
            string periodStartAsClockStringValueWithColon;

            try
            {
                game = BaseTableDataAccess.Instance().GetGameByGameID(gameID);

                BaseTableDataAccess.Instance().UpdateGameStatus(gameID, "IN PROGRESS");

                periodStartAsClockStringValueWithColon = GetPeriodStartAsClockStringValueWithColon(game, 1);
                DAL.Instance().UpdateGameCurrentPeriodAndCurrentClock(gameID, 1, periodStartAsClockStringValueWithColon);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        //Sets game to final, sets clock to 00:00 for clock down game, for clock up game it uses clock value passed in    
        public void EndGame(int gameID, string currentClock)
        {
            Game game = new Game();

            try
            {
                game = BaseTableDataAccess.Instance().GetGameByGameID(gameID);

                BaseTableDataAccess.Instance().UpdateGameStatus(gameID, "FINAL");

                if (game.ClockUpOrDown.ToUpper() == "DOWN")
                {
                    BaseTableDataAccess.Instance().UpdateGameCurrentClock(gameID, "00:00");
                }
                else
                {
                    BaseTableDataAccess.Instance().UpdateGameCurrentClock(gameID, currentClock);
                }

                UpdateWinningAndLosingTeams(game);
                Task.Factory.StartNew(() => StatCalculationsModule.CalculateALLPlayerMinutes(gameID));
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void UpdateWinningAndLosingTeams(Game game)
        {
            string homeTeamDecision;
            string awayTeamDecision;

            try
            {
                if (game.GameStatus.ToUpper() == "FINAL")
                {
                    if (game.HomeTeamScore > game.AwayTeamScore)
                    {
                        homeTeamDecision = "W";
                        awayTeamDecision = "L";
                    }
                    else if (game.AwayTeamScore > game.HomeTeamScore)
                    {
                        homeTeamDecision = "L";
                        awayTeamDecision = "W";
                    }
                    else //game is tied, if shootout then check shootout goals
                    {
                        if (game.HomeTeamShootOutGoals > game.AwayTeamShootOutGoals)
                        {
                            homeTeamDecision = "W";
                            awayTeamDecision = "L";
                        }
                        else if (game.AwayTeamShootOutGoals > game.HomeTeamShootOutGoals)
                        {
                            homeTeamDecision = "L";
                            awayTeamDecision = "W";
                        }
                        else
                        {
                            homeTeamDecision = "T";
                            awayTeamDecision = "T";
                        }
                    }

                    BaseTableDataAccess.Instance().UpdateGameTeamDecisions(game.GameID, homeTeamDecision, awayTeamDecision);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void SetHomeTeamSideOfField(int gameID, string sideOfField)
        {
            try
            {
                BaseTableDataAccess.Instance().UpdateHomeTeamSideOfField(gameID, sideOfField.ToUpper());

                //Now update away team to the other side
                if (sideOfField.ToUpper() == "LEFT")
                {
                    BaseTableDataAccess.Instance().UpdateAwayTeamSideOfField(gameID, "RIGHT");
                }
                else
                {
                    BaseTableDataAccess.Instance().UpdateAwayTeamSideOfField(gameID, "LEFT");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public int GetDemoGameGameID()
        {
            int returnResult = -1;
            try
            {
                var list = BaseTableDataAccess.Instance().GetAllGamesList();

                if (list != null)
                {
                    foreach (var gameEntry in list)
                    {
                        if (gameEntry.SampleData == "Y")
                        {
                            GameModel _game = new GameModel();
                            _game.Game = gameEntry;
                            returnResult = _game.Game.GameID;
                            break;
                        }
                    }
                }
                return returnResult;
            }
            catch (Exception ex)
            {
                return returnResult;
            }
        }

        #endregion "Game"

        #region "Play"

        //Note Substitution plays do not come here, they go to SaveSubstitutionPlay
        public void UpsertPlay(Play play, string whosCalling)
        {
            Play mostRecentPriorPlay = new Play();
            GameModel game = new GameModel();

            try
            {
                //Debug.WriteLine(DateTime.Now + " - Start - UpsertPlay. " + play.GameTime + " Playtext = " + play.PlayText);

                //Get game info as it is needed in Elapsed Time Calc, Sub Processing, Shot Processing, getting latest score to use for play score
                game = DAL.Instance().GetGame(play.GameID);

                //Based on the clock time, get the most recent play (i.e. play before this clock time) and the home and away play score and use for this play score  
                play.ElapsedTimeInSeconds = StatCalculationsModule.CalculateTimeElapsedInGameInSeconds(game.Game, play.Period, play.GameTime);

                mostRecentPriorPlay = DAL.Instance().GetPlaysForGamePriorToElapsedTime(play.GameID, play.ElapsedTimeInSeconds, play.PlayID);
                play.AwayScore = mostRecentPriorPlay.AwayScore;
                play.HomeScore = mostRecentPriorPlay.HomeScore;

                //Update the score in the game table and in the play table ((if this is not a goal play UpdateGameAndPlayScore will do nothing)               
                DAL.Instance().UpdateGameAndPlayScore(play);

                //Update game current period, current clock based upon this play, only do this for a new play not and edited play     
                if (play.ElapsedTimeInSeconds > game.Game.CurrentElapsedTimeInSeconds)
                {
                    DAL.Instance().UpdateGameCurrentPeriodAndCurrentClock(play.GameID, play.Period, play.GameTime);
                }

                //Now update the remaining part of the play (i.e. stats, plus minus if a goal, update running score if goal in the past)
                //If GM was adding the play, we can put this on a thread and return, if PBP screen was adding the play, we need to process the rest of the play on this thread 
                //before returning to the PBP screen
                if (whosCalling.ToUpper() == "GM")
                {
                    Task.Factory.StartNew(() => UpsertRemainingPartOfPlay(play, whosCalling));
                }
                else
                {
                    UpsertRemainingPartOfPlay(play, whosCalling);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //This saves the remaining part of the play (i.e. everything but the game score/time, i.e. stats, plus minus, running score if goal in past)
        public void UpsertRemainingPartOfPlay(Play play, string whosCalling)
        {
            GameModel game = new GameModel();

            try
            {
                game = DAL.Instance().GetGame(play.GameID);

                play.PlayText = Common.Instance().CreatePlayText(play);

                //Shot (1), Penalty Kick (16), Direct Free Kick (14), Corner Kick for goal (9/17)
                if (Common.Instance().IsThisAShotPlay(play) == true)
                {
                    Common.Instance().ShotProcessing(game, play);
                }
                else if (play.StatCategoryID == 21)   //Shootout Kick
                {
                    Common.Instance().ShootoutProcessing(game, play);
                }

                BaseTableDataAccess.Instance().UpsertPlay(play);

                //Goal (4), Own Goal (12), For Goal but not Shootout (17, !21),
                if (Common.Instance().IsThisAGCoalScoredPlay(play, true, false))
                {
                    Task.Factory.StartNew(() => StatCalculationsModule.AdjustPlusMinus(play, "ADD", whosCalling));
                }

                //Now we need to save this stat to the database
                UpsertStatPlayToFlatTotals(play, false);

                //if this was a goal in the past then we need to rebuld the running score                          
                if (Common.Instance().IsThisAGCoalScoredPlay(play, true, false) && (play.ElapsedTimeInSeconds < game.Game.CurrentElapsedTimeInSeconds))
                {
                    PBPModule.RebuildRunningScoreDueToScoringPlayChangedNEW(play, "ADD");
                }

                // Debug.WriteLine(DateTime.Now + " - End - UpsertPlay. " + play.GameTime + " Playtext = " + play.PlayText);
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        //public  void UpsertPlayOLD(Play play)
        //{
        //    GameModel game = new GameModel();
        //    Play mostRecentPriorPlay = new Play();
        //    int gameElapsedTimeInSeconds;

        //    try
        //    {
        //        //Get game info as it is needed in Elapsed Time Calc, Sub Processing, Shot Processing, getting latest score to use for play score
        //        game = DAL.Instance().GetGame(play.GameID);
        //        gameElapsedTimeInSeconds = StatCalculationsModule.CalculateTimeElapsedInGameInSeconds(game.Game, game.Game.CurrentPeriod, game.Game.CurrentClock);

        //        //Set some more play properties that were not set during stat selection              
        //        play.PlayText = Common.Instance().CreatePlayText(play);
        //        play.ElapsedTimeInSeconds = StatCalculationsModule.CalculateTimeElapsedInGameInSeconds(game.Game, play.Period, play.GameTime);

        //        //Based on the clock time, get the most recent play (i.e. play before this clock time) and the home and away play score and use for this play score    
        //        mostRecentPriorPlay = DAL.Instance().GetPlaysForGamePriorToElapsedTime(play.GameID, play.ElapsedTimeInSeconds);
        //        play.AwayScore = mostRecentPriorPlay.AwayScore;
        //        play.HomeScore = mostRecentPriorPlay.HomeScore;

        //        if (play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Substitution"))
        //        {
        //            if (play.ElapsedTimeInSeconds >= gameElapsedTimeInSeconds)
        //            {
        //                Common.Instance().SubstitutionProcessing(game, play);
        //            }

        //            //If the game has not started then all we want(ed) to do is in SubstitutionProcessing (i.e. update onfield and starter status)
        //            //we do not want to save this sub to the database as we do not want or need it for player minutes calculations
        //            if (game.Game.GameStatus == "NOT STARTED")
        //            {
        //                return;
        //            }
        //        }

        //        if ((play.StatCategoryID == 1) ||
        //            ((play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Penalty Kick"))))
        //        {
        //            Common.Instance().ShotProcessing(game, play);
        //        }

        //        if (play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Shootout Kick"))
        //        {
        //            Common.Instance().ShootoutProcessing(game, play);
        //        }

        //        if (play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Own Goal"))
        //        {
        //            //Update the score in the game table and in the play table              
        //            DAL.Instance().UpdateGameAndPlayScore(play);
        //        }

        //        BaseTableDataAccess.UpsertPlay(play);

        //        //Now we need to save this stat to the database
        //        UpsertStatPlayToStatTotals(play, false);

        //        //Update game current period, current clock based upon this play,only do this for a new play not and edited play     
        //        if (play.ElapsedTimeInSeconds > gameElapsedTimeInSeconds)
        //        {
        //            BaseTableDataAccess.UpdateGameCurrentPeriodAndCurrentClock(play.GameID, play.Period, play.GameTime);
        //        }

        //        //if this was a goal in the past then we need to rebuld the running score
        //        if (((DAL.Instance().GetStatDescriptionNameById(play.StatDescriptionID) == "Goal") || play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Own Goal"))
        //            && (play.ElapsedTimeInSeconds < gameElapsedTimeInSeconds))
        //        {
        //            PBPModule.RebuildRunningScoreDueToScoringPlayerChanged(play.GameID);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error upserting play. Error = " + ex.Message);
        //    }
        //}

        //public static void UpsertPlayFromPBPEditScreenOLD(Play play)
        //{
        //    GameModel game = new GameModel();
        //    Play mostRecentPriorPlay = new Play();
        //    try
        //    {
        //        game = DAL.Instance().GetGame(play.GameID);

        //        //Set some more play properties that were not set during stat selection              
        //        play.PlayText = Common.Instance().CreatePlayText(play);
        //        play.ElapsedTimeInSeconds = StatCalculationsModule.CalculateTimeElapsedInGameInSeconds(game.Game, play.Period, play.GameTime);

        //        //Based on the clock time, get the most recent play (i.e. play before this clock time) and the home and away play score and use for this play score    
        //        mostRecentPriorPlay = DAL.Instance().GetPlaysForGamePriorToElapsedTime(play.GameID, play.ElapsedTimeInSeconds);
        //        play.AwayScore = mostRecentPriorPlay.AwayScore;
        //        play.HomeScore = mostRecentPriorPlay.HomeScore;

        //        BaseTableDataAccess.UpsertPlay(play);

        //        //Now we need to save this stat to the database
        //        UpsertStatPlayToStatTotals(play, false);

        //        if (DAL.Instance().GetStatDescriptionNameById(play.StatDescriptionID) == "Goal")
        //        {
        //            //If this is a goal need to adjust the play home and away score (i.e. running score), plus minus  
        //            PBPModule.GoalWasAddedOrDeleted(play.GameID, play.TeamID, "ADDED");
        //            // PBPModule.RebuildPlusMinusAndPlayScoresFromPBPDueToScoringPlayChanged(play.GameID);
        //        }
        //        else if (DAL.Instance().GetStatCategoryNameById(play.StatCategoryID) == "Substitution")
        //        {
        //            //If this is a sub we need to recalc plus minus and minutes played
        //            //we are no longer calculating plus minus OR player minutes as subs happen, only when needed.
        //            //  PBPModule.RebuildPlusMinusAndMinutesPlayedFromPBPDueToSubstitutionPlayChanged(play.GameID);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

        //public static void UpsertPlayFromGameManagerOLD(Play play)
        //{
        //    GameModel game = new GameModel();
        //    Play mostRecentPriorPlay = new Play();

        //    try
        //    {
        //        //Get game info as it is needed in Elapsed Time Calc, Sub Processing, Shot Processing, getting latest score to use for play score
        //        game = DAL.Instance().GetGame(play.GameID);

        //        //Set some more play properties that were not set during stat selection              
        //        play.PlayText = Common.Instance().CreatePlayText(play);
        //        play.ElapsedTimeInSeconds = StatCalculationsModule.CalculateTimeElapsedInGameInSeconds(game.Game, play.Period, play.GameTime);

        //        //Based on the clock time, get the most recent play (i.e. play before this clock time) and the home and away play score and use for this play score    
        //        mostRecentPriorPlay = DAL.Instance().GetPlaysForGamePriorToElapsedTime(play.GameID, play.ElapsedTimeInSeconds);
        //        play.AwayScore = mostRecentPriorPlay.AwayScore;
        //        play.HomeScore = mostRecentPriorPlay.HomeScore;

        //        if (play.StatCategoryID == 22)
        //        {
        //            Common.Instance().SubstitutionProcessing(game, play);

        //            //If the game has not started then all we want(ed) to do is in SubstitutionProcessing (i.e. update onfield and starter status)
        //            //we do not want to save this sub to the database as we do not want or need it for player minutes calculations
        //            if (game.Game.GameStatus == "NOT STARTED")
        //            {
        //                return;
        //            }
        //        }

        //        if ((play.StatCategoryID == 1) || ((play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Shootout Kick")) || ((play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("Penalty Kick")))))
        //        {
        //            Common.Instance().ShotProcessing(game, play);
        //        }

        //        BaseTableDataAccess.UpsertPlay(play);

        //        //FYI, we do not save left, right, or headed shot info, this is only saved as details in the play (in ShotType field)

        //        //Now we need to save this stat to the database
        //        UpsertStatPlayToStatTotals(play, false);

        //        //Update game current period, current clock based upon this play        
        //        BaseTableDataAccess.UpdateGameCurrentPeriodAndCurrentClock(play.GameID, play.Period, play.GameTime);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error upserting play. Error = " + ex.Message);
        //    }
        //}

        //This is identical to delete play, with one exception.  This gets the most recent play in the game (based upon elapsed time) and deletes it (as opposed to deleting a specific play)
        //AND in addition to deleting the play, IF it is a SUBSTITUTION play, will put the players involved back in the game and on the bench appropriately
        //i.e. if undoING Bob in, Bill out...then Bill is put back in the game and Bob is put back out/on the bench.

        //This routine also will reset the game current period and current clock back to the period/clock of the new most recent play
        public string UndoLastPlay(int gameID)
        {
            string lastPlayText = string.Empty;
            ObservableCollection<PlayModel> playList = new ObservableCollection<PlayModel>();

            try
            {
                //Get all the plays, order so the most recent play is the first play in the list 
                playList = DAL.Instance().GetPlaysForGame(gameID, "DESC");

                //only going to process the first play in this list, which is the most recent play               
                foreach (var playEntry in playList)
                {
                    lastPlayText = playEntry.Play.Period + "/" + playEntry.Play.GameTime + ": " + playEntry.Play.PlayText;
                    DAL.Instance().DeletePlay(gameID, playEntry.Play.PlayID);

                    //if this is a substitution play then want to put the players back in and back out of the game
                    if (playEntry.Play.StatCategoryID == 22)    //Substitution (22)
                    {
                        Common.Instance().SubstitutionProcessing(gameID, playEntry.Play, true);
                    }

                    if (playEntry.Play.StatCategoryID == 33)    //Move (33)
                    {
                        //Update players GM position
                        BaseTableDataAccess.Instance().UpdatePlayersGMPlayerPositionID(playEntry.Play.GameID, playEntry.Play.TeamID, playEntry.Play.Player1ID, playEntry.Play.GMPlayer1PositionID);
                        BaseTableDataAccess.Instance().UpdatePlayersGMPlayerPositionID(playEntry.Play.GameID, playEntry.Play.TeamID, playEntry.Play.Player2ID, playEntry.Play.GMPlayer2PositionID);
                    }

                    //leave now, only wanted to delete most recent play
                    break;
                }

                // We (Mark) decided we would NOT update the clock when the user clicks undo to delete last play.

                ////Get the new most recent play and update the current period and current clock to that play
                //playList = DAL.Instance().GetPlaysForGame(gameID, "DESC");

                ////only going to process the first play in this list, which is the most recent play               
                //foreach (var playEntry in playList)
                //{
                //    DAL.Instance().UpdateGameCurrentPeriodAndCurrentClock(gameID, playEntry.Play.Period, playEntry.Play.GameTime);
                //    break;
                //}

                return lastPlayText;

            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
        }

        public void BackOutStatsForAPlay(int gameID, Play playDetails)
        {
            int otherTeamID;

            try
            {
                //If backing out a scoring play we need to recalculate the plus minus (we include own goals but not shootout goals as those do not impact the game score
                if (Common.Instance().IsThisAGCoalScoredPlay(playDetails, true, false))
                {
                    Task.Factory.StartNew(() => StatCalculationsModule.AdjustPlusMinus(playDetails, "DELETE", "PBPSCREEN"));
                }

                otherTeamID = Common.Instance().GetOtherTeamID(playDetails.GameID, playDetails.TeamID);

                //need to back out the opportunity and the goal if it applicable from the game table
                if (playDetails.StatCategoryID == 21)   //Shootout Kick (21)
                {
                    if (playDetails.StatDescriptionID == 4) //Goal (4)
                    {
                        PBPModule.ShootOutGoalWasDeletedBackOutGoalFromGameShootoutScore(gameID, playDetails.TeamID);
                    }

                    PBPModule.BackOutTeamShootOutOpp(gameID, playDetails.TeamID);

                    //Back out the shot on goal
                    if (playDetails.ShotOnGoal == "Y")
                    {
                        DAL.Instance().UpsertFlatTotalsStat(playDetails.GameID, playDetails.Player1ID, playDetails.TeamID, "Shot On Goal", true);
                    }

                    DAL.Instance().UpsertStatPlayToFlatTotals(playDetails, true);
                    return;
                }

                //If this is a goal play being deleted need to back out goal scored, goal allowed, assist, shot on goal then rebuild running score
                if (Common.Instance().IsThisAGCoalScoredPlay(playDetails, true, false))
                {
                    //Back out the goal scored stat
                    DAL.Instance().UpsertStatPlayToFlatTotals(playDetails, true);

                    //Back out the goal allowed stats (do not include own goals or shootout goals)
                    if (Common.Instance().IsThisAGCoalScoredPlay(playDetails, false, false) && (playDetails.OtherTeamGoalieID != null))
                    {
                        DAL.Instance().UpsertFlatTotalsStat(playDetails.GameID, playDetails.OtherTeamGoalieID, otherTeamID, "Goal Allowed", true);
                    }

                    //Back out the assist (if there was an assist)
                    if (playDetails.AssistID != null)
                    {
                        DAL.Instance().UpsertFlatTotalsStat(playDetails.GameID, playDetails.AssistID, playDetails.TeamID, "Assist", true);
                    }

                    //Back out the shot on goal
                    if (playDetails.ShotOnGoal == "Y")
                    {
                        DAL.Instance().UpsertFlatTotalsStat(playDetails.GameID, playDetails.Player1ID, playDetails.TeamID, "Shot On Goal", true);
                    }

                    if (playDetails.StatCategoryID == 12)   //Own Goal (12)
                    {
                        PBPModule.GoalWasDeletedBackOutGoalFromGameScoreAndUpdatePBPRunningScore(playDetails, otherTeamID);
                    }
                    else
                    {
                        PBPModule.GoalWasDeletedBackOutGoalFromGameScoreAndUpdatePBPRunningScore(playDetails, playDetails.TeamID);
                    }
                }
                else //NO goal scored Need to simply back out the stat, since no goal scored no other plays are impacted (i.e. play home and away score values)
                {
                    //Back out the stat
                    DAL.Instance().UpsertStatPlayToFlatTotals(playDetails, true);

                    //If shot then check to see if shot on goal, save, blocked need to also be backed out
                    if (Common.Instance().IsThisAShotPlay(playDetails) == true)
                    {
                        if (playDetails.ShotOnGoal == "Y")
                        {
                            DAL.Instance().UpsertFlatTotalsStat(playDetails.GameID, playDetails.Player1ID, playDetails.TeamID, "Shot On Goal", true);
                        }

                        if (playDetails.ShotBlockedByID != null)
                        {
                            string statName;
                            if (playDetails.ShotOnGoal == "Y") { statName = "Save"; } else { statName = "Blocked"; }

                            DAL.Instance().UpsertFlatTotalsStat(playDetails.GameID, playDetails.ShotBlockedByID, otherTeamID, statName, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting play. Error = " + ex.Message);
            }
        }

        public void DeletePlay(int gameID, int playID)
        {
            Play playDetails = new Play();

            try
            {
                playDetails = BaseTableDataAccess.Instance().GetPlay(gameID, playID);

                //Debug.WriteLine(DateTime.Now + " - Start - DeletePlay. Playtext = " + playDetails.PlayText);

                //Delete the play
                BaseTableDataAccess.Instance().DeletePlay(gameID, playID);

                //Back out the stats for the play
                BackOutStatsForAPlay(gameID, playDetails);

                //Debug.WriteLine(DateTime.Now + " - End - DeletePlay. Playtext = " + playDetails.PlayText);


            }
            catch (Exception ex)
            {
            }
        }

        public ObservableCollection<PlayModel> GetPlaysForGame(int gameID, string sortByElapsedTimeAscOrDesc)
        {
            ObservableCollection<PlayModel> plays = new ObservableCollection<PlayModel>();
            Game game = new Game();

            try
            {
                game = BaseTableDataAccess.Instance().GetGameByGameID(gameID);

                var list = BaseTableDataAccess.Instance().GetAllPlaysForGame(gameID, sortByElapsedTimeAscOrDesc);

                if (list != null)
                {
                    foreach (var playEntry in list)
                    {
                        PlayModel playModel = new PlayModel();
                        playModel.Play = playEntry;
                        plays.Add(playModel);
                    }
                }
                return plays;
            }
            catch (Exception ex)
            {
                return plays;
            }
        }

        public ObservableCollection<PlayModel> GetPlaysForGameWithPlayerStatObjectFilledIn(int gameID, string sortByElapsedTimeAscOrDesc)
        {
            ObservableCollection<PlayModel> plays = new ObservableCollection<PlayModel>();
            Game game = new Game();

            try
            {
                game = BaseTableDataAccess.Instance().GetGameByGameID(gameID);

                var list = BaseTableDataAccess.Instance().GetAllPlaysForGame(gameID, sortByElapsedTimeAscOrDesc);

                if (list != null)
                {
                    foreach (var playEntry in list)
                    {
                        PlayModel playModel = new PlayModel();
                        playModel.Play = playEntry;
                        playModel.Player1 = GetPlayer(playEntry.Player1ID);
                        playModel.StatCategory.StatCategoryName = BaseTableDataAccess.Instance().GetStatCategoryByID(playEntry.StatCategoryID).StatCategoryName;
                        playModel.StatDescription.StatDescriptionName = GetStatDescriptionNameById(playEntry.StatDescriptionID);
                        plays.Add(playModel);
                    }
                }
                return plays;
            }
            catch (Exception ex)
            {
                return plays;
            }
        }

        public Play GetPlaysForGamePriorToElapsedTime(int gameID, int elapsedTimeInSeconds, int playID)
        {
            Play mostRecentPlay = new Play();

            try
            {
                var list = BaseTableDataAccess.Instance().GetPlaysForGamePriorToElapsedTime(gameID, elapsedTimeInSeconds);

                if (list != null)
                {
                    foreach (var playEntry in list)
                    {
                        if ((playEntry.ElapsedTimeInSeconds == elapsedTimeInSeconds) && (playEntry.PlayID > playID) && (playID != 0))
                        {
                            // This is a play with the same time and it is a play that happend after in terms of user entry so ignore this play
                            //and move on to the next one (i.e playID = 0) this is a new play so can take most recent even if at the same time
                        }
                        else
                        {
                            mostRecentPlay = playEntry;
                            break;
                        }
                    }
                }
                return mostRecentPlay;
            }
            catch (Exception ex)
            {
                return mostRecentPlay;
            }
        }

        public ObservableCollection<Play> GetSUBPlaysForGamePriorToElapsedTime(int gameID, int teamID, int elapsedTimeInSeconds)
        {
            ObservableCollection<Play> subPlays = new ObservableCollection<Play>();

            try
            {
                var list = BaseTableDataAccess.Instance().GetPlaysForGamePriorToElapsedTime(gameID, elapsedTimeInSeconds);

                if (list != null)
                {
                    foreach (var playEntry in list)
                    {
                        if ((playEntry.StatCategoryID == 22) && (playEntry.TeamID == teamID))
                        {
                            subPlays.Add(playEntry);
                        }
                    }
                }
                return subPlays;
            }
            catch (Exception ex)
            {
                return subPlays;
            }
        }

        public void UpsertStatPlayToFlatTotals(Play play, bool areWeBackingOutTheStat)
        {
            int statValue = 0;
            string flatTotalsColumn = string.Empty;
            FlatTotals playerFlatTotals = new FlatTotals();

            try
            {
                flatTotalsColumn = Common.Instance().GetFlatTotalsColumnName(play.StatCategoryID, play.StatDescriptionID);

                //Substitution plays do not have stats
                if (flatTotalsColumn == string.Empty)
                {
                    return;
                }

                playerFlatTotals = BaseTableDataAccess.Instance().GetPlayersFlatTotalsRecord(play.GameID, play.TeamID, play.Player1ID);

                statValue = Common.Instance().GetFlatTotalsColumnValue(playerFlatTotals, flatTotalsColumn);

                if (statValue != 0)
                {
                    //PlusMinus stat gets summed differently, add 1 if plus, subtract 1 if minus
                    if (play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("PlusMinus"))
                    {
                        if (play.PlayText == "Plus") { statValue = statValue + 1; } else { statValue = statValue - 1; }
                    }
                    else
                    {
                        if (areWeBackingOutTheStat == true) { statValue = statValue - 1; } else { statValue = statValue + 1; }
                    }
                }
                else  //game, player, stat combo does not exist in table, so set stat value to 1
                {
                    //PlusMinus stat gets initialized differently, 1 is plus, -1 if minus
                    if (play.StatCategoryID == DAL.Instance().GetStatCategoryIDByName("PlusMinus"))
                    {
                        if (play.PlayText == "Plus") { statValue = 1; } else { statValue = -1; }
                    }
                    else //should never get here if we are backing out a stat as it has to already exist to back out
                    {
                        statValue = 1;
                    }
                }

                UpsertFlatTotals(play.GameID, play.TeamID, play.Player1ID, play.StatCategoryID, play.StatDescriptionID, statValue);
            }
            catch (Exception ex)
            {
            }
        }

        //This proc does not need to be translated as we are querying the database for stat cat name and descriptions
        public void UpsertFlatTotals(int gameID, int teamID, int playerID, int statCategoryID, int? statCategoryDescription, int statValue)
        {
            int totalGoals;
            int totalShots;
            string categoryName;
            string categoryDescription;
            string flatTotalsColumnToUpdate = string.Empty;
            FlatTotals flatTotals = new FlatTotals();
            FlatTotals playerFlatTotals = new FlatTotals();

            try
            {
                //If player does not have a flat totals record, need to create one first (they will not have one until the first stat for them is saved)
                playerFlatTotals = BaseTableDataAccess.Instance().GetPlayersFlatTotalsRecord(gameID, teamID, playerID);
                if (playerFlatTotals == null)
                {
                    flatTotals.GameID = gameID;
                    flatTotals.TeamID = teamID;
                    flatTotals.PlayerID = playerID;
                    BaseTableDataAccess.Instance().InsertFlatTotals(flatTotals);
                }

                //categoryName = BaseTableDataAccess.GetStatCategoryByID(statEntry.StatCategoryID).StatCategoryName;
                categoryName = DAL.Instance().GetStatCategoryNameById(statCategoryID);
                categoryDescription = DAL.Instance().GetStatDescriptionNameById(statCategoryDescription);

                switch (categoryName)
                {
                    case "Shot":
                        if (categoryDescription == "Miss") { flatTotalsColumnToUpdate = "ShotMiss"; }
                        else if (categoryDescription == "Hit Post") { flatTotalsColumnToUpdate = "ShotHitPost"; }
                        else if (categoryDescription == "Blocked") { flatTotalsColumnToUpdate = "ShotBlocked"; }
                        else if (categoryDescription == "Goal") { flatTotalsColumnToUpdate = "ShotGoal"; }

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);

                        //Sum up high level stats for shots (miss, hitpost, blocked, goal).  Also need to sum up Goals, if this is a goal play.
                        playerFlatTotals = BaseTableDataAccess.Instance().GetPlayersFlatTotalsRecord(gameID, teamID, playerID);

                        totalShots = playerFlatTotals.ShotMiss + playerFlatTotals.ShotHitPost + playerFlatTotals.ShotBlocked + playerFlatTotals.ShotGoal +
                            playerFlatTotals.PenaltyKickMiss + playerFlatTotals.PenaltyKickHitPost + playerFlatTotals.PenaltyKickBlocked + playerFlatTotals.PenaltyKickGoal +
                            playerFlatTotals.CornerKickForGoal + playerFlatTotals.DirectFreeKickForGoal;

                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "ShotTotal", totalShots);

                        //if this was a goal need to also total up goals (not counting shootoutgoals in goal count)
                        if (categoryDescription == "Goal")
                        {
                            totalGoals = playerFlatTotals.ShotGoal + playerFlatTotals.CornerKickForGoal + playerFlatTotals.DirectFreeKickForGoal + playerFlatTotals.PenaltyKickGoal;
                            BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "TotalGoals", totalGoals);
                        }
                        break;
                    case "Pass":
                        if (categoryDescription == "Excellent") { flatTotalsColumnToUpdate = "PassExcellent"; }
                        else if (categoryDescription == "Good") { flatTotalsColumnToUpdate = "PassGood"; }
                        else if (categoryDescription == "Poor") { flatTotalsColumnToUpdate = "PassPoor"; }

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);

                        //Sum up high level stats for passes (excellent, good, poor).  
                        playerFlatTotals = BaseTableDataAccess.Instance().GetPlayersFlatTotalsRecord(gameID, teamID, playerID);

                        int totalPasses = playerFlatTotals.PassExcellent + playerFlatTotals.PassGood + playerFlatTotals.PassPoor;
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "PassTotal", totalPasses);
                        break;
                    case "Turnover":
                        if (categoryDescription == "Illegal Throw In") { flatTotalsColumnToUpdate = "TurnoverIllegalThrowIn"; }
                        else if (categoryDescription == "Lost Dribble") { flatTotalsColumnToUpdate = "TurnoverLostDribble"; }

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);

                        //Sum up high level stats for turnovers (illegal throw in, lost dribble).  
                        playerFlatTotals = BaseTableDataAccess.Instance().GetPlayersFlatTotalsRecord(gameID, teamID, playerID);

                        int totalTurnovers = playerFlatTotals.TurnoverIllegalThrowIn + playerFlatTotals.TurnoverLostDribble;
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "TurnoverTotal", totalTurnovers);
                        break;
                    case "Offsides":
                        flatTotalsColumnToUpdate = "OffsidesTotal";

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);
                        break;
                    case "Foul Committed":
                        if (categoryDescription == "Charging") { flatTotalsColumnToUpdate = "FoulCommittedCharging"; }
                        else if (categoryDescription == "Holding") { flatTotalsColumnToUpdate = "FoulCommittedHolding"; }
                        else if (categoryDescription == "Illegal Tackle") { flatTotalsColumnToUpdate = "FoulCommittedIllegalTackle"; }
                        else if (categoryDescription == "Kicking") { flatTotalsColumnToUpdate = "FoulCommittedKicking"; }
                        else if (categoryDescription == "Pushing") { flatTotalsColumnToUpdate = "FoulCommittedPushing"; }
                        else if (categoryDescription == "Tripping") { flatTotalsColumnToUpdate = "FoulCommittedTripping"; }

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);

                        //Sum up high level stats for foul committed (kicking, charging...).  
                        playerFlatTotals = BaseTableDataAccess.Instance().GetPlayersFlatTotalsRecord(gameID, teamID, playerID);

                        int totalFoulCommitted = playerFlatTotals.FoulCommittedCharging + playerFlatTotals.FoulCommittedHolding + playerFlatTotals.FoulCommittedIllegalTackle +
                            playerFlatTotals.FoulCommittedKicking + playerFlatTotals.FoulCommittedPushing + playerFlatTotals.FoulCommittedTripping;
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "FoulCommittedTotal", totalFoulCommitted);
                        break;
                    case "Out Of Bounds":
                        flatTotalsColumnToUpdate = "OutOfBoundsTotal";

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);
                        break;
                    case "Cross":
                        if (categoryDescription == "Excellent") { flatTotalsColumnToUpdate = "CrossExcellent"; }
                        else if (categoryDescription == "Good") { flatTotalsColumnToUpdate = "CrossGood"; }
                        else if (categoryDescription == "Poor") { flatTotalsColumnToUpdate = "CrossPoor"; }

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);

                        //Sum up high level stats for crosses (excellent, good, poor).  
                        playerFlatTotals = BaseTableDataAccess.Instance().GetPlayersFlatTotalsRecord(gameID, teamID, playerID);

                        int totalCrosses = playerFlatTotals.CrossExcellent + playerFlatTotals.CrossGood + playerFlatTotals.CrossPoor;
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "CrossTotal", totalCrosses);
                        break;
                    case "Throw In":
                        flatTotalsColumnToUpdate = "ThrowInTotal";

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);
                        break;
                    case "Corner Kick":
                        if (categoryDescription == "Excellent") { flatTotalsColumnToUpdate = "CornerKickExcellent"; }
                        else if (categoryDescription == "Good") { flatTotalsColumnToUpdate = "CornerKickGood"; }
                        else if (categoryDescription == "Poor") { flatTotalsColumnToUpdate = "CornerKickPoor"; }
                        else if (categoryDescription == "For Goal") { flatTotalsColumnToUpdate = "CornerKickForGoal"; }

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);

                        //Sum up high level stats for corner kicks (excellent, good, poor, for goal).  Also need to sum up Goals, if this is a goal play.
                        playerFlatTotals = BaseTableDataAccess.Instance().GetPlayersFlatTotalsRecord(gameID, teamID, playerID);

                        int totalCornerKicks = playerFlatTotals.CornerKickExcellent + playerFlatTotals.CornerKickGood + playerFlatTotals.CornerKickPoor + playerFlatTotals.CornerKickForGoal;
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "CornerKickTotal", totalCornerKicks);

                        totalShots = playerFlatTotals.ShotMiss + playerFlatTotals.ShotHitPost + playerFlatTotals.ShotBlocked + playerFlatTotals.ShotGoal +
                         playerFlatTotals.PenaltyKickMiss + playerFlatTotals.PenaltyKickHitPost + playerFlatTotals.PenaltyKickBlocked + playerFlatTotals.PenaltyKickGoal +
                         playerFlatTotals.CornerKickForGoal + playerFlatTotals.DirectFreeKickForGoal;

                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "ShotTotal", totalShots);

                        //if this was a goal need to also total up goals (not counting shootoutgoals in goal count)
                        if (categoryDescription == "For Goal")
                        {
                            totalGoals = playerFlatTotals.ShotGoal + playerFlatTotals.CornerKickForGoal + playerFlatTotals.DirectFreeKickForGoal + playerFlatTotals.PenaltyKickGoal;
                            BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "TotalGoals", totalCornerKicks);
                        }
                        break;
                    case "Tackle":
                        flatTotalsColumnToUpdate = "TackleTotal";

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);
                        break;
                    case "Goalie Kick":
                        flatTotalsColumnToUpdate = "GoalieKickTotal";

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);
                        break;
                    case "Own Goal":
                        flatTotalsColumnToUpdate = "OwnGoalTotal";

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);
                        break;
                    case "Foul Drawn":
                        flatTotalsColumnToUpdate = "FoulDrawnTotal";

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);
                        break;
                    case "Direct Free Kick":
                        if (categoryDescription == "Not For Goal") { flatTotalsColumnToUpdate = "DirectFreeKickNotForGoal"; }
                        else if (categoryDescription == "For Goal") { flatTotalsColumnToUpdate = "DirectFreeKickForGoal"; }

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);

                        //Sum up high level stats for direct free kicks (miss, hitpost, blocked, goal).  Also need to sum up Goals, if this is a goal play.
                        playerFlatTotals = BaseTableDataAccess.Instance().GetPlayersFlatTotalsRecord(gameID, teamID, playerID);

                        int totalDirectFreeKickShots = playerFlatTotals.DirectFreeKickNotForGoal + playerFlatTotals.DirectFreeKickForGoal;
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "DirectFreeKickTotal", totalDirectFreeKickShots);

                        totalShots = playerFlatTotals.ShotMiss + playerFlatTotals.ShotHitPost + playerFlatTotals.ShotBlocked + playerFlatTotals.ShotGoal +
                         playerFlatTotals.PenaltyKickMiss + playerFlatTotals.PenaltyKickHitPost + playerFlatTotals.PenaltyKickBlocked + playerFlatTotals.PenaltyKickGoal +
                         playerFlatTotals.CornerKickForGoal + playerFlatTotals.DirectFreeKickForGoal;

                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "ShotTotal", totalShots);

                        //if this was a goal need to also total up goals (not counting shootoutgoals in goal count)
                        if (categoryDescription == " For Goal")
                        {
                            totalGoals = playerFlatTotals.ShotGoal + playerFlatTotals.CornerKickForGoal + playerFlatTotals.DirectFreeKickForGoal + playerFlatTotals.PenaltyKickGoal;
                            BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "TotalGoals", totalGoals);
                        }
                        break;
                    case "Indirect Free Kick":
                        flatTotalsColumnToUpdate = "IndirectFreeKickTotal";

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);
                        break;
                    case "Penalty Kick":
                        if (categoryDescription == "Miss") { flatTotalsColumnToUpdate = "PenaltyKickMiss"; }
                        else if (categoryDescription == "Hit Post") { flatTotalsColumnToUpdate = "PenaltyKickHitPost"; }
                        else if (categoryDescription == "Blocked") { flatTotalsColumnToUpdate = "PenaltyKickBlocked"; }
                        else if (categoryDescription == "Goal") { flatTotalsColumnToUpdate = "PenaltyKickGoal"; }

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);

                        //Sum up high level stats for penalty free kicks (miss, hitpost, blocked, goal).  Also need to sum up Goals, if this is a goal play.
                        playerFlatTotals = BaseTableDataAccess.Instance().GetPlayersFlatTotalsRecord(gameID, teamID, playerID);

                        int totalPenaltyKickShots = playerFlatTotals.PenaltyKickMiss + playerFlatTotals.PenaltyKickHitPost + playerFlatTotals.PenaltyKickBlocked + playerFlatTotals.PenaltyKickGoal;
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "PenaltyKickTotal", totalPenaltyKickShots);

                        totalShots = playerFlatTotals.ShotMiss + playerFlatTotals.ShotHitPost + playerFlatTotals.ShotBlocked + playerFlatTotals.ShotGoal +
                         playerFlatTotals.PenaltyKickMiss + playerFlatTotals.PenaltyKickHitPost + playerFlatTotals.PenaltyKickBlocked + playerFlatTotals.PenaltyKickGoal +
                         playerFlatTotals.CornerKickForGoal + playerFlatTotals.DirectFreeKickForGoal;

                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "ShotTotal", totalShots);

                        //if this was a goal need to also total up goals (not counting shootoutgoals in goal count)
                        if (categoryDescription == "Goal")
                        {
                            totalGoals = playerFlatTotals.ShotGoal + playerFlatTotals.CornerKickForGoal + playerFlatTotals.DirectFreeKickForGoal + playerFlatTotals.PenaltyKickGoal;
                            BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "TotalGoals", totalGoals);
                        }
                        break;
                    case "Yellow Card":
                        if (categoryDescription == "Unsportsmanlike Conduct") { flatTotalsColumnToUpdate = "YellowCardUnsportsmanLikeConduct"; }
                        else if (categoryDescription == "Delaying Restart Of Play") { flatTotalsColumnToUpdate = "YellowCardDelayingRestartOfPlay"; }

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);

                        //Sum up high level stats for yellow card (unsportsmanlike conduct, delaying restart of play).  
                        playerFlatTotals = BaseTableDataAccess.Instance().GetPlayersFlatTotalsRecord(gameID, teamID, playerID);

                        int totalYellowCard = playerFlatTotals.YellowCardDelayingRestartOfPlay + playerFlatTotals.YellowCardUnsportsmanLikeConduct;
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "YellowCardTotal", totalYellowCard);
                        break;
                    case "Red Card":
                        if (categoryDescription == "Foul Play") { flatTotalsColumnToUpdate = "RedCardFoulPlay"; }
                        else if (categoryDescription == "Violent Conduct") { flatTotalsColumnToUpdate = "RedCardIllegalHands"; }
                        else if (categoryDescription == "Illegal Hands") { flatTotalsColumnToUpdate = "RedCardSecondYellowCard"; }
                        else if (categoryDescription == "Second Yellow Card") { flatTotalsColumnToUpdate = "RedCardViolentConduct"; }

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);

                        //Sum up high level stats for red card (foul play, illegal hands...).  
                        playerFlatTotals = BaseTableDataAccess.Instance().GetPlayersFlatTotalsRecord(gameID, teamID, playerID);

                        int totalRedCard = playerFlatTotals.RedCardFoulPlay + playerFlatTotals.RedCardIllegalHands + playerFlatTotals.RedCardSecondYellowCard + playerFlatTotals.RedCardViolentConduct;
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "RedCardTotal", totalRedCard);
                        break;
                    case "Drop Kick":
                        if (categoryDescription == "Excellent") { flatTotalsColumnToUpdate = "DropKickExcellent"; }
                        else if (categoryDescription == "Good") { flatTotalsColumnToUpdate = "DropKickGood"; }
                        else if (categoryDescription == "Poor") { flatTotalsColumnToUpdate = "DropKickPoor"; }

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);

                        //Sum up high level stats for drop kick (excellent, good, poor).  
                        playerFlatTotals = BaseTableDataAccess.Instance().GetPlayersFlatTotalsRecord(gameID, teamID, playerID);

                        int totalDropKick = playerFlatTotals.DropKickExcellent + playerFlatTotals.DropKickGood + playerFlatTotals.DropKickPoor;
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "DropKickTotal", totalDropKick);
                        break;
                    case "Dribble":
                        flatTotalsColumnToUpdate = "DribbleTotal";

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);
                        break;
                    case "Shootout Kick":
                        if (categoryDescription == "Miss") { flatTotalsColumnToUpdate = "ShootoutKickMiss"; }
                        else if (categoryDescription == "Hit Post") { flatTotalsColumnToUpdate = "ShootoutKickHitPost"; }
                        else if (categoryDescription == "Blocked") { flatTotalsColumnToUpdate = "ShootoutKickBlocked"; }
                        else if (categoryDescription == "Goal") { flatTotalsColumnToUpdate = "ShootoutKickGoal"; }

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);

                        //Sum up high level stats for shootout kicks (miss, hitpost, blocked, goal).  
                        playerFlatTotals = BaseTableDataAccess.Instance().GetPlayersFlatTotalsRecord(gameID, teamID, playerID);

                        int totalShootoutShots = playerFlatTotals.ShootoutKickMiss + playerFlatTotals.ShootoutKickHitPost + playerFlatTotals.ShootoutKickBlocked + playerFlatTotals.ShootoutKickGoal;
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "ShootoutKickTotal", totalShootoutShots);
                        break;
                    case "Substitution":
                        break;
                    case "Move":
                        break;
                    case "Clock":
                        break;
                    case "SecondsPlayed":
                        flatTotalsColumnToUpdate = "SecondsPlayedTotal";

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);
                        break;
                    case "MinutesPlayed":
                        flatTotalsColumnToUpdate = "MinutesPlayedTotal";

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);
                        break;
                    case "PlusMinus":
                        flatTotalsColumnToUpdate = "PlusMinusTotal";

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);
                        break;
                    case "Blocked":
                        flatTotalsColumnToUpdate = "BlockedTotal";

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);
                        break;
                    case "Goal Allowed":
                        flatTotalsColumnToUpdate = "GoalAllowedTotal";

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);
                        break;
                    case "Save":
                        flatTotalsColumnToUpdate = "SaveTotal";

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);
                        break;
                    case "Shot On Goal":
                        flatTotalsColumnToUpdate = "ShotOnGoalTotal";

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);
                        break;
                    case "Assist":
                        flatTotalsColumnToUpdate = "AssistTotal";

                        //save this stat to flat table
                        BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, flatTotalsColumnToUpdate, statValue);
                        break;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public PlayModel GetPlay(int gameID, int playID)
        {
            PlayModel _play = new PlayModel();

            try
            {
                var selectedPlay = BaseTableDataAccess.Instance().GetPlay(gameID, playID);

                if (selectedPlay != null)
                {
                    _play.Play = selectedPlay;
                }

                return _play;
            }
            catch (Exception ex)
            {
                return _play;
            }
        }

        public void SaveSubstitutionPlay(string whosCalling, int gameID, int teamID, int period, string gameTime, int playerINID, string gmPlayerINPositionID, int? playerOUTID, string gmPlayerOUTPositionID)
        {
            Play play = new Play();
            GameModel game = new GameModel();
            Play mostRecentPriorPlay = new Play();
            int gameElapsedTimeInSeconds;

            try
            {
                //Set the play info for this substitution
                play.GameID = gameID;
                play.TeamID = teamID;
                play.Period = period;
                play.GameTime = gameTime;
                play.Player1ID = playerINID;
                play.Player2ID = playerOUTID;
                play.GMPlayer1PositionID = gmPlayerINPositionID;
                play.GMPlayer2PositionID = gmPlayerOUTPositionID;
                play.StatCategoryID = 22;
                play.StatDescriptionID = 0;
                play.OtherTeamGoalieID = 0;

                //Get game info as it is needed in Elapsed Time Calc, Sub Processing, Shot Processing, getting latest score to use for play score
                game = DAL.Instance().GetGame(play.GameID);
                gameElapsedTimeInSeconds = StatCalculationsModule.CalculateTimeElapsedInGameInSeconds(game.Game, game.Game.CurrentPeriod, game.Game.CurrentClock);

                //Set some more play properties that were not set during stat selection              
                play.PlayText = Common.Instance().CreatePlayText(play);
                play.ElapsedTimeInSeconds = StatCalculationsModule.CalculateTimeElapsedInGameInSeconds(game.Game, play.Period, play.GameTime);

                //Based on the clock time, get the most recent play (i.e. play before this clock time) and the home and away play score and use for this play score    
                mostRecentPriorPlay = DAL.Instance().GetPlaysForGamePriorToElapsedTime(play.GameID, play.ElapsedTimeInSeconds, play.PlayID);
                play.AwayScore = mostRecentPriorPlay.AwayScore;
                play.HomeScore = mostRecentPriorPlay.HomeScore;

                //We can not use play.ElapsedTimeInSeconds >= gameElapsedTimeInSeconds since the play elapsed time is when user clicked the menu button
                //so it would ALWAYS be < gameElapsedtime and therefore we would never be executing SubstitutionProcessing
                if (whosCalling.ToUpper() == "GM")
                {
                    Common.Instance().SubstitutionProcessing(gameID, play, false);
                }

                //If the game has not started then all we want(ed) to do is in SubstitutionProcessing (i.e. update onfield and starter status)
                //we do not want to save this sub to the database as we do not want or need it for player minutes calculations
                if (game.Game.GameStatus == "NOT STARTED")
                {
                    return;
                }

                BaseTableDataAccess.Instance().UpsertPlay(play);

                //Now we need to save this stat to the database
                UpsertStatPlayToFlatTotals(play, false);

                //Update game current period, current clock based upon this play,only do this for a new play not and edited play    
                //TJY Commented this out for 2 reasons, first is elapsedtime would NEVER be greater than gameElapsedTime since play elapsed time
                //is when user click menu (see comment above), AND for a sub play no need to update period/clock
                //if (play.ElapsedTimeInSeconds > gameElapsedTimeInSeconds)
                //DAL.Instance().UpdateGameCurrentPeriodAndCurrentClock(play.GameID, play.Period, play.GameTime);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void SaveMovePlay(int gameID, int teamID, int period, string gameTime, int player1ID, string gmPlayer1PositionID, int? player2ID, string gmPlayer2PositionID)
        {
            Play play = new Play();
            Play mostRecentPriorPlay = new Play();
            Game game = new Game();

            try
            {
                game = BaseTableDataAccess.Instance().GetGameByGameID(gameID);

                //Set the play info for this substitution
                play.GameID = gameID;
                play.TeamID = teamID;
                play.Period = period;
                play.GameTime = gameTime;
                play.Player1ID = player1ID;
                play.Player2ID = player2ID;
                play.GMPlayer1PositionID = gmPlayer1PositionID;
                play.GMPlayer2PositionID = gmPlayer2PositionID;
                play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Move");
                play.StatDescriptionID = 0;
                play.OtherTeamGoalieID = 0;

                //Set some more play properties that were not set during stat selection              
                play.PlayText = Common.Instance().CreatePlayText(play);
                play.ElapsedTimeInSeconds = StatCalculationsModule.CalculateTimeElapsedInGameInSeconds(game, play.Period, play.GameTime);

                //Based on the clock time, get the most recent play (i.e. play before this clock time) and the home and away play score and use for this play score    
                mostRecentPriorPlay = DAL.Instance().GetPlaysForGamePriorToElapsedTime(play.GameID, play.ElapsedTimeInSeconds, play.PlayID);
                play.AwayScore = mostRecentPriorPlay.AwayScore;
                play.HomeScore = mostRecentPriorPlay.HomeScore;

                //Update Both players GM Position
                BaseTableDataAccess.Instance().UpdatePlayersGMPlayerPositionID(play.GameID, play.TeamID, play.Player1ID, play.GMPlayer2PositionID);
                BaseTableDataAccess.Instance().UpdatePlayersGMPlayerPositionID(play.GameID, play.TeamID, play.Player2ID, play.GMPlayer1PositionID);

                //If the game has not started then we do not want to save this move play to the database 
                //(i.e. user is simply moving players around before the game)
                if (game.GameStatus != "NOT STARTED")
                {
                    BaseTableDataAccess.Instance().UpsertPlay(play);
                }
            }

            catch (Exception ex)
            {

                throw;
            }
        }

        public void SaveClockPlay(int gameID, int period, string gameTime)
        {
            Play play = new Play();
            Play mostRecentPriorPlay = new Play();
            Game game = new Game();

            try
            {
                game = BaseTableDataAccess.Instance().GetGameByGameID(gameID);

                play.GameID = gameID;
                play.Period = period;
                play.GameTime = gameTime;
                play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName("Clock");
                play.StatDescriptionID = 0;
                play.OtherTeamGoalieID = 0;

                //Set some more play properties that were not set during stat selection              
                play.PlayText = Common.Instance().CreatePlayText(play);
                play.ElapsedTimeInSeconds = StatCalculationsModule.CalculateTimeElapsedInGameInSeconds(game, play.Period, play.GameTime);

                //Based on the clock time, get the most recent play (i.e. play before this clock time) and the home and away play score and use for this play score    
                mostRecentPriorPlay = DAL.Instance().GetPlaysForGamePriorToElapsedTime(play.GameID, play.ElapsedTimeInSeconds, play.PlayID);
                play.AwayScore = mostRecentPriorPlay.AwayScore;
                play.HomeScore = mostRecentPriorPlay.HomeScore;

                //Calling base UpsertPlay because do not want to save anything other than this playtext
                BaseTableDataAccess.Instance().UpsertPlay(play);
            }


            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion "Play"

        #region "FlatTotals"

        public ObservableCollection<FlatTotalsModel> GetGamesPlayerFlatStats(int gameID, int teamID)
        {
            ObservableCollection<FlatTotalsModel> gamesPlayerFlatStats = new ObservableCollection<FlatTotalsModel>();
            List<EventRoster> eventRosterForTeamList = new List<EventRoster>();
            FlatTotalsModel playersFlatStatsForGame = new FlatTotalsModel();

            try
            {
                eventRosterForTeamList = BaseTableDataAccess.Instance().GetEventRoster(gameID, teamID);

                foreach (var eventRosterEntry in eventRosterForTeamList)
                {
                    playersFlatStatsForGame = DAL.Instance().GetPlayersFlatTotalsForGame(gameID, teamID, eventRosterEntry.PlayerID);

                    //Debug for printing of specific stats (used to verfiy what is and what is not in flattotals table during testing
                    //Debug.WriteLine(playersFlatStatsForGame.Player.FirstName + " assist=" + playersFlatStatsForGame.FlatTotals.AssistTotal + " shots=" + playersFlatStatsForGame.FlatTotals.ShotTotal);

                    gamesPlayerFlatStats.Add(playersFlatStatsForGame);
                }

                //Add unknown player stats
                playersFlatStatsForGame = DAL.Instance().GetPlayersFlatTotalsForGame(gameID, teamID, -1);

                gamesPlayerFlatStats.Add(playersFlatStatsForGame);


                return gamesPlayerFlatStats;
            }
            catch (Exception ex)
            {

                return gamesPlayerFlatStats;
            }
        }

        public FlatTotalsModel GetTeamsFlatStatsForGame(int gameID, int teamID)
        {
            FlatTotalsModel flatTotalsModel = new FlatTotalsModel();

            try
            {
                var teamFlatTotalsRecord = BaseTableDataAccess.Instance().GetTeamsFlatStatsForGame(gameID, teamID);

                if (teamFlatTotalsRecord != null)
                {
                    flatTotalsModel.FlatTotals = teamFlatTotalsRecord;
                    flatTotalsModel.Team = BaseTableDataAccess.Instance().GetTeamByTeamID(teamFlatTotalsRecord.TeamID);
                }
                else
                {
                    flatTotalsModel.FlatTotals = new FlatTotals();
                    flatTotalsModel.Team = BaseTableDataAccess.Instance().GetTeamByTeamID(teamID);
                }

                return flatTotalsModel;
            }
            catch (Exception ex)
            {
                return flatTotalsModel;
            }
        }

        public FlatTotalsModel GetPlayersFlatTotalsForGame(int gameID, int teamID, int playerID)
        {
            FlatTotalsModel flatTotalsModel = new FlatTotalsModel();

            try
            {
                var selectedFlatTotalsRecord = BaseTableDataAccess.Instance().GetPlayersFlatTotalsRecord(gameID, teamID, playerID);

                if (selectedFlatTotalsRecord != null)
                {
                    flatTotalsModel.FlatTotals = selectedFlatTotalsRecord;
                    flatTotalsModel.Player = GetPlayer(selectedFlatTotalsRecord.PlayerID);
                    flatTotalsModel.Team = BaseTableDataAccess.Instance().GetTeamByTeamID(selectedFlatTotalsRecord.TeamID);
                    flatTotalsModel.TeamRoster = GetPlayerTeamRosterEntry(teamID, playerID);
                    flatTotalsModel.RosterDisplayText = flatTotalsModel.TeamRoster.UniformNumber.PadRight(4, ' ') + " " + flatTotalsModel.Player.FirstName + " " + flatTotalsModel.Player.LastName;
                }
                else   //player does not have any stats
                {
                    flatTotalsModel.FlatTotals = new FlatTotals();
                    flatTotalsModel.Player = GetPlayer(playerID);
                    flatTotalsModel.Team = BaseTableDataAccess.Instance().GetTeamByTeamID(teamID);
                    flatTotalsModel.TeamRoster = GetPlayerTeamRosterEntry(teamID, playerID);
                    flatTotalsModel.RosterDisplayText = flatTotalsModel.TeamRoster.UniformNumber.PadRight(4, ' ') + " " + flatTotalsModel.Player.FirstName + " " + flatTotalsModel.Player.LastName;
                }

                return flatTotalsModel;
            }


            catch (Exception ex)
            {
                return flatTotalsModel;
            }
        }

        public void UpsertFlatTotalsStat(int gameID, int? playerID, int teamID, string statName, bool areWeBackingOutStat)
        {
            try
            {
                Play play = new Play();

                play.GameID = gameID;
                play.Player1ID = (int)playerID;
                play.TeamID = teamID;
                play.StatCategoryID = DAL.Instance().GetStatCategoryIDByName(statName);
                play.StatDescriptionID = null;
                DAL.Instance().UpsertStatPlayToFlatTotals(play, areWeBackingOutStat);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion "FlatTotals"

        #region "Stats"

        public ObservableCollection<StatCategoryModel> GetVisibleStats(bool orderByName, bool includeSubCategory)
        {
            ObservableCollection<StatCategoryModel> visibleStats = new ObservableCollection<StatCategoryModel>();
            try
            {
                if (orderByName == true)
                {
                    App.gStatsList = App.gStatsList.OrderBy(x => x.StatCategory.StatCategoryName).ToList();
                }
                else //order by sort order
                {
                    App.gStatsList = App.gStatsList.OrderBy(x => x.StatCategory.SortOrder).ToList();
                }

                foreach (var item in App.gStatsList)
                {
                    if ((item.StatCategory.Visible == "Y") || (item.StatCategory.StatCategoryName == "Substitution") && (includeSubCategory == true))
                    {
                        item.ShowDescriptions = false; //Setting to false so previous state is cleared (i.e. do not want previous user stat selection showing)
                        visibleStats.Add(item);
                    }
                }
                return visibleStats;
            }
            catch (Exception ex)
            {
                return visibleStats;
            }
        }


        #endregion "Stats"

        #region "Jersey"

        public void PopulateJerseyTable()
        {
            try
            {
                BaseTableDataAccess.Instance().UpsertJersey(new Jersey { JerseyName = AppResources.Red, ImagePath = "/Assets/Jerseys/1.png" });
                BaseTableDataAccess.Instance().UpsertJersey(new Jersey { JerseyName = AppResources.BlueRed, ImagePath = "/Assets/Jerseys/2.png" });
                BaseTableDataAccess.Instance().UpsertJersey(new Jersey { JerseyName = AppResources.DarkRed, ImagePath = "/Assets/Jerseys/3.png" });
                BaseTableDataAccess.Instance().UpsertJersey(new Jersey { JerseyName = AppResources.BlueStripe, ImagePath = "/Assets/Jerseys/4.png" });
                BaseTableDataAccess.Instance().UpsertJersey(new Jersey { JerseyName = AppResources.OrangeGreen, ImagePath = "/Assets/Jerseys/5.png" });
                BaseTableDataAccess.Instance().UpsertJersey(new Jersey { JerseyName = AppResources.GreenRed, ImagePath = "/Assets/Jerseys/6.png" });
                BaseTableDataAccess.Instance().UpsertJersey(new Jersey { JerseyName = AppResources.BlueCross, ImagePath = "/Assets/Jerseys/7.png" });
                BaseTableDataAccess.Instance().UpsertJersey(new Jersey { JerseyName = AppResources.WhiteBlue, ImagePath = "/Assets/Jerseys/8.png" });
                BaseTableDataAccess.Instance().UpsertJersey(new Jersey { JerseyName = AppResources.WhiteRed, ImagePath = "/Assets/Jerseys/9.png" });
                BaseTableDataAccess.Instance().UpsertJersey(new Jersey { JerseyName = AppResources.BlueWhite, ImagePath = "/Assets/Jerseys/10.png" });
                BaseTableDataAccess.Instance().UpsertJersey(new Jersey { JerseyName = AppResources.RedGreen, ImagePath = "/Assets/Jerseys/11.png" });
                BaseTableDataAccess.Instance().UpsertJersey(new Jersey { JerseyName = AppResources.Orange, ImagePath = "/Assets/Jerseys/12.png" });
                BaseTableDataAccess.Instance().UpsertJersey(new Jersey { JerseyName = AppResources.WhiteGreen, ImagePath = "/Assets/Jerseys/13.png" });
                BaseTableDataAccess.Instance().UpsertJersey(new Jersey { JerseyName = AppResources.RedStripes, ImagePath = "/Assets/Jerseys/14.png" });
                BaseTableDataAccess.Instance().UpsertJersey(new Jersey { JerseyName = AppResources.YellowBlue, ImagePath = "/Assets/Jerseys/15.png" });
                BaseTableDataAccess.Instance().UpsertJersey(new Jersey { JerseyName = AppResources.GermanColors, ImagePath = "/Assets/Jerseys/16.png" });
                BaseTableDataAccess.Instance().UpsertJersey(new Jersey { JerseyName = AppResources.LightBlue, ImagePath = "/Assets/Jerseys/17.png" });
                BaseTableDataAccess.Instance().UpsertJersey(new Jersey { JerseyName = AppResources.YellowGreen, ImagePath = "/Assets/Jerseys/18.png" });
                BaseTableDataAccess.Instance().UpsertJersey(new Jersey { JerseyName = AppResources.DarkBlue, ImagePath = "/Assets/Jerseys/19.png" });
                BaseTableDataAccess.Instance().UpsertJersey(new Jersey { JerseyName = AppResources.RedBlack, ImagePath = "/Assets/Jerseys/20.png" });
            }
            catch (Exception ex)
            {
            }
        }

        public ObservableCollection<Jersey> GetJerseys()
        {
            ObservableCollection<Jersey> jerseys = new ObservableCollection<Jersey>();

            try
            {
                var list = BaseTableDataAccess.Instance().GetAllJerseysList();

                if (list != null)
                {
                    foreach (var jersey in list)
                    {
                        jerseys.Add(jersey);
                    }
                }
                return jerseys;
            }
            catch (Exception ex)
            {
                return jerseys;
            }
        }

        public Jersey GetJerseyByJerseyId(int jerseyID)
        {
            return BaseTableDataAccess.Instance().GetJerseyByJerseyID(jerseyID);
        }

        #endregion "Jersey"

        #region "Formations"

        public void UpsertFormation(Formation formation)
        {
            try
            {
                BaseTableDataAccess.Instance().UpsertFormation(formation);
            }
            catch (Exception ex)
            {
            }
        }

        public void CreateFormationList()
        {
            UpsertFormation(new Formation { FormationCount = 8, FormationName = "2-3-2" });
            UpsertFormation(new Formation { FormationCount = 8, FormationName = "3-3-2" });
            UpsertFormation(new Formation { FormationCount = 8, FormationName = "3-2-3" });
            UpsertFormation(new Formation { FormationCount = 11, FormationName = "5-3-2" });
            UpsertFormation(new Formation { FormationCount = 11, FormationName = "4-3-3" });
            UpsertFormation(new Formation { FormationCount = 11, FormationName = "4-2-4" });
        }

        public string GetFormationByFormationID(int formationID)
        {
            Formation _formation = new Formation();

            try
            {
                var selectedFormation = BaseTableDataAccess.Instance().GetFormationByFormationID(formationID);

                if (selectedFormation != null)
                {
                    _formation = selectedFormation;
                }

                return _formation.FormationName;
            }
            catch (Exception ex)
            {
                return _formation.FormationName;
            }
        }

        public ObservableCollection<Formation> GetFormationsByNumberOfPlayers(int playerCount)
        {
            ObservableCollection<Formation> formations = new ObservableCollection<Formation>();

            try
            {
                var list = BaseTableDataAccess.Instance().GetFormationsByNumberOfPlayers(playerCount);

                if (list != null)
                {
                    foreach (var formation in list)
                    {
                        formations.Add(formation);
                    }
                }
                return formations;
            }
            catch (Exception ex)
            {
                return formations;
            }
        }

        #endregion "Formations"

        #region "EventRoster"

        public ObservableCollection<EventRoster> GetEventRoster(int gameID, int teamID)
        {
            ObservableCollection<EventRoster> eventRoster = new ObservableCollection<EventRoster>();

            try
            {
                BaseTableDataAccess.Instance().GetEventRoster(gameID, teamID);

                var list = BaseTableDataAccess.Instance().GetEventRoster(gameID, teamID);

                if (list != null)
                {
                    foreach (var entry in list)
                    {
                        eventRoster.Add(entry);
                    }
                }
                return eventRoster;
            }
            catch (Exception ex)
            {
                return eventRoster;
            }
        }

        //Instead of calling this in UPSERT game when a game is added, we should call this when user click on GM screen.
        //This way if user were to enter in the entire season AHEAD of time, by moving it to when GM clicked we would successfully 
        //auto-generate the starters because it would be likey games would have been played (i.e. previous game starter info would exist)
        //This method will populate event rosters and set the starters/isonfield property
        //public void InitiliazeEventRosterOLD(int gameID, int teamID)
        //{
        //    try
        //    {
        //        int _starterCount = 0;
        //        bool _didPlayerStartLastGame = false;
        //        GameModel _gameDetails = GetGame(gameID);
        //        ObservableCollection<EventRoster> eventRosterPass1 = new ObservableCollection<EventRoster>();
        //        ObservableCollection<TeamRosterModel> rosterList = BaseTableDataAccess.Instance().GetTeamRoster(teamID);


        //        if (GetEventRoster(gameID, teamID).Count > 0)
        //        {
        //            return;
        //        }

        //        //Pass one, go through this teams roster and try and set starters based upon if the player is active and if he started for this team the last time they played
        //        foreach (var playerEntry in rosterList)
        //        {
        //            EventRoster eventRosterEntry = new EventRoster();
        //            eventRosterEntry.GameID = gameID;
        //            eventRosterEntry.TeamID = teamID;
        //            eventRosterEntry.PlayerID = playerEntry.Player.PlayerID;

        //            _didPlayerStartLastGame = DidPlayerStartLastGameThisTeamPlayed(gameID, teamID, eventRosterEntry.PlayerID);

        //            if ((_starterCount < _gameDetails.Game.PlayersPerTeam) && (playerEntry.TeamRoster.Active == true) && _didPlayerStartLastGame)
        //            {
        //                eventRosterEntry.Starter = "Y";
        //                UpdatePlayersGameStartedStat(gameID, teamID, eventRosterEntry.PlayerID, true);
        //                eventRosterEntry.IsPlayerOnField = "Y";
        //                eventRosterEntry.GMPlayerPositionID = "";
        //                _starterCount = _starterCount + 1;
        //            }
        //            else
        //            {
        //                eventRosterEntry.Starter = "N";
        //                UpdatePlayersGameStartedStat(gameID, teamID, eventRosterEntry.PlayerID, false);
        //                eventRosterEntry.IsPlayerOnField = "N";
        //                eventRosterEntry.GMPlayerPositionID = "";
        //            }

        //            eventRosterPass1.Add(eventRosterEntry);
        //        }

        //        if (eventRosterPass1.Count != 0)
        //        {
        //            BaseTableDataAccess.Instance().UpsertEventRoster(eventRosterPass1);
        //        }

        //        //If we do not have enough starters for this team then we need to do a pass 2, we need to randomly select players to start so we have enough players
        //        //So we will go through event roster we just built looking to set players to starter who where not starters until we get to the needed amount of starters
        //        if (_starterCount < _gameDetails.Game.PlayersPerTeam)
        //        {
        //            ObservableCollection<EventRoster> eventRosterPass2 = new ObservableCollection<EventRoster>();

        //            foreach (var eventRosterEntry in eventRosterPass1)
        //            {
        //                if (eventRosterEntry.Starter == "N" && _starterCount < _gameDetails.Game.PlayersPerTeam)
        //                {
        //                    eventRosterEntry.Starter = "Y";
        //                    UpdatePlayersGameStartedStat(gameID, teamID, eventRosterEntry.PlayerID, true);
        //                    eventRosterEntry.IsPlayerOnField = "Y";
        //                    eventRosterEntry.GMPlayerPositionID = "";
        //                    _starterCount = _starterCount + 1;
        //                }

        //                eventRosterPass2.Add(eventRosterEntry);
        //            }

        //            if (eventRosterPass2.Count != 0)
        //            {
        //                BaseTableDataAccess.Instance().UpsertEventRoster(eventRosterPass2);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        public void InitiliazeEventRoster(int gameID, int teamID)
        {
            try
            {
                int _starterCount = 0;
                GameModel _gameDetails = GetGame(gameID);
                ObservableCollection<EventRoster> eventRosterPass1 = new ObservableCollection<EventRoster>();
                ObservableCollection<TeamRosterModel> rosterList = BaseTableDataAccess.Instance().GetTeamRoster(teamID);

                if (GetEventRoster(gameID, teamID).Count > 0)
                {
                    return;
                }

                //Go through this teams roster and try and set starters based upon if the player is active
                foreach (var playerEntry in rosterList)
                {
                    EventRoster eventRosterEntry = new EventRoster();
                    eventRosterEntry.GameID = gameID;
                    eventRosterEntry.TeamID = teamID;
                    eventRosterEntry.PlayerID = playerEntry.Player.PlayerID;

                    //Only add players to event roster that are active on the team roster
                    if (playerEntry.TeamRoster.Active == "Y")
                    {
                        if ((_starterCount < _gameDetails.Game.PlayersPerTeam))
                        {
                            eventRosterEntry.Starter = "Y";
                            eventRosterEntry.IsPlayerOnField = "Y";
                            eventRosterEntry.GMPlayerPositionID = "";
                            _starterCount = _starterCount + 1;
                        }
                        else
                        {
                            eventRosterEntry.Starter = "N";
                            eventRosterEntry.IsPlayerOnField = "N";
                            eventRosterEntry.GMPlayerPositionID = "";
                        }
                        eventRosterPass1.Add(eventRosterEntry);
                    }
                }

                if (eventRosterPass1.Count != 0)
                {
                    BaseTableDataAccess.Instance().UpsertEventRoster(eventRosterPass1);
                }
            }
            catch (Exception ex)
            {
                ErrorLogConnection cloud = new ErrorLogConnection();
                cloud.UpdateErrorLog("DAL.InitializeEventRoster", ex.Message.ToString());
            }
        }

        //In order to do an Upsert there needs to be a primary key, the primary key should be game id, player id, but I could not figure out
        //how to create a multi column primary key.  As a result I can not use update, so doing delete/insert all the time

        //Since eventroster does not have a primary key, I need to do a delete this way and not with db.Delete
        public void DeleteEventRoster(int gameID, int teamID)
        {
            try
            {
                BaseTableDataAccess.Instance().DeleteEventRoster(gameID, teamID);
            }
            catch (Exception ex)
            {
            }
        }

        //public void UpdateGameStartedStatForGameOLD(int gameID, int teamID)
        //{
        //    try
        //    {
        //        FlatTotals playerFlatTotals = new FlatTotals();
        //        List<EventRoster> eventRosterList = new List<EventRoster>();

        //        eventRosterList = BaseTableDataAccess.Instance().GetEventRoster(gameID, teamID);

        //        foreach (var eventRosterEntry in eventRosterList)
        //        {
        //            if (eventRosterEntry.Starter == "Y")
        //            {
        //                UpdatePlayersGameStartedStat(gameID, teamID, eventRosterEntry.PlayerID, true);
        //            }
        //            else
        //            {
        //                //If we ever run this from GM as a one time deal, we need to uncomment the line below in case there are changes to starters from GM prior to game starting
        //                //UpdatePlayersGameStartedStat(gameID, teamID, eventRosterEntry.PlayerID, false);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogConnection cloud = new ErrorLogConnection();
        //        cloud.UpdateErrorLog("DAL.UpdateGameStartedStatForGame", ex.Message.ToString());
        //    }
        //}

        public void UpdateGameStartedStatForGame(int gameID)
        {
            try
            {
                FlatTotals playerFlatTotals = new FlatTotals();
                List<EventRoster> eventRosterList = new List<EventRoster>();

                eventRosterList = BaseTableDataAccess.Instance().GetEventRosterByGameID(gameID);

                foreach (var eventRosterEntry in eventRosterList)
                {
                    if (eventRosterEntry.Starter == "Y")
                    {
                        UpdatePlayersGameStartedStat(gameID, eventRosterEntry.TeamID, eventRosterEntry.PlayerID, true);
                    }
                    else
                    {
                        UpdatePlayersGameStartedStat(gameID, eventRosterEntry.TeamID, eventRosterEntry.PlayerID, false);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogConnection cloud = new ErrorLogConnection();
                cloud.UpdateErrorLog("DAL.UpdateGameStartedStatForGame", ex.Message.ToString());
            }
        }

        public void UpdatePlayersGameStartedStat(int gameID, int teamID, int playerID, bool isPlayerStarting)
        {
            FlatTotals playerFlatTotals = new FlatTotals();
            List<EventRoster> eventRosterList = new List<EventRoster>();

            try
            {
                //If player does not have a flat totals record, need to create one first (they will not have one until the first stat for them is saved)
                playerFlatTotals = BaseTableDataAccess.Instance().GetPlayersFlatTotalsRecord(gameID, teamID, playerID);

                if (playerFlatTotals == null)
                {
                    FlatTotals flatTotals = new FlatTotals();

                    flatTotals.GameID = gameID;
                    flatTotals.TeamID = teamID;
                    flatTotals.PlayerID = playerID;
                    BaseTableDataAccess.Instance().InsertFlatTotals(flatTotals);
                }
                if (isPlayerStarting == true)
                {
                    BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "GameStarted", 1);
                }
                else
                {
                    BaseTableDataAccess.Instance().UpdatePlayersFlatTotalsRecord(gameID, playerID, teamID, "GameStarted", 0);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void UpdatePlayersGMPlayerPositionID(int gameID, int teamID, int? playerID, string gmPlayerPositionID)
        {
            try
            {
                BaseTableDataAccess.Instance().UpdatePlayersGMPlayerPositionID(gameID, teamID, playerID, gmPlayerPositionID);
            }
            catch (Exception ex)
            {
            }
        }

        public bool DidPlayerStartLastGameThisTeamPlayed(int gameID, int teamID, int playerID)
        {
            bool rtnResult = false;
            int _gameID = 0;

            try
            {
                using (var db = new SQLiteConnection(App.gDBPath))
                {
                    //Get teams last game played that was either in progress or final (as long as game was started we will use the last game this team played)
                    var list = BaseTableDataAccess.Instance().GetGamesForTeam(teamID);

                    if (list != null)
                    {
                        foreach (var game in list)
                        {
                            //When looking for teams last game do not include this game we are checking for
                            if (game.GameID != gameID)
                            {
                                _gameID = game.GameID;
                                break;
                            }
                        }
                    }

                    //If no games returned then this player has not played for this team before
                    if (_gameID == 0)
                    {
                        rtnResult = false;
                    }
                    else //player has played for this team before, check this games event roster to see if this player started that game
                    {
                        //Checking eventroster for this player in this game, did he start?
                        var playerEventRosterEntry = BaseTableDataAccess.Instance().GetEventRosterByGameTeamPlayer(_gameID, teamID, playerID);

                        if (playerEventRosterEntry.Starter == "Y")
                        {
                            rtnResult = true;
                        }
                        else
                        {
                            rtnResult = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return rtnResult;
            }

            return rtnResult;
        }

        public List<EventRoster> GetPlayersInGivenPointInGame(int gameID, int teamID, int elapsedTimeInSeconds)
        {
            List<EventRoster> playersInGameList = new List<EventRoster>();
            List<EventRoster> eventRosterList = new List<EventRoster>();
            List<Play> subPlaysList = new List<Play>();
            Game game = new Game();

            try
            {
                game = BaseTableDataAccess.Instance().GetGameByGameID(gameID);

                if (elapsedTimeInSeconds >= game.CurrentElapsedTimeInSeconds)
                {
                    //if this is a current play can simply user players on the field
                    eventRosterList = BaseTableDataAccess.Instance().GetListOfPlayersOnTheField(gameID, teamID);

                    foreach (var eventRosterEntry in eventRosterList)
                    {
                        playersInGameList.Add(eventRosterEntry);
                    }
                }
                else
                {
                    //this is a time in the past so need to analyze sub plays to build list of players in the game
                    //Go through eventRoster and put the starters in playersInGame and the non starters in playersOutOfGame
                    //We will then use the SUB plays up to the time being passed in to move guys in and out of those collections
                    eventRosterList = BaseTableDataAccess.Instance().GetEventRoster(gameID, teamID);
                    playersInGameList = eventRosterList.Where(x => x.Starter.ToUpper().Equals("Y")).ToList();
                    subPlaysList = BaseTableDataAccess.Instance().GetAllPlaysForGame(gameID, "ASC").Where(x => x.StatCategoryID == 22 && x.TeamID == teamID && x.ElapsedTimeInSeconds <= elapsedTimeInSeconds).ToList();

                    subPlaysList.ForEach(x =>
                        {
                            if (playersInGameList.Where(xx => xx.PlayerID == x.Player2ID).Count() > 0)
                            {
                                playersInGameList.Add(eventRosterList.Where(er => er.PlayerID == x.Player1ID).First());
                            }

                            if (x.Player2ID != null)
                            {
                                //Pull out number 2
                                playersInGameList.Remove(playersInGameList.Where(xx => xx.PlayerID == Convert.ToInt32(x.Player2ID)).First());
                            }

                        });
                }

                return playersInGameList;
            }
            catch (Exception ex)
            {
                return playersInGameList;
            }
        }

        public static ObservableCollection<TeamRosterModel> GetPlayersInAndOutAtAGivenPointInGameNOTWORKINGYET(int gameID, int teamID, int elapsedTimeInSeconds, string mode)
        {
            List<EventRoster> playersInGameList = new List<EventRoster>();
            List<EventRoster> playersOutGameList = new List<EventRoster>();
            List<EventRoster> eventRosterList = new List<EventRoster>();
            List<Play> subPlaysList = new List<Play>();
            ObservableCollection<TeamRosterModel> returnList = new ObservableCollection<TeamRosterModel>();

            try
            {
                //Go through eventRoster and put the starters in playersInGame and the non starters in playersOutOfGame
                //We will then use the SUB plays up to the time being passed in to move guys in and out of those collections
                eventRosterList = BaseTableDataAccess.Instance().GetEventRoster(gameID, teamID);
                playersInGameList = eventRosterList.Where(x => x.Starter.ToUpper().Equals("Y")).ToList();
                playersOutGameList = eventRosterList.Where(x => x.Starter.ToUpper().Equals("N")).ToList();
                subPlaysList = BaseTableDataAccess.Instance().GetAllPlaysForGame(gameID, "ASC").Where(x => x.StatCategoryID == 22 && x.TeamID == teamID && x.ElapsedTimeInSeconds <= elapsedTimeInSeconds).ToList();


                subPlaysList.ForEach(x =>
                {
                    if (playersInGameList.Where(xx => xx.PlayerID == x.Player2ID).Count() > 0)
                    {
                        playersInGameList.Add(eventRosterList.Where(er => er.PlayerID == x.Player1ID).First());
                        if (x.Player2ID != null)
                        {
                            playersInGameList.Remove(playersInGameList.Where(xx => xx.PlayerID == Convert.ToInt32(x.Player2ID)).First());
                        }

                        playersOutGameList.Add(eventRosterList.Where(xx => xx.PlayerID == Convert.ToInt32(x.Player2ID)).First());


                        playersOutGameList.Add(eventRosterList.Where(er => er.PlayerID == x.Player1ID).First());

                    }


                });

                //Go through the list needed (i.e. mode) and convert it to an observablecollection of teamRosterModel
                if (mode.ToUpper() == "IN")
                {
                    foreach (var item in playersInGameList)
                    {
                        TeamRosterModel teamRoster = new TeamRosterModel();
                        teamRoster = DAL.Instance().GetPlayersPhyiscalAndTeamRosterInfo(gameID, teamID, item.PlayerID);
                        returnList.Add(teamRoster);
                    }
                }
                else
                {
                    foreach (var item in playersOutGameList)
                    {
                        TeamRosterModel teamRoster = new TeamRosterModel();
                        teamRoster = DAL.Instance().GetPlayersPhyiscalAndTeamRosterInfo(gameID, teamID, item.PlayerID);
                        returnList.Add(teamRoster);
                    }
                }

                return returnList;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public ObservableCollection<TeamRosterModel> GetPlayersInAndOutAtAGivenPointInGame(int gameID, int teamID, int elapsedTimeInSeconds, string mode)
        {
            List<int?> playersInGameList = new List<int?>();
            List<int?> playersOutOfGameList = new List<int?>();
            ObservableCollection<EventRoster> eventRosterList = new ObservableCollection<EventRoster>();
            ObservableCollection<TeamRosterModel> returnList = new ObservableCollection<TeamRosterModel>();
            ObservableCollection<Play> subPlaysList = new ObservableCollection<Play>();

            try
            {
                //Go through eventRoster and put the starters in playersInGame and the non starters in playersOutOfGame
                //We will then use the SUB plays up to the time being passed in to move guys in and out of those collections
                eventRosterList = DAL.Instance().GetEventRoster(gameID, teamID);

                foreach (var eventRosterEntry in eventRosterList)
                {
                    if (eventRosterEntry.Starter == "Y")
                    {
                        playersInGameList.Add(eventRosterEntry.PlayerID);
                    }
                    else
                    {
                        playersOutOfGameList.Add(eventRosterEntry.PlayerID);
                    }
                }

                subPlaysList = DAL.Instance().GetSUBPlaysForGamePriorToElapsedTime(gameID, teamID, elapsedTimeInSeconds);

                foreach (var subPlay in subPlaysList)
                {
                    //Process the player coming in the game
                    if (Common.Instance().IsPlayerInList(playersInGameList, subPlay.Player1ID) == true)
                    {
                        //Should never get here (i.e. if player is in game, then he should not have a sub putting him in the game
                    }
                    else //this player has an IN sub so put him in the INList, and remove him from the OUTList
                    {
                        playersInGameList.Add(subPlay.Player1ID);
                        playersOutOfGameList.Remove(subPlay.Player1ID);
                    }

                    //Process the player coming out the game
                    if (Common.Instance().IsPlayerInList(playersOutOfGameList, subPlay.Player2ID) == true)
                    {
                        //Should never get here (i.e. if player is out of game, then he should not have a sub putting him out of the game
                    }
                    else //this player has an OUT sub so put him in the OUTList, and remove him from the INList
                    {
                        playersOutOfGameList.Add(subPlay.Player2ID);
                        playersInGameList.Remove(subPlay.Player2ID);
                    }
                }

                //Go through the list needed (i.e. mode) and convert it to an observablecollection of teamRosterModel
                if (mode.ToUpper() == "IN")
                {
                    foreach (var item in playersInGameList)
                    {
                        TeamRosterModel teamRoster = new TeamRosterModel();
                        teamRoster = DAL.Instance().GetPlayersPhyiscalAndTeamRosterInfo(gameID, teamID, (int)item);
                        returnList.Add(teamRoster);
                    }
                }
                else
                {
                    foreach (var item in playersOutOfGameList)
                    {
                        TeamRosterModel teamRoster = new TeamRosterModel();
                        teamRoster = DAL.Instance().GetPlayersPhyiscalAndTeamRosterInfo(gameID, teamID, (int)item);
                        returnList.Add(teamRoster);
                    }
                }

                return returnList;
            }
            catch (Exception ex)
            {
                return returnList;
            }
        }



        #endregion "EventRoster"

        #region "StatCategory"

        //*************************************************************************************************************************************  
        //NEVER EVER CHANGE THIS ROUTINE.  THE ENTIRE APPLICATION IS BASED ON THAT STAT CATEGORIIES AND DESCRIPTIONS HAVE CERTAIN IDS (I.E. SHOT = 1).
        //CHANGING THIS ROUTINE WILL BREAK THE ENTIRE APPLICATION.  STAT CATEGORIES CAN BE APPENDED TO THE END.  DO SO CAREFULLY AND WITH TESTING.
        //*************************************************************************************************************************************
        public void PopulateStatCategoryTable()
        {
            try
            {
                //Miss (1), Hit Post (2), Blocked (3), Goal (4), Excellent (5), Good (6), Poor (7), Illegal Throw In (8), Bad Pass (9), Lost Dribble (10),
                //Kicking (11), Tripping (12), Charging (13), Pushing (14), Holding (15), Illegal Tackle (16), For Goal (17), Not For Goal (18),
                //Unsportsmanlike Conduct (19), Delaying Restart Of Play (20), Foul Play (21), Violent Conduct (22), Illegal Hands (23), Second Yellow Card (24),
                //In (25), Out (26), Left (27), Right (28), Headed (29), Shot Details (30),

                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Shot", SortOrder = 1, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "1,2,3,4" }); //Hit Post,Blocked,Goal" 
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Pass", SortOrder = 2, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "5,6,7" }); //Excellent,Good,Poor"
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Turnover", SortOrder = 3, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "8,9" }); //Illegal Throw In,Bad Pass"
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Offsides", SortOrder = 4, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Foul Committed", SortOrder = 5, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "11,12,13,14,15,16" }); //Kicking,Tripping,Charging,Pushing,Holding,Illegal Tackle"
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Out Of Bounds", SortOrder = 6, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Cross", SortOrder = 7, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "5,6,7" }); //Excellent,Good,Poor"
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Throw In", SortOrder = 8, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Corner Kick", SortOrder = 9, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "5,6,7" }); //Excellent,Good,Poor"
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Tackle", SortOrder = 10, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Goalie Kick", SortOrder = 11, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Own Goal", SortOrder = 12, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Foul Drawn", SortOrder = 13, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Direct Free Kick", SortOrder = 14, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "17,18" }); //For Goal, Not For Goal"
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Indirect Free Kick", SortOrder = 15, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Penalty Kick", SortOrder = 16, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "1,2,3,4" }); //Miss,Hit Post,Block,Goal"
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Yellow Card", SortOrder = 17, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "19,20" }); //Unsportsmanlike Conduct,Delaying Restart of Play"
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Red Card", SortOrder = 18, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "21,22,23,24" }); //Foul Play,Violent Conduct,Illegal Hands,Second Yellow Card"
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Drop Kick", SortOrder = 19, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "5,6,7" }); //Excellent,Good,Poor"
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Dribble", SortOrder = 20, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Shootout Kick", SortOrder = 21, Active = "N", Visible = "Y", UserSelectedVisible = "Y", Descriptions = "1,2,3,4" }); //Miss,Hit Post,Block,Goal"

                //These stats should NEVER be visible, they are for stat calculation purposes only (i.e. not for user to select)
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Substitution", SortOrder = 22, Active = "N", Visible = "N", UserSelectedVisible = "N", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "SecondsPlayed", SortOrder = 23, Active = "N", Visible = "N", UserSelectedVisible = "N", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "MinutesPlayed", SortOrder = 24, Active = "N", Visible = "N", UserSelectedVisible = "N", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "PlusMinus", SortOrder = 25, Active = "N", Visible = "N", UserSelectedVisible = "N", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Note", SortOrder = 26, Active = "N", Visible = "N", UserSelectedVisible = "N", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Blocked", SortOrder = 27, Active = "N", Visible = "N", UserSelectedVisible = "N", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Goal Allowed", SortOrder = 28, Active = "N", Visible = "N", UserSelectedVisible = "N", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Save", SortOrder = 29, Active = "N", Visible = "N", UserSelectedVisible = "N", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Assist", SortOrder = 30, Active = "N", Visible = "N", UserSelectedVisible = "N", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Shot On Goal", SortOrder = 31, Active = "N", Visible = "N", UserSelectedVisible = "N", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Clock", SortOrder = 32, Active = "N", Visible = "N", UserSelectedVisible = "N", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "Move", SortOrder = 33, Active = "N", Visible = "N", UserSelectedVisible = "N", Descriptions = "" });
                BaseTableDataAccess.Instance().UpsertStatCategory(new StatCategory { StatCategoryName = "GameStarted", SortOrder = 34, Active = "N", Visible = "N", UserSelectedVisible = "N", Descriptions = "" });
            }
            catch (Exception)
            {
            }
        }

        public StatCategoryModel GetStatCategoryByName(string statCategorynName)
        {
            StatCategoryModel _statCategory = new StatCategoryModel();

            try
            {
                var selectedStatCategory = BaseTableDataAccess.Instance().GetStatCategoryByName(statCategorynName);

                if (selectedStatCategory != null)
                {
                    _statCategory.StatCategory = selectedStatCategory;
                }
                return _statCategory;
            }
            catch (Exception ex)
            {
                return _statCategory;
            }
        }

        public int GetStatCategoryIDByName(string statCategorynName)
        {
            int returnValue = 0;

            try
            {
                var selectedStatCategory = BaseTableDataAccess.Instance().GetStatCategoryByName(statCategorynName);

                if (selectedStatCategory != null)
                {
                    returnValue = selectedStatCategory.StatCategoryID;
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                return returnValue;
            }
        }

        public int GetStatDescriptionIDByName(string statDescriptionName)
        {
            int returnValue = 0;

            try
            {
                var selectedStatCategoryDescription = BaseTableDataAccess.Instance().GetStatDescriptionByName(statDescriptionName);

                if (selectedStatCategoryDescription != null)
                {
                    returnValue = selectedStatCategoryDescription.StatDescriptionID;
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                return returnValue;
            }
        }

        public string GetStatCategoryNameById(int statCategoryID)
        {
            string returnValue = string.Empty;

            try
            {
                var selectedStatCategory = BaseTableDataAccess.Instance().GetStatCategoryByID(statCategoryID);

                if (selectedStatCategory != null)
                {
                    returnValue = selectedStatCategory.StatCategoryName;
                }
                return returnValue;
            }
            catch (Exception ex)
            {
                return returnValue;
            }
        }

        #endregion "StatCategory"

        #region "StatDescription"

        //************************************************************************************************************************************* 
        //NEVER EVER CHANGE THIS ROUTINE.  THE ENTIRE APPLICATION IS BASED ON THAT STAT CATEGORIIES AND DESCRIPTIONS HAVE CERTAIN IDS (I.E. SHOT = 1).
        //CHANGING THIS ROUTINE WILL BREAK THE ENTIRE APPLICATION.  STAT CATEGORIES CAN BE APPENDED TO THE END.  DO SO CAREFULLY AND WITH TESTING.
        //*************************************************************************************************************************************
        public void PopulateStatDescriptionTable()
        {
            try
            {
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Miss", Visible = "Y" });     //1
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Hit Post", Visible = "Y" });     //2
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Blocked", Visible = "Y" });      //3
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Goal", Visible = "Y" });     //4
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Excellent", Visible = "Y" });        //5
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Good", Visible = "Y" });     //6
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Poor", Visible = "Y" });     //7
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Illegal Throw In", Visible = "Y" });     //8
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Bad Pass", Visible = "Y" });     //9
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Lost Dribble", Visible = "Y" });     //10
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Kicking", Visible = "Y" });      //11
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Tripping", Visible = "Y" });     //12
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Charging", Visible = "Y" });     //13
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Pushing", Visible = "Y" });      //14
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Holding", Visible = "Y" });  //15
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Illegal Tackle", Visible = "Y" });   //16
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "For Goal", Visible = "Y" }); //17
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Not For Goal", Visible = "Y" }); //18
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Unsportsmanlike Conduct", Visible = "Y" });  //19
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Delaying Restart Of Play", Visible = "Y" });     //20
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Foul Play", Visible = "Y" });    //21
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Violent Conduct", Visible = "Y" });  //22
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Illegal Hands", Visible = "Y" });        //23
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Second Yellow Card", Visible = "Y" });   //24
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "In", Visible = "Y" });   //25
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Out", Visible = "Y" });  //26
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Left", Visible = "N" });     //27
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Right", Visible = "N" });    //28
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Headed", Visible = "N" });   //29
                BaseTableDataAccess.Instance().UpsertStatDescription(new StatDescription { StatDescriptionName = "Shot Details", Visible = "N" }); //30
            }
            catch (Exception ex)
            {
            }
        }

        public StatDescriptionModel GetStatDescriptionByName(string statDescriptionName)
        {
            StatDescriptionModel _statDescription = new StatDescriptionModel();

            try
            {
                var selectedStatDescription = BaseTableDataAccess.Instance().GetStatDescriptionByName(statDescriptionName);

                if (selectedStatDescription != null)
                {
                    _statDescription.StatDescription = selectedStatDescription;
                }
                return _statDescription;
            }
            catch (Exception ex)
            {
                return _statDescription;
            }
        }

        public string GetStatDescriptionNameById(int? statDescriptionID)
        {
            string returnValue = string.Empty;

            try
            {
                if (statDescriptionID != null)
                {
                    var selectedStatDescription = BaseTableDataAccess.Instance().GetStatDescriptionByID(statDescriptionID);

                    if (selectedStatDescription != null)
                    {
                        returnValue = selectedStatDescription.StatDescriptionName;
                    }
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                return returnValue;
            }
        }

        public ObservableCollection<StatDescription> GetStatDescriptions(string includeInVisible)
        {
            ObservableCollection<StatDescription> statDescriptions = new ObservableCollection<StatDescription>();

            try
            {
                var list = BaseTableDataAccess.Instance().GetStatDescriptions();
                if (list != null)
                {
                    foreach (var statDesc in list)
                    {
                        if ((includeInVisible == "Y") || (includeInVisible == "N" && statDesc.Visible == "Y"))
                        {

                            statDescriptions.Add(statDesc);
                        }
                    }
                }
                return statDescriptions;
            }
            catch (Exception ex)
            {
                return statDescriptions;
            }
        }

        #endregion "StatDescription"
    }
}
