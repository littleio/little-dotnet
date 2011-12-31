using Newtonsoft.Json;

namespace Little
{
   public class AssetRank
   {
      public string Asset { get; set; }
      public int Type { get; set; }
      public int Votes { get; set; }
      [JsonProperty("rate")]
      public int Rating { get; set; }
      [JsonProperty("rate_count")]
      public int RatingCount { get; set; }
   }
}