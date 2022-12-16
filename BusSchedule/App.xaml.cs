using Acr.UserDialogs;
using BusSchedule.Core.CloudService;
using BusSchedule.Core.CloudService.Impl;
using BusSchedule.Core.Messages;
using BusSchedule.Core.Services;
using BusSchedule.Core.Utils;
using BusSchedule.Interfaces.Implementation;
using BusSchedule.Pages;
using BusSchedule.Providers;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Plugin.Firebase;

namespace BusSchedule
{
    public partial class App : Application
    {
        public static string DB_FILENAME = "sqlite20221215.db";
        private SemaphoreSlim _updateSemafor = new SemaphoreSlim(1);
        public App()
        {
            InitializeComponent();
            VersionTracking.Track();
            RegisterIoC();
            UserAppTheme = OSAppTheme.Light;
            TaskScheduler.UnobservedTaskException += UnobservedTaskExceptionHandler;
           
            MainPage = new NavigationPage(new RoutesPage()) { BarBackgroundColor = Color.FromHex("#237194") };
        }

        private void RegisterIoC()
        {
            var container = TinyIoCContainer.Current;
            container.Register<IPreferences, CustomPreferences>();
            
            var databasePath = DependencyService.Get<IFileAccess>().GetLocalFilePath(GetDatabaseFilename());
            var dataProvider = new SQLDataProvider(databasePath);
            container.Register<IDataProvider, SQLDataProvider>(dataProvider);
            container.Register<ICloudService, FirebaseCloudService>();
            container.Register<INewsService, NewsService>(new NewsService(new FirebaseCloudService(), dataProvider));
            container.Register<IFirebaseStorage, Storage>().AsSingleton();
            container.Register<IScheduleUpdater, ScheduleUpdater>();
        }

        private string GetDatabaseFilename()
        {
            var container = TinyIoCContainer.Current;
            var preferences = container.Resolve<IPreferences>();
            return preferences.Get("dbFilename", DB_FILENAME);
        }

        private void UnobservedTaskExceptionHandler(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Crashes.TrackError(e.Exception);
        }

        protected override async void OnStart()
        {
            AppCenter.Start("android=fc2cc03c-f502-42d6-b5ed-8373e82d03c2;",
                  typeof(Analytics), typeof(Crashes));
            Task.Run(async () =>
            {
                try
                {
                    var resolver = TinyIoCContainer.Current;
                    var scheduleUpdater = resolver.Resolve<IScheduleUpdater>();
                    await TryUpdateSchedule(scheduleUpdater);
                    await TryUpdateNews(resolver.Resolve<INewsService>(), resolver.Resolve<IPreferences>());
                }
                catch(Exception ex)
                {
                    var message = ex.ToString();
                }
            });
        }

        protected override void OnSleep()
        {
        }

        protected override async void OnResume()
        {
            Task.Run(async () =>
            {
                var resolver = TinyIoCContainer.Current;
                await TryUpdateSchedule(resolver.Resolve<IScheduleUpdater>());
                await TryUpdateNews(resolver.Resolve<INewsService>(), resolver.Resolve<IPreferences>());
            });
        }

        private async Task TryUpdateNews(INewsService newsService, IPreferences preferences)
        {
            await _updateSemafor.WaitAsync();
            try
            {
                var current = Connectivity.NetworkAccess;
                if (current == NetworkAccess.Internet && await newsService.TryUpdateNews(preferences.Get("lastNewsUpdate", DateTime.MinValue)))
                {
                    preferences.Set("lastNewsUpdate", DateTime.Now);
                }
            }
            catch (Exception exc)
            {
                Crashes.TrackError(exc, new Dictionary<string, string>
                {
                    { "connectivity", Connectivity.NetworkAccess.ToString() }
                });
            }
            finally
            {
                _updateSemafor.Release();
            }
        }

        private async Task TryUpdateSchedule(IScheduleUpdater scheduleUpdater)
        {
            try
            {
                var current = Connectivity.NetworkAccess;
                if (current == NetworkAccess.Internet && await scheduleUpdater.TryUpdateSchedule(DependencyService.Get<IFileAccess>(), DB_FILENAME))
                {
                    await OnScheduleUpdated();
                }
            }
            catch (Exception exc)
            {
                Crashes.TrackError(exc, new Dictionary<string, string>
                {
                    { "connectivity", Connectivity.NetworkAccess.ToString() }
                });
            }
        }

        private async Task OnScheduleUpdated()
        {
            var fileAccess = DependencyService.Get<IFileAccess>();
            var resolver = TinyIoCContainer.Current;
            var dataProvider = resolver.Resolve<IDataProvider>();

            var filename = GetDatabaseFilename();
            var databasePath = fileAccess.GetLocalFilePath(filename);
            dataProvider.SetDatabasePath(databasePath);
            Analytics.TrackEvent("ScheduleUpdated");

            await Xamarin.Forms.Device.InvokeOnMainThreadAsync(async () =>
            {
                await MainPage.Navigation.PopToRootAsync();
                MessagingCenter.Send(new ScheduleDataUpdatedMessage(), ScheduleDataUpdatedMessage.Name);
                UserDialogs.Instance.Toast("Rozkład jazdy został zaktualizowany");
            });
        }
    }
}

