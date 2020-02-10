using Microsoft.WindowsAzure.MobileServices;
using ScoreSoccer8.Cloud.DbObjects;
using ScoreSoccer8.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScoreSoccer8.Cloud
{
    public class AllUsersConnection
    {
        private const string USER_ID = "USER_ID";
        private IMobileServiceTable<AllUsers> allUsersTable = App.MobileService.GetTable<AllUsers>();
        private static AllUsers _clientRow;

        public async void UpdateUserInformation(bool trial)
        {
            #if DEBUG

            #else

            await Task.Factory.StartNew(() => WriteUserInfoToCloud(trial));

            #endif
        }

        private async void WriteUserInfoToCloud(bool trial)
        {
            try
            {
                bool isAppPaid = false;

                if (App.FREE_VERSION)
                {
                    if (App.DoesUserHaveAbilityToTrackAllStats())
                    {
                        isAppPaid = true;
                    }
                }
                else
                {
                    if (!App.IsTrial)
                    {
                        isAppPaid = true;
                    }
                }

                if (IS.GetSetting(USER_ID) == null)
                {
                    string newGuid = Guid.NewGuid().ToString();

                    AllUsers rowToInsert = new AllUsers()
                    {
                        Guid = newGuid,
                        TimesOpened = 1,
                        FreeVersion = App.FREE_VERSION,
                        TrialVersion = App.IsTrial,
                        TotalTimeInApp = new TimeSpan(0, 0, 0),
                        Paid = isAppPaid,
                        LastLogin = DateTime.Now
                    };

                    await App.MobileService.GetTable<AllUsers>().InsertAsync(rowToInsert);

                    var newUserRow = await allUsersTable
                        .Where(x => x.Guid == newGuid).ToListAsync();

                    _clientRow = newUserRow.FirstOrDefault();

                    //Update IS
                    IS.SaveSetting(USER_ID, _clientRow.Id);

                    //Remove the GUID, we no longer need it
                    rowToInsert.Guid = string.Empty;
                    await App.MobileService.GetTable<AllUsers>().UpdateAsync(rowToInsert);
                }
                //Update
                else
                {
                    var rows = await allUsersTable
                        .Where(x => x.Id == IS.GetSettingStringValue(USER_ID)).ToListAsync();

                    AllUsers userRow = rows.ToList().FirstOrDefault();
                    userRow.LastLogin = DateTime.Now;
                    userRow.TimesOpened += 1;
                    userRow.FreeVersion = App.FREE_VERSION;
                    userRow.TrialVersion = App.IsTrial;
                    userRow.Paid = isAppPaid;

                    _clientRow = userRow;

                    await App.MobileService.GetTable<AllUsers>().UpdateAsync(userRow);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public async void UpdateTimeInApp(TimeSpan ts)
        {
            try
            {
                #if DEBUG

                #else

                _clientRow.TotalTimeInApp = _clientRow.TotalTimeInApp + ts;
                await App.MobileService.GetTable<AllUsers>().UpdateAsync(_clientRow);

                #endif
            }
            catch (Exception ex)
            {

            }
        }

        public async void UpdateEmailAndPassword(string emailAddress, string password)
        {
            try
            {
                var newUserRow = await allUsersTable
                        .Where(x => x.Email.Trim().ToUpper().Equals(emailAddress.ToUpper().Trim())).ToListAsync();

                if (newUserRow.Count() == 0)
                {

                    _clientRow.Email = emailAddress;
                    _clientRow.Password = password;

                    await App.MobileService.GetTable<AllUsers>().UpdateAsync(_clientRow);
                }
                else
                {
                    MessageBox.Show("The Email you have entered is already in use.");
                }

            }
            catch (Exception ex)
            {

            }
        }
    }
}
