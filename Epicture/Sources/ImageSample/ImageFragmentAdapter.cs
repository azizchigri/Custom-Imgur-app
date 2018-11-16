using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Imgur.API.Models;

namespace Epicture.Sources.ImageSample
{
    class ImageFragmentAdapter : FragmentPagerAdapter
    {
        List<IImage> imageList;

        public ImageFragmentAdapter(Android.Support.V4.App.FragmentManager fm, List<IImage> imageList)
        : base(fm)
        {
            this.imageList = imageList;
        }

        public override int Count
        {
            get { return imageList.Count(); }
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return ImageFragment.newInstance(
                    imageList[position].Title, imageList[position].Description, imageList[position].Link);
        }
    }
}