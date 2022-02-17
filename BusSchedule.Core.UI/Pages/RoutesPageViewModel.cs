using BusSchedule.Core.Model;
using BusSchedule.Core.UI.Utils;
using BusSchedule.Core.Utils;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BusSchedule.Pages.ViewModels
{
    public class RoutesPageViewModel : INotifyPropertyChanged
    {
        private IDataProvider _dataProvider;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<Routes> Routes { get; private set; }
        public DateTime FeedStartDate { get; private set; }
        public DateTime FeedEndDate { get; private set; }

#if DEBUG
        public bool IsDebug => true;
#else
        public bool IsDebug => false;
#endif

        private Command testScheduleCommand;
        public ICommand TestScheduleCommand => testScheduleCommand ??= new Command(RunScheduleTest);

        public RoutesPageViewModel(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task RefreshBusServicesAsync()
        {
            var (startDate, endDate) = await _dataProvider.GetFeedStartEndDates();
            FeedStartDate = startDate;
            FeedEndDate = endDate;
            Routes = await _dataProvider.GetRoutes();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FeedStartDate)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FeedEndDate)));
        }

        public Task<Destination> GetDestinationsForRoute(Routes route)
        {
            return _dataProvider.GetRouteDestinations(route);
        }

        private void RunScheduleTest()
        {
            var scheduleTester = new ScheduleTester(_dataProvider);
            scheduleTester.CheckRouteStopsValid();
        }
    }
}
