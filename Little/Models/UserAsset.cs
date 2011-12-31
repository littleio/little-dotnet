using Newtonsoft.Json;

namespace Little
{
   public class UserAsset
   {
      public string User { get; set; }
      public string Asset { get; set; }
      public int Type { get; set; }
      public bool? Vote { get; set; }
      [JsonProperty("rate")]
      public int? Rating { get; set; }
   }
}