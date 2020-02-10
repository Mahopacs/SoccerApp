using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ScoreSoccer8.Resources;
using System.IO;
using ScoreSoccer8.Classes;
using System.Collections.Generic;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.DataAccess;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.Phone.Marketplace;
using ScoreSoccer8.Cloud;
using System.Threading.Tasks;
using ScoreSoccer8.Views;
using ScoreSoccer8.Utilities;
using System.Reflection;


//globalization complete: Teams, Team Details, Stats Reports, Leagues, League Details, Rosters, Games, Game Details, 
//globalization complete: Player List, Player Details, Stats Picker, Shot Details, Play List, Play Details
//globalization to do: Game Manager, Main Page, BoxScore, Tutorial, App Level (i.e. create play text, processing)

namespace ScoreSoccer8
{
    public partial class App : Application
    {

        public static MobileServiceClient MobileService = new MobileServiceClient(
            "https://utrack.azure-mobile.net/",
            "fYXCWUkszsqYkvkQCOESZhAWTaunrW80"
            );

        private static LicenseInformation _licenseInfo = new LicenseInformation();
        private static bool _isTrial;
        public static bool IsTrial
        {
            get
            {
                return _isTrial;
            }
        }

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame
        {
            get;
            private set;
        }

        private DateTime _timeOpened = DateTime.Now;

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        /// 
        public static string gDBName = "Soccer.sqlite";
        public static string FULL_STATS_FREE_VERSION = "FULL_STATS";
        private static string TRIAL_START_DATE = "TRIAL_START_DATE";

        //TO BE CHANGED WHEN DEPLOYING.  FOR FREE VERSION, SET TO TRUE.  FOR PAID APP STORE VERSION SET TO FALSE.
        public static bool FREE_VERSION = true;

        public static string gDBPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, gDBName);
        public static List<StatCategoryModel> gStatsList = new List<StatCategoryModel>();

        //WHEN DEPLOYING MAKE SURE gIsDeveloperDebugMode IS SET TO FALSE (otherwise they will get our sample data)
        public static bool gIsDeveloperDebugMode = false;

        public static int gDBRetryLimit = 20;        //Used in our retry database exception logic
        public static int gDBRetrySleepTime = 100;  //Used in our retry database exception logic
        public static string gHasAppBeenRated = "NO";
        public static int gAppOpenedCount = 0;
        public static bool gHaveWePromptedToPurchase = false;

        //START This is needed as we navigate screens with jersey prompts, so we know when to prompt for jersey or not
        public static bool gPromptForJersey;
        public static int gPromptForJerseyTeamID;
        public static string gPromptForJerseyTeamName;
        public static int gPromptForJerseyPlayerID;
        //END This is needed as we navigate screens with jersey prompts, so we know when to prompt for jersey or not

        //Revision History (1.0.1.0)
        //1. Game Manager will now load visually fine even if there are only 2 players per row (previously it would “throw” players on top of each other)
        //2. Game Manager will now load correctly for a game where a player is deleted from a team roster (previously it would error out in the code resulting in none of the players being loaded on the field)
        //3. Spacing fix for unknown player play text (added a space between player name and home or visitor)
        //4. Version is now bound to a variable (previously was hard coded)
        //5. Game Manager fix for when navigating away and coming back (i.e. going to Stats and then back to gm).  (Previously was making players  players on the field smaller)

        //Revision History (1.0.2.0)
        //1. For move plays prior to start of game do not save play to database, only update gm player postion id (i.e. user just moving players around prior to the game)
        //2. Only players that are active on team roster are added to the event roster
        //3. Add OnCloud to all tables/classes (this is a string property)
        //4. If database exists, proc is called to add 'OnCloud' column to all the tables.  This is needed for existing users who may not have
        //   column on their tables.  Proc has exception handling to if 'column already exists' error occurs it just leaves and does nothing
        //5. Changes to game manager for how subs and moves work.
        //6. Assist are now being saved correctly
        //7. Player Minutes was not showing seconds on team boxscores.

        //Revision History (1.0.3.0)
        //1. Changed so we are now prompting to rate every 2 times instead of every 10 times.
        //2. Changed so on free version if user rates app we will now enable all stats functionality.
        //3. Changed the rate prompt so it not lets user know that if they rate they will get full stats functionality.


        //Not being used right now
        public enum gStatCategory
        {
            Shot, Pass, Turnover, Offsides, Foul_Commited, Out_Of_Bounds, Cross, Throw_In, Corner_Kick, Tackle, Goalie_Kick, Own_Goal,
            Foul_Drawn, Direct_Free_Kick, Indirect_Free_Kick, Penalty_Kick, Yellow_Card, Red_Card, Drop_Kick, Dribble, Shootout_Kick
        }

        //Not being used right now
        public enum gStatDescription
        {
            Miss, Hit_Post, Block, Goal, Excellent, Good, Poor, Illegal_Throw_In, Bad_Pass, Lost_Dribble, Kicking, Tripping, Charging, Pushing,
            Holding, Illegal_Tackle, For_Goal, Not_For_Goal, Unsportsmanlike_Conduct, Delaying_Restart_Of_Play, Foul_Play, Violent_Conduct,
            Illegal_Hands, Second_Yellow_Card
        }

        #region app enabling and trial info

        private void CheckLicense()
        {
            try
            {
                //_isTrial = false;
                //TO Be commented OUT when we release the APP.
                //_isTrial = _licenseInfo.IsTrial();
                //_isTrial = true;
                //gIsPaidVersion = false;
                _isTrial = false;

                //After checking Licence, call into CLOUD.
                AllUsersConnection cloud = new AllUsersConnection();
                cloud.UpdateUserInformation(IsTrial);
            }
            catch (Exception)
            {
                _isTrial = false;
            }
        }

        /// <summary>
        /// This should be called when loading the stats picker.  If this method returns true, the user should be able to track ALL stats.  If this method returns
        /// false, the user should only be able to track Shots / Shot details.
        /// 
        /// TODO: This needs to be hooked into in the following places:
        /// 1.  When loading the stats picker
        /// 2.  When loading the main screen: If this returns true, do not display the "Purchase" button
        /// 3.  When loading the main screen:  If this returns true, do not display the "Lite" icon, display the full icon
        /// </summary>
        /// <returns></returns>
        public static bool DoesUserHaveAbilityToTrackAllStats()
        {
            bool returnValue = false;

            //If the user of a free app has bought the full stats package, they have access to track all of the stats.
            if (FREE_VERSION)
            {
                if (Convert.ToBoolean(IS.GetSetting(FULL_STATS_FREE_VERSION)))
                {
                    returnValue = true;
                }
            }
            else
            {
                returnValue = true;
            }

            //10/25/14 TJY Change so if user has rated app they get ALL stats now
            if (App.gHasAppBeenRated.ToUpper() == "YES")
            {
                returnValue = true;
            }
           
            return returnValue;
        }

        /// <summary>
        /// This method will return whether or not the APP is enabled at all.  This should be called on init of the main screen.
        /// For Free apps, this will always return true, as they can always use the app.
        /// For the trial, we will return false after 3 days. 
        /// TODO:  Disable All buttons on the main screen if this returns FALSE.  If the user clicks on a button, prompt them "Trial has expired, do you wish to purchase app?"
        /// If they click yes, navigate to the store.  This method will automatically check licence information after the purchase has been completed.
        /// </summary>
        /// <returns></returns>
        public static bool IsAppEnabled()
        {
            bool appEnabled = false;

            //App is always enabled for free version.
            if (FREE_VERSION)
            {
                appEnabled = true;
            }
            //Paid version
            else
            {
                //If it is not a trial, it is the full paid version
                if (!_isTrial)
                {
                    appEnabled = true;
                }
                else
                {
                    // It is a trial.  We need to have logic here to see if the trial is EXPIRED.  If the trial is expired, we need to return FALSE to disable
                    //The application.  For now, we are only using a 3 day trial.

                    //Step 1:  Save the trial date if there is not one already.  This will be the date the application is first opened.
                    if (IS.GetSetting(TRIAL_START_DATE) == null)
                    {
                        //Save today
                        IS.SaveSetting(TRIAL_START_DATE, DateTime.Today);
                    }

                    //Step 2:  Check to see if the Trial Date + 4 days = Today.  We do 4 days to actually allow them to use it on the 3rd day.
                    DateTime trialStart = Convert.ToDateTime(IS.GetSetting(TRIAL_START_DATE));
                    DateTime trialEndDate = trialStart.AddDays(4);

                    //Step 3:  If the today is greater than or equal to the end date, trial is over 
                    if (DateTime.Today >= trialEndDate.Date)
                    {
                        //Trial OVER
                        appEnabled = false;
                    }
                }
            }

            return appEnabled;
        }

        #endregion app enabling and trial info

        public App()
        {
            // RootFrame = new TransitionFrame();

            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Language display initialization
            InitializeLanguage();

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            //TJY ADDED ON 5/5/17
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            CheckLicense();
            DAL.Instance().InitializeApplication();

            InitStore();
        }

        //TJY ADDED ON 5/5/17
        public static IEnumerable<Assembly> GetAssemblies()
        {
            var list = new List<string>();
            var stack = new Stack<Assembly>();

        //    stack.Push(Assembly.GetEntryAssembly());

            do
            {
                var asm = stack.Pop();

                yield return asm;

                foreach (var reference in asm.GetReferencedAssemblies())
                    if (!list.Contains(reference.FullName))
                    {
                        stack.Push(Assembly.Load(reference));
                        list.Add(reference.FullName);
                    }

            }
            while (stack.Count > 0);

        }

        private void InitStore()
        {
#if DEBUG
            MockIAPLib.MockIAP.Init();
            MockIAPLib.MockIAP.RunInMockMode(true);
            MockIAPLib.MockIAP.SetListingInformation(1, "en-us", "Some description", "1", "TestApp");

            // Add some more items manually.

            MockIAPLib.ProductListing p = new MockIAPLib.ProductListing
            {
                Name = "FullStats",
                ImageUri = new Uri("/Assets/StatsPickerPic.png", UriKind.Relative),
                ProductId = "FullStats",
                ProductType = Windows.ApplicationModel.Store.ProductType.Durable,
                Keywords = new string[] { "Full Stats" },
                Description = @"Full stats package includes: Player Minutes, Plus Minus,
                                Pass, Turnover, Offsides, 
                                Foul Committed, Out of Bounds, Cross, Throw In, Corner Kick, 
                                Tackle, Goalie Kick, Own Goal, Foul Drawn, Direct Free Kick,
                                Indirect Free Kick, Penalty Kick, Yellow Card, Red Card, Drop Kick,
                                Dribble, Shootout Kick, Substitution",
                FormattedPrice = "4.99",
                Tag = string.Empty
            };

            MockIAPLib.MockIAP.AddProductListing("FullStats", p);
#endif
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            CheckLicense();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            AllUsersConnection cloud = new AllUsersConnection();
            cloud.UpdateTimeInApp(DateTime.Now - _timeOpened);
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            // RootFrame = new PhoneApplicationFrame();
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        // Initialize the app's font and flow direction as defined in its localized resource strings.
        //
        // To ensure that the font of your application is aligned with its supported languages and that the
        // FlowDirection for each of those languages follows its traditional direction, ResourceLanguage
        // and ResourceFlowDirection should be initialized in each resx file to match these values with that
        // file's culture. For example:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage's value should be "es-ES"
        //    ResourceFlowDirection's value should be "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage's value should be "ar-SA"
        //     ResourceFlowDirection's value should be "RightToLeft"
        //
        // For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        private void InitializeLanguage()
        {
            try
            {
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }
    }
}