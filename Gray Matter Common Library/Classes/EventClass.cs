using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
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
            bool test = true;
            string siteContent = string.Empty;
            int searchMasterID = 1;
            int latestAdvertID = Convert.ToInt32(SQLClass.GetSingleCellDataComplex("SELECT ISNULL(MAX(AdvertID),0) FROM TABLE_ADVERT (NOLOCK)"));
            int advertID;
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
                        siteContent = client.DownloadString(siteAddress);
                    }
                    File.WriteAllText(filePath, siteContent);
                }
                List<string> PriceList = GeneralClass.ParsingHelperArray(HTMLCriteriaClass.EmptyCriteria, HTMLCriteriaClass.PriceCriteria, siteContent);
                List<string> AdvertIDList = GeneralClass.ParsingHelperArray(HTMLCriteriaClass.EmptyCriteria, HTMLCriteriaClass.AdvertLinkCriteria, siteContent);
                List<string> DescriptionList = GeneralClass.ParsingHelperArray(HTMLCriteriaClass.EmptyCriteria, HTMLCriteriaClass.DescriptionCriteria, siteContent);
                List<string> ThumbnailList = GeneralClass.ParsingHelperArray(HTMLCriteriaClass.EmptyCriteria, HTMLCriteriaClass.ThumbnailCriteria, siteContent);
                List<string> LocationList = GeneralClass.ParsingHelperArray(HTMLCriteriaClass.EmptyCriteria, HTMLCriteriaClass.LocationCriteria, siteContent);
                List<string> DateList = GeneralClass.ParsingHelperArray(HTMLCriteriaClass.EmptyCriteria, HTMLCriteriaClass.DateCriteria, siteContent);

                int RecordCount = PriceList.Count;
                List<ResultModel> ResultModelList = new List<ResultModel>();

                for (int i = 0; i < RecordCount; i++)
                {
                    advertID = Convert.ToInt32(AdvertIDList[i].Substring(AdvertIDList[i].LastIndexOf("-") + 1));
                    if (advertID > latestAdvertID)
                    {
                        ResultModel Addition = new ResultModel
                        {
                            AdvertDate = GeneralClass.ConvertToDateTime(DateList[i].Replace("</span>\n                        <br/>\n                        <span>", " ")),
                            Description = DescriptionList[i],
                            AdvertID = advertID,
                            Location = LocationList[i].Replace("<br/>", " - "),
                            Price = Convert.ToInt32(PriceList[i].Replace(".", "").Replace(" TL", "")),
                            SearchMasterID = searchMasterID,
                            ThumbnailLink = ThumbnailList[i]
                        };
                        ResultModelList.Add(Addition);
                    }
                }
                using (DataTable dataTable = GeneralClass.ConvertListToDataTable(ResultModelList))
                {
                    SQLClass.BulkInsert(dataTable, "TABLE_ADVERT");
                }
                if (!test)
                    File.Delete(filePath);
                if (ResultModelList.Count < 20)
                    contiuneOnNextPage = false;
                currentPage++;
            }
        }
        #endregion
    }
}
