﻿using Android.App;
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

namespace Epicture.Upload
{
    [Activity(Label = "My images", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class UploadedImages : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private ImgurClient currentUser;
        private LvImgBinder _adapter;
        private ListView _lv;
        public static readonly int UploadImageId = 2000;
        private IEnumerable<IImage> images;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ThreadPool.QueueUserWorkItem(o => LoadUser());
            SetContentView(Resource.Layout.activity_upload);
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

            Button button = FindViewById<Button>(Resource.Id.pickButton);
            button.Click += (sender, e) =>
            {
                StartActivityForResult(typeof(UploadActivity), UploadImageId);
            };
            ThreadPool.QueueUserWorkItem(o => GetImagesAsync());
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if ((requestCode == UploadImageId) && (resultCode == Result.Ok))
            {
                ThreadPool.QueueUserWorkItem(o => GetImagesAsync());
                View view = this.CurrentFocus;
                Snackbar.Make(view, "Updating images, please wait...", Snackbar.LengthLong)
                    .SetAction("Update", (View.IOnClickListener)null).Show();
            }
        }

        private async Task GetImagesAsync()
        {
            var endpoint = new AccountEndpoint(currentUser);
            images = await endpoint.GetImagesAsync();
            _adapter = new LvImgBinder(this, Resource.Layout.listview_model, images.ToList(), currentUser);
            RunOnUiThread(() =>
            {
                _lv.Adapter = _adapter;
                _lv.ItemClick += lv_ItemClick;
            });
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            ThreadPool.QueueUserWorkItem(o => GetImagesAsync());
            View view = (View)sender;
            Snackbar.Make(view, "Updating images, please wait...", Snackbar.LengthLong)
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
            ImageFragmentActivity.images = this.images.ToList();
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