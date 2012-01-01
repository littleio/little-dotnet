using System;
using System.Collections.Generic;

namespace Little
{
   public interface IAssetDriver
   {
      /// <summary>
      /// Vote for an asset
      /// </summary>
      /// <param name="user">the user who is voting</param>
      /// <param name="asset">the asset being voted on</param>
      /// <param name="type">the asset's type</param>
      /// <param name="up">true for up, false for down</param>
      VoteResponse Vote(string user, string asset, int type, bool up);

      /// <summary>
      /// Rate an asset
      /// </summary>
      /// <param name="user">the user doing the rating</param>
      /// <param name="asset">the asset being rated</param>
      /// <param name="type">the asset's type</param>
      /// <param name="rating">the asset's rating</param>
      RateResponse Rate(string user, string asset, int type, int rating);

      /// <summary>
      /// Returns the signature required to vote on an asset
      /// </summary>
      /// <remarks>
      /// This is only useful when combined with the javascript library
      /// </remarks>
      /// <param name="user">the user who is voting</param>
      /// <param name="asset">the asset being voted on</param>
      /// <param name="type">the asset's type</param>
      string VoteSignature(string user, string asset, int type);

      /// <summary>
      /// Returns the signature required to rate on an asset
      /// </summary>
      /// <remarks>
      /// This is only useful when combined with the javascript library
      /// </remarks>
      /// <param name="user">the user doing the rating</param>
      /// <param name="asset">the asset being rated</param>
      /// <param name="type">the asset's type</param>
      string RateSignature(string user, string asset, int type);

      /// <summary>
      /// Gets a specific record for a user and asset
      /// </summary>
      /// <param name="user">the user </param>
      /// <param name="asset">the asset </param>
      /// <param name="type">the asset's type</param>
      UserAsset UserAsset(string user, string asset, int type);

      /// <summary>
      /// Returns all of the assets the user did something to
      /// </summary>
      /// <param name="user">the user doing the tagging</param>
      /// <param name="page">the page to get</param>
      /// <param name="records">the number of records per page</param>
      ICollection<UserAsset> ForUser(string user, int page, int records);

      /// <summary>
      /// Returns all of the assets the user did something to filtered by vote and/or rating
      /// </summary>
      /// <param name="user">the user doing the tagging</param>
      /// <param name="vote">true for upvotes, false for downvotes, null for either</param>
      /// <param name="rated_only">true for when the user rated the asset, false for when he didn't, null for either</param>
      /// <param name="page">the page to get</param>
      /// <param name="records">the number of records per page</param>
      ICollection<UserAsset> ForUser(string user, bool? vote, bool? rated_only, int page, int records);

      /// <summary>
      /// Returns the number of assets the use did something to
      /// </summary>
      /// <param name="user">the user doing the tagging</param>
      int ForUserCount(string user);

      /// <summary>
      /// Returns the number of assets the use did something to filtered by vote and/or rating
      /// </summary>
      /// <param name="user">the user doing the tagging</param>
      /// <param name="vote">true for upvotes, false for downvotes, null for either</param>
      /// <param name="rated_only">true for when the user rated the asset, false for when he didn't, null for either</param>
      int ForUserCount(string user, bool? vote, bool? rated_only);

      /// <summary>
      /// Returns all of the assets by asset and type (all the user's who did something to the asset)
      /// </summary>
      /// <param name="asset">the asset</param>
      /// <param name="type">the asset's type</param>
      /// <param name="page">the page to get</param>
      /// <param name="records">the number of records per page</param>
      ICollection<UserAsset> ForAsset(string asset, int type, int page, int records);

      /// <summary>
      /// Returns all of the assets by asset and type (all the user's who did something to the asset) filtered by vote and/or rating
      /// </summary>
      /// <param name="asset">the asset</param>
      /// <param name="type">the asset's type</param>
      /// <param name="vote">true for upvotes, false for downvotes, null for either</param>
      /// <param name="rated_only">true for when the user rated the asset, false for when he didn't, null for either</param>
      /// <param name="page">the page to get</param>
      /// <param name="records">the number of records per page</param>
      ICollection<UserAsset> ForAsset(string asset, int type, bool? vote, bool? rated_only, int page, int records);

      /// <summary>
      /// Returns the number of users who did something to the assset
      /// </summary>
      /// <param name="asset">the asset</param>
      /// <param name="type">the asset's type</param>
      int ForAssetCount(string asset, int type);

      /// <summary>
      /// Returns the number of users who did something to the assset filtered by vote and/or rating
      /// </summary>
      /// <param name="asset">the asset</param>
      /// <param name="type">the asset's type</param>
      /// <param name="vote">true for upvotes, false for downvotes, null for either</param>
      /// <param name="rated_only">true for when the user rated the asset, false for when he didn't, null for either</param>
      int ForAssetCount(string asset, int type, bool? vote, bool? rated_only);

      /// <summary>
      /// The assets by rating (desc) for a type
      /// </summary>
      /// <param name="type">the type</param>
      /// <param name="page">the page to get</param>
      /// <param name="records">the number of records per page</param>
      ICollection<AssetRank> HighestRated(int type, int page, int records);
      
      /// <summary>
      /// The assets by vote (desc) for a type
      /// </summary>
      /// <param name="type">the type</param>
      /// <param name="page">the page to get</param>
      /// <param name="records">the number of records per page</param>
      ICollection<AssetRank> MostVotes(int type, int page, int records);

      /// <summary>
      /// The number of assets for the type
      /// </summary>
      /// <param name="type">the type</param>
      int CountByType(int type);

      /// <summary>
      /// Queues the asset for deletion. This will purge all assets (votes/rating) and tags...forever!!!
      /// </summary>
      void Delete(string asset, int type);
   }

   public class AssetDriver : IAssetDriver
   {
      private readonly IRequestContext _context;

      public AssetDriver(IRequestContext context)
      {
         _context = context;
      }

      public VoteResponse Vote(string user, string asset, int type, bool up)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "asset", asset }, { "type", type }, { "vote", up ? 1 : 0 } };
         return new Communicator(_context).Send <VoteResponse>(Communicator.Post, "assets", "vote", payload, "user", "asset", "type");
      }

      public RateResponse Rate(string user, string asset, int type, int rating)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "asset", asset }, { "type", type }, { "rate", rating } };
         return new Communicator(_context).Send<RateResponse>(Communicator.Post, "assets", "rate", payload, "user", "asset", "type");
      }

      public string VoteSignature(string user, string asset, int type)
      {
         return Communicator.GetSignature(new Dictionary<string, object> { { "user", user }, { "asset", asset }, { "type", type }, { "key", _context.Key } }, _context.Secret, "assets", "user", "asset", "type");
      }

      public string RateSignature(string user, string asset, int type)
      {
         return Communicator.GetSignature(new Dictionary<string, object> { { "user", user }, { "asset", asset }, { "type", type }, { "key", _context.Key } }, _context.Secret, "assets", "user", "asset", "type");
      }

      public UserAsset UserAsset(string user, string asset, int type)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "asset", asset }, { "type", type } };
         return new Communicator(_context).Send<UserAsset>(Communicator.Get, "assets", null, payload);
      }

      public ICollection<UserAsset> ForUser(string user, int page, int records)
      {
         return ForUser(user, null, null, page, records);
      }

      public ICollection<UserAsset> ForUser(string user, bool? vote, bool? rated_only, int page, int records)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "page", page }, { "records", records } };
         if (vote != null) { payload["vote"] = vote == true ? 1 : 0; }
         if (rated_only != null) { payload["rate"] = rated_only == true ? 1 : 0; }
         return new Communicator(_context).Send<ICollection<UserAsset>>(Communicator.Get, "assets", null, payload);
      }

      public int ForUserCount(string user)
      {
         return ForUserCount(user, null, null);
      }

      public int ForUserCount(string user, bool? vote, bool? rated_only)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "count", 1 } };
         if (vote != null) { payload["vote"] = vote == true ? 1 : 0; }
         if (rated_only != null) { payload["rate"] = rated_only == true ? 1 : 0; }
         return new Communicator(_context).Send<CountContainer>(Communicator.Get, "assets", null, payload).Count;
      }

      public ICollection<UserAsset> ForAsset(string asset, int type, int page, int records)
      {
         return ForAsset(asset, type, null, null, page, records);
      }

      public ICollection<UserAsset> ForAsset(string asset, int type, bool? vote, bool? rated_only, int page, int records)
      {
         var payload = new Dictionary<string, object> { { "asset", asset }, { "type", type }, { "page", page }, { "records", records } };
         if (vote != null) { payload["vote"] = vote == true ? 1 : 0; }
         if (rated_only != null) { payload["rate"] = rated_only == true ? 1 : 0; }
         return new Communicator(_context).Send<ICollection<UserAsset>>(Communicator.Get, "assets", null, payload);
      }

      public int ForAssetCount(string asset, int type)
      {
         return ForAssetCount(asset, type, null, null);
      }

      public int ForAssetCount(string asset, int type, bool? vote, bool? rated_only)
      {
         var payload = new Dictionary<string, object> { { "asset", asset }, { "type", type }, { "count", 1 } };
         if (vote != null) { payload["vote"] = vote == true ? 1 : 0; }
         if (rated_only != null) { payload["rate"] = rated_only == true ? 1 : 0; }
         return new Communicator(_context).Send<CountContainer>(Communicator.Get, "assets", null, payload).Count;
      }

      public ICollection<AssetRank> HighestRated(int type, int page, int records)
      {
         var payload = new Dictionary<string, object> {  { "type", type }, { "page", page }, { "records", records } };
         return new Communicator(_context).Send<ICollection<AssetRank>>(Communicator.Get, "assets", "by_rate", payload);
      }

      public ICollection<AssetRank> MostVotes(int type, int page, int records)
      {
         var payload = new Dictionary<string, object> { { "type", type }, { "page", page }, { "records", records } };
         return new Communicator(_context).Send<ICollection<AssetRank>>(Communicator.Get, "assets", "by_vote", payload);
      }

      public int CountByType(int type)
      {
         var payload = new Dictionary<string, object> { { "type", type } };
         return new Communicator(_context).Send<CountContainer>(Communicator.Get, "assets", "count", payload).Count;
      }

      public void Delete(string asset, int type)
      {
         var payload = new Dictionary<string, object> { { "asset", asset }, {"type", type}, { "verify", "kludge" }};
         new Communicator(_context).Send(Communicator.Delete, "assets", null, payload, "asset", "type", "verify");
      }
   }
}