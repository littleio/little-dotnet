using System.Collections.Generic;

namespace Little
{
   public interface IDriver
   {
      /// <summary>
      /// Returns the version of the API this library understands
      /// </summary
      string ApiVersion { get; }

      /// <summary>
      /// Likes an asset
      /// </summary>
      /// <param name="user">the user doing the liking</param>
      /// <param name="asset">the asset being liked</param>
      /// <param name="type">the asset's type</param>
      void Like(string user, string asset, int type);

      /// <summary>
      /// Returns the signature required to like something
      /// </summary>
      /// <remarks>
      /// This is only useful when combined with the javascript library
      /// </remarks>
      /// <param name="user">the user doing the liking</param>
      string LikeSignature(string user);

      /// <summary>
      /// Returns whether or not the user has liked the asset
      /// </summary>
      /// <param name="user">the user doing the liking</param>
      /// <param name="asset">the asset being liked</param>
      /// <param name="type">the asset's type</param>
      bool DoesUserLikeAsset(string user, string asset, int type);

      /// <summary>
      /// Returns all of a user's likes
      /// </summary>
      /// <param name="user">the user doing the liking</param>
      /// <param name="page">the page to get</param>
      /// <param name="records">the number of records per page</param>
      ICollection<Like> UserLikes(string user, int page, int records);

      /// <summary>
      /// Returns the number of assets liked by the user
      /// </summary>
      /// <param name="user">the user doing the liking</param>
      int UserLikeCount(string user);

      /// <summary>
      /// Returns all the user's who liked an asset
      /// </summary>
      /// <param name="asset">the asset being liked</param>
      /// <param name="type">the asset's type</param>
      /// <param name="page">the page to get</param>
      /// <param name="records">the number of records per page</param>
      ICollection<string> AssetLikedBy(string asset, int type, int page, int records);

      /// <summary>
      /// Returns the number of users who liked an assset
      /// </summary>
      /// <param name="asset">the asset being liked</param>
      /// <param name="type">the asset's type</param>
      int AssetLikedCount(string asset, int type);

      /// <summary>
      /// Returns the assets for a type ordered by number of likes
      /// </summary>
      /// <param name="type">the asset's type</param>
      /// <param name="page">the page to get</param>
      /// <param name="records">the number of records per page</param>
      ICollection<LikeTypeGroup> LikedAssetsByType(int type, int page, int records);

      /// <summary>
      /// Returns the number of assets for a type
      /// </summary>
      /// <param name="type">the asset's type</param>
      int LikedAssetsByTypeCount(int type);
   }
}