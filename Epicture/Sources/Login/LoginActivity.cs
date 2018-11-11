using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Xamarin.Auth;
using Imgur.API.Models.Impl;
using Imgur.API.Authentication.Impl;
using System.Linq;
using Android.Content;

namespace Epicture.Login
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        private static Xamarin.Auth.Account currentUser = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_login);

            Button connectButton = FindViewById<Button>(Resource.Id.connectButton);
            connectButton.Click += (sender, e) =>
            {
                currentUser = AccountStore.Create(Application.ApplicationContext, Constants.appPwd).FindAccountsForService(Constants.appName).FirstOrDefault();
                if (currentUser == null)
                    ConnectWithOAuth2(bundle);
                else
                {
                    StartActivity(typeof(Home));
                    Finish();
                }
            };
        }

        private void ConnectWithOAuth2(Bundle bundle)
        {
            AuthenticationState.Authenticator = new OAuth2Authenticator(
                        Constants.appId,
                        Constants.appSecret,
                        "",
                        new Uri("https://api.imgur.com/oauth2/authorize"),
                        new Uri("myapp://test/oauth2redirect"),
                        new Uri("https://api.imgur.com/oauth2/token"),
                        null,
                        true)
            {
                ShowErrors = false,
                AllowCancel = true,
                Title = "Login"
            };
            AuthenticationState.Authenticator.Completed += OnAuthCompleted;
            global::Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, bundle);
            var uiObj = AuthenticationState.Authenticator.GetUI(this);
            StartActivity(uiObj);
        }

        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            if (AuthenticationState.Authenticator != null)
            {
                AuthenticationState.Authenticator.Completed -= OnAuthCompleted;
            }
            if (e.IsAuthenticated)
            {
                AccountStore.Create(Application.ApplicationContext, Constants.appPwd).Save(e.Account, Constants.appName);
                currentUser = e.Account;
                StartActivity(typeof(Home));
                Finish();
            }
        }

        public static ImgurClient GetImgurClient()
        {
            var account = currentUser;
            if (account == null || account.Properties == null)
                return null;
            var token = new OAuth2Token(account.Properties["access_token"],
                                        account.Properties["refresh_token"], account.Properties["token_type"], account.Properties["account_id"],
                                        account.Properties["account_username"], int.Parse(account.Properties["expires_in"]));
            var client = new ImgurClient(Constants.appId, Constants.appSecret, token);
            return client;
        }
    }

    internal class AuthenticationState
    {
        public static OAuth2Authenticator Authenticator;

    }
}