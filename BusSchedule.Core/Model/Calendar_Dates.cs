using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class Calendar_Dates
    {
        public string Service_Id { get; set; }
        public string Date { get; set; }
        public string Exception_Type { get; set; }

        public const string ServiceAdded = "1";
        public const string ServiceRemoved = "2";
    }
}
