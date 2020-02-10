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

namespace ScoreSoccer8.Views
{
    public partial class Statistics_Timeline : PhoneApplicationPage
    {

        private TimelineViewModel _vm;

        public Statistics_Timeline()
        {
            InitializeComponent();

            _vm = new TimelineViewModel();
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

    }
}