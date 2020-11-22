using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class Stop_Times
    {
        public string Trip_Id { get; set; }
        public string Arrival_Time { get; set; }
        public string Departure_Time { get; set; }
        public string Stop_Id { get; set; }
        public string Stop_Sequence { get; set; }
        public string Pickup_Type { get; set; }
        public string Drop_Off_Type { get; set; }
    }
}
