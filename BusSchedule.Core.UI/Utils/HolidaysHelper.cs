using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.UI.Utils
{
    public static class HolidaysHelper
    {
        private static List<string> PublicHolidays = new List<string>
        {
            "01.01" ,
            "06.01",
            "01.05",
            "03.05",
            "15.08",
            "01.11",
            "11.11",
            "25.12",
            "26.12"
        };

        public static bool IsTodayHoliday()
        {
            return PublicHolidays.Contains(DateTime.Now.ToString("dd.MM"));
        }
    }
}
