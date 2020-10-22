using System;
using System.Collections.Generic;
using System.Text;
using static BusSchedule.Core.Model.RouteBeginTime;

namespace BusSchedule.Core.Model
{
    public class StationTimeAdjustment
    {
        public int StationId { get; set; }
        public int BeginTimeId { get; set; }
        public int RouteId { get; set; }
        public int RouteVariantId { get; set; }
        public int TimeAdjustmentMin { get; set; }
        public ScheduleDays Days { get; set; }
    }
}
