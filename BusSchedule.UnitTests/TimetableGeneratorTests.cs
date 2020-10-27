using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusSchedule.Core.UseCase;
using BusSchedule.Core.Model;
using NUnit.Framework;
using Moq;
using BusSchedule.Core.Utils;

namespace UnitTestProject1
{
    [TestFixture]
    public class TimetableGeneratorTests
    {
        [Test]
        public async Task GeneratorTest_NoVariants()
        {
            //act
            var station = new BusStation
            {
                Id = 2
            };
            var route = new BusRoute();
            var dataProviderMock = new Mock<IDataProvider>();
            dataProviderMock.Setup(provider => provider.GetStationDetailsForRoute(It.IsAny<BusRoute>(), It.IsAny<BusStation>())).ReturnsAsync(new List<BusRouteDetails>
            {
                new BusRouteDetails { BusRouteId = 0, BusStopId = 0, OrderNum = 0, RouteVariant = 0, TimeDiff = 0 },
            });

            dataProviderMock.Setup(provider => provider.GetRouteBeginTimes(It.IsAny<BusRoute>())).ReturnsAsync(new List<RouteBeginTime>
            {
                new RouteBeginTime { Id = 0, RouteId = 0, RouteVariant = 0, Time = TimeSpan.FromHours(7), Days = RouteBeginTime.ScheduleDays.WorkingDays },
                new RouteBeginTime { Id = 0, RouteId = 0, RouteVariant = 0, Time = TimeSpan.FromHours(8), Days = RouteBeginTime.ScheduleDays.Saturday },
                new RouteBeginTime { Id = 0, RouteId = 0, RouteVariant = 0, Time = TimeSpan.FromHours(9), Days = RouteBeginTime.ScheduleDays.SundayAndHolidays },
            });

            dataProviderMock.Setup(provider => provider.GetStationsDetailsForRouteVariant(It.IsAny<BusRoute>(), It.IsAny<int>())).ReturnsAsync(new List<BusRouteDetails>
            {
                new BusRouteDetails { BusRouteId = 0, BusStopId = 0, OrderNum = 0, RouteVariant = 0, TimeDiff = 0 },
                new BusRouteDetails { BusRouteId = 0, BusStopId = 1, OrderNum = 1, RouteVariant = 0, TimeDiff = 1 },
                new BusRouteDetails { BusRouteId = 0, BusStopId = 2, OrderNum = 2, RouteVariant = 0, TimeDiff = 1 },
            });

            dataProviderMock.Setup(provider => provider.GetTimeAdjustmentForRoute(It.IsAny<int>())).ReturnsAsync(new List<StationTimeAdjustment>());

            //arrange
            var timeTable = await TimetableGenerator.Generate(station, route, dataProviderMock.Object);

            //assert
            Assert.AreEqual(3, timeTable.Keys.Count);
            Assert.AreEqual(TimeSpan.Parse("07:02:00"), timeTable[RouteBeginTime.ScheduleDays.WorkingDays][0]);
            Assert.AreEqual(TimeSpan.Parse("08:02:00"), timeTable[RouteBeginTime.ScheduleDays.Saturday][0]);
            Assert.AreEqual(TimeSpan.Parse("09:02:00"), timeTable[RouteBeginTime.ScheduleDays.SundayAndHolidays][0]);
        }

        [Test]
        public async Task GeneratorTest_WithVariants()
        {
            //act
            var station = new BusStation
            {
                Id = 2
            };
            var route = new BusRoute();
            var dataProviderMock = new Mock<IDataProvider>();
            dataProviderMock.Setup(provider => provider.GetStationDetailsForRoute(It.IsAny<BusRoute>(), It.IsAny<BusStation>())).ReturnsAsync(new List<BusRouteDetails>
            {
                new BusRouteDetails { BusRouteId = 0, BusStopId = 0, OrderNum = 0, RouteVariant = 0, TimeDiff = 0 },
                new BusRouteDetails { BusRouteId = 0, BusStopId = 0, OrderNum = 0, RouteVariant = 1, TimeDiff = 0 },
            });

            dataProviderMock.Setup(provider => provider.GetRouteBeginTimes(It.IsAny<BusRoute>())).ReturnsAsync(new List<RouteBeginTime>
            {
                new RouteBeginTime { Id = 0, RouteId = 0, RouteVariant = 0, Time = TimeSpan.FromHours(7), Days = RouteBeginTime.ScheduleDays.WorkingDays },
                new RouteBeginTime { Id = 0, RouteId = 0, RouteVariant = 1, Time = TimeSpan.FromHours(8), Days = RouteBeginTime.ScheduleDays.WorkingDays },
            });

            dataProviderMock.Setup(provider => provider.GetStationsDetailsForRouteVariant(It.IsAny<BusRoute>(), 0)).ReturnsAsync(new List<BusRouteDetails>
            {
                new BusRouteDetails { BusRouteId = 0, BusStopId = 0, OrderNum = 0, RouteVariant = 0, TimeDiff = 0 },
                new BusRouteDetails { BusRouteId = 0, BusStopId = 1, OrderNum = 1, RouteVariant = 0, TimeDiff = 1 },
                new BusRouteDetails { BusRouteId = 0, BusStopId = 2, OrderNum = 2, RouteVariant = 0, TimeDiff = 1 },
            });
            dataProviderMock.Setup(provider => provider.GetStationsDetailsForRouteVariant(It.IsAny<BusRoute>(), 1)).ReturnsAsync(new List<BusRouteDetails>
            {
                new BusRouteDetails { BusRouteId = 0, BusStopId = 0, OrderNum = 0, RouteVariant = 1, TimeDiff = 0 },
                new BusRouteDetails { BusRouteId = 0, BusStopId = 1, OrderNum = 1, RouteVariant = 1, TimeDiff = 2 },
                new BusRouteDetails { BusRouteId = 0, BusStopId = 2, OrderNum = 2, RouteVariant = 1, TimeDiff = 1 },
            });

            dataProviderMock.Setup(provider => provider.GetTimeAdjustmentForRoute(It.IsAny<int>())).ReturnsAsync(new List<StationTimeAdjustment>());

            //arrange
            var timeTable = await TimetableGenerator.Generate(station, route, dataProviderMock.Object);

            //assert
            Assert.AreEqual(3, timeTable.Keys.Count);
            Assert.AreEqual(TimeSpan.Parse("07:02:00"), timeTable[RouteBeginTime.ScheduleDays.WorkingDays][0]);
            Assert.AreEqual(TimeSpan.Parse("08:03:00"), timeTable[RouteBeginTime.ScheduleDays.WorkingDays][1]);
        }

        [Test]
        public async Task GeneratorTest_WithAdjustments()
        {
            //act
            var station = new BusStation
            {
                Id = 2
            };
            var route = new BusRoute();
            var dataProviderMock = new Mock<IDataProvider>();
            dataProviderMock.Setup(provider => provider.GetStationDetailsForRoute(It.IsAny<BusRoute>(), It.IsAny<BusStation>())).ReturnsAsync(new List<BusRouteDetails>
            {
                new BusRouteDetails { BusRouteId = 0, BusStopId = 0, OrderNum = 0, RouteVariant = 0, TimeDiff = 0 },
            });

            dataProviderMock.Setup(provider => provider.GetRouteBeginTimes(It.IsAny<BusRoute>())).ReturnsAsync(new List<RouteBeginTime>
            {
                new RouteBeginTime { Id = 0, RouteId = 0, RouteVariant = 0, Time = TimeSpan.FromHours(7), Days = RouteBeginTime.ScheduleDays.WorkingDays },
                new RouteBeginTime { Id = 0, RouteId = 0, RouteVariant = 0, Time = TimeSpan.FromHours(8), Days = RouteBeginTime.ScheduleDays.Saturday },
                new RouteBeginTime { Id = 0, RouteId = 0, RouteVariant = 0, Time = TimeSpan.FromHours(9), Days = RouteBeginTime.ScheduleDays.SundayAndHolidays },
            });

            dataProviderMock.Setup(provider => provider.GetStationsDetailsForRouteVariant(It.IsAny<BusRoute>(), It.IsAny<int>())).ReturnsAsync(new List<BusRouteDetails>
            {
                new BusRouteDetails { BusRouteId = 0, BusStopId = 0, OrderNum = 0, RouteVariant = 0, TimeDiff = 0 },
                new BusRouteDetails { BusRouteId = 0, BusStopId = 1, OrderNum = 1, RouteVariant = 0, TimeDiff = 1 },
                new BusRouteDetails { BusRouteId = 0, BusStopId = 2, OrderNum = 2, RouteVariant = 0, TimeDiff = 1 },
            });

            dataProviderMock.Setup(provider => provider.GetTimeAdjustmentForRoute(It.IsAny<int>())).ReturnsAsync(new List<StationTimeAdjustment>
            {
                new StationTimeAdjustment { BeginTimeId = 0, Days = RouteBeginTime.ScheduleDays.WorkingDays, RouteId = 0, RouteVariantId = 0, StationId = 1, TimeAdjustmentMin = -1 }
            });

            //arrange
            var timeTable = await TimetableGenerator.Generate(station, route, dataProviderMock.Object);

            //assert
            Assert.AreEqual(3, timeTable.Keys.Count);
            Assert.AreEqual(TimeSpan.Parse("07:01:00"), timeTable[RouteBeginTime.ScheduleDays.WorkingDays][0]);
            Assert.AreEqual(TimeSpan.Parse("08:02:00"), timeTable[RouteBeginTime.ScheduleDays.Saturday][0]);
            Assert.AreEqual(TimeSpan.Parse("09:02:00"), timeTable[RouteBeginTime.ScheduleDays.SundayAndHolidays][0]);
        }
    }
}
