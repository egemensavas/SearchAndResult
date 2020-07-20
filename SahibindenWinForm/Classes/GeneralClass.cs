using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using static SahibindenWinForm.Models.GeneralModel;

namespace SahibindenWinForm.Classes
{
    public class GeneralClass
    {
        #region Properties
        readonly SQLClass SQLClass;
        readonly HTMLCriteriaClass HTMLCriteriaClass;
        enum AdvertType
        {
            Item = 1,
            RealEstate = 2,
            Car = 3
        }
        #endregion

        #region Constructors
        public GeneralClass()
        {
            SQLClass = new SQLClass();
            HTMLCriteriaClass = new HTMLCriteriaClass();
        }
        #endregion

        #region Helper Methods
        public string ReplaceNonAnsiChars(string Input)
        {
            string result = Input;
            NameValueCollection settingCollection = (NameValueCollection)ConfigurationManager.GetSection("CustomAppSettingsForChars");
            string[] allKeys = settingCollection.AllKeys;
            foreach (string key in allKeys)
                result = result.Replace(key, settingCollection[key]);
            return result;
        }

        public string GetDataFromSplittedHTML(SearchCriteria SC, string Input)
        {
            string result = "";
            if (Input.IndexOf(SC.SearchStart) >= 0 && Input.IndexOf(SC.SearchEnd) >= 0)
            {
                string tempString = Input.Substring(Input.IndexOf(SC.SearchStart) + SC.SearchStart.Length - 1);
                if (tempString.IndexOf(SC.SearchEnd) > -1)
                    tempString = tempString.Substring(1, tempString.IndexOf(SC.SearchEnd) - 1);
                if (!string.IsNullOrEmpty(tempString) && !string.IsNullOrEmpty(tempString))
                    result = tempString;
            }
            return result;
        }

        internal List<int> DataTabletoIntList(DataTable dtAdvert)
        {
            List<int> result = new List<int>();
            foreach (DataRow item in dtAdvert.Rows)
            {
                result.Add(Convert.ToInt32(item[0]));
            }
            return result;
        }

        public string TrimHelper(DivisionCriteria TC, string Input)
        {
            string result = "";
            if (!string.IsNullOrEmpty(TC.DivisionStart) && !string.IsNullOrEmpty(TC.DivisionEnd))
            {
                if (Input.IndexOf(TC.DivisionStart) >= 0 && Input.IndexOf(TC.DivisionEnd) >= 0)
                {
                    result = Input.Substring(Input.IndexOf(TC.DivisionStart) + TC.DivisionStart.Length);
                    result = result.Substring(0, result.IndexOf(TC.DivisionEnd));
                }
            }
            return result;
        }

        public List<string> SplitStringHelper(SplitStringCriteria SSC, string Input)
        {
            List<string> result = new List<string>();
            string alteredInput = Input;
            string temp;
            if (!string.IsNullOrEmpty(SSC.SplitString))
            {
                while (alteredInput.Contains(SSC.SplitString))
                {
                    temp = alteredInput.Substring(0, alteredInput.IndexOf(SSC.SplitString) - 1);
                    result.Add(temp);
                    alteredInput = alteredInput.Substring(alteredInput.IndexOf(SSC.SplitString) + SSC.SplitString.Length);
                }
                result.Add(alteredInput);
            }
            return result;
        }

        public List<ResultModel> PopulateResultModel(List<string> splittedInput, int AdvertTypeID, int searchMasterID, List<int> advertDBList, out List<int> advertWebList)
        {
            List<ResultModel> ResultModelList = new List<ResultModel>();
            int advertID;
            AdvertType advertTypeID;
            advertTypeID = (AdvertType)AdvertTypeID;
            bool IsReaLEstate = advertTypeID == AdvertType.RealEstate;
            advertWebList = new List<int>();
            string AttributesContent;
            List<string> ResultAttribute = new List<string>();
            foreach (var item in splittedInput)
            {
                if (IsReaLEstate)
                {
                    AttributesContent = TrimHelper(HTMLCriteriaClass.AttributeTrimCriteria, item).Replace("    ", "");
                    ResultAttribute = SplitDivisionHelper(HTMLCriteriaClass.ResultAttributeDivisionCriteria, AttributesContent, true);
                }
                advertID = Convert.ToInt32(GetDataFromSplittedHTML(HTMLCriteriaClass.AdvertIDCriteria, item));
                advertWebList.Add(advertID);
                if (!advertDBList.Contains(advertID))
                {
                    ResultModel Addition = new ResultModel
                    {
                        AdvertDate = ConvertToDateTime(GetDataFromSplittedHTML(HTMLCriteriaClass.DateCriteria, item).Replace("</span>\n                        <br/>\n                        <span>", " ")),
                        Description = GetDataFromSplittedHTML(HTMLCriteriaClass.DescriptionCriteria, item),
                        AdvertID = advertID,
                        Location = GetDataFromSplittedHTML(HTMLCriteriaClass.LocationCriteria, item).Replace("<br/>", " - "),
                        Price = Convert.ToInt32(GetDataFromSplittedHTML(HTMLCriteriaClass.PriceCriteria, item).Replace(".", "").Replace(" TL", "")),
                        SearchMasterID = searchMasterID,
                        ThumbnailLink = GetDataFromSplittedHTML(HTMLCriteriaClass.ThumbnailCriteria, item),
                        Size = IsReaLEstate ? Convert.ToInt32(ResultAttribute[0]) : 0,
                        Room = IsReaLEstate ? ResultAttribute[1] : "",
                        Heating = IsReaLEstate ? ResultAttribute[2] : ""
                    };
                    ResultModelList.Add(Addition);
                }
            }
            return ResultModelList;
        }

        public void MarkAsDeleted(List<int> advertDBList, List<int> advertWebList)
        {
            string SQLCommand = "";
            foreach (var item in advertDBList)
            {
                if (!advertWebList.Contains(item))
                {
                    if (string.IsNullOrEmpty(SQLCommand))
                        SQLCommand = "UPDATE TABLE_ADVERT SET ISDELETED = 1 WHERE ADVERTID IN (";
                    SQLCommand += item.ToString() + ", ";
                }
            }
            if (!string.IsNullOrEmpty(SQLCommand))
                SQLClass.GetDataTable(SQLCommand + "0)");
        }

        public List<string> SplitDivisionHelper(DivisionCriteria SDC, string Input, bool IsRemoveDivision)
        {
            List<string> result = new List<string>();
            string alteredInput = Input;
            string temp;
            if (!string.IsNullOrEmpty(SDC.DivisionStart) && !string.IsNullOrEmpty(SDC.DivisionEnd))
            {
                while (alteredInput.Contains(SDC.DivisionStart) && alteredInput.Contains(SDC.DivisionEnd))
                {
                    temp = alteredInput.Substring(alteredInput.IndexOf(SDC.DivisionStart), alteredInput.IndexOf(SDC.DivisionEnd) + SDC.DivisionEnd.Length);
                    if (IsRemoveDivision)
                        temp = temp.Replace(SDC.DivisionStart, "").Replace(SDC.DivisionEnd, "").Replace("\n", "");
                    result.Add(temp);
                    alteredInput = alteredInput.Substring(alteredInput.IndexOf(SDC.DivisionEnd) + SDC.DivisionEnd.Length);
                }
                if (!IsRemoveDivision)
                    result.Add(alteredInput);
            }
            foreach (var item in result.ToList())
            {
                if (!IsRemoveDivision && (!item.Contains(SDC.DivisionStart) || !item.Contains(SDC.DivisionEnd)))
                    result.Remove(item);
            }
            return result;
        }

        public string CleanData(string Input)
        {
            string result = Input.Replace("src=\"https://image5.", "");
            result = result.Replace("src=\"https://s0.", "");
            result = result.Replace("<div class=\"installmentCount\">\n            <span>", "");
            result = result.Replace("\" alt=\"", "\"\n alt=\"");
            return result;
        }

        public DateTime ConvertToDateTime(string input)
        {
            DateTime result;

            string month = input.Substring(input.IndexOf(" "), input.LastIndexOf(" ") - 1);
            string alteredMonth = "";

            switch (month)
            {
                case " Ocak ":
                    alteredMonth = ".1.";
                    break;
                case " Şubat ":
                    alteredMonth = ".2.";
                    break;
                case " Mart ":
                    alteredMonth = ".3.";
                    break;
                case " Nisan ":
                    alteredMonth = ".4.";
                    break;
                case " Mayıs ":
                    alteredMonth = ".5.";
                    break;
                case " Haziran ":
                    alteredMonth = ".6.";
                    break;
                case " Temmuz ":
                    alteredMonth = ".7.";
                    break;
                case " Ağustos ":
                    alteredMonth = ".8.";
                    break;
                case " Eylül ":
                    alteredMonth = ".9.";
                    break;
                case " Ekim ":
                    alteredMonth = ".10.";
                    break;
                case " Kasım ":
                    alteredMonth = ".11.";
                    break;
                case " Aralık ":
                    alteredMonth = ".12.";
                    break;
            }

            input = input.Replace(month, alteredMonth);

            result = Convert.ToDateTime(input);

            return result;
        }

        public List<string> GetAppConfigStringArray(string CustomAppSetting)
        {
            NameValueCollection settingCollection = (NameValueCollection)ConfigurationManager.GetSection(CustomAppSetting);
            List<string> result = new List<string>();
            string[] allKeys = settingCollection.AllKeys;
            foreach (string key in allKeys)
                result.Add(settingCollection[key]);
            return result;
        }

        public void CreateRequiredFolders(List<string> RequiredFolders, string AppStartUpPath)
        {
            foreach (var folder in RequiredFolders)
            {
                if (!Directory.Exists(AppStartUpPath + folder))
                    Directory.CreateDirectory(AppStartUpPath + folder);
            }
        }

        public DataTable ConvertListToDataTable(List<ResultModel> ResultModelList)
        {
            using (DataTable DataTable = SQLClass.GetDataTable("SELECT * FROM TABLE_ADVERT (NOLOCK) WHERE 1 = 2"))
            {
                foreach (ResultModel item in ResultModelList)
                {
                    DataRow dr = DataTable.NewRow();
                    dr["SearchMasterID"] = item.SearchMasterID;
                    dr["CreateDate"] = DateTime.Now;
                    dr["IsSeen"] = false;
                    dr["AdvertID"] = item.AdvertID;
                    dr["Description"] = item.Description;
                    dr["ThumbnailLink"] = item.ThumbnailLink;
                    dr["Location"] = item.Location;
                    dr["AdvertDate"] = item.AdvertDate;
                    dr["Price"] = item.Price;
                    dr["Size"] = item.Size;
                    dr["Room"] = item.Room;
                    dr["Heating"] = item.Heating;
                    DataTable.Rows.Add(dr);
                }
                return DataTable;
            }
        }
        #endregion
    }
}
