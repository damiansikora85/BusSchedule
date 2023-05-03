using BusSchedule.Core.UI.Pages.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BusSchedule.Pages.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EcardView : ContentView
    {
        private readonly EcardViewModel _viewModel;
        public EcardView()
        {
            InitializeComponent();
            _viewModel = new EcardViewModel();
            BindingContext = _viewModel;
        }
    }
}