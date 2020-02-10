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
    public partial class BoxScore : PhoneApplicationPage
    {
        private BoxScoreViewModel _vm;
        private bool comingFromSendScreen = false;


        public BoxScore()
        {
            InitializeComponent();
            _vm = new BoxScoreViewModel();
            this.DataContext = _vm;
        }

        #region "Events"
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (comingFromSendScreen == false)
            {
                if (NavigationContext.QueryString.Count > 0)
                {
                    int gameId = Convert.ToInt32(NavigationContext.QueryString["gameid"]);
                    _vm.Initialize(gameId);
                }
            }
        }

        #endregion


        private ListBox LoadBoxscore()
        {
            //<ListBox 
            //x:Name="AwayPlayerGrid"
            //Grid.Row="1" 
            //Grid.Column="1"
            //ItemsSource="{Binding BoxscoreAwayItems}"
            //ScrollViewer.VerticalScrollBarVisibility="Disabled" 
            //ScrollViewer.HorizontalScrollBarVisibility="Disabled"
            //>

            ListBox boxscoreListbox = new ListBox();
            boxscoreListbox.ItemsSource = _vm.BoxscoreAwayItems;




            return boxscoreListbox;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                // var bmp = new WriteableBitmap(AwayPlayerGrid, null);
                // var width = (int)bmp.PixelWidth;
                // var height = (int)bmp.PixelHeight;

                comingFromSendScreen = true;

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

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            MediaLibrary _mediaLibrary = new MediaLibrary();

            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {

                    //TextBlock newTextBox = new TextBlock();

                    //newTextBox.FontSize = 30;
                    //newTextBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
                    //newTextBox.Text = "ScoreSoccer8";
                    //newTextBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    //newTextBox.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    //newTextBox.FontWeight = FontWeights.Bold;
                    //newTextBox.Opacity = 0.5;
                    //newTextBox.Margin = new Thickness(10);
                    //AwayPlayerGrid.Children.Add(newTextBox);
                    Grid AwayPlayerGrid = new Grid();

                    var bitmap = new WriteableBitmap(Convert.ToInt32(2000), Convert.ToInt32(400)); // w / h

                    var transform = new TransformGroup();

                    var st = new ScaleTransform()
                    {
                        ScaleX = 0.5,
                        ScaleY = 0.5,
                        CenterX = (AwayPlayerGrid.ActualWidth / 2.0),
                        CenterY = (AwayPlayerGrid.ActualHeight / 2.0)
                    };
                    transform.Children.Add(st);

                    bitmap.Render(AwayPlayerGrid, null);

                    bitmap.Invalidate();

                    String tempJPEG = "logo.jpg";

                    IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(tempJPEG);

                    // Encode WriteableBitmap object to a JPEG stream.
                    Extensions.SaveJpeg(bitmap, fileStream, bitmap.PixelWidth, bitmap.PixelHeight, 0, 100);

                    fileStream.Close();
                    fileStream.Dispose();

                    IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile(tempJPEG, FileMode.Open, FileAccess.Read);

                    Picture picture = _mediaLibrary.SavePictureToCameraRoll("somefile.jpg", stream);


                    ShareMediaTask task = new ShareMediaTask();
                    task.FilePath = picture.GetPath();
                    task.Show();

                    //BorderToSave.Children.Remove(newTextBox);

                    stream.Close();
                    stream.Dispose();

                }
                catch (Exception ex)
                {
                    //Debug.WriteLine(ex.ToString());
                }

            }
        }




    }
}



