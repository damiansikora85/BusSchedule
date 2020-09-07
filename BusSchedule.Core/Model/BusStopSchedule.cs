using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class BusStopSchedule
    {
        public int BusServiceId { get; set; }
        public int BusRouteId { get; set; }
        public int BusStopId { get; set; }
        public IList<TimeSpan> Times { get; set; }
    }
}
