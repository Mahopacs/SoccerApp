using Microsoft.Phone.Tasks;
using ScoreSoccer8.Classes;
using ScoreSoccer8.DataAccess;
using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ScoreSoccer8.ViewModels
{
    class MainPageViewModel : Notification
    {

        private string _imagePathLite = "/Assets/uTrackSoccerLite.png";
        private string _imagePathFull = "/Assets/uTrackSoccer.png";

        public MainPageViewModel()
        {
            FreeVisability = Visibility.Collapsed;
            RateVisability = Visibility.Collapsed;
            BuyMargin = new Thickness(0, 0, 0, 75);
            
            _imagePath = _imagePathFull;

            AppVersion = "v" + System.Reflection.Assembly.GetExecutingAssembly().FullName.Split('=')[1].Split(',')[0];

            CheckForFreeMode();

        }

        #region Commands
    
        private ICommand _teamsClickCommand;
        public ICommand TeamsClickCommand
        {
            get
            {
                if (_teamsClickCommand == null)
                {
                    _teamsClickCommand = new DelegateCommand(param => this.GoToTeamsScreen(), param => true);
                }

                return _teamsClickCommand;
            }
        }

        private ICommand _gamesClickCommand;
        public ICommand GamesClickCommand
        {
            get
            {
                if (_gamesClickCommand == null)
                {
                    _gamesClickCommand = new DelegateCommand(param => this.GoToGamesScreen(), param => true);
                }

                return _gamesClickCommand;
            }
        }

        private ICommand _userClickCommand;
        public ICommand UserClickCommand
        {
            get
            {
                if (_userClickCommand == null)
                {
                    _userClickCommand = new DelegateCommand(param => this.GoToUserScreen(), param => true);
                }

                return _userClickCommand;
            }
        }


        private ICommand _leaguesClickCommand;
        public ICommand LeaguesClickCommand
        {
            get
            {
                if (_leaguesClickCommand == null)
                {
                    _leaguesClickCommand = new DelegateCommand(param => this.GoToLeaguesScreen(), param => true);
                }

                return _leaguesClickCommand;
            }
        }
       
        private ICommand _playersClickCommand;
        public ICommand PlayersClickCommand
        {
            get
            {
                if (_playersClickCommand == null)
                {
                    _playersClickCommand = new DelegateCommand(param => this.GoToPlayersScreen(), param => true);
                }

                return _playersClickCommand;
            }
        }

        private ICommand _tutorialClickCommand;
        public ICommand TutorialClickCommand
        {
            get
            {
                if (_tutorialClickCommand == null)
                {
                    _tutorialClickCommand = new DelegateCommand(param => this.GoToTutorialScreen(), param => true);
                }

                return _tutorialClickCommand;
            }
        }


        private ICommand _aboutClickCommand;
        public ICommand AboutClickCommand
        {
            get
            {
                if (_aboutClickCommand == null)
                {
                    _aboutClickCommand = new DelegateCommand(param => this.GoToAboutScreen(), param => true);
                }

                return _aboutClickCommand;
            }
        }

        private ICommand _goToDemoClickCommand;
        public ICommand GoToDemoClickCommand
        {
            get
            {
                if (_goToDemoClickCommand == null)
                {
                    _goToDemoClickCommand = new DelegateCommand(param => this.GoToDemo(), param => true);
                }

                return _goToDemoClickCommand;
            }
        }

        #endregion  Commands

        #region "Commands For Testing"
    
        private ICommand _automatedTestingClickCommand;
        public ICommand AutomatedTestingClickCommand
        {
            get
            {
                if (_automatedTestingClickCommand == null)
                {
                    _automatedTestingClickCommand = new DelegateCommand(param => this.GoToAutomatedTestingScreen(), param => true);
                }

                return _automatedTestingClickCommand;
            }
        }

        private ICommand _statsPickerClickCommand;
        public ICommand StatsPickerClickCommand
        {
            get
            {
                if (_statsPickerClickCommand == null)
                {
                    _statsPickerClickCommand = new DelegateCommand(param => this.GoToStatsPickerScreen(), param => true);
                }

                return _statsPickerClickCommand;
            }
        }

        private ICommand _rateAppClickCommand;
        public ICommand RateAppClickCommand
        {
            get
            {
                if (_rateAppClickCommand == null)
                {
                    _rateAppClickCommand = new DelegateCommand(param => this.RateApp(), param => true);
                }

                return _rateAppClickCommand;
            }
        }

        private ICommand _goBuyAppClickCommand;
        public ICommand GoBuyAppClickCommand
        {
            get
            {
                if (_goBuyAppClickCommand == null)
                {
                    _goBuyAppClickCommand = new DelegateCommand(param => this.BuyApp(), param => true);
                }

                return _goBuyAppClickCommand;
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

        #endregion "Commands For Testing"
        
        #region Properties

        private Visibility _freeVisability;
        public Visibility FreeVisability
        {
            get { return _freeVisability; }
            set { _freeVisability = value; NotifyPropertyChanged("FreeVisability"); }
        }

        private Visibility _rateVisability;
        public Visibility RateVisability
        {
            get { return _rateVisability; }
            set { _rateVisability = value; NotifyPropertyChanged("RateVisability"); }
        }

        private Thickness _buyMargin;
        public Thickness BuyMargin
        {
            get { return _buyMargin; }
            set { _buyMargin = value; NotifyPropertyChanged("BuyMargin"); }
        }

        private string _appVersion;
        public string AppVersion
        {
            get { return _appVersion; }
            set { _appVersion = value; NotifyPropertyChanged("AppVersion"); }
        }

        private string _imagePath;
        public string ImagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; NotifyPropertyChanged("ImagePath"); }
        }

        #endregion
        
        #region "Methods"

        public void CheckForFreeMode()
        {
            if (App.DoesUserHaveAbilityToTrackAllStats() == false)
            {
                ImagePath = _imagePathLite;
                FreeVisability = Visibility.Visible;
            }
            else
            {
                ImagePath = _imagePathFull;
                FreeVisability = Visibility.Collapsed;
            }

            if (App.gHasAppBeenRated == "NO")
            {
                RateVisability = Visibility.Visible;
            }
            else
            {
                RateVisability = Visibility.Collapsed;
            }

        }
        
        private void GoHelpScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Help_Buttons.xaml?screenId=" + Enums.Screen.All, UriKind.Relative));
        }

        private void BuyApp()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/PurchaseApp.xaml", UriKind.Relative));
        }

        private void RateApp()
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
            IS.SaveSetting("AppRated", "YES");
            App.gHasAppBeenRated = "YES";
            DAL.Instance().SetUpStatsForApp(); //10/25/14 TJY If Rate app give all stats functionality
        }

        private void GoToDemo()
        {
            int demoGameID = DAL.Instance().GetDemoGameGameID();
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/GameManagerView.xaml?gameid=" + demoGameID, UriKind.Relative));
        }

        private void GoToAboutScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/About.xaml", UriKind.Relative));
        }

        private void GoToTestScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Testing/ControlTest.xaml", UriKind.Relative));
        }

        private void GoToTeamsScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Teams.xaml", UriKind.Relative));
        }

        private void GoToGamesScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Games.xaml", UriKind.Relative));
        }

        private void GoToUserScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/UserScreen.xaml", UriKind.Relative));
        }
        
        private void GoToLeaguesScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Leagues.xaml", UriKind.Relative));
        }

        private void GoToPlayersScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/PlayerList.xaml?gameID=0&teamID=0&teamName=''", UriKind.Relative));           
        }
      
        private void GoToTutorialScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Tutorial.xaml", UriKind.Relative));
        }

        #endregion "Methods"

        #region "Methods For Testing"

        private void GoToAutomatedTestingScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Testing/FlatTotals.xaml", UriKind.Relative));
        }

        private void GoToStatsPickerScreen()
        {
            string statsPickerNavigateString;
            statsPickerNavigateString = "?gameID=1&teamID=1&playerID=1&period=1&awayScore=2&homeScore=1&gameTime=11:23&playerPosition=F&otherTeamGoalieID=3";
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/StatsPicker.xaml" + statsPickerNavigateString, UriKind.Relative));
        }

     

        #endregion "Methods For Testing"
    }
}
