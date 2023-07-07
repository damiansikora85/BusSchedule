using Acr.UserDialogs;
using BusSchedule.Core.Model;
using BusSchedule.Core.UI.Pages;
using Microsoft.AppCenter.Crashes;
using System;
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
            try
            {
                await _viewModel.RefreshData();
            }
            catch(Exception exc)
            {
                Crashes.TrackError(exc);
                UserDialogs.Instance.Toast(new ToastConfig("Wystąpił błąd podczas pobierania danych karty") { MessageTextColor = Color.Red });
                await Navigation.PopAsync();
            }
        }
    }
}