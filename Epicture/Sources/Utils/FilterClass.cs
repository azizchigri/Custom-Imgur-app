using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Imgur.API.Models;

namespace Epicture.Sources.Utils
{
    public class LvEntity
    {
        public string Name { set; get; }
        public string Link { set; get; }
        public string Description { set; get; }
        public string Id { set; get; }
        public bool Favorite { set; get; }

        public enum ImgType
        {
            IMAGE,
            ALBUM
        }

        public ImgType type { set; get; }

        public LvEntity(string Name, string Link, string Description, string Id, bool Favorite, ImgType t)
        {
            this.Name = Name;
            this.Link = Link;
            this.Description = this.Description;
            this.Id = Id;
            this.Favorite = Favorite;
            this.type = t;
        }
    }

    class FilterClass<T>
    {
        private static void PutInList(T elem, string query, List<LvEntity> list)
        {
            if (typeof(IImage).IsAssignableFrom(elem.GetType()))
            {
                IImage result = (IImage)elem;
                if (query == null || (result.Title ?? result.Name).Contains(query))
                    list.Add(new LvEntity(result.Title ?? result.Name, result.Link, result.Description, result.Id, result.Favorite.Value, LvEntity.ImgType.IMAGE));
            }
            else if (typeof(IGalleryImage).IsAssignableFrom(elem.GetType()))
            {
                IGalleryImage result = (IGalleryImage)elem;
                if (query == null || (result.Title ?? result.Name).Contains(query))
                    list.Add(new LvEntity(result.Title ?? result.Name, result.Link, result.Description, result.Id, result.Favorite.Value, LvEntity.ImgType.IMAGE));
            }
            else if (typeof(IGalleryAlbum).IsAssignableFrom(elem.GetType()))
            {
                IGalleryAlbum result = (IGalleryAlbum)elem;
                if (query == null || result.Title.Contains(query))
                    list.Add(new LvEntity(result.Title, result.Link, result.Description, result.Id, result.Favorite.Value, LvEntity.ImgType.ALBUM));
            }
        }

        public static List<LvEntity> convertList(string query, List<T> imgurList)
        {
            List<LvEntity> list = new List<LvEntity>();

            foreach (T elem in imgurList)
            {
                PutInList(elem, query, list);
            }
            return list;
        }
    }
}