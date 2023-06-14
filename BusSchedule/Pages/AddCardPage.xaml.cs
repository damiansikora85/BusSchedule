using BusSchedule.Core.UI.Interfaces;
using BusSchedule.Core.UI.Pages;
using BusSchedule.Interfaces.Implementation;
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
			var name = await DisplayPromptAsync("Dodaj karte", "Podaj nazwe karty", initialValue: _viewModel.CardNumber);
			await _viewModel.SaveCard(name);
			await Navigation.PopAsync();
        }
    }
}