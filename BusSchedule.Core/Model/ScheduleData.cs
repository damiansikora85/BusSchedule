using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class ScheduleData
    {
        public int Version { get; set; }
        public List<BusService> BusServices { get; set; } = new List<BusService>();
        public List<BusStation> BusStations { get; set; } = new List<BusStation>();
        public List<BusRoute> Routes { get; set; } = new List<BusRoute>();
        public List<BusRouteDetails> RoutesDetails { get; set; } = new List<BusRouteDetails>();
        public List<RouteBeginTime> RoutesBeginTimes { get; set; } = new List<RouteBeginTime>();
        public List<StationTimeAdjustment> TimeAdjustments { get; set; } = new List<StationTimeAdjustment>();

        public ScheduleData()
        {

        }
    }
}
