using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace BusSchedule.Pages.ViewModels
{
    public class RoutesPageViewModel
    {
        private IDataProvider _dataProvider;
        public List<Routes> Routes { get; private set; }

        public RoutesPageViewModel(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task RefreshBusServicesAsync()
        {
            Routes = await _dataProvider.GetRoutes();
        }

        public Task<Destination> GetDestinationsForRoute(Routes route)
        {
            return _dataProvider.GetRouteDestinations(route);
        }
    }
}
