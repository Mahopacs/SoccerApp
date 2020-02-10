using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace ScoreSoccer8.Views
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
        }

        private void SendEmail_Click(object sender, RoutedEventArgs e)
        {

            EmailComposeTask emailcomposer = new EmailComposeTask();
            emailcomposer.To = "KLBCreation01@yahoo.com";
            emailcomposer.Subject = "Feedback";
            emailcomposer.Show();

        }



    }
}