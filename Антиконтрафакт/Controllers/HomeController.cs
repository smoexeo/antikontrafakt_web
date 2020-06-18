using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace Антикотрафакт.Controllers
{
    public  class HomeController : Controller
    {
        private static HttpClient client = new HttpClient();

        private static int p = 0;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
        public ActionResult Authorization()
        {
            return View();
        }

        

        [HttpPost]
        public  ActionResult Authorization(string Email, string Password)
        {
            p++;
            using (var client = new WebClient())
            {
                var values = new NameValueCollection();
                //client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                values.Add("email", Email);
                values.Add("code", Password);
                var json = JsonConvert.SerializeObject(values);
                //string url = @"http://godnext-001-site1.btempurl.com/api/Login";
                string url = @"http://localhost:51675/api/Login";
                byte[] utf8Bytes = client.UploadValues(url, "POST", values);
                Encoding utf8 = Encoding.GetEncoding("UTF-8");
                Encoding win1251 = Encoding.GetEncoding("Windows-1251");
                byte[] win1251Bytes = Encoding.Convert(utf8, win1251, utf8Bytes);
                var responseString = win1251.GetString(win1251Bytes);
                ViewBag.Name = responseString;
            }


            return View();
        }
    }
}