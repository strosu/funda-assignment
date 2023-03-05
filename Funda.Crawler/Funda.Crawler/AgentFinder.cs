namespace Funda.Crawler
{
    public class AgentFinder
    {
        private readonly ICrawlerScheduler _crawlerScheduler;
        private readonly IResultFormatter _resultFormatter;
        private readonly ITimedOperation _timedOperation;

        public AgentFinder(ICrawlerScheduler crawlerScheduler, IResultFormatter resultFormatter, ITimedOperation timedOperation)
        {
            _crawlerScheduler = crawlerScheduler;
            _resultFormatter = resultFormatter;
            _timedOperation = timedOperation;
        }

        public async Task GetAndDisplayProlificAgentsAsync(string urlTemplate, int degreeOfParallelism)
        {
            var results = await _timedOperation.TimeOperationAsync(async () => await _crawlerScheduler.GetListingsAsync(urlTemplate, degreeOfParallelism));
            _resultFormatter.DisplayResults(results);
        }
    }


}

