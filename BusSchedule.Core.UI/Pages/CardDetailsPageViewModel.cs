using BusSchedule.Core.Model;
using MvvmHelpers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BusSchedule.Core.UI.Pages
{
    public class CardDetailsPageViewModel : BaseViewModel
    {
        public bool IsLoading { get; private set; }
        public string CardNumber => _cardData.Number;
        public string CardName => _cardData.Name;
        public DateTime ValidTo => _cardData.ValidTo;
        public DateTime DiscountValidTo => _cardData.DiscountValidTo;
        private ElectronicCardData _cardData;
        public ObservableRangeCollection<TicketData> Tickets { get; private set; }

        public CardDetailsPageViewModel(ElectronicCardData cardData)
        {
            _cardData = cardData;
            Tickets = new ObservableRangeCollection<TicketData>();
        }

        public async Task RefreshData()
        {
            Tickets.Clear();
            IsLoading = true;
            OnPropertyChanged(nameof(IsLoading));
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"https://api.mzkwejherowo.pl/public/bilet-elektroniczny/k2z7d10rasogmy8uj6b5f3tc4iv9qxle/cards/{_cardData.Number}.json");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(result);
                if (json["data"] != null)
                {
                    _cardData = json["data"].ToObject<ElectronicCardData>(new JsonSerializer { DateFormatString = "yyyy-MM-dd HH:mm:ss" });
                }
                    var ticketsResponse = await httpClient.GetAsync($"https://api.mzkwejherowo.pl/public/bilet-elektroniczny/k2z7d10rasogmy8uj6b5f3tc4iv9qxle/devices/{_cardData.Number}/tickets2.json");
                if(ticketsResponse.IsSuccessStatusCode) 
                {
                    var ticketsResult = await ticketsResponse.Content.ReadAsStringAsync();
                    var ticketsJson = JObject.Parse(ticketsResult);
                    if (ticketsJson["data"] != null)
                    {
                        var array = ticketsJson["data"];
                        var ar = JArray.FromObject(array);
                        Tickets.AddRange(ar.ToObject<List<TicketData>>(new JsonSerializer { DateFormatString = "yyyy-MM-dd HH:mm:ss" }));
                    }
                }
            }

            IsLoading = false;
            OnPropertyChanged(nameof(IsLoading));
            OnPropertyChanged(nameof(CardNumber));
            OnPropertyChanged(nameof(CardName));
            OnPropertyChanged(nameof(ValidTo));
            OnPropertyChanged(nameof(DiscountValidTo));
        }
    }
}
