using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;

namespace ScoreSoccer8.ViewModels
{
    public class PlayListViewModel : Notification
    {

        public PlayListViewModel()
        {
            if (Messaging.HideLoadingScreen == null)
                Messaging.HideLoadingScreen += CloseLoadingScreen;

            if (Messaging.ShowLoadingScreen == null)
                Messaging.ShowLoadingScreen += ShowLoadingScreen;

            LoadingScreenVisibility = System.Windows.Visibility.Collapsed;
            TestString = "First";
        }

        #region "Properties"

        private ObservableCollection<PlayModel> _playByPlayList;
        public ObservableCollection<PlayModel> PlayByPlayList
        {
            get { return _playByPlayList; }
            set { _playByPlayList = value; NotifyPropertyChanged("PlayByPlayList"); }
        }

        private int _gameID;
        public int GameID
        {
            get { return _gameID; }
            set { _gameID = value; NotifyPropertyChanged("GameID"); }
        }

        #endregion "Properties"

        #region "Commands"

        private ICommand _addPlayClickCommand;
        public ICommand AddPlayClickCommand
        {
            get
            {
                if (_addPlayClickCommand == null)
                {
                    _addPlayClickCommand = new DelegateCommand(param => this.GoToPlayDetailsScreen(), param => true);
                }

                return _addPlayClickCommand;
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

        #endregion "Commands"

        #region "Methods"

        private void GoHelpScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Help_Buttons.xaml?screenId=" + Enums.Screen.PlayList, UriKind.Relative));
        }

        public void Initialize(int gameID)
        {
            GameID = gameID;
            PopulatePlayList();
        }

        private void GoToPlayDetailsScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/PlayDetails.xaml?gameID=" + GameID + "&playID=0", UriKind.Relative));
        }

        private void PopulatePlayList()
        {
            PlayByPlayList = DAL.Instance().GetPlaysForGame(GameID, "DESC");

            int i = 0;
            foreach (PlayModel item in PlayByPlayList)
            {
                item.PlayDeleted += item_PlayDeleted;

                if (i % 2 != 0)
                {
                    item.BackgroundColor = "White";
                    item.BackgroundOpacity = 0.3;
                }

                i++;
            }
        }

        void item_PlayDeleted(object sender, EventArgs e)
        {
            PopulatePlayList();
        }

        private void ShowLoadingScreen(object sender, EventArgs e)
        {
            //Debug.WriteLine("Open");
            TestString = "Opening";
            LoadingScreenVisibility = System.Windows.Visibility.Visible;
        }

        private void CloseLoadingScreen(object sender, EventArgs e)
        {
            //Debug.WriteLine("Close");
            TestString = "Closing";
            LoadingScreenVisibility = System.Windows.Visibility.Collapsed;
        }

        private Visibility _loadingScreenVisibility = System.Windows.Visibility.Collapsed;
        public Visibility LoadingScreenVisibility
        {
            get { return _loadingScreenVisibility; }
            set { _loadingScreenVisibility = value; NotifyPropertyChanged("LoadingScreenVisibility"); }
        }

        private string _testString;
        public string TestString
        {
            get { return _testString; }
            set { _testString = value; NotifyPropertyChanged("TestString"); }
        }

        #endregion "Methods"
    }
}
