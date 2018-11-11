using Android.App;
using Android.OS;
using Epicture.Login;
using Android.Widget;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Imgur.API.Authentication.Impl;
using Imgur.API.Models;
using Imgur.API.Endpoints.Impl;

namespace Epicture.Gallery
{
    [Activity(Label = "Gallery", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class SearchInGallery : Activity
    {
        private ImgurClient currentUser;
        private LvGalleryBinder _adapter;
        private ListView _lv;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            LoadUser();
            //SetContentView(Resource.Layout.activity_gallery);

            _lv = FindViewById<ListView>(Resource.Id.lvGallery);
            //EditText filterText = FindViewById<EditText>(Resource.Id.galleryFilter);
            Button searchButton = FindViewById<Button>(Resource.Id.buttonPanel);
            searchButton.Click += (sender, e) =>
            {
                //GetGalleryImagesAsync(filterText.Text);
            };
        }

        private async Task GetGalleryImagesAsync(string query)
        {
            var endpoint = new GalleryEndpoint(currentUser);
            IEnumerable<IGalleryItem> images = await endpoint.SearchGalleryAsync(query);
            _adapter = new LvGalleryBinder(this, Resource.Layout.listview_model, images.ToList());
            _lv.Adapter = _adapter;
            _lv.ItemClick += lv_ItemClick;
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