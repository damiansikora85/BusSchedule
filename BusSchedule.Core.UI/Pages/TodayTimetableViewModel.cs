using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using BusSchedule.Core.Model;
using System.Threading.Tasks;
using BusSchedule.Core.Utils;
using System.ComponentModel;
using BusSchedule.Core.GTFS;
using System.Linq;
using BusSchedule.Core.UI.Utils;
using System.Collections;

namespace BusSchedule.Core.UI.Pages
{
    public class TodayTimetableViewModel : BaseViewModel
    {
        public Routes Route { get; private set; }
        public Stops Station { get; private set; }

        private IDataProvider _dataProvider;
        private int? _direction;

        public List<TimetableItem> Timetable { get; private set; }

        public TimetableItem NextBus { get; private set; }

        public List<Trip_Description> TimetableLegend { get; private set; }
        public string TodayDate => DateTime.Now.ToString("d");

        public IList<TimetableDate> Dates { get; private set; }
        public TimetableDate SelectedDay { get; set; }

        public TodayTimetableViewModel(Routes route, Stops station, int? direction, IDataProvider dataProvider)
        {
            Route = route;
            Station = station;
            _direction = direction;
            _dataProvider = dataProvider;
            var today = DateTime.Now;
            Dates = Enumerable.Range(0, 100).Select(i => new TimetableDate(today.AddDays(i))).ToList();
            SelectedDay = Dates.First();
            SelectedDay.Select();
        }

        public async Task RefreshTimetableAsync()
        {
            var legendData = await _dataProvider.GetRouteLegend(Route.Route_Id, _direction);
            TimetableLegend = ParseLegend(legendData);
            var schedule = await GetScheduleForDay();
            Timetable = Setup(schedule);
            HighlightNextBus();

            OnPropertyChanged(nameof(Timetable));
            OnPropertyChanged(nameof(TimetableLegend));
        }

        private async Task<IList<TimetableTuple>> GetScheduleForDay()
        {
            return _direction.HasValue ? await GtfsUtils.GetSchedule(_dataProvider, Route, Station, _direction.Value, SelectedDay.Date) : await GtfsUtils.GetSchedule(_dataProvider, Route, Station, SelectedDay.Date); ;
        }

        private List<Trip_Description> ParseLegend(IEnumerable<Trip_Description> legendData)
        {
            var result = new List<Trip_Description>();
            var alreadyAddedDescription = new List<string>();
            foreach (var legend in legendData)
            {
                if (!alreadyAddedDescription.Contains(legend.ShortDescription))
                {
                    alreadyAddedDescription.Add(legend.ShortDescription);
                    result.Add(legend);
                }
            }
            return result;
        }

        private List<TimetableItem> Setup(IList<TimetableTuple> timetable)
        {
            var sorted = timetable.OrderBy(t => t.Time);
            var grouped = sorted.GroupBy(item => item.Time.Hours);
            var list = new List<TimetableItem>();
            foreach (var group in grouped)
            {
                var timetableItem = new TimetableItem
                {
                    Hour = group.Key,
                    Minutes = group.Select(it => new TimetableItem.TimetableItemMinutes { Minutes = it.Time.Minutes, AdditionalInfo = it.AdditionalDescription?.ShortDescription }).ToList()
                };
                list.Add(timetableItem);
            }
            return list;
        }

        public async Task OnNewDaySelected()
        {
            var schedule = await GetScheduleForDay();
            Timetable = Setup(schedule);
            if(SelectedDay.Date.Date == DateTime.Now.Date)
            {
                HighlightNextBus();
            }

            OnPropertyChanged(nameof(Timetable));
        }

        private void HighlightNextBus()
        {
            if (NextBus != null)
            {
                NextBus.IsHighlighted = false;
            }

            var currentTime = DateTime.Now;
            NextBus = Timetable.FirstOrDefault(item => item.Hour == currentTime.Hour);
            if ((NextBus == null || NextBus.Minutes.Last().Minutes < currentTime.Minute) && Timetable.FirstOrDefault(item => item.Hour == currentTime.Hour + 1) != null)
            {
                NextBus = Timetable.FirstOrDefault(item => item.Hour == currentTime.Hour + 1);
            }
            if (NextBus != null)
            {
                NextBus.IsHighlighted = true;
            }
            OnPropertyChanged(nameof(NextBus.BackgroundColor));
        }
    }
}
