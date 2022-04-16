using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class Trace
    {
        public Trace()
        {
            Points = new List<Point>();
        }

        public List<Point> Points { get; set; }
    }
}
