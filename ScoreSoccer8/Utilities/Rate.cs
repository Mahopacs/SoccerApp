using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using ScoreSoccer8.Cloud;
using ScoreSoccer8.DataAccess;
using ScoreSoccer8.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScoreSoccer8.Utilities
{
    public static class Rate
    {

        //Prompt to rate every 10th time. 25th time ok only to rate, never prompt them again after they rate.
        public static void RateTheApp()
        {      
            try
            {
                //Could have called procs to get if app has been rated or the opened count, but since we do this in app intialize, using global variables instead
                //hasAppBeenRated = HasAppBeenRated();
                //appOpenedCount = GetAppOpenedCount();

                //10/25/14 TJY Change from prompting to rate every 10 times to every 2 times
                if ((App.gAppOpenedCount % 2 == 0) && App.gHasAppBeenRated.ToUpper() == "NO")
                {
                    CustomMessageBox messageBox = new CustomMessageBox()
                    {
                        Caption = AppResources.RateApp,
                        Message = AppResources.RateAppQuestion,
                        LeftButtonContent = AppResources.RateLeftButton,
                        RightButtonContent = AppResources.RateRightButton
                    };

                    messageBox.Show();

                    messageBox.Dismissed += (s1, e1) =>
                    {
                        switch (e1.Result)
                        {
                            case CustomMessageBoxResult.LeftButton:
                                MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
                                marketplaceReviewTask.Show();
                                IS.SaveSetting("AppRated", "YES");
                                App.gHasAppBeenRated = "YES";
                                DAL.Instance().SetUpStatsForApp(); //10/25/14 TJY If Rate app give all stats functionality
                                break;
                            case CustomMessageBoxResult.RightButton:
                                IS.SaveSetting("AppRated", "NO");
                                App.gHasAppBeenRated = "NO";
                                break;
                            case CustomMessageBoxResult.None:
                                break;
                            default:
                                break;
                        }
                    };
                }
                else
                {
                    if (App.gAppOpenedCount >= 12 && App.gHasAppBeenRated.ToUpper() == "NO")
                    {
                        CustomMessageBox messageBox = new CustomMessageBox()
                        {
                            Caption = AppResources.RateApp,
                            Message = AppResources.RateAppPrompt,
                            LeftButtonContent = AppResources.Ok                       
                        };
                        messageBox.Show();

                        messageBox.Dismissed += (s1, e1) =>
                        {
                            switch (e1.Result)
                            {
                                case CustomMessageBoxResult.LeftButton:
                                    MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
                                    marketplaceReviewTask.Show();
                                    IS.SaveSetting("AppRated", "YES");
                                    App.gHasAppBeenRated = "YES";
                                    DAL.Instance().SetUpStatsForApp(); //10/25/14 TJY If Rate app give all stats functionality
                                    break;
                                case CustomMessageBoxResult.RightButton:
                                    break;
                                case CustomMessageBoxResult.None:
                                    break;
                                default:
                                    break;
                            }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogConnection cloud = new ErrorLogConnection();
                cloud.UpdateErrorLog("RATE.RateTheApp", ex.Message.ToString());        
            }
        }

        public static int GetAppOpenedCount()
        {
            int returnValue = 0;
            string settingValue = string.Empty;

            try
            {
                if (IS.GetSettingStringValue("AppOpenedCount") != string.Empty)
                {
                    settingValue = IS.GetSettingStringValue("AppOpenedCount");
                    returnValue = Convert.ToInt16(settingValue);
                }
                else   //has not been opened yet 
                {
                    returnValue = 0;
                }
            }
            catch (Exception ex)
            {
                ErrorLogConnection cloud = new ErrorLogConnection();
                cloud.UpdateErrorLog("Rate.GetOpenedCount", ex.Message.ToString());
                return 0;
            }
            return returnValue;
        }

        public static void UpdateOpenedCount()
        {
            int newOpenedCount = 0;
            string settingValue = string.Empty;

            try
            {
                if (IS.GetSettingStringValue("AppOpenedCount") != string.Empty)
                {
                    settingValue = IS.GetSettingStringValue("AppOpenedCount");
                    newOpenedCount = Convert.ToInt16(settingValue) + 1;
                    IS.SaveSetting("AppOpenedCount", newOpenedCount.ToString());
                }
                else   //has not been opened yet so intitialize as first time being opened
                {
                    IS.SaveSetting("AppOpenedCount", "1");
                }
            }
            catch (Exception ex)
            {
                ErrorLogConnection cloud = new ErrorLogConnection();
                cloud.UpdateErrorLog("Rate.UpdateOpenedCount", ex.Message.ToString());
            }
        }



        public static string HasAppBeenRated()
        {
            string returnValue = string.Empty;

            try
            {
                if (IS.GetSettingStringValue("AppRated") != string.Empty)
                {
                    returnValue = IS.GetSettingStringValue("AppRated").ToUpper();
                }
                else
                {
                    IS.SaveSetting("AppRated", "NO");
                    returnValue = "NO";
                }
            }
            catch (Exception)
            {
                return "No";
            }
            return returnValue;
        }
    }
}
