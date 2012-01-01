using System.Collections.Generic;
using System.Linq;

namespace Little
{
   public interface IDriver
   {
      /// <summary>
      /// Returns the version of the API this library understands
      /// </summary
      string ApiVersion { get; }

      /// <summary>
      /// Methods to deal with assets
      /// </summary>
      IAssetDriver Asset { get; }

      /// <summary>
      /// Methods to deal with tags
      /// </summary>
      ITagDriver Tag { get; }

      /// <summary>
      /// Methods to deal with notifications
      /// </summary>
      INotificationDriver Notification { get; }

      /// <summary>
      /// Methods to deal with login attempts
      /// </summary>
      IUserDriver User { get; }
   }

   public class Driver : IDriver, IRequestContext
   {
      public static readonly string[] BlankSigature = new string[0];

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
         Asset = new AssetDriver(this);
         Tag = new TagDriver(this);
         Notification = new NotificationDriver(this);
         User = new UserDriver(this);
      }

      public IAssetDriver Asset { get; private set; }
      public ITagDriver Tag { get; private set; }
      public INotificationDriver Notification { get; private set; }
      public IUserDriver User { get; private set; }


      public bool DoesUserLikeAsset(string user, string asset, int type)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "asset", asset }, { "type", type } };
         return new Communicator(this).Send<bool>(Communicator.Get, "likes", null, payload);
      }

      public ICollection<UserAsset> UserLikes(string user, int page, int records)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "page", page }, { "records", records } };
         return new Communicator(this).Send<ICollection<UserAsset>>(Communicator.Get, "likes", null, payload);
      }

      public int UserLikeCount(string user)
      {
         var payload = new Dictionary<string, object> { { "user", user }, {"count", 1}};
         return new Communicator(this).Send<CountContainer>(Communicator.Get, "likes", null, payload).Count;
      }

      public ICollection<string> AssetLikedBy(string asset, int type, int page, int records)
      {
         var payload = new Dictionary<string, object> { { "asset", asset }, { "type", type }, { "page", page }, { "records", records } };
         return new Communicator(this).Send<ICollection<UserAsset>>(Communicator.Get, "likes", null, payload).Select(l => l.User).ToList();
      }

      public int AssetLikedCount(string asset, int type)
      {
         var payload = new Dictionary<string, object> { { "asset", asset }, { "type", type }, { "count", 1 } };
         return new Communicator(this).Send<CountContainer>(Communicator.Get, "likes", null, payload).Count;
      }

      public ICollection<LikeTypeGroup> LikedAssetsByType(int type, int page, int records)
      {
         var payload = new Dictionary<string, object> { { "type", type }, { "page", page }, { "records", records } };
         return new Communicator(this).Send<ICollection<LikeTypeGroup>>(Communicator.Get, "likes", null, payload);
      }

      public int LikedAssetsByTypeCount(int type)
      {
         var payload = new Dictionary<string, object> { { "type", type }, { "count", 1 } };
         return new Communicator(this).Send<CountContainer>(Communicator.Get, "likes", null, payload).Count;
      }
   }
}