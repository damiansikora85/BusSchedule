using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using BusSchedule.UI.ViewModels;
using Microsoft.AppCenter.Crashes;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using TinyIoC;

namespace BusSchedule.Pages;

public partial class RoutePage : ContentPage
{
    private RoutePageViewModel _viewModel;
    private bool _firstTimeAppearing = true;

    public RoutePage(Routes route, string destinationName, int? direction)
    {
        Shell.SetTabBarIsVisible(this, false);
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
            await DisplayAlert("Nowa funkcja - mapa", "Aby zobaczyæ swoj¹ lokalizacjê na mapie, aplikacja potrzebuje Twojej zgody.", "Rozumiem");
            _ = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }
    }
    private MapSpan mapPos;
    private async Task SetMapPosition(bool locationPermissionGranted)
    {
        var defaultPosition = new Location(54.605868, 18.235334);
        try
        {
            if (locationPermissionGranted)
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location != null)
                {
                    var positionOnMap = new Location(location.Latitude, location.Longitude);
                    mapPos = MapSpan.FromCenterAndRadius(positionOnMap, Distance.FromMeters(500));
                    map.MoveToRegion(mapPos);
                }
            }
            else
            {
                var centerPoint = _viewModel.CalculateCenterPosition();
                mapPos = MapSpan.FromCenterAndRadius(new Location(centerPoint.Latitude, centerPoint.Longitude), Distance.FromMeters(2000));
                map.MoveToRegion(mapPos);
            }
        }
        catch (Exception ex) when (ex is FeatureNotSupportedException || ex is FeatureNotEnabledException || ex is PermissionException)
        {
            map.MoveToRegion(MapSpan.FromCenterAndRadius(defaultPosition, Distance.FromMeters(2000)));
        }
        catch (Exception exc)
        {
            map.MoveToRegion(MapSpan.FromCenterAndRadius(defaultPosition, Distance.FromMeters(2000)));

            // Unable to get location
            Crashes.TrackError(exc, new Dictionary<string, string>
                {
                    {"route", _viewModel.Route.Route_Short_Name },
                    {"direction", _viewModel.Direction.ToString()}
                });
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
                polyline.Geopath.Add(new Location(point.Latitude, point.Longitude));
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
        if (sender is Pin pin && pin.BindingContext is Stops stop)
        {
            await ShowScheduleForStop(stop);
        }
    }

    private async Task ShowScheduleForStop(Stops station)
    {
        try
        {
            var page = new TimetablePage(station, _viewModel.Route, _viewModel.Direction);
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


    private void OnMapClicked(object sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (!map.IsVisible)
            {
                map.IsVisible = true;
                listView.IsVisible = false;
                map.MoveToRegion(mapPos);
            }
        });
    }
    private void OnListClicked(object sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (!listView.IsVisible)
            {
                map.IsVisible = false;
                listView.IsVisible = true;
            }
        });
    }
}