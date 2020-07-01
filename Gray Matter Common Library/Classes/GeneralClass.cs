using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using static SahibindenWinForm.Models.GeneralModel;

namespace SahibindenWinForm.Classes
{
    public class GeneralClass
    {
        #region Properties
        readonly SQLClass SQLClass;
        #endregion

        #region Constructors
        public GeneralClass()
        {
            SQLClass = new SQLClass();
        }
        #endregion

        #region Helper Methods
        public string ParsingHelper(SearchCriteria SC, string Input)
        {
            string result = string.Empty;
            string tempString;
            if (!string.IsNullOrEmpty(SC.DivisionStart) && !string.IsNullOrEmpty(SC.DivisionEnd))
            {
                if (Input.IndexOf(SC.DivisionStart) > 0 && Input.IndexOf(SC.DivisionEnd) > 0)
                {
                    Input = Input.Substring(Input.IndexOf(SC.DivisionStart) - 1);
                    Input = Input.Substring(0, Input.IndexOf(SC.DivisionEnd));
                }
                else
                    return result;
            }
            for (int i = 0; i < SC.MaxIndex; i++)
            {
                if (Input.IndexOf(SC.SearchStart) > 0 && Input.IndexOf(SC.SearchEnd) > 0)
                {
                    tempString = Input.Substring(Input.IndexOf(SC.SearchStart) + SC.SearchStart.Length - 1);
                    tempString = tempString.Substring(1, tempString.IndexOf(SC.SearchEnd) - 1);
                    if (result.IndexOf(tempString + ", ") == -1)
                        result += tempString + ", ";
                    Input = Input.Substring(Input.IndexOf(tempString + SC.SearchEnd) + tempString.Length - 1);
                }
                else
                    break;
            }
            if (result.Length > 2)
                result = result.Substring(0, result.Length - 2);
            result = ReplaceNonAnsiChars(result);
            result = ReplaceXMLChars(result);
            result = result.Trim();
            return result;
        }

        string ReplaceNonAnsiChars(string Input)
        {
            string result = Input;
            NameValueCollection settingCollection = (NameValueCollection)ConfigurationManager.GetSection("CustomAppSettingsForChars");
            string[] allKeys = settingCollection.AllKeys;
            foreach (string key in allKeys)
                result = result.Replace(key, settingCollection[key]);
            return result;
        }

        string ReplaceXMLChars(string Input)
        {
            string result = Input;
            List<XMLReplace> XMLCharList = new List<XMLReplace>();
            XMLReplace node = new XMLReplace
            {
                Replaced = "&quot;",
                Replacee = "\""
            };
            XMLCharList.Add(node);
            foreach (XMLReplace key in XMLCharList)
                result = result.Replace(key.Replaced, key.Replacee);
            return result;
        }

        public List<string> ParsingHelperArray(SearchCriteria SC, SearchCriteria SCDetail, string Input)
        {
            List<string> result = new List<string>();
            if (!string.IsNullOrEmpty(SC.DivisionStart) && !string.IsNullOrEmpty(SC.DivisionEnd))
            {
                if (Input.IndexOf(SC.DivisionStart) > 0 && Input.IndexOf(SC.DivisionEnd) > 0)
                {
                    Input = Input.Substring(Input.IndexOf(SC.DivisionStart) - 1);
                    Input = Input.Substring(0, Input.IndexOf(SC.DivisionEnd));
                }
                else
                    return result;
            }
            for (int i = 0; i < SCDetail.MaxIndex; i++)
            {
                if (Input.IndexOf(SCDetail.SearchStart) > 0 && Input.IndexOf(SCDetail.SearchEnd) > 0)
                {
                    string tempString = Input.Substring(Input.IndexOf(SCDetail.SearchStart) + SCDetail.SearchStart.Length - 1);
                    if (tempString.IndexOf(SCDetail.SearchEnd) > -1)
                        tempString = tempString.Substring(1, tempString.IndexOf(SCDetail.SearchEnd) - 1);
                    else
                        break;
                    if (!string.IsNullOrEmpty(tempString) && !string.IsNullOrEmpty(tempString))
                        result.Add(ReplaceNonAnsiChars(ReplaceXMLChars(tempString)));
                    Input = Input.Substring(Input.IndexOf(tempString + SCDetail.SearchEnd) + tempString.Length - 1);
                }
                else
                    break;
            }
            return result;
        }

        public DateTime ConvertToDateTime(string input)
        {
            DateTime result;

            string month = input.Substring(input.IndexOf(" "), input.LastIndexOf(" ") - 1);
            string alteredMonth = "";

            switch(month)
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
                    dr["AdvertID"] = item.AdvertID;
                    dr["Description"] = item.Description;
                    dr["ThumbnailLink"] = item.ThumbnailLink;
                    dr["Location"] = item.Location;
                    dr["AdvertDate"] = item.AdvertDate;
                    dr["CreateDate"] = DateTime.Now;
                    dr["SearchMasterID"] = item.SearchMasterID;
                    dr["Price"] = item.Price;
                    DataTable.Rows.Add(dr);
                }
                return DataTable;
            }
        }
        #endregion
    }
}
