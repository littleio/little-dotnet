using System.Collections.Generic;

namespace Little
{
   public interface IUserDriver
   {
      /// <summary>
      /// Logs a login attempt
      /// </summary>
      /// <param name="user">the user logging in</param>
      /// <param name="ipAddress">the user's ip address</param>
      /// <param name="success">whether the login was successful or not</param>
      /// <returns>The number of failed logins in the last 0.5, 1, 3 and 5 minutes</returns>
      LoginFailureRate Attempt(string user, string ipAddress, bool success);

      /// <summary>
      /// Gets the previous (2nd last) successful login attempt
      /// </summary>
      /// <param name="user">the user to get the login attempt for</param>
      LoginAttempt PreviousSuccessful(string user);

      /// <summary>
      /// Gets the previous (2nd last) successful login attempt
      /// </summary>
      /// <param name="user">the user to get the login attempts for</param>
      /// <param name="count">the number of login attepts to get</param>
      ICollection<LoginAttempt> GetAttempts(string user, int count);

      /// <summary>
      /// Returns the signature required to get login attempts or the previous successful login attempt
      /// </summary>
      /// <remarks>
      /// This is only useful when combined with the javascript library
      /// </remarks>
      /// <param name="user">the user</param>
      string GetAttemptsSignature(string user);
   }

   public class UserDriver : IUserDriver
   {
      private readonly IRequestContext _context;

      public UserDriver(IRequestContext context)
      {
         _context = context;
      }

      public LoginFailureRate Attempt(string user, string ipAddress, bool success)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "ip", ipAddress }, { "ok", success ? 1 : 0 } };
         return new Communicator(_context).Send<LoginFailureRate>(Communicator.Post, "attempts", null, payload, "user", "ip", "ok");
      }

      public LoginAttempt PreviousSuccessful(string user)
      {
         var payload = new Dictionary<string, object> { { "user", user } };
         return new Communicator(_context).Send<LoginAttempt>(Communicator.Get, "attempts", null, payload, "user");
      }

      public ICollection<LoginAttempt> GetAttempts(string user, int count)
      {
         var payload = new Dictionary<string, object> { { "user", user }, { "count", count } };
         return new Communicator(_context).Send<ICollection<LoginAttempt>>(Communicator.Get, "attempts", null, payload, "user");
      }

      public string GetAttemptsSignature(string user)
      {
         return Communicator.GetSignature(new Dictionary<string, object> { { "user", user }, { "key", _context.Key } }, _context.Secret, "attempts", "user");
      }
   }
}