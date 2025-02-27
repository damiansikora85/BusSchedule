using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Dialogs.ViewModels
{
    public class RouteSelectionViewModel
    {
        public string FirstDirection { get; }
        public string SecondDirection { get; }

        public RouteSelectionViewModel(Destination destination)
        {
            FirstDirection = destination.Outbound;
            SecondDirection = destination.Inbound;
        }
    }
}
