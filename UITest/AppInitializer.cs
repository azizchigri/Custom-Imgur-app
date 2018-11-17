using System;
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
                app =  ConfigureApp
                            .Android
                            .ApkFile("C:/Users/achigri/Epicture/Epicture/bin/Debug/Epicture.Epicture.apk")
                            .StartApp(AppDataMode.DoNotClear);
            }
            else
                app = ConfigureApp.iOS.StartApp();
        }
    }
}