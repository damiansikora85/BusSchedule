using BusSchedule.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Interfaces
{
    public interface IFavoritesManager
    {
        void Add(string routeId, string stopId, int? direction);
        IList<FavoriteDescription> GetAll();
        void Delete(string routeId, string stopId);
        bool IsOnList(string routeId, string stopId);
    }
}
