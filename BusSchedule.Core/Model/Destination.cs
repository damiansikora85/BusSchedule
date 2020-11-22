using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class Destination
    {
        public string Route_Id { get; set; }
        public string Outbound { get; set; }
        public string Inbound { get; set; }
    }
}
