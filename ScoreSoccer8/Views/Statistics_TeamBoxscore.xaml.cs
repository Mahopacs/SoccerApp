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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;
using System.IO;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;

namespace ScoreSoccer8.Views
{
    public partial class Statistics_TeamBoxscore : PhoneApplicationPage
    {
        private TeamBoxscoreViewModel _vm;
        int _gameId = 0;
        int _teamId = 0;

        public Statistics_TeamBoxscore()
        {
            InitializeComponent();

            _vm = new TeamBoxscoreViewModel();
            this.DataContext = _vm;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.Count > 0)
            {
                _gameId = Convert.ToInt32(NavigationContext.QueryString["gameid"]);
                _teamId = Convert.ToInt32(NavigationContext.QueryString["teamid"]);

                _vm.Initialize(_gameId, _teamId);
            }

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                // var bmp = new WriteableBitmap(AwayPlayerGrid, null);
                // var width = (int)bmp.PixelWidth;
                // var height = (int)bmp.PixelHeight;

                ToSaveAwayPlayerListBox.Visibility = Visibility.Visible;

                // System.Threading.Thread.Sleep(1000);

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
                    var picture = lib.SavePicture(string.Format("TimeLine.jpg"), ms);

                    //ShareLinkTask a = new ShareLinkTask();
                    //string test = @"<a href=""http://www.utracksports.com"" target=""_blank"" <img src=""" + picture.GetPath().ToString() + @""" />";
                    //a.LinkUri = new Uri(test, UriKind.Absolute);
                    //a.Show();

                    var task = new ShareMediaTask();
                    task.FilePath = picture.GetPath();
                    task.Show();
                }

                ToSaveAwayPlayerListBox.Visibility = Visibility.Collapsed;

            }
            catch (Exception ex)
            {
                //Debug.WriteLine(ex.ToString());
            }

        }





    }
}

