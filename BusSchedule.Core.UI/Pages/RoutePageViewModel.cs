using BusSchedule.Core.Model;
using BusSchedule.Core.Services;
using BusSchedule.Core.UI.Utils;
using BusSchedule.Core.Utils;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BusSchedule.UI.ViewModels
{
    public class RoutePageViewModel : INotifyPropertyChanged
    {
        public IList<Stops> Stops { get; private set; }
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
        }

        public async Task RefreshDataAsync()
        {
            Stops = Direction.HasValue ? await _dataProvider.GetStopsForRoute(Route, Direction.Value) :
                await _dataProvider.GetStopsForRoute(Route);
            Stops[^1].IsLast = true;
            Stops[0].IsFirst = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Stops)));

            _trace = await _dataProvider.GetRouteTrace(Route.Route_Short_Name, Direction);
        }

        public Point CalculateCenterPosition()
        {
            double avarageLat = 0;
            double avarageLon = 0;
            foreach(var stop in Stops)
            {
                avarageLat += double.Parse(stop.Stop_Lat, CultureInfo.InvariantCulture);
                avarageLon += double.Parse(stop.Stop_Lon, CultureInfo.InvariantCulture);
            }
            return new Point(avarageLat / Stops.Count, avarageLon / Stops.Count);
        }
    }
}
