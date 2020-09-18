using BusSchedule.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Creator.Model
{
    public class RouteStationViewModel
    {
        public int RouteId { get; set; }
        public BusStation BusStation { get; set; }
        public int TimeDiff { get; set; }
        public int OrderNum { get; set; }

        public override string ToString()
        {
            return $"{BusStation.Name} ({TimeDiff})";
        }
    }
}
