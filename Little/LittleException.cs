using System;
using Newtonsoft.Json;

namespace Little
{
   public class LittleException : Exception
   {
      public LittleException() { }
      public LittleException(string message) : base(message) { }
      public LittleException(string message, Exception innerException) : base(message, innerException) { }
   }

   public class ErrorMessage
   {
      [JsonProperty("error")]
      public string Message { get; internal set; }
      [JsonProperty("info")]
      public string Info { get; internal set; }
      [JsonProperty("maintenance")]
      public string Maintenance { get; internal set; }
      public Exception InnerException { get; internal set; }
   }
}