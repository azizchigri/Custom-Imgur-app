using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Epicture.Login;

namespace Epicture
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, Icon = "@drawable/logo", NoHistory = true)]
    public class MainActivity : AppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StartActivity(typeof(LoginActivity));
            Finish();
        }
    }
}

