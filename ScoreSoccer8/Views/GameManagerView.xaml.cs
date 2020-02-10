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
using Coding4Fun.Toolkit.Controls;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ScoreSoccer8.Views
{
    public partial class GameManagerView : PhoneApplicationPage
    {
        GameManagerViewModel _vm;
        public GameManagerView()
        {
            try
            {
                InitializeComponent();

                LoadingPopup ovr = new LoadingPopup();
                loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
                loadingGrid.Children.Add(ovr);

                PhoneApplicationService.Current.Deactivated += Current_Deactivated;
            }
            catch (Exception ex)
            {

            }
        }

        private void Current_Deactivated(object sender, DeactivatedEventArgs e)
        {
            PhoneApplicationService.Current.State["LastPage"] = "HomeScreen";
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
            MenuExpender.IsExpanded = false;
            PhoneApplicationService.Current.State["LastPage"] = "GameManager";
        }

        private void Stats_Clicked(object sender, RoutedEventArgs e)
        {
            loadingGrid.Visibility = System.Windows.Visibility.Visible;
            _vm.GameStatsCommand.Execute(e);
        }

        private void Edit_Clicked(object sender, RoutedEventArgs e)
        {
            loadingGrid.Visibility = System.Windows.Visibility.Visible;
            _vm.EditPlayCommand.Execute(e);
        }

        private void Help_Clicked(object sender, RoutedEventArgs e)
        {
            loadingGrid.Visibility = System.Windows.Visibility.Visible;
            _vm.HelpClickCommand.Execute(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (_vm == null)
            {
                InitFromScratch();
            }
            else if (_adjustingTime)
            {
                TimeSpan ts = picker.Value ?? TimeSpan.Zero;
                _vm.ClockIsDoneBeingAdjusted(ts);
                _adjustingTime = false;
            }
            else if (PhoneApplicationService.Current.State.ContainsKey("LastPage"))
            {
                if (PhoneApplicationService.Current.State["LastPage"].ToString().ToUpper().Equals("PLAYERLIST"))
                {
                    //Only Need to reload players here, everything else should be working
                    _vm.ReloadPlayers();
                }
                else if (PhoneApplicationService.Current.State["LastPage"].ToString().ToUpper().Equals("STATSPICKER"))
                {
                    //Just reload the score
                    _vm.ReloadOnlyScore();
                }
                else if (PhoneApplicationService.Current.State["LastPage"].ToString().ToUpper().Equals("PLAYLIST"))
                {
                    //Just reload the score
                    _vm.ReloadOnlyScore();
                }
                else if (PhoneApplicationService.Current.State["LastPage"].ToString().ToUpper().Equals("HOMESCREEN"))
                {
                    //Not reloading anything for now... not sure
                }
                else if (PhoneApplicationService.Current.State["LastPage"].ToString().ToUpper().Equals("PICKSTATISTICSSCREEN"))
                {
                    //Nothing here, as this means the user went to the stats screen in GM
                }
                else if (PhoneApplicationService.Current.State["LastPage"].ToString().ToUpper().Equals("GAMEMANAGER"))
                {
                    //Nothing here, as this means the user went to the adjust the clock
                }
                else if (PhoneApplicationService.Current.State["LastPage"].ToString().ToUpper().Equals("GAMES"))
                {
                    InitFromScratch();
                }
                else
                {
                    InitFromScratch();
                }
            }
            else
            {
                InitFromScratch();
            }
        }

        private void InitFromScratch()
        {
            if (_vm != null)
            {
                _vm.Dispose();
            }

            _vm = new GameManagerViewModel();
            this.DataContext = _vm;

            if (NavigationContext.QueryString.Count > 0)
            {
                int paramValue = Convert.ToInt32(NavigationContext.QueryString["gameid"]);

                try
                {
                    LoadVm(paramValue);
                }
                catch (Exception ex)
                {

                }
            }
        }

        private async void LoadVm(int gameID)
        {
            _vm.Initialize(gameID);
        }

        private void TimePickerTapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _vm.ClockIsBeingAdjusted();
        }

        private bool _adjustingTime = false;
        private void TSClick(object sender, RoutedEventArgs e)
        {
            picker.ValueChanged -= picker_ValueChanged;

            _adjustingTime = true;
            _vm.ClockIsBeingAdjusted();
            //tsPicker.Unloaded += tsPickerUnloaded;
            //tsPicker.Value = ClockTime;
            picker.OpenPicker();
            picker.Value = _vm.ClockTime;

            picker.ValueChanged += picker_ValueChanged;
        }

        void picker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<TimeSpan> e)
        {
            if (e.OldValue != e.NewValue)
            {
                _vm.ClockIsDoneBeingAdjusted(e.NewValue);
                _adjustingTime = false;
            }
        }

        protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
        {
            Clock.StopClock();

            if (_vm != null)
            {
                _vm.Dispose();
            }

            if (picker != null)
            {
                picker.ValueChanged -= picker_ValueChanged;
            }

            base.OnRemovedFromJournal(e);
        }

        private void ExpanderView_Expanded(object sender, RoutedEventArgs e)
        {
            _vm.SubMode = true;
            _vm.StatsMode = false;
            _vm.ModeCommandClicked();
            //subWatermark.Visibility = System.Windows.Visibility.Visible;
        }

        private void ExpanderView_Collapsed(object sender, RoutedEventArgs e)
        {
            _vm.SubMode = false;
            _vm.StatsMode = true;
            _vm.ModeCommandClicked();
            //subWatermark.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}