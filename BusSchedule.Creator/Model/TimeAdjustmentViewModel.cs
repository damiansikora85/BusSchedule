using BusSchedule.Core.Model;
using System;

namespace BusSchedule.Creator.Model
{
    public class TimeAdjustmentViewModel
    {
        public RouteBeginTime RouteBeginTime;
        public TimeSpan TimeAdjustment;
        public int StationId;

        public TimeAdjustmentViewModel(RouteBeginTime time, TimeSpan timeAdjustment, int stationId)
        {
            this.RouteBeginTime = time;
            this.TimeAdjustment = timeAdjustment;
            this.StationId = stationId;
        }

        public override string ToString()
        {
            return $"{RouteBeginTime} ({TimeAdjustment})";
        }
    }
}
