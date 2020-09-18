using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class BusRouteDetails
    {
        public int BusRouteId { get; set; }
        public int BusStopId { get; set; }
        public int TimeDiff { get; set; }
        public int OrderNum { get; set; }
    }
}
