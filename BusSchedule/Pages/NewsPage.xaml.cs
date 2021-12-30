using BusSchedule.Core.Services;
using BusSchedule.Core.UI.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BusSchedule.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewsPage : ContentPage
    {
        private NewsPageViewModel _viewModel;
        public NewsPage(INewsService newsService) 
        {
            _viewModel = new NewsPageViewModel(newsService);
            BindingContext  = _viewModel;
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await _viewModel.RefreshView();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(sender is ListView listView)
            {
                listView.SelectedItem = null;
            }
        }
    }
}