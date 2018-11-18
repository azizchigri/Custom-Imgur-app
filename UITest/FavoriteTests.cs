using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace UITest
{

    [TestFixture(Platform.Android)]
    public class FavoriteTests
    {
        IApp app;
        Platform platform;

        public FavoriteTests(Platform platform)
        {
            this.platform = platform;
        }

        [OneTimeSetUp]
        public void BeforeFirstTest()
        {
            AppInitializer.StartApp(platform);
            app = AppInitializer.app;
            app.WaitForElement(marked: "Open navigation drawer", timeout: TimeSpan.FromMinutes(1));
            app.Tap("Open navigation drawer");
            app.Tap("Favorites");
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app.WaitForElement(marked: "toolbar");
        }

        [Test]
        public void FirstUnitTest()
        {
            app.Repl();
        }

        [Test]
        public void FavoriteListView()
        {
            app.WaitForElement(marked: "imgurImg");
            AppResult[] results = app.Query(c => c.Marked("imgurImg"));
            Assert.IsTrue(results.Any());
        }

        [Test]
        public void FavoriteUpdate()
        {
            Assert.IsTrue(true);
        }

        private void likePhoto()
        {
            app.WaitForElement(marked: "Open navigation drawer");
            app.Tap("Open navigation drawer");
            app.Tap("Gallery");
            app.Tap("likeButton");
            app.Tap("Open navigation drawer");
            app.Tap("Favorites");
            //update result
        }

        [Test]
        public void Like()
        {
            AppResult[] results = app.Query(c => c.Marked("imgurImg"));
            var total = results.Count();
            likePhoto();
            results = app.Query(c => c.Marked("imgurImg"));
            Assert.IsTrue(results.Count() == total + 1);
        }

        [Test]
        public void Dislike()
        {
            AppResult[] results = app.Query(c => c.Marked("imgurImg"));
            var total = results.Count();
            if (total == 0)
            {
                likePhoto();
                results = app.Query(c => c.Marked("imgurImg"));
                total = results.Count();
            }
            app.Tap("likeButton");
            //update result
            results = app.Query(c => c.Marked("imgurImg"));
            Assert.IsTrue(results.Count() == total - 1);
        }
    }
}
