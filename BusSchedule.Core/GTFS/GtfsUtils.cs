using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Core.GTFS
{
    public class GtfsUtils
    {
        public static async Task<IEnumerable<Stops>> GetStopsForRoute(IDataProvider dataProvider,  Routes route, int direction)
        {
            var stopsForRoute = new List<Stops>();
            var trips = await dataProvider.GetTripsForRoute(route, direction);
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

        public static async Task<Dictionary<string, List<TimeSpan>>> GetSchedule(IDataProvider dataProvider, Routes route, Stops station)
        {
            var schedule = new Dictionary<string, List<TimeSpan>>();
            var calendar = await dataProvider.GetCalendar();
            foreach (var day in calendar)
            {
                schedule.Add(day.Service_Id, new List<TimeSpan>());
                var tripsForRoute = await dataProvider.GetTripsForRoute(route, day.Service_Id);
                foreach (var trip in tripsForRoute)
                {
                    schedule[day.Service_Id].AddRange((await dataProvider.GetStopTimesForTrip(trip.Trip_Id, station.Stop_Id)).Select(stopTime => TimeSpan.Parse(stopTime.Arrival_Time)));
                }
            }
            return schedule;
        }

        public static async Task<Dictionary<string, List<TimeSpan>>> GetSchedule(IDataProvider dataProvider, Routes route, Stops station, int direction)
        {
            var schedule = new Dictionary<string, List<TimeSpan>>();
            var calendar = await dataProvider.GetCalendar();
            foreach (var day in calendar)
            {
                schedule.Add(day.Service_Id, new List<TimeSpan>());
                var tripsForRoute = await dataProvider.GetTripsForRoute(route, direction, day.Service_Id);
                foreach (var trip in tripsForRoute)
                {
                    schedule[day.Service_Id].AddRange((await dataProvider.GetStopTimesForTrip(trip.Trip_Id, station.Stop_Id)).Select(stopTime => TimeSpan.Parse(stopTime.Arrival_Time)));
                }
            }
            return schedule;
        }
    }
}
