using Funda.Crawler.Models;
using Funda.Crawler.Services;

namespace Funda.Crawler
{
    public interface IResultFormatter
    {
        void DisplayResults(IEnumerable<Listing> listings);
    }

    public class TopTenFormatter : IResultFormatter
    {
        private readonly ILogger _logger;

        public TopTenFormatter(ILogger logger)
        {
            _logger = logger;
        }

        public void DisplayResults(IEnumerable<Listing> listings)
        {
            var mostListings = listings.GroupBy(x => (x.SellerId, x.SellerName)).Select(x => new AgentListings
            {
                AgentName = x.Key.SellerName,
                Listings = x
            }).ToList().OrderByDescending(x => x.Listings.Count()).Take(10);

            _logger.Log("Results:");

            foreach (var prolificAgent in mostListings)
            {
                _logger.Log($"{prolificAgent.AgentName} - {prolificAgent.Listings.Count()}");
            }
        }
    }
}

