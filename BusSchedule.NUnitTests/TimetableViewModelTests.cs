using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BusSchedule.Core.GTFS;
using BusSchedule.Core.Interfaces;
using BusSchedule.Core.Model;
using BusSchedule.Core.UI;
using BusSchedule.Core.UI.Pages;
using BusSchedule.Core.Utils;
using Moq;
using NUnit.Framework;

namespace BusSchedule.NUnitTests
{
    [TestFixture]
    public class TimetableViewModelTests
    {
        [Test]
        public void IsOnFavoritesList_ReturnsTrueWhenRouteAndStopAreFavorite()
        {
            var route = new Routes { Route_Id = "R1" };
            var stop = new Stops { Stop_Id = "S1", Stop_Name = "Stop 1" };
            var favoritesManagerMock = new Mock<IFavoritesManager>();
            favoritesManagerMock.Setup(x => x.IsOnList("R1", "S1")).Returns(true);

            var viewModel = new TimetableViewModel(route, stop, 0, Mock.Of<IDataProvider>(), favoritesManagerMock.Object);

            Assert.That(viewModel.IsOnFavoritesList(), Is.True);
        }

        [Test]
        public void AddThisToFavorites_CallsFavoritesManagerAddWithCurrentRouteStopAndDirection()
        {
            var route = new Routes { Route_Id = "R1" };
            var stop = new Stops { Stop_Id = "S1", Stop_Name = "Stop 1" };
            var favoritesManagerMock = new Mock<IFavoritesManager>();

            var viewModel = new TimetableViewModel(route, stop, 1, Mock.Of<IDataProvider>(), favoritesManagerMock.Object);

            viewModel.AddThisToFavorites();

            favoritesManagerMock.Verify(x => x.Add("R1", "S1", 1), Times.Once);
        }

        [Test]
        public async Task RefreshTimetableAsync_PopulatesTimetableLegendAndSetsDirection()
        {
            // Arrange
            var route = new Routes { Route_Id = "R1" };
            var stop = new Stops { Stop_Id = "S1", Stop_Name = "Station A" };
            int? direction = 0;

            var dataProviderMock = new Mock<IDataProvider>();

            // Destinations
            dataProviderMock
                .Setup(dp => dp.GetRouteDestinations(It.IsAny<Routes>()))
                .ReturnsAsync(new Destination { Outbound = "OutboundDest", Inbound = "InboundDest" });

            // Legend (with duplicate short descriptions to exercise ParseLegend deduplication)
            var legend = new List<Trip_Description>
            {
                new Trip_Description { Shape_Id = "SH1", ShortDescription = "X", LongDescription = "L1" },
                new Trip_Description { Shape_Id = "SH2", ShortDescription = "X", LongDescription = "L2" }
            };
            dataProviderMock
                .Setup(dp => dp.GetRouteLegend(route.Route_Id, direction))
                .ReturnsAsync(legend);

            // Service id for the selected day
            dataProviderMock
                .Setup(dp => dp.GetServiceIdByDate(It.IsAny<DateTime>()))
                .ReturnsAsync("WD");

            // Trips for route for service id
            var trips = new List<Trips>
            {
                new Trips { Trip_Id = "T1", Shape_Id = "SH1", Route_Id = route.Route_Id }
            };
            dataProviderMock
                .Setup(dp => dp.GetTripsForRoute(route.Route_Id, direction.Value, "WD"))
                .ReturnsAsync(trips);

            // Descriptions for trips
            dataProviderMock
                .Setup(dp => dp.GetRouteDescriptionForTrips(It.IsAny<IEnumerable<Trips>>()))
                .ReturnsAsync(new List<Trip_Description>
                {
                    new Trip_Description { Shape_Id = "SH1", ShortDescription = "Short1", LongDescription = "Long1" }
                });

            // Stop times for trip at the station
            var stopTimes = new List<Stop_Times>
            {
                new Stop_Times { Trip_Id = "T1", Stop_Id = stop.Stop_Id, Arrival_Time = "08:15:00", Pickup_Type = "0" },
                new Stop_Times { Trip_Id = "T1", Stop_Id = stop.Stop_Id, Arrival_Time = "09:20:00", Pickup_Type = "0" }
            };
            dataProviderMock
                .Setup(dp => dp.GetStopTimesForTrip("T1", stop.Stop_Id))
                .ReturnsAsync(stopTimes);

            var favoritesMock = new Mock<IFavoritesManager>();

            var vm = new TimetableViewModel(route, stop, direction, dataProviderMock.Object, favoritesMock.Object);

            // Act
            await vm.RefreshTimetableAsync();

            // Assert
            Assert.AreEqual("OutboundDest", vm.Direction, "Direction should be set based on route destinations and provided direction.");
            Assert.IsNotNull(vm.Timetable, "Timetable should be populated.");
            Assert.IsNotEmpty(vm.Timetable, "Timetable should contain items after refresh.");
            // ParseLegend should remove duplicate short descriptions -> only one unique entry expected
            Assert.AreEqual(1, vm.TimetableLegend.Count, "TimetableLegend should contain deduplicated descriptions.");
            Assert.AreEqual(stop.Stop_Name, vm.StopName, "StopName should expose the station's name.");
        }

        [Test]
        public void ParseLegend_RemovesDuplicateShortDescriptions()
        {
            var route = new Routes { Route_Id = "R1" };
            var stop = new Stops { Stop_Id = "S1", Stop_Name = "Stop 1" };
            var viewModel = new TimetableViewModel(route, stop, null, Mock.Of<IDataProvider>(), Mock.Of<IFavoritesManager>());

            var legendItems = new List<Trip_Description>
            {
                new Trip_Description { ShortDescription = "A", LongDescription = "First" },
                new Trip_Description { ShortDescription = "A", LongDescription = "Second" },
                new Trip_Description { ShortDescription = "B", LongDescription = "Third" }
            };

            var method = typeof(TimetableViewModel).GetMethod("ParseLegend", BindingFlags.Instance | BindingFlags.NonPublic);
            var result = (List<Trip_Description>)method.Invoke(viewModel, new object[] { legendItems });

            Assert.That(result.Select(i => i.ShortDescription), Is.EquivalentTo(new[] { "A", "B" }));
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void Setup_GroupsTimetableTuplesByHourAndIncludesAdditionalInfo()
        {
            var route = new Routes { Route_Id = "R1" };
            var stop = new Stops { Stop_Id = "S1", Stop_Name = "Stop 1" };
            var viewModel = new TimetableViewModel(route, stop, null, Mock.Of<IDataProvider>(), Mock.Of<IFavoritesManager>());

            var tuples = new List<TimetableTuple>
            {
                new TimetableTuple { Time = new TimeSpan(8, 5, 0), AdditionalDescription = new Trip_Description { ShortDescription = "X" } },
                new TimetableTuple { Time = new TimeSpan(8, 15, 0), AdditionalDescription = new Trip_Description { ShortDescription = "Y" } },
                new TimetableTuple { Time = new TimeSpan(9, 0, 0), AdditionalDescription = new Trip_Description { ShortDescription = "Z" } }
            };

            var method = typeof(TimetableViewModel).GetMethod("Setup", BindingFlags.Instance | BindingFlags.NonPublic);
            var result = (List<TimetableItem>)method.Invoke(viewModel, new object[] { tuples });

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Hour, Is.EqualTo(8));
            Assert.That(result[0].Minutes.Select(m => m.Minutes), Is.EquivalentTo(new[] { 5, 15 }));
            Assert.That(result[1].Hour, Is.EqualTo(9));
        }
    }
}
