using System.Collections.Generic;
using System.Linq;

namespace Little
{
   public class Driver : IDriver, IRequestContext
   {
      private static readonly string[] _blankSigature = new string[0];

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

      public string LoginAttemptsSignature(string user)
      {
         return Communicator.GetSignature(new Dictionary<string, object> { { "user", user }, { "key", Key } }, Secret, "attempts", "user");
      }

      public Notification Notification(string user, int type)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "type", type } };
         return new Communicator(this).Send<Notification>(Communicator.Get, "notifications", payload);
      }

      public void RespondToNotification(string user, string notificationId, int response)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "notification", notificationId }, { "response", response } };
         new Communicator(this).Send(Communicator.Post, "notifications", payload, "user", "notification");
      }

      public string RespondToNotificationSignature(string user, string notificationId)
      {
         return Communicator.GetSignature(new Dictionary<string, object> { { "user", user }, {"notification", notificationId}, { "key", Key }}, Secret, "user", "notification");
      }

      public string Tag(string user, string asset, int type, bool share)
      {
         return Tag(user, asset, type, share, null);
      }

      public string Tag(string user, string asset, int type, bool share, string data)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "asset", asset }, { "type", type }, {"share", share ? 1 : 0} };
         if (!string.IsNullOrEmpty(data))
         {
            payload["data"] = data;
         }
         return new Communicator(this).Send<IdContainer>(Communicator.Post, "tags", payload, "user").Id;
      }

      public string TagSignature(string user)
      {
         return Communicator.GetSignature(new Dictionary<string, object> { { "user", user }, { "key", Key } }, Secret, "tags", "user");
      }

      public Tag TagById(string id, bool sharedOnly)
      {
         var payload = new Dictionary<string, object> { { "id", id } };
         return new Communicator(this).Send<Tag>(Communicator.Get, "tags", payload, sharedOnly ? _blankSigature : new[]{"id"});
      }

      public ICollection<Tag> UserTags(string user, int page, int records, bool sharedOnly)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "page", page }, { "records", records } };
         return new Communicator(this).Send<ICollection<Tag>>(Communicator.Get, "tags", payload, sharedOnly ? _blankSigature : new[]{"user"});
      }

      public int UserTagCount(string user, bool sharedOnly)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "count", 1 } };
         return new Communicator(this).Send<CountContainer>(Communicator.Get, "tags", payload, sharedOnly ? _blankSigature : new[] { "user" }).Count;
      }

      public ICollection<Tag> UserTags(string user, string asset, int type, int page, int records, bool sharedOnly)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "asset", asset }, { "type", type }, { "page", page }, { "records", records } };
         return new Communicator(this).Send<ICollection<Tag>>(Communicator.Get, "tags", payload, sharedOnly ? _blankSigature : new[] { "user", "asset", "type" });
      }

      public int UserTagCount(string user, string asset, int type, bool sharedOnly)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "asset", asset }, { "type", type }, { "count", 1 } };
         return new Communicator(this).Send<CountContainer>(Communicator.Get, "tags", payload, sharedOnly ? _blankSigature : new[] { "user", "asset", "type" }).Count;
      }

      public string UserTagsSignature(string user)
      {
         return Communicator.GetSignature(new Dictionary<string, object> { { "user", user }, { "key", Key } }, Secret, "tags", "user");
      }

      public string UserTagsSignature(string user, string asset, int type)
      {
         return Communicator.GetSignature(new Dictionary<string, object> { { "user", user }, { "asset", asset }, { "type", type }, { "key", Key } }, Secret, "tags", "user", "asset", "type");
      }

      public ICollection<Tag> AssetTags(string asset, int type, int page, int records)
      {
         var payload = new Dictionary<string, object> { { "asset", asset }, { "type", type }, { "page", page }, { "records", records } };
         return new Communicator(this).Send<ICollection<Tag>>(Communicator.Get, "tags", payload);
      }

      public int AssetTagCount(string asset, int type)
      {
         var payload = new Dictionary<string, object> { { "asset", asset }, { "type", type }, { "count", 1 } };
         return new Communicator(this).Send<CountContainer>(Communicator.Get, "tags", payload).Count;
      }
   }
}