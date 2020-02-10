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
    public partial class PurchaseApp : PhoneApplicationPage
    {
        private PurchaseAppViewModel _vm;

        public PurchaseApp()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            _vm = new PurchaseAppViewModel();
            DataContext = _vm;
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (e.Orientation == PageOrientation.PortraitDown || e.Orientation == PageOrientation.PortraitUp)
            {

                //Grid.SetRow(images, 0);
                //Grid.SetColumn(images, 0);
                //Grid.SetColumnSpan(images, 2);
                //Grid.SetRowSpan(images, 1);


                //Grid.SetRow(buttons, 1);
                //Grid.SetColumn(buttons, 0);
                //Grid.SetColumnSpan(buttons, 2);


                //Grid.SetRow(teamsButton, 0);
                //Grid.SetColumn(teamsButton, 0);
                //Grid.SetColumnSpan(teamsButton, 2);

                //Grid.SetRow(playersButton, 0);
                //Grid.SetColumn(playersButton, 2);
                //Grid.SetColumnSpan(playersButton, 2);

                //Grid.SetRow(leaguesButton, 1);
                //Grid.SetColumn(leaguesButton, 0);
                //Grid.SetColumnSpan(leaguesButton, 2);

                //Grid.SetRow(gamesButton, 1);
                //Grid.SetColumn(gamesButton, 2);
                //Grid.SetColumnSpan(gamesButton, 2);

                //_vm.BuyMargin = new Thickness(0, 0, 0, 75);

            }
            else
            {

                //Grid.SetRow(images, 0);
                //Grid.SetColumn(images, 0);
                //Grid.SetColumnSpan(images, 1);
                //Grid.SetRowSpan(images, 2);


                //Grid.SetRow(buttons, 1);
                //Grid.SetColumn(buttons, 0);
                //Grid.SetColumnSpan(buttons, 2);


                //Grid.SetRow(teamsButton, 0);
                //Grid.SetColumn(teamsButton, 0);
                //Grid.SetColumnSpan(teamsButton, 1);

                //Grid.SetRow(playersButton, 0);
                //Grid.SetColumn(playersButton, 1);
                //Grid.SetColumnSpan(playersButton, 1);

                //Grid.SetRow(leaguesButton, 0);
                //Grid.SetColumn(leaguesButton, 2);
                //Grid.SetColumnSpan(leaguesButton, 1);

                //Grid.SetRow(gamesButton, 0);
                //Grid.SetColumn(gamesButton, 3);
                //Grid.SetColumnSpan(gamesButton, 1);

                //_vm.BuyMargin = new Thickness(0, 0, 0, 5);

            }
        }

    }
}