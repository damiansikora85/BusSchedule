using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class FavoriteDescription
    {
        public string RouteId { get; set; }
        public string StopId { get; set; }
        public int Direction { get; set; }
    }
}
