using BusSchedule.Core.Model;
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
        public ObservableCollection<BusRoute> Routes { get; private set; }

        public MainWindowViewModel()
        {
            _theSchedule = new ScheduleData();
            BusStations = new ObservableCollection<BusStation>();
            BusServices = new ObservableCollection<BusService>();
            Routes = new ObservableCollection<BusRoute>();
        }

        internal void Setup(string jsonData)
        {
            try
            {
                _theSchedule = JsonConvert.DeserializeObject<ScheduleData>(jsonData);
                foreach (var service in _theSchedule.BusServices)
                {
                    BusServices.Add(service);
                }

                foreach (var station in _theSchedule.BusStations)
                {
                    BusStations.Add(station);
                }
                foreach(var routes in _theSchedule.Routes)
                {
                    Routes.Add(routes);
                }
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
            return JsonConvert.SerializeObject(schedule);
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
            var routesForBusService = Routes.Where(route => route.BusServiceId == busServiceId);
        }

        internal void AddRoute(BusRoute route)
        {
            route.Id = Routes.Count;
            Routes.Add(route);
            OnPropertyChanged(nameof(Routes));
        }
    }
}
