using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Dialogs.ViewModels
{
    public class RouteSelectionViewModel
    {
        public List<BusRoute> Routes { get; }
        public string FirstRouteName { get; }
        public string SecondRouteName { get; }
        public BusRoute FirstRoute => Routes[0];
        public BusRoute SecondRoute => Routes[1];

        public RouteSelectionViewModel(List<BusRoute> routes)
        {
            if(routes.Count < 2)
            {
                return;
            }
            Routes = routes;
            FirstRouteName = Routes[0].Name;
            SecondRouteName = Routes[1].Name;
        }
    }
}
