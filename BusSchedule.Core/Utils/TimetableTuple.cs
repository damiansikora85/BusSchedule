using BusSchedule.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Utils
{
    public class TimetableTuple
    {
        public TimeSpan Time { get; set; }
        public Trip_Description AdditionalDescription { get; set; }
    }
}
