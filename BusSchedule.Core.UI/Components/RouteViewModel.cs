using BusSchedule.Core.Model;
using System;
using System.Drawing;
using System.Windows.Input;

namespace BusSchedule.Components.ViewModels
{
    public class RouteViewModel
    {
        public string Name => Route.Route_Short_Name;
        public Action<Routes> OnClick;
        public Routes Route { get; }
        public Color RouteColor { get; }
        public Color RouteTextColor { get; }

        public RouteViewModel(Routes busService)
        {
            Route = busService;
            RouteColor = GetColorFromString(Route.Route_Color);
            RouteTextColor = GetColorFromString(Route.Route_Text_Color);
        }

        private Color GetColorFromString(string colorString)
        {
            var red = StringToHex(colorString.Substring(0, 2));
            var green = StringToHex(colorString.Substring(2, 2));
            var blue = StringToHex(colorString.Substring(4, 2));
            return Color.FromArgb(red, green, blue);
        }

        private int StringToHex(string hex)
        {
            return int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }
    }
}
