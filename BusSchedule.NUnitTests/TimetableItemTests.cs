using BusSchedule.Core.UI;
using NUnit.Framework;

namespace BusSchedule.NUnitTests
{
    [TestFixture]
    public class TimetableItemTests
    {
        [Test]
        public void ToString_ReturnsHourAsString()
        {
            var item = new TimetableItem { Hour = 8 };

            Assert.That(item.ToString(), Is.EqualTo("8"));
        }

        [Test]
        public void MinutesToString_ReturnsZeroPaddedMinutesWithoutAdditionalInfo()
        {
            var minute = new TimetableItem.TimetableItemMinutes { Minutes = 5 };

            Assert.That(minute.ToString(), Is.EqualTo("05"));
        }

        [Test]
        public void MinutesToString_ReturnsMinutesWithAdditionalInfo()
        {
            var minute = new TimetableItem.TimetableItemMinutes { Minutes = 15, AdditionalInfo = "A" };

            Assert.That(minute.ToString(), Is.EqualTo("15A"));
        }
    }
}
