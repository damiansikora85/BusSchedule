using BusSchedule.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Core.Utils
{
    public interface IDataProvider
    {
        void SetDatabasePath(string databasePath);
        Task<List<Routes>> GetRoutes();
        Task<Destination> GetRouteDestinations(Routes route);
        Task<IEnumerable<Trips>> GetTripsForRoute(Routes route, int direction);
        Task<List<Stops>> GetStopsForRoute(Routes route, int direction);
        Task<IEnumerable<Trips>> GetTripsForRoute(Routes route, int direction, string serviceId);
        Task<IEnumerable<Trips>> GetTripsForRoute(Routes route, string serviceId);
        Task<IEnumerable<Stop_Times>> GetStopTimesForTrip(Trips trip);
        Task<(DateTime startDate, DateTime endDate)> GetFeedStartEndDates();
        Task<Stops> GetStopById(string stop_Id);
        Task SaveNews(IList<News> news);
        Task<IEnumerable<Stop_Times>> GetStopTimesForTrip(string tripId, string stopId);
        Task<IEnumerable<Calendar>> GetCalendar();
        Task<IList<News>> GetNews(bool showOnly);
        Task<Routes> GetRoute(string routeId);
        Task<IEnumerable<Trip_Description>> GetRouteLegend(string route_Id, int? direction);
        Task<IEnumerable<Trip_Description>> GetRouteDescriptionForTrips(IEnumerable<Trips> tripsForRoute);
        Task<string> GetWorkdaysServiceId();
        Task<string> GetSaturdayServiceId();
        Task<string> GetSundayServiceId();
        Task Test();
    }
}
