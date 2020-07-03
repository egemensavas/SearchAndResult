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
        public IEnumerable<string> Index()
        {
            List<string> a = new List<string>();
            a.Add("test");
            a.Add("test2");
            return a;
        }
    }
}
