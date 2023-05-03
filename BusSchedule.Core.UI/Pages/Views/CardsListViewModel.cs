using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace BusSchedule.Core.UI.Pages.Views
{
    public class CardsListViewModel : BaseViewModel
    {
        public IList<string> Cards { get; private set; }
        public bool HasCards => Cards.Any();
        public ICommand AddCardCommand { get; private set; }

        public CardsListViewModel()
        {
            Cards = new List<string>();
            AddCardCommand = new Command(AddCard);
        }

        private void AddCard()
        {
            
        }
    }
}
