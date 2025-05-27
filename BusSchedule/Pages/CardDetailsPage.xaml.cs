using Acr.UserDialogs;
using BusSchedule.Core.Model;
using BusSchedule.Core.Services;
using BusSchedule.Core.UI.Pages;
using TinyIoC;

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
                TinyIoCContainer.Current.Resolve<IAnalyticsService>().LogException(exc);
#if ANDROID
                UserDialogs.Instance.Toast(new ToastConfig("Wystąpił błąd podczas pobierania danych karty") { MessageTextColor = System.Drawing.Color.Red });
#endif
                await Navigation.PopAsync();
            }
        }
    }
}