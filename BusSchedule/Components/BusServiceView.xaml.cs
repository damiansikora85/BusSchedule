using BusSchedule.Components.ViewModels;
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
        private BusServiceViewModel _viewModel;
        public BusServiceView(Core.Model.BusService busService)
        {
            InitializeComponent();
            _viewModel = new BusServiceViewModel(busService);
            BindingContext = _viewModel;
        }
    }
}