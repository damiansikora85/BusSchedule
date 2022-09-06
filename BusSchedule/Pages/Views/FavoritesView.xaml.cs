using BusSchedule.Core.UI.Components;
using BusSchedule.Core.UI.Pages.Views;
using BusSchedule.Core.Utils;
using BusSchedule.Interfaces.Implementation;
using TinyIoC;

namespace BusSchedule.Pages.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FavoritesView : ContentView
    {
        private FavoritesViewModel _viewModel;

        public FavoritesView()
        {
            InitializeComponent();
            _viewModel = new FavoritesViewModel(new FavoritesManager(), TinyIoCContainer.Current.Resolve<IDataProvider>());
            ListView.ItemsSource = _viewModel.Favorites;
        }

        public async Task RefreshView()
        {
            await _viewModel.RefreshData();
            ListView.IsVisible = _viewModel.HasAnyFavorites;
            EmptyListLabel.IsVisible = _viewModel.HasNoFavorites;
        }

        private async void FavoriteItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (ListView.SelectedItem is FavoriteData favoriteData)
            {
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("FavoriteClicked");
                var page = new TodayTimetablePage(favoriteData.Stop, favoriteData.Route, favoriteData.Direction);
                await Navigation.PushAsync(page);
                ListView.SelectedItem = null;
            }
        }

        private async void OnDeleteClicked(object sender, System.EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is FavoriteData favoriteData)
            {
                if (await App.Current.MainPage.DisplayAlert("Uwaga", "Czy na pewno chcesz usunąć?", "Tak", "Nie"))
                {
                    _viewModel.DeleteItem(favoriteData);
                    ListView.IsVisible = _viewModel.HasAnyFavorites;
                    EmptyListLabel.IsVisible = _viewModel.HasNoFavorites;
                }
            }
        }
    }
}