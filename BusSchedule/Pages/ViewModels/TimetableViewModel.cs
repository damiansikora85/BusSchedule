using BusSchedule.Core.Model;
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

        public ObservableCollection<TimeSpan> Times { get; private set; } = new ObservableCollection<TimeSpan>();

        public TimetableViewModel(BusStation station, BusRoute route, IDataProvider dataProvider)
        {
            _station = station;
            _route = route;
            _dataProvider = dataProvider;
        }

        internal async Task RefreshTimetableAsync()
        {
            var routeStartTimes = await _dataProvider.GetRouteBeginTimes(_route);
            var workingDays = routeStartTimes.Where(time => time.Days == RouteBeginTime.ScheduleDays.WorkingDays);
            var saturdays = routeStartTimes.Where(time => time.Days == RouteBeginTime.ScheduleDays.Saturday);
            var sundaysAndHolidays = routeStartTimes.Where(time => time.Days == RouteBeginTime.ScheduleDays.SundayAndHolidays);


            foreach(var time in workingDays)
            {
                Times.Add(time.Time);
            }

            var stationsDetails = await _dataProvider.GetStationsDetailsForRoute(_route);
            stationsDetails.Sort((st1, st2) => st1.OrderNum.CompareTo(st2.OrderNum));
            var timespan = TimeSpan.Zero;
            foreach(var station in stationsDetails)
            {
                if(station.BusStopId == _station.Id)
                {
                    break;
                }
                timespan += TimeSpan.FromMinutes(station.TimeDiff);
            //    Times.Add(timespan);
            }
            foreach (var time in workingDays)
            {
                Times.Add(time.Time+ timespan);
            }
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Times)));
        }
    }
}
