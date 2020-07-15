using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Windows.Forms;
using static SahibindenWinForm.Models.GeneralModel;

namespace SahibindenWinForm.Classes
{
    class EventClass
    {
        #region Properties
        readonly GeneralClass GeneralClass;
        readonly HTMLCriteriaClass HTMLCriteriaClass;
        readonly SQLClass SQLClass;
        List<string> RequiredFolders;
        readonly public string AppStartUpPath = Application.StartupPath;
        #endregion

        #region Constructors
        public EventClass()
        {
            GeneralClass = new GeneralClass();
            HTMLCriteriaClass = new HTMLCriteriaClass();
            SQLClass = new SQLClass();
            StartUpDefinitions();
        }

        void StartUpDefinitions()
        {
            RequiredFolders = GeneralClass.GetAppConfigStringArray("CustomAppSettingsForRequiredFolders");
            GeneralClass.CreateRequiredFolders(RequiredFolders, AppStartUpPath);
        }
        #endregion

        #region Form Control Methods
        public void ButtonClickEvent(object sender, EventArgs e)
        {
            bool test = false;
            string siteContent = string.Empty;
            DataTable dtSearchMaster = SQLClass.GetDataTable("SELECT ID, ADVERTTYPEID FROM TABLE_SEARCH_MASTER (NOLOCK)");
            int searchMasterID;
            foreach (DataRow item in dtSearchMaster.Rows)
            {
                searchMasterID = Convert.ToInt32(item["ID"]);
                int advertTypeID = Convert.ToInt32(item["ADVERTTYPEID"]);
                bool contiuneOnNextPage = true;
                string filePath = AppStartUpPath + "\\HTML\\tobedeleted.html";
                int currentPage = 1;
                string siteAddress;
                while (contiuneOnNextPage)
                {
                    siteAddress = SQLClass.GetSingleCellDataComplex("SP_GETSEARCHURL " + searchMasterID.ToString() + ", " + currentPage.ToString());
                    if (File.Exists(filePath))
                        siteContent = File.ReadAllText(filePath);
                    else
                    {
                        using (WebClient client = new WebClient())
                        {
                            client.Headers.Add("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.61 Safari/537.36");
                            siteContent = client.DownloadString(siteAddress);
                        }
                        File.WriteAllText(filePath, siteContent);
                    }
                    if (siteContent.Contains("too-many-requests"))
                        throw new Exception("Yakalandık :)");
                    string trimmedSiteContent = GeneralClass.TrimHelper(HTMLCriteriaClass.AdvertTrimCriteria, siteContent);
                    string cleanedSiteContent = WebUtility.HtmlDecode(GeneralClass.ReplaceNonAnsiChars(GeneralClass.CleanData(trimmedSiteContent)));
                    List<string> splittedInput = GeneralClass.SplitDivisionHelper(HTMLCriteriaClass.AdvertSplitDivisionCriteria, cleanedSiteContent, false);
                    List<ResultModel> ResultModelList = GeneralClass.PopulateResultModel(splittedInput, advertTypeID, searchMasterID);
                    using (DataTable dataTable = GeneralClass.ConvertListToDataTable(ResultModelList))
                        SQLClass.BulkInsert(dataTable, "TABLE_ADVERT");
                    if (!test)
                        File.Delete(filePath);
                    if (splittedInput.Count < 20)
                        contiuneOnNextPage = false;
                    currentPage++;
                }
            }
        }
        #endregion
    }
}
