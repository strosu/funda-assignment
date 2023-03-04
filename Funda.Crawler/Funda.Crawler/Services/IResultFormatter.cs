using Funda.Crawler.Models;

namespace Funda.Crawler
{
    public interface IResultFormatter
    {
        void DisplayResults(IEnumerable<Listing> listings);
    }

    public class TopTenFormatter : IResultFormatter
    {
        public void DisplayResults(IEnumerable<Listing> listings)
        {
            var mostListings = listings.GroupBy(x => (x.SellerId, x.SellerName)).Select(x => new AgentListings
            {
                AgentName = x.Key.SellerName,
                Listings = x
            }).ToList().OrderByDescending(x => x.Listings.Count()).Take(10);

            Console.WriteLine("Results:");

            foreach (var prolificAgent in mostListings)
            {
                Console.WriteLine($"{prolificAgent.AgentName} - {prolificAgent.Listings.Count()}");
            }
        }
    }
}

