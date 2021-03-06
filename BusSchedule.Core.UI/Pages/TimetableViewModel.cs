﻿using BusSchedule.Core.GTFS;
using BusSchedule.Core.Interfaces;
using BusSchedule.Core.Model;
using BusSchedule.Core.UI;
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

            TimetableLegend = (await _dataProvider.GetRouteLegend(Route.Route_Id, _direction)).ToList();

            var timetableAll = _direction.HasValue ? await GtfsUtils.GetSchedule(_dataProvider, Route, Station, _direction.Value)
                : await GtfsUtils.GetSchedule(_dataProvider, Route, Station);

            var workingDaysId = await _dataProvider.GetWorkdaysServiceId();
            var saturdayaId = await _dataProvider.GetSaturdayServiceId();
            var sundayId = await _dataProvider.GetSundayServiceId();
            TimetableWorkingDays = Setup(timetableAll[workingDaysId]);
            TimetableSaturdays = Setup(timetableAll[saturdayaId]);
            TimetableHolidays = Setup(timetableAll[sundayId]);

            _currentCalendarService = Calendar.Service.WorkingDays;
            CurrentTimetable = TimetableWorkingDays;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTimetable)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WorkingDaysVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaturdaysVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HolidaysVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimetableLegend)));
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
