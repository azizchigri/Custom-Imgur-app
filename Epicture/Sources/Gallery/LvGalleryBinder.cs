﻿using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Epicture.Utils;
using Imgur.API.Models;
using Square.Picasso;

namespace Epicture.Gallery
{
    class LvGalleryBinder : ArrayAdapter
    {
        private Context c;
        private List<IGalleryItem> images;
        private LayoutInflater inflater;
        private int resource;

        public LvGalleryBinder(Context context, int resource, List<IGalleryItem> images) : base(context, resource, images)
        {
            this.c = context;
            this.resource = resource;
            this.images = images;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (inflater == null)
            {
                inflater = (LayoutInflater)c.GetSystemService(Context.LayoutInflaterService);
            }

            if (convertView == null)
            {
                convertView = inflater.Inflate(resource, parent, false);
            }
            LvHolder holder = null;
            if (typeof(IGalleryImage).IsAssignableFrom(images[position].GetType()))
                holder = forImages(position, convertView);
            else if (typeof(IGalleryAlbum).IsAssignableFrom(images[position].GetType()))
                holder = forAlbum(position, convertView);
            // Have to add Animated img
            convertView.SetBackgroundColor(Constants.lv_Background);

            return convertView;
        }

        private LvHolder forAlbum(int position, View convertView)
        {
            IGalleryAlbum img = (IGalleryAlbum)images[position];
            LvHolder holder = new LvHolder(convertView)
            {
                NameTxt = { Text = img.Title }

            };
            Picasso.With(this.c).Load(img.Link).Into(holder.Img);
            return holder;
        }

        private LvHolder forImages(int position, View convertView)
        {
            IGalleryImage img = (IGalleryImage)images[position];
            LvHolder holder = new LvHolder(convertView)
            {
                NameTxt = { Text = img.Title }

            };
            Picasso.With(this.c).Load(img.Link).Into(holder.Img);
            return holder;
        }

    }
}