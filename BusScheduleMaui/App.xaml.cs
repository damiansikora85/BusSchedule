using Acr.UserDialogs;
using BusSchedule.Core.CloudService;
using BusSchedule.Core.CloudService.Impl;
using BusSchedule.Core.Messages;
using BusSchedule.Core.Services;
using BusSchedule.Core.Utils;
using BusSchedule.Interfaces.Implementation;
using BusSchedule.Providers;
using TinyIoC;
using Xamarin.Plugin.Firebase;
using IPreferences = BusSchedule.Core.Services.IPreferences;

namespace BusSchedule;

public partial class App : Application
{
    public static string DB_FILENAME = "sqlite20250531.db";
    private SemaphoreSlim _updateSemafor = new SemaphoreSlim(1);
    public App()
    {
        InitializeComponent();
        VersionTracking.Track();
        RegisterIoC();
        Application.Current.UserAppTheme = AppTheme.Light;
        TaskScheduler.UnobservedTaskException += UnobservedTaskExceptionHandler;

        try
        {
            MainPage = new AppShell();//new NavigationPage(new RoutesPage()) { BarBackgroundColor = Color.FromHex("#237194") };
        }
        catch (Exception exc)
        {
            var msg = exc.Message;
        }
    }

    private void RegisterIoC()
    {
        var container = TinyIoCContainer.Current;
        container.Register<IPreferences, CustomPreferences>();

        var fileAccess = new FileAccessService();//Handler.MauiContext.Services.GetService<IFileAccess>();
        var databasePath = Path.Combine(FileSystem.AppDataDirectory, GetDatabaseFilename());//fileAccess.GetLocalFilePath(GetDatabaseFilename());
        var dataProvider = new SQLDataProvider(databasePath);
        container.Register<IDataProvider, SQLDataProvider>(dataProvider);
        container.Register<IFileAccess, FileAccessService>();
        container.Register<ICloudService, FirebaseCloudService>();
        container.Register<INewsService, NewsService>(new NewsService(new FirebaseCloudService(), dataProvider));
        container.Register<IFirebaseStorage, Storage>().AsSingleton();
        container.Register<IScheduleUpdater, ScheduleUpdater>();
        container.Register<IAnalyticsService, FirebaseAnalyticsService>();
        container.Register((c, p) => Preferences.Default);
    }

    private string GetDatabaseFilename()
    {
        var container = TinyIoCContainer.Current;
        var preferences = container.Resolve<IPreferences>();
        return preferences.Get("dbFilename", DB_FILENAME);
    }

    private void UnobservedTaskExceptionHandler(object sender, UnobservedTaskExceptionEventArgs e)
    {
        TinyIoCContainer.Current.Resolve<IAnalyticsService>().LogException(e.Exception);
    }

    protected override async void OnStart()
    {
        Task.Run(async () =>
        {
            try
            {
                var resolver = TinyIoCContainer.Current;
                var scheduleUpdater = resolver.Resolve<IScheduleUpdater>();
                await TryUpdateSchedule(scheduleUpdater);
                await TryUpdateNews(resolver.Resolve<INewsService>(), resolver.Resolve<IPreferences>());
            }
            catch (Exception ex)
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
            TinyIoCContainer.Current.Resolve<IAnalyticsService>().LogException(exc);
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
            if (current == NetworkAccess.Internet && await scheduleUpdater.TryUpdateSchedule(TinyIoCContainer.Current.Resolve<IFileAccess>(), DB_FILENAME))
            {
                await OnScheduleUpdated();
            }
        }
        catch (Exception exc)
        {
            TinyIoCContainer.Current.Resolve<IAnalyticsService>().LogException(exc);
        }
    }

    private async Task OnScheduleUpdated()
    {

        var resolver = TinyIoCContainer.Current;
        var dataProvider = resolver.Resolve<IDataProvider>();
        var fileAccess = resolver.Resolve<IFileAccess>();

        var filename = GetDatabaseFilename();
        var databasePath = fileAccess.GetLocalFilePath(filename);
        dataProvider.SetDatabasePath(databasePath);

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await MainPage.Navigation.PopToRootAsync();
            MessagingCenter.Send(new ScheduleDataUpdatedMessage(), ScheduleDataUpdatedMessage.Name);
#if ANDROID
            UserDialogs.Instance.Toast("Rozkład jazdy został zaktualizowany");
#endif
        });
    }
}
