using System.Collections.Generic;
using Epicture.Utils;
using Android.Content;
using Android.Views;
using Android.Widget;
using Imgur.API.Models;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Request;
using Imgur.API.Endpoints.Impl;
using System.Threading;
using Imgur.API.Authentication.Impl;
using Epicture.Sources.Utils;

namespace Epicture.Upload
{
    class LvImgBinder : ArrayAdapter
    {
        private Context c;
        private List<LvEntity> images;
        private LayoutInflater inflater;
        private int resource;
        private ImgurClient client;

        public LvImgBinder(Context context, int resource, List<LvEntity> images, ImgurClient client) : base(context, resource, images)
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

            LvHolder holder = new LvHolder(convertView)
            {
                NameTxt = { Text = images[position].Name }

            };
            holder.button.Focusable = false;
            holder.button.FocusableInTouchMode = false;
            holder.button.Clickable = true;
            if (images[position].Favorite)
                holder.button.SetImageResource(Resource.Drawable.like);
            else
                holder.button.SetImageResource(Resource.Drawable.dislike);
            holder.button.Click += delegate
            {
                if (images[position].type == LvEntity.ImgType.IMAGE)
                {
                    likeImage(position, holder);
                }
                else
                {
                    likeAlbum(position, holder);
                }
            };
            Glide
                .With(this.c)
                .Load(images[position].Link)
                .Apply(RequestOptions.CircleCropTransform()).Into(holder.Img);

            convertView.SetBackgroundColor(Constants.lv_Background);

            return convertView;
        }

        void likeImage(int position, LvHolder holder)
        {
            var endpoint = new ImageEndpoint(client);
            ThreadPool.QueueUserWorkItem(o => endpoint.FavoriteImageAsync(images[position].Id));
            images[position].Favorite = !images[position].Favorite;
            if (images[position].Favorite)
                holder.button.SetImageResource(Resource.Drawable.like);
            else
                holder.button.SetImageResource(Resource.Drawable.dislike);
        }

        void likeAlbum(int position, LvHolder holder)
        {
            var endpoint = new AlbumEndpoint(client);
            ThreadPool.QueueUserWorkItem(o => endpoint.FavoriteAlbumAsync(images[position].Id));
            images[position].Favorite = !images[position].Favorite;
            if (images[position].Favorite)
                holder.button.SetImageResource(Resource.Drawable.like);
            else
                holder.button.SetImageResource(Resource.Drawable.dislike);
        }
    }
}
