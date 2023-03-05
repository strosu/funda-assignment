using Funda.Crawler.Models;
using Moq;
using Xunit;

namespace Funda.Crawler.Tests
{
    public class AgentCrawlerTests
    {
        [Fact]
        public async Task AgentFinder_GetsListings_AndDisplaysThem()
        {
            var scheduler = new Mock<ICrawlerScheduler>();
            var formatter = new Mock<IResultFormatter>();
            var timer = new Mock<ITimedOperation>();

            IEnumerable<Listing> mockListings = new List<Listing>()
            {
                new Listing
                {
                    SellerId = 1,
                    SellerName = "1",
                    Id = "5"
                },
                new Listing
                {
                    SellerId = 1,
                    SellerName = "1",
                    Id = "6"
                },
                new Listing
                {
                    SellerId = 2,
                    SellerName = "2",
                    Id = "7"
                }
            };

            timer.ConfigureAsPassthrough();

            scheduler.Setup(x => x.GetListingsAsync("template {0}", 5)).Returns(Task.FromResult(mockListings));

            var agent = new AgentFinder(scheduler.Object, formatter.Object, timer.Object);
            await agent.GetAndDisplayProlificAgentsAsync("template {0}", 5);

            scheduler.Verify(x => x.GetListingsAsync("template {0}", 5), Times.Once);
            formatter.Verify(x => x.DisplayResults(mockListings), Times.Once);
        }
    }
}

