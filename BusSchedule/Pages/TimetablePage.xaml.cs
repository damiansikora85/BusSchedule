using BusSchedule.Core.Utils;
using BusSchedule.Pages.ViewModels;
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
    public partial class TimetablePage : ContentPage
    {
        private TimetableViewModel _viewModel;
        public TimetablePage(Core.Model.BusStation station, Core.Model.BusRoute route)
        {
            _viewModel = new TimetableViewModel(station, route, TinyIoCContainer.Current.Resolve<IDataProvider>());
            InitializeComponent();
            BindingContext = _viewModel;
            Title = station.Name;
        }

        protected override async void OnAppearing()
        {
            await _viewModel.RefreshTimetableAsync();
            base.OnAppearing();
        }
    }
}