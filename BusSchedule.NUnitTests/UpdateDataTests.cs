using System;
using System.Collections.Generic;
using System.Text;
using BusSchedule.Tools;
using NUnit.Framework;
using Moq;
using BusSchedule.Interfaces;
using System.Threading.Tasks;

namespace BusSchedule.NUnitTests
{
    public class UpdateDataTests
    {
        [Test]
        public async Task FirstLaunchCopyDbTest()
        {
            var fileAccessMock = new Mock<IFileAccess>();
            fileAccessMock.Setup(fa => fa.CheckLocalFileExist(It.IsAny<string>())).Returns(false);
            var preferencesMock = new Mock<IPreferences>();

            await DataUpdater.UpdateDataIfNeeded(fileAccessMock.Object, preferencesMock.Object);
            fileAccessMock.Verify(fa => fa.CopyFromAssetsToLocal(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task UpToDateTest()
        {
            var fileAccessMock = new Mock<IFileAccess>();
            fileAccessMock.Setup(fa => fa.CheckLocalFileExist(It.IsAny<string>())).Returns(true);
            var preferencesMock = new Mock<IPreferences>();

            await DataUpdater.UpdateDataIfNeeded(fileAccessMock.Object, preferencesMock.Object);
            fileAccessMock.Verify(fa => fa.CopyFromAssetsToLocal(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task UpdateDbTest()
        {
            var fileAccessMock = new Mock<IFileAccess>();
            fileAccessMock.Setup(fa => fa.CheckLocalFileExist(It.IsAny<string>())).Returns(true);
            fileAccessMock.Setup(fa => fa.ReadAssetFile(It.IsAny<string>())).ReturnsAsync("2");

            var preferencesMock = new Mock<IPreferences>();
            preferencesMock.Setup(pref => pref.Get(It.IsAny<string>(), It.IsAny<string>())).Returns("1");

            await DataUpdater.UpdateDataIfNeeded(fileAccessMock.Object, preferencesMock.Object);
            fileAccessMock.Verify(fa => fa.CopyFromAssetsToLocal(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
