using System;
using System.Linq;
using NUnit.Framework;

namespace Little.Tests
{
   public class UserTests : BaseFixture
   {
      [Test]
      public void SendsAnAttemptRequest()
      {
         Server.Stub(new ApiExpectation { Method = "POST", Url = "/v1/user/attempt", Request = "user=jessica&ip=1.2.3.4&ok=0&key=akey&sig=fe720a5042b2ff8998f78244a941dc4e66200585", Response = "{half: 2, one: 3, three:3, five:6}" });
         var rates = new Driver("akey", "sssshh2").User.Attempt("jessica", "1.2.3.4", false);
         Assert.AreEqual(2, rates.Last30Seconds);
         Assert.AreEqual(3, rates.LastMinute);
         Assert.AreEqual(3, rates.Last3Minutes);
         Assert.AreEqual(6, rates.Last5Minutes);
      }

      [Test]
      public void GetsThePreviousSuccessfulLoginAttempt()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/user/attempts", Request = "user=jessica&key=k1&sig=63370d046b6656b422568832563ab483e9c2c8d0", Response = "{ip: '233.203.94.99', ok: true, ts: '2011-12-27T07:30:36Z', c: 'Caladan'}" });
         var attempt = new Driver("k1", "bb1").User.PreviousSuccessful("jessica");
         AssertFirstAttempt(attempt);
      }

      [Test]
      public void GetsTheLastXAttempts()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/user/attempts", Request = "user=jessica&count=9&key=k1&sig=63370d046b6656b422568832563ab483e9c2c8d0", Response = "[{ip: '233.203.94.99', ok: true, ts: '2011-12-27T07:30:36Z', c: 'Caladan'},{ip: '2.2.2.2', ok: false, ts: '2010-11-26T06:29:35Z', c: 'geidi prime'}]" });
         var attempts = new Driver("k1", "bb1").User.GetAttempts("jessica", 9);
         Assert.AreEqual(2, attempts.Count);
         AssertFirstAttempt(attempts.ElementAt(0));
         Assert.AreEqual("2.2.2.2", attempts.ElementAt(1).IpAddress);
         Assert.AreEqual(false, attempts.ElementAt(1).Success);
         Assert.AreEqual(new DateTime(2010, 11, 26, 6, 29, 35, 0), attempts.ElementAt(1).Timestamp.ToUniversalTime());
         Assert.AreEqual("geidi prime", attempts.ElementAt(1).Country);
      }

      [Test]
      public void GetsTheSignature()
      {
         Assert.AreEqual("e86fa1d454445ece94a90a219019e133273a9b22", new Driver("over", "9000").User.GetAttemptsSignature("4e"));
      }

      private static void AssertFirstAttempt(LoginAttempt attempt)
      {
         Assert.AreEqual("233.203.94.99", attempt.IpAddress);
         Assert.AreEqual(true, attempt.Success);
         Assert.AreEqual(new DateTime(2011, 12, 27, 7, 30, 36, 0), attempt.Timestamp.ToUniversalTime());
         Assert.AreEqual("Caladan", attempt.Country);
      }
   }
}