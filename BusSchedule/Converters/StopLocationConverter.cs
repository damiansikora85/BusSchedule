using BusSchedule.Core.Model;
using BusSchedule.Core.UI.Utils;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace BusSchedule.Converters
{
    internal class StopLocationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Stops stop)
            {
                try
                {
                    return new Position(double.Parse(stop.Stop_Lat, CultureInfo.InvariantCulture), double.Parse(stop.Stop_Lon, CultureInfo.InvariantCulture));
                }
                catch (FormatException ex)
                {
                    throw new FormatException($"Error: {stop.Stop_Lat} {stop.Stop_Lon}");
                }
            }

            return new Position();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
