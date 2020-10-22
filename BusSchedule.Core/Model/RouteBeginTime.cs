using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class RouteBeginTime
    {
        public RouteBeginTime(RouteBeginTime second)
        {
            Id = second.Id;
            RouteId = second.RouteId;
            RouteVariant = second.RouteVariant;
            Days = second.Days;
            Time = second.Time;
        }

        public RouteBeginTime()
        {
        }

        public enum ScheduleDays
        {
            WorkingDays,
            Saturday,
            SundayAndHolidays
        }
        public int Id { get; set; }
        public int RouteId { get; set; }
        public int RouteVariant { get; set; }
        public TimeSpan Time { get; set; }
        public ScheduleDays Days { get; set; }

        public override string ToString()
        {
            return Time.ToString();
        }
    }
}
