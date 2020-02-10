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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Diagnostics;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using ScoreSoccer8.Utilities;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;

namespace ScoreSoccer8.Views
{
    public partial class Leagues : PhoneApplicationPage
    {
        LeaguesViewModel _vm;
        int _leagueId = 0;

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {

        }

        public Leagues()
        {
            InitializeComponent();
            LoadingPopup ovr = new LoadingPopup();
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
            loadingGrid.Children.Add(ovr);


            _vm = new LeaguesViewModel();
            this.DataContext = _vm;
        }

        private void AddTeam_Clicked(object sender, RoutedEventArgs e)
        {

            loadingGrid.Visibility = System.Windows.Visibility.Visible;
            _vm.AddLeagueClickCommand.Execute(e);

        }

        private void GoToLeague_Clicked(object sender, RoutedEventArgs e)
        {
            _leagueId = (int)((Button)sender).Tag;
            loadingGrid.Visibility = System.Windows.Visibility.Visible;
            GoToLeaguesDetailsCommand.Execute(e);

        }


        private ICommand _goToLeaguesDetailsCommand;
        public ICommand GoToLeaguesDetailsCommand
        {
            get
            {
                if (_goToLeaguesDetailsCommand == null)
                {
                    _goToLeaguesDetailsCommand = new DelegateCommand(param => this.GoToLeagueDetailsScreen(), param => true);
                }

                return _goToLeaguesDetailsCommand;
            }
        }

        public void GoToLeagueDetailsScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/LeagueDetails.xaml?parameter=" + _leagueId, UriKind.Relative));
        }












        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _vm.Initialize();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
        }


        private void ListBox_Loaded(object sender, RoutedEventArgs e)
        {
            ListBox listbox = sender as ListBox;
            int index = leaguesListbox.Items.IndexOf(leaguesListbox.DataContext);
            if (index % 2 == 0)
            {

                // = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Orange);
            }
            else
            {
                listbox.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
            }
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {


            //var bmp = new WriteableBitmap(BorderToSave, null);
            //var width = (int)bmp.PixelWidth;
            //var height = (int)bmp.PixelHeight;
            //using (var ms = new MemoryStream(width * height * 4))
            //{
            //    bmp.SaveJpeg(ms, width, height, 0, 100);
            //    ms.Seek(0, SeekOrigin.Begin);
            //    var lib = new MediaLibrary();
            //    var picture = lib.SavePicture(string.Format("test.jpg"), ms);

            //    var task = new ShareMediaTask();

            //    task.FilePath = picture.GetPath();

            //    task.Show();
            //}
        }
        
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            MediaLibrary _mediaLibrary = new MediaLibrary();

            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {

                    TextBlock newTextBox = new TextBlock();

                    newTextBox.FontSize = 30;
                    newTextBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
                    newTextBox.Text = "ScoreSoccer8";
                    newTextBox.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    newTextBox.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    newTextBox.FontWeight = FontWeights.Bold;
                    newTextBox.Opacity = 0.5;
                    newTextBox.Margin = new Thickness(10);
                    BorderToSave.Children.Add(newTextBox);
                    
                    var bitmap = new WriteableBitmap(Convert.ToInt32(BorderToSave.ActualWidth), Convert.ToInt32(BorderToSave.ActualHeight));
                    bitmap.Render(BorderToSave, null);                   
                    bitmap.Invalidate();

                    String tempJPEG = "logo.jpg";

                    IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(tempJPEG);

                    // Encode WriteableBitmap object to a JPEG stream.
                    Extensions.SaveJpeg(bitmap, fileStream, bitmap.PixelWidth, bitmap.PixelHeight, 0, 100);

                    fileStream.Close();
                    fileStream.Dispose();

                    IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile(tempJPEG, FileMode.Open, FileAccess.Read);
                    
                    //BitmapImage b = new BitmapImage();
                    //b.SetSource(stream);

                    //EmailComposeTask emailComposeTask = new EmailComposeTask();

                    //emailComposeTask.Subject = "Stats";
                    //emailComposeTask.Body = "Stats Of some game" + b.UriSource;
                    //emailComposeTask.To = "skulczyk@gmail.com";
                                        
                    //emailComposeTask.Show();

                    _mediaLibrary.SavePictureToCameraRoll("somefile.jpg", stream);

                    //ShareMediaTask shareMediaTask = new ShareMediaTask();
                    //shareMediaTask.FilePath = "somefile.jpg";
                    //shareMediaTask.Show();

                    ShareStatusTask shareStatusTask = new ShareStatusTask();
                    shareStatusTask.Status = "I'm developing a Windows Phone application!";
                    shareStatusTask.Show();

                    BorderToSave.Children.Remove(newTextBox);

                    stream.Close();
                    stream.Dispose();

                }
                catch (Exception ex)
                {
                   // Debug.WriteLine(ex.ToString());
                }

            }
        } 


    }
}