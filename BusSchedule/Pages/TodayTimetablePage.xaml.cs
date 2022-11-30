using Acr.UserDialogs;
using BusSchedule.Core.Model;
using BusSchedule.Core.UI.Pages;
using BusSchedule.Core.UI.Utils;
using BusSchedule.Core.Utils;
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
        private TodayTimetableViewModel _viewModel;

        public TodayTimetablePage(Stops station, Routes route, int? direction)
        {
            InitializeComponent();
            _viewModel = new TodayTimetableViewModel(route, station, direction, TinyIoCContainer.Current.Resolve<IDataProvider>());
            BindingContext = _viewModel;
            Title = station.Stop_Name;
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

        private async void SelectedDayChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.PreviousSelection.Any() && e.PreviousSelection.First() is TimetableDate previous)
            {
                previous.Deselect();
            }
            if(e.CurrentSelection.Any() && e.CurrentSelection.First() is TimetableDate current)
            {
                current.Select();
            }
            await _viewModel.OnNewDaySelected();
        }
    }
}