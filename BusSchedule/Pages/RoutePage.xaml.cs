using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using BusSchedule.UI.ViewModels;
using TinyIoC;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BusSchedule.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutePage : ContentPage
    {
        private RoutePageViewModel _viewModel;
        private bool _fakeDirection;

        public RoutePage(Routes route, string destinationName, int direction)
        {
            _viewModel = new RoutePageViewModel(route, direction, TinyIoCContainer.Current.Resolve<IDataProvider>());
            InitializeComponent();
            BindingContext = _viewModel;
            _fakeDirection = route.Route_Id == "2";
            Title = $"Linia: {route.Route_Short_Name} - {destinationName}";
        }

        protected override async void OnAppearing()
        {
            await _viewModel.RefreshDataAsync();
            base.OnAppearing();
        }

        private async void OnStationSelected(object sender, SelectedItemChangedEventArgs e)
        {
            listView.SelectedItem = null;
            if (e.SelectedItem is Stops station)
            {
                var page = _fakeDirection ? new TimetablePage(station, _viewModel.Route) : new TimetablePage(station, _viewModel.Route, _viewModel.Direction);
                await Navigation.PushAsync(page);
            }
        }
    }
}