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

        public ObservableCollection<TimeSpan> TimetableWorkingDays { get; private set; } = new ObservableCollection<TimeSpan>();
        public ObservableCollection<TimeSpan> TimetableSaturdays { get; private set; } = new ObservableCollection<TimeSpan>();
        public ObservableCollection<TimeSpan> TimetableHolidays { get; private set; } = new ObservableCollection<TimeSpan>();
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

            //lista wariantow trasy dla tego przystanku
            var busStopWithVariants = (await _dataProvider.GetStationDetailsForRoute(_route, _station)).Select(item => item.RouteVariant);

            //lista czasow rozpoczecia dla wszystkich wariantow trasy
            var routeStartTimes = (await _dataProvider.GetRouteBeginTimes(_route)).Where(item => busStopWithVariants.Contains(item.RouteVariant));

            //podział
            var workingDays = routeStartTimes.Where(time => time.Days == RouteBeginTime.ScheduleDays.WorkingDays).OrderBy(item => item.Time);
            var saturdays = routeStartTimes.Where(time => time.Days == RouteBeginTime.ScheduleDays.Saturday).OrderBy(item => item.Time);
            var sundaysAndHolidays = routeStartTimes.Where(time => time.Days == RouteBeginTime.ScheduleDays.SundayAndHolidays).OrderBy(item => item.Time);

            //szczegoly przystanek-trasa dla wariantu trasy
            var stationsDetails = new Dictionary<int, List<BusRouteDetails>>();
            var timeShifts = new Dictionary<int, TimeSpan>();
            //var timeAdjustments = new Dictionary<int, >
            foreach (var variantNum in busStopWithVariants)
            {
                var details = await _dataProvider.GetStationsDetailsForRouteVariant(_route, variantNum);
                stationsDetails.Add(variantNum, details);
                //stationsDetails.Sort((st1, st2) => st1.OrderNum.CompareTo(st2.OrderNum));

                timeShifts.Add(variantNum, CalculateTimeDiff(details, _station));
            }

            //
            var timeAdjustments = await _dataProvider.GetTimeAdjustmentForRoute(_route.Id);

            //var timespan = CalculateTimeDiff(stationsDetails);
            foreach (var time in workingDays)
            {
                //TODO
                //cache calculated or pre-calculate timeAdjustments
                var timeAdjustment = CalculateTimeAdjustment(time, stationsDetails[time.RouteVariant], timeAdjustments);
                TimetableWorkingDays.Add(time.Time + timeShifts[time.RouteVariant] + timeAdjustment);
            }

            foreach (var time in saturdays)
            {
                var timeAdjustment = CalculateTimeAdjustment(time, stationsDetails[time.RouteVariant], timeAdjustments);
                TimetableSaturdays.Add(time.Time + timeShifts[time.RouteVariant] + timeAdjustment);

                //var timeAdjustment = CalculateTimeAdjustment(time, stationsDetails, timeAdjustments);
                //TimetableSaturdays.Add(time.Time + timespan + timeAdjustment);
            }

            foreach (var time in sundaysAndHolidays)
            {
                var timeAdjustment = CalculateTimeAdjustment(time, stationsDetails[time.RouteVariant], timeAdjustments);
                TimetableHolidays.Add(time.Time + timeShifts[time.RouteVariant] + timeAdjustment);

                //var timeAdjustment = CalculateTimeAdjustment(time, stationsDetails, timeAdjustments);
                //TimetableHolidays.Add(time.Time + timespan + timeAdjustment);
            }

            //var stationsInfo = await _dataProvider.GetStationsForRoute(_route);
            //foreach(var station in stationsDetails)
            //{
            //    var stationInfo = stationsInfo.FirstOrDefault(info => info.Id == station.BusStopId);
            //    if(stationInfo != null)
            //    {
            //        RouteDetails.Add($"{stationInfo.Name} - {}");
            //    }
            //}
        }

        private TimeSpan CalculateTimeAdjustment(RouteBeginTime time, List<BusRouteDetails> stationsDetails, List<StationTimeAdjustment> timeAdjustments)
        {
            var timespan = TimeSpan.Zero;
            foreach (var station in stationsDetails)
            {
                var adjustment = timeAdjustments.FirstOrDefault(item => item.StationId == station.BusStopId && item.BeginTimeId == time.Id && item.Days == time.Days && item.RouteVariantId == time.RouteVariant);
                if (adjustment != null)
                {
                    timespan += TimeSpan.FromMinutes(adjustment.TimeAdjustmentMin);
                }
                if (station.BusStopId == _station.Id)
                {
                    break;
                }
            }

            return timespan;
        }

        private TimeSpan CalculateTimeDiff(List<BusRouteDetails> stationsDetails, BusStation thisStation)
        {
            var timespan = TimeSpan.Zero;
            foreach (var station in stationsDetails)
            {
                timespan += TimeSpan.FromMinutes(station.TimeDiff);
                if (station.BusStopId == thisStation.Id)
                {
                    break;
                }
            }

            return timespan;
        }
    }
}
