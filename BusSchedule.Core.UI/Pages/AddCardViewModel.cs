using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BusSchedule.Core.Model;
using Newtonsoft.Json.Converters;
using BusSchedule.Core.UI.Interfaces;
using BusSchedule.Core.UI.Helpers;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BusSchedule.Core.UI.Pages
{
    public class AddCardViewModel : INotifyPropertyChanged//BaseViewModel
    {
        public string SearchCardNumber { get; set; }
        public string CardNumber => _foundCard != null ? _foundCard.Number : "";
        public string CardName => _foundCard != null ? _foundCard.Name : "";
        public DateTime ValidTo => _foundCard != null ? _foundCard.ValidTo : DateTime.MinValue;
        public DateTime DiscountValidTo => _foundCard != null ? _foundCard.DiscountValidTo : DateTime.MinValue;
        public bool IsCardFound { get; private set; }
        public bool IsSearching { get; private set; }
        private readonly ICardsManager _cardsManager;
        private ElectronicCardData _foundCard;

        public event PropertyChangedEventHandler PropertyChanged;

        public AddCardViewModel(ICardsManager cardsManager)
        {
            _cardsManager = cardsManager;
        }

        public async Task SearchCard()
        {
            try
            {
                IsSearching = true;
                OnPropertyChanged(nameof(IsSearching));
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"https://api.mzkwejherowo.pl/public/bilet-elektroniczny/k2z7d10rasogmy8uj6b5f3tc4iv9qxle/cards/{SearchCardNumber}.json");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    if (result.StartsWith('{') && result.EndsWith('}'))
                    {
                        var json = JObject.Parse(result);
                        if (json["data"] != null)
                        {
                            _foundCard = json["data"].ToObject<ElectronicCardData>(new JsonSerializer { DateFormatString = "yyyy-MM-dd HH:mm:ss" });
                            IsCardFound = true;

                            OnPropertyChanged(nameof(IsCardFound));
                            OnPropertyChanged(nameof(CardNumber));
                            OnPropertyChanged(nameof(CardName));
                            OnPropertyChanged(nameof(ValidTo));
                            OnPropertyChanged(nameof(DiscountValidTo));
                        }
                    }
                    else
                    {
                        throw new ElectronicCardException($"Card not found: {SearchCardNumber}", System.Net.HttpStatusCode.NotFound, "Not found");
                    }
                }
            }
            catch(Exception) 
            {
                throw;
            }
            finally
            {
                IsSearching = false;
                OnPropertyChanged(nameof(IsSearching));
            }
        }

        public async Task SaveCard(string name)
        {
            _foundCard.Name = name;
            await _cardsManager.SaveCard(_foundCard);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
