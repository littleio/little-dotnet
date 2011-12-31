using System.Collections.Generic;

namespace Little
{
   public interface INotificationDriver
   {
      /// <summary>
      /// Gets the notification for the user for the given type
      /// </summary>
      /// <remarks>
      /// If the user has responded to this particular notification, the return value will be null
      /// </remarks>
      /// <param name="user">the user</param>
      /// <param name="type">the notification type</param>
      Notification Create(string user, int type);

      /// <summary>
      /// Saves a user's response to a notification
      /// </summary>
      /// <param name="user">the user</param>
      /// <param name="notificationId">the notification id</param>
      /// <param name="response">the user's response</param>
      void Respond(string user, string notificationId, int response);

      /// <summary>
      /// The signature to respond to a notification
      /// </summary>
      /// <remarks>
      /// This is only useful when combined with the javascript library
      /// </remarks>
      /// <param name="user">the user</param>
      /// <param name="notificationId">the notification id</param>
      string RespondSignature(string user, string notificationId);
   }

   public class NotificationDriver : INotificationDriver
   {
      private readonly IRequestContext _context;

      public NotificationDriver(IRequestContext context)
      {
         _context = context;
      }

      public Notification Create(string user, int type)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "type", type } };
         return new Communicator(_context).Send<Notification>(Communicator.Get, "notifications", null, payload);
      }

      public void Respond(string user, string notificationId, int response)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "notification", notificationId }, { "response", response } };
         new Communicator(_context).Send(Communicator.Post, "notifications", null, payload, "user", "notification");
      }

      public string RespondSignature(string user, string notificationId)
      {
         return Communicator.GetSignature(new Dictionary<string, object> { { "user", user }, { "notification", notificationId }, { "key", _context.Key } }, _context.Secret, "user", "notification");
      }
   }
}