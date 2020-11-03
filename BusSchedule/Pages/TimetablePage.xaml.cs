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
        public TimetablePage(Core.Model.BusStation station, Core.Model.BusRoute route)
        {
            _viewModel = new TimetableViewModel(station, route, TinyIoCContainer.Current.Resolve<IDataProvider>());
            InitializeComponent();
            BindingContext = _viewModel;
            Title = station.Name;
        }

        protected override async void OnAppearing()
        {
            await _viewModel.RefreshTimetableAsync();
            base.OnAppearing();
        }

        private void WorkingDaysClicked(object sender, System.EventArgs e)
        {
            _viewModel.ScheduleDaysChanged(RouteBeginTime.ScheduleDays.WorkingDays);
        }

        private void SaturdaysClicked(object sender, System.EventArgs e)
        {
            _viewModel.ScheduleDaysChanged(RouteBeginTime.ScheduleDays.Saturday);
        }

        private void HolidaysClicked(object sender, System.EventArgs e)
        {
            _viewModel.ScheduleDaysChanged(RouteBeginTime.ScheduleDays.SundayAndHolidays);
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            listView.SelectedItem = null;
        }
    }
}