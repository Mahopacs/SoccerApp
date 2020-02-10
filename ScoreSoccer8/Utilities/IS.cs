using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.Utilities
{
    public static class IS
    {
        public static void SaveSetting(string key, object value)
        {
            try
            {
                IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
                if (!appSettings.Contains(key))
                {
                    appSettings.Add(key, value);
                }
                else
                {
                    appSettings[key] = value;
                }

                appSettings.Save();
            }
            catch (Exception ex)
            {

            }
        }

        public static object GetSetting(string key)
        {
            object returnValue = null;

            try
            {
                IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
                if (appSettings.Contains(key))
                {
                    returnValue = appSettings[key];
                }
            }
            catch (Exception ex)
            {

            }

            return returnValue;
        }

        public static string GetSettingStringValue(string key)
        {
            string returnValue = string.Empty;

            try
            {
                IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
                if (appSettings.Contains(key))
                {
                    returnValue = appSettings[key].ToString();
                }
            }
            catch (Exception ex)
            {

            }

            return returnValue;
        }
        public static void RemoveSetting(string key)
        {
            try
            {
                IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;

                appSettings.Remove(key);
                appSettings.Save();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
