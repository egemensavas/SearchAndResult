using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Web;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Viewer_ASP.NET_Core.Controllers
{
    public class APIController : Controller
    {
        [HttpGet]
        public IEnumerable<AdvertModel> FillDataToScreen([FromQuery] int SearchMasterID)
        {
            List<AdvertModel> result_ = new List<AdvertModel>();
            GeneralClass cls = new GeneralClass();
            var a = cls.GetAdvertData(SearchMasterID);
            foreach (var b in a)
            {
                AdvertModel x = new AdvertModel
                {
                    AdvertLink = "https://www.sahibinden.com/ilan/" + b.AdvertID.ToString() + "/detay",
                    Description = b.Description,
                    ThumbnailLink = b.ThumbnailLink,
                    Location = b.Location,
                    AdvertDate = b.AdvertDate.ToString(),
                    Price = b.Price.ToString() + " TL"
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

        public void OneSignalCall(string Message)
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
    }
}
