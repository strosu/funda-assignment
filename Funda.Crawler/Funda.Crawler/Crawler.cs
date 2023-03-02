using System;
using Funda.Crawler.Extensions;
using Funda.Crawler.Models;
using Funda.Crawler.Services;
using Newtonsoft.Json;

namespace Funda.Crawler
{
    public class Crawler
    {
        private readonly IRequestService<ResultList> _requestService;

        public Crawler(IRequestService<ResultList> requestService)
        {
            _requestService = requestService;
        }

        public async Task<IEnumerable<Listing>> GetListings(string urlTemplate)
        {
            var result = new HashSet<Listing>();

            ResultList currentResultPage = null;
            var currentPageIndex = 1;
            do
            {
                currentResultPage = await GetPageResults(urlTemplate, currentPageIndex);
                foreach (var listing in currentResultPage?.Objects)
                {
                    result.Add(listing);
                }

                currentPageIndex++;
            }
            while (!currentResultPage.IsLastPage());


            // Load the first page again to see if anything else was added

            return result;
        }

        private async Task<ResultList> GetPageResults(string urlTemplate, int pageIndex)
        {
            Console.WriteLine($"Processing page {pageIndex}");

            return await _requestService.GetPageResult(string.Format(urlTemplate, pageIndex));
        }
    }
}

