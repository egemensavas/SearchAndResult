using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using static SahibindenWinForm.Models.GeneralModel;

namespace SahibindenWinForm.Classes
{
    class EventClass
    {
        #region Properties
        GeneralClass GeneralClass;
        HTMLCriteriaClass HTMLCriteriaClass;
        List<string> RequiredFolders;
        readonly public string AppStartUpPath = Application.StartupPath;
        readonly string connectionString = "mongodb+srv://sahibindendbadmin:6yHN_mongodb_7uJM@cluster0.hbpwq.azure.mongodb.net/test";
        MongoClient dbClient;
        IMongoDatabase db;
        IMongoCollection<BsonDocument> advertCollection;
        IMongoCollection<BsonDocument> searchCollection;
        #endregion

        #region Constructors
        public EventClass()
        {
            StartUpDefinitions();
        }

        void StartUpDefinitions()
        {
            GeneralClass = new GeneralClass();
            HTMLCriteriaClass = new HTMLCriteriaClass();
            RequiredFolders = GeneralClass.GetAppConfigStringArray("CustomAppSettingsForRequiredFolders");
            GeneralClass.CreateRequiredFolders(RequiredFolders, AppStartUpPath);
            dbClient = new MongoClient(connectionString);
            db = dbClient.GetDatabase("SahibindenMongoDatabase");
            advertCollection = db.GetCollection<BsonDocument>("AdvertCollection");
            searchCollection = db.GetCollection<BsonDocument>("SearchCollection");
        }
        #endregion

        #region Form Control Methods
        public void ButtonClickEvent(object sender, EventArgs e)
        {
            string siteContent = string.Empty;
            List<SearchModel> SearchList = GetSearchList();
            int searchMasterID;
            foreach (SearchModel item in SearchList)
            {
                searchMasterID = Convert.ToInt32(item.SearchID);
                List<ResultModel> AdvertIDList = GetAdvertIDList(searchMasterID);
                List<int> advertWebList = new List<int>();
                Enum.TryParse(item.AdvertType, out AdvertTypes advertType);
                bool contiuneOnNextPage = true;
                int currentPage = 1;
                string siteAddress;
                while (contiuneOnNextPage)
                {
                    List<int> advertWebList_ = new List<int>();
                    siteAddress = GenerateSiteAddress(item, currentPage);
                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.61 Safari/537.36");
                        siteContent = client.DownloadString(siteAddress);
                        System.Threading.Thread.Sleep(10000);
                    }
                    if (siteContent.Contains("too-many-requests"))
                        throw new Exception("Yakalandık :)");
                    string trimmedSiteContent = GeneralClass.TrimHelper(HTMLCriteriaClass.AdvertTrimCriteria, siteContent);
                    string cleanedSiteContent = WebUtility.HtmlDecode(GeneralClass.ReplaceNonAnsiChars(GeneralClass.CleanData(trimmedSiteContent)));
                    List<string> splittedInput = GeneralClass.SplitDivisionHelper(HTMLCriteriaClass.AdvertSplitDivisionCriteria, cleanedSiteContent, false);
                    List<ResultModel> ResultModelList = GeneralClass.PopulateResultModel(splittedInput, advertType, searchMasterID, AdvertIDList, out advertWebList_);
                    InsertIntoMongoDB(ResultModelList);
                    if (splittedInput.Count < 20)
                        contiuneOnNextPage = false;
                    currentPage++;
                    advertWebList.AddRange(advertWebList_);
                }
                GeneralClass.MarkAsDeleted(AdvertIDList, advertWebList, advertCollection);
            }
        }

        private string GenerateSiteAddress(SearchModel searchModel, int page)
        {
            string result_ = string.Empty;

            string pageOffsetString = string.Empty;
            string searchKeywords = searchModel.Detail.FirstOrDefault(n => (SearchableTypes)Enum.Parse(typeof(SearchableTypes), n.SearchableType) == SearchableTypes.Keywords) == null ? "" : searchModel.Detail.FirstOrDefault(n => (SearchableTypes)Enum.Parse(typeof(SearchableTypes), n.SearchableType) == SearchableTypes.Keywords).SearchableValue;
            string searchCategory = searchModel.Detail.FirstOrDefault(n => (SearchableTypes)Enum.Parse(typeof(SearchableTypes), n.SearchableType) == SearchableTypes.Category) == null ? "" : searchModel.Detail.FirstOrDefault(n => (SearchableTypes)Enum.Parse(typeof(SearchableTypes), n.SearchableType) == SearchableTypes.Category).SearchableValue;
            string searchLocation = searchModel.Detail.FirstOrDefault(n => (SearchableTypes)Enum.Parse(typeof(SearchableTypes), n.SearchableType) == SearchableTypes.Location) == null ? "" : searchModel.Detail.FirstOrDefault(n => (SearchableTypes)Enum.Parse(typeof(SearchableTypes), n.SearchableType) == SearchableTypes.Location).SearchableValue;
            string searchSort = searchModel.Detail.FirstOrDefault(n => (SearchableTypes)Enum.Parse(typeof(SearchableTypes), n.SearchableType) == SearchableTypes.Sort) == null ? "" : searchModel.Detail.FirstOrDefault(n => (SearchableTypes)Enum.Parse(typeof(SearchableTypes), n.SearchableType) == SearchableTypes.Sort).SearchableValue;
            string searchPriceMax = searchModel.Detail.FirstOrDefault(n => (SearchableTypes)Enum.Parse(typeof(SearchableTypes), n.SearchableType) == SearchableTypes.PriceMax) == null ? "" : searchModel.Detail.FirstOrDefault(n => (SearchableTypes)Enum.Parse(typeof(SearchableTypes), n.SearchableType) == SearchableTypes.PriceMax).SearchableValue;
            if (page > 1)
            {
                int pageOffset = (page - 1) * 20;
                pageOffsetString = "pagingOffset=" + pageOffset.ToString() + "&";
            }
            string temp = string.IsNullOrEmpty(searchCategory) ? "ikinci-el-ve-sifir-alisveris" : searchCategory;
            result_ = "https://www.sahibinden.com/" + temp + "/";
            temp = string.IsNullOrEmpty(searchLocation) ? "" : searchLocation;
            result_ += temp;
            if (!string.IsNullOrEmpty(pageOffsetString))
                result_ = result_.Substring(0, result_.IndexOf("?") + 1) + pageOffsetString + result_.Substring(result_.IndexOf("?") + 1);
            result_ += !result_.Contains("?") ? "?" : "";
            result_ += result_.LastIndexOf("?") == result_.Length - 1 || result_.LastIndexOf("&") == result_.Length - 1 ? "" : "&";
            if (!string.IsNullOrEmpty(searchSort))
                result_ += "sorting=" + searchSort;
            result_ += !result_.Contains("?") ? "?" : "";
            result_ += result_.LastIndexOf("?") == result_.Length - 1 || result_.LastIndexOf("&") == result_.Length - 1 ? "" : "&";
            if (!string.IsNullOrEmpty(searchKeywords))
                result_ += "query_text=" + searchKeywords;
            result_ += !result_.Contains("?") ? "?" : "";
            result_ += result_.LastIndexOf("?") == result_.Length - 1 || result_.LastIndexOf("&") == result_.Length - 1 ? "" : "&";
            if (!string.IsNullOrEmpty(searchPriceMax))
                result_ += "price_max=" + searchPriceMax;
            result_ = result_.LastIndexOf("?") == result_.Length - 1 || result_.LastIndexOf("&") == result_.Length - 1 ? result_.Substring(0, result_.Length - 2) : result_;
            result_ = result_.Replace("/?", "?");
            return result_;
        }

        private void InsertIntoMongoDB(List<ResultModel> ResultModelList)
        {
            List<BsonDocument> docs = new List<BsonDocument>();
            foreach (var item in ResultModelList)
            {
                BsonDocument doc = new BsonDocument
                {
                    {"AdvertDate", item.AdvertDate},
                    {"Price", item.Price},
                    {"SearchID", item.SearchMasterID},
                    {"CreateDate", DateTime.Now},
                    {"IsSeen", false},
                    {"IsDeleted", false},
                    {"AdvertID", item.AdvertID},
                    {"Description", item.Description},
                    {"ThumbnailLink", item.ThumbnailLink},
                    {"Location", item.Location},
                    {"Size", item.Size},
                    {"Room", item.Room},
                    {"Heating", item.Heating}
                };
                docs.Add(doc);
            }
            advertCollection.InsertMany(docs);
        }

        private List<SearchModel> GetSearchList()
        {
            List<BsonDocument> list_ = searchCollection.Find(new BsonDocument()).ToList();
            List<SearchModel> result_ = new List<SearchModel>();
            foreach (var item in list_)
            {
                SearchModel temp = BsonSerializer.Deserialize<SearchModel>(item);
                result_.Add(temp);
            }
            return result_;
        }

        private List<ResultModel> GetAdvertIDList(int SearchID)
        {
            List<ResultModel> result_ = new List<ResultModel>();
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("SearchID", SearchID) & builder.Eq("IsDeleted", false);
            var list_ = advertCollection.Find(filter).Project(Builders<BsonDocument>.Projection.Include("AdvertID")).ToList();
            foreach (var item in list_)
            {
                ResultModel temp = BsonSerializer.Deserialize<ResultModel>(item);
                result_.Add(temp);
            }
            return result_;
        }
        #endregion
    }
}
