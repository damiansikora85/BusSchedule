using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.UI.ViewModels
{
    public class RoutePageViewModel : INotifyPropertyChanged
    {
        public List<BusStation> Stations { get; private set; }
        public BusRoute Route { get; }
        private IDataProvider _dataProvider;

        public event PropertyChangedEventHandler PropertyChanged;

        public RoutePageViewModel(BusRoute route, IDataProvider dataProvider)
        {
            Route = route;
            _dataProvider = dataProvider;
        }

        public async Task RefreshDataAsync()
        {
            Stations = await _dataProvider.GetStationsForRoute(Route);
            Stations[Stations.Count - 1].IsLast = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Stations)));
        }
    }
}
