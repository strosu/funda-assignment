using Funda.Crawler.Models;
using Funda.Crawler.Services;

namespace Funda.Crawler
{
    /// <summary>
    /// Orchestrates multiple crawlers in order to parallelize some of the requests
    /// </summary>
    public interface ICrawlerScheduler
    {
        Task<IEnumerable<Listing>> GetListingsAsync(string urlTemplate, int degreeOfParallelism);
    }

    public class CrawlerScheduler : ICrawlerScheduler
    {
        private readonly IRequestService<ResultPage> _requestService;
        private readonly ICrawlerFactory _crawlerFactory;

        public CrawlerScheduler(ICrawlerFactory crawlerFactory, IRequestService<ResultPage> requestService)
        {
            _crawlerFactory = crawlerFactory;
            _requestService = requestService;
        }

        public async Task<IEnumerable<Listing>> GetListingsAsync(string urlTemplate, int degreeOfParallelism)
        {
            var pageInformation = await GetNumberOfPages(urlTemplate);
            var numberOfPages = pageInformation.TotalPageNumber;

            var taskList = new List<Task<IEnumerable<Listing>>>();
            var bucketedUrls = DistributeUrls(degreeOfParallelism, numberOfPages, urlTemplate);

            foreach (var urlList in bucketedUrls)
            {
                var crawler = _crawlerFactory.GetNewCrawler();
                taskList.Add(crawler.GetListingsSeriallyAsync(urlList));
            }

            await Task.WhenAll(taskList);

            // Flatten the list of results
            return taskList.Select(x => x.Result).SelectMany(x => x);
        }

        private IEnumerable<IEnumerable<string>> DistributeUrls(int numberOfCrawlers, int totalPages, string urlTemplate)
        {
            var result = new List<List<string>>();

            // If there are less pages than crawlers, no need to spawn additional ones
            for (var i = 0; i < Math.Min(numberOfCrawlers, totalPages); i++)
            {
                result.Add(new List<string>());
            }

            // Distribute the urls in a round robin fashion, to ensure it's as even as possible
            for (var i = 1; i <= totalPages; i++)
            {
                var pageUrl = BuildPageUrl(urlTemplate, i);
                var crawlerID = (i - 1) % numberOfCrawlers;
                result[crawlerID].Add(pageUrl);
            }

            return result;
        }

        private async Task<PageInformation> GetNumberOfPages(string urlTemplate)
        {
            var firstPage = await _requestService.GetPageResultAsync(BuildPageUrl(urlTemplate, 1));
            return firstPage.Paging;
        }

        private string BuildPageUrl(string urlTemplate, int pageNumber)
        {
            return string.Format(urlTemplate, pageNumber);
        }
    }
}

