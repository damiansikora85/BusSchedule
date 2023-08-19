using BusSchedule.Core.Model;
using BusSchedule.Core.UI.Helpers;
using BusSchedule.Core.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BusSchedule.Core.UI.Pages.Views
{
    public class CardsListViewModel : BaseViewModel
    {
        public IList<ElectronicCardData> Cards { get; private set; }
        public bool HasCards => Cards.Any();
        public ICommand AddCardCommand { get; private set; }
        public ElectronicCardData SelectedCard { get; set; }

        private readonly ICardsManager _cardsManager;

        public CardsListViewModel(ICardsManager cardsManager)
        {
            _cardsManager = cardsManager;
            Cards = new List<ElectronicCardData>();
            AddCardCommand = new Command(AddCard);
        }

        public async Task RefreshCards()
        {
            Cards.Clear();
            Cards = await _cardsManager.GetCards();
            OnPropertyChanged(nameof(Cards));
            OnPropertyChanged(nameof(HasCards));
        }

        private void AddCard()
        {
            
        }

        public async Task DeleteCard(ElectronicCardData cardData)
        {
            await _cardsManager.DeleteCard(cardData);
            await RefreshCards();
        }

        public async Task EditCard(ElectronicCardData cardData, string newCardName)
        {
            await _cardsManager.EditCard(cardData, newCardName);
            await RefreshCards();
        }
    }
}
