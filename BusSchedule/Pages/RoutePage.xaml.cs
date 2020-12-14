using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using BusSchedule.UI.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
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
            try
            {
                await _viewModel.RefreshDataAsync();
            }
            catch(Exception exc)
            {
                Crashes.TrackError(exc, new Dictionary<string, string>
                {
                    {"route", _viewModel.Route.Route_Short_Name },
                    {"direction", _viewModel.Direction.ToString()}
                });
            }
            base.OnAppearing();
        }

        private async void OnStationSelected(object sender, SelectedItemChangedEventArgs e)
        {
            listView.SelectedItem = null;

            if (e.SelectedItem is Stops station)
            {
                try
                {
                    var page = _fakeDirection ? new TimetablePage(station, _viewModel.Route) : new TimetablePage(station, _viewModel.Route, _viewModel.Direction);
                    await Navigation.PushAsync(page);
                }
                catch(Exception exc)
                {
                    Crashes.TrackError(exc, new Dictionary<string, string>
                    {
                        {"route", _viewModel.Route.Route_Short_Name },
                        {"direction", _viewModel.Direction.ToString() },
                        {"station", station.Stop_Name }
                    });
                }
            }
        }
    }
}