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
        public Color BackgroundColor => IsSelected ? Color.FromArgb(23, 71, 94) : Color.White;

        public TimetableDate(DateTime date)
        {
            Date = date;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Select()
        {
            IsSelected = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BackgroundColor)));
        }

        public void Deselect()
        {
            IsSelected = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BackgroundColor)));
        }
    }
}
