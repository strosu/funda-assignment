using System;
using System.Diagnostics;
using Funda.Crawler.Extensions;
using Funda.Crawler.Models;
using Funda.Crawler.Services;
using Newtonsoft.Json;

namespace Funda.Crawler
{
    public interface ICrawler
    {
        Task<IEnumerable<Listing>> GetListingsAsync(string urlTemplate);

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
        private readonly IRequestService<ResultList> _requestService;

        public SerialCrawler(IRequestService<ResultList> requestService)
        {
            _requestService = requestService;
        }

        /// <summary>
        /// Takes a URL template and retries all the listings
        /// This implementation does not consume the NextPageUrl, instead it fills in the page number
        /// </summary>
        /// <param name="urlTemplate"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Listing>> GetListingsAsync(string urlTemplate)
        {
            var firstPage = await GetPageResults(urlTemplate, 1);
            var pageInfo = firstPage.Paging;

            // If we're dealing with just 1 page, we're done here
            if (firstPage.IsLastPage())
            {
                return firstPage.Listings;
            }

            var taskList = new List<Task<ResultList>>();
            for (var i = 2; i < pageInfo.TotalPageNumber; i++)
            {
                taskList.Add(GetPageResults(urlTemplate, i));
            }

            await Task.WhenAll(taskList);

            // TODO - Load the first page again to see if anything else was added
            // This would be essential if the list of listings would be likely to change during the execution

            return GetUniqueListings(taskList.Select(x => x.Result));
        }

        private IEnumerable<Listing> GetUniqueListings(IEnumerable<ResultList> listings)
        {
            var result = new HashSet<Listing>();
            foreach (var page in listings)
            {
                var currentPage = page.Listings;
                foreach (var listing in currentPage)
                {
                    result.Add(listing);
                }
            }

            return result;
        }

        private async Task<ResultList> GetPageResults(string urlTemplate, int pageIndex)
        {
            return await _requestService.GetPageResult(string.Format(urlTemplate, pageIndex));
        }

        public async Task<IEnumerable<Listing>> GetListingsSeriallyAsync(IEnumerable<string> pageUrls)
        {
            var results = new List<ResultList>();

            foreach (var url in pageUrls)
            {
                results.Add(await _requestService.GetPageResult(url));
            }

            return GetUniqueListings(results);
        }
    }
}

