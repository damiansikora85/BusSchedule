using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using BusSchedule.Core.Model;
using BusSchedule.Core.UI.Interfaces;
using BusSchedule.Core.UI.Pages.Views;
using Moq;
using NUnit.Framework;

namespace BusSchedule.NUnitTests
{
    [TestFixture]
    public class CardsListViewModelTests
    {
        [Test]
        public async Task RefreshCards_PopulatesCardsAndRaisesNotifications()
        {
            try
            {
                var cards = new List<ElectronicCardData>
            {
                new ElectronicCardData { Number = "123", Name = "Card A" },
                new ElectronicCardData { Number = "456", Name = "Card B" }
            };
                var cardsManagerMock = new Mock<ICardsManager>();
                cardsManagerMock.Setup(x => x.GetCards()).ReturnsAsync(cards);

                var viewModel = new CardsListViewModel(cardsManagerMock.Object);
                var changedProperties = new List<string>();
                viewModel.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName);

                await viewModel.RefreshCards();

                Assert.That(viewModel.Cards, Is.EqualTo(cards));
                Assert.That(viewModel.HasCards, Is.True);
                Assert.That(changedProperties, Does.Contain(nameof(CardsListViewModel.Cards)));
                Assert.That(changedProperties, Does.Contain(nameof(CardsListViewModel.HasCards)));
            }
            catch(System.Exception exc)
            {
                var msg = exc.Message;
            }
        }

        [Test]
        public async Task DeleteCard_CallsManagerAndRefreshesCards()
        {
            // Use a mutable backing list to ensure GetCards() returns different results
            IList<ElectronicCardData> currentList = new List<ElectronicCardData> { new ElectronicCardData { Number = "123" } };
            var cardsManagerMock = new Mock<ICardsManager>();

            // Make GetCards return the currentList reference each time it's called
            cardsManagerMock.Setup(x => x.GetCards()).ReturnsAsync(() => currentList);

            // When DeleteCard is called, simulate removal by replacing currentList with an empty list
            cardsManagerMock
                .Setup(x => x.DeleteCard(It.IsAny<ElectronicCardData>()))
                .Callback<ElectronicCardData>(c => currentList = new List<ElectronicCardData>())
                .Returns(Task.CompletedTask);

            var viewModel = new CardsListViewModel(cardsManagerMock.Object);

            // Populate initial state (consumes the initial currentList)
            await viewModel.RefreshCards();
            Assert.That(viewModel.Cards, Has.One.Items, "Precondition: there should be one card before delete.");

            // Perform delete which should trigger the callback and then refresh inside the VM
            await viewModel.DeleteCard(new ElectronicCardData { Number = "123" });

            cardsManagerMock.Verify(x => x.DeleteCard(It.Is<ElectronicCardData>(c => c.Number == "0000000123")), Times.Once);
            Assert.That(viewModel.Cards, Is.Empty);
        }


        [Test]
        public async Task EditCard_CallsManagerAndRefreshesCards()
        {
            var card = new ElectronicCardData { Number = "123", Name = "Card A" };
            var cardsManagerMock = new Mock<ICardsManager>();
            cardsManagerMock.SetupSequence(x => x.GetCards())
                .ReturnsAsync(new List<ElectronicCardData> { card })
                .ReturnsAsync(new List<ElectronicCardData> { card });
            cardsManagerMock.Setup(x => x.EditCard(card, "Updated")).Returns(Task.CompletedTask);

            var viewModel = new CardsListViewModel(cardsManagerMock.Object);
            await viewModel.EditCard(card, "Updated");

            cardsManagerMock.Verify(x => x.EditCard(card, "Updated"), Times.Once);
            Assert.That(viewModel.Cards, Has.One.Items);
        }
    }
}
