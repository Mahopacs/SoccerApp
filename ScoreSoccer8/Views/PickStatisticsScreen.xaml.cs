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
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace ScoreSoccer8.Views
{
    public partial class PickStatisticsScreen : PhoneApplicationPage
    {
        private PickStatisticsViewModel _vm;
        int _gameId = 0;

        public PickStatisticsScreen()
        {
            InitializeComponent();

            LoadingPopup ovr = new LoadingPopup();
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
            loadingGrid.Children.Add(ovr);

            _vm = new PickStatisticsViewModel();
            this.DataContext = _vm;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
            PhoneApplicationService.Current.State["LastPage"] = "PickStatisticsScreen";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.Count > 0)
            {
                _gameId = Convert.ToInt32(NavigationContext.QueryString["gameid"]);
                _vm.Initialize(_gameId);
            }

        }

        private void TimeLine_Click(object sender, RoutedEventArgs e)
        {
            loadingGrid.Visibility = System.Windows.Visibility.Visible;
            _vm.GoToTimelineCommand.Execute(e);
        }

        private void GameStats_Click(object sender, RoutedEventArgs e)
        {
            loadingGrid.Visibility = System.Windows.Visibility.Visible;
            _vm.GoToGameStatsCommand.Execute(e);
        }

        private void HomeBoxscore_Click(object sender, RoutedEventArgs e)
        {
            loadingGrid.Visibility = System.Windows.Visibility.Visible;
            _vm.GoToHomeTeamBoxscoreCommand.Execute(e);
        }

        private void AwayBoxscore_Click(object sender, RoutedEventArgs e)
        {
            loadingGrid.Visibility = System.Windows.Visibility.Visible;
            _vm.GoToAwayTeamBoxscoreCommand.Execute(e);
        }

        private void PlayByPlay_Click(object sender, RoutedEventArgs e)
        {
            loadingGrid.Visibility = System.Windows.Visibility.Visible;
            _vm.GoToPlayByPlayCommand.Execute(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ToSaveAwayPlayerListBox.Visibility = Visibility.Visible;

                var bmp = new WriteableBitmap(Convert.ToInt32(ToSaveAwayPlayerListBox.ActualWidth), Convert.ToInt32(ToSaveAwayPlayerListBox.ActualHeight));

                bmp.Render(ToSaveAwayPlayerListBox, null);
                bmp.Invalidate();

                var width = (int)ToSaveAwayPlayerListBox.ActualWidth;
                var height = (int)ToSaveAwayPlayerListBox.ActualHeight;

                using (var ms = new MemoryStream(width * height * 4))
                {
                    bmp.SaveJpeg(ms, width, height, 0, 100);
                    ms.Seek(0, SeekOrigin.Begin);
                    var lib = new MediaLibrary();
                    var picture = lib.SavePicture(string.Format("uTrackSoccer_GameStats"), ms);

                    var task = new ShareMediaTask();
                    task.FilePath = picture.GetPath();
                    task.Show();
                }

                ToSaveAwayPlayerListBox.Visibility = Visibility.Collapsed;

            }
            catch (Exception ex)
            {
                // Debug.WriteLine(ex.ToString());
            }

        }

    }
}