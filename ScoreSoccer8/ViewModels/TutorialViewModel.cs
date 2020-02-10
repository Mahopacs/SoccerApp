using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ScoreSoccer8.ViewModels
{
    public class TutorialViewModel : Notification
    {
        public TutorialViewModel()
        {
            CurrentPage = 1;
            TotalNumberOfPages = 16;
            SetDisplay();
            IsPreviousEnabled = false;
            IsNextEnabled = true;
        }

        #region "Properties"

        private int _currentPage;
        public int CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value; NotifyPropertyChanged("CurrentPage"); }
        }

        private int _totalNumberOfPages;
        public int TotalNumberOfPages
        {
            get { return _totalNumberOfPages; }
            set { _totalNumberOfPages = value; NotifyPropertyChanged("TotalNumberOfPages"); }
        }

        private string _picToDisplay;
        public string PicToDisplay
        {
            get { return _picToDisplay; }
            set { _picToDisplay = value; NotifyPropertyChanged("PicToDisplay"); }
        }

        private string _titleToDisplay;
        public string TitleToDisplay
        {
            get { return _titleToDisplay; }
            set { _titleToDisplay = value; NotifyPropertyChanged("TitleToDisplay"); }
        }

        private string _textToDisplay;
        public string TextToDisplay
        {
            get { return _textToDisplay; }
            set { _textToDisplay = value; NotifyPropertyChanged("TextToDisplay"); }
        }

        private string _pageCountToDisplay;
        public string PageCountToDisplay
        {
            get { return _pageCountToDisplay; }
            set { _pageCountToDisplay = value; NotifyPropertyChanged("PageCountToDisplay"); }
        }

        private bool _isPreviousEnabled;
        public bool IsPreviousEnabled
        {
            get { return _isPreviousEnabled; }
            set { _isPreviousEnabled = value; NotifyPropertyChanged("IsPreviousEnabled"); }
        }

        private bool _isNextEnabled;
        public bool IsNextEnabled
        {
            get { return _isNextEnabled; }
            set { _isNextEnabled = value; NotifyPropertyChanged("IsNextEnabled"); }
        }

        #endregion "Properties"

        #region "Commands"

        private ICommand _previousClickCommand;
        public ICommand PreviousClickCommand
        {
            get
            {
                if (_previousClickCommand == null)
                {
                    _previousClickCommand = new DelegateCommand(param => this.GoToPreviousClicked(), param => true);
                }

                return _previousClickCommand;
            }
        }

        private ICommand _nextClickCommand;
        public ICommand NextClickCommand
        {
            get
            {
                if (_nextClickCommand == null)
                {
                    _nextClickCommand = new DelegateCommand(param => this.GoToNextClicked(), param => true);
                }

                return _nextClickCommand;
            }
        }

        private ICommand _closeTutorialClickCommand;
        public ICommand CloseTutorialClickCommand
        {
            get
            {
                if (_closeTutorialClickCommand == null)
                {
                    _closeTutorialClickCommand = new DelegateCommand(param => this.GoToCloseTutorialClicked(), param => true);
                }

                return _closeTutorialClickCommand;
            }
        }

        #endregion "Commands"

        #region "Methods"

        private void GoToPreviousClicked()
        {
            if (CurrentPage > 1)
            {
                IsPreviousEnabled = true;
                IsNextEnabled = true;
                CurrentPage = CurrentPage - 1;
                SetDisplay();
            }

            if (CurrentPage == 1)
            {
                IsPreviousEnabled = false;
                IsNextEnabled = true;
            }
        }
        private void GoToNextClicked()
        {
            if (CurrentPage < TotalNumberOfPages)
            {
                IsNextEnabled = true;
                IsPreviousEnabled = true;
                CurrentPage = CurrentPage + 1;
                SetDisplay();
            }

            if (CurrentPage == TotalNumberOfPages)
            {
                IsNextEnabled = false;
                IsPreviousEnabled = true;
            }
        }

        private void GoToCloseTutorialClicked()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/MainPage.xaml", UriKind.Relative));
        }

        private void SetDisplay()
        {
            PageCountToDisplay = "Page " + CurrentPage + " out of " + TotalNumberOfPages;
            switch (CurrentPage)
            {
                case 1:
                    PicToDisplay = "/Assets/Fields/SoccerField1.png";
                    TitleToDisplay = "Welcome!";
                    TextToDisplay = "UTrack Soccer is the most complete, intuitive, flexible, and accurate soccer scoring application on the market. " +
                        "With UTrack Soccer you can track over 50 different stats: including goals, substiutions, passes, and shots.";
                    break;
                case 2:
                    PicToDisplay = "/Assets/Tutorial/TeamDetailsScreen.png";
                    TitleToDisplay = "Setup Teams";
                    TextToDisplay = "Before you can start tracking stats for a game, you need to setup the teams." + Environment.NewLine + Environment.NewLine +
                        "To setup teams click on the 'Teams' button and then click on the 'Add Team' button at the bottom of the screen. Enter in the team information, " +
                        "only the team name is a required field.";
                    break;
                case 3:
                    PicToDisplay = "/Assets/Tutorial/PlayerDetailsScreen.png";
                    TitleToDisplay = "Setup Players";
                    TextToDisplay = "Before you can add players to a team, you first need to add players to the global list of players." + Environment.NewLine + Environment.NewLine +
                        "To add players to the global list of players click on the 'Players' button and then click on the 'Add Player' button at the bottom of the screen.  Enter " +
                        "in the player information, only the players first name is a required field.";
                    break;
                case 4:
                    PicToDisplay = "/Assets/Tutorial/PlayerListAddPlayer.png";
                    TitleToDisplay = "Setup Team Roster";
                    TextToDisplay = "Once you have setup teams and a list of players you are now ready to build the team roster." + Environment.NewLine + Environment.NewLine +
                        "To setup team roster click on the 'Teams' button and then click on the roster icon for the team you are trying to create a roster for." + Environment.NewLine + Environment.NewLine +
                        "Once on the team roster screen you then click on the 'Add Player To Team Roster' button on the bottom of the screen." + Environment.NewLine + Environment.NewLine +
                        "The 'Player List' screen will be displayed, find the player you want to add to the team and click the add icon to the left of the player name.  After selecting a player you will be " +
                        "prompted to enter the players jersey number.  Repeat this procedure for all of the players you want to add to the team.";
                    break;
                case 5:
                    PicToDisplay = "/Assets/Tutorial/GameDetailsScreen.png";
                    TitleToDisplay = "Setup Games";
                    TextToDisplay = "Once you have setup teams, players, and team rosters, you can setup a game." + Environment.NewLine + Environment.NewLine +
                    "To setup games click on the 'Games' button and then clcik on the 'Add Game' button at the bottom of the screen. Enter in game information, " +
                    "all fields are required.";
                    break;
                case 6:
                    PicToDisplay = "/Assets/Fields/SoccerField6.png";
                    TitleToDisplay = "Track Game";
                    TextToDisplay = "Now that you have setup teams, players, team rosters, and a game, you are ready to start tracking the game." + Environment.NewLine + Environment.NewLine +
                    "To start tracking a game click on the 'Games' button and then find the game you want to track and click the clipboard icon for that game.  This will bring up the game manager screen. " +
                    "From this screen you will track all the plays for the game." + Environment.NewLine + Environment.NewLine +
                    "The following pages will explain how various parts of this screen work.";
                    break;
                case 7:
                    PicToDisplay = "/Assets/Fields/SoccerField7.png";
                    TitleToDisplay = "Track Game - Plays";
                    TextToDisplay = "To track plays, simply click on a player while in 'Track' mode.  This will bring up the 'StatsPicker' screen, where you can select the play." + Environment.NewLine + Environment.NewLine +
                    "Some plays have sub descriptions (i.e. shot details, type of red card...).";
                    break;
                case 8:
                    PicToDisplay = "/Assets/Fields/SoccerField8.png";
                    TitleToDisplay = "Track Game - Subs";
                    TextToDisplay = "To enter a substitution you need to put the game into 'Sub' mode.  To do this click on the Menu option on the bottom right of the screen." + Environment.NewLine + Environment.NewLine +
                    "This will display each teams bench, simply click on the player to put in the game and click on the player to take out of the game. When you are finished entering subs place the game back in 'Track' mode by clicking on the 'Track' button.";
                    break;
                case 9:
                    PicToDisplay = "/Assets/Fields/SoccerField9.png";
                    TitleToDisplay = "Track Game - Edit Play";
                    TextToDisplay = "To edit a play, click on the Menu option on the bottom right of the screen." + Environment.NewLine + Environment.NewLine +
                    "Then click on 'More Menu Items' and click the 'Edit' button.  This will bring up the play by play screen.  From this screen you can add, delete, or edit a play.  To edit a play simply click on the play and edit any parts of the play.";
                    break;
                case 10:
                    PicToDisplay = "/Assets/Fields/SoccerField10.png";
                    TitleToDisplay = "Track Game - Undo PLay";
                    TextToDisplay = "To undo a play, click on the Menu option on the bottom right of the screen." + Environment.NewLine + Environment.NewLine +
                    "Then click on 'More Menu Items' and click the 'Undo' button.  This will automatically delete the last play of the game.";
                    break;
                case 11:
                    PicToDisplay = "/Assets/Fields/SoccerField10.png";
                    TitleToDisplay = "Track Game - Add Player";
                    TextToDisplay = "To undo a play, click on the Menu option on the bottom right of the screen." + Environment.NewLine + Environment.NewLine +
                    "Then click on '+' button.  This will bring up the list of global players, on this screen find the player you want to add and click the '+' button to the left of their name. " +
                    "You will then be prompted to enter in the players jersey number." + Environment.NewLine + Environment.NewLine +
                    "After entering the jersey number you will be returned to the game manager screen with that player on the teams bench.";
                    break;
                case 12:
                    PicToDisplay = "/Assets/Fields/SoccerField10.png";
                    TitleToDisplay = "Track Game - Adjust Clock";
                    TextToDisplay = "To adjust the clock simply click on the clock and enter the time you would like to set the clock to.";
                    break;
                case 13:
                    PicToDisplay = "/Assets/Fields/SoccerField10.png";
                    TitleToDisplay = "Track Game - Stats";
                    TextToDisplay = "To view stats for the game, click on the Menu option on the bottom right of the screen." + Environment.NewLine + Environment.NewLine +
                     "Then click on 'More Menu Items' and click the 'Stats' button.  This will bring up the stats report screen where you can view the game timeline, team stats, player stats, and play by play.";
                    break;
                case 14:
                    PicToDisplay = "/Assets/Fields/SoccerField10.png";
                    TitleToDisplay = "FAQ";
                    TextToDisplay = "";
                    break;
                case 15:
                    PicToDisplay = "/Assets/Fields/SoccerField10.png";
                    TitleToDisplay = "Coming Soon";
                    TextToDisplay = "";
                    break;
                case 16:
                    PicToDisplay = "/Assets/Fields/SoccerField10.png";
                    TitleToDisplay = "Feedback";
                    TextToDisplay = "";
                    break;
                default:
                    break;
            }
        }

        #endregion "Methods"
    }
}
