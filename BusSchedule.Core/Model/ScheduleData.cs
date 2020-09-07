using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class ScheduleData
    {
        public List<BusService> BusServices { get; set; } = new List<BusService>();
        public List<BusStation> BusStations { get; set; } = new List<BusStation>();
        public List<BusRoute> Routes { get; set; } = new List<BusRoute>();

        public ScheduleData()
        {

        }
    }
}
