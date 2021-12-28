using BusSchedule.Core.CloudService;
using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusSchedule.Core.Services
{
    public class NewsService : INewsService
    {
        private readonly ICloudService _cloudService;
        private readonly IDataProvider _dataProvider;
        public event EventHandler NewsUpdated;
        private const int NEWS_UPDATE_DAYS = 1;

        public NewsService(ICloudService cloudService, IDataProvider dataProvider)
        {
            _cloudService = cloudService;
            _dataProvider = dataProvider;
        }

        public async Task<IList<News>> GetNews()
        {
            return await _dataProvider.GetNews();
        }

        public async Task<bool> TryUpdateNews(DateTime lastNewsUpdateTime)
        {
            if ((DateTime.Now - lastNewsUpdateTime).TotalDays >= NEWS_UPDATE_DAYS)
            {
                await UpdateNews();
                return true;
            }
            return false;
        }

        private async Task UpdateNews()
        {
            var news = await _cloudService.GetNews();
            await _dataProvider.SaveNews(news);
            NewsUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
