using Acr.UserDialogs;
using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using BusSchedule.UI.ViewModels;
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
            _viewModel = new TimetableViewModel(station, route, TinyIoCContainer.Current.Resolve<IDataProvider>());
            InitializeComponent();
            BindingContext = _viewModel;
            Title = station.Stop_Name;
        }

        public TimetablePage(Stops station, Routes route, int direction)
        {
            _viewModel = new TimetableViewModel(station, route, direction, TinyIoCContainer.Current.Resolve<IDataProvider>());
            InitializeComponent();
            BindingContext = _viewModel;
            Title = station.Stop_Name;
        }

        protected override async void OnAppearing()
        {
            UserDialogs.Instance.ShowLoading("");
            await _viewModel.RefreshTimetableAsync();
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
    }
}