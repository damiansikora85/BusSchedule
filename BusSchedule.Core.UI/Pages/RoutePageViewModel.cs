using BusSchedule.Core.GTFS;
using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.UI.ViewModels
{
    public class RoutePageViewModel : INotifyPropertyChanged
    {
        public List<Stops> Stations { get; private set; }
        public Routes Route { get; }
        public int Direction { get; }
        private IDataProvider _dataProvider;

        public event PropertyChangedEventHandler PropertyChanged;

        public RoutePageViewModel(Routes route, int direction, IDataProvider dataProvider)
        {
            Route = route;
            Direction = direction;
            _dataProvider = dataProvider;
        }

        public async Task RefreshDataAsync()
        {
            Stations = await _dataProvider.GetStopsForRoute(Route, Direction);
            Stations[^1].IsLast = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Stations)));
        }
    }
}
