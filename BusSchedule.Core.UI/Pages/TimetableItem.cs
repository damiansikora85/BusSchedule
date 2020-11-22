using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.UI
{
    public class TimetableItem
    {
        public int Hour { get; set; }
        public List<int> Minutes { get; set; }

        public override string ToString()
        {
            return Hour.ToString();
        }
    }
}
