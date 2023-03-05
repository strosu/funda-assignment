using System;
using Newtonsoft.Json;

namespace Funda.Crawler.Models
{
    public class ResultList
    {
        [JsonProperty("Objects")]
        public List<Listing> Listings { get; set; }

        /// <summary>
        /// Describes the total number of objects to be returned, across all pages
        /// </summary>
        [JsonProperty("TotaalAantalObjecten")]
        public int TotalListings { get; set; }

        public PageInformation Paging { get; set; }
    }

    public class Listing : IEquatable<Listing>
    {
        public int GlobalId { get; set; }

        public string Id { get; set; }

        [JsonProperty("MakelaarId")]
        public int SellerId { get; set; }

        [JsonProperty("MakelaarNaam")]
        public string SellerName { get; set; }

        public bool Equals(Listing? other)
        {
            if (other == null)
            {
                return false;
            }

            return Id == other.Id;
        }
    }

    public class PageInformation
    {
        /// <summary>
        /// Total number of pages
        /// </summary>
        [JsonProperty("AantalPaginas")]
        public int TotalPageNumber { get; set; }

        /// <summary>
        /// Current page number
        /// </summary>
        [JsonProperty("HuidigePagina")]
        public int CurrentPageNumber { get; set; }

        /// <summary>
        /// Next page relative URL
        /// </summary>
        [JsonProperty("VolgendeUrl")]
        public string NextPageUrl { get; set; }
    }
}

