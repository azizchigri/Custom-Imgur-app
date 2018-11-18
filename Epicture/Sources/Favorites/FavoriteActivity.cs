using Android.App;
using Android.OS;
using Imgur.API.Authentication.Impl;
using Epicture.Login;
using Android.Widget;
using System.Linq;
using Imgur.API.Endpoints.Impl;
using System.Collections.Generic;
using Imgur.API.Models;
using System.Threading.Tasks;
using Epicture.Gallery;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Views;
using Epicture.Upload;
using Android.Content;
using Android.Support.V4.Widget;
using Android.Support.V4.View;
using System;
using System.Threading;

namespace Epicture.Favorites
{
    [Activity(Label = "Favorites", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class FavoriteActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private ImgurClient currentUser;
        private LvGalleryBinder _adapter;
        private ListView _lv;
        private IEnumerable<IGalleryItem> images;
        SwipeRefreshLayout mSwipe;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ThreadPool.QueueUserWorkItem(o => LoadUser());
            SetContentView(Resource.Layout.activity_favorite);

            mSwipe = FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_refresh);
            mSwipe.SetColorSchemeColors(Android.Resource.Color.HoloBlueBright, Android.Resource.Color.HoloBlueDark, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloRedLight);
            mSwipe.Refresh += mSwipe_Refresh;

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            _lv = FindViewById<ListView>(Resource.Id.lvFavorite);
            ThreadPool.QueueUserWorkItem(o => GetFavoriteImagesAsync());
        }

        private async Task GetFavoriteImagesAsync()
        {
            var endpoint = new AccountEndpoint(currentUser);
            images = await endpoint.GetAccountFavoritesAsync();
            _adapter = new LvGalleryBinder(this, Resource.Layout.listview_model, images.ToList(), currentUser);
            RunOnUiThread(() =>
            {
                _lv.Adapter = _adapter;
                _lv.ItemClick += lv_ItemClick;
            });
        }

        void mSwipe_Refresh(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(o => GetFavoriteImagesAsync());
            RunOnUiThread(() =>
            {
                mSwipe.Refreshing = false;
            });
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_gallery)
            {
                Intent intent = new Intent(this, typeof(Home));
                StartActivity(intent);
            }
            else if (id == Resource.Id.nav_uploaded)
            {
                Intent intent = new Intent(this, typeof(UploadedImages));
                StartActivity(intent);
            }
            else if (id == Resource.Id.nav_search)
            {
                Intent intent = new Intent(this, typeof(SearchInGallery));
                StartActivity(intent);
            }
            else if (id == Resource.Id.nav_favorites)
            {
                return false;
            }
            else if (id == Resource.Id.nav_disconnect)
            {
                LoginActivity.Disconnect();
                Intent intent = new Intent(this, typeof(LoginActivity));
                StartActivity(intent);
                FinishAffinity();
            }
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }

        private void lv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            GalleryFragmentActivity.images = this.images.ToList();
            var activity = new Intent(this, typeof(GalleryFragmentActivity));
            activity.PutExtra("position", e.Position);
            StartActivity(activity);
        }

        private async void LoadUser()
        {
            currentUser = LoginActivity.GetImgurClient();
            var endpoint = new AccountEndpoint(currentUser);
            IAccountSettings submissions = await endpoint.GetAccountSettingsAsync();
            RunOnUiThread(() =>
            {
                //Bind user infos
                TextView username = FindViewById<TextView>(Resource.Id.userName);
                username.Text = submissions.AccountUrl;
                TextView usermail = FindViewById<TextView>(Resource.Id.userMail);
                usermail.Text = submissions.Email;
            });
        }
    }
}