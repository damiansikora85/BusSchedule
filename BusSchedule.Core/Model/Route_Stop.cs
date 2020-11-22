using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class Route_Stop
    {
        public string Route_Id { get; set; }
        public int Direction_Id { get; set; }
        public int Stop_Sequence { get; set; }
        public string Stop_Id { get; set; }
    }
}
