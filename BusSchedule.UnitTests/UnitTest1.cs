using BusSchedule.Core.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UnitTestProject1
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void TestMethod1()
        {
            var data = new ScheduleData
            {

                BusServices = new List<BusService>
                {
                    new BusService { Id = 1, Name = "1" },
                    new BusService { Id = 2, Name = "2" },
                    new BusService { Id = 3, Name = "3" },
                    new BusService { Id = 4, Name = "4" },
                    new BusService { Id = 5, Name = "5" }
                }
            };

            var jsonStr = JsonConvert.SerializeObject(data);
            Assert.IsNotNull(jsonStr);
        }
    }
}
