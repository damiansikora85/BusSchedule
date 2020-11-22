using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class Stops
    {
        public string Stop_Id { get; set; }
        public string Stop_Code { get; set; }
        public string Stop_Name { get; set; }
        public string Stop_Lat { get; set; }
        public bool IsLast { get; set; }
        public string Stop_Lon { get; set; }

    }
}
