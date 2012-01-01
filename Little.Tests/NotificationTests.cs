using NUnit.Framework;

namespace Little.Tests
{
   public class NotificationTests: BaseFixture
   {
      [Test]
      public void SendsALikeRequest()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/notifications", Request = "user=paul&type=5&key=yek", Response = "{_id: '4efac75d563d8a5d21000099', body: '<div>something</div>'}" });
         var notification = new Driver("yek", "secrettt").Notification.Create("paul", 5);
         Assert.AreEqual("4efac75d563d8a5d21000099", notification.Id);
         Assert.AreEqual("<div>something</div>", notification.Body);
      }

      [Test]
      public void HandlesANullResponse()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/notifications", Request = "user=paul&type=5&key=yek", Response = "" });
         var notification = new Driver("yek", "secrettt").Notification.Create("paul", 5);
         Assert.IsNull(notification);
      }

      [Test]
      public void RespondToNotification()
      {
         Server.Stub(new ApiExpectation { Method = "POST", Url = "/v1/notifications/respond", Request = "user=paul&notification=123&response=6&key=yek&sig=4103de2da916c9fe4792cdb48641e48698a47ecb", Response = "" });
         new Driver("yek", "secrettt").Notification.Respond("paul", "123", 6);
      }

      [Test]
      public void RespondToNotificationSignature()
      {
         Assert.AreEqual("9695fe627633f344b6bde27cd2391e772d8026d8", new Driver("yek", "secrettt").Notification.RespondSignature("duncan", "8458577"));
      }
   }
}