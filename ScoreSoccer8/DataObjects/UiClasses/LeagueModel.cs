using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.Resources;
using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ScoreSoccer8.DataObjects.UiClasses
{
    public class LeagueModel : Notification
    {
        public event EventHandler LeagueDeleted;

        public LeagueModel()
        {
            League = new League();
        }

        #region "Properties"

        private League _league;
        public League League
        {
            get { return _league; }
            set { _league = value; NotifyPropertyChanged("League"); }
        }

        #endregion "Properties"

        #region "Events"

        protected virtual void OnLeagueDeleted(EventArgs e)
        {
            EventHandler handler = LeagueDeleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion "Events"

        #region Commands

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

        private ICommand _goToDeleteLeaguesCommand;
        public ICommand GoToDeleteLeaguesCommand
        {
            get
            {
                if (_goToDeleteLeaguesCommand == null)
                {
                    _goToDeleteLeaguesCommand = new DelegateCommand(param => this.GoToDeleteLeague(), param => true);
                }

                return _goToDeleteLeaguesCommand;
            }
        }

        #endregion Commands

        #region "Methods"

        private void GoToLeagueDetailsScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/LeagueDetails.xaml?parameter=" + League.LeagueID, UriKind.Relative));
        }

        private void GoToDeleteLeague()
        {
            MessageBoxResult result = MessageBox.Show(AppResources.Delete + " '" + League.LeagueName + "' " + AppResources.League + "?", AppResources.DeleteLeague, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                this.League.Visible = "N";
                BaseTableDataAccess.Instance().UpsertLeague(this.League);
                OnLeagueDeleted(EventArgs.Empty);
            }
        }
        #endregion "Methods"
    }
}
