using BusSchedule.Core.Exceptions;
using BusSchedule.Core.Interfaces;
using BusSchedule.Core.Services;
using BusSchedule.Core.UI.Components;
using BusSchedule.Core.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace BusSchedule.Core.UI.Pages.Views
{
    public partial class FavoritesViewModel : ObservableObject
    {
        private readonly IFavoritesManager _favoritesManager;
        private readonly IDataProvider _dataProvider;

        public ObservableCollection<FavoriteData> Favorites { get; } = new ObservableCollection<FavoriteData>();
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
                    _favoritesManager.Delete(favoriteData.RouteId, favoriteData.StopId);
                }
                catch (Exception exc)
                {
                    //TinyIoCContainer.Current.Resolve<IAnalyticsService>().LogException(favoriteException);
                    //Crashes.TrackError(exc, new Dictionary<string, string>
                    //{
                    //    {"routeId", favoriteData.RouteId },
                    //    {"stopId", favoriteData.StopId }
                    //});
                }
            }
        }

        public void DeleteItem(FavoriteData favoriteData)
        {
            if(Favorites.Remove(favoriteData))
            {
                _favoritesManager.Delete(favoriteData.Route.Route_Id, favoriteData.Stop.Stop_Id);
                OnPropertyChanged(nameof(HasAnyFavorites));
                OnPropertyChanged(nameof(HasNoFavorites));
            }
        }
    }
}
