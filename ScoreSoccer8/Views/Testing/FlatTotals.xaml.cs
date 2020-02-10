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

namespace ScoreSoccer8.Views.Testing
{
    public partial class FlatTotals : PhoneApplicationPage
    {
        private FlatTotalsViewModel _vm;

        public FlatTotals()
        {
            InitializeComponent();
            _vm = new FlatTotalsViewModel();
            this.DataContext = _vm;
        }
    }
}