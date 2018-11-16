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
    class GalleryFragmentAdapter : FragmentPagerAdapter
    {
        List<IGalleryItem> images;

        public GalleryFragmentAdapter(Android.Support.V4.App.FragmentManager fm, List<IGalleryItem> imageList)
        : base(fm)
        {
            this.images = imageList;
        }

        public override int Count
        {
            get { return images.Count(); }
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            if (typeof(IGalleryImage).IsAssignableFrom(images[position].GetType()))
            {
                IGalleryImage img = (IGalleryImage)images[position];
                return ImageFragment.newInstance(
                    img.Title, img.Description, img.Link);
            }
            else
            {
                IGalleryAlbum img = (IGalleryAlbum)images[position];
                return ImageFragment.newInstance(
                    img.Title, img.Description, img.Link);
            }
        }
    }
}