using Funda.Crawler.Models;
using Funda.Crawler.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Funda.Crawler
{
    public interface ICrawlerFactory
    {
        ICrawler GetNewCrawler();
    }

    public class CrawlerFactory : ICrawlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CrawlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICrawler GetNewCrawler()
        {
            var requestService = _serviceProvider.GetService<IRequestService<ResultPage>>();
            return new SerialCrawler(requestService);
        }
    }
}

