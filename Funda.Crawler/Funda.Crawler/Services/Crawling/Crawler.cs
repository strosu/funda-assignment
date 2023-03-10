using Funda.Crawler.Models;
using Funda.Crawler.Services;

namespace Funda.Crawler
{
    public interface ICrawler
    {
        /// <summary>
        /// Takes a list of pages to retrieve and gets the results for all of them
        /// In a more complex system, this would be a separate service with its own queue. The producer end would pipe urls into it, and this service (the consumer) would send the queries
        /// </summary>
        /// <param name="pageUrls"></param>
        /// <returns></returns>
        Task<IEnumerable<Listing>> GetListingsSeriallyAsync(IEnumerable<string> pageUrls);
    }

    public class SerialCrawler : ICrawler
    {
        private readonly IRequestService<ResultPage> _requestService;

        public SerialCrawler(IRequestService<ResultPage> requestService)
        {
            _requestService = requestService;
        }

        public async Task<IEnumerable<Listing>> GetListingsSeriallyAsync(IEnumerable<string> pageUrls)
        {
            var results = new HashSet<Listing>();

            foreach (var url in pageUrls)
            {
                var pageResults = await _requestService.GetPageResultAsync(url);
                foreach (var listing in pageResults.Listings)
                {
                    results.Add(listing);
                }
            }

            return results;
        }
    }
}

