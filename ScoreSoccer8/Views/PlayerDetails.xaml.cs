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
using System.Windows.Input;
using ScoreSoccer8.Resources;
using ScoreSoccer8.Utilities;
using System.ComponentModel;

namespace ScoreSoccer8.Views
{
    public partial class PlayerDetails : PhoneApplicationPage
    {
        private PlayerDetailsViewModel _vm;
        private bool listPickerMode = false; //needed because listpicker open and closing fires on navigated to and from, so need to know if you 
        //are coming from listpicker or going to listpicker
        private string _messageBoxTitle = AppResources.PlayerDetails;

        public PlayerDetails()
        {
            InitializeComponent();
            _vm = new PlayerDetailsViewModel();
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
            txtFirstName.Focus();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (listPickerMode == false)
            {
                if (NavigationContext.QueryString.Count > 0)
                {
                    int gameID = Convert.ToInt32(NavigationContext.QueryString["gameID"]);
                    int teamID = Convert.ToInt32(NavigationContext.QueryString["teamID"]);
                    int playerID = Convert.ToInt32(NavigationContext.QueryString["playerID"]);

                    _vm.Initialize(gameID, teamID, playerID);
                }
                else
                {
                    listPickerMode = true;
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if ((e.Uri.OriginalString == "/Microsoft.Phone.Controls.Toolkit;component/DateTimePickers/DatePickerPage.xaml") ||
              (e.Uri.OriginalString == "/Views/Help_Buttons.xaml?screenId=PlayerDetails"))
            {
                listPickerMode = true;
            }
            else
            {
                if (ValidateScreen() == true)
                {
                    _vm.PlayerDetails.BirthDate = _vm.BirthDate;
                    _vm.SaveToDatabase(_vm.TeamID);
                }
            }

            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void txtWeight_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Decimal || e.Key == Key.Unknown)
            {
                e.Handled = true;
            }

            if (e.Key == Key.Enter)
            {
                btnKicksLeft.Focus();
            }
        }

        private void txtJerseyNumber_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Decimal || e.Key == Key.Unknown)
            {
                e.Handled = true;
            }

            if (e.Key == Key.Enter)
            {
                btnActive.Focus();
            }
        }

        private void txtFirstName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtLastName.Focus();
            }
        }

        private void txtLastName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtHeight.Focus();
            }
        }

        private void txtHeight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtWeight.Focus();
            }
        }


        #endregion "Events"

        #region "Methods"

        private bool ValidateScreen()
        {
            bool rtnValue = true;

            try
            {
                if (_vm.PlayerDetails.Visible == "N")
                {
                    MessageBox.Show(AppResources.CannotChangeDeletedPlayerInfo, _messageBoxTitle, MessageBoxButton.OK);
                    rtnValue = false;
                }
                else if (_vm.PlayerDetails.FirstName == string.Empty || _vm.PlayerDetails.FirstName == null)
                {
                    //if player first name empty we can not save, if the rest of the screen is empty then user simply came to this screen and hit the back button so no need to display "not saved message"
                    if ((_vm.PlayerDetails.LastName == string.Empty || _vm.PlayerDetails.LastName == null) &&
                    (_vm.PlayerDetails.Height == string.Empty || _vm.PlayerDetails.Height == null) &&
                        (_vm.PlayerDetails.Weight == 0 || _vm.PlayerDetails.Weight == null))
                    {

                    }
                    else
                    {
                        MessageBox.Show(AppResources.FirstNameNotEntered, _messageBoxTitle, MessageBoxButton.OK);
                    }
                    rtnValue = false;
                }
                else if (_vm.UniformNumber != null)
                {
                    if (_vm.UniformNumber.Length > 3)
                    {
                        MessageBox.Show(AppResources.JerseyNumberInvalidPlayerNotSaved, _messageBoxTitle, MessageBoxButton.OK);
                        rtnValue = false;
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
