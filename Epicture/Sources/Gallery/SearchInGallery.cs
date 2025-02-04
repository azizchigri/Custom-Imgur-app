﻿using Android.App;
using Android.OS;
using Epicture.Login;
using Android.Widget;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Imgur.API.Authentication.Impl;
using Imgur.API.Models;
using Imgur.API.Endpoints.Impl;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Support.V4.Widget;
using Android.Content;
using Android.Support.V4.View;
using Epicture.Upload;
using System;
using Epicture.Favorites;
using System.Threading;
using Epicture.Sources.Utils;

namespace Epicture.Gallery
{
    [Activity(Label = "Search", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class SearchInGallery : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private ImgurClient currentUser;
        private LvImgBinder _adapter;
        private ListView _lv;
        private List<LvEntity> images = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ThreadPool.QueueUserWorkItem(o => LoadUser());
            SetContentView(Resource.Layout.activity_search);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            _lv = FindViewById<ListView>(Resource.Id.lvSearched);

            SearchView searchButton = FindViewById<SearchView>(Resource.Id.searchButton);
            searchButton.SetQueryHint("Enter your query");
            searchButton.SetIconifiedByDefault(false);
            searchButton.QueryTextSubmit += (sender, e) =>
            {
                ThreadPool.QueueUserWorkItem(o => GetGalleryImagesAsync(searchButton.Query));
            };
        }

        private async Task GetGalleryImagesAsync(string query)
        {
            var endpoint = new GalleryEndpoint(currentUser);
            IEnumerable<IGalleryItem> images = await endpoint.SearchGalleryAsync(query);
            if (this.images != null)
                this.images.Clear();
            this.images = FilterClass<IGalleryItem>.convertList(null, images.ToList());
            _adapter = new LvImgBinder(this, Resource.Layout.listview_model, this.images, currentUser);
            RunOnUiThread(() =>
            {
                _lv.Adapter = _adapter;
                _lv.ItemClick += lv_ItemClick;
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
                return false;
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