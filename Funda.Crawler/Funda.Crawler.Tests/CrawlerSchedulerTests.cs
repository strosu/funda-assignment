using Funda.Crawler.Models;
using Funda.Crawler.Services;
using Moq;
using Xunit;

namespace Funda.Crawler.Tests
{
    public class CrawlerSchedulerTests
    {
        [Fact]
        public async void CrawlerScheduler_SplitsPages_StartsCrawlers()
        {
            var requestService = new Mock<IRequestService<ResultPage>>();
            
            requestService.Setup(x => x.GetPageResultAsync("page1")).Returns(Task.FromResult(new ResultPage
            {
                Paging = new PageInformation
                {
                    TotalPageNumber = 4
                }
            }));

            var crawler = new Mock<ICrawler>();

            crawler.Setup(x => x.GetListingsSeriallyAsync(new List<string> { "page1", "page4"})).Returns(
                Task.FromResult(new List<Listing>() 
                { RandomModels.Listing1 }.AsEnumerable()));

            crawler.Setup(x => x.GetListingsSeriallyAsync(new List<string> { "page2"})).Returns(
                Task.FromResult(new List<Listing>()
                { RandomModels.Listing2 }.AsEnumerable()));

            crawler.Setup(x => x.GetListingsSeriallyAsync(new List<string> { "page3" })).Returns(
                Task.FromResult(new List<Listing>()
                { RandomModels.Listing3 }.AsEnumerable()));

            var crawlerFactory = new Mock<ICrawlerFactory>();

            // Returns the same crawler every time
            crawlerFactory.Setup(x => x.GetNewCrawler()).Returns(crawler.Object);

            var scheduler = new CrawlerScheduler(crawlerFactory.Object, requestService.Object);

            var result = await scheduler.GetListingsAsync("page{0}", 3);
            Assert.Equal(result, new List<Listing> { RandomModels.Listing1, RandomModels.Listing2, RandomModels.Listing3 });
        }
    }
}
