﻿using BusSchedule.Core.Model;
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
        public RoutePage(BusService busService, BusRoute route)
        {
            _viewModel = new RoutePageViewModel(route, TinyIoCContainer.Current.Resolve<IDataProvider>());
            InitializeComponent();
            BindingContext = _viewModel;
            Title = $"Linia: {busService.Name} - {route.Name}";
        }

        protected override async void OnAppearing()
        {
            await _viewModel.RefreshDataAsync();
            base.OnAppearing();
        }

        private async void OnStationSelected(object sender, SelectedItemChangedEventArgs e)
        {
            listView.SelectedItem = null;
            if (e.SelectedItem is BusStation station)
            {
                await Navigation.PushAsync(new TimetablePage(station, _viewModel.Route));
            }
        }
    }
}