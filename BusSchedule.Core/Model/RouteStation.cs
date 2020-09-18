using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class RouteStation
    {
        public int RouteId { get; set; }
        public int BusStationId { get; set; }
        public int TimeDiffMinutes { get; set; }
    }
}
