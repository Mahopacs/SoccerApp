using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using System.Diagnostics;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ScoreSoccer8.ViewModels;
using ScoreSoccer8.Utilities;
using ScoreSoccer8.Resources;
using ScoreSoccer8.Cloud;


namespace ScoreSoccer8.Views
{
    public partial class Games : PhoneApplicationPage
    {
        GamesViewModel _vm = new GamesViewModel();
       
        int gameId = 0;

        public Games()
        {
            InitializeComponent();

            LoadingPopup ovr = new LoadingPopup();
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
            loadingGrid.Children.Add(ovr);
            
            this.DataContext = _vm;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
            PhoneApplicationService.Current.State["LastPage"] = "Games";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DetermineWhetherToPromptUserToPurchaseApp();         
        }
        
        private void GM_Click(object sender, RoutedEventArgs e)
        {

            gameId = (int)((Button)sender).Tag;
            loadingGrid.Visibility = System.Windows.Visibility.Visible;

            PopUPGoCommand.Execute(true);

        }

        private void Stats_Click(object sender, RoutedEventArgs e)
        {

            gameId = (int)((Button)sender).Tag;
            loadingGrid.Visibility = System.Windows.Visibility.Visible;

            GoToStatsCommand.Execute(true);

        }


        private ICommand _popUPGoCommand;
        public ICommand PopUPGoCommand
        {
            get
            {
                if (_popUPGoCommand == null)
                {
                    _popUPGoCommand = new DelegateCommand(param => this.GoToGameManager(), param => true);
                }

                return _popUPGoCommand;
            }
        }
        
        public void GoToGameManager()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/GameManagerView.xaml?gameid=" + gameId, UriKind.Relative));
        }



        private ICommand _goToStatsCommand;
        public ICommand GoToStatsCommand
        {
            get
            {
                if (_goToStatsCommand == null)
                {
                    _goToStatsCommand = new DelegateCommand(param => this.GoToStats(), param => true);
                }

                return _goToStatsCommand;
            }
        }

        public void GoToStats()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/PickStatisticsScreen.xaml?gameid=" + gameId, UriKind.Relative));
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            //this.popup.IsOpen = false;
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
        }


        #region "Methods"
        //If this is the free version (i.e. only shot available), prompt the user if they would like to purchase the app 
        //(this is only done when the 4th play is being entered)
        private void DetermineWhetherToPromptUserToPurchaseApp()
        {
            try
            {
                if (App.DoesUserHaveAbilityToTrackAllStats() == false)
                {
                    if ((App.gAppOpenedCount % 5 == 0) && (App.gHaveWePromptedToPurchase == false))
                    {
                        App.gHaveWePromptedToPurchase = true;
                        CustomMessageBox messageBox = new CustomMessageBox()
                        {
                            Caption = AppResources.PurchaseApp,
                            Message = AppResources.PurchaseAppPrompt,
                            LeftButtonContent = AppResources.PurchaseAppLeftButton,
                            RightButtonContent = AppResources.PurchaseAppRightButton
                        };
                        messageBox.Show();

                        messageBox.Dismissed += (s1, e1) =>
                        {
                            switch (e1.Result)
                            {
                                case CustomMessageBoxResult.LeftButton:
                                    (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/PurchaseApp.xaml", UriKind.Relative));
                                    return;
                                case CustomMessageBoxResult.RightButton:
                                    _vm.Initialize();
                                    break;
                                case CustomMessageBoxResult.None:
                                    _vm.Initialize();
                                    break;
                                default:
                                    break;
                            }
                        };
                    }
                    else
                    {
                        _vm.Initialize();
                    }
                }
                else
                {
                    _vm.Initialize();
                }
            }
            catch (Exception ex)
            {
                ErrorLogConnection cloud = new ErrorLogConnection();
                cloud.UpdateErrorLog("Games.xaml.cs.DetermineWhetherToPromptUserToPurchaseApp", ex.Message.ToString());           
                _vm.Initialize();
            }
        }

        #endregion "Methods"
    }
}