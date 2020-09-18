using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class RouteBeginTime
    {
        public enum ScheduleDays
        {
            WorkingDays,
            Saturday,
            SundayAndHolidays
        }
        public int RouteId { get; set; }
        public TimeSpan Time { get; set; }
        public ScheduleDays Days { get; set; }

        public override string ToString()
        {
            return Time.ToString();
        }
    }
}
