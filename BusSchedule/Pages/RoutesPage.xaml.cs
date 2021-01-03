using Acr.UserDialogs;
using BusSchedule.Components;
using BusSchedule.Core.Utils;
using BusSchedule.Dialogs;
using BusSchedule.Interfaces;
using BusSchedule.Pages.ViewModels;
using BusSchedule.Tools;
using Microsoft.AppCenter.Crashes;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using TinyIoC;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using System.Threading.Tasks;
using Polly;
using Xamarin.Essentials;

namespace BusSchedule.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutesPage : ContentPage
    {
        private RoutesPageViewModel _viewModel;
        public RoutesPage()
        {
            InitializeComponent();
            _viewModel = new RoutesPageViewModel(TinyIoCContainer.Current.Resolve<IDataProvider>());
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            UserDialogs.Instance.ShowLoading("");
            await DataUpdater.UpdateDataIfNeeded(DependencyService.Get<IFileAccess>(), TinyIoCContainer.Current.Resolve<IPreferences>());
            await RefreshData();
            UserDialogs.Instance.HideLoading();

            int row = 0, col = 0;
            int maxCol = grid.ColumnDefinitions.Count;
            foreach (var busService in _viewModel.Routes)
            {
                var item = new RouteView(busService);
                item.OnServiceClicked += OnBusServiceSelected;
                grid.Children.Add(item, col, row);
                col++;
                row = col == maxCol ? row + 1 : row;
                col %= maxCol;
            }
            base.OnAppearing();
        }

        private Task RefreshData()
        {
            return Policy.Handle<Exception>().RetryAsync(async (exc, retryNum) =>
            {
                Crashes.TrackError(exc);
                await DataUpdater.ForceCopy(DependencyService.Get<IFileAccess>());
            }).ExecuteAsync(async () => await _viewModel.RefreshBusServicesAsync());
        }

        private async void OnBusServiceSelected(Core.Model.Routes route)
        {
            try
            {
                var destination = await _viewModel.GetDestinationsForRoute(route);
                if (!string.IsNullOrEmpty(destination.Outbound) && !string.IsNullOrEmpty(destination.Inbound))
                {
                    var dialog = new RouteSelectionDialog(destination);
                    await PopupNavigation.Instance.PushAsync(dialog);
                    var selectedDirection = await dialog.WaitForResult();
                    await Navigation.PushAsync(new RoutePage(route, selectedDirection == 0 ? destination.Outbound : destination.Inbound, selectedDirection));
                }
                else if (!string.IsNullOrEmpty(destination.Outbound) || !string.IsNullOrEmpty(destination.Inbound))
                {
                    await Navigation.PushAsync(new RoutePage(route, string.IsNullOrEmpty(destination.Outbound) ? destination.Inbound : destination.Outbound, string.IsNullOrEmpty(destination.Outbound) ? 1 : 0));
                }
            }
            catch(Exception exc)
            {
                Crashes.TrackError(exc, new Dictionary<string, string> { { "route", route.Route_Short_Name } });
            }
        }

        private async void OnContactClicked(object sender, EventArgs e)
        {
            try
            {
                var message = new EmailMessage
                {
                    Subject = "Rozkład jazdy MZK Wejherowo",
                    To = new List<string> { "darktowerlab@gmail.com" },
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device
                Crashes.TrackError(fbsEx);
            }
            catch (Exception exc)
            {
                // Some other exception occurred
                Crashes.TrackError(exc);
            }
        }
    }
}