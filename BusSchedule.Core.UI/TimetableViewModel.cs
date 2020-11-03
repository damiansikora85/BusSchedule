using BusSchedule.Core.Model;
using BusSchedule.Core.UI;
using BusSchedule.Core.UseCase;
using BusSchedule.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BusSchedule.UI.ViewModels
{
    public class TimetableViewModel : INotifyPropertyChanged
    {
        private IDataProvider _dataProvider;
        private BusRoute _route;
        private BusStation _station;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<TimetableItem> TimetableWorkingDays { get; private set; } = new List<TimetableItem>();
        public List<TimetableItem> TimetableSaturdays { get; private set; } = new List<TimetableItem>();
        public List<TimetableItem> TimetableHolidays { get; private set; } = new List<TimetableItem>();

        public List<TimetableItem> CurrentTimetable { get; private set; }
        public ObservableCollection<string> RouteDetails { get; private set; } = new ObservableCollection<string>();
        private RouteBeginTime.ScheduleDays _currentScheduleDays;
        public bool WorkingDaysVisible => _currentScheduleDays == RouteBeginTime.ScheduleDays.WorkingDays;
        public bool SaturdaysVisible => _currentScheduleDays == RouteBeginTime.ScheduleDays.Saturday;
        public bool HolidaysVisible => _currentScheduleDays == RouteBeginTime.ScheduleDays.SundayAndHolidays;

        public void ScheduleDaysChanged(RouteBeginTime.ScheduleDays scheduleDays)
        {
            switch (scheduleDays)
            {
                case RouteBeginTime.ScheduleDays.WorkingDays:
                    CurrentTimetable = TimetableWorkingDays;
                    break;
                case RouteBeginTime.ScheduleDays.Saturday:
                    CurrentTimetable = TimetableSaturdays;
                    break;
                case RouteBeginTime.ScheduleDays.SundayAndHolidays:
                    CurrentTimetable = TimetableHolidays;
                    break;
            }

            _currentScheduleDays = scheduleDays;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTimetable)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WorkingDaysVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaturdaysVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HolidaysVisible)));
        }

        public ICommand ScheduleDaysChangedCommand { get; private set; }

        public TimetableViewModel(BusStation station, BusRoute route, IDataProvider dataProvider)
        {
            _station = station;
            _route = route;
            _dataProvider = dataProvider;
        }

        public async Task RefreshTimetableAsync()
        {
            TimetableWorkingDays.Clear();
            TimetableSaturdays.Clear();
            TimetableHolidays.Clear();

            var timetableAll = await TimetableGenerator.Generate(_station, _route, _dataProvider);
            TimetableWorkingDays = Setup(timetableAll[RouteBeginTime.ScheduleDays.WorkingDays]);
            TimetableSaturdays = Setup(timetableAll[RouteBeginTime.ScheduleDays.Saturday]);
            TimetableHolidays = Setup(timetableAll[RouteBeginTime.ScheduleDays.SundayAndHolidays]);

            _currentScheduleDays = RouteBeginTime.ScheduleDays.WorkingDays;
            CurrentTimetable = TimetableWorkingDays;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTimetable)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WorkingDaysVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaturdaysVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HolidaysVisible)));
        }

        private List<TimetableItem> Setup(List<TimeSpan> timetable)
        {
            var grouped = timetable.GroupBy(item => item.Hours);
            var list = new List<TimetableItem>();
            foreach(var group in grouped)
            {
                var timetableItem = new TimetableItem
                {
                    Hour = group.Key,
                    Minutes = group.Select(it => it.Minutes).ToList()
                };
                list.Add(timetableItem);
            }
            return list;
        }
    }
}
