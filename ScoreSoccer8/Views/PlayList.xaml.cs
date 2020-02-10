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
using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.Resources;
using ScoreSoccer8.DataAccess;
using System.Windows.Input;
using ScoreSoccer8.Utilities;
using System.Diagnostics;
using System.ComponentModel;

namespace ScoreSoccer8.Views
{
    public partial class PlayList : PhoneApplicationPage
    {
        private PlayListViewModel _vm;

        public PlayList()
        {
            InitializeComponent();

            LoadingPopup ovr = new LoadingPopup();
            loadingGrid.Children.Add(ovr);

            _vm = new PlayListViewModel();
            this.DataContext = _vm;

        }



        #region "Events"



        private void Add_Click(object sender, RoutedEventArgs e)
        {
            loadingGrid.Visibility = System.Windows.Visibility.Visible;
            _vm.AddPlayClickCommand.Execute(true);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            loadingGrid.Visibility = System.Windows.Visibility.Collapsed;
            PhoneApplicationService.Current.State["LastPage"] = "PlayList";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.Count > 0)
            {
                int gameID = Convert.ToInt32(NavigationContext.QueryString["gameID"]);
          
                _vm.Initialize(gameID);
            }
        }

        #endregion "Events"

    }
}