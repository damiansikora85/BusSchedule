using BusSchedule.Components;
using BusSchedule.Core.Utils;
using BusSchedule.Dialogs;
using BusSchedule.Pages.ViewModels;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyIoC;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BusSchedule.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BusServicesPage : ContentPage
    {
        private BusServicesPageViewModel _viewModel;
        public BusServicesPage()
        {
            InitializeComponent();
            _viewModel = new BusServicesPageViewModel(TinyIoCContainer.Current.Resolve<IDataProvider>());
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            await _viewModel.RefreshBusServicesAsync();
            int row = 0, col = 0;
            int maxCol = grid.ColumnDefinitions.Count;
            foreach (var busService in _viewModel.BusServices)
            {
                var item = new BusServiceView(busService);
                item.OnServiceClicked += OnBusServiceSelected;
                grid.Children.Add(item, col, row);
                col++;
                row = col == maxCol ? row + 1 : row;
                col %= maxCol;
            }
            base.OnAppearing();
        }

        private async void OnBusServiceSelected(Core.Model.BusService busService)
        {
            var routes = await _viewModel.GetRoutesForServiceAsync(busService);
            var dialog = new RouteSelectionDialog(routes);
            await PopupNavigation.Instance.PushAsync(dialog);
            var route = await dialog.WaitForResult();
            await Navigation.PushAsync(new RoutePage(busService, route));
        }
    }
}