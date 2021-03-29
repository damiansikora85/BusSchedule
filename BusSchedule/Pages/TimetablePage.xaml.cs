using Acr.UserDialogs;
using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using BusSchedule.Interfaces;
using BusSchedule.Interfaces.Implementation;
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
    public partial class TimetablePage : ContentPage
    {
        private TimetableViewModel _viewModel;

        public TimetablePage(Stops station, Routes route)
        {
            _viewModel = new TimetableViewModel(station, route, TinyIoCContainer.Current.Resolve<IDataProvider>(), new FavoritesManager(TinyIoCContainer.Current.Resolve<IPreferences>()));
            InitializeComponent();
            BindingContext = _viewModel;
            Title = station.Stop_Name;
        }

        public TimetablePage(Stops station, Routes route, int direction)
        {
            _viewModel = new TimetableViewModel(station, route, direction, TinyIoCContainer.Current.Resolve<IDataProvider>(), new FavoritesManager(TinyIoCContainer.Current.Resolve<IPreferences>()));
            InitializeComponent();
            BindingContext = _viewModel;
            Title = station.Stop_Name;
        }

        protected override async void OnAppearing()
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
            UserDialogs.Instance.HideLoading();
            base.OnAppearing();
        }

        private void WorkingDaysClicked(object sender, System.EventArgs e)
        {
            _viewModel.ScheduleDaysChanged(Calendar.Service.WorkingDays);
        }

        private void SaturdaysClicked(object sender, System.EventArgs e)
        {
            _viewModel.ScheduleDaysChanged(Calendar.Service.Saturdays);
        }

        private void HolidaysClicked(object sender, System.EventArgs e)
        {
            _viewModel.ScheduleDaysChanged(Calendar.Service.SundayAndHolidays);
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //listView.SelectedItem = null;
        }

        private void AddToFavoritesClicked(object sender, EventArgs e)
        {
            _viewModel.AddThisToFavorites();
        }
    }
}