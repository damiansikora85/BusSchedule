using BusSchedule.Core.Interfaces;
using BusSchedule.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using TinyIoC;

namespace BusSchedule.Interfaces.Implementation
{
    public class FavoritesManager : IFavoritesManager
    {
        private IPreferences _preferences;

        public FavoritesManager(IPreferences preferences)
        {
            _preferences = preferences;
        }

        public void Add(string route_Id, Stops station, int? direction)
        {
            var favoritesSavedCount = _preferences.Get("favorites_count", 0);
            var favoriteData = $"{route_Id}|{direction}|{station.Stop_Id}";
            _preferences.Set($"favorite_{favoritesSavedCount}", favoriteData);
            _preferences.Set("favorites_count", ++favoritesSavedCount);
        }

        public IList<string> GetAll()
        {
            var favorites = new List<string>();
            var favoritesSavedCount = _preferences.Get("favorites_count", 0);
            for (int i = 0; i < favoritesSavedCount; i++)
            {
                var data = _preferences.Get($"favorite_{i}", "");
                if(!string.IsNullOrEmpty(data))
                {
                    favorites.Add(data);
                }
            }
            return favorites;
        }
    }
}
