using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Model
{
    public class Calendar
    {
        public string Service_Id { get; set; }
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }
        public string Sunday { get; set; }
        public string Start_Date { get; set; }
        public string End_Date { get; set; }

        public enum Service
        {
            WorkingDays,
            Saturdays,
            SundayAndHolidays
        }

        public int GetWorkingDaysCount()
        {
            return int.Parse(Monday) + int.Parse(Tuesday) + int.Parse(Wednesday) + int.Parse(Thursday) + int.Parse(Friday);
        }
    }
}
