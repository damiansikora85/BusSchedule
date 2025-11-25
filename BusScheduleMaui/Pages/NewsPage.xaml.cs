using BusSchedule.Core.Services;
using BusSchedule.Core.UI.Pages;
using TinyIoC;

namespace BusSchedule.Pages;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class NewsPage : ContentPage
{
    private NewsPageViewModel _viewModel;
    public NewsPage(INewsService newsService) 
    {
        _viewModel = new NewsPageViewModel(newsService);
        BindingContext  = _viewModel;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        try
        {
            Shell.SetTabBarIsVisible(this, false);
            await _viewModel.RefreshView();
        }
        catch(Exception exc)
        {
            TinyIoCContainer.Current.Resolve<IAnalyticsService>().LogException(exc);
        }
    }
}