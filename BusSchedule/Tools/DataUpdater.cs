using BusSchedule.Interfaces;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace BusSchedule.Tools
{
    public class DataUpdater
    {
        public static async Task UpdateDataIfNeeded(IFileAccess fileAccess, IPreferences preferences)
        {
            var dbVersion = await fileAccess.ReadAssetFile("DbVersion.txt");
            if (!fileAccess.CheckLocalFileExist("sqlite.db"))
            {
                _ = await fileAccess.CopyFromAssetsToLocal(fileAccess.GetLocalFilePath("sqlite.db"), "sqlite.db");
            }
            else
            {
                var currentDbVersion = preferences.Get("dbVersion", "1");
                if(currentDbVersion != dbVersion)
                {
                    _ = await fileAccess.CopyFromAssetsToLocal(fileAccess.GetLocalFilePath("sqlite.db"), "sqlite.db");
                }
            }
            preferences.Set("dbVersion", dbVersion);
        }

        public static async Task ForceCopy(IFileAccess fileAccess)
        {
            try
            {
                _ = await fileAccess.CopyFromAssetsToLocal(fileAccess.GetLocalFilePath("sqlite.db"), "sqlite.db");
            }
            catch(Exception exc)
            {
                var msg = exc.Message;
            }
        }
    }
}
