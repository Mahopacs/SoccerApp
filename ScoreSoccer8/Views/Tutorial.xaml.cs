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
using System.ComponentModel;

namespace ScoreSoccer8.Views
{
    public partial class Tutorial : PhoneApplicationPage
    {
        TutorialViewModel _vm;

        public Tutorial()
        {
            InitializeComponent();
            _vm = new TutorialViewModel();
            this.DataContext = _vm;
        }

        private void BackButtonClicked(object sender, CancelEventArgs e)
        {
        }
    }
}
