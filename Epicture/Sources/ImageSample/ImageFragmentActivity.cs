using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.App;
using Android.OS;
using Epicture.Sources.ImageSample;
using Epicture.Upload;
using System.Linq;
using Imgur.API.Models;
using System.Collections.Generic;
using Epicture.Sources.Utils;

namespace Epicture
{
    [Activity(Label = "ImageFragmentActivity", MainLauncher = false)]
    public class ImageFragmentActivity : FragmentActivity
    {
        public static List<LvEntity> images;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.image_sample);
            ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            ImageFragmentAdapter adapter = new ImageFragmentAdapter(SupportFragmentManager, images);
            viewPager.Adapter = adapter;
            var position = Intent.GetSerializableExtra("position").ToString();
            viewPager.SetCurrentItem(int.Parse(position), false);
        }
    }
}