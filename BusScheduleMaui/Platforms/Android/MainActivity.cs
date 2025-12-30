using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Xamarin.Google.Android.Play.Core.AppUpdate;
using Xamarin.Google.Android.Play.Core.Install.Model;
using Xamarin.Google.Android.Play.Core.Tasks;

namespace BusScheduleMaui
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity, IOnSuccessListener
    {
        private IAppUpdateManager _appUpdateManager;
        private const int UpdateRequestCode = 123;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            UserDialogs.Init(this);
            base.OnCreate(savedInstanceState);
            _appUpdateManager = AppUpdateManagerFactory.Create(this);
        }

        protected override void OnResume()
        {
            var sharedPref = GetPreferences(FileCreationMode.Private);
            var defaultValue = DateTime.MinValue.Ticks;
            var savedValue = sharedPref.GetLong("LAST_UPDATE_CHECK", defaultValue);
            var lastUpdateCheckTime = new DateTime(savedValue);

            if ((DateTime.Now - lastUpdateCheckTime).TotalHours >= 24)
            {
                var appUpdateInfoTask = _appUpdateManager.AppUpdateInfo;
                appUpdateInfoTask.AddOnSuccessListener(this);
            }
            base.OnResume();
        }

        public void OnSuccess(Java.Lang.Object data)
        {
            if (data is AppUpdateInfo appUpdateInfo)
            {
                var sharedPref = GetPreferences(FileCreationMode.Private);
                var editor = sharedPref.Edit();
                editor.PutLong("LAST_UPDATE_CHECK", DateTime.Now.Ticks);
                editor.Commit();

                if (appUpdateInfo.UpdateAvailability() == UpdateAvailability.UpdateAvailable
                          // For a flexible update, use AppUpdateType.FLEXIBLE
                          && appUpdateInfo.IsUpdateTypeAllowed(AppUpdateType.Immediate))
                {
                    // Request the update.
                    _appUpdateManager.StartUpdateFlowForResult(
                    // Pass the intent that is returned by 'getAppUpdateInfo()'.
                    appUpdateInfo,
                    // Or 'AppUpdateType.FLEXIBLE' for flexible updates.
                    AppUpdateType.Immediate,
                    // The current activity making the update request.
                    this,
                    // Include a request code to later monitor this update request.
                    UpdateRequestCode);
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
