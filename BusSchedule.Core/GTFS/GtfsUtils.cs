using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusSchedule.Core.Model.Calendar;

namespace BusSchedule.Core.GTFS
{
    public class GtfsUtils
    {
        public static async Task<IEnumerable<Stops>> GetStopsForRoute(IDataProvider dataProvider,  Routes route, int direction)
        {
            var stopsForRoute = new List<Stops>();
            var trips = await dataProvider.GetTripsForRoute(route.Route_Id, direction);
            var filteredTrips = trips.GroupBy(trip => trip.Shape_Id).Select(group => group.First());
            var stopTimesForRoute = new List<List<Stop_Times>>();
            foreach (var trip in filteredTrips)
            {
                stopTimesForRoute.Add((await dataProvider.GetStopTimesForTrip(trip)).ToList());
            }

            foreach (var stopTimes in stopTimesForRoute.OrderByDescending(st => st.Count()))
            {
                foreach (var stopTime in stopTimes)
                {
                    if (stopsForRoute.FirstOrDefault(stop => stop.Stop_Id == stopTime.Stop_Id) == null)
                    {
                        var stop = await dataProvider.GetStopById(stopTime.Stop_Id);
                        stopsForRoute.Add(stop);
                    }
                }
            }

            return stopsForRoute;
        }

        public static async Task<Dictionary<string, List<TimetableTuple>>> GetSchedule(IDataProvider dataProvider, Routes route, Stops station)
        {
            var schedule = new Dictionary<string, List<TimetableTuple>>();
            var calendar = await dataProvider.GetCalendar();
            foreach (var day in calendar)
            {
                schedule.Add(day.Service_Id, new List<TimetableTuple>());
                var tripsForRoute = await dataProvider.GetTripsForRoute(route.Route_Id, day.Service_Id);

                var desc = await dataProvider.GetRouteDescriptionForTrips(tripsForRoute);
                foreach (var trip in tripsForRoute)
                {
                    var stopTimes = (await dataProvider.GetStopTimesForTrip(trip.Trip_Id, station.Stop_Id)).Select(stopTime => TimeSpan.Parse(stopTime.Arrival_Time));
                    var item = stopTimes.Select(st => new TimetableTuple
                    {
                        Time = st,
                        AdditionalDescription = desc.Where(d => d.Shape_Id == trip.Shape_Id).FirstOrDefault()
                    });
                    schedule[day.Service_Id].AddRange(item);
                    //schedule[day.Service_Id].AddRange((await dataProvider.GetStopTimesForTrip(trip.Trip_Id, station.Stop_Id)).Select(stopTime => TimeSpan.Parse(stopTime.Arrival_Time)));
                }
            }
            return schedule;
        }

        public static async Task<List<TimetableTuple>> GetSchedule(IDataProvider dataProvider, Routes route, Stops station, DateTime date)
        {
            var schedule = new List<TimetableTuple>();
            var serviceId = await GetServiceIdForDate(date, dataProvider);

            var tripsForRoute = await dataProvider.GetTripsForRoute(route.Route_Id, serviceId);
            var desc = await dataProvider.GetRouteDescriptionForTrips(tripsForRoute);
            foreach (var trip in tripsForRoute)
            {
                var stopTimes = (await dataProvider.GetStopTimesForTrip(trip.Trip_Id, station.Stop_Id)).Select(stopTime => TimeSpan.Parse(stopTime.Arrival_Time));
                var item = stopTimes.Select(st => new TimetableTuple
                {
                    Time = st,
                    AdditionalDescription = desc.FirstOrDefault(d => d.Shape_Id == trip.Shape_Id)
                });
                schedule.AddRange(item);
            }
            return schedule;
        }

        public static async Task<List<TimetableTuple>> GetSchedule(IDataProvider dataProvider, Routes route, Stops station, int direction, DateTime date)
        {
            var schedule = new List<TimetableTuple>();
            var serviceId = await GetServiceIdForDate(date, dataProvider);

            var tripsForRoute = await dataProvider.GetTripsForRoute(route.Route_Id, direction, serviceId);
            var desc = await dataProvider.GetRouteDescriptionForTrips(tripsForRoute);
            foreach (var trip in tripsForRoute)
            {
                var stopTimes = (await dataProvider.GetStopTimesForTrip(trip.Trip_Id, station.Stop_Id)).Select(stopTime => TimeSpan.Parse(stopTime.Arrival_Time));
                var item = stopTimes.Select(st => new TimetableTuple
                {
                    Time = st,
                    AdditionalDescription = desc.FirstOrDefault(d => d.Shape_Id == trip.Shape_Id)
                });
                schedule.AddRange(item);
            }
            return schedule;
        }

        private static async Task<string> GetServiceIdForDate(DateTime date, IDataProvider dataProvider)
        {
            var serviceId = await dataProvider.GetServiceIdByDate(date);
            if(!string.IsNullOrEmpty(serviceId))
            {
                return serviceId;
            }
            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                return await dataProvider.GetSaturdayServiceId();
            }
            else if (date.DayOfWeek == DayOfWeek.Sunday || HolidaysHelper.IsTodayHoliday())
            {
                return await dataProvider.GetSundayServiceId();
            }
            else
            {
                return await dataProvider.GetWorkdaysServiceId();
            }
        }

        public static async Task<Dictionary<string, List<TimetableTuple>>> GetSchedule(IDataProvider dataProvider, Routes route, Stops station, int direction)
        {
            var schedule = new Dictionary<string, List<TimetableTuple>>();
            var calendar = await dataProvider.GetCalendar();
            foreach (var day in calendar)
            {
                schedule.Add(day.Service_Id, new List<TimetableTuple>());
                var tripsForRoute = await dataProvider.GetTripsForRoute(route.Route_Id, direction, day.Service_Id);

                //get descriptions for trips
                var desc = await dataProvider.GetRouteDescriptionForTrips(tripsForRoute);
                foreach (var trip in tripsForRoute)
                {
                    var stopTimes = (await dataProvider.GetStopTimesForTrip(trip.Trip_Id, station.Stop_Id)).Select(stopTime => TimeSpan.Parse(stopTime.Arrival_Time));
                    var item = stopTimes.Select(st => new TimetableTuple
                    {
                        Time = st,
                        AdditionalDescription = desc.FirstOrDefault(d => d.Shape_Id == trip.Shape_Id)
                    });
                    schedule[day.Service_Id].AddRange(item);
                }
            }
            return schedule;
        }
    }
}
