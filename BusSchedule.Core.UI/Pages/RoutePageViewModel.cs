using BusSchedule.Core.Model;
using BusSchedule.Core.UI.ViewModels;
using BusSchedule.Core.Utils;
using System.ComponentModel;
using System.Globalization;
using Point = BusSchedule.Core.Model.Point;

namespace BusSchedule.UI.ViewModels
{
    public class RoutePageViewModel : INotifyPropertyChanged
    {
        public IList<RouteStopViewModel> RouteStops { get; private set; }
        public Routes Route { get; }
        public int? Direction { get; }
        private readonly IDataProvider _dataProvider;
        private IList<Trace> _trace;
        public IList<Trace> Traces => _trace;


        public event PropertyChangedEventHandler PropertyChanged;

        public RoutePageViewModel(Routes route, int? direction, IDataProvider dataProvider)
        {
            Route = route;
            Direction = direction;
            _dataProvider = dataProvider;
            RouteStops = new List<RouteStopViewModel>();
        }

        public async Task RefreshDataAsync()
        {
            var stops = Direction.HasValue ? await _dataProvider.GetStopsForRoute(Route, Direction.Value) :
                await _dataProvider.GetStopsForRoute(Route);
            RouteStops = stops.Select((s, i) => new RouteStopViewModel(s, i == 0, i == (stops.Count-1), i < 3)).ToList();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RouteStops)));

            _trace = await _dataProvider.GetRouteTrace(Route.Route_Short_Name, Direction);
        }

        public Point CalculateCenterPosition()
        {
            double avarageLat = 0;
            double avarageLon = 0;
            foreach(var stop in RouteStops)
            {
                avarageLat += stop.StopLocation.Latitude;
                avarageLon += stop.StopLocation.Longitude;
            }
            return new Point(avarageLat / RouteStops.Count, avarageLon / RouteStops.Count);
        }
    }
}
