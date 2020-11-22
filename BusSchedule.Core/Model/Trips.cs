using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class Trips
    {
        public string Trip_Id { get; set; }
        public string Route_Id { get; set; }
        public string Service_Id { get; set; }
        public string Trip_Headsign { get; set; }
        public string Direction_Id { get; set; }
        public string Shape_Id { get; set; }
    }
}
