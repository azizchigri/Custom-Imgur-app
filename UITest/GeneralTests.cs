using System;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace UITest
{

    [TestFixture(Platform.Android)]
    public class GeneralTests
    {
        IApp app;
        Platform platform;

        public GeneralTests(Platform platform)
        {
            this.platform = platform;
        }

        [OneTimeSetUp]
        public void BeforeFirstTest()
        {
            app = AppInitializer.app;
            app.WaitForElement(marked: "Open navigation drawer", timeout: TimeSpan.FromMinutes(1));
            app.Tap("Open navigation drawer");
        }

        [Test]
        public void CheckUserInfo()
        {
            AppResult[] results = app.Query(c => c.Marked("xamarin@microsoft.com"));
            Assert.IsTrue(results.Count() == 0);
            results = app.Query(c => c.Marked("Xamarin.Android"));
            Assert.IsTrue(results.Count() == 0);
        }

        [Test]
        public void Disconnect()
        {
            app.Tap("Disconnect");
            AppResult[] results = app.Query(c => c.Marked("Open navigation drawer"));
            Assert.IsTrue(results.Count() == 0);
        }
    }
}
