using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Epicture.Favorites;
using Epicture.Gallery;
using Epicture.Login;
using Epicture.Sources.Utils;
using Epicture.Upload;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;

namespace Epicture
{
    [Activity(Label = "Gallery", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class Home : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private static ImgurClient currentUser = null;
        private LvImgBinder _adapter;
        private ListView _lv;
        private List<LvEntity> images = null;
        SwipeRefreshLayout mSwipe;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ThreadPool.QueueUserWorkItem(o => LoadUser());
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            mSwipe = FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_refresh);
            mSwipe.SetColorSchemeColors(Android.Resource.Color.HoloBlueBright, Android.Resource.Color.HoloBlueDark, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloRedLight);
            mSwipe.Refresh += mSwipe_Refresh;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            _lv = FindViewById<ListView>(Resource.Id.lvGallery);

            SearchView searchButton = FindViewById<SearchView>(Resource.Id.filterGallery);
            searchButton.SetQueryHint("Enter your filter query");
            searchButton.QueryTextChange += (sender, e) =>
            {
                ThreadPool.QueueUserWorkItem(o => GetGalleryImagesAsync(e.NewText));
            };

            ThreadPool.QueueUserWorkItem(o => GetGalleryImagesAsync(null));
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

        private async Task GetGalleryImagesAsync(string query)
        {
            var endpoint = new GalleryEndpoint(new ImgurClient(Constants.appId));
            IEnumerable<IGalleryItem> images = await endpoint.GetGalleryAsync();
            if (this.images != null)
                this.images.Clear();
            this.images = FilterClass<IGalleryItem>.convertList(query, images.ToList());
            _adapter = new LvImgBinder(this, Resource.Layout.listview_model, this.images, currentUser);
            RunOnUiThread(() =>
            {
                _lv.Adapter = _adapter;
                _lv.ItemClick += lv_ItemClick;
            });
        }

        void mSwipe_Refresh(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(o => GetGalleryImagesAsync(null));
            RunOnUiThread(() =>
            {
                mSwipe.Refreshing = false;
            });
        }

        private void lv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            ImageFragmentActivity.images = this.images;
            var activity = new Intent(this, typeof(ImageFragmentActivity));
            activity.PutExtra("position", e.Position);
            StartActivity(activity);
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return false;
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_gallery)
            {
                return false;
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
                Intent intent = new Intent(this, typeof(FavoriteActivity));
                StartActivity(intent);
            }
            else if (id == Resource.Id.nav_disconnect)
            {
                LoginActivity.Disconnect();
                Intent intent = new Intent(this, typeof(LoginActivity));
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
                StartActivity(intent);
                FinishAffinity();
            }
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
    }
}

