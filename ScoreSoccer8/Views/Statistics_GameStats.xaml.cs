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
    public partial class Statistics_GameStats : PhoneApplicationPage
    {

        private GameStatsViewModel _vm;

        public Statistics_GameStats()
        {
            InitializeComponent();

            _vm = new GameStatsViewModel();
            this.DataContext = _vm;
        }

        #region Events
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.Count > 0)
            {
                int gameId = Convert.ToInt32(NavigationContext.QueryString["gameid"]);
                _vm.Initialize(gameId);
            }
        }
        #endregion Events

        #region Share Image

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
                    var picture = lib.SavePicture(string.Format("uTrackSoccer_GameStats.jpg"), ms);

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

        #endregion

    }
}