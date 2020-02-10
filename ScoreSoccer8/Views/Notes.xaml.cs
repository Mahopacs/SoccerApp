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
    public partial class Notes : UserControl
    {
        NotesViewModel _vm = new NotesViewModel();

        public Notes()
        {
            InitializeComponent();
            this.DataContext = _vm;
        }
    }
}