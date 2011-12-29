using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Little
{
   public class Communicator
   {
      public const string Get = "GET";
      public const string Post = "POST";

      private readonly IRequestContext _context;

      public Communicator(IRequestContext context)
      {
         _context = context;
      }

      public void Send(string method, string resource, IDictionary<string, object> partialPayload, params string[] signatureKeys)
      {
         using (SendPayload(method, resource, partialPayload, signatureKeys)){ }
      }
      public T Send<T>(string method, string resource, IDictionary<string, object> partialPayload, params string[] signatureKeys)
      {
         using (var response = SendPayload(method, resource, partialPayload, signatureKeys))
         {
            try
            {
               var body = GetResponseBody(response);
               return string.IsNullOrEmpty(body) ? default(T) : JsonConvert.DeserializeObject<T>(body);
            }
            catch (Exception e)
            {
               throw HandleException(e);
            }
         }
      }

      private HttpWebResponse SendPayload(string method, string resource, IDictionary<string, object> partialPartialPayload, params string[] signatureKeys)
      {
         var isGet = method == Get;
         var url = string.Concat(DriverConfiguration.Data.Url, _context.ApiVersion, "/", resource);
         var payload = FinalizePayload(partialPartialPayload, resource, signatureKeys);
         if (isGet) { url += '?' + payload; }
         var request = (HttpWebRequest)WebRequest.Create(url);
         request.Method = method;
         request.UserAgent = "little-csharp";
         request.Timeout = 10000;
         request.ReadWriteTimeout = 10000;
         request.KeepAlive = false;

         if (!isGet)
         {
            WriteResponse(request, payload);
         }
         try
         {
            return (HttpWebResponse)request.GetResponse();
         }
         catch (Exception e)
         {
            throw HandleException(e);
         }
         
      }

      private static void WriteResponse(WebRequest request, string payload)
      {
         var data = Encoding.UTF8.GetBytes(payload);
         using (var stream = request.GetRequestStream())
         {
            stream.Write(data, 0, data.Length);
            stream.Flush();
            stream.Close();
         }
      }

      private string FinalizePayload(IDictionary<string, object> payload, string resource, string[] signatureKeys)
      {
         payload.Add("key", _context.Key);
         if (signatureKeys != null && signatureKeys.Length > 0)
         {
            payload.Add("sig", GetSignature(payload, _context.Secret, resource, signatureKeys));
         }
         var sb = new StringBuilder();
         foreach (var kvp in payload)
         {
            if (kvp.Value == null) { continue; }
            var valueType = kvp.Value.GetType();
            if (!typeof(string).IsAssignableFrom(valueType) && typeof(IEnumerable).IsAssignableFrom(valueType))
            {
               sb.Append(Serialize(kvp.Key, (IEnumerable) kvp.Value));
            }
            else
            {
               sb.Append(SerializeParameter(kvp.Key, kvp.Value.ToString()));
            }
         }
         return sb.Remove(sb.Length - 1, 1).ToString();
      }

      private static string Serialize(string key, IEnumerable values)
      {
         var sb = new StringBuilder();
         key = string.Concat(key, "[]");
         foreach(var value in values)
         {
            sb.Append(SerializeParameter(key, value.ToString()));
         }
         return sb.ToString();
      }

      private static string SerializeParameter(string key, string value)
      {
         return string.Concat(key, '=', Uri.EscapeDataString(value), '&');
      }

      public static string GetSignature(IEnumerable<KeyValuePair<string, object>> parameters, string secret, string resource, params string[] signatureKeys)
      {
         var sorted = SortParameterForSignature(parameters, signatureKeys);
         var sb = new StringBuilder();
         foreach (var parameter in sorted)
         {
            sb.AppendFormat("{0}|{1}|", parameter.Key, parameter.Value);
         }
         sb.Append(secret);
         sb.Append('|');
         sb.Append(resource);
         using (var hasher = new SHA1Managed())
         {
            var bytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
            var data = new StringBuilder(bytes.Length * 2);
            for (var i = 0; i < bytes.Length; ++i)
            {
               data.Append(bytes[i].ToString("x2"));
            }
            return data.ToString();
         }
      }

      private static IEnumerable<KeyValuePair<string, object>> SortParameterForSignature(IEnumerable<KeyValuePair<string, object>> payload, string[] signatureKeys)
      {
         var keys = new HashSet<string>(signatureKeys);
         var parameters = new SortedDictionary<string, object>();
         foreach (var kvp in payload)
         {
            if (!keys.Contains(kvp.Key) && kvp.Key != "key") { continue; }
            if (kvp.Value == null) { continue; }
            var valueType = kvp.Value.GetType();
            if (typeof(string).IsAssignableFrom(valueType))
            {
               parameters.Add(kvp.Key, kvp.Value);
            }
            else if (typeof(int).IsAssignableFrom(valueType) || typeof(long).IsAssignableFrom(valueType))
            {
               parameters.Add(kvp.Key, kvp.Value.ToString());
            }
            else if (typeof(bool).IsAssignableFrom(valueType))
            {
               parameters.Add(kvp.Key, (bool)kvp.Value ? "true" : "false");
            }
         }
         return parameters;
      }

      private static LittleException HandleException(Exception exception)
      {         
         if (exception is WebException)
         {
            var response = ((WebException)exception).Response;
            if (response == null)
            {
               return new LittleException("null response", exception);
            }
            var body = GetResponseBody(response);
            try
            {
               return new LittleException(body, exception);
            }
            catch (Exception)
            {
               return new LittleException(body, exception);
            }
         }
         return new LittleException("unknown error", exception);
      }

      private static string GetResponseBody(WebResponse response)
      {
         using (var stream = response.GetResponseStream())
         {
            var sb = new StringBuilder();
            int read;
            var bufferSize = response.ContentLength == -1 ? 2048 : (int)response.ContentLength;
            if (bufferSize == 0) { return null; }
            do
            {
               var buffer = new byte[2048];
               read = stream.Read(buffer, 0, buffer.Length);
               sb.Append(Encoding.UTF8.GetString(buffer, 0, read));
            } while (read > 0);
            return sb.ToString();
         }
      }
   }
}