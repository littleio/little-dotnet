using System;
using Newtonsoft.Json;

namespace Little
{
   public class LoginAttempt
   {
      [JsonProperty("ip")]
      public string IpAddress { get; set; }
      [JsonProperty("ok")]
      public bool Success { get; set; }
      [JsonProperty("ts")]
      public DateTime Timestamp { get; set; }
      [JsonProperty("c")]
      public string Country { get; set; }
   }
}