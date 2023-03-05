using Newtonsoft.Json;
namespace Funda.Crawler.Models
{
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
}