using System;
using System.Reflection;
using System.Threading.Tasks;
using BusSchedule.Core.Model;
using BusSchedule.Core.UI.Interfaces;
using BusSchedule.Core.UI.Pages;
using Moq;
using NUnit.Framework;

namespace BusSchedule.NUnitTests
{
    [TestFixture]
    public class AddCardViewModelTests
    {
        [Test]
        public async Task SaveCard_UpdatesCardNameAndCallsSaveCard()
        {
            var cardsManagerMock = new Mock<ICardsManager>();
            var viewModel = new AddCardViewModel(cardsManagerMock.Object);
            var card = new ElectronicCardData
            {
                Number = "123",
                Name = "Old name",
                ValidTo = new DateTime(2026, 12, 31),
                DiscountValidTo = new DateTime(2026, 11, 30)
            };

            var field = typeof(AddCardViewModel).GetField("_foundCard", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(viewModel, card);

            await viewModel.SaveCard("Updated name");

            cardsManagerMock.Verify(x => x.SaveCard(It.Is<ElectronicCardData>(c => c.Number == "0000000123" && c.Name == "Updated name")), Times.Once);
            Assert.That(card.Name, Is.EqualTo("Updated name"));
        }

        [Test]
        public void Properties_ReturnExpectedValuesFromFoundCard()
        {
            var viewModel = new AddCardViewModel(Mock.Of<ICardsManager>());
            var card = new ElectronicCardData
            {
                Number = "456",
                Name = "Ticket card",
                ValidTo = new DateTime(2025, 5, 10),
                DiscountValidTo = new DateTime(2025, 4, 10)
            };

            var field = typeof(AddCardViewModel).GetField("_foundCard", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(viewModel, card);

            Assert.That(viewModel.CardNumber, Is.EqualTo("0000000456"));
            Assert.That(viewModel.CardName, Is.EqualTo("Ticket card"));
            Assert.That(viewModel.ValidTo, Is.EqualTo(new DateTime(2025, 5, 10)));
            Assert.That(viewModel.DiscountValidTo, Is.EqualTo(new DateTime(2025, 4, 10)));
        }
    }
}
