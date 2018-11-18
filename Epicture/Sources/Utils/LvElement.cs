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
using Java.Lang;

namespace Epicture.Sources.Utils
{
    class LvElement
    {
        public string Name { set; get; }
        public string Link { set; get; }
        public bool Favorite { set; get; }

        public string Id { set; get; }

        public string Description { set; get; }

        public LvElement(string id, string name, string link, bool favorite, string description)
        {
            Id = id;
            Name = name;
            Link = link;
            Favorite = favorite;
            Description = description;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}