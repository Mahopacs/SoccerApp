using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.ComponentModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ScoreSoccer8.ViewModels;
using ScoreSoccer8.Resources;
using ScoreSoccer8.Utilities;

namespace ScoreSoccer8.Views
{
    public partial class PlayDetails : PhoneApplicationPage
    {
        private PlayDetailsViewModel _vm;
        private bool listPickerMode = false; //needed because listpicker open and closing fires on navigated to and from, so need to know if you 
        //are coming from listpicker or going to listpicker
        private string _messageBoxTitle = AppResources.PlayDetails;

        public PlayDetails()
        {
            InitializeComponent();
            _vm = new PlayDetailsViewModel();
            this.DataContext = _vm;

            LoadingPopup ovr = new LoadingPopup();
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
            loadingGrid.Children.Add(ovr);
        }

        private void BackButtonClicked(object sender, CancelEventArgs e)
        {
            loadingGrid.Visibility = System.Windows.Visibility.Visible;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            loadingGrid.Visibility = System.Windows.Visibility.Visible;
            
            _vm.OkClickCommand.Execute(e);

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
                        int gameID = Convert.ToInt32(NavigationContext.QueryString["gameID"]);
                        int playID = Convert.ToInt32(NavigationContext.QueryString["playID"]);

                        _vm.Initialize(gameID, playID);
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

           

            if ((e.Uri.OriginalString == "/Microsoft.Phone.Controls.Toolkit;component/ListPicker/ListPickerPage.xaml") ||
                 (e.Uri.OriginalString == "/Coding4Fun.Toolkit.Controls;component/ValuePicker/TimeSpanPicker/TimeSpanPickerPage.xaml") ||
                (e.Uri.OriginalString == "/Views/Help_Buttons.xaml?screenId=PlayDetails"))
            {
                listPickerMode = true;
            }
            else
            {

                //This is needed because ShotDetails would simply go back to main stat picker, we want it to go back from where it was
                //called from (i.e. Game Manager), so need to really go back 1 more...
                //NavigationService.GoBack();
                if ((_vm.CancelButtonClicked == false)  && (_vm.OkButtonClicked == false))
                {
                    if (_vm.ValidateScreen())
                    {
                        _vm.OkClickCommand.Execute(e);
                    }
                }
            }

            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
        }

        private bool ValidateScreen()
        {
            bool rtnValue = true;
            int elapsedTimeInGameInSeconds;

            try
            {
                elapsedTimeInGameInSeconds = StatCalculationsModule.CalculateTimeElapsedInGameInSeconds(_vm.Game.Game, _vm.Game.Game.CurrentPeriod, _vm.Game.Game.CurrentClock);

                if (_vm.Clock == string.Empty || _vm.Clock == null)
                {
                    MessageBox.Show(AppResources.ClockNotEntered, _messageBoxTitle, MessageBoxButton.OK);
                    rtnValue = false;
                }
                else
                {
                    //Make sure this is not a period/clock time that is in the future, that is what Game Manager is for.
                    int elapsedTimeOfThisPlayInSeconds = StatCalculationsModule.CalculateTimeElapsedInGameInSeconds(_vm.Game.Game, Convert.ToInt32(_vm.SelectedPeriod), _vm.Clock);

                    if (elapsedTimeOfThisPlayInSeconds > elapsedTimeInGameInSeconds)
                    {
                        MessageBox.Show(AppResources.PeriodClockValueInFuture, _messageBoxTitle, MessageBoxButton.OK);
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
    }
}