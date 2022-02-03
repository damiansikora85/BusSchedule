using BusSchedule.Core.Model;
using System;

namespace BusSchedule.Core.Exceptions
{
    public class FavoriteCreateException : Exception
    {
        public string RouteId { get; }
        public string StopId { get; }
        public int Direction { get; }

        public FavoriteCreateException(FavoriteDescription data)
        {
            RouteId = data.RouteId;
            StopId = data.StopId;
            Direction = data.Direction;
        }
    }
}
