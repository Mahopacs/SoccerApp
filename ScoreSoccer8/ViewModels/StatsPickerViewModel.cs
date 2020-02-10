using Microsoft.Phone.Controls;
using ScoreSoccer8.Classes;
using ScoreSoccer8.DataAccess;
using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.DataObjects.UiClasses;
using ScoreSoccer8.Resources;
using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ScoreSoccer8.ViewModels
{
    public class StatsPickerViewModel : Notification, IDisposable
    {

        public StatsPickerViewModel()
        {
            LoadScreen();
            PlayModel = new PlayModel();
        }

        private ICommand _goToTranslationsCommand;
        public ICommand GoToTranslationsCommand
        {
            get
            {
                if (_goToTranslationsCommand == null)
                {
                    _goToTranslationsCommand = new DelegateCommand(param => this.GoToTranslations(), param => true);
                }

                return _goToTranslationsCommand;
            }
        }

        public void GoToTranslations()
        {
            (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/StatPickerHelp.xaml", UriKind.Relative));
        }


        #region "Properties"

        private ObservableCollection<StatCategoryModel> _statCategories1;
        public ObservableCollection<StatCategoryModel> StatCategories1
        {
            get { return _statCategories1; }
            set { _statCategories1 = value; NotifyPropertyChanged("StatCategories1"); }
        }

        private ObservableCollection<StatCategoryModel> _statCategories2;
        public ObservableCollection<StatCategoryModel> StatCategories2
        {
            get { return _statCategories2; }
            set { _statCategories2 = value; NotifyPropertyChanged("StatCategories2"); }
        }

        private Visibility _hasDescriptions;
        public Visibility HasDescriptions
        {
            get { return _hasDescriptions; }
            set { _hasDescriptions = value; NotifyPropertyChanged("HasDescriptions"); }
        }

        private PlayModel _playModel;
        public PlayModel PlayModel
        {
            get { return _playModel; }
            set { _playModel = value; NotifyPropertyChanged("PlayModel"); }
        }

        #endregion "Properties"

        #region "Methods"

        private void LoadScreen()
        {
            ObservableCollection<StatCategoryModel> statList = new ObservableCollection<StatCategoryModel>();

            StatCategories1 = new ObservableCollection<StatCategoryModel>();
            StatCategories2 = new ObservableCollection<StatCategoryModel>();

            statList = DAL.Instance().GetVisibleStats(false, false);

            foreach (var item in statList)
            {
                item.Initialize();
                item.SaveStatsToDatabase += item_SaveStatsToDatabase;

                if (item.StatCategory.Active == "Y")
                {
                    item.IsStatEnabled = true;
                    item.IsStatGray = Visibility.Collapsed;
                }
                else
                {
                    item.IsStatEnabled = false;
                    item.IsStatGray = Visibility.Visible;
                }

                Common.Instance().GlobalizeStatCatAndDescription(item);
                StatCategories1.Add(item);
            }

        }

        public void Dispose()
        {
            try
            {
                //Now need to unsuscribe from event
                foreach (var item in StatCategories1)
                {
                    item.SaveStatsToDatabase -= item_SaveStatsToDatabase;
                    item.Dispose();
                }

                foreach (var item in StatCategories2)
                {
                    item.SaveStatsToDatabase -= item_SaveStatsToDatabase;
                    item.Dispose();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void item_SaveStatsToDatabase(object sender, StatCategoryModel.SaveStatsEventArgs e)
        {
            StatCategory stat = sender as StatCategory;
            StatDescriptionModel statDesc = sender as StatDescriptionModel;

            if (stat != null)
            {
                PlayModel.Play.StatCategoryID = stat.StatCategoryID;
                PlayModel.StatCategory.StatCategoryName = stat.StatCategoryName;
                PlayModel.StatDescription.StatDescriptionName = string.Empty;
            }
            else
            {
                PlayModel.Play.StatCategoryID = e.StatCat.StatCategory.StatCategoryID;
                PlayModel.StatCategory.StatCategoryName = e.StatCat.StatCategory.StatCategoryName;

                if (statDesc != null)
                {
                    PlayModel.Play.StatDescriptionID = statDesc.StatDescription.StatDescriptionID;
                    PlayModel.StatDescription.StatDescriptionName = statDesc.StatDescription.StatDescriptionName;
                }
            }
           
            if ((PlayModel.StatDescription.StatDescriptionName == AppResources.ShotDetails) || (PlayModel.StatDescription.StatDescriptionName == AppResources.ForGoal))
            {
                (Application.Current.RootVisual as Frame).Navigate(new Uri("/Views/ShotDetails.xaml?playID=0&gameID= " + PlayModel.Play.GameID +
                    "&playerID= " + PlayModel.Play.Player1ID + "&teamID=" + PlayModel.Play.TeamID +
                     "&period= " + PlayModel.Play.Period + "&gameTime=" + PlayModel.Play.GameTime +
                     "&GMPlayer1PositionID=" + PlayModel.Play.GMPlayer1PositionID + "&otherTeamGoalieID=" + PlayModel.Play.OtherTeamGoalieID +
                     "&statcategoryID=" + PlayModel.Play.StatCategoryID +
                     "&statdescriptionID=" + PlayModel.Play.StatDescriptionID, UriKind.Relative));
            }
            else
            {
                DAL.Instance().UpsertPlay(PlayModel.Play, "GM");
                (Application.Current.RootVisual as Frame).GoBack();
            }
        }
   
        public void Initialize(Play play)
        {
            PlayModel.Play = play;
        }

        #endregion "Methods"

    }
}

