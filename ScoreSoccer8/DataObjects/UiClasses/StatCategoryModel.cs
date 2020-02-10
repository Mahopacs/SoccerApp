using ScoreSoccer8.DataObjects.DbClasses;
using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ScoreSoccer8.DataObjects.UiClasses
{
    public class StatCategoryModel : Notification
    {

        public delegate void SaveStatsToDatabaseDelegate(object sender, SaveStatsEventArgs e);
        public event SaveStatsToDatabaseDelegate SaveStatsToDatabase;

        public class SaveStatsEventArgs
        {
            public SaveStatsEventArgs(StatCategoryModel statCat)
            {
                StatCat = statCat;
            }

            public StatCategoryModel StatCat { get; set; }
        }

        public StatCategoryModel()
        {
            StatCategory = new StatCategory();
            IsStatGray = Visibility.Collapsed;
        }

        #region "Properties"

        private StatCategory _statCategory;
        public StatCategory StatCategory
        {
            get { return _statCategory; }
            set { _statCategory = value; NotifyPropertyChanged("StatCategory"); }
        }

        private bool _showDescriptions = false;
        public bool ShowDescriptions
        {
            get { return _showDescriptions; }
            set
            {
                _showDescriptions = value;
                NotifyPropertyChanged("ShowDescriptions");
                SetDetailsVisibility();
            }
        }

        private Visibility _showCheckBox;
        public Visibility ShowCheckBox
        {
            get { return _showCheckBox; }
            set { _showCheckBox = value; NotifyPropertyChanged("ShowCheckBox"); }
        }

        private Visibility _detailsVisibility;
        public Visibility DetailsVisibility
        {
            get { return _detailsVisibility; }
            set { _detailsVisibility = value; NotifyPropertyChanged("DetailsVisibility"); }
        }

        private ObservableCollection<StatDescriptionModel> _descriptions;
        public ObservableCollection<StatDescriptionModel> Descriptions
        {
            get { return _descriptions; }
            set { _descriptions = value; NotifyPropertyChanged("Descriptions"); }
        }

        private bool _isStatEnabled;
        public bool IsStatEnabled
        {
            get { return _isStatEnabled; }
            set { _isStatEnabled = value; NotifyPropertyChanged("IsStatEnabled"); }
        }

        private Visibility _isStatGray;
        public Visibility IsStatGray
        {
            get { return _isStatGray; }
            set { _isStatGray = value; NotifyPropertyChanged("IsStatGray"); }
        }

        #endregion "Properties"

        #region "Events"

        private void desc_SaveStatDesc(object sender, EventArgs e)
        {
            SaveStatsToDatabase(sender, new SaveStatsEventArgs(this));
        }

        #endregion "Events"

        #region Commands

        private ICommand _goToSaveStatRightNowClickCommand;
        public ICommand GoToSaveStatRightNowClickCommand
        {
            get
            {
                if (_goToSaveStatRightNowClickCommand == null)
                {
                    _goToSaveStatRightNowClickCommand = new DelegateCommand(param => this.SaveStatPlayToDatabase(), param => true);
                }

                return _goToSaveStatRightNowClickCommand;
            }
        }

        private ICommand _showDescriptionClickCommand;
        public ICommand ShowDescriptionClickCommand
        {
            get
            {
                if (_showDescriptionClickCommand == null)
                {
                    _showDescriptionClickCommand = new DelegateCommand(param => this.SetDetailsVisibility(), param => true);
                }

                return _showDescriptionClickCommand;
            }
        }


        #endregion "Commands"

        #region "Methods"

        public void Initialize()
        {
            SetDetailsVisibility();

            if (Descriptions != null)
            {
                ShowCheckBox = Visibility.Visible;

                foreach (var desc in Descriptions)
                {
                    desc.SaveStatDesc += desc_SaveStatDesc;
                }
            }
            else
            {
                ShowCheckBox = Visibility.Collapsed;
            }
        }


        private void SaveStatPlayToDatabase()
        {
            if (SaveStatsToDatabase != null)
            {
                SaveStatsToDatabase(this, new SaveStatsEventArgs(this));
            }
        }

        private void SetDetailsVisibility()
        {
            if (_showDescriptions == false)
            {
                _showDescriptions = true;
            }
            else
            {
                _showDescriptions = false;
            }


            if (Descriptions != null && ShowDescriptions)
            {
                DetailsVisibility = Visibility.Visible;
            }
            else
            {
                DetailsVisibility = Visibility.Collapsed;
            }
        }

        public void Dispose()
        {
            if (Descriptions != null)
            {
                foreach (var desc in Descriptions)
                {
                    desc.SaveStatDesc -= desc_SaveStatDesc;
                }
            }
        }

        #endregion "Methods"

    }
}
