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
using ScoreSoccer8.Utilities;
using ScoreSoccer8.Classes;

namespace ScoreSoccer8.Views
{
    public partial class Help_Buttons : PhoneApplicationPage
    {

        IconsViewModel _vm = new IconsViewModel();

        public Help_Buttons()
        {
            InitializeComponent();
            this.DataContext = _vm;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.Count > 0)
            {
                 Enums.Screen scrrenEnum = (Enums.Screen)Enum.Parse(typeof(Enums.Screen), NavigationContext.QueryString["screenId"]);

                //int screenId = Convert.ToInt32(NavigationContext.QueryString["screenId"]);
                //Enums.Screen scrrenEnum = (Enums.Screen)screenId;
                _vm.Initialize(scrrenEnum);
            }
        }

    }
}