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

      /// <summary>
      /// Logs a login attempt
      /// </summary>
      /// <param name="user">the user logging in</param>
      /// <param name="ipAddress">the user's ip address</param>
      /// <param name="success">whether the login was successful or not</param>
      /// <returns>The number of failed logins in the last 0.5, 1, 3 and 5 minutes</returns>
      LoginFailureRate LoginAttempt(string user, string ipAddress, bool success);

      /// <summary>
      /// Gets the previous (2nd last) successful login attempt
      /// </summary>
      /// <param name="user">the user to get the login attempt for</param>
      LoginAttempt PreviousSuccessfulLoginAttempt(string user);

      /// <summary>
      /// Gets the previous (2nd last) successful login attempt
      /// </summary>
      /// <param name="user">the user to get the login attempts for</param>
      /// <param name="count">the number of login attepts to get</param>
      ICollection<LoginAttempt> LoginAttempts(string user, int count);

      /// <summary>
      /// Returns the signature required to get login attempts or the previous successful login attempt
      /// </summary>
      /// <remarks>
      /// This is only useful when combined with the javascript library
      /// </remarks>
      /// <param name="user">the user</param>
      string LoginAttemptsSignature(string user);

      /// <summary>
      /// Gets the notification for the user for the given type
      /// </summary>
      /// <remarks>
      /// If the user has responded to this particular notification, the return value will be null
      /// </remarks>
      /// <param name="user">the user</param>
      /// <param name="type">the notification type</param>
      Notification Notification(string user, int type);

      /// <summary>
      /// Saves a user's response to a notification
      /// </summary>
      /// <param name="user">the user</param>
      /// <param name="notificationId">the notification id</param>
      /// <param name="response">the user's response</param>
      void RespondToNotification(string user, string notificationId, int response);

      /// <summary>
      /// The signature to respond to a notification
      /// </summary>
      /// <remarks>
      /// This is only useful when combined with the javascript library
      /// </remarks>
      /// <param name="user">the user</param>
      /// <param name="notificationId">the notification id</param>
      string RespondToNotificationSignature(string user, string notificationId);
   }
}