using Android.Views;
using Android.Widget;

namespace Epicture.Utils
{
    class LvHolder
    {
        public TextView NameTxt;
        public ImageView Img;
        public ImageButton button;

        public LvHolder(View v)
        {
            this.NameTxt = v.FindViewById<TextView>(Resource.Id.imgTitle);
            this.Img = v.FindViewById<ImageView>(Resource.Id.imgurImg);
            this.button = v.FindViewById<ImageButton>(Resource.Id.likeButton);
        }
    }
}