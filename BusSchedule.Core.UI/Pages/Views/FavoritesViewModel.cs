using BusSchedule.Core.Exceptions;
using BusSchedule.Core.Interfaces;
using BusSchedule.Core.UI.Components;
using BusSchedule.Core.Utils;
using Microsoft.AppCenter.Crashes;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusSchedule.Core.UI.Pages.Views
{
    public class FavoritesViewModel : BaseViewModel
    {
        private readonly IFavoritesManager _favoritesManager;
        private readonly IDataProvider _dataProvider;

        public ObservableRangeCollection<FavoriteData> Favorites { get; } = new ObservableRangeCollection<FavoriteData>();
        public bool HasAnyFavorites => Favorites.Any();
        public bool HasNoFavorites => !Favorites.Any();

        public FavoritesViewModel(IFavoritesManager favoritesManager, IDataProvider dataProvider)
        {
            _favoritesManager = favoritesManager;
            _dataProvider = dataProvider;
        }

        public async Task RefreshData()
        {
            Favorites.Clear();
            var favoritesList = _favoritesManager.GetAll();
            foreach(var favoriteData in favoritesList)
            {
                try
                {
                    var result = await FavoriteData.Create(favoriteData, _dataProvider);
                    Favorites.Add(result);
                }
                catch(FavoriteCreateException favoriteException)
                {
                    Crashes.TrackError(favoriteException, new Dictionary<string, string>
                    {
                        {"routeId", favoriteData.RouteId },
                        {"stopId", favoriteData.StopId }
                    });
                    _favoritesManager.Delete(favoriteData.RouteId, favoriteData.StopId);
                }
                catch (Exception exc)
                {
                    Crashes.TrackError(exc, new Dictionary<string, string>
                    {
                        {"routeId", favoriteData.RouteId },
                        {"stopId", favoriteData.StopId }
                    });
                }
            }
        }

        public void DeleteItem(FavoriteData favoriteData)
        {
            if(Favorites.Contains(favoriteData))
            {
                Favorites.Remove(favoriteData);
                _favoritesManager.Delete(favoriteData.Route.Route_Id, favoriteData.Stop.Stop_Id);
                OnPropertyChanged(nameof(HasAnyFavorites));
                OnPropertyChanged(nameof(HasNoFavorites));
            }
        }
    }
}
