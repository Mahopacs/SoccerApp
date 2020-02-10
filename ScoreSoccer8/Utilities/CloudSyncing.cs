using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSoccer8.Utilities
{
    public static class CloudSyncing
    {

        //TO DO
        //1) When saving data on the phone also need to save to the cloud
        //2) After data also saved to cloud, update cloud tables 'On Phone' = 'Y' and update phone tables 'On Cloud' = 'Y'

        //TO DO
        //1) Need a user login screen, where the user creates an id/pw.
        //2) As part of the initialization, SyncDataFromCloud and SyncDataFromDevice should be run


        public static void SyncData()
        {
            SyncDataFromCloud();
            SyncDataFromDevice();
        }

        //What on cloud is not on the device?
        //Proc will go through all tables on the cloud and for all records where the ‘On Phone’         
        //(or if being synced to Tablet the ‘On Tablet’ column) value is false, 
        //it will write those records to the device (and set the ‘On Cloud’ value on the phone to true, i.e. since we just synced from the cloud)

        public static void SyncDataFromCloud()
        {
            //1) Get data from cloud tables where 'On Phone' != 'Y'
            //2) Write this data to the phone tables and set the 'On Cloud' = 'Y'
            //3) Set the cloud tables column for 'On Phone' = 'Y'
        }

        //What on device is not on the cloud?
        //Proc will go through all tables on the device and for all records where the ‘On Cloud’ value is false, 
        //it will write those records to the cloud (and set the ‘On Phone’ value on the cloud to true, i.e. since we just synced from the phone)
        //(or if syncing to the tablet, set the ‘On Tablet’ column to true)
        public static void SyncDataFromDevice()
        {
            //1) Get data from phone tables where 'On Cloud' != 'Y'
            //2) Write this data to the cloud tables and set the 'On Phone' = 'Y'
            //3) Set the phone tables column for 'On Cloud' = 'Y'
        }
    }
}
