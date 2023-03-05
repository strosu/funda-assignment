using Funda.Crawler.Models;
using Funda.Crawler.Services;
using Moq;
using Xunit;

namespace Funda.Crawler.Tests
{
    public class CrawlerTests
    {
        [Fact]
        public async Task Crawler_GetsListOfUrls_CrawlsAll()
        {
            var requestServiceMock = new Mock<IRequestService<ResultPage>>();
            var crawler = new SerialCrawler(requestServiceMock.Object);

            requestServiceMock.Setup(x => x.GetPageResultAsync("first")).Returns(Task.FromResult(new ResultPage
            {
                Listings = new List<Listing> { RandomModels.Listing1, RandomModels.Listing2 }
            }));

            requestServiceMock.Setup(x => x.GetPageResultAsync("second")).Returns(Task.FromResult(new ResultPage
            {
                Listings = new List<Listing> { RandomModels.Listing2, RandomModels.Listing3 }
            }));

            var urls = new List<string> { "first", "second" };

            var result = await crawler.GetListingsSeriallyAsync(urls);

            Assert.Equal(result, new List<Listing> { RandomModels.Listing1, RandomModels.Listing2, RandomModels.Listing3 });
        }
    }
}
