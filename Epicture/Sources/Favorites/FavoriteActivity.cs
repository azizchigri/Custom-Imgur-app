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

namespace Epicture.Favorites
{
    [Activity(Label = "Favorites", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class FavoriteActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private ImgurClient currentUser;
        private LvGalleryBinder _adapter;
        private ListView _lv;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            LoadUser();
            SetContentView(Resource.Layout.activity_favorite);

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

            _lv = FindViewById<ListView>(Resource.Id.lvFavorite);
            GetFavoriteImagesAsync();
        }

        private async Task GetFavoriteImagesAsync()
        {
            var endpoint = new AccountEndpoint(currentUser);
            IEnumerable<IGalleryItem> images = await endpoint.GetAccountFavoritesAsync();
            _adapter = new LvGalleryBinder(this, Resource.Layout.listview_model, images.ToList());
            _lv.Adapter = _adapter;
            _lv.ItemClick += lv_ItemClick;
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            GetFavoriteImagesAsync();
            View view = (View)sender;
            Snackbar.Make(view, "Updating favorites, please wait...", Snackbar.LengthLong)
                .SetAction("Update", (View.IOnClickListener)null).Show();
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

        private void lv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Toast.MakeText(this, "Tu viens de cliquer", ToastLength.Short).Show();
        }

        private void LoadUser()
        {
            currentUser = LoginActivity.GetImgurClient();
        }
    }
}