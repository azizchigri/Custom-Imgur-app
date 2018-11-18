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
    public class ImagesTests
    {
        IApp app;
        Platform platform;

        public ImagesTests(Platform platform)
        {
            this.platform = platform;
        }

        [OneTimeSetUp]
        public void BeforeFirstTest()
        {
            app = AppInitializer.app;
            app.WaitForElement(marked: "Open navigation drawer", timeout: TimeSpan.FromMinutes(1));
            app.Tap("Open navigation drawer");
            app.Tap("My images");
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app.WaitForElement(marked: "toolbar");
        }

        [Test]
        public void ImagesListView()
        {
            app.WaitForElement(marked: "imgurImg");
            AppResult[] results = app.Query(c => c.Marked("imgurImg"));
            Assert.IsTrue(results.Any());
        }

        [Test]
        public void ImagesUpdate()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void ImagesUpload()
        {
            AppResult[] results = app.Query(c => c.Marked("imgurImg"));
            var total = results.Count();
            app.Tap("fab");
            app.WaitForElement(marked: "txtiTitleUpload", timeout: TimeSpan.FromMinutes(1));
            app.EnterText(c => c.Marked("txtiTitleUpload"), "Ma photo de test");
            app.ScrollDownTo("txtiDescriptionUpload");
            app.EnterText(c => c.Marked("txtiDescriptionUpload"), "Description de ma photo de test");
            app.ScrollDownTo("btnUpload");
            app.Tap("btnUpload");
            app.WaitForElement(marked: "Open navigation drawer", timeout: TimeSpan.FromMinutes(2));
            app.WaitForElement(marked: "Ma photo de test");
            results = app.Query(c => c.Marked("Ma photo de test"));
            Assert.IsTrue(results.Any());
            results = app.Query(c => c.Marked("imgurImg"));
            Assert.IsTrue(results.Count() == total + 1);
        }
    }
}
