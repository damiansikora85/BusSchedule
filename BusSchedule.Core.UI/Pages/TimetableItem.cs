using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.UI
{
    public class TimetableItem
    {
        public int Hour { get; set; }
        public List<TimetableItemMinutes> Minutes { get; set; }

        public override string ToString()
        {
            return Hour.ToString();
        }

        public class TimetableItemMinutes
        {
            public int Minutes { get; set; }
            public string AdditionalInfo { get; set; }

            public override string ToString()
            {
                return string.IsNullOrEmpty(AdditionalInfo) ? Minutes.ToString() : $"{Minutes}{AdditionalInfo}";
            }
        }
    }
}
