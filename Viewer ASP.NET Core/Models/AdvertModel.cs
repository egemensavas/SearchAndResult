using System;

namespace Viewer_ASP.NET_Core
{
    public class AdvertModel
    {
        public string AdvertLink { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public string ThumbnailLink { get; set; }

        public string AdvertDate { get; set; }

        public string Price { get; set; }
        public string Size { get; set; }
        public string Room { get; set; }
        public string Heating { get; set; }
        public int Price_sort { get; set; }
        public int SearchMasterID { get; set; }
        public long Date_sort { get; set; }
    }
}
