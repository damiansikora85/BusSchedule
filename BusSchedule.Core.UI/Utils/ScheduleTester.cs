using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Core.UI.Utils
{
    public class ScheduleTester
    {
        private IDataProvider _dataProvider;

        public ScheduleTester(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task CheckRouteStopsValid()
        {
            //var routes = await _dataProvider.GetRoutes();
            //foreach (var route in routes)
            //{
            //    var stops = await _dataProvider.GetStopsForRoute(route, 0);
            //    foreach (var stop in stops)
            //    {
            //        var stop
            //    }
            //}
        }
    }
}
