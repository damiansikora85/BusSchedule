using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Pages.ViewModels
{
    public class RoutePageViewModel
    {
        public List<BusStation> Stations { get; private set; }
        private BusRoute _route;
        private IDataProvider _dataProvider;


        public RoutePageViewModel(BusRoute route, IDataProvider dataProvider)
        {
            _route = route;
            _dataProvider = dataProvider;
        }

        public async Task RefreshDataAsync()
        {
            Stations = await _dataProvider.GetStationsForRoute(_route);
        }
    }
}
