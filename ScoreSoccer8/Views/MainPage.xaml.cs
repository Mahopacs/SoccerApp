using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ScoreSoccer8.Resources;
using ScoreSoccer8.ViewModels;
using ScoreSoccer8.Classes;
using ScoreSoccer8.DataAccess;
using System.Windows.Input;
using System.Diagnostics;
using ScoreSoccer8.Utilities;
using ScoreSoccer8.Cloud;
using Microsoft.Phone.Tasks;

namespace ScoreSoccer8.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        MainPageViewModel _vm = new MainPageViewModel();
        private bool demoPrompt = false;
        private bool ratePrompt = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.DataContext = _vm;

            LoadingPopup ovr = new LoadingPopup();
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
            loadingGrid.Children.Add(ovr);

            App.IsAppEnabled();
 
        }

        #region "Events"

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (e.Orientation == PageOrientation.PortraitDown || e.Orientation == PageOrientation.PortraitUp)
            {

                Grid.SetRow(images, 0);
                Grid.SetColumn(images, 0);
                Grid.SetColumnSpan(images, 2);
                Grid.SetRowSpan(images, 1);
                

                Grid.SetRow(buttons, 1);
                Grid.SetColumn(buttons, 0);
                Grid.SetColumnSpan(buttons, 2);
                

                Grid.SetRow(teamsButton, 0);
                Grid.SetColumn(teamsButton, 0);
                Grid.SetColumnSpan(teamsButton, 2);

                Grid.SetRow(playersButton, 0);
                Grid.SetColumn(playersButton, 2);
                Grid.SetColumnSpan(playersButton, 2);

                Grid.SetRow(leaguesButton, 1);
                Grid.SetColumn(leaguesButton, 0);
                Grid.SetColumnSpan(leaguesButton, 2);

                Grid.SetRow(gamesButton, 1);
                Grid.SetColumn(gamesButton, 2);
                Grid.SetColumnSpan(gamesButton, 2);

                _vm.BuyMargin = new Thickness(0,0,0,75);

            }
            else
            {

                Grid.SetRow(images, 0);
                Grid.SetColumn(images, 0);
                Grid.SetColumnSpan(images, 2);
                Grid.SetRowSpan(images, 1);


                Grid.SetRow(buttons, 1);
                Grid.SetColumn(buttons, 0);
                Grid.SetColumnSpan(buttons, 2);
                

                Grid.SetRow(teamsButton, 0);
                Grid.SetColumn(teamsButton, 0);
                Grid.SetColumnSpan(teamsButton, 1);

                Grid.SetRow(playersButton, 0);
                Grid.SetColumn(playersButton, 1);
                Grid.SetColumnSpan(playersButton, 1);

                Grid.SetRow(leaguesButton, 0);
                Grid.SetColumn(leaguesButton, 2);
                Grid.SetColumnSpan(leaguesButton, 1);

                Grid.SetRow(gamesButton, 0);
                Grid.SetColumn(gamesButton, 3);
                Grid.SetColumnSpan(gamesButton, 1);

                _vm.BuyMargin = new Thickness(0, 0, 0, 5);

            }
        }
        
        private void Teams_Clicked(object sender, RoutedEventArgs e)
        {
            if (App.IsAppEnabled() == false)
            {
                PromptToBuy();
            }
            else
            {
                loadingGrid.Visibility = System.Windows.Visibility.Visible;
                _vm.TeamsClickCommand.Execute(e);
            }
        }

        private void Players_Clicked(object sender, RoutedEventArgs e)
        {
            if (App.IsAppEnabled() == false)
            {
                PromptToBuy();
            }
            else
            {
                loadingGrid.Visibility = System.Windows.Visibility.Visible;
                _vm.PlayersClickCommand.Execute(e);
            }
        }

        private void Games_Clicked(object sender, RoutedEventArgs e)
        {
            if (App.IsAppEnabled() == false)
            {
                PromptToBuy();
            }
            else
            {
                loadingGrid.Visibility = System.Windows.Visibility.Visible;
                _vm.GamesClickCommand.Execute(e);
            }
        }

        private void Leagues_Clicked(object sender, RoutedEventArgs e)
        {
            if (App.IsAppEnabled() == false) 
            {
                PromptToBuy();
            }
            else
            {
                loadingGrid.Visibility = System.Windows.Visibility.Visible;
                _vm.LeaguesClickCommand.Execute(e);
            }
        }
        
        private void PromptToBuy()
        {
            MarketplaceDetailTask _marketPlaceDetailTask = new MarketplaceDetailTask();
            _marketPlaceDetailTask.Show();
        }
        
        private void About_Clicked(object sender, RoutedEventArgs e)
        {
            loadingGrid.Visibility = System.Windows.Visibility.Visible;
            _vm.AboutClickCommand.Execute(e);
        }

        private void Demo_Clicked(object sender, RoutedEventArgs e)
        {
            loadingGrid.Visibility = System.Windows.Visibility.Visible;
            _vm.GoToDemoClickCommand.Execute(e);
        }

        private void Purchuse_Clicked(object sender, RoutedEventArgs e)
        {
            loadingGrid.Visibility = System.Windows.Visibility.Visible;
            _vm.GoBuyAppClickCommand.Execute(e);
        }




        private void Test_Clicked(object sender, RoutedEventArgs e)
        {

            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/StatPickerHelp.xaml", UriKind.Relative));

        }



        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            DetermineWhetherToPromptUserToGoToDemoGame();
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (ratePrompt == false)
            {
                Rate.RateTheApp();
                ratePrompt = true;
            }

            DetermineWhetherToPromptUserToGoToDemoGame();
            _vm.CheckForFreeMode();
        }

        #endregion "Events"

        #region "Methods"

        //This is the code to determine whether to prompt user to go to DEMO game
        //We only prompt user to go to demo game if first time opening app AND Free Version
        private void DetermineWhetherToPromptUserToGoToDemoGame()
        {
            try
            {
                if ((demoPrompt == false) && (App.gAppOpenedCount == 1))
                {
                    demoPrompt = true;

                    CustomMessageBox messageBox = new CustomMessageBox()
                    {
                        Caption = AppResources.DemoGame,
                        Message = AppResources.GoToDemoGamePrompt,
                        LeftButtonContent = AppResources.DemoLeftButton,
                        RightButtonContent = AppResources.DemoRightButton
                    };

                    messageBox.Show();

                    messageBox.Dismissed += (s1, e1) =>
                    {
                        switch (e1.Result)
                        {
                            case CustomMessageBoxResult.LeftButton:
                                Demo_Clicked(this, null);
                                break;
                            case CustomMessageBoxResult.RightButton:
                                break;
                            case CustomMessageBoxResult.None:
                                break;
                            default:
                                break;
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                ErrorLogConnection cloud = new ErrorLogConnection();
                cloud.UpdateErrorLog("MainPage.xaml.cs.DetermineWhetherToPromptUserToGoToDemoGame", ex.Message.ToString());
            }
        }

        #endregion "Methods"
    }
}