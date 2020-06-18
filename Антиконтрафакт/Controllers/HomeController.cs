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

        //private static string url = @"http://godnext-001-site1.btempurl.com/api/";
        private static string url = @"http://localhost:51675/api/";
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

            string[] token = Response.Cookies.AllKeys;
            if(!String.IsNullOrEmpty(Response.Cookies["token"].Value))
            {
                var values = new NameValueCollection();
                values.Add("token", Response.Cookies["token"].Value);
                var result = RequestPost(url + "istrytoken", values);
                if (JsonConvert.DeserializeObject<bool>(result))
                {
                    return RedirectToAction("_ViewStart");
                }

            }
            return View();
        }

        //перевод кодировок из utf8 в win1251
        string Utf8ToWin1251(byte[] utf8Bytes)
        {
            Encoding utf8 = Encoding.GetEncoding("UTF-8");
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");
            byte[] win1251Bytes = Encoding.Convert(utf8, win1251, utf8Bytes);
            return win1251.GetString(win1251Bytes);
        }
      
        //отрправляет пост запрос с данными
        string RequestPost(string url, NameValueCollection values)
        {
            string result;
            using (var client = new WebClient())
            {
                var responseString = Utf8ToWin1251(client.UploadValues(url, "POST", values));
                result = responseString;
            }
            return result;
        }
        
        [HttpPost]
        public  ActionResult Authorization(string Email, string Password)
        {

           
            var values = new NameValueCollection();
            values.Add("email", Email);
            values.Add("code", Password);
            var res = JsonConvert.DeserializeObject<string>(RequestPost(url+"Login", values));
            if (res == "")
            {
                @ViewBag.Name = "Неверная почта или пароль.";
                return View();
            }
            Response.Cookies.Add(new HttpCookie("token",res));
            return RedirectToAction("_ViewStart");
        }
    }
}