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
using System.ComponentModel;
using ScoreSoccer8.Utilities;
using System.Windows.Input;

namespace ScoreSoccer8.Views
{
    public partial class ShotDetails : PhoneApplicationPage
    {
        private ShotDetailsViewModel _vm;
        private bool listPickerMode = false; //needed because listpicker open and closing fires on navigated to and from, so need to know if you 
        //are coming from listpicker or going to listpicker
        private string _messageBoxTitle = AppResources.ShotDetails;

        public ShotDetails()
        {
            InitializeComponent();
            _vm = new ShotDetailsViewModel();
            this.DataContext = _vm;

            LoadingPopup ovr = new LoadingPopup();
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
            loadingGrid.Children.Add(ovr);
        }

        #region "Events"

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
                        int playerID = Convert.ToInt32(NavigationContext.QueryString["playerID"]);
                        int teamID = Convert.ToInt32(NavigationContext.QueryString["teamID"]);
                        int period = Convert.ToInt32(NavigationContext.QueryString["period"]);
                        string gameTime = NavigationContext.QueryString["gameTime"];
                        string GMPlayer1PositionID = NavigationContext.QueryString["GMPlayer1PositionID"];
                        int otherTeamGoalieID = Convert.ToInt32(NavigationContext.QueryString["otherTeamGoalieID"]);
                        int statCategoryID = Convert.ToInt32(NavigationContext.QueryString["statcategoryID"]);
                        int statDescriptionID = Convert.ToInt32(NavigationContext.QueryString["statdescriptionID"]);

                        _vm.Initialize(gameID, playerID, teamID, period, gameTime, GMPlayer1PositionID, otherTeamGoalieID, statCategoryID, statDescriptionID);
                    }
                }
            }
            else
            {
                listPickerMode = true;
            }
        }

        private void BackButtonClicked(object sender, CancelEventArgs e)
        {
            loadingGrid.Visibility = System.Windows.Visibility.Visible;
        }
        
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if ((e.Uri.OriginalString == "/Microsoft.Phone.Controls.Toolkit;component/ListPicker/ListPickerPage.xaml") ||
                (e.Uri.OriginalString == "/Views/Help_Buttons.xaml?screenId=ShotDetails"))
            {
                listPickerMode = true;
            }
            else
            {
                //This is needed because ShotDetails would simply go back to main stat picker, we want it to go back from where it was
                //called from (i.e. Game Manager), so need to really go back 1 more...
                if ((_vm.CancelButtonClicked == false) && (_vm.OkButtonClicked == false))
                {
                    _vm.SaveToDatabase();
                }

                //NavigationService.RemoveBackEntry();
                NavigationService.GoBack();
            }

            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
        }

        #endregion "Events"

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            loadingGrid.Visibility = System.Windows.Visibility.Visible;
            _vm.OkClickCommand.Execute(e);
        }
    }
}