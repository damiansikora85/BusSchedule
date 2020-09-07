using BusSchedule.Core.Model;
using BusSchedule.Core.UseCase;
using BusSchedule.Core.Utils;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    [TestFixture]
    public class ModelTests
    {
        [Test]
        public void GetBusServicesTest()
        {
            var dataProviderMock = new Mock<IDataProvider>();
            dataProviderMock.Setup(provider => provider.GetBusServices()).Returns(new List<BusService> {
                new BusService{ Id = 1, Name = "1" },
                new BusService{ Id = 2, Name = "2" },
                new BusService{ Id = 3, Name = "3" }
            });
            var coreService = ScheduleService.Create(dataProviderMock.Object);
            Assert.IsTrue(coreService.GetBusServices().Count > 0);
        }

        //get directions by service
        [Test]
        public void GetDirectionTest()
        {
            var dataProviderMock = new Mock<IDataProvider>();
            dataProviderMock.Setup(provider => provider.GetBusRoutes(It.IsAny<int>())).Returns(new List<BusRoute> { new BusRoute { BusServiceId = 1, EndStationId = 2, StartStationId = 1, Id = 1 } });
            var coreService = ScheduleService.Create(dataProviderMock.Object);
            var routes = coreService.GetBusRoutes(1);
            Assert.IsNotNull(routes.Count > 0);
        }

        // get timetable for bus stop
        [Test]
        public void GetFullTimetableTest()
        {
            var dataProviderMock = new Mock<IDataProvider>();
            var coreService = ScheduleService.Create(dataProviderMock.Object);
            Assert.IsNotNull(coreService.GetScheduleForBusStop(1, 1));
        }

        // get route details
        [Test]
        public void GetRouteDetailsTest()
        {
            var dataProviderMock = new Mock<IDataProvider>();
            var coreService = ScheduleService.Create(dataProviderMock.Object);
            Assert.IsNotNull(coreService.GetRouteDetails(1));
        }
    }
}
