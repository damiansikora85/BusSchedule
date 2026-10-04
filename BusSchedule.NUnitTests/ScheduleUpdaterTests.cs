using System;
using System.Threading.Tasks;
using Moq;
using BusSchedule.Core.Services;
using BusSchedule.Core.CloudService;
using Xamarin.Plugin.Firebase;
using NUnit.Framework; // jeśli inna przestrzeń nazw, dostosuj
// using namespace dla IFileAccess i IPreferences jeśli są w innych miejscach

namespace BusSchedule.Core.Tests.Services
{
    public class ScheduleUpdaterTests
    {
        [Test]
        public async Task TryUpdateSchedule_ReturnsTrue_WhenNewFilenameDownloaded()
        {
            // Arrange
            var cloudServiceMock = new Mock<ICloudService>();
            var firebaseStorageMock = new Mock<IFirebaseStorage>();
            var preferencesMock = new Mock<IPreferences>();
            var fileAccessMock = new Mock<IFileAccess>();

            var defaultDbFilename = "default.db";
            var latestFilename = "new.db";
            var downloadedPath = "C:\\temp\\new.db";

            // last update was long time ago
            preferencesMock
                .Setup(p => p.Get("lastScheduleUpdate", It.IsAny<DateTime>()))
                .Returns(DateTime.Now.AddDays(-2));

            // current stored filename is different -> should trigger update
            preferencesMock
                .Setup(p => p.Get("dbFilename", defaultDbFilename))
                .Returns("old.db");

            cloudServiceMock
                .Setup(c => c.GetLatestScheduleFilename())
                .ReturnsAsync(latestFilename);

            firebaseStorageMock
                .Setup(f => f.DownloadFileToLocalStorage("/" + latestFilename))
                .ReturnsAsync(downloadedPath);

            fileAccessMock
                .Setup(f => f.CopyToLocal(downloadedPath, latestFilename))
                .Returns(Task.FromResult(true));

            // Act
            var updater = new ScheduleUpdater(cloudServiceMock.Object, firebaseStorageMock.Object, preferencesMock.Object);
            var result = await updater.TryUpdateSchedule(fileAccessMock.Object, defaultDbFilename);

            // Assert
            Assert.True(result);

            // dodatkowe asercje (opcjonalne) - sprawdź, że plik został skopiowany i pref. ustawione
            fileAccessMock.Verify(f => f.CopyToLocal(downloadedPath, latestFilename), Times.Once);
            preferencesMock.Verify(p => p.Set("dbFilename", latestFilename), Times.Once);
            preferencesMock.Verify(p => p.Set("lastScheduleUpdate", It.IsAny<DateTime>()), Times.Once);
        }
    }
}