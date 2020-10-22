using BusSchedule.Core.Model;
using BusSchedule.Creator.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Creator
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private ScheduleData _theSchedule;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<BusService> BusServices { get; private set; }
        public ObservableCollection<BusStation> BusStations { get; private set; }
        public List<BusRoute> Routes { get; private set; }
        public List<BusRoute> RoutesForService { get; private set; }
        public List<int> RouteVariants { get; private set; }
        public List<RouteStationViewModel> RouteDetails { get; private set; }
        public List<RouteStationViewModel> RouteDetailsForRoute { get; private set; }
        public List<RouteBeginTime> RouteBeginTimes { get; private set; }
        public List<RouteBeginTime> BeginTimesForRoute { get; private set; }
        public List<TimeAdjustmentViewModel> TimeAdjustments { get; private set; }
        public List<TimeAdjustmentViewModel> TimeAdjustmentsForSelection { get; private set; }

        public MainWindowViewModel()
        {
            _theSchedule = new ScheduleData();
            BusStations = new ObservableCollection<BusStation>();
            BusServices = new ObservableCollection<BusService>();
            Routes = new List<BusRoute>();
            RoutesForService = new List<BusRoute>();
            RouteVariants = new List<int>();
            RouteDetails = new List<RouteStationViewModel>();
            RouteBeginTimes = new List<RouteBeginTime>();
            TimeAdjustments = new List<TimeAdjustmentViewModel>();
        }

        internal void Setup(string jsonData)
        {
            try
            {
                BusServices.Clear();
                _theSchedule = JsonConvert.DeserializeObject<ScheduleData>(jsonData);
                foreach (var service in _theSchedule.BusServices)
                {
                    BusServices.Add(service);
                }

                BusStations.Clear();
                foreach (var station in _theSchedule.BusStations)
                {
                    BusStations.Add(station);
                }
                Routes.Clear();
                Routes.AddRange(_theSchedule.Routes);

                RouteDetails.Clear();
                RouteDetails.AddRange(_theSchedule.RoutesDetails.Select(rd => new RouteStationViewModel { RouteId = rd.BusRouteId, BusStation = BusStations.FirstOrDefault(BusStopSchedule => BusStopSchedule.Id == rd.BusStopId), OrderNum = rd.OrderNum, TimeDiff = rd.TimeDiff, RouteVariantId = rd.RouteVariant }));

                RouteBeginTimes.Clear();
                RouteBeginTimes.AddRange(_theSchedule.RoutesBeginTimes);

                TimeAdjustments.Clear();
                TimeAdjustments.AddRange(_theSchedule.TimeAdjustments.Select(adj => new TimeAdjustmentViewModel(RouteBeginTimes.First(rbt => rbt.RouteId == adj.RouteId && rbt.RouteVariant == adj.RouteVariantId && rbt.Id == adj.BeginTimeId && rbt.Days == adj.Days), TimeSpan.FromMinutes(adj.TimeAdjustmentMin), adj.StationId)));

                OnPropertyChanged(nameof(BusServices));
                OnPropertyChanged(nameof(BusStations));
            }
            catch(Exception exc)
            {
                var msg = exc.Message;
            }
        }

        internal string GetScheduleDataString()
        {
            var schedule = new ScheduleData();
            schedule.BusServices.AddRange(BusServices);
            schedule.BusStations.AddRange(BusStations);
            schedule.Routes.AddRange(Routes);
            schedule.RoutesDetails.AddRange(GetRoutesDetails(RouteDetails));
            schedule.RoutesBeginTimes.AddRange(RouteBeginTimes);
            schedule.TimeAdjustments.AddRange(TimeAdjustments.Select(item => new StationTimeAdjustment
                {
                    BeginTimeId = item.RouteBeginTime.Id,
                    RouteId = item.RouteBeginTime.RouteId,
                    RouteVariantId = item.RouteBeginTime.RouteVariant,
                    StationId = item.StationId,
                    TimeAdjustmentMin = (int)item.TimeAdjustment.TotalMinutes,
                    Days = item.RouteBeginTime.Days
                }));
            return JsonConvert.SerializeObject(schedule);
        }

        private IEnumerable<BusRouteDetails> GetRoutesDetails(List<RouteStationViewModel> routeDetails)
        {
            return routeDetails.Select(rd => new BusRouteDetails { BusRouteId = rd.RouteId, BusStopId = rd.BusStation.Id, TimeDiff = rd.TimeDiff, OrderNum = rd.OrderNum, RouteVariant = rd.RouteVariantId });
        }

        internal void AddBusService(string name, int id)
        {
            _theSchedule.BusServices.Add(new BusService { Id = id, Name = name });
            OnPropertyChanged(nameof(BusServices));
        }

        internal void RemoveBusService(BusService busServiceToRemove)
        {
            _theSchedule.BusServices.Remove(busServiceToRemove);
            OnPropertyChanged(nameof(BusServices));
        }

        internal bool AddBusStation(BusStation station)
        {
            if(BusStations.FirstOrDefault(s => s.Name == station.Name) != null)
            {
                return false;
            }
            BusStations.Add(station);
            OnPropertyChanged(nameof(BusStations));
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void RemoveBusStation(BusStation busStation)
        {
            throw new NotImplementedException();
        }

        internal void OnBusServiceChanged(int busServiceId)
        {
            RoutesForService = Routes.Where(route => route.BusServiceId == busServiceId).ToList();
            OnPropertyChanged(nameof(RoutesForService));
        }

        internal void AddRoute(BusRoute route)
        {
            route.Id = Routes.Count;
            Routes.Add(route);
            OnPropertyChanged(nameof(Routes));
        }

        internal void OnRouteChanged(BusRoute selectedRoute, RouteBeginTime.ScheduleDays scheduleDays, int routeVariant)
        {
            RouteDetailsForRoute = RouteDetails.Where(route => route.RouteId == selectedRoute.Id && route.RouteVariantId == routeVariant).ToList();
            BeginTimesForRoute = RouteBeginTimes.Where(beginTime => beginTime.RouteId == selectedRoute.Id && beginTime.Days == scheduleDays && beginTime.RouteVariant == routeVariant).OrderBy(beginTime => beginTime.Time).ToList();
            RouteVariants = Enumerable.Range(0, selectedRoute.VariantsNum).ToList();

            OnPropertyChanged(nameof(RouteDetailsForRoute));
            OnPropertyChanged(nameof(BeginTimesForRoute));
            OnPropertyChanged(nameof(RouteVariants));
        }

        internal void AddRouteDetails(List<RouteStationViewModel> routeDetails)
        {
            RouteDetails.RemoveAll(item => item.RouteId == routeDetails[0].RouteId && item.RouteVariantId == routeDetails[0].RouteVariantId);
            RouteDetails.AddRange(routeDetails);
            RouteDetailsForRoute = routeDetails;
            OnPropertyChanged(nameof(RouteDetailsForRoute));
        }

        internal TimeSpan GetTimeShiftForStation(BusRoute route, RouteStationViewModel routeStationView)
        {
            var timespan = TimeSpan.Zero;
            foreach (var station in RouteDetailsForRoute)
            {
                timespan += TimeSpan.FromMinutes(station.TimeDiff);
                if (station.BusStation.Id == routeStationView.BusStation.Id)
                {
                    break;
                }
            }

            return timespan;
        }

        internal void UpdateTimeAdjustments(IEnumerable<TimeAdjustmentViewModel> timeAdjustments)
        {
            var first = timeAdjustments.First();
            TimeAdjustments.RemoveAll(item => item.RouteBeginTime.RouteId == first.RouteBeginTime.RouteId && item.RouteBeginTime.Id == first.RouteBeginTime.Id && item.RouteBeginTime.RouteVariant == first.RouteBeginTime.RouteVariant && item.StationId == first.StationId);
            TimeAdjustments.AddRange(timeAdjustments);
            TimeAdjustmentsForSelection = timeAdjustments.ToList();
            OnPropertyChanged(nameof(TimeAdjustmentsForSelection));
        }

        internal void AddRouteBeginTimes(IEnumerable<RouteBeginTime> beginTimes)
        {
            var first = beginTimes.First();
            RouteBeginTimes.RemoveAll(item => item.RouteId == first.RouteId && item.RouteVariant == first.RouteVariant && item.Days == first.Days);
            RouteBeginTimes.AddRange(beginTimes);
            BeginTimesForRoute = beginTimes.ToList();
            OnPropertyChanged(nameof(BeginTimesForRoute));
        }

        internal void OnRouteStationChanged(BusRoute route, int routeVariant, RouteBeginTime.ScheduleDays scheduleDays, RouteStationViewModel stationViewModel)
        {
            TimeAdjustmentsForSelection = TimeAdjustments.Where(item => item.RouteBeginTime.RouteId == route.Id && item.RouteBeginTime.RouteVariant == routeVariant && item.RouteBeginTime.Days == scheduleDays && item.StationId == stationViewModel.BusStation.Id).ToList();
            OnPropertyChanged(nameof(TimeAdjustmentsForSelection));
        }
    }
}
