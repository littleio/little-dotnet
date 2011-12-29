using System.Linq;
using NUnit.Framework;

namespace Little.Tests
{
   public class TagTests : BaseFixture
   {
      private const string _firstTagJson = "{_id: 9000, user:'leto', asset:'spice', type: 5, share: 'true'}";
      private static readonly string _firstTagJsonArray = string.Format("[{0}]", _firstTagJson);

      [Test]
      public void SendsTagRequestWithoutData()
      {
         Server.Stub(new ApiExpectation { Method = "POST", Url = "/v1/tags", Request = "user=leto&asset=asset&type=1&share=1&key=ak&sig=57323afa9ccad1c0b1b9bf37cd9e860a57d456db", Response = "{_id: 'abc123'}" });
         Assert.AreEqual("abc123", new Driver("ak", "sa").Tag("leto", "asset", 1, true));
      }

      [Test]
      public void SendsTagRequestWithData()
      {
         Server.Stub(new ApiExpectation { Method = "POST", Url = "/v1/tags", Request = "user=paul&asset=asset&type=1&share=0&data=yumm&key=ak&sig=ae1784623952d7728d11bb2e1b4bb3ac6cd5ddee", Response = "{_id: '9000!!'}" });
         Assert.AreEqual("9000!!", new Driver("ak", "sa").Tag("paul", "asset", 1, false, "yumm"));
      }

      [Test]
      public void GetsATagSignature()
      {
         Assert.AreEqual("0652e0f61b870c2aed62c1e29ccbc34b62a33b17", new Driver("over", "9000").TagSignature("er"));
      }

      [Test]
      public void GetsASharedTagById()
      {
         Server.Stub(new ApiExpectation {Method = "GET", Url = "/v1/tags", Request = "id=95&key=ak", Response = _firstTagJson});
         AssertFirstTag(new Driver("ak", "sa").TagById("95", true));
      }

      [Test]
      public void GetsANonSharedTagById()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/tags", Request = "id=93&key=ak&sig=45d17601a3f17fd1fd4832085f08f4ed71b95d36", Response = _firstTagJson });
         AssertFirstTag(new Driver("ak", "sa").TagById("93", false));
      }

      [Test]
      public void GetByIdHandlesBlankReturn()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/tags", Response = string.Empty });
         Assert.IsNull(new Driver("ak", "sa").TagById("93", false));
      }

      [Test]
      public void GetsAUsersSharedTags()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/tags", Request = "user=93&page=1&records=20&key=3945", Response = _firstTagJsonArray });
         var tags = new Driver("3945", "sa").UserTags("93", 1, 20, true);
         Assert.AreEqual(1, tags.Count);
         AssertFirstTag(tags.ElementAt(0));
      }

      [Test]
      public void GetsAUsersSharedTagCount()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/tags", Request = "user=83&count=1&key=22", Response = "{count: 9}" });
         Assert.AreEqual(9, new Driver("22", "sa").UserTagCount("83", true));
      }

      [Test]
      public void GetsAUsersNonSharedTags()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/tags", Request = "user=93&page=1&records=20&key=3945&sig=0c2592e2d08c38938affca93b540e4317e0e4563", Response = _firstTagJsonArray });
         var tags = new Driver("3945", "sa").UserTags("93", 1, 20, false);
         Assert.AreEqual(1, tags.Count);
         AssertFirstTag(tags.ElementAt(0));
      }

      [Test]
      public void GetsAUsersNonSharedTagCount()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/tags", Request = "user=83&count=1&key=22&sig=8fc76af9461d57fb204d05d1e869c74c28a6502b", Response = "{count: 9}" });
         Assert.AreEqual(9, new Driver("22", "sa").UserTagCount("83", false));
      }

      [Test]
      public void GetsAUsersSharedTagsForAnAsset()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/tags", Request = "user=93&asset=3884&type=5&page=1&records=20&key=3945", Response = _firstTagJsonArray });
         var tags = new Driver("3945", "sa").UserTags("93", "3884", 5, 1, 20, true);
         Assert.AreEqual(1, tags.Count);
         AssertFirstTag(tags.ElementAt(0));
      }

      [Test]
      public void GetsAUsersSharedTagCountForAnAsset()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/tags", Request = "user=6&asset=ass&type=69&count=1&key=22", Response = "{count: 22}" });
         Assert.AreEqual(22, new Driver("22", "sa").UserTagCount("6", "ass", 69, true));
      }

      [Test]
      public void GetsAUsersNonSharedTagsForAnAsset()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/tags", Request = "user=93&asset=4&type=5553&page=1&records=20&key=3945&sig=497dd7d913ad9cc432ba11dbf81906071ac88a7e", Response = _firstTagJsonArray });
         var tags = new Driver("3945", "sa").UserTags("93", "4", 5553, 1, 20, false);
         Assert.AreEqual(1, tags.Count);
         AssertFirstTag(tags.ElementAt(0));
      }

      [Test]
      public void GetsAUsersNonSharedTagCountForAnAsset()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/tags", Request = "user=6&asset=ass&type=69&count=1&key=22&sig=be4bcbf2d01f18dee73efe13b3617d737359047a", Response = "{count: 22}" });
         Assert.AreEqual(22, new Driver("22", "sa").UserTagCount("6", "ass", 69, false));
      }

      [Test]
      public void GetsAUserTagsSignature()
      {
         Assert.AreEqual("97c34e75b8996ab82d599c1fabc9d595c9482d5a", new Driver("over", "9000").UserTagsSignature("plz"));
      }

      [Test]
      public void GetsAUserTagsSignatureForAnAsset()
      {
         Assert.AreEqual("296ca981eaed54ca110a016e287ed22251b22c3f", new Driver("over", "9000").UserTagsSignature("er", "asett", 855));
      }

      [Test]
      public void GetsAnAssetsTags()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/tags", Request = "asset=92&type=3&page=1&records=20&key=3945", Response = _firstTagJsonArray });
         var tags = new Driver("3945", "sa").AssetTags("92", 3, 1, 20);
         Assert.AreEqual(1, tags.Count);
         AssertFirstTag(tags.ElementAt(0));
      }

      [Test]
      public void GetsAnAssetsTagCount()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/v1/tags", Request = "asset=912&type=32&count=1&key=21", Response = "{count: 7}" });
         Assert.AreEqual(7, new Driver("21", "sa").AssetTagCount("912", 32));
      }

      private static void AssertFirstTag(Tag tag)
      {
         Assert.AreEqual("9000", tag.Id);
         Assert.AreEqual("leto", tag.User);
         Assert.AreEqual("spice", tag.Asset);
         Assert.AreEqual(5, tag.Type);
         Assert.AreEqual(true, tag.Share);
      }
   }
}