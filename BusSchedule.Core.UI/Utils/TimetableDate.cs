using System;
using System.ComponentModel;
using System.Drawing;

namespace BusSchedule.Core.UI.Utils
{
    public class TimetableDate : INotifyPropertyChanged
    {
        public DateTime Date { get; private set; }
        public string DayOfWeek => Date.ToString("ddd");
        public string DayNum => Date.ToString("dd.MM");
        public bool IsSelected { get; set; }
        public Color BackgroundColor => IsSelected ? Color.FromArgb(35, 113, 148) : Color.FromArgb(240, 240, 240);
        public Color TextColor => IsSelected ? Color.White : Color.FromArgb(128, 128, 128);

        public TimetableDate(DateTime date)
        {
            Date = date;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Select()
        {
            IsSelected = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BackgroundColor)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextColor)));
        }

        public void Deselect()
        {
            IsSelected = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BackgroundColor)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextColor)));
        }
    }
}
