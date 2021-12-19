using BusSchedule.Core.CloudService;
using BusSchedule.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusSchedule.Core.Services
{
    public class NewsService : INewsService
    {
        private ICloudService _cloudService;
        public event EventHandler NewsUpdated;
        public int NewsCount => _news.Count;
        private IList<News> _news;

        public NewsService(ICloudService cloudService)
        {
            _cloudService = cloudService;
            _news = new List<News>();
        }

        public async Task UpdateNews()
        {
            _news = await _cloudService.GetNews();
            NewsUpdated?.Invoke(this, EventArgs.Empty);
        }

        public IList<News> GetNews()
        {
            return Enumerable.Empty<News>().ToList();
        }
    }
}
