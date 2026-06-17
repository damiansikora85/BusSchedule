using System.Collections.Generic;
using System.Threading.Tasks;
using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using BusSchedule.UI.ViewModels;
using Moq;
using NUnit.Framework;

namespace BusSchedule.NUnitTests
{
    [TestFixture]
    public class RoutePageViewModelTests
    {
        [Test]
        public async Task RefreshDataAsync_PopulatesRouteStopsAndTrace()
        {
            var route = new Routes { Route_Id = "R1", Route_Short_Name = "1" };
            var station1 = new Stops { Stop_Id = "S1", Stop_Name = "Stop 1", Stop_Lat = "50.0", Stop_Lon = "20.0" };
            var station2 = new Stops { Stop_Id = "S2", Stop_Name = "Stop 2", Stop_Lat = "50.5", Stop_Lon = "20.5" };
            var trace = new List<Trace> { new Trace { Points = new List<Point> { new Point(50, 20) } } };

            var dataProviderMock = new Mock<IDataProvider>();
            dataProviderMock.Setup(x => x.GetStopsForRoute(route, It.IsAny<int>())).ReturnsAsync(new List<Stops> { station1, station2 });
            dataProviderMock.Setup(x => x.GetRouteTrace(route.Route_Short_Name, 1)).ReturnsAsync(trace);

            var viewModel = new RoutePageViewModel(route, 1, dataProviderMock.Object);

            await viewModel.RefreshDataAsync();

            Assert.That(viewModel.Stops.Count, Is.EqualTo(2));
            Assert.That(viewModel.Stops[0].Stop_Name, Is.EqualTo("Stop 1"));
            Assert.That(viewModel.Stops[1].Stop_Name, Is.EqualTo("Stop 2"));
            Assert.That(viewModel.Traces, Is.EqualTo(trace));
            Assert.That(viewModel.CalculateCenterPosition().Latitude, Is.EqualTo((50.0 + 50.5) / 2).Within(0.0001));
            Assert.That(viewModel.CalculateCenterPosition().Longitude, Is.EqualTo((20.0 + 20.5) / 2).Within(0.0001));
        }
    }
}
