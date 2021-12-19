using BusSchedule.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace BusSchedule.Core.Services
{
    public interface INewsService
    {
        public event EventHandler NewsUpdated;
        public int NewsCount { get; }

        public Task UpdateNews();

        public IList<News> GetNews();
    }
}