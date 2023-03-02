using System;
namespace Funda.Crawler.Models
{
    public class ResultList
    {
        public List<Listing> Objects { get; set; }

        /// <summary>
        /// Describes the total number of objects to be returned, across all pages
        /// </summary>
        public int TotaalAantalObjecten { get; set; }

        public PageInformation Paging { get; set; }
    }

    public class Listing : IEquatable<Listing>
    {
        public int GlobalId { get; set; }

        public string Id { get; set; }

        public int MakelaarId { get; set; }

        public string MakelaarNaam { get; set; }

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
        public int AantalPaginas { get; set; }

        /// <summary>
        /// Current page number
        /// </summary>
        public int HuidigePagina { get; set; }

        /// <summary>
        /// Next page relative URL
        /// </summary>
        public string VolgendeUrl { get; set; }
    }
}

