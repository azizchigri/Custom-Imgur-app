using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace UITest
{

    [TestFixture(Platform.Android)]
    public class GalleryTests
    {
        IApp app;
        Platform platform;

        public GalleryTests(Platform platform)
        {
            this.platform = platform;
        }

        [OneTimeSetUp]
        public void BeforeFirstTest()
        {
            app = AppInitializer.app;
            app.WaitForElement(marked: "Open navigation drawer", timeout: TimeSpan.FromMinutes(1));
            app.Tap("Open navigation drawer");
            app.Tap("Gallery");
        }

        [Test]
        public void GalleryListView()
        {
            app.WaitForElement(marked: "imgurImg");
            AppResult[] results = app.Query(c => c.Marked("imgurImg"));
            Assert.IsTrue(results.Any());
        }

        [Test]
        public void GalleryUpdate()
        {
            Assert.IsTrue(true);
        }
    }
}
