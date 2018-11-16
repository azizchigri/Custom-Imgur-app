using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.App;
using Android.OS;
using Epicture.Sources.ImageSample;
using Epicture.Upload;
using System.Linq;
using Imgur.API.Models;
using System.Collections.Generic;

namespace Epicture
{
    [Activity(Label = "GalleryFragmentActivity", MainLauncher = false)]
    public class GalleryFragmentActivity : FragmentActivity
    {
        public static List<IGalleryItem> images;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.image_sample);
            ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            GalleryFragmentAdapter adapter = new GalleryFragmentAdapter(SupportFragmentManager, images);
            viewPager.Adapter = adapter;
            var position = Intent.GetSerializableExtra("position").ToString();
            viewPager.SetCurrentItem(int.Parse(position), false);
        }
    }
}