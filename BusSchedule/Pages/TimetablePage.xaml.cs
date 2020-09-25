using BusSchedule.Pages.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BusSchedule.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimetablePage : ContentPage
    {
        private TimetableViewModel _viewModel;
        public TimetablePage(Core.Model.BusStation station)
        {
            _viewModel = new TimetableViewModel();
            InitializeComponent();
            BindingContext = _viewModel;
            Title = station.Name;
        }
    }
}