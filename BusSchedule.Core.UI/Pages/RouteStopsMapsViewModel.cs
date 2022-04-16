using BusSchedule.Core.UI.Utils;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.UI.Pages
{
    public class RouteStopsMapsViewModel : BaseViewModel
    {
        public IList<StopLocation> Locations { get; }
    }
}
