using BusSchedule.Core.Model;
using BusSchedule.Core.UI.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BusSchedule.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CardDetailsPage : ContentPage
    {
        private readonly CardDetailsPageViewModel _viewModel;
        public CardDetailsPage(ElectronicCardData cardData)
        {
            _viewModel = new CardDetailsPageViewModel(cardData);
            BindingContext = _viewModel;
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            await _viewModel.RefreshData();
        }
    }
}