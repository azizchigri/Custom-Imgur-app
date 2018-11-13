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
        private static Application app = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            CustomTabsConfiguration.CustomTabsClosingMessage = null;
            app = Application;
            currentUser = AccountStore.Create(Application.ApplicationContext, Constants.appPwd).FindAccountsForService(Constants.appName).FirstOrDefault();
            if (currentUser == null)
                ConnectWithOAuth2(bundle);
            else
            {
                StartActivity(typeof(Home));
                Finish();
            }
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

        void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
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

        public static void Disconnect()
        {
            currentUser = null;
            var accounts = AccountStore.Create(app.ApplicationContext, Constants.appPwd).FindAccountsForService(Constants.appName).ToList();
            accounts.ForEach(account => AccountStore.Create(app.ApplicationContext, Constants.appPwd).Delete(account, Constants.appName));
            app = null;
        }
    }

    internal class AuthenticationState
    {
        public static OAuth2Authenticator Authenticator;

    }
}