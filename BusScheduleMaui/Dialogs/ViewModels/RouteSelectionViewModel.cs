using BusSchedule.Core.Model;

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
