﻿using Acr.UserDialogs;
using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using BusSchedule.Interfaces;
using BusSchedule.Interfaces.Implementation;
using BusSchedule.UI.ViewModels;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyIoC;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BusSchedule.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimetablePage : ContentPage
    {
        private TimetableViewModel _viewModel;

        public TimetablePage(Stops station, Routes route, int? direction)
        {
            _viewModel = new TimetableViewModel(station, route, direction, TinyIoCContainer.Current.Resolve<IDataProvider>(), new FavoritesManager());
            InitializeComponent();
            BindingContext = _viewModel;
            Title = station.Stop_Name;
            SetupToolbar();
        }

        private void SetupToolbar()
        {
            if(!_viewModel.IsOnFavoritesList())
            {
                var toolbarItem = new ToolbarItem
                {
                    IconImageSource = "baseline_favorite_white_24",
                };
                toolbarItem.Clicked += AddToFavoritesClicked;
                ToolbarItems.Add(toolbarItem);
            }
        }

        protected override async void OnAppearing()
        {
            UserDialogs.Instance.ShowLoading("");
            try
            {
                await _viewModel.RefreshTimetableAsync();
                ScrollToCurrentTime(true);
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

        private void ScrollToCurrentTime(bool animated)
        {
            if (_viewModel.NextBus != null)
            {
                listView.ScrollTo(_viewModel.NextBus, ScrollToPosition.Center, animated);
            }
        }

        private void TodayClicked(object sender, System.EventArgs e)
        {
            _viewModel.ScheduleDaysChanged(Calendar.Service.Today);
            ScrollToCurrentTime(true);
        }

        private void WorkingDaysClicked(object sender, System.EventArgs e)
        {
            _viewModel.ScheduleDaysChanged(Calendar.Service.WorkingDays);
            ScrollToCurrentTime(true);
        }

        private void SaturdaysClicked(object sender, System.EventArgs e)
        {
            _viewModel.ScheduleDaysChanged(Calendar.Service.Saturdays);
            ScrollToCurrentTime(true);
        }

        private void HolidaysClicked(object sender, System.EventArgs e)
        {
            _viewModel.ScheduleDaysChanged(Calendar.Service.SundayAndHolidays);
            ScrollToCurrentTime(true);
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //listView.SelectedItem = null;
        }

        private void AddToFavoritesClicked(object sender, EventArgs e)
        {
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("FavoriteAdd");
            _viewModel.AddThisToFavorites();
            ToolbarItems.Clear();
            UserDialogs.Instance.Toast("Dodano do Ulubionych");
        }
    }
}