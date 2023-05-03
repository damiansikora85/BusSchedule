using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BusSchedule.Core.UI.Pages.Views
{
    public class EcardViewModel : BaseViewModel
    {
        public string CardNumber { get; set; }
        public ICommand FindCardCommand { get; private set; }

        public EcardViewModel() 
        {
            FindCardCommand = new AsyncCommand(OnFindCard);
        }

        private async Task OnFindCard()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://api.mzkwejherowo.pl/public/bilet-elektroniczny/k2z7d10rasogmy8uj6b5f3tc4iv9qxle/devices/1270567785/tickets2.json");
            if(response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var t = result.Length;
            }
        }
    }
}
