using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ScoreSoccer8.ViewModels;
using Coding4Fun.Toolkit.Controls;
using ScoreSoccer8.Classes;
using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.DataAccess;
using System.Windows.Input;
using ScoreSoccer8.Resources;
using ScoreSoccer8.Utilities;

namespace ScoreSoccer8.Views
{
    public partial class PlayerList : PhoneApplicationPage
    {

        private string _messageBoxTitle = AppResources.PlayerList;
        private PlayerListViewModel _vm;
        int _playerId = 0;

        public PlayerList()
        {
            InitializeComponent();
            _vm = new PlayerListViewModel();
            this.DataContext = _vm;

            LoadingPopup ovr = new LoadingPopup();
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
            loadingGrid.Children.Add(ovr);
        }

        #region "Properties"

        private int _gameID;
        public int GameID
        {
            get { return _gameID; }
            set { _gameID = value; }
        }

        private int _teamID;
        public int TeamID
        {
            get { return _teamID; }
            set { _teamID = value; }
        }

        private string _teamName;
        public string TeamName
        {
            get { return _teamName; }
            set { _teamName = value; }
        }


        #endregion "Properties"


        private void GoToPlayerDetail_Click(object sender, RoutedEventArgs e)
        {

            _playerId = (int)((Button)sender).Tag;
            loadingGrid.Visibility = System.Windows.Visibility.Visible;

            GoToPlayerDetailsCommand.Execute(true);

        }

        private void AddPlayer_Click(object sender, RoutedEventArgs e)
        {

            _playerId = 0;
            loadingGrid.Visibility = System.Windows.Visibility.Visible;

            GoToPlayerDetailsCommand.Execute(true);

        }


        private ICommand _goToPlayerDetailsCommand;
        public ICommand GoToPlayerDetailsCommand
        {
            get
            {
                if (_goToPlayerDetailsCommand == null)
                {
                    _goToPlayerDetailsCommand = new DelegateCommand(param => this.GoToPlayerDetailsScreen(), param => true);
                }

                return _goToPlayerDetailsCommand;
            }
        }

        private void GoToPlayerDetailsScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/PlayerDetails.xaml?gameID=0" + "&teamID=0" + "&playerID=" + _playerId, UriKind.Relative));
        }


        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            PhoneApplicationService.Current.State["LastPage"] = "PlayerList";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.Count > 0)
            {
                GameID = Convert.ToInt32(NavigationContext.QueryString["gameID"]);
                TeamID = Convert.ToInt32(NavigationContext.QueryString["teamID"]);
                TeamName = NavigationContext.QueryString["teamName"];

                _vm.Initialize(GameID, TeamID, TeamName);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
        }
       
        //This will work 99.9% of the time.  The only time it will not work is if user backspaces to get to a 'S'
        private void txtSearchBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string searchString = string.Empty;

            if ((e.Key >= Key.A && e.Key <= Key.Z) || e.Key == Key.Back)
            {
                if (e.Key != Key.Back)
                {
                    searchString = txtSearchBox.Text + e.Key;
                    _vm.ReDisplaySorted(GameID, TeamID, TeamName, searchString);
                }
                else
                {
                    if (txtSearchBox.Text.Length != 0)
                    {
                        searchString = txtSearchBox.Text.Substring(0, txtSearchBox.Text.Length - 1);
                        if ((searchString != "Search") && (searchString != "Searc") && (searchString != "Sear") && (searchString != "Sea") && (searchString != "Se") && (searchString != "S"))
                        {
                            _vm.ReDisplaySorted(GameID, TeamID, TeamName, searchString);
                        }
                    }
                }
            }
        }
    }
}
