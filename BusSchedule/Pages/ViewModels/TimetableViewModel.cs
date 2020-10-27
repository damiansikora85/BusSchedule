using BusSchedule.Core.Model;
using BusSchedule.Core.UseCase;
using BusSchedule.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Pages.ViewModels
{
    public class TimetableViewModel : INotifyPropertyChanged
    {
        private IDataProvider _dataProvider;
        private BusRoute _route;
        private BusStation _station;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<TimeSpan> TimetableWorkingDays { get; private set; } = new List<TimeSpan>();
        public List<TimeSpan> TimetableSaturdays { get; private set; } = new List<TimeSpan>();
        public List<TimeSpan> TimetableHolidays { get; private set; } = new List<TimeSpan>();
        public ObservableCollection<string> RouteDetails { get; private set; } = new ObservableCollection<string>();

        public TimetableViewModel(BusStation station, BusRoute route, IDataProvider dataProvider)
        {
            _station = station;
            _route = route;
            _dataProvider = dataProvider;
        }

        internal async Task RefreshTimetableAsync()
        {
            TimetableWorkingDays.Clear();
            TimetableSaturdays.Clear();
            TimetableHolidays.Clear();

            var timetableAll = await TimetableGenerator.Generate(_station, _route, _dataProvider);
            TimetableWorkingDays = timetableAll[RouteBeginTime.ScheduleDays.WorkingDays];
            TimetableSaturdays = timetableAll[RouteBeginTime.ScheduleDays.Saturday];
            TimetableHolidays = timetableAll[RouteBeginTime.ScheduleDays.SundayAndHolidays];

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimetableWorkingDays)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimetableSaturdays)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimetableHolidays)));
        }
    }
}
