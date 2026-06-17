using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using BusSchedule.Pages.ViewModels;
using Moq;
using NUnit.Framework;

namespace BusSchedule.NUnitTests
{
    [TestFixture]
    public class RoutesPageViewModelTests
    {
        [Test]
        public async Task RefreshBusServicesAsync_PopulatesFeedDatesAndRoutes()
        {
            var dataProviderMock = new Mock<IDataProvider>();
            var expectedRoutes = new List<Routes> { new Routes { Route_Id = "R1" }, new Routes { Route_Id = "R2" } };
            var expectedStart = new DateTime(2026, 1, 1);
            var expectedEnd = new DateTime(2026, 12, 31);

            dataProviderMock.Setup(x => x.GetFeedStartEndDates()).ReturnsAsync((expectedStart, expectedEnd));
            dataProviderMock.Setup(x => x.GetRoutes()).ReturnsAsync(expectedRoutes);

            var viewModel = new RoutesPageViewModel(dataProviderMock.Object);
            var changedProperties = new List<string>();
            viewModel.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName);

            await viewModel.RefreshBusServicesAsync();

            Assert.That(viewModel.FeedStartDate, Is.EqualTo(expectedStart));
            Assert.That(viewModel.FeedEndDate, Is.EqualTo(expectedEnd));
            Assert.That(viewModel.Routes, Is.EqualTo(expectedRoutes));
            Assert.That(changedProperties, Does.Contain(nameof(RoutesPageViewModel.FeedStartDate)));
            Assert.That(changedProperties, Does.Contain(nameof(RoutesPageViewModel.FeedEndDate)));
        }

        [Test]
        public async Task GetDestinationsForRoute_DelegatesToProvider()
        {
            var route = new Routes { Route_Id = "R1" };
            var expectedDestination = new Destination { Route_Id = "R1", Inbound = "Home", Outbound = "Work" };
            var dataProviderMock = new Mock<IDataProvider>();
            dataProviderMock.Setup(x => x.GetRouteDestinations(route)).ReturnsAsync(expectedDestination);

            var viewModel = new RoutesPageViewModel(dataProviderMock.Object);
            var destination = await viewModel.GetDestinationsForRoute(route);

            Assert.That(destination, Is.EqualTo(expectedDestination));
        }
    }
}
