using BusSchedule.Core.Model;
using System.Globalization;

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
                    return new Location(double.Parse(stop.Stop_Lat, CultureInfo.InvariantCulture), double.Parse(stop.Stop_Lon, CultureInfo.InvariantCulture));
                }
                catch (FormatException ex)
                {
                    throw new FormatException($"Error: {stop.Stop_Lat} {stop.Stop_Lon}");
                }
            }

            return new Location();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
