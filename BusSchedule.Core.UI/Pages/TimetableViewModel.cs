using BusSchedule.Core.GTFS;
using BusSchedule.Core.Interfaces;
using BusSchedule.Core.Model;
using BusSchedule.Core.UI;
using BusSchedule.Core.UI.Utils;
using BusSchedule.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
        private IFavoritesManager _favoritesManager;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<TimetableItem> TimetableWorkingDays { get; private set; } = new List<TimetableItem>();
        public List<TimetableItem> TimetableSaturdays { get; private set; } = new List<TimetableItem>();
        public List<TimetableItem> TimetableHolidays { get; private set; } = new List<TimetableItem>();
        public List<TimetableItem> CurrentTimetable { get; private set; }
        public List<Trip_Description> TimetableLegend { get; private set; }
        public ObservableCollection<string> RouteDetails { get; private set; } = new ObservableCollection<string>();
        public TimetableItem NextBus { get; private set; }

        public bool IsOnFavoritesList()
        {
            return _favoritesManager.IsOnList(Route.Route_Id, Station.Stop_Id);
        }

        private Calendar.Service _currentCalendarService;
        public bool WorkingDaysVisible => _currentCalendarService == Calendar.Service.WorkingDays;
        public bool SaturdaysVisible => _currentCalendarService == Calendar.Service.Saturdays;
        public bool HolidaysVisible => _currentCalendarService == Calendar.Service.SundayAndHolidays;

        public ICommand ScheduleDaysChangedCommand { get; private set; }

        public TimetableViewModel(Stops station, Routes route, IDataProvider dataProvider, IFavoritesManager favoritesManager)
        {
            Station = station;
            Route = route;
            _dataProvider = dataProvider;
            _favoritesManager = favoritesManager;
        }

        public TimetableViewModel(Stops station, Routes route, int direction, IDataProvider dataProvider, IFavoritesManager favoritesManager)
        {
            Station = station;
            Route = route;
            _direction = direction;
            _dataProvider = dataProvider;
            _favoritesManager = favoritesManager;
        }

        public async Task RefreshTimetableAsync()
        {
            TimetableWorkingDays.Clear();
            TimetableSaturdays.Clear();
            TimetableHolidays.Clear();

            var legendData = await _dataProvider.GetRouteLegend(Route.Route_Id, _direction);
            TimetableLegend = ParseLegend(legendData);

            var timetableAll = _direction.HasValue ? await GtfsUtils.GetSchedule(_dataProvider, Route, Station, _direction.Value)
                : await GtfsUtils.GetSchedule(_dataProvider, Route, Station);

            var workingDaysId = await _dataProvider.GetWorkdaysServiceId();
            var saturdayaId = await _dataProvider.GetSaturdayServiceId();
            var sundayId = await _dataProvider.GetSundayServiceId();
            TimetableWorkingDays = Setup(timetableAll[workingDaysId]);
            TimetableSaturdays = Setup(timetableAll[saturdayaId]);
            TimetableHolidays = Setup(timetableAll[sundayId]);

            ShowScheduleForToday();
            HighlightNextBus();

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTimetable)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WorkingDaysVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaturdaysVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HolidaysVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimetableLegend)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NextBus)));
        }

        private void ShowScheduleForToday()
        {
            var today = DateTime.Today;
            if(today.DayOfWeek == DayOfWeek.Saturday)
            {
                _currentCalendarService = Calendar.Service.Saturdays;
                CurrentTimetable = TimetableSaturdays;
            }
            else if(today.DayOfWeek == DayOfWeek.Sunday || HolidaysHelper.IsTodayHoliday())
            {
                _currentCalendarService = Calendar.Service.SundayAndHolidays;
                CurrentTimetable = TimetableHolidays;
            }
            else
            {
                _currentCalendarService = Calendar.Service.WorkingDays;
                CurrentTimetable = TimetableWorkingDays;
            }
        }

        private void HighlightNextBus()
        {
            if (NextBus != null)
            {
                NextBus.IsHighlighted = false;
            }

            var currentTime = DateTime.Now;
            NextBus = CurrentTimetable.FirstOrDefault(item => item.Hour == currentTime.Hour);
            if ((NextBus == null || NextBus.Minutes.Last().Minutes < currentTime.Minute) && CurrentTimetable.FirstOrDefault(item => item.Hour == currentTime.Hour+1) != null )
            {
                NextBus = CurrentTimetable.FirstOrDefault(item => item.Hour == currentTime.Hour + 1);
            }
            if(NextBus != null)
            {
                NextBus.IsHighlighted = true;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NextBus.BackgroundColor)));
        }
            
        private List<Trip_Description> ParseLegend(IEnumerable<Trip_Description> legendData)
        {
            var result = new List<Trip_Description>();
            var alreadyAddedDescription = new List<string>();
            foreach(var legend in legendData)
            {
                if(!alreadyAddedDescription.Contains(legend.ShortDescription))
                {
                    alreadyAddedDescription.Add(legend.ShortDescription);
                    result.Add(legend);
                }
            }
            return result;
        }

        public void AddThisToFavorites()
        {
            _favoritesManager.Add(Route.Route_Id, Station.Stop_Id, _direction);
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

            HighlightNextBus();
            _currentCalendarService = calendarService;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTimetable)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WorkingDaysVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaturdaysVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HolidaysVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NextBus)));
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
