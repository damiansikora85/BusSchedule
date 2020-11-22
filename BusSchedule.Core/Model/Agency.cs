using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class Agency
    {
        public string Agency_Id { get; set; }
        public string Agency_Name { get; set; }
        public string Agency_Url { get; set; }
        public string Agency_Timezone { get; set; }
        public string Agency_Lang { get; set; }
        public string Agency_Phone { get; set; }
        public string Agency_Fare_Url { get; set; }
    }
}
