using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class Routes
    {
        public string Route_Id { get; set; }
        public string Agency_Id { get; set; }
        public string Route_Short_Name { get; set; }
        public string Route_Long_Name { get; set; }
        public string Route_Desc { get; set; }
        public string Route_Type { get; set; }
        public string Route_Color { get; set; }
        public string Route_Text_Color { get; set; }
    }
}
