using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using Epicture.Login;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;

namespace Epicture.Sources.Upload
{
    [Activity(Label = "SendImageToUpload")]
    public class SendImageToUpload : Activity
    {
        private ImgurClient currentUser;
        private string imagePath;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_edit_image);

            currentUser = LoginActivity.GetImgurClient();
            imagePath = Intent.GetStringExtra("path");
            var img = FindViewById<ImageView>(Resource.Id.imgvImageUpload);
            Glide.With(this)
                .Load(imagePath)
                .Into(img);

            var btn = FindViewById<Button>(Resource.Id.btnUpload);
            btn.Enabled = false;
            var title = FindViewById<EditText>(Resource.Id.txtiTitleUpload);
            title.AfterTextChanged += (s, e) =>
            {
                if (title.Text == null || title.Text == "")
                {
                    btn.Enabled = false;
                }
                else
                {
                    btn.Enabled = true;
                }
            };
            btn.Click += btn_OnClick;
        }

        private void btn_OnClick(object sender, EventArgs eventArgs)
        {
            var title = FindViewById<EditText>(Resource.Id.txtiTitleUpload);
            var description = FindViewById<EditText>(Resource.Id.txtiDescriptionUpload);
            var endpoint = new ImageEndpoint(currentUser);
            var file = System.IO.File.ReadAllBytes(imagePath);
            ThreadPool.QueueUserWorkItem(o => endpoint.UploadImageBinaryAsync(file, title: title.Text, description: description.Text));
            Intent returnIntent = new Intent();
            SetResult(Result.Ok, returnIntent);
            Finish();
        }
    }
}