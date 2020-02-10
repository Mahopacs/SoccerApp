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
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using ScoreSoccer8.Utilities;

namespace ScoreSoccer8.Views
{
    public partial class Teams : PhoneApplicationPage
    {
        TeamsViewModel _vm;
        int teamId = 0;

        public Teams()
        {
            InitializeComponent();
            _vm = new TeamsViewModel();
            this.DataContext = _vm;

            LoadingPopup ovr = new LoadingPopup();
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
            loadingGrid.Children.Add(ovr);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _vm.Initialize();        
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
        }


        private void GoToTeamDetails_Click(object sender, RoutedEventArgs e)
        {

            teamId = (int)((Button)sender).Tag;
            loadingGrid.Visibility = System.Windows.Visibility.Visible;

            GoToTeamDetailsCommand.Execute(true);

        }

        private ICommand _goToTeamDetailsCommand;
        public ICommand GoToTeamDetailsCommand
        {
            get
            {
                if (_goToTeamDetailsCommand == null)
                {
                    _goToTeamDetailsCommand = new DelegateCommand(param => this.GoToTeamDetailsScreen(), param => true);
                }

                return _goToTeamDetailsCommand;
            }
        }

        private void GoToTeamDetailsScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/TeamDetails.xaml?parameter=" + teamId, UriKind.Relative));
        }


        private void AddTeam_Click(object sender, RoutedEventArgs e)
        {

            teamId = 0;
            loadingGrid.Visibility = System.Windows.Visibility.Visible;

            AddTeamClickCommand.Execute(true);

        }

        private ICommand _addTeamsClickCommand;
        public ICommand AddTeamClickCommand
        {
            get
            {
                if (_addTeamsClickCommand == null)
                {
                    _addTeamsClickCommand = new DelegateCommand(param => this.GoToTeamDetailsScreen(), param => true);
                }

                return _addTeamsClickCommand;
            }
        }




        private void GoRoster_Click(object sender, RoutedEventArgs e)
        {

            teamId = (int)((Button)sender).Tag;
            loadingGrid.Visibility = System.Windows.Visibility.Visible;

            GoToRosterCommand.Execute(true);

        }

        private ICommand _goToRosterCommand;
        public ICommand GoToRosterCommand
        {
            get
            {
                if (_goToRosterCommand == null)
                {
                    _goToRosterCommand = new DelegateCommand(param => this.GoToRostersScreen(), param => true);
                }

                return _goToRosterCommand;
            }
        }

        private void GoToRostersScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Rosters.xaml?teamID=" + teamId, UriKind.Relative));
        }

    }
}