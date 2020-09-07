using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class BusRouteDetails
    {
        public int BusRouteId { get; set; }
        public IList<BusStation> BusStops { get; set; }
    }
}
