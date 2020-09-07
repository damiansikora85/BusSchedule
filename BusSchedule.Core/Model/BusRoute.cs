using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class BusRoute
    {
        public int Id { get; set; }
        public int BusServiceId { get; set; }
        public int StartStationId { get; set; }
        public int EndStationId { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
