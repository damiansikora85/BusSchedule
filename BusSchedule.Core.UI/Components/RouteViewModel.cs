using BusSchedule.Core.Model;
using System;
using System.Windows.Input;

namespace BusSchedule.Components.ViewModels
{
    public class RouteViewModel
    {
        public string Name => Route.Route_Short_Name;
        public Action<Routes> OnClick;
        public Routes Route { get; }

        public RouteViewModel(Routes busService)
        {
            Route = busService;
        }
    }
}
