using System;
using System.Collections.Generic;

namespace Little
{
   public interface ITagDriver
   {
      /// <summary>
      /// Tags an asset
      /// </summary>
      /// <param name="user">the user doing the tagging</param>
      /// <param name="asset">the asset being tagged</param>
      /// <param name="type">the asset's type</param>
      /// <param name="share">whether the tag is shared (public) or not</param>
      /// <returns>the tag id</returns>
      string Create(string user, string asset, int type, bool share);

      /// <summary>
      /// Tags an asset with data
      /// </summary>
      /// <param name="user">the user doing the tagging</param>
      /// <param name="asset">the asset being tagged</param>
      /// <param name="type">the asset's type</param>
      /// <param name="share">whether the tag is shared (public) or not</param>
      /// <param name="data">arbitrary data to store with the tag</param>
      /// <returns>the tag id</returns>
      string Create(string user, string asset, int type, bool share, string data);

      /// <summary>
      /// Returns the signature required to tag something
      /// </summary>
      /// <remarks>
      /// This is only useful when combined with the javascript library
      /// </remarks>
      /// <param name="user">the user doing the tagging</param>
      string CreateSignature(string user);

      /// <summary>
      /// Returns the tag by id
      /// </summary>
      /// <param name="id">the tag's id</param>
      /// <param name="sharedOnly">only return the tag if it's shared</param>
      Tag ById(string id, bool sharedOnly);

      /// <summary>
      /// Deletes te tag
      /// </summary>
      /// <param name="id">the id of the tag to delete</param>
      void Delete(string id);


      /// <summary>
      /// Returns all of a user's tags
      /// </summary>
      /// <param name="user">the user doing the tagging</param>
      /// <param name="page">the page to get</param>
      /// <param name="records">the number of records per page</param>
      /// <param name="sharedOnly">only return the tag if it's shared</param>
      ICollection<Tag> ForUser(string user, int page, int records, bool sharedOnly);

      /// <summary>
      /// Returns the number of tags by the user (possibly more than 1 per asset)
      /// </summary>
      /// <param name="user">the user doing the tagging</param>
      /// <param name="sharedOnly">only count the tag if it's shared</param>
      int ForUserCount(string user, bool sharedOnly);

      /// <summary>
      /// Returns all of a user's tags for a specific asset
      /// </summary>
      /// <param name="user">the user doing the tagging</param>
      /// <param name="asset">the asset</param>
      /// <param name="type">the asset's type</param>
      /// <param name="page">the page to get</param>
      /// <param name="records">the number of records per page</param>
      /// <param name="sharedOnly">only return the tag if it's shared</param>
      ICollection<Tag> ForUser(string user, string asset, int type, int page, int records, bool sharedOnly);

      /// <summary>
      /// Returns the number of tags by the user for a specific asset
      /// </summary>
      /// <param name="user">the user doing the tagging</param>
      /// <param name="asset">the asset</param>
      /// <param name="type">the asset's type</param>
      /// <param name="sharedOnly">only count the tag if it's shared</param>
      int ForUserCount(string user, string asset, int type, bool sharedOnly);

      /// <summary>
      /// Returns the signature required to get the user's tags for a specific asset
      /// </summary>
      /// <remarks>
      /// This is only useful when combined with the javascript library
      /// </remarks>
      /// <param name="user">the user to get the tags for</param>
      /// <param name="asset">the asset</param>
      /// <param name="type">the asset's type</param>
      string ForUserSignature(string user, string asset, int type);

      /// <summary>
      /// Returns all of an assets shared tag
      /// </summary>
      /// <param name="asset">the asset</param>
      /// <param name="type">the asset's type</param>
      /// <param name="page">the page to get</param>
      /// <param name="records">the number of records per page</param>
      ICollection<Tag> ForAsset(string asset, int type, int page, int records);

      /// <summary>
      /// Returns the number of shared tags for a specific asset
      /// </summary>
      /// <param name="asset">the asset</param>
      /// <param name="type">the asset's type</param>
      int ForAssetCount(string asset, int type);
   }

   public class TagDriver : ITagDriver
   {
      private readonly IRequestContext _context;

      public TagDriver(IRequestContext context)
      {
         _context = context;
      }

      public string Create(string user, string asset, int type, bool share)
      {
         return Create(user, asset, type, share, null);
      }

      public string Create(string user, string asset, int type, bool share, string data)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "asset", asset }, { "type", type }, { "share", share ? 1 : 0 } };
         if (!string.IsNullOrEmpty(data))
         {
            payload["data"] = data;
         }
         return new Communicator(_context).Send<IdContainer>(Communicator.Post, "tags", null, payload, "user", "asset", "type").Id;
      }

      public string CreateSignature(string user)
      {
         return Communicator.GetSignature(new Dictionary<string, object> { { "user", user }, { "key", _context.Key } }, _context.Secret, "tags", "user");
      }

      public Tag ById(string id, bool sharedOnly)
      {
         var payload = new Dictionary<string, object> { { "id", id } };
         return new Communicator(_context).Send<Tag>(Communicator.Get, "tags", null, payload, sharedOnly ? Driver.BlankSigature : new[] { "id" });
      }

      public void Delete(string id)
      {
         var payload = new Dictionary<string, object> { { "id", id } };
         new Communicator(_context).Send(Communicator.Delete, "tags", null, payload, "id");
      }

      public ICollection<Tag> ForUser(string user, int page, int records, bool sharedOnly)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "page", page }, { "records", records } };
         return new Communicator(_context).Send<ICollection<Tag>>(Communicator.Get, "tags", null, payload, sharedOnly ? Driver.BlankSigature : new[] { "user" });
      }

      public int ForUserCount(string user, bool sharedOnly)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "count", 1 } };
         return new Communicator(_context).Send<CountContainer>(Communicator.Get, "tags", null, payload, sharedOnly ? Driver.BlankSigature : new[] { "user" }).Count;
      }

      public ICollection<Tag> ForUser(string user, string asset, int type, int page, int records, bool sharedOnly)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "asset", asset }, { "type", type }, { "page", page }, { "records", records } };
         return new Communicator(_context).Send<ICollection<Tag>>(Communicator.Get, "tags", null, payload, sharedOnly ? Driver.BlankSigature : new[] { "user", "asset", "type" });
      }

      public int ForUserCount(string user, string asset, int type, bool sharedOnly)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "asset", asset }, { "type", type }, { "count", 1 } };
         return new Communicator(_context).Send<CountContainer>(Communicator.Get, "tags", null, payload, sharedOnly ? Driver.BlankSigature : new[] { "user", "asset", "type" }).Count;
      }

      public string ForUserSignature(string user, string asset, int type)
      {
         return Communicator.GetSignature(new Dictionary<string, object> { { "user", user }, { "asset", asset }, { "type", type }, { "key", _context.Key } }, _context.Secret, "tags", "user", "asset", "type");
      }

      public ICollection<Tag> ForAsset(string asset, int type, int page, int records)
      {
         var payload = new Dictionary<string, object> { { "asset", asset }, { "type", type }, { "page", page }, { "records", records } };
         return new Communicator(_context).Send<ICollection<Tag>>(Communicator.Get, "tags", null, payload);
      }

      public int ForAssetCount(string asset, int type)
      {
         var payload = new Dictionary<string, object> { { "asset", asset }, { "type", type }, { "count", 1 } };
         return new Communicator(_context).Send<CountContainer>(Communicator.Get, "tags", null, payload).Count;
      }
   }
}