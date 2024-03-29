﻿using System;
using System.Data;

namespace SahibindenWinForm.Models
{
    public class GeneralModel
    {
        public class SearchCriteria
        {
            public string DivisionStart { get; set; }
            public string DivisionEnd { get; set; }
            public string SearchStart { get; set; }
            public string SearchEnd { get; set; }
            public int MaxIndex { get; set; }
            public string Divider { get; set; }
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
            public int Price { get; set; }
            public string Location { get; set; }
            public int AdvertID { get; set; }
            public string Description { get; set; }
            public string ThumbnailLink { get; set; }
            public DateTime AdvertDate { get; set; }
            public int SearchMasterID { get; set; }

        }
    }
}
