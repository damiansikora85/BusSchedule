using BusSchedule.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Core.UI.Components
{
    public class FavoriteData
    {
        public Model.Routes Route { get; }
        public Model.Stops Stop { get; }
        public int Direction { get; }
        public string DestinationName { get; }

        public FavoriteData(Model.Routes route, Model.Stops stop, int direction, string destinantionName)
        {
            Route = route;
            Stop = stop;
            Direction = direction;
            DestinationName = destinantionName;
        }

        public static async Task<FavoriteData> Create(string data, IDataProvider dataProvider)
        {
            var values = data.Split('|');
            var routeId = values[0];
            var direction = int.Parse(values[1]);
            var stopId = values[2];

            var route = await dataProvider.GetRoute(routeId);
            var destinations = await dataProvider.GetRouteDestinations(route);
            var stop = await dataProvider.GetStopById(stopId);

            var destination = direction == 0 ? destinations.Outbound : destinations.Inbound;
            return new FavoriteData(route, stop, direction, destination);
        }

        public override string ToString()
        {
            return $"{Route.Route_Short_Name} - {DestinationName} - {Stop.Stop_Name}";
        }
    }
}
