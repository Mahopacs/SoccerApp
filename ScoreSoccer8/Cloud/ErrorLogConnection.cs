using ScoreSoccer8.Cloud.DbObjects;
using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.Cloud
{
    public class ErrorLogConnection
    {
        private const string USER_ID = "USER_ID";

        public async void UpdateErrorLog(string method, string error)
        {
            await Task.Factory.StartNew(() => WriteErrorToCloud(IS.GetSettingStringValue(USER_ID), method, error));
        }

        private async void WriteErrorToCloud(string user, string method, string error)
        {
            try
            {
                ErrorLog rowToInsert = new ErrorLog() { ClientId = user, ErrorMessage = error, Method = method };
                await App.MobileService.GetTable<ErrorLog>().InsertAsync(rowToInsert);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
