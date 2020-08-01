using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using static Viewer_ASP.NET_Core.Models.GeneralModel;

namespace Viewer_ASP.NET_Core.Controllers
{
    public class APIController : Controller
    {
        [HttpGet]
        public IEnumerable<AdvertModel> FillDataToScreen([FromQuery] int SearchMasterID)
        {
            List<AdvertModel> result_ = new List<AdvertModel>();
            GeneralClass cls = new GeneralClass();
            IEnumerable<TABLE_ADVERT> Adverts = cls.GetAdvertData(SearchMasterID);
            foreach (var advert in Adverts)
            {
                AdvertModel x = new AdvertModel
                {
                    AdvertLink = "https://www.sahibinden.com/ilan/" + advert.AdvertID.ToString() + "/detay",
                    Description = advert.Description,
                    ThumbnailLink = advert.ThumbnailLink,
                    Location = advert.Location,
                    AdvertDate = String.Format("{0:dd/MM/yy}", advert.AdvertDate),
                    Price = String.Format("{0:n0}", advert.Price) + " TL",
                    Size = advert.Size.ToString(),
                    Room = advert.Room,
                    Heating = advert.Heating,
                    Price_sort = advert.Price,
                    SearchMasterID = advert.SearchMasterID,
                    Date_sort = advert.AdvertDate.Ticks
                };
                result_.Add(x);
            }
            return result_;
        }

        [HttpGet]
        public IEnumerable<SearchMasterModel> FillSearchComboData()
        {
            List<SearchMasterModel> result_ = new List<SearchMasterModel>();
            GeneralClass cls = new GeneralClass();
            IEnumerable<VIEW_SEARCHMASTER> SearchMasters = cls.GetSearchMasterData();
            foreach (var searchMaster in SearchMasters)
            {
                SearchMasterModel x = new SearchMasterModel
                {
                    ID = searchMaster.ID,
                    Description = searchMaster.Description,
                    Notes = searchMaster.Notes,
                    RecordCount = Convert.ToInt32(searchMaster.RecordCount)
                };
                result_.Add(x);
            }
            return result_;
        }

        [HttpGet]
        public void SendNotification([FromQuery] int SearchMasterID)
        {
            GeneralClass cls = new GeneralClass();
            string Message = cls.NotificationMessage(SearchMasterID);
            if (!string.IsNullOrEmpty(Message))
            {
                OneSignalCall(Message);
                cls.UpdateSeen(SearchMasterID);
            }
        }

        [HttpGet]
        public void OneSignalCall([FromQuery] string Message)
        {
            string URL = "https://onesignal.com/api/v1/notifications";
            string DATA = @"{
                                ""app_id"": ""708d286a-e547-4c5c-8575-5cc801c4096b"",
                                ""included_segments"": [""All""],
                                ""contents"": {""en"": """ + HttpUtility.UrlDecode(Message) + @"""}
                            }";

            var request = (HttpWebRequest)WebRequest.Create(URL);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Basic OThhYzIxM2YtMmZjNi00N2Y0LTg4NTMtZjYzNWYxOWE2MmY3");
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = DATA;

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)request.GetResponse();
            using var streamReader = new StreamReader(httpResponse.GetResponseStream());
            var result_ = streamReader.ReadToEnd();
        }

        [HttpGet]
        public IEnumerable<string> Scrapper()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            SQLClass SQLClass = new SQLClass();
            HelperClass HelperClass = new HelperClass();
            HTMLCriteriaClass HTMLCriteriaClass = new HTMLCriteriaClass();
            List<string> result = new List<string>();
            string siteContent = string.Empty;
            result.Add("Step 1");
            DataTable dtSearchMaster = SQLClass.GetDataTable("SELECT ID, ADVERTTYPEID FROM TABLE_SEARCH_MASTER (NOLOCK) WHERE ISACTIVE = 1", out string Error);
            result.Add("Step 2: " + Error);
            int searchMasterID;
            foreach (DataRow item in dtSearchMaster.Rows)
            {
                result.Add("Step 3: " + Error);
                searchMasterID = Convert.ToInt32(item["ID"]);
                DataTable dtAdvert = SQLClass.GetDataTable("SELECT AdvertID FROM TABLE_ADVERT (NOLOCK) WHERE SearchMasterID = " + searchMasterID, out Error);
                result.Add("Step 4: " + Error);
                List<int> advertDBList = HelperClass.DataTabletoIntList(dtAdvert);
                List<int> advertWebList = new List<int>();
                int advertTypeID = Convert.ToInt32(item["ADVERTTYPEID"]);
                bool contiuneOnNextPage = true;
                int currentPage = 1;
                string siteAddress;
                while (contiuneOnNextPage)
                {
                    List<int> advertWebList_ = new List<int>();
                    siteAddress = SQLClass.GetSingleCellDataComplex("SP_GETSEARCHURL " + searchMasterID.ToString() + ", " + currentPage.ToString());
                    result.Add(siteAddress);
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.61 Safari/537.36");
                        using HttpResponseMessage response = client.GetAsync(siteAddress).Result;
                        using HttpContent content = response.Content;
                        siteContent = content.ReadAsStringAsync().Result;
                    }
                    result.Add(siteContent);
                    if (siteContent.Contains("too-many-requests"))
                    {
                        result = new List<string>() { "We are banned :)" };
                        return result;
                    }
                    else if (siteContent.Contains("forceLoginPageMessage"))
                    {
                        AutomatedUILogin selenium = new AutomatedUILogin();
                        selenium.SahibindenLogin();
                        selenium.Dispose();
                    }
                    string trimmedSiteContent = HelperClass.TrimHelper(HTMLCriteriaClass.AdvertTrimCriteria, siteContent);
                    result.Add(trimmedSiteContent);
                    string cleanedSiteContent = WebUtility.HtmlDecode(HelperClass.ReplaceNonAnsiChars(HelperClass.CleanData(trimmedSiteContent)));
                    result.Add(cleanedSiteContent);
                    List<string> splittedInput = HelperClass.SplitDivisionHelper(HTMLCriteriaClass.AdvertSplitDivisionCriteria, cleanedSiteContent, false);
                    List<ResultModel> ResultModelList = HelperClass.PopulateResultModel(splittedInput, advertTypeID, searchMasterID, advertDBList, out advertWebList_);
                    using (DataTable dataTable = HelperClass.ConvertListToDataTable(ResultModelList))
                        SQLClass.BulkInsert(dataTable, "TABLE_ADVERT");
                    if (splittedInput.Count < 20)
                        contiuneOnNextPage = false;
                    currentPage++;
                    advertWebList.AddRange(advertWebList_);
                }
                if (advertWebList.Count > 0)
                    HelperClass.MarkAsDeleted(advertDBList, advertWebList);
                SendNotification(searchMasterID);
            }
            watch.Stop();
            result.Add("Done in " + (watch.ElapsedMilliseconds / 1000).ToString() + " seconds.");
            return result;
        }
    }
}
