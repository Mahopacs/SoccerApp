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
using ScoreSoccer8.Classes;
using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.Resources;

namespace ScoreSoccer8.Views
{
    public partial class StatsPicker : PhoneApplicationPage
    {
        private StatsPickerViewModel _vm;

        public StatsPicker()
        {
            InitializeComponent();
            _vm = new StatsPickerViewModel();
            this.DataContext = _vm;

            LoadingPopup ovr = new LoadingPopup();
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
            loadingGrid.Children.Add(ovr);

        }

        private void Translations_Clicked(object sender, RoutedEventArgs e)
        {

            loadingGrid.Visibility = System.Windows.Visibility.Visible;
            _vm.GoToTranslationsCommand.Execute(e);

        }





        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _vm.Dispose();
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
            PhoneApplicationService.Current.State["LastPage"] = "StatsPicker";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Play play =  new Play();
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.Count > 0)
            {
                int gameID = Convert.ToInt32(NavigationContext.QueryString["gameID"]);
                int teamID = Convert.ToInt32(NavigationContext.QueryString["teamID"]);
                int playerID = Convert.ToInt32(NavigationContext.QueryString["playerID"]);
                int period = Convert.ToInt32(NavigationContext.QueryString["period"]);
                string gameTime = NavigationContext.QueryString["gameTime"];
                string playerPosition = NavigationContext.QueryString["playerPosition"];
                int otherTeamGoalieID = Convert.ToInt32(NavigationContext.QueryString["otherTeamGoalieID"]);

                play.GameID = gameID;
                play.GameTime = gameTime;
                play.Period = period;
                play.Player1ID = playerID;
                play.TeamID = teamID;
                play.OtherTeamGoalieID = otherTeamGoalieID;

                _vm.Initialize(play);                 
            }
        }    
    }
}