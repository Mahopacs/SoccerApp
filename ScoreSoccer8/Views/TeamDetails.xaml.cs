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
using ScoreSoccer8.Resources;
using System.Windows.Input;

namespace ScoreSoccer8.Views
{
    public partial class TeamDetails : PhoneApplicationPage
    {
        private TeamDetailsViewModel _vm;
        private bool listPickerMode = false; //needed because listpicker open and closing fires on navigated to and from, so need to know if you 
        //are coming from listpicker or going to listpicker
        private string _messageBoxTitle = AppResources.TeamDetails;

        public TeamDetails()
        {
            InitializeComponent();
            _vm = new TeamDetailsViewModel();
            this.DataContext = _vm;

            LoadingPopup ovr = new LoadingPopup();
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
            loadingGrid.Children.Add(ovr);
        }

        private void BackButtonClicked(object sender, CancelEventArgs e)
        {
            loadingGrid.Visibility = System.Windows.Visibility.Visible;
        }

        #region "Events"

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            txtTeamName.Focus();
        }

        private void txtTeamName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtShortName.Focus();
            }
        }
        private void txtShortName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtContactNumber.Focus();
            }
        }

        private void txtContactNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtCoach.Focus();
            }
        }

        private void txtCoach_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                listPickerJersey.Focus();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (listPickerMode == false)
            {
                if (e.Uri.OriginalString != "/Microsoft.Phone.Controls.Toolkit;component/ListPicker/ListPickerPage.xaml")
                {
                    if (NavigationContext.QueryString.Count > 0)
                    {
                        int teamID = Convert.ToInt32(NavigationContext.QueryString["parameter"]);

                        _vm.Initialize(teamID);
                    }
                }
            }
            else
            {
                listPickerMode = true;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (e.Uri.OriginalString != "/Microsoft.Phone.Controls.Toolkit;component/ListPicker/ListPickerPage.xaml")
            {
                if (ValidateScreen())
                {
                    if (_vm.SelectedColor != null)
                    {
                        _vm.TeamDetails.Team.Color = _vm.SelectedColor.ColorName;

                    }

                    if (_vm.SelectedJersey != null)
                    {
                        _vm.TeamDetails.Team.JerseyID = _vm.SelectedJersey.JerseyID;

                    }

                    if (_vm.SelectedLeague != null)
                    {
                        _vm.TeamDetails.Team.LeagueID = _vm.SelectedLeague.League.LeagueID;

                    }


                    _vm.SaveToDatabase();
                }
            }
            else
            {
                listPickerMode = true;
            }

            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
        }

        #endregion "Events"

        #region "Methods"

        private bool ValidateScreen()
        {
            bool rtnValue = true;

            try
            {
                if (_vm.TeamDetails.Team.TeamName == string.Empty || _vm.TeamDetails.Team.TeamName == null)
                {
                    //if team name empty we can not save, if the rest of the screen is empty then user simply came to this screen and hit the back button so no need to display "not saved message"
                    if ((_vm.TeamDetails.Team.TeamShortName == string.Empty || _vm.TeamDetails.Team.TeamShortName == null) &&
                    (_vm.TeamDetails.Team.ContactNumber == string.Empty || _vm.TeamDetails.Team.ContactNumber == null) &&
                        (_vm.TeamDetails.Team.Coach == string.Empty || _vm.TeamDetails.Team.Coach == null) &&
                        (_vm.TeamDetails.Team.ContactNumber == string.Empty || _vm.TeamDetails.Team.ContactNumber == null))
                    {

                    }
                    else
                    {
                        MessageBox.Show(AppResources.TeamNotEnteredValidationMessage, _messageBoxTitle, MessageBoxButton.OK);
                    }

                    rtnValue = false;
                }
                else //If TeamShortName is blank make it the first 2 characters of the TeamName
                {
                    if (_vm.TeamDetails.Team.TeamShortName == string.Empty || _vm.TeamDetails.Team.TeamShortName == null)
                    {
                        if (_vm.TeamDetails.Team.TeamName.Length >= 2)
                        {
                            _vm.TeamDetails.Team.TeamShortName = _vm.TeamDetails.Team.TeamName.Substring(0, 2).ToUpper();
                        }
                    }
                }
                return rtnValue;
            }
            catch (Exception ex)
            {
                return rtnValue;
            }
        }

        #endregion "Methods"
     
    }
}