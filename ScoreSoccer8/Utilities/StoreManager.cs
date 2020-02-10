using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#if DEBUG
using MockIAPLib;
using Store = MockIAPLib;
using ScoreSoccer8.Utilities;
#else
using Windows.ApplicationModel.Store;
#endif

namespace IAPMockLibSample
{
    public class StoreManager
    {
        public Dictionary<string, string> StoreItems = new Dictionary<string, string>();

        public StoreManager()
        {
            // Populate the store
            StoreItems.Add("Full Stats Package", "/Assets/StatsPickerPic.png");
            //StoreItems.Add("img.2", "/Res/Image/2.png");
            //StoreItems.Add("img.3", "/Res/Image/3.png");
            //StoreItems.Add("img.4", "/Res/Image/4.png");
            //StoreItems.Add("img.5", "/Res/Image/5.png");
            //StoreItems.Add("img.6", "/Res/Image/6.png");
        }

        public async Task<List<string>> GetOwnedItems()
        {
            List<string> items = new List<string>();

            ListingInformation li = await CurrentApp.LoadListingInformationAsync();

            foreach (string key in li.ProductListings.Keys)
            {
                if (CurrentApp.LicenseInformation.ProductLicenses[key].IsActive && StoreItems.ContainsKey(key))
                    items.Add(StoreItems[key]);
            }

            return items;
        }
    }
}

