using Newtonsoft.Json;

namespace Little
{
   public class LoginFailureRate
   {
      [JsonProperty("half")]
      public int Last30Seconds { get; set; }
      [JsonProperty("one")]
      public int LastMinute { get; set; }
      [JsonProperty("three")]
      public int Last3Minutes { get; set; }
      [JsonProperty("five")]
      public int Last5Minutes { get; set; }
   }
}