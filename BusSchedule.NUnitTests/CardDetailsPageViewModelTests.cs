using System;
using System.Reflection;
using BusSchedule.Core.Model;
using BusSchedule.Core.UI.Pages;
using NUnit.Framework;

namespace BusSchedule.NUnitTests
{
    [TestFixture]
    public class CardDetailsPageViewModelTests
    {
        [Test]
        public void Constructor_InitializesCardPropertiesAndTickets()
        {
            var cardData = new ElectronicCardData
            {
                Number = "999",
                Name = "Special card",
                ValidTo = new DateTime(2024, 1, 1),
                DiscountValidTo = new DateTime(2024, 2, 1)
            };

            var viewModel = new CardDetailsPageViewModel(cardData);

            Assert.That(viewModel.CardNumber, Is.EqualTo("0000000999"));
            Assert.That(viewModel.CardName, Is.EqualTo("Special card"));
            Assert.That(viewModel.ValidTo, Is.EqualTo(new DateTime(2024, 1, 1)));
            Assert.That(viewModel.DiscountValidTo, Is.EqualTo(new DateTime(2024, 2, 1)));
            Assert.That(viewModel.Tickets, Is.Not.Null);
            Assert.That(viewModel.Tickets, Is.Empty);
        }

        [Test]
        public void Properties_AfterPrivateCardDataChange_ReflectNewValues()
        {
            var cardData = new ElectronicCardData
            {
                Number = "111",
                Name = "Original",
                ValidTo = new DateTime(2024, 8, 1),
                DiscountValidTo = new DateTime(2024, 9, 1)
            };

            var viewModel = new CardDetailsPageViewModel(cardData);
            var newCardData = new ElectronicCardData
            {
                Number = "222",
                Name = "Updated",
                ValidTo = new DateTime(2024, 12, 12),
                DiscountValidTo = new DateTime(2024, 12, 31)
            };

            var field = typeof(CardDetailsPageViewModel).GetField("_cardData", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(viewModel, newCardData);

            Assert.That(viewModel.CardNumber, Is.EqualTo("0000000222"));
            Assert.That(viewModel.CardName, Is.EqualTo("Updated"));
            Assert.That(viewModel.ValidTo, Is.EqualTo(new DateTime(2024, 12, 12)));
            Assert.That(viewModel.DiscountValidTo, Is.EqualTo(new DateTime(2024, 12, 31)));
        }
    }
}
