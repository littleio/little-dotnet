using Newtonsoft.Json;

namespace Little
{
   public class Notification
   {
      [JsonProperty("_id")]
      public string Id { get; set; }

      public string Body { get; set; }
   }
}