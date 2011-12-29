using Newtonsoft.Json;

namespace Little
{
   public class IdContainer
   {
      [JsonProperty("_id")]
      public string Id { get; set; }
   }
}