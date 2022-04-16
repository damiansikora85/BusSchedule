using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.UI.Utils
{
    public class StopLocation
    {
        public StopLocation(string name, double latitude, double longitude, string stopId)
        {
            Latitude = latitude;
            Longitude = longitude;
            StopId = stopId;
            Name = name;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string StopId { get; set; }
        public string Name { get; set; }

    }
}
