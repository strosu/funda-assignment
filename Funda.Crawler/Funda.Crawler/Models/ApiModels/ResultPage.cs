using System;
using Newtonsoft.Json;

namespace Funda.Crawler.Models
{
    public class ResultPage
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

