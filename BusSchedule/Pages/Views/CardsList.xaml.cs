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
    public partial class CardsList : ContentView
    {
        private readonly CardsListViewModel _viewModel;

        public CardsList()
        {
            InitializeComponent();
            _viewModel = new CardsListViewModel();
            BindingContext = _viewModel;
        }
    }
}