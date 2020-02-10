using ScoreSoccer8.Classes;
using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ScoreSoccer8.ViewModels
{   
    public class GamesViewModel : Notification
    {   
        public GamesViewModel()
        {
            
       }

        #region "Properties"

        private ObservableCollection<GameModel> _gamesList;
        public ObservableCollection<GameModel> GamesList
        {
            get { return _gamesList; }
            set { _gamesList = value; NotifyPropertyChanged("GamesList"); }
        }

        private bool _displayDeletedGames;
        public bool DisplayDeletedGames
        {
            get { return _displayDeletedGames; }
            set
            {
                _displayDeletedGames = value;
                NotifyPropertyChanged("DisplayDeletedGames");
                DisplayDeletedGamesClicked();
            }
        }

        #endregion "Properties"

        #region "Commands"


        private ICommand _gameStatsClickCommand;
        public ICommand GameStatsClickCommand
        {
            get
            {
                if (_gameStatsClickCommand == null)
                {
                    _gameStatsClickCommand = new DelegateCommand(param => this.GoToGameStatsScreen(), param => true);
                }

                return _gameStatsClickCommand;
            }
        }


        private ICommand _addGameClickCommand;
        public ICommand AddGameClickCommand
        {
            get
            {
                if (_addGameClickCommand == null)
                {
                    _addGameClickCommand = new DelegateCommand(param => this.GoToGameDetailsScreenAddGame(), param => true);
                }

                return _addGameClickCommand;
            }
        }

        private ICommand _helpClickCommand;
        public ICommand HelpClickCommand
        {
            get
            {
                if (_helpClickCommand == null)
                {
                    _helpClickCommand = new DelegateCommand(param => this.GoHelpScreen(), param => true);
                }

                return _helpClickCommand;
            }
        }

        #endregion "Commands"

        #region "Methods"
      
        public void Initialize()
        {
            PopulateGamesList("N");
        }
            
        private void PopulateGamesList(string displayDeletedGames)
        {
            GamesList = new ObservableCollection<GameModel>();

            GamesList = new ObservableCollection<GameModel>(DAL.Instance().GetGames(displayDeletedGames).Where(x => x.Game.SampleData.ToUpper().Equals("N")));
         
            foreach (var item in GamesList)
            {
                item.GameDeleted += item_GameDeleted;
            }
        }

        void item_GameDeleted(object sender, EventArgs e)
        {
            if (_displayDeletedGames == true)
            {
                PopulateGamesList("Y");
            }
            else
            {
                PopulateGamesList("N");
            }
        }

        private void GoHelpScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/Help_Buttons.xaml?screenId=" + Enums.Screen.Games, UriKind.Relative));
        }

        private void GoToGameStatsScreen()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/BoxScore.xaml?teamid=0", UriKind.Relative));
        }

        private void GoToGameDetailsScreenAddGame()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/GameDetails.xaml?parameter=0", UriKind.Relative));
        }

        private void DisplayDeletedGamesClicked()
        {
            if (_displayDeletedGames == true)
            {
                PopulateGamesList("Y");
            }
            else
            {
                PopulateGamesList("N");
            }
        } 

        #endregion "Methods"
    }
}
