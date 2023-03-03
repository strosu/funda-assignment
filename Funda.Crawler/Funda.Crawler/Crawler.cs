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

            var firstPage = await GetPageResults(urlTemplate, 1);
            var pageInfo = firstPage.Paging;

            if (firstPage.IsLastPage())
            {
                return firstPage.Listings;
            }

            var taskList = new List<Task<ResultList>>();
            for (var i = 2; i < pageInfo.TotalpageNumber; i++)
            {
                taskList.Add(GetPageResults(urlTemplate, i));
            }

            await Task.WhenAll(taskList);

            foreach (var task in taskList)
            {
                var currentPage = task.Result.Listings;
                foreach (var listing in currentPage)
                {
                    result.Add(listing);
                }
            }

            //ResultList currentResultPage = null;
            //var currentPageIndex = 1;
            //do
            //{
            //    currentResultPage = await GetPageResults(urlTemplate, currentPageIndex);
            //    foreach (var listing in currentResultPage?.Objects)
            //    {
            //        result.Add(listing);
            //    }

            //    currentPageIndex++;
            //}
            //while (!currentResultPage.IsLastPage());


            // Load the first page again to see if anything else was added

            return result;
        }

        private async Task<ResultList> GetPageResults(string urlTemplate, int pageIndex)
        {
            Console.WriteLine($"Processing page {pageIndex}");

            var result = await _requestService.GetPageResult(string.Format(urlTemplate, pageIndex));

            Console.WriteLine($"Finished getting page {pageIndex}");

            return result;
        }
    }
}

