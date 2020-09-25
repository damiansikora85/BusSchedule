using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Pages.ViewModels
{
    public class RoutePageViewModel : INotifyPropertyChanged
    {
        public List<BusStation> Stations { get; private set; }
        private BusRoute _route;
        private IDataProvider _dataProvider;

        public event PropertyChangedEventHandler PropertyChanged;

        public RoutePageViewModel(BusRoute route, IDataProvider dataProvider)
        {
            _route = route;
            _dataProvider = dataProvider;
        }

        public async Task RefreshDataAsync()
        {
            Stations = await _dataProvider.GetStationsForRoute(_route);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Stations)));
        }
    }
}
