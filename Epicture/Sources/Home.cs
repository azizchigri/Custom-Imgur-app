using System;
using System.Collections.Generic;
using System.Linq;
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
        private LvGalleryBinder _adapter;
        private ListView _lv;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            currentUser = LoginActivity.GetImgurClient();
            SetContentView(Resource.Layout.activity_main);
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

            _lv = FindViewById<ListView>(Resource.Id.lvGallery);

            GetGalleryImagesAsync();
        }

        private async Task GetGalleryImagesAsync()
        {
            var endpoint = new GalleryEndpoint(new ImgurClient(Constants.appId));
            IEnumerable<IGalleryItem> images = await endpoint.GetGalleryAsync();
            _adapter = new LvGalleryBinder(this, Resource.Layout.listview_model, images.ToList());
            _lv.Adapter = _adapter;
            _lv.ItemClick += lv_ItemClick;
        }

        private void lv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Toast.MakeText(this, "Tu viens de cliquer", ToastLength.Short).Show();
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
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            GetGalleryImagesAsync();
            View view = (View)sender;
            Snackbar.Make(view, "Updating gallery, please wait...", Snackbar.LengthLong)
                .SetAction("Update", (View.IOnClickListener)null).Show();
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
            else if (id == Resource.Id.nav_manage)
            {

            }
            else if (id == Resource.Id.nav_share)
            {

            }
            else if (id == Resource.Id.nav_send)
            {

            }
            Finish();
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
    }
}

