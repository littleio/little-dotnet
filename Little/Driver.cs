using System;
using System.Collections.Generic;
using System.Linq;

namespace Little
{
   public class Driver : IDriver, IRequestContext
   {
      public const string Version = "v1";
      public string Key { get; private set; }
      public string Secret { get; private set; }
      public string ApiVersion
      {
         get { return Version; }
      }

      public Driver(string key, string secret)
      {
         Key = key;
         Secret = secret;
      }

      public void Like(string user, string asset, int type)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "asset", asset }, { "type", type } };
         new Communicator(this).Send(Communicator.Post, "likes", payload, "user");
      }

      public string LikeSignature(string user)
      {
         return Communicator.GetSignature(new Dictionary<string, object> {{"user", user}, {"key", Key}}, Secret, "likes", "user");
      }

      public bool DoesUserLikeAsset(string user, string asset, int type)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "asset", asset }, { "type", type } };
         return new Communicator(this).Send<bool>(Communicator.Get, "likes", payload);
      }

      public ICollection<Like> UserLikes(string user, int page, int records)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "page", page }, { "records", records } };
         return new Communicator(this).Send<ICollection<Like>>(Communicator.Get, "likes", payload);
      }

      public int UserLikeCount(string user)
      {
         var payload = new Dictionary<string, object> { { "user", user }, {"count", 1}};
         return new Communicator(this).Send<CountContainer>(Communicator.Get, "likes", payload).Count;
      }

      public ICollection<string> AssetLikedBy(string asset, int type, int page, int records)
      {
         var payload = new Dictionary<string, object> { { "asset", asset }, { "type", type }, { "page", page }, { "records", records } };
         return new Communicator(this).Send<ICollection<Like>>(Communicator.Get, "likes", payload).Select(l => l.User).ToList();
      }

      public int AssetLikedCount(string asset, int type)
      {
         var payload = new Dictionary<string, object> { { "asset", asset }, { "type", type }, { "count", 1 } };
         return new Communicator(this).Send<CountContainer>(Communicator.Get, "likes", payload).Count;
      }

      public ICollection<LikeTypeGroup> LikedAssetsByType(int type, int page, int records)
      {
         var payload = new Dictionary<string, object> { { "type", type }, { "page", page }, { "records", records } };
         return new Communicator(this).Send<ICollection<LikeTypeGroup>>(Communicator.Get, "likes", payload);
      }

      public int LikedAssetsByTypeCount(int type)
      {
         var payload = new Dictionary<string, object> { { "type", type }, { "count", 1 } };
         return new Communicator(this).Send<CountContainer>(Communicator.Get, "likes", payload).Count;
      }

      public LoginFailureRate LoginAttempt(string user, string ipAddress, bool success)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "ip", ipAddress }, {"ok", success ? 1 : 0} };
         return new Communicator(this).Send<LoginFailureRate>(Communicator.Post, "attempts", payload, "user", "ip", "ok");
      }

      public LoginAttempt PreviousSuccessfulLoginAttempt(string user)
      {
         var payload = new Dictionary<string, object> { { "user", user } };
         return new Communicator(this).Send<LoginAttempt>(Communicator.Get, "attempts", payload, "user");
      }

      public ICollection<LoginAttempt> LoginAttempts(string user, int count)
      {
         var payload = new Dictionary<string, object> { { "user", user }, {"count", count} };
         return new Communicator(this).Send<ICollection<LoginAttempt>>(Communicator.Get, "attempts", payload, "user");
      }
   }
}