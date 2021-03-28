using BusSchedule.Core.Interfaces;
using BusSchedule.Core.Utils;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BusSchedule.Core.UI.Pages.Views
{
    public class FavoritesViewModel : INotifyPropertyChanged
    {
        private readonly IFavoritesManager _favoritesManager;
        private readonly IDataProvider _dataProvider;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<string> Favorites { get; } = new List<string>();
        public bool HasAnyFavorites => Favorites.Any();
        public bool HasNoFavorites => !Favorites.Any();

        public FavoritesViewModel(IFavoritesManager favoritesManager, IDataProvider dataProvider)
        {
            _favoritesManager = favoritesManager;
            _dataProvider = dataProvider;
        }

        public async Task RefreshData()
        {
            var favoritesList = _favoritesManager.GetAll();
            var resultList = new List<string>();
            foreach(var favoriteData in favoritesList)
            {
                string result = await ParseData(favoriteData);
                resultList.Add(result);
            }
            Favorites.AddRange(resultList);


            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Favorites)));
        }

        private async Task<string> ParseData(string data)
        {
            var values = data.Split('|');
            var routeId = values[0];
            var direction = int.Parse(values[1]);
            var stopId = values[2];

            var route = await _dataProvider.GetRoute(routeId);
            var destinations = await _dataProvider.GetRouteDestinations(route);
            var stop = await _dataProvider.GetStopById(stopId);

            var destination = direction == 0 ? destinations.Outbound : destinations.Inbound;
            var result = $"{route.Route_Short_Name} - {destination} - {stop.Stop_Name}";
            return result;
        }
    }
}
