using BusSchedule.Components;
using BusSchedule.Pages.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BusSchedule
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BusServicesPage : ContentPage
    {
        private BusServicesPageViewModel _viewModel;
        public BusServicesPage()
        {
            InitializeComponent();
            _viewModel = new BusServicesPageViewModel();
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
                grid.Children.Add(item, col, row);
                col++;
                row = col == maxCol ? row + 1 : row;
                col %= maxCol;
            }
            base.OnAppearing();
        }
    }
}