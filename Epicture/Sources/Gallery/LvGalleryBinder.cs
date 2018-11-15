using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using Epicture.Utils;
using Imgur.API.Models;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Request;
using Imgur.API.Models.Impl;
using System;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using System.Threading;

namespace Epicture.Gallery
{
    class LvGalleryBinder : ArrayAdapter
    {
        private Context c;
        private List<IGalleryItem> images;
        private LayoutInflater inflater;
        private int resource;
        private ImgurClient client;

        public LvGalleryBinder(Context context, int resource, List<IGalleryItem> images, ImgurClient client) : base(context, resource, images)
        {
            this.c = context;
            this.resource = resource;
            this.images = images;
            this.client = client;
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
            if (img.Favorite.Value)
                holder.button.SetImageResource(Resource.Drawable.like);
            else
                holder.button.SetImageResource(Resource.Drawable.dislike);
            holder.button.Click += delegate
            {
                var endpoint = new ImageEndpoint(client);
                ThreadPool.QueueUserWorkItem(o => endpoint.FavoriteImageAsync(img.Id));
                img.Favorite = !img.Favorite;
                if (img.Favorite.Value)
                    holder.button.SetImageResource(Resource.Drawable.like);
                else
                    holder.button.SetImageResource(Resource.Drawable.dislike);
            };
            Glide
                .With(this.c)
                .Load(img.Link)
                .Apply(RequestOptions.CircleCropTransform()).Into(holder.Img);
            return holder;
        }

        private LvHolder forImages(int position, View convertView)
        {
            IGalleryImage img = (IGalleryImage)images[position];
            LvHolder holder = new LvHolder(convertView)
            {
                NameTxt = { Text = img.Title }

            };
            if (img.Favorite.Value)
                holder.button.SetImageResource(Resource.Drawable.like);
            else
                holder.button.SetImageResource(Resource.Drawable.dislike);
            holder.button.Click += delegate
            {
                var endpoint = new ImageEndpoint(client);
                ThreadPool.QueueUserWorkItem(o => endpoint.FavoriteImageAsync(img.Id));
                img.Favorite = !img.Favorite;
                if (img.Favorite.Value)
                    holder.button.SetImageResource(Resource.Drawable.like);
                else
                    holder.button.SetImageResource(Resource.Drawable.dislike);
            };
            Glide
                .With(this.c)
                .Load(img.Link)
                .Apply(RequestOptions.CircleCropTransform()).Into(holder.Img);
            return holder;
        }

    }
}