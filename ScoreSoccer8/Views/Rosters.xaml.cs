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
using Coding4Fun.Toolkit.Controls;
using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.DataAccess;
using System.Collections.ObjectModel;
using ScoreSoccer8.Resources;
using System.Windows.Input;

namespace ScoreSoccer8.Views
{
    public partial class Rosters : PhoneApplicationPage
    {
        private RostersViewModel _vm;
        private string _messageBoxTitle = AppResources.Rosters;

        public Rosters()
        {
            InitializeComponent();
            _vm = new RostersViewModel();
            this.DataContext = _vm;
        }

        #region "Properties"

        private int _gameID;
        public int GameID
        {
            get { return _gameID; }
            set { _gameID = value; }
        }

        #endregion "Properties"

        #region "Methods"

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            PhoneApplicationService.Current.State["LastPage"] = "Rosters";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.Count > 0)
            {
                int teamID = Convert.ToInt32(NavigationContext.QueryString["teamID"]);
     
                if (teamID != 0)
                {
                    _vm.Initialize(teamID);
                }
            }

            //Since we are using GoBack I did not have parameters to use so I needed to set some global variables to know 
            //if user got here because they clicked back button or added a player
            //If added a player want to prompty for uniform number, if hit back button they did not add a player so no
            //need to prompt. (if using Navigate instead of GoBack I would have had parameters, but Navigate was acting very
            //weird and was contantly going back and forth between player list and team roster screen.

            if (App.gPromptForJersey == true)
            {
                InputPrompt input = new InputPrompt();
                InputScope scope = new InputScope();
                InputScopeName name = new InputScopeName();

                name.NameValue = InputScopeNameValue.Number;
                scope.Names.Add(name);

                input.Completed += input_Completed;
                input.Title = AppResources.JerseyNumber;
                input.InputScope = scope;           

                input.Message = AppResources.EnterPlayersJerseyNumber;
                input.Show();
            }
            App.gPromptForJersey = false;
        }

        void input_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            int number;
            bool parsed = int.TryParse(e.Result, out number);

            if (e.Result == null)
            {
                MessageBox.Show(AppResources.JerseyMustBeANumber, _messageBoxTitle, MessageBoxButton.OK);
            }
            else if (e.Result.Length > 3)
            {
                MessageBox.Show(AppResources.JerseyMustBeLessThan3Characters, _messageBoxTitle, MessageBoxButton.OK);
            }
            else if (parsed == false)
            {
                MessageBox.Show(AppResources.JerseyMustBeANumber, _messageBoxTitle, MessageBoxButton.OK);
            }
            else
            {
                TeamRoster teamRoster = new TeamRoster();

                teamRoster.TeamID = App.gPromptForJerseyTeamID;
                teamRoster.PlayerID = App.gPromptForJerseyPlayerID;
                teamRoster.UniformNumber = e.Result.ToString();
                teamRoster.Active = "Y";
                BaseTableDataAccess.Instance().UpsertTeamRoster(teamRoster);

                //Now need to reload team roster so added player with his jersey is displayed.
                _vm.Initialize(teamRoster.TeamID);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void AddPlayerToEventRoster()
        {
            EventRoster eventRosterEntry = new EventRoster();

            try
            {
                eventRosterEntry.GameID = GameID;
                eventRosterEntry.TeamID = App.gPromptForJerseyTeamID;
                eventRosterEntry.PlayerID = App.gPromptForJerseyPlayerID;
                eventRosterEntry.Starter = "N";
                eventRosterEntry.IsPlayerOnField = "N";
                eventRosterEntry.GMPlayerPositionID = "";

                BaseTableDataAccess.Instance().InsertEventRoster(eventRosterEntry);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion "Methods"
    }
}