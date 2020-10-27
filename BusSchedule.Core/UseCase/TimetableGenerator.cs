using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Core.UseCase
{
    public static class TimetableGenerator
    {
        public async static Task<Dictionary<RouteBeginTime.ScheduleDays, List<TimeSpan>>> Generate(BusStation station, BusRoute route, IDataProvider _dataProvider)
        {
            var timetable = new Dictionary<RouteBeginTime.ScheduleDays, List<TimeSpan>>();

            //lista wariantow trasy dla tego przystanku
            var busStopWithVariants = (await _dataProvider.GetStationDetailsForRoute(route, station)).Select(item => item.RouteVariant);

            //lista czasow rozpoczecia dla wszystkich wariantow trasy
            var routeStartTimes = (await _dataProvider.GetRouteBeginTimes(route)).Where(item => busStopWithVariants.Contains(item.RouteVariant));

            //podział
            var workingDays = routeStartTimes.Where(time => time.Days == RouteBeginTime.ScheduleDays.WorkingDays).OrderBy(item => item.Time);
            var saturdays = routeStartTimes.Where(time => time.Days == RouteBeginTime.ScheduleDays.Saturday).OrderBy(item => item.Time);
            var sundaysAndHolidays = routeStartTimes.Where(time => time.Days == RouteBeginTime.ScheduleDays.SundayAndHolidays).OrderBy(item => item.Time);

            //szczegoly przystanek-trasa dla wariantu trasy
            var stationsDetails = new Dictionary<int, List<BusRouteDetails>>();
            var timeShifts = new Dictionary<int, TimeSpan>();

            foreach (var variantNum in busStopWithVariants)
            {
                var details = await _dataProvider.GetStationsDetailsForRouteVariant(route, variantNum);
                stationsDetails.Add(variantNum, details);
                timeShifts.Add(variantNum, CalculateTimeDiff(details, station));
            }

            var timeAdjustments = await _dataProvider.GetTimeAdjustmentForRoute(route.Id);

            timetable.Add(RouteBeginTime.ScheduleDays.WorkingDays, CalculateTimetable(station, workingDays, stationsDetails, timeShifts, timeAdjustments));
            timetable.Add(RouteBeginTime.ScheduleDays.Saturday, CalculateTimetable(station, saturdays, stationsDetails, timeShifts, timeAdjustments));
            timetable.Add(RouteBeginTime.ScheduleDays.SundayAndHolidays, CalculateTimetable(station, sundaysAndHolidays, stationsDetails, timeShifts, timeAdjustments));

            return timetable;
        }

        private static List<TimeSpan> CalculateTimetable(BusStation station, IOrderedEnumerable<RouteBeginTime> workingDays, Dictionary<int, List<BusRouteDetails>> stationsDetails, Dictionary<int, TimeSpan> timeShifts, List<StationTimeAdjustment> timeAdjustments)
        {
            var timetable = new List<TimeSpan>();
            foreach (var time in workingDays)
            {
                //TODO
                //cache calculated or pre-calculate timeAdjustments
                var timeAdjustment = CalculateTimeAdjustment(time, station, stationsDetails[time.RouteVariant], timeAdjustments);
                timetable.Add(time.Time + timeShifts[time.RouteVariant] + timeAdjustment);
            }
            return timetable;
        }

        private static TimeSpan CalculateTimeDiff(List<BusRouteDetails> stationsDetails, BusStation thisStation)
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

        private static TimeSpan CalculateTimeAdjustment(RouteBeginTime time, BusStation destination, List<BusRouteDetails> stationsDetails, List<StationTimeAdjustment> timeAdjustments)
        {
            var timespan = TimeSpan.Zero;
            foreach (var station in stationsDetails)
            {
                var adjustment = timeAdjustments.FirstOrDefault(item => item.StationId == station.BusStopId && item.BeginTimeId == time.Id && item.Days == time.Days && item.RouteVariantId == time.RouteVariant);
                if (adjustment != null)
                {
                    timespan += TimeSpan.FromMinutes(adjustment.TimeAdjustmentMin);
                }
                if (station.BusStopId == destination.Id)
                {
                    break;
                }
            }

            return timespan;
        }
    }
}
