﻿using BusSchedule.Core.Model;
using BusSchedule.Core.Services;
using BusSchedule.Core.Utils;
using BusSchedule.UI.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace BusSchedule.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutePage : ContentPage
    {
        private RoutePageViewModel _viewModel;
        private bool _firstTimeAppearing = true;

        public RoutePage(Routes route, string destinationName, int? direction)
        {
            _viewModel = new RoutePageViewModel(route, direction, TinyIoCContainer.Current.Resolve<IDataProvider>());
            InitializeComponent();
            BindingContext = _viewModel;
            Title = $"Linia: {route.Route_Short_Name} - {destinationName}";
        }

        protected override async void OnAppearing()
        {
            try
            {
                if (_firstTimeAppearing)
                {
                    _firstTimeAppearing = false;
                    await CheckPermission();
                }

                await _viewModel.RefreshDataAsync();
                CreateRoutePath();

                await SetMapPosition(await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() == PermissionStatus.Granted);
            }
            catch (Exception exc)
            {
                Crashes.TrackError(exc, new Dictionary<string, string>
                {
                    {"route", _viewModel.Route.Route_Short_Name },
                    {"direction", _viewModel.Direction.ToString()}
                });
            }

            base.OnAppearing();
        }

        private async Task CheckPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                var preferences = TinyIoCContainer.Current.Resolve<IPreferences>();
                if (preferences.Get("wasAskingLocationPermission", false))   //was asking permission but not granted
                {
                    var shouldShowRationale = Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>();
                    if (shouldShowRationale)
                    {
                        await RequestLocationPermissionWithExplanation();
                    }
                }
                else //first time asking permission
                {
                    await RequestLocationPermissionWithExplanation();
                }
                preferences.Set("wasAskingLocationPermission", true);
            }

            async Task RequestLocationPermissionWithExplanation()
            {
                await DisplayAlert("Nowa funkcja - mapa", "Aby zobaczyć swoją lokalizację na mapie, aplikacja potrzebuje Twojej zgody.", "Rozumiem");
                _ = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }
        }

        

        private async Task SetMapPosition(bool locationPermissionGranted)
        {
            if(locationPermissionGranted)
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                var positionOnMap = new Position(location.Latitude, location.Longitude);
                map.MoveToRegion(MapSpan.FromCenterAndRadius(positionOnMap, Distance.FromMeters(500)));
            }
            else
            {
                var centerPoint = _viewModel.CalculateCenterPosition();
                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(centerPoint.Latitude, centerPoint.Longitude), Distance.FromMeters(2000)));
            }
        }

        private void CreateRoutePath()
        {
           
            foreach (var trace in _viewModel.Traces)
            {
                Polyline polyline = new()
                {
                    StrokeColor = Color.FromHex(_viewModel.Route.Route_Color),
                    StrokeWidth = 12
                };
                foreach (var point in trace.Points)
                {
                    polyline.Geopath.Add(new Position(point.Latitude, point.Longitude));
                }
                map.MapElements.Add(polyline);
            }
        }

        private async void OnStationSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //listView.SelectedItem = null;

            if (e.SelectedItem is Stops station)
            {
                await ShowScheduleForStop(station);
            }
        }

        private async void Pin_MarkerClicked(object sender, PinClickedEventArgs e)
        {
            if(sender is Pin pin && pin.BindingContext is Stops stop)
            {
                await ShowScheduleForStop(stop);
            }
        }

        private async Task ShowScheduleForStop(Stops station)
        {
            try
            {
                var page = new TimetablePage(station, _viewModel.Route, _viewModel.Direction.Value);
                await Navigation.PushAsync(page);
            }
            catch (Exception exc)
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