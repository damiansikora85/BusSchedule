using BusSchedule.Core.Interfaces;
using BusSchedule.Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BusSchedule.Interfaces.Implementation
{
    public class FavoritesManager : IFavoritesManager
    {
        private const string FILENAME = "favorites.json";
        public FavoritesManager()
        {

        }

        public void Add(string routeId, string stopId, int? direction)
        {
            var favoritesList = GetAll();
            var favorite = new FavoriteDescription
            {
                RouteId = routeId,
                StopId = stopId,
                Direction = direction ?? -1
            };
            favoritesList.Add(favorite);
            SaveFavoritesList(favoritesList);
        }

        private void SaveFavoritesList(IList<FavoriteDescription> favoritesList)
        {
            var jsonString = JsonConvert.SerializeObject(favoritesList);
            File.WriteAllText(GetFilename(), jsonString);
        }

        public IList<FavoriteDescription> GetAll()
        {
            var fileName = GetFilename();
            var favoritesList = new List<FavoriteDescription>();
            if (File.Exists(fileName))
            {
                var favoritesJsonString = File.ReadAllText(fileName);
                favoritesList = JsonConvert.DeserializeObject<List<FavoriteDescription>>(favoritesJsonString);
            }
            return favoritesList;
        }

        public void Delete(string routeId, string stopId)
        {
            var favoritesList = GetAll();
            var favoriteFound = favoritesList.FirstOrDefault(f => f.RouteId == routeId && f.StopId == stopId);
            if(favoriteFound != null)
            {
                favoritesList.Remove(favoriteFound);
            }
            SaveFavoritesList(favoritesList);
        }

        private string GetFilename()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), FILENAME); ;
        }

        public bool IsOnList(string routeId, string stopId)
        {
            var favoritesList = GetAll();
            var favoriteFound = favoritesList.FirstOrDefault(f => f.RouteId == routeId && f.StopId == stopId);
            return favoriteFound != null;
        }
    }
}
