using BusSchedule.Components;
using BusSchedule.Core.Utils;
using BusSchedule.Dialogs;
using BusSchedule.Pages.ViewModels;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using TinyIoC;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BusSchedule.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutesPage : ContentPage
    {
        private RoutesPageViewModel _viewModel;
        public RoutesPage()
        {
            InitializeComponent();
            _viewModel = new RoutesPageViewModel(TinyIoCContainer.Current.Resolve<IDataProvider>());
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            await _viewModel.RefreshBusServicesAsync();
            int row = 0, col = 0;
            int maxCol = grid.ColumnDefinitions.Count;
            foreach (var busService in _viewModel.Routes)
            {
                var item = new RouteView(busService);
                item.OnServiceClicked += OnBusServiceSelected;
                grid.Children.Add(item, col, row);
                col++;
                row = col == maxCol ? row + 1 : row;
                col %= maxCol;
            }
            base.OnAppearing();
        }

        private async void OnBusServiceSelected(Core.Model.Routes route)
        {
            //Task.Run(async () => await Task.Delay(500));
            //await Navigation.PushAsync(new ThreadsTestPage());
            var destination = await _viewModel.GetDestinationsForRoute(route);//await _viewModel.GetRoutesForServiceAsync(busService);
            var dialog = new RouteSelectionDialog(destination);
            await PopupNavigation.Instance.PushAsync(dialog);
            var selectedDirection = await dialog.WaitForResult();
            await Navigation.PushAsync(new RoutePage(route, selectedDirection == 0 ? destination.Outbound : destination.Inbound, selectedDirection));
        }

        private IList<string> GetRoutesForService(Core.Model.Routes busService)
        {
            throw new NotImplementedException();
        }
    }
}