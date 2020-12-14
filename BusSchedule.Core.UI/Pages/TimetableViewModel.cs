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
        public Routes Route { get; private set; }
        public Stops Station { get; private set; }
        private int? _direction;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<TimetableItem> TimetableWorkingDays { get; private set; } = new List<TimetableItem>();
        public List<TimetableItem> TimetableSaturdays { get; private set; } = new List<TimetableItem>();
        public List<TimetableItem> TimetableHolidays { get; private set; } = new List<TimetableItem>();
        public List<TimetableItem> CurrentTimetable { get; private set; }
        public List<Trip_Description> TimetableLegend { get; private set; }
        public ObservableCollection<string> RouteDetails { get; private set; } = new ObservableCollection<string>();
        private Calendar.Service _currentCalendarService;
        public bool WorkingDaysVisible => _currentCalendarService == Calendar.Service.WorkingDays;
        public bool SaturdaysVisible => _currentCalendarService == Calendar.Service.Saturdays;
        public bool HolidaysVisible => _currentCalendarService == Calendar.Service.SundayAndHolidays;

        public ICommand ScheduleDaysChangedCommand { get; private set; }

        public TimetableViewModel(Stops station, Routes route, IDataProvider dataProvider)
        {
            Station = station;
            Route = route;
            _dataProvider = dataProvider;
        }

        public TimetableViewModel(Stops station, Routes route, int direction, IDataProvider dataProvider)
        {
            Station = station;
            Route = route;
            _direction = direction;
            _dataProvider = dataProvider;
        }

        public async Task RefreshTimetableAsync()
        {
            TimetableWorkingDays.Clear();
            TimetableSaturdays.Clear();
            TimetableHolidays.Clear();

            TimetableLegend = (await _dataProvider.GetRouteLegend(Route.Route_Id, _direction)).ToList();

            var timetableAll = _direction.HasValue ? await GtfsUtils.GetSchedule(_dataProvider, Route, Station, _direction.Value)
                : await GtfsUtils.GetSchedule(_dataProvider, Route, Station);
            TimetableWorkingDays = Setup(timetableAll["24"]);
            TimetableSaturdays = Setup(timetableAll["7"]);
            TimetableHolidays = Setup(timetableAll["4"]);

            _currentCalendarService = Calendar.Service.WorkingDays;
            CurrentTimetable = TimetableWorkingDays;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTimetable)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WorkingDaysVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaturdaysVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HolidaysVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimetableLegend)));
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

        private List<TimetableItem> Setup(List<TimetableTuple> timetable)
        {
            var sorted = timetable.OrderBy(t => t.Time);
            var grouped = sorted.GroupBy(item => item.Time.Hours);
            var list = new List<TimetableItem>();
            foreach(var group in grouped)
            {
                var timetableItem = new TimetableItem
                {
                    Hour = group.Key,
                    Minutes = group.Select(it => new TimetableItem.TimetableItemMinutes { Minutes = it.Time.Minutes, AdditionalInfo = it.AdditionalDescription?.ShortDescription }).ToList()
                };
                list.Add(timetableItem);
            }
            return list;
        }
    }
}
