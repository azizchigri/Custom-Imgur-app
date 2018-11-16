using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Epicture.Login;
using Epicture.Sources.Upload;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;

namespace Epicture.Upload
{
    [Activity(Label = "UploadActivity")]
    public class UploadActivity : Activity
    {
        public static readonly int PickImageId = 1000;
        public static readonly int UploadImageId = 2000;

        private ImgurClient currentUser;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            currentUser = LoginActivity.GetImgurClient();
            ButtonOnClick();
        }
        // Create a Method ButtonOnClick.   
        private void ButtonOnClick()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted)
            {
                Intent = new Intent();
                Intent.SetType("image/*");
                Intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);
            }
            else
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.ReadExternalStorage }, 0);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted)
            {
                Intent = new Intent();
                Intent.SetType("image/*");
                Intent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);
            }
        }

        private void StartUploadImageActivity(Intent data)
        {
            string imagePath = GetPathToImage(data.Data);
            var intent = new Intent(this, typeof(SendImageToUpload));
            intent.PutExtra("path", imagePath);
            StartActivityForResult(intent, UploadImageId);
        }

        // Create a Method OnActivityResult(it is select the image controller)   
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
            {
                StartUploadImageActivity(data);
            }
            else if ((requestCode == PickImageId) && (resultCode == Result.Canceled))
            {
                Finish();
            }
            else if ((requestCode == UploadImageId) && (resultCode == Result.Ok))
            {
                Intent returnIntent = new Intent();
                SetResult(Result.Ok, returnIntent);
                Finish();
            }
            else if ((requestCode == UploadImageId) && (resultCode == Result.Canceled))
            {
                Finish();
            }
        }

        private string GetPathToImage(Android.Net.Uri uri)
        {
            ICursor cursor = this.ContentResolver.Query(uri, null, null, null, null);
            cursor.MoveToFirst();
            string document_id = cursor.GetString(0);
            document_id = document_id.Split(':')[1];
            cursor.Close();

            cursor = ContentResolver.Query(
            MediaStore.Images.Media.ExternalContentUri,
            null, MediaStore.Images.Media.InterfaceConsts.Id + " = ? ", new String[] { document_id }, null);
            cursor.MoveToFirst();
            string path = cursor.GetString(cursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.Data));
            cursor.Close();

            return path;
        }
    }
}