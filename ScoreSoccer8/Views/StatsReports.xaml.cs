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
    public partial class StatsReports : PhoneApplicationPage
    {
        StatsReportsViewModel _vm;

        public StatsReports()
        {
            InitializeComponent();
            _vm = new StatsReportsViewModel();
            this.DataContext = _vm;
        }
    }
}