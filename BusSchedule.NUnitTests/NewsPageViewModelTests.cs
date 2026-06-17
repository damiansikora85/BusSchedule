using BusSchedule.Core.Model;
using BusSchedule.Core.Services;
using BusSchedule.Core.UI.Pages;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusSchedule.NUnitTests
{
    [TestFixture]
    public class NewsPageViewModelTests
    {
        [Test]
        public async Task RefreshView_PopulatesNewsAndRaisesNotifications()
        {
            var newsServiceMock = new Mock<INewsService>();
            var expectedNews = new List<News>
            {
                new News { Title = "A", Message = "Text" },
                new News { Title = "B", Message = "Other" }
            };

            newsServiceMock.Setup(x => x.GetNews(false)).ReturnsAsync(expectedNews);

            var viewModel = new NewsPageViewModel(newsServiceMock.Object);
            var changedProperties = new List<string>();
            viewModel.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName);

            await viewModel.RefreshView();

            Assert.That(viewModel.HasAnyNews, Is.True);
            Assert.That(changedProperties, Does.Contain(nameof(NewsPageViewModel.News)));
            Assert.That(changedProperties, Does.Contain(nameof(NewsPageViewModel.HasAnyNews)));
            newsServiceMock.Verify(x => x.GetNews(false), Times.Once);
        }
    }
}
