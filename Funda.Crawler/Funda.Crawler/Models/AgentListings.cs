using System;
namespace Funda.Crawler.Models
{
    public class AgentListings
    {
        public string AgentName { get; set; }

        public IEnumerable<Listing> Listings { get; set; }
    }
}

