using Acr.UserDialogs;
using BusSchedule.Core.Model;
using BusSchedule.Core.UI.Pages;
using BusSchedule.Core.UI.Utils;
using BusSchedule.Core.Utils;
using BusSchedule.Interfaces.Implementation;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyIoC;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BusSchedule.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TodayTimetablePage : ContentPage
    {
        private TimetableViewModel _viewModel;

        public TodayTimetablePage(Stops station, Routes route, int? direction)
        {
            InitializeComponent();
            _viewModel = new TimetableViewModel(route, station, direction, TinyIoCContainer.Current.Resolve<IDataProvider>(), new FavoritesManager());
            BindingContext = _viewModel;
            Title = station.Stop_Name;
            SetupToolbar();
        }

        protected async override void OnAppearing()
        {
            UserDialogs.Instance.ShowLoading("");
            try
            {
                await _viewModel.RefreshTimetableAsync();
            }
            catch (Exception exc)
            {
                Crashes.TrackError(exc, new Dictionary<string, string>
                {
                    {"route", _viewModel.Route.Route_Short_Name },
                    {"station", _viewModel.Station.Stop_Name }
                });
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
                base.OnAppearing();
            }
        }

        private void SetupToolbar()
        {
            if (!_viewModel.IsOnFavoritesList())
            {
                var toolbarItem = new ToolbarItem
                {
                    IconImageSource = "baseline_favorite_white_24",
                };
                toolbarItem.Clicked += AddToFavoritesClicked;
                ToolbarItems.Add(toolbarItem);
            }
        }

        private void AddToFavoritesClicked(object sender, EventArgs e)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("FavoriteAdd");
            _viewModel.AddThisToFavorites();
            ToolbarItems.Clear();
            UserDialogs.Instance.Toast("Dodano do Ulubionych");
        }

        private async void SelectedDayChanged(object sender, SelectionChangedEventArgs e)
        {
            UserDialogs.Instance.ShowLoading();
            if(e.PreviousSelection.Any() && e.PreviousSelection.First() is TimetableDate previous)
            {
                previous.Deselect();
            }
            if(e.CurrentSelection.Any() && e.CurrentSelection.First() is TimetableDate current)
            {
                current.Select();
            }
            await _viewModel.OnNewDaySelected();
            UserDialogs.Instance.HideLoading();
        }
    }
}