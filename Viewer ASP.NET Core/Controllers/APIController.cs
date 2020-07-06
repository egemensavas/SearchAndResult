using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Viewer_ASP.NET_Core.Controllers
{
    public class APIController : Controller
    {
        [HttpGet]
        public IEnumerable<AdvertModel> Index()
        {
            List<AdvertModel> result_ = new List<AdvertModel>();
            AdvertModel a = new AdvertModel
            {
                AdvertLink = "https://www.sahibinden.com/ilan/837317934/detay",
                Description = "Canon eos 6d mark 2",
                ThumbnailLink = "https://i0.shbdn.com/photos/00/18/86/lthmb_825001886frl.jpg",
                Location = "Mersin - Akdeniz",
                AdvertDate = "10.06.2018",
                Price = "180 TL"
            };
            result_.Add(a);
            AdvertModel b = new AdvertModel
            {
                AdvertLink = "https://www.sahibinden.com/ilan/821136976/detay",
                Description = "Canon EOS 6D Mark II Body | sıfır kutusunda 2 garantili",
                ThumbnailLink = "https://i0.shbdn.com/photos/13/69/76/lthmb_8211369764g8.jpg",
                Location = "İstanbul - Fatih",
                AdvertDate = "10.06.2018",
                Price = "180 TL"
            };
            result_.Add(b);
            AdvertModel c = new AdvertModel
            {
                AdvertLink = "https://www.sahibinden.com/ilan/821136976/detay",
                Description = "Canon EOS 6D Mark II Body | sıfır kutusunda 2 garantili",
                ThumbnailLink = "https://i0.shbdn.com/photos/13/69/76/lthmb_8211369764g8.jpg",
                Location = "İstanbul - Fatih",
                AdvertDate = "10.06.2018",
                Price = "180 TL"
            };
            result_.Add(c);
            return result_;
        }
    }
}
