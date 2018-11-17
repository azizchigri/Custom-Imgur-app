using System;
using System.IO;
using Xamarin.UITest;
using Xamarin.UITest.Configuration;
using Xamarin.UITest.Queries;

namespace UITest
{
    public class AppInitializer
    {
        public static IApp app = null;
        public static void StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                var path = System.AppDomain.CurrentDomain.BaseDirectory;
                path = Directory.GetParent(path).Parent.Parent.Parent.FullName;
                path = Path.Combine(path, "Epicture/bin/Debug/Epicture.Epicture.apk");
                app =  ConfigureApp
                            .Android
                            .ApkFile(path)
                            .StartApp(AppDataMode.DoNotClear);
            }
            else
                app = ConfigureApp.iOS.StartApp();
        }
    }
}