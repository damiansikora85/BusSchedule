using BusSchedule.Core.CloudService;
using BusSchedule.Core.Services;
using BusSchedule.Core.Utils;
using BusSchedule.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using TinyIoC;
using Xamarin.Plugin.Firebase;
using IPreferences = BusSchedule.Core.Services.IPreferences;

namespace BusSchedule.Tools
{
    public class DataUpdater
    {
        public static async Task<bool> InitDataOnline(IFileAccess fileAccess)
        {
            return await TryUpdateSchedule(fileAccess);
        }

        public static async Task InitDataOffline(IFileAccess fileAccess)
        {
            if (!fileAccess.CheckLocalFileExist(App.DB_FILENAME))
            {
                _ = await fileAccess.CopyFromAssetsToLocal(fileAccess.GetLocalFilePath(App.DB_FILENAME), App.DB_FILENAME);
            }
        }
        public static async Task<bool> TryUpdateSchedule(IFileAccess fileAccess)
        {
            var resolver = TinyIoCContainer.Current;
            var cloudService = resolver.Resolve<ICloudService>();
            var cloudStorage = resolver.Resolve<IFirebaseStorage>();
            var preferences = resolver.Resolve<IPreferences>();

            var filename = await cloudService.GetLatestScheduleFilename();
            if(preferences.Get("dbFilename", "sqlite20211129.db") != filename)
            {
                var path = await cloudStorage.DownloadFileToLocalStorage("/"+filename);
                await fileAccess.CopyToLocal(path, filename);
                preferences.Set("dbFilename", filename);
            }

            var dataProvider = resolver.Resolve<IDataProvider>();
            var databasePath = fileAccess.GetLocalFilePath(filename);
            dataProvider.SetDatabasePath(databasePath);
            return true;
        }

        public static async Task UpdateDataIfNeeded(IFileAccess fileAccess, IPreferences preferences)
        {
            //var dbVersion = await fileAccess.ReadAssetFile("DbVersion.txt");
            if (!fileAccess.CheckLocalFileExist(App.DB_FILENAME))
            {
                _ = await fileAccess.CopyFromAssetsToLocal(fileAccess.GetLocalFilePath(App.DB_FILENAME), App.DB_FILENAME);
            }
            else
            {
                var currentDbVersion = preferences.Get("dbVersion", "1");
                //if (currentDbVersion != dbVersion)
                {
                    _ = await fileAccess.CopyFromAssetsToLocal(fileAccess.GetLocalFilePath(App.DB_FILENAME), App.DB_FILENAME);
                }
            }
            //preferences.Set("dbVersion", dbVersion);
        }
    }
}
