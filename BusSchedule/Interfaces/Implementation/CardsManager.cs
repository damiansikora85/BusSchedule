using BusSchedule.Core.Model;
using BusSchedule.Core.UI.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public async Task DeleteCard(ElectronicCardData cardData)
        {
            var cards = await GetCards();
            var foundCard = cards.FirstOrDefault(card => card.Number == cardData.Number);
            if (foundCard != null)
            {
                cards.Remove(foundCard);
                await SaveCards(cards);
            }
        }
        public async Task EditCard(ElectronicCardData cardData, string newCardName)
        {
            var cards = await GetCards();
            var foundCard = cards.FirstOrDefault(card => card.Number == cardData.Number);
            if (foundCard != null)
            {
                foundCard.Name = newCardName;
                await SaveCards(cards);
            }
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
