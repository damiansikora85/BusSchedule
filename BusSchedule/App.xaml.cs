using BusSchedule.Core.CloudService;
using BusSchedule.Core.CloudService.Impl;
using BusSchedule.Core.Services;
using BusSchedule.Core.Utils;
using BusSchedule.Interfaces;
using BusSchedule.Interfaces.Implementation;
using BusSchedule.Pages;
using BusSchedule.Providers;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Threading.Tasks;
using TinyIoC;
using Xamarin.Forms;

namespace BusSchedule
{
    public partial class App : Application
    {
        public static string DB_FILENAME = "sqlite20220108.db";
        public App()
        {
            InitializeComponent();
            Xamarin.Essentials.VersionTracking.Track();
            RegisterIoC();
            UserAppTheme = OSAppTheme.Light;
            MainPage = new NavigationPage(new RoutesPage()) { BarBackgroundColor = Color.FromHex("#237194") };
        }

        private void RegisterIoC()
        {
            var container = TinyIoCContainer.Current;
            var databasePath = DependencyService.Get<IFileAccess>().GetLocalFilePath(DB_FILENAME);
            var dataProvider = new SQLDataProvider(databasePath);
            container.Register<IDataProvider, SQLDataProvider>(dataProvider);
            container.Register<IPreferences, CustomPreferences>();
            container.Register<ICloudService, FirebaseCloudService>();
            container.Register<INewsService, NewsService>(new NewsService(new FirebaseCloudService(), dataProvider));
        }

        protected override void OnStart()
        {
            AppCenter.Start("android=fc2cc03c-f502-42d6-b5ed-8373e82d03c2;",
                  typeof(Analytics), typeof(Crashes));
            Task.Run(async () =>
            {
                await TryUpdateNews(TinyIoCContainer.Current.Resolve<INewsService>(), TinyIoCContainer.Current.Resolve<IPreferences>());
            });
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
            Task.Run(async () =>
            {
                await TryUpdateNews(TinyIoCContainer.Current.Resolve<INewsService>(), TinyIoCContainer.Current.Resolve<IPreferences>());
            });
        }

        private async Task TryUpdateNews(INewsService newsService, IPreferences preferences)
        {
            try
            {
                if (await newsService.TryUpdateNews(preferences.Get("lastNewsUpdate", DateTime.MinValue)))
                {
                    preferences.Set("lastNewsUpdate", DateTime.Now);
                }
            }
            catch(Exception exc)
            {
                Crashes.TrackError(exc);
            }
        }
    }
}
