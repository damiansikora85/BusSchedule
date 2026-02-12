using BusSchedule.Core.UI.Components;
using BusSchedule.Core.UI.Pages.Views;
using BusSchedule.Core.Utils;
using BusSchedule.Interfaces.Implementation;
using TinyIoC;

namespace BusSchedule.Pages;

public partial class FavoritesPage : ContentPage
{
    private FavoritesViewModel _viewModel;

    public FavoritesPage()
	{
		InitializeComponent();
        _viewModel = new FavoritesViewModel(new FavoritesManager(), TinyIoCContainer.Current.Resolve<IDataProvider>());
        BindingContext = _viewModel;
        //ListView.ItemsSource = _viewModel.Favorites;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RefreshView();
    }

    public async Task RefreshView()
    {
        await _viewModel.RefreshData();
    }

    private async void FavoriteItemSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.First() is FavoriteData favoriteData)
        {
            var page = new TimetablePage(favoriteData.Stop, favoriteData.Route, favoriteData.Direction);
            await Navigation.PushAsync(page);
        }
    }

    private async void OnDeleteClicked(object sender, System.EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is FavoriteData favoriteData)
        {
            if (await App.Current.Windows[0].Page.DisplayAlertAsync("Uwaga", "Czy na pewno chcesz usun¹æ?", "Tak", "Nie"))
            {
                _viewModel.DeleteItem(favoriteData);
            }
        }
    }
}