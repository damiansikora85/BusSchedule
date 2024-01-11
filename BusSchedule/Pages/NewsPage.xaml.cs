using BusSchedule.Core.Services;
using BusSchedule.Core.UI.Pages;
using Microsoft.AppCenter.Crashes;

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
            Crashes.TrackError(exc);
        }
    }

    private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if(sender is ListView listView)
        {
            listView.SelectedItem = null;
        }
    }
}