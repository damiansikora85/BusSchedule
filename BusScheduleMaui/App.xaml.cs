using Acr.UserDialogs;
using BusSchedule.Core.CloudService;
using BusSchedule.Core.CloudService.Impl;
using BusSchedule.Core.Messages;
using BusSchedule.Core.Services;
using BusSchedule.Core.Utils;
using BusSchedule.Interfaces.Implementation;
using BusSchedule.Providers;
using CommunityToolkit.Mvvm.Messaging;
using TinyIoC;
using Xamarin.Plugin.Firebase; 
using IPreferences = BusSchedule.Core.Services.IPreferences;

namespace BusSchedule;

public partial class App : Application
{
    public static string DB_FILENAME = "sqlite20260825.db";
    private SemaphoreSlim _updateSemafor = new SemaphoreSlim(1);
    private CancellationTokenSource _updateCancellationTokenSource = new CancellationTokenSource();
    public App()
    {
        InitializeComponent();
        VersionTracking.Track();
        RegisterIoC();
        Application.Current.UserAppTheme = AppTheme.Light;
        TaskScheduler.UnobservedTaskException += UnobservedTaskExceptionHandler;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
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
            catch (Exception exc)
            {
                var analyticsService = TinyIoCContainer.Current.Resolve<IAnalyticsService>();
                analyticsService.LogEvent("Exception in OnStart");
                analyticsService.LogException(exc);
            }
        });
    }

    protected override async void OnResume()
    {
        System.Diagnostics.Debug.WriteLine("App.OnResume called");
        _updateCancellationTokenSource = new CancellationTokenSource(); // Reset token on resume
        Task.Run(async () =>
        {
            try
            {
                var resolver = TinyIoCContainer.Current;
                await TryUpdateSchedule(resolver.Resolve<IScheduleUpdater>());
                await TryUpdateNews(resolver.Resolve<INewsService>(), resolver.Resolve<IPreferences>());
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in OnResume task: {exc.ToString()}");
                var analyticsService = TinyIoCContainer.Current.Resolve<IAnalyticsService>();
                analyticsService.LogEvent("Exception in OnResume");
                analyticsService.LogException(exc);
            }
        });
    }

    protected override void OnSleep()
    {
        _updateCancellationTokenSource.Cancel();
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
        var analyticsService = TinyIoCContainer.Current.Resolve<IAnalyticsService>();
        try
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet && await scheduleUpdater.TryUpdateSchedule(TinyIoCContainer.Current.Resolve<IFileAccess>(), DB_FILENAME))
            {
                analyticsService.LogEvent("Schedule updated successfully");
                await OnScheduleUpdated();
            }
        }
        catch (Exception exc)
        {
            analyticsService.LogEvent("Exception in TryUpdateSchedule");
            analyticsService.LogException(exc);
        }
    }

    private async Task OnScheduleUpdated()
    {
        if (_updateCancellationTokenSource.IsCancellationRequested)
        {
            TinyIoCContainer.Current.Resolve<IAnalyticsService>().LogEvent("OnScheduleUpdated cancelled due to app sleep");
            return;
        }
        var resolver = TinyIoCContainer.Current;
        var dataProvider = resolver.Resolve<IDataProvider>();
        var fileAccess = resolver.Resolve<IFileAccess>();

        var filename = GetDatabaseFilename();
        var databasePath = fileAccess.GetLocalFilePath(filename);
        dataProvider.SetDatabasePath(databasePath);

        try
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Windows[0].Page.Navigation.PopToRootAsync();
                WeakReferenceMessenger.Default.Send(new ScheduleDataUpdatedMessage(), ScheduleDataUpdatedMessage.Name);
#if ANDROID
                UserDialogs.Instance.Toast("Rozkład jazdy został zaktualizowany");
#endif
            });
        }
        catch (Exception ex)
        {
            TinyIoCContainer.Current.Resolve<IAnalyticsService>().LogException(ex);
            throw;
        }
    }
}
