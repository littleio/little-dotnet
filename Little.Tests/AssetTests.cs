using NUnit.Framework;

namespace Little.Tests
{
   public class AssetTests : BaseFixture
   {
      [Test]
      public void SendsAVoteRequest()
      {
         Server.Stub(new ApiExpectation { Method = "POST", Url = "/v1/assets/vote", Request = "user=leto&asset=asset&type=1&vote=1&key=akey&sig=25e438af76653b66a0f5ab9500029e046213c392", Response = "{votes: 4}" });
         var response = new Driver("akey", "sssshh2").Asset.Vote("leto", "asset", 1, true);
         Assert.AreEqual(4, response.Votes);
      }

      [Test]
      public void SendsARatingRequest()
      {
         Server.Stub(new ApiExpectation { Method = "POST", Url = "/v1/assets/rate", Request = "user=jessica&asset=sand&type=3&rate=4&key=akey&sig=5025515ccee647d623d73a8969ab30711b345d5c", Response = "{rate:4.3, count: 5}" });
         var response = new Driver("akey", "sssshh2").Asset.Rate("jessica", "sand", 3, 4);
         Assert.AreEqual(4.3, response.Rate);
         Assert.AreEqual(5, response.Count);
      }

      [Test]
      public void GetsAVoteSignature()
      {
         Assert.AreEqual("46ba7cabfce46017b9383825ca6e8598c91dbaab", new Driver("over", "3").Asset.VoteSignature("er", "ar", 5));
      }

      [Test]
      public void GetsARateSignature()
      {
         Assert.AreEqual("fe4c24fa9dfafdaae49c7e2afcaa084f2d3f25d2", new Driver("over", "2").Asset.RateSignature("er1", "ar1", 6));
      }

      [Test]
      public void GetAUserAsset()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/assets", Request = "user=jessica&asset=sand&type=3&key=akey", Response = "{asset:'sand', type:3, user:'jessica', vote:true, rate: 55}" });
         var response = new Driver("akey", "sssshh2").Asset.UserAsset("jessica", "sand", 3);
         Assert.AreEqual("jessica", response.User);
         Assert.AreEqual("sand", response.Asset);
         Assert.AreEqual(3, response.Type);
         Assert.AreEqual(true, response.Vote);
         Assert.AreEqual(55, response.Rating);
      }

      [Test]
      public void ForUserTests()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/assets", Request = "user=jessica&page=4&records=10&key=akey", Response = "[{asset: 'spice', type: 1, user: 'leto', vote: true, rate: 1},{asset: 'sand', type: 2, user: 'leto', vote: false}]" });
         var response = new Driver("akey", "sssshh2").Asset.ForUser("jessica", 4, 10);
         Assert.AreEqual(2, response.Count);
      }

      [Test]
      public void ForUserWithFiltersTests()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/assets", Request = "user=jessica&page=4&records=10&vote=1&rate=0&key=kk", Response = "[{asset: 'spice', type: 1, user: 'leto', vote: true, rate: 1},{asset: 'sand', type: 2, user: 'leto', vote: false}]" });
         var response = new Driver("kk", "sssshh2").Asset.ForUser("jessica", true, false, 4, 10);
         Assert.AreEqual(2, response.Count);
      }

      [Test]
      public void ForUserCountTests()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/assets", Request = "user=jessica&count=1&key=akey", Response = "{count: 57}" });
         var response = new Driver("akey", "sssshh2").Asset.ForUserCount("jessica");
         Assert.AreEqual(57, response);
      }

      [Test]
      public void ForUserWithFiltersCountTests()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/assets", Request = "user=jessica&count=1&vote=1&rate=0&key=kk", Response = "{count: 75}" });
         var response = new Driver("kk", "sssshh2").Asset.ForUserCount("jessica", true, false);
         Assert.AreEqual(75, response);
      }

      [Test]
      public void ForAssetTests()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/assets", Request = "asset=ink!!&type=4&page=4&records=10&key=akey", Response = "[{asset: 'spice', type: 1, user: 'leto', vote: true, rate: 1},{asset: 'sand', type: 2, user: 'leto', vote: false}]" });
         var response = new Driver("akey", "sssshh2").Asset.ForAsset("ink!!", 4, 4, 10);
         Assert.AreEqual(2, response.Count);
      }

      [Test]
      public void ForAssetWithFiltersTests()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/assets", Request = "asset=ink&type=3&page=5&records=6&vote=1&rate=0&key=kk", Response = "[{asset: 'spice', type: 1, user: 'leto', vote: true, rate: 1},{asset: 'sand', type: 2, user: 'leto', vote: false}]" });
         var response = new Driver("kk", "sssshh2").Asset.ForAsset("ink", 3, true, false, 5, 6);
         Assert.AreEqual(2, response.Count);
      }

      [Test]
      public void ForAssetCountTests()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/assets", Request = "asset=gum&type=3&count=1&key=akey", Response = "{count: 57}" });
         var response = new Driver("akey", "sssshh2").Asset.ForAssetCount("gum", 3);
         Assert.AreEqual(57, response);
      }

      [Test]
      public void ForAssetWithFiltersCountTests()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/assets", Request = "asset=dbz&type=9000&count=1&vote=0&rate=1&key=kk", Response = "{count: 75}" });
         var response = new Driver("kk", "sssshh2").Asset.ForAssetCount("dbz", 9000, false, true);
         Assert.AreEqual(75, response);
      }

      [Test]
      public void HighestRatedTests()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/assets/by_rate", Request = "type=10&page=5&records=20&key=kk", Response = "[]" });
         new Driver("kk", "sssshh2").Asset.HighestRated(10, 5, 20);
      }

      [Test]
      public void MostVotesTests()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/assets/by_vote", Request = "type=5&page=15&records=25&key=kk", Response = "[]" });
         new Driver("kk", "sssshh2").Asset.MostVotes(5, 15, 25);
      }

      [Test]
      public void CountByTypeTests()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/assets/count", Request = "type=6&key=kk", Response = "{count:588}" });
         new Driver("kk", "sssshh2").Asset.CountByType(6);
      }

      [Test]
      public void DeletesTheAsset()
      {
         Server.Stub(new ApiExpectation { Method = "DELETE", Url = "/v1/assets", Request = "asset=iwillgetu&type=9&verify=kludge&key=kkeyy&sig=56517fb77873feaf3905da7bc9f893eee86a66e3", Response = "" });
         new Driver("kkeyy", "s1").Asset.Delete("iwillgetu", 9);
      }
   }
}