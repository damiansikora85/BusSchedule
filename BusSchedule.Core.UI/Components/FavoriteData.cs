using BusSchedule.Core.Exceptions;
using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using System.Threading.Tasks;

namespace BusSchedule.Core.UI.Components
{
    public class FavoriteData
    {
        public Routes Route { get; }
        public Stops Stop { get; }
        public int? Direction { get; }
        public string DestinationName { get; }

        public FavoriteData(Routes route, Stops stop, string destinantionName)
        {
            Route = route;
            Stop = stop;
            DestinationName = destinantionName;
        }

        public FavoriteData(Routes route, Stops stop, int direction, string destinantionName)
        {
            Route = route;
            Stop = stop;
            Direction = direction;
            DestinationName = destinantionName;
        }

        public static async Task<FavoriteData> Create(FavoriteDescription data, IDataProvider dataProvider)
        {
            var stop = await dataProvider.GetStopById(data.StopId);
            if(stop == null)
            {
                throw new FavoriteCreateException(data);
            }
            var route = await dataProvider.GetRoute(data.RouteId);
            if(route == null)
            {
                throw new FavoriteCreateException(data);
            }
            var destinations = await dataProvider.GetRouteDestinations(route);

            var destination = data.Direction == 0 ? destinations.Outbound : destinations.Inbound;
            return data.Direction < 0 ? new FavoriteData(route, stop, destination) : new FavoriteData(route, stop, data.Direction, destination);
        }

        public override string ToString()
        {
            return $"{Route.Route_Short_Name} - {DestinationName} - {Stop.Stop_Name}";
        }
    }
}
