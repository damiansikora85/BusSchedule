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
        public List<RouteStationViewModel> RouteDetails { get; private set; }
        public List<RouteStationViewModel> RouteDetailsForRoute { get; private set; }
        public List<RouteBeginTime> RouteBeginTimes { get; private set; }
        public List<RouteBeginTime> BeginTimesForRoute { get; private set; }

        public MainWindowViewModel()
        {
            _theSchedule = new ScheduleData();
            BusStations = new ObservableCollection<BusStation>();
            BusServices = new ObservableCollection<BusService>();
            Routes = new List<BusRoute>();
            RoutesForService = new List<BusRoute>();
            RouteDetails = new List<RouteStationViewModel>();
            RouteBeginTimes = new List<RouteBeginTime>();
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
                RouteDetails.AddRange(_theSchedule.RoutesDetails.Select(rd => new RouteStationViewModel { RouteId = rd.BusRouteId, BusStation = BusStations.FirstOrDefault(BusStopSchedule => BusStopSchedule.Id == rd.BusStopId), OrderNum = rd.OrderNum, TimeDiff = rd.TimeDiff }));

                RouteBeginTimes.Clear();
                RouteBeginTimes.AddRange(_theSchedule.RoutesBeginTimes);

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
            return JsonConvert.SerializeObject(schedule);
        }

        private IEnumerable<BusRouteDetails> GetRoutesDetails(List<RouteStationViewModel> routeDetails)
        {
            return routeDetails.Select(rd => new BusRouteDetails { BusRouteId = rd.RouteId, BusStopId = rd.BusStation.Id, TimeDiff = rd.TimeDiff, OrderNum = rd.OrderNum });
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

        internal void OnRouteChanged(BusRoute selectedRoute, RouteBeginTime.ScheduleDays scheduleDays)
        {
            RouteDetailsForRoute = RouteDetails.Where(route => route.RouteId == selectedRoute.Id).ToList();
            BeginTimesForRoute = RouteBeginTimes.Where(beginTime => beginTime.RouteId == selectedRoute.Id && beginTime.Days == scheduleDays).ToList();
            OnPropertyChanged(nameof(RouteDetailsForRoute));
            OnPropertyChanged(nameof(BeginTimesForRoute));
        }

        internal void AddRouteDetails(List<RouteStationViewModel> routeDetails)
        {
            RouteDetails.AddRange(routeDetails);
            RouteDetailsForRoute = routeDetails;
            OnPropertyChanged(nameof(RouteDetailsForRoute));
        }

        internal void AddRouteBeginTimes(IEnumerable<RouteBeginTime> beginTimes)
        {
            RouteBeginTimes.AddRange(beginTimes);
            BeginTimesForRoute = beginTimes.ToList();
            OnPropertyChanged(nameof(BeginTimesForRoute));
        }

        internal void OnScheduleDaysChanged(int routeId, RouteBeginTime.ScheduleDays scheduleDays)
        {
            BeginTimesForRoute = RouteBeginTimes.Where(beginTime => beginTime.RouteId == routeId && beginTime.Days == scheduleDays).ToList();
            OnPropertyChanged(nameof(BeginTimesForRoute));
        }
    }
}
