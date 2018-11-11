using System.Collections.Generic;
using Epicture.Utils;
using Android.Content;
using Android.Views;
using Android.Widget;
using Imgur.API.Models;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Request;

namespace Epicture.Upload
{
    class LvImgBinder :  ArrayAdapter
    {
        private Context c;
        private List<IImage> images;
        private LayoutInflater inflater;
        private int resource;

        public LvImgBinder(Context context, int resource, List<IImage> images) : base(context, resource, images)
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

            LvHolder holder = new LvHolder(convertView)
            {
                NameTxt = { Text = images[position].Name ?? images[position].Title }

            };
            Glide
                .With(this.c)
                .Load(images[position].Link)
                .Apply(RequestOptions.CircleCropTransform()).Into(holder.Img);

            convertView.SetBackgroundColor(Constants.lv_Background);

            return convertView;
        }
    }
}
