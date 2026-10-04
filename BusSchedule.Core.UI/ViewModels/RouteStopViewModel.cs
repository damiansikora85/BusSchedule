using BusSchedule.Core.Model;
using System.Globalization;

namespace BusSchedule.Core.UI.ViewModels
{
    public class RouteStopViewModel
    {
        public enum RouteStopType
        {
            Default,
            Optional,
            OptionalFirst,
            OptionalLast
        }

        private Stops _stop;
        public bool IsFirst { get; }
        public bool IsLast { get; }
        public bool IsOptional { get; }
        public bool IsOnRequest { get; }

        public RouteStopType StopType { get; set; }

        public string Name => _stop.Stop_Name;
        public Location StopLocation { get; }

        public RouteStopViewModel(Stops stop, bool isFirst, bool isLast, Route_Stop.StopType type)
        {
            _stop = stop;
            IsFirst = isFirst;
            IsLast = isLast;
            var latitude = double.Parse(_stop.Stop_Lat, CultureInfo.InvariantCulture);
            var longitude = double.Parse(_stop.Stop_Lon, CultureInfo.InvariantCulture);
            StopLocation = new Location(latitude, longitude);
            IsOptional = type == Route_Stop.StopType.Optional;
            IsOnRequest = stop.Stop_Name.EndsWith("n/ż");
        }
    }
}
