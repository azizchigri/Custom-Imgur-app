using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;

namespace Epicture.Sources.ImageSample
{
    public class ImageFragment : Android.Support.V4.App.Fragment
    {
        public ImageFragment() { }

        private static string PICTURE_TITLE = "title";
        private static string PICTURE_DESCRIPTION = "description";
        private static string PICTURE_LINK = "link";

        public static ImageFragment newInstance(string title, string description, string pictureLink)
        {
            ImageFragment fragment = new ImageFragment();
            Bundle args = new Bundle();
            args.PutString(PICTURE_TITLE, title);
            args.PutString(PICTURE_DESCRIPTION, description);
            args.PutString(PICTURE_LINK, pictureLink);
            fragment.Arguments = args;

            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            string title = Arguments.GetString(PICTURE_TITLE, "");
            string description = Arguments.GetString(PICTURE_DESCRIPTION, "");
            string link = Arguments.GetString(PICTURE_LINK, "");

            View view = inflater.Inflate(Resource.Layout.content_image, container, false);
            TextView pictureTitle = (TextView)view.FindViewById(Resource.Id.pictureTitle);
            pictureTitle.Text = title;
            TextView pictureDesc = (TextView)view.FindViewById(Resource.Id.pictureDescription);
            pictureDesc.Text = description;
            ImageView picture = (ImageView)view.FindViewById(Resource.Id.picture);
            Glide
                .With(Context)
                .Load(link)
                .Into(picture);
            return view;
        }
    }
}