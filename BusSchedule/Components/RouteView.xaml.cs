using BusSchedule.Components.ViewModels;
using BusSchedule.Core.Model;
using BusSchedule.Providers;
using BusSchedule.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BusSchedule.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RouteView : ContentView
    {
        public Action<Routes> OnServiceClicked;
        private RouteViewModel _viewModel;
        public RouteView(Routes route)
        {
            InitializeComponent();
            _viewModel = new RouteViewModel(route);
            BindingContext = _viewModel;
        }

        private void OnClick(Routes busService)
        {
            OnServiceClicked?.Invoke(busService);
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            OnServiceClicked?.Invoke(_viewModel.Route);
        }
    }
}