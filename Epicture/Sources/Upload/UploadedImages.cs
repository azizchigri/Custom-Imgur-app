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
using Android.Support.Design.Widget;
using Android.Content;
using Android.Support.V4.Widget;
using Android.Support.V4.View;
using Android.Views;
using System;
using Android.Support.V7.App;
using Epicture.Gallery;
using Epicture.Favorites;
using System.Threading;
using Newtonsoft.Json;
using System.ComponentModel;
using Epicture.Sources.Utils;

namespace Epicture.Upload
{
    [Activity(Label = "My images", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class UploadedImages : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private ImgurClient currentUser;
        private LvImgBinder _adapter;
        private ListView _lv;
        public static readonly int UploadImageId = 2000;
        private List<LvEntity> images = null;
        SwipeRefreshLayout mSwipe;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ThreadPool.QueueUserWorkItem(o => LoadUser());
            SetContentView(Resource.Layout.activity_upload);

            mSwipe = FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_refresh);
            mSwipe.SetColorSchemeColors(Android.Resource.Color.HoloBlueBright, Android.Resource.Color.HoloBlueDark, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloRedLight);
            mSwipe.Refresh += mSwipe_Refresh;

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);
            _lv = FindViewById<ListView>(Resource.Id.lvUpload);

            SearchView searchButton = FindViewById<SearchView>(Resource.Id.filterUpload);
            searchButton.SetQueryHint("Enter your filter query");
            searchButton.QueryTextChange += (sender, e) =>
            {
                ThreadPool.QueueUserWorkItem(o => GetImagesAsync(0, e.NewText));
            };

            ThreadPool.QueueUserWorkItem(o => GetImagesAsync(0, null));
        }

        private async Task GetImagesAsync(int delay, string query)
        {
            await Task.Delay(delay);
            var endpoint = new AccountEndpoint(currentUser);
            IEnumerable<IImage> images = await endpoint.GetImagesAsync();
            if (this.images != null)
                this.images.Clear();
            this.images = FilterClass<IImage>.convertList(query, images.ToList());
            _adapter = new LvImgBinder(this, Resource.Layout.listview_model, this.images, currentUser);
            RunOnUiThread(() =>
            {
                _lv.Adapter = _adapter;
                _lv.ItemClick += lv_ItemClick;
            });
        }

        void mSwipe_Refresh(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(o => GetImagesAsync(0, null));
            RunOnUiThread(() =>
            {
                mSwipe.Refreshing = false;
            });
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            StartActivityForResult(typeof(UploadActivity), UploadImageId);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if ((requestCode == UploadImageId) && (resultCode == Result.Ok))
            {
                var updateDelay = 6000;
                ThreadPool.QueueUserWorkItem(o => GetImagesAsync(updateDelay, null));
                View view = this.CurrentFocus;
                Snackbar.Make(view, "Updating images, please wait...", duration: updateDelay)
                    .SetAction("Update", (View.IOnClickListener)null).Show();
            }
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
                return false;
            }
            else if (id == Resource.Id.nav_search)
            {
                Intent intent = new Intent(this, typeof(SearchInGallery));
                StartActivity(intent);
            }
            else if (id == Resource.Id.nav_favorites)
            {
                Intent intent = new Intent(this, typeof(FavoriteActivity));
                StartActivity(intent);
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
            ImageFragmentActivity.images = this.images;
            var activity = new Intent(this, typeof(ImageFragmentActivity));
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