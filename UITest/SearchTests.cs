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
    public class SearchTests
    {
        IApp app;
        Platform platform;

        public SearchTests(Platform platform)
        {
            this.platform = platform;
        }

        [OneTimeSetUp]
        public void BeforeFirstTest()
        {
            app = AppInitializer.app;
            app.WaitForElement(marked: "Open navigation drawer", timeout: TimeSpan.FromMinutes(1));
            app.Tap("Open navigation drawer");
            app.Tap("Search");
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app.WaitForElement(marked: "toolbar");
        }

        [Test]
        public void SearchListView()
        {
            app.WaitForElement(marked: "searchFilter", timeout: TimeSpan.FromMinutes(1));
            app.EnterText(c => c.Marked("searchFilter"), "tutu");
            app.Tap("searchButton");
            app.WaitForElement(marked: "imgurImg", timeout: TimeSpan.FromMinutes(1));
            AppResult[] results = app.Query(c => c.Marked("imgurImg"));
            Assert.IsTrue(results.Any());
        }
    }
}
