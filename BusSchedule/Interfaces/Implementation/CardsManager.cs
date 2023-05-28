using BusSchedule.Core.Model;
using BusSchedule.Core.UI.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BusSchedule.Interfaces.Implementation
{
    public class CardsManager : ICardsManager
    {
        private const string FILENAME = "cards.json";

        public async Task<IList<ElectronicCardData>> GetCards()
        {
            var fileName = GetFilename();
            if (File.Exists(fileName))
            {
                var jsonString = await File.ReadAllTextAsync(fileName);
                return JsonConvert.DeserializeObject<List<ElectronicCardData>>(jsonString);
            }
            return new List<ElectronicCardData>();
        }

        public async Task SaveCard(ElectronicCardData card)
        {
            var cards = await GetCards();
            cards.Add(card);
            await SaveCards(cards);
        }

        private string GetFilename()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), FILENAME);
        }

        private async Task SaveCards(IList<ElectronicCardData> cards)
        {
            var jsonString = JsonConvert.SerializeObject(cards);
            File.WriteAllText(GetFilename(), jsonString);
        }
    }
}
