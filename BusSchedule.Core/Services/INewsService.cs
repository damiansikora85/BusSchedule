using BusSchedule.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace BusSchedule.Core.Services
{
    public interface INewsService
    {
        public event EventHandler NewsUpdated;

        public Task<IList<News>> GetNews(bool showOnly = true);
        Task<bool> TryUpdateNews(DateTime lastNewsUpdateTime);
    }
}