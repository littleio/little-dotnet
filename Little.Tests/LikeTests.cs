using System.Linq;
using NUnit.Framework;

namespace Little.Tests.LikeTests
{
   public class LikeTests : BaseFixture
   {
      [Test]
      public void SendsALikeRequest()
      {
         Server.Stub(new ApiExpectation { Method = "POST", Url = "/v1/likes", Request = "user=leto&asset=asset&type=1&key=akey&sig=8a82d5cc57c2b6f8d97c0e68104ed4c436dabb41", Response = "" });
         new Driver("akey", "sssshh2").Like("leto", "asset", 1);
      }

      [Test]
      public void GetsALikeSignature()
      {
         Assert.AreEqual("9bc17e63af214d3297b01fa88fdc2d026c62a3e5", new Driver("over", "9000").LikeSignature("wy"));
      }

      [Test]
      public void SendsADoesUserLikesRequest()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/likes", Request = "user=eto&asset=set&type=2&key=akey", Response = "true" });
         Assert.AreEqual(true, new Driver("akey", "sssshh2").DoesUserLikeAsset("eto", "set", 2));
      }

      [Test]
      public void SendsAUserLikesRequest()
      {
         Server.Stub(new ApiExpectation {Method = "GET", Url = "/v1/likes", Request = "user=wy&page=2&records=3&key=akey", Response = "[{asset: 'spice', type:1},{asset: '9000', type:2}]"});
         var likes = new Driver("akey", "sssshh2").UserLikes("wy", 2, 3);
         Assert.AreEqual(2, likes.Count);
         Assert.AreEqual("spice", likes.ElementAt(0).Asset);
         Assert.AreEqual(1, likes.ElementAt(0).Type);
         Assert.AreEqual("9000", likes.ElementAt(1).Asset);
         Assert.AreEqual(2, likes.ElementAt(1).Type);
      }

      [Test]
      public void SendsAUserLikeCountRequest()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/likes", Request = "user=wy&count=1&key=k", Response = "{count:44}" });
         Assert.AreEqual(44, new Driver("k", "sssshh2").UserLikeCount("wy"));
      }

      [Test]
      public void SendsAAssetLikedRequest()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/likes", Request = "asset=dbz&type=3&page=2&records=3&key=akey", Response = "[{user:'vegeta'},{user:'goku'},{user:'gohan'}]" });
         var users = new Driver("akey", "sssshh2").AssetLikedBy("dbz", 3, 2, 3);
         Assert.AreEqual(3, users.Count);
         Assert.AreEqual("vegeta", users.ElementAt(0));
         Assert.AreEqual("goku", users.ElementAt(1));
         Assert.AreEqual("gohan", users.ElementAt(2));
      }

      [Test]
      public void SendsAAssetLikeCountRequest()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/likes", Request = "asset=zbd&type=4&count=1&key=k", Response = "{count:22}" });
         Assert.AreEqual(22, new Driver("k", "sssshh2").AssetLikedCount("zbd", 4));
      }

      [Test]
      public void SendsAGroupByTypeRequest()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/likes", Request = "type=1&page=5&records=10&key=akey", Response = "[{asset: 'sand', count:102}]" });
         var assets = new Driver("akey", "sssshh2").LikedAssetsByType(1, 5, 10);
         Assert.AreEqual(1, assets.Count);
         Assert.AreEqual("sand", assets.ElementAt(0).Asset);
         Assert.AreEqual(102, assets.ElementAt(0).Count);
      }

      [Test]
      public void SendsAGroupByTypeCountRequest()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/likes", Request = "type=55&count=1&key=k", Response = "{count:2}" });
         Assert.AreEqual(2, new Driver("k", "sssshh2").LikedAssetsByTypeCount(55));
      }
   }
}