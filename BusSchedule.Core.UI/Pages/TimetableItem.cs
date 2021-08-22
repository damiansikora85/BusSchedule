using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BusSchedule.Core.UI
{
    public class TimetableItem
    {
        public int Hour { get; set; }
        public List<TimetableItemMinutes> Minutes { get; set; }

        public Color BackgroundColor => IsHighlighted ? Color.FromArgb(247, 212, 148) : Color.Transparent;
        public bool IsHighlighted { get; set; }

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
                return string.IsNullOrEmpty(AdditionalInfo) ? Minutes.ToString("D2") : $"{Minutes}{AdditionalInfo}";
            }
        }
    }
}
