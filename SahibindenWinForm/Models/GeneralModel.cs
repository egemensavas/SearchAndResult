using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Data;

namespace SahibindenWinForm.Models
{
    public class GeneralModel
    {
        public enum SearchableTypes
        {
            Keywords = 1,
            Category = 2,
            Location = 3,
            Sort = 4,
            PriceMax = 5
        }
        public enum AdvertTypes
        {
            Item = 1,
            RealEstate = 2,
            Car = 3
        }

        public class SearchCriteria
        {
            public string SearchStart { get; set; }
            public string SearchEnd { get; set; }
        }

        public class DivisionCriteria
        {
            public string DivisionStart { get; set; }
            public string DivisionEnd { get; set; }
        }

        public class SplitStringCriteria
        {
            public string SplitString { get; set; }
        }

        public class XMLReplace
        {
            public string Replaced { get; set; }
            public string Replacee { get; set; }
        }

        public class SQLParameter
        {
            public SqlDbType SQLDBType { get; set; }
            public object Value { get; set; }
        }

        public class ColumnValuePair
        {
            public string ColumnName { get; set; }
            public object Value { get; set; }
        }

        public class ResultModel
        {
#pragma warning disable IDE1006 // Naming Styles
            public ObjectId _id { get; set; }
#pragma warning restore IDE1006 // Naming Styles
            public int Price { get; set; }
            public string Location { get; set; }
            public int AdvertID { get; set; }
            public string Description { get; set; }
            public string ThumbnailLink { get; set; }
            public DateTime AdvertDate { get; set; }
            public int SearchMasterID { get; set; }
            public int Size { get; set; }
            public string Room { get; set; }
            public string Heating { get; set; }
        }

        public class SearchModel
        {
#pragma warning disable IDE1006 // Naming Styles
            public ObjectId _id { get; set; }
#pragma warning restore IDE1006 // Naming Styles
            public int SearchID { get; set; }
            public string Description { get; set; }
            public string Notes { get; set; }
            public string AdvertType { get; set; }
            public bool IsActive { get; set; }
            public string OneSignalSegment { get; set; }
            public string OneSignalMessage { get; set; }
            public List<SearchDetailModel> Detail { get; set; }
        }

        public class SearchDetailModel
        {
            public string SearchableType { get; set; }
            public string SearchableValue { get; set; }

        }
    }
}
