using Acr.UserDialogs;
using BusSchedule.Core.Exceptions;
using BusSchedule.Core.UI.Pages;
using BusSchedule.Interfaces.Implementation;
using Microsoft.AppCenter.Crashes;

namespace BusSchedule.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddCardPage : ContentPage
	{
		private readonly AddCardViewModel _viewModel;
		public AddCardPage ()
		{
			_viewModel = new AddCardViewModel(new CardsManager());
			BindingContext = _viewModel;
			InitializeComponent ();
		}

        protected override void OnAppearing()
        {
            Shell.SetTabBarIsVisible(this, false);
            base.OnAppearing();
        }

        private async void OnAddCardClicked(object sender, EventArgs e)
        {
			try
			{
				var name = await DisplayPromptAsync("Dodaj kartę", "Podaj nazwę karty", initialValue: _viewModel.CardNumber);
				await _viewModel.SaveCard(name);
				Microsoft.AppCenter.Analytics.Analytics.TrackEvent("CardAdded");
			}
			catch (Exception ex) 
			{
				Crashes.TrackError(ex);
#if ANDROID
				UserDialogs.Instance.Toast(new ToastConfig("Wystąpił błąd podczas zapisywania") { MessageTextColor = System.Drawing.Color.Red });
#endif
			}
			finally
			{
                await Navigation.PopAsync();
            }
        }

        private async void OnSearchCardClicked(object sender, EventArgs e)
        {
			try
			{
				await _viewModel.SearchCard();
			}
			catch(ElectronicCardException cardException)
			{
                Crashes.TrackError(cardException, new Dictionary<string, string>
                {
                    { "cardNum", _viewModel.SearchCardNumber }
                });
#if ANDROID
                UserDialogs.Instance.Toast(new ToastConfig($"Nie znaleziono karty o numerze: {_viewModel.SearchCardNumber}") { MessageTextColor = System.Drawing.Color.Red });
#endif
            }
			catch (Exception ex) 
			{
				Crashes.TrackError(ex, new Dictionary<string, string>
				{
					{ "cardNum", _viewModel.SearchCardNumber }
				});
#if ANDROID
				UserDialogs.Instance.Toast(new ToastConfig("Wystąpił problem podczas wyszukiwania karty") { MessageTextColor = System.Drawing.Color.Red });
#endif
			}
        }
    }
}