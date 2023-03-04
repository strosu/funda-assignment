using System;
using Funda.Crawler.Models;

namespace Funda.Crawler
{
    public class AgentFinder
    {
        private readonly ICrawler _crawler;
        private readonly IResultFormatter _resultFormatter;
        private readonly ITimedOperation _timedOperation;

        public AgentFinder(ICrawler crawler, IResultFormatter resultFormatter, ITimedOperation timedOperation)
        {
            _crawler = crawler;
            _resultFormatter = resultFormatter;
            _timedOperation = timedOperation;
        }

        public async Task GetAndDisplayProlificAgentsAsync(string urlTemplate)
        {
            var results = await _timedOperation.TimeOperationAsync(async () => await _crawler.GetListingsAsync(urlTemplate));
            _resultFormatter.DisplayResults(results);
        }
    }


}

