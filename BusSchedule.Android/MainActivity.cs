using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Acr.UserDialogs;
using Com.Google.Android.Play.Core.Appupdate;
using Com.Google.Android.Play.Core.Tasks;
using Com.Google.Android.Play.Core.Install.Model;
using Android.Content;

namespace BusSchedule.Droid
{
    [Activity(Label = "Rozkład jazdy MZK Wejherowo", Icon = "@mipmap/icon", RoundIcon = "@mipmap/icon_round", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IOnSuccessListener
    {
        private IAppUpdateManager _appUpdateManager;
        private const int UpdateRequestCode = 123;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            _appUpdateManager = AppUpdateManagerFactory.Create(this);

            UserDialogs.Init(this);
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.SetFlags("Shapes_Experimental");
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
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
    }
}