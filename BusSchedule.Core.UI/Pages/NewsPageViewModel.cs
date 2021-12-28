using BusSchedule.Core.Model;
using BusSchedule.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Core.UI.Pages
{
    public class NewsPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IList<News> News { get; private set; }
        private readonly INewsService _newsService;

        public NewsPageViewModel(INewsService newsService)
        {
            News = new List<News>();
            _newsService = newsService;
        }

        public async Task RefreshView()
        {
            //News = await _newsService.GetNews();
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(News)));
        }
    }
}
