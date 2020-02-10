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

namespace ScoreSoccer8.Views
{
    public partial class GameDetails : PhoneApplicationPage
    {
        private GameDetailsViewModel _vm;
        private bool listPickerMode = false; //needed because listpicker open and closing fires on navigated to and from, so need to know if you 
        //are coming from listpicker or going to listpicker
        private string _messageBoxTitle = AppResources.GameDetails;

        public GameDetails()
        {
            InitializeComponent();
            _vm = new GameDetailsViewModel();
            this.DataContext = _vm;
        }

        #region "Events"
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (listPickerMode == false)
            {
                if (NavigationContext.QueryString.Count > 0)
                {
                    int gameID = Convert.ToInt32(NavigationContext.QueryString["parameter"]);
                    _vm.Initialize(gameID);
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

            if (e.Uri.OriginalString != "/Microsoft.Phone.Controls.Toolkit;component/ListPicker/ListPickerPage.xaml" &&
                e.Uri.OriginalString != "/Microsoft.Phone.Controls.Toolkit;component/DateTimePickers/DatePickerPage.xaml" &&
                e.Uri.OriginalString != "/Microsoft.Phone.Controls.Toolkit;component/DateTimePickers/TimePickerPage.xaml" &&
                e.Uri.OriginalString != "/Views/Help_Buttons.xaml?screenId=GameDetails")
            {
                if ((_vm.GameDetails.Game.GameStatus == "NOT STARTED") || (_vm.GameDetails.Game.GameStatus == null))
                {
                    if (_vm.SelectedHomeTeam != null && _vm.SelectedAwayTeam != null)
                    {
                        _vm.GameDetails.Game.AwayTeamID = _vm.SelectedAwayTeam.Team.TeamID;
                        _vm.GameDetails.AwayTeam.TeamName = _vm.SelectedAwayTeam.Team.TeamName;
                        _vm.GameDetails.Game.HomeTeamID = _vm.SelectedHomeTeam.Team.TeamID;
                        _vm.GameDetails.HomeTeam.TeamName = _vm.SelectedHomeTeam.Team.TeamName;
                        _vm.GameDetails.Game.GameDate = _vm.GameDate;
                        _vm.GameDetails.Game.GameTime = _vm.GameTime;

                        if (_vm.PeriodLength == null)
                        {
                            _vm.GameDetails.Game.PeriodLength = 0;
                        }
                        else
                        {
                            _vm.GameDetails.Game.PeriodLength = (int)_vm.PeriodLength;
                        }

                        if (_vm.OverTimeLength == null)
                        {
                            _vm.GameDetails.Game.OverTimeLength = 0;
                        }
                        else
                        {
                            _vm.GameDetails.Game.OverTimeLength = (int)_vm.OverTimeLength;
                        }
                    }
                    if (ValidateScreen() == true)
                    {
                        _vm.SaveToDatabase();
                    }
                }
            }
            else
            {
                listPickerMode = true;
            }
        }

        private void txtPeriodLength_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Decimal || e.Key == Key.Unknown)
            {
                e.Handled = true;
            }
        }

        private void txtOTLength_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Decimal || e.Key == Key.Unknown)
            {
                e.Handled = true;
            }
        }

        #endregion "Events"

        #region "Methods"

        private bool ValidateScreen()
        {
            bool rtnValue = true;

            try
            {
                //if both home and away teams are -1 (i.e. Select Team), then user was likely not intending to save so simply do not save without a warning message
                if ((_vm.GameDetails.Game.AwayTeamID == -1) && (_vm.GameDetails.Game.HomeTeamID == -1))
                {
                    rtnValue = false;
                }
                else if ((_vm.GameDetails.Game.AwayTeamID == 0) || (_vm.GameDetails.Game.AwayTeamID == -1)) 
                {
                    MessageBox.Show(AppResources.AwayTeamNotSeleted, _messageBoxTitle, MessageBoxButton.OK);
                    rtnValue = false;
                }
                else if ((_vm.GameDetails.Game.HomeTeamID == 0) || (_vm.GameDetails.Game.HomeTeamID == -1)) 
                {
                    MessageBox.Show(AppResources.HomeTeamNotSelected, _messageBoxTitle, MessageBoxButton.OK);
                    rtnValue = false;
                }
                else if (_vm.GameDetails.Game.AwayTeamID == _vm.GameDetails.Game.HomeTeamID)
                {
                    MessageBox.Show(AppResources.HomeAndAwayCanNotBeSameTeam, _messageBoxTitle, MessageBoxButton.OK);
                    rtnValue = false;
                }
                else if (_vm.GameDetails.Game.PlayersPerTeam == 0)
                {
                    MessageBox.Show(AppResources.PlayersPerTeamNotSet, _messageBoxTitle, MessageBoxButton.OK);
                    rtnValue = false;
                }
                else if (_vm.GameDetails.Game.Periods == 0)
                {
                    MessageBox.Show(AppResources.PeriodNotSet, _messageBoxTitle, MessageBoxButton.OK);
                    rtnValue = false;
                }
                else if (_vm.GameDetails.Game.PeriodLength == 0)
                {
                    MessageBox.Show(AppResources.PeriodLengthNotSet, _messageBoxTitle, MessageBoxButton.OK);
                    rtnValue = false;
                }

                else if (_vm.GameDetails.Game.HasOverTime == true)
                {
                    if (_vm.GameDetails.Game.OverTimeLength <= 0)
                    {
                        MessageBox.Show(AppResources.InvalidOTLength, _messageBoxTitle, MessageBoxButton.OK);
                        rtnValue = false;
                    }
                }

                else if (_vm.GameDetails.Game.ClockUpOrDown == string.Empty)
                {
                    MessageBox.Show(AppResources.ClockUpDownNotSet, _messageBoxTitle, MessageBoxButton.OK);
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