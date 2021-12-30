using BusSchedule.Core.Model;
using BusSchedule.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Core.UI.Pages
{
    public class NewsPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IList<News> News { get; private set; }
        public bool HasAnyNews => News.Any();
        private readonly INewsService _newsService;

        public NewsPageViewModel(INewsService newsService)
        {
            News = new List<News>();
            _newsService = newsService;
        }

        public async Task RefreshView()
        {
#if DEBUG
            News = await _newsService.GetNews(false);
#else
            News = await _newsService.GetNews();
#endif
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(News)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasAnyNews)));
        }
    }
}
