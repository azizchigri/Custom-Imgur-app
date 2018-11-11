using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Content.PM;

namespace Epicture.Login
{
    [Activity(Label = "ImgurAuthInterceptor", NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(
    new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataScheme = "myapp",
    DataHost = "test",
    DataPath = "/oauth2redirect")]
    public class ImgurAuthInterceptor : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Convert Android.Net.Url to Uri
            var uri = new Uri(Intent.Data.ToString());

            // Load redirectUrl page
            AuthenticationState.Authenticator.OnPageLoading(uri);

            StartActivity(typeof(Home));
            Finish();
        }
    }
}