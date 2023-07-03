using Acr.UserDialogs;
using BusSchedule.Core.UI.Interfaces;
using BusSchedule.Core.UI.Pages;
using BusSchedule.Interfaces.Implementation;
using Microsoft.AppCenter.Crashes;
using System;
using TinyIoC;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

        private async void OnAddCardClicked(object sender, EventArgs e)
        {
			try
			{
				var name = await DisplayPromptAsync("Dodaj karte", "Podaj nazwe karty", initialValue: _viewModel.CardNumber);
				await _viewModel.SaveCard(name);
				Microsoft.AppCenter.Analytics.Analytics.TrackEvent("CardAdded");
			}
			catch (Exception ex) 
			{
				Crashes.TrackError(ex);
				UserDialogs.Instance.Toast(new ToastConfig("Wystąpił błąd podczas zapisywania") { MessageTextColor = System.Drawing.Color.Red });
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
			catch (Exception ex) 
			{
				Crashes.TrackError(ex);
				UserDialogs.Instance.Toast(new ToastConfig("Wystąpił problem podczas wyszukiwania karty") { MessageTextColor = System.Drawing.Color.Red });
			}
        }
    }
}