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
using ScoreSoccer8.Resources;
using System.Windows.Input;
using System.ComponentModel;

namespace ScoreSoccer8.Views
{
    public partial class LeagueDetails : PhoneApplicationPage
    {
        private LeagueDetailsViewModel _vm;
        private bool listPickerMode = false; //needed because listpicker open and closing fires on navigated to and from, so need to know if you 
        //are coming from listpicker or going to listpicker
        private string _messageBoxTitle = AppResources.LeagueDetails;

        public LeagueDetails()
        {
            InitializeComponent();
            _vm = new LeagueDetailsViewModel();
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
            txtLeagueName.Focus();
        } 

        private void txtLeagueName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtContactName.Focus();
            }
        }

        private void txtContactName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtContactNumber.Focus();
            }
        }

        private void txtContactNumber_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

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
                        int leagueID = Convert.ToInt32(NavigationContext.QueryString["parameter"]);

                        _vm.Initialize(leagueID);
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

            if ((e.Uri.OriginalString != "/Microsoft.Phone.Controls.Toolkit;component/ListPicker/ListPickerPage.xaml") &&
                (e.Uri.OriginalString != "/Views/Help_Buttons.xaml?screenId=LeagueDetails"))
            {                
                 if (ValidateScreen())
                 {
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
                if (_vm.LeagueDetails.League.LeagueName == string.Empty || _vm.LeagueDetails.League.LeagueName == null)
                {
                    //if league name empty we can not save, if the rest of the screen is empty then user simply came to this screen and hit the back button so no need to display "not saved message"
                    if ((_vm.LeagueDetails.League.LeagueContactName == string.Empty || _vm.LeagueDetails.League.LeagueContactName == null) &&
                    (_vm.LeagueDetails.League.LeagueContactNumber == string.Empty || _vm.LeagueDetails.League.LeagueContactNumber == null))
                    {

                    }
                    else
                    {
                        MessageBox.Show(AppResources.LeagueNotEnteredValidationMessage, _messageBoxTitle, MessageBoxButton.OK);
                    }
                    rtnValue = false;
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