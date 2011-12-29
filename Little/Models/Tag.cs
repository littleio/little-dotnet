using Newtonsoft.Json;

namespace Little
{
   public class Tag
   {
      [JsonProperty("_id")]
      public string Id { get; set; }
      public string User { get; set; }
      public string Asset { get; set; }
      public int Type { get; set; }
      public bool Share { get; set; }
   }
}