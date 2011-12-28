using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Little.Tests
{
   public abstract class BaseFixture
   {
      protected static readonly Func<IDictionary<string, object>> EmptyPayload = () => new Dictionary<string, object>();
      protected virtual bool NeedAServer
      {
         get { return true; }
      }
      protected FakeServer Server;
      
      [SetUp]
      public void SetUp()
      {
         if (NeedAServer)
         {
            Server = new FakeServer();
            DriverConfiguration.Configuration(c => c.ConnectTo("http://localhost:" + FakeServer.Port + "/"));
         }
         BeforeEachTest();
      }
      [TearDown]
      public void TearDown()
      {
         if (Server != null)
         {
            Server.Dispose();
            DriverConfiguration.ResetToDefaults();
         }
         AfterEachTest();
      }
      public virtual void AfterEachTest() { }
      public virtual void BeforeEachTest() { }

      protected void AssertMogadeException(string expectedMessage, Action code)
      {
         var ex = Assert.Throws<LittleException>(() => code());
         Assert.AreEqual(expectedMessage, ex.Message);
      }
   }
}