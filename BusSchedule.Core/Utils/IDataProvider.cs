using BusSchedule.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Core.Utils
{
    public interface IDataProvider
    {
        Task<List<BusService>> GetBusServices();
        Task<List<BusRoute>> GetBusRoutes(int busServiceId);
        Task UpdateAsync(ScheduleData schedule);
        Task<List<BusStation>> GetStationsForRoute(BusRoute route);
        Task<List<BusRouteDetails>> GetStationsDetailsForRouteVariant(BusRoute route, int routeVariant);
        Task<List<RouteBeginTime>> GetRouteBeginTimes(BusRoute route);
        Task<List<BusStation>> GetBusRoutes(List<BusRouteDetails> stationsDetails);
        Task<List<BusRouteDetails>> GetStationDetailsForRoute(BusRoute route, BusStation station);
        Task<List<StationTimeAdjustment>> GetTimeAdjustmentForRoute(int id);
    }
}
