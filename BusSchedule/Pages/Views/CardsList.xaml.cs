﻿using BusSchedule.Core.UI.Pages.Views;
using BusSchedule.Interfaces.Implementation;
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
        //1270567785
        //2892283797

        public CardsList()
        {
            InitializeComponent();
            _viewModel = new CardsListViewModel(new CardsManager());
            BindingContext = _viewModel;
        }

        public async void OnAddCard(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddCardPage());
        }

        public async Task RefreshView()
        {
            await _viewModel.RefreshCards();
        }

        private async void OnCardSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (_viewModel.SelectedCard != null)
            {
                await Navigation.PushAsync(new CardDetailsPage(_viewModel.SelectedCard));
            }
        }
    }
}