using BusSchedule.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusSchedule.Core.Interfaces
{
    public interface IFavoritesManager
    {
        void Add(string route_Id, Stops station, int? direction);
        IList<string> GetAll();
    }
}
