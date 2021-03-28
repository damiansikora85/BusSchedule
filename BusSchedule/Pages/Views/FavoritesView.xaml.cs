using BusSchedule.Core.UI.Pages.Views;
using BusSchedule.Core.Utils;
using BusSchedule.Interfaces;
using BusSchedule.Interfaces.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyIoC;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BusSchedule.Pages.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FavoritesView : ContentView
    {
        private FavoritesViewModel _viewModel;

        public FavoritesView()
        {
            InitializeComponent();
            _viewModel = new FavoritesViewModel(new FavoritesManager(TinyIoCContainer.Current.Resolve<IPreferences>()), TinyIoCContainer.Current.Resolve<IDataProvider>());
            ListView.ItemsSource = _viewModel.Favorites;
        }

        public async Task RefreshView()
        {
            await _viewModel.RefreshData();
            ListView.IsVisible = _viewModel.HasAnyFavorites;
            EmptyListLabel.IsVisible = _viewModel.HasNoFavorites;
        }
    }
}