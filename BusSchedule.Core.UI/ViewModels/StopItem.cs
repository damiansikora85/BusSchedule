using BusSchedule.Core.Model;

namespace BusSchedule.UI.ViewModels
{
    // Wrapper used by the UI. Keeps optional flag only here while Stop data remains unchanged.
    public class StopItem
    {
        public Stops Stop { get; set; }

        // True if this stop is optional for the current route/direction
        public bool IsOptional { get; set; }
    }
}