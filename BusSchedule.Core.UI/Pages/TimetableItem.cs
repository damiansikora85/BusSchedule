using Color = Microsoft.Maui.Graphics.Color;

namespace BusSchedule.Core.UI;

public class TimetableItem
{
    public int Hour { get; set; }
    public List<TimetableItemMinutes> Minutes { get; set; }

    public Color BackgroundColor => IsHighlighted ? Color.FromRgba(247, 212, 148, 255) : Colors.Transparent;
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
            return string.IsNullOrEmpty(AdditionalInfo) ? Minutes.ToString("D2") : $"{Minutes:D2}{AdditionalInfo}";
        }
    }
}
