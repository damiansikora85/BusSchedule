using BusSchedule.Core.GTFS;
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
        private Routes _route;
        private Stops _station;
        private int? _direction;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<TimetableItem> TimetableWorkingDays { get; private set; } = new List<TimetableItem>();
        public List<TimetableItem> TimetableSaturdays { get; private set; } = new List<TimetableItem>();
        public List<TimetableItem> TimetableHolidays { get; private set; } = new List<TimetableItem>();

        public List<TimetableItem> CurrentTimetable { get; private set; }
        public ObservableCollection<string> RouteDetails { get; private set; } = new ObservableCollection<string>();
        private Calendar.Service _currentCalendarService;
        public bool WorkingDaysVisible => _currentCalendarService == Calendar.Service.WorkingDays;
        public bool SaturdaysVisible => _currentCalendarService == Calendar.Service.Saturdays;
        public bool HolidaysVisible => _currentCalendarService == Calendar.Service.SundayAndHolidays;

        public ICommand ScheduleDaysChangedCommand { get; private set; }

        public TimetableViewModel(Stops station, Routes route, IDataProvider dataProvider)
        {
            _station = station;
            _route = route;
            _dataProvider = dataProvider;
        }

        public TimetableViewModel(Stops station, Routes route, int direction, IDataProvider dataProvider)
        {
            _station = station;
            _route = route;
            _direction = direction;
            _dataProvider = dataProvider;
        }

        public async Task RefreshTimetableAsync()
        {
            TimetableWorkingDays.Clear();
            TimetableSaturdays.Clear();
            TimetableHolidays.Clear();

            var timetableAll = _direction.HasValue ? await GtfsUtils.GetSchedule(_dataProvider, _route, _station, _direction.Value)
                : await GtfsUtils.GetSchedule(_dataProvider, _route, _station);
            TimetableWorkingDays = Setup(timetableAll["24"]);
            TimetableSaturdays = Setup(timetableAll["7"]);
            TimetableHolidays = Setup(timetableAll["4"]);

            _currentCalendarService = Calendar.Service.WorkingDays;
            CurrentTimetable = TimetableWorkingDays;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTimetable)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WorkingDaysVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaturdaysVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HolidaysVisible)));
        }

        public void ScheduleDaysChanged(Calendar.Service calendarService)
        {
            switch (calendarService)
            {
                case Calendar.Service.WorkingDays:
                    CurrentTimetable = TimetableWorkingDays;
                    break;
                case Calendar.Service.Saturdays:
                    CurrentTimetable = TimetableSaturdays;
                    break;
                case Calendar.Service.SundayAndHolidays:
                    CurrentTimetable = TimetableHolidays;
                    break;
            }

            _currentCalendarService = calendarService;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTimetable)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WorkingDaysVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaturdaysVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HolidaysVisible)));
        }

        private List<TimetableItem> Setup(List<TimeSpan> timetable)
        {
            timetable.Sort();
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
