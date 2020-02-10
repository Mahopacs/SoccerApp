using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if DEBUG
using MockIAPLib;
using Store = MockIAPLib;
using IAPMockLibSample;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows;
using System.Windows.Controls;
using ScoreSoccer8.DataAccess;
#else
using Windows.ApplicationModel.Store;
using Store = Windows.ApplicationModel.Store;
using System.Windows.Input;
using System.Text.RegularExpressions;
using ScoreSoccer8.DataAccess;
using System.Windows;
using System.Windows.Controls;
#endif

namespace ScoreSoccer8.ViewModels
{
    public class PurchaseAppViewModel : Notification
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PurchaseAppViewModel()
        {
            ScreenWidth = System.Windows.Application.Current.Host.Content.ActualWidth - 20;
            PicItems = new ObservableCollection<PicItem>();
            LoadItems();
        }

        private ObservableCollection<PicItem> _picItems;
        public ObservableCollection<PicItem> PicItems
        {
            get { return _picItems; }
            set { _picItems = value; }
        }

        private double _screenWidth;
        public double ScreenWidth
        {
            get { return _screenWidth; }
            set { _screenWidth = value; NotifyPropertyChanged("ScreenWidth"); }
        }

        private async void LoadItems()
        {
            PicItems.Clear();

            ListingInformation li = await Store.CurrentApp.LoadListingInformationAsync();

            foreach (string key in li.ProductListings.Keys)
            {
                ProductListing pListing = li.ProductListings[key];

                string status = Windows.ApplicationModel.Store.CurrentApp.LicenseInformation.ProductLicenses[key].IsActive ? "Purchased." : "Available to purchase.";

                string imageLink = string.Empty;

                //if (mySM.StoreItems.TryGetValue(key, out imageLink))
                PicItems.Add(new PicItem { ImgLink = "/Assets/StatsPickerPic.png", Status = status, Key = key, Price = pListing.FormattedPrice,
                    Description = Regex.Replace(pListing.Description, @"\s+", " ") });
            }
        }
    }

    public class PicItem : Notification
    {
        private string _imgLink;
        public string ImgLink
        {
            get { return _imgLink; }
            set { _imgLink = value; NotifyPropertyChanged("ImgLink"); }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set { _status = value; NotifyPropertyChanged("Status"); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; NotifyPropertyChanged("Description"); }
        }

        private string _key;
        public string Key
        {
            get { return _key; }
            set { _key = value; NotifyPropertyChanged("Key"); }
        }

        private string _price;
        public string Price
        {
            get { return _price; }
            set { _price = value; NotifyPropertyChanged("Price"); }
        }
        

        private ICommand _purchaseCommand;
        public ICommand PurchaseCommand
        {
            get
            {
                if (_purchaseCommand == null)
                {
                    _purchaseCommand = new DelegateCommand(param => Purchase(), param => true);
                }

                return _purchaseCommand;
            }
        }

        public async void Purchase()
        {
            try
            {
                if (!Store.CurrentApp.LicenseInformation.ProductLicenses[Key].IsActive)
                {
                    ListingInformation li = await Store.CurrentApp.LoadListingInformationAsync();
                    string pID = li.ProductListings[Key].ProductId;

                    string receipt = await Store.CurrentApp.RequestProductPurchaseAsync(pID, true);

                    string status = Store.CurrentApp.LicenseInformation.ProductLicenses[Key].IsActive ? "Purchased" : "Available to purchase.";

                    if (status == "Purchased")
                    {
                        IS.SaveSetting(App.FULL_STATS_FREE_VERSION, true);
                        DAL.Instance().SetUpStatsForApp();
                    }

                    (Application.Current.RootVisual as Frame).GoBack();
                }
            }
            catch (Exception ex)
            {
                //Trapping when the user clicks cancel here
            }
        }
    }
}
