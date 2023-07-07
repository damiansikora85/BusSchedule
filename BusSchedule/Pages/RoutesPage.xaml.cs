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
using System.Threading.Tasks;
using Polly;
using Xamarin.Essentials;
using Rg.Plugins.Popup.Extensions;
using BusSchedule.Popups;
using BusSchedule.Core.CloudService;
using BusSchedule.Core.Services;
using BusSchedule.Core.Messages;

namespace BusSchedule.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutesPage : ContentPage
    {
        private readonly RoutesPageViewModel _viewModel;
        private INewsService _newsService;
        private bool _selectionLock;

        public RoutesPage()
        {
            InitializeComponent();
            _viewModel = new RoutesPageViewModel(TinyIoCContainer.Current.Resolve<IDataProvider>());
            BindingContext = _viewModel;
            _newsService = TinyIoCContainer.Current.Resolve<INewsService>();
            MessagingCenter.Subscribe<ScheduleDataUpdatedMessage>(this, ScheduleDataUpdatedMessage.Name, OnScheduleUpdated);
        }

        private async void OnScheduleUpdated(ScheduleDataUpdatedMessage message)
        {
            await RefreshData();
        }

        protected override async void OnAppearing()
        {
            UserDialogs.Instance.ShowLoading("");
            var preferences = TinyIoCContainer.Current.Resolve<IPreferences>();
            await DataUpdater.UpdateDataIfNeeded(DependencyService.Get<IFileAccess>(), preferences);
            await RefreshData();
            UserDialogs.Instance.HideLoading();

            int row = 0, col = 0;
            int maxCol = grid.ColumnDefinitions.Count;
            grid.Children.Clear();
            foreach (var busService in _viewModel.Routes)
            {
                var item = new RouteView(busService);
                item.OnServiceClicked += OnBusServiceSelected;
                grid.Children.Add(item, col, row);
                col++;
                row = col == maxCol ? row + 1 : row;
                col %= maxCol;
            }

            if (DateTime.TryParse(preferences.Get("rate_popup_last_shown", DateTime.MinValue.ToString()), out var ratePopupLastShown))
            {
                if (!preferences.IsFirstLaunch && preferences.Get("rated", "0") != "1" && (DateTime.Today - ratePopupLastShown).TotalDays >= 5)
                {
                    await Navigation.PushPopupAsync(new RatePopup(preferences));
                }
            }
            else
            {
                preferences.Set("rate_popup_last_shown", DateTime.Today.ToString());
            }

            await FavoritesView.RefreshView();
            await CardListView.RefreshView();

            base.OnAppearing();
        }

        private Task RefreshData()
        {
            return Policy.Handle<Exception>().RetryAsync(async (exc, retryNum) =>
            {
                Crashes.TrackError(exc);
                //await DataUpdater.ForceCopy(DependencyService.Get<IFileAccess>());
            }).ExecuteAsync(async () => await _viewModel.RefreshBusServicesAsync());
        }

        private async void OnBusServiceSelected(Core.Model.Routes route)
        {
            if(_selectionLock)
            {
                return;
            }
            try
            {
                _selectionLock = true;
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
                    await Navigation.PushAsync(new RoutePage(route, string.IsNullOrEmpty(destination.Outbound) ? destination.Inbound : destination.Outbound, null));
                }
            }
            catch(TaskCanceledException)
            {
                //just catch this
            }
            catch(Exception exc)
            {
                Crashes.TrackError(exc, new Dictionary<string, string> { { "route", route.Route_Short_Name } });
            }
            finally
            {
                _selectionLock = false;
            }
        }

        private async void OnContactClicked(object sender, EventArgs e)
        {
            try
            {
                var message = new EmailMessage
                {
                    Subject = "Rozkład jazdy MZK Wejherowo",
                    To = new List<string> { "kontakt@darktowerlab.pl" },
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device
                Crashes.TrackError(fbsEx);
                await DisplayAlert("Uwaga", "Wystąpił problem - nie można wysłać wiadomości", "OK");
            }
            catch (Exception exc)
            {
                // Some other exception occurred
                Crashes.TrackError(exc);
                await DisplayAlert("Uwaga", "Wystąpił problem - nie można wysłać wiadomości", "OK");
            }
        }

        private async void OnNewsClicked(object sender, EventArgs e)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("NewsClicked");
            await Navigation.PushAsync(new NewsPage(_newsService));
        }

        private async void OnCardTabSelected(object sender, Xamarin.CommunityToolkit.UI.Views.TabTappedEventArgs e)
        {
            var preferences = TinyIoCContainer.Current.Resolve<IPreferences>();
            if(preferences.IsFirstLaunchVersion) 
            {
                await DisplayAlert("Stan karty", "Dodaj swoją kartę elektroniczną aby sprawdzić informacje oraz ważne bilety zakupione dla twojej karty.", "Rozumiem");
            }
        }
    }
}
