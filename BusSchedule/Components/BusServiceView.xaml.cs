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
    public partial class BusServiceView : ContentView
    {
        public Action<BusService> OnServiceClicked;
        private BusServiceViewModel _viewModel;
        public BusServiceView(BusService busService)
        {
            InitializeComponent();
            _viewModel = new BusServiceViewModel(busService);
            _viewModel.OnClick += OnClick;
            BindingContext = _viewModel;
        }

        private void OnClick(BusService busService)
        {
            OnServiceClicked?.Invoke(busService);
        }
    }
}