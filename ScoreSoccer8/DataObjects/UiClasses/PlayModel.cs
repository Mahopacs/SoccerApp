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
    public class PlayModel: Notification
    {
        public event EventHandler PlayDeleted;

        public PlayModel()
        {
            Play = new Play();
            Player1 = new Player();
            StatCategory = new StatCategory();
            StatDescription = new StatDescription();

            BackgroundColor = "Transparent";
            BackgroundOpacity = 1;

        }
        
        #region "Properties"

        private string _backgroundColor;
        public string BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; NotifyPropertyChanged("BackgroundColor"); }
        }

        private double _backgroundOpacity;
        public double BackgroundOpacity
        {
            get { return _backgroundOpacity; }
            set { _backgroundOpacity = value; NotifyPropertyChanged("BackgroundOpacity"); }
        }


        private Play _play;
        public Play Play
        {
            get { return _play; }
            set { _play = value; NotifyPropertyChanged("Play"); }
        }

        private Player _player1;
        public Player Player1
        {
            get { return _player1; }
            set { _player1 = value; NotifyPropertyChanged("Player1"); }
        }

        private StatCategory _statCategory;
        public StatCategory StatCategory
        {
            get { return _statCategory; }
            set { _statCategory = value; NotifyPropertyChanged("StatCategory"); }
        }

        private StatDescription _statDescription;
        public StatDescription StatDescription
        {
            get { return _statDescription; }
            set { _statDescription = value; NotifyPropertyChanged("StatDescription"); }
        }
        
        #endregion "Properties"

        #region "Events"

        protected virtual void OnPlayDeleted(EventArgs e)
        {
            EventHandler handler = PlayDeleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion "Events"

        #region Commands

        private ICommand _goToPlayDetailsCommand;
        public ICommand GoToPlayDetailsCommand
        {
            get
            {
                if (_goToPlayDetailsCommand == null)
                {
                    _goToPlayDetailsCommand = new DelegateCommand(param => this.GoToPlayDetailsScreen(), param => true);
                }

                return _goToPlayDetailsCommand;
            }
        }

        private ICommand _goToDeletePlayCommand;
        public ICommand GoToDeletePlayCommand
        {
            get
            {
                if (_goToDeletePlayCommand == null)
                {
                    _goToDeletePlayCommand = new DelegateCommand(param => this.RunTest(), param => true);
                }

                return _goToDeletePlayCommand;
            }
        }

        private ICommand _testCommand;
        public ICommand TestCommand
        {
            get
            {
                if (_testCommand == null)
                {
                    _testCommand = new DelegateCommand(param => this.GoToDeletePlay(), param => true);
                }

                return _testCommand;
            }
        }

        private void RunTest()
        {
            Messaging.RaiseShowLoadingScreen(this, new EventArgs());

            TestCommand.Execute(true);
        }


        #endregion Commands

        #region "Methods"

        private void GoToPlayDetailsScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/PlayDetails.xaml?gameID="+ Play.GameID +"&playID=" + Play.PlayID, UriKind.Relative));
        }

        private void GoToDeletePlay()
        {
            string playTextToDisplay;
            
            playTextToDisplay = Play.Period + "/" + Play.GameTime + ": " + Play.PlayText;
            MessageBoxResult result = MessageBox.Show(AppResources.Delete + " '" + playTextToDisplay + "' " + AppResources.Play + "?", AppResources.DeletePlay, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {                         
                DAL.Instance().DeletePlay(Play.GameID, Play.PlayID);
                OnPlayDeleted(EventArgs.Empty);                 
            }

            Messaging.RaiseHideLoadingScreen(this, new EventArgs());
        }
        #endregion "Methods"
    }
}
