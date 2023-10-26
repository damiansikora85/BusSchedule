using BusSchedule.Core.CloudService;
using System;
using System.Threading.Tasks;
using Xamarin.Plugin.Firebase;

namespace BusSchedule.Core.Services
{
    public class ScheduleUpdater : IScheduleUpdater
    {
        private readonly ICloudService _cloudService;
        private readonly IFirebaseStorage _firebaseStorage;
        private readonly IPreferences _preferences;
        private const int SCHEDULE_UPDATE_DAYS = 1;

        public ScheduleUpdater(ICloudService cloudService, IFirebaseStorage firebaseStorage, IPreferences preferences)
        {
            _cloudService = cloudService;
            _firebaseStorage = firebaseStorage;
            _preferences = preferences;
        }

        public async Task<bool> TryUpdateSchedule(IFileAccess fileAccess, string defaultDbFilename)
        {
            //return false;
            var result = false;
            var lastNewsUpdateTime = _preferences.Get("lastScheduleUpdate", DateTime.MinValue);
            if ((DateTime.Now - lastNewsUpdateTime).TotalDays < SCHEDULE_UPDATE_DAYS)
            {
                return false;
            }
            var filename = await _cloudService.GetLatestScheduleFilename();
            if (_preferences.Get("dbFilename", defaultDbFilename) != filename)
            {
                var path = await _firebaseStorage.DownloadFileToLocalStorage("/" + filename);
                await fileAccess.CopyToLocal(path, filename);
                _preferences.Set("dbFilename", filename);
                result = true;
            }
            _preferences.Set("lastScheduleUpdate", DateTime.Now);
            return result;
        }
    }
}
