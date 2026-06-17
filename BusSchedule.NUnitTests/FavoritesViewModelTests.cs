using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusSchedule.Core.Exceptions;
using BusSchedule.Core.Interfaces;
using BusSchedule.Core.Model;
using BusSchedule.Core.UI.Components;
using BusSchedule.Core.UI.Interfaces;
using BusSchedule.Core.UI.Pages.Views;
using BusSchedule.Core.Utils;
using Moq;
using NUnit.Framework;

namespace BusSchedule.NUnitTests
{
    [TestFixture]
    public class FavoritesViewModelTests
    {
        [Test]
        public async Task RefreshData_AddsFavoriteWhenCreateSucceeds()
        {
            var favoritesManagerMock = new Mock<IFavoritesManager>();
            var dataProviderMock = new Mock<IDataProvider>();
            var favoriteDescription = new FavoriteDescription { RouteId = "R1", StopId = "S1", Direction = 0 };
            favoritesManagerMock.Setup(x => x.GetAll()).Returns(new List<FavoriteDescription> { favoriteDescription });

            dataProviderMock.Setup(x => x.GetStopById("S1")).ReturnsAsync(new Stops { Stop_Id = "S1", Stop_Name = "Stop 1", Stop_Lat = "50.0", Stop_Lon = "20.0" });
            dataProviderMock.Setup(x => x.GetRoute("R1")).ReturnsAsync(new Routes { Route_Id = "R1", Route_Short_Name = "1" });
            dataProviderMock.Setup(x => x.GetRouteDestinations(It.IsAny<Routes>())).ReturnsAsync(new Destination { Outbound = "Outbound", Inbound = "Inbound" });

            var viewModel = new FavoritesViewModel(favoritesManagerMock.Object, dataProviderMock.Object);

            await viewModel.RefreshData();

            Assert.That(viewModel.Favorites.Count, Is.EqualTo(1));
            Assert.That(viewModel.HasAnyFavorites, Is.True);
            Assert.That(viewModel.Favorites.First().DestinationName, Is.EqualTo("Outbound"));
            favoritesManagerMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Test]
        public async Task RefreshData_DeletesFavoriteWhenCreateThrowsFavoriteCreateException()
        {
            var favoritesManagerMock = new Mock<IFavoritesManager>();
            var dataProviderMock = new Mock<IDataProvider>();
            var favoriteDescription = new FavoriteDescription { RouteId = "R1", StopId = "S1", Direction = 1 };
            favoritesManagerMock.Setup(x => x.GetAll()).Returns(new List<FavoriteDescription> { favoriteDescription });
            dataProviderMock.Setup(x => x.GetStopById("S1")).ReturnsAsync((Stops)null);

            var viewModel = new FavoritesViewModel(favoritesManagerMock.Object, dataProviderMock.Object);

            await viewModel.RefreshData();

            Assert.That(viewModel.Favorites, Is.Empty);
            favoritesManagerMock.Verify(x => x.Delete("R1", "S1"), Times.Once);
        }

        [Test]
        public void DeleteItem_RemovesItemAndUpdatesFlags()
        {
            var favoritesManagerMock = new Mock<IFavoritesManager>();
            var dataProviderMock = new Mock<IDataProvider>();

            var favorite = new FavoriteData(new Routes { Route_Id = "R1", Route_Short_Name = "1" }, new Stops { Stop_Id = "S1", Stop_Name = "Stop 1", Stop_Lat = "50.0", Stop_Lon = "20.0" }, 0, "Outbound");
            var viewModel = new FavoritesViewModel(favoritesManagerMock.Object, dataProviderMock.Object);
            viewModel.Favorites.Add(favorite);

            viewModel.DeleteItem(favorite);

            Assert.That(viewModel.Favorites, Is.Empty);
            Assert.That(viewModel.HasAnyFavorites, Is.False);
            Assert.That(viewModel.HasNoFavorites, Is.True);
            favoritesManagerMock.Verify(x => x.Delete("R1", "S1"), Times.Once);
        }
    }
}
